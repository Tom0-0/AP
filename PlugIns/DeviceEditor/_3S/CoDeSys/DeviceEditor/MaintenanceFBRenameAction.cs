using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MaintenanceFBRenameAction : IUndoableAction
	{
		private FbInstanceTreeTableNode _node;

		private string _stNewName = string.Empty;

		private string _stOldName = string.Empty;

		public string Description => string.Empty;

		public MaintenanceFBRenameAction(FbInstanceTreeTableNode node, string stNewName)
		{
			_node = node;
			_stNewName = stNewName;
			if (node.FbInstance is IFbInstance5)
			{
				ref string stOldName = ref _stOldName;
				IFbInstance fbInstance = node.FbInstance;
				stOldName = ((IFbInstance5)((fbInstance is IFbInstance5) ? fbInstance : null)).BaseName;
			}
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}

		public object Redo()
		{
			RenameInstanceName(_stNewName, _stOldName);
			return _node;
		}

		public object Undo()
		{
			RenameInstanceName(_stOldName, _stNewName);
			return _node;
		}

		internal void RenameInstanceName(string stNewName, string stOldName)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected O, but got Unknown
			if (stNewName == stOldName)
			{
				return;
			}
			IIoProvider val = ((_node.Model.InstanceProvider?.Editor)?.GetParameterSetProvider())?.GetIoProvider(bToModify: true);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				IDriverInfo driverInfo = val.DriverInfo;
				obj = ((driverInfo != null) ? driverInfo.RequiredLibs : null);
			}
			IRequiredLibsList2 val2 = (IRequiredLibsList2)((obj is IRequiredLibsList2) ? obj : null);
			if (val2 == null)
			{
				return;
			}
			foreach (IRequiredLib4 item in (IEnumerable)val2)
			{
				foreach (IFbInstance item2 in (IEnumerable)((IRequiredLib)item).FbInstances)
				{
					IFbInstance val3 = item2;
					if (string.Compare(((IFbInstance5)((val3 is IFbInstance5) ? val3 : null)).BaseName, stOldName, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						continue;
					}
					string text = stOldName;
					string text2 = stNewName;
					object obj2;
					if (val == null)
					{
						obj2 = null;
					}
					else
					{
						IMetaObject metaObject = val.GetMetaObject();
						obj2 = ((metaObject != null) ? metaObject.Name : null);
					}
					string text3 = (string)obj2;
					if (!string.IsNullOrEmpty(text3))
					{
						text = text.Replace("$(DeviceName)", text3);
						text2 = text2.Replace("$(DeviceName)", text3);
					}
					((IFbInstance5)((val3 is IFbInstance5) ? val3 : null)).BaseName=(stNewName);
					val3.Instance.Variable=(text2);
					foreach (IParameter item3 in (IEnumerable)val.ParameterSet)
					{
						IParameter val4 = item3;
						if (((ICollection)((IDataElement)val4).IoMapping.VariableMappings).Count > 0 && ((IDataElement)val4).IoMapping.VariableMappings[0] is IVariableMapping3 && (item3.IoMapping.VariableMappings[0] as IVariableMapping3).IoChannelFBInstance == stOldName)
						{
							((IDataElement)val4).IoMapping.VariableMappings[0]
								.Variable=(((IDataElement)val4).IoMapping.VariableMappings[0]
									.Variable
									.Replace(text, text2));
						}
					}
					_node.Model.Refresh();
					return;
				}
			}
		}
	}
}
