namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetCommentAction : AbstractUndoableAction
	{
		private string _stOldComment;

		private string _stNewComment;

		internal SetCommentAction(VariableListModel model, string stName, string stOldComment, string stNewComment)
			: base(model, stName)
		{
			_stOldComment = stOldComment;
			_stNewComment = stNewComment;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetComment(_stOldComment);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetComment(_stNewComment);
			return _stName;
		}
	}
}
