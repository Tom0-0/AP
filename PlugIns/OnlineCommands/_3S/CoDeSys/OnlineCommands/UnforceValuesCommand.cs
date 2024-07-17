#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.WatchList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{311E81C3-C31A-48f9-B48B-0F6B6EA99713}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_unforce_all_values.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/unforce_values.htm")]
    public class UnforceValuesCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "unforcevalues" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceAllApplicationsCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceAllApplicationsCommand_Name");

        public bool Enabled
        {
            get
            {
                if (OnlineCommandHelper.GetAllOnlineApplications().Length != 0)
                {
                    return OnlineFeatureHelper.CheckAllOnlineApplications((OnlineFeatureEnum)4);
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceAllApplicationsCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            List<Guid> list = new List<Guid>();
            IOnlineApplication[] allOnlineApplications = OnlineCommandHelper.GetAllOnlineApplications();
            Debug.Assert(allOnlineApplications != null);
            IOnlineApplication[] array = allOnlineApplications;
            foreach (IOnlineApplication val in array)
            {
                if (val != null && val.IsLoggedIn)
                {
                    list.Add(val.ApplicationGuid);
                }
            }
            if (OnlineCommandHelper.PromptExecuteOperation_MultipleApplications((ICommand)(object)this, list, bPromptInNormalMode: false))
            {
                return new string[0];
            }
            return null;
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (OnlineFeatureHelper.CheckAllOnlineApplications((OnlineFeatureEnum)4))
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
                    OnlineCommandHelper.UnforceValues();
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
