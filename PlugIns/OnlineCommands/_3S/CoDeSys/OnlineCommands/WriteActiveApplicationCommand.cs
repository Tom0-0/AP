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
    [TypeGuid("{975591C7-7BC5-46ee-9B49-2940D6C13003}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_write_values.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/write_values.htm")]
    public class WriteActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "writeactiveapplication" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WriteValuesCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WriteValuesCommand_Name");

        public bool Enabled
        {
            get
            {
                if (OnlineFeatureHelper.CheckAllOnlineApplications((OnlineFeatureEnum)5))
                {
                    return OnlineCommandHelper.CanWriteValues();
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WriteValuesCommand_ContextlessName");

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
            if (OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)5))
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
                    OnlineCommandHelper.WriteActiveApplication();
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
