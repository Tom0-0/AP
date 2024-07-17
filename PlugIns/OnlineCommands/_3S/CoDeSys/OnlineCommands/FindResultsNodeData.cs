using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class FindResultsNodeData
    {
        internal string Text { get; private set; }

        internal FontStyle FontStyle { get; private set; }

        internal FindResultsNodeData(string formattedString, FontStyle fontStyle)
        {
            Text = formattedString;
            FontStyle = fontStyle;
        }
    }
}
