using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Online;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ResetOriginModel : AbstractTreeTableModel
    {
        internal const int COLUMN_CHECKBOX = 0;

        internal const int COLUMN_DESCRIPTION = 1;

        internal ResetOriginModel(IList<IResetOriginConfigurationItem> items)
            : base()
        {
            base.UnderlyingModel.AddColumn("Delete", HorizontalAlignment.Left, CheckBoxTreeTableViewRenderer.CheckBoxWithReadOnly, CheckBoxTreeTableViewEditor.CheckBox, true);
            base.UnderlyingModel.AddColumn("Item", HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
            for (int i = 0; i < items.Count; i++)
            {
                ResetOriginNode resetOriginNode = new ResetOriginNode(items[i], this, i);
                base.UnderlyingModel.AddRootNode(resetOriginNode);
            }
        }
    }
}
