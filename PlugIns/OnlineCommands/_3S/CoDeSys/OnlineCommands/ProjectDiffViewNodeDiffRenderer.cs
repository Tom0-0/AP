using _3S.CoDeSys.Controls.Controls;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ProjectDiffViewNodeDiffRenderer : ITreeTableViewRenderer
    {
        public static readonly ProjectDiffViewNodeDiffRenderer Singleton = new ProjectDiffViewNodeDiffRenderer();

        public void DrawCell(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
        }

        public Rectangle GetEditableArea(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            return node.GetBounds(nColumnIndex, (CellBoundsPortion)2);
        }

        public int GetPreferredWidth(TreeTableViewNode node, int nColumnIndex, Graphics g)
        {
            return 10;
        }

        public string GetStringRepresentation(TreeTableViewNode node, int nColumnIndex)
        {
            return string.Empty;
        }
    }
}
