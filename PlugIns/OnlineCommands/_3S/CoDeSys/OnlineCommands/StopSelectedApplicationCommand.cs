using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{d8ee4c28-e09b-4ace-ad55-600a8a3c906f}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_stop.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/stop.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Stop_application.htm")]
    public class StopSelectedApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "stopMultiapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "StopSelectedApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "StopSelectedApplicationCommand_Name");

        public bool Enabled
        {
            get
            {
                if (!OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)8))
                {
                    return false;
                }
                return true;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.StopSmall.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "StopSelectedApplicationCommand_ContextlessName");

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
            try
            {
                _bIsExecutionInProgress = true;
                Guid[] selectedApplicationGuids = OnlineCommandHelper.GetSelectedApplicationGuids();
                List<string> list = new List<string>();
                for (int i = 0; i < selectedApplicationGuids.Length; i++)
                {
                    if (OnlineCommandHelper.IsApplicationOnline(selectedApplicationGuids[i]) && OnlineCommandHelper.PromptExecuteOperation_SpecificApplication(selectedApplicationGuids[i], (ICommand)(object)this, bPromptInNormalMode: false, null))
                    {
                        list.Add(selectedApplicationGuids[i].ToString());
                    }
                }
                return list.ToArray();
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                INavigatorControl navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null;
                return navigatorControl != null && OnlineCommandHelper.CanStopAny(navigatorControl.SelectedSVNodes) && OnlineFeatureHelper.CheckSelectedApplications(OnlineFeatureEnum.CoreApplicationHandling);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            if (_bIsExecutionInProgress)
            {
                return;
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            Guid result = Guid.Empty;
            if (arguments.Length == 1 && !Guid.TryParse(arguments[0], out result))
            {
                throw new ArgumentException("argument[0]: Expected a Guid!");
            }
            List<Guid> list = new List<Guid>();
            try
            {
                _bIsExecutionInProgress = true;
                if (arguments.Length < 1)
                {
                    return;
                }
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (Guid.TryParse(arguments[i], out result))
                    {
                        list.Add(result);
                    }
                }
                foreach (Guid item in list)
                {
                    if (!(item != Guid.Empty) || !OnlineCommandHelper.IsApplicationOnline(item))
                    {
                        continue;
                    }
                    try
                    {
                        if (OnlineCommandHelper.CanStop(item))
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
                        APEnvironment.MessageService.Error(ex2.Message, "ErrorStopSelectedApp", Array.Empty<object>());
                    }
                }
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }
    }
}
