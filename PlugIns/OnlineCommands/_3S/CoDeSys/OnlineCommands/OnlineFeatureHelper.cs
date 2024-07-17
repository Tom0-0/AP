using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class OnlineFeatureHelper
    {
        private static int s_nApplicationCount = -1;

        private static bool s_bAttachedToEvents = false;

        internal static int ApplicationCount
        {
            get
            {
                //IL_00a5: Unknown result type (might be due to invalid IL or missing references)
                //IL_00af: Expected O, but got Unknown
                //IL_00bb: Unknown result type (might be due to invalid IL or missing references)
                //IL_00c5: Expected O, but got Unknown
                //IL_00d1: Unknown result type (might be due to invalid IL or missing references)
                //IL_00db: Expected O, but got Unknown
                if (s_nApplicationCount >= 0)
                {
                    return s_nApplicationCount;
                }
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject != null)
                {
                    s_nApplicationCount = 0;
                    Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(primaryProject.Handle);
                    foreach (Guid guid in allObjects)
                    {
                        IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(primaryProject.Handle, guid);
                        if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                        {
                            s_nApplicationCount++;
                        }
                    }
                }
                else
                {
                    s_nApplicationCount = 0;
                }
                if (!s_bAttachedToEvents)
                {
                    ((IEngine)APEnvironment.Engine).Projects.PrimaryProjectSwitched += (new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
                    ((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded += (new ObjectEventHandler(OnObjectAdded));
                    ((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved += (new ObjectRemovedEventHandler(OnObjectRemoved));
                    s_bAttachedToEvents = true;
                }
                return s_nApplicationCount;
            }
        }

        private static void OnObjectRemoved(object sender, ObjectRemovedEventArgs e)
        {
            s_nApplicationCount = -1;
        }

        private static void OnObjectAdded(object sender, ObjectEventArgs e)
        {
            s_nApplicationCount = -1;
        }

        private static void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
        {
            s_nApplicationCount = -1;
            SourceDownload.SourceCodesDownloadedToDevice.Clear();
        }

        internal static bool CheckActiveApplication(OnlineFeatureEnum feature)
        {
            //IL_0005: Unknown result type (might be due to invalid IL or missing references)
            return ((IOnlineManager6)APEnvironment.OnlineMgr).CheckActiveApplicationFeatureSupport(feature);
        }

        internal static bool CheckSelectedApplications(OnlineFeatureEnum feature)
        {
            //IL_005e: Unknown result type (might be due to invalid IL or missing references)
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject != null)
            {
                ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
                foreach (ISVNode val in selectedSVNodes)
                {
                    if (typeof(IApplicationObject).IsAssignableFrom(val.GetMetaObjectStub().ObjectType))
                    {
                        IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(val.GetMetaObjectStub().ObjectGuid);
                        if (application != null && !((IOnlineManager6)APEnvironment.OnlineMgr).CheckOnlineApplicationFeatureSupport(feature, application))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static bool CheckSpecificApplication(OnlineFeatureEnum feature, Guid guidApplicationToCheck)
        {
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guidApplicationToCheck);
                if (application != null)
                {
                    return ((IOnlineManager6)APEnvironment.OnlineMgr).CheckOnlineApplicationFeatureSupport(feature, application);
                }
            }
            return false;
        }

        internal static bool CheckCascading(OnlineFeatureEnum feature, Guid guidApplicationToCheck)
        {
            //IL_0013: Unknown result type (might be due to invalid IL or missing references)
            //IL_0032: Unknown result type (might be due to invalid IL or missing references)
            //IL_0038: Expected O, but got Unknown
            //IL_0038: Unknown result type (might be due to invalid IL or missing references)
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return false;
            }
            if (!CheckSpecificApplication(feature, guidApplicationToCheck))
            {
                return false;
            }
            foreach (IOnlineApplication childApplicationObject in OnlineCommandHelper.GetChildApplicationObjects(guidApplicationToCheck))
            {
                IOnlineApplication val = childApplicationObject;
                if (!CheckSpecificApplication(feature, val.ApplicationGuid))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool CheckAllOnlineApplications(OnlineFeatureEnum feature)
        {
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                IOnlineApplication[] allOnlineApplications = OnlineCommandHelper.GetAllOnlineApplications();
                foreach (IOnlineApplication val in allOnlineApplications)
                {
                    if (val != null && !((IOnlineManager6)APEnvironment.OnlineMgr).CheckOnlineApplicationFeatureSupport(feature, val))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool CheckSelectedDevice(OnlineFeatureEnum feature, IDeviceObject device)
        {
            //IL_0031: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_0061: Expected I4, but got Unknown
            if (device != null)
            {
                ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(device.DeviceIdentification);
                if (targetSettingsById != null)
                {
                    bool flag = !LocalTargetSettings.OnlyExplicitFeaturesSupported.GetBoolValue(targetSettingsById);
                    switch ((int)feature - 1)
                    {
                        case 7:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.CoreApplicationHandlingSupported.Path, flag);
                        case 0:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.SourceDownloadAllowed.Path, flag);
                        case 1:
                            if (targetSettingsById.GetBoolValue(LocalTargetSettings.OnlineChangeSupported.Path, flag))
                            {
                                return !LocalTargetSettings.CompactDownload.GetBoolValue(targetSettingsById);
                            }
                            return false;
                        case 2:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.BootApplicationSupported.Path, flag);
                        case 3:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.ForceVariablesSupported.Path, flag);
                        case 4:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.WriteVariablesSupported.Path, flag);
                        case 5:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.ConnectDeviceSupported.Path, flag);
                        case 6:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.FileTransferSupported.Path, flag);
                        case 8:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.BreakpointsSupported.Path, flag);
                        case 9:
                            return targetSettingsById.GetBoolValue(LocalTargetSettings.ConditionalBreakpointsSupported.Path, flag);
                        default:
                            return flag;
                    }
                }
            }
            return false;
        }
    }
}
