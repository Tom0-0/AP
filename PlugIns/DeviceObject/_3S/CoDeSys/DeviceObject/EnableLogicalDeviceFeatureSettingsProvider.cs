using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{641D8B97-4B01-4803-B1BA-A648879A1584}")]
	public class EnableLogicalDeviceFeatureSettingsProvider : IFeatureSettingProvider
	{
		public string Id => "enable-logicaldevices";

		public Guid GroupProvider => new Guid("{398CB3B5-CD61-4e3b-8FB0-DE23E9A4769A}");

		public string Name => Strings.FeatureLogicalDeviceName;

		public string Description => Strings.FeatureLogicalDeviceDescription;

		public bool DefaultValue => false;

		public FeatureSettingAccess Access => (FeatureSettingAccess)0;

		public bool HookBeforeSet(bool bValue)
		{
			return bValue;
		}

		public void HookAfterSet(bool bValue)
		{
			DeviceObjectHelper.FeatureChanged();
		}

		public bool HookGet(bool bValue)
		{
			return bValue;
		}
	}
}
