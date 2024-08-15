#define DEBUG
using System;
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
	[TypeGuid("{B3FFC7BE-394A-4e9c-8E85-2FE445849EF6}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_add_device.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceObject.Devices.chm::/Add_Device.htm")]
	public class AddDeviceCmd : AbstractDeviceCmd, IStandardCommand, ICommand, IInteractiveDeviceCommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "add" };

		public bool Enabled
		{
			get
			{
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null)
				{
					return false;
				}
				ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
				if (selectedSVNodes == null || selectedSVNodes.Length == 0)
				{
					return true;
				}
				if (selectedSVNodes.Length > 1)
				{
					return false;
				}
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(selectedSVNodes[0].ProjectHandle, selectedSVNodes[0].ObjectGuid);
				Debug.Assert(metaObjectStub != null);
				string[] array = null;
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedSVNodes[0].ProjectHandle, selectedSVNodes[0].ObjectGuid);
					array = ((IDeviceObject)objectToRead.Object).GetPossibleInterfacesForInsert(objectToRead.SubObjectGuids.Length);
				}
				else if (typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedSVNodes[0].ProjectHandle, selectedSVNodes[0].ObjectGuid);
					array = ((IExplicitConnector)objectToRead2.Object).GetPossibleInterfacesForInsert(objectToRead2.SubObjectGuids.Length);
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

		public string[] CreateBatchArguments()
		{
			return new string[1] { "<interactive>" };
		}

		public string[] CreateBatchArguments(IDeviceIdentification deviceId, string stName, params object[] list)
		{
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return null;
			}
			Guid guid;
			if (primaryProject.SelectedSVNodes != null && primaryProject.SelectedSVNodes.Length == 0)
			{
				guid = Guid.Empty;
			}
			else
			{
				if (primaryProject.SelectedSVNodes == null || primaryProject.SelectedSVNodes.Length != 1)
				{
					return null;
				}
				guid = primaryProject.SelectedSVNodes[0].ObjectGuid;
			}
			if (!(deviceId is IModuleIdentification))
			{
				return new string[7]
				{
					primaryProject.Handle.ToString(),
					guid.ToString(),
					"-1",
					deviceId.Type.ToString(),
					deviceId.Id,
					deviceId.Version,
					stName
				};
			}
			return new string[8]
			{
				primaryProject.Handle.ToString(),
				guid.ToString(),
				"-1",
				deviceId.Type.ToString(),
				deviceId.Id,
				deviceId.Version,
				((IModuleIdentification)deviceId).ModuleId,
				stName
			};
		}

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
					addDeviceFormEx.Initialize(InitialAddDeviceMode.Append);
					addDeviceFormEx.Show((IWin32Window)APEnvironment.FrameForm);
				}
				else
				{
					AddDeviceFormEx.GetInstance?.Initialize(InitialAddDeviceMode.Append);
				}
			}
			else
			{
				ExecuteBatchNonInteractive(arguments);
			}
		}

		public bool GetFilterAndContext(out string[] filter, out IDeviceIdentification context, out IDeviceIdentification[] AllowOnlyDevices)
		{
			filter = null;
			context = null;
			AllowOnlyDevices = null;
			ArrayList arrayList = new ArrayList();
			if (!Enabled)
			{
				return false;
			}
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return false;
			}
			ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
			if (selectedSVNodes != null && selectedSVNodes.Length > 1)
			{
				return false;
			}
			_=primaryProject.Handle;
			_ = Guid.Empty;
			if (selectedSVNodes != null && selectedSVNodes.Length == 1)
			{
				IMetaObject objectToRead = selectedSVNodes[0].GetObjectToRead();
				Debug.Assert(objectToRead != null);
				_=objectToRead.ProjectHandle;
				_=objectToRead.ObjectGuid;
				_ = objectToRead.SubObjectGuids.Length;
				if (objectToRead.Object is IConnector)
				{
					ArrayList arrayList2 = new ArrayList();
					arrayList2.Add(((IConnector)objectToRead.Object).Interface);
					if (objectToRead.Object is IConnector7)
					{
						IObject @object = objectToRead.Object;
						IConnector7 val = (IConnector7)(object)((@object is IConnector7) ? @object : null);
						arrayList2.AddRange(val.AdditionalInterfaces);
					}
					filter = new string[arrayList2.Count];
					arrayList2.CopyTo(filter);
					filter = AbstractDeviceCmd.CheckConstraints(objectToRead, filter, bCheckParent: false, bUpdate: false);
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
				}
				else if (objectToRead.Object is IDeviceObject)
				{
					filter = ((IDeviceObject)objectToRead.Object).GetPossibleInterfacesForInsert(objectToRead.SubObjectGuids.Length);
					context = ((IDeviceObject5)objectToRead.Object).DeviceIdentificationNoSimulation;
					IObject object3 = objectToRead.Object;
					foreach (IConnector8 item in (IEnumerable)((IDeviceObject)((object3 is IDeviceObject) ? object3 : null)).Connectors)
					{
						IConnector8 val2 = item;
						arrayList.AddRange(val2.AllowOnlyDevices);
					}
					filter = AbstractDeviceCmd.CheckConstraints(objectToRead, filter, bCheckParent: false, bUpdate: false);
				}
			}
			else
			{
				filter = null;
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
			return "core.DeviceObject.Devices.chm::/Add_Device.htm";
		}
	}
}
