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
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{A196E6A7-B6D7-4BDC-A64C-D6EDD8166153}")]
    public class SourceDownload : ISourceDownload2, ISourceDownload
    {
        private static List<Guid> _SourceCodes = new List<Guid>();

        internal static List<Guid> SourceCodesDownloadedToDevice => _SourceCodes;

        public void DownloadFromUserCommand()
        {
            Download(null, bUserCommand: true, bProgramDownload: false, bCreateBootproject: false);
        }

        public void DownloadFromProgramDownload()
        {
            Download(null, bUserCommand: false, bProgramDownload: true, bCreateBootproject: false);
        }

        public void DownloadFromCreatingBootproject()
        {
            Download(null, bUserCommand: false, bProgramDownload: false, bCreateBootproject: true);
        }

        public void DownloadToSelectedDevice(IOnlineDevice online)
        {
            //IL_0011: Unknown result type (might be due to invalid IL or missing references)
            //IL_002c: Expected O, but got Unknown
            if (online is IOnlineDevice2)
            {
                ProjectOptionsHelper.IsCompileInformationIncludedInSource(bWarnUserAndOptionallyChangeSourceContent: true);
                ExecuteDownload((IOnlineDevice2)online, Guid.Empty, ProjectOptionsHelper.SourceDownloadContent2, ProjectOptionsHelper.SourceDownloadCompact, bCheckTargetSettings: true, bThrowExceptions: false);
            }
        }

        public void Download(IOnlineDevice online, bool bUserCommand, bool bProgramDownload, bool bCreateBootproject)
        {
            //IL_0040: Unknown result type (might be due to invalid IL or missing references)
            //IL_0106: Unknown result type (might be due to invalid IL or missing references)
            //IL_016d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0173: Invalid comparison between Unknown and I4
            //IL_018b: Unknown result type (might be due to invalid IL or missing references)
            IDeviceObject val = null;
            if (online == null)
            {
                val = OnlineCommandHelper.GetActiveDevice();
            }
            else if (online is IOnlineDevice10 && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IOnlineDevice10)online).DeviceObjectGuid).Object;
                val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
            }
            if (val == null)
            {
                return;
            }
            if (online == null)
            {
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)val).MetaObject.ObjectGuid);
                online = ((onlineDevice is IOnlineDevice2) ? onlineDevice : null);
            }
            if (online == null)
            {
                return;
            }
            ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentification);
            if (!LocalTargetSettings.SourceDownloadAllowed.GetBoolValue(targetSettingsById) || !online.IsConnected || (ProjectOptionsHelper.GetDestinationDevice != Guid.Empty && ProjectOptionsHelper.GetDestinationDevice != ((IObject)val).MetaObject.ObjectGuid))
            {
                return;
            }
            if (ProjectOptionsHelper.OnlyOnDemand)
            {
                if (!bUserCommand)
                {
                    return;
                }
            }
            else if (ProjectOptionsHelper.ImplicitlyAtBootAppAndDownload)
            {
                if (!(bProgramDownload || bCreateBootproject))
                {
                    return;
                }
                if (bCreateBootproject && online is IOnlineDevice10)
                {
                    Guid deviceObjectGuid = ((IOnlineDevice10)online).DeviceObjectGuid;
                    if (bProgramDownload && !SourceCodesDownloadedToDevice.Contains(deviceObjectGuid))
                    {
                        SourceCodesDownloadedToDevice.Add(deviceObjectGuid);
                    }
                }
            }
            else if (ProjectOptionsHelper.ImplicitlyAtDownload)
            {
                if (!bProgramDownload)
                {
                    return;
                }
            }
            else if (ProjectOptionsHelper.PromptAtDownload)
            {
                if (bCreateBootproject)
                {
                    return;
                }
                string text = string.Format(Strings.SourceDownloadPrompt_MessageBoxText, Strings.SourceDownloadOptionEditorName);
                if ((int)APEnvironment.MessageService.Prompt(text, (PromptChoice)2, (PromptResult)2, "SourceDownloadPrompt_MessageBoxText", Array.Empty<object>()) == 3)
                {
                    return;
                }
            }
            else if (ProjectOptionsHelper.ImplicitlyAtCreatingBootproject)
            {
                if (!bCreateBootproject)
                {
                    return;
                }
                if (online is IOnlineDevice10)
                {
                    Guid deviceObjectGuid2 = ((IOnlineDevice10)online).DeviceObjectGuid;
                    if (!SourceCodesDownloadedToDevice.Contains(deviceObjectGuid2))
                    {
                        SourceCodesDownloadedToDevice.Add(deviceObjectGuid2);
                    }
                }
            }
            ProjectOptionsHelper.IsCompileInformationIncludedInSource(bWarnUserAndOptionallyChangeSourceContent: true);
            ExecuteDownload((IOnlineDevice2)(object)((online is IOnlineDevice2) ? online : null), Guid.Empty, ProjectOptionsHelper.SourceDownloadContent2, ProjectOptionsHelper.SourceDownloadCompact, bCheckTargetSettings: true, bThrowExceptions: false);
        }

        public void DownloadWithParameters(IOnlineDevice10 online, bool bCompactDownload, bool bCheckTargetSettings, Guid guidDeviceObject, int nProjectHandle, Guid[] additionalArchiveCategories)
        {
            //IL_004a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0050: Expected O, but got Unknown
            if (online == null || (guidDeviceObject == Guid.Empty && bCheckTargetSettings) || (guidDeviceObject == Guid.Empty && bCompactDownload))
            {
                throw new InvalidOperationException(Strings.InvalidParameters);
            }
            object obj = new object();
            if (bCheckTargetSettings)
            {
                IDeviceObject val = (IDeviceObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guidDeviceObject).Object;
                ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentification);
                if (!LocalTargetSettings.SourceDownloadAllowed.GetBoolValue(targetSettingsById))
                {
                    throw new InvalidOperationException(Strings.SourceDownloadNotSupported);
                }
            }
            try
            {
                ((IOnlineDevice3)online).SharedConnect(obj);
                ExecuteDownload((IOnlineDevice2)(object)online, guidDeviceObject, additionalArchiveCategories, bCompactDownload, bCheckTargetSettings, bThrowExceptions: true);
            }
            finally
            {
                if (((IOnlineDevice)online).IsConnected)
                {
                    ((IOnlineDevice3)online).SharedDisconnect(obj);
                }
            }
        }

        private void ExecuteDownload(IOnlineDevice2 online, Guid deviceObjectToArchive, Guid[] additionalArchiveCategories, bool bCompactDownload, bool bCheckTargetSettings, bool bThrowExceptions)
        {
            //IL_0108: Unknown result type (might be due to invalid IL or missing references)
            //IL_010d: Invalid comparison between I4 and Unknown
            //IL_0113: Unknown result type (might be due to invalid IL or missing references)
            //IL_0147: Unknown result type (might be due to invalid IL or missing references)
            //IL_014e: Expected O, but got Unknown
            //IL_024a: Unknown result type (might be due to invalid IL or missing references)
            //IL_028d: Unknown result type (might be due to invalid IL or missing references)
            //IL_02b9: Unknown result type (might be due to invalid IL or missing references)
            //IL_03c5: Unknown result type (might be due to invalid IL or missing references)
            //IL_03c8: Unknown result type (might be due to invalid IL or missing references)
            //IL_03da: Expected I4, but got Unknown
            //IL_03f6: Unknown result type (might be due to invalid IL or missing references)
            IProjectArchiveService val = APEnvironment.CreateProjectArchiveService();
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            bool flag = true;
            List<IProjectArchiveItem> list = new List<IProjectArchiveItem>();
            if (additionalArchiveCategories != null)
            {
                Guid[] array = additionalArchiveCategories;
                foreach (Guid guid in array)
                {
                    if (guid == SourceTransferHelper.GUID_PAC_OPTIONS)
                    {
                        continue;
                    }
                    try
                    {
                        IProjectArchiveCategory val2 = APEnvironment.CreateProjectArchiveCategory(guid);
                        Debug.Assert(val2 != null);
                        foreach (string archiveItemId in val2.GetArchiveItemIds(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle))
                        {
                            ProjectArchiveItem item = new ProjectArchiveItem(val2, archiveItemId);
                            list.Add((IProjectArchiveItem)(object)item);
                        }
                    }
                    catch
                    {
                        if (bThrowExceptions)
                        {
                            throw;
                        }
                    }
                }
            }
            if (APEnvironment.FrameForm != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Dirty && 2 != (int)APEnvironment.MessageService.Prompt(Strings.PromptSaveProjectDuringSourceDownload, (PromptChoice)2, (PromptResult)2, "Prompt_SourceDownloadProjectDirty", Array.Empty<object>()))
            {
                if (bThrowExceptions)
                {
                    throw new CancelledByUserException();
                }
                return;
            }
            string directoryName = Path.GetDirectoryName(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path);
            string text = "Archive.prj";
            AuthFileStream val3 = new AuthFileStream(directoryName + "\\" + text, FileMode.Create);
            bool flag2 = false;
            if (bCompactDownload && val is IProjectArchiveService2)
            {
                try
                {
                    Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
                    if (deviceObjectToArchive == Guid.Empty)
                    {
                        deviceObjectToArchive = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication;
                        IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, deviceObjectToArchive);
                        while (metaObjectStub.ParentObjectGuid != Guid.Empty)
                        {
                            metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, metaObjectStub.ParentObjectGuid);
                        }
                        deviceObjectToArchive = metaObjectStub.ObjectGuid;
                    }
                    List<Guid> list2 = new List<Guid>();
                    Guid[] array = allObjects;
                    foreach (Guid guid2 in array)
                    {
                        if (guid2 != deviceObjectToArchive)
                        {
                            IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid2);
                            if (metaObjectStub2.ParentObjectGuid == Guid.Empty && typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType))
                            {
                                list2.Add(guid2);
                            }
                        }
                    }
                    flag2 = ((IProjectArchiveService2)val).CreateCompactPlcArchiveFile(handle, deviceObjectToArchive, list2, flag, (IEnumerable<IProjectArchiveItem>)list, "Compact source download", (Stream)(object)val3);
                }
                catch
                {
                    if (bThrowExceptions)
                    {
                        throw;
                    }
                }
            }
            else
            {
                flag2 = val.CreateArchiveFile(handle, flag, (IEnumerable<IProjectArchiveItem>)list, (IEnumerable<string>)null, "Source download", (Stream)(object)val3);
            }
            if (online != null && online is IOnlineDevice3)
            {
                ((IOnlineDevice3)(IOnlineDevice13)online).CreateDirFunctionService();
            }
            if (val3 != null && bCheckTargetSettings)
            {
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                if (activeDevice != null && online is IOnlineDevice10 && ((IOnlineDevice10)online).DeviceObjectGuid == ((IObject)activeDevice).MetaObject.ObjectGuid)
                {
                    ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(activeDevice.DeviceIdentification);
                    int intValue = LocalTargetSettings.MaxSourceDownloadSize.GetIntValue(targetSettingsById);
                    if (intValue > 0 && ((Stream)(object)val3).Length > intValue)
                    {
                        ((Stream)(object)val3).Close();
                        throw new Exception(string.Format(Strings.Err_SourceDownload_ArchiveTooBig, intValue));
                    }
                }
            }
            if (!flag2)
            {
                ((Stream)(object)val3)?.Close();
                return;
            }
            if (SourceTransferHelper.GetRtsVersionOnline((IOnlineDevice)(object)online) >= SourceTransferHelper.RTS_VERSION_3580)
            {
                text = "$PlcLogic$/" + text;
            }
            IFileDownload obj3 = online.CreateFileDownload((Stream)(object)val3, text);
            IFileDownload2 val4 = (IFileDownload2)(object)((obj3 is IFileDownload2) ? obj3 : null);
            IAsyncResult asyncResult = ((IFileDownload)val4).BeginDownload(true, (AsyncCallback)null, (object)null);
            IProgressCallback val5 = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
            val5.Abortable = (true);
            try
            {
                long num = 0L;
                long num2 = -1L;
                long num3 = 0L;
                val5.NextTask(Strings.SourceProgressTransferringArchiveFile, (int)((Stream)(object)val3).Length, Strings.SourceProgressBytes);
                DownloadProgress val6 = default(DownloadProgress);
                while (!asyncResult.AsyncWaitHandle.WaitOne(10, exitContext: false))
                {
                    val4.GetProgress(asyncResult, out val6, out num, out num2);
                    switch ((int)val6 - 1)
                    {
                        case 0:
                            val5.TaskProgress(Strings.SourceProgressInitializing, 0);
                            break;
                        case 1:
                            if (val5 is IProgressCallback2)
                            {
                                ((IProgressCallback2)val5).TaskProgressAbsolute(Strings.SourceProgressDownload, (int)num);
                            }
                            else
                            {
                                val5.TaskProgress(Strings.SourceProgressDownload, (int)(num - num3));
                            }
                            num3 = num;
                            break;
                        case 2:
                            val5.TaskProgress(Strings.SourceProgressFinishing, 0);
                            break;
                    }
                    if (val5.Aborting)
                    {
                        ((IFileDownload)val4).CancelDownload(asyncResult);
                    }
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw new Exception(Strings.SourceTransferFailed + ". " + ex.Message);
            }
            finally
            {
                if (val5 != null)
                {
                    val5.Finish();
                }
                ((Stream)(object)val3)?.Close();
            }
            ((IFileDownload)val4).EndDownload(asyncResult);
        }

        public void DownloadToDevice(IOnlineDevice device, SourceDownloadMode mode)
        {
            //IL_0000: Unknown result type (might be due to invalid IL or missing references)
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            //IL_0005: Invalid comparison between Unknown and I4
            if ((int)mode != 0)
            {
                if ((int)mode == 1)
                {
                    Download(device, bUserCommand: false, bProgramDownload: true, bCreateBootproject: true);
                }
            }
            else
            {
                Download(device, bUserCommand: false, bProgramDownload: true, bCreateBootproject: false);
            }
        }
    }
}
