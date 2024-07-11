using System;
using System.Collections.Generic;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	public class ApplicationPrefixEditor : ITreeTableViewEditor
	{
		private static ApplicationPrefixEditor s_singleton = new ApplicationPrefixEditor();

		public static ITreeTableViewEditor Singleton => (ITreeTableViewEditor)(object)s_singleton;

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
			ComboBox comboBox = new ComboBox
			{
				BackColor = ((Control)(object)view).BackColor,
				Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = ((Control)(object)view).Font,
				ForeColor = ((Control)(object)view).ForeColor,
				IntegralHeight = true,
				Sorted = false
			};
			_ = (WatchListNode)(object)node.View.GetModelNode(node);
			IList<string> list = (IList<string>)new LList<string>();
			ApplicationPrefixItem applicationPrefixItem = obj as ApplicationPrefixItem;
			string text = Common.LookForActiveDeviceAppPrefix();
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(text);
			}
			if (applicationPrefixItem != null && !string.IsNullOrEmpty(applicationPrefixItem.ApplicationPrefix) && text != applicationPrefixItem.ApplicationPrefix)
			{
				list.Insert(0, applicationPrefixItem.ApplicationPrefix);
			}
			IList<string> list2 = Common.LookForAllDeviceAppPrefixes();
			if (list2 != null && list2.Count > 0)
			{
				foreach (string item in list2)
				{
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			foreach (string item2 in list)
			{
				comboBox.Items.Add(item2);
			}
			if (comboBox.Items.Count > 0)
			{
				comboBox.SelectedIndex = 0;
				return comboBox;
			}
			return null;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return ((ComboBox)control).SelectedItem;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		protected virtual string ConvertToString(object value)
		{
			if (value != null && value is ApplicationPrefixItem)
			{
				return ((ApplicationPrefixItem)value).ApplicationPrefix;
			}
			return null;
		}
	}
}
