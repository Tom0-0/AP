using _3S.CoDeSys.Core.LanguageModel;
using System.Collections;
using System.Collections.Generic;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class LightSVNode : IEnumerable<LightSVNode>, IEnumerable
    {
        private string _stComparableId;

        private Dictionary<string, LightSVNode> _children;

        private ICompiledElementInfoStruct _ceinfo;

        private ISignature _sign;

        internal ICompiledElementInfoStruct CompiledElementInfo => _ceinfo;

        internal string ComparableId => _stComparableId;

        internal ISignature Signature => _sign;

        internal int ChildCount
        {
            get
            {
                if (_children == null)
                {
                    return 0;
                }
                return _children.Count;
            }
        }

        internal LightSVNode()
        {
            _stComparableId = string.Empty;
            _ceinfo = null;
            _sign = null;
        }

        internal LightSVNode(ICompiledElementInfoStruct ceinfo, IApplicationContent appcontent, ISignature sign)
        {
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0032: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_0036: Invalid comparison between Unknown and I4
            //IL_0038: Unknown result type (might be due to invalid IL or missing references)
            //IL_003b: Invalid comparison between Unknown and I4
            _ceinfo = ceinfo;
            string fullName = ((object)ceinfo).GetType().FullName;
            _sign = sign;
            IPOUMethodInfoStruct val = (IPOUMethodInfoStruct)(object)((ceinfo is IPOUMethodInfoStruct) ? ceinfo : null);
            if (val != null)
            {
                string text = null;
                Operator parentPOUType = val.ParentPOUType;
                _stComparableId = $"{(((int)parentPOUType == 93) ? ((ICompiledElementInfoStruct)appcontent.POUs[((IMethodInfoStruct)val).ParentPOUIndex]).Name : (((int)parentPOUType != 109) ? string.Empty : ((ICompiledElementInfoStruct)appcontent.GVLs[((IMethodInfoStruct)val).ParentPOUIndex]).Name))}.{ceinfo.Name}: {fullName}";
            }
            else if (ceinfo is IMethodInfoStruct)
            {
                string name = ((ICompiledElementInfoStruct)appcontent.FBs[((IMethodInfoStruct)((ceinfo is IMethodInfoStruct) ? ceinfo : null)).ParentPOUIndex]).Name;
                _stComparableId = $"{name}.{ceinfo.Name}: {fullName}";
            }
            _stComparableId = $"{ceinfo.Name}: {fullName}";
        }

        internal LightSVNode(string stLibraryId)
        {
            _ceinfo = (ICompiledElementInfoStruct)(object)new LibraryElementInfo(stLibraryId);
            _stComparableId = "@@LibraryInfo:" + stLibraryId;
        }

        internal LightSVNode(string stParentPOU, string stProperty)
        {
            _ceinfo = (ICompiledElementInfoStruct)(object)new PropertyElementInfo(stProperty);
            _stComparableId = stParentPOU + ":" + stProperty;
        }

        internal void AddChild(LightSVNode node)
        {
            if (_children == null)
            {
                _children = new Dictionary<string, LightSVNode>();
            }
            _children[node._stComparableId] = node;
        }

        internal LightSVNode GetChild(LightSVNode node)
        {
            if (_children == null)
            {
                return null;
            }
            _children.TryGetValue(node._stComparableId, out var value);
            return value;
        }

        public IEnumerator<LightSVNode> GetEnumerator()
        {
            if (_children == null)
            {
                return new List<LightSVNode>().GetEnumerator();
            }
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
