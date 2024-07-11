using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.TargetSettings;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{834FB67F-9445-45A4-AAF9-1558EFD0B921}")]
	internal class LocalTargetSettings : ITargetSettingsUser
	{
		internal static IRegisteredTargetSetting ByteAddressing => APEnvironment.TargetSettingsProvider.GetSetting("memory-layout\\byte-addressing");

		internal static IRegisteredTargetSetting RuntimeVersion => APEnvironment.TargetSettingsProvider.GetSetting("runtime_identification\\version");
	}
}
