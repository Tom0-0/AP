using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{ecc6fbeb-d37f-4171-887b-c07635bb6302}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_reset_origin.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Reset_Origin.htm")]
    public class ResetOriginActiveApplicationCommand : ResetActiveApplicationCommand
    {
        private static string[] BATCH_COMMAND = new string[2] { "online", "resetoriginactiveapp" };

        public override string[] BatchCommand => BATCH_COMMAND;

        public override string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetOriginSelectedApplicationCmd_Name");

        public override string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetOriginSelectedApplicationCmd_Description");

        public override ResetOption ResetOption => (ResetOption)2;
    }
}
