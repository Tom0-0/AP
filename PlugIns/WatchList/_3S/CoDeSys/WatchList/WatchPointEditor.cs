using System;
using System.Collections.Generic;
using System.Windows.Forms;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	public class WatchPointEditor : ITreeTableViewEditor
	{
		private static WatchPointEditor s_singleton = new WatchPointEditor();

		public static ITreeTableViewEditor Singleton => (ITreeTableViewEditor)(object)s_singleton;

		internal IList<WatchPointItem> CollectAllWatchpoints()
		{
			IList<WatchPointItem> list = (IList<WatchPointItem>)new LList<WatchPointItem>();
			list.Add(new WatchPointItem(null));
			IBP[] breakpoints = ((IBPManager)APEnvironment.BPMgr).GetBreakpoints();
			foreach (IBP val in breakpoints)
			{
				if (val is IBP4 && ((IBP2)((val is IBP4) ? val : null)).Watchpoint)
				{
					list.Add(new WatchPointItem((IBP4)(object)((val is IBP4) ? val : null)));
				}
			}
			return list;
		}

		internal IList<WatchPointItem> CollectWatchpointsInsideFunction(IVarRef2 varRef)
		{
			IList<WatchPointItem> list = (IList<WatchPointItem>)new LList<WatchPointItem>();
			list.Add(new WatchPointItem(null));
			if (APEnvironment.LanguageModelUtilities.CompileUtils is ICompileUtilities3)
			{
				ICompileUtilities compileUtils = APEnvironment.LanguageModelUtilities.CompileUtils;
				IExpression val = default(IExpression);
				ISignature calledSignatureForStackVariable = ((ICompileUtilities3)((compileUtils is ICompileUtilities3) ? compileUtils : null)).GetCalledSignatureForStackVariable(varRef, out val);
				if (calledSignatureForStackVariable != null)
				{
					IBP[] breakpoints = ((IBPManager)APEnvironment.BPMgr).GetBreakpoints();
					foreach (IBP val2 in breakpoints)
					{
						IBP4 val3 = (IBP4)(object)((val2 is IBP4) ? val2 : null);
						if (val3 != null && ((IBP2)((val2 is IBP4) ? val2 : null)).Watchpoint)
						{
							ISignature signature = val3.GetSignature();
							if (calledSignatureForStackVariable.Id == signature.Id)
							{
								list.Add(new WatchPointItem(val3));
							}
						}
					}
				}
			}
			return list;
		}

		public Control BeginEdit(TreeTableViewNode node, int nColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			TreeTableView view = node.View;
			if (view == null)
			{
				throw new ArgumentException("The node is not associated with a view.");
			}
			object obj = node.CellValues[nColumnIndex];
			ComboBox comboBox = new ComboBox();
			comboBox.BackColor = ((Control)(object)view).BackColor;
			comboBox.Bounds = node.GetBounds(nColumnIndex, (CellBoundsPortion)4);
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox.Font = ((Control)(object)view).Font;
			comboBox.ForeColor = ((Control)(object)view).ForeColor;
			comboBox.IntegralHeight = true;
			comboBox.Sorted = false;
			WatchListNode watchListNode = (WatchListNode)(object)node.View.GetModelNode(node);
			//node.CellValues[watchListNode.COLUMN_EXPRESSION];
			IVarRef varReference = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(watchListNode.Expression);
			IVarRef2 val = (IVarRef2)(object)((varReference is IVarRef2) ? varReference : null);
			IList<WatchPointItem> list = (IList<WatchPointItem>)new LList<WatchPointItem>();
			if (val != null)
			{
				list = ((!(val.AddressInfo is IAddressInfo4) || !(val.AddressInfo as IAddressInfo4).ContainsStackRelativeAddress) ? this.CollectAllWatchpoints() : this.CollectWatchpointsInsideFunction(val));
			}
			foreach (WatchPointItem item in list)
			{
				comboBox.Items.Add(item);
			}
			if (obj is WatchPointItem)
			{
				comboBox.SelectedItem = (WatchPointItem)obj;
				return comboBox;
			}
			return null;
		}

		public object AcceptEdit(TreeTableViewNode node, Control control)
		{
			return ((ComboBox)control).SelectedItem;
		}

		public bool OneClickEdit(TreeTableViewNode node, int nColumnIndex)
		{
			return false;
		}

		protected virtual string ConvertToString(object value)
		{
			if (value != null)
			{
				(value as WatchPointItem).ToString();
			}
			return null;
		}
	}
}
