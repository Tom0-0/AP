#define DEBUG
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{EE464F31-CC6E-44c0-B222-7B40646A3C5A}")]
    [SystemInterface("_3S.CoDeSys.OnlineUI.IOnlineUIServices")]
    public class OnlineUIManager : IOnlineUIServices5, IOnlineUIServices4, IOnlineUIServices3, IOnlineUIServices2, IOnlineUIServices, ISystemInstanceRequiresInitialization
    {
        private object _preselectedOnlineState;

        private ArrayList _autoSwitches = new ArrayList();

        private LHashSet<Guid> _applicationsInOnlinePerspective = new LHashSet<Guid>();

        private int _allowOnlineModificationCounter;

        private static string OnlinePerspectiveName
        {
            get
            {
                string text = string.Empty;
                if (OptionsHelper.UseHmiPerspective)
                {
                    text = "HMI";
                }
                if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("OnlineCommands", "OnlinePerspectiveName"))
                {
                    return text + ((IEngine3)APEnvironment.Engine).OEMCustomization.GetStringValue("OnlineCommands", "OnlinePerspectiveName");
                }
                return text + "Online";
            }
        }

        private static string OnlineBackupPerspectiveName
        {
            get
            {
                string text = string.Empty;
                if (OptionsHelper.UseHmiPerspective)
                {
                    text = "HMI";
                }
                if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("OnlineCommands", "OnlineBackupPerspectiveName"))
                {
                    return text + ((IEngine3)APEnvironment.Engine).OEMCustomization.GetStringValue("OnlineCommands", "OnlineBackupPerspectiveName");
                }
                return text + "Online_Backup";
            }
        }

        internal bool AllowOnlineModification => _allowOnlineModificationCounter > 0;

        public event AfterLoginEventHandler AfterLogin;

        public event AfterLogoutEventHandler AfterLogout;

        public void OnAllSystemInstancesAvailable()
        {

            ((IEngine)APEnvironment.Engine).UIReady += (new EngineStateHandler(OnUIReady));
            ((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout += (new AfterApplicationLogoutEventHandler(OnAfterApplicationLogout));
            ((IOnlineManager2)APEnvironment.OnlineMgr).AfterApplicationDownload += (new AfterApplicationDownloadEventHandler(OnAfterApplicationDownload));
            ((IOnlineManager7)APEnvironment.OnlineMgr).FlowControlEnabledChanged += ((EventHandler<OnlineEventArgs>)OnFlowControlEnabledChanged);
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved += (new ObjectRemovedEventHandler(OnObjectRemoved));
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoving += (new ObjectCancelEventHandler(OnObjectRemoving));
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectRenaming += (new ObjectRenamingEventHandler(OnObjectRenaming));
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectAdding += (new ObjectAddingEventHandler(OnObjectAdding));
            InternalCodeStateProvider.AttachToEvents();
            InternalOfflineCodeStateProvider.AttachToEvents();
            OnlineCommandHelper.RegisterEvents();
        }

        private void OnUIReady()
        {
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Expected O, but got Unknown
            //IL_0038: Unknown result type (might be due to invalid IL or missing references)
            //IL_0042: Expected O, but got Unknown
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_007f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0089: Expected O, but got Unknown
            if (((IEngine)APEnvironment.Engine).Frame != null)
            {
                ((IEngine)APEnvironment.Engine).Frame.ViewOpening += (new ViewCancelEventHandler(OnViewOpening));
                ((IEngine)APEnvironment.Engine).Frame.ViewClosed += (new ViewEventHandler(OnViewClosed));
            }
            if (((IEngine)APEnvironment.Engine).Frame is IFrame9)
            {
                ((IFrame9)((IEngine)APEnvironment.Engine).Frame).WindowLayoutResetting += ((EventHandler)OnWindowLayoutResetting);
            }
            ((IEngine)APEnvironment.Engine).UIReady -= (new EngineStateHandler(OnUIReady));
        }

        public void Login(Guid onlineApplicationGuid)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Expected O, but got Unknown
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            //IL_009a: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a1: Expected O, but got Unknown
            //IL_00a3: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ce: Expected O, but got Unknown
            //IL_00dd: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e2: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ee: Unknown result type (might be due to invalid IL or missing references)
            //IL_0156: Unknown result type (might be due to invalid IL or missing references)
            //IL_0163: Unknown result type (might be due to invalid IL or missing references)
            //IL_0168: Unknown result type (might be due to invalid IL or missing references)
            //IL_0181: Unknown result type (might be due to invalid IL or missing references)
            //IL_0219: Unknown result type (might be due to invalid IL or missing references)
            //IL_023b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0280: Unknown result type (might be due to invalid IL or missing references)
            //IL_0285: Unknown result type (might be due to invalid IL or missing references)
            //IL_028c: Expected O, but got Unknown
            //IL_02b9: Unknown result type (might be due to invalid IL or missing references)
            //IL_02f3: Unknown result type (might be due to invalid IL or missing references)
            //IL_034b: Unknown result type (might be due to invalid IL or missing references)
            //IL_03c3: Unknown result type (might be due to invalid IL or missing references)
            if ((object)this.AfterLogin != null)
            {
                this.AfterLogin.Invoke((object)this, new OnlineUIServicesEventArgs(onlineApplicationGuid));
            }
            if (((IEngine)APEnvironment.Engine).Frame == null)
            {
                return;
            }
            IView val = null;
            IEditorView val2 = null;
            if (((IEngine)APEnvironment.Engine).Frame is IFrame15)
            {
                val = ((IFrame15)((IEngine)APEnvironment.Engine).Frame).ViewInFront;
            }
            else
            {
                val2 = ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront;
            }
            IEditorView[] editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews();
            Debug.Assert(editorViews != null);
            IEditorView[] array = editorViews;
            OnlineState val6 = default(OnlineState);
            foreach (IEditorView val3 in array)
            {
                try
                {
                    if (!(val3 is IHasOnlineMode))
                    {
                        continue;
                    }
                    IHasOnlineMode val4 = (IHasOnlineMode)val3;
                    if (val4.OnlineState.OnlineApplication == Guid.Empty)
                    {
                        if (!(val3 is IHasOnlineMode2))
                        {
                            goto IL_00fa;
                        }
                        IHasOnlineMode2 val5 = (IHasOnlineMode2)val4;
                        if (val5 == null || !val5.NotifyOnly())
                        {
                            goto IL_00fa;
                        }
                        OnlineState onlineState = ((IHasOnlineMode)val5).OnlineState;
                        onlineState.OnlineApplication = onlineApplicationGuid;
                        SetOnlineState((IHasOnlineMode)(object)val5, onlineState);
                    }
                    goto end_IL_008c;
                IL_00fa:
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val3.Editor.ProjectHandle, val3.Editor.ObjectGuid);
                    Debug.Assert(metaObjectStub != null);
                    if (OnlineCommandHelper.CollectInstancePaths(metaObjectStub, onlineApplicationGuid, out var result, out var _, out var _, out var _) && result.Length == 1)
                    {
                        val6.InstancePath = result[0];
                        val6.OnlineApplication = onlineApplicationGuid;
                        if (!ExistsView(metaObjectStub, val6))
                        {
                            OnlineStateAutoSwitch value = new OnlineStateAutoSwitch(val3, val4.OnlineState, val6);
                            _autoSwitches.Add(value);
                            SetOnlineState(val4, val6);
                        }
                    }
                end_IL_008c:;
                }
                catch
                {
                }
            }
            OnlineViewInfoTable onlineViewInfoTable = OptionsHelper.OnlineViewInfoTable;
            Debug.Assert(onlineViewInfoTable != null);
            OnlineViewInfo[] onlineViews = onlineViewInfoTable.GetOnlineViews(onlineApplicationGuid);
            Debug.Assert(onlineViews != null);
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            OnlineViewInfo[] array2 = onlineViews;
            OnlineState val7 = default(OnlineState);
            foreach (OnlineViewInfo onlineViewInfo in array2)
            {
                if (!Common.GetApplicationGuid(handle, onlineViewInfo.ObjectGuid).Equals(onlineApplicationGuid))
                {
                    continue;
                }
                try
                {
                    if (((IEngine)APEnvironment.Engine).Frame is IFrame6)
                    {
                        ((IFrame6)((IEngine)APEnvironment.Engine).Frame).SetOpenViewReason((OpenViewReason)2);
                    }
                    val7.OnlineApplication = onlineApplicationGuid;
                    val7.InstancePath = onlineViewInfo.InstancePath;
                    BeginPreselectOnlineState(val7);
                    IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, onlineViewInfo.ObjectGuid);
                    IEditorView val8 = ((IEngine)APEnvironment.Engine).Frame.OpenEditorView(metaObjectStub2, onlineViewInfo.FactoryGuid, (string)null);
                    if (val8 != null && val8 is IHasOnlineMode)
                    {
                        SetOnlineState((IHasOnlineMode)val8, val7);
                    }
                    if (!onlineViewInfo.FloatingRect.IsEmpty && ((IEngine)APEnvironment.Engine).Frame is IFrame11)
                    {
                        ((IFrame11)((IEngine)APEnvironment.Engine).Frame).SetFloatingRectangle((IView)(object)val8, onlineViewInfo.FloatingRect, false);
                    }
                }
                catch
                {
                }
                finally
                {
                    EndPreselectOnlineState();
                    if (((IEngine)APEnvironment.Engine).Frame is IFrame6)
                    {
                        ((IFrame6)((IEngine)APEnvironment.Engine).Frame).ResetOpenViewReason();
                    }
                }
            }
            string onlinePerspectiveName = OnlinePerspectiveName;
            string onlineBackupPerspectiveName = OnlineBackupPerspectiveName;
            if (_applicationsInOnlinePerspective.Count == 0 && ((IEngine)APEnvironment.Engine).Frame is IFrame9 && onlinePerspectiveName != null && onlineBackupPerspectiveName != null)
            {
                IPerspectiveManager perspectiveMgr = ((IFrame9)((IEngine)APEnvironment.Engine).Frame).PerspectiveMgr;
                IPerspective val9 = perspectiveMgr.GetPerspective(onlineBackupPerspectiveName);
                if (val9 == null)
                {
                    val9 = perspectiveMgr.CreatePerspective(onlineBackupPerspectiveName);
                }
                val9.Update();
                IPerspective val10 = perspectiveMgr.GetPerspective(onlinePerspectiveName);
                if (val10 == null)
                {
                    val10 = perspectiveMgr.CreatePerspective(onlinePerspectiveName);
                }
                val10.Activate(false);
            }
            _applicationsInOnlinePerspective.Add(onlineApplicationGuid);
            if (val != null && ((IEngine)APEnvironment.Engine).Frame is IFrame15)
            {
                ((IFrame15)((IEngine)APEnvironment.Engine).Frame).ViewInFront = (val);
            }
            else if (val2 != null)
            {
                ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront = (val2);
            }
        }

        public void BeginPreselectOnlineState(OnlineState onlineState)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            _preselectedOnlineState = onlineState;
        }

        public void EndPreselectOnlineState()
        {
            _preselectedOnlineState = null;
        }

        public bool SelectOnlineState(int nProjectHandle, Guid objectGuid, out OnlineState onlineState)
        {
            return SelectOnlineStateFormatted(nProjectHandle, objectGuid, null, out onlineState);
        }

        public bool SelectOnlineStateFormatted(int nProjectHandle, Guid objectGuid, IInstanceFormatter formatter, out OnlineState onlineState)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_0029: Unknown result type (might be due to invalid IL or missing references)
            //IL_014a: Unknown result type (might be due to invalid IL or missing references)
            //IL_014f: Unknown result type (might be due to invalid IL or missing references)
            onlineState.OnlineApplication = Guid.Empty;
            onlineState.InstancePath = null;
            if (_preselectedOnlineState != null)
            {
                onlineState = (OnlineState)_preselectedOnlineState;
                return true;
            }
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
            Debug.Assert(metaObjectStub != null);
            string[] result;
            IList<Guid> guidApps;
            string implementation;
            ISignature parentsign;
            bool flag = OnlineCommandHelper.CollectInstancePaths(metaObjectStub, Guid.Empty, out result, out guidApps, out implementation, out parentsign);
            if (flag && (result == null || result.Length == 0) && !string.IsNullOrEmpty(implementation))
            {
                Common.SplitInstancePath(implementation, out var _, out var _, out var applicationGuid);
                onlineState.InstancePath = implementation;
                onlineState.OnlineApplication = applicationGuid;
            }
            else if (flag && result != null && result.Length != 0)
            {
                if (result.Length == 1)
                {
                    AcceptSingleInstancePath(ref onlineState, guidApps, result[0]);
                }
                else
                {
                    string stPreselectedInstancePath = GetInstancePathFromEditor();
                    if (!string.IsNullOrEmpty(stPreselectedInstancePath) && result.Any((string p) => p.Equals(stPreselectedInstancePath, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        AcceptSingleInstancePath(ref onlineState, guidApps, stPreselectedInstancePath);
                    }
                    else
                    {
                        SelectOnlineStateDialog selectOnlineStateDialog = new SelectOnlineStateDialog();
                        selectOnlineStateDialog.Initialize(metaObjectStub, result, formatter, implementation);
                        Guid guid = new Guid("{DF44AB4F-DDEA-48A8-923E-3796DC28F1FD}");
                        DialogHelper.RestoreStateWithPosition((Form)selectOnlineStateDialog, guid);
                        if (((IEngine)APEnvironment.Engine).Frame == null || selectOnlineStateDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) != DialogResult.OK)
                        {
                            return false;
                        }
                        onlineState = selectOnlineStateDialog.OnlineState;
                        DialogHelper.StoreState((Form)selectOnlineStateDialog, guid);
                    }
                }
            }
            return true;
        }

        private void AcceptSingleInstancePath(ref OnlineState onlineState, IList<Guid> guidApps, string stInstancePath)
        {
            Common.SplitInstancePath(stInstancePath, out var _, out var _, out var applicationGuid);
            onlineState.InstancePath = stInstancePath;
            if (guidApps != null && guidApps.Count == 1)
            {
                onlineState.OnlineApplication = guidApps[0];
            }
            else
            {
                onlineState.OnlineApplication = applicationGuid;
            }
        }

        private string GetInstancePathFromEditor()
        {
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0064: Expected O, but got Unknown
            //IL_006a: Unknown result type (might be due to invalid IL or missing references)
            string empty = string.Empty;
            try
            {
                IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
                IEditorView val = (IEditorView)(object)((activeView is IEditorView) ? activeView : null);
                if (val != null)
                {
                    long num = default(long);
                    int num2 = default(int);
                    val.GetSelection(out num, out num2);
                    if (num2 > 0)
                    {
                        long num3 = default(long);
                        short num4 = default(short);
                        PositionHelper.SplitPosition(num, out num3, out num4);
                        num4 = (short)(num4 + num2);
                        num = PositionHelper.CombinePosition(num3, num4);
                    }
                    if (val is ISupportOnlineInstancePreselection)
                    {
                        empty = ((ISupportOnlineInstancePreselection)((val is ISupportOnlineInstancePreselection) ? val : null)).GetInstancePath(num);
                        IHasOnlineMode val2 = (IHasOnlineMode)val;
                        if (val2 != null)
                        {
                            return Common.NormalizeInstancePath(val2.OnlineState.OnlineApplication, val.Editor.ObjectGuid, empty);
                        }
                        return empty;
                    }
                    return empty;
                }
                return empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static bool ExistsView(IMetaObjectStub mos, OnlineState onlineState)
        {
            //IL_0039: Unknown result type (might be due to invalid IL or missing references)
            //IL_003f: Expected O, but got Unknown
            //IL_0042: Unknown result type (might be due to invalid IL or missing references)
            if (((IEngine)APEnvironment.Engine).Frame == null)
            {
                return false;
            }
            IEditorView[] editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews(mos);
            Debug.Assert(editorViews != null);
            IEditorView[] array = editorViews;
            foreach (IEditorView val in array)
            {
                if (val is IHasOnlineMode)
                {
                    IHasOnlineMode hasOnlineMode = (IHasOnlineMode)val;
                    if (onlineState.Equals(hasOnlineMode.OnlineState))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnViewOpening(object sender, ViewCancelEventArgs e)
        {
            //IL_0034: Unknown result type (might be due to invalid IL or missing references)
            //IL_003a: Expected O, but got Unknown
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e.View == null)
            {
                throw new ArgumentException("e.View is null.");
            }
            if (e.View is IHasOnlineMode)
            {
                IHasOnlineMode val = (IHasOnlineMode)e.View;
                OnlineState state = default(OnlineState);
                if (val.SelectOnlineState((IOnlineUIServices)(object)this, out state))
                {
                    SetOnlineState(val, state);
                }
                else
                {
                    ((CancelEventArgs)(object)e).Cancel = true;
                }
            }
        }

        private void OnViewClosed(object sender, ViewEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e.View == null)
            {
                throw new ArgumentException("e.View is null.");
            }
            for (int num = _autoSwitches.Count - 1; num >= 0; num--)
            {
                if (((OnlineStateAutoSwitch)_autoSwitches[num]).View == e.View)
                {
                    _autoSwitches.RemoveAt(num);
                }
            }
        }

        private void OnWindowLayoutResetting(object sender, EventArgs e)
        {
            //IL_001e: Unknown result type (might be due to invalid IL or missing references)
            IPerspectiveManager val = ((((IEngine)APEnvironment.Engine).Frame is IFrame9) ? ((IFrame9)((IEngine)APEnvironment.Engine).Frame).PerspectiveMgr : null);
            if (val == null)
            {
                return;
            }
            if (_applicationsInOnlinePerspective.Count == 0)
            {
                string onlinePerspectiveName = OnlinePerspectiveName;
                IPerspective val2 = ((onlinePerspectiveName != null) ? val.GetPerspective(onlinePerspectiveName) : null);
                if (val2 != null)
                {
                    val2.Remove();
                }
                return;
            }
            string onlineBackupPerspectiveName = OnlineBackupPerspectiveName;
            IPerspective val3 = ((onlineBackupPerspectiveName != null) ? val.GetPerspective(onlineBackupPerspectiveName) : null);
            if (val3 != null)
            {
                val3.Activate(true);
            }
            string onlinePerspectiveName2 = OnlinePerspectiveName;
            IPerspective val4 = ((onlinePerspectiveName2 != null) ? val.GetPerspective(onlinePerspectiveName2) : null);
            if (val4 != null)
            {
                val4.Remove();
            }
            if (val3 != null)
            {
                val3.Update();
            }
            val4 = val.CreatePerspective(onlinePerspectiveName2);
            val4.Activate(false);
        }

        private void OnAfterApplicationLogout(object sender, OnlineEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (this.AfterLogout != null)
            {
                this.AfterLogout.Invoke((object)this, new OnlineUIServicesEventArgs(e.GuidObject));
            }
            for (int num = _autoSwitches.Count - 1; num >= 0; num--)
            {
                OnlineStateAutoSwitch onlineStateAutoSwitch = (OnlineStateAutoSwitch)_autoSwitches[num];
                Debug.Assert(onlineStateAutoSwitch != null);
                if (onlineStateAutoSwitch.NewOnlineState.OnlineApplication == e.GuidObject)
                {
                    IEditorView view = onlineStateAutoSwitch.View;
                    IEditorView obj = ((view is IHasOnlineMode) ? view : null);
                    Debug.Assert(obj != null);
                    SetOnlineState((IHasOnlineMode)(object)obj, onlineStateAutoSwitch.OldOnlineState);
                    _autoSwitches.RemoveAt(num);
                }
            }
            if (((IEngine)APEnvironment.Engine).Frame != null)
            {
                IEditorView[] editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews();
                Debug.Assert(editorViews != null);
                OnlineViewInfoTable onlineViewInfoTable = OptionsHelper.OnlineViewInfoTable;
                Debug.Assert(onlineViewInfoTable != null);
                onlineViewInfoTable.ClearOnlineViews(e.GuidObject);
                List<IEditorView> list = new List<IEditorView>();
                IEditorView[] array = editorViews;
                foreach (IEditorView val in array)
                {
                    if (!(val is IHasOnlineMode))
                    {
                        continue;
                    }
                    IHasOnlineMode val2 = (IHasOnlineMode)val;
                    if (!(val2.OnlineState.OnlineApplication == e.GuidObject))
                    {
                        continue;
                    }
                    if (val is IHasOnlineMode2)
                    {
                        IHasOnlineMode2 val3 = (IHasOnlineMode2)val;
                        if (val3 != null && val3.NotifyOnly())
                        {
                            OnlineState onlineState = ((IHasOnlineMode)val3).OnlineState;
                            onlineState.OnlineApplication = Guid.Empty;
                            SetOnlineState((IHasOnlineMode)(object)val3, onlineState);
                            continue;
                        }
                    }
                    Rectangle floatingRect = ((((IEngine)APEnvironment.Engine).Frame is IFrame11) ? ((IFrame11)((IEngine)APEnvironment.Engine).Frame).GetFloatingRectangle((IView)(object)val, false) : Rectangle.Empty);
                    OnlineViewInfo onlineViewInfo = new OnlineViewInfo(val.Editor.ObjectGuid, Guid.Empty, val2.OnlineState.InstancePath, floatingRect);
                    onlineViewInfoTable.AddOnlineView(e.GuidObject, onlineViewInfo);
                    if (((IEngine)APEnvironment.Engine).Frame.EditorViewInFront != val)
                    {
                        ((IEngine)APEnvironment.Engine).Frame.CloseView((IView)(object)val);
                        list.Add(val);
                        continue;
                    }
                    OnlineState state = default(OnlineState);
                    state.OnlineApplication = Guid.Empty;
                    state.InstancePath = null;
                    SetOnlineState(val2, state);
                }
                OptionsHelper.OnlineViewInfoTable = onlineViewInfoTable;
                editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews();
                Debug.Assert(editorViews != null);
                for (int num2 = editorViews.Length - 1; num2 >= 0; num2--)
                {
                    if (list.Contains(editorViews[num2]))
                    {
                        continue;
                    }
                    if (editorViews[num2] is IHasOnlineMode)
                    {
                        IHasOnlineMode val4 = (IHasOnlineMode)editorViews[num2];
                        if (val4.OnlineState.OnlineApplication != Guid.Empty && val4.OnlineState.OnlineApplication != e.GuidObject)
                        {
                            continue;
                        }
                    }
                    if (editorViews[num2] == ((IEngine)APEnvironment.Engine).Frame.EditorViewInFront)
                    {
                        continue;
                    }
                    for (int num3 = editorViews.Length - 1; num3 >= 0; num3--)
                    {
                        if (num2 != num3 && editorViews[num2].Editor.ProjectHandle == editorViews[num3].Editor.ProjectHandle && editorViews[num2].Editor.ObjectGuid == editorViews[num3].Editor.ObjectGuid && !list.Contains(editorViews[num3]))
                        {
                            ((IEngine)APEnvironment.Engine).Frame.CloseView((IView)(object)editorViews[num2]);
                            list.Add(editorViews[num2]);
                            break;
                        }
                    }
                }
            }
            string onlinePerspectiveName = OnlinePerspectiveName;
            string onlineBackupPerspectiveName = OnlineBackupPerspectiveName;
            if (_applicationsInOnlinePerspective.Count == 1 && _applicationsInOnlinePerspective.Contains(e.GuidObject) && ((IEngine)APEnvironment.Engine).Frame is IFrame9 && onlinePerspectiveName != null && onlineBackupPerspectiveName != null)
            {
                IPerspectiveManager perspectiveMgr = ((IFrame9)((IEngine)APEnvironment.Engine).Frame).PerspectiveMgr;
                IPerspective val5 = perspectiveMgr.GetPerspective(onlinePerspectiveName);
                if (val5 == null)
                {
                    val5 = perspectiveMgr.CreatePerspective(onlinePerspectiveName);
                }
                val5.Update();
                IPerspective perspective = perspectiveMgr.GetPerspective(onlineBackupPerspectiveName);
                if (perspective != null)
                {
                    perspective.Activate(false);
                }
            }
            _applicationsInOnlinePerspective.Remove(e.GuidObject);
        }

        private void OnAfterApplicationDownload(object sender, OnlineEventArgs e)
        {
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_0071: Unknown result type (might be due to invalid IL or missing references)
            //IL_0078: Expected O, but got Unknown
            //IL_007a: Unknown result type (might be due to invalid IL or missing references)
            //IL_008b: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f0: Unknown result type (might be due to invalid IL or missing references)
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if ((e is OnlineDownloadEventArgs2 && ((OnlineDownloadEventArgs2)e).DownloadException != null) || ((IEngine)APEnvironment.Engine).Frame == null)
            {
                return;
            }
            IEditorView[] editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews();
            if (editorViews == null)
            {
                return;
            }
            OnlineViewInfoTable onlineViewInfoTable = OptionsHelper.OnlineViewInfoTable;
            if (onlineViewInfoTable == null)
            {
                return;
            }
            bool flag = false;
            IEditorView[] array = editorViews;
            foreach (IEditorView val in array)
            {
                if (!(val is IHasOnlineMode))
                {
                    continue;
                }
                IHasOnlineMode val2 = (IHasOnlineMode)val;
                if (val2.OnlineState.InstancePath != null && val2.OnlineState.OnlineApplication == e.GuidObject)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val.Editor.ProjectHandle, val.Editor.ObjectGuid);
                    if (metaObjectStub != null && !OnlineCommandHelper.HasInstancePath(metaObjectStub, e.GuidObject))
                    {
                        flag = true;
                        onlineViewInfoTable.ClearOnlineView(e.GuidObject, val.Editor.ObjectGuid, val2.OnlineState.InstancePath);
                        ((IEngine)APEnvironment.Engine).Frame.CloseView((IView)(object)val);
                    }
                }
            }
            if (flag)
            {
                OptionsHelper.OnlineViewInfoTable = onlineViewInfoTable;
            }
        }

        private void OnFlowControlEnabledChanged(object sender, OnlineEventArgs e)
        {
            //IL_006c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0073: Expected O, but got Unknown
            //IL_0075: Unknown result type (might be due to invalid IL or missing references)
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (((IEngine)APEnvironment.Engine).Frame == null)
            {
                return;
            }
            IEditorView[] editorViews = ((IEngine)APEnvironment.Engine).Frame.GetEditorViews();
            if (editorViews == null)
            {
                return;
            }
            IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.GuidObject);
            IOnlineApplication12 val = (IOnlineApplication12)(object)((application is IOnlineApplication12) ? application : null);
            bool flowControlEnabled = val != null && val.FlowControlEnabled;
            IEditorView[] array = editorViews;
            foreach (IEditorView val2 in array)
            {
                if (val2 is IHasOnlineMode3)
                {
                    IHasOnlineMode3 val3 = (IHasOnlineMode3)val2;
                    if (((IHasOnlineMode)val3).OnlineState.OnlineApplication == e.GuidObject)
                    {
                        val3.FlowControlEnabled = (flowControlEnabled);
                    }
                }
            }
        }

        private void OnObjectRemoved(object sender, ObjectRemovedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e.MetaObject == null)
            {
                throw new ArgumentException("e.MetaObject is null.");
            }
            OnlineViewInfoTable onlineViewInfoTable = OptionsHelper.OnlineViewInfoTable;
            Debug.Assert(onlineViewInfoTable != null);
            onlineViewInfoTable.ClearOnlineViews(e.MetaObject.ObjectGuid);
            OptionsHelper.OnlineViewInfoTable = onlineViewInfoTable;
        }

        private void OnObjectRemoving(object sender, ObjectCancelEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
            Debug.Assert(metaObjectStub != null, "Meta Object is null");
            if (OnlineCommandHelper.IsUsedOnline(metaObjectStub) && !AllowOnlineModification)
            {
                string message = string.Format(Strings.CannotRemoveOnlineObject, ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(e.ProjectHandle, e.ObjectGuid));
                e.Cancel((Exception)new InvalidOperationException(message));
            }
        }

        private void OnObjectRenaming(object sender, ObjectRenamingEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
            Debug.Assert(metaObjectStub != null, "Meta Object is null");
            if (OnlineCommandHelper.IsUsedOnline(metaObjectStub) && !AllowOnlineModification)
            {
                string message = string.Format(Strings.CannotRenameOnlineObject, ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(e.ProjectHandle, e.ObjectGuid));
                e.Cancel((Exception)new InvalidOperationException(message));
            }
        }

        private void OnObjectAdding(object sender, ObjectAddingEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e.ParentObjectGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(e.ProjectHandle, e.ParentObjectGuid))
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ParentObjectGuid);
                Debug.Assert(metaObjectStub != null);
                if (OnlineCommandHelper.IsUsedOnline(metaObjectStub) && !AllowOnlineModification)
                {
                    e.Cancel((Exception)new InvalidOperationException(Strings.CannotAddOnlineObject));
                }
            }
        }

        private static void SetOnlineState(IHasOnlineMode hom, OnlineState state)
        {
            //IL_0004: Unknown result type (might be due to invalid IL or missing references)
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Unknown result type (might be due to invalid IL or missing references)
            if (hom != null)
            {
                hom.OnlineState = (state);
            }
            IHasOnlineMode3 val = (IHasOnlineMode3)(object)((hom is IHasOnlineMode3) ? hom : null);
            if (val != null)
            {
                if (state.OnlineApplication != Guid.Empty)
                {
                    IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(state.OnlineApplication);
                    IOnlineApplication12 val2 = (IOnlineApplication12)(object)((application is IOnlineApplication12) ? application : null);
                    val.FlowControlEnabled = (val2 != null && val2.FlowControlEnabled);
                }
                else
                {
                    val.FlowControlEnabled = (false);
                }
            }
        }

        public bool IsUsedOnline(IMetaObjectStub mos)
        {
            return OnlineCommandHelper.IsUsedOnline(mos);
        }

        public void BeginAllowOnlineModification()
        {
            _allowOnlineModificationCounter++;
        }

        public void EndAllowOnlineModification()
        {
            if (_allowOnlineModificationCounter > 0)
            {
                _allowOnlineModificationCounter--;
            }
        }
    }
}
