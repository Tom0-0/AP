using System;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.ProjectCompare;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class DeviceObjectDiffViewNode : ITreeTableNode
	{
		private int _nIndexInParent;

		private DeviceObjectDiffViewModel _model;

		private DeviceObjectDiffViewNode _parent;

		private LList<DeviceObjectDiffViewNode> _childNodes = new LList<DeviceObjectDiffViewNode>();

		private NodeDiffData _diffData;

		internal DeviceObjectDiffViewModel Model => _model;

		internal AcceptState AcceptState
		{
			get
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				return _diffData.AcceptState;
			}
			set
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Expected O, but got Unknown
				_diffData.AcceptState = value;
				((DefaultTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)_parent, _nIndexInParent, (ITreeTableNode)(object)this));
			}
		}

		internal DiffState DiffState => _diffData.DiffState;

		internal int IndexInParent => _nIndexInParent;

		public int ChildCount => _childNodes.Count;

		public bool HasChildren => _childNodes.Count > 0;

		public ITreeTableNode Parent => (ITreeTableNode)(object)_parent;

		internal DeviceObjectDiffViewNode(DeviceObjectDiffViewModel model, DeviceObjectDiffViewNode parent, NodeDiffData diffData, int nIndexInParent)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			_model = model;
			_parent = parent;
			_diffData = diffData;
			_nIndexInParent = nIndexInParent;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return (ITreeTableNode)(object)_childNodes[nIndex];
		}

		public int GetIndex(ITreeTableNode node)
		{
			if (node is DeviceObjectDiffViewNode)
			{
				return ((DeviceObjectDiffViewNode)(object)node)._nIndexInParent;
			}
			return -1;
		}

		public abstract object GetValue(int nColumnIndex);

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
			DeviceObjectDiffViewNode deviceObjectDiffViewNode = _childNodes[nIndex1];
			_childNodes[nIndex1]= _childNodes[nIndex2];
			_childNodes[nIndex2]= deviceObjectDiffViewNode;
			_childNodes[nIndex1]._nIndexInParent = nIndex1;
			_childNodes[nIndex2]._nIndexInParent = nIndex2;
		}

		internal void AddChildNode(DeviceObjectDiffViewNode childNode)
		{
			if (childNode == null)
			{
				throw new ArgumentNullException("childNode");
			}
			_childNodes.Add(childNode);
			childNode._nIndexInParent = _childNodes.Count - 1;
		}

		internal bool IsDirty()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)AcceptState != 0)
			{
				return true;
			}
			for (int i = 0; i < ChildCount; i++)
			{
				if (((DeviceObjectDiffViewNode)(object)GetChild(i)).IsDirty())
				{
					return true;
				}
			}
			return false;
		}

		internal int GetAcceptedCount(DiffState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			if (DiffState == state && (int)AcceptState != 0)
			{
				num++;
			}
			for (int i = 0; i < ChildCount; i++)
			{
				num += ((DeviceObjectDiffViewNode)(object)GetChild(i)).GetAcceptedCount(state);
			}
			return num;
		}

		internal bool IsEverythingAccepted()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if ((int)DiffState != 0 && (int)DiffState != 128 && (int)AcceptState == 0)
			{
				return false;
			}
			for (int i = 0; i < ChildCount; i++)
			{
				if (!((DeviceObjectDiffViewNode)(object)GetChild(i)).IsEverythingAccepted())
				{
					return false;
				}
			}
			return true;
		}
	}
}
