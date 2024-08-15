namespace _3S.CoDeSys.DeviceObject
{
    internal class CompositeDeviceCatalogFilter : IDeviceCatalogueFilter
    {
        internal IDeviceCatalogueFilter childConnectorFilter;

        internal IDeviceCatalogueFilter customFilter;

        public bool Match(IDeviceDescription device)
        {
            if (childConnectorFilter.Match(device))
            {
                if (customFilter != null)
                {
                    return customFilter.Match(device);
                }
                return true;
            }
            return false;
        }
    }
}
