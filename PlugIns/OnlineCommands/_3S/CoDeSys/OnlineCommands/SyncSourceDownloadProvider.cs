#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.ProjectArchive;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{1FFF597C-05FC-40C1-BD58-65489F151598}")]
    public class SyncSourceDownloadProvider : ISyncOnlineFileProvider2, ISyncOnlineFileProvider
    {
        public string Name => Strings.SourceDownload_SyncProviderName;

        public bool IsVisible => false;

        public bool IsUpToDate(Guid guidDeviceObject)
        {
            return false;
        }

        public IList<ISyncOnlineFile> UpdateFileInformation(Guid guidDeviceObject)
        {
            if (ProjectOptionsHelper.GetDestinationDevice != Guid.Empty && ProjectOptionsHelper.GetDestinationDevice != guidDeviceObject)
            {
                return null;
            }
            if (ProjectOptionsHelper.ImplicitlyAtBootAppAndDownload || ProjectOptionsHelper.ImplicitlyAtDownload)
            {
                Version rtsVersionFromDevdesc = SourceTransferHelper.GetRtsVersionFromDevdesc(guidDeviceObject);
                if (rtsVersionFromDevdesc >= SourceTransferHelper.RTS_VERSION_3570)
                {
                    return (IList<ISyncOnlineFile>)new LList<ISyncOnlineFile>((IEnumerable<ISyncOnlineFile>)(object)new ISyncOnlineFile[1]
                    {
                        new SyncSourceFile((ISyncOnlineFileProvider)(object)this, guidDeviceObject, rtsVersionFromDevdesc)
                    });
                }
            }
            return null;
        }

        public bool PrepareTransfer(Guid guidDeviceObject, Guid guidApplication, SyncOnlineFileTiming timing)
        {
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            return PrepareTransfer(guidDeviceObject, guidApplication, timing, bIsOnlineChange: false);
        }

        public void CleanUpTransfer(Guid guidDeviceObject, Guid guidApplication)
        {
        }

        public FileStream GetFileStream(Guid guidDeviceObject, Guid guidApplication, string stFileName, object tag)
        {
            //IL_0140: Unknown result type (might be due to invalid IL or missing references)
            //IL_0146: Expected O, but got Unknown
            //IL_022f: Unknown result type (might be due to invalid IL or missing references)
            AuthFileStream val = null;
            if (stFileName == "Archive.prj" && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidDeviceObject))
            {
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guidDeviceObject).Object;
                IDeviceObject val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
                IProjectArchiveService val3 = APEnvironment.CreateProjectArchiveService();
                bool flag = true;
                List<IProjectArchiveItem> list = new List<IProjectArchiveItem>();
                if (ProjectOptionsHelper.SourceDownloadContent2 != null)
                {
                    Guid[] sourceDownloadContent = ProjectOptionsHelper.SourceDownloadContent2;
                    foreach (Guid guid in sourceDownloadContent)
                    {
                        if (guid == SourceTransferHelper.GUID_PAC_OPTIONS)
                        {
                            continue;
                        }
                        IProjectArchiveCategory val4 = APEnvironment.CreateProjectArchiveCategory(guid);
                        Debug.Assert(val4 != null);
                        foreach (string archiveItemId in val4.GetArchiveItemIds(handle))
                        {
                            ProjectArchiveItem item = new ProjectArchiveItem(val4, archiveItemId);
                            list.Add((IProjectArchiveItem)(object)item);
                        }
                    }
                }
                val = new AuthFileStream(Path.GetDirectoryName(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path) + "\\" + stFileName, FileMode.Create);
                bool flag2 = false;
                if (ProjectOptionsHelper.SourceDownloadCompact && val3 is IProjectArchiveService2)
                {
                    try
                    {
                        Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
                        if (guidDeviceObject == Guid.Empty)
                        {
                            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guidDeviceObject);
                            while (metaObjectStub.ParentObjectGuid != Guid.Empty)
                            {
                                metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, metaObjectStub.ParentObjectGuid);
                            }
                            guidDeviceObject = metaObjectStub.ObjectGuid;
                        }
                        List<Guid> list2 = new List<Guid>();
                        Guid[] sourceDownloadContent = allObjects;
                        foreach (Guid guid2 in sourceDownloadContent)
                        {
                            if (guid2 != guidDeviceObject)
                            {
                                IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid2);
                                if (metaObjectStub2.ParentObjectGuid == Guid.Empty && typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType))
                                {
                                    list2.Add(guid2);
                                }
                            }
                        }
                        flag2 = ((IProjectArchiveService2)val3).CreateCompactPlcArchiveFile(handle, guidDeviceObject, list2, flag, (IEnumerable<IProjectArchiveItem>)list, "Compact source download", (Stream)(object)val);
                    }
                    catch
                    {
                        val = null;
                        throw;
                    }
                }
                else
                {
                    flag2 = val3.CreateArchiveFile(handle, flag, (IEnumerable<IProjectArchiveItem>)list, (IEnumerable<string>)null, "Source download", (Stream)(object)val);
                }
                if (val != null && val2 != null)
                {
                    ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val2.DeviceIdentification);
                    int intValue = LocalTargetSettings.MaxSourceDownloadSize.GetIntValue(targetSettingsById);
                    if (intValue > 0 && ((Stream)(object)val).Length > intValue)
                    {
                        ((Stream)(object)val).Close();
                        val = null;
                        throw new Exception(string.Format(Strings.Err_SourceDownload_ArchiveTooBig, intValue));
                    }
                }
                if (!flag2 && val != null)
                {
                    ((Stream)(object)val).Close();
                    val = null;
                }
            }
            return (FileStream)(object)val;
        }

        public Stream GetStream(Guid guidDeviceObject, Guid guidApplication, string stFileName, object tag)
        {
            return GetFileStream(guidDeviceObject, guidApplication, stFileName, tag);
        }

        public bool PrepareTransfer(Guid guidDeviceObject, Guid guidApplication, SyncOnlineFileTiming timing, bool bIsOnlineChange)
        {
            //IL_0084: Unknown result type (might be due to invalid IL or missing references)
            //IL_0089: Invalid comparison between I4 and Unknown
            if (SourceDownload.SourceCodesDownloadedToDevice.Contains(guidDeviceObject))
            {
                return false;
            }
            if (ProjectOptionsHelper.GetDestinationDevice != Guid.Empty && ProjectOptionsHelper.GetDestinationDevice != guidDeviceObject)
            {
                return false;
            }
            if (!ProjectOptionsHelper.ImplicitlyAtBootAppAndDownload && !ProjectOptionsHelper.ImplicitlyAtDownload)
            {
                return false;
            }
            if (APEnvironment.FrameForm != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Dirty && 2 != (int)APEnvironment.MessageService.Prompt(Strings.PromptSaveProjectDuringSourceDownload, (PromptChoice)2, (PromptResult)2, "Prompt_SourceDownloadProjectDirty", Array.Empty<object>()))
            {
                SourceDownload.SourceCodesDownloadedToDevice.Add(guidDeviceObject);
                return false;
            }
            ProjectOptionsHelper.IsCompileInformationIncludedInSource(bWarnUserAndOptionallyChangeSourceContent: true);
            SourceDownload.SourceCodesDownloadedToDevice.Add(guidDeviceObject);
            return true;
        }
    }
}
