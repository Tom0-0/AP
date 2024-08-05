using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MappingTreeTableViewEditor : ITreeTableViewEditor
	{
		public static readonly ITreeTableViewEditor Singleton = (ITreeTableViewEditor)(object)new MappingTreeTableViewEditor();

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			TextBox result = new TextBox
			{
				Bounds = Rectangle.Empty,
				Tag = node.CellValues[nColumnIndex]
			};
			bEditComplete = true;
			return result;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			if (control.Tag is MappingTreeTableViewCellValue)
			{
				return (MappingTreeTableViewCellValue)control.Tag switch
				{
					MappingTreeTableViewCellValue.CreateVariable => MappingTreeTableViewCellValue.MapToExistingVariable, 
					MappingTreeTableViewCellValue.MapToExistingVariable => MappingTreeTableViewCellValue.CreateVariable, 
					_ => MappingTreeTableViewCellValue.None, 
				};
			}
			return null;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}
	}
}
