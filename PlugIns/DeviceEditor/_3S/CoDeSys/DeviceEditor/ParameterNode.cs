using System.Collections.Generic;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal abstract class ParameterNode : IGenericConfigurationNode
	{
		internal IParameterTreeTable _model;

		internal ITreeTableNode2 _node;

		public bool IsEditing
		{
			get
			{
				if (_node != null && _model?.View != null)
				{
					TreeTableViewNode viewNode = _model.View.GetViewNode((ITreeTableNode)(object)_node);
					if (viewNode != null)
					{
						int indexOfColumn = _model.GetIndexOfColumn(7);
						int indexOfColumn2 = _model.GetIndexOfColumn(5);
						return viewNode.IsEditing(indexOfColumn) | viewNode.IsEditing(indexOfColumn2);
					}
				}
				return false;
			}
		}

		public bool IsExpanded
		{
			get
			{
				if (_node != null && _model?.View != null)
				{
					TreeTableViewNode viewNode = _model.View.GetViewNode((ITreeTableNode)(object)_node);
					if (viewNode != null)
					{
						return ((ITreeTableNode)_node).HasChildren & viewNode.Expanded;
					}
				}
				return false;
			}
		}

		public IList<IGenericConfigurationNode> Nodes
		{
			get
			{
				LList<IGenericConfigurationNode> val = new LList<IGenericConfigurationNode>();
				if (_node != null && ((ITreeTableNode)_node).HasChildren)
				{
					for (int i = 0; i < ((ITreeTableNode)_node).ChildCount; i++)
					{
						ITreeTableNode child = ((ITreeTableNode)_node).GetChild(i);
						IGenericConfigurationNode val2 = (IGenericConfigurationNode)(object)((child is IGenericConfigurationNode) ? child : null);
						if (val2 != null)
						{
							val.Add(val2);
						}
					}
				}
				return (IList<IGenericConfigurationNode>)val;
			}
		}

		public IOnlineVarRef OnlineVarRef
		{
			get
			{
				if (_node is DataElementNode)
				{
					return (_node as DataElementNode).OnlineVarRef;
				}
				return null;
			}
		}

		public string PreparedValue
		{
			get
			{
				if (_node != null && _model != null)
				{
					int indexOfColumn = _model.GetIndexOfColumn(7);
					if (indexOfColumn >= 0)
					{
						return ((ITreeTableNode)_node).GetValue(indexOfColumn).ToString();
					}
				}
				return string.Empty;
			}
			set
			{
				if (_node != null && _model != null)
				{
					int indexOfColumn = _model.GetIndexOfColumn(7);
					if (indexOfColumn >= 0)
					{
						((ITreeTableNode)_node).SetValue(indexOfColumn, (object)value);
					}
				}
			}
		}

		public string Value
		{
			get
			{
				if (_node != null && _model != null)
				{
					int indexOfColumn = _model.GetIndexOfColumn(5);
					if (indexOfColumn >= 0)
					{
						return ((ITreeTableNode)_node).GetValue(indexOfColumn).ToString();
					}
				}
				return string.Empty;
			}
			set
			{
				if (_node != null && _model != null)
				{
					int indexOfColumn = _model.GetIndexOfColumn(5);
					if (indexOfColumn >= 0)
					{
						((ITreeTableNode)_node).SetValue(indexOfColumn, (object)value);
					}
				}
			}
		}

		public IDataElement DataElement
		{
			get
			{
				if (_node is DataElementNode)
				{
					return (_node as DataElementNode).DataElement;
				}
				return null;
			}
		}

		public IParameter Parameter
		{
			get
			{
				if (_node is DataElementNode)
				{
					return (_node as DataElementNode).Parameter;
				}
				return null;
			}
		}

		public void Collapse()
		{
			if (_node == null || _model?.View == null)
			{
				return;
			}
			TreeTableViewNode viewNode = _model.View.GetViewNode((ITreeTableNode)(object)_node);
			if (viewNode != null)
			{
				try
				{
					_model.View.BeginUpdate();
					viewNode.Collapse();
				}
				finally
				{
					_model.View.EndUpdate();
				}
			}
		}

		public void CollapseAll()
		{
			if (_node == null || _model?.View == null)
			{
				return;
			}
			TreeTableViewNode viewNode = _model.View.GetViewNode((ITreeTableNode)(object)_node);
			if (viewNode != null)
			{
				try
				{
					_model.View.BeginUpdate();
					viewNode.CollapseAll();
				}
				finally
				{
					_model.View.EndUpdate();
				}
			}
		}

		public void Expand()
		{
			if (_node == null || _model?.View == null)
			{
				return;
			}
			TreeTableViewNode viewNode = _model.View.GetViewNode((ITreeTableNode)(object)_node);
			if (viewNode != null)
			{
				try
				{
					_model.View.BeginUpdate();
					viewNode.Expand();
				}
				finally
				{
					_model.View.EndUpdate();
				}
			}
		}

		public void ExpandAll()
		{
			if (_node == null || _model?.View == null)
			{
				return;
			}
			TreeTableViewNode viewNode = _model.View.GetViewNode((ITreeTableNode)(object)_node);
			if (viewNode != null)
			{
				try
				{
					_model.View.BeginUpdate();
					viewNode.ExpandAll();
				}
				finally
				{
					_model.View.EndUpdate();
				}
			}
		}
	}
}
