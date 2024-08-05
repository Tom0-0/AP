using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DescriptionNode : ITaskNode, ITreeTableNode2, ITreeTableNode
	{
		private string _stDescription;

		private Icon _icon;

		private TaskUpdateModel _model;

		private LList<ITreeTableNode> _children = new LList<ITreeTableNode>();

		private const int COLUMN_DESCRIPTION = 0;

		public LList<ITreeTableNode> Children => _children;

		public string ItemData => _stDescription;

		public bool HasChildren => ChildCount > 0;

		public int ChildCount => _children.Count;

		public ITreeTableNode Parent => null;

		public DescriptionNode(string stDescription, Icon icon, TaskUpdateModel model)
		{
			_stDescription = stDescription;
			_model = model;
			_icon = icon;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			if (nColumnIndex == 1)
			{
				return string.Empty;
			}
			Image image = null;
			if (_icon != null)
			{
				image = _icon.ToBitmap();
			}
			return (object)new IconLabelTreeTableViewCellData(image, (object)_stDescription);
		}

		public virtual void RemoveChild(TaskUpdateNode node)
		{
			_children.Remove((ITreeTableNode)(object)node);
		}

		public void SetValue(int nColumnIndex, object value)
		{
		}

		public int GetIndex(ITreeTableNode node)
		{
			return _children.IndexOf(node);
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			TaskUpdateNode taskUpdateNode = (TaskUpdateNode)(object)_children[nIndex1];
			_children[nIndex1]= _children[nIndex2];
			_children[nIndex2]= (ITreeTableNode)(object)taskUpdateNode;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			if (nIndex < _children.Count)
			{
				return _children[nIndex];
			}
			return null;
		}

		public bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public string GetToolTipText(int nColumnIndex)
		{
			return string.Empty;
		}
	}
}
