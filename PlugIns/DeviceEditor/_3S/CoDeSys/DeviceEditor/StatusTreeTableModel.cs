using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class StatusTreeTableModel : AbstractTreeTableModel
	{
		private TreeTableView _treeTable;

		public TreeTableView TreeTable => _treeTable;

		internal void AdjustColumnWidth()
		{
			if (_treeTable != null)
			{
				for (int i = 0; i < _treeTable.Columns.Count; i++)
				{
					_treeTable.AdjustColumnWidth(i, true);
				}
			}
		}

		public StatusTreeTableModel(TreeTableView treeTable)
			: base()
		{
			_treeTable = treeTable;
			UnderlyingModel.AddColumn(Strings.ColumnNameParameter, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
		}

		private void ShowHideNodes(StatusNode node, bool bShow)
		{
			if (node == null)
			{
				return;
			}
			if (node.HasChildren)
			{
				for (int i = 0; i < node.ChildCount; i++)
				{
					StatusNode statusNode = node.GetChild(i) as StatusNode;
					if (statusNode != null)
					{
						ShowHideNodes(statusNode, bShow);
					}
				}
			}
			if (node.WatchedVariable != null)
			{
				if (bShow)
				{
					((IOnlineVarRef)node.WatchedVariable).ResumeMonitoring();
				}
				else
				{
					((IOnlineVarRef)node.WatchedVariable).SuspendMonitoring();
				}
			}
		}

		public void ShowValueColumn()
		{
			if (UnderlyingModel.ColumnCount == 1)
			{
				UnderlyingModel.AddColumn(Strings.ColumnNameCurrentValue, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithoutForceIndicator, TextBoxTreeTableViewEditor.TextBox, false);
				UnderlyingModel.AddColumn(Strings.ColumnNameDescription, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
				for (int i = 0; i < UnderlyingModel.Sentinel.ChildCount; i++)
				{
					StatusNode node = UnderlyingModel.Sentinel.GetChild(i) as StatusNode;
					ShowHideNodes(node, bShow: true);
				}
			}
		}

		public void HideValueColumn()
		{
			if (UnderlyingModel.ColumnCount > 1)
			{
				UnderlyingModel.RemoveColumn(2);
				UnderlyingModel.RemoveColumn(1);
				for (int i = 0; i < UnderlyingModel.Sentinel.ChildCount; i++)
				{
					StatusNode node = UnderlyingModel.Sentinel.GetChild(i) as StatusNode;
					ShowHideNodes(node, bShow: false);
				}
			}
		}

		public void Reload(IParameter param, IConverterToIEC converter)
		{
			UnderlyingModel.ClearRootNodes();
			if (param != null)
			{
				UnderlyingModel.AddRootNode((ITreeTableNode)(object)new StatusNode((IDataElement)(object)param, null, (ITreeTableModel)(object)this, converter));
			}
		}

		public void SetConverterToIec(IConverterToIEC converter)
		{
			for (int i = 0; i < UnderlyingModel.Sentinel.ChildCount; i++)
			{
				(UnderlyingModel.Sentinel.GetChild(i) as StatusNode)?.SetConverterToIec(converter);
			}
		}

		public void ReleaseMonitoring()
		{
			for (int i = 0; i < UnderlyingModel.Sentinel.ChildCount; i++)
			{
				(UnderlyingModel.Sentinel.GetChild(i) as StatusNode)?.ReleaseMonitoring();
			}
		}
	}
}
