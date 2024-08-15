using System;
using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceIdUpdate
	{
		private int _nProjectHandle;

		private Guid _guidObject;

		private IDeviceIdentification _deviceId;

		internal int ProjectHandle => _nProjectHandle;

		internal Guid ObjectGuid => _guidObject;

		internal IDeviceIdentification DeviceId => _deviceId;

		public DeviceIdUpdate(int nProjectHandle, Guid guidObject, IDeviceIdentification deviceId)
		{
			_nProjectHandle = nProjectHandle;
			_guidObject = guidObject;
			_deviceId = deviceId;
		}
	}
}
