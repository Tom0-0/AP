namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetScopeAction : AbstractUndoableAction
	{
		private ModelTokenType _oldScope;

		private ModelTokenType _newScope;

		internal SetScopeAction(VariableListModel model, string stName, ModelTokenType oldScope, ModelTokenType newScope)
			: base(model, stName)
		{
			_oldScope = oldScope;
			_newScope = newScope;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetScope(_oldScope);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetScope(_newScope);
			return _stName;
		}
	}
}
