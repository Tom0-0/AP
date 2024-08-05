using System.Collections;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceEditor.SimpleMappingEditor;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MappingAction : IUndoableAction
	{
		private DataElementNode _node;

		private string _stOldMapping = string.Empty;

		private string _stNewMapping = string.Empty;

		private string _stDefaultVariable = string.Empty;

		private string _stFBIoInstance = string.Empty;

		private bool _bEmptyMapping;

		public string Description => "";

		public MappingAction(DataElementNode node, string stNewMapping, bool bEmptyMapping)
		{
			_node = node;
			_stNewMapping = stNewMapping;
			_bEmptyMapping = bEmptyMapping;
		}

		public MappingAction(DataElementNode node, string stNewMapping, string stOldMapping)
		{
			_node = node;
			_stNewMapping = stNewMapping;
			_stOldMapping = stOldMapping;
		}

		public object Undo()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Invalid comparison between Unknown and I4
			_node.UpdateData(bModifiy: true);
			IVariableMappingCollection variableMappings = _node.DataElement.IoMapping.VariableMappings;
			if (!string.IsNullOrEmpty(_stOldMapping))
			{
				bool flag = true;
				if (_stOldMapping.Contains("."))
				{
					flag = false;
				}
				if ((int)_node.Parameter.ChannelType == 3)
				{
					flag = true;
				}
				if (((ICollection)variableMappings).Count == 0)
				{
					IVariableMapping val = variableMappings.AddMapping(_stOldMapping, flag);
					if (val is IVariableMapping2)
					{
						((IVariableMapping2)((val is IVariableMapping2) ? val : null)).DefaultVariable=(_stDefaultVariable);
					}
					if (val is IVariableMapping3)
					{
						((IVariableMapping3)((val is IVariableMapping3) ? val : null)).IoChannelFBInstance=(_stFBIoInstance);
					}
				}
				else
				{
					variableMappings[0].Variable=(_stOldMapping);
					variableMappings[0].CreateVariable=(flag);
				}
			}
			else if (((ICollection)variableMappings).Count > 0)
			{
				IVariableMapping obj = variableMappings[0];
				IVariableMapping2 val2 = (IVariableMapping2)(object)((obj is IVariableMapping2) ? obj : null);
				if (val2 != null && val2.DefaultVariable != string.Empty && !_bEmptyMapping)
				{
					((IVariableMapping)val2).Variable=(val2.DefaultVariable);
				}
				else
				{
					variableMappings.RemoveAt(0);
				}
			}
			if (_node.ParameterSetProvider is _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider)
			{
				(_node.ParameterSetProvider as _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider).StoreObject();
			}
			_node.PlcNode.TreeModel.RaiseChanged(_node);
			return _node;
		}

		public object Redo()
		{
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Invalid comparison between Unknown and I4
			try
			{
				_node.UpdateData(bModifiy: true);
				IVariableMappingCollection variableMappings = _node.DataElement.IoMapping.VariableMappings;
				if (((ICollection)variableMappings).Count > 0)
				{
					IVariableMapping obj = variableMappings[0];
					IVariableMapping2 val = (IVariableMapping2)(object)((obj is IVariableMapping2) ? obj : null);
					_stOldMapping = ((IVariableMapping)val).Variable;
					if (string.IsNullOrEmpty(_stNewMapping))
					{
						if (val is IVariableMapping3)
						{
							_stFBIoInstance = ((IVariableMapping3)((val is IVariableMapping3) ? val : null)).IoChannelFBInstance;
						}
						if (val != null && val.DefaultVariable != string.Empty)
						{
							_stDefaultVariable = val.DefaultVariable;
							if (!_bEmptyMapping)
							{
								((IVariableMapping)val).Variable=(val.DefaultVariable);
							}
							else
							{
								variableMappings.RemoveAt(0);
							}
						}
						else
						{
							variableMappings.RemoveAt(0);
						}
						_node.PlcNode.TreeModel.RaiseChanged(_node);
					}
				}
				if (!string.IsNullOrEmpty(_stNewMapping))
				{
					bool flag = true;
					if (_stNewMapping.Contains("."))
					{
						flag = false;
					}
					if ((int)_node.Parameter.ChannelType == 3)
					{
						flag = true;
					}
					if (((ICollection)variableMappings).Count == 0)
					{
						variableMappings.AddMapping(_stNewMapping, flag);
					}
					else
					{
						variableMappings[0].Variable=(_stNewMapping);
						variableMappings[0].CreateVariable=(flag);
					}
					if (variableMappings[0] is IVariableMapping3)
					{
						IVariableMapping obj2 = variableMappings[0];
						((IVariableMapping3)((obj2 is IVariableMapping3) ? obj2 : null)).IoChannelFBInstance=(string.Empty);
					}
					_node.PlcNode.TreeModel.RaiseChanged(_node);
				}
				if (_node.ParameterSetProvider is _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider)
				{
					(_node.ParameterSetProvider as _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider).StoreObject();
				}
				return _node;
			}
			catch
			{
				throw;
			}
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
