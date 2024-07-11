using System.Collections.Generic;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class TabularDeclarationBlock
	{
		private LinkedList<ModelToken> _tokens;

		private TabularDeclarationList _list;

		private LinkedListNode<ModelToken> _scopeToken;

		private LinkedListNode<ModelToken> _endScopeToken;

		private ModelTokenRange _constantTokens;

		private LinkedListNode<ModelToken> _constantTextToken;

		private ModelTokenRange _retainTokens;

		private LinkedListNode<ModelToken> _retainTextToken;

		private ModelTokenRange _persistentTokens;

		private LinkedListNode<ModelToken> _persistentTextToken;

		private LinkedList<TabularDeclarationItem> _items = new LinkedList<TabularDeclarationItem>();

		internal ModelTokenType Scope => _scopeToken.Value.Type;

		internal bool Constant => _constantTextToken != null;

		internal bool Retain => _retainTextToken != null;

		internal bool Persistent => _persistentTextToken != null;

		internal LinkedList<TabularDeclarationItem> Items => _items;

		internal TabularDeclarationList List => _list;

		internal LinkedListNode<ModelToken> ScopeToken => _scopeToken;

		internal LinkedListNode<ModelToken> EndScopeToken => _endScopeToken;

		internal TabularDeclarationBlock(LinkedList<ModelToken> tokens, TabularDeclarationList list, LinkedListNode<ModelToken> scopeToken, LinkedListNode<ModelToken> endScopeToken)
		{
			_tokens = tokens;
			_list = list;
			_scopeToken = scopeToken;
			_endScopeToken = endScopeToken;
			LinkedListNode<ModelToken> next = Common.GetNext(_scopeToken);
			while (next != null)
			{
				if (next.Value.HasType(ModelTokenType.Constant))
				{
					_constantTextToken = next;
					_constantTokens = new ModelTokenRange(_constantTextToken.Previous, _constantTextToken.Next);
					next = Common.GetNext(next);
					continue;
				}
				if (next.Value.HasType(ModelTokenType.Retain))
				{
					_retainTextToken = next;
					_retainTokens = new ModelTokenRange(_retainTextToken.Previous, _retainTextToken.Next);
					next = Common.GetNext(next);
					continue;
				}
				if (!next.Value.HasType(ModelTokenType.Persistent))
				{
					break;
				}
				_persistentTextToken = next;
				_persistentTokens = new ModelTokenRange(_persistentTextToken.Previous, _persistentTextToken.Next);
				next = Common.GetNext(next);
			}
			while (next != null && next != _endScopeToken)
			{
				LinkedListNode<ModelToken> itemsToken = next;
				bool flag = false;
				while (next != null)
				{
					if (next.Value.HasType(ModelTokenType.Semicolon))
					{
						if (flag)
						{
							_items.AddLast(new TabularDeclarationItem(_tokens, this, itemsToken, next));
						}
						break;
					}
					if (next.Value.HasType(ModelTokenType.Text))
					{
						flag = true;
					}
					next = Common.GetNext(next);
				}
				next = Common.GetNext(next);
			}
		}

		internal ModelTokenRange CalculateLineBoundaryRange()
		{
			LinkedListNode<ModelToken> scopeToken = _scopeToken;
			scopeToken = scopeToken.Previous;
			while (scopeToken != null && scopeToken.Value.HasType(ModelTokenType.AnyBlankOrComment | ModelTokenType.Pragma))
			{
				scopeToken = scopeToken.Previous;
			}
			while (scopeToken != null)
			{
				if (scopeToken.Value.HasType(ModelTokenType.CPlusPlusCommentOccupyingWholeLine) || scopeToken.Value.HasType(ModelTokenType.CPlusPlusCommentInMixedLine) || scopeToken.Value.HasType(ModelTokenType.EndOfLine))
				{
					scopeToken = scopeToken.Next;
					break;
				}
				scopeToken = scopeToken.Next;
			}
			LinkedListNode<ModelToken> endScopeToken = _endScopeToken;
			for (endScopeToken = endScopeToken.Next; endScopeToken != null; endScopeToken = endScopeToken.Next)
			{
				if (endScopeToken.Value.HasType(ModelTokenType.CPlusPlusCommentOccupyingWholeLine) || endScopeToken.Value.HasType(ModelTokenType.CPlusPlusCommentInMixedLine) || endScopeToken.Value.HasType(ModelTokenType.EndOfLine))
				{
					endScopeToken = endScopeToken.Next;
					break;
				}
			}
			return new ModelTokenRange(scopeToken, endScopeToken);
		}
	}
}
