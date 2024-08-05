using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class TaskUpdateModel : AbstractTreeTableModel
	{
		private TreeTableView _view;

		private LList<TaskInfo> _liTaskInfos;

		private LDictionary<int, TaskInfo> _dictColumns = new LDictionary<int, TaskInfo>();

		public TreeTableView View => _view;

		public LList<TaskInfo> TaskInfos => _liTaskInfos;

		public TaskUpdateModel(TreeTableView view, LList<TaskInfo> liTaskInfos)
			: base()
		{
			_view = view;
			_liTaskInfos = liTaskInfos;
			UnderlyingModel.AddColumn(Strings.TaskUpdateIOChannelsColumn, HorizontalAlignment.Left, MyIconLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Strings.ColumnNameChannel, HorizontalAlignment.Left, MyLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			foreach (TaskInfo liTaskInfo in liTaskInfos)
			{
				int num = UnderlyingModel.AddColumn(liTaskInfo.taskInfo.TaskName + " (" + liTaskInfo.iPriority + ")", HorizontalAlignment.Left, MyCheckBoxTreeTableViewRenderer.CheckBox, TextBoxTreeTableViewEditor.TextBox, false);
				_dictColumns.Add(num, liTaskInfo);
			}
		}

		public void RaiseChanged(TaskUpdateNode node)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			int num = ((node.Parent != null) ? node.Parent.GetIndex((ITreeTableNode)(object)node) : UnderlyingModel.Sentinel.GetIndex((ITreeTableNode)(object)node));
			((AbstractTreeTableModel)this).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)node, num, (ITreeTableNode)(object)node));
		}

		public void RemoveNode(TaskUpdateNode node)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			try
			{
				int num = ((node.Parent != null) ? node.Parent.GetIndex((ITreeTableNode)(object)node) : UnderlyingModel.Sentinel.GetIndex((ITreeTableNode)(object)node));
				if (node.Parent != null)
				{
					TaskUpdateNode taskUpdateNode = (TaskUpdateNode)(object)node.Parent;
					taskUpdateNode.RemoveChild(node);
					((AbstractTreeTableModel)this).RaiseRemoved(new TreeTableModelEventArgs((ITreeTableNode)(object)taskUpdateNode, num, (ITreeTableNode)(object)node));
				}
				else
				{
					UnderlyingModel.RemoveRootNode(num);
				}
			}
			catch
			{
			}
		}

		public void Invalidate()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			if (UnderlyingModel.Sentinel.ChildCount > 0)
			{
				ITreeTableNode child = UnderlyingModel.Sentinel.GetChild(0);
				int num = ((child.Parent != null) ? child.Parent.GetIndex(child) : UnderlyingModel.Sentinel.GetIndex(child));
				((AbstractTreeTableModel)this).RaiseStructureChanged(new TreeTableModelEventArgs((ITreeTableNode)null, num, child));
			}
		}

		public void Clear()
		{
			UnderlyingModel.ClearRootNodes();
		}

		public DescriptionNode AddRootNode(string stDescription, Icon icon)
		{
			DescriptionNode descriptionNode = null;
			try
			{
				_view.BeginUpdate();
				descriptionNode = new DescriptionNode(stDescription, icon, this);
				UnderlyingModel.AddRootNode((ITreeTableNode)(object)descriptionNode);
				return descriptionNode;
			}
			finally
			{
				_view.EndUpdate();
			}
		}

		public TaskUpdateNode AddNode(Guid objectGuid, Guid busTaskGuid, LList<Guid> liTaskGuids, IDataElement dataElement, ChannelType channelType, ITaskNode parent)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			TaskUpdateNode taskUpdateNode = null;
			try
			{
				_view.BeginUpdate();
				taskUpdateNode = new TaskUpdateNode(objectGuid, busTaskGuid, liTaskGuids, dataElement, channelType, this, parent);
				((AbstractTreeTableModel)this).RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)parent, ((ITreeTableNode)parent).GetIndex((ITreeTableNode)(object)taskUpdateNode), (ITreeTableNode)(object)taskUpdateNode));
				return taskUpdateNode;
			}
			finally
			{
				_view.EndUpdate();
			}
		}

		public TaskInfo GetColumnTaskInfo(int nColumn)
		{
			TaskInfo result = null;
			_dictColumns.TryGetValue(nColumn, out result);
			return result;
		}
	}
}
