using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.ProjectInfoObject;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{6FF9FFDB-8572-4B73-96F3-9F9404D11BCF}")]
    [SuppressMessage("AP.Patterns", "Pat002", Justification = "No F1 help available, opens dialog on doubleklick in status field.Help is to be shown for opened dialog.")]
    public class ShowCompileChangeDetailsCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "compile", "ShowCompileChangeDetails" };

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ShowCompileChangeDetails_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ShowCompileChangeDetails_Name");

        public bool Enabled => true;

        public Icon SmallIcon => null;

        public Guid Category => Guid.Empty;

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
            return !bContextMenu;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 1);
            }
            try
            {
                IOnlineApplication application = OnlineCommandHelper.GetApplication(OnlineCommandHelper.ActiveAppObjectGuid);
                IOnlineApplication14 val = (IOnlineApplication14)(object)((application is IOnlineApplication14) ? application : null);
                IApplicationInfoObjectProperty applicationInfoObjectProperty = OnlineCommandHelper.GetApplicationInfoObjectProperty(((IOnlineApplication)val).ApplicationGuid);
                IProjectInfoObject projectInfoObject = OnlineCommandHelper.GetProjectInfoObject();
                IOnlineApplicationInfo val2 = ((IOnlineApplication10)val).ReadApplicationInfo();
                if (val2 == null)
                {
                    APEnvironment.MessageService.Error(Strings.CannotReadApplicationInfoFromPLC, "CannotReadApplicationInfoFromPLC01", Array.Empty<object>());
                    return;
                }
                //((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                string text = ((!((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("Online", "ProjectNameIDE")) ? Path.GetFileNameWithoutExtension(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path) : ((IEngine3)APEnvironment.Engine).OEMCustomization.GetStringValue("Online", "ProjectNameIDE"));
                string stApplicationName = text;
                IOnlineApplicationObject obj = OnlineCommandHelper.OnlApplObject(((IOnlineApplication)val).ApplicationGuid);
                IOnlineApplicationObject2 val3 = (IOnlineApplicationObject2)(object)((obj is IOnlineApplicationObject2) ? obj : null);
                if (val3 != null)
                {
                    stApplicationName = val3.ApplicationName;
                }
                string projectName = val2.ProjectName;
                string emptyDownloadTime = Strings.EmptyDownloadTime;
                string stLastModificationPLC = OnlineCommandHelper.FormatTimestamp(val2.LastModification);
                string profileName = APEnvironment.ProfileName;
                string profile = val2.Profile;
                string author = OnlineCommandHelper.GetAuthor(applicationInfoObjectProperty, projectInfoObject);
                string author2 = val2.Author;
                string version = OnlineCommandHelper.GetVersion(applicationInfoObjectProperty, projectInfoObject);
                string version2 = val2.Version;
                string description = OnlineCommandHelper.GetDescription(applicationInfoObjectProperty, projectInfoObject);
                string description2 = val2.Description;
                IApplicationContent appcontentIDE;
                try
                {
                    appcontentIDE = APEnvironment.LMServiceProvider.CompileService.QueryCompiledApplicationSet(((IOnlineApplication)val).ApplicationGuid).ApplicationContent;
                }
                catch
                {
                    appcontentIDE = null;
                }
                IApplicationContent appcontentPLC;
                try
                {
                    appcontentPLC = APEnvironment.LMServiceProvider.DownloadedApplicationService.QueryCompiledApplicationSet(((IOnlineApplication)val).ApplicationGuid).ApplicationContent;
                }
                catch
                {
                    appcontentPLC = null;
                }
                IEnumerable<IChangedLMObject> changedObjects = null;
                ILMCompiledApplicationSet compiledApplicationSet = APEnvironment.LMServiceProvider.CompileService.GetCompiledApplicationSet(((IOnlineApplication)val).ApplicationGuid);
                ICompileContext9 comcon = (ICompileContext9)(object)((compiledApplicationSet is ICompileContext9) ? compiledApplicationSet : null);
                if (val != null)
                {
                    changedObjects = OnlineCommandHelper.GetChangedObjects(((IOnlineApplication)val).ApplicationGuid);
                }
                ApplicationInfoDialogWithContent applicationInfoDialogWithContent = new ApplicationInfoDialogWithContent();
                applicationInfoDialogWithContent.Initialize(text, projectName, emptyDownloadTime, stLastModificationPLC, profileName, profile, author, author2, version, version2, description, description2, changedObjects, appcontentIDE, appcontentPLC, comcon, ((IOnlineApplication)val).ApplicationGuid, stApplicationName);
                applicationInfoDialogWithContent.ShowDialog();
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "ErrorShowCompileChangeDetailsCommand", Array.Empty<object>());
            }
        }
    }
}
