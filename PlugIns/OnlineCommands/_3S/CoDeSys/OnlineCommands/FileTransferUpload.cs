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
    [TypeGuid("{B099ED07-D6D3-4e2b-AED2-BF15B59C2F35}")]
    [SuppressMessage("AP.Patterns", "Pat002", Justification = "No F1 help available")]
    public class FileTransferUpload : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND_FILE_UPLOAD = new string[2] { "online", "fileUpload" };

        public string ToolTipText => null;

        public Icon LargeIcon => null;

        public string Description => null;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "FileTransferUpload");

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

        public string[] BatchCommand => BATCH_COMMAND_FILE_UPLOAD;

        public object AsyncState => null;

        public bool CompletedSynchronously => false;

        public WaitHandle AsyncWaitHandle => null;

        public bool IsCompleted => false;

        public string[] CreateBatchArguments()
        {
            return new string[2] { "testclient2.txt", "testplc.txt" };
        }

        public bool IsVisible(bool bContextMenu)
        {
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_006b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0072: Expected O, but got Unknown
            //IL_0095: Unknown result type (might be due to invalid IL or missing references)
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
            if (val != null)
            {
                IFileUpload val2;
                try
                {
                    Stream stream = (Stream)new AuthFileStream(arguments[0], FileMode.Create);
                    string text = arguments[1];
                    val2 = val.CreateFileUpload(stream, text);
                }
                catch (Exception value)
                {
                    Debug.WriteLine(value);
                    throw new BatchWrongArgumentTypeException(BatchCommand, 0, "***");
                }
                AsyncCallback asyncCallback = UploadCallback;
                val2.BeginUpload(true, asyncCallback, (object)null);
            }
        }

        private void UploadCallback(IAsyncResult asyncResult)
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
