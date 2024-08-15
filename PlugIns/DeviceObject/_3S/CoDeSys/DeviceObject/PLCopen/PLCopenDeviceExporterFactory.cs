using _3S.CoDeSys.Core.Components;
using System.Collections.Generic;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
    [TypeGuid("{034EF58E-3556-48c3-A4FF-38B0B25125C0}")]
    internal class PLCopenDeviceExporterFactory : IDeviceExporterFactory
    {
        private List<IDeviceExporter> root = new List<IDeviceExporter>();

        public IDeviceExporter CreateExporter(string format)
        {
            if (format == "PLCopen")
            {
                return (IDeviceExporter)(object)new PLCopenDeviceExporter();
            }
            return null;
        }
    }
}
