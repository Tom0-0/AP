using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{c25ccb8f-a743-4cbb-9ff9-697838f49e62}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_reset_cold.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Reset_Cold.htm")]
    public class ResetColdActiveApplicationCommand : ResetActiveApplicationCommand
    {
        private static string[] BATCH_COMMAND = new string[2] { "online", "resetcoldactiveapp" };

        public override string[] BatchCommand => BATCH_COMMAND;

        public override string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetColdSelectedApplicationCmd_Name");

        public override string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetColdSelectedApplicationCmd_Description");

        public override ResetOption ResetOption => (ResetOption)1;
    }
}
