using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.WatchList
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
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
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
			ValueData valueData = node.CellValues[nColumnIndex] as ValueData;
			try
			{
				if (valueData == null)
				{
					return;
				}
				string textToRender = WatchListNodeUtils.GetTextToRender(valueData.Text, valueData.TypeClass);
				Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
				bounds.X += 2;
				bounds.Width -= 2;
				if (valueData.Value == PreparedValues.Unforce)
				{
					Brush black = Brushes.Black;
					g.DrawString("<Unforce>", ((Control)(object)view).Font, black, bounds, base._leftFormat);
				}
				else if (valueData.Value == PreparedValues.UnforceAndRestore)
				{
					Brush black2 = Brushes.Black;
					g.DrawString("<Unforce and restore>", ((Control)(object)view).Font, black2, bounds, base._leftFormat);
				}
				else if (textToRender != null)
				{
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
						SizeF sizeF = g.MeasureString(valueData.Text, ((Control)(object)view).Font, bounds.Width, base._leftFormat);
						Rectangle rect = bounds;
						rect.X -= 2;
						rect.Width = sizeF.ToSize().Width + 4;
						g.FillRectangle(brush, rect);
						g.DrawString(textToRender, ((Control)(object)view).Font, white3, bounds, base._leftFormat);
					}
					else
					{
						Brush black3 = Brushes.Black;
						g.DrawString(textToRender, ((Control)(object)view).Font, black3, bounds, base._leftFormat);
					}
				}
			}
			catch
			{
			}
		}

		protected override string ConvertToString(object value)
		{
			ValueData valueData = value as ValueData;
			if (valueData != null)
			{
				if (valueData.Value == PreparedValues.Unforce)
				{
					return "<Unforce>";
				}
				if (valueData.Value == PreparedValues.UnforceAndRestore)
				{
					return "<Unforce and restore>";
				}
				if (valueData.Text != null)
				{
					string text = string.Empty;
					if (_bWithForceIndicator && valueData.Constant)
					{
						text += "(C) ";
					}
					if (_bWithForceIndicator && valueData.Forced)
					{
						text += "(F) ";
					}
					return text + valueData.Text;
				}
			}
			return ConvertToString(value);
		}
	}
}
