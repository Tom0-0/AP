namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetConstantAction : AbstractUndoableAction
	{
		private bool _bOldConstant;

		private bool _bNewConstant;

		internal SetConstantAction(VariableListModel model, string stName, bool bOldConstant, bool bNewConstant)
			: base(model, stName)
		{
			_bOldConstant = bOldConstant;
			_bNewConstant = bNewConstant;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetConstant(_bOldConstant);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetConstant(_bNewConstant);
			return _stName;
		}
	}
}
