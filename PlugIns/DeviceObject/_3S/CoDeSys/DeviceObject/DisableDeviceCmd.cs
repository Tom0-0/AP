#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{53bb412d-8007-4439-9af9-5708da68aa48}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_disable_device.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceObject.Devices.chm::/Disable_Device.htm")]
	public class DisableDeviceCmd : IStandardCommand, ICommand
	{
		private static readonly string[] ARGUMENT_TYPES = new string[3] { "int", "Guid", "bool" };

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DisableCmdDescription");

		public string Name
		{
			get
			{
				string text = "DisableCmdName";
				IList<IDeviceObject> selectedDevices = DeviceCommandHelper.GetSelectedDevices();
				if (selectedDevices != null)
				{
					foreach (IDeviceObject item in selectedDevices)
					{
						if (item != null && item.Disable)
						{
							text = "EnableCmdName";
						}
					}
				}
				return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), text);
			}
		}

		public bool Enabled
		{
			get
			{
				if (DeviceCommandHelper.IsOffline)
				{
					IList<IDeviceObject> selectedDevices = DeviceCommandHelper.GetSelectedDevices();
					if (selectedDevices != null)
					{
						bool flag = false;
						foreach (IDeviceObject item in selectedDevices)
						{
							IDeviceObject parentDeviceObject = DeviceObjectHelper.GetParentDeviceObject(item);
							IDeviceObject13 val = (IDeviceObject13)(object)((parentDeviceObject is IDeviceObject13) ? parentDeviceObject : null);
							if (val != null)
							{
								flag = val.InheritedDisable;
							}
						}
						return !flag;
					}
					return DeviceCommandHelper.IsOffline;
				}
				return false;
			}
		}

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string[] BatchCommand => null;

		public string[] CreateBatchArguments()
		{
			IList<IDeviceObject> selectedDevices = DeviceCommandHelper.GetSelectedDevices();
			LList<string> val = new LList<string>();
			foreach (IDeviceObject item in selectedDevices)
			{
				Debug.Assert(item != null);
				IMetaObject metaObject = ((IObject)item).MetaObject;
				val.Add(metaObject.ProjectHandle.ToString());
				val.Add(metaObject.ObjectGuid.ToString());
				val.Add((!item.Disable).ToString());
			}
			string[] array = new string[val.Count];
			val.CopyTo(array);
			return array;
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (!bContextMenu)
			{
				return false;
			}
			if (((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) == null)
			{
				return false;
			}
			IList<IDeviceObject> selectedDevices = DeviceCommandHelper.GetSelectedDevices();
			if (selectedDevices != null)
			{
				foreach (IDeviceObject item in selectedDevices)
				{
					IHideDeviceDisableFactory factory = DeviceDisableFactoryManager.Instance.GetFactory(((IObject)item).MetaObject);
					if (factory != null && factory.HideDisableDevice(((IObject)item).MetaObject))
					{
						return false;
					}
					if (item is ILogicalDevice && ((ILogicalDevice)((item is ILogicalDevice) ? item : null)).IsLogical)
					{
						return false;
					}
					if (item.AllowsDirectCommunication)
					{
						return false;
					}
				}
				return (selectedDevices.Count > 0) & DeviceCommandHelper.IsOffline;
			}
			return false;
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			LList<Guid> val = new LList<Guid>();
			int num2 = 0;
			try
			{
				while (num < arguments.Length)
				{
					IMetaObject val2 = null;
					num2 = int.Parse(arguments[num]);
					num++;
					Guid guid = new Guid(arguments[num]);
					num++;
					bool disable = DeviceObjectHelper.ParseBool(arguments[num], bDefault: true);
					num++;
					try
					{
						val2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(num2, guid);
						((IDeviceObject)val2.Object).Disable=(disable);
						val.Add(guid);
					}
					finally
					{
						if (val2 != null && val2.IsToModify)
						{
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val2, true, (object)null);
						}
					}
				}
			}
			catch
			{
				throw new BatchWrongArgumentTypeException(BatchCommand, num, ARGUMENT_TYPES[num]);
			}
			finally
			{
				Guid[] array = new Guid[val.Count];
				val.CopyTo(array);
				DeviceCommandHelper.RefillNavigator(num2, array);
			}
		}
	}
}
