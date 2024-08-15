using System.Drawing;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceObjectDiffViewNodeData
	{
		internal readonly string StLine;

		internal readonly Color ForeColor;

		internal readonly Color BackColor;

		internal readonly FontStyle FontStyle;

		internal readonly Image Image;

		internal DeviceObjectDiffViewNodeData(string stLine, Color foreColor, Color backColor, FontStyle fontStyle, Image image)
		{
			StLine = stLine;
			ForeColor = foreColor;
			BackColor = backColor;
			FontStyle = fontStyle;
			Image = image;
		}
	}
}
