#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Controls.Controls.Utilities;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{267A06F9-7C19-41af-9801-40D857E7E446}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	public class WatchListView : UserControl, IWatchListView9, IWatchListView8, IWatchListView7, IWatchListView6, IWatchListView5, IWatchListView4, IWatchListView3, IWatchListView2, IWatchListView, IView, IDisposable, IInputAssistantTarget, ICloseViewOnClose, INotifyOnVisibilityChanged, ILocalizableEditor
	{
		private Icon _smallIcon;

		private Icon _largeIcon;

		private string _stCaption;

		private DockingPosition _defaultDockingPosition;

		private DockingPosition _possibleDockingPositions;

		private Guid _persistenceGuid = Guid.Empty;

		private Guid _persistenceGuid2 = Guid.Empty;

		private WatchListModel _model;

		private Guid _guidApplication = Guid.Empty;

		private int _nFocusedRow = -1;

		private bool _bInEnableWatchListNodes;

		private bool _bInLoadSaveExpressions;

		private bool _bInUpdateColumnWidths;

		private bool _bIsLocalDragOperation;

		private bool _bLocalizationActive;

		private bool _bVisible;

		private bool _bVisibilityChangeNotificationEnabled;

		private const bool ONLINE = true;

		private const bool OFFLINE = false;

		private static string GVL_IOCONFIG_GLOBALS_MAPPING = "IoConfig_Globals_Mapping";

		private static readonly Guid GUID_FACTORY = new Guid("{6DAF8F8C-0A99-4f30-B4BA-67D3819A8AD2}");

		private static readonly Guid GUID_VIEW1 = new Guid("{2EFCE11C-EFB8-4282-B233-0644A415088D}");

		private static readonly Guid GUID_EDITCUT = new Guid("{586FB001-60CA-4dd1-A2F9-F9319EE13C95}");

		private static readonly Guid GUID_EDITCOPY = new Guid("{E76B8E0B-9247-41e7-93D5-80FE36AF9449}");

		private static readonly Guid GUID_EDITPASTE = new Guid("{73A7678B-2707-44f6-963B-8A4B3C05A331}");

		private static readonly Guid GUID_EDITDELETE = new Guid("{C5AAECF0-F55A-4864-871E-4584D1CCC9AF}");

		private static readonly Guid GUID_EDITSELECTALL = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		private static readonly Guid GUID_BREAKPOINTVIEWFACTORY = new Guid("{DF48A931-91FB-4e9e-B733-A80EEE0654BA}");

		private TreeTableView _treeTableView;

		private IContainer components;

		private bool _bInReplacePlaceholderNodes;

		public string InstancePath
		{
			get
			{
				return _model.InstancePath;
			}
			set
			{
				_model.InstancePath = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _model.ReadOnly;
			}
			set
			{
				_model.ReadOnly = value;
			}
		}

		public Guid PersistenceGuid
		{
			get
			{
				return _persistenceGuid;
			}
			set
			{
				_persistenceGuid = value;
			}
		}

		public Guid PersistenceGuid2
		{
			get
			{
				return _persistenceGuid2;
			}
			set
			{
				_persistenceGuid2 = value;
			}
		}

		public Guid ApplicationGuid
		{
			get
			{
				return _guidApplication;
			}
			set
			{
				_guidApplication = value;
			}
		}

		internal WatchListNode SelectedArrayNode
		{
			get
			{
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Invalid comparison between Unknown and I4
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Invalid comparison between Unknown and I4
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					if (_treeTableView.SelectedNodes != null && ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count == 1)
					{
						WatchListNode watchListNode = _treeTableView.GetModelNode(((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0]) as WatchListNode;
						if (watchListNode != null)
						{
							if (watchListNode.VarRef != null && watchListNode.VarRef.WatchExpression != null && (int)((IType)watchListNode.VarRef.WatchExpression.Type.DeRefType).Class == 26)
							{
								return watchListNode;
							}
							if (watchListNode.Parent is WatchListNode && ((WatchListNode)(object)watchListNode.Parent).VarRef != null && ((WatchListNode)(object)watchListNode.Parent).VarRef.WatchExpression != null && (int)((IType)((WatchListNode)(object)watchListNode.Parent).VarRef.WatchExpression.Type.DeRefType).Class == 26 && ((IArrayType)((WatchListNode)(object)watchListNode.Parent).VarRef.WatchExpression.Type.DeRefType).Dimensions.Length == 1)
							{
								return (WatchListNode)(object)watchListNode.Parent;
							}
						}
					}
				}
				catch
				{
				}
				return null;
			}
		}

		private bool CanCut
		{
			get
			{
				if (!_model.ReadOnly)
				{
					return GetSelectedNodes().Length != 0;
				}
				return false;
			}
		}

		private bool CanCopy => GetSelectedNodes().Length != 0;

		private bool CanPaste
		{
			get
			{
				if (_model.ReadOnly)
				{
					return false;
				}
				try
				{
					return Clipboard.GetDataObject()?.GetDataPresent(DataFormats.StringFormat) ?? false;
				}
				catch
				{
					return false;
				}
			}
		}

		private bool CanDelete
		{
			get
			{
				if (!_model.ReadOnly)
				{
					WatchListNode[] selectedNodes = GetSelectedNodes();
					for (int i = 0; i < selectedNodes.Length; i++)
					{
						if (selectedNodes[i].Parent == null)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		private bool CanSelectAll => _treeTableView.Nodes.Count > 1;

		public Control Control => this;

		public Control[] Panes => new Control[1] { (Control)(object)_treeTableView };

		public Icon LargeIcon => _largeIcon;

		public string Caption => _stCaption;

		public string Description => Caption;

		public DockingPosition DefaultDockingPosition => _defaultDockingPosition;

		public Icon SmallIcon => _smallIcon;

		public DockingPosition PossibleDockingPositions => _possibleDockingPositions;

		public bool IsEnabled
		{
			get
			{
				if (!_model.ReadOnly)
				{
					return ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count == 1;
				}
				return false;
			}
		}

		public IInputAssistantArgumentsFormatter ActionArgumentsFormatter => APEnvironment.STActionArgumentsFormatter;

		public IInputAssistantArgumentsFormatter FunctionArgumentsFormatter => APEnvironment.STFunctionArgumentsFormatter;

		public IInputAssistantArgumentsFormatter FunctionBlockArgumentsFormatter => APEnvironment.STFunctionBlockArgumentsFormatter;

		public IInputAssistantArgumentsFormatter MethodArgumentsFormatter => APEnvironment.STMethodArgumentsFormatter;

		public IInputAssistantArgumentsFormatter ProgramArgumentsFormatter => APEnvironment.STProgramArgumentsFormatter;

		public WatchListView()
		{
			InitializeComponent();
			_model = new WatchListModel(bShowCommentColumn: true, bShowWatchpointColumn: false, bIsForceListView: false);
			_treeTableView.Model=((ITreeTableModel)(object)_model);
			_model.View = _treeTableView;
		}

		public WatchListView(bool bShowWatchpointColumn, bool bIsForceListView)
		{
			InitializeComponent();
			_model = new WatchListModel(bShowCommentColumn: true, bShowWatchpointColumn, bIsForceListView);
			_treeTableView.Model=((ITreeTableModel)(object)_model);
			_model.View = _treeTableView;
		}

		public override bool Equals(object obj)
		{
			WatchListView watchListView = obj as WatchListView;
			if (watchListView != null)
			{
				return _stCaption == watchListView._stCaption;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (_stCaption == null)
			{
				return 0;
			}
			return _stCaption.GetHashCode();
		}

		public void SetAppearance(Icon smallIcon, Icon largeIcon, string stCaption, DockingPosition defaultDockingPosition, DockingPosition possibleDockingPositions, BorderStyle borderStyle)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			_smallIcon = smallIcon;
			_largeIcon = largeIcon;
			_stCaption = stCaption;
			_defaultDockingPosition = defaultDockingPosition;
			_possibleDockingPositions = possibleDockingPositions;
			_treeTableView.BorderStyle=(borderStyle);
		}

		public void SetObject(int nProjectHandle, Guid ObjectGuid)
		{
			_model.SetObject(nProjectHandle, ObjectGuid);
		}

		public void Refill()
		{
			_model.LoadActiveWatchPoints = PersistenceGuid == GUID_VIEW1;
			_model.LocalizationActive = _bLocalizationActive;
			ITreeTableModel model = _treeTableView.Model;
			try
			{
				_treeTableView.Model=((ITreeTableModel)null);
				_model.Refill(_guidApplication);
			}
			finally
			{
				_treeTableView.Model=(model);
				UpdateColumnWidths(bInitial: false);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
				_model.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			base.OnHandleCreated(e);
			UpdateColumnWidths(bInitial: true);
			LoadExpressions();
			APEnvironment.OptionStorage.OptionChanged+=(new OptionEventHandler(OnOptionChanged));
			APEnvironment.OptionStorage.OptionDeleted+=(new OptionEventHandler(OnOptionDeleted));
			_model.ExpressionInserted += OnExpressionInserted;
			_model.ExpressionChanged += OnExpressionChanged;
			_model.ExpressionRemoved += OnExpressionRemoved;
			_model.AllExpressionsChanged += OnAllExpressionsChanged;
		}

		private void OnTreeTableViewColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (_model == null || !ReadOnly)
			{
				return;
			}
			_treeTableView.StoreExpandedNodes();
			_treeTableView.BeginUpdate();
			_ = e.Column;
			try
			{
				if (e.Column == _model.SortColumn)
				{
					if (_model.SortOrder == SortOrder.Ascending)
					{
						_model.Sort(e.Column, SortOrder.Descending);
					}
					else
					{
						_model.Sort(e.Column, SortOrder.Ascending);
					}
					_treeTableView.SetColumnHeaderSortOrderIcon(e.Column, _model.SortOrder);
				}
				else
				{
					_treeTableView.SetColumnHeaderSortOrderIcon(_model.SortColumn, SortOrder.None);
					_model.Sort(e.Column, SortOrder.Ascending);
					_treeTableView.SetColumnHeaderSortOrderIcon(e.Column, _model.SortOrder);
				}
			}
			finally
			{
				_treeTableView.EndUpdate();
				_treeTableView.RestoreExpandedNodes();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_treeTableView.HeaderStyle=((!ReadOnly) ? ColumnHeaderStyle.Nonclickable : ColumnHeaderStyle.Clickable);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			_model.UnregisterEvents();
			_model.ExpressionInserted -= OnExpressionInserted;
			_model.ExpressionChanged -= OnExpressionChanged;
			_model.ExpressionRemoved -= OnExpressionRemoved;
			_model.AllExpressionsChanged -= OnAllExpressionsChanged;
			_model.Dispose();
			APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
			APEnvironment.OptionStorage.OptionDeleted-=(new OptionEventHandler(OnOptionDeleted));
			base.OnHandleDestroyed(e);
		}

		private void UpdateColumnWidths(bool bInitial)
		{
			_bInUpdateColumnWidths = true;
			if (base.IsHandleCreated)
			{
				if (bInitial)
				{
					_treeTableView.Columns[1].Width = 120;
					_treeTableView.Columns[2].Width = 100;
					_treeTableView.Columns[3].Width = 120;
					_treeTableView.Columns[4].Width = 100;
					if (_model.ShowCommentColumn)
					{
						_treeTableView.Columns[5].Width = 240;
					}
				}
				if (_model.ShowCommentColumn)
				{
					_treeTableView.Columns[0].Width = ((Control)(object)_treeTableView).Width - SystemInformation.VerticalScrollBarWidth - 2 * SystemInformation.Border3DSize.Width - _treeTableView.Columns[1].Width - _treeTableView.Columns[2].Width - _treeTableView.Columns[3].Width - _treeTableView.Columns[4].Width - _treeTableView.Columns[5].Width;
				}
				else
				{
					_treeTableView.Columns[0].Width = ((Control)(object)_treeTableView).Width - SystemInformation.VerticalScrollBarWidth - 2 * SystemInformation.Border3DSize.Width - _treeTableView.Columns[1].Width - _treeTableView.Columns[2].Width - _treeTableView.Columns[3].Width - _treeTableView.Columns[4].Width;
				}
				if (_treeTableView.Columns[0].Width < 120)
				{
					_treeTableView.Columns[0].Width = 120;
				}
				_ = _persistenceGuid2;
				if (_persistenceGuid2 != Guid.Empty)
				{
					int[] array = GlobalOptionsHelper.LoadColumnWidths(_persistenceGuid2);
					if (array != null && array.Length != 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (i < _treeTableView.Columns.Count)
							{
								_treeTableView.Columns[i].Width = array[i];
							}
						}
					}
				}
			}
			_bInUpdateColumnWidths = false;
		}

		private void LoadExpressions()
		{
			if (_persistenceGuid == Guid.Empty || _bInLoadSaveExpressions)
			{
				return;
			}
			LocalOptionsHelper.LoadWatchExpressions(_persistenceGuid, out var watchExpressions, out var expandedExpressions);
			string text = _model.Save();
			try
			{
				_bInLoadSaveExpressions = true;
				if (watchExpressions != text)
				{
					StoreSelection();
					_model.Load(watchExpressions);
				}
				if (expandedExpressions != null)
				{
					HashSet<string> hashSet = new HashSet<string>(expandedExpressions.Split('\r', '\n'));
					ICollection<ExpandedModelNodeDescription> collection = NodeExpansionHelper.Instance.CreateExpandedModelNodeDescriptionInstances((IEnumerable<string>)hashSet);
					NodeExpansionHelper.Instance.ExpandNodes(_treeTableView, collection);
				}
			}
			finally
			{
				RestoreSelection();
				_bInLoadSaveExpressions = false;
			}
		}

		private void SaveExpressions(bool calledFromExpandOrCollapseEvent)
		{
			if (_persistenceGuid == Guid.Empty || _bInLoadSaveExpressions)
			{
				return;
			}
			try
			{
				_bInLoadSaveExpressions = true;
				string text = _model.Save();
				string expandedExpressions = string.Empty;
				IEnumerable<string> expandedNodes = NodeExpansionHelper.Instance.GetExpandedNodes(_treeTableView);
				if (expandedNodes != null)
				{
					expandedExpressions = string.Join(Environment.NewLine, expandedNodes);
				}
				if (text != null)
				{
					LocalOptionsHelper.SaveWatchExpressions(_persistenceGuid, text, expandedExpressions, calledFromExpandOrCollapseEvent);
				}
			}
			finally
			{
				_bInLoadSaveExpressions = false;
			}
		}

		private void RestoreSelection()
		{
			int nFocusedRow = _nFocusedRow;
			TreeTableViewNode[] array = (TreeTableViewNode[])(object)new TreeTableViewNode[((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count];
			((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).CopyTo((Array)array, 0);
			TreeTableViewNode[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Selected=(false);
			}
			if (nFocusedRow >= 0)
			{
				nFocusedRow = Math.Min(nFocusedRow, _treeTableView.VisibleRowCount - 1);
				TreeTableViewNode nodeAtRow = _treeTableView.GetNodeAtRow(nFocusedRow);
				nodeAtRow.Selected=(true);
				nodeAtRow.Focus(0);
			}
		}

		private void StoreSelection()
		{
			TreeTableViewNode focusedNode = _treeTableView.FocusedNode;
			_nFocusedRow = ((focusedNode != null) ? _treeTableView.GetRowOfNode(focusedNode) : (-1));
		}

		private void EnableWatchListNodes()
		{
			if (_bInEnableWatchListNodes || base.IsDisposed)
			{
				return;
			}
			try
			{
				_bInEnableWatchListNodes = true;
				if (_bVisible || !_bVisibilityChangeNotificationEnabled)
				{
					TreeTableViewNode val = _treeTableView.TopNode;
					ArrayList arrayList = new ArrayList();
					while (val != null && val.Displayed)
					{
						WatchListNode watchListNode = _treeTableView.GetModelNode(val) as WatchListNode;
						if (watchListNode != null)
						{
							arrayList.Add(watchListNode);
						}
						val = val.NextVisibleNode;
					}
					WatchListNode[] array = new WatchListNode[arrayList.Count];
					arrayList.CopyTo(array);
					_model.EnableMonitoring(array);
				}
				else
				{
					_model.EnableMonitoring(new WatchListNode[0]);
				}
			}
			finally
			{
				_bInEnableWatchListNodes = false;
			}
		}

		private void ReplacePlaceholderNodes()
		{
			if (_bInReplacePlaceholderNodes || base.IsDisposed)
			{
				return;
			}
			try
			{
				_bInReplacePlaceholderNodes = true;
				TreeTableViewNode val = _treeTableView.TopNode;
				WatchListModel watchListModel = _treeTableView.Model as WatchListModel;
				if (watchListModel != null)
				{
					ITreeTableNode modelNode = _treeTableView.GetModelNode(val);
					ITreeTableNode lastVisibleModelNode = null;
					while (val != null && val.Displayed)
					{
						lastVisibleModelNode = _treeTableView.GetModelNode(val);
						val = val.NextVisibleNode;
					}
					watchListModel.ReplacePlaceholderNodes(modelNode, lastVisibleModelNode);
				}
			}
			finally
			{
				_bInReplacePlaceholderNodes = false;
			}
		}

		private void InitializeComponent()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Expected O, but got Unknown
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Expected O, but got Unknown
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.WatchList.WatchListView));
			_treeTableView = new TreeTableView();
			SuspendLayout();
			_treeTableView.AllowColumnReorder=(false);
			_treeTableView.AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_treeTableView).BackColor = System.Drawing.SystemColors.Window;
			_treeTableView.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			componentResourceManager.ApplyResources(_treeTableView, "_treeTableView");
			_treeTableView.DoNotShrinkColumnsAutomatically=(false);
			_treeTableView.ForceFocusOnClick=(false);
			_treeTableView.GridLines=(true);
			_treeTableView.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Nonclickable);
			_treeTableView.ImmediateEdit=(true);
			_treeTableView.Indent=(20);
			_treeTableView.KeepColumnWidthsAdjusted=(false);
			_treeTableView.Model=((ITreeTableModel)null);
			_treeTableView.MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_treeTableView).Name = "_treeTableView";
			_treeTableView.NoSearchStrings=(false);
			_treeTableView.OnlyWhenFocused=(false);
			_treeTableView.OpenEditOnDblClk=(true);
			_treeTableView.ReadOnly=(false);
			_treeTableView.Scrollable=(true);
			_treeTableView.ShowLines=(false);
			_treeTableView.ShowPlusMinus=(true);
			_treeTableView.ShowRootLines=(true);
			_treeTableView.ToggleOnDblClk=(false);
			_treeTableView.ColumnClick+=(new System.Windows.Forms.ColumnClickEventHandler(OnTreeTableViewColumnClick));
			_treeTableView.NodeDrag+=(new TreeTableViewDragEventHandler(OnTreeTableViewNodeDrag));
			_treeTableView.AfterCollapse+=(new TreeTableViewEventHandler(OnTreeTableViewAfterCollapse));
			_treeTableView.AfterExpand+=(new TreeTableViewEventHandler(OnTreeTableViewAfterExpand));
			_treeTableView.Scroll+=(new System.EventHandler(OnTreeTableViewScroll));
			_treeTableView.ColumnWidthChanged+=(new System.EventHandler(OnTreeTableViewColumnWidthChanged));
			((System.Windows.Forms.Control)(object)_treeTableView).SizeChanged += new System.EventHandler(OnTreeTableViewSizeChanged);
			((System.Windows.Forms.Control)(object)_treeTableView).DragDrop += new System.Windows.Forms.DragEventHandler(OnDragDrop);
			((System.Windows.Forms.Control)(object)_treeTableView).DragEnter += new System.Windows.Forms.DragEventHandler(OnDragEnter);
			((System.Windows.Forms.Control)(object)_treeTableView).QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(OnQueryContinueDrag);
			((System.Windows.Forms.Control)(object)_treeTableView).DoubleClick += new System.EventHandler(OnDoubleClick);
			((System.Windows.Forms.Control)(object)_treeTableView).KeyDown += new System.Windows.Forms.KeyEventHandler(OnKeyDown);
			((System.Windows.Forms.Control)(object)_treeTableView).MouseUp += new System.Windows.Forms.MouseEventHandler(OnTreeTableViewMouseUp);
			base.Controls.Add((System.Windows.Forms.Control)(object)_treeTableView);
			componentResourceManager.ApplyResources(this, "$this");
			base.Name = "WatchListView";
			ResumeLayout(false);
		}

		internal WatchListNode[] GetSelectedNodes()
		{
			List<WatchListNode> list = new List<WatchListNode>();
			for (int i = 0; i < ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count; i++)
			{
				TreeTableViewNode val = ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[i];
				WatchListNode watchListNode = _treeTableView.GetModelNode(val) as WatchListNode;
				if (watchListNode != null)
				{
					list.Add(watchListNode);
				}
			}
			return list.ToArray();
		}

		internal ExecutionPointWarningNode GetSelectedWarningNode()
		{
			for (int i = 0; i < ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count; i++)
			{
				TreeTableViewNode val = ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[i];
				ExecutionPointWarningNode executionPointWarningNode = _treeTableView.GetModelNode(val) as ExecutionPointWarningNode;
				if (executionPointWarningNode != null)
				{
					return executionPointWarningNode;
				}
			}
			return null;
		}

		private void Cut()
		{
			Copy();
			Delete();
		}

		private void Copy()
		{
			string text = string.Empty;
			WatchListNode[] selectedNodes = GetSelectedNodes();
			if (selectedNodes.Length > 1)
			{
				WatchListNode[] array = selectedNodes;
				foreach (WatchListNode watchListNode in array)
				{
					if (CastExpressionFormatter.Instance.IsCastExpression(watchListNode.Expression))
					{
						int cOLUMN_EXPRESSION = ((WatchListModel)(object)_treeTableView.Model).COLUMN_EXPRESSION;
						TreeTableViewNode viewNode = _treeTableView.GetViewNode((ITreeTableNode)(object)watchListNode);
						text = text + _treeTableView.Model.GetColumnRenderer(cOLUMN_EXPRESSION).GetStringRepresentation(viewNode, cOLUMN_EXPRESSION) + Environment.NewLine;
					}
					else
					{
						text = text + CastExpressionFormatter.Instance.GetCastExpressionDisplayString(watchListNode.Expression) + Environment.NewLine;
					}
				}
			}
			else
			{
				int num = default(int);
				TreeTableViewNode focusedNode = _treeTableView.GetFocusedNode(out num);
				if (focusedNode != null)
				{
					try
					{
						if (num == ((WatchListModel)(object)_treeTableView.Model).COLUMN_EXPRESSION || num == ((WatchListModel)(object)_treeTableView.Model).COLUMN_APPLICATION_PREFIX)
						{
							WatchListNode watchListNode2 = _treeTableView.GetModelNode(focusedNode) as WatchListNode;
							if (watchListNode2 != null)
							{
								text = ((!CastExpressionFormatter.Instance.IsCastExpression(watchListNode2.Expression)) ? (watchListNode2.Expression + Environment.NewLine) : _treeTableView.Model.GetColumnRenderer(num).GetStringRepresentation(focusedNode, num));
							}
						}
						else
						{
							text = _treeTableView.Model.GetColumnRenderer(num).GetStringRepresentation(focusedNode, num);
						}
					}
					catch
					{
					}
				}
			}
			Clipboard.SetDataObject(text, copy: true);
		}

		private void Paste()
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return;
			}
			try
			{
				_treeTableView.BeginUpdate();
				_model.BeginUpdate();
				TreeTableViewNode val = ((((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count > 0) ? ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0] : null);
				if (val != null)
				{
					while (val.Parent != null)
					{
						val = val.Parent;
					}
				}
				int num = ((val != null) ? val.Index : _treeTableView.Nodes.Count);
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject != null && dataObject.GetDataPresent(DataFormats.StringFormat))
				{
					string obj = dataObject.GetData(DataFormats.StringFormat) as string;
					Debug.Assert(obj != null);
					TextReader textReader = new StringReader(obj);
					for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
					{
						_model.Insert(num++, text);
					}
				}
			}
			catch
			{
			}
			finally
			{
				_model.EndUpdate();
				_treeTableView.EndUpdate();
			}
		}

		private void Delete()
		{
			try
			{
				_treeTableView.BeginUpdate();
				_model.BeginUpdate();
				WatchListNode[] selectedNodes = GetSelectedNodes();
				foreach (WatchListNode watchListNode in selectedNodes)
				{
					if (watchListNode.Parent == null)
					{
						_model.Remove(watchListNode);
					}
				}
				if (GetSelectedNodes().Length == 0 && _treeTableView.Nodes.Count > 0)
				{
					int num = _treeTableView.Nodes.Count - 1;
					_treeTableView.Nodes[num].Selected=(true);
					_treeTableView.Nodes[num].Focus(0);
				}
			}
			finally
			{
				_model.EndUpdate();
				_treeTableView.EndUpdate();
			}
		}

		private void SelectAll()
		{
			try
			{
				_treeTableView.BeginUpdate();
				for (TreeTableViewNode val = ((_treeTableView.Nodes.Count > 0) ? _treeTableView.Nodes[0] : null); val != null; val = val.NextVisibleNode)
				{
					if (val.NextVisibleNode != null)
					{
						val.Selected=(true);
					}
				}
			}
			finally
			{
				_treeTableView.EndUpdate();
			}
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITCUT)
			{
				return CanCut;
			}
			if (commandGuid == GUID_EDITCOPY)
			{
				return CanCopy;
			}
			if (commandGuid == GUID_EDITPASTE)
			{
				return CanPaste;
			}
			if (commandGuid == GUID_EDITDELETE)
			{
				return CanDelete;
			}
			if (commandGuid == GUID_EDITSELECTALL)
			{
				return CanSelectAll;
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITCUT)
			{
				Cut();
			}
			else if (commandGuid == GUID_EDITCOPY)
			{
				Copy();
			}
			else if (commandGuid == GUID_EDITPASTE)
			{
				Paste();
			}
			else if (commandGuid == GUID_EDITDELETE)
			{
				Delete();
			}
			else if (commandGuid == GUID_EDITSELECTALL)
			{
				SelectAll();
			}
		}

		private void OnTreeTableViewSizeChanged(object sender, EventArgs e)
		{
			ReplacePlaceholderNodes();
			UpdateColumnWidths(bInitial: false);
			EnableWatchListNodes();
		}

		public IInputAssistantCategory[] GetInputAssistantCategories()
		{
			IInputAssistantCategory val = APEnvironment.CreateWatchVariablesInputAssistantCategory();
			Debug.Assert(val != null);
			return (IInputAssistantCategory[])(object)new IInputAssistantCategory[1] { val };
		}

		public void InputAssistantCompleted(string stText)
		{
			Debug.Assert(((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count == 1);
			TreeTableViewNode val = ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0];
			WatchListNode watchListNode = _treeTableView.GetModelNode(val) as WatchListNode;
			if (watchListNode != null)
			{
				IPreCompileUtilities preCompileUtils = ((ILanguageModelUtilities)APEnvironment.LanguageModelUtilities).PreCompileUtils;
				IPreCompileUtilities obj = ((preCompileUtils is IPreCompileUtilities7) ? preCompileUtils : null);
				Debug.Assert(obj != null);
				stText = ((IPreCompileUtilities7)obj).AddDeviceApplicationPrefix(stText);
				watchListNode.SetValue(0, new ExpressionData(string.Empty, stText, null));
			}
		}

		private void OnTreeTableViewColumnWidthChanged(object sender, EventArgs e)
		{
			_ = _persistenceGuid2;
			if (!(_persistenceGuid2 == Guid.Empty) && !_bInUpdateColumnWidths)
			{
				int[] array = new int[_treeTableView.Columns.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _treeTableView.Columns[i].Width;
				}
				GlobalOptionsHelper.SaveColumnWidths(_persistenceGuid2, array);
			}
		}

		private void OnTreeTableViewScroll(object sender, EventArgs e)
		{
			ReplacePlaceholderNodes();
			EnableWatchListNodes();
		}

		private void OnTreeTableViewAfterCollapse(object sender, TreeTableViewEventArgs e)
		{
			ReplacePlaceholderNodes();
			SaveExpressions(calledFromExpandOrCollapseEvent: true);
			EnableWatchListNodes();
		}

		private void OnTreeTableViewAfterExpand(object sender, TreeTableViewEventArgs e)
		{
			SaveExpressions(calledFromExpandOrCollapseEvent: true);
			EnableWatchListNodes();
		}

		private void OnOptionChanged(object sender, OptionEventArgs e)
		{
			if (e.OptionKey.Name == _persistenceGuid.ToString())
			{
				LoadExpressions();
			}
		}

		private void OnOptionDeleted(object sender, OptionEventArgs e)
		{
			if (e.OptionKey.Name == _persistenceGuid.ToString())
			{
				LoadExpressions();
			}
		}

		private void OnExpressionInserted(object sender, EventArgs e)
		{
			SaveExpressions(calledFromExpandOrCollapseEvent: false);
			EnableWatchListNodes();
		}

		private void OnExpressionChanged(object sender, EventArgs e)
		{
			SaveExpressions(calledFromExpandOrCollapseEvent: false);
		}

		private void OnExpressionRemoved(object sender, EventArgs e)
		{
			SaveExpressions(calledFromExpandOrCollapseEvent: false);
			EnableWatchListNodes();
		}

		private void OnAllExpressionsChanged(object sender, EventArgs e)
		{
			SaveExpressions(calledFromExpandOrCollapseEvent: false);
			EnableWatchListNodes();
		}

		public string[] GetExpressions()
		{
			return _model.GetExpressions();
		}

		public void InsertExpression(int nIndex, string stExpression)
		{
			_model.Insert(nIndex, stExpression);
		}

		public void RemoveExpressionAt(int nIndex)
		{
			_model.Remove(nIndex);
		}

		private void OnTreeTableViewMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && ((IEngine)APEnvironment.Engine).Frame != null)
			{
				((IEngine)APEnvironment.Engine).Frame.DisplayContextMenu(Guid.Empty, (Guid[])null, (ContextMenuFilterCallback)null, (Control)this, new Point(e.X, e.Y));
			}
		}

		public void DisableTreeTableViewColumns()
		{
			((Control)(object)_treeTableView).Enabled = false;
		}

		public void EmptyList()
		{
			for (int num = _treeTableView.GetNodeCount(true) - 1; num > 0; num--)
			{
				RemoveExpressionAt(num);
			}
		}

		public IOnlineApplication6[] GetAllOnlineApplications()
		{
			return GetAllApplications(bGetOnline: true);
		}

		public IOnlineApplication6[] GetAllApplications(bool bGetOnline)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			ArrayList arrayList = new ArrayList();
			if (primaryProject != null)
			{
				Guid[] allObjects = APEnvironment.ObjectMgr.GetAllObjects(primaryProject.Handle);
				Debug.Assert(allObjects != null);
				Guid[] array = allObjects;
				foreach (Guid guid in array)
				{
					try
					{
						IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(primaryProject.Handle, guid);
						Debug.Assert(metaObjectStub != null);
						if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guid);
							IOnlineApplication6 val = (IOnlineApplication6)(object)((application is IOnlineApplication6) ? application : null);
							if (val != null && ((((IOnlineApplication)val).IsLoggedIn && bGetOnline) || (!((IOnlineApplication)val).IsLoggedIn && !bGetOnline)))
							{
								arrayList.Add(val);
							}
						}
					}
					catch
					{
						return null;
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return null;
			}
			IOnlineApplication6[] array2 = (IOnlineApplication6[])(object)new IOnlineApplication6[arrayList.Count];
			arrayList.CopyTo(array2);
			return array2;
		}

		public void UnloadAllVariablesFromOfflineApps()
		{
			IOnlineApplication[] allApplications = (IOnlineApplication[])(object)GetAllApplications(bGetOnline: false);
			if (allApplications == null || allApplications.Length == 0)
			{
				return;
			}
			string[] array = new string[allApplications.Length];
			for (int i = 0; i < allApplications.Length; i++)
			{
				array[i] = Common.GetApplicationName(allApplications[i].ApplicationGuid);
			}
			for (int num = _treeTableView.Nodes.Count - 1; num >= 0; num--)
			{
				TreeTableViewNode nodeAtRow = _treeTableView.GetNodeAtRow(num);
				WatchListNode watchListNode = _treeTableView.GetModelNode(nodeAtRow) as WatchListNode;
				string[] array2 = array;
				foreach (string value in array2)
				{
					if (watchListNode != null && watchListNode.Expression.Contains(value))
					{
						RemoveExpressionAt(num);
					}
				}
			}
		}

		public void UnloadAllReleasedVariables()
		{
			IWatchListView3 forceListView = getForceListView(GUID_FACTORY);
			if (forceListView == null)
			{
				return;
			}
			IOnlineVarRef val = null;
			IVarRef val2 = null;
			TreeTableViewNode val3 = null;
			string[] expressions = ((IWatchListView2)forceListView).GetExpressions();
			foreach (string text in expressions)
			{
				val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(text);
				val = ((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(val2);
				if (val == null || val.Forced)
				{
					continue;
				}
				for (int num = _treeTableView.Nodes.Count - 1; num >= 0; num--)
				{
					val3 = _treeTableView.GetNodeAtRow(num);
					WatchListNode watchListNode = _treeTableView.GetModelNode(val3) as WatchListNode;
					if (watchListNode != null && watchListNode.Expression.Equals(text))
					{
						RemoveExpressionAt(num);
					}
				}
			}
		}

		public void UnforceAllSelectedVariables(bool bRestore)
		{
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			WatchListNode[] selectedNodes = GetSelectedNodes();
			IList<IOnlineVarRef> list = (IList<IOnlineVarRef>)new LList<IOnlineVarRef>();
			WatchListNode[] array = selectedNodes;
			foreach (WatchListNode watchListNode in array)
			{
				if (watchListNode.Expression != "")
				{
					IVarRef varReference = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(watchListNode.Expression);
					if (varReference != null)
					{
						list.Add(((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(varReference));
					}
				}
			}
			foreach (IOnlineVarRef item in list)
			{
				if (bRestore)
				{
					item.PreparedValue=(PreparedValues.UnforceAndRestore);
				}
				else
				{
					item.PreparedValue=(PreparedValues.Unforce);
				}
			}
			IOnlineApplication6[] allApplications = GetAllApplications(bGetOnline: true);
			if (allApplications == null)
			{
				return;
			}
			IOnlineApplication6[] array2 = allApplications;
			foreach (IOnlineApplication6 val in array2)
			{
				IOnlineVarRef[] forcedVarRefs = ((IOnlineApplication5)val).GetForcedVarRefs();
				LList<IOnlineVarRef> val2 = new LList<IOnlineVarRef>();
				IOnlineVarRef[] array3 = forcedVarRefs;
				foreach (IOnlineVarRef val3 in array3)
				{
					foreach (IOnlineVarRef item2 in list)
					{
						if (((object)item2.Expression).Equals((object)val3.Expression))
						{
							val2.Add(item2);
						}
						else if (item2 is IOnlineVarRef6 && val is IOnlineApplication16 && item2.Expression is IAddressExpression && ((IAddressExpression)item2.Expression).DirectAddress != null)
						{
							IMonitoringExpression monitoringExpression = (IMonitoringExpression)(object)((IOnlineVarRef6)item2).GetMonitoringExpression();
							if (monitoringExpression != null && monitoringExpression.VarRef != null && monitoringExpression.VarRef.AddressInfo is IAbsoluteAddressInfo && ((IOnlineApplication16)val).FindIoVariableForDirectAddress(monitoringExpression.VarRef.AddressInfo) == ((IExprement)val3.Expression).ToString())
							{
								val2.Add(item2);
							}
						}
						else
						{
							if (!(item2 is IOnlineVarRef6) || !(item2.Expression is IVariableExpression2))
							{
								continue;
							}
							IMonitoringExpression3 monitoringExpression2 = ((IOnlineVarRef6)item2).GetMonitoringExpression();
							IMonitoringExpression3 monitoringExpression3 = ((IOnlineVarRef6)val3).GetMonitoringExpression();
							if (monitoringExpression2 != null && monitoringExpression3 != null && ((IMonitoringExpression)monitoringExpression2).VarRef != null && ((IMonitoringExpression)monitoringExpression3).VarRef != null)
							{
								IAddressInfo addressInfo = ((IMonitoringExpression)monitoringExpression2).VarRef.AddressInfo;
								IAbsoluteAddressInfo val4 = (IAbsoluteAddressInfo)(object)((addressInfo is IAbsoluteAddressInfo) ? addressInfo : null);
								IAddressInfo addressInfo2 = ((IMonitoringExpression)monitoringExpression3).VarRef.AddressInfo;
								IAbsoluteAddressInfo val5 = (IAbsoluteAddressInfo)(object)((addressInfo2 is IAbsoluteAddressInfo) ? addressInfo2 : null);
								if (val4.Area == val5.Area && val4.BitOffset == val5.BitOffset && val4.Offset == val5.Offset && ((IAddressInfo)val4).Size == ((IAddressInfo)val5).Size)
								{
									val2.Add(item2);
								}
							}
						}
					}
				}
				if (val != null)
				{
					((IOnlineApplication)val).ForceVariables(val2.ToArray());
				}
			}
		}

		public void LoadAllForcedExpressions()
		{
			UpdateAllForcedExpressions(Guid.Empty, Guid.Empty);
		}

		public void UpdateAllForcedExpressions(Guid singleAppGuid, Guid viewGuid)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			IWatchListView3 forceListView = getForceListView(viewGuid);
			if (forceListView == null)
			{
				return;
			}
			IOnlineApplication[] array = null;
			IOnlineVarRef[] forcedVarArray = null;
			IOnlineVarRef[] preparedVarArray = null;
			LList<IOnlineApplication> val = new LList<IOnlineApplication>();
			if (singleAppGuid == Guid.Empty)
			{
				array = (IOnlineApplication[])(object)GetAllApplications(bGetOnline: true);
			}
			else
			{
				val.Add(((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(singleAppGuid));
				array = val.ToArray();
			}
			int num = ((IWatchListView2)forceListView).GetExpressions().Count();
			if (array == null)
			{
				return;
			}
			IOnlineApplication[] array2 = array;
			foreach (IOnlineApplication val2 in array2)
			{
				if (val2 is IOnlineApplication6)
				{
					forcedVarArray = ((IOnlineApplication5)(IOnlineApplication6)val2).GetForcedVarRefs();
					preparedVarArray = ((IOnlineApplication)(IOnlineApplication6)val2).PreparedVarRefs;
				}
				LList<string> expressionsToInsert;
				Dictionary<string, string> allForcedVarRefs = GetAllForcedVarRefs(forceListView, val2, forcedVarArray, out expressionsToInsert);
				GetAllPreparedVarRefs(forceListView, allForcedVarRefs, (IReadOnlyCollection<IOnlineVarRef>)(object)preparedVarArray, (IList<string>)expressionsToInsert);
				expressionsToInsert.Sort();
				foreach (string item in expressionsToInsert)
				{
					if (TryInsertExpression((IWatchListView2)(object)forceListView, item, num))
					{
						num++;
					}
				}
			}
			_model.RemoveExecutionPointWarning();
			array2 = array;
			foreach (IOnlineApplication val3 in array2)
			{
				if (val3 == null || !val3.IsLoggedIn)
				{
					continue;
				}
				bool flag = false;
				IBP[] breakpoints = ((IBPManager)APEnvironment.BPMgr).GetBreakpoints(val3.ApplicationGuid);
				foreach (IBP val4 in breakpoints)
				{
					if (val4.IsEnabled && val4 is IBP4 && ((IBP2)((val4 is IBP4) ? val4 : null)).Watchpoint)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					_model.InsertExecutionPointWarning();
					break;
				}
			}
			((Control)(object)_treeTableView).Enabled = true;
		}

		private static void GetAllPreparedVarRefs(IWatchListView3 wlv, IDictionary<string, string> existingExpressions, IReadOnlyCollection<IOnlineVarRef> preparedVarArray, IList<string> expressionsToInsert)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			string[] expressions = ((IWatchListView2)wlv).GetExpressions();
			foreach (string text in expressions)
			{
				existingExpressions[text] = text;
			}
			if (preparedVarArray == null || preparedVarArray.Count <= 0)
			{
				return;
			}
			foreach (IOnlineVarRef item in preparedVarArray)
			{
				string text2 = ((!(item is IOnlineVarRef5) || !(((IOnlineVarRef5)item).WatchListExpression != string.Empty)) ? ((IMonitoringExpression)((IOnlineVarRef6)item).GetMonitoringExpression()).VarRef.GetQualifiedPath() : ((IOnlineVarRef5)item).WatchListExpression);
				if (!existingExpressions.ContainsKey(text2) && !expressionsToInsert.Contains(text2))
				{
					expressionsToInsert.Add(text2);
				}
			}
		}

		private Dictionary<string, string> GetAllForcedVarRefs(IWatchListView3 wlv, IOnlineApplication onlineApp, IOnlineVarRef[] forcedVarArray, out LList<string> expressionsToInsert)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] expressions = ((IWatchListView2)wlv).GetExpressions();
			foreach (string text in expressions)
			{
				dictionary[text] = text;
			}
			expressionsToInsert = new LList<string>();
			string applicationName = Common.GetApplicationName(onlineApp.ApplicationGuid);
			foreach (IOnlineVarRef val in forcedVarArray)
			{
				string text2 = string.Empty;
				if (!(val is IOnlineVarRef6) || !(val.Expression is ICompoAccessExpression))
				{
					text2 = ((!(val is IOnlineVarRef5) || !(((IOnlineVarRef5)val).WatchListExpression != string.Empty)) ? (applicationName + "." + ((IExprement)val.Expression).ToString()) : ((IOnlineVarRef5)val).WatchListExpression);
				}
				else
				{
					ICompoAccessExpression val2 = (ICompoAccessExpression)val.Expression;
					try
					{
						if (val2.Left is IVariableExpression && ((IVariableExpression)val2.Left).Name == GVL_IOCONFIG_GLOBALS_MAPPING)
						{
							int signatureId = ((IVariableExpression)val2.Left).SignatureId;
							if (signatureId > 0)
							{
								ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(onlineApp.ApplicationGuid);
								if (referenceContextIfAvailable != null)
								{
									IVariable val3 = referenceContextIfAvailable.GetSignatureById(signatureId)[((IVariableExpression)(IVariableExpression2)val2.Right).VariableId];
									if (val3 != null && val3.Attributes != null)
									{
										expressions = val3.Attributes;
										for (int j = 0; j < expressions.Length; j++)
										{
											if (expressions[j].ToUpperInvariant() == "HIDE")
											{
												text2 = applicationName + "." + ((object)val3.Address).ToString();
												break;
											}
										}
									}
								}
							}
						}
					}
					catch
					{
					}
					if (string.IsNullOrEmpty(text2))
					{
						text2 = ((IOnlineVarRef5)val).WatchListExpression;
					}
				}
				if (text2.Contains(GVL_IOCONFIG_GLOBALS_MAPPING))
				{
					int num = text2.LastIndexOf(GVL_IOCONFIG_GLOBALS_MAPPING) - 1;
					if (num > 0)
					{
						text2 = text2.Remove(num, GVL_IOCONFIG_GLOBALS_MAPPING.Length + 1);
					}
				}
				if (!dictionary.ContainsValue(text2))
				{
					expressionsToInsert.Add(text2);
				}
			}
			return dictionary;
		}

		public void AddExpressionOnChangingPreparedValue(IVarRef varRef)
		{
			IWatchListView2 forceListView = (IWatchListView2)(object)getForceListView(GUID_FACTORY);
			if (forceListView == null)
			{
				return;
			}
			string qualifiedPath = varRef.GetQualifiedPath();
			string[] expressions = forceListView.GetExpressions();
			int nInsertIndex = expressions.Length;
			string[] array = expressions;
			foreach (string text in array)
			{
				if (!(text == qualifiedPath))
				{
					continue;
				}
				for (int num = _treeTableView.Nodes.Count - 1; num >= 0; num--)
				{
					WatchListNode modelNodeAt = GetModelNodeAt(num);
					if (modelNodeAt != null && modelNodeAt.Expression.Equals(text) && modelNodeAt.OnlineVarRef.PreparedRawValue == null && !modelNodeAt.OnlineVarRef.Forced)
					{
						RemoveExpressionAt(num);
						break;
					}
				}
				return;
			}
			TryInsertExpression(forceListView, qualifiedPath, nInsertIndex);
		}

		private bool TryInsertExpression(IWatchListView2 wlv, string stVarRefExpr, int nInsertIndex)
		{
			wlv.InsertExpression(nInsertIndex, stVarRefExpr);
			if (!HasValidVarRef(GetModelNodeAt(nInsertIndex)))
			{
				wlv.RemoveExpressionAt(nInsertIndex);
				return false;
			}
			return true;
		}

		private static bool HasValidVarRef(WatchListNode newNode)
		{
			try
			{
				return newNode.VarRef.AddressInfo != null;
			}
			catch (InvalidVarRefException)
			{
				return false;
			}
		}

		private WatchListNode GetModelNodeAt(int i)
		{
			TreeTableViewNode nodeAtRow = _treeTableView.GetNodeAtRow(i);
			return _treeTableView.GetModelNode(nodeAtRow) as WatchListNode;
		}

		private IWatchListView3 getForceListView(Guid inGuid)
		{
			if (inGuid == Guid.Empty)
			{
				IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
				return (IWatchListView3)(object)((activeView is IWatchListView3) ? activeView : null);
			}
			if (inGuid == GUID_FACTORY)
			{
				IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews(GUID_FACTORY);
				if (views == null || views.Length < 1 || !(views[0] is IWatchListView))
				{
					return null;
				}
				IView obj = views[0];
				return (IWatchListView3)(object)((obj is IWatchListView3) ? obj : null);
			}
			return null;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Return) && ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count == 1 && ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0].Nodes.Count == 0)
			{
				((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0].Focus(3);
				((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0].BeginEdit(3);
			}
		}

		public void OnVisibilityChanged(bool bVisible)
		{
			_bVisible = bVisible;
			EnableWatchListNodes();
		}

		public void EnableVisibilityChangeNotification()
		{
			_bVisibilityChangeNotificationEnabled = true;
		}

		public bool SelectExpression(string stExpression)
		{
			return SelectExpression(stExpression, exclusive: true);
		}

		internal bool SelectExpression(string stExpression, bool exclusive)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			if (!string.IsNullOrEmpty(_model.InstancePath))
			{
				stExpression = _model.InstancePath + "." + stExpression;
			}
			foreach (TreeTableViewNode node in _treeTableView.Nodes)
			{
				TreeTableViewNode val = node;
				WatchListNode watchListNode = _treeTableView.GetModelNode(val) as WatchListNode;
				if (watchListNode != null && string.Equals(stExpression, watchListNode.Expression, StringComparison.OrdinalIgnoreCase))
				{
					if (exclusive)
					{
						_treeTableView.DeselectAll();
					}
					val.Selected=(true);
					val.Focus(0);
					val.EnsureVisible(0);
					return true;
				}
			}
			return false;
		}

		public string GetSelectedExpression(bool bReturnRootExpression, out bool bIsRootExpression)
		{
			TreeTableViewNode focusedNode = _treeTableView.FocusedNode;
			WatchListNode watchListNode = _treeTableView.GetModelNode(focusedNode) as WatchListNode;
			if (watchListNode == null || watchListNode.IsEmpty)
			{
				bIsRootExpression = false;
				return null;
			}
			bIsRootExpression = true;
			if (bReturnRootExpression && watchListNode.Parent is WatchListNode)
			{
				bIsRootExpression = false;
				while (watchListNode.Parent is WatchListNode)
				{
					watchListNode = (WatchListNode)(object)watchListNode.Parent;
				}
			}
			string text = watchListNode.Expression;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(_model.InstancePath) && text.StartsWith(_model.InstancePath + ".", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(_model.InstancePath.Length + 1);
			}
			return text;
		}

		public IEnumerable<string> GetSelectedExpressions(bool bReturnRootExpression, out bool bIsRootExpression)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			SelectedTreeTableViewNodeCollection selectedNodes = _treeTableView.SelectedNodes;
			List<string> list = new List<string>();
			bIsRootExpression = false;
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)selectedNodes)
			{
				TreeTableViewNode val = item;
				WatchListNode watchListNode = _treeTableView.GetModelNode(val) as WatchListNode;
				if (watchListNode == null || watchListNode.IsEmpty)
				{
					bIsRootExpression = false;
					return null;
				}
				bIsRootExpression = true;
				if (bReturnRootExpression && watchListNode.Parent is WatchListNode)
				{
					bIsRootExpression = false;
					while (watchListNode.Parent is WatchListNode)
					{
						watchListNode = (WatchListNode)(object)watchListNode.Parent;
					}
				}
				string text = watchListNode.Expression;
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(_model.InstancePath) && text.StartsWith(_model.InstancePath + ".", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(_model.InstancePath.Length + 1);
				}
				list.Add(text);
			}
			return list;
		}

		private bool CanProcessAP_EditorSelectionData(DragEventArgs e, out string stExpression, out int iProjectHandle, out Guid objectGuid, out IPreCompileContext pcc, out OnlineState onlineState)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			stExpression = null;
			iProjectHandle = -1;
			objectGuid = Guid.Empty;
			pcc = null;
			int length = 0;
			onlineState = default(OnlineState);
			if (e.Data.GetDataPresent("AP_EditorSelectionData"))
			{
				GetSelectionFromDataObject(e.Data, out stExpression, out iProjectHandle, out objectGuid, out pcc, out length, out onlineState);
				if (!string.IsNullOrEmpty(stExpression) && iProjectHandle >= 0 && objectGuid != Guid.Empty)
				{
					return pcc != null;
				}
				return false;
			}
			return false;
		}

		private bool CanProcessAP_EditorSelectionData(DragEventArgs e)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			string stExpression = null;
			int iProjectHandle = -1;
			Guid objectGuid = Guid.Empty;
			IPreCompileContext pcc = null;
			OnlineState onlineState = default(OnlineState);
			return CanProcessAP_EditorSelectionData(e, out stExpression, out iProjectHandle, out objectGuid, out pcc, out onlineState);
		}

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.None;
			if (ReadOnly)
			{
				return;
			}
			if (e.Data.GetDataPresent("AP_EditorExpressionData"))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			if (CanProcessAP_EditorSelectionData(e))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			string text = e.Data.GetData(typeof(string)) as string;
			if (text == null)
			{
				return;
			}
			if (_bIsLocalDragOperation)
			{
				if (!string.IsNullOrWhiteSpace(text.Trim()))
				{
					e.Effect = DragDropEffects.Copy;
				}
				return;
			}
			string[] array = text.Split('\r', '\n');
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(array[i]))
				{
					e.Effect = DragDropEffects.Copy;
					break;
				}
			}
		}

		private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
		}

		private void OnDragDrop(object sender, DragEventArgs e)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			e.Effect = DragDropEffects.None;
			if (ReadOnly || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return;
			}
			if (e.Data.GetDataPresent("AP_EditorExpressionData"))
			{
				string text = e.Data.GetData("AP_EditorExpressionData") as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					Common.AddToWatch(new string[1] { text }, this);
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}
			if (CanProcessAP_EditorSelectionData(e, out var stExpression, out var iProjectHandle, out var objectGuid, out var pcc, out var onlineState) && Common.AddToWatch(onlineState, pcc, iProjectHandle, objectGuid, stExpression, this))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			string text2 = e.Data.GetData(typeof(string)) as string;
			if (text2 == null)
			{
				return;
			}
			bool flag = false;
			if (_bIsLocalDragOperation)
			{
				if (((TreeTableViewNodeCollection)_treeTableView.SelectedNodes).Count == 1)
				{
					Point point = ((Control)(object)_treeTableView).PointToClient(new Point(e.X, e.Y));
					TreeTableViewNode nodeAt = _treeTableView.GetNodeAt(point.X, point.Y);
					TreeTableViewNode val = ((TreeTableViewNodeCollection)_treeTableView.SelectedNodes)[0];
					if (nodeAt != null && val != null && nodeAt.Index < _treeTableView.Nodes.Count - 1 && nodeAt.Index != val.Index)
					{
						try
						{
							int index = nodeAt.Index;
							_treeTableView.DeselectAll();
							_treeTableView.BeginUpdate();
							_model.BeginUpdate();
							_model.Remove(val.Index);
							_model.Insert(index, text2);
							_treeTableView.Nodes[index].Selected=(true);
							_treeTableView.Nodes[index].Focus(0);
						}
						finally
						{
							_treeTableView.EndUpdate();
							_model.EndUpdate();
						}
					}
					else
					{
						flag = true;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag && Common.AddToWatch(text2.Split('\r', '\n'), this))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private static IExprement ExtractVariableExpression(string expression, ISourcePosition sourceposition, out IPreCompileContext pcc)
		{
			pcc = null;
			IExprement result = null;
			int num = sourceposition.PositionOffset;
			string text = expression;
			for (int i = 0; i < text.Length && string.IsNullOrWhiteSpace(text[i].ToString()); i++)
			{
				num++;
			}
			int num2 = 0;
			text = expression.TrimStart();
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (string.IsNullOrWhiteSpace(c.ToString()) || c == ';' || c == ':')
				{
					break;
				}
				num2++;
			}
			if (num != sourceposition.PositionOffset || num2 != expression.Length)
			{
				SourcePosition sourcePosition = new SourcePosition(sourceposition.ProjectHandle, sourceposition.ObjectGuid, sourceposition.Position, (short)num, (short)num2);
				result = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindExpressionAtSourcePosition((ISourcePosition)(object)sourcePosition, (WhatToFind)1, out pcc);
			}
			return result;
		}

		private static void GetSelectionFromDataObject(IDataObject dataObject, out string expression, out int projectHandle, out Guid objectGuid, out IPreCompileContext pcc, out int length, out OnlineState onlineState)
		{
			expression = null;
			projectHandle = -1;
			objectGuid = Guid.Empty;
			pcc = null;
			length = 0;
			onlineState.InstancePath = null;
			onlineState.OnlineApplication = Guid.Empty;
			if (dataObject == null || !dataObject.GetDataPresent("AP_EditorSelectionData"))
			{
				return;
			}
			try
			{
				string[] array = ((string)dataObject.GetData("AP_EditorSelectionData")).Split(';');
				projectHandle = int.Parse(array[0]);
				objectGuid = new Guid(array[1]);
				long num = long.Parse(array[2]);
				length = int.Parse(array[3]);
				onlineState.OnlineApplication = new Guid(array[4]);
				onlineState.InstancePath = array[5];
				if (onlineState.InstancePath == "(null)")
				{
					onlineState.InstancePath = null;
				}
				long num2 = default(long);
				short num3 = default(short);
				if (length < 0)
				{
					length = -length;
					PositionHelper.SplitPosition(num, out num2, out num3);
					num3 = (short)(num3 - length);
					num = PositionHelper.CombinePosition(num2, num3);
				}
				PositionHelper.SplitPosition(num, out num2, out num3);
				ISourcePosition val = (ISourcePosition)(object)new SourcePosition(projectHandle, objectGuid, num2, num3, (short)length);
				IExprement val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindExpressionAtSourcePosition(val, (WhatToFind)4, out pcc);
				if (val2 == null)
				{
					val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindExpressionAtSourcePosition(val, (WhatToFind)1, out pcc);
				}
				if (val2 != null)
				{
					expression = val2.ToString();
					return;
				}
				IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(projectHandle, objectGuid);
				Debug.Assert(objectToRead != null);
				IObject @object = objectToRead.Object;
				Debug.Assert(@object != null);
				expression = @object.GetContentString(ref num, ref length, true);
				if (!string.IsNullOrWhiteSpace(expression))
				{
					IExprement val3 = ExtractVariableExpression(expression, val, out pcc);
					if (val3 != null)
					{
						expression = val3.ToString();
					}
				}
			}
			catch
			{
			}
		}

		public IEnumerable<string> GetSelectedExpressions_FullPath()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			LList<string> val = new LList<string>();
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)_treeTableView.SelectedNodes)
			{
				TreeTableViewNode val2 = item;
				WatchListNode watchListNode = _treeTableView.GetModelNode(val2) as WatchListNode;
				if (!string.IsNullOrWhiteSpace(watchListNode?.Expression))
				{
					val.Add(watchListNode.Expression);
				}
			}
			return (IEnumerable<string>)val;
		}

		private void OnTreeTableViewNodeDrag(object sender, TreeTableViewDragEventArgs e)
		{
			IEnumerable<string> selectedExpressions_FullPath = GetSelectedExpressions_FullPath();
			DataObject dataObject = new DataObject(string.Join("\r\n", selectedExpressions_FullPath));
			if (selectedExpressions_FullPath.Count() == 1)
			{
				_bIsLocalDragOperation = true;
			}
			try
			{
				_treeTableView.DoDragDrop((object)dataObject, DragDropEffects.Copy);
			}
			finally
			{
				_bIsLocalDragOperation = false;
			}
		}

		private void OnDoubleClick(object sender, EventArgs e)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				WatchListNode[] selectedNodes = GetSelectedNodes();
				if (selectedNodes.Length == 0)
				{
					if (GetSelectedWarningNode() != null)
					{
						((IEngine)APEnvironment.Engine).Frame.OpenView(GUID_BREAKPOINTVIEWFACTORY, (string)null);
					}
				}
				else
				{
					if (selectedNodes.Length != 1)
					{
						return;
					}
					int num = -1;
					TreeTableViewNode focusedNode = _treeTableView.GetFocusedNode(out num);
					_treeTableView.GetModelNode(focusedNode);
					if (num == _model.COLUMN_PREPARED_VALUE)
					{
						return;
					}
					if (num == _model.COLUMN_TYPE)
					{
						WatchListNode selectedArrayNode = SelectedArrayNode;
						if (selectedArrayNode == null)
						{
							return;
						}
						try
						{
							IArrayDimension[] dimensions = ((IArrayType)selectedArrayNode.VarRef.WatchExpression.Type.DeRefType).Dimensions;
							if (dimensions != null)
							{
								MonitoringRangeContext monitoringRangeContext = new MonitoringRangeContext(dimensions, 1, selectedArrayNode.GuidApplication, selectedArrayNode.GetCompiledType(), selectedArrayNode.Expression);
								if (selectedArrayNode.MonitoringRange != null)
								{
									monitoringRangeContext.Start = selectedArrayNode.MonitoringRange.Item1;
									monitoringRangeContext.End = selectedArrayNode.MonitoringRange.Item2;
								}
								else if (monitoringRangeContext.End - monitoringRangeContext.Start >= WatchListModel.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY)
								{
									monitoringRangeContext.End = monitoringRangeContext.Start + WatchListModel.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY - 1;
								}
								if (monitoringRangeContext.End - monitoringRangeContext.Start >= WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY)
								{
									monitoringRangeContext.End = monitoringRangeContext.Start + WatchListModel.MAX_MONITORING_ELEMENTS_PER_ARRAY - 1;
								}
								MonitoringRangeForm monitoringRangeForm = new MonitoringRangeForm(monitoringRangeContext);
								Guid guid = new Guid("{5109C95D-090B-4384-9EC2-CCB1BED2D4B4}");
								DialogHelper.RestoreStateWithPosition((Form)monitoringRangeForm, guid);
								if (monitoringRangeForm.ShowDialog((IWin32Window)APEnvironment.FrameForm) == DialogResult.OK && (monitoringRangeContext.Start != monitoringRangeForm.NewStart || monitoringRangeContext.End != monitoringRangeForm.NewEnd))
								{
									selectedArrayNode.MonitoringRange = new Tuple<int, int, int, int, bool>(monitoringRangeForm.NewStart, monitoringRangeForm.NewEnd, monitoringRangeContext.LowerBound, monitoringRangeContext.UpperBound, monitoringRangeContext.SingleDimension);
								}
								DialogHelper.StoreState((Form)monitoringRangeForm, guid);
							}
						}
						catch
						{
						}
					}
					else
					{
						if (num != _model.COLUMN_EXPRESSION)
						{
							return;
						}
						if (selectedNodes[0].ChildCount > 0 && focusedNode != null)
						{
							if (focusedNode.Expanded)
							{
								focusedNode.Collapse();
							}
							else
							{
								focusedNode.Expand();
							}
						}
						else if (selectedNodes[0].Parent != null)
						{
							focusedNode = _treeTableView.GetViewNode(selectedNodes[0].Parent);
							if (focusedNode != null)
							{
								focusedNode.Collapse();
							}
						}
					}
					return;
				}
			}
			catch
			{
			}
		}

		internal void HandleRefactoring(RefactoringCommittedEventArgs e)
		{
			if (e == null || ((AbstractRefactoringEventArgs)e).Operation == null || ((AbstractRefactoringExecutionEventArgs)e).CrossReferences == null)
			{
				return;
			}
			bool flag = false;
			try
			{
				_model.BeginUpdate();
				string[] expressions = _model.GetExpressions();
				LList<string> exprsRefactored = null;
				flag = RefactorWatchExpressions(e, expressions, _model, out exprsRefactored);
				if (flag)
				{
					_model.MarkModelAsOutdated();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				flag = false;
			}
			finally
			{
				_model.EndUpdate();
				if (flag)
				{
					_model.Save();
				}
			}
		}

		internal static bool RefactorWatchExpressions(RefactoringCommittedEventArgs e, string[] exprs, WatchListModel model, out LList<string> exprsRefactored)
		{
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Expected O, but got Unknown
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Expected O, but got Unknown
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Invalid comparison between Unknown and I4
			//IL_0855: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Invalid comparison between Unknown and I4
			//IL_088e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Invalid comparison between Unknown and I4
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0975: Expected O, but got Unknown
			//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cd3: Expected O, but got Unknown
			//IL_0e42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e4c: Expected O, but got Unknown
			bool flag = false;
			exprsRefactored = new LList<string>();
			if (exprs != null && exprs.Length != 0 && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				exprsRefactored.AddRange((IEnumerable<string>)(string[])exprs.Clone());
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				{
					IPreCompileContext val3 = default(IPreCompileContext);
					IPreCompileContext val9 = default(IPreCompileContext);
					foreach (IRefactoringOperation selectedSubOperation in ((AbstractRefactoringEventArgs)e).Operation.SelectedSubOperations)
					{
						if (selectedSubOperation is IRefactoringRenameVariableOperation || selectedSubOperation is IRefactoringRenameSignatureOperation)
						{
							IRefactoringRenameOperation val = (IRefactoringRenameOperation)(object)((selectedSubOperation is IRefactoringRenameOperation) ? selectedSubOperation : null);
							bool flag2 = selectedSubOperation is IRefactoringRenameVariableOperation;
							string text = string.Empty;
							string text2 = string.Empty;
							_ = Guid.Empty;
							Guid empty = Guid.Empty;
							if (val.Signature == null)
							{
								continue;
							}
							empty = val.Signature.ObjectGuid;
							while (empty != Guid.Empty)
							{
								IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, empty);
								if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
								{
									//metaObjectStub.ObjectGuid;
									text = metaObjectStub.Name;
									IObject @object = APEnvironment.ObjectMgr.GetObjectToRead(handle, metaObjectStub.ObjectGuid).Object;
									IApplicationObject5 val2 = (IApplicationObject5)(object)((@object is IApplicationObject5) ? @object : null);
									if (val2 != null && APEnvironment.ObjectMgr.ExistsObject(handle, ((IOnlineApplicationObject)val2).DeviceGuid))
									{
										text2 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, ((IOnlineApplicationObject)val2).DeviceGuid).Name;
									}
									break;
								}
								empty = metaObjectStub.ParentObjectGuid;
							}
							for (int i = 0; i < exprs.Length; i++)
							{
								bool flag3 = false;
								string text3 = exprs[i];
								string[] array = text3.Split('.', '[', ']');
								string[] array2 = array;
								for (int j = 0; j < array2.Length; j++)
								{
									if (array2[j].ToLowerInvariant() == val.OldName.ToLowerInvariant())
									{
										flag3 = true;
										break;
									}
								}
								if (!flag3 || (!(text == string.Empty) && (array.Length <= 3 || !(text.ToLowerInvariant() == array[1].ToLowerInvariant()) || !(text2.ToLowerInvariant() == array[0].ToLowerInvariant()))))
								{
									continue;
								}
								if (flag2)
								{
									text3 = Regex.Replace(text3, array[0] + "." + array[1] + ".", "", RegexOptions.IgnoreCase);
									ISignature obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindSignature(val.Signature.ObjectGuid, out val3);
									ISignature4 val4 = (ISignature4)(object)((obj is ISignature4) ? obj : null);
									if (val4 == null || !(val3 is IPreCompileContext9))
									{
										continue;
									}
									VariableVisitor variableVisitor = new VariableVisitor(((ISignature)val4).OrgName);
									IScanner val5 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text3, true, true, true, true);
									IParser obj2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val5);
									IExpression val6 = ((obj2 is IParser4) ? obj2 : null).ParseOperand();
									((IPreCompileUtilities6)((ILanguageModelUtilities)APEnvironment.LanguageModelUtilities).PreCompileUtils).VisitAllVariables((IVariableVisitor)(object)variableVisitor, val4, (IPreCompileContext9)val3, (IExprement)(object)val6);
									foreach (string match in variableVisitor.Matches)
									{
										flag = RefactorSingleVariableOrSignature(exprs[i], match, val.OldName, val.NewName, flag2, i, model, ref exprsRefactored) || flag;
									}
									if (variableVisitor.Matches.Count == 0 && text3.ToLowerInvariant().Contains(val.Signature.OrgName.ToLowerInvariant() + "."))
									{
										flag = RefactorSingleVariableOrSignature(exprs[i], val.Signature.OrgName, val.OldName, val.NewName, flag2, i, model, ref exprsRefactored) || flag;
									}
								}
								else
								{
									flag = RefactorSingleVariableOrSignature(exprs[i], val.Signature.OrgName, val.OldName, val.NewName, flag2, i, model, ref exprsRefactored) || flag;
								}
							}
						}
						else if (selectedSubOperation is IRefactoringRenamePropertyOperation)
						{
							IRefactoringRenamePropertyOperation val7 = (IRefactoringRenamePropertyOperation)(object)((selectedSubOperation is IRefactoringRenamePropertyOperation) ? selectedSubOperation : null);
							string text4 = string.Empty;
							string text5 = string.Empty;
							_ = Guid.Empty;
							if (APEnvironment.ObjectMgr.ExistsObject(handle, val7.PropertyGuid))
							{
								Guid guid = val7.PropertyGuid;
								while (guid != Guid.Empty)
								{
									IMetaObjectStub metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, guid);
									if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub2.ObjectType))
									{
										//metaObjectStub2.ObjectGuid;
										text4 = metaObjectStub2.Name;
										IObject object2 = APEnvironment.ObjectMgr.GetObjectToRead(handle, metaObjectStub2.ObjectGuid).Object;
										IApplicationObject5 val8 = (IApplicationObject5)(object)((object2 is IApplicationObject5) ? object2 : null);
										if (val8 != null && APEnvironment.ObjectMgr.ExistsObject(handle, ((IOnlineApplicationObject)val8).DeviceGuid))
										{
											text5 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, ((IOnlineApplicationObject)val8).DeviceGuid).Name;
										}
										break;
									}
									guid = metaObjectStub2.ParentObjectGuid;
								}
							}
							for (int k = 0; k < exprs.Length; k++)
							{
								string text6 = exprs[k];
								if (!text6.ToLowerInvariant().Contains(((IRefactoringRenameOperation)val7).OldName.ToLowerInvariant()))
								{
									continue;
								}
								string[] array3 = text6.Split('.', '[', ']');
								if (!(text4 == string.Empty) && (array3.Length <= 4 || !(text4.ToLowerInvariant() == array3[1].ToLowerInvariant()) || !(text5.ToLowerInvariant() == array3[0].ToLowerInvariant())))
								{
									continue;
								}
								text6 = Regex.Replace(text6, array3[0] + "." + array3[1] + ".", "", RegexOptions.IgnoreCase);
								ISignature obj3 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindSignature(((IRefactoringRenameOperation)val7).Signature.ObjectGuid, out val9);
								ISignature4 val10 = (ISignature4)(object)((obj3 is ISignature4) ? obj3 : null);
								if (val10 == null || !(val9 is IPreCompileContext9))
								{
									continue;
								}
								VariableVisitor variableVisitor2 = new VariableVisitor(((ISignature)val10).OrgName);
								IScanner val11 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text6, true, true, true, true);
								IParser obj4 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val11);
								IExpression val12 = ((obj4 is IParser4) ? obj4 : null).ParseOperand();
								((IPreCompileUtilities6)((ILanguageModelUtilities)APEnvironment.LanguageModelUtilities).PreCompileUtils).VisitAllVariables((IVariableVisitor)(object)variableVisitor2, val10, (IPreCompileContext9)val9, (IExprement)(object)val12);
								foreach (string match2 in variableVisitor2.Matches)
								{
									flag = RefactorSingleProperty(exprs[k], match2, ((IRefactoringRenameOperation)val7).OldName, ((IRefactoringRenameOperation)val7).NewName, k, model, ref exprsRefactored) || flag;
								}
							}
						}
						else if (selectedSubOperation is IRefactoringAddVariableOperation)
						{
							IRefactoringAddVariableOperation val13 = (IRefactoringAddVariableOperation)(object)((selectedSubOperation is IRefactoringAddVariableOperation) ? selectedSubOperation : null);
							string text7 = string.Empty;
							string text8 = string.Empty;
							_ = Guid.Empty;
							Guid empty2 = Guid.Empty;
							if (val13.Signature == null || (int)val13.VariableDeclaration.Scope != 2)
							{
								continue;
							}
							empty2 = val13.Signature.ObjectGuid;
							while (empty2 != Guid.Empty)
							{
								IMetaObjectStub metaObjectStub3 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, empty2);
								if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub3.ObjectType))
								{
									//metaObjectStub3.ObjectGuid;
									text7 = metaObjectStub3.Name;
									IObject object3 = APEnvironment.ObjectMgr.GetObjectToRead(handle, metaObjectStub3.ObjectGuid).Object;
									IApplicationObject5 val14 = (IApplicationObject5)(object)((object3 is IApplicationObject5) ? object3 : null);
									if (val14 != null && APEnvironment.ObjectMgr.ExistsObject(handle, ((IOnlineApplicationObject)val14).DeviceGuid))
									{
										text8 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, ((IOnlineApplicationObject)val14).DeviceGuid).Name;
									}
									break;
								}
								empty2 = metaObjectStub3.ParentObjectGuid;
							}
							for (int l = 0; l < exprs.Length; l++)
							{
								bool flag4 = false;
								string text9 = exprs[l];
								string[] array4 = text9.Split('.', '(', ')');
								string[] array2 = array4;
								for (int j = 0; j < array2.Length; j++)
								{
									if (array2[j].ToUpperInvariant() == val13.Signature.Name.ToUpperInvariant())
									{
										flag4 = true;
										break;
									}
								}
								if (!flag4)
								{
									continue;
								}
								string text10 = string.Empty;
								if (!(text7 == string.Empty) && (array4.Length <= 3 || !(text7.ToLowerInvariant() == array4[1].ToLowerInvariant()) || !(text8.ToLowerInvariant() == array4[0].ToLowerInvariant())))
								{
									continue;
								}
								string text11 = ", ";
								if (val13.Signature.AllInputs.Length == 0)
								{
									text11 = string.Empty;
								}
								if ((int)val13.CallParameterMode == 2)
								{
									text10 = text9.Insert(text9.Length - 1, text11 + val13.DefaultParameterValue);
									exprsRefactored[l]= text10;
									flag = true;
								}
								else if ((int)val13.CallParameterMode == 1)
								{
									text10 = text9.Insert(text9.Length - 1, text11 + val13.ParameterAssignmentDeclaration);
									exprsRefactored[l]= text10;
									flag = true;
								}
								if (text10 != string.Empty && model != null)
								{
									ITreeTableNode child = ((AbstractTreeTableModel)model).Sentinel.GetChild(l);
									ExpressionData expressionData = child.GetValue(model.COLUMN_EXPRESSION) as ExpressionData;
									if (expressionData != null)
									{
										int startIndex = expressionData.FullExpression.IndexOf(expressionData.DisplayExpression);
										string stFullExpression = text10;
										string text12 = text10.Substring(startIndex);
										ExpressionData expressionData2 = null;
										expressionData2 = ((model.COLUMN_APPLICATION_PREFIX <= -1) ? new ExpressionData(stFullExpression, text12, expressionData.Image) : new ExpressionData(text12, text12, expressionData.Image));
										child.SetValue(model.COLUMN_EXPRESSION, (object)expressionData2);
										model.RaiseValueChanged(new TreeTableModelEventArgs((ITreeTableNode)null, 0, child));
									}
								}
							}
						}
						else if (selectedSubOperation is IRefactoringRemoveVariableOperation)
						{
							IRefactoringRemoveVariableOperation val15 = (IRefactoringRemoveVariableOperation)(object)((selectedSubOperation is IRefactoringRemoveVariableOperation) ? selectedSubOperation : null);
							string text13 = string.Empty;
							string text14 = string.Empty;
							_ = Guid.Empty;
							Guid empty3 = Guid.Empty;
							if (val15.Signature == null)
							{
								continue;
							}
							int num = -1;
							int num2 = 0;
							IVariable[] allInputs = val15.Signature.AllInputs;
							for (int j = 0; j < allInputs.Length; j++)
							{
								if (allInputs[j] == val15.Variable)
								{
									num = num2;
									break;
								}
								num2++;
							}
							if (num == -1)
							{
								continue;
							}
							empty3 = val15.Signature.ObjectGuid;
							while (empty3 != Guid.Empty)
							{
								IMetaObjectStub metaObjectStub4 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, empty3);
								if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub4.ObjectType))
								{
									//metaObjectStub4.ObjectGuid;
									text13 = metaObjectStub4.Name;
									IObject object4 = APEnvironment.ObjectMgr.GetObjectToRead(handle, metaObjectStub4.ObjectGuid).Object;
									IApplicationObject5 val16 = (IApplicationObject5)(object)((object4 is IApplicationObject5) ? object4 : null);
									if (val16 != null && APEnvironment.ObjectMgr.ExistsObject(handle, ((IOnlineApplicationObject)val16).DeviceGuid))
									{
										text14 = APEnvironment.ObjectMgr.GetMetaObjectStub(handle, ((IOnlineApplicationObject)val16).DeviceGuid).Name;
									}
									break;
								}
								empty3 = metaObjectStub4.ParentObjectGuid;
							}
							for (int m = 0; m < exprs.Length; m++)
							{
								bool flag5 = false;
								string text15 = exprs[m];
								string[] array5 = text15.Split('.', '(', ')');
								string[] array2 = array5;
								for (int j = 0; j < array2.Length; j++)
								{
									if (array2[j].ToUpperInvariant() == val15.Signature.Name.ToUpperInvariant())
									{
										flag5 = true;
										break;
									}
								}
								if (!flag5)
								{
									continue;
								}
								string text16 = string.Empty;
								if (!(text13 == string.Empty) && (array5.Length <= 3 || !(text13.ToLowerInvariant() == array5[1].ToLowerInvariant()) || !(text14.ToLowerInvariant() == array5[0].ToLowerInvariant())))
								{
									continue;
								}
								array5 = text15.Split(',', '(', ')');
								if (array5.Length > num + 2)
								{
									text16 = array5[0] + "(";
									num2 = 0;
									for (int n = 1; n < array5.Length - 1; n++)
									{
										if (num + 1 != n)
										{
											text16 += array5[n];
											if (num2 < array5.Length - 4)
											{
												text16 += ",";
											}
											num2++;
										}
									}
									text16 += ")";
									exprsRefactored[m]= text16;
									flag = true;
								}
								if (text16 != string.Empty && model != null)
								{
									ITreeTableNode child2 = ((AbstractTreeTableModel)model).Sentinel.GetChild(m);
									ExpressionData expressionData3 = child2.GetValue(model.COLUMN_EXPRESSION) as ExpressionData;
									if (expressionData3 != null)
									{
										int startIndex2 = expressionData3.FullExpression.IndexOf(expressionData3.DisplayExpression);
										string stFullExpression2 = text16;
										string text17 = text16.Substring(startIndex2);
										ExpressionData expressionData4 = null;
										expressionData4 = ((model.COLUMN_APPLICATION_PREFIX <= -1) ? new ExpressionData(stFullExpression2, text17, expressionData3.Image) : new ExpressionData(text17, text17, expressionData3.Image));
										child2.SetValue(model.COLUMN_EXPRESSION, (object)expressionData4);
										model.RaiseValueChanged(new TreeTableModelEventArgs((ITreeTableNode)null, 0, child2));
									}
								}
							}
						}
						else
						{
							if (!(selectedSubOperation is IRefactoringRenameLanguageModelProvidingObjectOperation))
							{
								continue;
							}
							IRefactoringRenameLanguageModelProvidingObjectOperation val17 = (IRefactoringRenameLanguageModelProvidingObjectOperation)(object)((selectedSubOperation is IRefactoringRenameLanguageModelProvidingObjectOperation) ? selectedSubOperation : null);
							if (string.IsNullOrEmpty(((IRefactoringRenameOperation)val17).OldName) || string.IsNullOrEmpty(((IRefactoringRenameOperation)val17).NewName))
							{
								continue;
							}
							_ = string.Empty;
							_ = string.Empty;
							for (int num3 = 0; num3 < exprs.Length; num3++)
							{
								string text18 = exprs[num3];
								string[] array6 = text18.Split('.');
								if (array6.Length < 2 || !(((IRefactoringRenameOperation)val17).OldName.ToUpperInvariant() == array6[1].ToUpperInvariant()))
								{
									continue;
								}
								int startIndex3 = array6[0].Length + 1 + array6[1].Length;
								string text19 = array6[0] + "." + ((IRefactoringRenameOperation)val17).NewName + text18.Substring(startIndex3);
								if (model != null)
								{
									ITreeTableNode child3 = ((AbstractTreeTableModel)model).Sentinel.GetChild(num3);
									ExpressionData expressionData5 = child3.GetValue(model.COLUMN_EXPRESSION) as ExpressionData;
									if (expressionData5 != null)
									{
										int startIndex4 = expressionData5.FullExpression.IndexOf(expressionData5.DisplayExpression);
										string stFullExpression3 = text19;
										string text20 = text19.Substring(startIndex4);
										ExpressionData expressionData6 = null;
										expressionData6 = ((model.COLUMN_APPLICATION_PREFIX <= -1) ? new ExpressionData(stFullExpression3, text20, expressionData5.Image) : new ExpressionData(text20, text20, expressionData5.Image));
										child3.SetValue(model.COLUMN_EXPRESSION, (object)expressionData6);
										model.RaiseValueChanged(new TreeTableModelEventArgs((ITreeTableNode)null, 0, child3));
									}
								}
								exprsRefactored[num3]= text19;
								flag = true;
							}
						}
					}
					return flag;
				}
			}
			return flag;
		}

		private static bool RefactorSingleVariableOrSignature(string stExistingExpression, string stSignatureOrgName, string stOldName, string stNewName, bool bVariableOnly, int nModelIndex, WatchListModel model, ref LList<string> exprsRefactored)
		{
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Expected O, but got Unknown
			if (string.IsNullOrEmpty(stExistingExpression) || string.IsNullOrEmpty(stSignatureOrgName) || string.IsNullOrEmpty(stOldName) || string.IsNullOrEmpty(stNewName))
			{
				return false;
			}
			bool result = false;
			try
			{
				string pattern = stOldName;
				string replacement = stNewName;
				if (bVariableOnly)
				{
					pattern = stSignatureOrgName + "." + stOldName;
					replacement = stSignatureOrgName + "." + stNewName;
				}
				if (model != null)
				{
					ITreeTableNode child = ((AbstractTreeTableModel)model).Sentinel.GetChild(nModelIndex);
					ExpressionData expressionData = child.GetValue(model.COLUMN_EXPRESSION) as ExpressionData;
					if (expressionData != null)
					{
						string text = Regex.Replace(expressionData.FullExpression, pattern, replacement, RegexOptions.IgnoreCase);
						string text2 = Regex.Replace(expressionData.DisplayExpression, pattern, replacement, RegexOptions.IgnoreCase);
						exprsRefactored[nModelIndex]= text;
						ExpressionData expressionData2 = null;
						expressionData2 = ((model.COLUMN_APPLICATION_PREFIX <= -1) ? new ExpressionData(text, text2, expressionData.Image) : new ExpressionData(text2, text2, expressionData.Image));
						child.SetValue(model.COLUMN_EXPRESSION, (object)expressionData2);
						model.RaiseValueChanged(new TreeTableModelEventArgs((ITreeTableNode)null, 0, child));
					}
				}
				else
				{
					exprsRefactored[nModelIndex]= Regex.Replace(stExistingExpression, pattern, replacement, RegexOptions.IgnoreCase);
				}
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return result;
			}
		}

		private static bool RefactorSingleProperty(string stExistingExpression, string stApplicationName, string stOldName, string stNewName, int nModelIndex, WatchListModel model, ref LList<string> exprsRefactored)
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Expected O, but got Unknown
			if (string.IsNullOrEmpty(stExistingExpression) || string.IsNullOrEmpty(stApplicationName) || string.IsNullOrEmpty(stOldName) || string.IsNullOrEmpty(stNewName))
			{
				return false;
			}
			string pattern = stApplicationName + "." + stOldName;
			string replacement = stApplicationName + "." + stNewName;
			bool result = false;
			try
			{
				if (model != null)
				{
					ITreeTableNode child = ((AbstractTreeTableModel)model).Sentinel.GetChild(nModelIndex);
					ExpressionData expressionData = child.GetValue(model.COLUMN_EXPRESSION) as ExpressionData;
					if (expressionData != null)
					{
						string stFullExpression = Regex.Replace(expressionData.FullExpression, pattern, replacement, RegexOptions.IgnoreCase);
						string text = Regex.Replace(expressionData.DisplayExpression, pattern, replacement, RegexOptions.IgnoreCase);
						ExpressionData expressionData2 = null;
						expressionData2 = ((model.COLUMN_APPLICATION_PREFIX <= -1) ? new ExpressionData(stFullExpression, text, expressionData.Image) : new ExpressionData(text, text, expressionData.Image));
						child.SetValue(model.COLUMN_EXPRESSION, (object)expressionData2);
						model.RaiseValueChanged(new TreeTableModelEventArgs((ITreeTableNode)null, 0, child));
					}
				}
				else
				{
					exprsRefactored[nModelIndex]= Regex.Replace(stExistingExpression, pattern, replacement, RegexOptions.IgnoreCase);
				}
				result = true;
				return result;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return result;
			}
		}

		public void SetLocalizedObject(IMetaObject obj, bool bLocalizationActive)
		{
			_bLocalizationActive = bLocalizationActive;
		}

		public bool IsComment(long nPositionCombined, string stText)
		{
			return false;
		}

		private TreeTableViewNode GetTreeTableViewNodeByExpression(TreeTableViewNode parentNode, string stExpression)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			if (!string.IsNullOrEmpty(_model.InstancePath))
			{
				stExpression = _model.InstancePath + "." + stExpression;
			}
			TreeTableViewNodeCollection val = null;
			val = ((parentNode == null) ? _treeTableView.Nodes : parentNode.Nodes);
			foreach (TreeTableViewNode item in val)
			{
				TreeTableViewNode val2 = item;
				WatchListNode watchListNode = _treeTableView.GetModelNode(val2) as WatchListNode;
				if (watchListNode != null && string.Equals(stExpression, watchListNode.Expression, StringComparison.OrdinalIgnoreCase))
				{
					val2.Focus(0);
					return val2;
				}
			}
			return null;
		}

		private TreeTableViewNode ExpandTreeTableViewNodes(IExpression exp)
		{
			TreeTableViewNode val = null;
			if (exp is IVariableExpression)
			{
				return GetTreeTableViewNodeByExpression(null, ((IExprement)exp).ToString());
			}
			if (exp is ICompoAccessExpression)
			{
				ICompoAccessExpression val2 = (ICompoAccessExpression)(object)((exp is ICompoAccessExpression) ? exp : null);
				val = ExpandTreeTableViewNodes(val2.Left);
			}
			else if (exp is IIndexAccessExpression)
			{
				IIndexAccessExpression val3 = (IIndexAccessExpression)(object)((exp is IIndexAccessExpression) ? exp : null);
				val = ExpandTreeTableViewNodes(val3.Var);
			}
			else if (exp is IDeRefAccessExpression)
			{
				IDeRefAccessExpression val4 = (IDeRefAccessExpression)(object)((exp is IDeRefAccessExpression) ? exp : null);
				val = ExpandTreeTableViewNodes(val4.Base);
			}
			if (val == null)
			{
				return null;
			}
			if (!val.Expanded)
			{
				val.Expand();
			}
			return GetTreeTableViewNodeByExpression(val, ((IExprement)exp).ToString());
		}

		public void SelectRecursiveInstancePath(IExpression instancePath)
		{
			TreeTableViewNode val = ExpandTreeTableViewNodes(instancePath);
			if (val != null)
			{
				_treeTableView.DeselectAll();
				val.Selected=(true);
				val.Focus(0);
				val.EnsureVisible(0);
			}
		}
	}
}
