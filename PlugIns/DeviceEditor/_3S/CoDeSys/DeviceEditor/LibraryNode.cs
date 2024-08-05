using System;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class LibraryNode : ITreeTableNode2, ITreeTableNode
	{
		private LList<FBCreateNode> Children = new LList<FBCreateNode>();

		private FbCreateTreeTableModel _model;

		internal string NodeName { get; }

		internal ISignature Signature { get; }

		public int ChildCount => Children.Count;

		public bool HasChildren => Children.Count > 0;

		public ITreeTableNode Parent => null;

		public LibraryNode(FbCreateTreeTableModel model, ISignature sign, string stNodeName)
		{
			_model = model;
			Signature = sign;
			NodeName = stNodeName;
		}

		public void AddFBCreateNode(FBCreateNode childNode)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			if (childNode == null)
			{
				throw new ArgumentNullException("childNode");
			}
			int count = Children.Count;
			Children.Add(childNode);
			((AbstractTreeTableModel)_model).RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)(object)this, count, (ITreeTableNode)(object)childNode));
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			if (nIndex >= 0 && nIndex < Children.Count)
			{
				return (ITreeTableNode)(object)Children[nIndex];
			}
			return null;
		}

		public int GetIndex(ITreeTableNode node)
		{
			return Children.IndexOf(node as FBCreateNode);
		}

		public string GetToolTipText(int nColumnIndex)
		{
			return string.Empty;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return (object)new IconLabelTreeTableViewCellData((Image)null, (object)NodeName);
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
			FBCreateNode fBCreateNode = Children[nIndex1];
			Children[nIndex1]= Children[nIndex2];
			Children[nIndex2]= fBCreateNode;
		}

		public int CompareTo(LibraryNode otherNode, int sortColumn)
		{
			if (otherNode == null)
			{
				throw new ArgumentNullException("otherNode");
			}
			return string.Compare(NodeName, otherNode.NodeName, ignoreCase: true);
		}
	}
}
