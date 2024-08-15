using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.ExternalFileObject;
using _3S.CoDeSys.FdtIntegration;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.TraceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ReplaceDeviceAction : IUndoableAction
	{
		internal static string REPLACEACTION = "Replace device";

		private int _nProjectHandle;

		private Guid _guidDevice;

		private IDeviceIdentification _deviceId;

		private List<ReplacedObject> _replacedObjects;

		private LList<DeviceEventArgs2> _liUpdated;

		public string Description => REPLACEACTION;

		internal ReplaceDeviceAction(int nProjectHandle, Guid guidDevice, IDeviceIdentification deviceId, LList<DeviceEventArgs2> liUpdated)
		{
			_nProjectHandle = nProjectHandle;
			_guidDevice = guidDevice;
			_deviceId = deviceId;
			_liUpdated = liUpdated;
		}

		public object Undo()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			if (_replacedObjects != null)
			{
				for (int num = _replacedObjects.Count - 1; num >= 0; num--)
				{
					ReplacedObject replacedObject = _replacedObjects[num];
					IMetaObject2 val = (IMetaObject2)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, replacedObject.ObjectGuid);
					try
					{
						if (replacedObject.OldObject != null)
						{
							val.ReplaceObject((IObject)((GenericObject)replacedObject.OldObject).Clone());
						}
						if (replacedObject.OldSlotObject != null)
						{
							val.ReplaceObject((IObject)((GenericObject)replacedObject.OldSlotObject).Clone());
						}
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_nProjectHandle, replacedObject.ObjectGuid);
						((IObjectManager)APEnvironment.ObjectMgr).SetObject((IMetaObject)(object)val, true, (object)null);
					}
					catch
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject((IMetaObject)(object)val, false, (object)null);
						throw;
					}
				}
			}
			return null;
		}

		public object Redo()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			if (_replacedObjects == null)
			{
				_replacedObjects = new List<ReplacedObject>();
				ReplaceObject(_guidDevice, _deviceId, bInRecursion: false);
			}
			else
			{
				for (int i = 0; i < _replacedObjects.Count; i++)
				{
					ReplacedObject replacedObject = _replacedObjects[i];
					IMetaObject2 val = (IMetaObject2)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, replacedObject.ObjectGuid);
					try
					{
						val.ReplaceObject((IObject)((GenericObject)replacedObject.NewObject).Clone());
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_nProjectHandle, replacedObject.ObjectGuid);
						((IObjectManager)APEnvironment.ObjectMgr).SetObject((IMetaObject)(object)val, true, (object)null);
					}
					catch
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject((IMetaObject)(object)val, false, (object)null);
						throw;
					}
				}
			}
			return null;
		}

		private List<Guid> GetApplicationList(IMetaObject topLevelDeviceMeta)
		{
			List<Guid> list = new List<Guid>();
			IMetaObjectStub val = null;
			Guid[] subObjectGuids = topLevelDeviceMeta.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(topLevelDeviceMeta.ProjectHandle, guid);
				if (val != null && typeof(IPlcLogicObject).IsAssignableFrom(val.ObjectType))
				{
					break;
				}
			}
			if (val == null)
			{
				return list;
			}
			List<Guid> list2 = new List<Guid>();
			subObjectGuids = val.SubObjectGuids;
			foreach (Guid item in subObjectGuids)
			{
				list2.Add(item);
			}
			bool flag = true;
			_ = Guid.Empty;
			while (flag)
			{
				List<Guid> list3 = new List<Guid>(list2);
				list2.Clear();
				flag = false;
				foreach (Guid item3 in list3)
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(topLevelDeviceMeta.ProjectHandle, item3);
					if (metaObjectStub != null && typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						list.Add(metaObjectStub.ObjectGuid);
					}
					subObjectGuids = metaObjectStub.SubObjectGuids;
					foreach (Guid item2 in subObjectGuids)
					{
						flag = true;
						list2.Add(item2);
					}
				}
			}
			return list;
		}

		private void CheckTargetSettingConstraints(IDeviceObject olddevice, IDeviceIdentification newDeviceID)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((olddevice is IDeviceObject5) ? olddevice : null)).DeviceIdentificationNoSimulation;
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceIdentificationNoSimulation);
			ITargetSettings targetSettingsById2 = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(newDeviceID);
			List<Guid> applicationList = GetApplicationList(((IObject)olddevice).MetaObject);
			if (targetSettingsById == null || targetSettingsById2 == null)
			{
				return;
			}
			int num = LocalTargetSettings.MaxNumberOfApps.GetIntValue(targetSettingsById);
			if (num == -1)
			{
				num = int.MaxValue;
			}
			int intValue = LocalTargetSettings.MaxNumberOfApps.GetIntValue(targetSettingsById2);
			if (intValue > -1 && intValue < num && intValue < applicationList.Count)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)12, Strings.ErrorIncompatibleNumOfApps);
			}
			if (intValue != 1 || applicationList.Count != 1)
			{
				return;
			}
			string stringValue = LocalTargetSettings.FixedAppName.GetStringValue(targetSettingsById2);
			if (stringValue != string.Empty)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)olddevice).MetaObject.ProjectHandle, applicationList[0]);
				if (metaObjectStub != null && metaObjectStub.Name != stringValue)
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)13, string.Format(Strings.ErrorIncompatibleAppName, metaObjectStub.Name, stringValue));
				}
			}
		}

		private void CheckFdtConstraints(IDeviceObject olddevice, IDeviceIdentification newDeviceId)
		{
			IFdtIntegration fdtIntegrationOrNull = (IFdtIntegration)(object)APEnvironment.FdtIntegrationOrNull;
			if (fdtIntegrationOrNull != null)
			{
				IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(newDeviceId);
				fdtIntegrationOrNull.CheckFdtUpdateDeviceConstraints(olddevice, device);
			}
		}

		private void CheckUpdateConstraints(IDeviceObjectBase newchild, Guid parentObjectGuid, bool bRecursion)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, parentObjectGuid);
			if (metaObjectStub != null && typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, parentObjectGuid);
				if (objectToRead != null)
				{
					IObject @object = objectToRead.Object;
					foreach (Connector item in (IEnumerable)((IDeviceObject)((@object is IDeviceObject) ? @object : null)).Connectors)
					{
						if (item.DriverInfo != null && item.DriverInfo.RequiredLibs != null)
						{
							item.CheckConstraints(newchild, bRecursion);
						}
					}
				}
			}
			if (metaObjectStub == null || !typeof(ExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return;
			}
			IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, parentObjectGuid);
			if (objectToRead2 == null)
			{
				return;
			}
			ExplicitConnector explicitConnector = objectToRead2.Object as ExplicitConnector;
			if (explicitConnector != null)
			{
				IIoModuleIterator val = explicitConnector.CreateModuleIterator();
				if (val != null && val.MoveToParent() && val.Current.IsConnector)
				{
					IIoModuleEditorHelper obj = val.Current.CreateEditorHelper();
					(obj.GetConnector(false) as Connector)?.CheckConstraints(newchild, bRecursion);
					((IDisposable)obj).Dispose();
				}
			}
		}

		private void LibraryToRemove(IDriverInfo driverinfo, IDeviceObject rootdevice, ArrayList arLibraryToRemove)
		{
			foreach (RequiredLib item in (IEnumerable)driverinfo.RequiredLibs)
			{
				string value = $"{item.LibName}, {item.Version} ({item.Vendor})";
				string placeHolderLib = item.PlaceHolderLib;
				if (placeHolderLib != string.Empty && rootdevice != null)
				{
					IDeviceIdentification val = null;
					val = ((!(rootdevice is IDeviceObject5)) ? rootdevice.DeviceIdentification : ((IDeviceObject5)((rootdevice is IDeviceObject5) ? rootdevice : null)).DeviceIdentificationNoSimulation);
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val);
					if (targetSettingsById != null)
					{
						string text = $"library-management\\placeholder-libraries\\{placeHolderLib}";
						string text2 = null;
						text2 = targetSettingsById.GetStringValue(text, (string)null);
						if (text2 != null && text2 != string.Empty)
						{
							value = text2;
						}
					}
				}
				arLibraryToRemove.Add(value);
			}
		}

		private void RemoveAllSystemLibs(Guid guidDevice, IDeviceObject rootdevice)
		{
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Expected O, but got Unknown
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guidDevice);
			if (metaObjectStub == null || (!typeof(IDeviceObject2).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType)))
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidDevice);
			Guid[] subObjectGuids;
			if (objectToRead.Object is IDeviceObject2)
			{
				IObject @object = objectToRead.Object;
				IDeviceObject2 val = (IDeviceObject2)(object)((@object is IDeviceObject2) ? @object : null);
				if (val == null)
				{
					return;
				}
				if (rootdevice == null)
				{
					rootdevice = DeviceObjectHelper.GetPlcDevice(_nProjectHandle, guidDevice);
				}
				if (val is DeviceObject && rootdevice == null)
				{
					rootdevice = (val as DeviceObject).GetHostDeviceObject();
				}
				if (objectToRead.Object is SlotDeviceObject && rootdevice == null)
				{
					rootdevice = (IDeviceObject)(object)(objectToRead.Object as SlotDeviceObject).GetDevice();
					if (rootdevice != null && rootdevice is DeviceObject && ((IObject)rootdevice).MetaObject.ParentObjectGuid != Guid.Empty)
					{
						rootdevice = (rootdevice as DeviceObject).GetHostDeviceObject();
					}
				}
				subObjectGuids = objectToRead.SubObjectGuids;
				foreach (Guid guidDevice2 in subObjectGuids)
				{
					RemoveAllSystemLibs(guidDevice2, rootdevice);
				}
				if (val.DriverInfo != null && val.DriverInfo.RequiredLibs != null)
				{
					LibraryToRemove(val.DriverInfo, rootdevice, arrayList);
				}
				foreach (Connector item in (IEnumerable)((IDeviceObject)val).Connectors)
				{
					if (item.DriverInfo != null && item.DriverInfo.RequiredLibs != null)
					{
						LibraryToRemove(item.DriverInfo, rootdevice, arrayList);
					}
				}
			}
			if (objectToRead.Object is IExplicitConnector)
			{
				subObjectGuids = objectToRead.SubObjectGuids;
				foreach (Guid guidDevice3 in subObjectGuids)
				{
					RemoveAllSystemLibs(guidDevice3, rootdevice);
				}
			}
			if (rootdevice == null || !(rootdevice is DeviceObject))
			{
				return;
			}
			IMetaObject application = (rootdevice as DeviceObject).GetApplication();
			if (application == null)
			{
				return;
			}
			subObjectGuids = application.SubObjectGuids;
			string text2 = default(string);
			string text3 = default(string);
			string text4 = default(string);
			string text6 = default(string);
			string text7 = default(string);
			string text8 = default(string);
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guid);
				if (!typeof(ILibManObject).IsAssignableFrom(metaObjectStub2.ObjectType))
				{
					continue;
				}
				IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guid).Object;
				ILibManItem[] allLibraries = ((ILibManObject)((object2 is ILibManObject) ? object2 : null)).GetAllLibraries(false);
				foreach (ILibManItem val2 in allLibraries)
				{
					string text = val2.Name;
					if (val2 is IPlaceholderLibManItem)
					{
						IPlaceholderLibManItem val3 = (IPlaceholderLibManItem)val2;
						if (val3.EffectiveResolution != null)
						{
							text = val3.EffectiveResolution.DisplayName;
						}
						else if (val3.DefaultResolution != null)
						{
							text = val3.DefaultResolution;
						}
					}
					if (arrayList.Contains(text))
					{
						((ILibraryLoader)APEnvironment.LibraryLoader).UnloadSystemLibrary(val2.Name, _nProjectHandle, application.ObjectGuid);
					}
					else
					{
						if (!((ILibraryLoader3)APEnvironment.LibraryLoader).ParseDisplayName(text, out text2, out text3, out text4))
						{
							continue;
						}
						foreach (string item2 in arrayList)
						{
							if (text != item2 && ((ILibraryLoader3)APEnvironment.LibraryLoader).ParseDisplayName(item2, out text6, out text7, out text8) && text2 == text6 && text8 == text4)
							{
								((ILibraryLoader)APEnvironment.LibraryLoader).UnloadSystemLibrary(val2.Name, _nProjectHandle, application.ObjectGuid);
								break;
							}
						}
					}
				}
			}
		}

		private bool ReplaceObject(Guid guidDevice, IDeviceIdentification deviceId, bool bInRecursion)
		{
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Expected O, but got Unknown
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Expected O, but got Unknown
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Expected O, but got Unknown
			//IL_0637: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Invalid comparison between Unknown and I4
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Expected O, but got Unknown
			//IL_0919: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c5b: Expected O, but got Unknown
			//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dba: Expected O, but got Unknown
			//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
			//IL_1160: Unknown result type (might be due to invalid IL or missing references)
			//IL_11e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_12f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_12fb: Expected O, but got Unknown
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1333: Expected O, but got Unknown
			//IL_135c: Unknown result type (might be due to invalid IL or missing references)
			//IL_1366: Expected O, but got Unknown
			LList<DeviceIdUpdate> val = new LList<DeviceIdUpdate>();
			IConnectorCollection val2 = null;
			IMetaObject val3 = null;
			IDeviceIdentification val4 = null;
			bool bMustBeRemoved = false;
			bool flag = false;
			try
			{
				Exception ex = ((IEngine2)APEnvironment.Engine).CheckObjectAccess(_nProjectHandle, guidDevice, (ObjectPermissionKind)1);
				if (ex != null)
				{
					flag = true;
					throw ex;
				}
				RemoveAllSystemLibs(guidDevice, null);
				val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidDevice);
				if (val3.Object is DeviceObject)
				{
					(val3.Object as DeviceObject).IsInUpdate = true;
				}
				if (val3.Object is SlotDeviceObject)
				{
					(val3.Object as SlotDeviceObject).IsInUpdate = true;
				}
				if (!flag)
				{
					try
					{
						APEnvironment.DeviceMgr.RaiseDeviceUpdating(_nProjectHandle, guidDevice, deviceId);
					}
					catch (Exception ex2)
					{
						APEnvironment.MessageService.Error(ex2.Message, "Exception", Array.Empty<object>());
						flag = true;
					}
				}
				if (!flag)
				{
					bool bVersionUpgrade = false;
					val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, guidDevice);
					if (val3 != null)
					{
						if (val3.Object is DeviceObject)
						{
							LList<Guid> val5 = new LList<Guid>();
							DeviceObject deviceObject = (DeviceObject)(object)val3.Object;
							DeviceObject deviceObject2 = (DeviceObject)((GenericObject)deviceObject).Clone();
							val4 = deviceObject2.DeviceIdentificationNoSimulation;
							CheckTargetSettingConstraints((IDeviceObject)(object)deviceObject, deviceId);
							CheckFdtConstraints((IDeviceObject)(object)deviceObject, deviceId);
							deviceObject.CreateBitChannels = DeviceObjectHelper.CreateBitChannels(_nProjectHandle, guidDevice);
							deviceObject.SetDeviceIdentification(deviceId, bUpdate: true, val, out bMustBeRemoved, out bVersionUpgrade);
							val2 = deviceObject.Connectors;
							bool flag2 = false;
							bool flag3 = false;
							for (int i = 0; i < ((ICollection)val2).Count; i++)
							{
								if (val2[i] is ErrorConnector)
								{
									flag2 = true;
									continue;
								}
								foreach (Parameter item in (IEnumerable)val2[i].HostParameterSet)
								{
									if (item.CreateInHostConnector)
									{
										flag3 = true;
									}
								}
							}
							if (flag2)
							{
								object obj = ((GenericObject)(deviceObject.Connectors as ConnectorList)).Clone();
								val2 = (IConnectorCollection)((obj is IConnectorCollection) ? obj : null);
								for (int j = 0; j < ((ICollection)deviceObject.Connectors).Count; j++)
								{
									if (!(deviceObject.Connectors[j] is ErrorConnector))
									{
										continue;
									}
									foreach (IAdapter item2 in (IEnumerable)deviceObject.Connectors[j].Adapters)
									{
										IAdapter val6 = item2;
										val5.AddRange((IEnumerable<Guid>)val6.Modules);
									}
									deviceObject.RawConnectorList.Remove(deviceObject.Connectors[j]);
								}
							}
							if (val3.ParentObjectGuid != Guid.Empty)
							{
								IMetaObject val7 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val3.ProjectHandle, val3.ParentObjectGuid);
								ArrayList arrayList = new ArrayList();
								if (val7.Object is DeviceObject)
								{
									DeviceObject deviceObject3 = val7.Object as DeviceObject;
									arrayList.AddRange(deviceObject3.RawConnectorList);
								}
								if (val7.Object is ExplicitConnector)
								{
									ExplicitConnector value = val7.Object as ExplicitConnector;
									arrayList.Add(value);
								}
								if (val7.Object is SlotDeviceObject)
								{
									DeviceObject deviceObject4 = (val7.Object as SlotDeviceObject).GetDeviceObject() as DeviceObject;
									if (deviceObject4 != null)
									{
										arrayList.AddRange(deviceObject4.RawConnectorList);
									}
								}
								if (arrayList.Count > 0)
								{
									bool flag4 = false;
									foreach (IConnector item3 in arrayList)
									{
										IConnector val8 = item3;
										foreach (IAdapter item4 in (IEnumerable)val8.Adapters)
										{
											Guid[] modules = item4.Modules;
											for (int k = 0; k < modules.Length; k++)
											{
												if (!(modules[k] == guidDevice))
												{
													continue;
												}
												foreach (Connector item5 in (IEnumerable)val2)
												{
													if (DeviceManager.CheckMatchInterface((IConnector7)(object)item5, (IConnector7)(object)((val8 is IConnector7) ? val8 : null)))
													{
														flag4 = true;
														break;
													}
												}
											}
										}
									}
									if (!flag4)
									{
										try
										{
											arrayList.Clear();
											val7 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val7.ProjectHandle, val7.ObjectGuid);
											if (val7.Object is DeviceObject)
											{
												DeviceObject deviceObject5 = val7.Object as DeviceObject;
												arrayList.AddRange(deviceObject5.RawConnectorList);
											}
											if (val7.Object is ExplicitConnector)
											{
												ExplicitConnector value2 = val7.Object as ExplicitConnector;
												arrayList.Add(value2);
											}
											if (val7.Object is SlotDeviceObject)
											{
												DeviceObject deviceObject6 = (val7.Object as SlotDeviceObject).GetDeviceObject() as DeviceObject;
												if (deviceObject6 != null)
												{
													arrayList.AddRange(deviceObject6.RawConnectorList);
												}
											}
											int num = 0;
											foreach (Connector item6 in arrayList)
											{
												if ((int)item6.ConnectorRole != 0)
												{
													continue;
												}
												foreach (IAdapter item7 in (IEnumerable)item6.Adapters)
												{
													IAdapter val9 = item7;
													int num2 = 0;
													Guid[] modules = val9.Modules;
													for (int k = 0; k < modules.Length; k++)
													{
														if (modules[k] == guidDevice)
														{
															num = num2;
															if (val9 is SlotAdapter)
															{
																(val9 as SlotAdapter).RemoveDevice(guidDevice);
															}
															if (val9 is FixedAdapter)
															{
																(val9 as FixedAdapter).RemoveDevice(guidDevice);
															}
															if (val9 is VarAdapter)
															{
																(val9 as VarAdapter).Remove(guidDevice);
															}
															break;
														}
														num2++;
													}
												}
											}
											foreach (Connector item8 in arrayList)
											{
												if ((int)item8.ConnectorRole != 0)
												{
													continue;
												}
												foreach (Connector item9 in (IEnumerable)val2)
												{
													if ((int)item9.ConnectorRole != 1 || !DeviceManager.CheckMatchInterface((IConnector7)(object)item9, (IConnector7)(object)item8))
													{
														continue;
													}
													foreach (IAdapter item10 in (IEnumerable)item8.Adapters)
													{
														IAdapter val10 = item10;
														if (val10 is IAdapterBase && (val10 as IAdapterBase).CanAddModule(num, val3.Object))
														{
															IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val3.ProjectHandle, val3.ObjectGuid);
															(val10 as IAdapterBase).AddModule(num, metaObjectStub);
															num = -1;
															break;
														}
													}
													if (num < 0)
													{
														break;
													}
												}
												if (num < 0)
												{
													break;
												}
											}
										}
										finally
										{
											if (val7 != null && val7.IsToModify)
											{
												((IObjectManager)APEnvironment.ObjectMgr).SetObject(val7, true, (object)null);
											}
										}
									}
								}
								CheckUpdateConstraints(deviceObject, val3.ParentObjectGuid, bInRecursion);
							}
							deviceObject.OnAfterAdded();
							((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_nProjectHandle, guidDevice);
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
							foreach (Guid item11 in val5)
							{
								RemoveObjects(item11, deviceId);
							}
							try
							{
								val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidDevice);
								if (val3 != null)
								{
									DeviceObject.UpdateChildObjects((DeviceObject)(object)val3.Object, bUpdate: true, bVersionUpgrade, val);
								}
								_replacedObjects.Add(new ReplacedObject(guidDevice, deviceObject2, deviceObject));
							}
							catch
							{
							}
							if (flag3 && val3.ParentObjectGuid != Guid.Empty)
							{
								IMetaObject val11 = null;
								try
								{
									val11 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val3.ProjectHandle, val3.ParentObjectGuid);
									ArrayList arrayList2 = new ArrayList();
									if (val11.Object is DeviceObject)
									{
										DeviceObject deviceObject7 = val11.Object as DeviceObject;
										arrayList2.AddRange(deviceObject7.RawConnectorList);
									}
									if (val11.Object is ExplicitConnector)
									{
										ExplicitConnector value3 = val11.Object as ExplicitConnector;
										arrayList2.Add(value3);
									}
									if (val11.Object is SlotDeviceObject)
									{
										DeviceObject deviceObject8 = (val11.Object as SlotDeviceObject).GetDeviceObject() as DeviceObject;
										if (deviceObject8 != null)
										{
											arrayList2.AddRange(deviceObject8.RawConnectorList);
										}
									}
									foreach (Connector item12 in arrayList2)
									{
										foreach (IAdapter item13 in (IEnumerable)item12.Adapters)
										{
											Guid[] modules = item13.Modules;
											for (int k = 0; k < modules.Length; k++)
											{
												if (!(modules[k] == val3.ObjectGuid))
												{
													continue;
												}
												ParameterSet parameterSet = item12.ParameterSet as ParameterSet;
												LList<long> val12 = new LList<long>();
												for (int l = 0; l < parameterSet.Count; l++)
												{
													Parameter parameter = parameterSet[l] as Parameter;
													if (parameter.CreateInHostConnector)
													{
														val12.Add(parameter.Id);
													}
												}
												foreach (IConnector item14 in (IEnumerable)deviceObject.Connectors)
												{
													foreach (Parameter item15 in (IEnumerable)item14.HostParameterSet)
													{
														if (!item15.CreateInHostConnector)
														{
															continue;
														}
														val12.Remove(item15.Id);
														if (!parameterSet.Contains(item15.Id))
														{
															if (item15.Section is ParameterSection)
															{
																ParameterSection section = item15.Section as ParameterSection;
																if (!parameterSet.ContainsParameterSection(section))
																{
																	parameterSet.AddParameterSection(section, -1);
																}
															}
															parameterSet.AddParameter(item15);
														}
														else
														{
															(parameterSet.GetParameter(item15.Id) as Parameter)?.Merge(item15);
														}
													}
												}
												foreach (long item16 in val12)
												{
													parameterSet.RemoveParameter(item16);
												}
												break;
											}
										}
									}
								}
								catch
								{
								}
								finally
								{
									if (val11 != null && val11.IsToModify)
									{
										((IObjectManager)APEnvironment.ObjectMgr).SetObject(val11, true, (object)null);
									}
								}
							}
						}
						else if (val3.Object is SlotDeviceObject)
						{
							LList<Guid> val13 = new LList<Guid>();
							SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)val3.Object;
							SlotDeviceObject slotDeviceObject2 = (SlotDeviceObject)((GenericObject)slotDeviceObject).Clone();
							val4 = slotDeviceObject2.DeviceIdentificationNoSimulation;
							CheckFdtConstraints((IDeviceObject)(object)slotDeviceObject, deviceId);
							DeviceObject device = slotDeviceObject.GetDevice();
							if (device != null)
							{
								device.CreateBitChannels = DeviceObjectHelper.CreateBitChannels(_nProjectHandle, guidDevice);
								device.SetDeviceIdentification(deviceId, bUpdate: true, val);
								val2 = device.Connectors;
								bool flag5 = false;
								for (int m = 0; m < ((ICollection)val2).Count; m++)
								{
									if (val2[m] is ErrorConnector)
									{
										flag5 = true;
									}
								}
								if (flag5)
								{
									object obj4 = ((GenericObject)(device.Connectors as ConnectorList)).Clone();
									val2 = (IConnectorCollection)((obj4 is IConnectorCollection) ? obj4 : null);
									for (int n = 0; n < ((ICollection)device.Connectors).Count; n++)
									{
										if (!(device.Connectors[n] is ErrorConnector))
										{
											continue;
										}
										foreach (IAdapter item17 in (IEnumerable)device.Connectors[n].Adapters)
										{
											IAdapter val14 = item17;
											val13.AddRange((IEnumerable<Guid>)val14.Modules);
										}
										device.RawConnectorList.Remove(device.Connectors[n]);
									}
								}
								if (val3.ParentObjectGuid != Guid.Empty)
								{
									CheckUpdateConstraints(slotDeviceObject, val3.ParentObjectGuid, bInRecursion);
								}
								slotDeviceObject.OnAfterAdded();
								((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_nProjectHandle, guidDevice);
							}
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
							foreach (Guid item18 in val13)
							{
								RemoveObjects(item18, deviceId);
							}
							try
							{
								val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidDevice);
								if (val3 != null)
								{
									DeviceObject.UpdateChildObjects((SlotDeviceObject)(object)val3.Object, bUpdate: true, bVersionUpgrade);
								}
								_replacedObjects.Add(new ReplacedObject(guidDevice, slotDeviceObject2, slotDeviceObject));
							}
							catch
							{
							}
						}
					}
				}
			}
			finally
			{
				try
				{
					if (val3 != null && val3.IsToModify)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, false, (object)null);
					}
					if (!flag)
					{
						if (_liUpdated != null)
						{
							DeviceEventArgs2 val15 = new DeviceEventArgs2(_nProjectHandle, guidDevice, deviceId, val4);
							_liUpdated.Add(val15);
						}
						else
						{
							APEnvironment.DeviceMgr.RaiseDeviceUpdated(_nProjectHandle, guidDevice, deviceId, val4);
						}
					}
				}
				finally
				{
					if (val3 != null && val3.Object is DeviceObject)
					{
						(val3.Object as DeviceObject).IsInUpdate = false;
					}
					if (val3 != null && val3.Object is SlotDeviceObject)
					{
						(val3.Object as SlotDeviceObject).IsInUpdate = false;
					}
					IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors();
					foreach (IEditor val16 in editors)
					{
						if (val16.ObjectGuid == val3.ObjectGuid && val16 is IView)
						{
							((IEngine)APEnvironment.Engine).Frame.UpdateCaption((IView)(object)((val16 is IView) ? val16 : null));
						}
					}
				}
			}
			if (bMustBeRemoved)
			{
				try
				{
					if (val3 != null)
					{
						RemoveObjects(guidDevice, deviceId);
					}
				}
				catch
				{
				}
			}
			if (val3 != null && !flag)
			{
				foreach (DeviceIdUpdate item19 in val)
				{
					if (item19.ObjectGuid != val3.ParentObjectGuid)
					{
						try
						{
							ReplaceObject(item19.ObjectGuid, item19.DeviceId, bInRecursion: true);
						}
						catch (Exception ex3)
						{
							DeviceMessage deviceMessage = new DeviceMessage(ex3.Message, (Severity)2);
							APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
						}
					}
				}
				val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidDevice);
				if (val3.Object is IDeviceObject)
				{
					val2 = ((IDeviceObject)val3.Object).Connectors;
				}
				Guid[] modules = val3.SubObjectGuids;
				foreach (Guid guid in modules)
				{
					bool flag6 = true;
					IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guid);
					if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType) && !typeof(SlotDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType) && !typeof(ExplicitConnector).IsAssignableFrom(metaObjectStub2.ObjectType))
					{
						XmlNode xmlNode = null;
						if (val3.Object is DeviceObject)
						{
							DeviceObject deviceObject9 = val3.Object as DeviceObject;
							if (deviceObject9 != null)
							{
								xmlNode = deviceObject9.GetFunctionalSection();
							}
						}
						if (val3.Object is SlotDeviceObject)
						{
							SlotDeviceObject slotDeviceObject3 = val3.Object as SlotDeviceObject;
							if (slotDeviceObject3 != null)
							{
								xmlNode = slotDeviceObject3.GetFunctionalSection();
							}
						}
						if (xmlNode != null)
						{
							ArrayList arrayList3 = new ArrayList();
							DeviceObject.GetImplicitObjects(arrayList3, xmlNode.ChildNodes, Guid.Empty);
							foreach (ChildObject item20 in arrayList3)
							{
								IObjectFactory factory = ((IObjectManager)APEnvironment.ObjectMgr).ObjectFactoryManager.GetFactory(item20.Guid);
								if (factory != null && factory.Namespace == metaObjectStub2.Namespace && factory.ObjectType == metaObjectStub2.ObjectType)
								{
									flag6 = false;
								}
							}
						}
						if (!flag6)
						{
							continue;
						}
					}
					foreach (DeviceIdUpdate item21 in val)
					{
						if (item21.ObjectGuid == guid)
						{
							flag6 = false;
						}
					}
					for (int num3 = 0; num3 < ((ICollection)val2).Count && flag6; num3++)
					{
						if (val2[num3] is Connector)
						{
							Connector connector5 = (Connector)(object)val2[num3];
							if (connector5.IsExplicit && connector5.ExplicitConnectorGuid == guid)
							{
								flag6 = false;
								break;
							}
							foreach (IAdapter item22 in (IEnumerable)connector5.Adapters)
							{
								Guid[] modules2 = item22.Modules;
								for (int num4 = 0; num4 < modules2.Length; num4++)
								{
									if (modules2[num4] == guid)
									{
										flag6 = false;
										break;
									}
								}
							}
						}
						if (!(val2[num3] is ErrorConnector))
						{
							continue;
						}
						foreach (IAdapter item23 in (IEnumerable)((ErrorConnector)(object)val2[num3]).Adapters)
						{
							Guid[] modules2 = item23.Modules;
							for (int num4 = 0; num4 < modules2.Length; num4++)
							{
								if (modules2[num4] == guid)
								{
									flag6 = true;
									break;
								}
							}
						}
					}
					if (typeof(ITraceObject).IsAssignableFrom(metaObjectStub2.ObjectType) || typeof(IExternalFileObject).IsAssignableFrom(metaObjectStub2.ObjectType))
					{
						flag6 = false;
					}
					if (flag6)
					{
						RemoveObjects(guid, deviceId);
					}
				}
				if (val3.Object is DeviceObject)
				{
					DeviceObject deviceObject10 = (DeviceObject)(object)val3.Object;
					try
					{
						if (val3.ParentObjectGuid != Guid.Empty)
						{
							if (!DeviceObjectHelper.AddListLanguageModel(val3.ProjectHandle, val3.ParentObjectGuid, bIsTaskLanguageModel: true))
							{
								DeviceObjectHelper.DeviceObjectHelper_TaskConfigChanged(deviceObject10, new CompileEventArgs(val3.ParentObjectGuid));
							}
						}
						else
						{
							IMetaObject application = deviceObject10.GetApplication();
							if (application != null)
							{
								IMetaObject taskConfig = DeviceObjectHelper.GetTaskConfig(application);
								if (taskConfig != null && taskConfig.Object is ITaskConfigObject)
								{
									ITaskConfigObject val17 = (ITaskConfigObject)taskConfig.Object;
									((ILanguageModelManager)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)val17, true);
								}
								if (!DeviceObjectHelper.AddListLanguageModel(val3.ProjectHandle, val3.ObjectGuid, bIsTaskLanguageModel: true))
								{
									DeviceObjectHelper.DeviceObjectHelper_TaskConfigChanged(deviceObject10, new CompileEventArgs(val3.ObjectGuid));
								}
							}
						}
					}
					catch
					{
					}
					LList<IDeviceIdentification> updateDevices = deviceObject10.GetUpdateDevices();
					if (updateDevices.Count > 0)
					{
						UpdateOtherDevices(updateDevices, deviceObject10.MetaObject);
					}
				}
				if (!bInRecursion && (val3.Object is DeviceObject || val3.Object is SlotDeviceObject))
				{
					DeviceObject deviceObject11 = null;
					if (val3.Object is DeviceObject)
					{
						deviceObject11 = val3.Object as DeviceObject;
					}
					if (val3.Object is SlotDeviceObject)
					{
						deviceObject11 = (val3.Object as SlotDeviceObject).GetDevice();
					}
					if (deviceObject11 != null)
					{
						ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceObject11.DeviceIdentificationNoSimulation);
						if (LocalTargetSettings.UpdateAllToEqualVersion.GetBoolValue(targetSettingsById))
						{
							if (!flag)
							{
								APEnvironment.MessageStorage.ClearMessages((IMessageCategory)(object)DeviceMessageCategory.Instance);
								DeviceMessage deviceMessage2 = new DeviceMessage(string.Format(Strings.InfoDeviceUpdated, deviceObject11.MetaObject.Name), (Severity)8);
								APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage2);
							}
							UpdateToSameversion(deviceObject11.MetaObject, deviceObject11.DeviceIdentificationNoSimulation.Version);
						}
					}
				}
			}
			return flag;
		}

		internal void RemoveObjects(Guid objectGuid, IDeviceIdentification devId)
		{
			try
			{
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, objectGuid))
				{
					DeviceManager.OnUpdateDeviceRemoving(this, _nProjectHandle, objectGuid, devId);
					((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(_nProjectHandle, objectGuid);
				}
			}
			catch
			{
			}
		}

		internal void UpdateToSameversion(IMetaObject parentDevice, string stNewVersion)
		{
			if (parentDevice == null)
			{
				return;
			}
			Guid[] subObjectGuids = parentDevice.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guid);
				if (metaObjectStub == null || !typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					continue;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guid);
				if (objectToRead == null)
				{
					continue;
				}
				IDeviceObject val = null;
				if (objectToRead.Object is ExplicitConnector)
				{
					(objectToRead.Object as ExplicitConnector).UpdateAddresses();
					UpdateToSameversion(objectToRead, stNewVersion);
					continue;
				}
				val = (IDeviceObject)((!(objectToRead.Object is ISlotDeviceObject)) ? (objectToRead.Object as IDeviceObject) : (objectToRead.Object as ISlotDeviceObject).GetDeviceObject());
				if (val == null)
				{
					continue;
				}
				IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((val is IDeviceObject5) ? val : null)).DeviceIdentificationNoSimulation;
				if (!(deviceIdentificationNoSimulation is ModuleIdentification))
				{
					DeviceIdentification deviceIdentification = new DeviceIdentification(deviceIdentificationNoSimulation);
					try
					{
						deviceIdentification.Version = stNewVersion;
						if (((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice((IDeviceIdentification)(object)deviceIdentification) != null)
						{
							bool num = ReplaceObject(guid, (IDeviceIdentification)(object)deviceIdentification, bInRecursion: true);
							objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guid);
							if (!num && objectToRead != null)
							{
								IObject @object = objectToRead.Object;
								val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
								DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.InfoDeviceUpdated, objectToRead.Name), (Severity)8);
								APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
							}
						}
						else
						{
							foreach (Connector item in (IEnumerable)val.Connectors)
							{
								item.UpdateAddresses();
							}
							DeviceMessage deviceMessage2 = new DeviceMessage(string.Format(Strings.InfoDeviceUpdateFailed, objectToRead.Name), (Severity)8);
							APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage2);
						}
					}
					catch (Exception ex)
					{
						DeviceMessage deviceMessage3 = new DeviceMessage(ex.Message, (Severity)2);
						APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage3);
					}
				}
				else
				{
					foreach (Connector item2 in (IEnumerable)val.Connectors)
					{
						item2.UpdateAddresses();
					}
				}
				UpdateToSameversion(objectToRead, stNewVersion);
			}
		}

		internal void UpdateOtherDevices(LList<IDeviceIdentification> liOtherDevices, IMetaObject parentDevice)
		{
			Guid[] subObjectGuids = parentDevice.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(parentDevice.ProjectHandle, guid);
				if (metaObjectStub == null || (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(ISlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType)))
				{
					continue;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guid);
				if (objectToRead == null)
				{
					continue;
				}
				IDeviceObject val = null;
				if (objectToRead.Object is IExplicitConnector)
				{
					UpdateOtherDevices(liOtherDevices, objectToRead);
					continue;
				}
				val = (IDeviceObject)((!(objectToRead.Object is ISlotDeviceObject)) ? (objectToRead.Object as IDeviceObject) : (objectToRead.Object as ISlotDeviceObject).GetDeviceObject());
				if (val != null)
				{
					foreach (IDeviceIdentification liOtherDevice in liOtherDevices)
					{
						IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((val is IDeviceObject5) ? val : null)).DeviceIdentificationNoSimulation;
						if (deviceIdentificationNoSimulation is DeviceIdentification && (deviceIdentificationNoSimulation as DeviceIdentification).Equals(liOtherDevice, bIgnoreVersion: true))
						{
							ReplaceObject(guid, liOtherDevice, bInRecursion: false);
						}
					}
				}
				UpdateOtherDevices(liOtherDevices, objectToRead);
			}
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
