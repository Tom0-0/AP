using _3S.CoDeSys.Controls.Controls;
using System.Windows.Forms;

namespace _3S.CoDeSys.DeviceObject
{
    internal class DeviceObjectDiffViewModel : DefaultTreeTableModel
    {
        private bool _bIsLeftModel;

        internal bool IsLeftModel => _bIsLeftModel;

        internal DeviceObjectDiffViewModel(bool bIsLeftModel)
            : base()
        {
            _bIsLeftModel = bIsLeftModel;
            ((DefaultTreeTableModel)this).AddColumn(Strings.DiffTreetableColumnName, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)DeviceObjectDiffViewNodeRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn(Strings.DiffTreetableColumnType, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)DeviceObjectDiffViewNodeRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn(Strings.DiffTreetableColumnValue, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)DeviceObjectDiffViewNodeRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn(Strings.DiffTreetableColumnVariable, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)DeviceObjectDiffViewNodeRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn(Strings.DiffTreetableColumnAddress, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)DeviceObjectDiffViewNodeRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn(Strings.DiffTreetableColumnDescription, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)DeviceObjectDiffViewNodeRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
        }
    }
}
