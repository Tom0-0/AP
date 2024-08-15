using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject.DevDesc;
using _3S.CoDeSys.DeviceObject.ExportDevice;
using _3S.CoDeSys.FdtIntegration;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.PLCopenXML;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class PLCopenDeviceImporter : IPLCopenDeviceImporter
	{
		internal IPLCopenXMLConfigImportObject _importObject;

		internal int index;

		internal XmlNode dataNode;

		internal Guid guid = Guid.Empty;

		internal bool isExplicitConnector;

		internal int nExplicitConnectorID = -1;

		private Device _device;

		private DeviceDescription devdesc;

		private IPLCopenXMLImportReporter reporter;

		private IDeviceCatalogue _deviceCatalogue;

		public IDeviceCatalogue DeviceCatalogue
		{
			get
			{
				if (_deviceCatalogue == null)
				{
					_deviceCatalogue = APEnvironment.CreateFirstDeviceCatalogue();
				}
				return _deviceCatalogue;
			}
		}

		internal void Init(IPLCopenXMLConfigImportObject importObject, int index, XmlNode dataNode)
		{
			_importObject = importObject;
			this.index = index;
			this.dataNode = dataNode;
			if (dataNode != null && !(dataNode.Name == "Device") && dataNode.Name == "ExplicitConnector")
			{
				isExplicitConnector = true;
				if (dataNode.Attributes["connectorID"] != null)
				{
					int.TryParse(dataNode.Attributes["connectorID"].Value, out nExplicitConnectorID);
				}
			}
		}

		private Device GetPLCopenDevice()
		{
			if (_device == null && dataNode != null && !isExplicitConnector)
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Device));
				XmlNodeReader xmlReader = new XmlNodeReader(dataNode);
				object obj = xmlSerializer.Deserialize(xmlReader);
				_device = obj as Device;
			}
			return _device;
		}

		public Guid Import(int projectHandle, Guid parentGuid, IPLCopenXMLImportReporter reporter)
		{
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Expected O, but got Unknown
			this.reporter = reporter;
			if (guid != Guid.Empty)
			{
				return guid;
			}
			if (!isExplicitConnector)
			{
				Device pLCopenDevice = GetPLCopenDevice();
				if (pLCopenDevice != null)
				{
					IDeviceIdentification devIdent = PLCopenImport.CreateDeviceIdentification(pLCopenDevice.DeviceType.Item);
					try
					{
						APEnvironment.DeviceMgr.IsPLCOpenXMLImport = true;
						string stBaseName = DeviceObjectHelper.BuildIecIdentifier(((IPLCopenXMLImportObject)_importObject).Name);
						string text = DeviceObjectHelper.CreateUniqueIdentifier(projectHandle, stBaseName, DeviceObjectHelper.GetHostStub(projectHandle, parentGuid));
						devdesc = ExportDevDesc.LoadDevdesc(ref devIdent, reporter);
						DeviceObject deviceObject = new DeviceBuilder().CreateDeviceObject(createBitChannels: DeviceObjectHelper.CreateBitChannels(projectHandle, parentGuid), device: pLCopenDevice, devIdent: devIdent);
						if ((((IPLCopenXMLImportObject)_importObject).Parent == null || !(((IPLCopenXMLImportObject)_importObject).Parent is IPLCopenXMLConfigImportObject)) && parentGuid != Guid.Empty)
						{
							IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, parentGuid).Object;
							if (@object is SlotDeviceObject)
							{
								SlotDeviceObject slotDeviceObject = @object as SlotDeviceObject;
								if (!slotDeviceObject.HasDevice || Match((IObject)(object)slotDeviceObject))
								{
									PlugDeviceIntoSlot(this, projectHandle, parentGuid, deviceObject, text);
									return parentGuid;
								}
							}
						}
						guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(projectHandle, parentGuid, Guid.NewGuid(), (IObject)(object)deviceObject, text, -1);
						ObjectCreatedStatic(this, projectHandle, guid);
						if (pLCopenDevice.DeviceType.ExcludeFromBuild)
						{
							IMetaObject val = null;
							try
							{
								IBuildProperty val2 = APEnvironment.CreateBuildProperty();
								if (val2 is IBuildProperty3)
								{
									((IBuildProperty3)val2).ExcludeFromBuildLocal=(pLCopenDevice.DeviceType.ExcludeFromBuild);
								}
								else
								{
									val2.ExcludeFromBuild=(pLCopenDevice.DeviceType.ExcludeFromBuild);
								}
								val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(projectHandle, guid);
								if (val != null)
								{
									val.AddProperty((IObjectProperty)(object)val2);
								}
							}
							finally
							{
								if (val != null && val.IsToModify)
								{
									((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
								}
							}
						}
						return guid;
					}
					catch (Exception)
					{
						throw;
					}
					finally
					{
						APEnvironment.DeviceMgr.IsPLCOpenXMLImport = false;
					}
				}
			}
			else
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, parentGuid);
				IDeviceObject val3 = null;
				if (objectToRead.Object is IDeviceObject)
				{
					IObject object2 = objectToRead.Object;
					val3 = (IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null);
				}
				if (objectToRead.Object is ISlotDeviceObject)
				{
					IObject object3 = objectToRead.Object;
					val3 = ((ISlotDeviceObject)((object3 is ISlotDeviceObject) ? object3 : null)).GetDeviceObject();
				}
				if (val3 != null)
				{
					foreach (IConnector item in (IEnumerable)val3.Connectors)
					{
						IConnector val4 = item;
						if (val4.ConnectorId == nExplicitConnectorID && val4.IsExplicit)
						{
							return (val4 as Connector).ExplicitConnectorGuid;
						}
					}
				}
			}
			return Guid.Empty;
		}

		private DeviceObject GetFixedDeviceObject(PLCopenDeviceImporter importer, IDeviceIdentification id, int nModuleOffset, bool createBitChannels, out PLCopenDeviceImporter fixedImporter)
		{
			IPLCopenXMLConfigImportObject importObject = importer._importObject;
			if (importObject != null)
			{
				IPLCopenXMLImportObject[] children = ((IPLCopenXMLImportObject)importObject).Children;
				foreach (IPLCopenXMLImportObject obj in children)
				{
					IPLCopenXMLConfigImportObject val = (IPLCopenXMLConfigImportObject)(object)((obj is IPLCopenXMLConfigImportObject) ? obj : null);
					if (val == null || !val.MarkedForImport)
					{
						continue;
					}
					PLCopenDeviceImporter pLCopenDeviceImporter = val.PLCopenDeviceImporter as PLCopenDeviceImporter;
					if (pLCopenDeviceImporter != null && pLCopenDeviceImporter.guid == Guid.Empty)
					{
						Device pLCopenDevice = pLCopenDeviceImporter.GetPLCopenDevice();
						if (pLCopenDevice != null && pLCopenDeviceImporter.index >= nModuleOffset && CheckEqual(CreateIDeviceIdentification(pLCopenDevice.DeviceType.Item), id))
						{
							DeviceBuilder deviceBuilder = new DeviceBuilder();
							DeviceDescription deviceDescription = null;
							deviceDescription = ((devdesc == null || !CheckEqual(devdesc.Device.DeviceIdentification, id)) ? ExportDevDesc.LoadDevdesc(ref id, reporter) : devdesc);
							DeviceObject result = deviceBuilder.CreateDeviceObject(pLCopenDevice, id, deviceDescription, createBitChannels);
							fixedImporter = pLCopenDeviceImporter;
							fixedImporter.devdesc = deviceDescription;
							return result;
						}
					}
				}
			}
			try
			{
				DeviceObject result2 = new DeviceObject(createBitChannels, id);
				fixedImporter = null;
				return result2;
			}
			catch (DeviceNotFoundException)
			{
				throw;
			}
		}

		private bool CheckEqual(_3S.CoDeSys.DeviceObject.DevDesc.DeviceIdentificationType xmlId, IDeviceIdentification id)
		{
			if (xmlId.Id == id.Id && xmlId.Type == id.Type)
			{
				return xmlId.Version == id.Version;
			}
			return false;
		}

		private bool CheckEqual(IDeviceIdentification id, IDeviceIdentification id2)
		{
			if (id.Id == id2.Id && id.Type == id2.Type && id.Version == id2.Version)
			{
				IModuleIdentification val = (IModuleIdentification)(object)((id is IModuleIdentification) ? id : null);
				IModuleIdentification val2 = (IModuleIdentification)(object)((id2 is IModuleIdentification) ? id2 : null);
				if (val == null && val2 == null)
				{
					return true;
				}
				if (val != null && val2 != null)
				{
					return val.ModuleId == val2.ModuleId;
				}
				return false;
			}
			return false;
		}

		private DeviceObject GetSlotDeviceObject(PLCopenDeviceImporter importer, DeviceIdentification defaultId, int nModuleOffset, bool createBitChannels, out PLCopenDeviceImporter slotImporter)
		{
			IPLCopenXMLConfigImportObject importObject = importer._importObject;
			if (importObject != null && nModuleOffset < ((IPLCopenXMLImportObject)importObject).Children.Length)
			{
				IPLCopenXMLImportObject obj = ((IPLCopenXMLImportObject)importObject).Children[nModuleOffset];
				IPLCopenXMLConfigImportObject val = (IPLCopenXMLConfigImportObject)(object)((obj is IPLCopenXMLConfigImportObject) ? obj : null);
				if (val != null && val.MarkedForImport)
				{
					PLCopenDeviceImporter pLCopenDeviceImporter = val.PLCopenDeviceImporter as PLCopenDeviceImporter;
					if (pLCopenDeviceImporter != null)
					{
						if (pLCopenDeviceImporter.dataNode == null)
						{
							slotImporter = pLCopenDeviceImporter;
							return null;
						}
						Device pLCopenDevice = pLCopenDeviceImporter.GetPLCopenDevice();
						if (pLCopenDevice != null)
						{
							DeviceBuilder deviceBuilder = new DeviceBuilder();
							_ = devdesc.Device.DeviceIdentification;
							IDeviceIdentification devIdent = CreateIDeviceIdentification(pLCopenDevice.DeviceType.Item);
							DeviceDescription deviceDescription = null;
							deviceDescription = ((devdesc == null || !CheckEqual(devdesc.Device.DeviceIdentification, devIdent)) ? ExportDevDesc.LoadDevdesc(ref devIdent, reporter) : devdesc);
							DeviceObject result = deviceBuilder.CreateDeviceObject(pLCopenDevice, devIdent, deviceDescription, createBitChannels);
							slotImporter = pLCopenDeviceImporter;
							slotImporter.devdesc = deviceDescription;
							return result;
						}
					}
				}
			}
			if (defaultId != null)
			{
				try
				{
					DeviceObject result2 = new DeviceObject(createBitChannels, (IDeviceIdentification)(object)defaultId);
					slotImporter = null;
					return result2;
				}
				catch (DeviceNotFoundException)
				{
					throw;
				}
			}
			slotImporter = null;
			return null;
		}

		private static IDeviceIdentification CreateIDeviceIdentification(DeviceIdentificationType deviceId)
		{
			IDeviceIdentification val = null;
			if (deviceId is ModuleIdentificationType)
			{
				ModuleIdentificationType moduleIdentificationType = deviceId as ModuleIdentificationType;
				return (IDeviceIdentification)(object)((IDeviceRepository)APEnvironment.DeviceRepository).CreateModuleIdentification(moduleIdentificationType.Type, moduleIdentificationType.Id, moduleIdentificationType.Version, moduleIdentificationType.ModuleId);
			}
			return ((IDeviceRepository)APEnvironment.DeviceRepository).CreateDeviceIdentification(deviceId.Type, deviceId.Id, deviceId.Version);
		}

		public void ObjectCreatedStatic(PLCopenDeviceImporter importer, int nProjectHandle, Guid guidObject)
		{
			IDeviceObjectBase deviceObjectBase = null;
			IMetaObject val = null;
			try
			{
				DeviceObjectHelper.BeginCreateDevice(guidObject);
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidObject);
				if (val != null)
				{
					deviceObjectBase = val.Object as IDeviceObjectBase;
				}
				if (deviceObjectBase != null)
				{
					UpdateChildObjects(importer, deviceObjectBase);
					reporter.ReportAdded(nProjectHandle, guidObject);
				}
			}
			catch
			{
			}
			finally
			{
				DeviceObjectHelper.EndCreateDevice(guidObject);
			}
		}

		internal void UpdateChildObjects(PLCopenDeviceImporter importer, IDeviceObjectBase device)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			bool bUpdate = false;
			if (((IObject)device).MetaObject.IsToModify)
			{
				throw new InvalidOperationException("This operation may not be executed on a modifyable copy of a device object!");
			}
			int projectHandle = ((IObject)device).MetaObject.ProjectHandle;
			Guid objectGuid = ((IObject)device).MetaObject.ObjectGuid;
			int nModuleOffset;
			if (device is DeviceObject)
			{
				DeviceObject.CreateTasks((IIoProvider)(object)(DeviceObject)device);
				nModuleOffset = ((DeviceObject)device).GetNumberOfFunctionalChildren();
			}
			else if (device is SlotDeviceObject)
			{
				SlotDeviceObject slotDeviceObject = (SlotDeviceObject)device;
				if (slotDeviceObject.HasDevice)
				{
					DeviceObject.CreateTasks((IIoProvider)(object)slotDeviceObject.GetDevice());
					nModuleOffset = slotDeviceObject.GetDevice().GetNumberOfFunctionalChildren();
				}
				else
				{
					nModuleOffset = 0;
				}
			}
			else
			{
				nModuleOffset = 0;
			}
			foreach (Connector item in (IEnumerable)((IDeviceObject)device).Connectors)
			{
				if ((int)item.ConnectorRole == 0)
				{
					bool flag = false;
					IIoProvider ioProviderParent = item.GetIoProviderParent();
					if (ioProviderParent is Connector)
					{
						foreach (IAdapter item2 in (IEnumerable)(ioProviderParent as Connector).Adapters)
						{
							IAdapter val = item2;
							if (val is SlotAdapter && val.ModulesCount == 1 && val.Modules[0] == Guid.Empty)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						continue;
					}
				}
				DeviceObject.CreateTasks((IIoProvider)(object)item);
				if ((int)item.ConnectorRole != 0)
				{
					continue;
				}
				IIoProvider val2 = (IIoProvider)(object)item;
				if (val2 != null)
				{
					IIoProvider[] children = val2.Children;
					for (int i = 0; i < children.Length; i++)
					{
						IMetaObject metaObject = children[i].GetMetaObject();
						if (metaObject != null)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObject.ProjectHandle, metaObject.ObjectGuid);
							item.createHostChildParameters(metaObjectStub);
						}
					}
				}
				try
				{
					if (item.IsExplicit && item != null)
					{
						PLCopenDeviceImporter pLCopenDeviceImporter = null;
						if (nModuleOffset < ((IPLCopenXMLImportObject)importer._importObject).Children.Length)
						{
							for (int j = 0; j < ((IPLCopenXMLImportObject)importer._importObject).Children.Length; j++)
							{
								IPLCopenXMLImportObject obj = ((IPLCopenXMLImportObject)importer._importObject).Children[j];
								IPLCopenXMLConfigImportObject val3 = (IPLCopenXMLConfigImportObject)(object)((obj is IPLCopenXMLConfigImportObject) ? obj : null);
								if (val3 != null && val3.MarkedForImport)
								{
									PLCopenDeviceImporter pLCopenDeviceImporter2 = val3.PLCopenDeviceImporter as PLCopenDeviceImporter;
									if (pLCopenDeviceImporter2 != null && pLCopenDeviceImporter2.isExplicitConnector && item.ConnectorId == pLCopenDeviceImporter2.nExplicitConnectorID)
									{
										pLCopenDeviceImporter = pLCopenDeviceImporter2;
										pLCopenDeviceImporter.devdesc = importer.devdesc;
										UpdateChildObjectsExplicit(pLCopenDeviceImporter, item, projectHandle, objectGuid, ref nModuleOffset, ((IDeviceObject11)device).CreateBitChannels);
										break;
									}
								}
							}
						}
						if (pLCopenDeviceImporter == null)
						{
							item.UpdateChildObjects(projectHandle, objectGuid, ref nModuleOffset, bUpdate: false, ((IDeviceObject11)device).CreateBitChannels, bVersionUpgrade: false);
						}
					}
					else if (item != null)
					{
						UpdateChildObjects(importer, item, projectHandle, objectGuid, ref nModuleOffset, ((IDeviceObject11)device).CreateBitChannels);
					}
				}
				catch (Exception ex)
				{
					APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
				}
			}
			DeviceObject.CreateImplicitObjects(projectHandle, objectGuid, out var bHasPlcLogic, bUpdate);
			if (bHasPlcLogic)
			{
				IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(projectHandle);
				OnPlcCreatedAction onPlcCreatedAction = new OnPlcCreatedAction(projectHandle, objectGuid);
				undoManager.AddAction((IUndoableAction)(object)onPlcCreatedAction);
				onPlcCreatedAction.Redo();
			}
		}

		public void UpdateChildObjects(PLCopenDeviceImporter importer, ConnectorBase con, int nProjectHandle, Guid guidDevice, ref int nModuleOffset, bool bCreateBitChannels)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			foreach (IAdapter item in (IEnumerable)con.Adapters)
			{
				IAdapter val = item;
				if (val is FixedAdapter)
				{
					FixedAdapter fixedAdapter = (FixedAdapter)(object)val;
					for (int i = 0; i < fixedAdapter.DefaultModules.Length; i++)
					{
						nModuleOffset++;
						DeviceIdentification deviceIdentification = fixedAdapter.DefaultModules[i];
						PLCopenDeviceImporter fixedImporter = null;
						DeviceObject fixedDeviceObject = GetFixedDeviceObject(importer, (IDeviceIdentification)(object)deviceIdentification, nModuleOffset - 1, bCreateBitChannels, out fixedImporter);
						fixedDeviceObject.ConnectorIDForChild = con.ConnectorId;
						string stBase = string.Empty;
						if (fixedImporter != null)
						{
							stBase = ((IPLCopenXMLImportObject)fixedImporter._importObject).Name;
						}
						else if (fixedDeviceObject != null && fixedDeviceObject.DeviceInfo != null)
						{
							stBase = DeviceObjectHelper.GetBaseName(deviceIdentification.BaseName, DeviceObjectHelper.CreateInstanceNameBase(fixedDeviceObject.DeviceInfo));
						}
						stBase = DeviceObjectHelper.BuildIecIdentifier(stBase);
						string text = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, stBase, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
						Guid guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)fixedDeviceObject, text, nModuleOffset - 1);
						if (fixedImporter != null)
						{
							fixedImporter.guid = guid;
							ObjectCreatedStatic(fixedImporter, nProjectHandle, guid);
						}
						else
						{
							DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guid);
						}
						if (fixedAdapter.SubDevicesCollapsed)
						{
							INavigatorControl3 val2 = (INavigatorControl3)((((IEngine)APEnvironment.Engine).Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null);
							if (val2 != null)
							{
								val2.Collapse(guid);
							}
						}
					}
				}
				else if (val is SlotAdapter)
				{
					SlotAdapter slotAdapter = (SlotAdapter)(object)val;
					for (int j = 0; j < slotAdapter.ModulesCount; j++)
					{
						nModuleOffset++;
						SlotDeviceObject slotDeviceObject = ((!slotAdapter.HiddenSlot) ? new SlotDeviceObject(con.Interface, con.AdditionalInterfaces, slotAdapter.AllowEmptySlot) : new HiddenSlotDeviceObject(con.Interface, con.AdditionalInterfaces, slotAdapter.AllowEmptySlot));
						DeviceIdentification deviceIdentification2 = (DeviceIdentification)(object)slotAdapter.GetDefaultDevice(j);
						if (slotAdapter.ModuleGuidsPlain[j] != Guid.Empty)
						{
							continue;
						}
						PLCopenDeviceImporter slotImporter = null;
						DeviceObject slotDeviceObject2 = GetSlotDeviceObject(importer, deviceIdentification2, nModuleOffset - 1, bCreateBitChannels, out slotImporter);
						string text2;
						if (slotDeviceObject2 != null)
						{
							string empty = string.Empty;
							empty = ((slotImporter == null) ? DeviceObjectHelper.GetBaseName(deviceIdentification2.BaseName, DeviceObjectHelper.CreateInstanceNameBase(slotDeviceObject2.DeviceInfo)) : ((IPLCopenXMLImportObject)slotImporter._importObject).Name);
							empty = DeviceObjectHelper.BuildIecIdentifier(empty);
							text2 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, empty, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
							slotDeviceObject.PlugDevice(slotDeviceObject2);
						}
						else
						{
							string slotName = slotAdapter.GetSlotName(j);
							if (!slotName.StartsWith("<"))
							{
								slotName = DeviceObjectHelper.BuildIecIdentifier(slotName);
								text2 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, slotName, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
							}
							else
							{
								text2 = slotName;
							}
						}
						slotDeviceObject.ConnectorIDForChild = con.ConnectorId;
						Guid guid2 = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)slotDeviceObject, text2, nModuleOffset - 1);
						if (slotDeviceObject2 != null)
						{
							if (slotImporter != null)
							{
								slotImporter.guid = guid2;
								ObjectCreatedStatic(slotImporter, nProjectHandle, guid2);
							}
							else
							{
								DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guid2);
							}
						}
						else if (slotImporter != null)
						{
							slotImporter.guid = guid2;
						}
						if (slotAdapter.SubDevicesCollapsed)
						{
							INavigatorControl3 val3 = (INavigatorControl3)((((IEngine)APEnvironment.Engine).Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null);
							if (val3 != null)
							{
								val3.Collapse(guid2);
							}
						}
					}
				}
				else
				{
					nModuleOffset += val.ModulesCount;
				}
			}
		}

		public void UpdateChildObjectsExplicit(PLCopenDeviceImporter importer, Connector con, int nProjectHandle, Guid guidObject, ref int nModuleOffset, bool bCreateBitChannels)
		{
			if (!con.IsExplicit)
			{
				return;
			}
			Guid guid = Guid.Empty;
			ExplicitConnector explicitConnector = new ExplicitConnector((IConnector)(object)con);
			try
			{
				guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidObject, Guid.NewGuid(), (IObject)(object)explicitConnector, explicitConnector.VisibleInterfaceName, -1);
				nModuleOffset++;
				importer.guid = guid;
			}
			catch
			{
				APEnvironment.MessageService.Error(Strings.ErrorUpdateDevice, "ErrorUpdateDevice", Array.Empty<object>());
			}
			IMetaObject val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
			explicitConnector = (ExplicitConnector)(object)val.Object;
			int nModuleOffset2 = 0;
			UpdateChildObjects(importer, explicitConnector, nProjectHandle, guid, ref nModuleOffset2, bCreateBitChannels);
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val);
				(con.GetDeviceObject() as DeviceObject).UpdateIoProvider((IIoProvider)(object)(ExplicitConnector)(object)val.Object);
				((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
			}
			catch
			{
				if (val != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
				}
			}
		}

		internal void PlugDeviceIntoSlot(PLCopenDeviceImporter importer, int nProjectHandle, Guid guidSlot, DeviceObject device, string stName)
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(nProjectHandle);
			bool flag = false;
			try
			{
				undoManager.BeginCompoundAction("Plug device");
				try
				{
					APEnvironment.DeviceMgr.RaiseDevicePlugging(nProjectHandle, guidSlot, device.DeviceIdentificationNoSimulation);
				}
				catch (Exception ex)
				{
					APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
					return;
				}
				((SlotDeviceObject)(object)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidSlot).Object).PlugDevice(device, bCheckForConstraints: true);
				DeviceCommandHelper.UnplugDeviceFromSlot(nProjectHandle, guidSlot, bCheckBeforeUnplug: false);
				PlugSlotAction plugSlotAction = new PlugSlotAction(nProjectHandle, guidSlot, device, stName);
				plugSlotAction.Redo();
				undoManager.AddAction((IUndoableAction)(object)plugSlotAction);
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidSlot);
				SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)objectToRead.Object;
				UpdateChildObjects(importer, slotDeviceObject.GetDevice());
				DeviceObjectHelper.AutoInsertLogicalDevice((ObjectEventArgs)new ObjectAddedEventArgs(objectToRead.ProjectHandle, objectToRead.ObjectGuid, 0, (IPastedObject)null));
				APEnvironment.DeviceMgr.RaiseDevicePlugged(nProjectHandle, guidSlot, device.DeviceIdentificationNoSimulation);
				IFdtIntegration2 fdtIntegrationOrNull = APEnvironment.FdtIntegrationOrNull;
				if (fdtIntegrationOrNull != null)
				{
					fdtIntegrationOrNull.NotifySlotPlugged(nProjectHandle, guidSlot);
				}
				flag = true;
			}
			catch
			{
				flag = false;
				throw;
			}
			finally
			{
				undoManager.EndCompoundAction();
				try
				{
					if (!flag)
					{
						undoManager.Undo();
					}
				}
				catch
				{
				}
			}
		}

		public bool AcceptsParentObject(IObject obj)
		{
			return Match(obj);
		}

		public bool Match(IObject parentObject)
		{
			if (parentObject is SlotDeviceObject)
			{
				string[] possibleInterfacesForPlug = (parentObject as SlotDeviceObject).GetPossibleInterfacesForPlug();
				return Match(possibleInterfacesForPlug);
			}
			if (parentObject is IDeviceObject)
			{
				IDeviceObject parentDevice = (IDeviceObject)(object)((parentObject is IDeviceObject) ? parentObject : null);
				return MatchDevice(parentDevice);
			}
			if (parentObject is IConnector)
			{
				return MatchConnector((IConnector)(object)((parentObject is IConnector) ? parentObject : null));
			}
			ISVNode[] selectedNodes = GetSelectedNodes();
			if (selectedNodes != null && selectedNodes.Length == 0 && !isExplicitConnector)
			{
				Device pLCopenDevice = GetPLCopenDevice();
				if (pLCopenDevice != null)
				{
					IDeviceIdentification val = PLCopenImport.CreateDeviceIdentification(pLCopenDevice.DeviceType.Item);
					IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val);
					if (device != null)
					{
						return device.AllowTopLevel;
					}
					if (parentObject == null)
					{
						DeviceNotFoundException ex = new DeviceNotFoundException(val);
						DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.ErrorImportDeviceNotPossible, ex.Message), (Severity)2, Guid.Empty, 0L);
						APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
					}
				}
			}
			return false;
		}

		private bool MatchConnector(IConnector connector)
		{
			string[] filter = new string[1] { connector.Interface };
			return Match(filter);
		}

		private bool MatchDevice(IDeviceObject parentDevice)
		{
			int num = ((IObject)parentDevice).MetaObject.SubObjectGuids.Length;
			string[] possibleInterfacesForInsert = parentDevice.GetPossibleInterfacesForInsert(num);
			return Match(possibleInterfacesForInsert);
		}

		private bool Match(string[] filter)
		{
			if (DeviceCatalogue != null)
			{
				IDeviceCatalogueFilter val = DeviceCatalogue.CreateChildConnectorFilter(filter);
				Device pLCopenDevice = GetPLCopenDevice();
				if (pLCopenDevice != null)
				{
					IDeviceIdentification val2 = PLCopenImport.CreateDeviceIdentification(pLCopenDevice.DeviceType.Item);
					IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val2);
					if (device != null)
					{
						return val.Match(device);
					}
					DeviceNotFoundException ex = new DeviceNotFoundException(val2);
					DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.ErrorImportDeviceNotPossible, ex.Message), (Severity)2, Guid.Empty, 0L);
					APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
				}
			}
			return false;
		}

		internal static ISVNode[] GetSelectedNodes()
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject != null)
			{
				return primaryProject.SelectedSVNodes;
			}
			return null;
		}
	}
}
