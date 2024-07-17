using _3S.CoDeSys.Controls.Controls;
using System;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class MultipleDownloadResultModel : AbstractTreeTableModel
    {
        internal MultipleDownloadResultModel(MultipleDownloadResult[] results)
            : base()
        {
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            UnderlyingModel.AddColumn(string.Empty, HorizontalAlignment.Left, IconLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
            UnderlyingModel.AddColumn(string.Empty, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
            for (int i = 0; i < results.Length; i++)
            {
                MultipleDownloadResultNode multipleDownloadResultNode = new MultipleDownloadResultNode(results[i]);
                UnderlyingModel.AddRootNode((ITreeTableNode)(object)multipleDownloadResultNode);
            }
        }
    }
}
