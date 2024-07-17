using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.TargetSettings;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{B147E1EC-68B5-49ED-ADBC-A57AF329ADE4}")]
    internal class LocalTargetSettings : ITargetSettingsUser
    {
        internal static IRegisteredTargetSetting RuntimeVersion => APEnvironment.TargetSettingsProvider.GetSetting("runtime_identification\\version");

        internal static IRegisteredTargetSetting SourceDownloadAllowed => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\source_download_allowed");

        internal static IRegisteredTargetSetting MaxSourceDownloadSize => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\max_source_download_size");

        internal static IRegisteredTargetSetting InteractiveLoginWink => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\interactive_login_wink");

        internal static IRegisteredTargetSetting LoginWithOutdatedCodeAllowed => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\login_with_outdated_code_allowed");

        internal static IRegisteredTargetSetting OnlineChangeSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\online_change_supported");

        internal static IRegisteredTargetSetting OnlyExplicitFeaturesSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\only_explicit_features_supported");

        internal static IRegisteredTargetSetting CoreApplicationHandlingSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\core_application_handling_supported");

        internal static IRegisteredTargetSetting BootApplicationSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\boot_application_supported");

        internal static IRegisteredTargetSetting ForceVariablesSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\force_variables_supported");

        internal static IRegisteredTargetSetting WriteVariablesSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\write_variables_supported");

        internal static IRegisteredTargetSetting ConnectDeviceSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\connect_device_supported");

        internal static IRegisteredTargetSetting FileTransferSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\file_transfer_supported");

        internal static IRegisteredTargetSetting BreakpointsSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\breakpoints_supported");

        internal static IRegisteredTargetSetting ConditionalBreakpointsSupported => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\conditional_breakpoints_supported");

        internal static IRegisteredTargetSetting CompactDownload => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\compact_download");

        internal static IRegisteredTargetSetting MaxNumberOfApps => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\max_number_of_apps");

        internal static IRegisteredTargetSetting CloseWindowsOnBootproject => APEnvironment.TargetSettingsProvider.GetSetting("online\\close_windows_on_bootproject");

        internal static IRegisteredTargetSetting PreserveApplication => APEnvironment.TargetSettingsProvider.GetSetting("online\\download\\preserve-application");

        internal static IRegisteredTargetSetting SimulationDisabled => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\simulation-disabled");
    }
}
