#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceEditor;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.ProjectArchive;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{C9F93965-9879-49D7-B46C-A2E7A8F097AC}")]
    public class SourceUpload : ISourceUpload
    {
        private static readonly Guid GUID_FILEOPENCOMMAND = new Guid("{9D09E965-6254-46fd-9C21-B27FD2E97B52}");

        public void UploadWithParameters(IOnlineDevice10 online, string stPathForUpload)
        {
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            //IL_005e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0064: Expected O, but got Unknown
            //IL_0105: Unknown result type (might be due to invalid IL or missing references)
            //IL_0108: Unknown result type (might be due to invalid IL or missing references)
            //IL_011a: Expected I4, but got Unknown
            //IL_0136: Unknown result type (might be due to invalid IL or missing references)
            if (online == null || stPathForUpload == null || stPathForUpload == string.Empty)
            {
                throw new InvalidOperationException(Strings.InvalidParameters);
            }
            object obj = new object();
            try
            {
                ((IOnlineDevice3)online).SharedConnect(obj);
                string text = "Archive.prj";
                if (online is IOnlineDevice17 && ((IOnlineDevice17)online).RuntimeSystemVersion >= SourceTransferHelper.RTS_VERSION_3580)
                {
                    text = "$PlcLogic$/" + text;
                }
                AuthFileStream val = new AuthFileStream(stPathForUpload, FileMode.Create);
                IFileUpload obj2 = ((IOnlineDevice2)online).CreateFileUpload((Stream)(object)val, text);
                IFileUpload2 val2 = (IFileUpload2)(object)((obj2 is IFileUpload2) ? obj2 : null);
                IAsyncResult asyncResult = ((IFileUpload)val2).BeginUpload(true, (AsyncCallback)null, (object)null);
                IProgressCallback val3 = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
                val3.Abortable = (true);
                try
                {
                    long num = 0L;
                    long num2 = -1L;
                    long num3 = 0L;
                    UploadProgress val4 = default(UploadProgress);
                    val2.GetProgress(asyncResult, out val4, out num, out num2);
                    if (num2 == 0L)
                    {
                        Thread.Sleep(200);
                        Application.DoEvents();
                        val2.GetProgress(asyncResult, out val4, out num, out num2);
                    }
                    if (num2 > int.MaxValue)
                    {
                        num2 = 2147483647L;
                    }
                    val3.NextTask(Strings.SourceProgressTransferringArchiveFile, (int)num2, Strings.SourceProgressBytes);
                    while (!asyncResult.AsyncWaitHandle.WaitOne(10, exitContext: false))
                    {
                        val2.GetProgress(asyncResult, out val4, out num, out num2);
                        switch ((int)val4 - 1)
                        {
                            case 0:
                                val3.TaskProgress(Strings.SourceProgressInitializing, 0);
                                break;
                            case 1:
                                if (val3 is IProgressCallback2)
                                {
                                    ((IProgressCallback2)val3).TaskProgressAbsolute(Strings.SourceProgressUpload, (int)num);
                                }
                                else
                                {
                                    val3.TaskProgress(Strings.SourceProgressUpload, (int)(num - num3));
                                }
                                num3 = num;
                                break;
                            case 2:
                                val3.TaskProgress(Strings.SourceProgressFinishing, 0);
                                break;
                        }
                        if (val3.Aborting)
                        {
                            ((IFileUpload)val2).CancelUpload(asyncResult);
                        }
                        Application.DoEvents();
                    }
                    ((IFileUpload)val2).EndUpload(asyncResult);
                }
                finally
                {
                    val3.Finish();
                    ((Stream)(object)val).Close();
                }
            }
            finally
            {
                if (online != null && ((IOnlineDevice)online).IsConnected)
                {
                    ((IOnlineDevice3)online).SharedDisconnect(obj);
                }
            }
        }

        public void Upload()
        {
            //IL_018c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0193: Expected O, but got Unknown
            //IL_023e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0241: Unknown result type (might be due to invalid IL or missing references)
            //IL_0253: Expected I4, but got Unknown
            //IL_026f: Unknown result type (might be due to invalid IL or missing references)
            //IL_031f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0326: Expected O, but got Unknown
            //IL_0373: Unknown result type (might be due to invalid IL or missing references)
            //IL_037a: Expected O, but got Unknown
            //IL_03bb: Unknown result type (might be due to invalid IL or missing references)
            //IL_03c1: Invalid comparison between Unknown and I4
            //IL_03cd: Unknown result type (might be due to invalid IL or missing references)
            ISelectDeviceService val = null;
            val = OnlineCommandHelper.GetCustomSelectDeviceProvider();
            if (val == null)
            {
                val = APEnvironment.CreateSelectDeviceService();
            }
            Debug.Assert(val != null);
            IOnlineDevice3 val2 = null;
            string text = default(string);
            IDeviceAddress val3 = default(IDeviceAddress);
            IDeviceIdentification val4 = default(IDeviceIdentification);
            val.Invoke((IWin32Window)APEnvironment.FrameForm, out text, out val3, out val4);
            if (text == null && val3 == null && val4 == null)
            {
                return;
            }
            if (val4 == null)
            {
                string arg = "-";
                if (val3 != null)
                {
                    arg = "[" + val3.ToString() + "]";
                }
                APEnvironment.MessageService.Error(string.Format(Strings.SpecifiedDeviceNotFound, arg), "SourceUpload_Error_NoDevice", Array.Empty<object>());
                return;
            }
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
                foreach (Guid guid in allObjects)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
                    if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        continue;
                    }
                    try
                    {
                        IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(guid);
                        IOnlineDevice5 val5 = (IOnlineDevice5)(object)((onlineDevice is IOnlineDevice5) ? onlineDevice : null);
                        if (((IOnlineDevice4)val5).GetDeviceAddressInUse().Equals(val3))
                        {
                            val2 = (IOnlineDevice3)(object)val5;
                            goto IL_0127;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            goto IL_0127;
        IL_0127:
            if (val2 == null)
            {
                IOnlineDevice obj2 = APEnvironment.OnlineMgr.CreateOnlineDevice(text, val3, val4, true);
                val2 = (IOnlineDevice3)(object)((obj2 is IOnlineDevice3) ? obj2 : null);
            }
            if (val2 == null)
            {
                return;
            }
            if (!((IOnlineDevice)val2).IsConnected)
            {
                ((IOnlineDevice)val2).Connect();
            }
            string text2 = "Archive.prj";
            if (SourceTransferHelper.GetRtsVersionOnline((IOnlineDevice)(object)val2) >= SourceTransferHelper.RTS_VERSION_3580)
            {
                text2 = "$PlcLogic$/" + text2;
            }
            string text3 = Path.Combine(OnlineCommandHelper.GetCreateDirForTemporaryFiles(), "ArchiveUpload.prj");
            AuthFileStream val6 = new AuthFileStream(text3, FileMode.Create);
            IFileUpload obj3 = ((IOnlineDevice2)val2).CreateFileUpload((Stream)(object)val6, text2);
            IFileUpload2 val7 = (IFileUpload2)(object)((obj3 is IFileUpload2) ? obj3 : null);
            IAsyncResult asyncResult = ((IFileUpload)val7).BeginUpload(true, (AsyncCallback)null, (object)null);
            IProgressCallback val8 = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
            val8.Abortable = (true);
            bool flag = false;
            try
            {
                long num = 0L;
                long num2 = -1L;
                long num3 = 0L;
                UploadProgress val9 = default(UploadProgress);
                val7.GetProgress(asyncResult, out val9, out num, out num2);
                if (num2 == 0L)
                {
                    Thread.Sleep(200);
                    Application.DoEvents();
                    val7.GetProgress(asyncResult, out val9, out num, out num2);
                }
                if (num2 > int.MaxValue)
                {
                    num2 = 2147483647L;
                }
                val8.NextTask(Strings.SourceProgressTransferringArchiveFile, (int)num2, Strings.SourceProgressBytes);
                while (!asyncResult.AsyncWaitHandle.WaitOne(10, exitContext: false))
                {
                    val7.GetProgress(asyncResult, out val9, out num, out num2);
                    switch ((int)val9 - 1)
                    {
                        case 0:
                            val8.TaskProgress(Strings.SourceProgressInitializing, 0);
                            break;
                        case 1:
                            if (val8 is IProgressCallback2)
                            {
                                ((IProgressCallback2)val8).TaskProgressAbsolute(Strings.SourceProgressUpload, (int)num);
                            }
                            else
                            {
                                val8.TaskProgress(Strings.SourceProgressUpload, (int)(num - num3));
                            }
                            num3 = num;
                            break;
                        case 2:
                            val8.TaskProgress(Strings.SourceProgressFinishing, 0);
                            break;
                    }
                    if (val8.Aborting)
                    {
                        ((IFileUpload)val7).CancelUpload(asyncResult);
                    }
                    Application.DoEvents();
                }
                ((IFileUpload)val7).EndUpload(asyncResult);
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
                APEnvironment.MessageService.Error(Strings.SourceUploadPrompt_NoUploadFile, "SourceUploadPrompt_NoUploadFile", Array.Empty<object>());
                flag = true;
            }
            finally
            {
                val8.Finish();
                ((Stream)(object)val6).Close();
            }
            ((IOnlineDevice)val2).Disconnect();
            if (flag)
            {
                return;
            }
            try
            {
                val6 = new AuthFileStream(text3, FileMode.Open);
                IProjectArchiveService val10 = APEnvironment.CreateProjectArchiveService();
                IEnumerable<string> enumerable = null;
                IEnumerable<IProjectArchiveItem> enumerable2 = null;
                string text4 = default(string);
                string text5 = default(string);
                val10.LoadArchiveFile((Stream)(object)val6, out text4, out enumerable2, out enumerable, out text5);
                ((Stream)(object)val6).Close();
                val6 = null;
                string text6 = default(string);
                IEnumerable<IProjectArchiveItem> enumerable3 = default(IEnumerable<IProjectArchiveItem>);
                IDictionary<string, string> dictionary = default(IDictionary<string, string>);
                if (val10.SelectArchiveContentsToExtract((IWin32Window)APEnvironment.FrameForm, text4, enumerable2, enumerable, text5, out text6, out enumerable3, out dictionary) == DialogResult.OK)
                {
                    val6 = new AuthFileStream(text3, FileMode.Open);
                    if (val10.ExtractArchiveFile((Stream)(object)val6, text6, enumerable3, dictionary, (IExtractProjectArchiveNotifyHandler)(object)APEnvironment.CreateExtractProjectArchiveNotifyHandler()) && text6 != null && ((IEngine)APEnvironment.Engine).CommandManager is ICommandManager2 && (int)APEnvironment.MessageService.Prompt(Strings.PromptOpenExtractedProject, (PromptChoice)2, (PromptResult)2, "PromptOpenExtractedProject", Array.Empty<object>()) == 2)
                    {
                        ((ICommandManager2)((IEngine)APEnvironment.Engine).CommandManager).ExecuteCommand(GUID_FILEOPENCOMMAND, new string[1] { text6 });
                    }
                }
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.ToString(), "ErrorDuringUpload", Array.Empty<object>());
            }
            finally
            {
                ((Stream)(object)val6)?.Close();
            }
        }
    }
}
