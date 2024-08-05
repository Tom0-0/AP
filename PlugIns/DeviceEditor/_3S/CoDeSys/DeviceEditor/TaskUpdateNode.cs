using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class TaskUpdateNode : ITaskNode, ITreeTableNode2, ITreeTableNode
	{
		private Guid _busTaskGuid;

		private LList<Guid> _liTaskGuids = new LList<Guid>();

		private IDataElement _dataElement;

		private ChannelType _channelType;

		private Guid _objectGuid;

		private TaskUpdateModel _model;

		private LList<ITreeTableNode> _children = new LList<ITreeTableNode>();

		private ITaskNode _root;

		private ITaskNode _parent;

		private static readonly Image s_paramImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.ParameterSmall.ico").Handle);

		private static readonly Image s_structuredParamImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.StructuredParameterSmall.ico").Handle);

		private static readonly Image s_inputChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.InputChannelSmall.ico").Handle);

		private static readonly Image s_outputChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.OutputChannelSmall.ico").Handle);

		private static readonly Image s_inputChannelSafetyImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.SafetyInputChannelSmall.ico").Handle);

		private static readonly Image s_outputChannelSafetyImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.SafetyOutputChannelSmall.ico").Handle);

		private static readonly Image s_manualAddressImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.ManualAddress.ico").Handle);

		private const int COLUMN_IO = 0;

		public LList<ITreeTableNode> Children => _children;

		public TaskUpdateModel Model => _model;

		public Guid BusTaskGuid => _busTaskGuid;

		public LList<Guid> TaskGuids => _liTaskGuids;

		public IDataElement DataElement => _dataElement;

		public Guid ObjectGuid => _objectGuid;

		public bool HasChildren => ChildCount > 0;

		public int ChildCount => _children.Count;

		public ITreeTableNode Parent => (ITreeTableNode)_parent;

		public TaskUpdateNode(Guid objectGuid, Guid busTaskGuid, LList<Guid> liTaskGuids, IDataElement dataElement, ChannelType channelType, TaskUpdateModel model, ITaskNode parent)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (parent == null)
			{
				_root = this;
			}
			else
			{
				parent.Children.Add((ITreeTableNode)(object)this);
			}
			_busTaskGuid = busTaskGuid;
			_liTaskGuids.AddRange((IEnumerable<Guid>)liTaskGuids);
			_dataElement = dataElement;
			_channelType = channelType;
			_model = model;
			_parent = parent;
			_objectGuid = objectGuid;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Expected I4, but got Unknown
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			switch (nColumnIndex)
			{
			case 0:
			{
				bool flag = false;
				string empty = string.Empty;
				if (((ICollection)_dataElement.IoMapping.VariableMappings).Count > 0)
				{
					IVariableMapping val = _dataElement.IoMapping.VariableMappings[0];
					empty = ((!val.CreateVariable) ? val.Variable : (val.Variable + " AT " + _dataElement.IoMapping.IecAddress));
				}
				else
				{
					empty = _dataElement.IoMapping.IecAddress;
				}
				IDataElement dataElement = _dataElement;
				if (((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).HasBaseType)
				{
					try
					{
						flag = _dataElement.BaseType.ToLowerInvariant().StartsWith("safe");
					}
					catch
					{
					}
				}
				ChannelType channelType = _channelType;
				Image image;
				switch ((int)channelType - 1)
				{
				case 0:
					image = ((!flag) ? s_inputChannelImage : s_inputChannelSafetyImage);
					break;
				case 1:
				case 2:
					image = ((!flag) ? s_outputChannelImage : s_outputChannelSafetyImage);
					break;
				default:
					image = s_paramImage;
					break;
				}
				return (object)new IconLabelTreeTableViewCellData(image, (object)empty);
			}
			case 1:
				return _dataElement.VisibleName;
			default:
			{
				TaskInfo columnTaskInfo = _model.GetColumnTaskInfo(nColumnIndex);
				if (columnTaskInfo != null && _liTaskGuids.Contains(columnTaskInfo.taskInfo.TaskGuid))
				{
					return true;
				}
				return false;
			}
			}
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
			if (_liTaskGuids.Count == 0)
			{
				return Strings.TaskUpdateNoTask;
			}
			return string.Format(Strings.TaskUpdateNbrTask, _liTaskGuids.Count);
		}
	}
}
