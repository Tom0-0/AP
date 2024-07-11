using System;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class AttributeEditor : ITreeTableViewEditor
	{
		private TabularDeclarationEditor _editor;

		internal AttributeEditor(TabularDeclarationEditor editor)
		{
			_editor = editor;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			TreeTableView view = node.View;
			if (view == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			VariableListNode variableListNode = view.GetModelNode(node) as VariableListNode;
			if (variableListNode == null)
			{
				return null;
			}
			AttributeDialog attributeDialog = new AttributeDialog();
			attributeDialog.Initialize((string)variableListNode.GetValue(nColumnIndex), _editor.OnlineState.OnlineApplication != Guid.Empty);
			if (attributeDialog.ShowDialog((IWin32Window)view) == DialogResult.OK)
			{
				variableListNode.SetValue(nColumnIndex, attributeDialog.Result);
			}
			bEditComplete = true;
			return null;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return null;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}
	}
}
