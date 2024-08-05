using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class ParameterListComparer : IComparer, IComparer<IParameterTreeNode>
	{
		private int _sortColumn;

		private SortOrder _sortOrder;

		internal ParameterListComparer(int sortColumn, SortOrder sortOrder)
		{
			_sortColumn = sortColumn;
			_sortOrder = sortOrder;
		}

		public int Compare(object x, object y)
		{
			IParameterTreeNode x2 = (IParameterTreeNode)x;
			IParameterTreeNode y2 = (IParameterTreeNode)y;
			return Compare(x2, y2);
		}

		public int Compare(IParameterTreeNode x, IParameterTreeNode y)
		{
			int num = x.CompareTo(y, _sortColumn);
			if (_sortOrder == SortOrder.Descending)
			{
				num = -num;
			}
			return num;
		}
	}
}
