using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{EC0DE984-12E7-462C-B2F4-1DD7C5AC7537}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_remove_application_from_device.htm")]
    public class DeleteApplicationFromDevice : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "removeapplicationfromdevice" };

        public string Name => Strings.CommandDeleteApplicationFromDevice;

        public string Description => Name;

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => null;

        public bool Enabled => true;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments()
        {
            return new string[0];
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                if (((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) != null)
                {
                    IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
                    if (primaryProject == null || primaryProject.SelectedSVNodes == null || primaryProject.SelectedSVNodes.Length != 1)
                    {
                        return false;
                    }
                    IMetaObjectStub metaObjectStub = primaryProject.SelectedSVNodes[0].GetMetaObjectStub();
                    if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            if (((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) != null)
            {
                IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
                if (primaryProject == null || primaryProject.SelectedSVNodes == null || primaryProject.SelectedSVNodes.Length != 1)
                {
                    return;
                }
                IMetaObjectStub metaObjectStub = primaryProject.SelectedSVNodes[0].GetMetaObjectStub();
                if (!typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    return;
                }
                if (APEnvironment.MessageService.Prompt(Strings.ReallyDeleteApplication, PromptChoice.OKCancel, PromptResult.Cancel, "RemoveApplicatioFromDevice", Array.Empty<object>()) == PromptResult.OK)
                {
                    this.DeleteApplication(metaObjectStub);
                }
            }
        }

        private void DeleteApplication(IMetaObjectStub mosApplication)
        {
            bool flag = false;
            try
            {
                IMetaObjectStub deviceObject = GetDeviceObject(mosApplication);
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(deviceObject.ObjectGuid);
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                if (!onlineDevice.IsConnected)
                {
                    onlineDevice.Connect();
                    flag = false;
                }
                onlineDevice.DeleteApplication(mosApplication.Name);
                try
                {
                    Guid applicationGuidByName = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetApplicationGuidByName(deviceObject.Name + "." + mosApplication.Name);
                    if (applicationGuidByName != Guid.Empty)
                    {
                        LogoutFromRemovedApplication(applicationGuidByName, handle);
                    }
                }
                catch
                {
                }
                if (!flag)
                {
                    onlineDevice.Disconnect();
                }
            }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledByUserNotificationFromPlc))
                {
                    APEnvironment.MessageService.Error(ex.Message, "RemoveApplicatioFromDevice", Array.Empty<object>());
                }
            }
        }

        private static IMetaObjectStub GetDeviceObject(IMetaObjectStub mosApplication)
        {
            IMetaObjectStub val = mosApplication;
            while (val.ParentObjectGuid != Guid.Empty)
            {
                val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, val.ParentObjectGuid);
                if (typeof(IDeviceObject).IsAssignableFrom(val.ObjectType))
                {
                    return val;
                }
            }
            return null;
        }

        private void LogoutFromRemovedApplication(Guid guidApp, int nProjectHandle)
        {
            IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guidApp);
            if (application == null)
            {
                return;
            }
            if (application.IsLoggedIn)
            {
                application.Logout();
            }
            Guid[] subObjectGuids = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guidApp).SubObjectGuids;
            foreach (Guid guid in subObjectGuids)
            {
                if (((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guid) != null)
                {
                    LogoutFromRemovedApplication(guid, nProjectHandle);
                }
            }
        }
    }
}
