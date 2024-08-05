using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class EnumerationTreeTableViewEditor : ITreeTableViewEditor
	{
		private ArrayList _items = new ArrayList();

		private bool _bAllowEmptyValue;

		public EnumerationTreeTableViewEditor(ICollection items, bool bAllowEmptyValue)
		{
			_items.AddRange(items);
			_bAllowEmptyValue = bAllowEmptyValue;
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
			object obj = node.CellValues[nColumnIndex];
			ComboBox comboBox = new ComboBox();
			comboBox.BackColor = ((Control)(object)view).BackColor;
			comboBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox.Font = ((Control)(object)view).Font;
			comboBox.ForeColor = ((Control)(object)view).ForeColor;
			comboBox.IntegralHeight = true;
			comboBox.Sorted = false;
			int num = 0;
			if (_bAllowEmptyValue)
			{
				comboBox.Items.Add("");
				num = 1;
			}
			foreach (object item2 in _items)
			{
				string item = ConvertToString(item2);
				comboBox.Items.Add(item);
			}
			if (obj is int)
			{
				comboBox.SelectedIndex = (int)obj + num;
			}
			else if (obj is EmptyValueData || "".Equals(obj))
			{
				comboBox.SelectedIndex = 0;
			}
			else
			{
				comboBox.SelectedIndex = ((EnumValueData)obj).Index + num;
			}
			return comboBox;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			int selectedIndex = ((ComboBox)control).SelectedIndex;
			if (_bAllowEmptyValue)
			{
				return selectedIndex - 1;
			}
			return selectedIndex;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		protected virtual string ConvertToString(object value)
		{
			if (value != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(value);
				if (converter != null && converter.CanConvertTo(typeof(string)))
				{
					return converter.ConvertToString(value);
				}
			}
			return null;
		}
	}
}
