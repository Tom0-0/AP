using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{63406A8B-DD43-4FB3-8F16-C270C15F5558}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_stop_execution_on_handled_exceptions.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/cds_cmd_stop_execution_on_handled_exceptions.htm")]
    public class HaltOnExceptionsCommand : IToggleCommand, ICommand
    {
        public bool RadioCheck => false;

        public bool Checked
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && InternalCodeStateProvider.GetApplicationExceptionState(activeAppObjectGuid, out var uiState))
                {
                    return uiState == 1;
                }
                return false;
            }
        }

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => Strings.HaltOnExceptionCommand_Name;

        public string Description => Strings.HaltOnExceptionCommand_Description;

        public string ToolTipText => Description;

        public Icon SmallIcon => null;

        public Icon LargeIcon => null;

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                uint uiState;
                if (activeAppObjectGuid != Guid.Empty)
                {
                    return InternalCodeStateProvider.GetApplicationExceptionState(activeAppObjectGuid, out uiState);
                }
                return false;
            }
        }

        public string[] BatchCommand => new string[0];

        public string[] CreateBatchArguments()
        {
            return new string[0];
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
            Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
            if (InternalCodeStateProvider.GetApplicationExceptionState(activeAppObjectGuid, out var uiState))
            {
                uiState = ((uiState == 0) ? 1u : 0u);
                InternalCodeStateProvider.SetApplicationExceptionState(activeAppObjectGuid, uiState);
            }
        }
    }
}
