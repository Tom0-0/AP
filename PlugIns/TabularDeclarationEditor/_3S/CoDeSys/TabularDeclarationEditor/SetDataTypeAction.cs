namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetDataTypeAction : AbstractUndoableAction
	{
		private string _stOldDatatype;

		private string _stNewDatatype;

		internal SetDataTypeAction(VariableListModel model, string stName, string stOldDatatype, string stNewDatatype)
			: base(model, stName)
		{
			_stOldDatatype = stOldDatatype;
			_stNewDatatype = stNewDatatype;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetDataType(_stOldDatatype);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetDataType(_stNewDatatype);
			return _stName;
		}
	}
}
