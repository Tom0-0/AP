using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{d887b9d2-d1ed-4dd9-9aba-dd796ec80b4b}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_reset_warm.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Reset_Warm.htm")]
    public class ResetWarmActiveApplicationCommand : ResetActiveApplicationCommand
    {
        private static string[] BATCH_COMMAND = new string[2] { "online", "resetwarmactiveapp" };

        public override string[] BatchCommand => BATCH_COMMAND;

        public override string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetWarmSelectedApplicationCmd_Name");

        public override string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetWarmSelectedApplicationCmd_Description");

        public override ResetOption ResetOption => (ResetOption)0;
    }
}
