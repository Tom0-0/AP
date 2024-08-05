using System;
using System.Collections.Generic;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	internal class DeviceNode : IParameterTreeNode, ITreeTableNode2, ITreeTableNode, IDeviceNode
	{
		private SimpleMappingTreeTableModel _model;

		private ISVNode _node;

		private DeviceNode _parent;

		private List<IParameterTreeNode> _childNodes = new List<IParameterTreeNode>();

		private string _stDevPath;

		private LDictionary<IParameterSection, SectionNode> _sectionNodes = new LDictionary<IParameterSection, SectionNode>();

		public string DevPath
		{
			get
			{
				if (string.IsNullOrEmpty(_stDevPath))
				{
					_stDevPath = ((IObjectManager)APEnvironment.ObjectMgr).GetDottedFullName(_node.ProjectHandle, _node.ObjectGuid);
				}
				return _stDevPath;
			}
		}

		public ISVNode ISVNode => _node;

		public bool HasChildren => _childNodes.Count > 0;

		public int ChildCount => _childNodes.Count;

		public List<IParameterTreeNode> ChildNodes => _childNodes;

		public ITreeTableNode Parent => (ITreeTableNode)(object)_parent;

		public IParameterTreeTable TreeModel => _model;

		public DeviceNode(SimpleMappingTreeTableModel model, ISVNode node, DeviceNode parent)
		{
			_model = model;
			_node = node;
			_parent = parent;
		}

		public IDeviceObject GetHost()
		{
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_node.ProjectHandle, _node.ObjectGuid))
			{
				return null;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_node.ProjectHandle, _node.ObjectGuid);
			while (metaObjectStub.ParentObjectGuid != Guid.Empty)
			{
				metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_node.ProjectHandle, metaObjectStub.ParentObjectGuid);
			}
			IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_node.ProjectHandle, metaObjectStub.ObjectGuid).Object;
			return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
		}

		public SectionNode GetOrCreateSectionNode(IParameterSection section, IDeviceNode device, IParameterTreeNode parent)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			if (section == null)
			{
				return null;
			}
			if (!_sectionNodes.ContainsKey(section))
			{
				SectionNode sectionNode = null;
				if (section.Section != null)
				{
					sectionNode = GetOrCreateSectionNode(section.Section, device, parent);
				}
				SectionNode sectionNode2 = ((sectionNode == null) ? new SectionNode(_model, device, device as IParameterTreeNode, section) : new SectionNode(_model, device, sectionNode, section));
				_model.DevPath2Node[sectionNode2.DevPath]= (IParameterTreeNode)sectionNode2;
				if (sectionNode != null)
				{
					sectionNode.AddSectionNode(sectionNode2);
				}
				else
				{
					TreeModel.RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)parent, ((ITreeTableNode)parent).ChildCount, (ITreeTableNode)(object)sectionNode2));
					if (parent is DeviceNode)
					{
						(parent as DeviceNode).ChildNodes.Add(sectionNode2);
					}
				}
				_sectionNodes[section]= sectionNode2;
			}
			return _sectionNodes[section];
		}

		public void RemoveNodeAt(int nIndex)
		{
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
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			if (nColumnIndex == 0)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_node.ProjectHandle, _node.ObjectGuid);
				string text = Helper.GetObjectName(metaObjectStub);
				if (APEnvironment.LocalizationManagerOrNull != null && _model.Localization)
				{
					text = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text, (LocalizationContent)1);
				}
				return (object)new IconLabelTreeTableViewCellData(Helper.GetImage(metaObjectStub), (object)text);
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

		public string GetToolTipText(int nColumnIndex)
		{
			return string.Empty;
		}

		public DataElementNode Get(IParameter parameter, IDataElement dataelement, string[] path)
		{
			return null;
		}

		public int CompareTo(IParameterTreeNode otherNode, int sortColumn)
		{
			return 0;
		}
	}
}
