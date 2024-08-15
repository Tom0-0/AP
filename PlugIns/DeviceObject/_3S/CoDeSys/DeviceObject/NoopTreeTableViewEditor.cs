using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceObject
{
	internal class NoopTreeTableViewEditor : ITreeTableViewEditor
	{
		internal static readonly NoopTreeTableViewEditor Singleton = new NoopTreeTableViewEditor();

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return null;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			return null;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}
	}
}
