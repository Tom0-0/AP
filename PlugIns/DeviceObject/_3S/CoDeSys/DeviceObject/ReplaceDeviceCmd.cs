#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{361c88a2-559e-4893-9fc5-c2fc635dcbc4}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_update_device.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceObject.Devices.chm::/Update_Device.htm")]
	public class ReplaceDeviceCmd : IStandardCommand, ICommand, IInteractiveDeviceCommand
	{
		private static readonly string[] BATCH_COMMAND = new string[2] { "device", "replace" };

		private static readonly string[] s_requiredArgumentType = new string[6] { "int", "int", "string", "string", "string", "Guid" };

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string Name => Strings.ReplaceDeviceCmdName;

		public string Description => Strings.ReplaceDeviceCmdDescription;

		public string ToolTipText => Strings.ReplaceDeviceCmdDescription;

		public Icon SmallIcon => null;

		public Icon LargeIcon => null;

		public bool Enabled
		{
			get
			{
				IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
				if (primaryProject == null)
				{
					return false;
				}
				ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
				if (selectedSVNodes.Length == 0)
				{
					return false;
				}
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(selectedSVNodes[0].ProjectHandle, selectedSVNodes[0].ObjectGuid);
				Debug.Assert(metaObjectStub != null);
				Guid parentObjectGuid = metaObjectStub.ParentObjectGuid;
				ISVNode[] array = selectedSVNodes;
				foreach (ISVNode val in array)
				{
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val.ProjectHandle, val.ObjectGuid);
					Debug.Assert(metaObjectStub != null);
					if (!typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(SlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						return false;
					}
					if (typeof(SlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && metaObjectStub.Namespace == Guid.Empty)
					{
						return false;
					}
					if (metaObjectStub.ParentObjectGuid != parentObjectGuid)
					{
						return false;
					}
					IMetaObject objectToRead = val.GetObjectToRead();
					DeviceObject deviceObject = null;
					SlotDeviceObject slotDeviceObject = null;
					if (objectToRead.Object is DeviceObject)
					{
						deviceObject = (DeviceObject)(object)objectToRead.Object;
					}
					if (objectToRead.Object is SlotDeviceObject)
					{
						slotDeviceObject = (SlotDeviceObject)(object)objectToRead.Object;
					}
					if (metaObjectStub.ParentObjectGuid != Guid.Empty)
					{
						((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val.ProjectHandle, metaObjectStub.ParentObjectGuid);
						IConnector val2 = null;
						if (deviceObject != null)
						{
							val2 = ((!deviceObject.IsLogical || !DeviceCommandHelper.IsUpdateForLogicalDevicesEnabled((IDeviceObject)(object)deviceObject)) ? FindChildConnector(deviceObject, metaObjectStub.ParentObjectGuid) : deviceObject.Connectors[0]);
						}
						if (slotDeviceObject != null)
						{
							val2 = FindChildConnector(slotDeviceObject.GetDevice(), metaObjectStub.ParentObjectGuid);
						}
						if (val2 == null)
						{
							return false;
						}
						if (val2 is Connector && !(val2 as Connector).UpdateAllowed)
						{
							return false;
						}
					}
				}
				return DeviceCommandHelper.IsOffline;
			}
		}

		public string[] BatchCommand => BATCH_COMMAND;

		public bool Selectable => AbstractDeviceCmd.IsSelectable((ICommand)(object)this);

		private IConnector FindChildConnector(DeviceObject device, Guid guidParent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Invalid comparison between Unknown and I4
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole != 1)
				{
					continue;
				}
				foreach (IAdapter item2 in (IEnumerable)val.Adapters)
				{
					if (Array.IndexOf(item2.Modules, guidParent) >= 0)
					{
						return val;
					}
				}
			}
			return null;
		}

		public string[] CreateBatchArguments(IDeviceIdentification deviceId, string stName, params object[] list)
		{
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (list != null && list.Length != 0 && list[0] is bool)
			{
				flag = (bool)list[0];
			}
			int num = -1;
			LList<string> val = new LList<string>();
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
				.Length != 0)
			{
				if (flag)
				{
					num = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
					Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(num);
					ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
					for (int i = 0; i < selectedSVNodes.Length; i++)
					{
						IObjectProperty obj = selectedSVNodes[i].GetMetaObjectStub().Properties.FirstOrDefault((IObjectProperty prop) => prop is IDeviceProperty);
						IDeviceProperty val2 = (IDeviceProperty)(object)((obj is IDeviceProperty) ? obj : null);
						if (val2 == null)
						{
							continue;
						}
						Guid[] array = allObjects;
						for (int j = 0; j < array.Length; j++)
						{
							Guid guid = array[j];
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(num, guid);
							if (metaObjectStub != null)
							{
								IObjectProperty obj2 = metaObjectStub.Properties.FirstOrDefault((IObjectProperty prop) => prop is IDeviceProperty);
								IDeviceProperty val3 = (IDeviceProperty)(object)((obj2 is IDeviceProperty) ? obj2 : null);
								if (val3 != null && val2.DeviceIdentification is DeviceIdentification && (val2.DeviceIdentification as DeviceIdentification).Equals(val3.DeviceIdentification, bIgnoreVersion: true))
								{
									val.Add(guid.ToString());
								}
							}
						}
					}
				}
				else
				{
					ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
					foreach (ISVNode val4 in selectedSVNodes)
					{
						num = val4.ProjectHandle;
						val.Add(val4.ObjectGuid.ToString());
					}
				}
			}
			string[] array2 = new string[0];
			if (val.Count > 0)
			{
				if (deviceId is IModuleIdentification)
				{
					array2 = new string[val.Count + 5];
					array2[0] = num.ToString();
					array2[1] = deviceId.Type.ToString();
					array2[2] = deviceId.Id;
					array2[3] = deviceId.Version;
					array2[4] = "-M=" + ((IModuleIdentification)deviceId).ModuleId;
					val.CopyTo(array2, 5);
				}
				else
				{
					array2 = new string[val.Count + 4];
					array2[0] = num.ToString();
					array2[1] = deviceId.Type.ToString();
					array2[2] = deviceId.Id;
					array2[3] = deviceId.Version;
					val.CopyTo(array2, 4);
				}
			}
			return array2;
		}

		public string[] CreateBatchArguments()
		{
			return new string[1] { "<interactive>" };
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu)
			{
				if (Enabled)
				{
					return ((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl;
				}
				return false;
			}
			return true;
		}

		public void ExecuteBatch(string[] arguments)
		{
			if (arguments != null && arguments.Length == 1 && arguments[0] == "<interactive>")
			{
				if (!AddDeviceFormEx.OneInstanceVisible)
				{
					AddDeviceFormEx addDeviceFormEx = new AddDeviceFormEx();
					addDeviceFormEx.Initialize(InitialAddDeviceMode.Replace);
					addDeviceFormEx.Show((IWin32Window)APEnvironment.FrameForm);
				}
				else
				{
					AddDeviceFormEx.GetInstance?.Initialize(InitialAddDeviceMode.Replace);
				}
				return;
			}
			IProgressCallback val = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
			try
			{
				ParseArguments(arguments, out var nProjectHandle, out var deviceId, out var objectsToReplace);
				IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(nProjectHandle);
				undoManager.BeginCompoundAction(ReplaceDeviceAction.REPLACEACTION);
				LList<DeviceEventArgs2> val2 = new LList<DeviceEventArgs2>();
				bool flag = false;
				try
				{
					APEnvironment.Engine.BeginSuppressUpdateLanguageModel();
					Guid[] array = objectsToReplace;
					foreach (Guid guid in array)
					{
						ReplaceDeviceAction replaceDeviceAction = new ReplaceDeviceAction(nProjectHandle, guid, deviceId, val2);
						undoManager.AddAction((IUndoableAction)(object)replaceDeviceAction);
						replaceDeviceAction.Redo();
						DeviceBasedObjectRelationshipCheckProvider.UpdateDevice(nProjectHandle, guid, deviceId, delegate(string message)
						{
							DeviceMessage deviceMessage = new DeviceMessage(message, (Severity)4);
							APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
						});
					}
					foreach (DeviceEventArgs2 item in val2)
					{
						try
						{
							APEnvironment.DeviceMgr.RaiseDeviceUpdated(item);
						}
						catch
						{
						}
					}
					APEnvironment.Engine.EndSuppressUpdateLanguageModel();
					undoManager.EndCompoundAction();
					flag = true;
					LanguageModelHelper.ClearDiagnosisInstance();
					DeviceObjectHelper.ClearLanguageModel(nProjectHandle, objectsToReplace, val2);
				}
				catch
				{
					if (!flag)
					{
						APEnvironment.Engine.EndSuppressUpdateLanguageModel();
						undoManager.EndCompoundAction();
						if (!undoManager.InCompoundAction)
						{
							undoManager.Undo();
						}
					}
					val2.Clear();
					throw;
				}
			}
			finally
			{
				if (val != null)
				{
					val.Finish();
				}
			}
		}

		protected void ParseArguments(string[] arguments, out int nProjectHandle, out IDeviceIdentification deviceId, out Guid[] objectsToReplace)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			int i = 0;
			if (arguments.Length < s_requiredArgumentType.Length - 1)
			{
				throw new BatchTooFewArgumentsException(BATCH_COMMAND, arguments.Length, s_requiredArgumentType.Length - 1);
			}
			try
			{
				nProjectHandle = int.Parse(arguments[i]);
				i++;
				int num = int.Parse(arguments[i]);
				i++;
				string text = arguments[i];
				i++;
				string text2 = arguments[i];
				i++;
				if (arguments[i].StartsWith("-M="))
				{
					string text3 = arguments[i].Substring(3);
					deviceId = (IDeviceIdentification)(object)((IDeviceRepository)APEnvironment.DeviceRepository).CreateModuleIdentification(num, text, text2, text3);
					i++;
				}
				else
				{
					deviceId = ((IDeviceRepository)APEnvironment.DeviceRepository).CreateDeviceIdentification(num, text, text2);
				}
				if (i == arguments.Length)
				{
					throw new BatchTooFewArgumentsException(BATCH_COMMAND, arguments.Length, i);
				}
				LList<Guid> val = new LList<Guid>();
				for (; i < arguments.Length; i++)
				{
					val.Add(new Guid(arguments[i]));
				}
				objectsToReplace = val.ToArray();
			}
			catch (BatchTooFewArgumentsException)
			{
				throw;
			}
			catch (Exception)
			{
				string text4 = ((i >= s_requiredArgumentType.Length - 1) ? "Guid" : s_requiredArgumentType[i]);
				throw new BatchWrongArgumentTypeException(BATCH_COMMAND, i, text4);
			}
		}

		public bool GetFilterAndContext(out string[] filter, out IDeviceIdentification context, out IDeviceIdentification[] AllowOnlyDevices)
		{
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Expected O, but got Unknown
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Invalid comparison between Unknown and I4
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Expected O, but got Unknown
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
			if (selectedSVNodes == null || selectedSVNodes.Length == 0)
			{
				return false;
			}
			int handle = primaryProject.Handle;
			IMetaObject objectToRead = selectedSVNodes[0].GetObjectToRead();
			Debug.Assert(objectToRead != null);
			DeviceObject deviceObject = null;
			SlotDeviceObject slotDeviceObject = null;
			if (objectToRead.Object is DeviceObject)
			{
				deviceObject = (DeviceObject)(object)objectToRead.Object;
			}
			if (objectToRead.Object is SlotDeviceObject)
			{
				slotDeviceObject = (SlotDeviceObject)(object)objectToRead.Object;
			}
			IMetaObject val = null;
			if (objectToRead.ParentObjectGuid != Guid.Empty)
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, objectToRead.ParentObjectGuid);
				IConnector val2 = null;
				if (deviceObject != null)
				{
					if (deviceObject.IsLogical)
					{
						context = ((IDeviceObject5)deviceObject).DeviceIdentificationNoSimulation;
						val2 = deviceObject.Connectors[0];
					}
					else
					{
						val2 = FindChildConnector(deviceObject, objectToRead.ParentObjectGuid);
					}
				}
				if (slotDeviceObject != null)
				{
					if (slotDeviceObject.SlotConnectors != null)
					{
						foreach (IConnector item in (IEnumerable)slotDeviceObject.SlotConnectors)
						{
							IConnector val3 = item;
							if ((int)val3.ConnectorRole != 1)
							{
								continue;
							}
							foreach (IAdapter item2 in (IEnumerable)val3.Adapters)
							{
								if (Array.IndexOf(item2.Modules, objectToRead.ParentObjectGuid) >= 0)
								{
									val2 = val3;
								}
							}
						}
					}
					if (val2 == null)
					{
						val2 = FindChildConnector(slotDeviceObject.GetDevice(), objectToRead.ParentObjectGuid);
					}
				}
				if (val2 == null)
				{
					return false;
				}
				ArrayList arrayList2 = new ArrayList();
				arrayList2.Add(val2.Interface);
				if (val2 is IConnector7)
				{
					IConnector7 val4 = (IConnector7)(object)((val2 is IConnector7) ? val2 : null);
					arrayList2.AddRange(val4.AdditionalInterfaces);
				}
				IConnector obj = ((val2 is IIoProvider) ? val2 : null);
				IIoProvider obj2 = ((obj != null) ? ((IIoProvider)obj).Parent : null);
				IConnector val5 = (IConnector)(object)((obj2 is IConnector) ? obj2 : null);
				if (val5 != null && (int)val5.ConnectorRole == 0)
				{
					if (!arrayList2.Contains(val5.Interface))
					{
						arrayList2.Add(val5.Interface);
					}
					if (val5 is IConnector7)
					{
						foreach (string additionalInterface in ((IConnector7)((val5 is IConnector7) ? val5 : null)).AdditionalInterfaces)
						{
							if (!arrayList2.Contains(additionalInterface))
							{
								arrayList2.Add(additionalInterface);
							}
						}
					}
				}
				filter = new string[arrayList2.Count];
				arrayList2.CopyTo(filter);
				filter = AbstractDeviceCmd.CheckConstraints(val, filter, bCheckParent: true, bUpdate: true);
				if (val.Object is IConnector)
				{
					IDeviceObject deviceObject2 = ((IConnector)val.Object).GetDeviceObject();
					if (deviceObject2 != null)
					{
						context = ((IDeviceObject5)((deviceObject2 is IDeviceObject5) ? deviceObject2 : null)).DeviceIdentificationNoSimulation;
					}
					if (val.Object is IConnector8)
					{
						IObject @object = val.Object;
						arrayList.AddRange(((IConnector8)((@object is IConnector8) ? @object : null)).AllowOnlyDevices);
					}
				}
				else if (val.Object is IDeviceObject)
				{
					context = ((IDeviceObject5)val.Object).DeviceIdentificationNoSimulation;
					IObject object2 = val.Object;
					foreach (IConnector8 item3 in (IEnumerable)((IDeviceObject)((object2 is IDeviceObject) ? object2 : null)).Connectors)
					{
						IConnector8 val6 = item3;
						arrayList.AddRange(val6.AllowOnlyDevices);
					}
				}
			}
			AllowOnlyDevices = (IDeviceIdentification[])(object)new IDeviceIdentification[arrayList.Count];
			arrayList.CopyTo(AllowOnlyDevices);
			return true;
		}

		public string GetFixObjectName()
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
				.Length == 1)
			{
				return ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[0].Name;
			}
			return string.Empty;
		}

		public string GetOnlineHelpUrl()
		{
			return "core.DeviceObject.Devices.chm::/Update_Device.htm";
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
