using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.OnlineUI;
using System;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class InstanceTreeTableModel : DefaultTreeTableModel
    {
        private string _findWhat;

        public InstanceTreeTableModel(string[] instancePaths, string stApplication, Guid applicationGuid, string stObjectName, IInstanceFormatter formatter)
            : base()
        {
            if (instancePaths == null)
            {
                throw new ArgumentNullException("instancePaths");
            }
            if (stApplication == null)
            {
                throw new ArgumentNullException("stApplication");
            }
            if (stObjectName == null)
            {
                throw new ArgumentNullException("stObjectName");
            }
            ((DefaultTreeTableModel)this).AddColumn(string.Empty, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new FindResultsNodeRenderer(() => _findWhat), TextBoxTreeTableViewEditor.TextBox, false);
            foreach (string text in instancePaths)
            {
                if (text.StartsWith(stApplication + "."))
                {
                    string text2 = text.Substring(stApplication.Length + 1);
                    //_findWhat = filter;
                    if (string.Compare(text2, stObjectName, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        InstanceTreeTableNode instanceTreeTableNode = new InstanceTreeTableNode(applicationGuid, stApplication, text2, formatter);
                        ((DefaultTreeTableModel)this).AddRootNode((ITreeTableNode)(object)instanceTreeTableNode);
                    }
                }
            }
        }
    }
}
