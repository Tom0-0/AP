#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class TabularDeclarationList
	{
		private LinkedList<ModelToken> _tokens;

		private TabularDeclarationModel _model;

		private LinkedList<TabularDeclarationBlock> _blocks = new LinkedList<TabularDeclarationBlock>();

		private LDictionary<string, TabularDeclarationItem> _itemsByName = new LDictionary<string, TabularDeclarationItem>();

		internal LinkedList<TabularDeclarationBlock> Blocks => _blocks;

		internal TabularDeclarationModel Model => _model;

		internal TabularDeclarationList(LinkedList<ModelToken> tokens, TabularDeclarationModel model)
		{
			_tokens = tokens;
			_model = model;
			for (LinkedListNode<ModelToken> linkedListNode = _tokens.First; linkedListNode != null; linkedListNode = Common.GetNext(linkedListNode))
			{
				LinkedListNode<ModelToken> linkedListNode2 = null;
				if (linkedListNode.Value.HasType(ModelTokenType.AnyVarBlock))
				{
					linkedListNode2 = linkedListNode;
					while (linkedListNode != null)
					{
						if (linkedListNode.Value.HasType(ModelTokenType.EndVar))
						{
							_blocks.AddLast(new TabularDeclarationBlock(_tokens, this, linkedListNode2, linkedListNode));
							break;
						}
						linkedListNode = Common.GetNext(linkedListNode);
					}
				}
			}
		}

		internal void CompactifyBlocks()
		{
			bool flag = false;
			LinkedListNode<TabularDeclarationBlock> linkedListNode = _blocks.First;
			while (linkedListNode != null)
			{
				LinkedListNode<TabularDeclarationBlock> linkedListNode2 = linkedListNode;
				linkedListNode = linkedListNode.Next;
				if (linkedListNode2.Value.Items.Count == 0)
				{
					ModelTokenRange modelTokenRange = linkedListNode2.Value.CalculateLineBoundaryRange();
					Debug.Assert(modelTokenRange != null);
					modelTokenRange.Remove();
					_blocks.Remove(linkedListNode2);
				}
			}
			LinkedListNode<TabularDeclarationBlock> linkedListNode3 = _blocks.First;
			LinkedListNode<ModelToken> linkedListNode4 = null;
			while (linkedListNode3 != null)
			{
				LinkedListNode<TabularDeclarationBlock> next = linkedListNode3.Next;
				if (next == null)
				{
					break;
				}
				if (linkedListNode4 == null)
				{
					linkedListNode4 = linkedListNode3.Value.EndScopeToken;
				}
				if (linkedListNode3.Value.Scope == next.Value.Scope && linkedListNode3.Value.Constant == next.Value.Constant && linkedListNode3.Value.Retain == next.Value.Retain && linkedListNode3.Value.Persistent == next.Value.Persistent)
				{
					ModelTokenRange modelTokenRange2 = next.Value.Items.First.Value.CalculateLineBoundaryRange();
					new ModelTokenRange(linkedListNode4, modelTokenRange2.First).Remove();
					linkedListNode4 = next.Value.EndScopeToken;
					_blocks.Remove(next);
					flag = true;
				}
				else
				{
					linkedListNode3 = next;
					linkedListNode4 = null;
				}
			}
			if (flag)
			{
				_model.RefreshList();
			}
		}

		internal TabularDeclarationItem GetItem(string stItem)
		{
			TabularDeclarationItem result = default(TabularDeclarationItem);
			_itemsByName.TryGetValue(stItem, out result);
			return result;
		}

		internal void RegisterItem(TabularDeclarationItem item)
		{
			_itemsByName[item.Name]= item;
		}

		internal void UnregisterItem(TabularDeclarationItem item)
		{
			_itemsByName.Remove(item.Name);
		}

		public string CreateNewValidName(ModelTokenType scope)
		{
			string scopeText = Common.GetScopeText(scope, bConstant: false, bRetain: false, bPersistent: false);
			scopeText = scopeText.ToLowerInvariant();
			scopeText = scopeText.Replace("_", " ");
			scopeText = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(scopeText);
			scopeText = scopeText.Replace(" ", "");
			return GetValidName("new" + scopeText);
		}

		private string GetValidName(string defaultName)
		{
			int num = 1;
			string text = defaultName;
			while (CheckUniqueVariableNames(new string[1] { text }) != null)
			{
				text = defaultName + num;
				num++;
			}
			return text;
		}

		internal TabularDeclarationItem AppendDefaultItem(ModelTokenType scope)
		{
			TabularDeclarationBlock tabularDeclarationBlock = ((Blocks != null && Blocks.Last != null) ? Blocks.Last.Value : null);
			string stText = CreateNewValidName(scope);
			if (tabularDeclarationBlock != null && tabularDeclarationBlock.Scope == scope && !tabularDeclarationBlock.Constant && !tabularDeclarationBlock.Persistent && !tabularDeclarationBlock.Retain)
			{
				ModelToken modelToken = new ModelToken(ModelTokenType.Text, stText);
				ModelToken modelToken2 = new ModelToken(ModelTokenType.Semicolon, ";");
				LList<ModelToken> obj = new LList<ModelToken>();
				obj.Add(new ModelToken(ModelTokenType.Whitespace, "\t"));
				obj.Add(modelToken);
				obj.Add(new ModelToken(ModelTokenType.Colon, ":"));
				obj.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				obj.Add(new ModelToken(ModelTokenType.Text, "BOOL"));
				obj.Add(modelToken2);
				obj.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
				foreach (ModelToken item in obj)
				{
					_tokens.AddBefore(tabularDeclarationBlock.EndScopeToken, item);
				}
				TabularDeclarationItem tabularDeclarationItem = new TabularDeclarationItem(_tokens, tabularDeclarationBlock, _tokens.Find(modelToken), _tokens.Find(modelToken2));
				tabularDeclarationBlock.Items.AddLast(tabularDeclarationItem);
				tabularDeclarationBlock.List.RegisterItem(tabularDeclarationItem);
				return tabularDeclarationItem;
			}
			ModelToken modelToken3 = new ModelToken(scope, Common.GetScopeText(scope, bConstant: false, bRetain: false, bPersistent: false));
			ModelToken modelToken4 = new ModelToken(ModelTokenType.EndVar, "END_VAR");
			LList<ModelToken> val = new LList<ModelToken>();
			if (_tokens.Last.Value.Type != ModelTokenType.EndOfLine)
			{
				val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			}
			val.Add(modelToken3);
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			val.Add(new ModelToken(ModelTokenType.Whitespace, "\t"));
			val.Add(new ModelToken(ModelTokenType.Text, stText));
			val.Add(new ModelToken(ModelTokenType.Colon, ":"));
			val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
			val.Add(new ModelToken(ModelTokenType.Text, "BOOL"));
			val.Add(new ModelToken(ModelTokenType.Semicolon, ";"));
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			val.Add(modelToken4);
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			foreach (ModelToken item2 in val)
			{
				_tokens.AddLast(item2);
			}
			TabularDeclarationBlock tabularDeclarationBlock2 = new TabularDeclarationBlock(_tokens, this, _tokens.Find(modelToken3), _tokens.Find(modelToken4));
			_blocks.AddLast(tabularDeclarationBlock2);
			TabularDeclarationItem value = tabularDeclarationBlock2.Items.First.Value;
			RegisterItem(value);
			return value;
		}

		internal string AppendItem(SerializableTabularDeclarationItem item)
		{
			TabularDeclarationBlock tabularDeclarationBlock = ((Blocks != null && Blocks.Last != null) ? Blocks.Last.Value : null);
			SerializableModelToken[] tokens;
			if (tabularDeclarationBlock != null && tabularDeclarationBlock.Scope == item.Scope && tabularDeclarationBlock.Constant == item.Constant && tabularDeclarationBlock.Retain == item.Retain && tabularDeclarationBlock.Persistent == item.Persistent)
			{
				ModelToken modelToken = null;
				ModelToken value = null;
				tokens = item.Tokens;
				foreach (SerializableModelToken serializableModelToken in tokens)
				{
					ModelToken modelToken2 = new ModelToken(serializableModelToken.Type, serializableModelToken.Text);
					if (modelToken == null && serializableModelToken.Type == ModelTokenType.Text)
					{
						modelToken = modelToken2;
					}
					else if (serializableModelToken.Type == ModelTokenType.Semicolon)
					{
						value = modelToken2;
					}
					_tokens.AddBefore(tabularDeclarationBlock.EndScopeToken, modelToken2);
				}
				TabularDeclarationItem tabularDeclarationItem = new TabularDeclarationItem(_tokens, tabularDeclarationBlock, _tokens.Find(modelToken), _tokens.Find(value));
				tabularDeclarationBlock.Items.AddLast(tabularDeclarationItem);
				RegisterItem(tabularDeclarationItem);
				return tabularDeclarationItem.Name;
			}
			ModelToken modelToken3 = new ModelToken(item.Scope, Common.GetScopeText(item.Scope, bConstant: false, bRetain: false, bPersistent: false));
			ModelToken modelToken4 = new ModelToken(ModelTokenType.EndVar, "END_VAR");
			LList<ModelToken> val = new LList<ModelToken>();
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			val.Add(modelToken3);
			if (item.Constant)
			{
				val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val.Add(new ModelToken(ModelTokenType.Constant, "CONSTANT"));
			}
			if (item.Retain)
			{
				val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val.Add(new ModelToken(ModelTokenType.Retain, "RETAIN"));
			}
			if (item.Persistent)
			{
				val.Add(new ModelToken(ModelTokenType.Whitespace, " "));
				val.Add(new ModelToken(ModelTokenType.Persistent, "PERSISTENT"));
			}
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			tokens = item.Tokens;
			foreach (SerializableModelToken serializableModelToken2 in tokens)
			{
				val.Add(new ModelToken(serializableModelToken2.Type, serializableModelToken2.Text));
			}
			val.Add(modelToken4);
			val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
			foreach (ModelToken item2 in val)
			{
				_tokens.AddLast(item2);
			}
			TabularDeclarationBlock tabularDeclarationBlock2 = new TabularDeclarationBlock(_tokens, this, _tokens.Find(modelToken3), _tokens.Find(modelToken4));
			_blocks.AddLast(tabularDeclarationBlock2);
			TabularDeclarationItem value2 = tabularDeclarationBlock2.Items.First.Value;
			RegisterItem(value2);
			return value2.Name;
		}

		private string GetReturnVariable()
		{
			if (Model == null || Model.Header == null)
			{
				return null;
			}
			string kind = Model.Header.Kind;
			if (string.Equals(kind, "function", StringComparison.OrdinalIgnoreCase))
			{
				return Model.Header.Name;
			}
			if (string.Equals(kind, "method", StringComparison.OrdinalIgnoreCase))
			{
				return Model.Header.Name;
			}
			return null;
		}

		internal string CheckUniqueVariableNames(IEnumerable<string> additionalDeclarators)
		{
			LHashSet<string> val = new LHashSet<string>();
			string returnVariable = GetReturnVariable();
			if (returnVariable != null)
			{
				val.Add(returnVariable.ToLowerInvariant());
			}
			foreach (TabularDeclarationBlock block in _blocks)
			{
				foreach (TabularDeclarationItem item in block.Items)
				{
					string[] array = Common.ParseDeclaratorList(item.Name, bThrowExceptionOnError: false);
					foreach (string text in array)
					{
						string text2 = text.ToLowerInvariant();
						if (val.Contains(text2))
						{
							return text;
						}
						val.Add(text2);
					}
				}
			}
			if (additionalDeclarators != null)
			{
				foreach (string additionalDeclarator in additionalDeclarators)
				{
					string text3 = additionalDeclarator.ToLowerInvariant();
					if (val.Contains(text3))
					{
						return additionalDeclarator;
					}
					val.Add(text3);
				}
			}
			return null;
		}
	}
}
