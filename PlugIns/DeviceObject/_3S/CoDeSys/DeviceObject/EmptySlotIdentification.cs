using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
	internal class EmptySlotIdentification : IDeviceIdentification
	{
		public string Id => "0000 0000";

		public string Version => "0.0.0.0";

		public int Type => 0;
	}
}
