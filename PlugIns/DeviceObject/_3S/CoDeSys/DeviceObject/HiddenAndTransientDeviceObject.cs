using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
    [TypeGuid("{E0CD50EF-A318-4448-A17B-C184E7E82B71}")]
    [StorageVersion("3.3.0.0")]
    public class HiddenAndTransientDeviceObject : DeviceObject, IHiddenObject, ITransientObject
    {
        public HiddenAndTransientDeviceObject()
        {
        }

        public HiddenAndTransientDeviceObject(bool bCreateBitChannels, DeviceIdentification id)
            : base(bCreateBitChannels, (IDeviceIdentification)(object)id)
        {
        }

        private HiddenAndTransientDeviceObject(HiddenAndTransientDeviceObject hdo)
            : base(hdo)
        {
        }

        public override object Clone()
        {
            HiddenAndTransientDeviceObject hiddenAndTransientDeviceObject = new HiddenAndTransientDeviceObject(this);
            ((GenericObject)hiddenAndTransientDeviceObject).AfterClone();
            return hiddenAndTransientDeviceObject;
        }
    }
}
