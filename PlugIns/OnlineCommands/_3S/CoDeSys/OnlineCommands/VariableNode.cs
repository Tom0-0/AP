using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal sealed class VariableNode : ITreeTableNode
    {
        private const int COL_VARNAME = 0;

        private const int COL_PREVENTS_ONLINECHANGE = 1;

        private static Bitmap BMP_VARIABLE;

        private string _stVariableName;

        private ITreeTableNode _parentNode;

        public bool HasChildren => false;

        public int ChildCount => 0;

        public ITreeTableNode Parent => _parentNode;

        static VariableNode()
        {
            BMP_VARIABLE = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(VariableNode), "_3S.CoDeSys.OnlineCommands.Resources.VarSmall.ico").Handle);
        }

        internal VariableNode(string stVariableName, ITreeTableNode parentNode)
        {
            _stVariableName = stVariableName;
            _parentNode = parentNode;
        }

        public object GetValue(int nColumnIndex)
        {
            //IL_0014: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Expected O, but got Unknown
            return nColumnIndex switch
            {
                0 => (object)new IconLabelTreeTableViewCellData((Image)BMP_VARIABLE, (object)_stVariableName),
                1 => string.Empty,
                _ => throw new ArgumentOutOfRangeException("nColumnIndex"),
            };
        }

        public void SetValue(int nColumnIndex, object value)
        {
        }

        public int GetIndex(ITreeTableNode node)
        {
            return -1;
        }

        public void SwapChildren(int nIndex1, int nIndex2)
        {
        }

        public ITreeTableNode GetChild(int nIndex)
        {
            return null;
        }

        public bool IsEditable(int nColumnIndex)
        {
            return false;
        }
    }
}
