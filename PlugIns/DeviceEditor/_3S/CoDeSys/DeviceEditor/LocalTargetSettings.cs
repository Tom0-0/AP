using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.TargetSettings;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{97653268-1EEF-406D-8736-6EE78E7A8F42}")]
	internal class LocalTargetSettings : ITargetSettingsUser
	{
		internal static IRegisteredTargetSetting CustomizedOnlineMgr => APEnvironment.TargetSettingsProvider.GetSetting("online\\customizedonlinemgr");

		internal static IRegisteredTargetSetting MappingChangeable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\mapping-changeable");

		internal static IRegisteredTargetSetting ManualAddressAllowed => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\manual-address-allowed");

		internal static IRegisteredTargetSetting MotorolaBitfields => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Motorola-bitfields");

		internal static IRegisteredTargetSetting UnionRootEditable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Union-root-editable");

		internal static IRegisteredTargetSetting BasetypeMappable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Basetype-mappable");

		internal static IRegisteredTargetSetting BitfieldMappable => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Bitfield-mappable");

		internal static IRegisteredTargetSetting MultipleMappableAllowed => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\Multiple-mappable-allowed");

		internal static IRegisteredTargetSetting MapAlwaysIecAddress => APEnvironment.TargetSettingsProvider.GetSetting("deviceconfiguration\\MapAlwaysIecAddress");
	}
}
