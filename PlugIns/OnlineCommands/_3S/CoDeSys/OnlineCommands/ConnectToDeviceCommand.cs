#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{f112e292-5944-49c0-bf7f-e67658573eb9}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_connect_to_device.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Connect_to_Device.htm")]
    public class ConnectToDeviceCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND_CONNECT = new string[2] { "online", "connectDevice" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => "";

        public Icon LargeIcon => null;

        public virtual string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ConnectToDeviceDescription");

        public virtual string Name
        {
            get
            {
                IDeviceObject selectedDevice = GetSelectedDevice();
                string arg = "";
                if (selectedDevice != null)
                {
                    arg = ((IObject)selectedDevice).MetaObject.Name;
                }
                return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ConnectToDeviceName"), arg);
            }
        }

        public bool Enabled
        {
            get
            {
                IDeviceObject selectedDevice = GetSelectedDevice();
                if (selectedDevice == null)
                {
                    return false;
                }
                if (!selectedDevice.AllowsDirectCommunication)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)6))
                {
                    return false;
                }
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)selectedDevice).MetaObject.ObjectGuid);
                if (onlineDevice == null)
                {
                    return false;
                }
                return IsEnabled(onlineDevice);
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public virtual string[] BatchCommand => BATCH_COMMAND_CONNECT;

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            IDeviceObject selectedDevice = GetSelectedDevice();
            Debug.Assert(selectedDevice != null);
            return new string[1] { ((IObject)selectedDevice).MetaObject.ObjectGuid.ToString() };
        }

        public bool IsVisible(bool bContextMenu)
        {
            Guid ObjectGuid = Guid.Empty;
            if (bContextMenu && ((IEngine)APEnvironment.Engine).Frame != null && ((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl && SelectedNodeIsDeviceObject(out ObjectGuid))
            {
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(ObjectGuid);
                if (onlineDevice != null && IsEnabled(onlineDevice))
                {
                    return true;
                }
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0019: Unknown result type (might be due to invalid IL or missing references)
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0061: Unknown result type (might be due to invalid IL or missing references)
            //IL_0092: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return;
            }
            if (arguments.Length < 1)
            {
                throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, 1);
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 1);
            }
            ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
            Guid guid;
            try
            {
                guid = new Guid(arguments[0]);
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
                throw new BatchWrongArgumentTypeException(BatchCommand, 0, "Guid");
            }
            try
            {
                _bIsExecutionInProgress = true;
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(guid);
                if (onlineDevice != null)
                {
                    Execute(onlineDevice);
                    return;
                }
                throw new BatchExecutionException(BatchCommand, "Invalid device object guid");
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        protected virtual bool IsEnabled(IOnlineDevice onlineDevice)
        {
            return !onlineDevice.IsConnected;
        }

        protected virtual void Execute(IOnlineDevice onlineDevice)
        {
            onlineDevice.Connect();
        }

        protected bool SelectedNodeIsDeviceObject(out Guid ObjectGuid)
        {
            ObjectGuid = Guid.Empty;
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
                .Length == 1 && typeof(IDeviceObject).IsAssignableFrom(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[0].GetMetaObjectStub().ObjectType))
            {
                ObjectGuid = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[0].ObjectGuid;
                return true;
            }
            return false;
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
