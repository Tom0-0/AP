using _3S.CoDeSys.Core.Device;
using System;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
    internal class PLCopenImport
    {
        internal Guid Import(int nProjectHandle, Guid parentGuid, string deviceName, Device device)
        {
            IDeviceIdentification val = null;
            val = CreateDeviceIdentification(device.DeviceType.Item);
            try
            {
                return new DeviceBuilder().CreateDevice(nProjectHandle, parentGuid, val, deviceName, device);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static IDeviceIdentification CreateDeviceIdentification(DeviceIdentificationType identType)
        {
            IDeviceIdentification val = null;
            string version = identType.Version;
            if (identType is ModuleIdentificationType)
            {
                return (IDeviceIdentification)(object)((IDeviceRepository)APEnvironment.DeviceRepository).CreateModuleIdentification(identType.Type, identType.Id, version, ((ModuleIdentificationType)identType).ModuleId);
            }
            return ((IDeviceRepository)APEnvironment.DeviceRepository).CreateDeviceIdentification(identType.Type, identType.Id, version);
        }
    }
}
