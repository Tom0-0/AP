namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetInitializationAction : AbstractUndoableAction
	{
		private string _stOldInitialization;

		private string _stNewInitialization;

		internal SetInitializationAction(VariableListModel model, string stName, string stOldInitialization, string stNewInitialization)
			: base(model, stName)
		{
			_stOldInitialization = stOldInitialization;
			_stNewInitialization = stNewInitialization;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetInitialization(_stOldInitialization);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetInitialization(_stNewInitialization);
			return _stName;
		}
	}
}
