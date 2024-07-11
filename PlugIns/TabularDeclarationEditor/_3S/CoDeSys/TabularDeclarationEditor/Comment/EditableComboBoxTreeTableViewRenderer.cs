using _3S.CoDeSys.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class EditableComboBoxTreeTableViewRenderer : LabelTreeTableViewRenderer
	{
		public EditableComboBoxTreeTableViewRenderer()
			: base(false)
		{
		}

		public override string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return ConvertToString(node.CellValues[nColumnIndex]);
		}

		protected override string ConvertToString(object value)
		{
			EditableComboBoxValue editableComboBoxValue = value as EditableComboBoxValue;
			if (editableComboBoxValue != null)
			{
				return editableComboBoxValue.Text;
			}
			return value.ToString();
		}

		protected override FontStyle GetFontStyle(TreeTableViewNode node, int nColumnIndex)
		{
			EditableComboBoxValue editableComboBoxValue = node.CellValues[nColumnIndex] as EditableComboBoxValue;
			if (editableComboBoxValue != null && !editableComboBoxValue.IsDefaultValue)
			{
				return FontStyle.Bold;
			}
			return FontStyle.Regular;
		}

		protected override Brush GetBackColorBrush(TreeTableViewNode node, int nColumnIndex)
		{
			if (nColumnIndex != 1)
			{
				return base.GetBackColorBrush(node, nColumnIndex);
			}
			InitValueNode initValueNode = node.View.GetModelNode(node) as InitValueNode;
			bool flag = false;
			if (initValueNode != null)
			{
				flag = initValueNode.NotFbOrFbInOut;
			}
			if (flag)
			{
				return base.GetBackColorBrush(node, nColumnIndex);
			}
			return new SolidBrush(Color.LightGray);
		}
	}
}
