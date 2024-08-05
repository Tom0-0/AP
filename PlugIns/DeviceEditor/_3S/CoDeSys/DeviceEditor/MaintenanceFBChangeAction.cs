using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MaintenanceFBChangeAction : IUndoableAction
	{
		private FbInstanceTreeTableNode _node;

		private int _nHandle;

		private Guid _guidApp;

		private bool _bIsOutput;

		private string _stOldFBName = string.Empty;

		private string _stOldFBType = string.Empty;

		private string _stOldLibName = string.Empty;

		private string _stNewFBName = string.Empty;

		private string _stNewFBType = string.Empty;

		private string _stNewLibName = string.Empty;

		public string Description => string.Empty;

		public MaintenanceFBChangeAction(FbInstanceTreeTableNode node, FBCreateNode fbNode, int nHandle, Guid guidApp)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			_node = node;
			_nHandle = nHandle;
			_guidApp = guidApp;
			FbInstanceProvider instanceProvider = _node.Model.InstanceProvider;
			object obj;
			if (instanceProvider == null)
			{
				obj = null;
			}
			else
			{
				IOMappingEditor editor = instanceProvider.Editor;
				if (editor == null)
				{
					obj = null;
				}
				else
				{
					IDriverInfo driverInfo = editor.GetDriverInfo(bToModify: false);
					obj = ((driverInfo != null) ? driverInfo.RequiredLibs : null);
				}
			}
			IRequiredLibsList val = (IRequiredLibsList)obj;
			if (val == null)
			{
				return;
			}
			foreach (IRequiredLib4 item in (IEnumerable)val)
			{
				IRequiredLib4 val2 = item;
				foreach (IFbInstance item2 in (IEnumerable)((IRequiredLib)val2).FbInstances)
				{
					IFbInstance val3 = item2;
					if (string.Compare(val3.Instance.Variable, _node.FbInstance.Instance.Variable, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						continue;
					}
					if (!string.IsNullOrEmpty(val2.LibName))
					{
						_stOldLibName = $"{val2.LibName}, {val2.Version} ({val2.Vendor})";
					}
					_stOldFBType = val3.FbName;
					_stOldFBName = val3.Instance.Variable;
					foreach (ICyclicCall item3 in (IEnumerable)val3.CyclicCalls)
					{
						if (string.Compare(item3.WhenToCall, "beforeWriteOutputs", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							_bIsOutput = true;
						}
					}
					if (fbNode != null)
					{
						_stNewLibName = fbNode.Signature.LibraryPath;
						_stNewFBType = fbNode.Signature.OrgName;
						_stNewFBName = _stOldFBName;
					}
					return;
				}
			}
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}

		public object Redo()
		{
			IOMappingEditor iOMappingEditor = _node.Model.InstanceProvider?.Editor;
			IParameterSetProvider paramProvider = iOMappingEditor?.GetParameterSetProvider();
			FBIoChannels.RemoveIECObject(paramProvider, _stOldFBName);
			if (!string.IsNullOrEmpty(_stNewFBName))
			{
				FBIoChannels.AddIECObject(paramProvider, _nHandle, _guidApp, _stNewLibName, ref _stNewFBName, _stNewFBType, _bIsOutput);
			}
			_node.Model.Refresh();
			IConnectorEditorFrame obj = iOMappingEditor?.GetFrame();
			IParameterSetEditorFrame2 val = (IParameterSetEditorFrame2)(object)((obj is IParameterSetEditorFrame2) ? obj : null);
			if (val != null)
			{
				val.UpdatePages((IBaseDeviceEditor)(object)((iOMappingEditor is IBaseDeviceEditor) ? iOMappingEditor : null));
			}
			return _node;
		}

		public object Undo()
		{
			IOMappingEditor iOMappingEditor = _node.Model.InstanceProvider?.Editor;
			IParameterSetProvider paramProvider = iOMappingEditor?.GetParameterSetProvider();
			if (!string.IsNullOrEmpty(_stNewFBName))
			{
				FBIoChannels.RemoveIECObject(paramProvider, _stNewFBName);
			}
			FBIoChannels.AddIECObject(paramProvider, _nHandle, _guidApp, _stOldLibName, ref _stOldFBName, _stOldFBType, _bIsOutput);
			_node.Model.Refresh();
			IConnectorEditorFrame obj = iOMappingEditor?.GetFrame();
			IParameterSetEditorFrame2 val = (IParameterSetEditorFrame2)(object)((obj is IParameterSetEditorFrame2) ? obj : null);
			if (val != null)
			{
				val.UpdatePages((IBaseDeviceEditor)(object)((iOMappingEditor is IBaseDeviceEditor) ? iOMappingEditor : null));
			}
			return _node;
		}
	}
}
