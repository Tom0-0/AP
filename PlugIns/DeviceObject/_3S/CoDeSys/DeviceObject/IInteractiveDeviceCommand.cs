using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
    public interface IInteractiveDeviceCommand
    {
        string Name { get; }

        bool Selectable { get; }

        bool GetFilterAndContext(out string[] filter, out IDeviceIdentification context, out IDeviceIdentification[] AllowOnlyDevices);

        string GetFixObjectName();

        string[] CreateBatchArguments(IDeviceIdentification deviceId, string stName, params object[] list);

        void OverridableExecuteBatch(string[] args);

        string GetOnlineHelpUrl();
    }
}
