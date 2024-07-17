using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{EC6352E2-715A-4349-8004-75B09FD3ADC0}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_reset_warm_all_applications.htm")]
    public class ResetWarmCascading : ResetCascading
    {
        private readonly string BATCH_COMMAND_VERB = "resetWarmCascading";

        public override string BatchCommandVerb => BATCH_COMMAND_VERB;

        public override string Description => Strings.ResetWarmCascadingCommand_Description;

        public override string Name => Strings.ResetWarmCascadingCommand_Name;

        public override ResetOption ResetOption => (ResetOption)0;
    }
}
