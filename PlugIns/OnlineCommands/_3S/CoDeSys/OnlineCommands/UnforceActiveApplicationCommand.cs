using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.WatchList;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{E2427937-56D2-4699-A2E8-1007D9620CE4}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_unforce_all_values.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/unforce_values.htm")]
    public class UnforceActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "unforceactiveapplication" };

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceValuesCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceValuesCommand_Name");

        public bool Enabled
        {
            get
            {
                if (OnlineCommandHelper.GetAllOnlineApplications().Length != 0)
                {
                    return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)4);
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceValuesCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            Guid activeApplication = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication;
            if (((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(activeApplication) != null && OnlineCommandHelper.PromptExecuteOperation_ActiveApplication((ICommand)(object)this, bPromptInNormalMode: false, null))
            {
                return new string[0];
            }
            return null;
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)4))
            {
                if (bContextMenu)
                {
                    return ((IEngine)APEnvironment.Engine).Frame.ActiveView is IWatchListView;
                }
                return true;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length != 0)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
            }
            OnlineCommandHelper.UnforceActiveApplication();
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }
    }
}
