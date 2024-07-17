using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class GenericRootNode : ITreeTableNode
    {
        private const int COL_DESCRIPTION = 0;

        private const int COL_PREVENTS_ONLINECHANGE = 1;

        private static Bitmap BMP_DEFAULT;

        protected EPouSetChange _eReason;

        private List<ITreeTableNode> _lstChildren;

        public bool HasChildren => ChildCount != 0;

        public int ChildCount
        {
            get
            {
                if (_lstChildren == null)
                {
                    return 0;
                }
                return _lstChildren.Count;
            }
        }

        public ITreeTableNode Parent => null;

        static GenericRootNode()
        {
            BMP_DEFAULT = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.Textual.ico").Handle);
        }

        internal GenericRootNode(EPouSetChange eReason)
        {
            //IL_0007: Unknown result type (might be due to invalid IL or missing references)
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            _eReason = eReason;
        }

        internal void AddChild(ITreeTableNode pouNode)
        {
            if (_lstChildren == null)
            {
                _lstChildren = new List<ITreeTableNode>();
            }
            _lstChildren.Add(pouNode);
        }

        public virtual object GetValue(int nColumnIndex)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0020: Expected O, but got Unknown
            return nColumnIndex switch
            {
                0 => (object)new IconLabelTreeTableViewCellData((Image)BMP_DEFAULT, (object)GetLocalizedReason(_eReason)),
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
            ITreeTableNode result = null;
            if (HasChildren && 0 <= nIndex && nIndex < _lstChildren.Count)
            {
                result = _lstChildren[nIndex];
            }
            return result;
        }

        public bool IsEditable(int nColumnIndex)
        {
            return false;
        }

        private string GetLocalizedReason(EPouSetChange eReason)
        {
            //IL_0002: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Invalid comparison between Unknown and I8
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0018: Invalid comparison between Unknown and I8
            //IL_001a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0024: Invalid comparison between Unknown and I8
            string text = null;
            if ((long)eReason != 4398046511104L)
            {
                if ((long)eReason != 8796093022208L)
                {
                    if ((long)eReason == 17592186044416L)
                    {
                        return Strings.Change_VariableDeleted;
                    }
                    return eReason.ToString();
                }
                return Strings.Change_VariableChanged;
            }
            return Strings.Change_VariableInserted;
        }
    }
}
