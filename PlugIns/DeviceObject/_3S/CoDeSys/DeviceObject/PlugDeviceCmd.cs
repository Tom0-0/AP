#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
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
	[TypeGuid("{3c9bacd6-cc7d-4ae7-90a6-d35fe7685028}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_plug_device.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceObject.Devices.chm::/Plug_Device.htm")]
	public class PlugDeviceCmd : IStandardCommand, ICommand, IInteractiveDeviceCommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "plug" };

		private static readonly string[] s_requiredArgumentType = new string[7] { "int", "GUID", "int", "string", "string", "string", "string" };

		public string ToolTipText => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PlugDeviceCmdToolTip");

		public Icon LargeIcon => SmallIcon;

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PlugDeviceCmdDescription");

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "PlugDeviceCmdName");

		public bool Enabled => DeviceCommandHelper.GetSelectedDevice() is SlotDeviceObject;

		public Icon SmallIcon => null;

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string[] BatchCommand => BATCH_COMMAND;

		public bool Selectable => AbstractDeviceCmd.IsSelectable((ICommand)(object)this);

		public string[] CreateBatchArguments(IDeviceIdentification deviceId, string stName, params object[] list)
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			ISVNode val = null;
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
				.Length == 1)
			{
				val = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[0];
			}
			if (val == null)
			{
				return null;
			}
			if (!(deviceId is IModuleIdentification))
			{
				return new string[6]
				{
					val.ProjectHandle.ToString(),
					val.ObjectGuid.ToString(),
					deviceId.Type.ToString(),
					deviceId.Id,
					deviceId.Version,
					stName
				};
			}
			return new string[7]
			{
				val.ProjectHandle.ToString(),
				val.ObjectGuid.ToString(),
				deviceId.Type.ToString(),
				deviceId.Id,
				deviceId.Version,
				((IModuleIdentification)deviceId).ModuleId,
				stName
			};
		}

		string[] IStandardCommand.CreateBatchArguments()
		{
			return new string[1] { "<interactive>" };
		}

		public bool IsVisible(bool bContextMenu)
		{
			return bContextMenu && ((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) != null && (this.Enabled & DeviceCommandHelper.IsOffline);
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (arguments != null && arguments.Length == 1 && arguments[0] == "<interactive>")
			{
				if (!AddDeviceFormEx.OneInstanceVisible)
				{
					AddDeviceFormEx addDeviceFormEx = new AddDeviceFormEx();
					addDeviceFormEx.Initialize(InitialAddDeviceMode.Plug);
					addDeviceFormEx.Show((IWin32Window)APEnvironment.FrameForm);
				}
				else
				{
					AddDeviceFormEx.GetInstance?.Initialize(InitialAddDeviceMode.Plug);
				}
				return;
			}
			ParseArguments(arguments, out var nProjectHandle, out var guidParent, out var deviceId, out var stName);
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidParent);
			Debug.Assert(objectToRead != null);
			if (!(objectToRead.Object is SlotDeviceObject))
			{
				throw new BatchExecutionException(BatchCommand, "Devices can only be plugged into slots");
			}
			DeviceCommandHelper.PlugDeviceIntoSlot(nProjectHandle, guidParent, deviceId, stName);
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		private void ParseArguments(string[] arguments, out int nProjectHandle, out Guid guidParent, out IDeviceIdentification deviceId, out string stName)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = s_requiredArgumentType.Length;
			if (arguments.Length < num2 - 1)
			{
				throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, num2);
			}
			if (arguments.Length > num2)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, num2 + 1);
			}
			try
			{
				nProjectHandle = int.Parse(arguments[num]);
				num++;
				if (arguments[num] == string.Empty)
				{
					guidParent = Guid.Empty;
				}
				else
				{
					guidParent = new Guid(arguments[num]);
				}
				num++;
				int num3 = int.Parse(arguments[num]);
				num++;
				string text = arguments[num];
				num++;
				string text2 = arguments[num];
				num++;
				if (arguments.Length == num2 - 1)
				{
					deviceId = ((IDeviceRepository)APEnvironment.DeviceRepository).CreateDeviceIdentification(num3, text, text2);
				}
				else
				{
					string text3 = arguments[num];
					num++;
					deviceId = (IDeviceIdentification)(object)((IDeviceRepository)APEnvironment.DeviceRepository).CreateModuleIdentification(num3, text, text2, text3);
				}
				stName = arguments[num];
				num++;
				Debug.Assert(num >= num2 - 1);
			}
			catch (Exception)
			{
				throw new BatchWrongArgumentTypeException(BatchCommand, num, s_requiredArgumentType[num]);
			}
		}

		public bool GetFilterAndContext(out string[] filter, out IDeviceIdentification context, out IDeviceIdentification[] AllowOnlyDevices)
		{
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Expected O, but got Unknown
			filter = null;
			context = null;
			AllowOnlyDevices = null;
			ArrayList arrayList = new ArrayList();
			if (!Enabled)
			{
				return false;
			}
			if (!(DeviceCommandHelper.GetSelectedDevice() is SlotDeviceObject))
			{
				return false;
			}
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return false;
			}
			ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length != 1)
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
				if (!(objectToRead.Object is SlotDeviceObject))
				{
					return false;
				}
				SlotDeviceObject slotDeviceObject = (SlotDeviceObject)(object)objectToRead.Object;
				filter = slotDeviceObject.GetPossibleInterfacesForPlug();
				IDeviceObject ownerDevice = slotDeviceObject.GetOwnerDevice();
				IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid);
				if (objectToRead2 != null)
				{
					filter = AbstractDeviceCmd.CheckConstraints(objectToRead2, filter, bCheckParent: true, bUpdate: false);
				}
				if (ownerDevice != null)
				{
					context = ((IDeviceObject5)((ownerDevice is IDeviceObject5) ? ownerDevice : null)).DeviceIdentificationNoSimulation;
					foreach (IConnector8 item in (IEnumerable)ownerDevice.Connectors)
					{
						IConnector8 val = item;
						arrayList.AddRange(val.AllowOnlyDevices);
					}
				}
				AllowOnlyDevices = (IDeviceIdentification[])(object)new IDeviceIdentification[arrayList.Count];
				arrayList.CopyTo(AllowOnlyDevices);
				return true;
			}
			return false;
		}

		public string GetFixObjectName()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			return "core.DeviceObject.Devices.chm::/Plug_Device.htm";
		}

		public void OverridableExecuteBatch(string[] batchArgs)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager2)
			{
				((ICommandManager2)((IEngine)APEnvironment.Engine).CommandManager).ExecuteCommand(TypeGuidAttribute.FromObject((object)this).Guid, batchArgs);
			}
			else
			{
				ExecuteBatch(batchArgs);
			}
		}
	}
}
