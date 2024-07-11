using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class ArrayCommentEditor : ITreeTableViewEditor
	{
		public object AcceptEdit(TreeTableViewNode node, Control control)
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
			CommentNode commentNode = view.GetModelNode(node) as CommentNode;
			if (commentNode.Parent != null)
			{
				return this.defaultEditor.AcceptEdit(node, control);
			}
			return control.Text;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002B98 File Offset: 0x00000D98
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
			CommentNode commentNode = view.GetModelNode(node) as CommentNode;
			if (commentNode.Parent != null)
			{
				return this.defaultEditor.BeginEdit(node, nColumnIndex, cImmediate, ref bEditComplete);
			}
			Rectangle bounds = node.GetBounds(nColumnIndex, CellBoundsPortion.Editable);
			TextBox textBox = new TextBox();
			textBox.AcceptsReturn = true;
			textBox.AcceptsTab = true;
			textBox.BackColor = view.BackColor;
			textBox.Font = view.Font;
			textBox.ForeColor = view.ForeColor;
			textBox.Multiline = true;
			textBox.WordWrap = false;
			textBox.Text = commentNode.CommentElement.Comment;
			textBox.KeyDown += this.OnKeyDown;
			textBox.TextChanged += this.OnTextChanged;
			textBox.HandleCreated += this.OnHandleCreated;
			textBox.Tag = bounds;
			this.AdaptTextBoxBounds(textBox);
			textBox.Focus();
			Label label = new Label();
			label.Text = Resources.PressCtrlEnterForANewLine;
			label.BackColor = view.BackColor;
			label.ForeColor = Color.DarkGray;
			label.AutoSize = true;
			label.TextAlign = ContentAlignment.BottomRight;
			label.Font = view.Font;
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
			return textBox;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002D3C File Offset: 0x00000F3C
		private void OnHandleCreated(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				this.AdaptTextBoxBounds(textBox);
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002D5C File Offset: 0x00000F5C
		private void AdaptTextBoxBounds(TextBox textBox)
		{
			if (textBox != null && textBox.IsHandleCreated)
			{
				Rectangle rectangle = (Rectangle)textBox.Tag;
				using (Graphics graphics = textBox.CreateGraphics())
				{
					Size size = graphics.MeasureString(textBox.Text, textBox.Font).ToSize();
					int val = Math.Max(size.Width, graphics.MeasureString(Resources.PressCtrlEnterForANewLine, textBox.Font).ToSize().Width) + 32;
					size.Width = Math.Max(rectangle.Width, val);
					size.Height = this.GetLineCount(textBox.Text) * rectangle.Height + 48;
					Point location = new Point(rectangle.X, rectangle.Y);
					textBox.Bounds = new Rectangle(location, size);
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002E50 File Offset: 0x00001050
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

		// Token: 0x0600003C RID: 60 RVA: 0x00002E80 File Offset: 0x00001080
		private void OnTextChanged(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				this.AdaptTextBoxBounds(textBox);
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002E9E File Offset: 0x0000109E
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002EA0 File Offset: 0x000010A0
		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
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
			CommentNode commentNode = view.GetModelNode(node) as CommentNode;
			return commentNode.Parent != null && this.defaultEditor.OneClickEdit(node, nColumnIndex);
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002EFC File Offset: 0x000010FC
		internal static ITreeTableViewEditor MultiTextEditor
		{
			get
			{
				if (ArrayCommentEditor.instance == null)
				{
					object objLock = ArrayCommentEditor._objLock;
					lock (objLock)
					{
						if (ArrayCommentEditor.instance == null)
						{
							ArrayCommentEditor.instance = new ArrayCommentEditor();
						}
					}
				}
				return ArrayCommentEditor.instance;
			}
		}

		// Token: 0x04000009 RID: 9
		private ITreeTableViewEditor defaultEditor = TextBoxTreeTableViewEditor.TextBox;

		// Token: 0x0400000A RID: 10
		private static ITreeTableViewEditor instance;

		// Token: 0x0400000B RID: 11
		private static object _objLock = new object();
	}
}

