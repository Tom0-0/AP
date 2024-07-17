using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ProjectDiffViewNodeObjectData
    {
        internal readonly Image Image;

        internal readonly string Name;

        internal readonly Color ForeColor;

        internal readonly Color BackColor;

        internal readonly FontStyle FontStyle;

        internal ProjectDiffViewNodeObjectData(Image image, string stName, Color foreColor, Color backColor, FontStyle fontStyle)
        {
            Image = image;
            Name = stName;
            ForeColor = foreColor;
            BackColor = backColor;
            FontStyle = fontStyle;
        }
    }
}
