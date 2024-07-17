using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{99ff1305-c130-43dc-9fe6-ac5ecb436359}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_disconnect_from_device.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Disconnect_from_Device.htm")]
    public class DisconnectFromDeviceCommand : ConnectToDeviceCommand
    {
        private static readonly string[] BATCH_COMMAND_DISCONNECT = new string[2] { "online", "disconnectDevice" };

        public override string[] BatchCommand => BATCH_COMMAND_DISCONNECT;

        public override string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DisconnectFromDeviceDescription");

        public override string Name
        {
            get
            {
                IDeviceObject selectedDevice = GetSelectedDevice();
                string arg = "";
                if (selectedDevice != null)
                {
                    arg = ((IObject)selectedDevice).MetaObject.Name;
                }
                return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "DisconnectFromDeviceName"), arg);
            }
        }

        protected override bool IsEnabled(IOnlineDevice onlineDevice)
        {
            return onlineDevice.IsConnected;
        }

        protected override void Execute(IOnlineDevice onlineDevice)
        {
            onlineDevice.Disconnect();
        }
    }
}
