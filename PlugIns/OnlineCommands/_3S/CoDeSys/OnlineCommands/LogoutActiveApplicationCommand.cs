#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{87CF633B-C70D-4e11-B40A-61BD7008FC78}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_logoff.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/logout_from.htm")]
    public class LogoutActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "logoutactiveapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LogoutActiveApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LogoutActiveApplicationCommand_Name");

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8))
                {
                    return OnlineCommandHelper.CanLogout(activeAppObjectGuid);
                }
                return false;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(LoginActiveApplicationCommand), "_3S.CoDeSys.OnlineCommands.Resources.LogoutSmall.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LogoutActiveApplicationCommand_ContextlessName");

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            return new string[0];
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
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                if (!_bIsExecutionInProgress)
                {
                    _bIsExecutionInProgress = true;
                    if (arguments == null)
                    {
                        throw new ArgumentNullException("arguments");
                    }
                    if (arguments.Length != 0)
                    {
                        throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
                    }
                    Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                    if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanLogout(activeAppObjectGuid) && OnlineCommandHelper.CheckForcedValues(activeAppObjectGuid))
                    {
                        OnlineCommandHelper.Logout(activeAppObjectGuid);
                    }
                    OnlineCommandHelper.LogoutDeviceAppIfNecessary(activeAppObjectGuid);
                }
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }
    }
}
