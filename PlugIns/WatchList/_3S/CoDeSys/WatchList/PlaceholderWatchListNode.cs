using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.WatchList
{
	internal class PlaceholderWatchListNode : ITreeTableNode2, ITreeTableNode
	{
		private WatchListNode _watchListNode;

		internal WatchListNode WatchListNode => _watchListNode;

		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => null;

		internal PlaceholderWatchListNode(WatchListNode watchListNode)
		{
			_watchListNode = watchListNode;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return null;
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public string GetToolTipText(int nColumnIndex)
		{
			return string.Empty;
		}

		public object GetValue(int nColumnIndex)
		{
			return string.Empty;
		}

		public bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public void SetValue(int nColumnIndex, object value)
		{
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
		}
	}
}
