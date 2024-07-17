using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    internal sealed class ChangeTreeTableModel : AbstractTreeTableModel
    {
        private bool _bEmpty = true;

        internal bool Empty => _bEmpty;

        internal ChangeTreeTableModel(IEnumerable<IChangedLMObject> changedObjects)
            : base()
        {
            UnderlyingModel.AddColumn(Strings.Col_Description, HorizontalAlignment.Left, IconLabelTreeTableViewRenderer.NormalString, IconTextBoxTreeTableViewEditor.TextBox, false);
            UnderlyingModel.AddColumn(Strings.Col_PreventsOnlineChange, HorizontalAlignment.Left, IconLabelTreeTableViewRenderer.NormalString, IconTextBoxTreeTableViewEditor.TextBox, false);
            Fill(changedObjects);
        }

        private void Fill(IEnumerable<IChangedLMObject> changedObjects)
        {
            //IL_002b: Unknown result type (might be due to invalid IL or missing references)
            //IL_006a: Unknown result type (might be due to invalid IL or missing references)
            //IL_006d: Unknown result type (might be due to invalid IL or missing references)
            //IL_00af: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b6: Unknown result type (might be due to invalid IL or missing references)
            //IL_00bd: Unknown result type (might be due to invalid IL or missing references)
            //IL_00be: Unknown result type (might be due to invalid IL or missing references)
            //IL_00bf: Unknown result type (might be due to invalid IL or missing references)
            //IL_00cd: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ce: Invalid comparison between I8 and Unknown
            //IL_00d9: Unknown result type (might be due to invalid IL or missing references)
            //IL_00da: Invalid comparison between I8 and Unknown
            //IL_00e5: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e6: Invalid comparison between I8 and Unknown
            //IL_00f0: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00fb: Unknown result type (might be due to invalid IL or missing references)
            //IL_0116: Unknown result type (might be due to invalid IL or missing references)
            //IL_0117: Unknown result type (might be due to invalid IL or missing references)
            //IL_01bd: Unknown result type (might be due to invalid IL or missing references)
            //IL_01be: Unknown result type (might be due to invalid IL or missing references)
            if (changedObjects == null)
            {
                return;
            }
            Dictionary<IChangedLMObject, object> dictionary = new Dictionary<IChangedLMObject, object>();
            foreach (IChangedLMObject changedObject in changedObjects)
            {
                if (!dictionary.ContainsKey(changedObject) && (int)changedObject.Change != 0)
                {
                    dictionary.Add(changedObject, null);
                }
            }
            List<IChangedLMObject> list = new List<IChangedLMObject>(dictionary.Keys);
            list.Sort(new ChangedLMObjectComparer());
            EPouSetChange val = (EPouSetChange)0;
            EPouSetChange val2 = (EPouSetChange)0;
            GenericRootNode genericRootNode = null;
            bool flag = false;
            string text = string.Empty;
            string text2 = string.Empty;
            PouNode pouNode = null;
            UnderlyingModel.ClearRootNodes();
            foreach (IChangedLMObject item in list)
            {
                IChangedLMObject2 val3 = (IChangedLMObject2)(object)((item is IChangedLMObject2) ? item : null);
                val2 = (EPouSetChange)((uint)item.Change & ((uint)item.Change ^ 0xFFFFFFFFu));
                flag = val3 != null && (8796093022208L == (long)val2 || 4398046511104L == (long)val2 || 17592186044416L == (long)val2);
                if (val != val2 || genericRootNode == null)
                {
                    if (flag)
                    {
                        genericRootNode = new GenericRootNode(val2);
                        text2 = item.Name;
                        if (text != text2 || val != val2 || pouNode == null)
                        {
                            pouNode = new PouNode(item, genericRootNode);
                            genericRootNode.AddChild((ITreeTableNode)(object)pouNode);
                        }
                    }
                    else
                    {
                        genericRootNode = new ChangeNode(item);
                    }
                    UnderlyingModel.AddRootNode((ITreeTableNode)(object)genericRootNode);
                    _bEmpty = false;
                }
                else if (flag)
                {
                    text2 = item.Name;
                    if (text != text2 || pouNode == null)
                    {
                        pouNode = new PouNode(item, genericRootNode);
                        genericRootNode.AddChild((ITreeTableNode)(object)pouNode);
                    }
                }
                if (!string.IsNullOrEmpty(item.Name))
                {
                    if (flag)
                    {
                        VariableNode pouNode2 = new VariableNode(val3.ChildName, (ITreeTableNode)(object)pouNode);
                        pouNode.AddChild((ITreeTableNode)(object)pouNode2);
                    }
                    else
                    {
                        PouNode pouNode3 = new PouNode(item, genericRootNode);
                        genericRootNode.AddChild((ITreeTableNode)(object)pouNode3);
                    }
                }
                val = val2;
                text = text2;
            }
        }
    }
}
