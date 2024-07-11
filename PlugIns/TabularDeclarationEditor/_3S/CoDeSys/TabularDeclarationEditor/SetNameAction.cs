namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetNameAction : AbstractUndoableAction
	{
		private string _stNewName;

		internal SetNameAction(VariableListModel model, string stOldName, string stNewName)
			: base(model, stOldName)
		{
			_stNewName = stNewName;
		}

		public override object Undo()
		{
			_model.GetNode(_stNewName).DoSetName(_stName);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetName(_stNewName);
			return _stNewName;
		}
	}
}
