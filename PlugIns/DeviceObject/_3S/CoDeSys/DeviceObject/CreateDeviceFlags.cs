using System;

namespace _3S.CoDeSys.DeviceObject
{
	[Flags]
	public enum CreateDeviceFlags
	{
		None = 0x0,
		UseModuleId = 0x1,
		HiddenDevice = 0x2,
		TransientDevice = 0x4
	}
}
