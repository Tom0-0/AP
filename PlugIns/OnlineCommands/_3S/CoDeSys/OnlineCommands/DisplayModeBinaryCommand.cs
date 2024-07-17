using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{5F5ED4AD-72E1-4d7c-B638-2DC19ECFA5BC}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_display_mode.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Display_Mode.htm")]
    public class DisplayModeBinaryCommand : AbstractDisplayModeCommand
    {
        public DisplayModeBinaryCommand()
            : base(OptionsHelper.DISPLAYMODE_BINARY, "bin")
        {
        }
    }
}
