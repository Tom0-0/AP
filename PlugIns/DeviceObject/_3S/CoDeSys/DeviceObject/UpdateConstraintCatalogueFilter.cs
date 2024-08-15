using _3S.CoDeSys.Core.Components;

namespace _3S.CoDeSys.DeviceObject
{
    [TypeGuid("{12B21444-34D7-45a3-95F0-AAAA2921395E}")]
    public class UpdateConstraintCatalogueFilter : ConstraintHelper, IDeviceCatalogueFilter2, IDeviceCatalogueFilter
    {
        internal static string ConstraintFilter = "{12B21444-34D7-45a3-95F0-AAAA2921395E}";

        public bool Match(IDeviceDescription device)
        {
            bool bUseDefaultFilter;
            return Match(device, out bUseDefaultFilter);
        }

        public bool Match(IDeviceDescription device, out bool bUseDefaultFilter)
        {
            return Match(device, out bUseDefaultFilter, bCheckParent: true, bUpdate: true);
        }
    }
}
