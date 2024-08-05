#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.BrowserCommands;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.ToolBox;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{c316ab8f-76a5-4e9e-9c41-3304a6f77a99}")]
	public class DeviceEditor : UserControl, IEditorView, IView, IEditor, IDeviceEditorFrame2, IDeviceEditorFrame, IParameterSetEditorFrame, IConnectorEditorFrame2, IConnectorEditorFrame, IHasOnlineMode2, IHasOnlineMode, IParameterSetEditorFrame3, IParameterSetEditorFrame2, IEditorFrameBase, IPrintableEx, IEditorTabManager, INotifyOnVisibilityChanged, IHasToolBoxItems2, IHasToolBoxItems, IHasAssociatedOnlineHelpTopic2, IHasAssociatedOnlineHelpTopic, IEditorBasedFindReplace, IBrowserCommandsTarget, ILocalizableEditor
	{
		private enum ReloadReason
		{
			Open = 0,
			ObjectAdded = 1,
			ObjectModified = 2,
			ObjectRemoved = 4,
			Other = 8
		}

		private enum DoOnlineState
		{
			Offline,
			Online,
			StoreStates,
			ClearStates
		}

		private int _nProjectHandle = -1;

		private Guid _guidObject;

		private IMetaObject _metaObject;

		private OnlineState _onlineState;

		private EditorList _editorList = new EditorList();

		private bool _bDevdescValid = true;

		private AssociatedDeviceEditor _associatedDeviceEditor;

		private IUndoManager _editorUndoMgr = APEnvironment.CreateUndoMgr();

		private bool _bLocalizationActive;

		private Container components;

		private Panel _pnlVerticalTabControl;

		private TabControl _tabControl;

		private ReloadReason _reason;

		private static string s_stLastSelectedPageName = string.Empty;

		private bool _bOpenView;

		private bool _bUndoActive;

		private bool _bReloadAfterCompound;

		private IUndoManager2 _undoMgr;

		private WeakMulticastDelegate _ehTaskConfigChanged;

		private bool _bInReload;

		private bool _bFirstReloadDone;

		private Label _missingLabel;

		private Dictionary<TabPage, IVisibleEditor> visibleComponents = new Dictionary<TabPage, IVisibleEditor>();

		private bool _bVisible = true;

		private bool _bInSave;

		private bool _bUpdateComponentVisibilityInSave;

		private string _stCaption = string.Empty;

		private Timer _retryTimer = new Timer();

		private Hashtable _htControlState = new Hashtable();

		private WeakMulticastDelegate _ehParameterChanged;

		private WeakMulticastDelegate _ehParameterSectionChanged;

		private WeakMulticastDelegate _ehParameterAdded;

		private WeakMulticastDelegate _ehParameterRemoved;

		private WeakMulticastDelegate _ehParameterMoved;

		internal IUndoManager EditorUndoMgr => _editorUndoMgr;

		internal bool OpenView => _bOpenView;

		public int ProjectHandle => _nProjectHandle;

		public Guid ObjectGuid => _guidObject;

		private bool NeedsSave
		{
			get
			{
				if (_metaObject != null)
				{
					return _metaObject.IsToModify;
				}
				return false;
			}
		}

		public IEditor Editor => (IEditor)(object)this;

		public Control Control => this;

		public Control[] Panes => null;

		public string Caption
		{
			get
			{
				string text = string.Empty;
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _guidObject))
				{
					IPSNode val = APEnvironment.ObjectMgr.GetProjectStructure(_nProjectHandle).FindNode(_guidObject, true);
					if (val != null)
					{
						text = val.GetName(true);
					}
				}
				if (APEnvironment.LocalizationManagerOrNull != null && _bLocalizationActive)
				{
					text = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text, (LocalizationContent)1);
				}
				if (!string.IsNullOrEmpty(_stCaption))
				{
					text = _stCaption;
				}
				return text;
			}
			set
			{
				_stCaption = value;
			}
		}

		public string Description
		{
			get
			{
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _guidObject))
				{
					return ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(_nProjectHandle, _guidObject);
				}
				return string.Empty;
			}
		}

		public Icon SmallIcon
		{
			get
			{
				if (_metaObject != null)
				{
					Icon icon = DeviceEditorFactory.GetSmallIcon(_metaObject);
					if (icon.Width != 16 || icon.Height != 16)
					{
						icon = GraphicsHelper.ScaleIcon(icon, 16);
					}
					if (icon.Height == 16 && icon.Width == 16)
					{
						return icon;
					}
				}
				return DeviceEditorFactory.GetSmallIcon();
			}
		}

		public Icon LargeIcon => DeviceEditorFactory.GetLargeIcon();

		public DockingPosition DefaultDockingPosition => (DockingPosition)32;

		public DockingPosition PossibleDockingPositions => (DockingPosition)32;

		public OnlineState OnlineState
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _onlineState;
			}
			set
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				bool flag = false;
				if (value.OnlineApplication != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _guidObject))
				{
					IMetaObjectStub plcDevice = APEnvironment.DeviceMgr.GetPlcDevice(_nProjectHandle, _guidObject);
					if (plcDevice != null && DeviceHelper.GetApplications(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(plcDevice.ProjectHandle, plcDevice.ObjectGuid), bWithHidden: true).Contains(value.OnlineApplication))
					{
						flag = true;
					}
				}
				bool num = _onlineState.OnlineApplication != value.OnlineApplication;
				if (flag || value.OnlineApplication == Guid.Empty)
				{
					_onlineState = value;
					if (this.OnlineStateChanged != null)
					{
						this.OnlineStateChanged(this, EventArgs.Empty);
					}
				}
				if (num)
				{
					DoOnlineStatePages((_onlineState.OnlineApplication != Guid.Empty) ? DoOnlineState.Online : DoOnlineState.Offline);
				}
			}
		}

		public IToolBoxItem[] ToolBoxItems
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				foreach (EditorInfo editor in _editorList)
				{
					if (editor == null || editor.Pages == null)
					{
						continue;
					}
					IEditorPage[] pages = editor.Pages;
					foreach (IEditorPage val in pages)
					{
						if (typeof(IHasToolBoxItems).IsAssignableFrom(((object)val).GetType()))
						{
							arrayList.AddRange(((IHasToolBoxItems)((val is IHasToolBoxItems) ? val : null)).ToolBoxItems);
						}
					}
				}
				IToolBoxItem[] array = (IToolBoxItem[])(object)new IToolBoxItem[arrayList.Count];
				arrayList.CopyTo(array);
				return array;
			}
		}

		public bool HasToolBoxItems
		{
			get
			{
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				foreach (EditorInfo editor in _editorList)
				{
					if (editor == null || editor.Pages == null)
					{
						continue;
					}
					IEditorPage[] pages = editor.Pages;
					foreach (IEditorPage val in pages)
					{
						if (val is IHasToolBoxItems2 && ((IHasToolBoxItems2)val).HasToolBoxItems)
						{
							return true;
						}
						if (val is IHasToolBoxItems)
						{
							IToolBoxItem[] toolBoxItems = ((IHasToolBoxItems)val).ToolBoxItems;
							if (toolBoxItems != null && toolBoxItems.Length != 0)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
		}

		public List<IBaseDeviceEditor> BaseDeviceEditors
		{
			get
			{
				List<IBaseDeviceEditor> list = new List<IBaseDeviceEditor>();
				foreach (EditorInfo editor in _editorList)
				{
					list.Add(editor.Editor);
				}
				return list;
			}
		}

		public event ParameterChangedEventHandler ParameterChanged
		{
			add
			{
				_ehParameterChanged = WeakMulticastDelegate.CombineUnique(_ehParameterChanged, (Delegate)(object)value);
			}
			remove
			{
				_ehParameterChanged = WeakMulticastDelegate.Remove(_ehParameterChanged, (Delegate)(object)value);
			}
		}

		public event ParameterEventHandler ParameterAdded
		{
			add
			{
				_ehParameterAdded = WeakMulticastDelegate.CombineUnique(_ehParameterAdded, (Delegate)(object)value);
			}
			remove
			{
				_ehParameterAdded = WeakMulticastDelegate.Remove(_ehParameterAdded, (Delegate)(object)value);
			}
		}

		public event ParameterEventHandler ParameterRemoved
		{
			add
			{
				_ehParameterRemoved = WeakMulticastDelegate.CombineUnique(_ehParameterRemoved, (Delegate)(object)value);
			}
			remove
			{
				_ehParameterRemoved = WeakMulticastDelegate.Remove(_ehParameterRemoved, (Delegate)(object)value);
			}
		}

		public event ParameterSectionChangedEventHandler ParameterSectionChanged
		{
			add
			{
				_ehParameterSectionChanged = WeakMulticastDelegate.CombineUnique(_ehParameterSectionChanged, (Delegate)(object)value);
			}
			remove
			{
				_ehParameterSectionChanged = WeakMulticastDelegate.Remove(_ehParameterSectionChanged, (Delegate)(object)value);
			}
		}

		public event ParameterMovedEventHandler ParameterMoved
		{
			add
			{
				_ehParameterMoved = WeakMulticastDelegate.CombineUnique(_ehParameterMoved, (Delegate)(object)value);
			}
			remove
			{
				_ehParameterMoved = WeakMulticastDelegate.Remove(_ehParameterMoved, (Delegate)(object)value);
			}
		}

		public event EventHandler TaskConfigChanged
		{
			add
			{
				_ehTaskConfigChanged = WeakMulticastDelegate.CombineUnique(_ehTaskConfigChanged, (Delegate)value);
			}
			remove
			{
				_ehTaskConfigChanged = WeakMulticastDelegate.Remove(_ehTaskConfigChanged, (Delegate)value);
			}
		}

		public event EventHandler OnlineStateChanged;

		public DeviceEditor()
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Expected O, but got Unknown
			InitializeComponent();
			_tabControl.ImageList = new ImageList();
			if (((IEngine)APEnvironment.Engine).Frame != null)
			{
				((IEngine)APEnvironment.Engine).Frame.ViewOpening+=(new ViewCancelEventHandler(OnViewOpening));
			}
		}

		private void OnViewOpening(object sender, ViewCancelEventArgs e)
		{
			if (e.View != null && e.View is IEditorView)
			{
				_bOpenView = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			if (disposing)
			{
				if (_tabControl?.SelectedTab != null)
				{
					s_stLastSelectedPageName = _tabControl.SelectedTab.Text;
				}
				if (((IEngine)APEnvironment.Engine).Frame != null)
				{
					((IEngine)APEnvironment.Engine).Frame.ViewOpening-=(new ViewCancelEventHandler(OnViewOpening));
				}
				if (components != null)
				{
					components.Dispose();
				}
				if (_tabControl.ImageList != null)
				{
					_tabControl.ImageList.Images.Clear();
					_tabControl.ImageList.Dispose();
				}
				if (_associatedDeviceEditor != null)
				{
					((IEngine)APEnvironment.Engine).EditorManager.CloseEditor((IEditor)(object)_associatedDeviceEditor);
				}
				foreach (EditorInfo editor in _editorList)
				{
					if (editor.Pages == null)
					{
						continue;
					}
					IEditorPage[] pages = editor.Pages;
					foreach (IEditorPage val in pages)
					{
						if (val is IDisposable)
						{
							((IDisposable)val).Dispose();
						}
						else
						{
							val.Control.Dispose();
						}
					}
				}
				ClearEditorList(bForce: true);
			}
			if (_metaObject != null && _metaObject.IsToModify)
			{
				Save(bCommit: true);
			}
			base.Dispose(disposing);
		}

		protected override void OnLoad(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			base.OnLoad(e);
			APEnvironment.OptionStorage.OptionChanged+=(new OptionEventHandler(OnOptionChanged));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded+=(new ObjectEventHandler(OnObjectMgrObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved+=(new ObjectRemovedEventHandler(OnObjectMgrObjectRemoved));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectModified+=(new ObjectModifiedEventHandler(OnObjectMgrObjectModified));
			((IOnlineManager)APEnvironment.OnlineMgr).BeforeApplicationLogin+=(new BeforeApplicationLoginEventHandler(OnlineMgr_BeforeApplicationLogin));
			APEnvironment.OnlineMgr.AfterApplicationDownload2+=(new AfterApplicationDownload2EventHandler(DeviceEditor_AfterApplicationDownload2));
		}

		private IMetaObjectStub GetHostStub(int nProjectHandle, Guid objectGuid)
		{
			IMetaObjectStub val = null;
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, objectGuid))
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
				if (val != null)
				{
					while (val != null && val.ParentObjectGuid != Guid.Empty)
					{
						val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, val.ParentObjectGuid);
					}
				}
			}
			return val;
		}

		public void DeviceEditor_AfterApplicationDownload2(object sender, OnlineEventArgs e)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			IMetaObjectStub hostStub = GetHostStub(_nProjectHandle, e.GuidObject);
			IMetaObjectStub hostStub2 = GetHostStub(_nProjectHandle, _guidObject);
			if (hostStub != null && hostStub2 != null && hostStub.ObjectGuid == hostStub2.ObjectGuid)
			{
				_onlineState = default(OnlineState);
				_onlineState.OnlineApplication = e.GuidObject;
				if (this.OnlineStateChanged != null)
				{
					this.OnlineStateChanged(this, EventArgs.Empty);
				}
				DoOnlineStatePages(DoOnlineState.Online);
			}
		}

		public void OnlineMgr_BeforeApplicationLogin(object sender, OnlineCancelEventArgs e)
		{
			IMetaObjectStub hostStub = GetHostStub(_nProjectHandle, e.ObjectGuid);
			IMetaObjectStub hostStub2 = GetHostStub(_nProjectHandle, _guidObject);
			if (hostStub != null && hostStub2 != null && hostStub.ObjectGuid == hostStub2.ObjectGuid)
			{
				IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(e.ObjectGuid);
				if (application == null || !application.IsLoggedIn)
				{
					DoOnlineStatePages(DoOnlineState.ClearStates);
					DoOnlineStatePages(DoOnlineState.StoreStates);
				}
			}
		}

		private void RaiseAddressesChanged(int nConnectorId, IParameter param, IDataElement elem, string[] path)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			OnParameterChanged(new ParameterChangedEventArgs(nConnectorId, param, elem, path));
			if (!elem.HasSubElements)
			{
				return;
			}
			string[] array = new string[path.Length + 1];
			path.CopyTo(array, 0);
			foreach (IDataElement item in (IEnumerable)elem.SubElements)
			{
				IDataElement val = item;
				array[array.Length - 1] = val.Identifier;
				RaiseAddressesChanged(nConnectorId, param, val, array);
			}
		}

		private void RaiseAddressesChanged(int nConnectorId, IParameterSet parameterset)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			foreach (IParameter item in (IEnumerable)parameterset)
			{
				IParameter val = item;
				if ((int)val.ChannelType != 0)
				{
					RaiseAddressesChanged(nConnectorId, val, (IDataElement)(object)val, new string[0]);
				}
			}
		}

		private void EndActions()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			if (!_bUndoActive)
			{
				return;
			}
			if (_metaObject != null && !_metaObject.IsToModify && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid))
			{
				_metaObject = null;
			}
			IMetaObject objectToRead = GetObjectToRead();
			if (objectToRead.Object is IDeviceObject)
			{
				IDeviceObject val = (IDeviceObject)objectToRead.Object;
				if (((ICollection)val.DeviceParameterSet).Count > 0)
				{
					RaiseAddressesChanged(-1, val.DeviceParameterSet);
				}
				foreach (IConnector item in (IEnumerable)val.Connectors)
				{
					IConnector val2 = item;
					RaiseAddressesChanged(val2.ConnectorId, val2.HostParameterSet);
				}
			}
			else if (objectToRead.Object is IExplicitConnector)
			{
				IExplicitConnector val3 = (IExplicitConnector)objectToRead.Object;
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid))
				{
					RaiseAddressesChanged(((IConnector)val3).ConnectorId, ((IConnector)val3).HostParameterSet);
				}
			}
			if (_undoMgr is IUndoManager4)
			{
				IUndoManager2 undoMgr = _undoMgr;
				((IUndoManager4)((undoMgr is IUndoManager4) ? undoMgr : null)).AfterEndCompoundAction2-=((EventHandler)_undoMgr_AfterEndCompoundAction);
			}
			else
			{
				_undoMgr.AfterEndCompoundAction-=((EventHandler)_undoMgr_AfterEndCompoundAction);
			}
			_bUndoActive = false;
			if (_bReloadAfterCompound)
			{
				_bReloadAfterCompound = false;
				if (((_tabControl != null && !_tabControl.IsDisposed) || (base.Controls.Count > 0 && base.Controls[0] is IEditorPage)) && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _guidObject))
				{
					Reload();
				}
			}
		}

		private void _undoMgr_AfterEndCompoundAction(object sender, EventArgs e)
		{
			EndActions();
		}

		private void OnObjectChanged(Type objectType, ReloadReason reason)
		{
			try
			{
				if (typeof(IDeviceObject).IsAssignableFrom(objectType) || typeof(IExplicitConnector).IsAssignableFrom(objectType))
				{
					_reason |= reason;
					if (!_bUndoActive)
					{
						_bUndoActive = true;
						IMetaObject objectToRead = GetObjectToRead();
						ref IUndoManager2 undoMgr = ref _undoMgr;
						IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(objectToRead.ProjectHandle);
						undoMgr = (IUndoManager2)(object)((undoManager is IUndoManager2) ? undoManager : null);
						if (((IUndoManager)_undoMgr).InCompoundAction)
						{
							if (_undoMgr is IUndoManager4)
							{
								IUndoManager2 undoMgr2 = _undoMgr;
								((IUndoManager4)((undoMgr2 is IUndoManager4) ? undoMgr2 : null)).AfterEndCompoundAction2+=((EventHandler)_undoMgr_AfterEndCompoundAction);
							}
							else
							{
								_undoMgr.AfterEndCompoundAction+=((EventHandler)_undoMgr_AfterEndCompoundAction);
							}
						}
						else
						{
							EndActions();
						}
					}
				}
				if (typeof(ITaskObject).IsAssignableFrom(objectType))
				{
					OnTaskConfigChanged(new EventArgs());
				}
			}
			catch
			{
			}
		}

		private void OnObjectMgrObjectModified(object sender, ObjectModifiedEventArgs e)
		{
			try
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
				OnObjectChanged(metaObjectStub.ObjectType, ReloadReason.ObjectModified);
			}
			catch
			{
			}
		}

		private void OnObjectMgrObjectAdded(object sender, ObjectEventArgs e)
		{
			try
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
				OnObjectChanged(metaObjectStub.ObjectType, ReloadReason.ObjectAdded);
			}
			catch
			{
			}
		}

		private void OnObjectMgrObjectRemoved(object sender, ObjectRemovedEventArgs e)
		{
			try
			{
				OnObjectChanged(((object)e.MetaObject.Object).GetType(), ReloadReason.ObjectRemoved);
			}
			catch
			{
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			base.OnHandleDestroyed(e);
			APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded-=(new ObjectEventHandler(OnObjectMgrObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved-=(new ObjectRemovedEventHandler(OnObjectMgrObjectRemoved));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectModified-=(new ObjectModifiedEventHandler(OnObjectMgrObjectModified));
			((IOnlineManager)APEnvironment.OnlineMgr).BeforeApplicationLogin-=(new BeforeApplicationLoginEventHandler(OnlineMgr_BeforeApplicationLogin));
			APEnvironment.OnlineMgr.AfterApplicationDownload2-=(new AfterApplicationDownload2EventHandler(DeviceEditor_AfterApplicationDownload2));
		}

		private void OnOptionChanged(object sender, OptionEventArgs e)
		{
			if (e.OptionKey != null)
			{
				_ = e.OptionKey.Name == OptionsHelper.SUB_KEY;
			}
		}

		private void OnTabChanged(object sender, EventArgs e)
		{
			try
			{
				if (!_bInReload && sender == _tabControl)
				{
					Save(bCommit: true);
					UpdateComponentVisibility();
					DoOnlineStatePages((_onlineState.OnlineApplication != Guid.Empty) ? DoOnlineState.Online : DoOnlineState.Offline);
				}
			}
			catch
			{
			}
		}

		private void InitializeComponent()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.DeviceEditor));
			_pnlVerticalTabControl = new System.Windows.Forms.Panel();
			_tabControl = (System.Windows.Forms.TabControl)new VerticalTabControl();
			_pnlVerticalTabControl.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_pnlVerticalTabControl, "_pnlVerticalTabControl");
			_pnlVerticalTabControl.Controls.Add(_tabControl);
			_pnlVerticalTabControl.Name = "_pnlVerticalTabControl";
			resources.ApplyResources(_tabControl, "_tabControl");
			_tabControl.AllowDrop = true;
			_tabControl.Name = "_tabControl";
			_tabControl.SelectedIndex = 0;
			_tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			_tabControl.SelectedIndexChanged += new System.EventHandler(OnTabChanged);
			_tabControl.DragOver += new System.Windows.Forms.DragEventHandler(_tabControl_DragOver);
			resources.ApplyResources(this, "$this");
			BackColor = System.Drawing.Color.White;
			base.Controls.Add(_pnlVerticalTabControl);
			base.Name = "DeviceEditor";
			_pnlVerticalTabControl.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		public override bool Equals(object obj)
		{
			DeviceEditor deviceEditor = obj as DeviceEditor;
			if (deviceEditor != null)
			{
				if (deviceEditor._nProjectHandle == _nProjectHandle)
				{
					return _guidObject.Equals(deviceEditor._guidObject);
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _nProjectHandle.GetHashCode() ^ _guidObject.GetHashCode();
		}

		public IDeviceObject GetDeviceObject(bool bToModify)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			if (_metaObject.Object is IDeviceObject)
			{
				IMetaObject val = ((!bToModify) ? GetObjectToRead() : GetObjectToModify());
				if (val == null)
				{
					return null;
				}
				return (IDeviceObject)val.Object;
			}
			if (_metaObject.Object is IExplicitConnector)
			{
				if (bToModify)
				{
					if (!base.IsHandleCreated)
					{
						return null;
					}
					if (_associatedDeviceEditor == null)
					{
						IDeviceObject deviceObject = ((IConnector)(IExplicitConnector)_metaObject.Object).GetDeviceObject();
						_associatedDeviceEditor = new AssociatedDeviceEditor();
						_associatedDeviceEditor.SetObject(((IObject)deviceObject).MetaObject.ProjectHandle, ((IObject)deviceObject).MetaObject.ObjectGuid);
						_associatedDeviceEditor.ReloadEvent += OnAssociatedDeviceEditorReload;
						_associatedDeviceEditor.SaveEvent += OnAssociatedDeviceEditorSave;
						((IEngine)APEnvironment.Engine).EditorManager.AddEditor((IEditor)(object)_associatedDeviceEditor);
					}
					return _associatedDeviceEditor.GetDeviceObject(bToModify: true);
				}
				if (_associatedDeviceEditor == null)
				{
					return ((IConnector)(IExplicitConnector)_metaObject.Object).GetDeviceObject();
				}
				return _associatedDeviceEditor.GetDeviceObject(bToModify: false);
			}
			return null;
		}

		private void OnAssociatedDeviceEditorSave(object sender, bool bCommit)
		{
			Save(bCommit);
		}

		private void OnAssociatedDeviceEditorReload(object sender, EventArgs e)
		{
			Reload();
		}

		protected void ParameterSet_ParameterAdded(object sender, ParameterEventArgs e)
		{
			OnParameterAdded(e);
		}

		protected void ParameterSet_ParameterRemoved(object sender, ParameterEventArgs e)
		{
			OnParameterRemoved(e);
		}

		protected void ParameterSet_ParameterChanged(object sender, ParameterChangedEventArgs e)
		{
			OnParameterChanged(e);
		}

		protected void ParameterSet_ParameterSectionChanged(object sender, ParameterSectionChangedEventArgs e)
		{
			OnParameterSectionChanged(e);
		}

		protected void ParameterSet_ParameterMoved(object sender, ParameterMovedEventArgs e)
		{
			OnParameterMoved(e);
		}

		protected virtual void OnParameterAdded(ParameterEventArgs e)
		{
			if (_ehParameterAdded != null)
			{
				_ehParameterAdded.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterRemoved(ParameterEventArgs e)
		{
			if (_ehParameterRemoved != null)
			{
				_ehParameterRemoved.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterChanged(ParameterChangedEventArgs e)
		{
			if (_ehParameterChanged != null)
			{
				_ehParameterChanged.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterSectionChanged(ParameterSectionChangedEventArgs e)
		{
			if (_ehParameterSectionChanged != null)
			{
				_ehParameterSectionChanged.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterMoved(ParameterMovedEventArgs e)
		{
			if (_ehParameterMoved != null)
			{
				_ehParameterMoved.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnTaskConfigChanged(EventArgs e)
		{
			if (_ehTaskConfigChanged != null)
			{
				_ehTaskConfigChanged.Invoke(new object[2] { this, e });
			}
		}

		IDeviceObject IConnectorEditorFrame.GetAssociatedDeviceObject(bool bToModify)
		{
			return GetDeviceObject(bToModify);
		}

		IConnector IConnectorEditorFrame.GetConnector(int nConnectorId, bool bToModify)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			if (_metaObject.Object is IDeviceObject)
			{
				IDeviceObject deviceObject = GetDeviceObject(bToModify);
				if (deviceObject == null)
				{
					return null;
				}
				foreach (IConnector item in (IEnumerable)deviceObject.Connectors)
				{
					IConnector val = item;
					if (val.ConnectorId == nConnectorId)
					{
						return val;
					}
				}
			}
			else if (_metaObject.Object is IExplicitConnector)
			{
				IMetaObject val2 = ((!bToModify) ? GetObjectToRead() : GetObjectToModify());
				if (val2 == null)
				{
					return null;
				}
				return (IConnector)(IExplicitConnector)val2.Object;
			}
			return null;
		}

		public void SetObject(int nProjectHandle, Guid objectGuid)
		{
			_metaObject = null;
			_nProjectHandle = nProjectHandle;
			_guidObject = objectGuid;
		}

		private void CreateCommunicationTabs(IDeviceObject device, EditorList editorList)
		{
			if (!device.AllowsDirectCommunication)
			{
				return;
			}
			IEnumerable<FactoryListElement<IDeviceCommunicationEditorFactory>> factories = DeviceCommunicationEditorFactoryManager.Instance.GetFactories(device);
			bool flag = false;
			int num = -1;
			foreach (FactoryListElement<IDeviceCommunicationEditorFactory> item in factories)
			{
				if (item != null)
				{
					if (item.Match <= -1 || (flag && num > item.Match))
					{
						break;
					}
					if (item.Factory != null)
					{
						ICommunicationEditor val = item.Factory.Create(device);
						val.DeviceEditorFrame=((IDeviceEditorFrame)(object)this);
						editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)val));
						flag = ((IBaseDeviceEditor)val).HideGenericEditor;
					}
					num = item.Match;
				}
			}
		}

		private void CreateDeviceConfigTabs(IDeviceObject device, StatusPage statusPage, EditorList editorList)
		{
			IEnumerable<FactoryListElement<IDeviceEditorFactory>> factories = DeviceEditorFactoryManager.Instance.GetFactories(device);
			bool showGenericConfiguration = OptionsHelper.ShowGenericConfiguration;
			bool flag = false;
			int num = -1;
			CombinedParameterFilter combinedParameterFilter = new CombinedParameterFilter();
			CombinedParameterFilter combinedParameterFilter2 = new CombinedParameterFilter();
			foreach (FactoryListElement<IDeviceEditorFactory> item in factories)
			{
				if (item == null)
				{
					continue;
				}
				if (item.Match <= -1 || (flag && num > item.Match))
				{
					break;
				}
				if (num > item.Match)
				{
					combinedParameterFilter2 = combinedParameterFilter;
					combinedParameterFilter = new CombinedParameterFilter();
					combinedParameterFilter.AddFilter(combinedParameterFilter2.GetParameterFilter());
				}
				if (item.Factory != null)
				{
					try
					{
						IDeviceEditor val = item.Factory.Create(device, combinedParameterFilter2.GetParameterFilter());
						val.DeviceEditorFrame=((IDeviceEditorFrame)(object)this);
						editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)val));
						combinedParameterFilter.AddFilter(val.GetParameterFilter());
						flag = ((IBaseDeviceEditor)val).HideGenericEditor && !showGenericConfiguration;
					}
					catch
					{
					}
				}
				num = item.Match;
			}
			if (((ICollection)device.DeviceParameterSet).Count > 0 || ((ICollection)((IDeviceObject2)((device is IDeviceObject2) ? device : null)).DriverInfo.RequiredLibs).Count > 0)
			{
				DeviceIoMappingEditor deviceIoMappingEditor = new DeviceIoMappingEditor();
				deviceIoMappingEditor.DeviceEditorFrame = (IDeviceEditorFrame)(object)this;
				editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)deviceIoMappingEditor));
			}
			bool result = false;
			string value = device.Attributes["UseParentPLC"];
			if (!string.IsNullOrEmpty(value))
			{
				bool.TryParse(value, out result);
			}
			if (result)
			{
				return;
			}
			Guid[] subObjectGuids = ((IObject)device).MetaObject.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)device).MetaObject.ProjectHandle, guid);
				if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					if (_bOpenView)
					{
						statusPage.SetDevice(device);
					}
					break;
				}
			}
		}

		private void CreateAllConnectorsConfigTabs(IDeviceObject device, StatusPage statusPage, Guid guidObject, EditorList editorList)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			Guid parentObjectGuid = ((IObject)device).MetaObject.ParentObjectGuid;
			bool bShowOnlyAdditionalControl = false;
			bool bShowOnlyDiagConnector = false;
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val = item;
				if (val.IsExplicit || ((int)val.ConnectorRole == 1 && !IsConnectorToParent(val, parentObjectGuid)))
				{
					continue;
				}
				if ((int)val.ConnectorRole == 0)
				{
					bool flag = true;
					foreach (IConnector item2 in (IEnumerable)device.Connectors)
					{
						IConnector val2 = item2;
						if (val2.ConnectorId == val.HostPath && !IsConnectorToParent(val2, parentObjectGuid))
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				CreateConnectorConfigTabs(device, val, statusPage, guidObject, editorList, ref bShowOnlyAdditionalControl, ref bShowOnlyDiagConnector);
			}
		}

		private void CreateConnectorConfigTabs(IDeviceObject device, IConnector connector, StatusPage statusPage, Guid guidObject, EditorList editorList, ref bool bShowOnlyAdditionalControl, ref bool bShowOnlyDiagConnector)
		{
			IEnumerable<FactoryListElement<IConnectorEditorFactory>> factories = ConnectorEditorFactoryManager.Instance.GetFactories(device, connector);
			bool showGenericConfiguration = OptionsHelper.ShowGenericConfiguration;
			bool flag = false;
			int num = -1;
			CombinedParameterFilter combinedParameterFilter = new CombinedParameterFilter();
			CombinedParameterFilter combinedParameterFilter2 = new CombinedParameterFilter();
			foreach (FactoryListElement<IConnectorEditorFactory> item in factories)
			{
				if (item == null)
				{
					continue;
				}
				if (item.Match <= -1 || (flag && num > item.Match && item.Match <= 0))
				{
					break;
				}
				if (num > item.Match)
				{
					combinedParameterFilter2 = combinedParameterFilter;
					combinedParameterFilter = new CombinedParameterFilter();
					combinedParameterFilter.AddFilter(combinedParameterFilter2.GetParameterFilter());
				}
				if (item.Factory != null)
				{
					try
					{
						IConnectorEditor val = item.Factory.Create(device, connector, combinedParameterFilter2.GetParameterFilter());
						if (val is ICommunicationEditor)
						{
							((ICommunicationEditor)((val is ICommunicationEditor) ? val : null)).DeviceEditorFrame=((IDeviceEditorFrame)(object)this);
						}
						val.ConnectorId=(connector.ConnectorId);
						val.ConnectorEditorFrame=((IConnectorEditorFrame)(object)this);
						editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)val));
						combinedParameterFilter.AddFilter(val.GetParameterFilter());
						flag = (flag | ((IBaseDeviceEditor)val).HideGenericEditor) && !showGenericConfiguration;
					}
					catch
					{
					}
				}
				num = item.Match;
			}
			ConnectorIoMappingEditor connectorIoMappingEditor = new ConnectorIoMappingEditor();
			connectorIoMappingEditor.ConnectorId = connector.ConnectorId;
			connectorIoMappingEditor.ConnectorEditorFrame = (IConnectorEditorFrame)(object)this;
			editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)connectorIoMappingEditor));
			if (_bOpenView)
			{
				statusPage.AddConnector(connector, guidObject, ref bShowOnlyAdditionalControl, ref bShowOnlyDiagConnector);
			}
		}

		private bool IsConnectorToParent(IConnector connector, Guid guidParent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (IAdapter item in (IEnumerable)connector.Adapters)
			{
				Guid[] modules = item.Modules;
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i] == guidParent)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ClearEditorList(bool bForce)
		{
			LList<EditorInfo> val = new LList<EditorInfo>();
			foreach (EditorInfo editor4 in _editorList)
			{
				if (!bForce && editor4.Editor is ICustomBaseDeviceEditor2 && (editor4.Editor as ICustomBaseDeviceEditor2).SkipReloadEditor)
				{
					val.Add(editor4);
					continue;
				}
				IBaseDeviceEditor editor = editor4.Editor;
				IConnectorEditor val2 = (IConnectorEditor)(object)((editor is IConnectorEditor) ? editor : null);
				if (val2 != null)
				{
					val2.ConnectorEditorFrame=((IConnectorEditorFrame)null);
				}
				IBaseDeviceEditor editor2 = editor4.Editor;
				IDeviceEditor val3 = (IDeviceEditor)(object)((editor2 is IDeviceEditor) ? editor2 : null);
				if (val3 != null)
				{
					val3.DeviceEditorFrame=((IDeviceEditorFrame)null);
				}
				IBaseDeviceEditor editor3 = editor4.Editor;
				ICommunicationEditor val4 = (ICommunicationEditor)(object)((editor3 is ICommunicationEditor) ? editor3 : null);
				if (val4 != null)
				{
					val4.DeviceEditorFrame=((IDeviceEditorFrame)null);
				}
				(editor4.Editor as IDisposable)?.Dispose();
			}
			_editorList.Clear();
			if (bForce)
			{
				return;
			}
			foreach (EditorInfo item in val)
			{
				_editorList.AddEditor(item);
			}
		}

		internal bool IsDevDescValid(IMetaObject meta)
		{
			IDeviceObject val = null;
			if (meta.Object is ISlotDeviceObject)
			{
				IObject @object = meta.Object;
				val = ((ISlotDeviceObject)((@object is ISlotDeviceObject) ? @object : null)).GetDeviceObject();
			}
			else if (meta.Object is IDeviceObject)
			{
				IObject object2 = meta.Object;
				val = (IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null);
			}
			if (val != null)
			{
				if (((IObject)val).Namespace != Guid.Empty && APEnvironment.DeviceRepository.GetDevice(val.DeviceIdentification) != null)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public void Reload()
		{
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Expected O, but got Unknown
			if (_bUndoActive)
			{
				_bReloadAfterCompound = true;
				return;
			}
			_bInReload = true;
			try
			{
				if (_reason != 0)
				{
					foreach (EditorInfo editor2 in _editorList)
					{
						if (editor2.Editor is ICustomBaseDeviceEditor && (editor2.Editor as ICustomBaseDeviceEditor).SkipReload)
						{
							return;
						}
					}
				}
				base.Visible = false;
				int selectedIndex = _tabControl.SelectedIndex;
				if (_associatedDeviceEditor != null && _associatedDeviceEditor.IsModifying)
				{
					_associatedDeviceEditor.Save(bCommit: true);
				}
				try
				{
					if (NeedsSave)
					{
						Save(bCommit: true);
					}
					else
					{
						_metaObject = null;
						GetObjectToRead();
					}
					foreach (EditorInfo editor3 in _editorList)
					{
						if ((!(editor3.Editor is ICustomBaseDeviceEditor2) || !(editor3.Editor as ICustomBaseDeviceEditor2).SkipReloadEditor) && editor3.Pages != null)
						{
							IEditorPage[] pages = editor3.Pages;
							for (int i = 0; i < pages.Length; i++)
							{
								pages[i].Control.Dispose();
							}
						}
					}
					RemoveMissingDevDescLabel();
					base.Controls.Remove(_tabControl);
					base.Controls.Remove(_pnlVerticalTabControl);
					if (_tabControl != null && !_tabControl.IsDisposed)
					{
						_tabControl.TabPages.Clear();
					}
					ClearEditorList(bForce: false);
					if (OptionsHelper.UseHorizontalTabPages)
					{
						if (_tabControl is VerticalTabControl || _tabControl.IsDisposed)
						{
							_pnlVerticalTabControl.Visible = false;
							base.Controls.Remove(_pnlVerticalTabControl);
							_tabControl.Dispose();
							base.Padding = new Padding(0);
							_tabControl = new TabControl();
							_tabControl.Dock = DockStyle.Fill;
							_tabControl.ImageList = new ImageList();
							_tabControl.SelectedIndexChanged += OnTabChanged;
							if (_bFirstReloadDone)
							{
								APEnvironment.DpiAdapter.AdaptControl((Control)_tabControl);
							}
						}
						base.Controls.Add(_tabControl);
					}
					else
					{
						if (!(_tabControl is VerticalTabControl) || _tabControl.IsDisposed)
						{
							base.Controls.Remove(_tabControl);
							_tabControl.Dispose();
							base.Padding = new Padding(5);
							_tabControl = (TabControl)new VerticalTabControl();
							_tabControl.Dock = DockStyle.Fill;
							_tabControl.ImageList = new ImageList();
							_pnlVerticalTabControl.Controls.Add(_tabControl);
							base.Controls.Add(_pnlVerticalTabControl);
							_pnlVerticalTabControl.Visible = true;
							_tabControl.SelectedIndexChanged += OnTabChanged;
							if (_bFirstReloadDone)
							{
								APEnvironment.DpiAdapter.AdaptControl((Control)_tabControl);
							}
						}
						base.Controls.Add(_pnlVerticalTabControl);
					}
					EditorList editorList = CreateEditorList(_metaObject);
					if (editorList == null)
					{
						return;
					}
					List<IEditorPage> list = new List<IEditorPage>();
					string[] allowedPages = GetAllowedPages();
					foreach (EditorInfo item in editorList)
					{
						try
						{
							if (!(item.Editor is ICustomBaseDeviceEditor2) || !(item.Editor as ICustomBaseDeviceEditor2).SkipReloadEditor)
							{
								goto IL_0413;
							}
							bool flag = false;
							foreach (EditorInfo editor4 in _editorList)
							{
								if (((object)editor4.Editor).GetType() == ((object)item.Editor).GetType())
								{
									flag = true;
								}
							}
							if (!flag)
							{
								goto IL_0413;
							}
							goto end_IL_0391;
							IL_0413:
							IBaseDeviceEditor editor = item.Editor;
							ILocalizableEditor val = (ILocalizableEditor)(object)((editor is ILocalizableEditor) ? editor : null);
							if (val != null)
							{
								val.SetLocalizedObject(_metaObject, _bLocalizationActive);
							}
							item.Editor.Reload();
							item.Pages = item.Editor.Pages;
							_editorList.AddEditor(item);
							end_IL_0391:;
						}
						catch
						{
						}
					}
					foreach (EditorInfo editor5 in _editorList)
					{
						if (editor5.Pages == null)
						{
							continue;
						}
						IEditorPage[] pages = editor5.Pages;
						foreach (IEditorPage val2 in pages)
						{
							editor5.ShowEditor = CheckEditorInfoVisibility(val2, editor5.Editor);
							if (editor5.ShowEditor)
							{
								list.Add(val2);
							}
						}
					}
					List<IEditorPage> list2 = SortEditorPages(_nProjectHandle, _guidObject, list, allowedPages);
					visibleComponents.Clear();
					switch (list2.Count)
					{
					case 0:
						_pnlVerticalTabControl.Visible = false;
						base.Controls.Remove(_pnlVerticalTabControl);
						_tabControl.Dispose();
						break;
					case 1:
					{
						_pnlVerticalTabControl.Visible = false;
						base.Controls.Remove(_pnlVerticalTabControl);
						_tabControl.Dispose();
						IEditorPage val4 = list2[0];
						base.Padding = new Padding(0);
						val4.Control.Dock = DockStyle.Fill;
						base.Controls.Add(val4.Control);
						if (_bFirstReloadDone)
						{
							APEnvironment.DpiAdapter.AdaptControl((Control)_pnlVerticalTabControl);
						}
						IVisibleEditor val5 = (IVisibleEditor)(object)((val4 is IVisibleEditor) ? val4 : null);
						if (val5 != null)
						{
							val5.IsHidden=(false);
						}
						break;
					}
					default:
					{
						Size size = new Size(0, 0);
						foreach (IEditorPage item2 in list2)
						{
							size.Width = Math.Max(item2.Control.Size.Width, size.Width);
							size.Height = Math.Max(item2.Control.Size.Height, size.Height);
							TabPage tabPage = (TabPage)(object)CreateTabPage(item2);
							_tabControl.TabPages.Add(tabPage);
							IVisibleEditor val3 = (IVisibleEditor)(object)((item2 is IVisibleEditor) ? item2 : null);
							if (val3 != null)
							{
								visibleComponents[tabPage] = val3;
							}
						}
						if (_reason == ReloadReason.Open && !string.IsNullOrEmpty(s_stLastSelectedPageName))
						{
							SelectEditorTab(s_stLastSelectedPageName);
						}
						else if (_tabControl.TabCount > selectedIndex && selectedIndex != -1)
						{
							_tabControl.SelectedTab = _tabControl.TabPages[selectedIndex];
						}
						using (Graphics graphics = CreateGraphics())
						{
							if (_tabControl is VerticalTabControl)
							{
								_pnlVerticalTabControl.AutoScrollMinSize = new Size((int)(graphics.DpiX / 96f * (float)(size.Width + _tabControl.ItemSize.Height)), (int)(graphics.DpiX / 96f * (float)_tabControl.MinimumSize.Height));
							}
							else
							{
								base.AutoScrollMinSize = new Size((int)(graphics.DpiX / 96f * (float)(size.Width + _tabControl.ItemSize.Height)), (int)(graphics.DpiX / 96f * (float)_tabControl.MinimumSize.Height));
							}
						}
						UpdateComponentVisibility();
						break;
					}
					}
				}
				catch
				{
					if (_tabControl != null)
					{
						if (_tabControl is VerticalTabControl)
						{
							_pnlVerticalTabControl.Visible = true;
							if (!_pnlVerticalTabControl.Controls.Contains(_tabControl))
							{
								_pnlVerticalTabControl.Controls.Add(_tabControl);
							}
						}
						else
						{
							_pnlVerticalTabControl.Visible = false;
							if (!base.Controls.Contains(_tabControl))
							{
								base.Controls.Add(_tabControl);
							}
						}
					}
				}
				base.Visible = true;
			}
			catch
			{
			}
			finally
			{
				if (_metaObject != null)
				{
					_bDevdescValid = IsDevDescValid(_metaObject);
					if (!_bDevdescValid)
					{
						AddMissingDevDescLabel();
					}
				}
				DoOnlineStatePages((_onlineState.OnlineApplication != Guid.Empty) ? DoOnlineState.Online : DoOnlineState.Offline);
				_reason = ReloadReason.Open;
				_bInReload = false;
				_bFirstReloadDone = true;
			}
		}

		private void AddMissingDevDescLabel()
		{
			if (_missingLabel == null)
			{
				_missingLabel = new Label();
				_missingLabel.Text = Strings.WarningDevdescNotInstalled;
				_missingLabel.BorderStyle = BorderStyle.FixedSingle;
				_missingLabel.AutoEllipsis = true;
				_missingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
				_missingLabel.Height = 32;
				_missingLabel.Dock = DockStyle.Top;
				_missingLabel.ForeColor = Color.FromArgb(159, 96, 0);
				_missingLabel.BackColor = Color.FromArgb(254, 239, 179);
				base.Controls.Add(_missingLabel);
			}
		}

		private void RemoveMissingDevDescLabel()
		{
			if (_missingLabel != null)
			{
				base.Controls.Remove(_missingLabel);
				_missingLabel = null;
			}
		}

		private EditorList CreateEditorList(IMetaObject metaObject)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			EditorList editorList = new EditorList();
			StatusPage statusPage = new StatusPage();
			statusPage.DeviceEditor = this;
			if (metaObject.Object is IDeviceObject)
			{
				IDeviceObject deviceObject = GetDeviceObject(bToModify: false);
				if (deviceObject == null)
				{
					return null;
				}
				CreateCommunicationTabs(deviceObject, editorList);
				CreateDeviceConfigTabs(deviceObject, statusPage, editorList);
				CreateAllConnectorsConfigTabs(deviceObject, statusPage, _metaObject.ObjectGuid, editorList);
			}
			else
			{
				if (!(metaObject.Object is IExplicitConnector))
				{
					Debug.Fail("Unexpected object type");
					return null;
				}
				IExplicitConnector val = (IExplicitConnector)_metaObject.Object;
				IDeviceObject deviceObject2 = ((IConnector)val).GetDeviceObject();
				bool bShowOnlyAdditionalControl = false;
				bool bShowOnlyDiagConnector = false;
				CreateConnectorConfigTabs(deviceObject2, (IConnector)(object)val, statusPage, _metaObject.ObjectGuid, editorList, ref bShowOnlyAdditionalControl, ref bShowOnlyDiagConnector);
			}
			if (statusPage.NumberPanels > 0)
			{
				if (_metaObject.Object is ILogicalDevice && (_metaObject.Object as ILogicalDevice).IsLogical)
				{
					statusPage = null;
				}
				DeviceInformationControl deviceInformationControl = new DeviceInformationControl(statusPage);
				deviceInformationControl.DeviceEditorFrame = (IDeviceEditorFrame)(object)this;
				editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)deviceInformationControl));
			}
			if (!OpenView)
			{
				DeviceInformationControl deviceInformationControl2 = new DeviceInformationControl(statusPage);
				deviceInformationControl2.DeviceEditorFrame = (IDeviceEditorFrame)(object)this;
				editorList.AddEditor(new EditorInfo((IBaseDeviceEditor)(object)deviceInformationControl2));
			}
			return editorList;
		}

		private void UpdateComponentVisibility()
		{
			if (_bInSave)
			{
				_bUpdateComponentVisibilityInSave = true;
			}
			else
			{
				if (_tabControl.IsDisposed)
				{
					return;
				}
				TabPage selectedTab = _tabControl.SelectedTab;
				foreach (TabPage key in visibleComponents.Keys)
				{
					bool flag = key != selectedTab;
					if (visibleComponents[key].IsHidden != flag)
					{
						visibleComponents[key].IsHidden=(flag);
					}
				}
			}
		}

		public void OnVisibilityChanged(bool bVisible)
		{
			_bVisible = bVisible;
			IEditorPage val = null;
			if (_tabControl != null && !_tabControl.IsDisposed)
			{
				TabPage selectedTab = _tabControl.SelectedTab;
				if (selectedTab != null)
				{
					object tag = selectedTab.Tag;
					val = (IEditorPage)((tag is IEditorPage) ? tag : null);
				}
			}
			foreach (EditorInfo editor2 in _editorList)
			{
				try
				{
					IBaseDeviceEditor editor = editor2.Editor;
					IVisibleEditor val2 = (IVisibleEditor)(object)((editor is IVisibleEditor) ? editor : null);
					if (val2 != null)
					{
						val2.IsHidden=(!bVisible);
					}
					IEditorPage[] pages = editor2.Pages;
					foreach (IEditorPage val3 in pages)
					{
						if (val3.Control == null || !val3.Control.IsDisposed)
						{
							IVisibleEditor val4 = (IVisibleEditor)(object)((val3 is IVisibleEditor) ? val3 : null);
							if (val4 != null)
							{
								val4.IsHidden=((val3 != val && val != null) || !bVisible);
							}
						}
					}
				}
				catch (Exception ex)
				{
					IMessageService messageService = ((IEngine)APEnvironment.Engine).MessageService;
					IMessageService5 val5 = (IMessageService5)(object)((messageService is IMessageService5) ? messageService : null);
					if (val5 != null)
					{
						((IMessageService)val5).Error($"Exception while changing device editor visibility:\r\n{ex.Message}\r\n{ex.StackTrace}");
					}
				}
			}
		}

		public void EnableVisibilityChangeNotification()
		{
		}

		private List<IEditorPage> SortEditorPages(int nProjectHandle, Guid objectGuid, List<IEditorPage> pages, string[] allowedPages)
		{
			List<IEditorPage> list = new List<IEditorPage>();
			if (allowedPages != null)
			{
				foreach (string text in allowedPages)
				{
					foreach (IEditorPage page in pages)
					{
						IEditorPageAppearance val = (IEditorPageAppearance)(object)((page is IEditorPageAppearance) ? page : null);
						if (val != null)
						{
							string pageIdentifier = val.PageIdentifier;
							if (text.ToLowerInvariant() == pageIdentifier.ToLowerInvariant() && !list.Contains(page))
							{
								list.Add(page);
							}
						}
					}
				}
				foreach (IEditorPage page2 in pages)
				{
					if (!list.Contains(page2))
					{
						list.Add(page2);
					}
				}
			}
			else
			{
				list.AddRange(pages);
			}
			if (AppearanceFactoryManager.Instance.SortPages(nProjectHandle, objectGuid, list, out var sortedPages))
			{
				return sortedPages;
			}
			return list;
		}

		private string[] GetAllowedPages()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Expected O, but got Unknown
			if (_metaObject.Object != null && typeof(IDeviceObject).IsAssignableFrom(((object)_metaObject.Object).GetType()))
			{
				foreach (IConnector item in (IEnumerable)((IDeviceObject)_metaObject.Object).Connectors)
				{
					IConnector val = item;
					if (typeof(IConnector6).IsAssignableFrom(((object)val).GetType()) && !val.IsExplicit)
					{
						IConnector6 val2 = (IConnector6)val;
						if (val2.AllowedPages != null && val2.AllowedPages.Length != 0)
						{
							return val2.AllowedPages;
						}
					}
				}
			}
			if (_metaObject.Object != null && typeof(IExplicitConnector).IsAssignableFrom(((object)_metaObject.Object).GetType()))
			{
				IObject @object = _metaObject.Object;
				IExplicitConnector val3 = (IExplicitConnector)(object)((@object is IExplicitConnector) ? @object : null);
				if (typeof(IConnector6).IsAssignableFrom(((object)val3).GetType()))
				{
					IConnector6 val4 = (IConnector6)val3;
					if (val4.AllowedPages != null && val4.AllowedPages.Length != 0)
					{
						return val4.AllowedPages;
					}
				}
			}
			return null;
		}

		internal bool CheckEditorInfoVisibility(IEditorPage page, IBaseDeviceEditor editor)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Expected O, but got Unknown
			bool flag = true;
			if (_metaObject.Object != null && typeof(IDeviceObject).IsAssignableFrom(((object)_metaObject.Object).GetType()))
			{
				foreach (IConnector item in (IEnumerable)((IDeviceObject)_metaObject.Object).Connectors)
				{
					IConnector val = item;
					if (AppearanceFactoryManager.Instance.ShowEditor(_nProjectHandle, _guidObject, val, page, out var bShowEditor))
					{
						return bShowEditor;
					}
					if (!typeof(IConnector6).IsAssignableFrom(((object)val).GetType()) || val.IsExplicit)
					{
						continue;
					}
					IConnector6 val2 = (IConnector6)val;
					if (val2.AllowedPages != null && val2.AllowedPages.Length != 0)
					{
						flag = CheckPageAppearance(val2.AllowedPages, page);
						if (flag)
						{
							break;
						}
					}
				}
			}
			if (_metaObject.Object != null && typeof(IExplicitConnector).IsAssignableFrom(((object)_metaObject.Object).GetType()))
			{
				IObject @object = _metaObject.Object;
				IExplicitConnector val3 = (IExplicitConnector)(object)((@object is IExplicitConnector) ? @object : null);
				if (AppearanceFactoryManager.Instance.ShowEditor(_nProjectHandle, _guidObject, (IConnector)(object)val3, page, out var bShowEditor2))
				{
					return bShowEditor2;
				}
				if (typeof(IConnector6).IsAssignableFrom(((object)val3).GetType()))
				{
					IConnector6 val4 = (IConnector6)val3;
					if (val4.AllowedPages != null && val4.AllowedPages.Length != 0)
					{
						flag = CheckPageAppearance(val4.AllowedPages, page);
					}
				}
			}
			object value = APEnvironment.Engine.OEMCustomization.GetValue("DeviceEditor", "AppearanceCallback");
			EditorPageAppearanceCallback val5 = (EditorPageAppearanceCallback)((value is EditorPageAppearanceCallback) ? value : null);
			if (val5 != null)
			{
				val5.Invoke(_nProjectHandle, _guidObject, editor, page, ref flag);
			}
			if (page is ParameterEditorPage && OptionsHelper.IsOemValueAvailable("DeviceEditor", "ShowGenericEditor", out var value2) && value2 is bool && (bool)value2)
			{
				flag = true;
			}
			return flag;
		}

		private bool CheckPageAppearance(string[] allowedPages, IEditorPage page)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			if (typeof(IEditorPageAppearance).IsAssignableFrom(((object)page).GetType()))
			{
				string pageIdentifier = ((IEditorPageAppearance)page).PageIdentifier;
				for (int i = 0; i < allowedPages.Length; i++)
				{
					if (allowedPages[i].ToLowerInvariant() == pageIdentifier.ToLowerInvariant())
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public void Save(bool bCommit)
		{
			_bInSave = true;
			if (NeedsSave)
			{
				IMetaObject metaObject = _metaObject;
				EditorList editorList = new EditorList();
				foreach (EditorInfo editor in _editorList)
				{
					editorList.AddEditor(editor);
				}
				try
				{
					foreach (EditorInfo item in editorList)
					{
						item.Editor.Save(bCommit);
					}
					if (metaObject != null && metaObject.IsToModify)
					{
						IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(metaObject.ProjectHandle);
						bool flag = false;
						try
						{
							undoManager.BeginCompoundAction(base.Name);
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(metaObject, bCommit, (object)this);
							flag = true;
						}
						finally
						{
							undoManager.EndCompoundAction();
							if (!flag)
							{
								undoManager.Undo();
							}
						}
					}
				}
				catch
				{
					if (metaObject != null && metaObject.IsToModify)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(metaObject, false, (object)this);
					}
					Reload();
					APEnvironment.MessageService.Error(Strings.ErrorSaveNotSuccessfull, "SaveNotSuccessfull", Array.Empty<object>());
				}
				UnregisterEvents(metaObject);
				_metaObject = GetObjectToRead();
			}
			_bInSave = false;
			if (_bUpdateComponentVisibilityInSave)
			{
				UpdateComponentVisibility();
				_bUpdateComponentVisibilityInSave = false;
			}
		}

		public IMetaObject GetObjectToRead()
		{
			if (_metaObject == null)
			{
				_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidObject);
			}
			return _metaObject;
		}

		public IMetaObject GetObjectToModify()
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			if (_nProjectHandle >= 0 && (_metaObject == null || !_metaObject.IsToModify))
			{
				try
				{
					IMetaObject val = ((_metaObject != null) ? _metaObject : ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidObject));
					if (val.Object is IGenerateableObject)
					{
						MetaObjectCancelEventArgs val2 = new MetaObjectCancelEventArgs(val);
						foreach (IGeneratedObjectProtector generatedObjectProtector in APEnvironment.GeneratedObjectProtectors)
						{
							generatedObjectProtector.CheckPermissionToModify((object)Editor, val2);
							if (((ObjectCancelEventArgs)val2).Exception != null)
							{
								return null;
							}
						}
					}
					_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _guidObject);
					if (_metaObject != null)
					{
						RegisterEvents(_metaObject);
					}
				}
				catch
				{
					return null;
				}
			}
			return _metaObject;
		}

		private void RegisterEvents(IMetaObject metaObject)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Expected O, but got Unknown
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Expected O, but got Unknown
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Expected O, but got Unknown
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Expected O, but got Unknown
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Expected O, but got Unknown
			if (_metaObject == null)
			{
				return;
			}
			if (metaObject.Object is IDeviceObject)
			{
				IDeviceObject val = (IDeviceObject)metaObject.Object;
				ParameterEventHandler val2 = new ParameterEventHandler(ParameterSet_ParameterAdded);
				ParameterEventHandler val3 = new ParameterEventHandler(ParameterSet_ParameterRemoved);
				ParameterChangedEventHandler val4 = new ParameterChangedEventHandler(ParameterSet_ParameterChanged);
				ParameterSectionChangedEventHandler val5 = new ParameterSectionChangedEventHandler(ParameterSet_ParameterSectionChanged);
				ParameterMovedEventHandler val6 = new ParameterMovedEventHandler(ParameterSet_ParameterMoved);
				val.DeviceParameterSet.ParameterAdded+=(val2);
				val.DeviceParameterSet.ParameterRemoved+=(val3);
				val.DeviceParameterSet.ParameterChanged+=(val4);
				IParameterSet deviceParameterSet = val.DeviceParameterSet;
				IParameterSet3 val7 = (IParameterSet3)(object)((deviceParameterSet is IParameterSet3) ? deviceParameterSet : null);
				if (val7 != null)
				{
					val7.ParameterSectionChanged+=(val5);
					val7.ParameterMoved+=(val6);
				}
				foreach (IConnector item in (IEnumerable)val.Connectors)
				{
					item.HostParameterSet.ParameterAdded+=(val2);
					item.HostParameterSet.ParameterRemoved+=(val3);
					item.HostParameterSet.ParameterChanged+=(val4);
					IParameterSet hostParameterSet = item.HostParameterSet;
					val7 = (IParameterSet3)(object)((hostParameterSet is IParameterSet3) ? hostParameterSet : null);
					if (val7 != null)
					{
						val7.ParameterSectionChanged+=(val5);
						val7.ParameterMoved+=(val6);
					}
				}
			}
			else if (metaObject.Object is IExplicitConnector)
			{
				IExplicitConnector val9 = (IExplicitConnector)metaObject.Object;
				((IConnector)val9).HostParameterSet.ParameterAdded+=(new ParameterEventHandler(ParameterSet_ParameterAdded));
				((IConnector)val9).HostParameterSet.ParameterRemoved+=(new ParameterEventHandler(ParameterSet_ParameterRemoved));
				((IConnector)val9).HostParameterSet.ParameterChanged+=(new ParameterChangedEventHandler(ParameterSet_ParameterChanged));
				IParameterSet hostParameterSet2 = ((IConnector)val9).HostParameterSet;
				IParameterSet3 val10 = (IParameterSet3)(object)((hostParameterSet2 is IParameterSet3) ? hostParameterSet2 : null);
				if (val10 != null)
				{
					val10.ParameterSectionChanged+=(new ParameterSectionChangedEventHandler(ParameterSet_ParameterSectionChanged));
					val10.ParameterMoved+=(new ParameterMovedEventHandler(ParameterSet_ParameterMoved));
				}
			}
		}

		private void UnregisterEvents(IMetaObject metaObject)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Expected O, but got Unknown
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Expected O, but got Unknown
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Expected O, but got Unknown
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Expected O, but got Unknown
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Expected O, but got Unknown
			if (metaObject == null)
			{
				return;
			}
			if (metaObject.Object is IDeviceObject)
			{
				IDeviceObject val = (IDeviceObject)metaObject.Object;
				ParameterEventHandler val2 = new ParameterEventHandler(ParameterSet_ParameterAdded);
				ParameterEventHandler val3 = new ParameterEventHandler(ParameterSet_ParameterRemoved);
				ParameterChangedEventHandler val4 = new ParameterChangedEventHandler(ParameterSet_ParameterChanged);
				ParameterSectionChangedEventHandler val5 = new ParameterSectionChangedEventHandler(ParameterSet_ParameterSectionChanged);
				ParameterMovedEventHandler val6 = new ParameterMovedEventHandler(ParameterSet_ParameterMoved);
				val.DeviceParameterSet.ParameterAdded-=(val2);
				val.DeviceParameterSet.ParameterRemoved-=(val3);
				val.DeviceParameterSet.ParameterChanged-=(val4);
				IParameterSet deviceParameterSet = val.DeviceParameterSet;
				IParameterSet3 val7 = (IParameterSet3)(object)((deviceParameterSet is IParameterSet3) ? deviceParameterSet : null);
				if (val7 != null)
				{
					val7.ParameterSectionChanged-=(val5);
					val7.ParameterMoved-=(val6);
				}
				foreach (IConnector item in (IEnumerable)val.Connectors)
				{
					item.HostParameterSet.ParameterAdded-=(val2);
					item.HostParameterSet.ParameterRemoved-=(val3);
					item.HostParameterSet.ParameterChanged-=(val4);
					IParameterSet hostParameterSet = item.HostParameterSet;
					val7 = (IParameterSet3)(object)((hostParameterSet is IParameterSet3) ? hostParameterSet : null);
					if (val7 != null)
					{
						val7.ParameterSectionChanged-=(val5);
						val7.ParameterMoved-=(val6);
					}
				}
			}
			else if (metaObject.Object is IExplicitConnector)
			{
				IExplicitConnector val9 = (IExplicitConnector)metaObject.Object;
				((IConnector)val9).HostParameterSet.ParameterAdded-=(new ParameterEventHandler(ParameterSet_ParameterAdded));
				((IConnector)val9).HostParameterSet.ParameterRemoved-=(new ParameterEventHandler(ParameterSet_ParameterRemoved));
				((IConnector)val9).HostParameterSet.ParameterChanged-=(new ParameterChangedEventHandler(ParameterSet_ParameterChanged));
				IParameterSet hostParameterSet2 = ((IConnector)val9).HostParameterSet;
				IParameterSet3 val10 = (IParameterSet3)(object)((hostParameterSet2 is IParameterSet3) ? hostParameterSet2 : null);
				if (val10 != null)
				{
					val10.ParameterSectionChanged-=(new ParameterSectionChangedEventHandler(ParameterSet_ParameterSectionChanged));
					val10.ParameterMoved-=(new ParameterMovedEventHandler(ParameterSet_ParameterMoved));
				}
			}
		}

		public void UpdatePages(IBaseDeviceEditor editor)
		{
			UpdatePages(editor, bKeepSelectedTab: true);
		}

		private void UpdatePages(IBaseDeviceEditor editor, bool bKeepSelectedTab)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			if (_tabControl.IsDisposed)
			{
				if (OptionsHelper.UseHorizontalTabPages)
				{
					base.Padding = new Padding(0);
					_tabControl = new TabControl();
					_tabControl.Dock = DockStyle.Fill;
					_tabControl.ImageList = new ImageList();
					_tabControl.SelectedIndexChanged += OnTabChanged;
					if (_bFirstReloadDone)
					{
						APEnvironment.DpiAdapter.AdaptControl((Control)_tabControl);
					}
					base.Controls.Add(_tabControl);
				}
				else
				{
					base.Padding = new Padding(5);
					_tabControl = (TabControl)new VerticalTabControl();
					_tabControl.Dock = DockStyle.Fill;
					_tabControl.ImageList = new ImageList();
					_pnlVerticalTabControl.Controls.Add(_tabControl);
					base.Controls.Add(_pnlVerticalTabControl);
					_pnlVerticalTabControl.Visible = true;
					_tabControl.SelectedIndexChanged += OnTabChanged;
					if (_bFirstReloadDone)
					{
						APEnvironment.DpiAdapter.AdaptControl((Control)_tabControl);
					}
					base.Controls.Add(_pnlVerticalTabControl);
				}
			}
			int nPageIndex = 0;
			EditorInfo editorInfo = _editorList.FindEditor(editor, out nPageIndex);
			if (editorInfo != null)
			{
				editorInfo.Pages = editorInfo.Editor.Pages;
			}
			List<IEditorPage> list = new List<IEditorPage>();
			foreach (EditorInfo editor2 in _editorList)
			{
				IEditorPage[] pages = editor2.Pages;
				foreach (IEditorPage val in pages)
				{
					editor2.ShowEditor = CheckEditorInfoVisibility(val, editor2.Editor);
					if (editor2.ShowEditor)
					{
						list.Add(val);
					}
				}
			}
			string[] allowedPages = GetAllowedPages();
			List<IEditorPage> list2 = SortEditorPages(_nProjectHandle, _guidObject, list, allowedPages);
			if (bKeepSelectedTab)
			{
				TabPage selectedTab = _tabControl.SelectedTab;
				if (selectedTab != null)
				{
					object tag = selectedTab.Tag;
					IEditorPage val2 = (IEditorPage)((tag is IEditorPage) ? tag : null);
					if (val2 != null && !list2.Contains(val2))
					{
						int selectedIndex = _tabControl.SelectedIndex;
						if (selectedIndex < list2.Count)
						{
							list2.Insert(selectedIndex, val2);
						}
						else
						{
							list2.Add(val2);
						}
					}
				}
			}
			List<TabPage> list3 = new List<TabPage>();
			foreach (TabPage tabPage3 in _tabControl.TabPages)
			{
				object tag2 = tabPage3.Tag;
				IEditorPage val3 = (IEditorPage)((tag2 is IEditorPage) ? tag2 : null);
				if (val3 != null && !list2.Contains(val3))
				{
					list3.Add(tabPage3);
				}
			}
			foreach (TabPage item in list3)
			{
				_tabControl.TabPages.Remove(item);
			}
			int num = 0;
			visibleComponents.Clear();
			foreach (IEditorPage item2 in list2)
			{
				int num2 = FindTabIndex(item2);
				TabPage tabPage2 = null;
				if (num2 == -1)
				{
					tabPage2 = (TabPage)(object)CreateTabPage(item2);
					try
					{
						if (num < _tabControl.TabPages.Count)
						{
							_tabControl.TabPages.Insert(num, tabPage2);
						}
						else
						{
							_tabControl.TabPages.Add(tabPage2);
						}
					}
					catch
					{
					}
				}
				else if (num2 != num)
				{
					if (num2 < _tabControl.TabPages.Count)
					{
						tabPage2 = _tabControl.TabPages[num2];
						try
						{
							if (_tabControl.TabPages.Contains(tabPage2))
							{
								_tabControl.TabPages.RemoveAt(num2);
							}
						}
						catch
						{
						}
						if (num < _tabControl.TabPages.Count)
						{
							_tabControl.TabPages.Insert(num, tabPage2);
						}
						else
						{
							_tabControl.TabPages.Add(tabPage2);
						}
					}
				}
				else
				{
					tabPage2 = _tabControl.TabPages[num2];
				}
				num++;
				IVisibleEditor val4 = (IVisibleEditor)(object)((item2 is IVisibleEditor) ? item2 : null);
				if (val4 != null)
				{
					visibleComponents[tabPage2] = val4;
				}
			}
			if (!_tabControl.IsDisposed)
			{
				try
				{
					switch (_tabControl.TabPages.Count)
					{
					case 0:
						_pnlVerticalTabControl.Visible = false;
						base.Controls.Remove(_pnlVerticalTabControl);
						_tabControl.Dispose();
						break;
					case 1:
					{
						_tabControl.TabPages.RemoveAt(0);
						_pnlVerticalTabControl.Visible = false;
						base.Controls.Remove(_pnlVerticalTabControl);
						_tabControl.Dispose();
						IEditorPage val5 = list2[0];
						base.Padding = new Padding(0);
						val5.Control.Dock = DockStyle.Fill;
						base.Controls.Add(val5.Control);
						if (_bFirstReloadDone)
						{
							APEnvironment.DpiAdapter.AdaptControl((Control)_pnlVerticalTabControl);
						}
						IVisibleEditor val6 = (IVisibleEditor)(object)((val5 is IVisibleEditor) ? val5 : null);
						if (val6 != null)
						{
							val6.IsHidden=(false);
						}
						break;
					}
					}
				}
				catch
				{
				}
			}
			UpdateComponentVisibility();
		}

		private int FindTabIndex(IEditorPage page)
		{
			int result = -1;
			int num = 0;
			foreach (TabPage tabPage in _tabControl.TabPages)
			{
				if (tabPage.Tag == page)
				{
					return num;
				}
				num++;
			}
			return result;
		}

		public void Mark(long nPosition, int nLength, object tag)
		{
		}

		public void UnmarkAll(object tag)
		{
		}

		public void Select(long lPosition, int nLength)
		{
			IEditorPage val = null;
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(lPosition, out num, out num2);
			foreach (EditorInfo editor2 in _editorList)
			{
				IBaseDeviceEditor editor = editor2.Editor;
				IBaseDeviceEditor2 val2 = (IBaseDeviceEditor2)(object)((editor is IBaseDeviceEditor2) ? editor : null);
				if (val2 != null)
				{
					IEditorPage val3 = val2.Select(num, (int)num2, nLength);
					if (val == null && val3 != null)
					{
						val = val3;
						break;
					}
				}
			}
			if (val == null || _tabControl.IsDisposed)
			{
				return;
			}
			foreach (TabPage tabPage in _tabControl.TabPages)
			{
				if (tabPage.Tag == val)
				{
					_tabControl.SelectedTab = tabPage;
					break;
				}
			}
		}

		public void GetSelection(out long nPosition, out int nLength)
		{
			IEditorPage2 val = null;
			if (!_tabControl.IsDisposed)
			{
				TabPage tabPage = _tabControl?.SelectedTab;
				if (tabPage != null)
				{
					object tag = tabPage.Tag;
					val = (IEditorPage2)((tag is IEditorPage2) ? tag : null);
				}
			}
			else if (base.Controls.Count > 0)
			{
				Control control = base.Controls[0];
				val = (IEditorPage2)(object)((control is IEditorPage2) ? control : null);
			}
			if (val != null)
			{
				long num = default(long);
				short num2 = default(short);
				val.GetSelection(out num, out num2, out nLength);
				nPosition = PositionHelper.CombinePosition(num, num2);
			}
			else
			{
				nPosition = -1L;
				nLength = 0;
			}
		}

		public int ComparePositions(long nPosition1, long nPosition2)
		{
			return 0;
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			IEditorPage3 val = null;
			if (!_tabControl.IsDisposed)
			{
				if (_tabControl.SelectedTab == null)
				{
					return false;
				}
				object tag = _tabControl.SelectedTab.Tag;
				val = (IEditorPage3)((tag is IEditorPage3) ? tag : null);
			}
			else if (base.Controls.Count > 0)
			{
				Control control = base.Controls[0];
				val = (IEditorPage3)(object)((control is IEditorPage3) ? control : null);
			}
			if (val == null)
			{
				return false;
			}
			return val.CanExecuteStandardCommand(commandGuid);
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			IEditorPage3 val = null;
			if (!_tabControl.IsDisposed)
			{
				if (_tabControl.SelectedTab == null)
				{
					throw new InvalidOperationException("Standard Commmands not allowed.");
				}
				object tag = _tabControl.SelectedTab.Tag;
				val = (IEditorPage3)((tag is IEditorPage3) ? tag : null);
			}
			else
			{
				Control control = base.Controls[0];
				val = (IEditorPage3)(object)((control is IEditorPage3) ? control : null);
			}
			if (val == null)
			{
				throw new InvalidOperationException("Standard Commmands not allowed.");
			}
			val.ExecuteStandardCommand(commandGuid);
		}

		public bool SelectOnlineState(IOnlineUIServices uiServices, out OnlineState onlineState)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			onlineState = default(OnlineState);
			if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _guidObject))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, _guidObject);
				if (metaObjectStub != null)
				{
					while (metaObjectStub != null && metaObjectStub.ParentObjectGuid != Guid.Empty)
					{
						metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, metaObjectStub.ParentObjectGuid);
					}
				}
				if (metaObjectStub != null)
				{
					Guid[] subObjectGuids = metaObjectStub.SubObjectGuids;
					foreach (Guid guid in subObjectGuids)
					{
						IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guid);
						if (metaObjectStub2 == null)
						{
							continue;
						}
						Guid[] subObjectGuids2 = metaObjectStub2.SubObjectGuids;
						foreach (Guid guid2 in subObjectGuids2)
						{
							IMetaObjectStub metaObjectStub3 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, guid2);
							if (metaObjectStub3 != null && typeof(IApplicationObject).IsAssignableFrom(metaObjectStub3.ObjectType))
							{
								IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guid2);
								if (application != null && application.IsLoggedIn)
								{
									onlineState.OnlineApplication = guid2;
								}
							}
						}
					}
				}
			}
			return true;
		}

		private void _retryTimer_Tick(object sender, EventArgs e)
		{
			Timer timer = sender as Timer;
			if (timer != null)
			{
				timer.Stop();
				timer.Tick -= _retryTimer_Tick;
				DoOnlineStatePages((DoOnlineState)timer.Tag);
			}
		}

		private void DoOnlineStatePages(DoOnlineState doOnlineState)
		{
			if (doOnlineState != DoOnlineState.ClearStates)
			{
				foreach (EditorInfo editor in _editorList)
				{
					if (editor.Pages == null)
					{
						continue;
					}
					IEditorPage[] pages = editor.Pages;
					foreach (IEditorPage val in pages)
					{
						bool flag = false;
						if (val is IEditorPageAppearance2)
						{
							flag = ((IEditorPageAppearance2)((val is IEditorPageAppearance2) ? val : null)).HasOnlineMode;
						}
						if (!flag && val.Control != null && val.Control.Controls != null)
						{
							if (_retryTimer.Enabled)
							{
								_retryTimer.Stop();
								_retryTimer.Tick -= _retryTimer_Tick;
							}
							if (!val.Control.Enabled)
							{
								_retryTimer.Tick += _retryTimer_Tick;
								_retryTimer.Interval = 2000;
								_retryTimer.Tag = doOnlineState;
								_retryTimer.Start();
								return;
							}
							DoOnlineControls(ControlExcludeList: (!(val is IEditorPageAppearance3)) ? new List<Control>() : ((IEditorPageAppearance3)((val is IEditorPageAppearance3) ? val : null)).OnlineModeExcludeList, control: val.Control, doOnlineState: doOnlineState);
						}
						if ((doOnlineState == DoOnlineState.Offline || doOnlineState == DoOnlineState.Online) && val is IEditorPageAppearance3)
						{
							((IEditorPageAppearance3)((val is IEditorPageAppearance3) ? val : null)).IsOnline=(doOnlineState == DoOnlineState.Online);
						}
					}
				}
			}
			else
			{
				_htControlState.Clear();
			}
		}

		private void DoOnlineControls(Control control, DoOnlineState doOnlineState, IList<Control> ControlExcludeList)
		{
			if (control == null || (doOnlineState != 0 && (!control.Enabled || (control is TreeTableView && ((TreeTableView)((control is TreeTableView) ? control : null)).ReadOnly))) || (ControlExcludeList != null && ControlExcludeList.Contains(control)))
			{
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				if (ControlExcludeList == null || !ControlExcludeList.Contains(control2))
				{
					DoOnlineControls(control2, doOnlineState, ControlExcludeList);
				}
			}
			if (control is Label || control is GroupBox || control is Panel || control is IEditorPage || control is ContainerControl)
			{
				return;
			}
			switch (doOnlineState)
			{
			case DoOnlineState.Online:
			case DoOnlineState.StoreStates:
				if (!_htControlState.ContainsKey(control))
				{
					if (control is TreeTableView)
					{
						_htControlState.Add(control, ((TreeTableView)((control is TreeTableView) ? control : null)).ReadOnly);
					}
					else
					{
						_htControlState.Add(control, control.Enabled);
					}
				}
				if (doOnlineState == DoOnlineState.Online)
				{
					if (control is TreeTableView)
					{
						((TreeTableView)((control is TreeTableView) ? control : null)).ReadOnly=(true);
					}
					else
					{
						control.Enabled = false;
					}
				}
				break;
			case DoOnlineState.Offline:
				if (_htControlState.ContainsKey(control))
				{
					if (control is TreeTableView)
					{
						((TreeTableView)((control is TreeTableView) ? control : null)).ReadOnly=(false);
					}
					else
					{
						control.Enabled = true;
					}
					_htControlState.Remove(control);
				}
				break;
			}
		}

		public IEditorPage GetActiveEditorPage()
		{
			if (_tabControl.SelectedTab != null)
			{
				object tag = _tabControl.SelectedTab.Tag;
				return (IEditorPage)((tag is IEditorPage) ? tag : null);
			}
			return null;
		}

		protected VerticalTabPage CreateTabPage(IEditorPage page)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			string pageName = page.PageName;
			VerticalTabPage val = new VerticalTabPage();
			((Control)(object)val).Text = pageName;
			((Control)(object)val).Controls.Add(page.Control);
			if (_bFirstReloadDone)
			{
				APEnvironment.DpiAdapter.AdaptControl(page.Control);
			}
			((Control)(object)val).Tag = page;
			page.Control.Dock = DockStyle.Fill;
			Icon icon = page.Icon;
			if (icon != null)
			{
				if (!_tabControl.ImageList.Images.ContainsKey(pageName))
				{
					Size smallIconSize = SystemInformation.SmallIconSize;
					if (!icon.Size.Equals(smallIconSize))
					{
						icon = GraphicsHelper.ScaleIcon(icon, smallIconSize.Height);
					}
					_tabControl.ImageList.Images.Add(pageName, icon);
				}
				((TabPage)(object)val).ImageIndex = _tabControl.ImageList.Images.IndexOfKey(pageName);
			}
			if (page is IVerticalTabPageAppearance)
			{
				val.VerticalTabPageAppearance=((IVerticalTabPageAppearance)(object)((page is IVerticalTabPageAppearance) ? page : null));
			}
			if (page is IVerticalTabPageBehavior)
			{
				val.VerticalTabPageBehavior=((IVerticalTabPageBehavior)(object)((page is IVerticalTabPageBehavior) ? page : null));
			}
			return val;
		}

		public IPrintPainterEx CreatePrintPainter(long nPosition, int nLength)
		{
			if (_metaObject != null)
			{
				IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(_metaObject);
				if (editors != null)
				{
					IEditor[] array = editors;
					foreach (IEditor val in array)
					{
						if (val is DeviceEditor && (val as DeviceEditor)?._metaObject != null && (val as DeviceEditor)._metaObject.IsToModify)
						{
							(val as DeviceEditor).Save(bCommit: true);
						}
					}
				}
			}
			EditorList editorList = _editorList;
			return (IPrintPainterEx)(object)new DevicePrintPainter(this, GetDeviceObject(bToModify: false), editorList, nPosition, nLength);
		}

		public bool NotifyOnly()
		{
			return true;
		}

		public bool RemoveEditorTab(IBaseDeviceEditor editor, string stPageName)
		{
			if (_editorList.FindEditor(editor, out var nPageIndex) == null && _editorList.UpdateEditorInfo(editor, out nPageIndex) == null)
			{
				return false;
			}
			foreach (TabPage tabPage in _tabControl.TabPages)
			{
				if (tabPage.Text.Equals(stPageName))
				{
					_editorList.UpdateEditorInfoPages(editor);
					UpdatePages(editor, bKeepSelectedTab: false);
					return true;
				}
			}
			return false;
		}

		public bool AddEditorTab(IBaseDeviceEditor editor, string stPageName)
		{
			int nPageIndex;
			EditorInfo editorInfo = _editorList.FindEditor(editor, out nPageIndex);
			if (editorInfo == null)
			{
				editorInfo = _editorList.UpdateEditorInfo(editor, out nPageIndex);
				if (editorInfo == null)
				{
					return false;
				}
			}
			IEditorPage[] pages = editorInfo.Pages;
			for (int i = 0; i < pages.Length; i++)
			{
				if (pages[i].PageName.Equals(stPageName))
				{
					return false;
				}
			}
			pages = editor.Pages;
			for (int i = 0; i < pages.Length; i++)
			{
				if (pages[i].PageName.Equals(stPageName))
				{
					_editorList.UpdateEditorInfoPages(editor);
					UpdatePages(editor, bKeepSelectedTab: false);
					return true;
				}
			}
			return false;
		}

		public void SelectEditorTab(string stPageName)
		{
			for (int i = 0; i < _tabControl.TabPages.Count; i++)
			{
				if (_tabControl.TabPages[i].Text.Equals(stPageName))
				{
					_tabControl.SelectTab(i);
					break;
				}
			}
		}

		public void UpdateEditorTabs(IBaseDeviceEditor editor)
		{
			if (_editorList.FindEditor(editor, out var _) != null)
			{
				UpdatePages(editor);
			}
		}

		internal IDeviceObject GetHost(bool bToModify)
		{
			try
			{
				int projectHandle = _metaObject.ProjectHandle;
				if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(projectHandle, _metaObject.ObjectGuid))
				{
					return null;
				}
				IMetaObjectStub val = APEnvironment.DeviceMgr.GetPlcDevice(projectHandle, _metaObject.ObjectGuid);
				if (val == null)
				{
					val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, _metaObject.ObjectGuid);
					while (val.ParentObjectGuid != Guid.Empty)
					{
						val = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, val.ParentObjectGuid);
					}
				}
				if (!bToModify)
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, val.ObjectGuid);
					if (objectToRead != null)
					{
						IObject @object = objectToRead.Object;
						return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
					}
				}
				else
				{
					IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(projectHandle, val.ObjectGuid);
					if (objectToModify != null)
					{
						IObject object2 = objectToModify.Object;
						return (IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null);
					}
				}
			}
			catch
			{
			}
			return null;
		}

		public void SetPageCaption(IEditorPage page, string stCaption)
		{
			foreach (TabPage tabPage in _tabControl.TabPages)
			{
				object tag = tabPage.Tag;
				IEditorPage val = (IEditorPage)((tag is IEditorPage) ? tag : null);
				if (val != null && page == val)
				{
					tabPage.Text = stCaption;
				}
			}
		}

		public void SetCaption(string stCaption)
		{
			Caption = stCaption;
			((IEngine)APEnvironment.Engine).Frame.UpdateCaption((IView)(object)this);
		}

		public string GetOnlineHelpKeyword()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			IEditorPage val = null;
			if (!_tabControl.IsDisposed)
			{
				if (_tabControl.SelectedTab != null)
				{
					object tag = _tabControl.SelectedTab.Tag;
					val = (IEditorPage)((tag is IEditorPage) ? tag : null);
				}
			}
			else
			{
				Control control = base.Controls[0];
				val = (IEditorPage)(object)((control is IEditorPage) ? control : null);
			}
			if (val != null)
			{
				object[] customAttributes = ((object)val).GetType().GetCustomAttributes(typeof(AssociatedOnlineHelpTopicAttribute), inherit: true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					return ((AssociatedOnlineHelpTopicAttribute)customAttributes[0]).Url;
				}
			}
			return null;
		}

		public IEnumerable<string> GetOnlineHelpUrls()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			LList<string> val = null;
			IEditorPage val2 = null;
			if (!_tabControl.IsDisposed)
			{
				if (_tabControl.SelectedTab != null)
				{
					object tag = _tabControl.SelectedTab.Tag;
					val2 = (IEditorPage)((tag is IEditorPage) ? tag : null);
				}
			}
			else
			{
				Control control = base.Controls[0];
				val2 = (IEditorPage)(object)((control is IEditorPage) ? control : null);
			}
			if (val2 != null)
			{
				object[] customAttributes = ((object)val2).GetType().GetCustomAttributes(typeof(AssociatedOnlineHelpTopicAttribute), inherit: true);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					val = new LList<string>();
					object[] array = customAttributes;
					foreach (object obj in array)
					{
						val.Add(((AssociatedOnlineHelpTopicAttribute)obj).Url);
					}
				}
			}
			return (IEnumerable<string>)val;
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			foreach (EditorInfo editor2 in _editorList)
			{
				IBaseDeviceEditor editor = editor2.Editor;
				IEditorBasedFindReplace val = (IEditorBasedFindReplace)(object)((editor is IEditorBasedFindReplace) ? editor : null);
				if (val != null && val.UndoableReplace(nPosition, nLength, stReplacement))
				{
					return true;
				}
			}
			return false;
		}

		private void _tabControl_DragOver(object sender, DragEventArgs e)
		{
			Point pt = _tabControl.PointToClient(new Point(e.X, e.Y));
			for (int i = 0; i < _tabControl.TabPages.Count; i++)
			{
				TabPage selectedTab = _tabControl.TabPages[i];
				if (_tabControl.GetTabRect(i).Contains(pt))
				{
					_tabControl.SelectedTab = selectedTab;
				}
			}
		}

		public bool GetSelectionForBrowserCommands(out string expression, out Guid objectGuid, out IPreCompileContext pcc)
		{
			expression = null;
			objectGuid = Guid.Empty;
			pcc = null;
			IEditorPage2 val = null;
			if (!_tabControl.IsDisposed)
			{
				TabPage selectedTab = _tabControl.SelectedTab;
				if (selectedTab != null)
				{
					object tag = selectedTab.Tag;
					val = (IEditorPage2)((tag is IEditorPage2) ? tag : null);
				}
			}
			else
			{
				Control control = base.Controls[0];
				val = (IEditorPage2)(object)((control is IEditorPage2) ? control : null);
			}
			if (val != null && val is IBrowserCommandsTarget)
			{
				return ((IBrowserCommandsTarget)((val is IBrowserCommandsTarget) ? val : null)).GetSelectionForBrowserCommands(out expression, out objectGuid, out pcc);
			}
			return false;
		}

		public bool GetSelectionForCrossReferenceCommand(out string expression, out Guid objectGuid)
		{
			expression = string.Empty;
			objectGuid = Guid.Empty;
			IEditorPage2 val = null;
			if (!_tabControl.IsDisposed)
			{
				TabPage selectedTab = _tabControl.SelectedTab;
				if (selectedTab != null)
				{
					object tag = selectedTab.Tag;
					val = (IEditorPage2)((tag is IEditorPage2) ? tag : null);
				}
			}
			else
			{
				Control control = base.Controls[0];
				val = (IEditorPage2)(object)((control is IEditorPage2) ? control : null);
			}
			if (val != null && val is IBrowserCommandsTarget)
			{
				return ((IBrowserCommandsTarget)((val is IBrowserCommandsTarget) ? val : null)).GetSelectionForCrossReferenceCommand(out expression, out objectGuid);
			}
			return false;
		}

		public void SetLocalizedObject(IMetaObject obj, bool bLocalizationActive)
		{
			_bLocalizationActive = bLocalizationActive;
			foreach (EditorInfo editor2 in _editorList)
			{
				IBaseDeviceEditor editor = editor2.Editor;
				ILocalizableEditor val = (ILocalizableEditor)(object)((editor is ILocalizableEditor) ? editor : null);
				if (val != null)
				{
					val.SetLocalizedObject(obj, bLocalizationActive);
				}
			}
		}

		public bool IsComment(long nPositionCombined, string stText)
		{
			foreach (EditorInfo editor2 in _editorList)
			{
				IBaseDeviceEditor editor = editor2.Editor;
				ILocalizableEditor val = (ILocalizableEditor)(object)((editor is ILocalizableEditor) ? editor : null);
				if (val != null && val.IsComment(nPositionCombined, stText))
				{
					return true;
				}
			}
			return false;
		}
	}
}
