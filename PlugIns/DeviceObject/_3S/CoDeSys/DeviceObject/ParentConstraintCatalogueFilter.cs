using _3S.CoDeSys.Core.Components;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{B0CA794F-1A9A-4ad4-8C93-8B06926AC77C}")]
	public class ParentConstraintCatalogueFilter : ConstraintHelper, IDeviceCatalogueFilter2, IDeviceCatalogueFilter
	{
		internal static string ConstraintFilter = "{B0CA794F-1A9A-4ad4-8C93-8B06926AC77C}";

		public bool Match(IDeviceDescription device)
		{
			bool bUseDefaultFilter;
			return Match(device, out bUseDefaultFilter);
		}

		public bool Match(IDeviceDescription device, out bool bUseDefaultFilter)
		{
			return Match(device, out bUseDefaultFilter, bCheckParent: true, bUpdate: false);
		}
	}
}
