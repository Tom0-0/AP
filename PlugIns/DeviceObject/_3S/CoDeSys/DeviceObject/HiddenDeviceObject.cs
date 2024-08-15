using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{DF661E2D-6067-4fd0-A99C-DCB8F63F8078}")]
	[StorageVersion("3.3.0.0")]
	public class HiddenDeviceObject : DeviceObject, IHiddenObject
	{
		public HiddenDeviceObject()
		{
		}

		public HiddenDeviceObject(bool bCreateBitChannels, DeviceIdentification id)
			: base(bCreateBitChannels, (IDeviceIdentification)(object)id)
		{
		}

		private HiddenDeviceObject(HiddenDeviceObject hdo)
			: base(hdo)
		{
		}

		public override object Clone()
		{
			HiddenDeviceObject hiddenDeviceObject = new HiddenDeviceObject(this);
			((GenericObject)hiddenDeviceObject).AfterClone();
			return hiddenDeviceObject;
		}
	}
}
