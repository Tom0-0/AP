#define DEBUG
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{99c26731-71f6-42cd-8184-434991a5bbc2}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_insert_device.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceObject.Devices.chm::/Insert_Device.htm")]
	public class InsertDeviceCmd : AbstractDeviceCmd, IStandardCommand2, IStandardCommand, ICommand, IInteractiveDeviceCommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "insert" };

		public bool Enabled
		{
			get
			{
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				string[] array = null;
				int nObjectIndex;
				IMetaObjectStub selectedObjectsParent = DeviceCommandHelper.GetSelectedObjectsParent(out nObjectIndex);
				if (selectedObjectsParent == null)
				{
					return false;
				}
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedObjectsParent.ProjectHandle, selectedObjectsParent.ObjectGuid);
				Debug.Assert(objectToRead != null);
				if (typeof(IDeviceObject).IsAssignableFrom(selectedObjectsParent.ObjectType))
				{
					array = ((IDeviceObject)objectToRead.Object).GetPossibleInterfacesForInsert(nObjectIndex);
				}
				else if (typeof(IExplicitConnector).IsAssignableFrom(selectedObjectsParent.ObjectType))
				{
					array = ((IExplicitConnector)objectToRead.Object).GetPossibleInterfacesForInsert(nObjectIndex);
				}
				if (array != null)
				{
					return array.Length != 0;
				}
				return false;
			}
		}

		public override string[] BatchCommand => BATCH_COMMAND;

		public bool Selectable => AbstractDeviceCmd.IsSelectable((ICommand)(object)this);

		public bool IsVisible(bool bContextMenu)
		{
			return bContextMenu && ((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) != null && (this.Enabled & DeviceCommandHelper.IsOffline);
		}

		public void ExecuteBatch(string[] arguments)
		{
			if (arguments != null && arguments.Length == 1 && arguments[0] == "<interactive>")
			{
				if (!AddDeviceFormEx.OneInstanceVisible)
				{
					AddDeviceFormEx addDeviceFormEx = new AddDeviceFormEx();
					addDeviceFormEx.Initialize(InitialAddDeviceMode.Insert);
					addDeviceFormEx.Show((IWin32Window)APEnvironment.FrameForm);
				}
				else
				{
					AddDeviceFormEx.GetInstance?.Initialize(InitialAddDeviceMode.Insert);
				}
			}
			else
			{
				ExecuteBatchNonInteractive(arguments);
			}
		}

		public string[] CreateBatchArguments(IDeviceIdentification deviceId, string stName, params object[] list)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			int nObjectIndex;
			IMetaObjectStub selectedObjectsParent = DeviceCommandHelper.GetSelectedObjectsParent(out nObjectIndex);
			if (selectedObjectsParent == null)
			{
				return null;
			}
			if (!(deviceId is IModuleIdentification))
			{
				return new string[7]
				{
					selectedObjectsParent.ProjectHandle.ToString(),
					selectedObjectsParent.ObjectGuid.ToString(),
					nObjectIndex.ToString(),
					deviceId.Type.ToString(),
					deviceId.Id,
					deviceId.Version,
					stName
				};
			}
			return new string[8]
			{
				selectedObjectsParent.ProjectHandle.ToString(),
				selectedObjectsParent.ObjectGuid.ToString(),
				nObjectIndex.ToString(),
				deviceId.Type.ToString(),
				deviceId.Id,
				deviceId.Version,
				((IModuleIdentification)deviceId).ModuleId,
				stName
			};
		}

		public string[] CreateBatchArguments(bool bInvokedByContextMenu)
		{
			return new string[1] { "<interactive>" };
		}

		public string[] CreateBatchArguments()
		{
			return CreateBatchArguments(bInvokedByContextMenu: true);
		}

		public bool GetFilterAndContext(out string[] filter, out IDeviceIdentification context, out IDeviceIdentification[] AllowOnlyDevices)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			filter = null;
			context = null;
			AllowOnlyDevices = null;
			ArrayList arrayList = new ArrayList();
			if (!Enabled)
			{
				return false;
			}
			int nObjectIndex;
			IMetaObjectStub selectedObjectsParent = DeviceCommandHelper.GetSelectedObjectsParent(out nObjectIndex);
			if (selectedObjectsParent == null)
			{
				return false;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedObjectsParent.ProjectHandle, selectedObjectsParent.ObjectGuid);
			Debug.Assert(objectToRead != null);
			if (objectToRead.Object is IDeviceObject)
			{
				filter = ((IDeviceObject)objectToRead.Object).GetPossibleInterfacesForInsert(nObjectIndex);
				context = ((IDeviceObject5)objectToRead.Object).DeviceIdentificationNoSimulation;
				IObject @object = objectToRead.Object;
				foreach (IConnector8 item in (IEnumerable)((IDeviceObject)((@object is IDeviceObject) ? @object : null)).Connectors)
				{
					IConnector8 val = item;
					arrayList.AddRange(val.AllowOnlyDevices);
				}
				filter = AbstractDeviceCmd.CheckConstraints(objectToRead, filter, bCheckParent: true, bUpdate: false);
			}
			else
			{
				if (!typeof(IExplicitConnector).IsAssignableFrom(selectedObjectsParent.ObjectType))
				{
					return false;
				}
				filter = ((IExplicitConnector)objectToRead.Object).GetPossibleInterfacesForInsert(nObjectIndex);
				IDeviceObject deviceObject = ((IConnector)objectToRead.Object).GetDeviceObject();
				if (deviceObject != null)
				{
					context = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
				}
				if (objectToRead.Object is IConnector8)
				{
					IObject object2 = objectToRead.Object;
					arrayList.AddRange(((IConnector8)((object2 is IConnector8) ? object2 : null)).AllowOnlyDevices);
				}
				filter = AbstractDeviceCmd.CheckConstraints(objectToRead, filter, bCheckParent: true, bUpdate: false);
			}
			if (filter == null)
			{
				return false;
			}
			AllowOnlyDevices = (IDeviceIdentification[])(object)new IDeviceIdentification[arrayList.Count];
			arrayList.CopyTo(AllowOnlyDevices);
			return true;
		}

		public string GetFixObjectName()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			return "codesys.chm::/_cds_cmd_insert_device.htm";
		}
	}
}
