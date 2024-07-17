using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{6A2E1650-7459-4a0f-8E38-45EE2E80CD95}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_display_mode.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Display_Mode.htm")]
    public class DisplayModeDecimalCommand : AbstractDisplayModeCommand
    {
        public DisplayModeDecimalCommand()
            : base(OptionsHelper.DISPLAYMODE_DECIMAL, "dec")
        {
        }
    }
}
