using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.TabularDeclarationEditor.Comment;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class CommentEditor : ITreeTableViewEditor
	{
		private class Data
		{
			internal readonly string Variable;

			internal readonly string DataType;

			internal readonly Control Panel;

			internal readonly VariableListNode Node;

			internal Data(string stVariable, string stDataType, Control panel, VariableListNode variableListNode)
			{
				Variable = stVariable;
				DataType = stDataType;
				Panel = panel;
				Node = variableListNode;
			}
		}

		private Panel panel;

		private int colWidth;

		private int buttonWidth;

		private Button button;

		private TextBox textBox;

		private TreeTableViewNode ttvNode;

		private IEditor _editor;

		private const int COLIDX_ATTRIBUTE = 9;

		// Token: 0x04000014 RID: 20
		private const int COLIDX_COMMENT = 8;

		internal CommentEditor(IEditor editor)
		{
			_editor = editor;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			ttvNode = node;
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
			panel = new Panel();
			Data data = new Data(variableListNode.Item.Name, variableListNode.Item.DataType, panel, variableListNode);
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			panel.Bounds = bounds;
			TextBox textBox = new TextBox();
			textBox.AcceptsReturn = true;
			textBox.AcceptsTab = true;
			textBox.BackColor = ((Control)(object)view).BackColor;
			textBox.Font = ((Control)(object)view).Font;
			textBox.ForeColor = ((Control)(object)view).ForeColor;
			textBox.Multiline = true;
			textBox.WordWrap = false;
			textBox.Text = variableListNode.Item.Comment;
			textBox.KeyDown += OnKeyDown;
			textBox.TextChanged += OnTextChanged;
			textBox.HandleCreated += OnHandleCreated;
			textBox.Tag = bounds;
			AdaptTextBoxBounds(textBox);
			textBox.Focus();
			Label label = new Label();
			label.Text = Resources.PressCtrlEnterForANewLine;
			label.BackColor = ((Control)(object)view).BackColor;
			label.ForeColor = Color.DarkGray;
			label.AutoSize = true;
			label.TextAlign = ContentAlignment.BottomRight;
			label.Font = ((Control)(object)view).Font;
			label.Dock = DockStyle.Bottom;
			textBox.Controls.Add(label);
			if (cImmediate == '\0')
			{
				textBox.SelectAll();
			}
			else
			{
				textBox.Text = new string(cImmediate, 1);
				textBox.Select(1, 0);
			}
			TabularDeclarationItem item = variableListNode.Item;
			colWidth = bounds.Width;
			panel.Controls.Add(textBox);
			IType type = Common.ParseType(item.DataType);
			IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(_editor.ProjectHandle, _editor.ObjectGuid);
			// 定义数组或数据单元、功能块
			if (type.Class == TypeClass.Array || (type.Class == TypeClass.Userdef && Common.CanExpandAllComment(metaObjectStub.ObjectGuid, data.Variable)))
			{
				using (Graphics graphics = ((Control)(object)view).CreateGraphics())
				{
					buttonWidth = (int)graphics.MeasureString("...", ((Control)(object)view).Font).Width + 10;
				}
				Button button = new Button();
				button.FlatStyle = FlatStyle.Standard;
				button.Text = "...";                        // 添加注释按钮
				button.Size = new Size(buttonWidth, bounds.Height);
				Point location = new Point(bounds.Width - buttonWidth, 0);
				location.Offset(-1, 1);
				button.Location = location;
				button.Font = ((Control)(object)view).Font;
				button.Tag = data;
				this.button = button;
				textBox.Dock = DockStyle.Fill;
				textBox.Width -= buttonWidth;
                button.Click += Button_Click;         // 按钮单击事件
				panel.Controls.Add(button);
				button.BringToFront();
			}
			else
			{
				textBox.Dock = DockStyle.Fill;
			}
			panel.Height += 48;
			this.textBox = textBox;			
			return panel;
		}

		/// <summary>
		/// 注释单击事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
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
				if (Common.ParseType(data.DataType) is IArrayType && Common.IsArraySizeOutOfRange(Common.ParseType(data.DataType) as IArrayType, 65536))
				{
					APEnvironment.MessageService.Error(Resources.ArraySupportSize, "InvalidExpression", Array.Empty<object>());
					return;
				}
				//IInitValueDialogService2 val = APEnvironment.CreateCommentDialogService();
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(_editor.ProjectHandle, _editor.ObjectGuid);
				string elemComment = GetElemComment(data.Node);
				string text = Common.FilterElemCommentAttr((string)data.Node.GetRealValue(9));
				string empty = string.Empty;
				empty = CommentValueDialog.ShowDialogAndStoreState((IWin32Window)data.Panel, data.Variable, data.DataType, textBox.Text, elemComment, metaObjectStub);
				if (empty == null)
				{
					return;
				}
				int num = empty.IndexOf('|');
				string text2 = empty.Substring(0, num);
				string text3 = empty.Substring(num + 1);
				try
				{
					if (!string.IsNullOrEmpty(text2))
					{
						text2 = text2.Replace("$7C", "|");
					}
					textBox.Text = text2;
					if (ttvNode != null)
					{
						ttvNode.EndEdit(6, false);  // 结束对指定单元格的编辑
					}
				}
				catch
				{
					data.Node.SetValue(8, text3);
				}
				data.Node.SetValue(9, text3 + text);
				panel.Controls[0].Focus();
			}
			catch (Exception ex)
			{
				string stackTrace = ex.StackTrace;
			}
		}

		/// <summary>
		/// 获取成员注释
		/// </summary>
		/// <param name="variableListNode"></param>
		/// <returns></returns>
		public static string GetElemComment(VariableListNode variableListNode)
		{
			string text = (string)variableListNode.GetRealValue(9);
			string[] array = text.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.None);
			foreach (string text2 in array)
			{
				if (text2.IndexOf("ElemComment") != -1)
				{
					string[] array3 = text2.Split(new string[]
					{
						":="
					}, StringSplitOptions.RemoveEmptyEntries);
					string text3 = array3[1].Trim();
					return text3.Trim(new char[]
					{
						'\''
					});
				}
			}
			return string.Empty;
		}

		private void OnHandleCreated(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				AdaptTextBoxBounds(textBox);
			}
		}

		/// <summary>
		/// 自适应控件尺寸
		/// </summary>
		/// <param name="textBox"></param>
		private void AdaptTextBoxBounds(TextBox textBox)
		{
			if (textBox != null && textBox.IsHandleCreated)
			{
				Rectangle rectangle = (Rectangle)textBox.Tag;
				using (Graphics graphics = textBox.CreateGraphics())
				{
					Size size = graphics.MeasureString(textBox.Text, textBox.Font).ToSize();
					size.Width = Math.Max(size.Width, graphics.MeasureString(Resources.PressCtrlEnterForANewLine, textBox.Font).ToSize().Width) + 32;
					size.Width = Math.Max(size.Width, this.colWidth);
					size.Height = this.GetLineCount(textBox.Text) * rectangle.Height + 48;
					Point location = new Point(rectangle.X, rectangle.Y);
					textBox.Bounds = new Rectangle(location, size);
					this.panel.Height = size.Height;
					this.panel.Width = Math.Max(100, size.Width);
					if (this.button != null)
					{
						this.button.Left = this.panel.Width - this.buttonWidth - 1;
					}
				}
			}
		}

		private int GetLineCount(string stText)
		{
			int num = 1;
			for (int i = 0; i < stText.Length; i++)
			{
				if (stText[i] == '\n')
				{
					num++;
				}
			}
			return num;
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				AdaptTextBoxBounds(textBox);
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return this.textBox.Text;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}
	}
}
