using System;
using System.Collections.Generic;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class SectionNode : ParameterNode, IParameterTreeNode, ITreeTableNode2, ITreeTableNode
	{
		private IDeviceNode _deviceNode;

		private IParameterTreeNode _parent;

		private IParameterSection _section;

		private List<IParameterTreeNode> _childNodes = new List<IParameterTreeNode>();

		private string _stDevPath;

		private static readonly Image s_image = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.SectionSmall.ico").Handle);

		public string DevPath
		{
			get
			{
				if (string.IsNullOrEmpty(_stDevPath))
				{
					if (_parent != null)
					{
						_stDevPath = _parent.DevPath + _section.Name;
					}
					else
					{
						_stDevPath = _section.Name;
					}
				}
				return _stDevPath;
			}
		}

		public IParameterSection Section
		{
			get
			{
				return _section;
			}
			set
			{
				_section = value;
			}
		}

		public bool HasChildren => _childNodes.Count > 0;

		public int ChildCount => _childNodes.Count;

		public List<IParameterTreeNode> ChildNodes => _childNodes;

		public ITreeTableNode Parent => (ITreeTableNode)_parent;

		public SectionNode(IParameterTreeTable model, IDeviceNode deviceNode, IParameterTreeNode parent, IParameterSection section)
		{
			if (deviceNode == null)
			{
				throw new ArgumentNullException("model");
			}
			if (section == null)
			{
				throw new ArgumentNullException("section");
			}
			_model = model;
			_node = (ITreeTableNode2)(object)this;
			_deviceNode = deviceNode;
			_parent = parent;
			_section = section;
		}

		public void AddSectionNode(SectionNode childNode)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			if (childNode == null)
			{
				throw new ArgumentNullException("childNode");
			}
			int count = _childNodes.Count;
			_childNodes.Add(childNode);
			_deviceNode.TreeModel.RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)(object)this, count, (ITreeTableNode)(object)childNode));
		}

		public void AddDataElementNode(DataElementNode childNode)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			if (childNode == null)
			{
				throw new ArgumentNullException("childNode");
			}
			int count = _childNodes.Count;
			_childNodes.Add(childNode);
			_deviceNode.TreeModel.RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)(object)this, count, (ITreeTableNode)(object)childNode));
		}

		public void InsertDataElementNode(int nIndex, DataElementNode childNode)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			if (childNode == null)
			{
				throw new ArgumentNullException("childNode");
			}
			_childNodes.Insert(nIndex, childNode);
			_deviceNode.TreeModel.RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)(object)this, nIndex, (ITreeTableNode)(object)childNode));
		}

		public void RemoveNodeAt(int nIndex)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			ITreeTableNode child = GetChild(nIndex);
			_childNodes.RemoveAt(nIndex);
			_deviceNode.TreeModel.RaiseRemoved(new TreeTableModelEventArgs((ITreeTableNode)(object)this, nIndex, child));
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return (ITreeTableNode)_childNodes[nIndex];
		}

		public int GetIndex(ITreeTableNode node)
		{
			return _childNodes.IndexOf(node as IParameterTreeNode);
		}

		public bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			if (nColumnIndex == 0)
			{
				return (object)new IconLabelTreeTableViewCellData(s_image, (object)_section.Name);
			}
			if (_deviceNode.TreeModel.MapColumn(nColumnIndex) == 10)
			{
				return _section.Description;
			}
			return string.Empty;
		}

		public void SetValue(int nColumnIndex, object value)
		{
			throw new InvalidOperationException("This node is read-only.");
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			IParameterTreeNode value = _childNodes[nIndex1];
			_childNodes[nIndex1] = _childNodes[nIndex2];
			_childNodes[nIndex2] = value;
		}

		public DataElementNode Get(IParameter parameter, IDataElement dataelement, string[] path)
		{
			DataElementNode dataElementNode = null;
			foreach (IParameterTreeNode childNode in _childNodes)
			{
				dataElementNode = childNode.Get(parameter, dataelement, path);
				if (dataElementNode != null)
				{
					return dataElementNode;
				}
			}
			return dataElementNode;
		}

		public string GetToolTipText(int nColumnIndex)
		{
			return string.Empty;
		}

		public int CompareTo(IParameterTreeNode otherNode, int sortColumn)
		{
			if (otherNode == null)
			{
				throw new ArgumentNullException("otherNode");
			}
			if (otherNode is SectionNode)
			{
				return string.Compare(_section.Name, (otherNode as SectionNode).Section.Name, ignoreCase: true);
			}
			if (otherNode is DataElementNode)
			{
				return string.Compare(_section.Name, (otherNode as DataElementNode).DataElement.VisibleName, ignoreCase: true);
			}
			return 0;
		}
	}
}
