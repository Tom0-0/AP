namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class MoveUpAction : AbstractUndoableAction
	{
		private int _nIndex;

		internal MoveUpAction(VariableListModel model, int nIndex)
			: base(model, string.Empty)
		{
			_nIndex = nIndex;
		}

		public override object Undo()
		{
			_model.DoMoveDown(_nIndex++);
			return _nIndex;
		}

		public override object Redo()
		{
			_model.DoMoveUp(_nIndex--);
			return _nIndex;
		}
	}
}
