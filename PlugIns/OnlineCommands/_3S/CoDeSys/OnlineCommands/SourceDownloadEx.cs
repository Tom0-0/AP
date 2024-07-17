#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceEditor;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class SourceDownloadEx
    {
        public void SelectDeviceAndTriggerDownload()
        {
            ISelectDeviceService val = null;
            val = OnlineCommandHelper.GetCustomSelectDeviceProvider();
            if (val == null)
            {
                val = APEnvironment.CreateSelectDeviceService();
            }
            Debug.Assert(val != null);
            IOnlineDevice3 val2 = null;
            int num = -1;
            bool flag = false;
            string text = default(string);
            IDeviceAddress val3 = default(IDeviceAddress);
            IDeviceIdentification val4 = default(IDeviceIdentification);
            val.Invoke((IWin32Window)APEnvironment.FrameForm, out text, out val3, out val4);
            if (text == null && val3 == null && val4 == null)
            {
                return;
            }
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                num = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(num);
                foreach (Guid guid in allObjects)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(num, guid);
                    if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        continue;
                    }
                    try
                    {
                        IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(guid);
                        IOnlineDevice5 val5 = (IOnlineDevice5)(object)((onlineDevice is IOnlineDevice5) ? onlineDevice : null);
                        IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(num, guid).Object;
                        IDeviceObject val6 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
                        if (((IOnlineDevice4)val5).GetDeviceAddressInUse().Equals(val3) && val6.DeviceIdentification.Id == val4.Id && val6.DeviceIdentification.Type == val4.Type)
                        {
                            val2 = (IOnlineDevice3)(object)val5;
                            goto IL_0132;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            goto IL_0132;
        IL_0132:
            if (val2 == null)
            {
                IOnlineDevice obj2 = APEnvironment.OnlineMgr.CreateOnlineDevice(text, val3, val4, true);
                val2 = (IOnlineDevice3)(object)((obj2 is IOnlineDevice3) ? obj2 : null);
            }
            if (val2 == null)
            {
                return;
            }
            if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)1))
            {
                APEnvironment.MessageService.Warning(Strings.SourceDownloadNotSupported, "SourceDownloadNotSupported", Array.Empty<object>());
                return;
            }
            if (!((IOnlineDevice)val2).IsConnected)
            {
                flag = true;
                ((IOnlineDevice)val2).Connect();
            }
            try
            {
                new SourceDownload().DownloadToSelectedDevice((IOnlineDevice)(object)val2);
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "ErrorSelectingDeviceAndTriggerDownload", Array.Empty<object>());
            }
            if (flag)
            {
                ((IOnlineDevice)val2).Disconnect();
            }
        }
    }
}
