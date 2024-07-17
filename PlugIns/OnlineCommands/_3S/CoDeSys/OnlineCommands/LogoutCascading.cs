using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{A7D25C5B-4506-4263-941B-F28BA2740F92}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_logout_all_applications.htm")]
    public class LogoutCascading : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "logoutCascading" };

        public bool Enabled
        {
            get
            {
                if (OnlineCommandHelper.GetDeviceApplicationGuidOfSelectedDevice() == Guid.Empty)
                {
                    return false;
                }
                IEnumerable<Guid> deviceAppSubApplicationsOfSelectedDevice = OnlineCommandHelper.GetDeviceAppSubApplicationsOfSelectedDevice();
                if (!deviceAppSubApplicationsOfSelectedDevice.All((Guid g) => OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)8, g)))
                {
                    return false;
                }
                return deviceAppSubApplicationsOfSelectedDevice.Any((Guid g) => OnlineCommandHelper.CanLogout(g));
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Description => Strings.LogoutCascadingCommand_Description;

        public Icon LargeIcon => SmallIcon;

        public string Name => Strings.LogoutCascadingCommand_Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.LogoutSmall.ico");

        public string ToolTipText => Name;

        public bool InExecution { get; set; }

        public string[] CreateBatchArguments()
        {
            if (InExecution)
            {
                return null;
            }
            Guid deviceApplicationGuidOfSelectedDevice = OnlineCommandHelper.GetDeviceApplicationGuidOfSelectedDevice();
            if (deviceApplicationGuidOfSelectedDevice == Guid.Empty)
            {
                throw new InvalidOperationException("No device application selected.");
            }
            return new string[1] { deviceApplicationGuidOfSelectedDevice.ToString() };
        }

        public void ExecuteBatch(string[] arguments)
        {
            if (InExecution)
            {
                return;
            }
            IList<Guid> list = OnlineCommandHelper.CollectCascadingAppsFromArguments(arguments);
            try
            {
                InExecution = true;
                foreach (Guid item in list)
                {
                    try
                    {
                        if (OnlineCommandHelper.CanLogout(item) && OnlineCommandHelper.CheckForcedValues(item))
                        {
                            OnlineCommandHelper.Logout(item);
                        }
                    }
                    catch (CancelledByUserException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex2)
                    {
                        APEnvironment.MessageService.Error(ex2.Message, "ErrorLogoutCascading", Array.Empty<object>());
                    }
                }
                if (list.Any())
                {
                    OnlineCommandHelper.LogoutDeviceAppIfNecessary(list.First());
                }
            }
            finally
            {
                InExecution = false;
            }
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                return Enabled;
            }
            return false;
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }
    }
}
