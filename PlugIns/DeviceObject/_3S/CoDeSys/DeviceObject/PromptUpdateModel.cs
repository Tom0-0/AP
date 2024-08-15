using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class PromptUpdateModel : AbstractTreeTableModel
	{
		internal const int COLIDX_DEVICE = 0;

		internal const int COLIDX_CURRENTVERSION = 1;

		internal const int COLIDX_RECOMMENDED_VERSION = 2;

		internal const int COLIDX_CONFIG_VERSION = 3;

		internal const int COLIDX_UPDATEACTION = 4;

		internal PromptUpdateModel(LList<UpdateInformation> updateList)
			: base()
		{
			UnderlyingModel.AddColumn(Strings.Device, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Strings.CurrentVersion, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Strings.RecommendedVersion, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Strings.ColumnConfigVersion, HorizontalAlignment.Center, CheckBoxTreeTableViewRenderer.CheckBoxWithReadOnly, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Strings.UpdateAction, HorizontalAlignment.Left, GenericCellTreeTableViewRenderer.Generic, GenericCellTreeTableViewEditor.Prompt, true);
			if (updateList != null && updateList.Count > 0)
			{
				foreach (UpdateInformation update in updateList)
				{
					UnderlyingModel.AddRootNode((ITreeTableNode)(object)new PromptUpdateNode(this, update));
				}
			}
			else
			{
				UnderlyingModel.AddRootNode((ITreeTableNode)(object)new PromptUpdateDummyNode());
			}
		}
	}
}
