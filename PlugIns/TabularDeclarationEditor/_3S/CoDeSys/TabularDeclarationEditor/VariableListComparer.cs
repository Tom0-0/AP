using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class VariableListComparer : IComparer, IComparer<AbstractVariableListNode>
	{
		private VariableListColumns _sortColumn;

		private SortOrder _sortOrder;

		internal VariableListComparer(VariableListColumns sortColumn, SortOrder sortOrder)
		{
			_sortColumn = sortColumn;
			_sortOrder = sortOrder;
		}

		public int Compare(object x, object y)
		{
			AbstractVariableListNode x2 = (AbstractVariableListNode)x;
			AbstractVariableListNode y2 = (AbstractVariableListNode)y;
			return Compare(x2, y2);
		}

		public int Compare(AbstractVariableListNode x, AbstractVariableListNode y)
		{
			if (x is EmptyListNode)
			{
				if (!(y is EmptyListNode))
				{
					return 1;
				}
				return 0;
			}
			if (y is EmptyListNode)
			{
				if (!(x is EmptyListNode))
				{
					return -1;
				}
				return 0;
			}
			int num = ((VariableListNode)x).CompareTo((VariableListNode)y, _sortColumn);
			if (_sortOrder == SortOrder.Descending)
			{
				num = -num;
			}
			return num;
		}
	}
}
