using _3S.CoDeSys.Core.Components;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7C73CE4B-773F-48c2-A450-3A6C245B4E30}")]
	public class ConstraintCatalogueFilter : ConstraintHelper, IDeviceCatalogueFilter2, IDeviceCatalogueFilter
	{
		internal static string ConstraintFilter = "{7C73CE4B-773F-48c2-A450-3A6C245B4E30}";

		public bool Match(IDeviceDescription device)
		{
			bool bUseDefaultFilter;
			return Match(device, out bUseDefaultFilter);
		}

		public bool Match(IDeviceDescription device, out bool bUseDefaultFilter)
		{
			return Match(device, out bUseDefaultFilter, bCheckParent: false, bUpdate: false);
		}
	}
}
