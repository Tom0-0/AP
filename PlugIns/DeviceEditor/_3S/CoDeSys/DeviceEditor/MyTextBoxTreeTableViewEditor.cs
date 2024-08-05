using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MyTextBoxTreeTableViewEditor : TextBoxTreeTableViewEditor
	{
		private static ITreeTableViewEditor s_textBox = (ITreeTableViewEditor)(object)new MyTextBoxTreeTableViewEditor();

		public static ITreeTableViewEditor TextBox => s_textBox;

		public override Control BeginEdit(TreeTableViewNode node, int nModelColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			if (cImmediate == '+' || cImmediate == '-' || cImmediate == '*' || cImmediate == '/')
			{
				return null;
			}
			return ((TextBoxTreeTableViewEditor)this).BeginEdit(node, nModelColumnIndex, cImmediate, ref bEditComplete);
		}

		public MyTextBoxTreeTableViewEditor()
			: base()
		{
		}
	}
}
