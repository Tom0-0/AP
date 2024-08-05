using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MyLabelTreeTableViewRenderer : LabelTreeTableViewRenderer
	{
		private static ITreeTableViewRenderer s_normalString = (ITreeTableViewRenderer)(object)new MyLabelTreeTableViewRenderer(bPathEllipses: false);

		private static readonly int MAX_STRING_LENGTH = 4096;

		private bool _bPathEllipses;

		internal Func<string> HighlightSubstringDelegate { get; private set; }

		public static ITreeTableViewRenderer NormalString => s_normalString;

		public MyLabelTreeTableViewRenderer(Func<string> highlightSubstringDelegate, bool bPathEllipses)
			: this(bPathEllipses)
		{
			_bPathEllipses = bPathEllipses;
			HighlightSubstringDelegate = highlightSubstringDelegate;
		}

		public MyLabelTreeTableViewRenderer(bool bPathEllipses)
			: base(bPathEllipses)
		{
			_bPathEllipses = bPathEllipses;
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

		protected override FontStyle GetFontStyle(TreeTableViewNode node, int nColumnIndex)
		{
			DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
			if (dataElementNode != null && nColumnIndex == dataElementNode.GetAddressIndex() && dataElementNode.HasInvalidIecAddress)
			{
				return base.GetFontStyle(node, nColumnIndex) | FontStyle.Strikeout;
			}
			return base.GetFontStyle(node, nColumnIndex);
		}

		public override void DrawCell(TreeTableViewNode node, int nModelColumnIndex, Graphics g)
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
			string text = base.ConvertToString(node.CellValues[nModelColumnIndex]);
			if (text == null)
			{
				return;
			}
			text = text.Replace("\r\n", "  ");
			text = text.Replace("\r", "  ");
			text = text.Replace("\n", "  ");
			StringFormat stringFormat = base.GetStringFormat(view, nModelColumnIndex);
			Rectangle bounds = node.GetBounds(nModelColumnIndex, (CellBoundsPortion)2);
			bounds.X += 2;
			bounds.Width -= 2;
			using (Brush brush = base.GetBackColorBrush(node, nModelColumnIndex))
			{
				g.FillRectangle(brush, bounds);
			}
			using Brush brush2 = base.GetForeColorBrush(node, nModelColumnIndex);
			using Font font = new Font(((Control)(object)view).Font, base.GetFontStyle(node, nModelColumnIndex));
			if (text.Length > MAX_STRING_LENGTH)
			{
				text = text.Substring(0, MAX_STRING_LENGTH);
			}
			if (brush2 is SolidBrush)
			{
				TextFormatFlags textFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
				if (_bPathEllipses)
				{
					textFormatFlags |= TextFormatFlags.PathEllipsis;
				}
				switch (stringFormat.Alignment)
				{
				case StringAlignment.Near:
					textFormatFlags |= TextFormatFlags.Default;
					break;
				case StringAlignment.Center:
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;
				case StringAlignment.Far:
					textFormatFlags |= TextFormatFlags.Right;
					break;
				}
				if (node.View.GetModelNode(node) is DataElementNode)
				{
					string text2 = ((HighlightSubstringDelegate != null) ? HighlightSubstringDelegate() : null);
					int num = ((!string.IsNullOrEmpty(text2)) ? text.IndexOf(text2, StringComparison.OrdinalIgnoreCase) : (-1));
					if (num >= 0)
					{
						string text3 = text.Substring(0, num);
						string text4 = text.Substring(num, text2.Length);
						int num2 = TextRenderer.MeasureText("<" + text3 + ">", font).Width - TextRenderer.MeasureText("<>", font).Width;
						int width = TextRenderer.MeasureText("<" + text4 + ">", font).Width - TextRenderer.MeasureText("<>", font).Width;
						Rectangle rect = new Rectangle(bounds.X + num2, bounds.Y, width, bounds.Height);
						rect.Intersect(bounds);
						g.FillRectangle(Brushes.Yellow, rect);
					}
				}
				TextRenderer.DrawText(g, text, font, bounds, ((SolidBrush)brush2).Color, Color.Transparent, textFormatFlags);
			}
			else
			{
				g.DrawString(text, font, brush2, bounds, stringFormat);
			}
		}
	}
}
