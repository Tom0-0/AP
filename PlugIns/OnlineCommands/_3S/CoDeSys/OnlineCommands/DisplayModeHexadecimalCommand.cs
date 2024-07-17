using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{B8B0B06C-DAF4-4fa8-9EC2-5826BD8E15C6}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_display_mode.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Display_Mode.htm")]
    public class DisplayModeHexadecimalCommand : AbstractDisplayModeCommand
    {
        public DisplayModeHexadecimalCommand()
            : base(OptionsHelper.DISPLAYMODE_HEXADECIMAL, "hex")
        {
        }
    }
}
