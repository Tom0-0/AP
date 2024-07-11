using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
    public partial class CommentValueDialog : Form
    {
		private IMetaObjectStub _mos;

		private TreeTableView _treeTableView;

		public CommentValueDialog()
        {
            InitializeComponent();
        }

		internal static string ShowDialogAndStoreState(IWin32Window owner, string stVariable, string stType, string stInitValue, string stElemCommentValue, IMetaObjectStub invokedBy)
		{
			CommentValueDialog commentValueDialog = new CommentValueDialog();
			Guid guid = new Guid("{566DD09C-4526-4659-BC45-4680B2C5319C}");
			DialogHelper.RestoreStateWithPosition((Form)commentValueDialog, guid);
			commentValueDialog.Initialize(stVariable, stType, stInitValue, stElemCommentValue, invokedBy);
			string result = null;
			if (DialogResult.OK == commentValueDialog.ShowDialog(owner))
			{
				result = commentValueDialog.GetCommentValue();
			}
			DialogHelper.StoreState((Form)commentValueDialog, guid);
			return result;
		}

		internal string GetCommentValue()
		{
			return ((CommentModel)this._treeTableView.Model).GetCommentValue();
		}

		internal void AdjustColumnWidth()
		{
			if (this._treeTableView.Columns.Count > 0)
			{
				int width = (this._treeTableView.Width - 10) / this._treeTableView.Columns.Count;
				this._treeTableView.BeginUpdate();
				try
				{
					for (int i = 0; i < this._treeTableView.Model.ColumnCount; i++)
					{
						this._treeTableView.Columns[i].Width = width;
					}
				}
				finally
				{
					this._treeTableView.EndUpdate();
				}
			}
		}

		private void Initialize(string stVariable, string stType, string stInitValue, string stElemCommentValue, IMetaObjectStub invokedBy)
		{
			_mos = invokedBy;
			_treeTableView.Model=((ITreeTableModel)(object)new CommentModel(stVariable, stType, stInitValue, stElemCommentValue, invokedBy));	// 初始化注释界面
			if (_treeTableView.Nodes.Count > 0)
			{
				_treeTableView.Nodes[0].Expand();
			}
			AdjustColumnWidth();
		}

		private void _treeTableView_SelectionChanged(object sender, EventArgs e)
		{
			InitValueNode initValueNode = this._treeTableView.GetModelNode(this._treeTableView.FocusedNode) as InitValueNode;
		}

		internal CommentNode[] GetSelectedNodes()
		{
			CommentNode[] array = new CommentNode[this._treeTableView.SelectedNodes.Count];
			for (int i = 0; i < this._treeTableView.SelectedNodes.Count; i++)
			{
				TreeTableViewNode viewNode = this._treeTableView.SelectedNodes[i];
				array[i] = (this._treeTableView.GetModelNode(viewNode) as CommentNode);
			}
			return array;
		}

		internal CommentNode SelectedArrayNode
		{
			get
			{
				try
				{
					if (this._treeTableView.SelectedNodes != null && this._treeTableView.SelectedNodes.Count == 1)
					{
						CommentNode commentNode = this._treeTableView.GetModelNode(this._treeTableView.SelectedNodes[0]) as CommentNode;
						if (commentNode.GetRealType().Class == TypeClass.Array)
						{
							return commentNode;
						}
						if (commentNode.Parent is CommentNode && ((CommentNode)commentNode.Parent).GetRealType().Class == TypeClass.Array)
						{
							return commentNode.Parent as CommentNode;
						}
					}
				}
				catch
				{
				}
				return null;
			}
		}

		

		private void _okButton_Click(object sender, EventArgs e)
        {
			int num = default(int);
			TreeTableViewNode focusedNode = _treeTableView.GetFocusedNode(out num);
			if (focusedNode != null && focusedNode.IsEditing(num))
			{
				focusedNode.EndEdit(num, false);
				base.DialogResult = DialogResult.None;
			}
		}

        private void _cancelButton_Click(object sender, EventArgs e)
        {
			int num = default(int);
			TreeTableViewNode focusedNode = _treeTableView.GetFocusedNode(out num);
			if (focusedNode != null && focusedNode.IsEditing(num))
			{
				focusedNode.EndEdit(num, true);
				base.DialogResult = DialogResult.None;
			}
		}
    }
}
