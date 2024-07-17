using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{F6B049E7-6C27-4736-9A45-4CC93C3E8C9D}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_online_change.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Online_Change.htm")]
    public class OnlineChangeActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "onlinechangeactiveapp" };

        private bool _bIsExecutionInProgress;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "OnlineChangeActiveApplicationCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "OnlineChangeActiveApplicationCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                //IL_002f: Unknown result type (might be due to invalid IL or missing references)
                //IL_0035: Invalid comparison between Unknown and I4
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid == Guid.Empty)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)2))
                {
                    return false;
                }
                if (OnlineCommandHelper.CanDownload(activeAppObjectGuid) && OnlineCommandHelper.OnlineChangeSupported(activeAppObjectGuid) && (int)InternalCodeStateProvider.InternalCodeState == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => Strings.OnlineChangeActiveApplicationCommand_ContextlessName;

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
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)2);
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
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanDownload(activeAppObjectGuid))
                {
                    if (OnlineCommandHelper.IsUpToDate(activeAppObjectGuid))
                    {
                        APEnvironment.MessageService.Information(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ProgramNotChangedOnlChange"), "ProgramNotChangedOnlChangeActiveApp", Array.Empty<object>());
                    }
                    else
                    {
                        OnlineCommandHelper.Download(activeAppObjectGuid, bOnlineChange: true);
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
                APEnvironment.MessageService.Error(ex2.Message, "ErrorOnlneChangeActiveApp", Array.Empty<object>());
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
