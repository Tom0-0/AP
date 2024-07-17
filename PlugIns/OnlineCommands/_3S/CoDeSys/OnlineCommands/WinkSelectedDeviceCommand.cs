using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{B5B39990-DA62-4568-9C64-D27A0C89680F}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_wink.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/wink.htm")]
    public class WinkSelectedDeviceCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "wink" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => Strings.CmdWink_Descr;

        public string Name => Strings.CmdWink_Name;

        public bool Enabled
        {
            get
            {
                INavigatorControl navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null;
                if (navigatorControl != null && navigatorControl.SelectedSVNodes != null && navigatorControl.SelectedSVNodes.Length == 1)
                {
                    IMetaObjectStub metaObjectStub = navigatorControl.SelectedSVNodes[0].GetMetaObjectStub();
                    if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        IDeviceObject deviceObject = APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid).Object as IDeviceObject;
                        if (deviceObject != null && deviceObject.AllowsDirectCommunication && deviceObject.DeviceIdentification != null)
                        {
                            ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceObject.DeviceIdentification);
                            return LocalTargetSettings.InteractiveLoginWink.GetBoolValue(targetSettingsById);
                        }
                    }
                }
                return false;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.CmdWinkIcon.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
            if (selectedDevice != null && ((IObject)selectedDevice).MetaObject != null)
            {
                return new string[1] { ((IObject)selectedDevice).MetaObject.ObjectGuid.ToString() };
            }
            return null;
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return false;
            }
            INavigatorControl navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null;
            return navigatorControl != null && navigatorControl.SelectedSVNodes != null && navigatorControl.SelectedSVNodes.Length == 1 && navigatorControl.SelectedSVNodes[0].GetMetaObjectStub().ParentObjectGuid == Guid.Empty;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_007d: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d2: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return;
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 1);
            }
            Guid result = Guid.Empty;
            if (Guid.TryParse(arguments[0], out result))
            {
                IOnlineDevice17 val = null;
                object obj = new object();
                try
                {
                    _bIsExecutionInProgress = true;
                    IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(result);
                    val = (IOnlineDevice17)(object)((onlineDevice is IOnlineDevice17) ? onlineDevice : null);
                    if (val != null)
                    {
                        ((IOnlineDevice3)val).SharedConnect(obj);
                        val.Wink();
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is InteractiveLoginFailedException) || !((InteractiveLoginFailedException)ex).ShouldBeHandledSilently)
                    {
                        APEnvironment.MessageService.Error(ex.Message, "ErrorWinking", Array.Empty<object>());
                    }
                }
                finally
                {
                    try
                    {
                        if (val != null && ((IOnlineDevice)val).IsConnected)
                        {
                            ((IOnlineDevice3)val).SharedDisconnect(obj);
                        }
                    }
                    catch
                    {
                    }
                    _bIsExecutionInProgress = false;
                }
                return;
            }
            throw new BatchWrongArgumentTypeException(BATCH_COMMAND, 0, "GUID");
        }
    }
}
