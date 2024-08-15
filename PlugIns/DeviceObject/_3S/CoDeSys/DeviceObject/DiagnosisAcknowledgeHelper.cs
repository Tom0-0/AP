#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal static class DiagnosisAcknowledgeHelper
	{
		internal static IList<object> GetSelectedDevices()
		{
			LList<object> val = new LList<object>();
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return null;
			}
			ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length == 0)
			{
				return null;
			}
			ISVNode[] array = selectedSVNodes;
			for (int i = 0; i < array.Length; i++)
			{
				IMetaObjectStub metaObjectStub = array[i].GetMetaObjectStub();
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
					Debug.Assert(objectToRead != null);
					val.Add((object)objectToRead.Object);
				}
			}
			return (IList<object>)val;
		}

		internal static bool IsVisible(bool bContextMenu)
		{
			bool result = false;
			if (bContextMenu && ((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) != null)
			{
				IList<object> selectedDevices = DiagnosisAcknowledgeHelper.GetSelectedDevices();
				if (selectedDevices != null && selectedDevices.Count > 0)
				{
					IDeviceObject host = DiagnosisAcknowledgeHelper.GetHost(selectedDevices[0]);
					if (host != null)
					{
						result = !DeviceCommandHelper.IsHostOffline(host);
					}
				}
			}
			return result;
		}

		internal static IDeviceObject GetHost(object device)
		{
			DeviceObject deviceObject = device as DeviceObject;
			if (deviceObject != null)
			{
				return deviceObject.GetHostDeviceObject();
			}
			SlotDeviceObject slotDeviceObject = device as SlotDeviceObject;
			if (slotDeviceObject != null)
			{
				return slotDeviceObject.GetDevice()?.GetHostDeviceObject();
			}
			return (device as ExplicitConnector)?.GetHost();
		}

		internal static string[] CreateBatchArguments()
		{
			LList<string> val = new LList<string>();
			IList<object> selectedDevices = GetSelectedDevices();
			if (selectedDevices != null && selectedDevices.Count > 0)
			{
				foreach (object item in selectedDevices)
				{
					object obj = ((item is IDeviceObject) ? item : null);
					IMetaObject val2 = ((obj != null) ? ((IObject)obj).MetaObject : null);
					if (val2 == null)
					{
						object obj2 = ((item is IExplicitConnector) ? item : null);
						val2 = ((obj2 != null) ? ((IObject)obj2).MetaObject : null);
					}
					if (val2 != null)
					{
						val.Add(val2.ProjectHandle.ToString());
						val.Add(val2.ObjectGuid.ToString());
					}
				}
			}
			return val.ToArray();
		}

		internal static void ExecuteBatch(string[] arguments, bool bRecursive)
		{
			LDictionary<Guid, int> ldictionary = new LDictionary<Guid, int>();
			for (int i = 0; i < arguments.Length; i += 2)
			{
				int num;
				Guid guid;
				if (int.TryParse(arguments[i], out num) && Guid.TryParse(arguments[i + 1], out guid) && APEnvironment.ObjectMgr.ExistsObject(num, guid))
				{
					ldictionary.Add(guid, num);
				}
			}
			if (ldictionary.Count > 0)
			{
				if (bRecursive)
				{
					LDictionary<Guid, int> ldictionary2 = new LDictionary<Guid, int>();
					ldictionary2.AddRange(ldictionary);
					foreach (KeyValuePair<Guid, int> keyValuePair in ldictionary2)
					{
						DiagnosisAcknowledgeHelper.CollectDevices(keyValuePair.Value, keyValuePair.Key, ldictionary);
					}
				}
				IOnlineDevice2 onlineDevice = null;
				foreach (KeyValuePair<Guid, int> keyValuePair2 in ldictionary)
				{
					IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(keyValuePair2.Value, keyValuePair2.Key);
					IDeviceObject deviceObject = ((objectToRead != null) ? objectToRead.Object : null) as IDeviceObject;
					if (deviceObject == null)
					{
						ISlotDeviceObject slotDeviceObject = ((objectToRead != null) ? objectToRead.Object : null) as ISlotDeviceObject;
						if (slotDeviceObject != null)
						{
							deviceObject = slotDeviceObject.GetDeviceObject();
						}
					}
					if (deviceObject != null)
					{
						if (onlineDevice == null)
						{
							onlineDevice = DiagnosisAcknowledgeHelper.GetOnlineDevice(DiagnosisAcknowledgeHelper.GetHost(deviceObject));
						}
						DiagnosisAcknowledgeHelper.AcknowledgeDevice(deviceObject, onlineDevice);
					}
					else
					{
						ExplicitConnector explicitConnector = ((objectToRead != null) ? objectToRead.Object : null) as ExplicitConnector;
						if (explicitConnector != null)
						{
							if (onlineDevice == null)
							{
								onlineDevice = DiagnosisAcknowledgeHelper.GetOnlineDevice(explicitConnector.GetHost());
							}
							DiagnosisAcknowledgeHelper.AcknowledgeParameter(explicitConnector.HostParameterSet, onlineDevice);
						}
					}
				}
			}
		}

		internal static IOnlineDevice2 GetOnlineDevice(IDeviceObject host)
		{
			if (host != null)
			{
				try
				{
					IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)host).MetaObject.ObjectGuid);
					return (IOnlineDevice2)(object)((onlineDevice is IOnlineDevice2) ? onlineDevice : null);
				}
				catch
				{
				}
			}
			return null;
		}

		internal static void CollectDevices(int nProjectHandle, Guid objectGuid, LDictionary<Guid, int> dictDevices)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
			if (metaObjectStub != null)
			{
				if ((typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType)) && !dictDevices.ContainsKey(metaObjectStub.ObjectGuid))
				{
					dictDevices.Add(metaObjectStub.ObjectGuid, metaObjectStub.ProjectHandle);
				}
				Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
				foreach (Guid objectGuid2 in subObjectGuids)
				{
					CollectDevices(nProjectHandle, objectGuid2, dictDevices);
				}
			}
		}

		internal static void AcknowledgeDevice(IDeviceObject device, IOnlineDevice2 onlineDevice)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (device == null)
			{
				return;
			}
			AcknowledgeParameter(device.DeviceParameterSet, onlineDevice);
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				AcknowledgeParameter(item.HostParameterSet, onlineDevice);
			}
		}

		internal static void AcknowledgeParameter(IParameterSet parameterSet, IOnlineDevice2 onlineDevice)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			foreach (IParameter2 item in (IEnumerable)parameterSet)
			{
				IParameter2 val = item;
				if ((int)val.DiagType != 2 || !(val is IDataElement2) || !((IDataElement2)val).CanWatch)
				{
					continue;
				}
				IOnlineVarRef val2 = ((IDataElement2)val).CreateWatch();
				if (val2 == null || onlineDevice == null)
				{
					break;
				}
				val2.PreparedValue=((object)true);
				IOnlineVarRef[] preparedVarRefs = onlineDevice.PreparedVarRefs;
				if (preparedVarRefs == null)
				{
					throw new ArgumentNullException("prepared");
				}
				IOnlineVarRef[] array = preparedVarRefs;
				foreach (IOnlineVarRef val3 in array)
				{
					if (((object)val3.Expression).Equals((object)val2.Expression))
					{
						onlineDevice.WriteVariables((IOnlineVarRef[])(object)new IOnlineVarRef[1] { val3 });
						break;
					}
				}
				break;
			}
		}
	}
}
