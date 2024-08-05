#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceEditor.CustomizedOnline;
using _3S.CoDeSys.DeviceEditor.SimpleMappingEditor;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.LegacyOnlineManager;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DataElementNode : ParameterNode, IParameterTreeNode, ITreeTableNode2, ITreeTableNode
	{
		private delegate void DoSetValueDelegate(int nColumnIndex, object value);

		private IPlcNode _plcNode;

		private ITreeTableNode _parent;

		private bool _bHasParamId;

		private uint _nParamId = uint.MaxValue;

		private uint _nIndexInDevDesc = uint.MaxValue;

		private string _stElementIdentifier;

		private List<IParameterTreeNode> _childNodes = new List<IParameterTreeNode>();

		private IConverterToIEC _converterToIec;

		private IConverterFromIEC _converterFromIec;

		private EnumerationTreeTableViewRenderer _enumerationRenderer;

		private EnumerationTreeTableViewEditor _enumerationEditor;

		private EnumerationTreeTableViewEditor _enumerationEditorWithEmptyValue;

		private IOnlineVarRef _onlineVarRef;

		private TypeClass _typeClass = (TypeClass)29;

		private ICompiledType _baseType;

		private string _stExceptionText = string.Empty;

		private static readonly Image s_paramImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.ParameterSmall.ico").Handle);

		private static readonly Image s_structuredParamImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.StructuredParameterSmall.ico").Handle);

		private static readonly Image s_inputChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.InputChannelSmall.ico").Handle);

		private static readonly Image s_outputChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.OutputChannelSmall.ico").Handle);

		private static readonly Image s_outputBidirectionalChannelImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.OutputBidirectionalChannelSmall.ico").Handle);

		private static readonly Image s_inputChannelSafetyImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.SafetyInputChannelSmall.ico").Handle);

		private static readonly Image s_outputChannelSafetyImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.SafetyOutputChannelSmall.ico").Handle);

		private static readonly Image s_manualAddressImage = Bitmap.FromHicon(((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(SectionNode), "_3S.CoDeSys.DeviceEditor.Resources.ManualAddress.ico").Handle);

		private string _stPreparedValue = string.Empty;

		private string _stOnlineValue = string.Empty;

		private string _stDevPath;

		private bool _bIsMapped;

		private IParameterSetProvider _parameterSetProvider;

		private IDataElement _dataElement;

		private IParameter _parameter;

		private bool _bModifiy;

		private int _nModificationCounter;

		private int _DecimalPlaces = -1;

		internal static readonly string HIDDENONLINECONFIGAPPLICATION = "HiddenOnlineConfigModeApp";

		public static string GVL_IOCONFIG_GLOBALS_MAPPING = "IoConfig_Globals_Mapping";

		public string PreparedOnlineParameter
		{
			get
			{
				return _stPreparedValue;
			}
			set
			{
				_stPreparedValue = value;
				_plcNode.TreeModel.RaiseChanged(this);
			}
		}

		public string OnlineOnlineParameter
		{
			get
			{
				return _stOnlineValue;
			}
			set
			{
				_stOnlineValue = value;
				_plcNode.TreeModel.RaiseChanged(this);
			}
		}

		public new IOnlineVarRef OnlineVarRef => _onlineVarRef;

		public IPlcNode PlcNode => _plcNode;

		public bool IsMapped => _bIsMapped;

		public IParameterSetProvider ParameterSetProvider => _parameterSetProvider;

		public string DevPath
		{
			get
			{
				if (_stDevPath == null)
				{
					if (_parent is IParameterTreeNode)
					{
						_stDevPath = (_parent as IParameterTreeNode).DevPath + DataElement.Identifier;
					}
					else
					{
						_stDevPath = DataElement.Identifier;
					}
				}
				return _stDevPath;
			}
		}

		public bool IsParameter => _bHasParamId;

		public long ParameterId
		{
			get
			{
				if (!_bHasParamId)
				{
					throw new InvalidOperationException("Node does not represent a parameter");
				}
				return _nParamId;
			}
		}

		public long ParameterIndexInDevDesc
		{
			get
			{
				if (!IsParameter)
				{
					throw new InvalidOperationException("Node does not represent a parameter");
				}
				return _nIndexInDevDesc;
			}
		}

		public new IParameter Parameter
		{
			get
			{
				if (_parameter == null)
				{
					UpdateData();
				}
				return _parameter;
			}
		}

		public new IDataElement DataElement
		{
			get
			{
				if (_dataElement == null)
				{
					UpdateData();
				}
				return _dataElement;
			}
		}

		public int DecimalPlaces
		{
			get
			{
				return _DecimalPlaces;
			}
			set
			{
				if (value != _DecimalPlaces)
				{
					_DecimalPlaces = value;
					_plcNode.TreeModel.RaiseChanged(this);
				}
			}
		}

		public IConverterToIEC ConverterToIec
		{
			get
			{
				return _converterToIec;
			}
			set
			{
				if (value == _converterToIec)
				{
					return;
				}
				_converterToIec = value;
				_plcNode.TreeModel.RaiseChanged(this);
				if (_childNodes == null)
				{
					return;
				}
				foreach (DataElementNode childNode in _childNodes)
				{
					childNode.ConverterToIec = value;
				}
			}
		}

		public IConverterFromIEC ConverterFromIec
		{
			get
			{
				return _converterFromIec;
			}
			set
			{
				if (value == _converterFromIec)
				{
					return;
				}
				_converterFromIec = value;
				_plcNode.TreeModel.RaiseChanged(this);
				if (_childNodes == null)
				{
					return;
				}
				foreach (DataElementNode childNode in _childNodes)
				{
					childNode.ConverterFromIec = value;
				}
			}
		}

		public bool MonitoringEnabled
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Invalid comparison between Unknown and I4
				try
				{
					return _onlineVarRef != null && (int)_onlineVarRef.State != 1;
				}
				catch
				{
					return false;
				}
			}
			set
			{
				try
				{
					if (_onlineVarRef != null && value != MonitoringEnabled)
					{
						if (value)
						{
							_onlineVarRef.ResumeMonitoring();
						}
						else
						{
							_onlineVarRef.SuspendMonitoring();
						}
					}
				}
				catch
				{
				}
			}
		}

		public bool IsOnline => _plcNode.OnlineApplication != Guid.Empty;

		public bool ReadOnly
		{
			get
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Invalid comparison between Unknown and I4
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Invalid comparison between Unknown and I4
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Invalid comparison between Unknown and I4
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Invalid comparison between Unknown and I4
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Invalid comparison between Unknown and I4
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Invalid comparison between Unknown and I4
				IParameter parameter = Parameter;
				bool flag = IsOnline;
				if (parameter != null)
				{
					flag = flag && (int)parameter.ChannelType == 0;
				}
				if (!(_dataElement is IParameter) && _dataElement is IDataElement6)
				{
					if (parameter != null)
					{
						AccessRight accessRight = Parameter.GetAccessRight(flag);
						if ((int)accessRight == 1 || (int)accessRight == 0)
						{
							return true;
						}
					}
					IDataElement dataElement = _dataElement;
					AccessRight accessRight2 = ((IDataElement6)((dataElement is IDataElement6) ? dataElement : null)).GetAccessRight(flag);
					if ((int)accessRight2 != 1)
					{
						return (int)accessRight2 == 0;
					}
					return true;
				}
				if (parameter != null)
				{
					AccessRight accessRight3 = Parameter.GetAccessRight(flag);
					if ((int)accessRight3 != 1)
					{
						return (int)accessRight3 == 0;
					}
					return true;
				}
				return true;
			}
		}

		public bool HasChildren => _childNodes.Count > 0;

		public int ChildCount => _childNodes.Count;

		public List<IParameterTreeNode> ChildNodes => _childNodes;

		public ITreeTableNode Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		private IVariableMapping VariableMapping
		{
			get
			{
				if (DataElement != null && DataElement.IoMapping != null && DataElement.IoMapping.VariableMappings != null && ((ICollection)DataElement.IoMapping.VariableMappings).Count > 0)
				{
					return DataElement.IoMapping.VariableMappings[0];
				}
				return null;
			}
		}

		public bool HasInvalidIecAddress
		{
			get
			{
				if (DataElement == null)
				{
					return false;
				}
				IVariableMapping variableMapping = VariableMapping;
				if (variableMapping == null)
				{
					return false;
				}
				if (variableMapping.CreateVariable)
				{
					return false;
				}
				return true;
			}
		}

		public DataElementNode(IParameterTreeTable model, IPlcNode plcNode, ITreeTableNode parent, IParameterSetProvider parameterSetProvider, IParameter parameter, IConverterToIEC converter)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (plcNode == null)
			{
				throw new ArgumentNullException("model");
			}
			_model = model;
			_node = (ITreeTableNode2)(object)this;
			_parameterSetProvider = parameterSetProvider;
			_plcNode = plcNode;
			_parent = parent;
			_bHasParamId = true;
			_nParamId = (uint)parameter.Id;
			if (parameter is IParameter6)
			{
				_nIndexInDevDesc = (uint)((IParameter6)((parameter is IParameter6) ? parameter : null)).IndexInDevDesc;
			}
			_converterToIec = converter;
			_converterFromIec = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
		}

		public DataElementNode(IParameterTreeTable model, IPlcNode plcNode, DataElementNode parent, IParameterSetProvider parameterSetProvider, IDataElement2 dataelement, string stElementIdentifier)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (plcNode == null)
			{
				throw new ArgumentNullException("model");
			}
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (stElementIdentifier == null)
			{
				throw new ArgumentNullException("stElementIdentifier");
			}
			_model = model;
			_plcNode = plcNode;
			_parameterSetProvider = parameterSetProvider;
			_parent = (ITreeTableNode)(object)parent;
			_bHasParamId = false;
			_stElementIdentifier = stElementIdentifier;
			if (string.IsInterned(_stElementIdentifier) == null)
			{
				_stElementIdentifier = string.Intern(_stElementIdentifier);
			}
			_converterToIec = parent.ConverterToIec;
			_converterFromIec = parent.ConverterFromIec;
		}

		public ITreeTableViewRenderer GetEnumerationRenderer()
		{
			Debug.Assert(DataElement.IsEnumeration);
			IDataElement dataElement = DataElement;
			IEnumerationDataElement val = (IEnumerationDataElement)(object)((dataElement is IEnumerationDataElement) ? dataElement : null);
			if (val == null)
			{
				return LabelTreeTableViewRenderer.NormalString;
			}
			if (_enumerationRenderer == null)
			{
				_enumerationRenderer = new EnumerationTreeTableViewRenderer(val.EnumerationValues);
			}
			return (ITreeTableViewRenderer)(object)_enumerationRenderer;
		}

		public ITreeTableViewEditor GetEnumerationEditor(bool bAllowEmptyValue)
		{
			Debug.Assert(DataElement.IsEnumeration);
			IDataElement dataElement = DataElement;
			IEnumerationDataElement val = (IEnumerationDataElement)(object)((dataElement is IEnumerationDataElement) ? dataElement : null);
			if (val == null)
			{
				return MyTextBoxTreeTableViewEditor.TextBox;
			}
			if (bAllowEmptyValue)
			{
				if (_enumerationEditorWithEmptyValue == null)
				{
					_enumerationEditorWithEmptyValue = new EnumerationTreeTableViewEditor(val.EnumerationValues, bAllowEmptyValue: true);
				}
				return (ITreeTableViewEditor)(object)_enumerationEditorWithEmptyValue;
			}
			if (_enumerationEditor == null)
			{
				_enumerationEditor = new EnumerationTreeTableViewEditor(val.EnumerationValues, bAllowEmptyValue: false);
			}
			return (ITreeTableViewEditor)(object)_enumerationEditor;
		}

		public DataElementNode Get(IParameter parameter, IDataElement dataelement, string[] path)
		{
			DataElementNode dataElementNode = null;
			if (_bHasParamId && _nParamId == parameter.Id)
			{
				if (path != null && path.Length != 0)
				{
					{
						foreach (IParameterTreeNode childNode in _childNodes)
						{
							dataElementNode = childNode.Get(parameter, dataelement, path);
							if (dataElementNode != null)
							{
								return dataElementNode;
							}
						}
						return dataElementNode;
					}
				}
				dataElementNode = this;
			}
			else if (path != null && path.Length != 0 && path[0].Equals(_stElementIdentifier))
			{
				if (path.Length != 1 || !(_stElementIdentifier == dataelement.Identifier))
				{
					string[] array = new string[path.Length - 1];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = path[i + 1];
					}
					{
						foreach (IParameterTreeNode childNode2 in _childNodes)
						{
							dataElementNode = childNode2.Get(parameter, dataelement, array);
							if (dataElementNode != null)
							{
								return dataElementNode;
							}
						}
						return dataElementNode;
					}
				}
				dataElementNode = this;
			}
			return dataElementNode;
		}

		internal void UpdateData()
		{
			_parameter = null;
			UpdateData(bModifiy: false);
		}

		internal void UpdateData(bool bModifiy)
		{
			IParameterSetProvider parameterSetProvider = _parameterSetProvider;
			object obj;
			if (parameterSetProvider == null)
			{
				obj = null;
			}
			else
			{
				IDeviceObject device = parameterSetProvider.GetDevice();
				obj = ((device != null) ? ((IObject)device).MetaObject : null);
			}
			IMetaObject val = (IMetaObject)obj;
			if (_parameter != null && _dataElement != null && _bModifiy == bModifiy && ((IMetaObject4)((val is IMetaObject4) ? val : null)).ModificationCounter == _nModificationCounter)
			{
				return;
			}
			_nModificationCounter = ((IMetaObject4)((val is IMetaObject4) ? val : null)).ModificationCounter;
			_bModifiy = bModifiy;
			if (_bHasParamId)
			{
				IParameterSet val2 = null;
				val2 = ((!bModifiy) ? _parameterSetProvider.GetParameterSet(bToModify: false) : _parameterSetProvider.GetParameterSet(bToModify: true));
				if (val2 != null)
				{
					_parameter = val2.GetParameter((long)_nParamId);
					_dataElement = (IDataElement)(object)_parameter;
				}
				else
				{
					_parameter = null;
					_dataElement = null;
				}
			}
			else
			{
				Debug.Assert(_parent is DataElementNode);
				((DataElementNode)(object)_parent).UpdateData(bModifiy);
				IParameter val3 = (_parameter = ((DataElementNode)(object)_parent).Parameter);
				IDataElement dataElement = ((DataElementNode)(object)_parent).DataElement;
				if (dataElement != null)
				{
					_dataElement = dataElement.SubElements[_stElementIdentifier];
				}
			}
		}

		public void AddDataElementNode(DataElementNode childNode)
		{
			if (childNode == null)
			{
				throw new ArgumentNullException("childNode");
			}
			_childNodes.Add(childNode);
		}

		public string GetVariableName()
		{
			if (DataElement == null)
			{
				return null;
			}
			IVariableMapping variableMapping = VariableMapping;
			if (variableMapping == null)
			{
				return null;
			}
			if (variableMapping.CreateVariable)
			{
				return variableMapping.Variable;
			}
			string text = variableMapping.Variable.Substring(variableMapping.Variable.IndexOf('.') + 1);
			if (text.Length > 0)
			{
				return text;
			}
			return null;
		}

		private bool Parameter_GetOnlineParameter(IParameter parameter)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (parameter is IParameter6)
			{
				return ((IParameter6)parameter).OnlineParameter;
			}
			if (parameter is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)parameter).IsFunctionAvailable("GetOnlineParameter"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				return XmlConvert.ToBoolean(((IGenericInterfaceExtensionProvider)parameter).CallFunction("GetOnlineParameter", xmlDocument).DocumentElement.InnerText);
			}
			return false;
		}

		internal static bool CheckForMultipleMapping(DataElementNode node, bool bNoMessage)
		{
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Invalid comparison between Unknown and I4
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Invalid comparison between Unknown and I4
			if (node != null)
			{
				if (node.DataElement is IDataElement2)
				{
					IDataElement dataElement = node.DataElement;
					IDataElement2 val = (IDataElement2)(object)((dataElement is IDataElement2) ? dataElement : null);
					if (!node.PlcNode.BaseTypeMappable && !val.HasBaseType)
					{
						if (!bNoMessage)
						{
							APEnvironment.MessageService.Information(Strings.ErrorStructuresNotMappable, "ErrorStructuresNotMappable", Array.Empty<object>());
						}
						return false;
					}
					if (!node.PlcNode.BitfieldMappable && ((IDataElement)val).HasSubElements && val.HasBaseType)
					{
						if (!bNoMessage)
						{
							APEnvironment.MessageService.Information(Strings.ErrorBitfieldNotMappable, "ErrorBitfieldNotMappable", Array.Empty<object>());
						}
						return false;
					}
				}
				DataElementNode dataElementNode = node;
				bool flag = true;
				while (dataElementNode != null)
				{
					IDataElement dataElement2 = dataElementNode.DataElement;
					if (dataElement2 is IDataElement5 && ((IDataElement5)((dataElement2 is IDataElement5) ? dataElement2 : null)).IsUnion)
					{
						if (flag && !node._plcNode.UnionRootEditable)
						{
							return false;
						}
						if (!flag && dataElement2.IoMapping != null && dataElement2.IoMapping.VariableMappings != null && ((ICollection)dataElement2.IoMapping.VariableMappings).Count > 0 && !dataElement2.IoMapping.VariableMappings[0]
							.CreateVariable)
						{
							return false;
						}
						return true;
					}
					dataElementNode = dataElementNode.Parent as DataElementNode;
					flag = false;
				}
				if (((int)node.Parameter.ChannelType == 2 || (int)node.Parameter.ChannelType == 3) && !node._plcNode.MultipleMappableAllowed)
				{
					if (node.DataElement.HasSubElements && !CheckSubMapping(node.DataElement, bOnlyCreateVariable: false) && (node.DataElement == null || node.DataElement.IoMapping == null || node.DataElement.IoMapping.VariableMappings == null || ((ICollection)node.DataElement.IoMapping.VariableMappings).Count <= 0))
					{
						if (!bNoMessage)
						{
							APEnvironment.MessageService.Information(Strings.ErrorMappingNotAllowed, "ErrorMappingNotAllowed", Array.Empty<object>());
						}
						return false;
					}
					for (DataElementNode dataElementNode2 = node.Parent as DataElementNode; dataElementNode2 != null; dataElementNode2 = dataElementNode2.Parent as DataElementNode)
					{
						IDataElement dataElement3 = dataElementNode2.DataElement;
						if (dataElement3 != null && dataElement3.IoMapping != null && dataElement3.IoMapping.VariableMappings != null && ((ICollection)dataElement3.IoMapping.VariableMappings).Count > 0 && (node.DataElement == null || node.DataElement.IoMapping == null || node.DataElement.IoMapping.VariableMappings == null || ((ICollection)node.DataElement.IoMapping.VariableMappings).Count <= 0))
						{
							if (!bNoMessage)
							{
								APEnvironment.MessageService.Information(Strings.ErrorMappingNotAllowed, "ErrorMappingNotAllowed", Array.Empty<object>());
							}
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static bool CheckUnionMapping(DataElementNode datanode)
		{
			DataElementNode dataElementNode = datanode;
			bool flag = true;
			while (dataElementNode != null)
			{
				IDataElement dataElement = dataElementNode.DataElement;
				if (dataElement is IDataElement5 && ((IDataElement5)((dataElement is IDataElement5) ? dataElement : null)).IsUnion)
				{
					if (!flag)
					{
						return false;
					}
					if (!CheckSubMapping(datanode.DataElement, bOnlyCreateVariable: false))
					{
						return false;
					}
				}
				dataElementNode = dataElementNode.Parent as DataElementNode;
				flag = false;
			}
			return true;
		}

		internal static bool CheckSubMapping(IDataElement datalement, bool bOnlyCreateVariable)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected O, but got Unknown
			if (datalement.HasSubElements)
			{
				foreach (IDataElement item in (IEnumerable)datalement.SubElements)
				{
					IDataElement val = item;
					if (val.IoMapping != null && val.IoMapping.VariableMappings != null && ((ICollection)val.IoMapping.VariableMappings).Count > 0)
					{
						foreach (IVariableMapping item2 in (IEnumerable)val.IoMapping.VariableMappings)
						{
							IVariableMapping val2 = item2;
							if (!bOnlyCreateVariable)
							{
								return false;
							}
							if (!val2.CreateVariable)
							{
								return false;
							}
						}
					}
					if (!CheckSubMapping(val, bOnlyCreateVariable))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void StartCustomizedMonitoring()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			IDataElement val = null;
			if (Parameter != null)
			{
				val = DataElement;
			}
			try
			{
				if (Parameter != null && val != null && _onlineVarRef == null)
				{
					_=Parameter.ChannelType;
				}
			}
			catch
			{
			}
		}

		public void StartMonitoring()
		{
			ILegacyOnlineManager val = null;
			IParameter parameter = Parameter;
			IDataElement val2 = null;
			if (parameter != null)
			{
				val2 = DataElement;
			}
			try
			{
				if (parameter == null || val2 == null || (_onlineVarRef != null && (int)_onlineVarRef.State != 5))
				{
					return;
				}
				IDeviceObject host;
				string iecAddress;
				IVarRef val3;
				string text;
				if ((int)parameter.ChannelType != 0)
				{
					host = _parameterSetProvider.GetHost();
					if (CustomizedOnlineHelper.HasCustomizedOnlineFunctionality(host))
					{
						val = APEnvironment.LegacyOnlineMgrOrNull;
					}
					iecAddress = DeviceHelper.GetIecAddress(DataElement, (_parent is DataElementNode) ? (_parent as DataElementNode).DataElement : null, _plcNode.MotorolaBitfields);
					if ((!(_plcNode.OnlineApplication != Guid.Empty) || iecAddress == null) && !CustomizedOnlineHelper.HasCustomizedOnlineFunctionality(host))
					{
						goto IL_0952;
					}
					val3 = null;
					text = GetVariableName();
					bool flag = false;
					if (_plcNode.OnlineApplication != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IObject)host).MetaObject.ProjectHandle, _plcNode.OnlineApplication))
					{
						flag = string.Compare(((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)host).MetaObject.ProjectHandle, _plcNode.OnlineApplication).Name, HIDDENONLINECONFIGAPPLICATION, ignoreCase: true) == 0;
					}
					if (text == null && !CheckSubMapping(DataElement, bOnlyCreateVariable: true))
					{
						return;
					}
					if (text == null && Parent != null)
					{
						try
						{
							DataElementNode dataElementNode = Parent as DataElementNode;
							if (dataElementNode != null && dataElementNode.DataElement != null)
							{
								while (dataElementNode != null)
								{
									IDataElement dataElement = dataElementNode.DataElement;
									if (dataElement is IDataElement5 && ((IDataElement5)((dataElement is IDataElement5) ? dataElement : null)).IsUnion)
									{
										IDataElement val4 = null;
										IVariableMapping val5 = null;
										foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
										{
											IDataElement val6 = item;
											if (val6 != null && val6.IoMapping != null && val6.IoMapping.VariableMappings != null && ((ICollection)val6.IoMapping.VariableMappings).Count > 0)
											{
												val5 = val6.IoMapping.VariableMappings[0];
												if (!val5.CreateVariable)
												{
													val4 = val6;
												}
											}
										}
										if (val4 != null && val5 != null && DataElement is IDataElement2 && (DataElement as IDataElement2).HasBaseType)
										{
											IScanner val7 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(DataElement.BaseType, false, false, false, false);
											IParser val8 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val7);
											_baseType = val8.ParseTypeDeclaration();
											text = val5.Variable.Substring(val5.Variable.IndexOf('.') + 1);
										}
									}
									if (dataElement != null && dataElement.IoMapping != null && dataElement.IoMapping.VariableMappings != null && ((ICollection)dataElement.IoMapping.VariableMappings).Count > 0)
									{
										IVariableMapping val9 = dataElement.IoMapping.VariableMappings[0];
										if (!val9.CreateVariable && DataElement is IDataElement2 && (DataElement as IDataElement2).HasBaseType)
										{
											IScanner val10 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(DataElement.BaseType, false, false, false, false);
											IParser val11 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val10);
											_baseType = val11.ParseTypeDeclaration();
											if (!flag)
											{
												text = val9.Variable.Substring(val9.Variable.IndexOf('.') + 1);
											}
										}
									}
									dataElementNode = dataElementNode.Parent as DataElementNode;
								}
							}
						}
						catch
						{
						}
					}
					IVariableMapping variableMapping = VariableMapping;
					if (flag && variableMapping != null && !variableMapping.CreateVariable)
					{
						text = string.Empty;
					}
					if (!string.IsNullOrEmpty(text))
					{
						int num = text.IndexOf(".");
						if (num >= 0)
						{
							string text2 = text.Substring(0, num);
							ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(_plcNode.OnlineApplication);
							if (compileContext != null && ((ICompileContextCommon)compileContext).GetSignature(text2) != null)
							{
								val3 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(_plcNode.OnlineApplication, text2, text);
							}
						}
						if (val3 == null)
						{
							val3 = ((val == null) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(_plcNode.OnlineApplication, text) : val.GetVarReference(((IObject)host).MetaObject.ObjectGuid, iecAddress));
						}
						goto IL_0772;
					}
					if (!(DataElement is IDataElement2) || !(DataElement as IDataElement2).HasBaseType)
					{
						return;
					}
					string text3 = string.Empty;
					string text4 = DataElement.BaseType.ToUpperInvariant();
					IScanner val12 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text4, false, false, false, false);
					IParser val13 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val12);
					_baseType = val13.ParseTypeDeclaration();
					if (_baseType == null || ((int)((IType)_baseType).Class != 15 && (int)((IType)_baseType).Class != 14))
					{
						if (_baseType is ISubrangeType2 && (int)((IType)_baseType).Class == 24)
						{
							ref ICompiledType baseType = ref _baseType;
							ICompiledType baseType2 = _baseType;
							baseType = ((ISubrangeType2)((baseType2 is ISubrangeType2) ? baseType2 : null)).BaseType;
							TypeClass @class = ((IType)_baseType).Class;
							text4 = @class.ToString().ToUpperInvariant();
						}
						switch (DataElement.GetBitSize())
						{
						case 8L:
							if (text4 != "BYTE" && text4 != "BOOL")
							{
								text3 = "BYTE_TO_" + text4 + "(" + iecAddress + ")";
							}
							break;
						case 16L:
							if (text4 != "WORD")
							{
								text3 = "WORD_TO_" + text4 + "(" + iecAddress + ")";
							}
							break;
						case 32L:
							if (text4 != "DWORD")
							{
								if (text4 == "DATE_AND_TIME")
								{
									text4 = "DT";
								}
								if (text4 == "TIME_OF_DAY")
								{
									text4 = "TOD";
								}
								text3 = "DWORD_TO_" + text4 + "(" + iecAddress + ")";
							}
							break;
						case 64L:
							if (text4 != "LWORD")
							{
								text3 = "LWORD_TO_" + DataElement.BaseType + "(" + iecAddress + ")";
							}
							break;
						}
					}
					if (!string.IsNullOrEmpty(text3))
					{
						val3 = ((val == null) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(_plcNode.OnlineApplication, text3) : val.GetVarReference(((IObject)host).MetaObject.ObjectGuid, iecAddress));
					}
					if (val3 == null || val3.WatchExpression.Type == null)
					{
						val3 = ((val == null) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(_plcNode.OnlineApplication, iecAddress) : val.GetVarReference(((IObject)host).MetaObject.ObjectGuid, iecAddress));
					}
					goto IL_0772;
				}
				if (val2 is IDataElement2)
				{
					if (parameter is IParameter17 && !string.IsNullOrEmpty(((IParameter17)((parameter is IParameter17) ? parameter : null)).FbInstanceVariable))
					{
						try
						{
							string watchFbInstanceVariable = ((IDataElement10)((val2 is IDataElement10) ? val2 : null)).WatchFbInstanceVariable;
							if (_plcNode.OnlineApplication != Guid.Empty && !string.IsNullOrEmpty(watchFbInstanceVariable))
							{
								IVarRef varReference = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetVarReference(_plcNode.OnlineApplication, watchFbInstanceVariable);
								_onlineVarRef = ((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(varReference);
								_bIsMapped = true;
							}
						}
						catch
						{
						}
					}
					else
					{
						bool num2 = Parameter_GetOnlineParameter(parameter);
						_bIsMapped = true;
						if (!num2)
						{
							IDataElement2 val14 = (IDataElement2)val2;
							if (val14.CanWatch)
							{
								_onlineVarRef = val14.CreateWatch();
							}
						}
					}
				}
				goto IL_0952;
				IL_0952:
				if (_onlineVarRef != null)
				{
					if (_onlineVarRef.Expression.Type != null)
					{
						_typeClass = ((IType)_onlineVarRef.Expression.Type).Class;
					}
					_onlineVarRef.SuspendMonitoring();
					_onlineVarRef.Changed+=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
				}
				goto end_IL_0016;
				IL_0772:
				Debug.Assert(val3 != null);
				Debug.Assert(host != null);
				_bIsMapped = false;
				bool flag2 = false;
				if (DataElement.IoMapping.VariableMappings != null && ((ICollection)DataElement.IoMapping.VariableMappings).Count > 0)
				{
					flag2 = DataElement.IoMapping.VariableMappings[0]
						.CreateVariable;
				}
				string text5 = ((string.IsNullOrEmpty(text) || flag2 || PlcNode.AlwaysMapToNew) ? iecAddress : text);
				foreach (Guid application in _plcNode.Applications)
				{
					_bIsMapped |= ((IDeviceManager4)APEnvironment.DeviceMgr).CheckIOChannelIsUpdated(((IObject)host).MetaObject.ProjectHandle, application, text5);
					if (_bIsMapped)
					{
						break;
					}
				}
				if (val != null)
				{
					_onlineVarRef = val.CreateWatch(val3, ((IObject)host).MetaObject.ObjectGuid);
					_bIsMapped = true;
				}
				else
				{
					_onlineVarRef = ((IOnlineManager)APEnvironment.OnlineMgr).CreateWatch(val3);
				}
				goto IL_0952;
				end_IL_0016:;
			}
			catch (Exception ex)
			{
				if (ex is InvalidVarRefException)
				{
					_stExceptionText = ex.Message;
				}
			}
		}

		public void ReleaseMonitoring(bool bClose)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			foreach (DataElementNode childNode in _childNodes)
			{
				childNode.ReleaseMonitoring(bClose);
			}
			if (_onlineVarRef != null && _onlineVarRef.PreparedValue == null)
			{
				_onlineVarRef.Release();
				_onlineVarRef.Changed-=(new OnlineVarRefEventHandler(OnOnlineVarRefChanged));
				_onlineVarRef = null;
				if (!bClose)
				{
					_plcNode.TreeModel.RaiseChanged(this);
				}
			}
		}

		internal bool CheckDefaultValueForInputs()
		{
			if (_model != null && _model.DefaultColumnForInputsEditable)
			{
				return false;
			}
			string value = _dataElement.Value;
			if (_dataElement.HasSubElements || string.IsNullOrEmpty(value))
			{
				return true;
			}
			try
			{
				object obj = default(object);
				TypeClass val = default(TypeClass);
				_converterFromIec.GetLiteralValue(value, out obj, out val);
				if (obj.GetType().IsValueType)
				{
					object value2 = 0;
					return (obj as ValueType).Equals(Convert.ChangeType(value2, obj.GetType()));
				}
			}
			catch
			{
			}
			return false;
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			return (ITreeTableNode)_childNodes[nIndex];
		}

		public int GetIndex(ITreeTableNode node)
		{
			return _childNodes.IndexOf(node as IParameterTreeNode);
		}

		public bool IsEditable(int nColumnIndex)
		{
			switch (_plcNode.TreeModel.MapColumn(nColumnIndex))
			{
			case 5:
				if (!_plcNode.DefaultColumnAvailable)
				{
					return false;
				}
				if ((int)_parameter.ChannelType != 0 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0))
				{
					DataElementNode dataElementNode3 = this;
					bool flag2 = false;
					while (dataElementNode3 != null)
					{
						object obj;
						if (dataElementNode3 == null)
						{
							obj = null;
						}
						else
						{
							IDataElement dataElement2 = dataElementNode3.DataElement;
							if (dataElement2 == null)
							{
								obj = null;
							}
							else
							{
								IIoMapping ioMapping = dataElement2.IoMapping;
								obj = ((ioMapping != null) ? ioMapping.VariableMappings : null);
							}
						}
						if (obj != null)
						{
							foreach (IVariableMapping item in (IEnumerable)dataElementNode3.DataElement.IoMapping.VariableMappings)
							{
								if (item.CreateVariable)
								{
									flag2 = true;
									break;
								}
							}
						}
						dataElementNode3 = dataElementNode3.Parent as DataElementNode;
					}
					if (!flag2)
					{
						return false;
					}
					if ((int)_parameter.ChannelType == 1 && CheckDefaultValueForInputs())
					{
						return false;
					}
				}
				if (DataElement is IDataElement2 && (DataElement as IDataElement2).HasBaseType)
					{
					return !ReadOnly & !IsOnline;
				}
				if (!_dataElement.HasSubElements && !ReadOnly)
				{
					return !IsOnline;
				}
				return false;
			case 7:
			{
				IParameter parameter = Parameter;
				IParameter6 val = (IParameter6)(object)((parameter is IParameter6) ? parameter : null);
				bool flag = false;
				if (val != null)
				{
					flag = val.OnlineParameter;
				}
				if (_onlineVarRef == null && !flag)
				{
					return false;
				}
				if ((int)Parameter.ChannelType != 0)
				{
					if (DataElement is IDataElement2 && (DataElement as IDataElement2).HasBaseType)
							{
						if ((int)_typeClass == 26 && _onlineVarRef != null && ((IType)_baseType).Class != ((IType)_onlineVarRef.Expression.Type.BaseType).Class && (_baseType.Size((IScope)null) != _onlineVarRef.Expression.Type.BaseType
							.Size((IScope)null) || !(_baseType as ICompiledType2).IsCompatible(_onlineVarRef.Expression.Type.BaseType, null)))
								{
							return false;
						}
						string text = ((IExprement)_onlineVarRef.Expression).ToString();
						if (!text.Contains(".") || text.Contains("%"))
						{
							if (_parent is DataElementNode)
							{
								DataElementNode dataElementNode = _parent as DataElementNode;
								if (dataElementNode.OnlineVarRef != null && (dataElementNode.OnlineVarRef.Forced || (dataElementNode.OnlineVarRef.PreparedValue != null && !string.IsNullOrEmpty(dataElementNode.OnlineVarRef.PreparedValue.ToString()))))
								{
									return false;
								}
							}
							for (int i = 0; i < ChildCount; i++)
							{
								DataElementNode dataElementNode2 = GetChild(i) as DataElementNode;
								if (dataElementNode2 != null && dataElementNode2.OnlineVarRef != null && (dataElementNode2.OnlineVarRef.Forced || (dataElementNode2.OnlineVarRef.PreparedValue != null && !string.IsNullOrEmpty(dataElementNode2.OnlineVarRef.PreparedValue.ToString()))))
								{
									return false;
								}
							}
						}
						return _plcNode.OnlineApplication != Guid.Empty;
					}
					return false;
				}
				if (Parameter is IParameter18 && (Parameter as IParameter18).PreparedValueAccess)
						{
					return !DataElement.HasSubElements;
				}
				AccessRight val2 = Parameter.GetAccessRight(true);
				if (!(_dataElement is IParameter) && _dataElement is IDataElement6)
				{
					IDataElement dataElement = _dataElement;
					AccessRight accessRight = ((IDataElement6)((dataElement is IDataElement6) ? dataElement : null)).GetAccessRight(false);
					if ((int)accessRight != 3)
					{
						val2 = accessRight;
					}
				}
				if (!DataElement.HasSubElements)
				{
					if ((int)val2 != 3)
					{
						return (int)val2 == 2;
					}
					return true;
				}
				return false;
			}
			case 6:
			{
				if ((int)Parameter.ChannelType != 0 && DataElement is IDataElement2 && (DataElement as IDataElement2).HasBaseType)
						{
					return _plcNode.OnlineApplication != Guid.Empty;
				}
				AccessRight val4 = Parameter.GetAccessRight(true);
				if (!(_dataElement is IParameter) && _dataElement is IDataElement6)
				{
					IDataElement dataElement3 = _dataElement;
					AccessRight accessRight2 = ((IDataElement6)((dataElement3 is IDataElement6) ? dataElement3 : null)).GetAccessRight(false);
					if ((int)accessRight2 != 3)
					{
						val4 = accessRight2;
					}
				}
				if (!DataElement.HasSubElements)
				{
					if ((int)val4 != 3)
					{
						return (int)val4 == 2;
					}
					return true;
				}
				return false;
			}
			case 0:
			{
				IParameter parameter2 = Parameter;
				IParameter20 val3 = (IParameter20)(object)((parameter2 is IParameter20) ? parameter2 : null);
				if (val3 != null && val3.DisableMapping)
				{
					return false;
				}
				if (!CheckForMultipleMapping(this, bNoMessage: false))
				{
					return false;
				}
				return _plcNode.OnlineApplication == Guid.Empty;
			}
			case 1:
				if ((int)Parameter.ChannelType == 3)
				{
					return false;
				}
				if (!CheckUnionMapping(this))
				{
					return false;
				}
				if (Parameter is IParameter9 && (Parameter as IParameter9).MapOnlyNew)
					{
					return false;
				}
				if (VariableMapping != null)
				{
					return _plcNode.OnlineApplication == Guid.Empty;
				}
				return false;
			case 3:
				if (!_plcNode.ManualAddress)
				{
					return false;
				}
				if (Parameter is IParameter8 && (Parameter as IParameter8).NoManualAddress)
					{
					return false;
				}
				if (_parent is DataElementNode)
				{
					return false;
				}
				return _plcNode.OnlineApplication == Guid.Empty;
			case 10:
				if (_parent != null && !(DataElement is IDataElement4))
				{
					return false;
				}
				return _plcNode.OnlineApplication == Guid.Empty;
			default:
				return false;
			}
		}

		public bool VariableEditable()
		{
			if (!CheckForMultipleMapping(this, bNoMessage: true))
			{
				return false;
			}
			return _plcNode.OnlineApplication == Guid.Empty;
		}

		public object GetValue(int nColumnIndex)
		{
			
			string text = string.Empty;
			IDataElement dataElement = DataElement;
			if (dataElement == null)
			{
				return text;
			}
			switch (_plcNode.TreeModel.MapColumn(nColumnIndex))
			{
			case 2:
				text = dataElement.VisibleName;
				break;
			case 4:
				text = dataElement.GetTypeString();
				break;
			case 5:
				if ((int)_parameter.ChannelType != 0 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0))
				{
					DataElementNode dataElementNode = this;
					bool flag = false;
					while (dataElementNode != null)
					{
						object obj2;
						if (dataElementNode == null)
						{
							obj2 = null;
						}
						else
						{
							IDataElement dataElement2 = dataElementNode.DataElement;
							if (dataElement2 == null)
							{
								obj2 = null;
							}
							else
							{
								IIoMapping ioMapping = dataElement2.IoMapping;
								obj2 = ((ioMapping != null) ? ioMapping.VariableMappings : null);
							}
						}
						if (obj2 != null)
						{
							foreach (IVariableMapping item in (IEnumerable)dataElementNode.DataElement.IoMapping.VariableMappings)
							{
								if (item.CreateVariable)
								{
									flag = true;
									break;
								}
							}
						}
						dataElementNode = dataElementNode.Parent as DataElementNode;
					}
					if (!flag)
					{
						return string.Empty;
					}
					if ((int)_parameter.ChannelType == 1 && CheckDefaultValueForInputs())
					{
						return string.Empty;
					}
				}
				if (dataElement.HasSubElements || !_plcNode.DefaultColumnAvailable)
				{
					if (_plcNode.DefaultColumnAvailable && dataElement is IDataElement2 && ((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).HasBaseType)
					{
						ulong num = 0uL;
						int num2 = 0;
						if (dataElement != null)
						{
							foreach (IDataElement2 item2 in (IEnumerable)dataElement.SubElements)
							{
								string value = ((IDataElement)item2).Value;
								if (value != null && value != string.Empty && (value.ToUpperInvariant() == "TRUE" || value == "1"))
								{
									num |= (ulong)(1L << num2);
								}
								num2++;
							}
						}
						try
						{
							if (_baseType == null)
							{
								string text2 = DataElement.BaseType.ToUpperInvariant();
								IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text2, false, false, false, false);
								IParser val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
								_baseType = val2.ParseTypeDeclaration();
							}
							if (_baseType != null && (int)((IType)_baseType).Class != 28)
							{
								return _converterToIec.GetInteger((object)num, ((IType)_baseType).Class);
							}
						}
						catch
						{
						}
					}
					return string.Empty;
				}
				if (dataElement.IsEnumeration && dataElement is IEnumerationDataElement)
				{
					return ((IEnumerationDataElement)dataElement).GetValueEnumerationIndex();
				}
				return dataElement.Value;
			case 7:
				if (dataElement is IParameter && Parameter_GetOnlineParameter((IParameter)(object)((dataElement is IParameter) ? dataElement : null)))
				{
					if (dataElement.IsEnumeration && dataElement is IEnumerationDataElement)
					{
						int nIndex2 = default(int);
						IEnumerationValue enumerationValue2 = ((IEnumerationDataElement)dataElement).GetEnumerationValue((object)_stPreparedValue, out nIndex2);
						if (enumerationValue2 == null)
						{
							return new EmptyValueData();
						}
						return new EnumValueData(enumerationValue2.VisibleName, nIndex2);
					}
					return _stPreparedValue;
				}
				if (_parameter == null || ((int)_parameter.ChannelType == 0 && dataElement.HasSubElements))
				{
					return string.Empty;
				}
				if (ConverterToIec == null)
				{
					return string.Empty;
				}
				if (_onlineVarRef == null || _onlineVarRef.PreparedValue == null)
				{
					return string.Empty;
				}
				if (_onlineVarRef.PreparedValue == PreparedValues.Unforce)
				{
					return "<Unforce>";
				}
				if (dataElement.IsEnumeration && dataElement is IEnumerationDataElement)
				{
					int nIndex3 = default(int);
					IEnumerationValue enumerationValue3 = ((IEnumerationDataElement)dataElement).GetEnumerationValue(_onlineVarRef.PreparedValue, out nIndex3);
					if (enumerationValue3 == null)
					{
						return new EmptyValueData();
					}
					return new EnumValueData(enumerationValue3.VisibleName, nIndex3);
				}
				if (((int)_typeClass == 26 || (int)_typeClass == 24) && _baseType == null)
				{
					return new RawValueData(_typeClass, ((IType)_onlineVarRef.Expression.Type.BaseType).Class, _onlineVarRef.PreparedValue, _converterToIec, bConstant: false, _onlineVarRef.Forced);
				}
				if ((int)_typeClass == 0 || (int)_typeClass == 1)
				{
					return GetOnlineValue(bPreparedValue: true);
				}
				return GetOnlineValue(bPreparedValue: true).ToString();
			case 8:
				if (dataElement.IsEnumeration && dataElement is IEnumerationDataElement)
				{
					int nIndex = default(int);
					IEnumerationValue enumerationValue = ((IEnumerationDataElement)dataElement).GetEnumerationValue((object)dataElement.DefaultValue, out nIndex);
					if (enumerationValue == null)
					{
						return new EmptyValueData();
					}
					return new EnumValueData(enumerationValue.VisibleName, nIndex);
				}
				text = ((dataElement.DefaultValue != null) ? dataElement.DefaultValue : string.Empty);
				break;
			case 9:
				text = dataElement.Unit;
				break;
			case 10:
				text = dataElement.Description;
				if (APEnvironment.LocalizationManagerOrNull != null && _parameterSetProvider.LocalizationActive)
				{
					text = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text, (LocalizationContent)8);
				}
				break;
			case 0:
				text = ((VariableMapping != null) ? VariableMapping.Variable : string.Empty);
				if (APEnvironment.LocalizationManagerOrNull != null && _parameterSetProvider.LocalizationActive)
				{
					text = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text, (LocalizationContent)2);
				}
				break;
			case 1:
				if (VariableMapping != null)
				{
					if (!VariableMapping.CreateVariable && !string.IsNullOrEmpty((VariableMapping as IVariableMapping3).IoChannelFBInstance))
						{
						return MappingTreeTableViewCellValue.MapToFBInstance;
					}
					return VariableMapping.CreateVariable ? MappingTreeTableViewCellValue.CreateVariable : MappingTreeTableViewCellValue.MapToExistingVariable;
				}
				return MappingTreeTableViewCellValue.None;
			case 3:
				try
				{
					text = DeviceHelper.GetIecAddress(DataElement, (_parent is DataElementNode) ? (_parent as DataElementNode).DataElement : null, _plcNode.MotorolaBitfields);
				}
				catch
				{
					text = string.Empty;
				}
				if (dataElement.IoMapping != null && dataElement.IoMapping.AutomaticIecAddress)
				{
					return (object)new IconLabelTreeTableViewCellData((Image)null, (object)text);
				}
				return (object)new IconLabelTreeTableViewCellData(s_manualAddressImage, (object)text);
			case 6:
				if (dataElement is IParameter && Parameter_GetOnlineParameter((IParameter)(object)((dataElement is IParameter) ? dataElement : null)))
				{
					return new EnumValueData(_stOnlineValue, 0);
				}
				return GetOnlineValue(bPreparedValue: false);
			}
			if (nColumnIndex == 0)
			{
				bool flag2 = false;
				try
				{
					if (dataElement is IDataElement2 && ((IDataElement2)((dataElement is IDataElement2) ? dataElement : null)).HasBaseType)
					{
						flag2 = dataElement.BaseType.ToLowerInvariant().StartsWith("safe");
					}
				}
				catch
				{
				}
				Image image = ((dataElement.HasSubElements && (int)_parameter.ChannelType == 0) ? s_structuredParamImage : (((int)_parameter.ChannelType == 1) ? ((!flag2) ? s_inputChannelImage : s_inputChannelSafetyImage) : (((int)_parameter.ChannelType == 2) ? (flag2 ? s_outputChannelSafetyImage : ((!(_parameter is IParameter21) || !(Parameter as IParameter21).BidirectionalOutput) ? s_outputChannelImage : s_outputBidirectionalChannelImage)) : (((int)_parameter.ChannelType != 3) ? s_paramImage : (flag2 ? s_outputChannelSafetyImage : ((!(_parameter is IParameter21) || !(Parameter as IParameter21).BidirectionalOutput) ? s_outputChannelImage : s_outputBidirectionalChannelImage))))));
				return (object)new IconLabelTreeTableViewCellData(image, (object)text);
			}
			return text;
		}

		private void RefreshAllChildren(DataElementNode node)
		{
			if (!node.HasChildren)
			{
				return;
			}
			for (int i = 0; i < node.ChildCount; i++)
			{
				DataElementNode dataElementNode = node.GetChild(i) as DataElementNode;
				if (dataElementNode != null)
				{
					RefreshAllChildren(dataElementNode);
					_plcNode.TreeModel.RaiseChanged(dataElementNode);
				}
			}
		}

		public void SetValue(int nColumnIndex, object value)
		{
			if (DeviceEditorAutomaticRefactoringOperationDefinitions.AutoRenameVarEnabled && _plcNode.TreeModel.MapColumn(nColumnIndex) == 0)
			{
				((IEngine)APEnvironment.Engine).InvokeInPrimaryThread((Delegate)new DoSetValueDelegate(DoSetValueWithTryCatch), new object[2] { nColumnIndex, value }, true);
			}
			else
			{
				DoSetValue(nColumnIndex, value);
			}
		}

		private void DoSetValueWithTryCatch(int nColumnIndex, object value)
		{
			try
			{
				DoSetValue(nColumnIndex, value);
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(ex.Message, "Error_DoSetValue", Array.Empty<object>());
			}
		}

		private void DoSetValue(int nColumnIndex, object value)
		{
			bool flag = false;
			if (value == null)
			{
				return;
			}
			string text = ((!(value is IconLabelTreeTableViewCellData)) ? value.ToString() : ((IconLabelTreeTableViewCellData)value).Label.ToString());
			if (_plcNode.TreeModel.MapColumn(nColumnIndex) == 7)
			{
				UpdateData(bModifiy: false);
			}
			else
			{
				UpdateData(bModifiy: true);
			}
			switch (_plcNode.TreeModel.MapColumn(nColumnIndex))
			{
			case 5:
			{
				string text6 = null;
				bool flag6 = false;
				if (DataElement.IsEnumeration && DataElement is IEnumerationDataElement)
				{
					int num14 = (int)value;
					if (num14 >= 0)
					{
						DataElement.EnumerationValue=(((IEnumerationDataElement)DataElement).EnumerationValues[num14]);
						flag = true;
						flag6 = true;
					}
				}
				else if (!DataElement.HasSubElements || (DataElement is IDataElement2 && (DataElement as IDataElement2).HasBaseType))
						{
					if (DataElement.IsRangeType)
					{
						LList<object> objectList2 = new LList<object>();
						if (CheckValue(text, objectList2, out var _))
						{
							flag6 = true;
							string text7 = DataElement.BaseType.ToUpperInvariant();
							IConverterFromIEC converterFromIEC2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
							object value2 = default(object);
							TypeClass val14 = default(TypeClass);
							switch (text7)
							{
							case "REAL":
							case "LREAL":
								try
								{
									CultureInfo provider = new CultureInfo("en-US");
									if (double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out var result) & double.TryParse(DataElement.MinValue, NumberStyles.Float | NumberStyles.AllowThousands, provider, out var result2) & double.TryParse(DataElement.MaxValue, NumberStyles.Float | NumberStyles.AllowThousands, provider, out var result3))
									{
										if (result >= result2 && result <= result3)
										{
											text6 = text;
										}
										if (result > result3)
										{
											text6 = DataElement.MaxValue;
										}
										if (result < result2)
										{
											text6 = DataElement.MinValue;
										}
									}
								}
								catch
								{
								}
								break;
							case "UINT":
							case "USINT":
							case "UDINT":
							case "ULINT":
							case "WORD":
							case "DWORD":
							case "LWORD":
							case "BYTE":
								try
								{
									converterFromIEC2.GetInteger(text, out value2, out val14);
									ulong num18 = Convert.ToUInt64(value2);
									converterFromIEC2.GetInteger(DataElement.MinValue, out value2, out val14);
									ulong num19 = Convert.ToUInt64(value2);
									converterFromIEC2.GetInteger(DataElement.MaxValue, out value2, out val14);
									ulong num20 = Convert.ToUInt64(value2);
									if (num18 >= num19 && num18 <= num20)
									{
										text6 = text;
									}
									if (num18 > num20)
									{
										text6 = DataElement.MaxValue;
									}
									if (num18 < num19)
									{
										text6 = DataElement.MinValue;
									}
								}
								catch
								{
								}
								break;
							case "INT":
							case "SINT":
							case "DINT":
							case "LINT":
								try
								{
									converterFromIEC2.GetInteger(text, out value2, out val14);
									long num15 = Convert.ToInt64(value2);
									converterFromIEC2.GetInteger(DataElement.MinValue, out value2, out val14);
									long num16 = Convert.ToInt64(value2);
									converterFromIEC2.GetInteger(DataElement.MaxValue, out value2, out val14);
									long num17 = Convert.ToInt64(value2);
									if (num15 >= num16 && num15 <= num17)
									{
										text6 = text;
									}
									if (num15 > num17)
									{
										text6 = DataElement.MaxValue;
									}
									if (num15 < num16)
									{
										text6 = DataElement.MinValue;
									}
								}
								catch
								{
								}
								break;
							case "TIME":
							case "LTIME":
								try
								{
									long duration = converterFromIEC2.GetDuration(text);
									long duration2 = converterFromIEC2.GetDuration(DataElement.MinValue);
									long duration3 = converterFromIEC2.GetDuration(DataElement.MaxValue);
									if (duration >= duration2 && duration <= duration3)
									{
										text6 = text;
									}
									if (duration > duration3)
									{
										text6 = DataElement.MaxValue;
									}
									if (duration < duration2)
									{
										text6 = DataElement.MinValue;
									}
								}
								catch
								{
								}
								break;
							default:
								text6 = text;
								break;
							}
						}
					}
					else
					{
						LList<object> objectList3 = new LList<object>();
						if (DataElement.BaseType == "STRING")
						{
							if (!text.StartsWith("'"))
							{
								text = "'" + text;
							}
							if (!text.EndsWith("'"))
							{
								text += "'";
							}
						}
						if (DataElement.BaseType == "WSTRING")
						{
							if (!text.StartsWith("\""))
							{
								text = "\"" + text;
							}
							if (!text.EndsWith("\""))
							{
								text += "\"";
							}
						}
						if (DataElement.BaseType.StartsWith("ARRAY"))
						{
							if (!text.StartsWith("["))
							{
								text = "[" + text;
							}
							if (!text.EndsWith("]"))
							{
								text += "]";
							}
						}
						if (Parameter is IParameter7 && (Parameter as IParameter7).IsFileType)
						{
							text6 = text;
							break;
						}
						if (CheckValue(text, objectList3, out var _))
						{
							flag6 = true;
							text6 = text;
							flag = true;
						}
					}
				}
				if (text6 != null)
				{
					if (_plcNode.TreeModel.UndoManager != null)
					{
						try
						{
							ValueAction valueAction2 = new ValueAction(this, bIsComment: false, text6);
							_plcNode.TreeModel.UndoManager.AddAction((IUndoableAction)(object)valueAction2);
							valueAction2.Redo();
							flag = true;
						}
						catch
						{
						}
					}
					else
					{
						DataElement.Value=(text6);
					}
					flag = true;
				}
				if (!flag6 && (DataElement as IDataElement2).HasBaseType && !string.IsNullOrEmpty(value.ToString()))
				{
					throw new Exception(string.Format(Strings.ErrorValueNotCompatible, value.ToString(), DataElement.BaseType));
				}
				break;
			}
			case 6:
			case 7:
			{
				bool flag2 = false;
				if (_plcNode.TreeModel.MapColumn(nColumnIndex) == 6 && !_plcNode.IsConfigModeOnlineApplication)
				{
					break;
				}
				if ((int)_parameter.ChannelType == 0)
				{
					Debug.Assert(!DataElement.HasSubElements);
				}
				bool flag3 = false;
				if (DataElement is IParameter)
				{
					IDataElement dataElement3 = DataElement;
					flag3 = (flag3 = Parameter_GetOnlineParameter((IParameter)(object)((dataElement3 is IParameter) ? dataElement3 : null)));
				}
				string text3;
				if (DataElement.IsEnumeration && DataElement is IEnumerationDataElement)
				{
					int num = (int)value;
					if (num >= 0)
					{
						IEnumerationValue val6 = ((IEnumerationDataElement)DataElement).EnumerationValues[num];
						text3 = ((IEnumerationDataElement)DataElement).GetValueForEnumeration(val6);
					}
					else
					{
						text3 = "";
						flag2 = true;
					}
				}
				else
				{
					text3 = (string)value;
				}
				if (!flag3)
				{
					Debug.Assert(_onlineVarRef != null);
					if (text3 == null || text3.Length == 0)
					{
						_onlineVarRef.PreparedValue=((object)null);
					}
					else
					{
						LList<object> val7 = new LList<object>();
						if (DataElement.BaseType == "STRING")
						{
							if (!text3.StartsWith("'"))
							{
								text3 = "'" + text3;
							}
							if (!text3.EndsWith("'"))
							{
								text3 += "'";
							}
						}
						if (DataElement.BaseType == "WSTRING")
						{
							if (!text3.StartsWith("\""))
							{
								text3 = "\"" + text;
							}
							if (!text3.EndsWith("\""))
							{
								text3 += "\"";
							}
						}
						if (DataElement.BaseType.StartsWith("ARRAY"))
						{
							if (!text3.StartsWith("["))
							{
								text3 = "[" + text3;
							}
							if (!text3.EndsWith("]"))
							{
								text3 += "]";
							}
						}
						TypeClass baseClass;
						bool flag4 = CheckValue(text3, val7, out baseClass);
						if (!flag4)
						{
							object obj4 = null;
							IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
							try
							{
								TypeClass val8 = default(TypeClass);
								switch ((int)baseClass - 6)
								{
								case 1:
								{
									converterFromIEC.GetInteger(text3, out obj4, out val8);
									byte[] bytes3 = BitConverter.GetBytes((ulong)obj4);
									flag2 = true;
									for (int k = 2; k < bytes3.Length; k++)
									{
										if (bytes3[k] != 0)
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										short num4 = BitConverter.ToInt16(bytes3, 0);
										_onlineVarRef.PreparedValue=((object)num4.ToString());
									}
									break;
								}
								case 2:
								{
									converterFromIEC.GetInteger(text3, out obj4, out val8);
									byte[] bytes2 = BitConverter.GetBytes((ulong)obj4);
									flag2 = true;
									for (int j = 4; j < bytes2.Length; j++)
									{
										if (bytes2[j] != 0)
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										int num3 = BitConverter.ToInt32(bytes2, 0);
										_onlineVarRef.PreparedValue=((object)num3.ToString());
									}
									break;
								}
								case 3:
								{
									converterFromIEC.GetInteger(text3, out obj4, out val8);
									long num2 = BitConverter.ToInt64(BitConverter.GetBytes((ulong)obj4), 0);
									_onlineVarRef.PreparedValue=((object)num2.ToString());
									flag2 = true;
									break;
								}
								case 0:
								{
									converterFromIEC.GetInteger(text3, out obj4, out baseClass);
									byte[] bytes = BitConverter.GetBytes((ulong)obj4);
									flag2 = true;
									for (int i = 1; i < bytes.Length; i++)
									{
										if (bytes[i] != 0)
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										sbyte b = (sbyte)bytes[0];
										_onlineVarRef.PreparedValue=((object)b.ToString());
									}
									break;
								}
								}
							}
							catch
							{
							}
						}
						if (flag4)
						{
							flag2 = true;
							if ((int)_typeClass == 26)
							{
								object[] array = ((_onlineVarRef.PreparedValue == null) ? (_onlineVarRef.Value as object[]) : (_onlineVarRef.PreparedValue as object[]));
								int num5 = array.Length;
								object[] array2 = new object[num5];
								int num6 = -1;
								if (Parent != null)
								{
									num6 = Parent.GetIndex((ITreeTableNode)(object)this);
								}
								int num7 = 0;
								for (int l = 0; l < num5; l++)
								{
									if (num7 < val7.Count && (num6 == l || Parent == null))
									{
										array2[l] = val7[num7];
										num7++;
									}
									else if (_onlineVarRef.PreparedValue == null)
									{
										switch ((int)baseClass)
										{
										case 2:
										case 3:
										case 4:
										case 5:
										case 6:
										case 7:
										case 8:
										case 9:
										case 10:
										case 11:
										case 12:
										case 13:
											array2[l] = 0;
											break;
										case 0:
										case 1:
											array2[l] = false;
											break;
										case 16:
											array2[l] = "";
											break;
										case 17:
											array2[l] = "";
											break;
										case 14:
										case 15:
											array2[l] = 0.0;
											break;
										case 18:
											array2[l] = 0L;
											break;
										case 37:
											array2[l] = 0uL;
											break;
										case 19:
										case 20:
										case 21:
											array2[l] = default(DateTime);
											break;
										}
									}
									else
									{
										array2[l] = array[l];
									}
								}
								_onlineVarRef.PreparedValue=((object)array2);
							}
							else
							{
								if ((int)_typeClass == 0 && (text3 == "0" || text3 == "1"))
								{
									text3 = "BOOL#" + text3;
								}
								object obj6 = default(object);
								TypeClass val9 = default(TypeClass);
								_converterFromIec.GetLiteralValue(text3, out obj6, out val9);
								if (_baseType != null)
								{
									string text4 = ((IExprement)_onlineVarRef.Expression).ToString();
									if (!text4.Contains("%") || ((IType)_baseType).Class != _typeClass)
									{
										try
										{
											ByteOrder val10 = (ByteOrder)0;
											short num8 = 0;
											DataElementNode dataElementNode = this;
											bool flag5 = false;
											ICompiledType val11 = null;
											do
											{
												IDataElement dataElement4 = dataElementNode.DataElement;
												if (dataElement4 != null && dataElement4.IoMapping != null && dataElement4.IoMapping.VariableMappings != null && ((ICollection)dataElement4.IoMapping.VariableMappings).Count > 0 && dataElement4.IoMapping.VariableMappings[0]
													.Variable
													.Contains(text4))
												{
													flag5 = true;
													if (dataElement4 is IDataElement4)
													{
														num8 = (short)((IDataElement4)((dataElement4 is IDataElement4) ? dataElement4 : null)).GetBitOffset();
													}
													_ = (dataElement4.GetBitSize() + 7) / 8;
													if (dataElement4 is IDataElement2 && ((IDataElement2)((dataElement4 is IDataElement2) ? dataElement4 : null)).HasBaseType)
													{
														IScanner val12 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(dataElement4.BaseType, false, false, false, false);
														val11 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val12).ParseTypeDeclaration();
													}
												}
												else
												{
													dataElementNode = dataElementNode.Parent as DataElementNode;
												}
											}
											while (!flag5 && dataElementNode != null);
											IDataElement dataElement5 = DataElement;
											short num9 = (short)(((IDataElement4)((dataElement5 is IDataElement4) ? dataElement5 : null)).GetBitOffset() - num8);
											ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(_plcNode.OnlineApplication);
											if (referenceContextIfAvailable != null && referenceContextIfAvailable.Codegenerator != null)
											{
												val10 = (ByteOrder)(referenceContextIfAvailable.Codegenerator.MotorolaByteOrder ? 1 : 0);
											}
											short num10 = (short)((DataElement.GetBitSize() + 7) / 8);
											byte[] array3 = ((_onlineVarRef.PreparedRawValue == null) ? ((byte[])_onlineVarRef.RawValue.Clone()) : _onlineVarRef.PreparedRawValue);
											if ((int)((IType)_baseType).Class == 0 || (int)((IType)_baseType).Class == 1)
											{
												short num11 = 0;
												num11 = (_plcNode.MotorolaBitfields ? ((short)(_onlineVarRef.PreparedRawValue.Length - num10 - num9 / 8)) : ((short)(num9 / 8)));
												if (num11 < array3.Length)
												{
													if (text3 == "0" || text3 == "FALSE")
													{
														array3[num11] &= (byte)(~(1 << num9 % 8));
													}
													else
													{
														array3[num11] |= (byte)(1 << num9 % 8);
													}
												}
											}
											else if (val11 != null)
											{
												byte[] array4 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj6, (IType)(object)val11, _plcNode.OnlineApplication, val10);
												short num12 = (short)(num9 / 8);
												if (array3.Length >= array4.Length + num12)
												{
													array4.CopyTo(array3, num12);
												}
											}
											else
											{
												byte[] array5 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj6, (IType)(object)_baseType, _plcNode.OnlineApplication, val10);
												short num13 = (short)(num9 / 8);
												if (array3.Length >= array5.Length + num13)
												{
													array5.CopyTo(array3, num13);
												}
												else
												{
													array3 = array5;
												}
												if (_onlineVarRef is IOnlineVarRef5)
												{
													IOnlineVarRef onlineVarRef = _onlineVarRef;
													((IOnlineVarRef5)((onlineVarRef is IOnlineVarRef5) ? onlineVarRef : null)).SetPreparedRawValue(array3);
												}
												flag = true;
											}
											if (val11 != null)
											{
												object preparedValue = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertRaw(array3, (IType)(object)val11, _plcNode.OnlineApplication, val10);
												_onlineVarRef.PreparedValue=(preparedValue);
												flag = true;
											}
										}
										catch
										{
										}
									}
								}
								if (!flag)
								{
									try
									{
										if ((int)_typeClass == 1)
										{
											_onlineVarRef.PreparedValue=((object)((!(text3 == "0") && !(text3 == "FALSE")) ? true : false));
										}
										else
										{
											_onlineVarRef.PreparedValue=(obj6);
										}
									}
									catch
									{
									}
									flag = true;
								}
							}
						}
					}
				}
				else
				{
					LList<object> objectList = new LList<object>();
					if (CheckValue(text3, objectList, out var _))
					{
						flag2 = true;
						flag = true;
						_stPreparedValue = text3;
					}
				}
				if (flag)
				{
					if (_plcNode.IsConfigModeOnlineApplication)
					{
						IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(_plcNode.OnlineApplication);
						if (application != null && application.IsLoggedIn)
						{
							application.WriteVariables((IOnlineVarRef[])(object)new IOnlineVarRef[1] { _onlineVarRef });
						}
					}
					if (!flag3)
					{
						flag = false;
					}
				}
				if (!flag2 && (DataElement as IDataElement2).HasBaseType && !string.IsNullOrEmpty(value.ToString()))
				{
					throw new Exception(string.Format(Strings.ErrorValueNotCompatible, value.ToString(), DataElement.BaseType));
				}
				break;
			}
			case 0:
			{
				IVariableMappingCollection variableMappings = DataElement.IoMapping.VariableMappings;
				Debug.Assert(variableMappings != null);
				string text2 = text.Trim();
				if (!text2.Contains(".") && !string.IsNullOrEmpty(text2))
				{
					IScanner obj2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text2, true, true, true, true);
					Debug.Assert(obj2 != null);
					IToken val5 = default(IToken);
					if (obj2.Match((TokenType)13, true, out val5) == -1)
					{
						throw new Exception(Strings.ErrorInvalidIdentifier);
					}
				}
				if (text2.Length > 0)
				{
					bool createVariable = true;
					if (text2.Contains("."))
					{
						createVariable = false;
						if (!CheckUnionMapping(this) || (Parameter is IParameter9 && (Parameter as IParameter9).MapOnlyNew))
								{
							return;
						}
					}
					if (_plcNode.TreeModel.UndoManager == null)
					{
						if (((ICollection)variableMappings).Count == 0)
						{
							variableMappings.AddMapping(text2, true);
						}
						else if ((int)CheckRefactoring(text2) == 1)
						{
							variableMappings[0].Variable=(text2);
						}
						if ((int)Parameter.ChannelType == 3)
						{
							createVariable = true;
						}
						if (((ICollection)variableMappings).Count > 0)
						{
							variableMappings[0].CreateVariable=(createVariable);
						}
					}
				}
				else if (((ICollection)variableMappings).Count > 0 && _plcNode.TreeModel.UndoManager == null)
				{
					variableMappings.RemoveAt(0);
				}
				if (_plcNode.TreeModel.UndoManager != null)
				{
					try
					{
						if (((((ICollection)variableMappings).Count > 0 && variableMappings[0].Variable != text2) || (((ICollection)variableMappings).Count == 0 && !string.IsNullOrEmpty(text2))) && (int)CheckRefactoring(text2) == 1)
						{
							MappingAction mappingAction = new MappingAction(this, text2, bEmptyMapping: false);
							_plcNode.TreeModel.UndoManager.AddAction((IUndoableAction)(object)mappingAction);
							mappingAction.Redo();
						}
					}
					catch
					{
					}
				}
				flag = true;
				RefreshAllChildren(this);
				break;
			}
			case 1:
				if (VariableMapping != null)
				{
					VariableMapping.CreateVariable=((MappingTreeTableViewCellValue)value == MappingTreeTableViewCellValue.CreateVariable);
					flag = true;
				}
				break;
			case 3:
				if (DataElement.IoMapping == null)
				{
					break;
				}
				try
				{
					IconLabelTreeTableViewCellData val13 = (IconLabelTreeTableViewCellData)value;
					if (val13.Label.ToString().Length == 0)
					{
						if (_plcNode.TreeModel.UndoManager != null)
						{
							try
							{
								AddressAction addressAction = new AddressAction(this, bAutomaticAddress: true, string.Empty);
								_plcNode.TreeModel.UndoManager.AddAction((IUndoableAction)(object)addressAction);
								addressAction.Redo();
							}
							catch
							{
							}
						}
						else
						{
							DataElement.IoMapping.AutomaticIecAddress=(true);
						}
						flag = true;
						break;
					}
					string text5 = val13.Label.ToString();
					if (!CheckAddress(val13.Label.ToString()) || (((int)Parameter.ChannelType != 1 || !text5.Contains("%I")) && ((int)Parameter.ChannelType != 2 || !text5.Contains("%Q")) && ((int)Parameter.ChannelType != 3 || !text5.Contains("%Q"))))
					{
						break;
					}
					if (_plcNode.TreeModel.UndoManager != null)
					{
						try
						{
							AddressAction addressAction2 = new AddressAction(this, bAutomaticAddress: false, text5);
							_plcNode.TreeModel.UndoManager.AddAction((IUndoableAction)(object)addressAction2);
							addressAction2.Redo();
						}
						catch
						{
						}
					}
					else
					{
						DataElement.IoMapping.AutomaticIecAddress=(false);
						DataElement.IoMapping.IecAddress=(text5);
					}
					flag = true;
				}
				catch
				{
				}
				break;
			case 10:
			{
				if (_plcNode.TreeModel.UndoManager != null)
				{
					try
					{
						ValueAction valueAction = new ValueAction(this, bIsComment: true, value.ToString());
						_plcNode.TreeModel.UndoManager.AddAction((IUndoableAction)(object)valueAction);
						valueAction.Redo();
						flag = true;
					}
					catch
					{
					}
					break;
				}
				IDataElement dataElement = DataElement;
				IDataElement4 val = (IDataElement4)(object)((dataElement is IDataElement4) ? dataElement : null);
				if (val != null)
				{
					IParameterSet parameterSet = ParameterSetProvider.GetParameterSet(bToModify: true);
					IParameterSet2 val2 = (IParameterSet2)(object)((parameterSet is IParameterSet2) ? parameterSet : null);
					if (val2 != null)
					{
						IStringTable stringTable = val2.StringTable;
						if (stringTable != null)
						{
							IStringRef description = stringTable.CreateStringRef("", "", value.ToString());
							val.SetDescription(description);
							flag = true;
						}
					}
					break;
				}
				IDataElement dataElement2 = DataElement;
				IParameter3 val3 = (IParameter3)(object)((dataElement2 is IParameter3) ? dataElement2 : null);
				if (val3 == null)
				{
					break;
				}
				IParameterSet parameterSet2 = ParameterSetProvider.GetParameterSet(bToModify: true);
				IParameterSet2 val4 = (IParameterSet2)(object)((parameterSet2 is IParameterSet2) ? parameterSet2 : null);
				if (val4 != null)
				{
					IStringTable stringTable2 = val4.StringTable;
					if (stringTable2 != null)
					{
						IStringRef description2 = stringTable2.CreateStringRef("", "", value.ToString());
						val3.SetDescription(description2);
						flag = true;
					}
				}
				break;
			}
			}
			if (!flag)
			{
				return;
			}
			_plcNode.TreeModel.RaiseChanged(this);
			if (DataElement.HasSubElements && (DataElement as IDataElement2).HasBaseType)
			{
				for (int m = 0; m < _childNodes.Count; m++)
				{
					IParameterTreeNode parameterTreeNode = _childNodes[m];
					if (parameterTreeNode is DataElementNode)
					{
						(parameterTreeNode as DataElementNode).UpdateData();
					}
					_plcNode.TreeModel.RaiseChanged(parameterTreeNode);
				}
			}
			if (_parent != null && _parent is IParameterTreeNode)
			{
				_plcNode.TreeModel.RaiseChanged(_parent as IParameterTreeNode);
			}
		}

		private bool CheckAddress(string st)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			if (st == null)
			{
				return false;
			}
			IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(st, false, false, false, false);
			IToken val2 = default(IToken);
			if (val.Match((TokenType)7, true, out val2) <= 0)
			{
				return false;
			}
			DirectVariableLocation val3 = default(DirectVariableLocation);
			DirectVariableSize val4 = default(DirectVariableSize);
			int[] array = default(int[]);
			bool flag = default(bool);
			val.GetDirectVariable(val2, out val3, out val4, out array, out flag);
			if (flag)
			{
				return false;
			}
			val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(DataElement.IoMapping.IecAddress, false, false, false, false);
			if (val.Match((TokenType)7, true, out val2) <= 0)
			{
				return false;
			}
			DirectVariableLocation val5 = default(DirectVariableLocation);
			DirectVariableSize val6 = default(DirectVariableSize);
			int[] array2 = default(int[]);
			val.GetDirectVariable(val2, out val5, out val6, out array2, out flag);
			if (val5 != val3 || val6 != val4 || array2.Length != array.Length)
			{
				return false;
			}
			return true;
		}

		internal RefactoringContextType GetRefactoringContext(out Guid objectGuid, out string stName)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			if (((ICollection)DataElement.IoMapping.VariableMappings).Count > 0)
			{
				IDeviceObject host = ParameterSetProvider.GetHost();
				IDeviceObject2 val = (IDeviceObject2)(object)((host is IDeviceObject2) ? host : null);
				if (val != null)
				{
					IDriverInfo5 val2 = (IDriverInfo5)val.DriverInfo;
					if (val2 != null)
					{
						IPreCompileContext2 val3 = (IPreCompileContext2)((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IDriverInfo2)val2).IoApplication);
						if (val3 != null)
						{
							ISignature[] array = ((IPreCompileContext)val3).FindSignature(GVL_IOCONFIG_GLOBALS_MAPPING);
							if (array.Length != 0)
							{
								ISignature[] array2 = array;
								for (int i = 0; i < array2.Length; i++)
								{
									IVariable[] all = array2[i].All;
									for (int j = 0; j < all.Length; j++)
									{
										if (string.Compare(all[j].Name, DataElement.IoMapping.VariableMappings[0]
											.Variable, ignoreCase: true) == 0)
										{
											stName = DataElement.IoMapping.VariableMappings[0]
												.Variable;
											objectGuid = array[0].ObjectGuid;
											return (RefactoringContextType)2;
										}
									}
								}
							}
						}
					}
				}
			}
			stName = string.Empty;
			objectGuid = Guid.Empty;
			return (RefactoringContextType)0;
		}

		private AutomaticRefactoringQueryResult CheckRefactoring(string stNewName)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(stNewName) && VariableMapping != null && !string.IsNullOrEmpty(VariableMapping.Variable))
			{
				IDeviceObject device = ParameterSetProvider.GetDevice();
				if (device != null && (int)GetRefactoringContext(out var objectGuid, out var _) == 2)
				{
					IPreCompileContext val = default(IPreCompileContext);
					ISignature val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindSignature(objectGuid, out val);
					if (val2 != null && val2[VariableMapping.Variable] != null)
					{
						if (ParameterSetProvider is _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider)
						{
							(ParameterSetProvider as _3S.CoDeSys.DeviceEditor.SimpleMappingEditor.DeviceParameterSetProvider).StoreObject();
						}
						return APEnvironment.RefactoringService.AutomaticRefactoringHelper.TryQueryAndPerformRenameOfVariable(DeviceEditorAutomaticRefactoringOperationDefinitions.ConfigContext_MappingEditor, ((IObject)device).MetaObject.ProjectHandle, ((IObject)device).MetaObject.ObjectGuid, objectGuid, VariableMapping.Variable, stNewName);
					}
				}
			}
			return (AutomaticRefactoringQueryResult)1;
		}

		private bool CheckValue(string stValueToCheck, LList<object> objectList, out TypeClass baseClass)
		{
			baseClass = (TypeClass)30;
			if (stValueToCheck.Length == 0)
			{
				return true;
			}
			IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(DataElement.BaseType, false, false, false, false);
			IParser obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
			IParser2 val2 = (IParser2)(object)((obj is IParser2) ? obj : null);
			try
			{
				if (val2 == null)
				{
					return false;
				}
				ICompiledType val3 = ((IParser)val2).ParseTypeDeclaration();
				if (val3 == null)
				{
					return false;
				}
				TypeClass @class = ((IType)val3).Class;
				if ((int)@class == 24)
				{
					@class = ((IType)((ISubrangeType2)((val3 is ISubrangeType2) ? val3 : null)).BaseType).Class;
				}
				LList<string> val4 = new LList<string>();
				string text = DataElement.BaseType;
				IToken val5 = default(IToken);
				ulong num = default(ulong);
				bool flag = default(bool);
				Operator val6 = default(Operator);
				bool flag2 = default(bool);
				if ((int)@class == 26)
				{
					val.Initialize(stValueToCheck);
					val.GetNext(out val5);
					DateTime dateTime = default(DateTime);
					DateTime dateTime2 = default(DateTime);
					do
					{
						if ((int)val5.Type == 13)
						{
							return false;
						}
						if ((int)val5.Type == 1)
						{
							val4.Add(val.GetBoolean(val5).ToString());
						}
						if ((int)val5.Type == 14)
						{
							val.GetInteger(val5, out num, out flag, out val6, out flag2);
							if (flag2)
							{
								return false;
							}
							val4.Add(num.ToString());
						}
						if ((int)val5.Type == 16)
						{
							double num2 = 0.0;
							val.GetReal(val5, out num2, out val6, out flag2);
							if (flag2)
							{
								return false;
							}
							val4.Add(_converterToIec.GetReal((object)num2, ((IType)val3.BaseType).Class));
						}
						if ((int)val5.Type == 17)
						{
							val4.Add("'" + val.GetSingleByteString(val5) + "'");
						}
						if ((int)val5.Type == 9)
						{
							val4.Add("\"" + val.GetDoubleByteString(val5) + "\"");
						}
						if ((int)val5.Type == 10)
						{
							uint num3 = 0u;
							val.GetDuration(val5, out num3, out flag2);
							if (flag2)
							{
								return false;
							}
							val4.Add(_converterToIec.GetDuration((long)num3));
						}
						if ((int)val5.Type == 11)
						{
							ulong num4 = 0uL;
							val.GetLDuration(val5, out num4, out flag2);
							if (flag2)
							{
								return false;
							}
							if (_converterToIec is IConverterToIEC2)
							{
								IConverterToIEC converterToIec = _converterToIec;
								val4.Add(((IConverterToIEC2)((converterToIec is IConverterToIEC2) ? converterToIec : null)).GetLDuration((long)num4));
							}
						}
						if ((int)val5.Type == 6)
						{
							val.GetDateAndTime(val5, out dateTime, out flag2);
							if (flag2)
							{
								return false;
							}
							val4.Add(_converterToIec.GetDateAndTime(dateTime));
						}
						if ((int)val5.Type == 18)
						{
							val.GetTimeOfDay(val5, out dateTime2, out flag2);
							if (flag2)
							{
								return false;
							}
							val4.Add(_converterToIec.GetTimeOfDay(dateTime2));
						}
						val.GetNext(out val5);
					}
					while ((int)val5.Type != 21);
					@class = ((IType)val3.BaseType).Class;
					if (val3 is IArrayType && ((IArrayType)((val3 is IArrayType) ? val3 : null)).Dimensions != null)
					{
						ulong num5 = 0uL;
						IArrayDimension[] dimensions = ((IArrayType)((val3 is IArrayType) ? val3 : null)).Dimensions;
						foreach (IArrayDimension val7 in dimensions)
						{
							ulong num6 = num5;
							ulong uLongValue = (val7.UpperBorder as ILiteralExpression).ULongValue;
							IExpression lowerBorder = val7.LowerBorder;
							num5 = num6 + (uLongValue - ((ILiteralExpression)((lowerBorder is ILiteralExpression) ? lowerBorder : null)).ULongValue + 1);
						}
						if ((ulong)val4.Count > num5)
						{
							return false;
						}
					}
					text = ((object)val3.BaseType).ToString();
				}
				else
				{
					val4.Add(stValueToCheck);
				}
				baseClass = (TypeClass)(int)@class;
				foreach (string item in val4)
				{
					object obj2 = null;
					switch ((int)@class)
					{
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
						val.Initialize(@class.ToString() + "#" + item.Trim());
						val.IncludeWhitespaces=(true);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 14)
							{
								val.GetInteger(val5, out num, out flag, out val6, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = num;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 1:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 14)
							{
								val.GetInteger(val5, out num, out flag, out val6, out flag2);
								if (flag2 || (num != 0L && num != 1))
								{
									return false;
								}
								obj2 = num;
							}
							else
							{
								if ((int)val5.Type != 1)
								{
									return false;
								}
								obj2 = _converterFromIec.GetBoolean(item);
							}
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					case 0:
					{
						string text2 = item;
						if (item == "0" || item == "1")
						{
							text2 = "BOOL#" + item;
						}
						val.Initialize(text2);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type != 1)
							{
								return false;
							}
							obj2 = _converterFromIec.GetBoolean(text2);
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					}
					case 16:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type != 17)
							{
								return false;
							}
							obj2 = _converterFromIec.GetSingleByteString(item);
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					case 17:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type != 9)
							{
								return false;
							}
							obj2 = _converterFromIec.GetDoubleByteString(item);
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					case 14:
					case 15:
						val.Initialize(text + "#" + item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 16)
							{
								double num8 = 0.0;
								val.GetReal(val5, out num8, out val6, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = num8;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 18:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 10)
							{
								uint num7 = 0u;
								val.GetDuration(val5, out num7, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = (long)num7;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 37:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 11)
							{
								val.GetLDuration(val5, out num, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = num;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 20:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type != 6)
							{
								return false;
							}
							obj2 = _converterFromIec.GetDateAndTime(item);
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					case 21:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type != 18)
							{
								return false;
							}
							obj2 = _converterFromIec.GetTimeOfDay(item);
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					case 19:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type != 5)
							{
								return false;
							}
							obj2 = _converterFromIec.GetDate(item);
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					case 40:
						val.Initialize("LWORD#" + item.Trim());
						val.IncludeWhitespaces=(true);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 14)
							{
								val.GetInteger(val5, out num, out flag, out val6, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = num;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 41:
						val.Initialize("LINT#" + item.Trim());
						val.IncludeWhitespaces=(true);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 14)
							{
								val.GetInteger(val5, out num, out flag, out val6, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = num;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 39:
						val.Initialize("ULINT#" + item.Trim());
						val.IncludeWhitespaces=(true);
						val.GetNext(out val5);
						do
						{
							if ((int)val5.Type == 14)
							{
								val.GetInteger(val5, out num, out flag, out val6, out flag2);
								if (flag2)
								{
									return false;
								}
								obj2 = num;
								val.GetNext(out val5);
								continue;
							}
							return false;
						}
						while ((int)val5.Type != 21);
						break;
					case 42:
						val.Initialize(item);
						val.GetNext(out val5);
						do
						{
							TokenType type = val5.Type;
							if ((int)type != 9)
							{
								if ((int)type != 17)
								{
									return false;
								}
								obj2 = _converterFromIec.GetSingleByteString(item);
							}
							else
							{
								obj2 = _converterFromIec.GetDoubleByteString(item);
							}
							val.GetNext(out val5);
						}
						while ((int)val5.Type != 21);
						break;
					default:
						return false;
					}
					if (obj2 != null)
					{
						objectList.Add(obj2);
					}
				}
			}
			catch
			{
			}
			return true;
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
			IParameterTreeNode value = _childNodes[nIndex1];
			_childNodes[nIndex1] = _childNodes[nIndex2];
			_childNodes[nIndex2] = value;
		}

		internal object GetOnlineValue(bool bPreparedValue)
		{
			try
			{
				if (_onlineVarRef != null)
				{
					VarRefState val;
					if ((int)_onlineVarRef.State == 0)
					{
						byte[] array = null;
						bool bConstant = false;
						if (bPreparedValue)
						{
							array = _onlineVarRef.PreparedRawValue;
						}
						else if (_onlineVarRef is IOnlineVarRef6)
						{
							IMonitoringExpression monitoringExpression = (IMonitoringExpression)(object)((IOnlineVarRef6)_onlineVarRef).GetMonitoringExpression();
							if (monitoringExpression != null && monitoringExpression.VarRef != null && monitoringExpression.VarRef.ConstantValue != null)
							{
								bConstant = true;
								try
								{
									array = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(monitoringExpression.VarRef.ConstantValue, (IType)(object)_onlineVarRef.Expression.Type, Guid.Empty, (ByteOrder)0);
								}
								catch
								{
									val = (VarRefState)5;
									return new ErrorValueData(val.ToString());
								}
							}
							else
							{
								array = _onlineVarRef.RawValue;
							}
						}
						else
						{
							array = _onlineVarRef.RawValue;
						}
						if (DataElement.IsEnumeration && DataElement is IEnumerationDataElement)
						{
							int num = default(int);
							IEnumerationValue enumerationValue = ((IEnumerationDataElement)DataElement).GetEnumerationValue(_onlineVarRef.Value, out num);
							if (enumerationValue != null)
							{
								return new RawValueData((TypeClass)29, enumerationValue.VisibleName, _converterToIec, bConstant: false, _onlineVarRef.Forced);
							}
						}
						if (((int)_typeClass == 26 || (int)_typeClass == 24) && _baseType == null)
						{
							return new RawValueData(_typeClass, ((IType)_onlineVarRef.Expression.Type.BaseType).Class, _onlineVarRef.Value, _converterToIec, bConstant: false, _onlineVarRef.Forced);
						}
						if (_baseType != null)
						{
							if (array == null)
							{
								return string.Empty;
							}
							string text = ((IExprement)_onlineVarRef.Expression).ToString();
							if (!text.Contains("%"))
							{
								try
								{
									ByteOrder val2 = (ByteOrder)0;
									short num2 = 0;
									DataElementNode dataElementNode = this;
									bool flag = false;
									do
									{
										IDataElement dataElement = dataElementNode.DataElement;
										if (dataElement != null && dataElement.IoMapping != null && dataElement.IoMapping.VariableMappings != null && ((ICollection)dataElement.IoMapping.VariableMappings).Count > 0 && dataElement.IoMapping.VariableMappings[0]
											.Variable
											.Contains(text))
										{
											flag = true;
											if (dataElement is IDataElement4)
											{
												num2 = (short)((IDataElement4)((dataElement is IDataElement4) ? dataElement : null)).GetBitOffset();
											}
										}
										else
										{
											dataElementNode = dataElementNode.Parent as DataElementNode;
										}
									}
									while (!flag && dataElementNode != null);
									IDataElement dataElement2 = DataElement;
									short num3 = (short)(((IDataElement4)((dataElement2 is IDataElement4) ? dataElement2 : null)).GetBitOffset() - num2);
									ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(_plcNode.OnlineApplication);
									if (referenceContextIfAvailable != null && referenceContextIfAvailable.Codegenerator != null)
									{
										val2 = (ByteOrder)(referenceContextIfAvailable.Codegenerator.MotorolaByteOrder ? 1 : 0);
									}
									short num4 = (short)((DataElement.GetBitSize() + 7) / 8);
									byte[] array2 = new byte[num4];
									if ((int)((IType)_baseType).Class == 0 || (int)((IType)_baseType).Class == 1)
									{
										short num5 = (short)(num3 / 8);
										if (_plcNode.MotorolaBitfields && Parent is DataElementNode && (Parent as DataElementNode).DataElement != null)
										{
											IDataElement dataElement3 = (Parent as DataElementNode).DataElement;
											if (dataElement3 is IDataElement2 && ((IDataElement2)((dataElement3 is IDataElement2) ? dataElement3 : null)).HasBaseType)
											{
												int num6 = (short)dataElement3.GetBitSize() / 8;
												num5 = (short)(num5 + (short)(num6 - 1 - num5 % num6 * 2));
											}
										}
										if (array.Length > num5 && num5 >= 0)
										{
											array2[0] = array[num5];
										}
										array2[0] = (byte)(((array2[0] & (1 << num3 % 8)) != 0) ? 1 : 0);
									}
									else
									{
										for (short num7 = 0; num7 < num4; num7 = (short)(num7 + 1))
										{
											short num8 = 0;
											num8 = (short)(num7 + num3 / 8);
											if (array.Length > num8 && num8 >= 0)
											{
												array2[num7] = array[num8];
											}
										}
									}
									object obj2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertRaw(array2, (IType)(object)_baseType, _plcNode.OnlineApplication, val2);
									if (_dataElement.IsEnumeration)
									{
										int num9 = default(int);
										IEnumerationValue enumerationValue2 = ((IEnumerationDataElement)DataElement).GetEnumerationValue(obj2, out num9);
										if (enumerationValue2 != null)
										{
											return new RawValueData((TypeClass)29, enumerationValue2.VisibleName, _converterToIec, bConstant: false, _onlineVarRef.Forced);
										}
									}
									return new RawValueData(((IType)_baseType).Class, obj2, _converterToIec, bConstant, _onlineVarRef.Forced);
								}
								catch
								{
								}
							}
						}
						if (_onlineVarRef.RawValue != null && _baseType != null && ((int)((IType)_baseType).Class == 14 || (int)((IType)_baseType).Class == 15) && _typeClass != ((IType)_baseType).Class)
						{
							ByteOrder val3 = (ByteOrder)0;
							ICompileContext referenceContextIfAvailable2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(_plcNode.OnlineApplication);
							if (referenceContextIfAvailable2 != null && referenceContextIfAvailable2.Codegenerator != null)
							{
								val3 = (ByteOrder)(referenceContextIfAvailable2.Codegenerator.MotorolaByteOrder ? 1 : 0);
							}
							return new RawValueData(value: (!bPreparedValue) ? ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertRaw(_onlineVarRef.RawValue, (IType)(object)_baseType, _plcNode.OnlineApplication, val3) : ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertRaw(_onlineVarRef.PreparedRawValue, (IType)(object)_baseType, _plcNode.OnlineApplication, val3), typeClass: ((IType)_baseType).Class, converter: _converterToIec, bConstant: bConstant, bForced: _onlineVarRef.Forced);
						}
						if (bPreparedValue)
						{
							return new RawValueData(_typeClass, _onlineVarRef.PreparedValue, _converterToIec, bConstant, _onlineVarRef.Forced);
						}
						return new RawValueData(_typeClass, _onlineVarRef.Value, _converterToIec, bConstant, _onlineVarRef.Forced);
					}
					val = _onlineVarRef.State;
					return new ErrorValueData(val.ToString());
				}
				if (!string.IsNullOrEmpty(_stExceptionText))
				{
					return new ErrorValueData(_stExceptionText);
				}
				return EmptyValueData.Empty;
			}
			catch (Exception ex)
			{
				return new ErrorValueData(ex.Message);
			}
		}

		private void OnOnlineVarRefChanged(IOnlineVarRef varRef)
		{
			if (varRef == _onlineVarRef && MonitoringEnabled && (_plcNode.OnlineApplication != Guid.Empty || _bIsMapped))
			{
				_plcNode.TreeModel.RaiseChanged(this);
			}
		}

		internal int GetAddressIndex()
		{
			return _plcNode.TreeModel.GetIndexOfColumn(3);
		}

		internal int GetOnlineIndex()
		{
			return _plcNode.TreeModel.GetIndexOfColumn(6);
		}

		public string GetToolTipText(int nColumnIndex)
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			if (_plcNode.TreeModel.GetIndexOfColumn(6) == nColumnIndex)
			{
				IDataElement dataElement = DataElement;
				long bitOffset = ((IDataElement4)((dataElement is IDataElement4) ? dataElement : null)).GetBitOffset();
				return bitOffset + " Byte " + bitOffset / 8;
			}
			if ((_plcNode.TreeModel.GetIndexOfColumn(5) == nColumnIndex || _plcNode.TreeModel.GetIndexOfColumn(7) == nColumnIndex) && DataElement.IsEnumeration && DataElement is IEnumerationDataElement)
			{
				StringBuilder stringBuilder = new StringBuilder();
				IEnumerationValue[] enumerationValues = ((IEnumerationDataElement)DataElement).EnumerationValues;
				foreach (IEnumerationValue val in enumerationValues)
				{
					stringBuilder.AppendLine(val.VisibleName);
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public int CompareTo(IParameterTreeNode otherNode, int sortColumn)
		{
			if (otherNode == null)
			{
				throw new ArgumentNullException("otherNode");
			}
			switch (_plcNode.TreeModel.MapColumn(sortColumn))
			{
			case 2:
				if (otherNode is SectionNode)
				{
					return string.Compare(DataElement.VisibleName, (otherNode as SectionNode).Section.Name, ignoreCase: true);
				}
				if (otherNode is DataElementNode)
				{
					return string.Compare(DataElement.VisibleName, (otherNode as DataElementNode).DataElement.VisibleName, ignoreCase: true);
				}
				return 0;
			case 5:
				if (otherNode is DataElementNode)
				{
					return string.Compare(DataElement.Value, (otherNode as DataElementNode).DataElement.Value, ignoreCase: true);
				}
				return 0;
			case 8:
				if (otherNode is DataElementNode)
				{
					return string.Compare(DataElement.DefaultValue, (otherNode as DataElementNode).DataElement.DefaultValue, ignoreCase: true);
				}
				return 0;
			case 10:
				if (otherNode is DataElementNode)
				{
					return string.Compare(DataElement.Description, (otherNode as DataElementNode).DataElement.Description, ignoreCase: true);
				}
				return 0;
			case 4:
				if (otherNode is DataElementNode)
				{
					return string.Compare(DataElement.GetTypeString(), (otherNode as DataElementNode).DataElement.GetTypeString(), ignoreCase: true);
				}
				return 0;
			case 9:
				if (otherNode is DataElementNode)
				{
					return string.Compare(DataElement.Unit, (otherNode as DataElementNode).DataElement.Unit, ignoreCase: true);
				}
				return 0;
			default:
				return 0;
			}
		}
	}
}
