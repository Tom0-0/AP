using System;
using System.Collections;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.WatchList
{
	internal class WatchListComparer : IComparer
	{
		private int _sortColumn;

		private SortOrder _sortOrder;

		private TreeTableView _view;

		internal WatchListComparer(int sortColumn, SortOrder sortOrder, TreeTableView view)
		{
			if (view == null)
			{
				throw new ArgumentNullException("view");
			}
			_sortColumn = sortColumn;
			_sortOrder = sortOrder;
			_view = view;
		}

		public int Compare(object x, object y)
		{
			WatchListNode watchListNode = x as WatchListNode;
			WatchListNode watchListNode2 = y as WatchListNode;
			if (watchListNode != null && watchListNode2 != null)
			{
				string displayString = GetDisplayString((ITreeTableNode)(object)watchListNode);
				string displayString2 = GetDisplayString((ITreeTableNode)(object)watchListNode2);
				int num = string.Compare(displayString, displayString2, ignoreCase: true);
				if (_sortOrder == SortOrder.Descending)
				{
					num = -num;
				}
				return num;
			}
			return 0;
		}

		private string GetDisplayString(ITreeTableNode node)
		{
			ITreeTableViewRenderer columnRenderer = _view.GetColumnRenderer(_sortColumn);
			TreeTableViewNode viewNode = _view.GetViewNode(node);
			return columnRenderer.GetStringRepresentation(viewNode, _sortColumn);
		}
	}
}
