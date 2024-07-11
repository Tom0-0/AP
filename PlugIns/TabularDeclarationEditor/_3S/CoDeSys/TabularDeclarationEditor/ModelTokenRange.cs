#define DEBUG
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class ModelTokenRange
	{
		private LinkedListNode<ModelToken> _first;

		private LinkedListNode<ModelToken> _next;

		internal LinkedListNode<ModelToken> First => _first;

		internal LinkedListNode<ModelToken> Next => _next;

		internal ModelTokenRange(LinkedListNode<ModelToken> first, LinkedListNode<ModelToken> next)
		{
			_first = first;
			_next = next;
		}

		internal string GetNormalizedText()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			LStringBuilder val = new LStringBuilder();
			LinkedListNode<ModelToken> linkedListNode = First;
			bool flag = false;
			while (linkedListNode != null && linkedListNode != Next)
			{
				if (linkedListNode.Value.HasType(ModelTokenType.AnyBlankOrComment))
				{
					if (!flag)
					{
						val.Append(" ");
						flag = true;
					}
				}
				else
				{
					val.Append(linkedListNode.Value.Text);
					flag = false;
				}
				linkedListNode = linkedListNode.Next;
			}
			return ((object)val).ToString().Trim();
		}

		internal string GetText()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			LStringBuilder val = new LStringBuilder();
			LinkedListNode<ModelToken> linkedListNode = First;
			while (linkedListNode != null && linkedListNode != Next)
			{
				val.Append(linkedListNode.Value.Text);
				linkedListNode = linkedListNode.Next;
			}
			return ((object)val).ToString();
		}

		internal void Remove()
		{
			if (First != null)
			{
				LinkedList<ModelToken> list = First.List;
				while (First != null && First != Next)
				{
					LinkedListNode<ModelToken> next = First.Next;
					list.Remove(First);
					_first = next;
				}
			}
		}

		internal void Replace(ModelToken token)
		{
			Debug.Assert(First != null);
			LinkedList<ModelToken> list = First.List;
			LinkedListNode<ModelToken> next = Next;
			Remove();
			if (next != null)
			{
				_first = list.AddBefore(next, token);
			}
			else
			{
				_first = list.AddLast(token);
			}
		}

		public override string ToString()
		{
			return GetText();
		}
	}
}
