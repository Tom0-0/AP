namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class MoveDownAction : AbstractUndoableAction
	{
		private int _nIndex;

		internal MoveDownAction(VariableListModel model, int nIndex)
			: base(model, string.Empty)
		{
			_nIndex = nIndex;
		}

		public override object Undo()
		{
			_model.DoMoveUp(_nIndex--);
			return _nIndex;
		}

		public override object Redo()
		{
			_model.DoMoveDown(_nIndex++);
			return _nIndex;
		}
	}
}
