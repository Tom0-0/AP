using System;
using System.Drawing;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class EmptyListNode : AbstractVariableListNode
	{
		private static readonly Image IMAGE_EMPTY = Bitmap.FromHicon(Resources.Empty.Handle);

		internal EmptyListNode(VariableListModel model)
			: base(model)
		{
		}

		public override object GetValue(int nColumnIndex)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			return base.Model.GetColumnMeaning(nColumnIndex) switch
			{
				VariableListColumns.Line => new LineCellData(-1, bBookmark: false), 
				VariableListColumns.Scope => (object)new IconLabelTreeTableViewCellData(IMAGE_EMPTY, (object)string.Empty), 
				VariableListColumns.Name => string.Empty, 
				VariableListColumns.Address => string.Empty, 
				VariableListColumns.DataType => string.Empty, 
				VariableListColumns.Initialization => string.Empty, 
				VariableListColumns.Comment => string.Empty, 
				VariableListColumns.Attributes => string.Empty, 
				VariableListColumns.NothingToDeclare => Resources.NoVariablesAllowed, 
				_ => null, 
			};
		}

		public override bool IsEditable(int nColumnIndex)
		{
			return false;
		}

		public override void SetValue(int nColumnIndex, object value)
		{
			throw new InvalidOperationException("This node is read-only.");
		}
	}
}
