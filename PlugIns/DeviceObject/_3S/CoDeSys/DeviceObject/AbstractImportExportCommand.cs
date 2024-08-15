using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.NavigatorControl;

namespace _3S.CoDeSys.DeviceObject
{
	public abstract class AbstractImportExportCommand
	{
		public abstract string[] BatchCommand { get; }

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Name");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Description");

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public bool Enabled
		{
			get
			{
				if (((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl)
				{
					IMetaObjectStub selectedStub = DeviceCommandHelper.GetSelectedStub();
					if (selectedStub == null)
					{
						return false;
					}
					if (typeof(IDeviceObject).IsAssignableFrom(selectedStub.ObjectType))
					{
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedStub.ProjectHandle, selectedStub.ObjectGuid);
						IObject obj = ((objectToRead != null) ? objectToRead.Object : null);
						ILogicalDevice val = (ILogicalDevice)(object)((obj is ILogicalDevice) ? obj : null);
						if (val != null && val.IsLogical)
						{
							return false;
						}
						return true;
					}
					if (typeof(IExplicitConnector).IsAssignableFrom(selectedStub.ObjectType))
					{
						return true;
					}
				}
				return IsVisible(bContextMenu: true);
			}
		}

		public abstract string[] CreateBatchArguments(bool bInvokedByContextMenu);

		public string[] CreateBatchArguments()
		{
			return CreateBatchArguments(bInvokedByContextMenu: true);
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu && ((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl)
			{
				return Enabled;
			}
			if (((IEngine)APEnvironment.Engine).Frame != null && ((IEngine)APEnvironment.Engine).Frame.ActiveView != null && ((IEngine)APEnvironment.Engine).Frame.ActiveView.Control
				.ContainsFocus)
			{
				IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
				IExportImportCSVCommands val = (IExportImportCSVCommands)(object)((activeView is IExportImportCSVCommands) ? activeView : null);
				if (val != null)
				{
					return val.IsVisible;
				}
			}
			return false;
		}
	}
}
