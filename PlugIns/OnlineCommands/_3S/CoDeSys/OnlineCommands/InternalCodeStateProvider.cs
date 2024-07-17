using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class InternalCodeStateProvider
    {
        internal static readonly DateTime ORIGIN = new DateTime(1, 1, 1, 0, 0, 0);

        private static DateTime _dtLastModificationTime = ORIGIN;

        private static OnlineCodeState _OnlineCodeState = (OnlineCodeState)3;

        private static bool _bPauseUpdates = false;

        private static bool _bDeviceSupportsOnlineChange = true;

        private static Color _clrChangedCodeText = SystemColors.ControlText;

        private static Color _clrUnchangedCodeText = SystemColors.ControlText;

        private static Color _clrChangedCodeBackground = Color.Transparent;

        private static Color _clrUnchangedCodeBackground = Color.Transparent;

        private static Guid _guidLastActiveApp = Guid.Empty;

        private static HashSet<IProject> _loadedProjects = new HashSet<IProject>();

        private static Dictionary<Guid, IOnlineVarRef> s_dicAppStateVarRefs = new Dictionary<Guid, IOnlineVarRef>();

        private static readonly string EXCEPTIONSTATE = "__System.ExceptionFlags.g_dwExFlags";

        internal static DateTime LastModificationTime => _dtLastModificationTime;

        internal static Color ChangedCodeTextColor
        {
            get
            {
                return _clrChangedCodeText;
            }
            set
            {
                _clrChangedCodeText = value;
            }
        }

        internal static Color ChangedCodeBackgroundColor
        {
            get
            {
                return _clrChangedCodeBackground;
            }
            set
            {
                _clrChangedCodeBackground = value;
            }
        }

        internal static Color UnchangedCodeTextColor
        {
            get
            {
                return _clrUnchangedCodeText;
            }
            set
            {
                _clrUnchangedCodeText = value;
            }
        }

        internal static Color UnchangedCodeBackgroundColor
        {
            get
            {
                return _clrUnchangedCodeBackground;
            }
            set
            {
                _clrUnchangedCodeBackground = value;
            }
        }

        internal static OnlineCodeState InternalCodeState
        {
            get
            {
                //IL_001d: Unknown result type (might be due to invalid IL or missing references)
                //IL_0023: Invalid comparison between Unknown and I4
                //IL_0025: Unknown result type (might be due to invalid IL or missing references)
                //IL_002f: Unknown result type (might be due to invalid IL or missing references)
                if (_guidLastActiveApp != OnlineCommandHelper.ActiveAppObjectGuid || (IsRelevantApplicationOnline(OnlineCommandHelper.ActiveAppObjectGuid) && (int)_OnlineCodeState == 3))
                {
                    InternalCodeState = GetOnlineCodeState();
                }
                return _OnlineCodeState;
            }
            set
            {
                if (value != InternalCodeStateProvider._OnlineCodeState || InternalCodeStateProvider._guidLastActiveApp != OnlineCommandHelper.ActiveAppObjectGuid)
                {
                    InternalCodeStateProvider._guidLastActiveApp = OnlineCommandHelper.ActiveAppObjectGuid;
                    InternalCodeStateProvider._OnlineCodeState = value;
                    if (InternalCodeStateProvider.InternalCodeStateChanged != null)
                    {
                        InternalCodeStateProvider.InternalCodeStateChanged(new object(), new OnlineCodeStateEventArgs(InternalCodeStateProvider._OnlineCodeState));
                    }
                }
            }
        }

        internal static event OnlineCodeStateChangedHandler InternalCodeStateChanged;

        internal static event LoginWithOutdatedCodeHandler InternalLoginWithOutdatedCodeDetected;

        internal static void AttachToEvents()
        {

            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).TaskConfigChanged += (new CompileEventHandler(OnTaskConfigChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CompiledPOUChanged += (new CompiledPOUChangedEventHandler(OnCompiledPouChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CompiledPOUDeleted += (new CompiledPOUChangedEventHandler(OnCompiledPouChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CompiledPOUInserted += (new CompiledPOUChangedEventHandler(OnCompiledPouChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SignatureChanged += (new SignatureChangedEventHandler(OnSignatureChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SignatureDeleted += (new SignatureChangedEventHandler(OnSignatureChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SignatureInserted += (new SignatureChangedEventHandler(OnSignatureChanged));
            ((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout += (new AfterApplicationLogoutEventHandler(OnAfterAppLogout));
            ((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin += (new AfterApplicationLoginEventHandler(OnAfterAppLogin));
            ((IOnlineManager)APEnvironment.OnlineMgr).ApplicationStateChanged += (new ApplicationStateChangedEventHandler(OnAppStateChanged));
            ((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload += (new AfterApplicationDownloadEventHandler(OnAfterAppDownload));
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectModified += (new ObjectModifiedEventHandler(OnObjectModified));
            APEnvironment.ObjectMgr.ProjectLoadFinished += (new ProjectLoadFinishedEventHandler(OnProjectLoadFinished));
            ((IEngine)APEnvironment.Engine).Projects.PrimaryProjectSwitched += (new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
        }

        internal static void StopOnlineCodeStateUpdates()
        {
            _bPauseUpdates = true;
        }

        internal static void StartOnlineCodeStateUpdates()
        {
            _bPauseUpdates = false;
        }

        private static void OnAppStateChanged(object sender, OnlineEventArgs e)
        {
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            if (e.GuidObject == OnlineCommandHelper.ActiveAppObjectGuid)
            {
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.GuidObject);
                if (application != null && application.IsLoggedIn && (int)application.ApplicationState == 0)
                {
                    InternalCodeState = (OnlineCodeState)2;
                }
            }
        }

        private static void OnTaskConfigChanged(object sender, CompileEventArgs e)
        {
            if (IsRelevantApplicationOnline(e.ApplicationGuid))
            {
                InternalCodeState = (OnlineCodeState)2;
            }
        }

        private static void OnSignatureChanged(object sender, SignatureChangedEventArgs e)
        {
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0015: Invalid comparison between Unknown and I4
            if (e.Significant && !_bPauseUpdates && (int)_OnlineCodeState != 2 && IsRelevantApplicationOnline(((CompileEventArgs)e).ApplicationGuid) && (e.OldSignature != null || AreAllProjectsLoaded()))
            {
                if (_bDeviceSupportsOnlineChange)
                {
                    InternalCodeState = (OnlineCodeState)1;
                }
                else
                {
                    InternalCodeState = (OnlineCodeState)2;
                }
            }
        }

        private static void OnProjectLoadFinished(object sender, ProjectLoadFinishedEventArgs e)
        {
            IProject projectByHandle = ((IEngine)APEnvironment.Engine).Projects.GetProjectByHandle(e.ProjectHandle);
            if (projectByHandle != null && !_loadedProjects.Contains(projectByHandle))
            {
                _loadedProjects.Add(projectByHandle);
            }
        }

        private static bool AreAllProjectsLoaded()
        {
            IProject[] projectsByAttributes = ((IEngine)APEnvironment.Engine).Projects.GetProjectsByAttributes(new Guid[1] { ProjectAttributes.ProvidesLanguageModel });
            if (projectsByAttributes != null)
            {
                IProject[] array = projectsByAttributes;
                foreach (IProject item in array)
                {
                    if (!_loadedProjects.Contains(item))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void OnCompiledPouChanged(object sender, CompiledPOUChangedEventArgs e)
        {
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0015: Invalid comparison between Unknown and I4
            if (e.Significant && !_bPauseUpdates && (int)_OnlineCodeState != 2 && IsRelevantApplicationOnline(((CompileEventArgs)e).ApplicationGuid) && (e.OldCompiledPOU != null || AreAllProjectsLoaded()))
            {
                if (_bDeviceSupportsOnlineChange)
                {
                    InternalCodeState = (OnlineCodeState)1;
                }
                else
                {
                    InternalCodeState = (OnlineCodeState)2;
                }
            }
        }

        private static void OnAfterAppLogin(object sender, OnlineEventArgs e)
        {
            //IL_000d: Unknown result type (might be due to invalid IL or missing references)
            if (IsRelevantApplicationOnline(e.GuidObject))
            {
                InternalCodeState = GetOnlineCodeState();
                _bDeviceSupportsOnlineChange = OnlineCommandHelper.OnlineChangeSupported(OnlineCommandHelper.ActiveAppObjectGuid);
            }
            TryAddVarRef(e.GuidObject);
        }

        private static void TryAddVarRef(Guid appGuid)
        {
            try
            {
                if (s_dicAppStateVarRefs.ContainsKey(appGuid))
                {
                    return;
                }
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(appGuid);
                if (application != null && application.IsLoggedIn)
                {
                    IVarRef varReference = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(appGuid, EXCEPTIONSTATE);
                    if (varReference != null && varReference.AddressInfo != null)
                    {
                        IOnlineVarRef value = ((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(varReference);
                        s_dicAppStateVarRefs.Add(appGuid, value);
                    }
                }
            }
            catch
            {
            }
        }

        internal static bool SetApplicationExceptionState(Guid guidAppl, uint uiState)
        {
            try
            {
                if (s_dicAppStateVarRefs.ContainsKey(guidAppl))
                {
                    IOnlineVarRef val = s_dicAppStateVarRefs[guidAppl];
                    IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guidAppl);
                    val.PreparedValue = ((object)uiState);
                    IOnlineVarRef[] array = (IOnlineVarRef[])(object)new IOnlineVarRef[1] { val };
                    application.WriteVariables(array);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        internal static bool GetApplicationExceptionState(Guid guidAppl, out uint uiState)
        {
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            uiState = 0u;
            ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guidAppl);
            try
            {
                TryAddVarRef(guidAppl);
                if (s_dicAppStateVarRefs.ContainsKey(guidAppl))
                {
                    IOnlineVarRef val = s_dicAppStateVarRefs[guidAppl];
                    if ((int)val.State == 0)
                    {
                        uiState = (uint)val.Value;
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        private static void OnAfterAppLogout(object sender, OnlineEventArgs e)
        {
            if (e.GuidObject == OnlineCommandHelper.ActiveAppObjectGuid && !ApplicationIsLoggedIn(OnlineCommandHelper.ActiveAppObjectGuid))
            {
                InternalCodeState = (OnlineCodeState)3;
            }
            if (s_dicAppStateVarRefs.ContainsKey(e.GuidObject))
            {
                s_dicAppStateVarRefs.Remove(e.GuidObject);
            }
        }

        private static void OnAfterAppDownload(object sender, OnlineEventArgs e)
        {
            //IL_0009: Unknown result type (might be due to invalid IL or missing references)
            if (!(e is OnlineDownloadEventArgs2) || ((OnlineDownloadEventArgs2)e).DownloadException == null)
            {
                if (IsRelevantApplicationOnline(e.GuidObject))
                {
                    InternalCodeState = (OnlineCodeState)0;
                }
                if (s_dicAppStateVarRefs.ContainsKey(e.GuidObject))
                {
                    s_dicAppStateVarRefs.Remove(e.GuidObject);
                }
                TryAddVarRef(e.GuidObject);
            }
        }

        private static void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
        {
            //IL_001f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0029: Expected O, but got Unknown
            //IL_0034: Unknown result type (might be due to invalid IL or missing references)
            //IL_003e: Expected O, but got Unknown
            _dtLastModificationTime = ORIGIN;
            _loadedProjects.Clear();
            if (oldProject != null)
            {
                oldProject.DirtyChanged -= (new ProjectChangedEventHandler(OnPrimaryProjectDirtyChanged));
            }
            if (newProject != null)
            {
                newProject.DirtyChanged += (new ProjectChangedEventHandler(OnPrimaryProjectDirtyChanged));
            }
        }

        private static void OnPrimaryProjectDirtyChanged(IProject project)
        {
            if (project != null && project.Dirty)
            {
                _dtLastModificationTime = DateTime.Now;
            }
            else
            {
                _dtLastModificationTime = ORIGIN;
            }
        }

        private static void OnObjectModified(object sender, ObjectModifiedEventArgs e)
        {
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && e.ProjectHandle == ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle)
            {
                _dtLastModificationTime = DateTime.Now;
            }
        }

        private static bool IsRelevantApplicationOnline(Guid appGuid)
        {
            if ((appGuid == Guid.Empty || appGuid == OnlineCommandHelper.ActiveAppObjectGuid) && OnlineCommandHelper.ActiveAppObjectGuid != Guid.Empty && OnlineCommandHelper.IsLoggedIn(OnlineCommandHelper.ActiveAppObjectGuid))
            {
                return true;
            }
            return false;
        }

        private static OnlineCodeState GetOnlineCodeState()
        {
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            //IL_0042: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Invalid comparison between Unknown and I4
            //IL_0057: Unknown result type (might be due to invalid IL or missing references)
            //IL_0067: Unknown result type (might be due to invalid IL or missing references)
            //IL_006b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0075: Unknown result type (might be due to invalid IL or missing references)
            //IL_0077: Unknown result type (might be due to invalid IL or missing references)
            if (!_bPauseUpdates)
            {
                OnlineCodeState result = (OnlineCodeState)3;
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                int num = default(int);
                if (primaryProject != null && ((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(primaryProject.Handle, out num) && ApplicationIsLoggedIn(OnlineCommandHelper.ActiveAppObjectGuid))
                {
                    _bPauseUpdates = true;
                    try
                    {
                        if ((int)_OnlineCodeState != 2)
                        {
                            if (OnlineCommandHelper.IsUpToDate(OnlineCommandHelper.ActiveAppObjectGuid))
                            {
                                return (OnlineCodeState)0;
                            }
                            if (OnlineCommandHelper.CanOnlineChange(OnlineCommandHelper.ActiveAppObjectGuid))
                            {
                                return (OnlineCodeState)1;
                            }
                            return (OnlineCodeState)2;
                        }
                        return result;
                    }
                    finally
                    {
                        _bPauseUpdates = false;
                    }
                }
                return result;
            }
            return _OnlineCodeState;
        }

        private static bool ApplicationIsLoggedIn(Guid activeAppGuid)
        {
            if (activeAppGuid != Guid.Empty)
            {
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(activeAppGuid);
                if (application != null && application.IsLoggedIn)
                {
                    return true;
                }
            }
            return false;
        }

        internal static void RaiseLoginWithOutdatedCodeDetected()
        {
            if ((object)InternalCodeStateProvider.InternalLoginWithOutdatedCodeDetected != null)
            {
                InternalCodeStateProvider.InternalLoginWithOutdatedCodeDetected.Invoke(new object(), new EventArgs());
            }
        }
    }
}
