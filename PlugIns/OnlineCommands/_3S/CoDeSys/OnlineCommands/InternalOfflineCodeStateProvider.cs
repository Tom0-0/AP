using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class InternalOfflineCodeStateProvider
    {
        internal static readonly DateTime ORIGIN = new DateTime(1, 1, 1, 0, 0, 0);

        private static DateTime _dtLastModificationTime = ORIGIN;

        private static DateTime _dtNextAllowedUpdate = ORIGIN;

        private static int _nSuppressUpdateTimespanInSeconds = 5;

        private static OfflineCodeState _OfflineCodeState = OfflineCodeState.UNKNOWN;

        private static bool _bPauseUpdates = false;

        private static Color _clrUnchangedCodeText = SystemColors.ControlText;

        private static Color _clrUnchangedCodeBackground = Color.Transparent;

        private static Guid _guidLastActiveApp = Guid.Empty;

        private static bool _bDeviceSupportsOnlineChange = false;

        private static bool _bLazyLoadFinished = false;

        private static bool _bUpdateCodeStateRequested;

        internal static DateTime LastModificationTime => _dtLastModificationTime;

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

        internal static OfflineCodeState InternalCodeState
        {
            get
            {
                if (ApplicationIsLoggedIn(OnlineCommandHelper.ActiveAppObjectGuid))
                {
                    InternalCodeState = OfflineCodeState.UNKNOWN;
                }
                else if (_guidLastActiveApp != OnlineCommandHelper.ActiveAppObjectGuid || (IsRelevantApplicationActive(OnlineCommandHelper.ActiveAppObjectGuid) && _OfflineCodeState == OfflineCodeState.UNKNOWN))
                {
                    if (APEnvironment.LMServiceProvider.DownloadedApplicationService.GetCompiledApplicationSet(OnlineCommandHelper.ActiveAppObjectGuid) == null)
                    {
                        InternalCodeState = OfflineCodeState.NO_CODE_GENERATED;
                    }
                    else
                    {
                        _bUpdateCodeStateRequested = true;
                    }
                }
                return _OfflineCodeState;
            }
            set
            {
                if (value != _OfflineCodeState || _guidLastActiveApp != OnlineCommandHelper.ActiveAppObjectGuid)
                {
                    _guidLastActiveApp = OnlineCommandHelper.ActiveAppObjectGuid;
                    _OfflineCodeState = value;
                }
            }
        }

        internal static void AttachToEvents()
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0016: Expected O, but got Unknown
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            //IL_002c: Expected O, but got Unknown
            //IL_0038: Unknown result type (might be due to invalid IL or missing references)
            //IL_0042: Expected O, but got Unknown
            //IL_004e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0058: Expected O, but got Unknown
            //IL_0064: Unknown result type (might be due to invalid IL or missing references)
            //IL_006e: Expected O, but got Unknown
            //IL_007a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0084: Expected O, but got Unknown
            //IL_0090: Unknown result type (might be due to invalid IL or missing references)
            //IL_009a: Expected O, but got Unknown
            //IL_00a6: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b0: Expected O, but got Unknown
            //IL_00f2: Unknown result type (might be due to invalid IL or missing references)
            //IL_00fc: Expected O, but got Unknown
            //IL_0108: Unknown result type (might be due to invalid IL or missing references)
            //IL_0112: Expected O, but got Unknown
            //IL_0123: Unknown result type (might be due to invalid IL or missing references)
            //IL_012d: Expected O, but got Unknown
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).TaskConfigChanged += (new CompileEventHandler(OnTaskConfigChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CompiledPOUChanged += (new CompiledPOUChangedEventHandler(OnCompiledPouChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CompiledPOUDeleted += (new CompiledPOUChangedEventHandler(OnCompiledPouChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CompiledPOUInserted += (new CompiledPOUChangedEventHandler(OnCompiledPouChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SignatureChanged += (new SignatureChangedEventHandler(OnSignatureChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SignatureDeleted += (new SignatureChangedEventHandler(OnSignatureChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).SignatureInserted += (new SignatureChangedEventHandler(OnSignatureChanged));
            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CodeChanged += (new CompileEventHandler(OnCodeChanged));
            APEnvironment.LMServiceProvider.LanguageModelProviderService.AfterLazyLibraryLoad += ((EventHandler)OnLazyLoadFinished);
            APEnvironment.LMServiceProvider.CommandService.BeforeClearAll += ((EventHandler)OnBeforeClearAll);
            ((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout += (new AfterApplicationLogoutEventHandler(OnAfterAppLogout));
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectModified += (new ObjectModifiedEventHandler(OnObjectModified));
            ((IEngine)APEnvironment.Engine).Projects.PrimaryProjectSwitched += (new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
        }

        internal static void OnIdleDoUpdate()
        {
            if (_bUpdateCodeStateRequested && !APEnvironment.Engine.LengthyOperationIsActive && _bLazyLoadFinished && IsUpdateIntervalElapsed(DateTime.Now))
            {
                _bUpdateCodeStateRequested = false;
                InternalCodeState = GetOfflineCodeState();
            }
        }

        internal static void StopOfflineCodeStateUpdates()
        {
            _bPauseUpdates = true;
        }

        internal static void StartOfflineCodeStateUpdates()
        {
            _bPauseUpdates = false;
        }

        private static void OnTaskConfigChanged(object sender, CompileEventArgs e)
        {
            if (IsRelevantApplicationActive(e.ApplicationGuid) && !_bPauseUpdates && _bLazyLoadFinished)
            {
                _bUpdateCodeStateRequested = true;
            }
        }

        private static void OnSignatureChanged(object sender, SignatureChangedEventArgs e)
        {
            if (e.Significant && !_bPauseUpdates && IsRelevantApplicationActive(((CompileEventArgs)e).ApplicationGuid) && _bLazyLoadFinished && e.OldSignature != null)
            {
                _bUpdateCodeStateRequested = true;
            }
        }

        private static void OnCompiledPouChanged(object sender, CompiledPOUChangedEventArgs e)
        {
            if (e.Significant && !_bPauseUpdates && IsRelevantApplicationActive(((CompileEventArgs)e).ApplicationGuid) && _bLazyLoadFinished)
            {
                _bUpdateCodeStateRequested = true;
            }
        }

        private static void OnCodeChanged(object sender, CompileEventArgs e)
        {
            if (IsRelevantApplicationActive(e.ApplicationGuid) && !_bPauseUpdates && _bLazyLoadFinished)
            {
                _bUpdateCodeStateRequested = true;
            }
        }

        private static void OnLazyLoadFinished(object sender, EventArgs e)
        {
            _bDeviceSupportsOnlineChange = OnlineCommandHelper.OnlineChangeSupported(OnlineCommandHelper.ActiveAppObjectGuid);
            _bLazyLoadFinished = true;
            _bUpdateCodeStateRequested = true;
        }

        private static void OnBeforeClearAll(object sender, EventArgs e)
        {
            _bLazyLoadFinished = false;
            InternalCodeState = OfflineCodeState.NO_CODE_GENERATED;
        }

        private static void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Expected O, but got Unknown
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0037: Expected O, but got Unknown
            //IL_0042: Unknown result type (might be due to invalid IL or missing references)
            //IL_004c: Expected O, but got Unknown
            //IL_0054: Unknown result type (might be due to invalid IL or missing references)
            //IL_005e: Expected O, but got Unknown
            _dtLastModificationTime = ORIGIN;
            _bLazyLoadFinished = false;
            if (oldProject != null)
            {
                oldProject.DirtyChanged -= (new ProjectChangedEventHandler(OnPrimaryProjectDirtyChanged));
                oldProject.ActiveApplicationChanged -= (new ProjectChangedEventHandler(OnActiveAppChanged));
            }
            if (newProject != null)
            {
                newProject.DirtyChanged += (new ProjectChangedEventHandler(OnPrimaryProjectDirtyChanged));
                newProject.ActiveApplicationChanged += (new ProjectChangedEventHandler(OnActiveAppChanged));
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

        private static void OnAfterAppLogout(object sender, OnlineEventArgs e)
        {
            if (IsRelevantApplicationActive(e.GuidObject))
            {
                InternalCodeState = OfflineCodeState.CODE_UP_TO_DATE;
                _bUpdateCodeStateRequested = true;
            }
        }

        private static void OnActiveAppChanged(IProject project)
        {
            if (project != null)
            {
                _bDeviceSupportsOnlineChange = OnlineCommandHelper.OnlineChangeSupported(OnlineCommandHelper.ActiveAppObjectGuid);
            }
        }

        private static bool IsRelevantApplicationActive(Guid appGuid)
        {
            if ((appGuid == Guid.Empty || appGuid == OnlineCommandHelper.ActiveAppObjectGuid) && OnlineCommandHelper.ActiveAppObjectGuid != Guid.Empty)
            {
                return true;
            }
            return false;
        }

        private static bool IsUpdateIntervalElapsed(DateTime dtUpdateRequest)
        {
            if (dtUpdateRequest > _dtNextAllowedUpdate)
            {
                _dtNextAllowedUpdate = dtUpdateRequest.AddSeconds(_nSuppressUpdateTimespanInSeconds);
                return true;
            }
            return false;
        }

        private static OfflineCodeState GetOfflineCodeState()
        {
            if (!_bPauseUpdates)
            {
                OfflineCodeState result = OfflineCodeState.UNKNOWN;
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
                {
                    if (!_bLazyLoadFinished || APEnvironment.LMServiceProvider.LanguageModelProviderService.DelayedLoaderWorking)
                    {
                        return result;
                    }
                    try
                    {
                        _bPauseUpdates = true;
                        if (APEnvironment.LMServiceProvider.DownloadedApplicationService.GetCompiledApplicationSet(OnlineCommandHelper.ActiveAppObjectGuid) == null)
                        {
                            return OfflineCodeState.NO_CODE_GENERATED;
                        }
                        bool bOnlineChangePossible = false;
                        if (OnlineCommandHelper.IsUpToDate(OnlineCommandHelper.ActiveAppObjectGuid, out bOnlineChangePossible))
                        {
                            return OfflineCodeState.CODE_UP_TO_DATE;
                        }
                        if (_bDeviceSupportsOnlineChange && bOnlineChangePossible)
                        {
                            return OfflineCodeState.ONLINECHANGE_POSSIBLE;
                        }
                        return OfflineCodeState.DOWNLOAD_NEEDED;
                    }
                    finally
                    {
                        _bPauseUpdates = false;
                    }
                }
                return result;
            }
            return _OfflineCodeState;
        }

        private static bool ApplicationIsLoggedIn(Guid activeAppGuid)
        {
            if (activeAppGuid != Guid.Empty)
            {
                try
                {
                    IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(activeAppGuid);
                    if (application != null && application.IsLoggedIn)
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
    }
}
