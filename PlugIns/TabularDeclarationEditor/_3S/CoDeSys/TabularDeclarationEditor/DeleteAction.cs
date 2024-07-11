namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class DeleteAction : AbstractUndoableAction
	{
		private int _nIndex;

		private SerializableTabularDeclarationItem _item;

		internal DeleteAction(VariableListModel model, string stName, int nIndex, SerializableTabularDeclarationItem item)
			: base(model, stName)
		{
			_nIndex = nIndex;
			_item = item;
		}

		public override object Undo()
		{
			_model.DoInsert(_nIndex, _item, _stName);
			return _stName;
		}

		public override object Redo()
		{
			_model.DoDelete(_nIndex);
			return _nIndex;
		}
	}
}
