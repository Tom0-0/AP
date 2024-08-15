#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.FdtIntegration;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceCommandHelper
	{
		public static readonly Guid GUID_DEVICECMDCATEGORY = new Guid("61ACA6D6-F298-47be-972E-99601E7E9410");

		internal static readonly Guid STRUCTURED_VIEW_DEVICES_GUID = new Guid("{D9B2B2CC-EA99-4c3b-AA42-1E5C49E65B84}");

		public static bool IsOffline => IsHostOffline(GetHostFromSelectedDevice());

		public static bool IsHostOffline(IDeviceObject host)
		{
			if (host != null)
			{
				IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)host).MetaObject.ObjectGuid);
				if (onlineDevice != null && onlineDevice.IsConnected)
				{
					IOnlineApplication[] loggedInApplications = APEnvironment.OnlineMgr.GetLoggedInApplications(false);
					if (loggedInApplications.Length != 0)
					{
						IOnlineApplication[] array = loggedInApplications;
						foreach (IOnlineApplication val in array)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)host).MetaObject.ProjectHandle, val.ApplicationGuid);
							if (!typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType) && DeviceObjectHelper.GetHostStub(((IObject)host).MetaObject.ProjectHandle, val.ApplicationGuid).ObjectGuid == ((IObject)host).MetaObject.ObjectGuid)
							{
								Guid guid = DeviceObjectHelper.ConfigModeApplication(((IObject)host).MetaObject.ObjectGuid);
								if (guid == Guid.Empty || guid != val.ApplicationGuid)
								{
									return false;
								}
							}
						}
					}
				}
			}
			return true;
		}

		public static IDeviceObject GetHostFromSelectedDevice()
		{
			IList<IDeviceObject> selectedDevices = GetSelectedDevices();
			if (selectedDevices != null)
			{
				foreach (IDeviceObject item in selectedDevices)
				{
					if (item is DeviceObject)
					{
						return (item as DeviceObject).GetHostDeviceObject();
					}
					if (((ICollection)item.Connectors).Count <= 0)
					{
						continue;
					}
					IConnector obj = item.Connectors[0];
					IConnector3 val = (IConnector3)(object)((obj is IConnector3) ? obj : null);
					if (val != null)
					{
						IDeviceObject host = val.GetHost();
						if (host != null)
						{
							return host;
						}
					}
				}
			}
			return null;
		}

		public static IMetaObjectStub GetSelectedStub()
		{
			return GetSelectedStub(bAll: false);
		}

		public static IMetaObjectStub GetSelectedStub(bool bAll)
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return null;
			}
			ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length != 1)
			{
				return null;
			}
			ISVNode val = selectedSVNodes[0];
			while (val.IsFolder && val.Parent != null)
			{
				val = val.Parent;
			}
			IMetaObjectStub metaObjectStub = val.GetMetaObjectStub();
			if (bAll)
			{
				return metaObjectStub;
			}
			if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				return metaObjectStub;
			}
			return null;
		}

		public static IMetaObjectStub GetHostFromSelectedStub()
		{
			IMetaObjectStub selectedStub = GetSelectedStub(bAll: true);
			if (selectedStub == null)
			{
				return null;
			}
			return DeviceObjectHelper.GetHostStub(selectedStub.ProjectHandle, selectedStub.ObjectGuid);
		}

		public static IDeviceObject GetSelectedDevice()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return null;
			}
			ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length != 1)
			{
				return null;
			}
			IMetaObjectStub metaObjectStub = selectedSVNodes[0].GetMetaObjectStub();
			if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
				Debug.Assert(objectToRead != null);
				return (IDeviceObject)objectToRead.Object;
			}
			return null;
		}

		public static IList<IDeviceObject> GetSelectedDevices()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Expected O, but got Unknown
			LList<IDeviceObject> val = new LList<IDeviceObject>();
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
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
					Debug.Assert(objectToRead != null);
					val.Add((IDeviceObject)objectToRead.Object);
				}
				if (typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
					Debug.Assert(objectToRead2 != null);
					val.Add((IDeviceObject)objectToRead2.Object);
				}
			}
			return (IList<IDeviceObject>)val;
		}

		public static IMetaObjectStub GetSelectedObjectsParent(out int nObjectIndex)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			nObjectIndex = -1;
			if (primaryProject == null)
			{
				return null;
			}
			ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length != 1)
			{
				return null;
			}
			IMetaObjectStub metaObjectStub = selectedSVNodes[0].GetMetaObjectStub();
			if (metaObjectStub.ParentObjectGuid == Guid.Empty)
			{
				return null;
			}
			IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
			Debug.Assert(metaObjectStub2 != null);
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
			if (objectToRead.Object is IDeviceObjectBase)
			{
				nObjectIndex = ((IOrderedSubObjects)(IDeviceObjectBase)objectToRead.Object).GetChildIndex(metaObjectStub.ObjectGuid);
				return metaObjectStub2;
			}
			if (objectToRead.Object is ExplicitConnector)
			{
				nObjectIndex = ((ExplicitConnector)(object)objectToRead.Object).GetChildIndex(metaObjectStub.ObjectGuid);
				return metaObjectStub2;
			}
			nObjectIndex = -1;
			return metaObjectStub2;
		}

		public static void UnplugDeviceFromSlot(int nProjectHandle, Guid guidSlot, bool bCheckBeforeUnplug)
		{
			bool flag = false;
			SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidSlot).Object;
			if (!slotDeviceObject.HasDevice || (bCheckBeforeUnplug && !slotDeviceObject.AllowEmpty))
			{
				return;
			}
			IFdtIntegration2 fdtIntegrationOrNull = APEnvironment.FdtIntegrationOrNull;
			if (fdtIntegrationOrNull != null && !fdtIntegrationOrNull.ValidateSlotUnplugging(nProjectHandle, guidSlot))
			{
				return;
			}
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(nProjectHandle);
			try
			{
				undoManager.BeginCompoundAction("Unplug device");
				APEnvironment.DeviceMgr.RaiseDevicePlugging(nProjectHandle, guidSlot, null);
				DeviceObject device = (DeviceObject)((GenericObject)slotDeviceObject.GetDevice()).Clone();
				slotDeviceObject.BeforeUnplugDevice();
				UnplugSlotAction unplugSlotAction = new UnplugSlotAction(nProjectHandle, guidSlot, device);
				unplugSlotAction.Redo();
				undoManager.AddAction((IUndoableAction)(object)unplugSlotAction);
				APEnvironment.DeviceMgr.RaiseDevicePlugged(nProjectHandle, guidSlot, null);
				if (fdtIntegrationOrNull != null)
				{
					fdtIntegrationOrNull.NotifySlotUnplugged(nProjectHandle, guidSlot);
				}
				flag = true;
			}
			catch
			{
				throw;
			}
			finally
			{
				undoManager.EndCompoundAction();
				if (!flag && !undoManager.InCompoundAction)
				{
					undoManager.Undo();
				}
			}
		}

		internal static void PlugDeviceIntoSlot(int nProjectHandle, Guid guidSlot, IDeviceIdentification deviceId, string stName)
		{
			bool bCreateBitChannels = DeviceObjectHelper.CreateBitChannels(nProjectHandle, guidSlot);
			PlugDeviceIntoSlot(nProjectHandle, guidSlot, new DeviceObject(bCreateBitChannels, deviceId), stName);
		}

		internal static void PlugDeviceIntoSlot(int nProjectHandle, Guid guidSlot, DeviceObject device, string stName)
		{
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Expected O, but got Unknown
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
				UnplugDeviceFromSlot(nProjectHandle, guidSlot, bCheckBeforeUnplug: false);
				PlugSlotAction plugSlotAction = new PlugSlotAction(nProjectHandle, guidSlot, device, stName);
				plugSlotAction.Redo();
				undoManager.AddAction((IUndoableAction)(object)plugSlotAction);
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidSlot);
				((SlotDeviceObject)(object)objectToRead.Object).AfterPlugDevice();
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

		internal static void RefillNavigator(int nProjectHandle, Guid[] objectGuids)
		{
			if (((IEngine)APEnvironment.Engine).Frame != null)
			{
				IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews();
				if (views != null)
				{
					IView[] array = views;
					foreach (IView val in array)
					{
						if (val is INavigatorControl)
						{
							((INavigatorControl)((val is INavigatorControl) ? val : null)).UpdateNodes(objectGuids);
						}
					}
				}
			}
			foreach (Guid guid in objectGuids)
			{
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, guid))
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
					RefillNavigator(nProjectHandle, metaObjectStub.SubObjectGuids);
				}
			}
		}

		internal static void RefillNavigator(Guid structuredViewGuid)
		{
			if (((IEngine)APEnvironment.Engine).Frame == null)
			{
				return;
			}
			IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews();
			if (views == null)
			{
				return;
			}
			IView[] array = views;
			foreach (IView obj in array)
			{
				INavigatorControl val = (INavigatorControl)(object)((obj is INavigatorControl) ? obj : null);
				if (val != null && val.StructuredViewGuid == structuredViewGuid)
				{
					val.Refill();
				}
			}
		}

		public static bool IsUpdateForLogicalDevicesEnabled(IDeviceObject device)
		{
			ILogicalDeviceCommandFactory factory = LogicalDeviceCommandFactoryManager.Instance.GetFactory(((IObject)device).MetaObject);
			if (factory != null && factory.CanUpdateLogicalDevice((ILogicalDevice)(object)((device is ILogicalDevice) ? device : null)))
			{
				return true;
			}
			return false;
		}
	}
}
