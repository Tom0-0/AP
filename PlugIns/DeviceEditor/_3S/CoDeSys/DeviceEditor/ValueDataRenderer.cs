using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class ValueDataRenderer : LabelTreeTableViewRenderer
	{
		private bool _bWithForceIndicator;

		public static readonly ValueDataRenderer WithForceIndicator = new ValueDataRenderer(bWithForceIndicator: true);

		public static readonly ValueDataRenderer WithoutForceIndicator = new ValueDataRenderer(bWithForceIndicator: false);

		public ValueDataRenderer(bool bWithForceIndicator)
			: base(false)
		{
			_bWithForceIndicator = bWithForceIndicator;
		}

		public override void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			TreeTableView view = node.View;
			if (view == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			int num = -1;
			if (dataElementNode != null)
			{
				num = dataElementNode.GetOnlineIndex();
			}
			object obj = node.CellValues[nColumnIndex];
			try
			{
				TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
				Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
				bounds.X += 2;
				bounds.Width -= 2;
				if (obj is string)
				{
					Color foreColor = Color.Black;
					if (nColumnIndex == num && !dataElementNode.IsMapped)
					{
						foreColor = Color.LightGray;
					}
					TextRenderer.DrawText(g, obj.ToString(), ((Control)(object)view).Font, bounds, foreColor, Color.Transparent, flags);
				}
				if (!(obj is ValueData))
				{
					return;
				}
				ValueData valueData = obj as ValueData;
				if (valueData.Value == PreparedValues.Unforce)
				{
					TextRenderer.DrawText(g, "<Unforce>", ((Control)(object)view).Font, bounds, Color.Black, Color.Transparent, flags);
				}
				else if (valueData.Value == PreparedValues.UnforceAndRestore)
				{
					TextRenderer.DrawText(g, "<Unforce and restore>", ((Control)(object)view).Font, bounds, Color.Black, Color.Transparent, flags);
				}
				else
				{
					if (valueData.Text == null)
					{
						return;
					}
					if (_bWithForceIndicator && valueData.Constant)
					{
						Brush white = Brushes.White;
						Brush green = Brushes.Green;
						Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + 1, bounds.Height - 2, bounds.Height - 2);
						g.FillEllipse(green, rectangle);
						g.DrawString("C", ((Control)(object)view).Font, white, rectangle, base._centerFormat);
						bounds.X += rectangle.Width + 4;
						bounds.Width -= rectangle.Width + 4;
					}
					if (_bWithForceIndicator && valueData.Forced)
					{
						Brush white2 = Brushes.White;
						Brush darkRed = Brushes.DarkRed;
						Rectangle rectangle2 = new Rectangle(bounds.X, bounds.Y + 1, bounds.Height - 2, bounds.Height - 2);
						g.FillEllipse(darkRed, rectangle2);
						g.DrawString("F", ((Control)(object)view).Font, white2, rectangle2, base._centerFormat);
						bounds.X += rectangle2.Width + 4;
						bounds.Width -= rectangle2.Width + 4;
					}
					if (valueData.Toggleable && valueData.Value != null && valueData.Value is bool)
					{
						Brush white3 = Brushes.White;
						Brush brush = (((bool)valueData.Value) ? Brushes.Blue : Brushes.Black);
						if (nColumnIndex == num && !dataElementNode.IsMapped)
						{
							brush = Brushes.LightGray;
						}
						SizeF sizeF = g.MeasureString(valueData.Text, ((Control)(object)view).Font, -1, base._leftFormat);
						Rectangle rect = bounds;
						rect.X -= 2;
						rect.Width = sizeF.ToSize().Width + 4;
						g.FillRectangle(brush, rect);
						g.DrawString(valueData.Text, ((Control)(object)view).Font, white3, bounds, base._leftFormat);
					}
					else
					{
						Color foreColor2 = Color.Black;
						if (nColumnIndex == num && !dataElementNode.IsMapped)
						{
							foreColor2 = Color.LightGray;
						}
						TextRenderer.DrawText(g, valueData.Text, ((Control)(object)view).Font, bounds, foreColor2, Color.Transparent, flags);
					}
					return;
				}
			}
			catch
			{
			}
		}
	}
}
