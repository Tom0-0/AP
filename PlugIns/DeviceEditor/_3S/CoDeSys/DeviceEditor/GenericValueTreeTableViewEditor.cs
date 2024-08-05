using System;
using System.Collections;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class GenericValueTreeTableViewEditor : ITreeTableViewEditor
	{
		private bool _bAllowNullValues;

		public GenericValueTreeTableViewEditor(bool bAllowNullValues)
		{
			_bAllowNullValues = bAllowNullValues;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			ITreeTableViewEditor editor = GetEditor(node);
			if (editor != null)
			{
				object obj;
				if (node == null)
				{
					obj = null;
				}
				else
				{
					TreeTableView view = node.View;
					obj = ((view != null) ? view.Model : null);
				}
				if (nColumnIndex == (obj as ParameterTreeTableModel)?.GetIndexOfColumn(7))
				{
					object obj2;
					if (node == null)
					{
						obj2 = null;
					}
					else
					{
						TreeTableView view2 = node.View;
						obj2 = ((view2 != null) ? view2.GetModelNode(node) : null);
					}
					DataElementNode dataElementNode = obj2 as DataElementNode;
					if (dataElementNode != null)
					{
						string text = dataElementNode.GetValue((node.View.Model as ParameterTreeTableModel).GetIndexOfColumn(6))?.ToString();
						Control control = editor.BeginEdit(node, nColumnIndex, cImmediate, ref bEditComplete);
						if (text != null && control != null)
						{
							control.Text = text;
							if (control is TextBoxBase)
							{
								(control as TextBoxBase).SelectAll();
							}
						}
						return control;
					}
				}
			}
			if (editor == null)
			{
				return null;
			}
			return editor.BeginEdit(node, nColumnIndex, cImmediate, ref bEditComplete);
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			ITreeTableViewEditor editor = GetEditor(node);
			if (node.View != null)
			{
				DataElementNode dataElementNode = node.View.GetModelNode(node) as DataElementNode;
				if (dataElementNode != null && dataElementNode.PlcNode.IsConfigModeOnlineApplication && editor is TextBoxTreeTableViewEditor)
				{
					return control.Text;
				}
			}
			if (editor == null)
			{
				return null;
			}
			return editor.AcceptEdit(node, control);
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			ITreeTableViewEditor editor = GetEditor(node);
			if (editor == null)
			{
				return false;
			}
			return editor.OneClickEdit(node, nColumnIndex);
		}

		private ITreeTableViewEditor GetEditor(TreeTableViewNode viewNode)
		{
			if (viewNode == null)
			{
				throw new ArgumentNullException("viewNode");
			}
			DataElementNode dataElementNode = (viewNode.View ?? throw new ArgumentException("This node is not associated with a view.")).GetModelNode(viewNode) as DataElementNode;
			if (dataElementNode == null)
			{
				return null;
			}
			if ((dataElementNode.DataElement.GetTypeString().ToUpperInvariant() == "BOOL" || dataElementNode.DataElement.GetTypeString().ToUpperInvariant() == "BIT") && !dataElementNode.DataElement.IsEnumeration)
			{
				bool flag = _bAllowNullValues;
				if (dataElementNode.Parent != null && dataElementNode.Parent is DataElementNode && dataElementNode.OnlineVarRef != null)
				{
					string text = ((IExprement)dataElementNode.OnlineVarRef.Expression).ToString();
					if (!text.Contains("%") && text.Contains("."))
					{
						DataElementNode dataElementNode2 = dataElementNode;
						do
						{
							IDataElement dataElement = dataElementNode2.DataElement;
							if (dataElement != null && dataElement.IoMapping != null && dataElement.IoMapping.VariableMappings != null && ((ICollection)dataElement.IoMapping.VariableMappings).Count > 0 && dataElement.IoMapping.VariableMappings[0]
								.Variable
								.Contains(text))
							{
								if (dataElementNode2 != dataElementNode)
								{
									flag = false;
								}
								break;
							}
							dataElementNode2 = dataElementNode2.Parent as DataElementNode;
						}
						while (dataElementNode2 != null);
					}
				}
				if (flag)
				{
					return (ITreeTableViewEditor)(object)BooleanTreeTableViewEditor.WithNullValue;
				}
				return (ITreeTableViewEditor)(object)BooleanTreeTableViewEditor.Simple;
			}
			if (dataElementNode.DataElement.IsEnumeration)
			{
				return dataElementNode.GetEnumerationEditor(_bAllowNullValues);
			}
			return MyTextBoxTreeTableViewEditor.TextBox;
		}
	}
}
