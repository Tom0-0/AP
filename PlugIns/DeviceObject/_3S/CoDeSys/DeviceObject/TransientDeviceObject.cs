using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{D009E832-CCA3-41ce-AD32-3FCD00A7AC73}")]
	[StorageVersion("3.3.0.0")]
	public class TransientDeviceObject : DeviceObject, ITransientObject
	{
		public TransientDeviceObject()
		{
		}

		public TransientDeviceObject(bool bCreateBitChannels, DeviceIdentification id)
			: base(bCreateBitChannels, (IDeviceIdentification)(object)id)
		{
		}

		private TransientDeviceObject(TransientDeviceObject hdo)
			: base(hdo)
		{
		}

		public override object Clone()
		{
			TransientDeviceObject transientDeviceObject = new TransientDeviceObject(this);
			((GenericObject)transientDeviceObject).AfterClone();
			return transientDeviceObject;
		}
	}
}
