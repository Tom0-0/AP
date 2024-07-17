#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Security;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.VisualObject;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{cf46872e-7cf6-484e-acee-f8b9da763449}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_create_boot_application.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Create_boot_application.htm")]
    public class CreateBootApplicationCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "createbootproject" };

        private static string stLastPath = string.Empty;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "CreateBootApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "CreateBootApplicationCommand_Name");

        public bool Enabled
        {
            get
            {
                //IL_0038: Unknown result type (might be due to invalid IL or missing references)
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid == Guid.Empty)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)3))
                {
                    return false;
                }
                IOnlineApplication application = OnlineCommandHelper.GetApplication(activeAppObjectGuid);
                if (!OnlineCommandHelper.CanCreateBootApplication(activeAppObjectGuid))
                {
                    return false;
                }
                if (application.IsLoggedIn)
                {
                    if ((int)InternalCodeStateProvider.InternalCodeState != 0)
                    {
                        return false;
                    }
                    IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                    IDeviceObject4 val = (IDeviceObject4)(object)((activeDevice is IDeviceObject4) ? activeDevice : null);
                    if (val != null && val.SimulationMode)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public Icon SmallIcon => null;

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
            return new string[0];
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                return false;
            }
            Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
            if (activeAppObjectGuid == Guid.Empty)
            {
                return false;
            }
            bool flag = OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)3);
            IOnlineApplication application = OnlineCommandHelper.GetApplication(activeAppObjectGuid);
            if (flag && application.IsLoggedIn)
            {
                flag = OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)12);
            }
            return flag;
        }

        public void SaveBootProjectHelp(Guid guidApplication)
        {
            //IL_00bb: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d3: Unknown result type (might be due to invalid IL or missing references)
            //IL_018f: Unknown result type (might be due to invalid IL or missing references)
            //IL_01ba: Unknown result type (might be due to invalid IL or missing references)
            //IL_01eb: Unknown result type (might be due to invalid IL or missing references)
            //IL_01f0: Unknown result type (might be due to invalid IL or missing references)
            //IL_01f6: Expected O, but got Unknown
            //IL_0218: Expected O, but got Unknown
            //IL_0222: Expected O, but got Unknown
            IOnlineApplication application = OnlineCommandHelper.GetApplication(guidApplication);
            string applicationNameByGuid = OnlineCommandHelper.GetApplicationNameByGuid(guidApplication);
            if (stLastPath == string.Empty)
            {
                stLastPath = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path;
                stLastPath = Path.Combine(Path.GetDirectoryName(stLastPath), applicationNameByGuid);
            }
            _ = stLastPath;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(applicationNameByGuid);
            saveFileDialog.InitialDirectory = stLastPath;
            saveFileDialog.Title = Strings.CreateBootAppTitle;
            saveFileDialog.AddExtension = false;
            saveFileDialog.Filter = Strings.FilterCreateBootApp;
            if (APEnvironment.SecurityMgr.X509CertificateProvider.IsUseCaseEnforced((X509UseCase)11) && (!(APEnvironment.SecurityMgr.CurrentUser is IX509CurrentUser2) || string.IsNullOrEmpty(((IX509CurrentUser2)APEnvironment.SecurityMgr.CurrentUser).SignatureThumbprint)))
            {
                throw new X509UsageException((X509UseCase)11, Strings.OperationAborted_MissingCertificate);
            }
            X509Certificate2Collection x509Certificate2Collection = null;
            IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidApplication).Object;
            IApplicationObject8 val = (IApplicationObject8)(object)((@object is IApplicationObject8) ? @object : null);
            if (val != null && ((IApplicationObject7)val).EncryptedDownloadWithX509Certificates)
            {
                foreach (string encryptedDownloadX509Thumbprint in val.EncryptedDownloadX509Thumbprints)
                {
                    X509Certificate2 certForThumbprint = APEnvironment.SecurityMgr.X509CertificateProvider.GetCertForThumbprint((X509UseCase)14, encryptedDownloadX509Thumbprint);
                    if (certForThumbprint != null)
                    {
                        if (x509Certificate2Collection == null)
                        {
                            x509Certificate2Collection = new X509Certificate2Collection();
                        }
                        x509Certificate2Collection.Add(certForThumbprint);
                    }
                }
                if (x509Certificate2Collection == null || x509Certificate2Collection.Count != val.EncryptedDownloadX509Thumbprints.Count)
                {
                    throw new X509UsageException((X509UseCase)14, Strings.OperationAborted_MissingCertificate);
                }
            }
            if (APEnvironment.SecurityMgr.X509CertificateProvider.IsUseCaseEnforced((X509UseCase)14) && (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0))
            {
                throw new X509UsageException((X509UseCase)14, Strings.OperationAborted_MissingCertificate);
            }
            if (saveFileDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) == DialogResult.OK)
            {
                try
                {
                    stLastPath = saveFileDialog.FileName;
                    _ = saveFileDialog.FilterIndex;
                    AuthFileStream val2 = new AuthFileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite);
                    BinaryWriter binaryWriter = new BinaryWriter((Stream)val2);
                    application.StreamDownloadCommand(binaryWriter);
                    binaryWriter.Flush();
                    binaryWriter.BaseStream.Flush();
                    ((Stream)val2).Close();
                }
                catch (CancelledByUserException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (OnlineManagerException val4)
                {
                    OnlineManagerException val5 = val4;
                    APEnvironment.MessageService.Error(((Exception)(object)val5).Message, "ErrorCreateBootApp01", Array.Empty<object>());
                }
                catch (Exception ex2)
                {
                    APEnvironment.MessageService.Error(ex2.Message, "ErrorCreateBootApp02", Array.Empty<object>());
                }
            }
        }

        private void DoIt(Guid guidAppl)
        {
            //IL_0069: Unknown result type (might be due to invalid IL or missing references)
            //IL_0070: Expected O, but got Unknown
            //IL_016b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0170: Unknown result type (might be due to invalid IL or missing references)
            //IL_0175: Unknown result type (might be due to invalid IL or missing references)
            //IL_017a: Unknown result type (might be due to invalid IL or missing references)
            //IL_017d: Invalid comparison between Unknown and I4
            //IL_0180: Unknown result type (might be due to invalid IL or missing references)
            //IL_0183: Invalid comparison between Unknown and I4
            //IL_01a0: Unknown result type (might be due to invalid IL or missing references)
            //IL_01a7: Unknown result type (might be due to invalid IL or missing references)
            //IL_01c5: Unknown result type (might be due to invalid IL or missing references)
            //IL_01ca: Unknown result type (might be due to invalid IL or missing references)
            //IL_01cc: Unknown result type (might be due to invalid IL or missing references)
            //IL_01cf: Invalid comparison between Unknown and I4
            //IL_0269: Unknown result type (might be due to invalid IL or missing references)
            Guid deviceAppGuidOfApplication = OnlineCommandHelper.GetDeviceAppGuidOfApplication(guidAppl);
            if (deviceAppGuidOfApplication != Guid.Empty)
            {
                DoIt(deviceAppGuidOfApplication);
            }
            IOnlineApplication application = OnlineCommandHelper.GetApplication(guidAppl);
            bool isLoggedIn = application.IsLoggedIn;
            if (isLoggedIn)
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                bool flag = false;
                if (primaryProject != null)
                {
                    IOnlineApplicationObject val = OnlineCommandHelper.OnlApplObject(guidAppl);
                    IDeviceObject val2 = (IDeviceObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, val.DeviceGuid).Object;
                    ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val2.DeviceIdentification);
                    flag = LocalTargetSettings.CloseWindowsOnBootproject.GetBoolValue(targetSettingsById);
                }
                if (flag)
                {
                    IEditorView[] editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews();
                    Debug.Assert(editorViews != null);
                    IEditorView[] array = editorViews;
                    foreach (IEditorView val3 in array)
                    {
                        ((IEngine)APEnvironment.Engine).Frame.CloseView((IView)(object)val3);
                    }
                }
                if (application is IOnlineApplication3)
                {
                    ((IOnlineApplication3)((application is IOnlineApplication3) ? application : null)).CreateBootProject();
                    OnlineCommandHelper.DownloadSystemApplications((IOnlineApplication7)(object)((application is IOnlineApplication7) ? application : null));
                }
                try
                {
                    new SourceDownload().DownloadFromCreatingBootproject();
                }
                catch
                {
                }
            }
            else
            {
                ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(guidAppl);
                bool flag2 = false;
                bool flag3 = false;
                if (compileContext == null)
                {
                    if (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateCode(guidAppl, false, false))
                    {
                        return;
                    }
                    flag2 = true;
                }
                else if (!APEnvironment.LanguageModelMgr.IsUpToDate(guidAppl))
                {
                    PromptResult val4;
                    if (((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(guidAppl) != null)
                    {
                        val4 = APEnvironment.MessageService.Prompt(Strings.QueryGenerateCode, (PromptChoice)3, (PromptResult)2, "PromptQueryGenerateCode", Array.Empty<object>());
                    }
                    else
                    {
                        val4 = (PromptResult)2;
                        flag2 = true;
                    }
                    if ((int)val4 == 1)
                    {
                        return;
                    }
                    if ((int)val4 == 2)
                    {
                        if (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateCode(guidAppl, false, false))
                        {
                            return;
                        }
                        flag3 = true;
                    }
                }
                SaveBootProjectHelp(guidAppl);
                PromptResult val5 = (PromptResult)3;
                if (flag2)
                {
                    val5 = (PromptResult)2;
                }
                else if (flag3)
                {
                    val5 = APEnvironment.MessageService.Prompt(Strings.QuerySafeReferenceContextOffline, (PromptChoice)2, (PromptResult)2, "QuerySafeReferenceContextOffline", Array.Empty<object>());
                }
                if ((int)val5 == 2)
                {
                    ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).UpdateDownloadContext(guidAppl);
                }
                IProject primaryProject2 = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject2 != null)
                {
                    if (stLastPath == string.Empty)
                    {
                        stLastPath = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path;
                    }
                    Guid visualManagerObjectFromApplication = OnlineCommandHelper.GetVisualManagerObjectFromApplication(primaryProject2.Handle, guidAppl);
                    if (visualManagerObjectFromApplication != Guid.Empty)
                    {
                        IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject2.Handle, visualManagerObjectFromApplication);
                        if (objectToRead != null && objectToRead.Object is IVisualManagerObject4)
                        {
                            ((IVisualManagerObject4)objectToRead.Object).CreateVisualizationFiles(Path.GetDirectoryName(stLastPath), true);
                        }
                    }
                }
            }
            foreach (IOnlineApplicationObject childApplicationObject in OnlineCommandHelper.GetChildApplicationObjects(guidAppl))
            {
                IOnlineApplicationObject4 val6 = (IOnlineApplicationObject4)(object)((childApplicationObject is IOnlineApplicationObject4) ? childApplicationObject : null);
                if (val6 != null)
                {
                    IOnlineApplication application2 = OnlineCommandHelper.GetApplication(((IOnlineApplicationObject)val6).ApplicationGuid);
                    if (application2 != null && application2.IsLoggedIn == isLoggedIn && val6.CreateBootProjectWithParent(true) && ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(application2.ApplicationGuid) != null)
                    {
                        DoIt(application2.ApplicationGuid);
                    }
                }
            }
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length != 0)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
            }
            ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
            Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
            DoIt(activeAppObjectGuid);
        }
    }
}
