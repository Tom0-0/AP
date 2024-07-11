namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetAttributesAction : AbstractUndoableAction
	{
		private string _stOldAttributes;

		private string _stNewAttributes;

		internal SetAttributesAction(VariableListModel model, string stName, string stOldAttributes, string stNewAttributes)
			: base(model, stName)
		{
			_stOldAttributes = stOldAttributes;
			_stNewAttributes = stNewAttributes;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetAttributes(_stOldAttributes);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetAttributes(_stNewAttributes);
			return _stName;
		}
	}
}
