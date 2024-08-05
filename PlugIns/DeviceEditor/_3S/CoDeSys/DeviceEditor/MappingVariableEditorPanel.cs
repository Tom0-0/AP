using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MappingVariableEditorPanel : Panel
	{
		private IconLabelTreeTableViewCellData _cellData;

		public IconLabelTreeTableViewCellData CellData
		{
			get
			{
				return _cellData;
			}
			set
			{
				_cellData = value;
			}
		}
	}
}
