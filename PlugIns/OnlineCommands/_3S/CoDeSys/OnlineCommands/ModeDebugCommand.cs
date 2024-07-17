using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{F1957746-E8F9-43D4-91F0-5EDD7C57E38E}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_operating_modes.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/online_operating_modes.htm")]
    public class ModeDebugCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "modedebug" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => Strings.CmdModeDebug_Descr;

        public string Name => Strings.OperatingModeDebug;

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                IOnlineDevice17 onldev = null;
                if (activeAppObjectGuid != Guid.Empty && (OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)15) || OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)14)))
                {
                    return OnlineCommandHelper.CanSwitchOperatingMode((DeviceOperatingMode)1, activeAppObjectGuid, out onldev);
                }
                return false;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.CmdDebug.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            return new string[1] { OnlineCommandHelper.ActiveAppObjectGuid.ToString() };
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)15))
                {
                    return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)14);
                }
                return true;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d2: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return;
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 1);
            }
            Guid result = Guid.Empty;
            if (Guid.TryParse(arguments[0], out result))
            {
                IOnlineDevice17 val = null;
                object obj = new object();
                try
                {
                    _bIsExecutionInProgress = true;
                    if (result != Guid.Empty && (OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)14, result) || OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)15, result)))
                    {
                        IOnlineDevice onlineDeviceForApplication = OnlineCommandHelper.GetOnlineDeviceForApplication(result);
                        val = (IOnlineDevice17)(object)((onlineDeviceForApplication is IOnlineDevice17) ? onlineDeviceForApplication : null);
                        if (val != null)
                        {
                            ((IOnlineDevice3)val).SharedConnect(obj);
                            OnlineCommandHelper.SetOperatingMode((DeviceOperatingMode)1, val);
                        }
                    }
                }
                catch (Exception ex)
                {
                    APEnvironment.MessageService.Error(ex.Message, "ErrorActivatingDebugMode", Array.Empty<object>());
                }
                finally
                {
                    if (val != null && ((IOnlineDevice)val).IsConnected)
                    {
                        ((IOnlineDevice3)val).SharedDisconnect(obj);
                    }
                    _bIsExecutionInProgress = false;
                }
                return;
            }
            throw new BatchWrongArgumentTypeException(BATCH_COMMAND, 0, "GUID");
        }
    }
}
