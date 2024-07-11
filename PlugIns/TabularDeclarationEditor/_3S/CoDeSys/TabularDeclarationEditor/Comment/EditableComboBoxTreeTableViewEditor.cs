using _3S.CoDeSys.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class EditableComboBoxTreeTableViewEditor : ITreeTableViewEditor
	{
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
			EditableComboBoxValue editableComboBoxValue = node.CellValues[nColumnIndex] as EditableComboBoxValue;
			if (editableComboBoxValue.Items != null && editableComboBoxValue.Items.Count > 0)
			{
				ComboBox comboBox = new ComboBox();
				comboBox.BackColor = ((Control)(object)view).BackColor;
				comboBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
				comboBox.DropDownStyle = ComboBoxStyle.DropDown;
				comboBox.Font = ((Control)(object)view).Font;
				comboBox.ForeColor = ((Control)(object)view).ForeColor;
				comboBox.IntegralHeight = true;
				comboBox.Sorted = false;
				foreach (string item in editableComboBoxValue.Items)
				{
					comboBox.Items.Add(item);
				}
				if (editableComboBoxValue.Text != null)
				{
					comboBox.Text = editableComboBoxValue.Text;
				}
				return comboBox;
			}
			HorizontalAlignment textAlign = view.Columns[nColumnIndex].TextAlign;
			TextBox textBox = new TextBox();
			textBox.BackColor = ((Control)(object)view).BackColor;
			textBox.BorderStyle = BorderStyle.FixedSingle;
			textBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			textBox.Font = ((Control)(object)view).Font;
			textBox.ForeColor = ((Control)(object)view).ForeColor;
			textBox.Text = ((cImmediate == '\0') ? editableComboBoxValue.Text : cImmediate.ToString());
			textBox.TextAlign = textAlign;
			textBox.Tag = editableComboBoxValue?.GetType();
			if (cImmediate == '\0')
			{
				textBox.SelectAll();
			}
			else
			{
				textBox.Select(1, 0);
			}
			return textBox;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return control.Text;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		protected virtual string ConvertToString(object value)
		{
			EditableComboBoxValue editableComboBoxValue = value as EditableComboBoxValue;
			if (editableComboBoxValue != null)
			{
				return editableComboBoxValue.Text;
			}
			return value.ToString();
		}

		protected virtual object ConvertFromString(string stText, Type type)
		{
			if (stText != null)
			{
				return stText;
			}
			return null;
		}
	}
}
