using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{b534bc77-e731-47a3-8a5e-93c57ed3e2a2}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_start.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/start.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Start_application.htm")]
    public class StartActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "startactiveapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "StartActiveApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "StartActiveApplicationCommand_Name");

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)8))
                {
                    return OnlineCommandHelper.CanStart(activeAppObjectGuid);
                }
                return false;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.StartSmall.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "StartActiveApplicationCommand_ContextlessName");

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
                if (OnlineCommandHelper.PromptExecuteOperation_ActiveApplication((ICommand)(object)this, bPromptInNormalMode: false, null))
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
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return;
            }
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
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanStart(activeAppObjectGuid))
                {
                    OnlineCommandHelper.Start(activeAppObjectGuid);
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
                APEnvironment.MessageService.Error(ex2.Message, "ErrorStartActiveApp", Array.Empty<object>());
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }
    }
}
