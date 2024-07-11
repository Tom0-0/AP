using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.WatchList
{
	internal class ExpressionRenderer : LabelTreeTableViewRenderer
	{
		private static ITreeTableViewRenderer s_singleton = (ITreeTableViewRenderer)(object)new ExpressionRenderer();

		public static ITreeTableViewRenderer Singleton => s_singleton;

		public ExpressionRenderer()
			: base(false)
		{
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
			if (((CollectionBase)(object)node.CellValues).Count == 0)
			{
				return;
			}
			ExpressionData expressionData = node.CellValues[nColumnIndex] as ExpressionData;
			if (expressionData == null)
			{
				return;
			}
			string displayExpression = expressionData.DisplayExpression;
			Image image = expressionData.Image;
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
			if (image != null)
			{
				g.DrawImage(image, bounds.Left + 2, bounds.Top + (bounds.Height - 16) / 2);
			}
			if (displayExpression == null)
			{
				return;
			}
			int iStartOfInterpretedAccess = 0;
			displayExpression = CastExpressionFormatter.Instance.GetCastExpressionDisplayString(displayExpression, out iStartOfInterpretedAccess);
			bounds.X += 22;
			bounds.Width -= 22;
			StringFormat stringFormat = GetStringFormat(view, nColumnIndex);
			Color color = (expressionData.IsOutdated ? Color.Gray : ((Control)(object)view).ForeColor);
			if (0 <= iStartOfInterpretedAccess)
			{
				string text = displayExpression.Substring(0, iStartOfInterpretedAccess);
				string s = displayExpression.Substring(iStartOfInterpretedAccess);
				SizeF sizeF = SizeF.Empty;
				using (Brush brush = new SolidBrush(color))
				{
					g.DrawString(text, ((Control)(object)view).Font, brush, bounds, stringFormat);
					sizeF = g.MeasureString(text, ((Control)(object)view).Font);
				}
				using Brush brush2 = new SolidBrush(Color.FromArgb(60, 60, 60));
				bounds.X += (int)sizeF.Width;
				bounds.Width -= (int)sizeF.Width;
				Font font = new Font(((Control)(object)view).Font, FontStyle.Italic);
				g.DrawString(s, font, brush2, bounds, stringFormat);
			}
			else
			{
				using Brush brush3 = new SolidBrush(color);
				g.DrawString(displayExpression, ((Control)(object)view).Font, brush3, bounds, stringFormat);
			}
		}

		public override Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			Rectangle editableArea = base.GetEditableArea(node, nColumnIndex, g);
			editableArea.X += 20;
			editableArea.Width -= 20;
			return editableArea;
		}

		public override int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			return base.GetPreferredWidth(node, nColumnIndex, g) + 20;
		}

		public override string GetStringRepresentation(TreeTableViewNode node, int nModelColumnIndex)
		{
			ExpressionData expressionData = node.CellValues[nModelColumnIndex] as ExpressionData;
			if (expressionData == null)
			{
				return base.GetStringRepresentation(node, nModelColumnIndex);
			}
			return CastExpressionFormatter.Instance.GetCastExpressionDisplayString(expressionData.DisplayExpression);
		}

		protected override string ConvertToString(object value)
		{
			ExpressionData expressionData = value as ExpressionData;
			if (expressionData != null && expressionData.DisplayExpression != null)
			{
				return expressionData.DisplayExpression;
			}
			return ConvertToString(value);
		}
	}
}
