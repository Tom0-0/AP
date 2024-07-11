namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class SetAddressAction : AbstractUndoableAction
	{
		private string _stOldAddress;

		private string _stNewAddress;

		internal SetAddressAction(VariableListModel model, string stName, string stOldAddress, string stNewAddress)
			: base(model, stName)
		{
			_stOldAddress = stOldAddress;
			_stNewAddress = stNewAddress;
		}

		public override object Undo()
		{
			_model.GetNode(_stName).DoSetAddress(_stOldAddress);
			return _stName;
		}

		public override object Redo()
		{
			_model.GetNode(_stName).DoSetAddress(_stNewAddress);
			return _stName;
		}
	}
}
