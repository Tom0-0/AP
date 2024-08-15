using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.TargetSettings;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{E9AA1506-B447-4B49-8B30-E6623C887B7E}")]
	internal class LocalTargetSettings : ITargetSettingsUser
	{
		internal static IRegisteredTargetSetting SetActivePathForChild => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\set_active_path_for_child");

		internal static IRegisteredTargetSetting OptimizeIoUpdate => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\optimize_io_update");

		internal static IRegisteredTargetSetting MaxNumberOfApps => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\max_number_of_apps");

		internal static IRegisteredTargetSetting FixedAppName => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\fixed_app_name");

		internal static IRegisteredTargetSetting InteractiveLoginMode => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\interactive_login_mode");

		internal static IRegisteredTargetSetting CycleControlInIec => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\cycle_control_in_iec");

		internal static IRegisteredTargetSetting DeviceApplicationDisabled => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\disable_device_application");

		internal static IRegisteredTargetSetting PreselectEncryptedCommunication => APEnvironment.TargetSettingsProvider.GetSetting("runtime_features\\preselect_encrypted_communication");

		internal static IRegisteredTargetSetting MonitoringInterval => APEnvironment.TargetSettingsProvider.GetSetting("onlinemanager\\monitoring-interval");

		internal static IRegisteredTargetSetting TraceManager => APEnvironment.TargetSettingsProvider.GetSetting("trace\\tracemanager");

		internal static IRegisteredTargetSetting NoPrecompileMessages => APEnvironment.TargetSettingsProvider.GetSetting("codegenerator\\no-precompile-messages");

		internal static IRegisteredTargetSetting PackMode => APEnvironment.TargetSettingsProvider.GetSetting("memory-layout\\pack-mode");

		internal static IRegisteredTargetSetting AddressAssignmentGuid => APEnvironment.TargetSettingsProvider.GetSetting("memory-layout\\address-assignment-guid");

		internal static IRegisteredTargetSetting ByteAddressing => APEnvironment.TargetSettingsProvider.GetSetting("memory-layout\\byte-addressing");

		internal static IRegisteredTargetSetting BitWordAddressing => APEnvironment.TargetSettingsProvider.GetSetting("memory-layout\\bit-word-addressing");

		internal static IRegisteredTargetSetting MinimalStructureGranularity => APEnvironment.TargetSettingsProvider.GetSetting("memory-layout\\minimal-structure-granularity");

		internal static IRegisteredTargetSetting UpdateOnlyDeviceVersion => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\update-only-device-version");

		internal static IRegisteredTargetSetting MaxLogicalInputBitSize => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\maxLogicalInputBitSize");

		internal static IRegisteredTargetSetting MaxLogicalOutputBitSize => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\maxLogicalOutputBitSize");

		internal static IRegisteredTargetSetting MultipleMappableAllowed => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Multiple-mappable-allowed");

		internal static IRegisteredTargetSetting UnionRootEditable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Union-root-editable");

		internal static IRegisteredTargetSetting BasetypeMappable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Basetype-mappable");

		internal static IRegisteredTargetSetting BitfieldMappable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Bitfield-mappable");

		internal static IRegisteredTargetSetting UpdateAllToEqualVersion => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\updateAllToEqualVersion");

		internal static IRegisteredTargetSetting ShowMultipleTaskMappingsAsError => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\ShowMultipleTaskMappingsAsError");

		internal static IRegisteredTargetSetting MotorolaBitfields => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Motorola-bitfields");

		internal static IRegisteredTargetSetting EnableAdditionalParameters => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\enableAdditionalParameters");

		internal static IRegisteredTargetSetting SkipAdditionalParametersForEmptyConnectors => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\skipAdditionalParametersForEmptyConnectors");

		internal static IRegisteredTargetSetting CreateBitChannels => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\createBitChannels");

		internal static IRegisteredTargetSetting HideIoFbInstances => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\hideIoFbInstances");

		internal static IRegisteredTargetSetting MapAlwaysIecAddress => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\MapAlwaysIecAddress");

		internal static IRegisteredTargetSetting CreateInstanceVariables => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\createInstanceVariables");

		internal static IRegisteredTargetSetting DefaultTaskPriority => APEnvironment.TargetSettingsProvider.GetSetting("taskconfiguration\\defaulttaskpriority");

		internal static IRegisteredTargetSetting CycleTimeDefault => APEnvironment.TargetSettingsProvider.GetSetting("taskconfiguration\\cycletimedefault");

		internal static IRegisteredTargetSetting LogicalmapApplicationVisible => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logicalmapApplicationVisible");

		internal static IRegisteredTargetSetting DisableChildApp => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\disableChildApp");

		internal static IRegisteredTargetSetting TaskName => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\taskName");

		internal static IRegisteredTargetSetting TaskPou => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\taskPou");

		internal static IRegisteredTargetSetting TaskEventEnable => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\taskEventEnable");

		internal static IRegisteredTargetSetting TaskEvent => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\taskEvent");

		internal static IRegisteredTargetSetting TaskInterval => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\taskInterval");

		internal static IRegisteredTargetSetting TaskPriority => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\taskPriority");

		internal static IRegisteredTargetSetting WatchdogEnable => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\watchdogEnable");

		internal static IRegisteredTargetSetting WatchDogSensitivity => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\watchDogSensitivity");

		internal static IRegisteredTargetSetting WatchdogTime => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\watchdogTime");

		internal static IRegisteredTargetSetting WatchdogTimeUnit => APEnvironment.TargetSettingsProvider.GetSetting("logical-devices\\logical-taskconfig\\watchdogTimeUnit");

		internal static IRegisteredTargetSetting PlaceholderLibraries => APEnvironment.TargetSettingsProvider.GetSetting("library-management\\placeholder-libraries");

		internal static IRegisteredTargetSetting ProhibitDuplicatePriorities => APEnvironment.TargetSettingsProvider.GetSetting("taskconfiguration\\prohibit-duplicate-priorities");

		internal static IRegisteredTargetSetting MaxTaskPriority => APEnvironment.TargetSettingsProvider.GetSetting("taskconfiguration\\maxtaskpriority");
	}
}
