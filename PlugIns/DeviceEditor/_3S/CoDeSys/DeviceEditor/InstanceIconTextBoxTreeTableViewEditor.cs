using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class InstanceIconTextBoxTreeTableViewEditor : TextBoxTreeTableViewEditor
	{
		private static ITreeTableViewEditor s_textBox = (ITreeTableViewEditor)(object)new InstanceIconTextBoxTreeTableViewEditor();

		public static ITreeTableViewEditor TextBox => s_textBox;

		public override Control BeginEdit(TreeTableViewNode node, int nModelColumnIndex, char cImmediate, ref bool bEditComplete)
		{
			object obj = node.CellValues[nModelColumnIndex];
			IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)((obj is IconLabelTreeTableViewCellData) ? obj : null);
			if (val != null)
			{
				Control control = ((TextBoxTreeTableViewEditor)this).BeginEdit(node, nModelColumnIndex, cImmediate, ref bEditComplete);
				control.Tag = val;
				return control;
			}
			return null;
		}

		public override object AcceptEdit(TreeTableViewNode node, Control control)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			if (control.Tag is IconLabelTreeTableViewCellData)
			{
				IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)control.Tag;
				if (val.Label != null)
				{
					object obj = ConvertFromString(control.Text, val.Label.GetType());
					return (object)new IconLabelTreeTableViewCellData(val.Image, obj);
				}
			}
			return null;
		}

		protected override string ConvertToString(object value)
		{
			if (value is InstanceIconLabelTreeTableViewCellData)
			{
				InstanceIconLabelTreeTableViewCellData instanceIconLabelTreeTableViewCellData = value as InstanceIconLabelTreeTableViewCellData;
				if (instanceIconLabelTreeTableViewCellData.FbInstance is IFbInstance5)
				{
					IFbInstance fbInstance = instanceIconLabelTreeTableViewCellData.FbInstance;
					return ((IFbInstance5)((fbInstance is IFbInstance5) ? fbInstance : null)).BaseName;
				}
			}
			IconLabelTreeTableViewCellData val = (IconLabelTreeTableViewCellData)((value is IconLabelTreeTableViewCellData) ? value : null);
			if (val == null)
			{
				return string.Empty;
			}
			return val.Label.ToString();
		}

		public InstanceIconTextBoxTreeTableViewEditor()
			: base()
		{
		}
	}
}
