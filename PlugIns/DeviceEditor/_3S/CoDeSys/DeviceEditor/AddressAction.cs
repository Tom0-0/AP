using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class AddressAction : IUndoableAction
	{
		private DataElementNode _node;

		private bool _bNewAutomaticAddress;

		private string _stNewAddress;

		private bool _bOldAutomaticAddress;

		private string _stOldAddress;

		public string Description => string.Empty;

		public AddressAction(DataElementNode node, bool bAutomaticAddress, string stAddress)
		{
			_node = node;
			_bNewAutomaticAddress = bAutomaticAddress;
			_stNewAddress = stAddress;
		}

		public object Undo()
		{
			try
			{
				_node.UpdateData(bModifiy: true);
				_node.DataElement.IoMapping.AutomaticIecAddress=(_bOldAutomaticAddress);
				if (!_bOldAutomaticAddress)
				{
					_node.DataElement.IoMapping.IecAddress=(_stOldAddress);
				}
				_node.PlcNode.TreeModel.RaiseChanged(_node);
			}
			catch
			{
			}
			return _node;
		}

		public object Redo()
		{
			try
			{
				_node.UpdateData(bModifiy: true);
				_bOldAutomaticAddress = _node.DataElement.IoMapping.AutomaticIecAddress;
				_stOldAddress = _node.DataElement.IoMapping.IecAddress;
				_node.DataElement.IoMapping.AutomaticIecAddress=(_bNewAutomaticAddress);
				if (!_bNewAutomaticAddress)
				{
					_node.DataElement.IoMapping.IecAddress=(_stNewAddress);
				}
				_node.PlcNode.TreeModel.RaiseChanged(_node);
			}
			catch
			{
			}
			return _node;
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
