using _3S.CoDeSys.Controls.Controls;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ProjectDiffViewNodeObjectRenderer : ITreeTableViewRenderer
    {
        private bool _bIndent;

        public static readonly ProjectDiffViewNodeObjectRenderer Indent;

        public static readonly ProjectDiffViewNodeObjectRenderer NoIndent;

        private static readonly StringFormat STRING_FORMAT;

        static ProjectDiffViewNodeObjectRenderer()
        {
            Indent = new ProjectDiffViewNodeObjectRenderer(bIndent: true);
            NoIndent = new ProjectDiffViewNodeObjectRenderer(bIndent: false);
            STRING_FORMAT = new StringFormat(StringFormat.GenericTypographic);
            STRING_FORMAT.Alignment = StringAlignment.Near;
            STRING_FORMAT.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
            STRING_FORMAT.HotkeyPrefix = HotkeyPrefix.None;
            STRING_FORMAT.LineAlignment = StringAlignment.Center;
            STRING_FORMAT.Trimming = StringTrimming.EllipsisCharacter;
        }

        internal ProjectDiffViewNodeObjectRenderer(bool bIndent)
        {
            _bIndent = bIndent;
        }

        public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }
            TreeTableView obj = node.View ?? throw new ArgumentException("The node is not associated with a view.");
            ProjectDiffViewNodeObjectData projectDiffViewNodeObjectData = node.CellValues[nColumnIndex] as ProjectDiffViewNodeObjectData;
            int num = projectDiffViewNodeObjectData.Image.Width + 4;
            using (Font font = new Font(((Control)(object)obj).Font, projectDiffViewNodeObjectData.FontStyle))
            {
                num += (int)g.MeasureString(projectDiffViewNodeObjectData.Name, font, -1, STRING_FORMAT).Width;
            }
            if (_bIndent)
            {
                num += GetLevel(node) * node.View.Indent;
            }
            return num;
        }

        public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            ProjectDiffViewNodeObjectData projectDiffViewNodeObjectData = node.CellValues[nColumnIndex] as ProjectDiffViewNodeObjectData;
            Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
            bounds.X += projectDiffViewNodeObjectData.Image.Width + 4;
            bounds.Width -= projectDiffViewNodeObjectData.Image.Width + 4;
            if (_bIndent)
            {
                bounds.X += GetLevel(node) * node.View.Indent;
                bounds.Width -= GetLevel(node) * node.View.Indent;
            }
            return bounds;
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
            ProjectDiffViewNodeObjectData projectDiffViewNodeObjectData = node.CellValues[nColumnIndex] as ProjectDiffViewNodeObjectData;
            Rectangle bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
            if (_bIndent)
            {
                bounds.X += GetLevel(node) * node.View.Indent;
                bounds.Width -= GetLevel(node) * node.View.Indent;
            }
            if (projectDiffViewNodeObjectData == null)
            {
                return;
            }
            using (Brush brush = new SolidBrush(projectDiffViewNodeObjectData.BackColor))
            {
                g.FillRectangle(brush, _bIndent ? node.GetBounds(nColumnIndex, (CellBoundsPortion)1) : bounds);
            }
            g.DrawImage(rect: new Rectangle(bounds.Left, bounds.Top + (bounds.Height - projectDiffViewNodeObjectData.Image.Height) / 2, projectDiffViewNodeObjectData.Image.Width, projectDiffViewNodeObjectData.Image.Height), image: projectDiffViewNodeObjectData.Image);
            bounds.X += projectDiffViewNodeObjectData.Image.Width + 6;
            bounds.Width -= projectDiffViewNodeObjectData.Image.Width + 6;
            using Brush brush2 = new SolidBrush(projectDiffViewNodeObjectData.ForeColor);
            using Font font = new Font(((Control)(object)view).Font, projectDiffViewNodeObjectData.FontStyle);
            g.DrawString(projectDiffViewNodeObjectData.Name, font, brush2, bounds, STRING_FORMAT);
        }

        public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            ProjectDiffViewNodeObjectData projectDiffViewNodeObjectData = node.CellValues[nColumnIndex] as ProjectDiffViewNodeObjectData;
            if (projectDiffViewNodeObjectData == null)
            {
                return string.Empty;
            }
            return projectDiffViewNodeObjectData.Name;
        }

        private static int GetLevel(TreeTableViewNode node)
        {
            int num = -1;
            while (node != null)
            {
                num++;
                node = node.Parent;
            }
            return num;
        }
    }
}
