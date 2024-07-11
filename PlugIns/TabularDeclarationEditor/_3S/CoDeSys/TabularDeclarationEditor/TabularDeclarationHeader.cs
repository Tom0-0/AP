#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class TabularDeclarationHeader
	{
		private LinkedList<ModelToken> _tokens;

		private LinkedListNode<ModelToken> _kindToken;

		private LinkedListNode<ModelToken> _nameToken;

		private ModelTokenRange _returnTypeTokens;

		private LinkedListNode<ModelToken> _returnTypeTextToken;

		private ModelTokenRange _extendsTokens;

		private LinkedListNode<ModelToken> _extendsTextToken;

		private ModelTokenRange _implementsTokens;

		private LinkedListNode<ModelToken> _implementsTextToken;

		private LinkedListNode<ModelToken> _commentToken;

		private LinkedListNode<ModelToken>[] _attributeTokens;

		internal string Kind
		{
			get
			{
				return _kindToken.Value.Text;
			}
			set
			{
				_kindToken.Value.Text = value;
			}
		}

		internal string Name
		{
			get
			{
				return _nameToken.Value.Text;
			}
			set
			{
				_nameToken.Value.Text = value;
			}
		}

		internal string ReturnType
		{
			get
			{
				if (_returnTypeTextToken == null)
				{
					return string.Empty;
				}
				return _returnTypeTextToken.Value.Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (_returnTypeTokens != null)
					{
						Debug.Assert(_returnTypeTextToken != null);
						_returnTypeTextToken.Value.Text = value;
						return;
					}
					ModelToken value2 = new ModelToken(ModelTokenType.Colon, ":");
					ModelToken value3 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value4 = new ModelToken(ModelTokenType.Text, value);
					LinkedListNode<ModelToken> next = _nameToken.Next;
					_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_nameToken, value2), value3), value4);
					_returnTypeTokens = new ModelTokenRange(_tokens.Find(value2), next);
					_returnTypeTextToken = _tokens.Find(value4);
				}
				else if (_returnTypeTokens != null)
				{
					_returnTypeTokens.Remove();
					_returnTypeTokens = null;
					_returnTypeTextToken = null;
				}
			}
		}

		internal string Extends
		{
			get
			{
				if (_extendsTextToken == null)
				{
					return string.Empty;
				}
				return _extendsTextToken.Value.Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (_extendsTokens != null)
					{
						Debug.Assert(_extendsTextToken != null);
						_extendsTextToken.Value.Text = value;
						return;
					}
					ModelToken value2 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value3 = new ModelToken(ModelTokenType.Extends, "EXTENDS");
					ModelToken value4 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value5 = new ModelToken(ModelTokenType.Text, value);
					LinkedListNode<ModelToken> next = _nameToken.Next;
					_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_nameToken, value2), value3), value4), value5);
					_extendsTokens = new ModelTokenRange(_tokens.Find(value2), next);
					_extendsTextToken = _tokens.Find(value5);
				}
				else if (_extendsTokens != null)
				{
					_extendsTokens.Remove();
					_extendsTokens = null;
					_extendsTextToken = null;
				}
			}
		}

		internal string Implements
		{
			get
			{
				if (_implementsTextToken == null)
				{
					return string.Empty;
				}
				return _implementsTextToken.Value.Text;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (_implementsTokens != null)
					{
						Debug.Assert(_implementsTextToken != null);
						_implementsTextToken.Value.Text = value;
						return;
					}
					ModelToken value2 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value3 = new ModelToken(ModelTokenType.Implements, "IMPLEMENTS");
					ModelToken value4 = new ModelToken(ModelTokenType.Whitespace, " ");
					ModelToken value5 = new ModelToken(ModelTokenType.Text, value);
					LinkedListNode<ModelToken> linkedListNode = ((_extendsTokens == null) ? _nameToken : ((_extendsTokens.Next != null) ? _extendsTokens.Next.Previous : _tokens.Last));
					LinkedListNode<ModelToken> next = linkedListNode.Next;
					_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(_tokens.AddAfter(linkedListNode, value2), value3), value4), value5);
					_implementsTokens = new ModelTokenRange(_tokens.Find(value2), next);
					_implementsTextToken = _tokens.Find(value5);
				}
				else if (_implementsTokens != null)
				{
					_implementsTokens.Remove();
					_implementsTokens = null;
					_implementsTextToken = null;
				}
			}
		}

		internal string Comment
		{
			get
			{
				return Common.GetCommentText(_commentToken);
			}
			set
			{
				if (value == Comment)
				{
					return;
				}
				if (_commentToken != null)
				{
					LinkedListNode<ModelToken> next = _commentToken.Next;
					while (next != null && next.Value.HasType(ModelTokenType.AnyComment))
					{
						next = next.Next;
					}
					new ModelTokenRange(_commentToken, next).Remove();
					_commentToken = null;
				}
				if (!string.IsNullOrEmpty(value))
				{
					LinkedListNode<ModelToken> node = _kindToken;
					if (_attributeTokens != null && _attributeTokens.Length != 0)
					{
						node = _attributeTokens[0];
					}
					_commentToken = _tokens.AddBefore(node, Common.CreateCommentToken(value, bIndent: false));
				}
			}
		}

		internal string Attributes
		{
			get
			{
				return Common.GetAttributes(_attributeTokens);
			}
			set
			{
				if (value == Attributes)
				{
					return;
				}
				if (_attributeTokens != null)
				{
					LinkedListNode<ModelToken>[] attributeTokens = _attributeTokens;
					foreach (LinkedListNode<ModelToken> linkedListNode in attributeTokens)
					{
						LinkedListNode<ModelToken> next = linkedListNode.Next;
						while (next != null && !next.Value.HasType(ModelTokenType.AnyKind | ModelTokenType.AnyComment | ModelTokenType.Pragma))
						{
							next = next.Next;
						}
						new ModelTokenRange(linkedListNode, next).Remove();
					}
					_attributeTokens = null;
				}
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				ModelToken[] array = Common.CreateAttributeBlockTokens(value, bIndent: false);
				if (array == null || array.Length == 0)
				{
					return;
				}
				LList<LinkedListNode<ModelToken>> val = new LList<LinkedListNode<ModelToken>>();
				ModelToken[] array2 = array;
				foreach (ModelToken modelToken in array2)
				{
					if (modelToken.Type == ModelTokenType.Pragma)
					{
						val.Add(_tokens.AddBefore(_kindToken, modelToken));
					}
					else
					{
						_tokens.AddBefore(_kindToken, modelToken);
					}
				}
				_attributeTokens = val.ToArray();
			}
		}

		internal TabularDeclarationHeader(LinkedList<ModelToken> tokens)
		{
			_tokens = tokens;
			if (_tokens.First.Value.HasType(ModelTokenType.AnyKind))
			{
				_kindToken = _tokens.First;
			}
			else
			{
				_kindToken = Common.Match(_tokens.First, ModelTokenType.AnyKind);
			}
			Debug.Assert(_kindToken != null);
			_nameToken = Common.Match(_kindToken, ModelTokenType.Text);
			Debug.Assert(_nameToken != null);
			LinkedListNode<ModelToken> next = Common.GetNext(_nameToken);
			if (next != null && next.Value.HasType(ModelTokenType.Colon))
			{
				_returnTypeTextToken = Common.Match(next, ModelTokenType.Text);
				Debug.Assert(_returnTypeTextToken != null);
				_returnTypeTokens = new ModelTokenRange(next, _returnTypeTextToken.Next);
				next = Common.GetNext(_returnTypeTextToken);
			}
			if (next != null && next.Value.HasType(ModelTokenType.Extends))
			{
				_extendsTextToken = Common.Match(next, ModelTokenType.Text);
				Debug.Assert(_extendsTextToken != null);
				_extendsTokens = new ModelTokenRange(next.Previous, _extendsTextToken.Next);
				next = Common.GetNext(_extendsTextToken);
			}
			if (next != null && next.Value.HasType(ModelTokenType.Implements))
			{
				_implementsTextToken = Common.Match(next, ModelTokenType.Text);
				Debug.Assert(_implementsTextToken != null);
				_implementsTokens = new ModelTokenRange(next.Previous, _implementsTextToken.Next);
				next = Common.GetNext(_implementsTextToken);
			}
			for (next = _kindToken.Previous; next != null; next = next.Previous)
			{
				if (!next.Value.HasType(ModelTokenType.AnyBlank | ModelTokenType.Pragma))
				{
					if (next.Value.HasType(ModelTokenType.AnyComment))
					{
						_commentToken = next;
					}
					break;
				}
			}
			LList<LinkedListNode<ModelToken>> val = new LList<LinkedListNode<ModelToken>>();
			for (next = _kindToken.Previous; next != null; next = next.Previous)
			{
				if (!next.Value.HasType(ModelTokenType.AnyBlankOrComment))
				{
					if (!next.Value.HasType(ModelTokenType.Pragma))
					{
						break;
					}
					val.Add(next);
				}
			}
			_attributeTokens = val.ToArray();
			Array.Reverse(_attributeTokens);
		}

		internal string GetNormalizedText()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			LStringBuilder val = new LStringBuilder();
			val.Append(_kindToken.Value.Text);
			val.Append(" ");
			val.Append(_nameToken.Value.Text);
			if (_extendsTokens != null)
			{
				val.Append(" " + _extendsTokens.GetNormalizedText());
			}
			if (_implementsTokens != null)
			{
				val.Append(" " + _implementsTokens.GetNormalizedText());
			}
			if (_returnTypeTokens != null)
			{
				val.Append(" " + _returnTypeTokens.GetNormalizedText());
			}
			return ((object)val).ToString();
		}
	}
}
