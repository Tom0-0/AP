using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{CC9AD1E5-FE78-4dc6-A4BE-B1991BF068EA}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_load.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Download__on__device_.htm")]
    public class DownloadActiveApplicationCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "downloadactiveapp" };

        private bool _bIsExecutionInProgress;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DownloadActiveApplicationCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DownloadActiveApplicationCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                //IL_0027: Unknown result type (might be due to invalid IL or missing references)
                //IL_002d: Invalid comparison between Unknown and I4
                //IL_002f: Unknown result type (might be due to invalid IL or missing references)
                //IL_0035: Invalid comparison between Unknown and I4
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid == Guid.Empty)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8))
                {
                    return false;
                }
                if (OnlineCommandHelper.CanDownload(activeAppObjectGuid) && ((int)InternalCodeStateProvider.InternalCodeState == 1 || (int)InternalCodeStateProvider.InternalCodeState == 2))
                {
                    return true;
                }
                return false;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

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
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                if (arguments == null)
                {
                    throw new ArgumentNullException("arguments");
                }
                if (arguments.Length != 0)
                {
                    throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
                }
                InternalCodeStateProvider.StopOnlineCodeStateUpdates();
                ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (!(activeAppObjectGuid != Guid.Empty) || !OnlineCommandHelper.CanDownload(activeAppObjectGuid))
                {
                    return;
                }
                bool flag = OnlineCommandHelper.IsUpToDate(activeAppObjectGuid);
                IOnlineApplication application = OnlineCommandHelper.GetApplication(activeAppObjectGuid);
                if (application.IsLoggedIn && application.DataId == Guid.Empty && application.CodeId == Guid.Empty)
                {
                    if (application.ApplicationSessionId == 0)
                    {
                        string parentName = OnlineCommandHelper.GetParentName(activeAppObjectGuid);
                        application.CreateAppOnDevice(parentName);
                        application.Login(false);
                    }
                    flag = false;
                }
                if (flag)
                {
                    APEnvironment.MessageService.Information(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ProgramNotChangedDownload"), "ProgramNotChangedDownload", Array.Empty<object>());
                }
                else
                {
                    OnlineCommandHelper.Download(activeAppObjectGuid, bOnlineChange: false);
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
                Guid activeAppObjectGuid2 = OnlineCommandHelper.ActiveAppObjectGuid;
                if (ex2.GetType() == typeof(OnlineManagerException) && ((IEngine)APEnvironment.Engine).MessageService is IMessageService5 && activeAppObjectGuid2 != Guid.Empty)
                {
                    IMessageService messageService = ((IEngine)APEnvironment.Engine).MessageService;
                    ((IMessageService5)((messageService is IMessageService5) ? messageService : null)).ErrorWithDetails(ex2.Message, (EventHandler)OnlineCommandHelper.UnknownLoginErrorDetailsClickHandler, (EventArgs)new OnlineCommandHelper.UnknownLoginErrorEventArgs(activeAppObjectGuid2), "UnknownErrorOnLogin", Array.Empty<object>());
                }
                else
                {
                    APEnvironment.MessageService.Error(ex2.Message, "ErrorDownloadActiveApp", Array.Empty<object>());
                }
            }
            finally
            {
                InternalCodeStateProvider.StartOnlineCodeStateUpdates();
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
                if (OnlineCommandHelper.PromptExecuteOperation_ActiveApplication((ICommand)(object)this, bPromptInNormalMode: true, null))
                {
                    return new string[0];
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
