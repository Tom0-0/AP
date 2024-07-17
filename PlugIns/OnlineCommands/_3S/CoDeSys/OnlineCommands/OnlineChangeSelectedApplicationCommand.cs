using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{C6FCCD24-B0DB-4EAB-A4B4-C9068340FAC2}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_online_change.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Online_Change.htm")]
    public class OnlineChangeSelectedApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "onlinechangeselectedapp" };

        private bool _bIsExecutionInProgress;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "OnlineChangeSelectedApplicationCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "OnlineChangeSelectedApplicationCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                Guid[] selectedApplicationGuids = OnlineCommandHelper.GetSelectedApplicationGuids();
                if (selectedApplicationGuids.Length != 1)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)2))
                {
                    return false;
                }
                if (OnlineCommandHelper.CanDownload(selectedApplicationGuids[0]) && OnlineCommandHelper.OnlineChangeSupported(selectedApplicationGuids[0]))
                {
                    return true;
                }
                return false;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => Strings.OnlineChangeSelectedApplicationCommand_ContextlessName;

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return false;
            }
            Guid[] selectedApplicationGuids = OnlineCommandHelper.GetSelectedApplicationGuids();
            if (selectedApplicationGuids.Length == 1 && OnlineCommandHelper.IsLoggedIn(selectedApplicationGuids[0]) && OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)2))
            {
                return true;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0023: Unknown result type (might be due to invalid IL or missing references)
            Guid guid = Guid.Empty;
            try
            {
                if (arguments == null)
                {
                    throw new ArgumentNullException("arguments");
                }
                if (arguments.Length > 1)
                {
                    throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
                }
                guid = new Guid(arguments[0]);
                if (guid == OnlineCommandHelper.ActiveAppObjectGuid)
                {
                    InternalCodeStateProvider.StopOnlineCodeStateUpdates();
                }
                ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
                if (guid != Guid.Empty && OnlineCommandHelper.CanDownload(guid))
                {
                    if (OnlineCommandHelper.IsUpToDate(guid))
                    {
                        APEnvironment.MessageService.Information(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ProgramNotChangedOnlChange"), "ProgramNotChangedOnlChangeSelectedApp", Array.Empty<object>());
                    }
                    else
                    {
                        OnlineCommandHelper.Download(guid, bOnlineChange: true);
                    }
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
                APEnvironment.MessageService.Error(ex2.Message, "ErrorOnlineChangeSelectedApp", Array.Empty<object>());
            }
            finally
            {
                if (guid == OnlineCommandHelper.ActiveAppObjectGuid)
                {
                    InternalCodeStateProvider.StartOnlineCodeStateUpdates();
                }
            }
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
                Guid selectedApplicationGuid = OnlineCommandHelper.GetSelectedApplicationGuid();
                if (OnlineCommandHelper.PromptExecuteOperation_SpecificApplication(selectedApplicationGuid, (ICommand)(object)this, bPromptInNormalMode: true, null))
                {
                    return new string[1] { selectedApplicationGuid.ToString() };
                }
                return null;
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }
    }
}
