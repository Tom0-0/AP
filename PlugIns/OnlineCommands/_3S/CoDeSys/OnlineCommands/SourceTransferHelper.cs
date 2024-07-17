using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class SourceTransferHelper
    {
        internal const string SOURCE_ARCHIVE_FILE = "Archive.prj";

        internal const string SOURCE_UPLOAD_FILE_ARCHIVE = "ArchiveUpload.prj";

        internal const string PLC_LOGIC_PLACEHOLDER_DOLLAR = "$PlcLogic$";

        internal static readonly Version RTS_VERSION_3570 = new Version("3.5.7.0");

        internal static readonly Version RTS_VERSION_3580 = new Version("3.5.8.0");

        internal static readonly Guid GUID_PAC_OPTIONS = new Guid("{9A035D59-E636-4a81-AF5D-B193334740A9}");

        internal static Version GetRtsVersionOnline(IOnlineDevice connectedOnlineDevice)
        {
            //IL_001c: Unknown result type (might be due to invalid IL or missing references)
            Version result = new Version("0.0.0.0");
            if (connectedOnlineDevice is IOnlineDevice18 && connectedOnlineDevice.IsConnected)
            {
                try
                {
                    string text = default(string);
                    Version version = default(Version);
                    ((IOnlineDevice18)connectedOnlineDevice).GetTargetIdent(out text, out text, out text, out version);
                    result = version;
                    return result;
                }
                catch
                {
                    return result;
                }
            }
            return result;
        }

        internal static Version GetRtsVersionFromDevdesc(Guid guidDeviceObject)
        {
            Version result = new Version("0.0.0.0");
            try
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
                {
                    if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidDeviceObject))
                    {
                        return result;
                    }
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidDeviceObject).Object;
                    IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
                    if (val != null)
                    {
                        ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentification);
                        if (targetSettingsById != null)
                        {
                            if (LocalTargetSettings.SourceDownloadAllowed.GetBoolValue(targetSettingsById))
                            {
                                result = new Version(LocalTargetSettings.RuntimeVersion.GetStringValue(targetSettingsById));
                                return result;
                            }
                            return result;
                        }
                        return result;
                    }
                    return result;
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
