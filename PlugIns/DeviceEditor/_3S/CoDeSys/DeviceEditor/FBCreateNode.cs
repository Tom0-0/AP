using System;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class FBCreateNode : ITreeTableNode2, ITreeTableNode
	{
		internal ISignature Signature { get; }

		internal IVariable Variable { get; }

		internal LibraryNode ParentNode { get; }

		public int ChildCount => 0;

		public bool HasChildren => false;

		public ITreeTableNode Parent => (ITreeTableNode)(object)ParentNode;

		public FBCreateNode(LibraryNode parent, ISignature sign, IVariable variable)
		{
			Signature = sign;
			Variable = variable;
			ParentNode = parent;
			ParentNode.AddFBCreateNode(this);
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return null;
		}

		public int GetIndex(ITreeTableNode node)
		{
			return 0;
		}

		public string GetToolTipText(int nColumnIndex)
		{
			return string.Empty;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			return (object)new IconLabelTreeTableViewCellData((Image)null, (object)((Signature != null) ? Signature.OrgName : string.Empty));
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

		public int CompareTo(FBCreateNode otherNode, int sortColumn)
		{
			if (otherNode == null)
			{
				throw new ArgumentNullException("otherNode");
			}
			return string.Compare(Signature.OrgName, otherNode.Signature.OrgName, ignoreCase: true);
		}
	}
}
