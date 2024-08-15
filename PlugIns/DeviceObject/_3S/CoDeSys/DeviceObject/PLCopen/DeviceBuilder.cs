using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject.DevDesc;
using _3S.CoDeSys.DeviceObject.ExportDevice;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class DeviceBuilder
	{
		private Device device;

		private DeviceObject _deviceObject;

		private IDeviceCatalogue _deviceCatalogue;

		internal DeviceBuilder()
		{
		}

		internal Guid PlugDevice(int projectHandle, Guid slotGuid, IDeviceIdentification deviceID, string deviceName, Device device)
		{
			this.device = device;
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, slotGuid);
			if (objectToRead.Object is SlotDeviceObject)
			{
				bool createBitChannels = DeviceObjectHelper.CreateBitChannels(projectHandle, slotGuid);
				_deviceObject = CreateDeviceObject(device, deviceID, createBitChannels);
				string stName = deviceName;
				if (objectToRead.Name != deviceName)
				{
					string stBaseName = DeviceObjectHelper.BuildIecIdentifier(deviceName);
					stName = DeviceObjectHelper.CreateUniqueIdentifier(projectHandle, stBaseName, DeviceObjectHelper.GetHostStub(projectHandle, slotGuid));
				}
				DeviceCommandHelper.PlugDeviceIntoSlot(projectHandle, slotGuid, _deviceObject, stName);
				return slotGuid;
			}
			return Guid.Empty;
		}

		internal DeviceObject CreateDeviceObject(Device device, IDeviceIdentification devIdent, bool createBitChannels)
		{
			DeviceDescription devdesc = ExportDevDesc.LoadDevdesc(devIdent);
			return CreateDeviceObject(device, devIdent, devdesc, createBitChannels);
		}

		internal DeviceObject CreateDeviceObject(Device device, IDeviceIdentification devIdent, DeviceDescription devdesc, bool createBitChannels)
		{
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			if (devIdent is IModuleIdentification)
			{
				IModuleIdentification val = (IModuleIdentification)(object)((devIdent is IModuleIdentification) ? devIdent : null);
				DeviceDescriptionModule[] modules = devdesc.Modules;
				foreach (DeviceDescriptionModule deviceDescriptionModule in modules)
				{
					if (deviceDescriptionModule.ModuleId == val.ModuleId)
					{
						UpdateModule(deviceDescriptionModule, device);
						break;
					}
				}
			}
			else
			{
				UpdateDevice(devdesc, device);
			}
			XmlDocument doc = ExportDevDesc.ConvertToXmlDocument(devdesc);
			TypeList typeList = null;
			if (device.Types != null && device.Types.Items != null)
			{
				try
				{
					MemoryStream memoryStream = new MemoryStream();
					new XmlSerializer(typeof(DeviceTypes), new XmlRootAttribute("Types")).Serialize(memoryStream, device.Types);
					XmlDocument xmlDocument = new XmlDocument();
					memoryStream.Position = 0L;
					xmlDocument.Load(memoryStream);
					XmlNode xmlNode = xmlDocument.GetElementsByTagName("Types")[0];
					if (xmlNode != null)
					{
						typeList = new TypeList();
						XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlNode);
						xmlNodeReader.Read();
						typeList.ReadTypes(xmlNodeReader, devIdent);
						foreach (KeyValuePair<string, ITypeDefinition> keyValuePair in typeList.TypeMap)
						{
							(keyValuePair.Value as TypeDefinition).CreatedType = true;
						}
					}
				}
				catch
				{
				}
			}
			DeviceObject deviceObject = DeviceObject.Create(devIdent, createBitChannels, doc, typeList);
			InitParameters((IDeviceObject)(object)deviceObject, device);
			deviceObject.Disable = device.DeviceType.Disable;
			deviceObject.Exclude = device.DeviceType.Exclude;
			return deviceObject;
		}

		private DeviceObject CreateDeviceObject_old(Device device, IDeviceIdentification deviceID)
		{
			return new DeviceObject(deviceID, noParameters: true)
			{
				Disable = device.DeviceType.Disable,
				Exclude = device.DeviceType.Exclude
			};
		}

		internal void ReplaceDevice(int projectHandle, Guid deviceGuid, IDeviceIdentification deviceID, string deviceName, Device device)
		{
			this.device = device;
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, deviceGuid);
			Guid parentObjectGuid = objectToRead.ParentObjectGuid;
			string text = deviceName;
			if (objectToRead.Name != deviceName)
			{
				string stBaseName = DeviceObjectHelper.BuildIecIdentifier(deviceName);
				text = DeviceObjectHelper.CreateUniqueIdentifier(projectHandle, stBaseName, DeviceObjectHelper.GetHostStub(projectHandle, deviceGuid));
			}
			bool createBitChannels = DeviceObjectHelper.CreateBitChannels(projectHandle, parentObjectGuid);
			_deviceObject = CreateDeviceObject(device, deviceID, createBitChannels);
			int num = Array.FindIndex(DeviceManager.GetOrderedSubGuids(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, parentObjectGuid)), (Guid match) => match == deviceGuid);
			((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(projectHandle, deviceGuid);
			RemoveChild(projectHandle, parentObjectGuid, deviceGuid);
			Guid guidObject = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(projectHandle, parentObjectGuid, deviceGuid, (IObject)(object)_deviceObject, text, num);
			DeviceObjectFactory.ObjectCreatedStatic(projectHandle, guidObject);
		}

		internal Guid CreateDevice(int projectHandle, Guid parentGuid, IDeviceIdentification deviceID, string deviceName, Device device)
		{
			this.device = device;
			APEnvironment.CreateDeviceObjectFactory();
			string stBaseName = DeviceObjectHelper.BuildIecIdentifier(deviceName);
			string text = DeviceObjectHelper.CreateUniqueIdentifier(projectHandle, stBaseName, DeviceObjectHelper.GetHostStub(projectHandle, parentGuid));
			bool createBitChannels = DeviceObjectHelper.CreateBitChannels(projectHandle, parentGuid);
			_deviceObject = CreateDeviceObject(device, deviceID, createBitChannels);
			Guid guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(projectHandle, parentGuid, Guid.NewGuid(), (IObject)(object)_deviceObject, text, -1);
			DeviceObjectFactory.ObjectCreatedStatic(projectHandle, guid);
			return guid;
		}

		private void RemoveChild(int projectHandle, Guid parentGuid, Guid childGuid)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(projectHandle, parentGuid);
			try
			{
				if (objectToModify == null || !(objectToModify.Object is DeviceObject))
				{
					return;
				}
				foreach (Connector item in (IEnumerable)(objectToModify.Object as DeviceObject).Connectors)
				{
					if ((int)item.ConnectorRole != 0)
					{
						continue;
					}
					foreach (IAdapter item2 in (IEnumerable)item.Adapters)
					{
						IAdapter val = item2;
						if (val is FixedAdapter)
						{
							FixedAdapter fixedAdapter = val as FixedAdapter;
							Guid[] moduleGuidsPlain = fixedAdapter.ModuleGuidsPlain;
							foreach (Guid guid in moduleGuidsPlain)
							{
								if (guid == childGuid)
								{
									fixedAdapter.RemoveDevice(guid);
								}
							}
						}
						else if (val is SlotAdapter)
						{
							SlotAdapter slotAdapter = val as SlotAdapter;
							Guid[] moduleGuidsPlain = slotAdapter.ModuleGuidsPlain;
							foreach (Guid guid2 in moduleGuidsPlain)
							{
								if (guid2 == childGuid)
								{
									slotAdapter.RemoveDevice(guid2);
								}
							}
						}
						else
						{
							if (!(val is VarAdapter))
							{
								continue;
							}
							VarAdapter varAdapter = val as VarAdapter;
							Guid[] moduleGuidsPlain = varAdapter.Modules;
							foreach (Guid guid3 in moduleGuidsPlain)
							{
								if (guid3 == childGuid)
								{
									varAdapter.Remove(guid3);
								}
							}
						}
					}
				}
			}
			finally
			{
				if (objectToModify != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(objectToModify, true, (object)null);
				}
			}
		}

		private void InitParameters(IDeviceObject deviceObject, Device device)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			ConnectorType[] connector = device.DeviceType.Connector;
			foreach (IConnector item in (IEnumerable)deviceObject.Connectors)
			{
				IConnector con = item;
				IParameterSet hostParameterSet = con.HostParameterSet;
				IParameterSet4 pset = (IParameterSet4)(object)((hostParameterSet is IParameterSet4) ? hostParameterSet : null);
				ConnectorType connectorType = Array.Find(connector, (ConnectorType cType) => con.ConnectorId == cType.connectorId);
				if (connectorType != null)
				{
					InitParameterSet(pset, connectorType.HostParameterSet);
				}
			}
			DeviceParameterListType deviceParameterSet = device.DeviceType.DeviceParameterSet;
			if (deviceParameterSet != null && deviceParameterSet.Items != null)
			{
				IParameterSet deviceParameterSet2 = deviceObject.DeviceParameterSet;
				IParameterSet4 pset2 = (IParameterSet4)(object)((deviceParameterSet2 is IParameterSet4) ? deviceParameterSet2 : null);
				InitParameterSet(pset2, deviceParameterSet.Items);
			}
		}

		private void InitParameterSet(IParameterSet4 pset, object[] parameterSet)
		{
			if (parameterSet == null)
			{
				return;
			}
			foreach (object obj in parameterSet)
			{
				if (obj is ParameterSectionType)
				{
					ParameterSectionType parameterSectionType = obj as ParameterSectionType;
					InitParameterSet(pset, parameterSectionType.Items);
				}
				else
				{
					if (!(obj is ParameterType))
					{
						continue;
					}
					ParameterType parameterType = obj as ParameterType;
					Parameter parameter = ((IParameterSet)pset).GetParameter((long)parameterType.ParameterId) as Parameter;
					if (parameter == null)
					{
						continue;
					}
					if (parameterType.IndexInDevDescSpecified)
					{
						parameter.IndexInDevDesc = parameterType.IndexInDevDesc;
					}
					if (!string.IsNullOrEmpty(parameterType.FixedAddress))
					{
						parameter.IoMapping.AutomaticIecAddress=(false);
						parameter.IoMapping.IecAddress=(parameterType.FixedAddress);
					}
					if (parameter.DataElementBase is DataElementArrayType)
					{
						SetArrayValues(pset, (IDataElement)(object)parameter.DataElementBase, parameterType.Value);
						if (parameterType.Mapping != null && parameterType.Mapping.Length != 0)
						{
							SetMapping((IDataElement)(object)parameter, parameterType.Mapping[0]);
						}
					}
					else
					{
						SetValue(pset, (IDataElement2)(object)parameter, parameterType.Value[0]);
						if (parameterType.Mapping != null && parameterType.Mapping.Length != 0)
						{
							SetMapping((IDataElement)(object)parameter, parameterType.Mapping[0]);
						}
					}
				}
			}
		}

		private bool Match(string[] filter, IDeviceIdentification deviceID)
		{
			_deviceCatalogue = APEnvironment.CreateFirstDeviceCatalogue();
			IDeviceCatalogueFilter obj = _deviceCatalogue.CreateChildConnectorFilter(filter);
			IDeviceDescription val = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceID);
			return obj.Match(val);
		}

		private void UpdateDevice(DeviceDescription devdesc, Device device)
		{
			if (devdesc?.Device?.Connector != null)
			{
				DeviceDescriptionDeviceConnector[] connector = devdesc.Device.Connector;
				foreach (_3S.CoDeSys.DeviceObject.DevDesc.ConnectorType con in connector)
				{
					UpdateConnector(device, con);
				}
			}
		}

		private void UpdateConnector(Device device, _3S.CoDeSys.DeviceObject.DevDesc.ConnectorType con)
		{
			ConnectorType connectorType = Array.Find(device.DeviceType.Connector, (ConnectorType cType) => con.connectorId == cType.connectorId);
			if (connectorType != null)
			{
				UpdateParamSet(con, connectorType);
				if (connectorType.Custom != null && connectorType.Custom.Any != null && connectorType.Custom.Any.Length != 0)
				{
					con.Custom = new _3S.CoDeSys.DeviceObject.DevDesc.CustomType();
					con.Custom.Any = connectorType.Custom.Any;
				}
				else
				{
					con.Custom = null;
				}
			}
		}

		private void UpdateModule(DeviceDescriptionModule devdescModule, Device device)
		{
			DeviceDescriptionDeviceConnector[] connector = devdescModule.Connector;
			foreach (_3S.CoDeSys.DeviceObject.DevDesc.ConnectorType con in connector)
			{
				UpdateConnector(device, con);
			}
		}

		private void UpdateParamSet(_3S.CoDeSys.DeviceObject.DevDesc.ConnectorType con, ConnectorType exportCon)
		{
			LList<object> val = CreateParamSet(con, exportCon.HostParameterSet);
			con.HostParameterSet = val.ToArray();
		}

		private LList<object> CreateParamSet(_3S.CoDeSys.DeviceObject.DevDesc.ConnectorType con, object[] exportItems)
		{
			LList<object> val = new LList<object>();
			if (exportItems != null)
			{
				foreach (object obj in exportItems)
				{
					if (obj is ParameterType)
					{
						ParameterType parameterType = obj as ParameterType;
						_3S.CoDeSys.DeviceObject.DevDesc.ParameterType parameterType2 = FindParameterType(con, parameterType.ParameterId);
						bool inDevdesc = parameterType2 != null;
						if (parameterType2 == null)
						{
							parameterType2 = new _3S.CoDeSys.DeviceObject.DevDesc.ParameterType();
						}
						UpdateParam(parameterType, parameterType2, inDevdesc);
						val.Add((object)parameterType2);
					}
					else if (obj is ParameterSectionType)
					{
						ParameterSectionType parameterSectionType = obj as ParameterSectionType;
						_3S.CoDeSys.DeviceObject.DevDesc.ParameterSectionType parameterSectionType2 = new _3S.CoDeSys.DeviceObject.DevDesc.ParameterSectionType();
						parameterSectionType2.Name = new StringRefType();
						parameterSectionType2.Name.Value = parameterSectionType.Name;
						parameterSectionType2.Items = CreateParamSet(con, parameterSectionType.Items).ToArray();
						if (parameterSectionType.Custom != null && parameterSectionType.Custom.Any != null && parameterSectionType.Custom.Any.Length != 0)
						{
							parameterSectionType2.Custom = new _3S.CoDeSys.DeviceObject.DevDesc.CustomType();
							parameterSectionType2.Custom.Any = parameterSectionType.Custom.Any;
						}
						else
						{
							parameterSectionType2.Custom = null;
						}
						val.Add((object)parameterSectionType2);
					}
				}
			}
			return val;
		}

		private _3S.CoDeSys.DeviceObject.DevDesc.ParameterType FindParameterType(_3S.CoDeSys.DeviceObject.DevDesc.ConnectorType con, uint id)
		{
			return FindParameterType(con.HostParameterSet, id);
		}

		private _3S.CoDeSys.DeviceObject.DevDesc.ParameterType FindParameterType(object[] items, uint id)
		{
			if (items != null)
			{
				foreach (object obj in items)
				{
					if (obj is _3S.CoDeSys.DeviceObject.DevDesc.ParameterType)
					{
						_3S.CoDeSys.DeviceObject.DevDesc.ParameterType parameterType = obj as _3S.CoDeSys.DeviceObject.DevDesc.ParameterType;
						if (parameterType.ParameterId == id)
						{
							return parameterType;
						}
					}
					else if (obj is _3S.CoDeSys.DeviceObject.DevDesc.ParameterSectionType)
					{
						_3S.CoDeSys.DeviceObject.DevDesc.ParameterSectionType parameterSectionType = obj as _3S.CoDeSys.DeviceObject.DevDesc.ParameterSectionType;
						_3S.CoDeSys.DeviceObject.DevDesc.ParameterType parameterType2 = FindParameterType(parameterSectionType.Items, id);
						if (parameterType2 != null)
						{
							return parameterType2;
						}
					}
				}
			}
			return null;
		}

		private static void UpdateParam(ParameterType exportParam, _3S.CoDeSys.DeviceObject.DevDesc.ParameterType param, bool inDevdesc)
		{
			param.ParameterId = exportParam.ParameterId;
			param.type = exportParam.type;
			if (!inDevdesc)
			{
				StringRefType stringRefType = new StringRefType();
				stringRefType.Value = exportParam.Unit;
				param.Unit = stringRefType;
				StringRefType stringRefType2 = new StringRefType();
				stringRefType2.Value = exportParam.Name;
				param.Name = stringRefType2;
				StringRefType stringRefType3 = new StringRefType();
				stringRefType3.Value = exportParam.Description;
				param.Description = stringRefType3;
			}
			else
			{
				if ((param.Name != null && param.Name.Value != exportParam.Name) || (param.Name == null && !string.IsNullOrEmpty(exportParam.Name)))
				{
					StringRefType stringRefType4 = new StringRefType();
					stringRefType4.Value = exportParam.Name;
					param.Name = stringRefType4;
				}
				if ((param.Description != null && param.Description.Value != exportParam.Description) || (param.Description == null && !string.IsNullOrEmpty(exportParam.Description)))
				{
					StringRefType stringRefType5 = new StringRefType();
					stringRefType5.Value = exportParam.Description;
					param.Description = stringRefType5;
				}
			}
			ParameterTypeAttributes attributes = exportParam.Attributes;
			param.Attributes = new _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributes();
			param.Attributes.alwaysmapping = attributes.alwaysmapping;
			switch (attributes.alwaysmappingMode)
			{
			case ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused:
				param.Attributes.alwaysmappingMode = _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused;
				break;
			case ParameterTypeAttributesAlwaysmappingMode.AlwaysInBusCycle:
				param.Attributes.alwaysmappingMode = _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesAlwaysmappingMode.AlwaysInBusCycle;
				break;
			}
			param.Attributes.channel = CreateChannel(attributes.channel);
			switch (param.Attributes.channel)
			{
			case _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.output:
				param.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.output;
				break;
			case _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.input:
				param.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.input;
				break;
			case _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.file:
				param.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.file;
				break;
			case _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.diag:
				param.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.diag;
				break;
			case _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.diagAck:
				param.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.diagAck;
				break;
			case _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.none:
				param.Attributes.channelAttr = ParameterTypeAttributesChannelCompatible.none;
				break;
			}
			param.Attributes.channelSpecified = param.Attributes.channel != _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.none;
			param.Attributes.createDownloadStructure = attributes.createDownloadStructure;
			param.Attributes.createInChildConnector = attributes.createInChildConnector;
			param.Attributes.createInHostConnector = attributes.createInHostConnector;
			param.Attributes.download = attributes.download;
			param.Attributes.functional = attributes.functional;
			param.Attributes.offlineaccess = CreateOfflineAccessRight(attributes.offlineaccess);
			param.Attributes.onlineaccess = CreateOnlineAccessRight(attributes.onlineaccess);
			param.Attributes.onlineparameter = attributes.onlineparameter;
			param.Attributes.instanceVariable = attributes.instanceVariable;
			param.Attributes.preparedValueAccess = attributes.preparedValueAccess;
			param.Attributes.driverSpecific = attributes.driverSpecific;
			param.Attributes.useRefactoring = attributes.useRefactoring;
			param.Attributes.disableMapping = attributes.disableMapping;
			if (exportParam.Custom != null && exportParam.Custom.Any != null && exportParam.Custom.Any.Length != 0)
			{
				param.Custom = new _3S.CoDeSys.DeviceObject.DevDesc.CustomType();
				param.Custom.Any = exportParam.Custom.Any;
			}
			else
			{
				param.Custom = null;
			}
		}

		internal static ParameterTypeAttributesOnlineaccess CreateOnlineAccessRight(AccessRightType accessType)
		{
			return accessType switch
			{
				AccessRightType.none => ParameterTypeAttributesOnlineaccess.none, 
				AccessRightType.read => ParameterTypeAttributesOnlineaccess.read, 
				AccessRightType.write => ParameterTypeAttributesOnlineaccess.write, 
				_ => ParameterTypeAttributesOnlineaccess.readwrite, 
			};
		}

		internal static ParameterTypeAttributesOfflineaccess CreateOfflineAccessRight(AccessRightType accessType)
		{
			return accessType switch
			{
				AccessRightType.none => ParameterTypeAttributesOfflineaccess.none, 
				AccessRightType.read => ParameterTypeAttributesOfflineaccess.read, 
				AccessRightType.write => ParameterTypeAttributesOfflineaccess.write, 
				_ => ParameterTypeAttributesOfflineaccess.readwrite, 
			};
		}

		internal static _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel CreateChannel(ParameterTypeAttributesChannel channelType)
		{
			return channelType switch
			{
				ParameterTypeAttributesChannel.input => _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.input, 
				ParameterTypeAttributesChannel.output => _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.output, 
				ParameterTypeAttributesChannel.diag => _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.diag, 
				ParameterTypeAttributesChannel.diagAck => _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.diagAck, 
				_ => _3S.CoDeSys.DeviceObject.DevDesc.ParameterTypeAttributesChannel.none, 
			};
		}

		private static void SetValue(IParameterSet4 pset, IDataElement2 dataElement, ValueType valueType)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Expected I4, but got Unknown
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Expected O, but got Unknown
			if (((IDataElement)dataElement).HasSubElements)
			{
				if (valueType.Element != null)
				{
					foreach (IDataElement2 item in (IEnumerable)((IDataElement)dataElement).SubElements)
					{
						IDataElement2 val = item;
						ValueTypeElement[] element = valueType.Element;
						foreach (ValueTypeElement valueTypeElement in element)
						{
							if (!(((IDataElement)val).Identifier == valueTypeElement.name))
							{
								continue;
							}
							SetValue(pset, val, valueTypeElement);
							if (!string.IsNullOrEmpty(valueTypeElement.desc))
							{
								IDataElement12 val2 = (IDataElement12)(object)((val is IDataElement12) ? val : null);
								if (val2 != null)
								{
									IStringRef descriptionStringRef = val2.DescriptionStringRef;
									IStringRef description = ((IParameterSet2)pset).StringTable.CreateStringRef(descriptionStringRef.Namespace, descriptionStringRef.Identifier, valueTypeElement.desc);
									((IDataElement4)val2).SetDescription(description);
								}
							}
							if (!string.IsNullOrEmpty(valueTypeElement.visiblename))
							{
								IDataElement12 val3 = (IDataElement12)(object)((val is IDataElement12) ? val : null);
								if (val3 != null)
								{
									IStringRef visibleNameStringRef = val3.VisibleNameStringRef;
									IStringRef name = ((IParameterSet2)pset).StringTable.CreateStringRef(visibleNameStringRef.Namespace, visibleNameStringRef.Identifier, valueTypeElement.visiblename);
									((IDataElement4)val3).SetName(name);
								}
							}
							IDataElement8 val4 = (IDataElement8)(object)((val is IDataElement8) ? val : null);
							if (val4 != null)
							{
								val4.SetAccessRight(false, EnumConv.CreateAccessRight(valueTypeElement.offlineaccess));
								val4.SetAccessRight(true, EnumConv.CreateAccessRight(valueTypeElement.onlineaccess));
							}
							if (valueTypeElement.Custom != null && valueTypeElement.Custom.Any != null && valueTypeElement.Custom.Any.Length != 0)
							{
								ICustomItemList customItems = ((IDataElement)val).CustomItems;
								ICustomItemList2 val5 = (ICustomItemList2)(object)((customItems is ICustomItemList2) ? customItems : null);
								bool isReadOnly = ((IList)val5).IsReadOnly;
								try
								{
									if (isReadOnly && val5 is CustomItemList)
									{
										(val5 as CustomItemList).IsReadOnly = false;
									}
									((IList)val5).Clear();
									XmlElement[] any = valueTypeElement.Custom.Any;
									foreach (XmlElement xmlElement in any)
									{
										val5.Add(xmlElement.OuterXml, -1);
									}
								}
								finally
								{
									if (isReadOnly && val5 is CustomItemList)
									{
										(val5 as CustomItemList).IsReadOnly = true;
									}
								}
							}
							else if (!((IList)((IDataElement)val).CustomItems).IsReadOnly)
							{
								((IList)((IDataElement)val).CustomItems).Clear();
							}
						}
					}
				}
				else
				{
					if (!dataElement.HasBaseType || valueType.Text == null || valueType.Text.Length == 0)
					{
						return;
					}
					DirectVariableSize size = FlatAdressAssignmentStrategy.GetSize(((IDataElement)dataElement).BaseType);
					switch ((int)size - 2)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					{
						ulong result = 0uL;
						if (!ulong.TryParse(valueType.Text[0], out result))
						{
							break;
						}
						int num = 0;
						foreach (IDataElement2 item2 in (IEnumerable)((IDataElement)dataElement).SubElements)
						{
							IDataElement2 val6 = item2;
							if (((result >> num++) & 1) != 0)
							{
								((IDataElement)val6).Value=("TRUE");
							}
							else
							{
								((IDataElement)val6).Value=("FALSE");
							}
						}
						break;
					}
					}
				}
			}
			else if (valueType.Text != null && valueType.Text.Length != 0)
			{
				((IDataElement)dataElement).Value=(valueType.Text[0]);
			}
		}

		private void SetArrayValues(IParameterSet4 pset, IDataElement dataElement, ValueType[] valueType)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < ((ICollection)dataElement.SubElements).Count; i++)
			{
				ValueType valueType2 = valueType[i];
				IDataElement obj = dataElement.SubElements[i];
				IDataElement4 val = (IDataElement4)(object)((obj is IDataElement4) ? obj : null);
				if (val != null)
				{
					if (!string.IsNullOrEmpty(valueType2.desc))
					{
						IDataElement12 val2 = (IDataElement12)(object)((val is IDataElement12) ? val : null);
						if (val2 != null)
						{
							IStringRef descriptionStringRef = val2.DescriptionStringRef;
							IStringRef description = ((IParameterSet2)pset).StringTable.CreateStringRef(descriptionStringRef.Namespace, descriptionStringRef.Identifier, valueType2.desc);
							((IDataElement4)val2).SetDescription(description);
						}
					}
					if (!string.IsNullOrEmpty(valueType2.visiblename))
					{
						IDataElement12 val3 = (IDataElement12)(object)((val is IDataElement12) ? val : null);
						if (val3 != null)
						{
							IStringRef visibleNameStringRef = val3.VisibleNameStringRef;
							IStringRef name = ((IParameterSet2)pset).StringTable.CreateStringRef(visibleNameStringRef.Namespace, visibleNameStringRef.Identifier, valueType2.visiblename);
							((IDataElement4)val3).SetName(name);
						}
					}
					IDataElement8 val4 = (IDataElement8)(object)((val is IDataElement8) ? val : null);
					if (val4 != null)
					{
						val4.SetAccessRight(false, EnumConv.CreateAccessRight(valueType2.offlineaccess));
						val4.SetAccessRight(true, EnumConv.CreateAccessRight(valueType2.onlineaccess));
					}
					if (valueType2.Custom != null && valueType2.Custom.Any != null && valueType2.Custom.Any.Length != 0)
					{
						ICustomItemList customItems = ((IDataElement)val).CustomItems;
						ICustomItemList2 val5 = (ICustomItemList2)(object)((customItems is ICustomItemList2) ? customItems : null);
						((IList)val5).Clear();
						XmlElement[] any = valueType2.Custom.Any;
						foreach (XmlElement xmlElement in any)
						{
							val5.Add(xmlElement.OuterXml, -1);
						}
					}
					else
					{
						((IList)((IDataElement)val).CustomItems).Clear();
					}
					if (((IDataElement)val).HasSubElements)
					{
						DataElementBase dataElementBase = val as DataElementBase;
						if (dataElementBase != null && dataElementBase is DataElementArrayType)
						{
							SetArrayValues(pset, (IDataElement)(object)dataElementBase, new ValueType[1] { valueType2 });
						}
						else
						{
							SetValue(pset, (IDataElement2)(object)val, valueType2);
						}
					}
				}
				if (valueType2.Text != null && valueType2.Text.Length != 0)
				{
					string value = valueType2.Text[0];
					if (!string.IsNullOrEmpty(value))
					{
						((IDataElement)val).Value=(value);
					}
				}
			}
		}

		private static void SetMapping(IDataElement dataElement, ValueType valueType)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			if (dataElement.HasSubElements)
			{
				foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
				{
					IDataElement val = item;
					if (valueType.Element == null)
					{
						continue;
					}
					ValueTypeElement[] element = valueType.Element;
					foreach (ValueTypeElement valueTypeElement in element)
					{
						if (val.Identifier == valueTypeElement.name)
						{
							SetMapping(val, valueTypeElement);
						}
					}
				}
			}
			if (valueType.Text == null)
			{
				return;
			}
			string[] text = valueType.Text;
			foreach (string text2 in text)
			{
				bool flag = !text2.Contains(".");
				while (((ICollection)dataElement.IoMapping.VariableMappings).Count > 0)
				{
					dataElement.IoMapping.VariableMappings.RemoveAt(0);
				}
				dataElement.IoMapping.VariableMappings.AddMapping(text2, flag);
			}
		}
	}
}
