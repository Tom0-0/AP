using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{285207b7-3503-4338-9c8d-b73c42d99ee3}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_single_cycle.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Single_Cycle.htm")]
    public class SingleCycleActiveApplicationCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "singlecycleactiveapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SingleCycleActiveApplicationCommand_Description");

        public string Name => string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SingleCycleActiveApplicationCommand_Name"), OnlineCommandHelper.ActiveAppName);

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8) && OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)9))
                {
                    return OnlineCommandHelper.CanSingleCycle(activeAppObjectGuid);
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

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
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
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
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanSingleCycle(activeAppObjectGuid))
                {
                    OnlineCommandHelper.SingleCycle(activeAppObjectGuid);
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
                APEnvironment.MessageService.Error(ex2.Message, "ErrorSingleCycleActiveApp", Array.Empty<object>());
            }
        }
    }
}
