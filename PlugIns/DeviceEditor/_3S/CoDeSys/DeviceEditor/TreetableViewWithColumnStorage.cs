using System;
using System.Collections;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class TreetableViewWithColumnStorage : TreeTableView
	{
		private Guid _objectGuid;

		private Guid _identificationGuid;

		private bool _bAdjustColumnWidth;

		internal static DeviceEditorColumnStorage _columnstorage = new DeviceEditorColumnStorage();

		internal static LList<TreeTableViewStorageData> _liExpandedNodes = new LList<TreeTableViewStorageData>();

		private bool _bIsInRestore;

		public Guid ObjectGuid
		{
			get
			{
				return _objectGuid;
			}
			set
			{
				_objectGuid = value;
			}
		}

		public Guid IdentificationGuid
		{
			get
			{
				return _identificationGuid;
			}
			set
			{
				_identificationGuid = value;
			}
		}

		internal bool IsInRestore
		{
			get
			{
				return _bIsInRestore;
			}
			set
			{
				_bIsInRestore = value;
			}
		}

		public TreetableViewWithColumnStorage()
			: base()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			((TreeTableView)this).ColumnWidthChanging+=((ColumnWidthChangingEventHandler)Treetable_ColumnWidthChanging);
			((TreeTableView)this).AfterCollapse+=(new TreeTableViewEventHandler(CollapseExpand));
			((TreeTableView)this).AfterExpand+=(new TreeTableViewEventHandler(CollapseExpand));
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			SaveTreetableColumnsWidth();
			OnHandleDestroyed(e);
		}

		private void Treetable_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
		{
			if (!_bAdjustColumnWidth)
			{
				SaveTreetableColumnsWidth(e.ColumnIndex, e.NewWidth, bCreate: false);
			}
		}

		private void SaveTreetableColumnsWidth(int nColumnIndex, bool bCreate)
		{
			SaveTreetableColumnsWidth(nColumnIndex, ((TreeTableView)this).Columns[nColumnIndex].Width, bCreate);
		}

		private void SaveTreetableColumnsWidth(int nColumnIndex, int nWidth, bool bCreate)
		{
			TreeTableViewStorageData treeTableViewStorageData = FindStorageData(_objectGuid, _identificationGuid);
			if (treeTableViewStorageData == null && bCreate && !((TreeTableView)this).KeepColumnWidthsAdjusted && _objectGuid != Guid.Empty && _identificationGuid != Guid.Empty)
			{
				treeTableViewStorageData = new TreeTableViewStorageData(_objectGuid, _identificationGuid);
				_columnstorage.AddStorageData(treeTableViewStorageData);
			}
			if (treeTableViewStorageData != null)
			{
				string text = ((TreeTableView)this).Columns[nColumnIndex].Text;
				treeTableViewStorageData.DictData[text] = nWidth;
			}
			if (!_bAdjustColumnWidth)
			{
				OptionsHelperColumnStorage.StorageData = _columnstorage;
			}
		}

		internal void SaveTreetableColumnsWidth()
		{
			for (int i = 0; i < ((TreeTableView)this).Columns.Count; i++)
			{
				SaveTreetableColumnsWidth(i, bCreate: false);
			}
		}

		public void AdjustColumnWidth(int nColumnIndex, bool bConsiderHeaderText)
		{
			AdjustColumnWidth(nColumnIndex, bConsiderHeaderText, bSave: true, bForceAdjust: false);
		}

		public void AdjustColumnWidth(int nColumnIndex, bool bConsiderHeaderText, bool bSave, bool bForceAdjust)
		{
			_bAdjustColumnWidth = true;
			try
			{
				if (bForceAdjust || !RestoreTreetableColumnWidth(nColumnIndex))
				{
					((TreeTableView)this).AdjustColumnWidth(nColumnIndex, bConsiderHeaderText);
					if (bSave)
					{
						SaveTreetableColumnsWidth(nColumnIndex, bCreate: true);
					}
				}
			}
			finally
			{
				_bAdjustColumnWidth = false;
			}
		}

		public void SetColumnWidth(int nColumnIndex, int nWidth, bool bForceAdjust)
		{
			_bAdjustColumnWidth = true;
			try
			{
				if (((TreeTableView)this).Columns.Count > nColumnIndex && (bForceAdjust || !RestoreTreetableColumnWidth(nColumnIndex)))
				{
					((TreeTableView)this).Columns[nColumnIndex].Width = nWidth;
					SaveTreetableColumnsWidth(nColumnIndex, bCreate: true);
				}
			}
			finally
			{
				_bAdjustColumnWidth = false;
			}
		}

		internal bool RestoreTreetableColumnWidth(int iColumn)
		{
			TreeTableViewStorageData treeTableViewStorageData = FindStorageData(_objectGuid, _identificationGuid);
			if (treeTableViewStorageData != null)
			{
				string text = ((TreeTableView)this).Columns[iColumn].Text;
				if (treeTableViewStorageData.DictData.ContainsKey(text))
				{
					((TreeTableView)this).Columns[iColumn].Width = (int)treeTableViewStorageData.DictData[text];
					return true;
				}
			}
			return false;
		}

		internal static TreeTableViewStorageData FindStorageData(Guid objectGuid, Guid identifcationGuid)
		{
			if (_columnstorage.TreeTableViewStorage.Count == 0)
			{
				DeviceEditorColumnStorage storageData = OptionsHelperColumnStorage.StorageData;
				if (storageData != null)
				{
					foreach (object item in storageData.TreeTableViewStorage)
					{
						if (item is TreeTableViewStorageData)
						{
							_columnstorage.TreeTableViewStorage.Add(item);
						}
					}
				}
			}
			foreach (object item2 in _columnstorage.TreeTableViewStorage)
			{
				TreeTableViewStorageData treeTableViewStorageData = item2 as TreeTableViewStorageData;
				if (treeTableViewStorageData != null && treeTableViewStorageData.ObjectGuid == objectGuid && treeTableViewStorageData.IdentificationGuid == identifcationGuid)
				{
					return treeTableViewStorageData;
				}
			}
			return null;
		}

		internal static TreeTableViewStorageData FindExpandedData(Guid objectGuid, Guid identifcationGuid)
		{
			foreach (TreeTableViewStorageData liExpandedNode in _liExpandedNodes)
			{
				if (liExpandedNode.ObjectGuid == objectGuid && liExpandedNode.IdentificationGuid == identifcationGuid)
				{
					return liExpandedNode;
				}
			}
			return null;
		}

		private void GetExpandedNodes(Hashtable storedExpandedNodes, TreeTableViewNode viewNode)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			IParameterTreeNode parameterTreeNode = ((TreeTableView)this).GetModelNode(viewNode) as IParameterTreeNode;
			if (parameterTreeNode == null || !((ITreeTableNode)parameterTreeNode).HasChildren)
			{
				return;
			}
			if (viewNode.Expanded)
			{
				storedExpandedNodes[parameterTreeNode.DevPath] = 0;
			}
			else
			{
				storedExpandedNodes.Remove(parameterTreeNode.DevPath);
			}
			foreach (TreeTableViewNode node in viewNode.Nodes)
			{
				TreeTableViewNode viewNode2 = node;
				GetExpandedNodes(storedExpandedNodes, viewNode2);
			}
		}

		internal void StoreExpandedNodes()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			if (_bIsInRestore)
			{
				return;
			}
			TreeTableViewStorageData treeTableViewStorageData = FindExpandedData(_objectGuid, _identificationGuid);
			if (treeTableViewStorageData == null)
			{
				treeTableViewStorageData = new TreeTableViewStorageData(_objectGuid, _identificationGuid);
				_liExpandedNodes.Add(treeTableViewStorageData);
			}
			foreach (TreeTableViewNode node in ((TreeTableView)this).Nodes)
			{
				TreeTableViewNode viewNode = node;
				GetExpandedNodes(treeTableViewStorageData.DictData, viewNode);
			}
		}

		private bool ExpandNodes(string nodeToExpand, IParameterTreeNode node)
		{
			if (nodeToExpand == node.DevPath)
			{
				TreeTableViewNode viewNode = ((TreeTableView)this).GetViewNode((ITreeTableNode)node);
				if (viewNode != null)
				{
					viewNode.Expand();
					return true;
				}
			}
			foreach (IParameterTreeNode childNode in node.ChildNodes)
			{
				if (ExpandNodes(nodeToExpand, childNode))
				{
					return true;
				}
			}
			return false;
		}

		public void RestoreExpandedNodes()
		{
			RestoreExpandedNodes(bExpandIfNoData: false);
		}

		public void RestoreExpandedNodes(bool bExpandIfNoData)
		{
			try
			{
				_bIsInRestore = true;
				((TreeTableView)this).BeginUpdate();
				TreeTableViewStorageData treeTableViewStorageData = FindExpandedData(_objectGuid, _identificationGuid);
				if (treeTableViewStorageData != null)
				{
					((TreeTableView)this).CollapseAll();
					if (treeTableViewStorageData.DictData.Count == 0 || ((TreeTableView)this).Model == null)
					{
						return;
					}
					foreach (string key in treeTableViewStorageData.DictData.Keys)
					{
						for (int i = 0; i < ((TreeTableView)this).Model.Sentinel.ChildCount; i++)
						{
							IParameterTreeNode node = ((TreeTableView)this).Model.Sentinel.GetChild(i) as IParameterTreeNode;
							if (ExpandNodes(key, node))
							{
								break;
							}
						}
					}
					return;
				}
				if (bExpandIfNoData)
				{
					((TreeTableView)this).ExpandAll();
				}
				else
				{
					((TreeTableView)this).CollapseAll();
				}
			}
			finally
			{
				((TreeTableView)this).EndUpdate();
				_bIsInRestore = false;
			}
		}

		public void CollapseExpand(object sender, TreeTableViewEventArgs e)
		{
			StoreExpandedNodes();
		}
	}
}
