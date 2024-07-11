#define DEBUG
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.AutoDeclare;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class DataTypeEditor : ITreeTableViewEditor
	{
		private IEditor _editor;

		internal DataTypeEditor(IEditor editor)
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
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			Panel panel = new Panel();
			panel.Bounds = bounds;
			int num;
			using (Graphics graphics = ((Control)(object)view).CreateGraphics())
			{
				num = (int)graphics.MeasureString(">", ((Control)(object)view).Font).Width + 16;
			}
			Button button = new Button();
			button.FlatStyle = FlatStyle.Standard;
			button.Text = ">";
			button.Size = new Size(num, bounds.Height);
			button.Location = new Point(bounds.Width - num, 0);
			button.Font = ((Control)(object)view).Font;
			button.Tag = panel;
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
			textBox.Tag = panel;
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
			if (control != null)
			{
				ContextMenu contextMenu = new ContextMenu();
				contextMenu.MenuItems.Add(Resources.InputAssistant, OnInputAssistant).Tag = control.Tag;
				contextMenu.MenuItems.Add(Resources.ArrayWizard, OnArrayWizard).Tag = control.Tag;
				contextMenu.Show(control, new Point(0, control.Height + 1));
			}
		}

		private void OnInputAssistant(object sender, EventArgs e)
		{
			try
			{
				MenuItem menuItem = sender as MenuItem;
				if (menuItem == null)
				{
					return;
				}
				Control control = menuItem.Tag as Control;
				if (control != null)
				{
					IInputAssistantService obj = APEnvironment.CreateInputAssistantService();
					Debug.Assert(obj != null);
					string text = obj.Invoke((IInputAssistantCategory[])(object)new IInputAssistantCategory[2]
					{
						(IInputAssistantCategory)APEnvironment.CreateStandardTypesInputAssistantCategory(),
						(IInputAssistantCategory)APEnvironment.CreateStructuredTypesInputAssistantCategory()
					}, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, _editor.ProjectHandle, _editor.ObjectGuid, (IWin32Window)control);
					if (text != null)
					{
						control.Controls[0].Text = text;
					}
				}
			}
			catch
			{
			}
		}

		private void OnArrayWizard(object sender, EventArgs e)
		{
			try
			{
				MenuItem menuItem = sender as MenuItem;
				if (menuItem == null)
				{
					return;
				}
				Control control = menuItem.Tag as Control;
				if (control != null)
				{
					IArrayTypeDialogService obj = APEnvironment.CreateArrayTypeDialogService();
					Debug.Assert(obj != null);
					string text = obj.Invoke((IWin32Window)control, control.Controls[0].Text);
					if (text != null)
					{
						control.Controls[0].Text = text;
					}
				}
			}
			catch
			{
			}
		}
	}
}
