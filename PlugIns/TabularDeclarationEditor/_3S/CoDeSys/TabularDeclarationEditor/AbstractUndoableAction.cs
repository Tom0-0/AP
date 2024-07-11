using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal abstract class AbstractUndoableAction : IUndoableAction
	{
		protected VariableListModel _model;

		protected string _stName;

		public string Description => string.Empty;

		protected AbstractUndoableAction(VariableListModel model, string stName)
		{
			_model = model;
			_stName = stName;
		}

		public abstract object Undo();

		public abstract object Redo();

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
