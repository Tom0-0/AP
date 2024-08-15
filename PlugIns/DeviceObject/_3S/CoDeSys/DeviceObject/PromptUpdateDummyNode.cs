using System;
using System.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class PromptUpdateDummyNode : ITreeTableNode
	{
		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => null;

		public ITreeTableNode GetChild(int nIndex)
		{
			throw new ArgumentOutOfRangeException("nIndex");
		}

		public int GetIndex(ITreeTableNode node)
		{
			return -1;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			return nColumnIndex switch
			{
				0 => Strings.AllDevicesAreUpToDate, 
				1 => string.Empty, 
				2 => string.Empty, 
				3 => false, 
				4 => (object)new FixedChoiceCellTreeTableViewCellData(-1, (ICollection)new LList<string>()), 
				_ => throw new ArgumentOutOfRangeException("nColumnIndex"), 
			};
		}

		public bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public void SetValue(int nColumnIndex, object value)
		{
			throw new InvalidOperationException("This node is read-only.");
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			throw new ArgumentOutOfRangeException("nIndex1");
		}
	}
}
