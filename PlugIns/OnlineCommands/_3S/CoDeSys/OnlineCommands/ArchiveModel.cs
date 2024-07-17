using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Components;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ArchiveModel : DefaultTreeTableModel
    {
        private class Comparer : IComparer
        {
            internal static readonly IComparer Singleton = new Comparer();

            public int Compare(object x, object y)
            {
                //IL_0001: Unknown result type (might be due to invalid IL or missing references)
                //IL_000c: Unknown result type (might be due to invalid IL or missing references)
                //IL_001c: Unknown result type (might be due to invalid IL or missing references)
                //IL_0027: Unknown result type (might be due to invalid IL or missing references)
                string strA = ((IconLabelTreeTableViewCellData)((ITreeTableNode)x).GetValue(0)).Label.ToString();
                string strB = ((IconLabelTreeTableViewCellData)((ITreeTableNode)y).GetValue(0)).Label.ToString();
                return string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase);
            }
        }

        private static readonly Guid GUID_PAC_OPTIONS = new Guid("{9A035D59-E636-4a81-AF5D-B193334740A9}");

        private static readonly Guid GUID_COMPILE_INFO_ARCHIVE = new Guid("{B0B53F83-AF78-49aa-8133-0063F476BD7C}");

        internal void Initialize(int nProjectHandle, Guid[] selectedCategories)
        {
            ((DefaultTreeTableModel)this).AddColumn(string.Empty, HorizontalAlignment.Left, IconLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
            foreach (ArchiveCategoryNode item in from c in APEnvironment.ProjectArchiveCategories
                                                 select new ArchiveCategoryNode(this, TypeGuidAttribute.FromObject((object)c).Guid, c) into n
                                                 where n.CategoryGuid != GUID_PAC_OPTIONS
                                                 where n.CategoryGuid == GUID_COMPILE_INFO_ARCHIVE || n.Category.GetArchiveItemIds(nProjectHandle).Any()
                                                 select n)
            {
                item.Checked = Array.IndexOf(selectedCategories, item.CategoryGuid) >= 0;
                ((DefaultTreeTableModel)this).AddRootNode((ITreeTableNode)(object)item);
            }
            ((DefaultTreeTableModel)this).Sort(((DefaultTreeTableModel)this).Sentinel, true, Comparer.Singleton);
        }

        public ArchiveModel()
            : base()
        {
        }
    }
}
