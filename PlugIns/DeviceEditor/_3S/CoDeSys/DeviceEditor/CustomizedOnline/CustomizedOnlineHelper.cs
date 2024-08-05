using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor.CustomizedOnline
{
	internal class CustomizedOnlineHelper
	{
		public static bool HasCustomizedOnlineFunctionality(IDeviceObject deviceObject)
		{
			if (deviceObject == null)
			{
				return false;
			}
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceObject.DeviceIdentification);
			return LocalTargetSettings.CustomizedOnlineMgr.GetBoolValue(targetSettingsById);
		}
	}
}
