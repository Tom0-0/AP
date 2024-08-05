using System.Collections;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FBInstanceComparer : IComparer
	{
		internal FBInstanceComparer()
		{
		}

		public int Compare(object x, object y)
		{
			FbInstanceTreeTableNode obj = (FbInstanceTreeTableNode)x;
			return string.Compare(strB: ((FbInstanceTreeTableNode)y).FbInstance.Instance.Variable, strA: obj.FbInstance.Instance.Variable);
		}
	}
}
