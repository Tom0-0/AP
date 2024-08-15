using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class UpdateInformation
	{
		internal readonly IDeviceIdentification DevId;

		internal readonly IDeviceInfo DevInfo;

		internal readonly DeviceVersionInformation[] AvailableUpdateVersions;

		internal DeviceVersionInformation SelectedUpdateVersion;

		internal readonly DeviceVersionInformation CurrentVersion;

		internal readonly IConfigVersionUpdateEnvironmentFactory Factory;

		internal UpdateInformation(IDeviceIdentification devId, IDeviceInfo devInfo, DeviceVersionInformation currentVersion, DeviceVersionInformation[] availableUpdateVersions, IConfigVersionUpdateEnvironmentFactory factory)
		{
			DevId = devId;
			CurrentVersion = currentVersion;
			DevInfo = devInfo;
			Factory = factory;
			LList<DeviceVersionInformation> val = new LList<DeviceVersionInformation>();
			for (int num = availableUpdateVersions.Length - 1; num >= 0; num--)
			{
				val.Add(availableUpdateVersions[num]);
			}
			AvailableUpdateVersions = val.ToArray();
		}
	}
}
