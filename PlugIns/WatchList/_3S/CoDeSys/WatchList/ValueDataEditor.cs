using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.WatchList
{
	internal class ValueDataEditor : ITreeTableViewEditor
	{
		private class TextValueDataEditor : ITreeTableViewEditor
		{
			public static readonly TextValueDataEditor Singleton = new TextValueDataEditor();

			public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				if (node == null)
				{
					throw new ArgumentNullException("node");
				}
				TreeTableView view = node.View;
				if (view == null)
				{
					throw new ArgumentException("The node is not associated with a view.");
				}
				ValueData valueData = node.CellValues[nColumnIndex] as ValueData;
				if (valueData != null)
				{
					string textToRender = WatchListNodeUtils.GetTextToRender(valueData.Text, valueData.TypeClass);
					TextBox textBox = new TextBox();
					textBox.BackColor = ((Control)(object)view).BackColor;
					textBox.BorderStyle = BorderStyle.FixedSingle;
					textBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
					textBox.Font = ((Control)(object)view).Font;
					textBox.ForeColor = ((Control)(object)view).ForeColor;
					textBox.Text = ((cImmediate == '\0') ? textToRender : cImmediate.ToString());
					textBox.TextAlign = view.Columns[nColumnIndex].TextAlign;
					textBox.Tag = valueData;
					if (cImmediate == '\0')
					{
						textBox.SelectAll();
					}
					else
					{
						textBox.Select(1, 0);
					}
					return textBox;
				}
				return null;
			}

			public object AcceptEdit(TreeTableViewNode node, Control control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				return new TextValueData((WatchListNode)(object)node.View.GetModelNode(node), control.Text, bForced: false);
			}

			public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
			{
				return false;
			}
		}

		private class ToggleValueDataEditor : ITreeTableViewEditor
		{
			public static readonly ToggleValueDataEditor Singleton = new ToggleValueDataEditor();

			public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
			{
				if (node == null)
				{
					throw new ArgumentNullException("node");
				}
				TextBox result = new TextBox
				{
					Bounds = Rectangle.Empty,
					Tag = node.CellValues[nColumnIndex]
				};
				bEditComplete = true;
				return result;
			}

			public object AcceptEdit(TreeTableViewNode node, Control control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				ValueData valueData = control.Tag as ValueData;
				try
				{
					if (!(valueData.Value is bool) && valueData.Value != null)
					{
						return new EmptyValueData(valueData.Node);
					}
					if (!(valueData.Node.OnlineVarRef.Value is bool))
					{
						return new EmptyValueData(valueData.Node);
					}
					bool num = (bool)valueData.Node.OnlineVarRef.Value;
					bool? flag = null;
					if (valueData.Value != null)
					{
						flag = (bool)valueData.Value;
					}
					flag = (num ? ((!flag.HasValue) ? new bool?(false) : ((!flag.Value) ? new bool?(true) : null)) : ((!flag.HasValue) ? new bool?(true) : ((!flag.Value) ? null : new bool?(false))));
					if (!flag.HasValue)
					{
						return new EmptyValueData(valueData.Node);
					}
					if (flag.Value)
					{
						if (flag.Value)
						{
							return new TextValueData(valueData.Node, "TRUE", bForced: false);
						}
						return new TextValueData(valueData.Node, "FALSE", bForced: false);
					}
					return new TextValueData(valueData.Node, "FALSE", bForced: false);
				}
				catch
				{
					return new EmptyValueData(valueData.Node);
				}
			}

			public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
			{
				return true;
			}
		}

		private class EnumValueDataEditor : ITreeTableViewEditor
		{
			public static readonly EnumValueDataEditor Singleton = new EnumValueDataEditor();

			public object AcceptEdit(TreeTableViewNode node, Control control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				return new TextValueData((WatchListNode)(object)node.View.GetModelNode(node), control.Text, bForced: false);
			}

			public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
			{
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Invalid comparison between Unknown and I4
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Invalid comparison between Unknown and I4
				if (node == null)
				{
					throw new ArgumentNullException("node");
				}
				TreeTableView view = node.View;
				if (view == null)
				{
					throw new ArgumentException("The node is not associated with a view.");
				}
				WatchListNode watchListNode = node.View.GetModelNode(node) as WatchListNode;
				if (watchListNode != null && watchListNode.OnlineVarRef != null && watchListNode.OnlineVarRef.Expression != null)
				{
					ICompiledType val = watchListNode.OnlineVarRef.Expression.Type;
					if ((int)((IType)val).Class == 23 && watchListNode.OnlineVarRef.Expression.Type.BaseType is IEnumType2)
					{
						val = watchListNode.OnlineVarRef.Expression.Type.BaseType;
					}
					if ((int)((IType)val).Class == 25)
					{
						ComboBox comboBox = new ComboBox();
						if (Common.LoadEnumValuesToComboBoxControl(comboBox, val, watchListNode.VarRef.ApplicationGuid) && comboBox.Items.Count > 0)
						{
							comboBox.SelectedItem = comboBox.Items[0];
							comboBox.Tag = new TextValueData(watchListNode, comboBox.Items[0].ToString(), bForced: false);
							comboBox.DropDownStyle = ComboBoxStyle.DropDown;
							comboBox.BackColor = ((Control)(object)view).BackColor;
							comboBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)1);
							comboBox.Font = ((Control)(object)view).Font;
							comboBox.ForeColor = ((Control)(object)view).ForeColor;
							return comboBox;
						}
					}
				}
				ValueData valueData = node.CellValues[nColumnIndex] as ValueData;
				if (valueData != null)
				{
					TextBox textBox = new TextBox();
					textBox.BackColor = ((Control)(object)view).BackColor;
					textBox.BorderStyle = BorderStyle.FixedSingle;
					textBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
					textBox.Font = ((Control)(object)view).Font;
					textBox.ForeColor = ((Control)(object)view).ForeColor;
					textBox.Text = ((cImmediate == '\0') ? valueData.Text : cImmediate.ToString());
					textBox.TextAlign = view.Columns[nColumnIndex].TextAlign;
					textBox.Tag = valueData;
					if (cImmediate == '\0')
					{
						textBox.SelectAll();
					}
					else
					{
						textBox.Select(1, 0);
					}
					return textBox;
				}
				return null;
			}

			public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
			{
				return false;
			}
		}

		private class TextValueDataDialogEditor : ITreeTableViewEditor
		{
			public static readonly TextValueDataDialogEditor Singleton = new TextValueDataDialogEditor();

			public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
			{
				if (node == null)
				{
					throw new ArgumentNullException("node");
				}
				if (node.View == null)
				{
					throw new ArgumentException("The node is not associated with a view.");
				}
				WatchListNode watchListNode = node.View.GetModelNode(node) as WatchListNode;
				if (watchListNode != null)
				{
					string expression = watchListNode.Expression;
					ICompiledType compiledType = watchListNode.GetCompiledType();
					ValueData valueData = watchListNode.GetValue(watchListNode.COLUMN_VALUE) as ValueData;
					string text = ((valueData != null) ? valueData.Text : string.Empty);
					ValueData valueData2 = watchListNode.GetValue(watchListNode.COLUMN_PREPARED_VALUE) as ValueData;
					object unforce = null;
					string text2 = string.Empty;
					if (valueData2.Value == PreparedValues.Unforce || valueData2.Value == PreparedValues.UnforceAndRestore)
					{
						unforce = valueData2.Value;
					}
					else
					{
						text2 = valueData2.Text;
					}
					if (text2 == string.Empty)
					{
						text2 = text;
					}
					TextValueDataDialog textValueDataDialog = new TextValueDataDialog(valueData.IsEnum, watchListNode);
					textValueDataDialog.Initialize(expression, compiledType, text, unforce, text2, watchListNode.GuidApplication);
					if (textValueDataDialog.ShowDialog((IWin32Window)node.View) == DialogResult.OK)
					{
						Control control = new Control();
						if (textValueDataDialog.Unforce != null)
						{
							control.Tag = new TextValueData((WatchListNode)(object)node.View.GetModelNode(node), textValueDataDialog.Unforce);
						}
						else
						{
							control.Tag = new TextValueData((WatchListNode)(object)node.View.GetModelNode(node), textValueDataDialog.PreparedValue, bForced: true);
						}
						bEditComplete = true;
						return control;
					}
				}
				return null;
			}

			public object AcceptEdit(TreeTableViewNode node, Control control)
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				return control.Tag;
			}

			public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
			{
				return true;
			}
		}

		public static readonly ValueDataEditor Singleton = new ValueDataEditor();

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			ITreeTableViewEditor editor = GetEditor(node, nColumnIndex);
			if (editor == null)
			{
				return null;
			}
			return editor.BeginEdit(node, nColumnIndex, cImmediate, ref bEditComplete);
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			ITreeTableViewEditor editor = GetEditor(control.Tag as ValueData);
			if (editor == null)
			{
				return null;
			}
			return editor.AcceptEdit(node, control);
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			ITreeTableViewEditor editor = GetEditor(node, nColumnIndex);
			if (editor == null)
			{
				return false;
			}
			return editor.OneClickEdit(node, nColumnIndex);
		}

		private ITreeTableViewEditor GetEditor(TreeTableViewNode node, int nColumnIndex)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return GetEditor(node.CellValues[nColumnIndex] as ValueData);
		}

		private ITreeTableViewEditor GetEditor(ValueData data)
		{
			if (data != null)
			{
				if (data.Forced)
				{
					return (ITreeTableViewEditor)(object)TextValueDataDialogEditor.Singleton;
				}
				if (data.Toggleable)
				{
					return (ITreeTableViewEditor)(object)ToggleValueDataEditor.Singleton;
				}
				if (data.IsEnum)
				{
					return (ITreeTableViewEditor)(object)EnumValueDataEditor.Singleton;
				}
				return (ITreeTableViewEditor)(object)TextValueDataEditor.Singleton;
			}
			return null;
		}
	}
}
