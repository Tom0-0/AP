#define DEBUG
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class ApplicationContentViewModel : DefaultTreeTableModel
    {
        private IApplicationContent _IDEContent;

        private IApplicationContent _PLCContent;

        private ICompileContext9 _comcon;

        private int _nAdditionsCount;

        private int _nDeletionsCount;

        private int _nChangesCount;

        public IApplicationContent IDEContent => _IDEContent;

        public IApplicationContent PLCContent => _PLCContent;

        internal int AdditionsCount => _nAdditionsCount;

        internal int DeletionsCount => _nDeletionsCount;

        internal int ChangesCount => _nChangesCount;

        internal ApplicationContentViewModel(IApplicationContent IDEContent, IApplicationContent PLCContent, ICompileContext9 comcon)
            : base()
        {
            if (PLCContent == null)
            {
                throw new ArgumentNullException("PLCContent");
            }
            _IDEContent = IDEContent;
            _PLCContent = PLCContent;
            _comcon = comcon;
            ((DefaultTreeTableModel)this).AddColumn("TODO: current Application", HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)ProjectDiffViewNodeObjectRenderer.NoIndent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn("Differences", HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)ProjectDiffViewNodeDiffRenderer.Singleton, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
            ((DefaultTreeTableModel)this).AddColumn("TODO: from PLC", HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)ProjectDiffViewNodeObjectRenderer.Indent, (ITreeTableViewEditor)(object)NoopTreeTableViewEditor.Singleton, false);
        }

        internal void UpdateContents(TreeTableView view)
        {
            //IL_00aa: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b4: Expected O, but got Unknown
            ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
            _nAdditionsCount = 0;
            _nDeletionsCount = 0;
            _nChangesCount = 0;
            ((DefaultTreeTableModel)this).ClearRootNodes();
            LightSVNode lightSVNode = new LightSVNode();
            LightSVNode lightSVNode2 = new LightSVNode();
            int nCount = 0;
            PopulateLightSVNodes(lightSVNode, _IDEContent, _comcon, ref nCount);
            PopulateLightSVNodes(lightSVNode2, _PLCContent, _comcon, ref nCount);
            CreateDiffTree(view, null, lightSVNode, lightSVNode2);
            ((DefaultTreeTableModel)this).Sort((ITreeTableNode)null, true, (IComparer)ProjectDiffViewNodeComparer.Singleton);
            for (int i = 0; i < ((DefaultTreeTableModel)this).Sentinel.ChildCount; i++)
            {
                ((ProjectDiffViewNode)(object)((DefaultTreeTableModel)this).Sentinel.GetChild(i)).IndexInParent = i;
            }
            ((DefaultTreeTableModel)this).RaiseStructureChanged(new TreeTableModelEventArgs((ITreeTableNode)null, -1, (ITreeTableNode)null));
        }

        internal ProjectDiffViewNode GetNextDiff(ProjectDiffViewNode currentNode)
        {
            if (currentNode == null)
            {
                throw new ArgumentNullException("currentNode");
            }
            do
            {
                currentNode = GetNextNode(currentNode);
            }
            while (currentNode != null && currentNode.DiffState == CompileDiffState.Equal);
            return currentNode;
        }

        internal ProjectDiffViewNode GetPreviousDiff(ProjectDiffViewNode currentNode)
        {
            if (currentNode == null)
            {
                throw new ArgumentNullException("currentNode");
            }
            do
            {
                currentNode = GetPreviousNode(currentNode);
            }
            while (currentNode != null && currentNode.DiffState == CompileDiffState.Equal);
            return currentNode;
        }

        private ProjectDiffViewNode GetNextNode(ProjectDiffViewNode currentNode)
        {
            Debug.Assert(currentNode != null);
            if (currentNode.ChildCount > 0)
            {
                return (ProjectDiffViewNode)(object)currentNode.GetChild(0);
            }
            ProjectDiffViewNode nextSiblingNode = GetNextSiblingNode(currentNode);
            if (nextSiblingNode != null)
            {
                return nextSiblingNode;
            }
            while (currentNode.Parent != null)
            {
                nextSiblingNode = GetNextSiblingNode((ProjectDiffViewNode)(object)currentNode.Parent);
                if (nextSiblingNode != null)
                {
                    return nextSiblingNode;
                }
                currentNode = (ProjectDiffViewNode)(object)currentNode.Parent;
            }
            return null;
        }

        private ProjectDiffViewNode GetPreviousNode(ProjectDiffViewNode currentNode)
        {
            Debug.Assert(currentNode != null);
            ProjectDiffViewNode previousSiblingNode = GetPreviousSiblingNode(currentNode);
            if (previousSiblingNode != null && previousSiblingNode.ChildCount > 0)
            {
                currentNode = (ProjectDiffViewNode)(object)previousSiblingNode.GetChild(previousSiblingNode.ChildCount - 1);
                while (currentNode.ChildCount > 0)
                {
                    currentNode = (ProjectDiffViewNode)(object)currentNode.GetChild(currentNode.ChildCount - 1);
                }
                return currentNode;
            }
            if (previousSiblingNode != null)
            {
                return previousSiblingNode;
            }
            return (ProjectDiffViewNode)(object)currentNode.Parent;
        }

        private ProjectDiffViewNode GetNextSiblingNode(ProjectDiffViewNode currentNode)
        {
            Debug.Assert(currentNode != null);
            int num = currentNode.IndexInParent + 1;
            if (currentNode.Parent == null)
            {
                if (num < ((DefaultTreeTableModel)this).Sentinel.ChildCount)
                {
                    return (ProjectDiffViewNode)(object)((DefaultTreeTableModel)this).Sentinel.GetChild(num);
                }
                return null;
            }
            if (num < currentNode.Parent.ChildCount)
            {
                return (ProjectDiffViewNode)(object)currentNode.Parent.GetChild(num);
            }
            return null;
        }

        private ProjectDiffViewNode GetPreviousSiblingNode(ProjectDiffViewNode currentNode)
        {
            Debug.Assert(currentNode != null);
            int num = currentNode.IndexInParent - 1;
            if (currentNode.Parent == null)
            {
                if (num >= 0)
                {
                    return (ProjectDiffViewNode)(object)((DefaultTreeTableModel)this).Sentinel.GetChild(num);
                }
                return null;
            }
            if (num >= 0)
            {
                return (ProjectDiffViewNode)(object)currentNode.Parent.GetChild(num);
            }
            return null;
        }

        private static void PopulateLightSVNodes(LightSVNode node, IApplicationContent appcontent, ICompileContext9 comcon, ref int nCount)
        {
            Debug.Assert(node != null);
            if (appcontent != null && comcon != null && node.CompiledElementInfo == null)
            {
                IApplicationContent2 val = (IApplicationContent2)(object)((appcontent is IApplicationContent2) ? appcontent : null);
                for (int i = 0; i < appcontent.POUs.Length; i++)
                {
                    IPOUInfoStruct val2 = appcontent.POUs[i];
                    ISignature signature = ((ICompileContextCommon)comcon).GetSignature(((ICompiledElementInfoStruct)val2).Name);
                    LightSVNode lightSVNode = new LightSVNode((ICompiledElementInfoStruct)(object)val2, appcontent, ((ICompileContextCommon)comcon).GetSignature(((ICompiledElementInfoStruct)val2).Name));
                    node.AddChild(lightSVNode);
                    nCount++;
                    nCount = AddMethodChildNodes((IMethodInfoStruct[])(object)((val != null) ? val.POUMethods?.ToArray() : null), appcontent, nCount, i, (ICompiledElementInfoStruct)(object)val2, signature, lightSVNode, (Operator)93);
                }
                for (int j = 0; j < appcontent.FBs.Length; j++)
                {
                    IFBInfoStruct val3 = appcontent.FBs[j];
                    ISignature signature2 = ((ICompileContextCommon)comcon).GetSignature(((ICompiledElementInfoStruct)val3).Name);
                    LightSVNode lightSVNode2 = new LightSVNode((ICompiledElementInfoStruct)(object)val3, appcontent, signature2);
                    node.AddChild(lightSVNode2);
                    nCount++;
                    nCount = AddMethodChildNodes(appcontent.Methods.ToArray(), appcontent, nCount, j, (ICompiledElementInfoStruct)(object)val3, signature2, lightSVNode2, (Operator)0);
                }
                IDUTInfoStruct[] dUTs = appcontent.DUTs;
                foreach (IDUTInfoStruct val4 in dUTs)
                {
                    LightSVNode node2 = new LightSVNode((ICompiledElementInfoStruct)(object)val4, appcontent, ((ICompileContextCommon)comcon).GetSignature(((ICompiledElementInfoStruct)val4).Name));
                    node.AddChild(node2);
                    nCount++;
                }
                for (int l = 0; l < appcontent.GVLs.Length; l++)
                {
                    IGVLInfoStruct val5 = appcontent.GVLs[l];
                    ISignature signature3 = ((ICompileContextCommon)comcon).GetSignature(((ICompiledElementInfoStruct)val5).Name);
                    LightSVNode lightSVNode3 = new LightSVNode((ICompiledElementInfoStruct)(object)val5, appcontent, ((ICompileContextCommon)comcon).GetSignature(((ICompiledElementInfoStruct)val5).Name));
                    node.AddChild(lightSVNode3);
                    nCount++;
                    nCount = AddMethodChildNodes((IMethodInfoStruct[])(object)((val != null) ? val.POUMethods?.ToArray() : null), appcontent, nCount, l, (ICompiledElementInfoStruct)(object)val5, signature3, lightSVNode3, (Operator)109);
                }
                string[] libs = appcontent.Libs;
                for (int k = 0; k < libs.Length; k++)
                {
                    LightSVNode node3 = new LightSVNode(libs[k]);
                    node.AddChild(node3);
                    nCount++;
                }
            }
        }

        private static int AddMethodChildNodes(IMethodInfoStruct[] allMethods, IApplicationContent appcontent, int nCount, int i, ICompiledElementInfoStruct ceis, ISignature sign, LightSVNode childNode, Operator eRequiredParentPOUType = 0)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            if (allMethods != null)
            {
                Dictionary<string, LightSVNode> dictionary = new Dictionary<string, LightSVNode>();
                foreach (IMethodInfoStruct val in allMethods)
                {
                    bool flag = true;
                    IPOUMethodInfoStruct val2 = (IPOUMethodInfoStruct)(object)((val is IPOUMethodInfoStruct) ? val : null);
                    if ((int)eRequiredParentPOUType != 0 && val2 != null)
                    {
                        flag = val2.ParentPOUType == eRequiredParentPOUType;
                    }
                    if (!(val.ParentPOUIndex == i && flag))
                    {
                        continue;
                    }
                    LightSVNode lightSVNode = childNode;
                    ISignature sign2 = ((sign != null) ? sign.GetSubSignature(((ICompiledElementInfoStruct)val).Name) : null);
                    string stPropertyName = null;
                    bool bGetter = false;
                    if (ProjectDiffViewNode.IsPropertyAccessor((ICompiledElementInfoStruct)(object)val, out stPropertyName, out bGetter))
                    {
                        LightSVNode value = null;
                        if (!dictionary.TryGetValue(stPropertyName, out value))
                        {
                            value = new LightSVNode(ceis.Name, stPropertyName);
                            dictionary.Add(stPropertyName, value);
                            childNode.AddChild(value);
                        }
                        lightSVNode = value;
                    }
                    LightSVNode node = new LightSVNode((ICompiledElementInfoStruct)(object)val, appcontent, sign2);
                    lightSVNode.AddChild(node);
                    nCount++;
                }
            }
            return nCount;
        }

        private CompileDiffState CreateDiffTree(TreeTableView editor, ProjectDiffViewNode parentNode, LightSVNode left, LightSVNode right)
        {
            Debug.Assert(left != null && right != null);
            CompileDiffState compileDiffState = CompileDiffState.Equal;
            foreach (LightSVNode item in left)
            {
                LightSVNode child = right.GetChild(item);
                ProjectDiffViewNode projectDiffViewNode;
                if (child != null)
                {
                    CompileDiffState compileDiffState2 = CompareObjects(item, child);
                    projectDiffViewNode = new ProjectDiffViewNode(this, parentNode, item.CompiledElementInfo, child.CompiledElementInfo, item.Signature, compileDiffState2, parentNode?.ChildCount ?? ((DefaultTreeTableModel)this).Sentinel.ChildCount, item.ComparableId);
                    _nChangesCount += ((compileDiffState2 != 0) ? 1 : 0);
                    compileDiffState |= compileDiffState2;
                    if (CreateDiffTree(editor, projectDiffViewNode, item, child) != 0 && item.CompiledElementInfo is PropertyElementInfo)
                    {
                        projectDiffViewNode.DiffState = CompileDiffState.PropertyAccessorChanged;
                    }
                }
                else
                {
                    projectDiffViewNode = new ProjectDiffViewNode(this, parentNode, item.CompiledElementInfo, null, item.Signature, CompileDiffState.Added, parentNode?.ChildCount ?? ((DefaultTreeTableModel)this).Sentinel.ChildCount, item.ComparableId);
                    _nAdditionsCount++;
                    TreeTableViewNode viewNode = editor.GetViewNode((ITreeTableNode)(object)projectDiffViewNode);
                    if (viewNode != null)
                    {
                        viewNode.EnsureVisible(0);
                    }
                    MarkSubtree(projectDiffViewNode, item, CompileDiffState.Added);
                    compileDiffState |= CompileDiffState.Added;
                }
                if (parentNode == null)
                {
                    ((DefaultTreeTableModel)this).AddRootNode((ITreeTableNode)(object)projectDiffViewNode);
                }
                else
                {
                    parentNode.AddChildNode(projectDiffViewNode);
                }
            }
            foreach (LightSVNode item2 in right)
            {
                if (left.GetChild(item2) == null)
                {
                    ProjectDiffViewNode projectDiffViewNode = new ProjectDiffViewNode(this, parentNode, null, item2.CompiledElementInfo, item2.Signature, CompileDiffState.Deleted, parentNode?.ChildCount ?? ((DefaultTreeTableModel)this).Sentinel.ChildCount, item2.ComparableId);
                    _nDeletionsCount++;
                    MarkSubtree(projectDiffViewNode, item2, CompileDiffState.Deleted);
                    if (parentNode == null)
                    {
                        ((DefaultTreeTableModel)this).AddRootNode((ITreeTableNode)(object)projectDiffViewNode);
                    }
                    else
                    {
                        parentNode.AddChildNode(projectDiffViewNode);
                    }
                }
            }
            return compileDiffState;
        }

        private void MarkSubtree(ProjectDiffViewNode parentNode, LightSVNode node, CompileDiffState diffState)
        {
            foreach (LightSVNode item in node)
            {
                ProjectDiffViewNode projectDiffViewNode = new ProjectDiffViewNode(this, parentNode, (diffState == CompileDiffState.Added) ? item.CompiledElementInfo : null, (diffState == CompileDiffState.Deleted) ? item.CompiledElementInfo : null, item.Signature, diffState, parentNode?.ChildCount ?? ((DefaultTreeTableModel)this).Sentinel.ChildCount, item.ComparableId);
                MarkSubtree(projectDiffViewNode, item, diffState);
                if (parentNode == null)
                {
                    ((DefaultTreeTableModel)this).AddRootNode((ITreeTableNode)(object)projectDiffViewNode);
                }
                else
                {
                    parentNode.AddChildNode(projectDiffViewNode);
                }
                if (diffState == CompileDiffState.Added)
                {
                    _nAdditionsCount++;
                }
                if (diffState == CompileDiffState.Deleted)
                {
                    _nDeletionsCount++;
                }
            }
        }

        private CompileDiffState CompareObjects(LightSVNode leftChild, LightSVNode rightChild)
        {
            ICompiledElementInfoStruct compiledElementInfo = leftChild.CompiledElementInfo;
            ICompiledElementInfoStruct compiledElementInfo2 = rightChild.CompiledElementInfo;
            CompileDiffState compileDiffState = CompileDiffState.Equal;
            if (compiledElementInfo is IPOUInfoStruct && compiledElementInfo2 is IPOUInfoStruct)
            {
                ICompiledElementInfoStruct obj = ((compiledElementInfo is IPOUInfoStruct) ? compiledElementInfo : null);
                IPOUInfoStruct val = (IPOUInfoStruct)(object)((compiledElementInfo2 is IPOUInfoStruct) ? compiledElementInfo2 : null);
                if (((IPOUInfoStruct)obj).CRCCode != val.CRCCode)
                {
                    compileDiffState |= CompileDiffState.CodeDifferent;
                }
                if (((IPOUInfoStruct)obj).CRCInterface != val.CRCInterface)
                {
                    compileDiffState |= CompileDiffState.InterfaceDifferent;
                }
                if (((IPOUInfoStruct)obj).AreaCodeLocation != val.AreaCodeLocation)
                {
                    compileDiffState |= CompileDiffState.LocationDifferent;
                }
                if (compiledElementInfo is IFBInfoStruct && compiledElementInfo2 is IFBInfoStruct)
                {
                    ICompiledElementInfoStruct obj2 = ((compiledElementInfo is IFBInfoStruct) ? compiledElementInfo : null);
                    IFBInfoStruct val2 = (IFBInfoStruct)(object)((compiledElementInfo2 is IFBInfoStruct) ? compiledElementInfo2 : null);
                    if (((IFBInfoStruct)obj2).CRCVFTable != val2.CRCVFTable)
                    {
                        compileDiffState |= CompileDiffState.VFTableDifferent;
                    }
                }
            }
            else if (compiledElementInfo is IDUTInfoStruct && compiledElementInfo2 is IDUTInfoStruct)
            {
                ICompiledElementInfoStruct obj3 = ((compiledElementInfo is IDUTInfoStruct) ? compiledElementInfo : null);
                IDUTInfoStruct val3 = (IDUTInfoStruct)(object)((compiledElementInfo2 is IDUTInfoStruct) ? compiledElementInfo2 : null);
                if (((IDUTInfoStruct)obj3).CRCInterface != val3.CRCInterface)
                {
                    compileDiffState |= CompileDiffState.InterfaceDifferent;
                }
            }
            else if (compiledElementInfo is IGVLInfoStruct && compiledElementInfo2 is IGVLInfoStruct)
            {
                ICompiledElementInfoStruct obj4 = ((compiledElementInfo is IGVLInfoStruct) ? compiledElementInfo : null);
                IGVLInfoStruct val4 = (IGVLInfoStruct)(object)((compiledElementInfo2 is IGVLInfoStruct) ? compiledElementInfo2 : null);
                if (((IGVLInfoStruct)obj4).CRCInterface != val4.CRCInterface)
                {
                    compileDiffState |= CompileDiffState.InterfaceDifferent;
                }
            }
            else if (compiledElementInfo is PropertyElementInfo && compiledElementInfo2 is PropertyElementInfo)
            {
                if (leftChild.ChildCount > rightChild.ChildCount)
                {
                    compileDiffState |= CompileDiffState.Added;
                }
                else if (leftChild.ChildCount < rightChild.ChildCount)
                {
                    compileDiffState |= CompileDiffState.Deleted;
                }
            }
            return compileDiffState;
        }
    }
}
