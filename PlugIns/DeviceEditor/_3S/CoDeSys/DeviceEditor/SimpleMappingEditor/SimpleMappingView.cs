#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.BrowserCommands;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	[TypeGuid("{22D3D298-04FA-45B9-8C6F-F891F76F5F6A}")]
	public class SimpleMappingView : UserControl, IView, INotifyOnVisibilityChanged, IBrowserCommandsTarget, IExportImportCSVCommands, IHasAssociatedOnlineHelpTopic, IRefactoringCommandContext, ICloseViewOnClose
	{
		private IUndoManager _undoMgr;

		private SimpleMappingTreeTableModel _model;

		private ISVNode _startNode;

		private IProjectStructure ps;

		private Guid _onlineGuid = Guid.Empty;

		private long _onlineDelayTicks;

		private int _iLastColumnIndex = -1;

		private bool _bGetParameterInProgress;

		private bool _bRefillInProgress;

		private Timer _collapseTimer;

		private bool _bDoRefill;

		private long _delayTicks;

		public static readonly Guid GUID_EDITCUT = new Guid("{586FB001-60CA-4dd1-A2F9-F9319EE13C95}");

		public static readonly Guid GUID_EDITCOPY = new Guid("{E76B8E0B-9247-41e7-93D5-80FE36AF9449}");

		public static readonly Guid GUID_EDITPASTE = new Guid("{73A7678B-2707-44f6-963B-8A4B3C05A331}");

		public static readonly Guid GUID_EDITDELETE = new Guid("{C5AAECF0-F55A-4864-871E-4584D1CCC9AF}");

		public static readonly Guid GUID_GOTODEFINITION = new Guid("{d19a9182-fc62-4ea5-bd01-cad58e092281}");

		public static readonly Guid GUID_CROSSREFERENCES = new Guid("{2D499D13-AF93-43b4-AFDA-E98349935B51}");

		public static readonly Guid GUID_EDITUNDO = new Guid("{9ECCAF22-3293-4165-943E-88C2C40B4A58}");

		public static readonly Guid GUID_EDITREDO = new Guid("{871B29A1-9E9F-47f9-A5CE-D56C40976743}");

		public static readonly Guid GUID_IMPORTCSV = new Guid("{6AA1FB55-612D-4295-AF34-77FFD904FA44}");

		public static readonly Guid GUID_EXPORTCSV = new Guid("{E898828E-47A5-4e5c-BC0F-4008FF1999EB}");

		public static readonly Guid GUID_REFACTORING = new Guid("{EEA6D3F9-7B35-4843-8D5B-911B5E6D2DAB}");

		public static readonly Guid GUID_EDITSELECTALL = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		private IContainer components;

		private TreeTableView _mappingTreeTable;

		private Timer Timer;

		private Timer _onlineTimer;

		private ToolStrip _toolStrip;

		private ToolStripTextBox _tbFilter;

		private ToolStripLabel toolStripLabel2;

		private ToolStripComboBox _comboFilter;

		private ToolStripLabel toolStripLabel1;

		private ToolStripButton _btAddFB;

		internal DeviceNode GetFocusedDeviceNode
		{
			get
			{
				TreeTableViewNode focusedNode = _mappingTreeTable.FocusedNode;
				if (focusedNode != null)
				{
					return _mappingTreeTable.GetModelNode(focusedNode) as DeviceNode;
				}
				return null;
			}
		}

		public Control Control => this;

		public Control[] Panes => new Control[1] { (Control)(object)_mappingTreeTable };

		public string Caption => Strings.SimpleMappingName;

		public string Description => Strings.SimpleMappingDescription;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public DockingPosition DefaultDockingPosition => (DockingPosition)32;

		public DockingPosition PossibleDockingPositions => (DockingPosition)63;

		public bool IsVisible => GetFocusedDeviceNode != null;

		public SimpleMappingView()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			InitializeComponent();
			_toolStrip.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
			LSortedList<string, IIoMappingEditorFilterFactory> val = new LSortedList<string, IIoMappingEditorFilterFactory>();
			foreach (IIoMappingEditorFilterFactory ioMappingFilter in APEnvironment.IoMappingFilters)
			{
				if (!val.ContainsKey(ioMappingFilter.FilterName))
				{
					val.Add(ioMappingFilter.FilterName, ioMappingFilter);
				}
			}
			_comboFilter.Items.Clear();
			FilterItem item = new FilterItem(null);
			_comboFilter.Items.Add(item);
			foreach (IIoMappingEditorFilterFactory value in val.Values)
			{
				FilterItem item2 = new FilterItem(value);
				_comboFilter.Items.Add(item2);
			}
			_comboFilter.SelectedIndex = 0;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected O, but got Unknown
			if (_model != null)
			{
				_model.DetachEventHandlers();
			}
			if (APEnvironment.LocalizationManagerOrNull != null)
			{
				APEnvironment.LocalizationManagerOrNull.LocalizationDone-=((EventHandler<LocalizationDoneEventArgs>)LocalizationManagerOrNull_LocalizationDone);
			}
			((IEngine)APEnvironment.Engine).Projects.BeforePrimaryProjectSwitched-=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitching));
			((IEngine)APEnvironment.Engine).Projects.AfterPrimaryProjectSwitched-=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
			if (ps != null)
			{
				ps.Changed-=((EventHandler<PSChangedEventArgs>)ProjectChanged);
			}
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin-=(new AfterApplicationLoginEventHandler(OnLoginChange));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout-=(new AfterApplicationLogoutEventHandler(OnLoginChange));
			APEnvironment.OnlineMgr.AfterApplicationDownload2-=(new AfterApplicationDownload2EventHandler(OnLoginChange));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterDeviceLogin-=(new AfterDeviceLoginEventHandler(OnLoginChange));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterDeviceLogout-=(new AfterDeviceLogoutEventHandler(OnLoginChange));
			base.OnHandleDestroyed(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			base.OnLoad(e);
			((IEngine)APEnvironment.Engine).Projects.BeforePrimaryProjectSwitched+=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitching));
			((IEngine)APEnvironment.Engine).Projects.AfterPrimaryProjectSwitched+=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogin+=(new AfterApplicationLoginEventHandler(OnLoginChange));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterApplicationLogout+=(new AfterApplicationLogoutEventHandler(OnLoginChange));
			APEnvironment.OnlineMgr.AfterApplicationDownload2+=(new AfterApplicationDownload2EventHandler(OnLoginChange));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterDeviceLogin+=(new AfterDeviceLoginEventHandler(OnLoginChange));
			((IOnlineManager)APEnvironment.OnlineMgr).AfterDeviceLogout+=(new AfterDeviceLogoutEventHandler(OnLoginChange));
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				ps = APEnvironment.ObjectMgr.GetProjectStructure(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle);
				ps.Changed+=((EventHandler<PSChangedEventArgs>)ProjectChanged);
			}
			if (APEnvironment.LocalizationManagerOrNull != null)
			{
				APEnvironment.LocalizationManagerOrNull.LocalizationDone+=((EventHandler<LocalizationDoneEventArgs>)LocalizationManagerOrNull_LocalizationDone);
			}
			try
			{
				bool bLocalization = false;
				if (APEnvironment.LocalizationManagerOrNull != null)
				{
					bLocalization = APEnvironment.LocalizationManagerOrNull.IsLocalizationActive;
				}
				_model = new SimpleMappingTreeTableModel(_mappingTreeTable, bLocalization);
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
				{
					_undoMgr = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle);
				}
				_mappingTreeTable.Model=((ITreeTableModel)(object)_model);
				_model.UndoManager = _undoMgr;
				_model.AttachEventHandlers();
				_model.VariableFilter = (_comboFilter.Items[0] as FilterItem).MappingFilter;
			}
			catch
			{
			}
		}

		private void LocalizationManagerOrNull_LocalizationDone(object sender, LocalizationDoneEventArgs e)
		{
			if (APEnvironment.LocalizationManagerOrNull != null)
			{
				_model.Localization = APEnvironment.LocalizationManagerOrNull.IsLocalizationActive;
				_model.Refill(_startNode);
			}
		}

		public void StartNode(ISVNode startNode)
		{
			_startNode = startNode;
			if (_model != null)
			{
				_model.Clear();
				_model.Refill(startNode);
			}
		}

		private void OnPrimaryProjectSwitching(IProject oldProject, IProject newProject)
		{
			if (_model != null)
			{
				Helper.SaveColumdWidths(_model);
			}
		}

		private void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			if (ps != null)
			{
				ps.Changed-=((EventHandler<PSChangedEventArgs>)ProjectChanged);
				ps = null;
			}
			if (APEnvironment.Engine.Frame is IFrame9 && (APEnvironment.Engine.Frame as IFrame9).GetVisibility(this) != ViewVisibility.Closed)
			{
				StartNode(null);
				if (newProject != null)
				{
					_undoMgr = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(newProject.Handle);
					_model.UndoManager = _undoMgr;
				}
			}
			if (newProject != null)
			{
				ps = APEnvironment.ObjectMgr.GetProjectStructure(newProject.Handle);
				ps.Changed+=((EventHandler<PSChangedEventArgs>)ProjectChanged);
			}
		}

		private void OnLoginChange(object sender, OnlineEventArgs e)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			if (APEnvironment.Engine.Frame is IFrame9 && (APEnvironment.Engine.Frame as IFrame9).GetVisibility(this) != ViewVisibility.Closed)
			{
				_onlineGuid = e.GuidObject;
				_onlineDelayTicks = DateTime.Now.Ticks;
			}
		}

		private void _onlineTimer_Tick(object sender, EventArgs e)
		{
			if (_onlineGuid != Guid.Empty && DateTime.Now.Ticks - _onlineDelayTicks > 5000000)
			{
				_model.SetOffOnline(_onlineGuid);
				_onlineGuid = Guid.Empty;
			}
		}

		private void ProjectChanged(object sender, PSChangedEventArgs e)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			try
			{
				if (APEnvironment.Engine.Frame is IFrame9 && (APEnvironment.Engine.Frame as IFrame9).GetVisibility(this) != ViewVisibility.Closed)
				{
					_bRefillInProgress = true;
					_model.Refill(_startNode);
				}
			}
			catch
			{
			}
		}

		internal LDictionary<TreeTableViewNode, ITreeTableNode2> GetSelectedNodes()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			LDictionary<TreeTableViewNode, ITreeTableNode2> val = new LDictionary<TreeTableViewNode, ITreeTableNode2>();
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)_mappingTreeTable.SelectedNodes)
			{
				TreeTableViewNode val2 = item;
				ITreeTableNode modelNode = _mappingTreeTable.GetModelNode(val2);
				ITreeTableNode2 val3 = (ITreeTableNode2)(object)((modelNode is ITreeTableNode2) ? modelNode : null);
				if (val3 != null)
				{
					val.Add(val2, val3);
				}
			}
			return val;
		}

		private void _mappingTreeTable_DoubleClick(object sender, EventArgs e)
		{
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> selectedNode in GetSelectedNodes())
			{
				if (selectedNode.Value is DeviceNode)
				{
					DeviceNode deviceNode = selectedNode.Value as DeviceNode;
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(deviceNode.ISVNode.ProjectHandle, deviceNode.ISVNode.ObjectGuid);
					Debug.Assert(metaObjectStub != null);
					Debug.Assert(APEnvironment.Engine.Frame != null);
					Type objectType = metaObjectStub.ObjectType;
					Type[] embeddedObjectTypes = metaObjectStub.EmbeddedObjectTypes;
					Guid defaultEditorViewFactory = APEnvironment.Engine.Frame.ViewFactoryManager.GetDefaultEditorViewFactory(objectType, embeddedObjectTypes);
					APEnvironment.Engine.Frame.OpenEditorView(metaObjectStub, defaultEditorViewFactory, null);
				}
			}
		}

		private void _mappingTreeTable_Leave(object sender, EventArgs e)
		{
			CheckFocuseNode(bCancel: false);
		}

		private void _mappingTreeTable_AfterEditAccepted(object sender, TreeTableViewEditEventArgs e)
		{
			_iLastColumnIndex = e.ColumnIndex;
			_model.StorePendingChanges();
			int indexOfColumn = _model.GetIndexOfColumn(3);
			if (e.ColumnIndex == indexOfColumn)
			{
				_model.Refill(_startNode);
			}
		}

		private void _mappingTreeTable_BeforeEdit(object sender, TreeTableViewEditEventArgs e)
		{
			if (_bGetParameterInProgress)
			{
				((CancelEventArgs)(object)e).Cancel = true;
				return;
			}
			try
			{
				_bGetParameterInProgress = true;
				int indexOfColumn = _model.GetIndexOfColumn(7);
				if (e.ColumnIndex == indexOfColumn)
				{
					ParameterTreeTableModel.ChangeForcedValue(e);
					return;
				}
				indexOfColumn = _model.GetIndexOfColumn(6);
				if (e.ColumnIndex == indexOfColumn)
				{
					return;
				}
				DataElementNode dataElementNode = _mappingTreeTable.GetModelNode(_mappingTreeTable.FocusedNode) as DataElementNode;
				IParameterSet val = null;
				_bRefillInProgress = false;
				val = dataElementNode.ParameterSetProvider.GetParameterSet(bToModify: true);
				if (_bRefillInProgress)
				{
					bool isStoring = _model.IsStoring;
					try
					{
						_model.IsStoring = true;
						(dataElementNode.ParameterSetProvider as DeviceParameterSetProvider).StoreObject();
					}
					finally
					{
						_model.IsStoring = isStoring;
					}
					((CancelEventArgs)(object)e).Cancel = true;
				}
				if (val == null)
				{
					((CancelEventArgs)(object)e).Cancel = true;
				}
			}
			finally
			{
				_bGetParameterInProgress = false;
			}
		}

		private void _mappingTreeTable_ExpandCollapse(object sender, TreeTableViewEventArgs e)
		{
			if (_collapseTimer == null)
			{
				_collapseTimer = new Timer();
				_collapseTimer.Interval = 100;
				_collapseTimer.Start();
				_collapseTimer.Tick += _collapseTimer_Tick;
			}
			else
			{
				_collapseTimer.Stop();
				_collapseTimer.Start();
			}
		}

		private void _collapseTimer_Tick(object sender, EventArgs e)
		{
			_collapseTimer.Stop();
			if (_model.IsInRefill)
			{
				return;
			}
			bool doNotShrinkColumnsAutomatically = _mappingTreeTable.DoNotShrinkColumnsAutomatically;
			try
			{
				_mappingTreeTable.DoNotShrinkColumnsAutomatically=(true);
				foreach (ColumnHeader column in _mappingTreeTable.Columns)
				{
					_mappingTreeTable.AdjustColumnWidth(column.Index, true);
				}
			}
			finally
			{
				_mappingTreeTable.DoNotShrinkColumnsAutomatically=(doNotShrinkColumnsAutomatically);
			}
			_model.StoreExpandedNodes();
			_model.StartStopMonitoring();
		}

		private void _mappingTreeTable_Scroll(object sender, EventArgs e)
		{
			if (_model != null)
			{
				_model.StartStopMonitoring();
			}
		}

		private void _mappingTreeTable_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Return || _mappingTreeTable.FocusedNode == null || _iLastColumnIndex < 0)
			{
				return;
			}
			TreeTableViewNode val = _mappingTreeTable.FocusedNode;
			val.Selected=(false);
			if (val.NextVisibleNode != null)
			{
				val = val.NextVisibleNode;
			}
			else
			{
				while (val.PrevVisibleNode != null)
				{
					val = val.PrevVisibleNode;
				}
			}
			DataElementNode dataElementNode = null;
			do
			{
				dataElementNode = _mappingTreeTable.GetModelNode(val) as DataElementNode;
				if (dataElementNode != null && (_iLastColumnIndex != _model.GetIndexOfColumn(0) || DataElementNode.CheckForMultipleMapping(dataElementNode, bNoMessage: true)) && dataElementNode.IsEditable(_iLastColumnIndex))
				{
					break;
				}
				val = val.NextVisibleNode;
			}
			while (val != null);
			if (val != null && val != _mappingTreeTable.FocusedNode)
			{
				val.Selected=(true);
				val.Focus(_iLastColumnIndex);
				val.EnsureVisible(_iLastColumnIndex);
			}
			_iLastColumnIndex = -1;
		}

		private void Undo()
		{
			if (_undoMgr != null && _undoMgr.CanUndo)
			{
				_undoMgr.Undo();
				_model.StorePendingChanges();
			}
		}

		private void Redo()
		{
			if (_undoMgr != null && _undoMgr.CanRedo)
			{
				_undoMgr.Redo();
				_model.StorePendingChanges();
			}
		}

		private void Cut()
		{
			Copy();
			Delete();
		}

		private void Copy()
		{
			string text = string.Empty;
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			bool flag = true;
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (_mappingTreeTable.FocusedNode.IsFocused(out var nModelColumnIndex) && nModelColumnIndex == _model.GetIndexOfColumn(0))
				{
					IconLabelTreeTableViewCellData iconLabelTreeTableViewCellData = item.Key.CellValues[nModelColumnIndex] as IconLabelTreeTableViewCellData;
					if (!flag)
					{
						text += ",";
					}
					text += iconLabelTreeTableViewCellData.Label.ToString();
					flag = false;
				}
				if (_mappingTreeTable.FocusedNode.IsFocused(out nModelColumnIndex) && nModelColumnIndex == _model.GetIndexOfColumn(10))
				{
					string text2 = item.Key.CellValues[nModelColumnIndex] as string;
					if (!flag)
					{
						text += ",";
					}
					text += text2;
					flag = false;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				Clipboard.SetText(text);
			}
		}

		private void Paste()
		{
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			int nModelColumnIndex = -1;
			_mappingTreeTable.FocusedNode.IsFocused(out nModelColumnIndex);
			if (nModelColumnIndex == _model.GetIndexOfColumn(0))
			{
				foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
				{
					if (item.Value is DataElementNode node && !DataElementNode.CheckForMultipleMapping(node, bNoMessage: false))
					{
						return;
					}
				}
			}
			string text = Clipboard.GetText();
			if (text != string.Empty)
			{
				SetVariable(text);
			}
		}

		private void Delete()
		{
			SetVariable(string.Empty);
		}

		private void SetVariable(string stText)
		{
			string[] array = stText.Split(',');
			int num = 0;
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			int nModelColumnIndex = -1;
			using (LDictionary<TreeTableViewNode, ITreeTableNode2>.Enumerator enumerator = selectedNodes.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.Key.IsFocused(out nModelColumnIndex))
				{
				}
			}
			if (nModelColumnIndex == -1)
			{
				return;
			}
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (item.Value is DataElementNode dataElementNode && dataElementNode.PlcNode.OnlineApplication == Guid.Empty && dataElementNode.ParameterSetProvider.GetParameterSet(bToModify: true) != null)
				{
					if (array.Length > num)
					{
						dataElementNode.SetValue(nModelColumnIndex, array[num]);
					}
					else
					{
						dataElementNode.SetValue(nModelColumnIndex, string.Empty);
					}
					num++;
				}
			}
			_model.StorePendingChanges();
		}

		internal void CheckFocuseNode(bool bCancel)
		{
			if (_mappingTreeTable != null && _mappingTreeTable.FocusedNode != null)
			{
				TreeTableViewNode focusedNode = _mappingTreeTable.FocusedNode;
				int num = default(int);
				if (focusedNode != null && focusedNode.IsFocused(out num) && focusedNode.IsEditing(num))
				{
					focusedNode.EndEdit(num, bCancel);
				}
			}
		}

		private void _tbFilter_TextChanged(object sender, EventArgs e)
		{
			bool enabled = string.IsNullOrEmpty(_tbFilter.Text);
			_comboFilter.Enabled = enabled;
			if (!_tbFilter.Text.StartsWith("%") || _tbFilter.Text.Length != 1)
			{
				_model.SearchText = _tbFilter.Text;
				_delayTicks = DateTime.Now.Ticks;
				_bDoRefill = true;
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (_bDoRefill && DateTime.Now.Ticks - _delayTicks > 2500000)
			{
				_bDoRefill = false;
				SetVariableFilter(_comboFilter.SelectedItem as FilterItem);
			}
		}

		private void SetVariableFilter(FilterItem filter)
		{
			if (filter == null)
			{
				return;
			}
			for (int i = 0; i < _comboFilter.Items.Count; i++)
			{
				if (_comboFilter.Items[i] as FilterItem == filter)
				{
					_comboFilter.SelectedIndex = i;
				}
			}
			if (_model != null)
			{
				_model.VariableFilter = filter.MappingFilter;
				_model.Refill(_startNode);
			}
		}

		private void _comboFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_model != null)
			{
				SetVariableFilter(_comboFilter.SelectedItem as FilterItem);
			}
		}

		private void SimpleMappingForm_SizeChanged(object sender, EventArgs e)
		{
			if (_model != null)
			{
				_model.StartStopMonitoring();
			}
		}

		public string GetOnlineHelpKeyword()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			return "codesys.chm::/_cds_edt_device_io_mapping.htm";
		}

		private bool MenuEnabled(bool bEditable)
		{
			bool result = false;
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			if (selectedNodes.Count > 0)
			{
				result = true;
			}
			int nModelColumnIndex = -1;
			if (_mappingTreeTable.FocusedNode != null)
			{
				_mappingTreeTable.FocusedNode.IsFocused(out nModelColumnIndex);
			}
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (nModelColumnIndex != _model.GetIndexOfColumn(10) && (!(item.Value is DataElementNode) || (bEditable && !(item.Value as DataElementNode).VariableEditable())))
				{
					result = false;
				}
				if (nModelColumnIndex != _model.GetIndexOfColumn(0) && nModelColumnIndex != _model.GetIndexOfColumn(10))
				{
					result = false;
				}
			}
			return result;
		}

		private string GetSelectedMapping(out ITreeTableNode node)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			int num = default(int);
			if (_mappingTreeTable != null && _mappingTreeTable.FocusedNode != null && _mappingTreeTable.FocusedNode.IsFocused(out num) && num == _model.GetIndexOfColumn(0))
			{
				TreeTableViewNode focusedNode = _mappingTreeTable.FocusedNode;
				if (focusedNode != null)
				{
					DataElementNode dataElementNode = _mappingTreeTable.GetModelNode(focusedNode) as DataElementNode;
					if (dataElementNode != null && dataElementNode.DataElement != null)
					{
						foreach (IVariableMapping item in (IEnumerable)dataElementNode.DataElement.IoMapping.VariableMappings)
						{
							IVariableMapping val = item;
							if (!string.IsNullOrEmpty(val.Variable))
							{
								node = (ITreeTableNode)(object)dataElementNode;
								return val.Variable;
							}
						}
					}
				}
			}
			node = null;
			return string.Empty;
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Expected O, but got Unknown
			if (commandGuid == GUID_EDITCUT || commandGuid == GUID_EDITPASTE || commandGuid == GUID_EDITDELETE)
			{
				return MenuEnabled(bEditable: true);
			}
			if (commandGuid == GUID_EDITCOPY)
			{
				return MenuEnabled(bEditable: false);
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITUNDO)
			{
				return (_undoMgr != null) & _undoMgr.CanUndo;
			}
			if (commandGuid == IoMappingEditorPage.GUID_EDITREDO)
			{
				return (_undoMgr != null) & _undoMgr.CanRedo;
			}
			if (commandGuid == GUID_IMPORTCSV || commandGuid == GUID_EXPORTCSV || commandGuid == GUID_GOTODEFINITION || commandGuid == GUID_CROSSREFERENCES)
			{
				return true;
			}
			if (commandGuid == GUID_REFACTORING)
			{
				LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
				if (selectedNodes.Count == 1)
				{
					foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
					{
						if (!(item.Value is DataElementNode dataElementNode) || dataElementNode.DataElement.IoMapping.VariableMappings.Count <= 0)
						{
							continue;
						}
						foreach (IVariableMapping variableMapping in dataElementNode.DataElement.IoMapping.VariableMappings)
						{
							if (variableMapping.CreateVariable && !string.IsNullOrEmpty(variableMapping.Variable))
							{
								return true;
							}
						}
					}
				}
			}
			else if (commandGuid == GUID_EDITSELECTALL)
			{
				return true;
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
			else if (commandGuid == GUID_EDITREDO)
			{
				Redo();
			}
			else if (commandGuid == GUID_EDITUNDO)
			{
				Undo();
			}
			else if (commandGuid == GUID_EDITSELECTALL)
			{
				for (TreeTableViewNode val = _mappingTreeTable.TopNode; val != null; val = val.NextVisibleNode)
				{
					val.Selected=(true);
				}
			}
		}

		private void _mappingTreeTable_MouseUp(object sender, MouseEventArgs e)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			if (e.Button == MouseButtons.Right && _mappingTreeTable.FocusedNode != null)
			{
				_=_mappingTreeTable.FocusedNode;
				try
				{
					ContextMenuFilterCallback val = new ContextMenuFilterCallback(CanExecuteStandardCommand);
					((IEngine)APEnvironment.Engine).Frame.DisplayContextMenu(Guid.Empty, (Guid[])null, val, (Control)(object)_mappingTreeTable, e.Location);
				}
				catch
				{
				}
			}
		}

		void INotifyOnVisibilityChanged.OnVisibilityChanged(bool bVisible)
		{
			if (_model != null)
			{
				if (!bVisible)
				{
					Helper.SaveColumdWidths(_model);
					_model.StopMonitoring(bForceClose: true);
				}
				else
				{
					_model.StartStopMonitoring();
				}
			}
		}

		void INotifyOnVisibilityChanged.EnableVisibilityChangeNotification()
		{
		}

		public bool GetSelectionForBrowserCommands(out string expression, out Guid objectGuid, out IPreCompileContext pcc)
		{
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Expected O, but got Unknown
			expression = null;
			objectGuid = Guid.Empty;
			pcc = null;
			ITreeTableNode node;
			string selectedMapping = GetSelectedMapping(out node);
			if (!string.IsNullOrEmpty(selectedMapping) && node != null)
			{
				expression = selectedMapping;
				if (expression.Contains("."))
				{
					int num = expression.IndexOf('.') + 1;
					string text = expression.Substring(0, num - 1).ToLowerInvariant();
					expression = expression.Substring(num);
					int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
					foreach (Guid application in (node as DataElementNode).PlcNode.Applications)
					{
						if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, application) && ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, application).Name.ToLowerInvariant() == text)
						{
							while (!(node.Parent is DeviceNode))
							{
								node = node.Parent;
							}
							DeviceNode deviceNode = node.Parent as DeviceNode;
							if (deviceNode != null)
							{
								objectGuid = deviceNode.ISVNode.ObjectGuid;
								pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(application);
								return true;
							}
						}
					}
				}
				else
				{
					while (!(node.Parent is DeviceNode))
					{
						node = node.Parent;
					}
					DeviceNode deviceNode2 = node.Parent as DeviceNode;
					if (deviceNode2 != null)
					{
						IDeviceObject host = deviceNode2.GetHost();
						IDriverInfo4 val = (IDriverInfo4)((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
						if (val != null)
						{
							objectGuid = deviceNode2.ISVNode.ObjectGuid;
							pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IDriverInfo2)val).IoApplication);
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool GetSelectionForCrossReferenceCommand(out string expression, out Guid objectGuid)
		{
			ITreeTableNode node;
			string selectedMapping = GetSelectedMapping(out node);
			if (!string.IsNullOrEmpty(selectedMapping) && node != null)
			{
				expression = selectedMapping;
				while (!(node.Parent is DeviceNode))
				{
					node = node.Parent;
				}
				DeviceNode deviceNode = node.Parent as DeviceNode;
				if (deviceNode != null)
				{
					objectGuid = deviceNode.ISVNode.ObjectGuid;
					return true;
				}
			}
			expression = null;
			objectGuid = Guid.Empty;
			return false;
		}

		public bool GetArgsForImportCommand(out string[] commands)
		{
			commands = null;
			DeviceNode getFocusedDeviceNode = GetFocusedDeviceNode;
			if (getFocusedDeviceNode != null)
			{
				ISVNode iSVNode = getFocusedDeviceNode.ISVNode;
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.DefaultExt = ".csv";
				openFileDialog.Filter = "csv files (*.csv)|*.csv";
				openFileDialog.FileName = iSVNode.Name.Replace("/", "_");
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					commands = new string[3];
					commands[0] = openFileDialog.FileName;
					while (iSVNode.Parent != null)
					{
						iSVNode = iSVNode.Parent;
					}
					commands[1] = iSVNode.ProjectHandle.ToString();
					commands[2] = iSVNode.ObjectGuid.ToString();
					return true;
				}
			}
			return false;
		}

		public bool GetArgsForExportCommand(out string[] commands)
		{
			commands = null;
			DeviceNode getFocusedDeviceNode = GetFocusedDeviceNode;
			if (getFocusedDeviceNode != null)
			{
				ISVNode iSVNode = getFocusedDeviceNode.ISVNode;
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.DefaultExt = ".csv";
				saveFileDialog.Filter = "csv files (*.csv)|*.csv";
				saveFileDialog.FileName = iSVNode.Name.Replace("/", "_");
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					commands = new string[3];
					commands[0] = saveFileDialog.FileName;
					commands[1] = iSVNode.ProjectHandle.ToString();
					commands[2] = iSVNode.ObjectGuid.ToString();
					return true;
				}
			}
			return false;
		}

		private void _mappingTreeTable_ColumnWidthChanged(object sender, EventArgs e)
		{
			if (!_model.IsInRefill)
			{
				Helper.SaveColumdWidths(_model);
			}
		}

		public RefactoringContextType GetRefactoringContext(out Guid objectGuid, out string stVariableName)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (_mappingTreeTable.FocusedNode != null)
			{
				DataElementNode dataElementNode = _mappingTreeTable.GetModelNode(_mappingTreeTable.FocusedNode) as DataElementNode;
				if (dataElementNode != null)
				{
					return dataElementNode.GetRefactoringContext(out objectGuid, out stVariableName);
				}
			}
			stVariableName = string.Empty;
			objectGuid = Guid.Empty;
			return (RefactoringContextType)0;
		}

		private void _btAddFB_Click(object sender, EventArgs e)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Invalid comparison between Unknown and I4
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			if (selectedNodes == null || selectedNodes.Count <= 0)
			{
				return;
			}
			ChannelType val = (ChannelType)0;
			string text = string.Empty;
			IDeviceObject val2 = null;
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (item.Value is DataElementNode)
				{
					val = (item.Value as DataElementNode).Parameter.ChannelType;
					text = ((item.Value as DataElementNode).DataElement as IDataElement2).BaseType;
					val2 = (item.Value as DataElementNode).PlcNode?.PlcDevice;
					break;
				}
			}
			if (string.IsNullOrEmpty(text) || val2 == null)
			{
				return;
			}
			int projectHandle = ((IObject)val2).MetaObject.ProjectHandle;
			if (val2 == null)
			{
				return;
			}
			IDriverInfo driverInfo = ((IDeviceObject2)((val2 is IDeviceObject2) ? val2 : null)).DriverInfo;
			Guid ioApplication = ((IDriverInfo2)((driverInfo is IDriverInfo4) ? driverInfo : null)).IoApplication;
			if (!(ioApplication != Guid.Empty))
			{
				return;
			}
			FBCreateForm fBCreateForm = new FBCreateForm(projectHandle, ioApplication, text, (int)val != 1);
			if (fBCreateForm.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			FBCreateNode selectedFB = fBCreateForm.SelectedFB;
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item2 in selectedNodes)
			{
				if (item2.Value is DataElementNode node)
				{
					new MaintenanceFBAction(null, node, selectedFB, projectHandle, ioApplication).Redo();
				}
			}
		}

		private void _mappingTreeTable_SelectionChanged(object sender, EventArgs e)
		{
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			if (selectedNodes != null && selectedNodes.Count > 0)
			{
				bool enabled = true;
				string text = string.Empty;
				ChannelType channelType = ChannelType.None;
				foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
				{
					if (item.Value is DataElementNode)
					{
						if (channelType == ChannelType.None)
						{
							channelType = (item.Value as DataElementNode).Parameter.ChannelType;
						}
						else if (channelType != (item.Value as DataElementNode).Parameter.ChannelType)
						{
							enabled = false;
							break;
						}
						if ((item.Value as DataElementNode).DataElement is IDataElement2 dataElement && dataElement.HasBaseType)
						{
							if (string.IsNullOrEmpty(text))
							{
								text = dataElement.BaseType;
							}
							else if (string.Compare(text, dataElement.BaseType, StringComparison.InvariantCultureIgnoreCase) != 0)
							{
								enabled = false;
								break;
							}
							continue;
						}
						enabled = false;
						break;
					}
					enabled = false;
					break;
				}
				_btAddFB.Enabled = enabled;
			}
			else
			{
				_btAddFB.Enabled = false;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.SimpleMappingEditor.SimpleMappingView));
			_mappingTreeTable = new TreeTableView();
			Timer = new System.Windows.Forms.Timer(components);
			_onlineTimer = new System.Windows.Forms.Timer(components);
			_toolStrip = new System.Windows.Forms.ToolStrip();
			toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			_tbFilter = new System.Windows.Forms.ToolStripTextBox();
			toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			_comboFilter = new System.Windows.Forms.ToolStripComboBox();
			_btAddFB = new System.Windows.Forms.ToolStripButton();
			_toolStrip.SuspendLayout();
			SuspendLayout();
			_mappingTreeTable.AllowColumnReorder=(false);
			resources.ApplyResources(_mappingTreeTable, "_mappingTreeTable");
			_mappingTreeTable.AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_mappingTreeTable).BackColor = System.Drawing.SystemColors.Window;
			_mappingTreeTable.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			_mappingTreeTable.DoNotShrinkColumnsAutomatically=(false);
			_mappingTreeTable.ForceFocusOnClick=(false);
			_mappingTreeTable.GridLines=(true);
			_mappingTreeTable.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Nonclickable);
			_mappingTreeTable.ImmediateEdit=(true);
			_mappingTreeTable.Indent=(20);
			_mappingTreeTable.KeepColumnWidthsAdjusted=(false);
			_mappingTreeTable.Model=((ITreeTableModel)null);
			_mappingTreeTable.MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_mappingTreeTable).Name = "_mappingTreeTable";
			_mappingTreeTable.NoSearchStrings=(true);
			_mappingTreeTable.OnlyWhenFocused=(false);
			_mappingTreeTable.OpenEditOnDblClk=(true);
			_mappingTreeTable.ReadOnly=(false);
			_mappingTreeTable.Scrollable=(true);
			_mappingTreeTable.ShowLines=(true);
			_mappingTreeTable.ShowPlusMinus=(true);
			_mappingTreeTable.ShowRootLines=(true);
			_mappingTreeTable.ToggleOnDblClk=(false);
			_mappingTreeTable.BeforeEdit+=(new TreeTableViewEditEventHandler(_mappingTreeTable_BeforeEdit));
			_mappingTreeTable.AfterEditAccepted+=(new TreeTableViewEditEventHandler(_mappingTreeTable_AfterEditAccepted));
			_mappingTreeTable.SelectionChanged+=(new System.EventHandler(_mappingTreeTable_SelectionChanged));
			_mappingTreeTable.AfterCollapse+=(new TreeTableViewEventHandler(_mappingTreeTable_ExpandCollapse));
			_mappingTreeTable.AfterExpand+=(new TreeTableViewEventHandler(_mappingTreeTable_ExpandCollapse));
			_mappingTreeTable.Scroll+=(new System.EventHandler(_mappingTreeTable_Scroll));
			_mappingTreeTable.ColumnWidthChanged+=(new System.EventHandler(_mappingTreeTable_ColumnWidthChanged));
			((System.Windows.Forms.Control)(object)_mappingTreeTable).DoubleClick += new System.EventHandler(_mappingTreeTable_DoubleClick);
			((System.Windows.Forms.Control)(object)_mappingTreeTable).KeyUp += new System.Windows.Forms.KeyEventHandler(_mappingTreeTable_KeyUp);
			((System.Windows.Forms.Control)(object)_mappingTreeTable).Leave += new System.EventHandler(_mappingTreeTable_Leave);
			((System.Windows.Forms.Control)(object)_mappingTreeTable).MouseUp += new System.Windows.Forms.MouseEventHandler(_mappingTreeTable_MouseUp);
			Timer.Enabled = true;
			Timer.Tick += new System.EventHandler(Timer_Tick);
			_onlineTimer.Enabled = true;
			_onlineTimer.Tick += new System.EventHandler(_onlineTimer_Tick);
			_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			_toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { toolStripLabel1, _tbFilter, toolStripLabel2, _comboFilter, _btAddFB });
			resources.ApplyResources(_toolStrip, "_toolStrip");
			_toolStrip.Name = "_toolStrip";
			toolStripLabel1.Name = "toolStripLabel1";
			resources.ApplyResources(toolStripLabel1, "toolStripLabel1");
			_tbFilter.Name = "_tbFilter";
			resources.ApplyResources(_tbFilter, "_tbFilter");
			_tbFilter.TextChanged += new System.EventHandler(_tbFilter_TextChanged);
			toolStripLabel2.Name = "toolStripLabel2";
			resources.ApplyResources(toolStripLabel2, "toolStripLabel2");
			_comboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_comboFilter.Items.AddRange(new object[5]
			{
				resources.GetString("_comboFilter.Items"),
				resources.GetString("_comboFilter.Items1"),
				resources.GetString("_comboFilter.Items2"),
				resources.GetString("_comboFilter.Items3"),
				resources.GetString("_comboFilter.Items4")
			});
			_comboFilter.Name = "_comboFilter";
			resources.ApplyResources(_comboFilter, "_comboFilter");
			_comboFilter.SelectedIndexChanged += new System.EventHandler(_comboFilter_SelectedIndexChanged);
			_btAddFB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			resources.ApplyResources(_btAddFB, "_btAddFB");
			_btAddFB.Name = "_btAddFB";
			_btAddFB.Click += new System.EventHandler(_btAddFB_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			base.Controls.Add(_toolStrip);
			base.Controls.Add((System.Windows.Forms.Control)(object)_mappingTreeTable);
			base.Name = "SimpleMappingView";
			resources.ApplyResources(this, "$this");
			base.SizeChanged += new System.EventHandler(SimpleMappingForm_SizeChanged);
			_toolStrip.ResumeLayout(false);
			_toolStrip.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
