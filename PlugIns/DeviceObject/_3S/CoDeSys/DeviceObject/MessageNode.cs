using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceObject
{
	internal class MessageNode : ITreeTableNode
	{
		private DeviceObjectDiffViewNodeData _data;

		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => null;

		public MessageNode(DeviceObjectDiffViewNodeData data)
		{
			_data = data;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return null;
		}

		public int GetIndex(ITreeTableNode node)
		{
			return 0;
		}

		public object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == 0)
			{
				return _data;
			}
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
