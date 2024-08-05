using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FBCreateComparer : IComparer, IComparer<FBCreateNode>
	{
		private readonly int _sortColumn;

		private readonly SortOrder _sortOrder;

		internal FBCreateComparer(int sortColumn, SortOrder sortOrder)
		{
			_sortColumn = sortColumn;
			_sortOrder = sortOrder;
		}

		public int Compare(object x, object y)
		{
			if (x is LibraryNode && y is LibraryNode)
			{
				LibraryNode x2 = (LibraryNode)x;
				LibraryNode y2 = (LibraryNode)y;
				return Compare(x2, y2);
			}
			if (x is FBCreateNode && y is FBCreateNode)
			{
				FBCreateNode x3 = (FBCreateNode)x;
				FBCreateNode y3 = (FBCreateNode)y;
				return Compare(x3, y3);
			}
			return 0;
		}

		public int Compare(FBCreateNode x, FBCreateNode y)
		{
			int num = x.CompareTo(y, _sortColumn);
			if (_sortOrder == SortOrder.Descending)
			{
				num = -num;
			}
			return num;
		}

		public int Compare(LibraryNode x, LibraryNode y)
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
