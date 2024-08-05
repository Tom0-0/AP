using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class ValueAction : IUndoableAction
	{
		private DataElementNode _node;

		private bool _bIsComment;

		private string _stNewValue;

		private string _stOldValue;

		public string Description => string.Empty;

		public ValueAction(DataElementNode node, bool bIsComment, string stValue)
		{
			_node = node;
			_bIsComment = bIsComment;
			_stNewValue = stValue;
		}

		internal void SetDescription(string stDescription)
		{
			IDataElement dataElement = _node.DataElement;
			IDataElement4 val = (IDataElement4)(object)((dataElement is IDataElement4) ? dataElement : null);
			if (val != null)
			{
				IParameterSet parameterSet = _node.ParameterSetProvider.GetParameterSet(bToModify: true);
				IParameterSet2 val2 = (IParameterSet2)(object)((parameterSet is IParameterSet2) ? parameterSet : null);
				if (val2 != null)
				{
					IStringTable stringTable = val2.StringTable;
					if (stringTable != null)
					{
						IStringRef description = stringTable.CreateStringRef("", "", stDescription);
						val.SetDescription(description);
					}
				}
				return;
			}
			IDataElement dataElement2 = _node.DataElement;
			IParameter3 val3 = (IParameter3)(object)((dataElement2 is IParameter3) ? dataElement2 : null);
			if (val3 == null)
			{
				return;
			}
			IParameterSet parameterSet2 = _node.ParameterSetProvider.GetParameterSet(bToModify: true);
			IParameterSet2 val4 = (IParameterSet2)(object)((parameterSet2 is IParameterSet2) ? parameterSet2 : null);
			if (val4 != null)
			{
				IStringTable stringTable2 = val4.StringTable;
				if (stringTable2 != null)
				{
					IStringRef description2 = stringTable2.CreateStringRef("", "", stDescription);
					val3.SetDescription(description2);
				}
			}
		}

		public object Undo()
		{
			try
			{
				_node.UpdateData(bModifiy: true);
				if (_bIsComment)
				{
					SetDescription(_stOldValue);
				}
				else
				{
					_node.DataElement.Value=(_stOldValue);
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
				if (_bIsComment)
				{
					_stOldValue = _node.DataElement.Description;
					SetDescription(_stNewValue);
				}
				else
				{
					_stOldValue = _node.DataElement.Value;
					_node.DataElement.Value=(_stNewValue);
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
