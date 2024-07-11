#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class TabularDeclarationItem
	{
		private LinkedList<ModelToken> _tokens;

		private TabularDeclarationBlock _block;

		private LinkedListNode<ModelToken> _itemsToken;

		private ModelTokenRange _addressTokens;

		private LinkedListNode<ModelToken> _addressTextToken;

		private ModelTokenRange _dataTypeTokens;

		private LinkedListNode<ModelToken> _dataTypeTextToken;

		private ModelTokenRange _initializationTokens;

		private LinkedListNode<ModelToken> _initializationPrefixToken;

		private LinkedListNode<ModelToken> _initializationTextToken;

		private LinkedListNode<ModelToken> _semicolonToken;

		private LinkedListNode<ModelToken> _commentToken;

		private bool _bIsCommentBeforeDeclaration;

		private LinkedListNode<ModelToken>[] _attributeTokens;

		internal string Name
		{
			get
			{
				return _itemsToken.Value.Text;
			}
			set
			{
				_block.List.UnregisterItem(this);
				_itemsToken.Value.Text = value;
				_block.List.RegisterItem(this);
			}
		}

		internal int NameOffset => _itemsToken.Value.Offset;

		internal string Address
		{
			get
			{
				if (_addressTextToken == null)
				{
					return string.Empty;
				}
				return _addressTextToken.Value.Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (_addressTokens != null)
					{
						Debug.Assert(_addressTextToken != null);
						_addressTextToken.Value.Text = value;
						return;
					}
					ModelToken value2 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value3 = new ModelToken(ModelTokenType.At, "AT");
					ModelToken value4 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value5 = new ModelToken(ModelTokenType.Text, value);
					LinkedListNode<ModelToken> next = _itemsToken.Next;
					_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_itemsToken, value2), value3), value4), value5);
					_addressTokens = new ModelTokenRange(_tokens.Find(value2), next);
					_addressTextToken = _tokens.Find(value5);
				}
				else if (_addressTokens != null)
				{
					_addressTokens.Remove();
					_addressTokens = null;
					_addressTextToken = null;
				}
			}
		}

		internal int AddressOffset
		{
			get
			{
				if (_addressTextToken == null)
				{
					return -1;
				}
				return _addressTextToken.Value.Offset;
			}
		}

		internal string DataType
		{
			get
			{
				return _dataTypeTextToken.Value.Text;
			}
			set
			{
				_dataTypeTextToken.Value.Text = value;
			}
		}

		internal int DataTypeOffset => _dataTypeTextToken.Value.Offset;

		internal string Initialization
		{
			get
			{
				if (_initializationTextToken == null)
				{
					return string.Empty;
				}
				return _initializationTextToken.Value.Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (_initializationTokens != null)
					{
						Debug.Assert(_initializationTextToken != null);
						_initializationTextToken.Value.Text = value;
						return;
					}
					ModelToken value2 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value3 = new ModelToken(ModelTokenType.Assign, ":=");
					ModelToken value4 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value5 = new ModelToken(ModelTokenType.Text, value);
					LinkedListNode<ModelToken> next = _dataTypeTokens.Next;
					_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_dataTypeTextToken, value2), value3), value4), value5);
					_initializationTokens = new ModelTokenRange(_tokens.Find(value2), next);
					_initializationTextToken = _tokens.Find(value5);
				}
				else if (_initializationTokens != null)
				{
					_initializationTokens.Remove();
					_initializationTokens = null;
					_initializationTextToken = null;
				}
			}
		}

		internal int InitializationOffset
		{
			get
			{
				if (_initializationTextToken == null)
				{
					return -1;
				}
				return _initializationTextToken.Value.Offset;
			}
		}

		internal string Comment
		{
			get
			{
				return Common.GetCommentText(this._commentToken);
			}
			set
			{
				if (value == this.Comment)
				{
					return;
				}
				if (!this._bIsCommentBeforeDeclaration && this._commentToken != null && value != null && value.IndexOf('\n') < 0)
				{
					if (value != "")
					{
						value = ((this._commentToken.Value.Type != ModelTokenType.PascalCommentOccupyingWholeLine && this._commentToken.Value.Type != ModelTokenType.PascalCommentInMixedLine) ? ("// " + value + "\n") : ("(* " + value + " *)"));
					}
					else
					{
						value += "\n";
					}
					this._commentToken.Value.Text = value;
					return;
				}
				if (this._commentToken != null)
				{
					if (this._bIsCommentBeforeDeclaration)
					{
						LinkedListNode<ModelToken> next = this._commentToken.Next;
						while (next != null && !next.Value.HasType(ModelTokenType.Text | ModelTokenType.Pragma))
						{
							next = next.Next;
						}
						new ModelTokenRange(this._commentToken, next).Remove();
					}
					else
					{
						LinkedListNode<ModelToken> previous = this._commentToken.Previous;
						while (previous != null && previous.Value.HasType(ModelTokenType.AnyBlank))
						{
							previous = previous.Previous;
						}
						new ModelTokenRange(previous.Next, this._commentToken.Next).Remove();
						if (this._commentToken.Value.Type != ModelTokenType.PascalCommentInMixedLine && this._commentToken.Value.Type != ModelTokenType.PascalCommentOccupyingWholeLine)
						{
							this._tokens.AddAfter(previous, new ModelToken(ModelTokenType.EndOfLine, "\n"));
						}
					}
					this._commentToken = null;
				}
				if (!string.IsNullOrEmpty(value))
				{
					LinkedListNode<ModelToken> node = this._itemsToken;
					if (this._attributeTokens != null && this._attributeTokens.Length != 0)
					{
						node = this._attributeTokens[0];
					}
					this._commentToken = this._tokens.AddBefore(node, Common.CreateCommentToken(value, true));
					this._tokens.AddBefore(node, new ModelToken(ModelTokenType.Whitespace, "\t"));
					this._bIsCommentBeforeDeclaration = true;
				}
			}
		}

		internal int CommentOffset
		{
			get
			{
				if (_commentToken == null)
				{
					return -1;
				}
				return _commentToken.Value.Offset;
			}
		}

		internal string Attributes
		{
			get
			{
				return Common.GetAttributes(this._attributeTokens);
			}
			set
			{
				if (value == this.Attributes)
				{
					return;
				}
				if (this._attributeTokens != null)
				{
					LinkedListNode<ModelToken>[] attributeTokens = this._attributeTokens;
					foreach (LinkedListNode<ModelToken> linkedListNode in attributeTokens)
					{
						LinkedListNode<ModelToken> next = linkedListNode.Next;
						while (next != null && !next.Value.HasType(ModelTokenType.Text | ModelTokenType.DocumentationComment | ModelTokenType.Pragma | ModelTokenType.CPlusPlusCommentOccupyingWholeLine | ModelTokenType.CPlusPlusCommentInMixedLine | ModelTokenType.PascalCommentOccupyingWholeLine | ModelTokenType.PascalCommentInMixedLine))
						{
							next = next.Next;
						}
						new ModelTokenRange(linkedListNode, next).Remove();
					}
					this._attributeTokens = null;
				}
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				ModelToken[] array2 = Common.CreateAttributeBlockTokens(value, false);
				if (array2 == null || array2.Length == 0)
				{
					return;
				}
				LList<LinkedListNode<ModelToken>> llist = new LList<LinkedListNode<ModelToken>>();
				ModelToken[] array3 = array2;
				foreach (ModelToken modelToken in array3)
				{
					if (modelToken.Type == ModelTokenType.Pragma)
					{
						llist.Add(this._tokens.AddBefore(this._itemsToken, modelToken));
					}
					else
					{
						this._tokens.AddBefore(this._itemsToken, modelToken);
					}
				}
				this._tokens.AddBefore(this._itemsToken, new ModelToken(ModelTokenType.Whitespace, "\t"));
				this._attributeTokens = llist.ToArray();
			}
		}

		internal int AttributesOffset
		{
			get
			{
				if (_attributeTokens == null || _attributeTokens.Length == 0)
				{
					return -1;
				}
				return _attributeTokens[0].Value.Offset;
			}
		}

		internal TabularDeclarationBlock Block => _block;

		internal TabularDeclarationItem(LinkedList<ModelToken> tokens, TabularDeclarationBlock block, LinkedListNode<ModelToken> itemsToken, LinkedListNode<ModelToken> semicolonToken)
		{
			_tokens = tokens;
			_block = block;
			_semicolonToken = semicolonToken;
			Debug.Assert(itemsToken.Value.HasType(ModelTokenType.Text));
			_itemsToken = itemsToken;
			LinkedListNode<ModelToken> linkedListNode = Common.GetNext(itemsToken);
			if (linkedListNode != null && linkedListNode.Value.HasType(ModelTokenType.At))
			{
				_addressTextToken = Common.Match(linkedListNode, ModelTokenType.Text);
				Debug.Assert(_addressTextToken != null);
				_addressTokens = new ModelTokenRange(linkedListNode.Previous, _addressTextToken.Next);
				linkedListNode = Common.GetNext(_addressTextToken);
			}
			if (linkedListNode != null && linkedListNode.Value.HasType(ModelTokenType.Colon))
			{
				_dataTypeTextToken = Common.Match(linkedListNode, ModelTokenType.Text);
				Debug.Assert(_dataTypeTextToken != null);
				_dataTypeTokens = new ModelTokenRange(linkedListNode, _dataTypeTextToken.Next);
				linkedListNode = Common.GetNext(_dataTypeTextToken);
			}
			if (linkedListNode != null && linkedListNode.Value.HasType(ModelTokenType.Assign))
			{
				LinkedListNode<ModelToken> linkedListNode2 = linkedListNode;
				_initializationPrefixToken = Common.Match(linkedListNode, ModelTokenType.Struct);
				if (_initializationPrefixToken != null)
				{
					linkedListNode = _initializationPrefixToken;
				}
				_initializationTextToken = Common.Match(linkedListNode, ModelTokenType.Text);
				Debug.Assert(_initializationTextToken != null);
				if (linkedListNode2.Previous.Value.HasType(ModelTokenType.Whitespace))
				{
					linkedListNode2 = linkedListNode2.Previous;
				}
				_initializationTokens = new ModelTokenRange(linkedListNode2, _initializationTextToken.Next);
				linkedListNode = Common.GetNext(_initializationTextToken);
			}
			for (linkedListNode = _semicolonToken.Next; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (!linkedListNode.Value.HasType(ModelTokenType.Whitespace))
				{
					if (linkedListNode.Value.HasType(ModelTokenType.AnyComment))
					{
						_commentToken = linkedListNode;
						_bIsCommentBeforeDeclaration = false;
					}
					break;
				}
			}
			if (_commentToken == null)
			{
				for (linkedListNode = _itemsToken.Previous; linkedListNode != null; linkedListNode = linkedListNode.Previous)
				{
					if (!linkedListNode.Value.HasType(ModelTokenType.AnyBlank | ModelTokenType.Pragma))
					{
						if (linkedListNode.Value.HasType(ModelTokenType.DocumentationComment | ModelTokenType.CPlusPlusCommentOccupyingWholeLine | ModelTokenType.PascalCommentOccupyingWholeLine))
						{
							_commentToken = linkedListNode;
							_bIsCommentBeforeDeclaration = true;
						}
						break;
					}
				}
			}
			LList<LinkedListNode<ModelToken>> val = new LList<LinkedListNode<ModelToken>>();
			for (linkedListNode = _itemsToken.Previous; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				if (!linkedListNode.Value.HasType(ModelTokenType.AnyBlankOrComment))
				{
					if (!linkedListNode.Value.HasType(ModelTokenType.Pragma))
					{
						break;
					}
					val.Add(linkedListNode);
				}
			}
			_attributeTokens = val.ToArray();
			Array.Reverse(_attributeTokens);
			Debug.Assert(_itemsToken != null);
			Debug.Assert(_dataTypeTokens != null);
			Debug.Assert(_dataTypeTextToken != null);
			Debug.Assert(_semicolonToken != null);
			block.List.RegisterItem(this);
		}

		internal void ChangeBlock(ModelTokenType scope, bool bConstant, bool bRetain, bool bPersistent)
		{
			ModelTokenRange modelTokenRange = CalculateLineBoundaryRange();
			if (modelTokenRange == null || modelTokenRange.First == null)
			{
				return;
			}
			LList<ModelToken> val = new LList<ModelToken>();
			val.Add(new ModelToken(ModelTokenType.EndVar, "END_VAR"));
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			val.Add(new ModelToken(scope, Common.GetScopeText(scope, bConstant: false, bRetain: false, bPersistent: false)));
			if (bConstant)
			{
				val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val.Add(new ModelToken(ModelTokenType.Constant, "CONSTANT"));
			}
			if (bRetain)
			{
				val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val.Add(new ModelToken(ModelTokenType.Retain, "RETAIN"));
			}
			if (bPersistent)
			{
				val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val.Add(new ModelToken(ModelTokenType.Persistent, "PERSISTENT"));
			}
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			LList<ModelToken> val2 = new LList<ModelToken>();
			val2.Add(new ModelToken(ModelTokenType.EndVar, "END_VAR"));
			val2.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			val2.Add(new ModelToken(_block.Scope, Common.GetScopeText(_block.Scope, bConstant: false, bRetain: false, bPersistent: false)));
			if (_block.Constant)
			{
				val2.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val2.Add(new ModelToken(ModelTokenType.Constant, "CONSTANT"));
			}
			if (_block.Retain)
			{
				val2.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val2.Add(new ModelToken(ModelTokenType.Retain, "RETAIN"));
			}
			if (_block.Persistent)
			{
				val2.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val2.Add(new ModelToken(ModelTokenType.Persistent, "PERSISTENT"));
			}
			val2.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			foreach (ModelToken item in val)
			{
				_tokens.AddBefore(modelTokenRange.First, item);
			}
			foreach (ModelToken item2 in val2)
			{
				if (modelTokenRange.Next != null)
				{
					_tokens.AddBefore(modelTokenRange.Next, item2);
				}
				else
				{
					_tokens.AddLast(item2);
				}
			}
			_block.List.Model.RefreshList();
			_block.List.Model.List.CompactifyBlocks();
		}

		internal void Delete()
		{
			ModelTokenRange modelTokenRange = CalculateLineBoundaryRange();
			if (modelTokenRange != null && modelTokenRange.First != null)
			{
				modelTokenRange.Remove();
				_block.Items.Remove(this);
				_block.List.UnregisterItem(this);
				_block.List.CompactifyBlocks();
			}
		}

		internal ModelTokenRange CalculateLineBoundaryRange()
		{
			LinkedListNode<ModelToken> itemsToken = _itemsToken;
			itemsToken = itemsToken.Previous;
			while (itemsToken != null && itemsToken.Value.HasType(ModelTokenType.AnyBlankOrComment | ModelTokenType.Pragma))
			{
				itemsToken = itemsToken.Previous;
			}
			while (itemsToken != null)
			{
				if (itemsToken.Value.HasType(ModelTokenType.CPlusPlusCommentOccupyingWholeLine) || itemsToken.Value.HasType(ModelTokenType.CPlusPlusCommentInMixedLine) || itemsToken.Value.HasType(ModelTokenType.EndOfLine))
				{
					itemsToken = itemsToken.Next;
					break;
				}
				itemsToken = itemsToken.Next;
			}
			LinkedListNode<ModelToken> semicolonToken = _semicolonToken;
			for (semicolonToken = semicolonToken.Next; semicolonToken != null; semicolonToken = semicolonToken.Next)
			{
				if (semicolonToken.Value.HasType(ModelTokenType.CPlusPlusCommentOccupyingWholeLine) || semicolonToken.Value.HasType(ModelTokenType.CPlusPlusCommentInMixedLine) || semicolonToken.Value.HasType(ModelTokenType.EndOfLine))
				{
					semicolonToken = semicolonToken.Next;
					break;
				}
			}
			return new ModelTokenRange(itemsToken, semicolonToken);
		}

		internal TabularDeclarationItem Duplicate(bool bAfter, TabularDeclarationList contextList)
		{
			ModelTokenRange modelTokenRange = CalculateLineBoundaryRange();
			if (modelTokenRange == null || modelTokenRange.First == null)
			{
				return null;
			}
			string stText = contextList.CreateNewValidName(_block.Scope);
			ModelToken modelToken = new ModelToken(ModelTokenType.Text, stText);
			ModelToken modelToken2 = new ModelToken(ModelTokenType.Semicolon, ";");
			LList<ModelToken> val = new LList<ModelToken>();
			val.Add(new ModelToken(ModelTokenType.Whitespace, "\t"));
			val.Add(modelToken);
			val.Add(new ModelToken(ModelTokenType.Colon, ":"));
			val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
			val.Add(new ModelToken(ModelTokenType.Text, DataType));
			val.Add(modelToken2);
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			if (bAfter)
			{
				foreach (ModelToken item in val)
				{
					if (modelTokenRange.Next != null)
					{
						_tokens.AddBefore(modelTokenRange.Next, item);
					}
					else
					{
						_tokens.AddLast(item);
					}
				}
			}
			else
			{
				foreach (ModelToken item2 in val)
				{
					_tokens.AddBefore(modelTokenRange.First, item2);
				}
			}
			TabularDeclarationItem tabularDeclarationItem = new TabularDeclarationItem(_tokens, _block, _tokens.Find(modelToken), _tokens.Find(modelToken2));
			if (bAfter)
			{
				_block.Items.AddAfter(_block.Items.Find(this), tabularDeclarationItem);
			}
			else
			{
				_block.Items.AddBefore(_block.Items.Find(this), tabularDeclarationItem);
			}
			_block.List.RegisterItem(tabularDeclarationItem);
			return tabularDeclarationItem;
		}

		internal string Insert(SerializableTabularDeclarationItem item)
		{
			ModelTokenRange modelTokenRange = CalculateLineBoundaryRange();
			if (modelTokenRange == null || modelTokenRange.First == null)
			{
				return null;
			}
			LinkedListNode<ModelToken> linkedListNode = null;
			LinkedListNode<ModelToken> linkedListNode2 = null;
			SerializableModelToken[] tokens = item.Tokens;
			foreach (SerializableModelToken serializableModelToken in tokens)
			{
				LinkedListNode<ModelToken> linkedListNode3 = _tokens.AddBefore(modelTokenRange.First, new ModelToken(serializableModelToken.Type, serializableModelToken.Text));
				if (linkedListNode == null && linkedListNode3.Value.HasType(ModelTokenType.Text))
				{
					linkedListNode = linkedListNode3;
				}
				else if (linkedListNode2 == null && linkedListNode3.Value.HasType(ModelTokenType.Semicolon))
				{
					linkedListNode2 = linkedListNode3;
				}
			}
			TabularDeclarationItem tabularDeclarationItem = new TabularDeclarationItem(_tokens, _block, linkedListNode, linkedListNode2);
			_block.Items.AddBefore(_block.Items.Find(this), tabularDeclarationItem);
			_block.List.RegisterItem(tabularDeclarationItem);
			string name = tabularDeclarationItem.Name;
			tabularDeclarationItem.ChangeBlock(item.Scope, item.Constant, item.Retain, item.Persistent);
			return name;
		}
	}
}
