#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.PlcLogicObject;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{B74A8B9C-0335-4303-A2CC-27034BA12FB6}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_simulation.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/simulate_device.htm")]
    public class SwitchSimulationModeSelection : IToggleCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "simulation" };

        public bool RadioCheck => false;

        public bool Checked
        {
            get
            {
                IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
                IDeviceObject4 val = (IDeviceObject4)(object)((selectedDevice is IDeviceObject4) ? selectedDevice : null);
                if (val != null)
                {
                    return val.SimulationMode;
                }
                return false;
            }
        }

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SwitchSimulationModeSelectionCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SwitchSimulationModeSelectionCommand_Description");

        public string ToolTipText => "";

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public static bool IsOffline
        {
            get
            {
                //IL_0023: Unknown result type (might be due to invalid IL or missing references)
                IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
                if (selectedDevice != null)
                {
                    if (((ICollection)selectedDevice.Connectors).Count > 0)
                    {
                        IDeviceObject host = ((IConnector3)selectedDevice.Connectors[0]).GetHost();
                        if (host != null)
                        {
                            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)host).MetaObject.ObjectGuid);
                            if (onlineDevice != null && onlineDevice.IsConnected)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public bool Enabled
        {
            get
            {
                bool flag = false;
                IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
                IDeviceObject4 val = (IDeviceObject4)(object)((selectedDevice is IDeviceObject4) ? selectedDevice : null);
                if (val != null)
                {
                    if (!OnlineFeatureHelper.CheckSelectedDevice((OnlineFeatureEnum)8, (IDeviceObject)(object)val))
                    {
                        return false;
                    }
                    if (!((IDeviceObject)val).AllowsDirectCommunication)
                    {
                        return val.SimulationMode;
                    }
                    ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject)val).DeviceIdentification);
                    flag = targetSettingsById == null || LocalTargetSettings.SimulationDisabled.GetBoolValue(targetSettingsById);
                }
                return !flag & IsOffline;
            }
        }

        public string[] BatchCommand => null;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SwitchSimulationModeSelectionCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
            IDeviceObject4 val = (IDeviceObject4)(object)((selectedDevice is IDeviceObject4) ? selectedDevice : null);
            Debug.Assert(val != null);
            IMetaObject metaObject = ((IObject)val).MetaObject;
            return new string[3]
            {
                metaObject.ProjectHandle.ToString(),
                metaObject.ObjectGuid.ToString(),
                (!val.SimulationMode).ToString()
            };
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
                INavigatorControl navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null;
                return this.Enabled && navigatorControl != null;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            Debug.Assert(arguments.Length == 3);
            int num = int.Parse(arguments[0]);
            Guid guid = new Guid(arguments[1]);
            bool flag = bool.Parse(arguments[2]);
            IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(num, guid);
            IObject @object = objectToModify.Object;
            IDeviceObject4 val = (IDeviceObject4)(object)((@object is IDeviceObject4) ? @object : null);
            bool flag2 = false;
            if (val != null)
            {
                if (flag != val.SimulationMode)
                {
                    flag2 = true;
                }
                val.SimulationMode = (flag);
            }
            ((IObjectManager)APEnvironment.ObjectMgr).SetObject(objectToModify, true, (object)null);
            if (!flag2)
            {
                return;
            }
            Guid[] subObjectGuids = ((IObject)val).MetaObject.SubObjectGuids;
            foreach (Guid guid2 in subObjectGuids)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)val).MetaObject.ProjectHandle, guid2);
                if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SimulationModeChanged(guid2);
                    break;
                }
            }
        }
    }
}
