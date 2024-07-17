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
    [TypeGuid("{DCEA4A16-B0EA-476b-9E44-1EB7569CA360}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_write_values.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/write_values.htm")]
    public class WriteValuesCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "writevalues" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WriteAllApplicationsCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WriteAllApplicationsCommand_Name");

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

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WriteAllApplicationsCommand_ContextlessName");

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
                if (val != null && val.PreparedVarRefs != null && val.PreparedVarRefs.Length != 0)
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
            if (OnlineFeatureHelper.CheckAllOnlineApplications((OnlineFeatureEnum)5))
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
                    OnlineCommandHelper.WriteValues();
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
