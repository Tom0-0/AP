using System;
using System.Collections;
using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class HostpathUpdate
	{
		internal static void ChangeActiveChildConnector(int projectHandle, Guid deviceGuid, int connectorID)
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			Guid parentObjectGuid = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, deviceGuid).ParentObjectGuid;
			bool bCreateBitChannels = DeviceObjectHelper.CreateBitChannels(projectHandle, deviceGuid);
			IMetaObject val = null;
			bool flag = false;
			bool flag2 = false;
			LList<Guid> subGuids = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(projectHandle, deviceGuid);
				subGuids = new LList<Guid>((IEnumerable<Guid>)DeviceManager.GetOrderedSubGuids(val));
				IDeviceObjectBase deviceObjectBase = val.Object as IDeviceObjectBase;
				Connector connector = ConnectorSearch.Find(((IDeviceObject)deviceObjectBase).Connectors, (Connector match) => (int)match.ConnectorRole == 1 && match.ConnectorId == connectorID);
				if (connector == null)
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)16, "child connnector not found");
				}
				Connector oldChildConnector = DeviceManager.GetActiveChildConnector((IDeviceObject)deviceObjectBase);
				if (oldChildConnector != null && oldChildConnector.ConnectorId != connector.ConnectorId)
				{
					foreach (IAdapter item in (IEnumerable)oldChildConnector.Adapters)
					{
						IAdapter val2 = item;
						if (val2 is SlotAdapter)
						{
							(val2 as SlotAdapter).RemoveDevice(parentObjectGuid);
							break;
						}
					}
					foreach (IAdapter item2 in (IEnumerable)connector.Adapters)
					{
						IAdapterBase adapterBase = item2 as IAdapterBase;
						if (adapterBase != null)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, parentObjectGuid);
							if (adapterBase is SlotAdapter && (adapterBase as SlotAdapter).CanAddModule(0, metaObjectStub.ObjectType))
							{
								adapterBase.AddModule(0, metaObjectStub);
								break;
							}
						}
					}
					foreach (Connector item3 in ConnectorSearch.FindAll(((IDeviceObject)deviceObjectBase).Connectors, (Connector match) => (int)match.ConnectorRole == 0 && match.HostPath == oldChildConnector.ConnectorId))
					{
						ClearAdapters(item3);
					}
				}
				else
				{
					flag2 = true;
				}
				flag = true;
			}
			finally
			{
				if (val != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, flag, (object)null);
				}
			}
			if (!(flag || flag2))
			{
				return;
			}
			LList<Connector> obj = ConnectorSearch.FindAll(((IDeviceObject)(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, deviceGuid).Object as IDeviceObjectBase)).Connectors, (Connector match) => (int)match.ConnectorRole == 0 && DeviceManager.GetParentDeviceGuid(match.GetIoProviderParent() as Connector) != Guid.Empty);
			int nModuleOffset = 0;
			foreach (Connector item4 in obj)
			{
				if (item4.HostPath == connectorID)
				{
					UpdateChildObjects(projectHandle, deviceGuid, item4, ref nModuleOffset, subGuids, bCreateBitChannels);
					continue;
				}
				foreach (IAdapterBase item5 in (IEnumerable)item4.Adapters)
				{
					nModuleOffset += ((IAdapter)item5).ModulesCount;
				}
			}
		}

		internal static void UpdateChildObjects(int nProjectHandle, Guid guidDevice, Connector parentCon, ref int nModuleOffset, LList<Guid> subGuids, bool bCreateBitChannels)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			int num = nModuleOffset;
			foreach (IAdapter item in (IEnumerable)parentCon.Adapters)
			{
				IAdapter val = item;
				_=((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidDevice).Object;
				if (val is FixedAdapter)
				{
					FixedAdapter fixedAdapter = (FixedAdapter)(object)val;
					int num2 = 0;
					while (num2 < fixedAdapter.DefaultModules.Length)
					{
						DeviceIdentification deviceIdentification = fixedAdapter.DefaultModules[num2];
						if (subGuids.Count > nModuleOffset)
						{
							Guid guid = subGuids[nModuleOffset];
							IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
							IDeviceObjectBase deviceObjectBase = objectToRead.Object as IDeviceObjectBase;
							SlotDeviceObject slotDeviceObject = objectToRead.Object as SlotDeviceObject;
							if (slotDeviceObject != null && slotDeviceObject.HasDevice)
							{
								deviceObjectBase = slotDeviceObject.GetDevice();
							}
							if (slotDeviceObject != null)
							{
								((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(nProjectHandle, guid);
								subGuids.RemoveAt(nModuleOffset);
							}
							if (deviceObjectBase != null && slotDeviceObject == null)
							{
								Connector connector = FindMatchingChildConnector(objectToRead, parentCon);
								if (connector != null)
								{
									int nModuleOffset2 = nModuleOffset - num;
									UpdateParentConAdapter(nProjectHandle, guidDevice, parentCon.ConnectorId, nModuleOffset2, guid);
									ChangeActiveChildConnector(nProjectHandle, guid, connector.ConnectorId);
								}
							}
							else
							{
								DeviceObject deviceObject;
								try
								{
									deviceObject = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification);
								}
								catch (DeviceNotFoundException ex)
								{
									ex.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, parentCon.Interface);
									throw;
								}
								deviceObject.ConnectorIDForChild = parentCon.ConnectorId;
								string baseName = DeviceObjectHelper.GetBaseName(deviceIdentification.BaseName, DeviceObjectHelper.CreateInstanceNameBase(deviceObject.DeviceInfo));
								baseName = DeviceObjectHelper.BuildIecIdentifier(baseName);
								string text = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, baseName, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
								Guid guid2 = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)deviceObject, text, nModuleOffset - 1);
								DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guid2);
								subGuids.Insert(nModuleOffset, guid2);
								if (fixedAdapter.SubDevicesCollapsed)
								{
									INavigatorControl3 val2 = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null;
									if (val2 != null)
									{
										val2.Collapse(guid2);
									}
								}
							}
						}
						num2++;
						nModuleOffset++;
					}
				}
				else if (val is SlotAdapter)
				{
					SlotAdapter slotAdapter = (SlotAdapter)(object)val;
					int num3 = 0;
					while (num3 < slotAdapter.ModulesCount)
					{
						if (subGuids.Count > nModuleOffset)
						{
							Guid guid3 = subGuids[nModuleOffset];
							IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid3);
							DeviceObject deviceObject2 = objectToRead2.Object as DeviceObject;
							SlotDeviceObject slotDeviceObject2 = objectToRead2.Object as SlotDeviceObject;
							if (slotDeviceObject2 != null)
							{
								Connector connector2 = null;
								if (slotDeviceObject2.HasDevice)
								{
									deviceObject2 = slotDeviceObject2.GetDevice();
									connector2 = FindMatchingChildConnector(deviceObject2, parentCon);
								}
								SlotDeviceObject slotDeviceObject3 = ((!slotAdapter.HiddenSlot) ? new SlotDeviceObject(parentCon.Interface, parentCon.AdditionalInterfaces, slotAdapter.AllowEmptySlot) : new HiddenSlotDeviceObject(parentCon.Interface, parentCon.AdditionalInterfaces, slotAdapter.AllowEmptySlot));
								if (deviceObject2 != null && connector2 != null)
								{
									slotDeviceObject3.PlugDevice(deviceObject2);
									slotDeviceObject3.ConnectorIDForChild = parentCon.ConnectorId;
									IAdapterBase obj = (IAdapterBase)((Connector)(object)slotDeviceObject3.SlotConnectors[0]).Adapters[0];
									IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guidDevice);
									obj.AddModule(0, metaObjectStub);
									ReplaceMetaObject(nProjectHandle, guid3, (IDeviceObject)(object)slotDeviceObject3);
									int nModuleOffset3 = nModuleOffset - num;
									UpdateParentConAdapter(nProjectHandle, guidDevice, parentCon.ConnectorId, nModuleOffset3, guid3);
									ChangeActiveChildConnector(nProjectHandle, guid3, connector2.ConnectorId);
								}
								else
								{
									DeviceIdentification deviceIdentification2 = (DeviceIdentification)(object)slotAdapter.GetDefaultDevice(num3);
									if (deviceIdentification2 != null)
									{
										DeviceObject deviceObject3;
										try
										{
											deviceObject3 = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification2);
										}
										catch (DeviceNotFoundException ex2)
										{
											ex2.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, parentCon.Interface);
											throw;
										}
										string baseName2 = DeviceObjectHelper.GetBaseName(deviceIdentification2.BaseName, DeviceObjectHelper.CreateInstanceNameBase(deviceObject3.DeviceInfo));
										baseName2 = DeviceObjectHelper.BuildIecIdentifier(baseName2);
										DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, baseName2, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
										slotDeviceObject3.PlugDevice(deviceObject3);
									}
									else
									{
										string slotName = slotAdapter.GetSlotName(num3);
										if (!slotName.StartsWith("<"))
										{
											slotName = DeviceObjectHelper.BuildIecIdentifier(slotName);
											DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, slotName, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
										}
									}
									slotDeviceObject3.ConnectorIDForChild = parentCon.ConnectorId;
									IAdapterBase obj2 = (IAdapterBase)((Connector)(object)slotDeviceObject3.SlotConnectors[0]).Adapters[0];
									IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guidDevice);
									obj2.AddModule(0, metaObjectStub2);
									ReplaceMetaObject(nProjectHandle, guid3, (IDeviceObject)(object)slotDeviceObject3);
									int nModuleOffset4 = nModuleOffset - num;
									UpdateParentConAdapter(nProjectHandle, guidDevice, parentCon.ConnectorId, nModuleOffset4, guid3);
								}
							}
						}
						else
						{
							SlotDeviceObject slotDeviceObject4 = ((!slotAdapter.HiddenSlot) ? new SlotDeviceObject(parentCon.Interface, parentCon.AdditionalInterfaces, slotAdapter.AllowEmptySlot) : new HiddenSlotDeviceObject(parentCon.Interface, parentCon.AdditionalInterfaces, slotAdapter.AllowEmptySlot));
							DeviceIdentification deviceIdentification3 = (DeviceIdentification)(object)slotAdapter.GetDefaultDevice(num3);
							string text2;
							if (deviceIdentification3 != null)
							{
								DeviceObject deviceObject4;
								try
								{
									deviceObject4 = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification3);
								}
								catch (DeviceNotFoundException ex3)
								{
									ex3.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, parentCon.Interface);
									throw;
								}
								string baseName3 = DeviceObjectHelper.GetBaseName(deviceIdentification3.BaseName, DeviceObjectHelper.CreateInstanceNameBase(deviceObject4.DeviceInfo));
								baseName3 = DeviceObjectHelper.BuildIecIdentifier(baseName3);
								text2 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, baseName3, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
								slotDeviceObject4.PlugDevice(deviceObject4);
							}
							else
							{
								string slotName2 = slotAdapter.GetSlotName(num3);
								if (!slotName2.StartsWith("<"))
								{
									slotName2 = DeviceObjectHelper.BuildIecIdentifier(slotName2);
									text2 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, slotName2, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
								}
								else
								{
									text2 = slotName2;
								}
							}
							slotDeviceObject4.ConnectorIDForChild = parentCon.ConnectorId;
							Guid guid4 = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)slotDeviceObject4, text2, nModuleOffset);
							if (deviceIdentification3 != null)
							{
								DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guid4);
							}
							subGuids.Insert(nModuleOffset, guid4);
							if (slotAdapter.SubDevicesCollapsed)
							{
								INavigatorControl3 val3 = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null;
								if (val3 != null)
								{
									val3.Collapse(guid4);
								}
							}
						}
						num3++;
						nModuleOffset++;
					}
				}
				else
				{
					if (!(val is VarAdapter))
					{
						continue;
					}
					_ = (VarAdapter)(object)val;
					int num4 = subGuids.Count - nModuleOffset;
					int num5 = 0;
					while (num5 < num4)
					{
						Guid guid5 = subGuids[nModuleOffset];
						IMetaObject objectToRead3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid5);
						IDeviceObjectBase deviceObjectBase2 = objectToRead3.Object as IDeviceObjectBase;
						SlotDeviceObject slotDeviceObject5 = objectToRead3.Object as SlotDeviceObject;
						if (slotDeviceObject5 != null && slotDeviceObject5.HasDevice)
						{
							deviceObjectBase2 = slotDeviceObject5.GetDevice();
						}
						if (slotDeviceObject5 != null)
						{
							((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(nProjectHandle, guid5);
							subGuids.RemoveAt(nModuleOffset);
						}
						if (deviceObjectBase2 != null && slotDeviceObject5 == null)
						{
							Connector connector3 = FindMatchingChildConnector(objectToRead3, parentCon);
							int nModuleOffset5 = nModuleOffset - num;
							UpdateParentConAdapter(nProjectHandle, guidDevice, parentCon.ConnectorId, nModuleOffset5, guid5);
							ChangeActiveChildConnector(nProjectHandle, guid5, connector3.ConnectorId);
						}
						num5++;
						nModuleOffset++;
					}
				}
			}
		}

		private static void ReplaceMetaObject(int projectHandle, Guid childGuid, IDeviceObject device)
		{
			IMetaObject3 val = null;
			try
			{
				IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(projectHandle, childGuid);
				val = (IMetaObject3)(object)((objectToModify is IMetaObject3) ? objectToModify : null);
				if (val != null && ((IMetaObject)val).IsToModify)
				{
					((IMetaObject2)val).ReplaceObject((IObject)(object)device);
				}
			}
			finally
			{
				if (val != null && ((IMetaObject)val).IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject((IMetaObject)(object)val, true, (object)null);
				}
			}
		}

		private static void UpdateParentConAdapter(int nProjectHandle, Guid guidDevice, int connectorID, int nModuleOffset, Guid subDeviceGuid)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, guidDevice);
			try
			{
				if (objectToModify == null || !(objectToModify.Object is DeviceObject))
				{
					return;
				}
				DeviceObject deviceObject = objectToModify.Object as DeviceObject;
				foreach (IConnector item in (IEnumerable)deviceObject.Connectors)
				{
					IConnector val = item;
					if ((int)val.ConnectorRole == 1)
					{
						continue;
					}
					foreach (IAdapter item2 in (IEnumerable)val.Adapters)
					{
						IAdapter val2 = item2;
						Guid[] modules = val2.Modules;
						for (int i = 0; i < modules.Length; i++)
						{
							if (modules[i] == subDeviceGuid)
							{
								if (val2 is SlotAdapter)
								{
									(val2 as SlotAdapter).RemoveDevice(subDeviceGuid);
								}
								if (val2 is FixedAdapter)
								{
									(val2 as FixedAdapter).RemoveDevice(subDeviceGuid);
								}
								if (val2 is VarAdapter)
								{
									(val2 as VarAdapter).Remove(subDeviceGuid);
								}
								break;
							}
						}
					}
				}
				Connector connector = ConnectorSearch.Find(deviceObject.Connectors, ConnectorSearch.CheckID(connectorID));
				int num = 0;
				foreach (IAdapterBase item3 in (IEnumerable)connector.Adapters)
				{
					Guid[] allModuleGuids = GetAllModuleGuids((IAdapter)item3);
					if (item3 is VarAdapter)
					{
						(item3 as VarAdapter).Insert(nModuleOffset - num, subDeviceGuid);
						break;
					}
					if (nModuleOffset < num + allModuleGuids.Length)
					{
						int num2 = nModuleOffset - num;
						FixedAdapter fixedAdapter = item3 as FixedAdapter;
						if (fixedAdapter != null)
						{
							fixedAdapter.ModuleGuidsPlain[num2] = subDeviceGuid;
							break;
						}
						SlotAdapter slotAdapter = item3 as SlotAdapter;
						if (slotAdapter != null)
						{
							slotAdapter.ModuleGuidsPlain[num2] = subDeviceGuid;
							break;
						}
					}
					else
					{
						num += allModuleGuids.Length;
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

		private static Guid[] GetAllModuleGuids(IAdapter adapter)
		{
			Guid[] result = null;
			if (adapter is FixedAdapter)
			{
				result = (adapter as FixedAdapter).ModuleGuidsPlain;
			}
			else if (adapter is SlotAdapter)
			{
				result = (adapter as SlotAdapter).ModuleGuidsPlain;
			}
			else if (adapter is VarAdapter)
			{
				result = (adapter as VarAdapter).Modules;
			}
			return result;
		}

		private static Connector FindMatchingChildConnector(IMetaObject subMo, Connector parentCon)
		{
			DeviceObject subDevice = subMo.Object as DeviceObject;
			SlotDeviceObject slotDeviceObject = subMo.Object as SlotDeviceObject;
			if (slotDeviceObject != null && slotDeviceObject.HasDevice)
			{
				subDevice = slotDeviceObject.GetDevice();
			}
			return FindMatchingChildConnector(subDevice, parentCon);
		}

		private static Connector FindMatchingChildConnector(DeviceObject subDevice, Connector parentCon)
		{
			if (subDevice != null)
			{
				return ConnectorSearch.FindAll(subDevice.Connectors, ConnectorSearch.CheckRole((ConnectorRole)1)).Find((Predicate<Connector>)((Connector subDeviceCon) => DeviceManager.CheckMatchInterface((IConnector7)(object)parentCon, (IConnector7)(object)subDeviceCon)));
			}
			return null;
		}

		private static void ClearAdapters(Connector oldParentConnector)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			foreach (IAdapter item in (IEnumerable)oldParentConnector.Adapters)
			{
				IAdapter val = item;
				if (val is FixedAdapter)
				{
					FixedAdapter fixedAdapter = val as FixedAdapter;
					Guid[] moduleGuidsPlain = fixedAdapter.ModuleGuidsPlain;
					foreach (Guid guidChild in moduleGuidsPlain)
					{
						fixedAdapter.RemoveDevice(guidChild);
					}
				}
				else if (val is SlotAdapter)
				{
					SlotAdapter slotAdapter = val as SlotAdapter;
					Guid[] moduleGuidsPlain = slotAdapter.ModuleGuidsPlain;
					foreach (Guid guidChild2 in moduleGuidsPlain)
					{
						slotAdapter.RemoveDevice(guidChild2);
					}
				}
				else if (val is VarAdapter)
				{
					VarAdapter varAdapter = val as VarAdapter;
					Guid[] moduleGuidsPlain = varAdapter.Modules;
					foreach (Guid guidDevice in moduleGuidsPlain)
					{
						varAdapter.Remove(guidDevice);
					}
				}
			}
		}

		private static int CountModules(Connector parentCon)
		{
			int num = 0;
			foreach (IAdapterBase item in (IEnumerable)parentCon.Adapters)
			{
				num += ((IAdapter)item).ModulesCount;
			}
			return num;
		}
	}
}
