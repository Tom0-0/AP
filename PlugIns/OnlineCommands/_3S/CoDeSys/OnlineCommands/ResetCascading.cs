using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _3S.CoDeSys.OnlineCommands
{
    public abstract class ResetCascading : IStandardCommand, ICommand
    {
        public abstract ResetOption ResetOption { get; }

        public bool Enabled
        {
            get
            {
                Guid deviceApplicationGuidOfSelectedDevice = OnlineCommandHelper.GetDeviceApplicationGuidOfSelectedDevice();
                if (deviceApplicationGuidOfSelectedDevice == Guid.Empty)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)8, deviceApplicationGuidOfSelectedDevice))
                {
                    return false;
                }
                IEnumerable<Guid> deviceAppSubApplicationsOfSelectedDevice = OnlineCommandHelper.GetDeviceAppSubApplicationsOfSelectedDevice();
                Func<Guid, bool> predicate = (Guid g) => OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)8, g) && OnlineCommandHelper.CanReset(g);
                return deviceAppSubApplicationsOfSelectedDevice.All(predicate);
            }
        }

        public abstract string BatchCommandVerb { get; }

        public string[] BatchCommand => new string[2] { "online", BatchCommandVerb };

        public Guid Category => OnlineCommandCategory.Guid;

        public abstract string Description { get; }

        public Icon LargeIcon => SmallIcon;

        public abstract string Name { get; }

        public Icon SmallIcon => null;

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
            //IL_0011: Unknown result type (might be due to invalid IL or missing references)
            //IL_0017: Invalid comparison between Unknown and I4
            //IL_0048: Unknown result type (might be due to invalid IL or missing references)
            if (InExecution)
            {
                return;
            }
            IList<Guid> list = OnlineCommandHelper.CollectCascadingAppsFromArguments(arguments);
            if ((int)ResetOption == 2)
            {
                list = list.Take(1).ToLList<Guid>();
            }
            try
            {
                InExecution = true;
                foreach (Guid item in list)
                {
                    try
                    {
                        if (OnlineCommandHelper.CanReset(item))
                        {
                            OnlineCommandHelper.ResetApplication(item, ResetOption);
                            continue;
                        }
                        return;
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
