using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.PInvoke;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class LineRenderer : ITreeTableViewRenderer, IDisposable
	{
		private StringFormat _rightFormat;

		private Brush _backBrush;

		private Pen _focusBorderPen;

		private Pen _noFocusBorderPen;

		private IntPtr _fontHandle;

		private const int FOLD_MARKER_SIZE = 12;

		private const int DECORATION_SIZE = 16;

		internal static readonly Icon BookmarkSmall = Resources.BookmarkSmall;

		internal LineRenderer()
		{
			_rightFormat = new StringFormat(StringFormat.GenericDefault);
			_rightFormat.Alignment = StringAlignment.Far;
			_rightFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
			_rightFormat.HotkeyPrefix = HotkeyPrefix.None;
			_rightFormat.LineAlignment = StringAlignment.Center;
			_rightFormat.Trimming = StringTrimming.EllipsisCharacter;
			UpdateCachedGraphicObjects();
		}

		private void UpdateCachedGraphicObjects()
		{
			if (_backBrush != null)
			{
				_backBrush.Dispose();
				_backBrush = null;
			}
			if (_focusBorderPen != null)
			{
				_focusBorderPen.Dispose();
				_focusBorderPen = null;
			}
			if (_noFocusBorderPen != null)
			{
				_noFocusBorderPen.Dispose();
				_noFocusBorderPen = null;
			}
			_backBrush = new SolidBrush(IECTextEditorOptions.MarginBackgroundColor);
			_focusBorderPen = new Pen(IECTextEditorOptions.MarginFocusBorderColor, 2f);
			_noFocusBorderPen = new Pen(IECTextEditorOptions.MarginNoFocusBorderColor, 2f);
			if (_fontHandle != IntPtr.Zero)
			{
				Gdi32.DeleteObject(_fontHandle);
				_fontHandle = IntPtr.Zero;
			}
			Font font = new Font(IECTextEditorOptions.MarginFont, IECTextEditorOptions.MarginFontSize, IECTextEditorOptions.MarginFontStyle);
			_fontHandle = font.ToHfont();
		}

		public void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
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
			bool flag = node.NextVisibleNode == null;
			LineCellData lineCellData = (LineCellData)node.CellValues[nColumnIndex];
			int preferredWidth = GetPreferredWidth(node, nColumnIndex, g);
			if (lineCellData == null)
			{
				return;
			}
			Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)1);
			g.FillRectangle(_backBrush, bounds);
			int height = (flag ? 10000 : bounds.Height);
			g.FillRectangle(Brushes.WhiteSmoke, bounds.X + preferredWidth - 16, bounds.Top, 16, height);
			if (IECTextEditorOptions.MarginLineNumbers)
			{
				int num = TextRenderer.GetWidth(g, _fontHandle, "12345") - 1;
				Color color = ((!node.Selected || !IECTextEditorOptions.MarginHighlightCurrentLine) ? IECTextEditorOptions.MarginForegroundColor : IECTextEditorOptions.MarginHighlightColor);
				if (lineCellData.LineNumber >= 0)
				{
					TextRenderer.Draw(g, bounds.X + 12 + num, bounds.Y, (Alignment)2, _fontHandle, color, lineCellData.LineNumber.ToString());
				}
			}
			int y = (flag ? (bounds.Top + 10000) : bounds.Bottom);
			if (((Control)(object)view).ContainsFocus)
			{
				g.DrawLine(_focusBorderPen, bounds.X + preferredWidth - 16 - 2, bounds.Top, bounds.X + preferredWidth - 16 - 2, y);
			}
			else
			{
				g.DrawLine(_noFocusBorderPen, bounds.X + preferredWidth - 16 - 2, bounds.Top, bounds.X + preferredWidth - 16 - 2, y);
			}
			if (lineCellData.Bookmark)
			{
				g.DrawIcon(targetRect: new Rectangle(bounds.X + preferredWidth - 16 + 2, bounds.Top + 3, 16, 16), icon: BookmarkSmall);
			}
		}

		public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
		}

		public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
		{
			int num = 31;
			if (IECTextEditorOptions.MarginLineNumbers)
			{
				num += TextRenderer.GetWidth(g, _fontHandle, "12345");
			}
			return num;
		}

		public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.View == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			LineCellData lineCellData = (LineCellData)node.CellValues[nColumnIndex];
			if (lineCellData != null)
			{
				return lineCellData.LineNumber.ToString();
			}
			return string.Empty;
		}

		public void Dispose()
		{
			if (_backBrush != null)
			{
				_backBrush.Dispose();
				_backBrush = null;
			}
			if (_focusBorderPen != null)
			{
				_focusBorderPen.Dispose();
				_focusBorderPen = null;
			}
			if (_noFocusBorderPen != null)
			{
				_noFocusBorderPen.Dispose();
				_noFocusBorderPen = null;
			}
			if (_fontHandle != IntPtr.Zero)
			{
				Gdi32.DeleteObject(_fontHandle);
				_fontHandle = IntPtr.Zero;
			}
		}
	}
}
