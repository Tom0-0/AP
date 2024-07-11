#define DEBUG
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.AutoDeclare;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class InitializationEditor : ITreeTableViewEditor
	{
		private class Data
		{
			internal readonly string Variable;

			internal readonly string DataType;

			internal readonly Control Panel;

			internal Data(string stVariable, string stDataType, Control panel)
			{
				Variable = stVariable;
				DataType = stDataType;
				Panel = panel;
			}
		}

		private IEditor _editor;

		internal InitializationEditor(IEditor editor)
		{
			_editor = editor;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
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
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			Panel panel = new Panel();
			Data tag = new Data(variableListNode.Item.Name, variableListNode.Item.DataType, panel);
			panel.Bounds = bounds;
			int num;
			using (Graphics graphics = ((Control)(object)view).CreateGraphics())
			{
				num = (int)graphics.MeasureString("...", ((Control)(object)view).Font).Width + 16;
			}
			Button button = new Button();
			button.FlatStyle = FlatStyle.Standard;
			button.Text = "...";
			button.Size = new Size(num, bounds.Height);
			button.Location = new Point(bounds.Width - num, 0);
			button.Font = ((Control)(object)view).Font;
			button.Tag = tag;
			button.Click += OnClick;
			TextBox textBox = new TextBox();
			textBox.AutoSize = false;
			textBox.BackColor = ((Control)(object)view).BackColor;
			textBox.BorderStyle = BorderStyle.FixedSingle;
			textBox.Size = new Size(bounds.Width - num, bounds.Height);
			textBox.Location = Point.Empty;
			textBox.Font = ((Control)(object)view).Font;
			textBox.ForeColor = ((Control)(object)view).ForeColor;
			textBox.Text = (string)node.CellValues[nColumnIndex];
			textBox.TextAlign = view.Columns[nColumnIndex].TextAlign;
			textBox.Tag = tag;
			if (cImmediate == '\0')
			{
				textBox.SelectAll();
			}
			else
			{
				textBox.Select(1, 0);
			}
			panel.Controls.Add(textBox);
			panel.Controls.Add(button);
			return panel;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return control.Controls[0].Text;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		private void OnClick(object sender, EventArgs e)
		{
			Control control = sender as Control;
			if (control == null)
			{
				return;
			}
			Data data = control.Tag as Data;
			if (data == null)
			{
				return;
			}
			try
			{
				IInitValueDialogService obj = APEnvironment.CreateInitValueDialogService();
				Debug.Assert(obj != null);
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(_editor.ProjectHandle, _editor.ObjectGuid);
				Debug.Assert(metaObjectStub != null);
				string text = obj.Invoke((IWin32Window)data.Panel, data.Variable, data.DataType, data.Panel.Controls[0].Text, metaObjectStub);
				if (text != null)
				{
					data.Panel.Controls[0].Text = text;
				}
			}
			catch
			{
			}
		}
	}
}
