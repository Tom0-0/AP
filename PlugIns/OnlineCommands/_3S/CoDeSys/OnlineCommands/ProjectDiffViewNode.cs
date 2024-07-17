#define DEBUG
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ProjectDiffViewNode : ITreeTableNode, IComparable<ProjectDiffViewNode>
    {
        private ApplicationContentViewModel _model;

        private ProjectDiffViewNode _parent;

        private ICompiledElementInfoStruct _leftElement;

        private ICompiledElementInfoStruct _rightElement;

        private CompileDiffState _diffState;

        private ISignature _sign;

        private List<ProjectDiffViewNode> _childNodes = new List<ProjectDiffViewNode>();

        private int _nIndexInParent;

        private string _stSortKey;

        private static Image IMAGE_POU = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.POUSmall.ico").Handle);

        private static Image IMAGE_GVL = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.GVLSmall.ico").Handle);

        private static Image IMAGE_DUT = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.DUTSmall.ico").Handle);

        private static Image IMAGE_LIB = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.LibManObjectSmall.ico").Handle);

        private static Image IMAGE_PROP = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(ArchiveCategoryNode), "_3S.CoDeSys.OnlineCommands.Resources.PropertySmall.ico").Handle);

        internal int IndexInParent
        {
            get
            {
                return _nIndexInParent;
            }
            set
            {
                _nIndexInParent = value;
            }
        }

        internal CompileDiffState DiffState
        {
            get
            {
                return _diffState;
            }
            set
            {
                _diffState = value;
            }
        }

        internal ICompiledElementInfoStruct LeftElement
        {
            get
            {
                return _leftElement;
            }
            set
            {
                _leftElement = value;
            }
        }

        internal ICompiledElementInfoStruct RightElement
        {
            get
            {
                return _rightElement;
            }
            set
            {
                _rightElement = value;
            }
        }

        public int ChildCount => _childNodes.Count;

        public bool HasChildren => _childNodes.Count > 0;

        public ITreeTableNode Parent => (ITreeTableNode)(object)_parent;

        internal ProjectDiffViewNode(ApplicationContentViewModel model, ProjectDiffViewNode parent, ICompiledElementInfoStruct leftElement, ICompiledElementInfoStruct rightElement, ISignature sign, CompileDiffState diffState, int nIndexInParent, string stSortKey)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            _model = model;
            _parent = parent;
            _leftElement = leftElement;
            _rightElement = rightElement;
            _sign = sign;
            _diffState = diffState;
            _nIndexInParent = nIndexInParent;
            _stSortKey = stSortKey;
        }

        private static bool CheckMessageGuidUnique(ISignature sign)
        {
            //IL_0039: Unknown result type (might be due to invalid IL or missing references)
            //IL_003f: Expected O, but got Unknown
            if (sign.ObjectGuid != sign.MessageGuid && sign.MessageGuid != Guid.Empty)
            {
                Guid guid = Guid.Empty;
                IVariable[] all = sign.All;
                for (int i = 0; i < all.Length; i++)
                {
                    IVariable5 val = (IVariable5)all[i];
                    if (!(val.MessageGuid == Guid.Empty))
                    {
                        if (guid == Guid.Empty)
                        {
                            guid = val.MessageGuid;
                        }
                        else if (val.MessageGuid != guid)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        internal static string GetElementDisplayName(ICompiledElementInfoStruct ceis, ISignature sign)
        {
            if (sign != null && sign.MessageGuid != sign.ObjectGuid && CheckMessageGuidUnique(sign) && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, sign.MessageGuid))
            {
                return ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, sign.MessageGuid).Name;
            }
            if (ceis != null)
            {
                string stPropertyName = null;
                bool bGetter = false;
                if (IsPropertyAccessor(ceis, out stPropertyName, out bGetter))
                {
                    if (bGetter)
                    {
                        return "Get";
                    }
                    return "Set";
                }
                return ceis.Name;
            }
            return string.Empty;
        }

        internal void AddChildNode(ProjectDiffViewNode childNode)
        {
            if (childNode == null)
            {
                throw new ArgumentNullException("childNode");
            }
            _childNodes.Add(childNode);
            childNode._nIndexInParent = _childNodes.Count - 1;
        }

        public ITreeTableNode GetChild(int nIndex)
        {
            return (ITreeTableNode)(object)_childNodes[nIndex];
        }

        public int GetIndex(ITreeTableNode node)
        {
            if (node is ProjectDiffViewNode)
            {
                return ((ProjectDiffViewNode)(object)node)._nIndexInParent;
            }
            return -1;
        }

        public object GetValue(int nColumnIndex)
        {
            Image idummy = IMAGE_POU;
            if (_leftElement is IDUTInfoStruct)
            {
                idummy = IMAGE_DUT;
            }
            if (_leftElement is IGVLInfoStruct)
            {
                idummy = IMAGE_GVL;
            }
            if (_leftElement is LibraryElementInfo)
            {
                idummy = IMAGE_LIB;
            }
            if (_leftElement is PropertyElementInfo)
            {
                idummy = IMAGE_PROP;
            }
            string elementDisplayName = GetElementDisplayName(_leftElement, _sign);
            string elementDisplayName2 = GetElementDisplayName(_rightElement, _sign);
            return nColumnIndex switch
            {
                0 => CreateProjectDiffViewNodeObjectData(idummy, elementDisplayName, elementDisplayName, string.Empty),
                1 => "test",
                2 => CreateProjectDiffViewNodeObjectData(idummy, elementDisplayName2, string.Empty, elementDisplayName2),
                _ => throw new ArgumentOutOfRangeException("nColumnIndex"),
            };
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
            ProjectDiffViewNode value = _childNodes[nIndex1];
            _childNodes[nIndex1] = _childNodes[nIndex2];
            _childNodes[nIndex2] = value;
            _childNodes[nIndex1]._nIndexInParent = nIndex1;
            _childNodes[nIndex2]._nIndexInParent = nIndex2;
        }

        public int CompareTo(ProjectDiffViewNode other)
        {
            return string.Compare(_stSortKey, other._stSortKey, StringComparison.OrdinalIgnoreCase);
        }

        private ProjectDiffViewNodeObjectData CreateProjectDiffViewNodeObjectData(Image idummy, string stNameCommon, string stNameAdded, string stNameDeleted)
        {
            if (_diffState == CompileDiffState.Equal)
            {
                return new ProjectDiffViewNodeObjectData(idummy, stNameCommon, Color.Black, Color.White, FontStyle.Regular);
            }
            if (_diffState == CompileDiffState.Added)
            {
                return new ProjectDiffViewNodeObjectData(idummy, stNameAdded, Color.Green, Color.LightGray, FontStyle.Bold);
            }
            if (_diffState == CompileDiffState.Deleted)
            {
                return new ProjectDiffViewNodeObjectData(idummy, stNameDeleted, Color.Blue, Color.LightGray, FontStyle.Bold);
            }
            if ((_diffState & CompileDiffState.AnythingDifferent) != 0)
            {
                return new ProjectDiffViewNodeObjectData(fontStyle: (_diffState == CompileDiffState.CodeDifferent || _diffState == CompileDiffState.InterfaceDifferent) ? FontStyle.Bold : FontStyle.Regular, image: idummy, stName: stNameCommon, foreColor: Color.Red, backColor: Color.LightGray);
            }
            Debug.Fail("Unknown diffstate");
            return null;
        }

        internal static bool IsPropertyAccessor(ICompiledElementInfoStruct ceis, out string stPropertyName, out bool bGetter)
        {
            stPropertyName = null;
            bGetter = false;
            bool flag = false;
            IMethodInfoStruct val = (IMethodInfoStruct)(object)((ceis is IMethodInfoStruct) ? ceis : null);
            if (val != null)
            {
                string value = string.Empty;
                if (5 <= ((ICompiledElementInfoStruct)val).Name.Length)
                {
                    value = ((ICompiledElementInfoStruct)val).Name.Substring(0, 5);
                }
                bool flag2 = "__get".Equals(value);
                bool flag3 = "__set".Equals(value);
                flag = flag2 || flag3;
                if (flag)
                {
                    bGetter = flag2;
                    stPropertyName = ((ICompiledElementInfoStruct)val).Name.Substring(5);
                }
            }
            return flag;
        }
    }
}
