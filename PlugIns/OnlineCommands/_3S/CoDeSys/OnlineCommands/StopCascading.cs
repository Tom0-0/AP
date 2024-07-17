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
    [TypeGuid("{1FBE6D20-C5C1-4DC1-80A6-DE5B384F0357}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_stop_all_applications.htm")]
    public class StopCascading : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "stopCascading" };

        public bool Enabled
        {
            get
            {
                if (OnlineCommandHelper.GetDeviceApplicationGuidOfSelectedDevice() == Guid.Empty)
                {
                    return false;
                }
                IEnumerable<Guid> deviceAppSubApplicationsOfSelectedDevice = OnlineCommandHelper.GetDeviceAppSubApplicationsOfSelectedDevice();
                if (!deviceAppSubApplicationsOfSelectedDevice.Any())
                {
                    return false;
                }
                Func<Guid, bool> predicate = (Guid g) => OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)8, g) && OnlineCommandHelper.CanStop(g);
                return deviceAppSubApplicationsOfSelectedDevice.All(predicate);
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Description => Strings.StopCascadingCommand_Description;

        public Icon LargeIcon => SmallIcon;

        public string Name => Strings.StopCascadingCommand_Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.StopSmall.ico");

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
            if (OnlineCommandHelper.IsDeviceApplication(list[0]))
            {
                list.RemoveAt(0);
            }
            try
            {
                InExecution = true;
                foreach (Guid item in list)
                {
                    try
                    {
                        if (OnlineCommandHelper.IsApplicationOnline(item) && OnlineCommandHelper.CanStop(item))
                        {
                            OnlineCommandHelper.Stop(item);
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
                        APEnvironment.MessageService.Error(ex2.Message, "ErrorStopCascading", Array.Empty<object>());
                        return;
                    }
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
