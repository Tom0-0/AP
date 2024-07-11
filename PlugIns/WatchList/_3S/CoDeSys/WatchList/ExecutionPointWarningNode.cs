using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.WatchList
{
	public class ExecutionPointWarningNode : ITreeTableNode2, ITreeTableNode
	{
		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => null;

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
			return Strings.ExecutionPointWarningNode_ToolTipText;
		}

		public object GetValue(int nColumnIndex)
		{
			if (nColumnIndex == 0)
			{
				string executionPointWarningNode_Text = Strings.ExecutionPointWarningNode_Text;
				return new ExpressionData(executionPointWarningNode_Text, executionPointWarningNode_Text, null);
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
