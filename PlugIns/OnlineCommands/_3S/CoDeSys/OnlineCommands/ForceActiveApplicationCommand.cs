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
    [TypeGuid("{DCDA7402-D805-460d-AB87-48C4A3D31C37}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_force_values.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/force_values.htm")]
    public class ForceActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "forceactiveaplication" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceValuesCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceValuesCommand_Name");

        public bool Enabled
        {
            get
            {
                if (OnlineFeatureHelper.CheckAllOnlineApplications((OnlineFeatureEnum)4))
                {
                    return OnlineCommandHelper.CanForceValues();
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceValuesCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
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
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            if (!_bIsExecutionInProgress)
            {
                if (arguments == null)
                {
                    throw new ArgumentNullException("arguments");
                }
                if (arguments.Length != 0)
                {
                    throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
                }
                try
                {
                    _bIsExecutionInProgress = true;
                    OnlineCommandHelper.ForceActiveApplication();
                }
                finally
                {
                    _bIsExecutionInProgress = false;
                }
            }
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }
    }
}
