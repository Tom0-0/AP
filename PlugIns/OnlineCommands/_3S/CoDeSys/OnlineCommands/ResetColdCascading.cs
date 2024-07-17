using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{E43329CA-CE4D-4368-B345-733B89FA4557}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_reset_cold_all_applications.htm")]
    public class ResetColdCascading : ResetCascading
    {
        private readonly string BATCH_COMMAND_VERB = "resetColdCascading";

        public override string BatchCommandVerb => BATCH_COMMAND_VERB;

        public override string Description => Strings.ResetColdCascadingCommand_Description;

        public override string Name => Strings.ResetColdCascadingCommand_Name;

        public override ResetOption ResetOption => (ResetOption)1;
    }
}
