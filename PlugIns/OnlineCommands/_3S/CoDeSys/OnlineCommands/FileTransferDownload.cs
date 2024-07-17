#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Threading;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{EDE62C94-9E06-4d0e-B713-01C9C66351EB}")]
    [SuppressMessage("AP.Patterns", "Pat002", Justification = "No F1 help available")]
    public class FileTransferDownload : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND_FILE_DOWNLOAD = new string[2] { "online", "fileDownload" };

        public string ToolTipText => null;

        public Icon LargeIcon => null;

        public string Description => null;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "FileTransferDownload");

        public bool Enabled
        {
            get
            {
                IDeviceObject selectedDevice = GetSelectedDevice();
                if (selectedDevice == null)
                {
                    return false;
                }
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)selectedDevice).MetaObject.ObjectGuid);
                if (onlineDevice == null)
                {
                    return false;
                }
                return onlineDevice.IsConnected;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND_FILE_DOWNLOAD;

        public object AsyncState => null;

        public bool CompletedSynchronously => false;

        public WaitHandle AsyncWaitHandle => null;

        public bool IsCompleted => false;

        public string[] CreateBatchArguments()
        {
            return new string[2] { "testclient.txt", "testplc.txt" };
        }

        public bool IsVisible(bool bContextMenu)
        {
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_006c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0072: Expected O, but got Unknown
            //IL_00a9: Unknown result type (might be due to invalid IL or missing references)
            if (arguments.Length < 2)
            {
                throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, 2);
            }
            if (arguments.Length > 2)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 2);
            }
            ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
            IDeviceObject selectedDevice = GetSelectedDevice();
            if (selectedDevice == null)
            {
                return;
            }
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)selectedDevice).MetaObject.ObjectGuid);
            IOnlineDevice2 val = (IOnlineDevice2)(object)((onlineDevice is IOnlineDevice2) ? onlineDevice : null);
            if (val == null)
            {
                return;
            }
            Stream stream = null;
            try
            {
                stream = (Stream)new AuthFileStream(arguments[0], FileMode.Open);
                string text = arguments[1];
                IFileDownload obj = val.CreateFileDownload(stream, text);
                AsyncCallback asyncCallback = DownloadCallback;
                obj.BeginDownload(true, asyncCallback, (object)null);
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
                throw new BatchWrongArgumentTypeException(BatchCommand, 0, "***");
            }
            finally
            {
                stream?.Close();
            }
        }

        private void DownloadCallback(IAsyncResult asyncResult)
        {
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        protected IDeviceObject GetSelectedDevice()
        {
            //IL_0054: Unknown result type (might be due to invalid IL or missing references)
            //IL_005a: Expected O, but got Unknown
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject == null)
            {
                return null;
            }
            ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
            if (selectedSVNodes == null || selectedSVNodes.Length != 1)
            {
                return null;
            }
            ISVNode val = selectedSVNodes[0];
            if (!typeof(IDeviceObject).IsAssignableFrom(val.GetMetaObjectStub().ObjectType))
            {
                return null;
            }
            return (IDeviceObject)val.GetObjectToRead().Object;
        }
    }
}
