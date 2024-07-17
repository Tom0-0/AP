using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ResetOriginNode : ITreeTableNode
    {
        private IResetOriginConfigurationItem _item;

        private ResetOriginModel _model;

        private int _index;

        public int ChildCount => 0;

        public bool HasChildren => false;

        public ITreeTableNode Parent => null;

        internal ResetOriginNode(IResetOriginConfigurationItem item, ResetOriginModel model, int index)
        {
            _item = item;
            _model = model;
            _index = index;
        }

        public ITreeTableNode GetChild(int nIndex)
        {
            return null;
        }

        public int GetIndex(ITreeTableNode node)
        {
            return 0;
        }

        public object GetValue(int nColumnIndex)
        {
            return nColumnIndex switch
            {
                0 => _item.Delete,
                1 => _item.Description,
                _ => null,
            };
        }

        public bool IsEditable(int nColumnIndex)
        {
            if (nColumnIndex == 0)
            {
                return _item.CanDelete;
            }
            return false;
        }

        public void SetValue(int nColumnIndex, object value)
        {
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            //IL_0031: Expected O, but got Unknown
            if (value is bool)
            {
                _item.Delete = ((bool)value);
                ((AbstractTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)null, _index, (ITreeTableNode)(object)this));
            }
        }

        public void SwapChildren(int nIndex1, int nIndex2)
        {
        }
    }
}
