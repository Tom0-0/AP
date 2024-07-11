#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Bookmarks;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using _3S.CoDeSys.TextDocument;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.WatchList;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[TypeGuid("{63042AFC-9261-421e-BCB9-76C151F14973}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_declaration_editor_basics.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/core_vardeclobject_editor_home.htm")]
	[AssociatedOnlineHelpTopic("core.VarDeclObject.Editor.chm::/home.htm")]
	public class TabularDeclarationEditor : UserControl, ITabularDeclarationEditor2, ITabularDeclarationEditor, IEditorView, IView, IEditor, IHasOnlineMode, IHasBookmarks, IPrintable, IEditorBasedFindReplace, INotifyOnVisibilityChanged, ISupportsMultiSelection, ILocalizableEditor, ISupportOnlineInstancePreselection
	{
		private class ScopeMenuData
		{
			internal readonly VariableListNode Node;

			internal readonly ModelTokenType Scope;

			internal ScopeMenuData(VariableListNode node, ModelTokenType scope)
			{
				Node = node;
				Scope = scope;
			}
		}

		private IEditor _editor;

		private Icon _smallIcon;

		private Icon _largeIcon;

		private ITextDocumentProvider _textDocumentProvider;

		private UnresolvedDeclarationContext _unresolvedContext;

		private bool _bInitialized;

		private OnlineState _onlineState;

		private TabularDeclarationModel _tabularDeclarationModel;

		private VariableListModel _variableListModel;

		private bool _bInScopeContextMenu;

		private bool _bLookingForwardForScopeContextMenu;

		private TreeTableViewNode _pendingNewViewNode;

		private int[] _selectedNodeIndices = new int[0];

		private bool _bPerformFocusCorrection;

		private IWatchListView _watchListView;

		private bool _bVisibilityChangeNotificationEnabled;

		private bool _bHasUnrecoverableError;

		private ToolStripSpringButton _headerButtonItem;

		private static Font s_boldFont;

		private static readonly ModelTokenType[] SCOPES = new ModelTokenType[10]
		{
			ModelTokenType.Var,
			ModelTokenType.VarAccess,
			ModelTokenType.VarConfig,
			ModelTokenType.VarExternal,
			ModelTokenType.VarGlobal,
			ModelTokenType.VarInOut,
			ModelTokenType.VarInput,
			ModelTokenType.VarOutput,
			ModelTokenType.VarStat,
			ModelTokenType.VarTemp
		};

		private static readonly Guid GUID_EDITUNDOCOMMAND = new Guid("{9ECCAF22-3293-4165-943E-88C2C40B4A58}");

		private static readonly Guid GUID_EDITREDOCOMMAND = new Guid("{871B29A1-9E9F-47f9-A5CE-D56C40976743}");

		private static readonly Guid GUID_EDITCUTCOMMAND = new Guid("{586FB001-60CA-4dd1-A2F9-F9319EE13C95}");

		private static readonly Guid GUID_EDITCOPYCOMMAND = new Guid("{E76B8E0B-9247-41e7-93D5-80FE36AF9449}");

		private static readonly Guid GUID_EDITPASTECOMMAND = new Guid("{73A7678B-2707-44f6-963B-8A4B3C05A331}");

		private static readonly Guid GUID_EDITDELETECOMMAND = new Guid("{C5AAECF0-F55A-4864-871E-4584D1CCC9AF}");

		private static readonly Guid GUID_EDITSELECTALLCOMMAND = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		private IMetaObject _localizedObject;

		private bool _bLocalizationActive;

		private bool _bIsEditing;

		private IContainer components;

		private Panel _correctPanel;

		private TreeTableView _declarationTable;

		private Panel _incorrectPanel;

		private Label _reasonLabel;

		private PictureBox _pictureBox;

		private ToolStrip _toolBar;

		private ToolStripButton _insertButtonItem;

		private ToolStripButton _moveUpButtonItem;

		private ToolStripButton _moveDownButtonItem;

		private ToolStripButton _deleteButtonItem;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripSeparator toolStripSeparator5;

		public IEditor Editor
		{
			get
			{
				if (!_bInitialized)
				{
					throw new InvalidOperationException("This instance has not been initialized yet.");
				}
				return _editor;
			}
		}

		public Control Control => this;

		public Control[] Panes => new Control[1] { (Control)(object)_declarationTable };

		public string Caption => string.Empty;

		public string Description => string.Empty;

		public Icon SmallIcon
		{
			get
			{
				if (!_bInitialized)
				{
					throw new InvalidOperationException("This instance has not been initialized yet.");
				}
				return _smallIcon;
			}
		}

		public Icon LargeIcon
		{
			get
			{
				if (!_bInitialized)
				{
					throw new InvalidOperationException("This instance has not been initialized yet.");
				}
				return _largeIcon;
			}
		}

		private bool CanUndo
		{
			get
			{
				if (HasUnrecoverableError)
				{
					return false;
				}
				if (_onlineState.OnlineApplication != Guid.Empty)
				{
					return false;
				}
				if (ReadOnly)
				{
					return false;
				}
				return _variableListModel.UndoMgr.CanUndo;
			}
		}

		private bool CanRedo
		{
			get
			{
				if (HasUnrecoverableError)
				{
					return false;
				}
				if (_onlineState.OnlineApplication != Guid.Empty)
				{
					return false;
				}
				if (ReadOnly)
				{
					return false;
				}
				return _variableListModel.UndoMgr.CanRedo;
			}
		}

		private bool CanCut
		{
			get
			{
				if (CanCopy)
				{
					return CanDelete;
				}
				return false;
			}
		}

		private bool CanCopy
		{
			get
			{
				if (!HasUnrecoverableError)
				{
					return SelectedNodeIndices.Length != 0;
				}
				return false;
			}
		}

		private bool CanPaste
		{
			get
			{
				if (HasUnrecoverableError)
				{
					return false;
				}
				if (_onlineState.OnlineApplication != Guid.Empty)
				{
					return false;
				}
				if (_tabularDeclarationModel.List == null)
				{
					return false;
				}
				if (ReadOnly)
				{
					return false;
				}
				return Clipboard.GetDataObject()?.GetDataPresent("TabularDeclarations") ?? false;
			}
		}

		private bool CanDelete
		{
			get
			{
				if (HasUnrecoverableError)
				{
					return false;
				}
				if (_onlineState.OnlineApplication != Guid.Empty)
				{
					return false;
				}
				if (ReadOnly)
				{
					return false;
				}
				if (SelectedNodeIndices.Length != 0)
				{
					return _pendingNewViewNode == null;
				}
				return false;
			}
		}

		private bool CanSelectAll => !HasUnrecoverableError;

		public DockingPosition DefaultDockingPosition => (DockingPosition)32;

		public DockingPosition PossibleDockingPositions => (DockingPosition)63;

		public int ProjectHandle => Editor.ProjectHandle;

		public Guid ObjectGuid => Editor.ObjectGuid;

		internal bool HasUnrecoverableError => _bHasUnrecoverableError;

		public OnlineState OnlineState
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _onlineState;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_onlineState = value;
				AdaptOnlineMode();
			}
		}

		internal TabularDeclarationModel TabularDeclarationModel => _tabularDeclarationModel;

		private int[] SelectedNodeIndices
		{
			get
			{
				if (_selectedNodeIndices == null)
				{
					LList<int> val = new LList<int>();
					for (int i = 0; i < _declarationTable.Nodes.Count; i++)
					{
						if (_declarationTable.Nodes[i].Selected && _declarationTable.GetModelNode(_declarationTable.Nodes[i]) is VariableListNode)
						{
							val.Add(i);
						}
					}
					_selectedNodeIndices = val.ToArray();
				}
				return _selectedNodeIndices;
			}
		}

		internal bool CanInsert
		{
			get
			{
				if (!HasUnrecoverableError && _onlineState.OnlineApplication == Guid.Empty && !ReadOnly && SelectedNodeIndices.Length <= 1 && _tabularDeclarationModel.List != null)
				{
					return _pendingNewViewNode == null;
				}
				return false;
			}
		}

		internal bool CanMoveUp
		{
			get
			{
				if (!HasUnrecoverableError && _onlineState.OnlineApplication == Guid.Empty && !ReadOnly && SelectedNodeIndices.Length != 0 && SelectedNodeIndices[0] > 0 && _tabularDeclarationModel.List != null)
				{
					return _pendingNewViewNode == null;
				}
				return false;
			}
		}

		internal bool CanMoveDown
		{
			get
			{
				if (!HasUnrecoverableError && _onlineState.OnlineApplication == Guid.Empty && !ReadOnly && SelectedNodeIndices.Length != 0 && SelectedNodeIndices[SelectedNodeIndices.Length - 1] < _declarationTable.Nodes.Count - 2 && _tabularDeclarationModel.List != null)
				{
					return _pendingNewViewNode == null;
				}
				return false;
			}
		}

		internal bool CanEditDeclarationHeader => !HasUnrecoverableError;

		private int PrimaryColumn
		{
			get
			{
				int num = _variableListModel.GetColumnIndex(VariableListColumns.Name);
				if (num < 0 && ((AbstractTreeTableModel)_variableListModel).ColumnCount > 1)
				{
					num = 1;
				}
				if (num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		public bool ReadOnly
		{
			get
			{
				if (_variableListModel != null && !_variableListModel.ReadOnly)
				{
					return _declarationTable.ReadOnly;
				}
				return true;
			}
			set
			{
				if (_variableListModel != null)
				{
					_variableListModel.ReadOnly = value;
				}
				_declarationTable.ReadOnly=(value);
			}
		}

		public bool IsEditing => _bIsEditing;

		public TabularDeclarationEditor()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			InitializeComponent();
			_toolBar.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
			_correctPanel.Visible = false;
			_incorrectPanel.Visible = false;
			_pictureBox.Image = Resources.Error;
			_headerButtonItem = new ToolStripSpringButton();
			_toolBar.Items.Add(_headerButtonItem);
			_headerButtonItem.Click += _headerButtonItem_Click;
			if (s_boldFont == null)
			{
				s_boldFont = new Font(Font, FontStyle.Bold | FontStyle.Underline);
			}
		}

		public void Initialize(IEditor editor, Icon smallIcon, Icon largeIcon, ITextDocumentProvider textDocumentProvider, UnresolvedDeclarationContext context)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (editor == null)
			{
				throw new ArgumentNullException("editor");
			}
			if (textDocumentProvider == null)
			{
				throw new ArgumentNullException("textDocumentProvider");
			}
			_editor = editor;
			_smallIcon = smallIcon;
			_largeIcon = largeIcon;
			_textDocumentProvider = textDocumentProvider;
			_unresolvedContext = context;
			_tabularDeclarationModel = new TabularDeclarationModel(_unresolvedContext);
			_variableListModel = new VariableListModel(this, _unresolvedContext);
			_declarationTable.Model=((ITreeTableModel)(object)_variableListModel);
			_bInitialized = true;
			_bIsEditing = false;
		}

		public void Mark(long nPosition, int nLength, object tag)
		{
		}

		public void UnmarkAll(object tag)
		{
		}

		public void Select(long nPosition, int nLength)
		{
			ITextDocument textDocument = GetTextDocument(bToModify: false);
			Debug.Assert(textDocument != null);
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			int num3 = textDocument.FindLineById(num);
			int nLine = -1;
			int nColumn = -1;
			if (num3 >= 0)
			{
				int nOffset = textDocument.GetLineStartOffset(num3) + num2;
				if (!_variableListModel.FindByOffset(nOffset, out nLine, out nColumn))
				{
					nLine = -1;
					nColumn = -1;
				}
			}
			if (nLine < 0)
			{
				nLine = _variableListModel.FindLineByPosition(nPosition);
				nColumn = _variableListModel.GetColumnIndex(VariableListColumns.Name);
			}
			if (nColumn < 0)
			{
				nColumn = 0;
			}
			if (nLine >= 0)
			{
				_declarationTable.DeselectAll();
				_declarationTable.Nodes[nLine].Selected=(true);
				if (nColumn >= 0)
				{
					_declarationTable.Nodes[nLine].Focus(nColumn);
				}
				_declarationTable.Nodes[nLine].EnsureVisible(nColumn);
				((Control)(object)_declarationTable).Focus();
			}
		}

		internal int GetSelectedNodeCount()
		{
			return ((TreeTableViewNodeCollection)_declarationTable.SelectedNodes).Count;
		}

		public void GetSelection(out long nPosition, out int nLength)
		{
			int column = default(int);
			TreeTableViewNode focusedNode = _declarationTable.GetFocusedNode(out column);
			GetSelectionDataForNode(focusedNode, column, out nPosition, out nLength);
		}

		public int ComparePositions(long nPosition1, long nPosition2)
		{
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition1, out num, out num2);
			long num3 = default(long);
			short num4 = default(short);
			PositionHelper.SplitPosition(nPosition2, out num3, out num4);
			ITextDocument textDocument = GetTextDocument(bToModify: false);
			Debug.Assert(textDocument != null);
			int num5 = textDocument.FindLineById(num);
			int num6 = textDocument.FindLineById(num3);
			if (num5 != num6)
			{
				return num5 - num6;
			}
			return num2 - num4;
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITUNDOCOMMAND)
			{
				return CanUndo;
			}
			if (commandGuid == GUID_EDITREDOCOMMAND)
			{
				return CanRedo;
			}
			if (commandGuid == GUID_EDITCUTCOMMAND)
			{
				return CanCut;
			}
			if (commandGuid == GUID_EDITCOPYCOMMAND)
			{
				return CanCopy;
			}
			if (commandGuid == GUID_EDITPASTECOMMAND)
			{
				return CanPaste;
			}
			if (commandGuid == GUID_EDITDELETECOMMAND)
			{
				return CanDelete;
			}
			if (commandGuid == GUID_EDITSELECTALLCOMMAND)
			{
				return CanSelectAll;
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITUNDOCOMMAND)
			{
				Undo();
			}
			else if (commandGuid == GUID_EDITREDOCOMMAND)
			{
				Redo();
			}
			else if (commandGuid == GUID_EDITCUTCOMMAND)
			{
				Cut();
			}
			else if (commandGuid == GUID_EDITCOPYCOMMAND)
			{
				Copy();
			}
			else if (commandGuid == GUID_EDITPASTECOMMAND)
			{
				Paste();
			}
			else if (commandGuid == GUID_EDITDELETECOMMAND)
			{
				Delete();
			}
			else if (commandGuid == GUID_EDITSELECTALLCOMMAND)
			{
				SelectAll();
			}
		}

		private void Undo()
		{
			try
			{
				_declarationTable.BeginUpdate();
				SelectNode(_variableListModel.UndoMgr.Undo());
			}
			finally
			{
				_variableListModel.AdjustLineNumbers();
				_declarationTable.EndUpdate();
			}
		}

		private void Redo()
		{
			try
			{
				_declarationTable.BeginUpdate();
				SelectNode(_variableListModel.UndoMgr.Redo());
			}
			finally
			{
				_variableListModel.AdjustLineNumbers();
				_declarationTable.EndUpdate();
			}
		}

		private void Cut()
		{
			Copy();
			Delete();
		}

		private void Copy()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			SerializableTabularDeclarationItem[] array = new SerializableTabularDeclarationItem[SelectedNodeIndices.Length];
			string[] array2 = new string[SelectedNodeIndices.Length];
			for (int i = 0; i < SelectedNodeIndices.Length; i++)
			{
				VariableListNode variableListNode = ((AbstractTreeTableModel)_variableListModel).Sentinel.GetChild(SelectedNodeIndices[i]) as VariableListNode;
				Debug.Assert(variableListNode != null);
				array[i] = new SerializableTabularDeclarationItem(variableListNode.Item);
				array2[i] = variableListNode.Name;
			}
			ClipboardData clipboardData = new ClipboardData(array, array2);
			IArchiveWriter obj = APEnvironment.CreateBinaryArchiveWriter();
			Debug.Assert(obj != null);
			ChunkedMemoryStream val = new ChunkedMemoryStream();
			obj.Initialize((Stream)(object)val, Encoding.UTF8);
			obj.Save((IArchivable)(object)clipboardData);
			((Stream)(object)val).Close();
			Clipboard.SetDataObject(new DataObject("TabularDeclarations", val.ToArray()), copy: true);
		}

		private void Paste()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			if (!CheckAllowStructuralModifications(Resources.CannotInsertItems))
			{
				return;
			}
			try
			{
				_declarationTable.BeginUpdate();
				IDataObject dataObject = Clipboard.GetDataObject();
				ChunkedMemoryStream val = new ChunkedMemoryStream((dataObject != null) ? (dataObject.GetData("TabularDeclarations") as byte[]) : null);
				IArchiveReader obj = APEnvironment.CreateBinaryArchiveReader();
				Debug.Assert(obj != null);
				obj.Initialize((Stream)(object)val);
				ClipboardData clipboardData = obj.Load() as ClipboardData;
				ITextDocument textDocument = GetTextDocument(bToModify: true);
				if (clipboardData == null || clipboardData.Items == null || clipboardData.Items.Length == 0 || textDocument == null)
				{
					return;
				}
				int nIndex = ((SelectedNodeIndices.Length != 0) ? SelectedNodeIndices[0] : (_declarationTable.Nodes.Count - 1));
				VariableListNode[] array = _variableListModel.Paste(nIndex, clipboardData);
				_declarationTable.DeselectAll();
				VariableListNode[] array2 = array;
				foreach (VariableListNode variableListNode in array2)
				{
					TreeTableViewNode viewNode = _declarationTable.GetViewNode((ITreeTableNode)(object)variableListNode);
					if (viewNode != null)
					{
						viewNode.Selected=(true);
						viewNode.Focus(PrimaryColumn);
					}
				}
			}
			catch
			{
			}
			finally
			{
				_declarationTable.EndUpdate();
			}
		}

		private void Delete()
		{
			try
			{
				_declarationTable.BeginUpdate();
				int[] selectedNodeIndices = SelectedNodeIndices;
				_variableListModel.Delete(selectedNodeIndices);
				_declarationTable.DeselectAll();
				if (selectedNodeIndices.Length != 0)
				{
					int num = selectedNodeIndices[selectedNodeIndices.Length - 1];
					if (num >= _declarationTable.Nodes.Count)
					{
						num = _declarationTable.Nodes.Count - 1;
					}
					if (num >= 0)
					{
						SelectNode(num);
					}
				}
			}
			finally
			{
				_declarationTable.EndUpdate();
			}
		}

		private void SelectAll()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_declarationTable.BeginUpdate();
				foreach (TreeTableViewNode node in _declarationTable.Nodes)
				{
					node.Selected=(true);
					node.Focus(PrimaryColumn);
				}
			}
			finally
			{
				_declarationTable.EndUpdate();
			}
		}

		public void SetObject(int nProjectHandle, Guid objectGuid)
		{
		}

		public void Reload()
		{
			Reload(bRefillVariableListModel: true);
		}

		internal bool Reload(bool bRefillVariableListModel)
		{
			_bHasUnrecoverableError = false;
			ITextDocument textDocument = GetTextDocument(bToModify: false);
			if (_tabularDeclarationModel.ReadText(textDocument, out var stMessage, out var resolvedContext))
			{
				UpdateHeaderButtonItem();
				if (bRefillVariableListModel)
				{
					TreeTableViewNode val = ((_declarationTable.FocusedNode != null) ? _declarationTable.FocusedNode : ((((TreeTableViewNodeCollection)_declarationTable.SelectedNodes).Count > 0) ? ((TreeTableViewNodeCollection)_declarationTable.SelectedNodes)[0] : null));
					int val2 = ((val != null) ? val.Index : 0);
					try
					{
						_declarationTable.BeginUpdate();
						_variableListModel.Refill(_tabularDeclarationModel.List, resolvedContext);
						for (int i = 0; i < _declarationTable.Columns.Count; i++)
						{
							_declarationTable.AdjustColumnWidth(i, true);
						}
						if (_tabularDeclarationModel.List != null)
						{
							((Control)(object)_declarationTable).Enabled = true;
							val2 = Math.Min(val2, _declarationTable.Nodes.Count - 1);
							if (_declarationTable.Nodes.Count > 0)
							{
								_declarationTable.Nodes[val2].Selected=(true);
								_declarationTable.Nodes[val2].Focus(PrimaryColumn);
								_declarationTable.Nodes[val2].EnsureVisible(PrimaryColumn);
							}
						}
						else
						{
							((Control)(object)_declarationTable).Enabled = false;
						}
					}
					finally
					{
						_declarationTable.EndUpdate();
					}
				}
				else
				{
					_variableListModel.ResolvedContext = resolvedContext;
				}
				_correctPanel.Visible = true;
				_incorrectPanel.Visible = false;
				_bIsEditing = false;
				UpdateControlStates();
				return true;
			}
			_correctPanel.Visible = false;
			_incorrectPanel.Visible = true;
			_reasonLabel.Text = stMessage;
			_bHasUnrecoverableError = true;
			_bIsEditing = false;
			UpdateControlStates();
			return false;
		}

		public void Save(bool bCommit)
		{
		}

		public IMetaObject GetObjectToRead()
		{
			return Editor.GetObjectToRead();
		}

		public IMetaObject GetObjectToModify()
		{
			return Editor.GetObjectToModify();
		}

		public bool SelectOnlineState(IOnlineUIServices uiServices, out OnlineState onlineState)
		{
			if (uiServices == null)
			{
				throw new ArgumentNullException("uiServices");
			}
			return uiServices.SelectOnlineState(Editor.ProjectHandle, Editor.ObjectGuid, out onlineState);
		}

		private void AdaptOnlineMode()
		{
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			if (!base.IsHandleCreated)
			{
				return;
			}
			if (_onlineState.OnlineApplication == Guid.Empty || _onlineState.InstancePath == null)
			{
				base.Controls.Clear();
				base.Controls.Add(_correctPanel);
				base.Controls.Add(_incorrectPanel);
				base.Controls.Add(_toolBar);
				if (_watchListView != null)
				{
					((IDisposable)_watchListView).Dispose();
					_watchListView = null;
				}
				return;
			}
			if (_watchListView == null)
			{
				_watchListView = APEnvironment.CreateWatchListView();
			}
			Debug.Assert(_watchListView != null);
			Debug.Assert(((IView)_watchListView).Control != null);
			_watchListView.PersistenceGuid=(Guid.Empty);
			_watchListView.ReadOnly=(true);
			((IView)_watchListView).Control.Dock = DockStyle.Fill;
			_watchListView.InstancePath=(_onlineState.InstancePath);
			if (_watchListView is IWatchListView6)
			{
				((IWatchListView6)_watchListView).SetObject(ProjectHandle, ObjectGuid);
			}
			if (_watchListView is INotifyOnVisibilityChanged && _bVisibilityChangeNotificationEnabled)
			{
				((INotifyOnVisibilityChanged)_watchListView).EnableVisibilityChangeNotification();
				((INotifyOnVisibilityChanged)_watchListView).OnVisibilityChanged(true);
			}
			if (_watchListView is IWatchListView7)
			{
				((IWatchListView7)_watchListView).ApplicationGuid=(_onlineState.OnlineApplication);
			}
			if (_watchListView is ILocalizableEditor)
			{
				((ILocalizableEditor)_watchListView).SetLocalizedObject(_localizedObject, _bLocalizationActive);
			}
			_watchListView.Refill();
			base.Controls.Clear();
			base.Controls.Add(((IView)_watchListView).Control);
		}

		public long ModifyPositionForBookmark(long nPosition)
		{
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			return PositionHelper.CombinePosition(num, (short)0);
		}

		public IPrintPainter CreatePrintPainter(long nPosition, int nLength)
		{
			return null;
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			return false;
		}

		public void OnVisibilityChanged(bool bVisible)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_watchListView is INotifyOnVisibilityChanged)
			{
				((INotifyOnVisibilityChanged)_watchListView).OnVisibilityChanged(bVisible);
			}
		}

		public void EnableVisibilityChangeNotification()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			_bVisibilityChangeNotificationEnabled = true;
			if (_watchListView is INotifyOnVisibilityChanged)
			{
				((INotifyOnVisibilityChanged)_watchListView).EnableVisibilityChangeNotification();
			}
		}

		public ITextDocument GetTextDocument(bool bToModify)
		{
			if (!_bInitialized)
			{
				throw new InvalidOperationException("This instance has not been initialized yet.");
			}
			if (_bLocalizationActive)
			{
				if (_localizedObject == null)
				{
					return null;
				}
				return _textDocumentProvider.GetTextDocument(_localizedObject.Object);
			}
			IMetaObject val = null;
			val = ((!bToModify) ? Editor.GetObjectToRead() : Editor.GetObjectToModify());
			if (val == null)
			{
				return null;
			}
			return _textDocumentProvider.GetTextDocument(val.Object);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			base.OnHandleCreated(e);
			APEnvironment.BookmarkMgr.BookmarkSet+=(new BookmarkEventHandler(OnBookmarkSet));
			APEnvironment.BookmarkMgr.BookmarkCleared+=(new BookmarkEventHandler(OnBookmarkCleared));
			AdaptOnlineMode();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			base.OnHandleDestroyed(e);
			APEnvironment.BookmarkMgr.BookmarkSet-=(new BookmarkEventHandler(OnBookmarkSet));
			APEnvironment.BookmarkMgr.BookmarkCleared-=(new BookmarkEventHandler(OnBookmarkCleared));
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			Invalidate(invalidateChildren: true);
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			Invalidate(invalidateChildren: true);
			_bIsEditing = false;
		}

		private void DisposeCore()
		{
			if (_variableListModel != null)
			{
				_variableListModel.Dispose();
				_variableListModel = null;
			}
		}

		private void UpdateHeaderButtonItem()
		{
			if (_tabularDeclarationModel.Header != null)
			{
				_headerButtonItem.Text = _tabularDeclarationModel.Header.GetNormalizedText();
				_headerButtonItem.Enabled = true;
			}
			else
			{
				_headerButtonItem.Text = string.Empty;
				_headerButtonItem.Enabled = false;
			}
		}

		private void _headerButtonItem_Click(object sender, EventArgs e)
		{
			EditDeclarationHeader();
		}

		private void _declarationTable_MouseDown(object sender, MouseEventArgs e)
		{
			_bLookingForwardForScopeContextMenu = false;
			if (e.Button == MouseButtons.Left)
			{
				int num = default(int);
				TreeTableViewNode nodeAt = _declarationTable.GetNodeAt(e.X, e.Y, out num);
				if (nodeAt != null)
				{
					_bLookingForwardForScopeContextMenu = nodeAt.Selected;
				}
			}
		}

		private void _declarationTable_MouseUp(object sender, MouseEventArgs e)
		{
			if (_variableListModel == null)
			{
				return;
			}
			if (e.Button == MouseButtons.Left)
			{
				if (_bLookingForwardForScopeContextMenu)
				{
					_bLookingForwardForScopeContextMenu = false;
					int num = default(int);
					TreeTableViewNode nodeAt = _declarationTable.GetNodeAt(e.X, e.Y, out num);
					if (nodeAt != null && nodeAt.Selected && _variableListModel.GetColumnMeaning(num) == VariableListColumns.Scope)
					{
						DisplayScopeContextMenu(nodeAt, num);
					}
				}
			}
			else if (_tabularDeclarationModel.List != null)
			{
				APEnvironment.Engine.Frame.DisplayContextMenu(Guid.Empty, (Guid[])null, (ContextMenuFilterCallback)null, (Control)(object)_declarationTable, new Point(e.X, e.Y));
			}
		}

		private void DisplayScopeContextMenu(TreeTableViewNode viewNode, int nColumnIndex)
		{
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Expected O, but got Unknown
			if (_bInScopeContextMenu)
			{
				return;
			}
			VariableListNode variableListNode = _declarationTable.GetModelNode(viewNode) as VariableListNode;
			if (variableListNode == null || ReadOnly)
			{
				return;
			}
			try
			{
				_bInScopeContextMenu = true;
				ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
				ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
				toolStripMenuItem.Text = Resources.ScopeColon;
				toolStripMenuItem.Enabled = false;
				toolStripMenuItem.Font = s_boldFont;
				contextMenuStrip.Items.Add(toolStripMenuItem);
				ModelTokenType[] sCOPES = SCOPES;
				foreach (ModelTokenType modelTokenType in sCOPES)
				{
					if ((modelTokenType & _variableListModel.GetAllowedScopes(variableListNode)) != (ModelTokenType)0L)
					{
						ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
						toolStripMenuItem2.Tag = new ScopeMenuData(variableListNode, modelTokenType);
						toolStripMenuItem2.Text = Common.GetScopeText(modelTokenType, bConstant: false, bRetain: false, bPersistent: false);
						toolStripMenuItem2.Image = _variableListModel.GetScopeImage(modelTokenType);
						toolStripMenuItem2.Checked = variableListNode.Scope == modelTokenType;
						toolStripMenuItem2.Click += OnScopeMenuItemClick;
						contextMenuStrip.Items.Add(toolStripMenuItem2);
					}
				}
				ToolStripSeparator value = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value);
				ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
				toolStripMenuItem3.Text = Resources.FlagsColon;
				toolStripMenuItem3.Enabled = false;
				toolStripMenuItem3.Font = s_boldFont;
				contextMenuStrip.Items.Add(toolStripMenuItem3);
				ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
				toolStripMenuItem4.Tag = new ScopeMenuData(variableListNode, ModelTokenType.Constant);
				toolStripMenuItem4.Text = "CONSTANT";
				toolStripMenuItem4.Checked = variableListNode.Constant;
				toolStripMenuItem4.Click += OnScopeMenuItemClick;
				contextMenuStrip.Items.Add(toolStripMenuItem4);
				ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem();
				toolStripMenuItem5.Tag = new ScopeMenuData(variableListNode, ModelTokenType.Retain);
				toolStripMenuItem5.Text = "RETAIN";
				toolStripMenuItem5.Checked = variableListNode.Retain;
				toolStripMenuItem5.Click += OnScopeMenuItemClick;
				contextMenuStrip.Items.Add(toolStripMenuItem5);
				ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
				toolStripMenuItem6.Tag = new ScopeMenuData(variableListNode, ModelTokenType.Persistent);
				toolStripMenuItem6.Text = "PERSISTENT";
				toolStripMenuItem6.Checked = variableListNode.Persistent;
				toolStripMenuItem6.Click += OnScopeMenuItemClick;
				contextMenuStrip.Items.Add(toolStripMenuItem6);
				Rectangle bounds = viewNode.GetBounds(nColumnIndex, (CellBoundsPortion)1);
				contextMenuStrip.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
				contextMenuStrip.Show((Control)(object)_declarationTable, new Point(bounds.Left, bounds.Bottom + 1));
			}
			finally
			{
				_bInScopeContextMenu = false;
			}
		}

		private void OnScopeMenuItemClick(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			if (toolStripMenuItem == null)
			{
				return;
			}
			ScopeMenuData scopeMenuData = toolStripMenuItem.Tag as ScopeMenuData;
			if (scopeMenuData != null)
			{
				if (scopeMenuData.Scope == ModelTokenType.Constant)
				{
					scopeMenuData.Node.Constant = !scopeMenuData.Node.Constant;
				}
				else if (scopeMenuData.Scope == ModelTokenType.Retain)
				{
					scopeMenuData.Node.Retain = !scopeMenuData.Node.Retain;
				}
				else if (scopeMenuData.Scope == ModelTokenType.Persistent)
				{
					scopeMenuData.Node.Persistent = !scopeMenuData.Node.Persistent;
				}
				else
				{
					scopeMenuData.Node.Scope = scopeMenuData.Scope;
				}
				int columnIndex = _variableListModel.GetColumnIndex(VariableListColumns.Scope);
				if (columnIndex >= 0)
				{
					_declarationTable.AdjustColumnWidth(columnIndex, true);
				}
			}
		}

		private void _declarationTable_Enter(object sender, EventArgs e)
		{
			Refresh();
		}

		private void _declarationTable_Leave(object sender, EventArgs e)
		{
			CancelPendingNewViewNode();
			Refresh();
		}

		private void _declarationTable_FocusedColumnChanged(object sender, EventArgs e)
		{
			if (_variableListModel == null)
			{
				return;
			}
			int columnIndex = _variableListModel.GetColumnIndex(VariableListColumns.Line);
			if (columnIndex >= 0)
			{
				int num = default(int);
				TreeTableViewNode focusedNode = _declarationTable.GetFocusedNode(out num);
				if (focusedNode != null && num == columnIndex)
				{
					focusedNode.Focus(PrimaryColumn);
				}
			}
		}

		private void _declarationTable_ColumnWidthChanged(object sender, EventArgs e)
		{
			if (_variableListModel == null)
			{
				return;
			}
			int columnIndex = _variableListModel.GetColumnIndex(VariableListColumns.Line);
			if (columnIndex >= 0)
			{
				using Graphics g = CreateGraphics();
				int preferredWidth = _variableListModel.GetLineRenderer().GetPreferredWidth(null, columnIndex, g);
				_declarationTable.Columns[columnIndex].Width = preferredWidth;
			}
			int columnIndex2 = _variableListModel.GetColumnIndex(VariableListColumns.NothingToDeclare);
			if (columnIndex2 >= 0)
			{
				using (CreateGraphics())
				{
					int num = ((Control)(object)_declarationTable).Width - 2 * SystemInformation.BorderSize.Width;
					_declarationTable.Columns[columnIndex2].Width = num;
				}
			}
		}

		private void _declarationTable_SizeChanged(object sender, EventArgs e)
		{
			if (_variableListModel == null)
			{
				return;
			}
			int columnIndex = _variableListModel.GetColumnIndex(VariableListColumns.NothingToDeclare);
			if (columnIndex >= 0)
			{
				using (CreateGraphics())
				{
					int num = ((Control)(object)_declarationTable).Width - 2 * SystemInformation.BorderSize.Width;
					_declarationTable.Columns[columnIndex].Width = num;
				}
			}
		}

		private void _declarationTable_SelectionChanged(object sender, EventArgs e)
		{
			_selectedNodeIndices = null;
			UpdateControlStates();
		}

		private void UpdateControlStates()
		{
			_insertButtonItem.Enabled = CanInsert;
			_moveUpButtonItem.Enabled = CanMoveUp;
			_moveDownButtonItem.Enabled = CanMoveDown;
			_deleteButtonItem.Enabled = CanDelete;
			_headerButtonItem.Enabled = !HasUnrecoverableError;
			if (_variableListModel == null)
			{
				return;
			}
			for (int i = 0; i < _declarationTable.Columns.Count; i++)
			{
				if (_variableListModel.GetColumnMeaning(i) == _variableListModel.SortColumn)
				{
					_declarationTable.SetColumnHeaderSortOrderIcon(i, _variableListModel.SortOrder);
				}
				else
				{
					_declarationTable.SetColumnHeaderSortOrderIcon(i, SortOrder.None);
				}
			}
		}

		private void SelectNode(object itemOrIndex)
		{
			TreeTableViewNode val = null;
			if (itemOrIndex is string)
			{
				VariableListNode node = _variableListModel.GetNode((string)itemOrIndex);
				val = _declarationTable.GetViewNode((ITreeTableNode)(object)node);
			}
			else if (itemOrIndex is int)
			{
				int num = (int)itemOrIndex;
				if (num >= _declarationTable.Nodes.Count)
				{
					num = _declarationTable.Nodes.Count - 1;
				}
				if (num >= 0)
				{
					val = _declarationTable.Nodes[num];
				}
			}
			if (val != null)
			{
				_declarationTable.DeselectAll();
				val.Selected=(true);
				val.Focus(PrimaryColumn);
				val.EnsureVisible(PrimaryColumn);
			}
		}

		private void _declarationTable_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (_variableListModel == null)
			{
				return;
			}
			try
			{
				_declarationTable.BeginUpdate();
				VariableListColumns columnMeaning = _variableListModel.GetColumnMeaning(e.Column);
				if (columnMeaning == _variableListModel.SortColumn)
				{
					if (_variableListModel.SortOrder == SortOrder.Ascending)
					{
						_variableListModel.Sort(columnMeaning, SortOrder.Descending);
					}
					else
					{
						_variableListModel.Sort(columnMeaning, SortOrder.Ascending);
					}
				}
				else
				{
					_variableListModel.Sort(columnMeaning, SortOrder.Ascending);
				}
				_bIsEditing = false;
				UpdateControlStates();
			}
			finally
			{
				_declarationTable.EndUpdate();
			}
		}

		private void _deleteButtonItem_Click(object sender, EventArgs e)
		{
			Delete();
		}

		private void _insertButtonItem_Click(object sender, EventArgs e)
		{
			Insert(null);
		}

		private void _moveUpButtonItem_Click(object sender, EventArgs e)
		{
			MoveUp();
		}

		private void _moveDownButtonItem_Click(object sender, EventArgs e)
		{
			MoveDown();
		}

		private bool CheckAllowStructuralModifications(string stErrorText)
		{
			if (HasUnrecoverableError)
			{
				return false;
			}
			if (!_variableListModel.AllowStructuralModifications)
			{
				APEnvironment.Engine.MessageService.Error(stErrorText);
				return false;
			}
			return true;
		}

		private void _declarationTable_AfterEditCancelled(object sender, TreeTableViewEditEventArgs e)
		{
			CancelPendingNewViewNode();
		}

		private void CancelPendingNewViewNode()
		{
			if (_pendingNewViewNode != null)
			{
				int index = _pendingNewViewNode.Index;
				_variableListModel.DoDelete(index);
				SelectNode(index);
				_bPerformFocusCorrection = true;
			}
			_pendingNewViewNode = null;
			_selectedNodeIndices = null;
			UpdateControlStates();
		}

		private void _declarationTable_AfterEditAccepted(object sender, TreeTableViewEditEventArgs e)
		{
			_pendingNewViewNode = null;
			_bIsEditing = false;
			UpdateControlStates();
		}

		internal void Insert(char? ncImmediate)
		{
			if (!CheckAllowStructuralModifications(Resources.CannotInsertItems) || !_variableListModel.AttemptModification())
			{
				return;
			}
			try
			{
				_declarationTable.BeginUpdate();
				int num = ((SelectedNodeIndices.Length != 0) ? SelectedNodeIndices[0] : (_declarationTable.Nodes.Count - 1));
				TreeTableViewNode val = _declarationTable.Nodes[num];
				VariableListNode variableListNode = _declarationTable.GetModelNode(val) as VariableListNode;
				bool bAfter = false;
				if (variableListNode == null && _declarationTable.Nodes.Count > 1)
				{
					val = _declarationTable.Nodes[num - 1];
					variableListNode = _declarationTable.GetModelNode(val) as VariableListNode;
					bAfter = true;
				}
				VariableListNode variableListNode2 = _variableListModel.DoCreateItem(num, variableListNode, bAfter);
				TreeTableViewNode val2 = (_pendingNewViewNode = _declarationTable.GetViewNode((ITreeTableNode)(object)variableListNode2));
				int columnIndex = _variableListModel.GetColumnIndex(VariableListColumns.Name);
				_declarationTable.DeselectAll();
				val2.Selected=(true);
				val2.Focus(columnIndex);
				if (ncImmediate.HasValue)
				{
					val2.BeginEdit(columnIndex, ncImmediate.Value);
				}
				else
				{
					val2.BeginEdit(columnIndex);
				}
				_bIsEditing = true;
			}
			finally
			{
				_declarationTable.EndUpdate();
				UpdateControlStates();
			}
		}

		internal void MoveUp()
		{
			if (!CheckAllowStructuralModifications(Resources.CannotMoveItems))
			{
				return;
			}
			try
			{
				_declarationTable.BeginUpdate();
				_variableListModel.UndoMgr.BeginCompoundAction(string.Empty);
				int[] selectedNodeIndices = SelectedNodeIndices;
				_declarationTable.DeselectAll();
				for (int i = 0; i < selectedNodeIndices.Length; i++)
				{
					_variableListModel.MoveUp(selectedNodeIndices[i]);
					_declarationTable.Nodes[selectedNodeIndices[i] - 1].Selected=(true);
				}
			}
			finally
			{
				_variableListModel.UndoMgr.EndCompoundAction();
				_declarationTable.EndUpdate();
			}
		}

		internal void MoveDown()
		{
			if (!CheckAllowStructuralModifications(Resources.CannotMoveItems))
			{
				return;
			}
			try
			{
				_declarationTable.BeginUpdate();
				_variableListModel.UndoMgr.BeginCompoundAction(string.Empty);
				int[] selectedNodeIndices = SelectedNodeIndices;
				_declarationTable.DeselectAll();
				for (int num = selectedNodeIndices.Length - 1; num >= 0; num--)
				{
					_variableListModel.MoveDown(selectedNodeIndices[num]);
					_declarationTable.Nodes[selectedNodeIndices[num] + 1].Selected=(true);
				}
			}
			finally
			{
				_variableListModel.UndoMgr.EndCompoundAction();
				_declarationTable.EndUpdate();
			}
		}

		internal void EditDeclarationHeader()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected I4, but got Unknown
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Expected O, but got Unknown
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Expected O, but got Unknown
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Expected O, but got Unknown
			string text = null;
			UnresolvedDeclarationContext unresolvedContext = _unresolvedContext;
			switch ((int)unresolvedContext - 2)
			{
			case 0:
			{
				IPOUHeaderDialog iPOUHeaderDialog = ((!Common.SupportsExtendedProgrammingFeatures) ? ((IPOUHeaderDialog)new POUHeaderDialogLight()) : ((IPOUHeaderDialog)new POUHeaderDialogFull()));
				iPOUHeaderDialog.Initialize(_tabularDeclarationModel, ProjectHandle, ObjectGuid, _onlineState.OnlineApplication != Guid.Empty || ReadOnly);
				if (iPOUHeaderDialog.ShowDialog(this) != DialogResult.OK)
				{
					break;
				}
				bool flag2 = false;
				if (iPOUHeaderDialog.ChangedComment != null)
				{
					_tabularDeclarationModel.Header.Comment = iPOUHeaderDialog.ChangedComment;
					flag2 = true;
				}
				if (iPOUHeaderDialog.ChangedKind != null)
				{
					_tabularDeclarationModel.Header.Kind = iPOUHeaderDialog.ChangedKind;
					flag2 = true;
				}
				if (iPOUHeaderDialog.ChangedName != null)
				{
					if (iPOUHeaderDialog.RenameWithRefactoring)
					{
						text = iPOUHeaderDialog.ChangedName;
					}
					else
					{
						_tabularDeclarationModel.Header.Name = iPOUHeaderDialog.ChangedName;
					}
					flag2 = true;
				}
				if (iPOUHeaderDialog.ChangedExtends != null)
				{
					_tabularDeclarationModel.Header.Extends = iPOUHeaderDialog.ChangedExtends;
					flag2 = true;
				}
				if (iPOUHeaderDialog.ChangedImplements != null)
				{
					_tabularDeclarationModel.Header.Implements = iPOUHeaderDialog.ChangedImplements;
					flag2 = true;
				}
				if (iPOUHeaderDialog.ChangedReturnType != null)
				{
					_tabularDeclarationModel.Header.ReturnType = iPOUHeaderDialog.ChangedReturnType;
					flag2 = true;
				}
				if (iPOUHeaderDialog.ChangedAttributes != null)
				{
					_tabularDeclarationModel.Header.Attributes = iPOUHeaderDialog.ChangedAttributes;
					flag2 = true;
				}
				if (!flag2)
				{
					return;
				}
				ITextDocument textDocument2 = GetTextDocument(bToModify: true);
				if (textDocument2 == null)
				{
					return;
				}
				LStringBuilder val2 = new LStringBuilder();
				_tabularDeclarationModel.WriteText(val2);
				textDocument2.Text=(((object)val2).ToString());
				Save(bCommit: true);
				Reload(bRefillVariableListModel: false);
				break;
			}
			case 1:
			{
				InterfaceHeaderDialog interfaceHeaderDialog = new InterfaceHeaderDialog();
				interfaceHeaderDialog.Initialize(_tabularDeclarationModel, ProjectHandle, ObjectGuid, _onlineState.OnlineApplication != Guid.Empty || ReadOnly);
				if (interfaceHeaderDialog.ShowDialog(this) != DialogResult.OK)
				{
					break;
				}
				bool flag3 = false;
				if (interfaceHeaderDialog.ChangedComment != null)
				{
					_tabularDeclarationModel.Header.Comment = interfaceHeaderDialog.ChangedComment;
					flag3 = true;
				}
				if (interfaceHeaderDialog.ChangedName != null)
				{
					if (interfaceHeaderDialog.RenameWithRefactoring)
					{
						text = interfaceHeaderDialog.ChangedName;
					}
					else
					{
						_tabularDeclarationModel.Header.Name = interfaceHeaderDialog.ChangedName;
					}
					flag3 = true;
				}
				if (interfaceHeaderDialog.ChangedExtends != null)
				{
					_tabularDeclarationModel.Header.Extends = interfaceHeaderDialog.ChangedExtends;
					flag3 = true;
				}
				if (interfaceHeaderDialog.ChangedAttributes != null)
				{
					_tabularDeclarationModel.Header.Attributes = interfaceHeaderDialog.ChangedAttributes;
					flag3 = true;
				}
				if (!flag3)
				{
					return;
				}
				ITextDocument textDocument3 = GetTextDocument(bToModify: true);
				if (textDocument3 == null)
				{
					return;
				}
				LStringBuilder val3 = new LStringBuilder();
				_tabularDeclarationModel.WriteText(val3);
				textDocument3.Text=(((object)val3).ToString());
				Save(bCommit: true);
				Reload(bRefillVariableListModel: false);
				break;
			}
			case 2:
			case 3:
			{
				MethodHeaderDialog methodHeaderDialog = new MethodHeaderDialog();
				methodHeaderDialog.Initialize(_tabularDeclarationModel, ProjectHandle, ObjectGuid, _onlineState.OnlineApplication != Guid.Empty || ReadOnly);
				if (methodHeaderDialog.ShowDialog(this) != DialogResult.OK)
				{
					break;
				}
				bool flag = false;
				if (methodHeaderDialog.ChangedComment != null)
				{
					_tabularDeclarationModel.Header.Comment = methodHeaderDialog.ChangedComment;
					flag = true;
				}
				if (methodHeaderDialog.ChangedName != null)
				{
					if (methodHeaderDialog.RenameWithRefactoring)
					{
						text = methodHeaderDialog.ChangedName;
					}
					else
					{
						_tabularDeclarationModel.Header.Name = methodHeaderDialog.ChangedName;
					}
					flag = true;
				}
				if (methodHeaderDialog.ChangedReturnType != null)
				{
					_tabularDeclarationModel.Header.ReturnType = methodHeaderDialog.ChangedReturnType;
					flag = true;
				}
				if (methodHeaderDialog.ChangedAttributes != null)
				{
					_tabularDeclarationModel.Header.Attributes = methodHeaderDialog.ChangedAttributes;
					flag = true;
				}
				if (!flag)
				{
					return;
				}
				ITextDocument textDocument = GetTextDocument(bToModify: true);
				if (textDocument == null)
				{
					return;
				}
				LStringBuilder val = new LStringBuilder();
				_tabularDeclarationModel.WriteText(val);
				textDocument.Text=(((object)val).ToString());
				Save(bCommit: true);
				Reload(bRefillVariableListModel: false);
				break;
			}
			default:
				APEnvironment.Engine.MessageService.Error("Not implemented yet.");
				break;
			}
			if (text == null)
			{
				return;
			}
			IRefactoringRenameSignatureOperation val4 = null;
			try
			{
				val4 = APEnvironment.RefactoringService.Factories.CreateRenameSignatureOperation(ObjectGuid, (string)null, text);
				APEnvironment.RefactoringService.PerformRefactoring((IRefactoringOperation)(object)val4, (IRefactoringUI)null);
			}
			catch (Exception ex)
			{
				IMessageService messageService = APEnvironment.Engine.MessageService;
				IMessageService3 val5 = (IMessageService3)(object)((messageService is IMessageService3) ? messageService : null);
				if (val5 != null)
				{
					val5.Error(ex.Message, "TabularDeclarationEditorRefactoringFailed", new object[3] { ex, ObjectGuid, val4 });
				}
				else
				{
					APEnvironment.Engine.MessageService.Error(ex.Message);
				}
			}
		}

		private void _declarationTable_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (_variableListModel == null)
			{
				return;
			}
			int num = default(int);
			TreeTableViewNode focusedNode = _declarationTable.GetFocusedNode(out num);
			AbstractVariableListNode abstractVariableListNode = _declarationTable.GetModelNode(focusedNode) as AbstractVariableListNode;
			if (abstractVariableListNode == null)
			{
				return;
			}
			switch (e.KeyChar)
			{
			case '_':
				if (abstractVariableListNode is EmptyListNode)
				{
					Insert(e.KeyChar);
				}
				break;
			case ' ':
				if (abstractVariableListNode is EmptyListNode)
				{
					Insert(null);
				}
				else if (abstractVariableListNode is VariableListNode && num == _variableListModel.GetColumnIndex(VariableListColumns.Scope))
				{
					DisplayScopeContextMenu(focusedNode, num);
				}
				break;
			default:
				if (char.IsLetterOrDigit(e.KeyChar))
				{
					Insert(e.KeyChar);
				}
				break;
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData)
			{
			case Keys.Tab:
			{
				int num2 = default(int);
				TreeTableViewNode focusedNode2 = _declarationTable.GetFocusedNode(out num2);
				if (focusedNode2 == null)
				{
					if (_declarationTable.Nodes.Count > 0)
					{
						FocusSelectAndMakeVisible(_declarationTable.Nodes[0], 1);
					}
				}
				else if (num2 < _declarationTable.Columns.Count - 1)
				{
					FocusSelectAndMakeVisible(focusedNode2, num2 + 1);
				}
				else if (focusedNode2.NextNode != null)
				{
					FocusSelectAndMakeVisible(focusedNode2.NextNode, 1);
				}
				else if (_declarationTable.Nodes.Count > 0)
				{
					FocusSelectAndMakeVisible(_declarationTable.Nodes[0], 1);
				}
				return true;
			}
			case Keys.Tab | Keys.Shift:
			{
				int num = default(int);
				TreeTableViewNode focusedNode = _declarationTable.GetFocusedNode(out num);
				if (focusedNode == null)
				{
					if (_declarationTable.Nodes.Count > 0)
					{
						FocusSelectAndMakeVisible(_declarationTable.Nodes[_declarationTable.Nodes.Count - 1], _declarationTable.Columns.Count - 1);
					}
				}
				else if (num > 1)
				{
					FocusSelectAndMakeVisible(focusedNode, num - 1);
				}
				else if (focusedNode.PrevNode != null)
				{
					FocusSelectAndMakeVisible(focusedNode.PrevNode, _declarationTable.Columns.Count - 1);
				}
				else if (_declarationTable.Nodes.Count > 0)
				{
					FocusSelectAndMakeVisible(_declarationTable.Nodes[_declarationTable.Nodes.Count - 1], _declarationTable.Columns.Count - 1);
				}
				return true;
			}
			default:
				return false;
			}
		}

		private void FocusSelectAndMakeVisible(TreeTableViewNode node, int nColumnIndex)
		{
			_declarationTable.DeselectAll();
			node.Selected=(true);
			node.Focus(nColumnIndex);
			node.EnsureVisible(nColumnIndex);
			_bIsEditing = false;
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			int num = default(int);
			if (_bPerformFocusCorrection && (_declarationTable.GetFocusedNode(out num) == null || num < 0))
			{
				if (((TreeTableViewNodeCollection)_declarationTable.SelectedNodes).Count == 0)
				{
					_declarationTable.Nodes[0].Selected=(true);
				}
				((TreeTableViewNodeCollection)_declarationTable.SelectedNodes)[0].Focus(PrimaryColumn);
				_bPerformFocusCorrection = false;
			}
		}

		private void OnBookmarkCleared(object sender, BookmarkEventArgs e)
		{
			try
			{
				_declarationTable.BeginUpdate();
				_variableListModel.AdjustLineNumbers();
			}
			finally
			{
				_declarationTable.EndUpdate();
			}
		}

		private void OnBookmarkSet(object sender, BookmarkEventArgs e)
		{
			try
			{
				_declarationTable.BeginUpdate();
				_variableListModel.AdjustLineNumbers();
			}
			finally
			{
				_declarationTable.EndUpdate();
			}
		}

		private void _declarationTable_NodeDrag(object sender, TreeTableViewDragEventArgs e)
		{
			VariableListNode variableListNode = null;
			if (((TreeTableViewNodeCollection)_declarationTable.SelectedNodes).Count == 1)
			{
				variableListNode = _declarationTable.GetModelNode(((TreeTableViewNodeCollection)_declarationTable.SelectedNodes)[0]) as VariableListNode;
			}
			if (variableListNode != null)
			{
				TreeTableViewNode viewNode = _declarationTable.GetViewNode((ITreeTableNode)(object)variableListNode);
				long position = 0L;
				int length = 0;
				int column = 1;
				GetSelectionDataForNode(viewNode, column, out position, out length);
				string data = string.Format("{0};{1};{2};{3};{4};{5}", _editor.ProjectHandle, _editor.ObjectGuid, position, length, _onlineState.OnlineApplication, _onlineState.InstancePath ?? "(null)");
				DataObject dataObject = new DataObject("AP_EditorSelectionData", data);
				IPreCompileContext val = default(IPreCompileContext);
				ISignature val2 = APEnvironment.LanguageModelMgr.FindSignature(_editor.ObjectGuid, out val);
				string text = "";
				if (val2.HasAttribute("qualified_only"))
				{
					text = val2.Name + "." + variableListNode.Name;
					string data2 = $"{-1};{variableListNode.Name}";
					dataObject.SetData("AP_ST_DragDropData", data2);
				}
				else
				{
					text = variableListNode.Name;
				}
				dataObject.SetData(typeof(string), text);
				_declarationTable.DoDragDrop((object)dataObject, DragDropEffects.Copy | DragDropEffects.Move);
			}
		}

		private bool GetSelectionDataForNode(TreeTableViewNode viewNode, int column, out long position, out int length)
		{
			position = -1L;
			length = 0;
			if (viewNode == null)
			{
				return false;
			}
			int num = _declarationTable.Nodes.IndexOf(viewNode);
			if (num < 0)
			{
				return false;
			}
			if (!_variableListModel.GetPosition(num, column, out var nOffset, out length))
			{
				return false;
			}
			ITextDocument textDocument = GetTextDocument(bToModify: false);
			Debug.Assert(textDocument != null);
			if (nOffset < 0 || nOffset > textDocument.Length)
			{
				return false;
			}
			int num2 = textDocument.FindLineByOffset(nOffset);
			if (num2 < 0)
			{
				return false;
			}
			short num3 = (short)(nOffset - textDocument.GetLineStartOffset(num2));
			if (num3 < 0)
			{
				return false;
			}
			num3 = (short)(num3 + (short)length);
			length = -length;
			long lineId = textDocument.GetLineId(num2);
			position = PositionHelper.CombinePosition(lineId, num3);
			return true;
		}

		public IEnumerable<SourceRange> GetSelection()
		{
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)_declarationTable.SelectedNodes)
			{
				TreeTableViewNode viewNode = item;
				for (int i = 0; i < _declarationTable.Columns.Count; i++)
				{
					if (GetSelectionDataForNode(viewNode, i, out var position, out var length))
					{
						yield return new SourceRange(position, length);
					}
				}
			}
		}

		public void Select(IEnumerable<SourceRange> selection)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			using IEnumerator<SourceRange> enumerator = selection.GetEnumerator();
			if (enumerator.MoveNext())
			{
				SourceRange current = enumerator.Current;
				Select(current.Position, current.Length);
			}
		}

		public void SetLocalizedObject(IMetaObject obj, bool bLocalizationActive)
		{
			_localizedObject = obj;
			_bLocalizationActive = bLocalizationActive;
		}

		public bool IsComment(long nPositionCombined, string stText)
		{
			return false;
		}

		public string GetPreselectedType(long nPosition)
		{
			return string.Empty;
		}

		public string GetInstancePath(long nPosition)
		{
			return string.Empty;
		}

		protected override void Dispose(bool disposing)
		{
			DisposeCore();
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Expected O, but got Unknown
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Expected O, but got Unknown
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Expected O, but got Unknown
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.TabularDeclarationEditor));
			_correctPanel = new System.Windows.Forms.Panel();
			_declarationTable = new TreeTableView();
			_incorrectPanel = new System.Windows.Forms.Panel();
			_reasonLabel = new System.Windows.Forms.Label();
			_pictureBox = new System.Windows.Forms.PictureBox();
			_toolBar = new System.Windows.Forms.ToolStrip();
			_insertButtonItem = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			_moveUpButtonItem = new System.Windows.Forms.ToolStripButton();
			_moveDownButtonItem = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			_deleteButtonItem = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer(components);
			System.Windows.Forms.Label label = new System.Windows.Forms.Label();
			_correctPanel.SuspendLayout();
			_incorrectPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
			_toolBar.SuspendLayout();
			SuspendLayout();
			timer.Enabled = true;
			timer.Interval = 250;
			timer.Tick += new System.EventHandler(_timer_Tick);
			resources.ApplyResources(label, "label1");
			label.ForeColor = System.Drawing.Color.Red;
			label.Name = "label1";
			_correctPanel.Controls.Add((System.Windows.Forms.Control)(object)_declarationTable);
			resources.ApplyResources(_correctPanel, "_correctPanel");
			_correctPanel.Name = "_correctPanel";
			((System.Windows.Forms.Control)(object)_declarationTable).BackColor = System.Drawing.SystemColors.Window;
			_declarationTable.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			resources.ApplyResources(_declarationTable, "_declarationTable");
			_declarationTable.DoNotShrinkColumnsAutomatically=(false);
			_declarationTable.ForceFocusOnClick=(false);
			_declarationTable.GridLines=(false);
			_declarationTable.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.Clickable);
			_declarationTable.HideSelection=(false);
			_declarationTable.ImmediateEdit=(true);
			_declarationTable.Indent=(20);
			_declarationTable.KeepColumnWidthsAdjusted=(false);
			_declarationTable.Model=((ITreeTableModel)null);
			_declarationTable.MultiSelect=(true);
			((System.Windows.Forms.Control)(object)_declarationTable).Name = "_declarationTable";
			_declarationTable.NoSearchStrings=(false);
			_declarationTable.OpenEditOnDblClk=(true);
			_declarationTable.ReadOnly=(false);
			_declarationTable.Scrollable=(true);
			_declarationTable.ShowLines=(false);
			_declarationTable.ShowPlusMinus=(false);
			_declarationTable.ShowRootLines=(false);
			_declarationTable.ToggleOnDblClk=(false);
			_declarationTable.AfterEditAccepted+=(new TreeTableViewEditEventHandler(_declarationTable_AfterEditAccepted));
			_declarationTable.AfterEditCancelled+=(new TreeTableViewEditEventHandler(_declarationTable_AfterEditCancelled));
			_declarationTable.ColumnClick+=(new System.Windows.Forms.ColumnClickEventHandler(_declarationTable_ColumnClick));
			_declarationTable.SelectionChanged+=(new System.EventHandler(_declarationTable_SelectionChanged));
			_declarationTable.FocusedColumnChanged+=(new System.EventHandler(_declarationTable_FocusedColumnChanged));
			_declarationTable.NodeDrag+=(new TreeTableViewDragEventHandler(_declarationTable_NodeDrag));
			_declarationTable.ColumnWidthChanged+=(new System.EventHandler(_declarationTable_ColumnWidthChanged));
			((System.Windows.Forms.Control)(object)_declarationTable).SizeChanged += new System.EventHandler(_declarationTable_SizeChanged);
			((System.Windows.Forms.Control)(object)_declarationTable).Enter += new System.EventHandler(_declarationTable_Enter);
			((System.Windows.Forms.Control)(object)_declarationTable).KeyPress += new System.Windows.Forms.KeyPressEventHandler(_declarationTable_KeyPress);
			((System.Windows.Forms.Control)(object)_declarationTable).Leave += new System.EventHandler(_declarationTable_Leave);
			((System.Windows.Forms.Control)(object)_declarationTable).MouseDown += new System.Windows.Forms.MouseEventHandler(_declarationTable_MouseDown);
			((System.Windows.Forms.Control)(object)_declarationTable).MouseUp += new System.Windows.Forms.MouseEventHandler(_declarationTable_MouseUp);
			_incorrectPanel.Controls.Add(_reasonLabel);
			_incorrectPanel.Controls.Add(label);
			_incorrectPanel.Controls.Add(_pictureBox);
			resources.ApplyResources(_incorrectPanel, "_incorrectPanel");
			_incorrectPanel.Name = "_incorrectPanel";
			resources.ApplyResources(_reasonLabel, "_reasonLabel");
			_reasonLabel.Name = "_reasonLabel";
			resources.ApplyResources(_pictureBox, "_pictureBox");
			_pictureBox.Name = "_pictureBox";
			_pictureBox.TabStop = false;
			resources.ApplyResources(_toolBar, "_toolBar");
			_toolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			_toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { _insertButtonItem, toolStripSeparator1, _moveUpButtonItem, _moveDownButtonItem, toolStripSeparator2, _deleteButtonItem, toolStripSeparator5 });
			_toolBar.Name = "_toolBar";
			_insertButtonItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(_insertButtonItem, "_insertButtonItem");
			_insertButtonItem.Name = "_insertButtonItem";
			_insertButtonItem.Click += new System.EventHandler(_insertButtonItem_Click);
			toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
			_moveUpButtonItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(_moveUpButtonItem, "_moveUpButtonItem");
			_moveUpButtonItem.Name = "_moveUpButtonItem";
			_moveUpButtonItem.Click += new System.EventHandler(_moveUpButtonItem_Click);
			_moveDownButtonItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(_moveDownButtonItem, "_moveDownButtonItem");
			_moveDownButtonItem.Name = "_moveDownButtonItem";
			_moveDownButtonItem.Click += new System.EventHandler(_moveDownButtonItem_Click);
			toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
			_deleteButtonItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(_deleteButtonItem, "_deleteButtonItem");
			_deleteButtonItem.Name = "_deleteButtonItem";
			_deleteButtonItem.Click += new System.EventHandler(_deleteButtonItem_Click);
			toolStripSeparator5.Name = "toolStripSeparator5";
			resources.ApplyResources(toolStripSeparator5, "toolStripSeparator5");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.Controls.Add(_incorrectPanel);
			base.Controls.Add(_correctPanel);
			base.Controls.Add(_toolBar);
			base.Name = "TabularDeclarationEditor";
			_correctPanel.ResumeLayout(false);
			_incorrectPanel.ResumeLayout(false);
			_incorrectPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
			_toolBar.ResumeLayout(false);
			_toolBar.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
