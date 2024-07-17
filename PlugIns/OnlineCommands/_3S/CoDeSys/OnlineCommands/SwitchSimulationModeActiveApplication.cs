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
using _3S.CoDeSys.PlcLogicObject;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{515E05EC-AECE-47d7-98A6-5C582BA392FB}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_simulation.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Simulate_Device.htm")]
    public class SwitchSimulationModeActiveApplication : IToggleCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "simulate_active_app" };

        public bool RadioCheck => false;

        public bool Checked
        {
            get
            {
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                IDeviceObject4 val = (IDeviceObject4)(object)((activeDevice is IDeviceObject4) ? activeDevice : null);
                if (val != null)
                {
                    return val.SimulationMode;
                }
                return false;
            }
        }

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SwitchSimulationModeActiveCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SwitchSimulationModeActiveCommand_Description");

        public string ToolTipText => "";

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public static bool IsOffline
        {
            get
            {
                //IL_0023: Unknown result type (might be due to invalid IL or missing references)
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                if (activeDevice != null)
                {
                    if (((ICollection)activeDevice.Connectors).Count > 0)
                    {
                        IDeviceObject host = ((IConnector3)activeDevice.Connectors[0]).GetHost();
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
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                if (activeDevice != null)
                {
                    ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(activeDevice.DeviceIdentification);
                    flag = targetSettingsById == null || LocalTargetSettings.SimulationDisabled.GetBoolValue(targetSettingsById);
                }
                return !flag & IsOffline;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SwitchSimulationModeActiveCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
            IDeviceObject4 val = (IDeviceObject4)(object)((activeDevice is IDeviceObject4) ? activeDevice : null);
            Debug.Assert(val != null);
            return new string[1] { (!val.SimulationMode).ToString() };
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            if (arguments.Length == 0)
            {
                throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, 1);
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 1);
            }
            bool flag = bool.Parse(arguments[0]);
            IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(((IObject)OnlineCommandHelper.GetActiveDevice()).MetaObject);
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
            foreach (Guid guid in subObjectGuids)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)val).MetaObject.ProjectHandle, guid);
                if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SimulationModeChanged(guid);
                    break;
                }
            }
        }
    }
}
