#define DEBUG
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.ProjectArchive;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ArchiveCategoryNode : ITreeTableNode
    {
        private ArchiveModel _model;

        private Guid _categoryGuid;

        private IProjectArchiveCategory _category;

        private bool _bChecked;

        private static Image IMAGE_CHECKED = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.Checked.ico").Handle);

        private static Image IMAGE_UNCHECKED = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.Unchecked.ico").Handle);

        internal bool Checked
        {
            get
            {
                return _bChecked;
            }
            set
            {
                //IL_002a: Unknown result type (might be due to invalid IL or missing references)
                //IL_0034: Expected O, but got Unknown
                _bChecked = value;
                ((DefaultTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs(((DefaultTreeTableModel)_model).Sentinel, ((DefaultTreeTableModel)_model).Sentinel.GetIndex((ITreeTableNode)(object)this), (ITreeTableNode)(object)this));
            }
        }

        internal Guid CategoryGuid => _categoryGuid;

        internal IProjectArchiveCategory Category => _category;

        public int ChildCount => 0;

        public bool HasChildren => ChildCount > 0;

        public ITreeTableNode Parent => null;

        internal ArchiveCategoryNode(ArchiveModel model, Guid categoryGuid, IProjectArchiveCategory category)
        {
            Debug.Assert(model != null);
            Debug.Assert(category != null);
            _model = model;
            _categoryGuid = categoryGuid;
            _category = category;
        }

        public ITreeTableNode GetChild(int nIndex)
        {
            throw new ArgumentOutOfRangeException("nIndex");
        }

        public int GetIndex(ITreeTableNode node)
        {
            return -1;
        }

        public object GetValue(int nColumnIndex)
        {
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            //IL_0028: Expected O, but got Unknown
            if (nColumnIndex == 0)
            {
                return (object)new IconLabelTreeTableViewCellData(_bChecked ? IMAGE_CHECKED : IMAGE_UNCHECKED, (object)_category.Name);
            }
            throw new ArgumentOutOfRangeException("nColumnIndex");
        }

        public bool IsEditable(int nColumnIndex)
        {
            return false;
        }

        public void SetValue(int nColumnIndex, object value)
        {
            throw new InvalidOperationException("This node is read-only.");
        }

        public void SwapChildren(int nIndex1, int nIndex2)
        {
            throw new ArgumentOutOfRangeException("nIndex1");
        }
    }
}
