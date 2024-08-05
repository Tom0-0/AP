using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceEditor.SimpleMappingEditor;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class MaintenanceFBAction : IUndoableAction
	{
		private IOMappingEditor _editor;

		private readonly FBCreateNode _fbNode;

		private readonly Guid _guidApp;

		private readonly int _nHandle;

		private readonly DataElementNode _node;

		private readonly IParameterSetProvider _provider;

		private readonly bool _bIsOutput;

		private string _stDefaultVariable = string.Empty;

		private string _stFBName = string.Empty;

		private string _stOldMapping = string.Empty;

		private string _stOldFBInstance;

		public string Description => string.Empty;

		public MaintenanceFBAction(IOMappingEditor editor, DataElementNode node, FBCreateNode fbNode, int nHandle, Guid guidApp)
		{
			_editor = editor;
			_node = node;
			_fbNode = fbNode;
			_nHandle = nHandle;
			_guidApp = guidApp;
		}

		public MaintenanceFBAction(IOMappingEditor editor, IParameterSetProvider provider, FBCreateNode fbNode, int nHandle, Guid guidApp, bool bIsOutput)
		{
			_editor = editor;
			_provider = provider;
			_fbNode = fbNode;
			_nHandle = nHandle;
			_guidApp = guidApp;
			_bIsOutput = bIsOutput;
		}

		public object Undo()
		{
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Invalid comparison between Unknown and I4
			if (_provider != null)
			{
				FBIoChannels.RemoveIECObject(_provider, _stFBName);
				if (_provider is _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider)
				{
					(_provider as _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider).StoreObject();
				}
				if (_editor?.InstancesModel != null)
				{
					_editor.InstancesModel.Refresh();
				}
				if (_editor?.GetFrame() is IParameterSetEditorFrame2 parameterSetEditorFrame)
				{
					parameterSetEditorFrame.UpdatePages(_editor as IBaseDeviceEditor);
				}
			}
			if (_node != null)
			{
				_node.UpdateData(bModifiy: true);
				FBIoChannels.RemoveIECObject(_node.ParameterSetProvider, _stFBName);
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
						IVariableMapping val3 = variableMappings.AddMapping(_stOldMapping, flag);
						if (val3 is IVariableMapping2)
						{
							((IVariableMapping2)((val3 is IVariableMapping2) ? val3 : null)).DefaultVariable=(_stDefaultVariable);
						}
					}
					else
					{
						variableMappings[0].Variable=(_stOldMapping);
						variableMappings[0].CreateVariable=(flag);
					}
					if (variableMappings[0] is IVariableMapping3)
					{
						IVariableMapping obj2 = variableMappings[0];
						((IVariableMapping3)((obj2 is IVariableMapping3) ? obj2 : null)).IoChannelFBInstance=(_stOldFBInstance);
					}
				}
				else if (((ICollection)variableMappings).Count > 0)
				{
					IVariableMapping obj3 = variableMappings[0];
					IVariableMapping2 val4 = (IVariableMapping2)(object)((obj3 is IVariableMapping2) ? obj3 : null);
					if (val4 != null && val4.DefaultVariable != string.Empty)
					{
						((IVariableMapping)val4).Variable=(val4.DefaultVariable);
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
				if (_editor?.InstancesModel != null)
				{
					_editor.InstancesModel.Refresh();
				}
				if (_editor?.GetFrame() is IParameterSetEditorFrame2 parameterSetEditorFrame2)
				{
					parameterSetEditorFrame2.UpdatePages(_editor as IBaseDeviceEditor);
				}
				return _node;
			}
			return null;
		}

		public object Redo()
		{
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Invalid comparison between Unknown and I4
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Invalid comparison between Unknown and I4
			try
			{
				if (_provider != null && _fbNode != null)
				{
					_stFBName = "$(DeviceName)_" + _fbNode.Signature.OrgName;
					FBIoChannels.AddIECObject(_provider, _nHandle, _guidApp, _fbNode.Signature.LibraryPath, ref _stFBName, _fbNode.Signature.OrgName, _bIsOutput);
					if (_provider is _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider)
					{
						(_provider as _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider).StoreObject();
					}
					if (_editor?.InstancesModel != null)
					{
						_editor.InstancesModel.Refresh();
					}
					if (_editor?.GetFrame() is IParameterSetEditorFrame2 parameterSetEditorFrame)
					{
						parameterSetEditorFrame.UpdatePages(_editor as IBaseDeviceEditor);
					}
				}
				if (_node != null)
				{
					_node.UpdateData(bModifiy: true);
					if (_fbNode != null)
					{
						_stFBName = "$(DeviceName)_" + _fbNode.Signature.OrgName;
						FBIoChannels.AddIECObject(_node.ParameterSetProvider, _nHandle, _guidApp, _fbNode.Signature.LibraryPath, ref _stFBName, _fbNode.Signature.OrgName, (int)_node.Parameter.ChannelType != 1);
						string text = _stFBName + "." + _fbNode.Variable.OrgName;
						IVariableMappingCollection variableMappings = _node.DataElement.IoMapping.VariableMappings;
						if (((ICollection)variableMappings).Count > 0)
						{
							IVariableMapping obj2 = variableMappings[0];
							IVariableMapping2 val3 = (IVariableMapping2)(object)((obj2 is IVariableMapping2) ? obj2 : null);
							_stOldMapping = ((IVariableMapping)val3).Variable;
							if (val3 is IVariableMapping3)
							{
								_stOldFBInstance = ((IVariableMapping3)((val3 is IVariableMapping3) ? val3 : null)).IoChannelFBInstance;
							}
							if (string.IsNullOrEmpty(text))
							{
								if (val3 != null && val3.DefaultVariable != string.Empty)
								{
									_stDefaultVariable = val3.DefaultVariable;
									variableMappings.RemoveAt(0);
								}
								else
								{
									variableMappings.RemoveAt(0);
								}
								_node.PlcNode.TreeModel.RaiseChanged(_node);
							}
						}
						if (!string.IsNullOrEmpty(text))
						{
							bool flag = true;
							if (text.Contains("."))
							{
								flag = false;
							}
							if ((int)_node.Parameter.ChannelType == 3)
							{
								flag = true;
							}
							if (((ICollection)variableMappings).Count == 0)
							{
								variableMappings.AddMapping(text, flag);
							}
							else
							{
								variableMappings[0].Variable=(text);
								variableMappings[0].CreateVariable=(flag);
							}
							if (variableMappings[0] is IVariableMapping3)
							{
								IVariableMapping obj3 = variableMappings[0];
								((IVariableMapping3)((obj3 is IVariableMapping3) ? obj3 : null)).IoChannelFBInstance=(_stFBName);
							}
							_node.PlcNode.TreeModel.RaiseChanged(_node);
						}
						if (_node.ParameterSetProvider is _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider)
						{
							(_node.ParameterSetProvider as _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider).StoreObject();
						}
						if (_editor?.InstancesModel != null)
						{
							_editor.InstancesModel.Refresh();
						}
						if (_editor?.GetFrame() is IParameterSetEditorFrame2 parameterSetEditorFrame2)
						{
							parameterSetEditorFrame2.UpdatePages(_editor as IBaseDeviceEditor);
						}
						return _node;
					}
				}
				return null;
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
