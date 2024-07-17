using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{3E908575-08B7-4d45-A7E0-3A74A456C403}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_flowcontrol.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/Flow_control.htm")]
    public class FlowControlActiveApplicationCommand : IToggleCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "flowcontrolactiveapp" };

        private bool _bIsExecutionInProgress;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "FlowControlActiveApplicationCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "FlowControlActiveApplicationCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(FlowControlActiveApplicationCommand), "_3S.CoDeSys.OnlineCommands.Resources.FlowControlSmall.ico");

        public Icon LargeIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(FlowControlActiveApplicationCommand), "_3S.CoDeSys.OnlineCommands.Resources.FlowControlLarge.ico");

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanFlowControl(activeAppObjectGuid))
                {
                    return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)19);
                }
                return false;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "FlowControlActiveApplicationCommand_ContextlessName");

        public bool RadioCheck => false;

        public bool Checked
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanFlowControl(activeAppObjectGuid))
                {
                    return OnlineCommandHelper.IsFlowControlEnabled(activeAppObjectGuid);
                }
                return false;
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

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            return !bContextMenu;
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
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanFlowControl(activeAppObjectGuid))
                {
                    OnlineCommandHelper.ToggleFlowControl(activeAppObjectGuid);
                }
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "ErrorFlowControlActiveApp", Array.Empty<object>());
            }
        }
    }
}
