using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	public class EnumerationTreeTableViewRenderer : LabelTreeTableViewRenderer
	{
		private ArrayList _items = new ArrayList();

		public EnumerationTreeTableViewRenderer(ICollection items)
			: base(false)
		{
			_items.AddRange(items);
		}

		protected override string ConvertToString(object value)
		{
			int num = -1;
			if (value is int)
			{
				num = (int)value;
			}
			else if (value is EnumValueData)
			{
				num = ((EnumValueData)value).Index;
			}
			if (num >= 0 && num < _items.Count)
			{
				return base.ConvertToString(_items[num]);
			}
			return "";
		}

		protected override Brush GetForeColorBrush(TreeTableViewNode node, int nColumnIndex)
		{
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			if (dataElementNode != null && dataElementNode.ReadOnly)
			{
				return new SolidBrush(ControlPaint.LightLight(((Control)(object)node.View).ForeColor));
			}
			return base.GetForeColorBrush(node, nColumnIndex);
		}
	}
}
