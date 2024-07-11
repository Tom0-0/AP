#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class VariableListNode : AbstractVariableListNode
	{
		private delegate void DoSetValueDelegate(int nColumnIndex, object value);

		private bool _bBookmark;

		private int _nLineNumber;

		private string _stItem;

		private int _nPendingIndex;

		internal ModelTokenType Scope
		{
			get
			{
				return Item.Block.Scope;
			}
			set
			{
				if (base.Model.AttemptModification())
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetScopeAction(base.Model, _stItem, Scope, value), bPerform: true, adjustLineNumbers: true);
				}
			}
		}

		internal bool Constant
		{
			get
			{
				return Item.Block.Constant;
			}
			set
			{
				if (base.Model.AttemptModification())
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetConstantAction(base.Model, _stItem, Constant, value), bPerform: true, adjustLineNumbers: true);
				}
			}
		}

		internal bool Retain
		{
			get
			{
				return Item.Block.Retain;
			}
			set
			{
				if (base.Model.AttemptModification())
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetRetainAction(base.Model, _stItem, Retain, value), bPerform: true, adjustLineNumbers: true);
				}
			}
		}

		internal bool Persistent
		{
			get
			{
				return Item.Block.Persistent;
			}
			set
			{
				if (base.Model.AttemptModification())
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetPersistentAction(base.Model, _stItem, Persistent, value), bPerform: true, adjustLineNumbers: true);
				}
			}
		}

		internal string Name => Item.Name;

		internal TabularDeclarationItem Item => base.Model.GetItem(_stItem);

		internal VariableListNode(VariableListModel model, int nLineNumber, string stItem, int nPendingIndex)
			: base(model)
		{
			_nLineNumber = nLineNumber;
			_stItem = stItem;
			_nPendingIndex = nPendingIndex;
		}

		public object GetRealValue(int nColumnIndex)
		{
			VariableListColumns variableListColumns;
			if (nColumnIndex == 9)
			{
				variableListColumns = VariableListColumns.Attributes;
			}
			else
			{
				variableListColumns = base.Model.GetColumnMeaning(nColumnIndex);
			}
			if (variableListColumns <= VariableListColumns.Initialization)
			{
				if (variableListColumns <= VariableListColumns.Address)
				{
					switch (variableListColumns)
					{
						case VariableListColumns.Line:
							return new LineCellData(this._nLineNumber, this._bBookmark);
						case VariableListColumns.Scope:
							return new IconLabelTreeTableViewCellData(base.Model.GetScopeImage(this.Scope), Common.GetScopeText(this.Scope, this.Item.Block.Constant, this.Item.Block.Retain, this.Item.Block.Persistent));
						case VariableListColumns.Line | VariableListColumns.Scope:
							break;
						case VariableListColumns.Name:
							return this.Item.Name;
						default:
							if (variableListColumns == VariableListColumns.Address)
							{
								return this.Item.Address;
							}
							break;
					}
				}
				else
				{
					if (variableListColumns == VariableListColumns.DataType)
					{
						return this.Item.DataType;
					}
					if (variableListColumns == VariableListColumns.Initialization)
					{
						return this.Item.Initialization;
					}
				}
			}
			else if (variableListColumns <= VariableListColumns.Constant)
			{
				if (variableListColumns == VariableListColumns.RetainPersistent)
				{
					return this.Retain && this.Persistent;
				}
				if (variableListColumns == VariableListColumns.Constant)
				{
					return this.Constant;
				}
			}
			else
			{
				if (variableListColumns == VariableListColumns.Comment)
				{
					return this.Item.Comment;
				}
				if (variableListColumns == VariableListColumns.Attributes)
				{
					return this.Item.Attributes;
				}
				if (variableListColumns == VariableListColumns.NothingToDeclare)
				{
					return string.Empty;
				}
			}
			return null;
		}

		internal void DoSetScope(ModelTokenType scope)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			Item.ChangeBlock(scope, Item.Block.Constant, Item.Block.Retain, Item.Block.Persistent);
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetConstant(bool bConstant)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			Item.ChangeBlock(Scope, bConstant, Retain, Persistent);
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetRetain(bool bRetain)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			Item.ChangeBlock(Scope, Constant, bRetain, Persistent);
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetPersistent(bool bPersistent)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			Item.ChangeBlock(Scope, Constant, Retain, bPersistent);
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void SetLineNumber(int nLineNumber, bool bBookmark)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			if (nLineNumber != _nLineNumber || bBookmark != _bBookmark)
			{
				_nLineNumber = nLineNumber;
				_bBookmark = bBookmark;
				((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, nLineNumber - 1, (ITreeTableNode)(object)this));
			}
		}

		public override object GetValue(int nColumnIndex)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Expected O, but got Unknown
			return base.Model.GetColumnMeaning(nColumnIndex) switch
			{
				VariableListColumns.Line => new LineCellData(_nLineNumber, _bBookmark), 
				VariableListColumns.Scope => (object)new IconLabelTreeTableViewCellData(base.Model.GetScopeImage(Scope), (object)Common.GetScopeText(Scope, Item.Block.Constant, Item.Block.Retain, Item.Block.Persistent)), 
				VariableListColumns.Name => Item.Name, 
				VariableListColumns.Address => Item.Address, 
				VariableListColumns.DataType => Item.DataType, 
				VariableListColumns.Initialization => Item.Initialization, 
				VariableListColumns.Comment => Item.Comment, 
				VariableListColumns.Attributes => HideElemCommentAttr(Item.Attributes),
				VariableListColumns.NothingToDeclare => string.Empty, 
				_ => null, 
			};
		}

		/// <summary>
		/// Òþ²ØÌØÐÔ×¢ÊÍ
		/// </summary>
		/// <param name="sAttr"></param>
		/// <returns></returns>
		private string HideElemCommentAttr(string sAttr)
		{
			if (string.IsNullOrEmpty(sAttr))
			{
				return string.Empty;
			}
			string[] array = sAttr.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.IndexOf("ElemComment") == -1)
				{
					stringBuilder.Append(text);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		public override bool IsEditable(int nColumnIndex)
		{
			VariableListColumns columnMeaning = base.Model.GetColumnMeaning(nColumnIndex);
			if ((uint)(columnMeaning - 1) <= 1u)
			{
				return false;
			}
			return true;
		}

		public override void SetValue(int nColumnIndex, object value)
		{
			//if (AutoRefactoringHelper.AutoRenameVariableEnabled)
			//{
			//	APEnvironment.Engine.InvokeInPrimaryThread((Delegate)new DoSetValueDelegate(DoSetValueWithTryCatch), new object[2] { nColumnIndex, value }, true);
			//}
			//else
			//{
			//	DoSetValue(nColumnIndex, value);
			//}
			if (nColumnIndex == 9)
			{
				nColumnIndex = base.Model.GetColumnIndex(VariableListColumns.Attributes);
			}
			this.DoSetValue(nColumnIndex, value);
		}

		private void DoSetValueWithTryCatch(int nColumnIndex, object value)
		{
			try
			{
				DoSetValue(nColumnIndex, value);
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(ex.Message, "Error_DoSetValue", Array.Empty<object>());
			}
		}

		private void DoSetValue(int nColumnIndex, object value)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Invalid comparison between Unknown and I4
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Expected O, but got Unknown
			if (!base.Model.AttemptModification())
			{
				return;
			}
			VariableListColumns columnMeaning = base.Model.GetColumnMeaning(nColumnIndex);
			bool flag = false;
			switch (columnMeaning)
			{
			case VariableListColumns.Line:
			case VariableListColumns.Scope:
			case VariableListColumns.NothingToDeclare:
				throw new InvalidOperationException("This node is not editable.");
			case VariableListColumns.Name:
			{
				string text = ((string)value).Trim();
				if (_nPendingIndex >= 0)
				{
					DoSetName(text);
					if (Item != null)
					{
						base.Model.RecordAction((IUndoableAction)(object)new InsertAction(base.Model, text, _nPendingIndex, new SerializableTabularDeclarationItem(Item)), bPerform: false, adjustLineNumbers: true);
						_nPendingIndex = -1;
						flag = true;
					}
				}
				else if (Name != text && (int)CheckRefactoring(text) == 1)
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetNameAction(base.Model, Name, text), bPerform: true, adjustLineNumbers: true);
					flag = true;
				}
				break;
			}
			case VariableListColumns.Address:
				if (Item.Address != (string)value)
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetAddressAction(base.Model, Name, Item.Address, (string)value), bPerform: true, adjustLineNumbers: true);
					flag = true;
				}
				break;
			case VariableListColumns.DataType:
				if (Item.DataType != (string)value)
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetDataTypeAction(base.Model, Name, Item.DataType, (string)value), bPerform: true, adjustLineNumbers: true);
					flag = true;
				}
				break;
			case VariableListColumns.Initialization:
				if (Item.Initialization != (string)value)
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetInitializationAction(base.Model, Name, Item.Initialization, (string)value), bPerform: true, adjustLineNumbers: true);
					flag = true;
				}
				break;
			case VariableListColumns.Comment:
				if (Item.Comment != (string)value)
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetCommentAction(base.Model, Name, Item.Comment, (string)value), bPerform: true, adjustLineNumbers: true);
					flag = true;
				}
				break;
			case VariableListColumns.Attributes:
				if (Item.Attributes != (string)value)
				{
					base.Model.RecordAction((IUndoableAction)(object)new SetAttributesAction(base.Model, Name, Item.Attributes, (string)value), bPerform: true, adjustLineNumbers: true);
					flag = true;
				}
				break;
			}
			if (flag)
			{
				((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
			}
		}

		private AutomaticRefactoringQueryResult CheckRefactoring(string stNewName)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (AutoRefactoringHelper.AutoRenameVariableEnabled)
			{
				TabularDeclarationEditor editor = base.Model.Editor;
				if (editor != null)
				{
					IEditor editor2 = editor.Editor;
					if (editor2 != null)
					{
						return AutoRefactoringHelper.TryQueryAndPerformRenameOfVariable(Name, stNewName, editor2);
					}
				}
			}
			return (AutomaticRefactoringQueryResult)1;
		}

		internal void DoSetName(string stName)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			string[] array = Common.ParseDeclaratorList(stName, bThrowExceptionOnError: true);
			stName = string.Join(", ", array);
			string[] subtrahend = Common.ParseDeclaratorList(_stItem, bThrowExceptionOnError: false);
			IEnumerable<string> additionalDeclarators = Common.SubtractDeclaratorList(array, subtrahend);
			string text = Item.Block.List.CheckUniqueVariableNames(additionalDeclarators);
			if (text != null)
			{
				throw new ApplicationException(string.Format(Resources.MultipleDeclaration, text));
			}
			TabularDeclarationItem item = Item;
			_stItem = stName;
			item.Name = _stItem;
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetAddress(string stAddress)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			Common.ParseAddress(stAddress, bThrowExceptionOnError: true);
			stAddress = stAddress.Trim();
			Item.Address = stAddress;
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetDataType(string stDataType)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			Common.ParseDataType(stDataType, base.Model.ResolvedContext, Scope, bThrowExceptionOnError: true);
			stDataType = stDataType.Trim();
			Item.DataType = stDataType;
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetInitialization(string stInitialization)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			Common.ParseInitValue(stInitialization, bThrowExceptionOnError: true);
			stInitialization = stInitialization.Trim();
			Item.Initialization = stInitialization;
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetComment(string stComment)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			Item.Comment = stComment;
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal void DoSetAttributes(string stAttributes)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			Item.Attributes = stAttributes;
			((AbstractTreeTableModel)base.Model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, ((AbstractTreeTableModel)base.Model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
		}

		internal int CompareTo(VariableListNode otherNode, VariableListColumns sortColumn)
		{
			if (otherNode == null)
			{
				throw new ArgumentNullException("otherNode");
			}
			switch (sortColumn)
			{
			case VariableListColumns.Line:
				return _nLineNumber - otherNode._nLineNumber;
			case VariableListColumns.Scope:
			{
				string scopeText = Common.GetScopeText(Scope, Item.Block.Constant, Item.Block.Retain, Item.Block.Persistent);
				string scopeText2 = Common.GetScopeText(otherNode.Scope, otherNode.Item.Block.Constant, otherNode.Item.Block.Retain, otherNode.Item.Block.Persistent);
				return string.Compare(scopeText, scopeText2, ignoreCase: true);
			}
			case VariableListColumns.Name:
				return string.Compare(Item.Name, otherNode.Item.Name, ignoreCase: true);
			case VariableListColumns.Address:
				return string.Compare(Item.Address, otherNode.Item.Address, ignoreCase: true);
			case VariableListColumns.DataType:
				return string.Compare(Item.DataType, otherNode.Item.DataType, ignoreCase: true);
			case VariableListColumns.Initialization:
				return string.Compare(Item.Initialization, otherNode.Item.Initialization, ignoreCase: true);
			case VariableListColumns.Comment:
				return string.Compare(Item.Comment, otherNode.Item.Comment, ignoreCase: true);
			case VariableListColumns.Attributes:
				return string.Compare(Item.Attributes, otherNode.Item.Attributes, ignoreCase: true);
			case VariableListColumns.NothingToDeclare:
				return 0;
			default:
				Debug.Fail("Sort not implemented for this column!");
				return 0;
			}
		}

		internal void GetOffset(int nColumnIndex, out int nOffset, out int nLength)
		{
			VariableListColumns columnMeaning = base.Model.GetColumnMeaning(nColumnIndex);
			if (columnMeaning <= VariableListColumns.Initialization)
			{
				switch (columnMeaning)
				{
				case VariableListColumns.Name:
					nOffset = Item.NameOffset;
					nLength = Item.Name.Length;
					return;
				case VariableListColumns.Address:
					nOffset = Item.AddressOffset;
					nLength = Item.Address.Length;
					return;
				case VariableListColumns.DataType:
					nOffset = Item.DataTypeOffset;
					nLength = Item.DataType.Length;
					return;
				case VariableListColumns.Initialization:
					nOffset = Item.InitializationOffset;
					nLength = Item.Initialization.Length;
					return;
				}
			}
			else
			{
				if (columnMeaning == VariableListColumns.Comment)
				{
					nOffset = Item.CommentOffset;
					nLength = Item.Comment.Length;
					return;
				}
				if (columnMeaning == VariableListColumns.Attributes)
				{
					nOffset = Item.AttributesOffset;
					nLength = Item.Attributes.Length;
					return;
				}
				_ = 256;
			}
			nOffset = -1;
			nLength = 0;
		}
	}
}
