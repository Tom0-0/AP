using _3S.CoDeSys.Controls.Controls;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class FindResultsNodeRenderer : ITreeTableViewRenderer2, ITreeTableViewRenderer
    {
        private static readonly StringFormat LEFT_FORMAT;

        internal Func<string> HighlightSubstringDelegate { get; private set; }

        static FindResultsNodeRenderer()
        {
            LEFT_FORMAT = new StringFormat(StringFormat.GenericTypographic);
            LEFT_FORMAT.Alignment = StringAlignment.Near;
            LEFT_FORMAT.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            LEFT_FORMAT.HotkeyPrefix = HotkeyPrefix.None;
            LEFT_FORMAT.LineAlignment = StringAlignment.Center;
            LEFT_FORMAT.Trimming = StringTrimming.EllipsisCharacter;
        }

        internal FindResultsNodeRenderer(Func<string> highlightSubstringDelegate)
        {
            HighlightSubstringDelegate = highlightSubstringDelegate;
        }

        public bool NeedsTooltip(TreeTableViewNode node, int nColumnIndex)
        {
            return false;
        }

        public void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            FindResultsNodeData findResultsNodeData = node.CellValues[nColumnIndex] as FindResultsNodeData;
            Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
            if (findResultsNodeData == null)
            {
                return;
            }
            bounds.X += 6;
            bounds.Width -= 6;
            if (findResultsNodeData.Text == null)
            {
                return;
            }
            int num = 0;
            using (Font font = new Font(((Control)(object)node.View).Font, findResultsNodeData.FontStyle))
            {
                string text = ((HighlightSubstringDelegate != null) ? HighlightSubstringDelegate() : null);
                int num2 = ((!string.IsNullOrEmpty(text)) ? findResultsNodeData.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) : (-1));
                if (num2 >= 0)
                {
                    string text2 = findResultsNodeData.Text.Substring(0, num2);
                    string text3 = findResultsNodeData.Text.Substring(num2, text.Length);
                    int num3 = TextRenderer.MeasureText("<" + text2 + ">", font).Width - TextRenderer.MeasureText("<>", font).Width;
                    int width = TextRenderer.MeasureText("<" + text3 + ">", font).Width - TextRenderer.MeasureText("<>", font).Width;
                    Rectangle rect = new Rectangle(bounds.X + num3, bounds.Y, width, bounds.Height);
                    rect.Intersect(bounds);
                    g.FillRectangle(Brushes.Yellow, rect);
                }
                TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;
                TextRenderer.DrawText(g, findResultsNodeData.Text, font, bounds, Color.Black, flags);
                num = TextRenderer.MeasureText(findResultsNodeData.Text, font).Width;
            }
            bounds.X += num + 12;
            bounds.Width -= num + 12;
        }

        public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
            bounds.X += 4;
            bounds.Width -= 4;
            return bounds;
        }

        public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            FindResultsNodeData findResultsNodeData = node.CellValues[nColumnIndex] as FindResultsNodeData;
            int num = 0;
            if (findResultsNodeData != null)
            {
                num += SystemInformation.SmallIconSize.Width + 6;
                if (findResultsNodeData.Text != null)
                {
                    int num2 = 0;
                    using (Font font = new Font(((Control)(object)node.View).Font, findResultsNodeData.FontStyle))
                    {
                        num2 = (int)g.MeasureString(findResultsNodeData.Text, font).Width;
                        if (num2 <= 0)
                        {
                            num2 = Encoding.Default.GetBytes(findResultsNodeData.Text).Length*8; // 一个字符占像素长度为8
                        }
                    }
                    num += num2 + 12;
                }
            }
            return num;
        }

        public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
        {
            FindResultsNodeData findResultsNodeData = node.CellValues[nColumnIndex] as FindResultsNodeData;
            if (findResultsNodeData == null)
            {
                return string.Empty;
            }
            return findResultsNodeData.Text ?? string.Empty;
        }
    }
}
