using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal sealed class PouNode : ITreeTableNode
    {
        private const int COL_POUNAME = 0;

        private const int COL_PREVENTS_ONLINECHANGE = 1;

        private static Dictionary<Operator, Bitmap> s_htBitmaps;

        private static Bitmap BMP_DEFAULT;

        private IChangedLMObject _changedLMObject;

        private GenericRootNode _parentNode;

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

        public ITreeTableNode Parent => (ITreeTableNode)(object)_parentNode;

        static PouNode()
        {
            s_htBitmaps = new Dictionary<Operator, Bitmap>();
            Bitmap value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.STPOUSmall.ico").Handle);
            s_htBitmaps.Add((Operator)93, value);
            s_htBitmaps.Add((Operator)87, value);
            s_htBitmaps.Add((Operator)88, value);
            value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.InterfaceSmall.ico").Handle);
            s_htBitmaps.Add((Operator)119, value);
            value = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.GVLSmall.ico").Handle);
            s_htBitmaps.Add((Operator)109, value);
            s_htBitmaps.Add((Operator)106, value);
            s_htBitmaps.Add((Operator)107, value);
            BMP_DEFAULT = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ChangeNode), "_3S.CoDeSys.OnlineCommands.Resources.POUSmall.ico").Handle);
        }

        internal PouNode(IChangedLMObject changedLMObject, GenericRootNode parentNode)
        {
            _changedLMObject = changedLMObject;
            _parentNode = parentNode;
        }

        internal void AddChild(ITreeTableNode pouNode)
        {
            if (_lstChildren == null)
            {
                _lstChildren = new List<ITreeTableNode>();
            }
            _lstChildren.Add(pouNode);
        }

        public object GetValue(int nColumnIndex)
        {
            //IL_001a: Unknown result type (might be due to invalid IL or missing references)
            //IL_003a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0040: Expected O, but got Unknown
            switch (nColumnIndex)
            {
                case 0:
                    {
                        Bitmap value = BMP_DEFAULT;
                        if (!s_htBitmaps.TryGetValue(_changedLMObject.POUType, out value))
                        {
                            value = BMP_DEFAULT;
                        }
                        return (object)new IconLabelTreeTableViewCellData((Image)value, (object)_changedLMObject.Name);
                    }
                case 1:
                    return string.Empty;
                default:
                    throw new ArgumentOutOfRangeException("nColumnIndex");
            }
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
    }
}
