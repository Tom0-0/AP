#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Bookmarks;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using _3S.CoDeSys.TextDocument;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class VariableListModel : AbstractTreeTableModel, IDisposable
	{
		private TabularDeclarationEditor _editor;

		private UnresolvedDeclarationContext _unresolvedContext;

		private ResolvedDeclarationContext _resolvedContext;

		private VariableListColumns _columns;

		private bool _bDuringReload;

		private bool _bDuringAdjustLineNumbers;

		private LineRenderer _lineRenderer = new LineRenderer();

		private VariableListColumns _sortColumn = VariableListColumns.Line;

		private SortOrder _sortOrder = SortOrder.Ascending;

		private IUndoManager _undoMgr;

		private static readonly Image IMAGE_VAR = Bitmap.FromHicon(Resources.VarSmall.Handle);

		private static readonly Image IMAGE_VARGLOBAL = Bitmap.FromHicon(Resources.VarGlobalSmall.Handle);

		private static readonly Image IMAGE_VARINOUT = Bitmap.FromHicon(Resources.VarInOutSmall.Handle);

		private static readonly Image IMAGE_VARINPUT = Bitmap.FromHicon(Resources.VarInputSmall.Handle);

		private static readonly Image IMAGE_VAROUTPUT = Bitmap.FromHicon(Resources.VarOutputSmall.Handle);

		private static readonly Image IMAGE_VARSTAT = Bitmap.FromHicon(Resources.VarStatSmall.Handle);

		private static readonly Image IMAGE_VARTEMP = Bitmap.FromHicon(Resources.VarTempSmall.Handle);

		private static readonly Image IMAGE_EMPTY = Bitmap.FromHicon(Resources.Empty.Handle);

		internal bool ReadOnly { get; set; }

		internal VariableListColumns SortColumn => _sortColumn;

		internal SortOrder SortOrder => _sortOrder;

		internal ResolvedDeclarationContext ResolvedContext
		{
			get
			{
				return _resolvedContext;
			}
			set
			{
				_resolvedContext = value;
			}
		}

		internal bool AllowStructuralModifications
		{
			get
			{
				if (_sortColumn == VariableListColumns.Line)
				{
					return _sortOrder == SortOrder.Ascending;
				}
				return false;
			}
		}

		internal IUndoManager UndoMgr => _undoMgr;

		internal TabularDeclarationEditor Editor => _editor;

		internal VariableListModel(TabularDeclarationEditor editor, UnresolvedDeclarationContext unresolvedContext)
			: base()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected I4, but got Unknown
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Expected O, but got Unknown
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			_editor = editor;
			_unresolvedContext = unresolvedContext;
			switch ((int)unresolvedContext)
			{
			case 1:
			case 2:
			case 4:
			case 5:
				_columns = VariableListColumns.Line | VariableListColumns.Scope | VariableListColumns.Name | VariableListColumns.Address | VariableListColumns.DataType | VariableListColumns.Initialization | VariableListColumns.Comment;
				if (Common.SupportsExtendedProgrammingFeatures)
				{
					_columns |= VariableListColumns.Attributes;
				}
				break;
			case 3:
				_columns = VariableListColumns.NothingToDeclare;
				break;
			case 0:
			case 6:
			case 7:
			case 8:
				throw new NotImplementedException();
			}
			foreach (VariableListColumns value in Enum.GetValues(typeof(VariableListColumns)))
			{
				if ((value & _columns) != 0)
				{
					CreateColumn(value);
				}
			}
			_undoMgr = APEnvironment.CreateUndoMgr();
			_undoMgr.ActionUndone+=(new UndoEventHandler(OnUndoMgrActionUndoneRedone));
			_undoMgr.ActionRedone+=(new UndoEventHandler(OnUndoMgrActionUndoneRedone));
		}

		private void OnUndoMgrActionUndoneRedone(object sender, UndoEventArgs e)
		{
			ApplyChanges();
		}

		internal void Refill(TabularDeclarationList declarationList, ResolvedDeclarationContext resolvedContext)
		{
			try
			{
				_bDuringReload = true;
				_resolvedContext = resolvedContext;
				(this).UnderlyingModel.ClearRootNodes();
				if (declarationList != null)
				{
					int num = 1;
					foreach (TabularDeclarationBlock block in declarationList.Blocks)
					{
						foreach (TabularDeclarationItem item in block.Items)
						{
							(this).UnderlyingModel.AddRootNode((ITreeTableNode)(object)new VariableListNode(this, num++, item.Name, -1));
						}
					}
				}
				(this).UnderlyingModel.AddRootNode((ITreeTableNode)(object)new EmptyListNode(this));
				DoSort();
				AdjustLineNumbers();
			}
			finally
			{
				_bDuringReload = false;
				_undoMgr.Clear();
			}
		}

		internal void Sort(VariableListColumns column, SortOrder order)
		{
			_sortColumn = column;
			_sortOrder = order;
			if (!_editor.IsEditing)
			{
				DoSort();
			}
		}

		private void DoSort()
		{
			(this).UnderlyingModel.Sort((ITreeTableNode)null, true, (IComparer)new VariableListComparer(SortColumn, SortOrder));
		}

		internal int GetColumnIndex(VariableListColumns column)
		{
			int num = -1;
			foreach (VariableListColumns value in Enum.GetValues(typeof(VariableListColumns)))
			{
				if ((_columns & value) != 0)
				{
					num++;
				}
				if (value == column)
				{
					return num;
				}
			}
			return -1;
		}

		internal VariableListColumns GetColumnMeaning(int nColIdx)
		{
			int num = -1;
			foreach (VariableListColumns value in Enum.GetValues(typeof(VariableListColumns)))
			{
				if ((_columns & value) != 0)
				{
					num++;
				}
				if (num == nColIdx)
				{
					return value;
				}
			}
			return VariableListColumns.None;
		}

		internal ModelTokenType GetAllowedScopes(VariableListNode node)
		{
			ModelTokenType modelTokenType = _resolvedContext switch
			{
				ResolvedDeclarationContext.Function => ModelTokenType.Var | ModelTokenType.VarInOut | ModelTokenType.VarInput | ModelTokenType.VarOutput | ModelTokenType.VarStat, 
				ResolvedDeclarationContext.FunctionBlock => ModelTokenType.Var | ModelTokenType.VarInOut | ModelTokenType.VarInput | ModelTokenType.VarOutput | ModelTokenType.VarStat | ModelTokenType.VarTemp, 
				ResolvedDeclarationContext.GlobalVariableList => ModelTokenType.VarGlobal, 
				ResolvedDeclarationContext.Interface => (ModelTokenType)0L, 
				ResolvedDeclarationContext.InterfaceMethod => ModelTokenType.VarInOut | ModelTokenType.VarInput | ModelTokenType.VarOutput, 
				ResolvedDeclarationContext.POUMethod => ModelTokenType.Var | ModelTokenType.VarInOut | ModelTokenType.VarInput | ModelTokenType.VarOutput | ModelTokenType.VarStat, 
				ResolvedDeclarationContext.Program => ModelTokenType.Var | ModelTokenType.VarInOut | ModelTokenType.VarInput | ModelTokenType.VarOutput | ModelTokenType.VarTemp, 
				_ => throw new NotImplementedException(), 
			};
			if (node != null)
			{
				if (node.Item.DataType.Trim().ToUpperInvariant() == "BIT")
				{
					modelTokenType &= ~(ModelTokenType.VarInOut | ModelTokenType.VarStat | ModelTokenType.VarTemp);
				}
				if (node.Item.DataType.Trim().ToUpperInvariant().Contains("REFERENCE TO"))
				{
					modelTokenType &= ~(ModelTokenType.VarInOut | ModelTokenType.VarOutput);
				}
			}
			return modelTokenType;
		}

		internal ModelTokenType GetDefaultScope()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected I4, but got Unknown
			UnresolvedDeclarationContext unresolvedContext = _unresolvedContext;
			switch ((int)unresolvedContext)
			{
			case 2:
			case 4:
				return ModelTokenType.Var;
			case 1:
				return ModelTokenType.VarGlobal;
			case 3:
				return (ModelTokenType)0L;
			case 5:
				return ModelTokenType.VarInput;
			default:
				throw new NotImplementedException();
			}
		}

		internal Image GetScopeImage(ModelTokenType scope)
		{
			switch (scope)
			{
			case ModelTokenType.Var:
			case ModelTokenType.VarAccess:
			case ModelTokenType.VarConfig:
			case ModelTokenType.VarExternal:
				return IMAGE_VAR;
			case ModelTokenType.VarGlobal:
				return IMAGE_VARGLOBAL;
			case ModelTokenType.VarInOut:
				return IMAGE_VARINOUT;
			case ModelTokenType.VarInput:
				return IMAGE_VARINPUT;
			case ModelTokenType.VarOutput:
				return IMAGE_VAROUTPUT;
			case ModelTokenType.VarStat:
				return IMAGE_VARSTAT;
			case ModelTokenType.VarTemp:
				return IMAGE_VARTEMP;
			default:
				return IMAGE_EMPTY;
			}
		}

		public override void RaiseChanged(TreeTableModelEventArgs e)
		{
			if (ApplyChanges())
			{
				base.RaiseChanged(e);
			}
		}

		public override void RaiseInserted(TreeTableModelEventArgs e)
		{
			if (ApplyChanges())
			{
				base.RaiseInserted(e);
			}
		}

		public override void RaiseRemoved(TreeTableModelEventArgs e)
		{
			if (ApplyChanges())
			{
				base.RaiseRemoved(e);
			}
		}

		public override void RaiseStructureChanged(TreeTableModelEventArgs e)
		{
			if (ApplyChanges())
			{
				base.RaiseStructureChanged(e);
			}
		}

		internal LineRenderer GetLineRenderer()
		{
			return _lineRenderer;
		}

		internal void Delete(int[] nodeIndices)
		{
			if (nodeIndices == null)
			{
				throw new ArgumentNullException("nodes");
			}
			if (!AttemptModification())
			{
				return;
			}
			try
			{
				_undoMgr.BeginCompoundAction(string.Empty);
				int num = int.MaxValue;
				for (int num2 = nodeIndices.Length - 1; num2 >= 0; num2--)
				{
					Debug.Assert(nodeIndices[num2] < num);
					num = nodeIndices[num2];
					VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(nodeIndices[num2]) as VariableListNode;
					Debug.Assert(variableListNode != null);
					RecordAction((IUndoableAction)(object)new DeleteAction(this, variableListNode.Name, nodeIndices[num2], new SerializableTabularDeclarationItem(variableListNode.Item)), bPerform: true, adjustLineNumbers: false);
				}
			}
			finally
			{
				AdjustLineNumbers();
				_undoMgr.EndCompoundAction();
				ApplyChanges();
			}
		}

		internal void DoDelete(int nIndex)
		{
			VariableListNode obj = (this).UnderlyingModel.Sentinel.GetChild(nIndex) as VariableListNode;
			Debug.Assert(obj != null);
			obj.Item.Delete();
			(this).UnderlyingModel.RemoveRootNode(nIndex);
		}

		internal void DoInsert(int nIndex, SerializableTabularDeclarationItem item, string stName)
		{
			VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(nIndex) as VariableListNode;
			if (variableListNode == null)
			{
				_editor.TabularDeclarationModel.List.AppendItem(item);
			}
			else
			{
				variableListNode.Item.Insert(item);
			}
			VariableListNode variableListNode2 = new VariableListNode(this, nIndex + 1, stName, -1);
			(this).UnderlyingModel.InsertRootNode(nIndex, (ITreeTableNode)(object)variableListNode2);
		}

		internal VariableListNode[] Paste(int nIndex, ClipboardData data)
		{
			if (!AttemptModification())
			{
				return new VariableListNode[0];
			}
			try
			{
				LList<VariableListNode> val = new LList<VariableListNode>();
				_undoMgr.BeginCompoundAction(string.Empty);
				string[] array = new string[1];
				for (int i = 0; i < data.Items.Length; i++)
				{
					array[0] = data.Names[i];
					int num = 0;
					while (_editor.TabularDeclarationModel.List.CheckUniqueVariableNames(array) != null)
					{
						if (num++ == 0)
						{
							array[0] = $"Copy_of_{data.Names[i]}";
						}
						else
						{
							array[0] = $"Copy_{num}_of_{data.Names[i]}";
						}
						if (array[0].IndexOf("__") >= 0)
						{
							array[0] = array[0].Replace("__", "_");
						}
					}
					SerializableTabularDeclarationItem serializableTabularDeclarationItem = data.Items[i];
					SerializableModelToken[] tokens = serializableTabularDeclarationItem.Tokens;
					foreach (SerializableModelToken serializableModelToken in tokens)
					{
						if (serializableModelToken.Type == ModelTokenType.Text && serializableModelToken.Text == data.Names[i])
						{
							serializableModelToken.Text = array[0];
							break;
						}
					}
					ModelTokenType allowedScopes = GetAllowedScopes(null);
					if ((serializableTabularDeclarationItem.Scope & allowedScopes) == (ModelTokenType)0L)
					{
						if (GetDefaultScope() == (ModelTokenType)0L)
						{
							continue;
						}
						serializableTabularDeclarationItem.Scope = GetDefaultScope();
					}
					RecordAction((IUndoableAction)(object)new InsertAction(this, array[0], nIndex++, serializableTabularDeclarationItem), bPerform: true, adjustLineNumbers: false);
					VariableListNode node = GetNode(array[0]);
					val.Add(node);
				}
				return val.ToArray();
			}
			finally
			{
				AdjustLineNumbers();
				_undoMgr.EndCompoundAction();
				ApplyChanges();
			}
		}

		internal void AdjustLineNumbers()
		{
			if (_bDuringAdjustLineNumbers)
			{
				return;
			}
			try
			{
				_bDuringAdjustLineNumbers = true;
				LHashSet<int> val = new LHashSet<int>();
				IBookmark[] bookmarks = APEnvironment.BookmarkMgr.GetBookmarks(_editor.ProjectHandle, _editor.ObjectGuid);
				if (bookmarks != null)
				{
					IBookmark[] array = bookmarks;
					foreach (IBookmark val2 in array)
					{
						int num = FindLineByPosition(val2.Position);
						if (num >= 0)
						{
							val.Add(num);
						}
					}
				}
				LDictionary<string, int> val3 = new LDictionary<string, int>();
				int num2 = 1;
				if (_editor.TabularDeclarationModel.List != null)
				{
					foreach (TabularDeclarationBlock block in _editor.TabularDeclarationModel.List.Blocks)
					{
						foreach (TabularDeclarationItem item in block.Items)
						{
							val3[item.Name] = num2++;
						}
					}
				}
				for (int j = 0; j < (this).UnderlyingModel.Sentinel.ChildCount; j++)
				{
					VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(j) as VariableListNode;
					if (variableListNode != null && _editor.TabularDeclarationModel.List != null && val3.TryGetValue(variableListNode.Name, out num2))
					{
						bool bBookmark = val.Contains(num2 - 1);
						variableListNode.SetLineNumber(num2, bBookmark);
					}
				}
			}
			finally
			{
				_bDuringAdjustLineNumbers = false;
			}
		}

		private bool ApplyChanges()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			if (_bDuringReload)
			{
				return true;
			}
			if (_bDuringAdjustLineNumbers)
			{
				return true;
			}
			if (_undoMgr.InCompoundAction || _undoMgr.InUndo || _undoMgr.InRedo)
			{
				return true;
			}
			if (_editor.HasUnrecoverableError)
			{
				return false;
			}
			ITextDocument textDocument = _editor.GetTextDocument(bToModify: false);
			Debug.Assert(textDocument != null);
			LStringBuilder val = new LStringBuilder();
			_editor.TabularDeclarationModel.WriteText(val);
			if (textDocument.Text == ((object)val).ToString())
			{
				return true;
			}
			textDocument = _editor.GetTextDocument(bToModify: true);
			if (textDocument != null)
			{
				textDocument.Text=(((object)val).ToString());
			}
			return _editor.Reload(textDocument == null);
		}

		private void CreateColumn(VariableListColumns column)
		{
			switch (column)
			{
			case VariableListColumns.Line:
				(this).UnderlyingModel.AddColumn(string.Empty, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)_lineRenderer, (ITreeTableViewEditor)new TextBoxTreeTableViewEditor(), false);
				break;
			case VariableListColumns.Scope:
				(this).UnderlyingModel.AddColumn(Resources.ScopeHeader, HorizontalAlignment.Left, (ITreeTableViewRenderer)new IconLabelTreeTableViewRenderer(false), (ITreeTableViewEditor)new TextBoxTreeTableViewEditor(), false);
				break;
			case VariableListColumns.Name:
				(this).UnderlyingModel.AddColumn(Resources.NameHeader, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new BoldRenderer(), (ITreeTableViewEditor)new TextBoxTreeTableViewEditor(), true);
				break;
			case VariableListColumns.Address:
				(this).UnderlyingModel.AddColumn(Resources.AddressHeader, HorizontalAlignment.Left, (ITreeTableViewRenderer)new LabelTreeTableViewRenderer(false), (ITreeTableViewEditor)new TextBoxTreeTableViewEditor(), true);
				break;
			case VariableListColumns.DataType:
				(this).UnderlyingModel.AddColumn(Resources.DataTypeHeader, HorizontalAlignment.Left, (ITreeTableViewRenderer)new LabelTreeTableViewRenderer(false), (ITreeTableViewEditor)(object)new DataTypeEditor((IEditor)(object)_editor), true);
				break;
			case VariableListColumns.Initialization:
				(this).UnderlyingModel.AddColumn(Resources.InitializationHeader, HorizontalAlignment.Left, (ITreeTableViewRenderer)new LabelTreeTableViewRenderer(false), (ITreeTableViewEditor)(object)new InitializationEditor((IEditor)(object)_editor), true);
				break;
			case VariableListColumns.Comment:
				//if (_unresolvedContext == UnresolvedContext.DUTEnum)
				//{
				//	((AbstractTreeTableModel)this).UnderlyingModel.AddColumn(Resources.CommentHeader, HorizontalAlignment.Left, MultilineTextTreeTableViewRenderer.get_MultilineString(), (ITreeTableViewEditor)new TextBoxTreeTableViewEditor(), true);
				//}
				//else
				(this).UnderlyingModel.AddColumn(Resources.CommentHeader, HorizontalAlignment.Left, MultilineTextTreeTableViewRenderer.MultilineString, (ITreeTableViewEditor)(object)new CommentEditor(_editor), true);
				break;
			case VariableListColumns.Attributes:
				(this).UnderlyingModel.AddColumn(Resources.AttributesHeader, HorizontalAlignment.Left, (ITreeTableViewRenderer)new LabelTreeTableViewRenderer(false), (ITreeTableViewEditor)(object)new AttributeEditor(_editor), true);
				break;
			case VariableListColumns.NothingToDeclare:
				(this).UnderlyingModel.AddColumn(string.Empty, HorizontalAlignment.Center, (ITreeTableViewRenderer)(object)new ItalicRenderer(), (ITreeTableViewEditor)new TextBoxTreeTableViewEditor(), false);
				break;
			}
		}

		public void Dispose()
		{
			if (_lineRenderer != null)
			{
				_lineRenderer.Dispose();
			}
		}

		internal VariableListNode GetNode(string stItem)
		{
			for (int i = 0; i < (this).UnderlyingModel.Sentinel.ChildCount; i++)
			{
				VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(i) as VariableListNode;
				if (variableListNode != null && variableListNode.Name == stItem)
				{
					return variableListNode;
				}
			}
			return null;
		}

		internal TabularDeclarationItem GetItem(string stItem)
		{
			if (_editor.TabularDeclarationModel.List != null)
			{
				return _editor.TabularDeclarationModel.List.GetItem(stItem);
			}
			return null;
		}

		internal VariableListNode DoCreateItem(int nIndex, VariableListNode masterNode, bool bAfter)
		{
			Debug.Assert(AllowStructuralModifications);
			TabularDeclarationItem tabularDeclarationItem = ((masterNode == null) ? _editor.TabularDeclarationModel.List.AppendDefaultItem(GetDefaultScope()) : masterNode.Item.Duplicate(bAfter, _editor.TabularDeclarationModel.List));
			VariableListNode variableListNode = new VariableListNode(this, -1, tabularDeclarationItem.Name, nIndex);
			(this).UnderlyingModel.InsertRootNode(nIndex, (ITreeTableNode)(object)variableListNode);
			return variableListNode;
		}

		internal void MoveUp(int nIndex)
		{
			if (AttemptModification())
			{
				RecordAction((IUndoableAction)(object)new MoveUpAction(this, nIndex), bPerform: true, adjustLineNumbers: true);
			}
		}

		internal void MoveDown(int nIndex)
		{
			if (AttemptModification())
			{
				RecordAction((IUndoableAction)(object)new MoveDownAction(this, nIndex), bPerform: true, adjustLineNumbers: true);
			}
		}

		internal void DoMoveUp(int nIndex)
		{
			Debug.Assert(AllowStructuralModifications);
			VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(nIndex) as VariableListNode;
			Debug.Assert(variableListNode != null);
			SerializableTabularDeclarationItem item = new SerializableTabularDeclarationItem(variableListNode.Item);
			variableListNode.Item.Delete();
			(this).UnderlyingModel.RemoveRootNode(nIndex);
			VariableListNode obj = (this).UnderlyingModel.Sentinel.GetChild(nIndex - 1) as VariableListNode;
			Debug.Assert(obj != null);
			obj.Item.Insert(item);
			(this).UnderlyingModel.InsertRootNode(nIndex - 1, (ITreeTableNode)(object)variableListNode);
		}

		internal void DoMoveDown(int nIndex)
		{
			Debug.Assert(AllowStructuralModifications);
			VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(nIndex) as VariableListNode;
			Debug.Assert(variableListNode != null);
			SerializableTabularDeclarationItem item = new SerializableTabularDeclarationItem(variableListNode.Item);
			variableListNode.Item.Delete();
			(this).UnderlyingModel.RemoveRootNode(nIndex);
			VariableListNode variableListNode2 = (this).UnderlyingModel.Sentinel.GetChild(nIndex + 1) as VariableListNode;
			if (variableListNode2 == null)
			{
				_editor.TabularDeclarationModel.List.AppendItem(item);
			}
			else
			{
				variableListNode2.Item.Insert(item);
			}
			(this).UnderlyingModel.InsertRootNode(nIndex + 1, (ITreeTableNode)(object)variableListNode);
		}

		internal bool AttemptModification()
		{
			if (!ReadOnly)
			{
				return _editor.GetTextDocument(bToModify: true) != null;
			}
			return false;
		}

		internal void RecordAction(IUndoableAction action, bool bPerform, bool adjustLineNumbers)
		{
			Debug.Assert(action != null);
			if (bPerform)
			{
				action.Redo();
			}
			_undoMgr.AddAction(action);
			if (adjustLineNumbers)
			{
				AdjustLineNumbers();
			}
		}

		internal bool FindByOffset(int nOffset, out int nLine, out int nColumn)
		{
			for (int i = 0; i < (this).UnderlyingModel.Sentinel.ChildCount; i++)
			{
				VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(i) as VariableListNode;
				if (variableListNode == null)
				{
					continue;
				}
				for (int j = 0; j < (this).UnderlyingModel.ColumnCount; j++)
				{
					variableListNode.GetOffset(j, out var nOffset2, out var nLength);
					if (nOffset2 <= nOffset && nOffset < nOffset2 + nLength)
					{
						nLine = i;
						nColumn = j;
						return true;
					}
				}
			}
			nLine = -1;
			nColumn = -1;
			return false;
		}

		internal int FindLineByPosition(long nPosition)
		{
			ITextDocument textDocument = _editor.GetTextDocument(bToModify: false);
			Debug.Assert(textDocument != null);
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			try
			{
				for (int i = 0; i < ((AbstractTreeTableModel)this).Sentinel.ChildCount; i++)
				{
					VariableListNode variableListNode = ((AbstractTreeTableModel)this).Sentinel.GetChild(i) as VariableListNode;
					if (variableListNode == null || variableListNode.Item == null)
					{
						continue;
					}
					ModelTokenRange modelTokenRange = variableListNode.Item.CalculateLineBoundaryRange();
					if (modelTokenRange == null)
					{
						continue;
					}
					LinkedListNode<ModelToken> linkedListNode = modelTokenRange.First;
					while (linkedListNode != null && linkedListNode != modelTokenRange.Next)
					{
						int offset = linkedListNode.Value.Offset;
						if (linkedListNode.Value.Text != null)
						{
							_ = linkedListNode.Value.Text.Length;
						}
						int num3 = textDocument.FindLineByOffset(offset);
						if (num3 >= 0)
						{
							long lineId = textDocument.GetLineId(num3);
							if (num == lineId)
							{
								return i;
							}
						}
						linkedListNode = linkedListNode.Next;
					}
				}
			}
			catch
			{
			}
			return -1;
		}

		internal bool GetPosition(int nLine, int nColumn, out int nOffset, out int nLength)
		{
			nOffset = -1;
			nLength = 0;
			if (nLine < 0 || nLine >= (this).UnderlyingModel.Sentinel.ChildCount)
			{
				return false;
			}
			VariableListNode variableListNode = (this).UnderlyingModel.Sentinel.GetChild(nLine) as VariableListNode;
			if (variableListNode == null || variableListNode.Item == null)
			{
				return false;
			}
			variableListNode.GetOffset(nColumn, out nOffset, out nLength);
			if (nOffset < 0 && nLength == 0)
			{
				variableListNode.GetOffset(GetColumnIndex(VariableListColumns.Name), out nOffset, out nLength);
			}
			return true;
		}
	}
}
