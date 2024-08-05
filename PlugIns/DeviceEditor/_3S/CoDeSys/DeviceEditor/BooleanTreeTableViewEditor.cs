using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class BooleanTreeTableViewEditor : ITreeTableViewEditor
	{
		public static BooleanTreeTableViewEditor Simple = new BooleanTreeTableViewEditor(bAllowNullValue: false);

		public static BooleanTreeTableViewEditor WithNullValue = new BooleanTreeTableViewEditor(bAllowNullValue: true);

		private bool _bAllowNullValue;

		public BooleanTreeTableViewEditor(bool bAllowNullValue)
		{
			_bAllowNullValue = bAllowNullValue;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			TextBox result = new TextBox
			{
				Bounds = Rectangle.Empty,
				Tag = node.CellValues[nColumnIndex]
			};
			bEditComplete = true;
			return result;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			string empty = string.Empty;
			bool flag = false;
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			if (dataElementNode != null && dataElementNode.PlcNode != null)
			{
				int indexOfColumn = dataElementNode.PlcNode.TreeModel.GetIndexOfColumn(6);
				if (indexOfColumn >= 0)
				{
					object obj = node.CellValues[indexOfColumn];
					if (obj is ValueData)
					{
						ValueData valueData = obj as ValueData;
						if (valueData.Toggleable && valueData.Value != null && valueData.Value is bool)
						{
							flag = (bool)valueData.Value;
						}
					}
				}
			}
			bool flag2 = false;
			bool result = false;
			if (control.Tag is RawValueData && (control.Tag as RawValueData).Value is bool)
			{
				result = (bool)(control.Tag as RawValueData).Value;
				flag2 = true;
			}
			if (control.Tag is string)
			{
				flag2 = bool.TryParse((string)control.Tag, out result);
			}
			empty = (flag ? ((!flag2) ? bool.FalseString : ((!result) ? bool.TrueString : (_bAllowNullValue ? string.Empty : bool.FalseString))) : ((!flag2) ? bool.TrueString : (result ? bool.FalseString : (_bAllowNullValue ? string.Empty : bool.TrueString))));
			return empty.ToUpperInvariant();
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			if (dataElementNode != null && dataElementNode.PlcNode.TreeModel.GetIndexOfColumn(7) >= 0)
			{
				return true;
			}
			return false;
		}
	}
}
