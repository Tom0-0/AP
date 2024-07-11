namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetPersistentAction : AbstractUndoableAction
	{
		private bool _bOldPersistent;

		private bool _bNewPersistent;

		internal SetPersistentAction(VariableListModel model, string stName, bool bOldPersistent, bool bNewPersistent)
			: base(model, stName)
		{
			_bOldPersistent = bOldPersistent;
			_bNewPersistent = bNewPersistent;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetPersistent(_bOldPersistent);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetPersistent(_bNewPersistent);
			return _stName;
		}
	}
}
