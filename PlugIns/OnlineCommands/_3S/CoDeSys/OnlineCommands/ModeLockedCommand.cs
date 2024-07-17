using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{C39DB5B0-9E3E-463C-9FBE-EA760895A195}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_operating_modes.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/online_operating_modes.htm")]
    public class ModeLockedCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "modelocked" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => Strings.CmdModeLocked_Descr;

        public string Name => Strings.OperatingModeLocked;

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                IOnlineDevice17 onldev = null;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.IsApplicationOnline(activeAppObjectGuid) && OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)14))
                {
                    return OnlineCommandHelper.CanSwitchOperatingMode((DeviceOperatingMode)2, activeAppObjectGuid, out onldev);
                }
                return false;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.CmdLocked.ico");

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
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)14);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a5: Unknown result type (might be due to invalid IL or missing references)
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
                try
                {
                    _bIsExecutionInProgress = true;
                    IOnlineDevice17 onldev = null;
                    if (result != Guid.Empty && OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)14, result) && OnlineCommandHelper.CanSwitchOperatingMode((DeviceOperatingMode)2, result, out onldev))
                    {
                        OnlineCommandHelper.SetOperatingMode((DeviceOperatingMode)2, onldev);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex2)
                {
                    APEnvironment.MessageService.Error(ex2.Message, "ErrorActivatingLockedMode", Array.Empty<object>());
                }
                finally
                {
                    _bIsExecutionInProgress = false;
                }
                return;
            }
            throw new BatchWrongArgumentTypeException(BATCH_COMMAND, 0, "GUID");
        }
    }
}
