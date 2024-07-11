namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetRetainAction : AbstractUndoableAction
	{
		private bool _bOldRetain;

		private bool _bNewRetain;

		internal SetRetainAction(VariableListModel model, string stName, bool bOldRetain, bool bNewRetain)
			: base(model, stName)
		{
			_bOldRetain = bOldRetain;
			_bNewRetain = bNewRetain;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetRetain(_bOldRetain);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetRetain(_bNewRetain);
			return _stName;
		}
	}
}
