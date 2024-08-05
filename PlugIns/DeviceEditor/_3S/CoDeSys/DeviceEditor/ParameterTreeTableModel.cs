#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceEditor.SimpleMappingEditor;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class ParameterTreeTableModel : AbstractTreeTableModel, IPlcNode, IDeviceNode, IParameterTreeTable
	{
		public const int COLIDX_VARIABLE = 0;

		public const int COLIDX_CREATEVARIABLE = 1;

		public const int COLIDX_NAME = 2;

		public const int COLIDX_ADDRESS = 3;

		public const int COLIDX_TYPE = 4;

		public const int COLIDX_VALUE = 5;

		public const int COLIDX_ONLINEVALUE = 6;

		public const int COLIDX_PREPAREDVALUE = 7;

		public const int COLIDX_DEFAULT = 8;

		public const int COLIDX_UNIT = 9;

		public const int COLIDX_DESCRIPTION = 10;

		private IUndoManager _undoMgr;

		private IParameterSetProvider _parameterSetProvider;

		private LDictionary<IParameterSection, SectionNode> _sectionNodes = new LDictionary<IParameterSection, SectionNode>();

		private LDictionary<long, DataElementNode> _positionToNode = new LDictionary<long, DataElementNode>();

		private List<int> _columnMap = new List<int>();

		private Guid _onlineApplication = Guid.Empty;

		private DataElementNode[] _monitoringNodes = new DataElementNode[0];

		private IConverterToIEC _binaryConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)0);

		private IConverterToIEC _decimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)1);

		private IConverterToIEC _hexadecimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)2);

		private bool _bMotorolaBitfields;

		private bool _bUnionRootEditable = true;

		private bool _bBaseTypeMappable = true;

		private bool _bBitfieldMappable = true;

		private bool _bMultipleMappableAllowed;

		private bool _bAlwaysMapToNew;

		private bool _bManualAddress = true;

		private bool _DefaultColumnAvailable;

		private LList<Guid> _liApplications;

		private IDeviceObject _host;

		private HideParameterDelegate _paramFilter;

		private ParameterTreeTableModelView _modelView;

		private WeakMulticastDelegate _ehConfigurationNodeChanged;

		private bool _bDefaultColumnForInputsEditable;

		private LDictionary<long, DataElementNode> _parameterDataNodes = new LDictionary<long, DataElementNode>();

		private SortOrder _sortOrder;

		private int _sortColumn;

		private string _stSearchText = string.Empty;

		private IIoMappingEditorFilterFactory _variableFilter;

		private bool _bIsInRestore;

		private TreeTableView _view;

		private LList<string> _storedExpandedNodes = new LList<string>();

		internal LList<DataElementNode> Nodes => Enumerable.ToLList<DataElementNode>((IEnumerable<DataElementNode>)_positionToNode.Values);

		public WeakMulticastDelegate ConfigurationNodeChanged
		{
			get
			{
				return _ehConfigurationNodeChanged;
			}
			set
			{
				_ehConfigurationNodeChanged = value;
			}
		}

		public bool MotorolaBitfields => _bMotorolaBitfields;

		public bool AlwaysMapToNew => _bAlwaysMapToNew;

		public bool UnionRootEditable => _bUnionRootEditable;

		public bool BaseTypeMappable => _bBaseTypeMappable;

		public bool BitfieldMappable => _bBitfieldMappable;

		public bool MultipleMappableAllowed => _bMultipleMappableAllowed;

		public bool ManualAddress => _bManualAddress;

		public bool DefaultColumnAvailable => _DefaultColumnAvailable;

		public bool DefaultColumnForInputsEditable => _bDefaultColumnForInputsEditable;

		public IDeviceObject PlcDevice => _host;

		public IList<Guid> Applications
		{
			get
			{
				if (_liApplications == null)
				{
					return (IList<Guid>)new LList<Guid>();
				}
				return (IList<Guid>)_liApplications;
			}
		}

		public IUndoManager UndoManager
		{
			get
			{
				return _undoMgr;
			}
			set
			{
				_undoMgr = value;
			}
		}

		public bool IsConfigModeOnlineApplication
		{
			get
			{
				if (_onlineApplication != Guid.Empty && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, _onlineApplication).Name == DataElementNode.HIDDENONLINECONFIGAPPLICATION)
				{
					IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(_onlineApplication);
					if (application != null && application.IsLoggedIn)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Guid OnlineApplication
		{
			get
			{
				return _onlineApplication;
			}
			set
			{
				if (value != _onlineApplication)
				{
					_onlineApplication = value;
					Debug.Assert(GetAllDataElementNodes() != null);
					if (_onlineApplication == Guid.Empty)
					{
						ReleaseMonitoring(bClose: false);
					}
				}
			}
		}

		public IParameterSet ParameterSet => _parameterSetProvider.GetParameterSet(bToModify: false);

		public IParameterSetProvider ParameterSetProvider => _parameterSetProvider;

		internal SortOrder SortOrder => _sortOrder;

		internal int SortColumn => _sortColumn;

		IParameterTreeTable IDeviceNode.TreeModel => this;

		internal bool IsExcludedFromBuild
		{
			get
			{
				if (_parameterSetProvider != null)
				{
					IDeviceObject device = _parameterSetProvider.GetDevice();
					if (device != null && ((IObject)device).MetaObject != null && ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).IsExcludedFromBuild(((IObject)device).MetaObject.ProjectHandle, ((IObject)device).MetaObject.ObjectGuid))
					{
						return true;
					}
				}
				return false;
			}
		}

		public TreeTableView View
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value;
			}
		}

		internal string SearchText
		{
			get
			{
				return _stSearchText;
			}
			set
			{
				_storedExpandedNodes.Clear();
				base.UnderlyingModel.ClearRootNodes();
				_stSearchText = value;
			}
		}

		internal IIoMappingEditorFilterFactory VariableFilter
		{
			get
			{
				return _variableFilter;
			}
			set
			{
				base.UnderlyingModel.ClearRootNodes();
				_variableFilter = value;
			}
		}

		public ParameterTreeTableModel(IParameterSetProvider parameterSetProvider, ParameterTreeTableModelView modelView, HideParameterDelegate paramFilter)
			: base()
		{
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Invalid comparison between Unknown and I4
			_modelView = modelView;
			_paramFilter = paramFilter;
			_parameterSetProvider = parameterSetProvider;
			IDeviceObject val = null;
			bool flag = true;
			try
			{
				object customizationValue = DeviceEditorFactory.GetCustomizationValue(DeviceEditorFactory.stDefaultColumnForInputsEditable);
				if (customizationValue is bool)
				{
					_bDefaultColumnForInputsEditable = (bool)customizationValue;
				}
			}
			catch
			{
			}
			if (modelView == ParameterTreeTableModelView.IOMappingsOffline || modelView == ParameterTreeTableModelView.IOMappingsOnline)
			{
				if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)0))
				{
					base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameVariable"), HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyIconLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false), (ITreeTableViewEditor)(object)new MappingVariableEditor(), true);
					_columnMap.Add(0);
				}
				val = _parameterSetProvider.GetHost();
				if (val != null)
				{
					_host = val;
					_liApplications = DeviceHelper.GetApplications(((IObject)val).MetaObject, bWithHidden: true);
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)((val is IDeviceObject5) ? val : null)).DeviceIdentificationNoSimulation);
					if (targetSettingsById != null)
					{
						flag = LocalTargetSettings.MappingChangeable.GetBoolValue(targetSettingsById);
						_bManualAddress = LocalTargetSettings.ManualAddressAllowed.GetBoolValue(targetSettingsById);
						_bMotorolaBitfields = LocalTargetSettings.MotorolaBitfields.GetBoolValue(targetSettingsById);
						_bUnionRootEditable = LocalTargetSettings.UnionRootEditable.GetBoolValue(targetSettingsById);
						_bBaseTypeMappable = LocalTargetSettings.BasetypeMappable.GetBoolValue(targetSettingsById);
						_bBitfieldMappable = LocalTargetSettings.BitfieldMappable.GetBoolValue(targetSettingsById);
						_bMultipleMappableAllowed = LocalTargetSettings.MultipleMappableAllowed.GetBoolValue(targetSettingsById);
						_bAlwaysMapToNew = LocalTargetSettings.MapAlwaysIecAddress.GetBoolValue(targetSettingsById);
					}
				}
				if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)1))
				{
					base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameMapping"), HorizontalAlignment.Left, MappingTreeTableViewRenderer.Singleton, MappingTreeTableViewEditor.Singleton, flag);
					_columnMap.Add(1);
				}
			}
			if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)2))
			{
				string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameParameter");
				string string2 = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameChannel");
				DefaultTreeTableModel underlyingModel = base.UnderlyingModel;
				string obj2 = ((modelView == ParameterTreeTableModelView.Parameters) ? @string : string2);
				ITreeTableViewRenderer obj3;
				if (base.UnderlyingModel.ColumnCount != 0)
				{
					ITreeTableViewRenderer val2 = (ITreeTableViewRenderer)(object)new MyLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false);
					obj3 = val2;
				}
				else
				{
					obj3 = MyIconLabelTreeTableViewRenderer.NormalString;
				}
				underlyingModel.AddColumn(obj2, HorizontalAlignment.Left, obj3, (base.UnderlyingModel.ColumnCount == 0) ? IconTextBoxTreeTableViewEditor.TextBox : TextBoxTreeTableViewEditor.TextBox, false);
				_columnMap.Add(2);
			}
			bool flag2 = false;
			IDeviceObject device = _parameterSetProvider.GetDevice();
			if (device != null && device is ILogicalDevice && ((ILogicalDevice)((device is ILogicalDevice) ? device : null)).IsLogical)
			{
				flag2 = true;
			}
			if (!flag2 && (modelView == ParameterTreeTableModelView.IOMappingsOffline || modelView == ParameterTreeTableModelView.IOMappingsOnline) && !GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)3))
			{
				base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameAddress"), HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyIconLabelTreeTableViewRenderer(() => _stSearchText.StartsWith("%") ? _stSearchText : string.Empty, bPathEllipses: false), MyIconTextBoxTreeTableViewEditor.TextBox, true);
				_columnMap.Add(3);
			}
			if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)4))
			{
				base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameType"), HorizontalAlignment.Left, MyLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
				_columnMap.Add(4);
			}
			switch (modelView)
			{
			case ParameterTreeTableModelView.IOMappingsOnline:
				if (val != null)
				{
					IDeviceObject2 val3 = (IDeviceObject2)(object)((val is IDeviceObject2) ? val : null);
					if (val3 != null && typeof(IDriverInfo4).IsAssignableFrom(((object)val3.DriverInfo).GetType()) && (int)((IDriverInfo4)val3.DriverInfo).StopResetBehaviourSetting == 1 && !GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)8))
					{
						base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameDefault"), HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)new GenericValueTreeTableViewRenderer(), (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: true), true);
						_columnMap.Add(5);
						_DefaultColumnAvailable = true;
					}
				}
				break;
			case ParameterTreeTableModelView.Parameters:
				if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)5))
				{
					_DefaultColumnAvailable = true;
					base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameValue"), HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)new GenericValueTreeTableViewRenderer(), (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: false), true);
					_columnMap.Add(5);
				}
				if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)8))
				{
					base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameDefault"), HorizontalAlignment.Right, MyLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
					_columnMap.Add(8);
				}
				break;
			}
			if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)9))
			{
				base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameUnit"), HorizontalAlignment.Left, MyLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
				_columnMap.Add(9);
			}
			bool flag3 = false;
			if (modelView == ParameterTreeTableModelView.IOMappingsOffline || modelView == ParameterTreeTableModelView.IOMappingsOnline)
			{
				flag3 = true;
			}
			if (!GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)10))
			{
				base.UnderlyingModel.AddColumn(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameDescription"), HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false), MyTextBoxTreeTableViewEditor.TextBox, flag3);
				_columnMap.Add(10);
			}
			Refill(modelView, paramFilter);
		}

		public void AttachEventHandlers()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			APEnvironment.OptionStorage.OptionChanged+=(new OptionEventHandler(OnOptionChanged));
		}

		public void DetachEventHandlers()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
			_ehConfigurationNodeChanged = null;
		}

		public void ShowOnlineValue(bool bIsMappingEditor, bool bShowPreparedValueColumn, bool bShowCurrentValueColumn)
		{
			if (GetIndexOfColumn(6) >= 0)
			{
				return;
			}
			int num = -1;
			num = ((!bIsMappingEditor) ? GetIndexOfColumn(5) : GetIndexOfColumn(9));
			if (num < 0)
			{
				num = base.UnderlyingModel.ColumnCount;
			}
			bool isConfigModeOnlineApplication = IsConfigModeOnlineApplication;
			if (bShowCurrentValueColumn && !GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)6))
			{
				_columnMap.Insert(num, 6);
				if (!isConfigModeOnlineApplication)
				{
					base.UnderlyingModel.InsertColumn(num, ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameCurrentValue"), HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithForceIndicator, TextBoxTreeTableViewEditor.TextBox, false);
				}
				else
				{
					base.UnderlyingModel.InsertColumn(num, ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNameCurrentValue"), HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithoutForceIndicator, (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: false), true);
				}
			}
			if (bShowPreparedValueColumn && !isConfigModeOnlineApplication && !GenericeEditorColumnHideFactoryManager.HideColumn(_parameterSetProvider.GetIoProvider(bToModify: false), (GenericEditorColumn)7))
			{
				num++;
				_columnMap.Insert(num, 7);
				base.UnderlyingModel.InsertColumn(num, ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ColumnNamePreparedValue"), HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithoutForceIndicator, (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: true), true);
			}
		}

		public void HideOnlineValue()
		{
			int indexOfColumn = GetIndexOfColumn(6);
			if (indexOfColumn >= 0)
			{
				_columnMap.RemoveAt(indexOfColumn);
				base.UnderlyingModel.RemoveColumn(indexOfColumn);
			}
			indexOfColumn = GetIndexOfColumn(7);
			if (indexOfColumn >= 0)
			{
				_columnMap.RemoveAt(indexOfColumn);
				base.UnderlyingModel.RemoveColumn(indexOfColumn);
			}
		}

		public void ReleaseMonitoring(bool bClose)
		{
			DataElementNode[] allDataElementNodes = GetAllDataElementNodes();
			Debug.Assert(allDataElementNodes != null);
			EnableMonitoring(null);
			DataElementNode[] array = allDataElementNodes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ReleaseMonitoring(bClose);
			}
		}

		public void EnableMonitoring(DataElementNode[] nodes)
		{
			LDictionary<DataElementNode, object> lDictionary = new LDictionary<DataElementNode, object>();
			LDictionary<DataElementNode, object> lDictionary2 = new LDictionary<DataElementNode, object>();
			if (_monitoringNodes != null)
			{
				DataElementNode[] monitoringNodes = _monitoringNodes;
				foreach (DataElementNode key in monitoringNodes)
				{
					lDictionary[key] = null;
				}
			}
			if (nodes != null)
			{
				DataElementNode[] monitoringNodes = nodes;
				foreach (DataElementNode key2 in monitoringNodes)
				{
					lDictionary2[key2] = null;
				}
			}
			foreach (KeyValuePair<DataElementNode, object> item in lDictionary)
			{
				DataElementNode key3 = item.Key;
				if (!lDictionary2.ContainsKey(key3))
				{
					key3.MonitoringEnabled = false;
				}
			}
			foreach (KeyValuePair<DataElementNode, object> item2 in lDictionary2)
			{
				DataElementNode key4 = item2.Key;
				key4.StartMonitoring();
				if (!key4.MonitoringEnabled)
				{
					key4.MonitoringEnabled = true;
					RaiseChanged(key4);
				}
			}
			_monitoringNodes = nodes;
		}

		public void RaiseChanged(IParameterTreeNode node)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected O, but got Unknown
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			int num = ((((ITreeTableNode)node).Parent != null) ? ((ITreeTableNode)node).Parent.GetIndex((ITreeTableNode)node) : base.UnderlyingModel.Sentinel.GetIndex((ITreeTableNode)node));
			try
			{
				base.RaiseChanged(new TreeTableModelEventArgs(((ITreeTableNode)node).Parent, num, (ITreeTableNode)node));
			}
			catch
			{
			}
			if (_ehConfigurationNodeChanged != null && node is IGenericConfigurationNode)
			{
				ConfigurationNodeEventArgs val = new ConfigurationNodeEventArgs((IGenericConfigurationNode)((node is IGenericConfigurationNode) ? node : null));
				_ehConfigurationNodeChanged.Invoke(new object[2] { this, val });
			}
		}

		public int MapColumn(int nColumnIndex)
		{
			return _columnMap[nColumnIndex];
		}

		public int GetIndexOfColumn(int nColumn)
		{
			return _columnMap.IndexOf(nColumn);
		}

		public DataElementNode[] GetAllDataElementNodes()
		{
			LList<ITreeTableNode> val = new LList<ITreeTableNode>();
			CollectDataElementNodes(base.UnderlyingModel.Sentinel, val);
			DataElementNode[] array = new DataElementNode[val.Count];
			val.CopyTo((ITreeTableNode[])(object)array);
			return array;
		}

		public void UpdateNode(IParameter parameter, IDataElement dataelement, string[] path)
		{
			DataElementNode dataElementNode = FindNode(parameter, dataelement, path);
			if (dataElementNode != null)
			{
				dataElementNode.UpdateData();
				RaiseChanged(dataElementNode);
			}
		}

		public void SetPositionNode(IParameterTreeNode node, long lPosition)
		{
			if (node is DataElementNode)
			{
				_positionToNode[lPosition]= node as DataElementNode;
			}
		}

		public DataElementNode GetNodeByPosition(long lPosition)
		{
			DataElementNode result = null;
			_positionToNode.TryGetValue(lPosition, out result);
			return result;
		}

		public int CompareNodesByIndex(DataElementNode node1, DataElementNode node2)
		{
			if (node1 == null)
			{
				throw new ArgumentNullException("node1");
			}
			if (node2 == null)
			{
				throw new ArgumentNullException("node2");
			}
			LList<ITreeTableNode> rootPath = GetRootPath((ITreeTableNode)(object)node1);
			LList<ITreeTableNode> rootPath2 = GetRootPath((ITreeTableNode)(object)node2);
			int num = rootPath.Count;
			int num2 = rootPath2.Count;
			while (num > 0 && num2 > 0)
			{
				num--;
				num2--;
				if (rootPath[num] != rootPath2[num2])
				{
					ITreeTableNode val = ((num != rootPath.Count - 1) ? rootPath[num + 1] : base.Sentinel);
					ITreeTableNode val2 = rootPath[num];
					ITreeTableNode val3 = rootPath2[num2];
					return val.GetIndex(val2).CompareTo(val.GetIndex(val3));
				}
			}
			if (num == 0)
			{
				if (num2 == 0)
				{
					return 0;
				}
				return -1;
			}
			return 1;
		}

		protected LList<ITreeTableNode> GetRootPath(ITreeTableNode node)
		{
			LList<ITreeTableNode> val = new LList<ITreeTableNode>();
			for (ITreeTableNode val2 = node; val2 != null; val2 = val2.Parent)
			{
				val.Add(val2);
			}
			return val;
		}

		protected DataElementNode FindNode(IParameter parameter, IDataElement dataelement, string[] path)
		{
			DataElementNode dataElementNode = null;
			if (_parameterDataNodes.TryGetValue(parameter.Id, out dataElementNode))
			{
				return dataElementNode.Get(parameter, dataelement, path);
			}
			return dataElementNode;
		}

		private void CollectDataElementNodes(ITreeTableNode node, LList<ITreeTableNode> list)
		{
			Debug.Assert(node != null);
			if (node is DataElementNode)
			{
				list.Add(node);
			}
			if (node.HasChildren)
			{
				for (int i = 0; i < node.ChildCount; i++)
				{
					CollectDataElementNodes(node.GetChild(i), list);
				}
			}
		}

		internal void Refill()
		{
			Refill(_modelView, _paramFilter);
		}

		private void Refill(ParameterTreeTableModelView modelView, HideParameterDelegate paramFilter)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			try
			{
				base.UnderlyingModel.ClearRootNodes();
				_positionToNode.Clear();
				_parameterDataNodes.Clear();
				_sectionNodes.Clear();
				IParameterSet parameterSet = ParameterSet;
				bool flag = false;
				IDeviceObject device = _parameterSetProvider.GetDevice();
				IDeviceObject9 val = (IDeviceObject9)(object)((device is IDeviceObject9) ? device : null);
				if (val != null && val.ShowParamsInDevDescOrder)
				{
					flag = true;
				}
				LSortedList<long, IParameter> val2 = new LSortedList<long, IParameter>();
				if (parameterSet != null)
				{
					foreach (IParameter item in (IEnumerable)parameterSet)
					{
						IParameter val3 = item;
						if (!ShouldInsertParameter(val3, modelView, paramFilter))
						{
							continue;
						}
						if (val3 is IParameter6 && flag)
						{
							long num = (int)((IParameter6)((val3 is IParameter6) ? val3 : null)).IndexInDevDesc;
							if (val2.ContainsKey(num))
							{
								num = val2.Keys[val2.Count - 1] + 1;
							}
							val2.Add(num, val3);
						}
						else
						{
							val2.Add(val3.Id, val3);
						}
					}
				}
				foreach (KeyValuePair<long, IParameter> item2 in val2)
				{
					AddParameterNode((IDeviceObject)(object)val, parameterSet, item2.Value, paramFilter, flag);
				}
			}
			finally
			{
				if (_view != null)
				{
					RestoreExpandedNodes();
				}
			}
		}

		internal static bool ShouldInsertParameter(IParameter parameter, ParameterTreeTableModelView modelView, HideParameterDelegate paramFilter)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Invalid comparison between Unknown and I4
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Invalid comparison between Unknown and I4
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Invalid comparison between Unknown and I4
			Debug.Assert(parameter != null);
			if (paramFilter != null && paramFilter.Invoke((int)parameter.Id, (string[])null))
			{
				return false;
			}
			bool flag = (int)parameter.ChannelType == 1 || (int)parameter.ChannelType == 2 || (int)parameter.ChannelType == 3;
			switch (modelView)
			{
			case ParameterTreeTableModelView.Parameters:
				if ((int)parameter.ChannelType == 0)
				{
					return (int)parameter.GetAccessRight(false) > 0;
				}
				return false;
			case ParameterTreeTableModelView.IOMappingsOffline:
				if (flag)
				{
					return (int)parameter.GetAccessRight(false) > 0;
				}
				return false;
			case ParameterTreeTableModelView.IOMappingsOnline:
				if (flag)
				{
					return (int)parameter.GetAccessRight(true) > 0;
				}
				return false;
			default:
				return false;
			}
		}

		public SectionNode GetOrCreateSectionNode(IParameterSection section, IDeviceNode device, IParameterTreeNode parent)
		{
			return GetOrCreateSectionNode(section);
		}

		private SectionNode GetOrCreateSectionNode(IParameterSection section)
		{
			if (section == null)
			{
				return null;
			}
			if (!_sectionNodes.ContainsKey(section))
			{
				SectionNode sectionNode = null;
				if (section.Section != null)
				{
					sectionNode = GetOrCreateSectionNode(section.Section);
				}
				SectionNode sectionNode2 = new SectionNode(this, this, sectionNode, section);
				if (sectionNode != null)
				{
					sectionNode.AddSectionNode(sectionNode2);
				}
				else
				{
					base.UnderlyingModel.AddRootNode((ITreeTableNode)(object)sectionNode2);
				}
				_sectionNodes[section]= sectionNode2;
			}
			return _sectionNodes[section];
		}

		public void UpdateSection(IParameterSection section)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			try
			{
				SectionNode sectionNode = null;
				_sectionNodes.TryGetValue(section, out sectionNode);
				if (sectionNode != null)
				{
					sectionNode.Section = section;
					int num = ((sectionNode.Parent != null) ? sectionNode.Parent.GetIndex((ITreeTableNode)(object)sectionNode) : base.UnderlyingModel.Sentinel.GetIndex((ITreeTableNode)(object)sectionNode));
					base.RaiseChanged(new TreeTableModelEventArgs(sectionNode.Parent, num, (ITreeTableNode)(object)sectionNode));
				}
			}
			catch
			{
			}
		}

		public void UpdateSection(IParameter parameter)
		{
			try
			{
				DataElementNode dataElementNode = null;
				_parameterDataNodes.TryGetValue(parameter.Id, out dataElementNode);
				if (dataElementNode == null)
				{
					return;
				}
				if (dataElementNode.Parent == null)
				{
					base.UnderlyingModel.RemoveRootNode((ITreeTableNode)(object)dataElementNode);
				}
				else if (dataElementNode.Parent is SectionNode)
				{
					int index = (dataElementNode.Parent as SectionNode).GetIndex((ITreeTableNode)(object)dataElementNode);
					if (index != -1)
					{
						(dataElementNode.Parent as SectionNode).RemoveNodeAt(index);
					}
					RemoveSectionNodeIfEmpty(dataElementNode.Parent as SectionNode);
				}
				if (_parameterDataNodes.ContainsKey(parameter.Id))
				{
					_parameterDataNodes.Remove(parameter.Id);
				}
				AddParameter(parameter, _modelView, _paramFilter);
			}
			catch
			{
			}
		}

		internal void AddParameter(IParameter parameter, ParameterTreeTableModelView modelView, HideParameterDelegate paramFilter)
		{
			bool bShowParamsInDevDescOrder = false;
			IDeviceObject device = _parameterSetProvider.GetDevice();
			IDeviceObject9 val = (IDeviceObject9)(object)((device is IDeviceObject9) ? device : null);
			if (val != null && val.ShowParamsInDevDescOrder)
			{
				bShowParamsInDevDescOrder = true;
			}
			if (ShouldInsertParameter(parameter, modelView, paramFilter))
			{
				AddParameterNode((IDeviceObject)(object)val, _parameterSetProvider.GetParameterSet(bToModify: false), parameter, paramFilter, bShowParamsInDevDescOrder);
			}
		}

		internal void AddParameterNode(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, HideParameterDelegate paramFilter, bool bShowParamsInDevDescOrder)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected O, but got Unknown
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Expected O, but got Unknown
			bool flag = false;
			bool flag2 = _sortOrder != 0 && _sortColumn != 0;
			Debug.Assert(parameter != null);
			DataElementNode dataElementNode = new DataElementNode(this, this, null, ParameterSetProvider, parameter, GetConverter(GlobalOptionsHelper.DisplayMode));
			_parameterDataNodes[parameter.Id]= dataElementNode;
			IDataElement2 val = (IDataElement2)(object)((parameter is IDataElement2) ? parameter : null);
			if (val != null)
			{
				SetPositionNode(dataElementNode, val.LanguageModelPositionId);
				SetPositionNode(dataElementNode, val.EditorPositionId);
			}
			flag = MatchSearchText(device, ParameterSet, parameter, (IDataElement)(object)parameter);
			if (((IDataElement)parameter).HasSubElements)
			{
				bool bHasMatch = false;
				if (paramFilter == null)
				{
					foreach (IDataElement item in (IEnumerable)((IDataElement)parameter).SubElements)
					{
						IDataElement dataElement = item;
						AddParameterSubNode(device, parameterSet, ParameterSetProvider, parameter, dataElementNode, dataElement, null, paramFilter, flag2, ref bHasMatch);
					}
				}
				else
				{
					foreach (IDataElement item2 in (IEnumerable)((IDataElement)parameter).SubElements)
					{
						IDataElement val2 = item2;
						LList<string> val3 = new LList<string>();
						val3.Add(val2.Identifier);
						if (!paramFilter.Invoke((int)parameter.Id, val3.ToArray()))
						{
							AddParameterSubNode(device, parameterSet, ParameterSetProvider, parameter, dataElementNode, val2, val3, paramFilter, flag2, ref bHasMatch);
						}
					}
				}
				flag = ((_variableFilter != null && !_variableFilter.MatchSubElements) ? (flag && bHasMatch) : (flag || bHasMatch));
			}
			if (!flag)
			{
				return;
			}
			SectionNode sectionNode = null;
			if (!flag2)
			{
				sectionNode = GetOrCreateSectionNode(parameter.Section);
			}
			if (sectionNode != null)
			{
				dataElementNode.Parent = (ITreeTableNode)(object)sectionNode;
				int num = GetParameterInsertIndex(parameter.Id, sectionNode);
				if (parameter is IParameter6 && bShowParamsInDevDescOrder)
				{
					num = GetIndexInsertDevDesc(((IParameter6)((parameter is IParameter6) ? parameter : null)).IndexInDevDesc, (ITreeTableNode)(object)sectionNode);
				}
				if (num < 0)
				{
					sectionNode.AddDataElementNode(dataElementNode);
				}
				else
				{
					sectionNode.InsertDataElementNode(num, dataElementNode);
				}
			}
			else if (!flag2 || !((IDataElement)parameter).HasSubElements)
			{
				int num2 = GetParameterInsertIndex(parameter.Id, base.UnderlyingModel.Sentinel);
				if (parameter is IParameter6 && bShowParamsInDevDescOrder)
				{
					num2 = GetIndexInsertDevDesc(((IParameter6)((parameter is IParameter6) ? parameter : null)).IndexInDevDesc, base.UnderlyingModel.Sentinel);
				}
				if (num2 < 0)
				{
					base.UnderlyingModel.AddRootNode((ITreeTableNode)(object)dataElementNode);
				}
				else
				{
					base.UnderlyingModel.InsertRootNode(num2, (ITreeTableNode)(object)dataElementNode);
				}
			}
		}

		public void RemoveParameterNode(IParameter parameter)
		{
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			if (parameter.Section != null)
			{
				SectionNode sectionNode = null;
				if (_sectionNodes.TryGetValue(parameter.Section, out sectionNode))
				{
					for (int i = 0; i < sectionNode.ChildCount; i++)
					{
						DataElementNode dataElementNode = sectionNode.GetChild(i) as DataElementNode;
						if (dataElementNode != null && dataElementNode.ParameterId == parameter.Id)
						{
							sectionNode.RemoveNodeAt(i);
							RemoveSectionNodeIfEmpty(sectionNode);
						}
					}
				}
			}
			else
			{
				bool flag = false;
				for (int j = 0; j < base.UnderlyingModel.Sentinel.ChildCount; j++)
				{
					DataElementNode dataElementNode2 = base.UnderlyingModel.Sentinel.GetChild(j) as DataElementNode;
					if (dataElementNode2 != null && dataElementNode2.ParameterId == parameter.Id)
					{
						base.UnderlyingModel.RemoveRootNode(j);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					List<SectionNode> list = new List<SectionNode>();
					foreach (KeyValuePair<IParameterSection, SectionNode> sectionNode in _sectionNodes)
					{
						SectionNode value2 = sectionNode.Value;
						for (int k = 0; k < value2.ChildCount; k++)
						{
							if (value2.GetChild(k) is DataElementNode dataElementNode3 && dataElementNode3.ParameterId == parameter.Id)
							{
								value2.RemoveNodeAt(k);
								if (!value2.HasChildren)
								{
									list.Add(value2);
								}
							}
						}
					}
					foreach (SectionNode item in list)
					{
						RemoveSectionNodeIfEmpty(item);
					}
				}
			}
			if (_parameterDataNodes.ContainsKey(parameter.Id))
			{
				_parameterDataNodes.Remove(parameter.Id);
			}
		}

		private void RemoveSectionNodeIfEmpty(SectionNode sn)
		{
			if (!sn.HasChildren)
			{
				if (sn.Parent == null)
				{
					base.UnderlyingModel.RemoveRootNode((ITreeTableNode)(object)sn);
					_sectionNodes.Remove(sn.Section);
					return;
				}
				SectionNode sectionNode = sn.Parent as SectionNode;
				Debug.Assert(sectionNode != null, "Parent of a section node is always a section");
				sectionNode.RemoveNodeAt(sectionNode.GetIndex((ITreeTableNode)(object)sn));
				_sectionNodes.Remove(sn.Section);
				RemoveSectionNodeIfEmpty(sectionNode);
			}
		}

		internal static int GetParameterInsertIndex(long lParameterId, SectionNode parent)
		{
			if (parent.ChildNodes.Count > 0)
			{
				DataElementNode dataElementNode = parent.ChildNodes[parent.ChildNodes.Count - 1] as DataElementNode;
				if (dataElementNode != null && dataElementNode.IsParameter && dataElementNode.ParameterId < lParameterId)
				{
					return -1;
				}
			}
			for (int i = 0; i < parent.ChildCount; i++)
			{
				DataElementNode dataElementNode2 = parent.ChildNodes[i] as DataElementNode;
				if (dataElementNode2 != null && dataElementNode2.IsParameter && dataElementNode2.ParameterId > lParameterId)
				{
					return i;
				}
			}
			return -1;
		}

		internal static int GetParameterInsertIndex(long lParameterId, ITreeTableNode parent)
		{
			if (parent.ChildCount > 0)
			{
				DataElementNode dataElementNode = parent.GetChild(parent.ChildCount - 1) as DataElementNode;
				if (dataElementNode != null && dataElementNode.IsParameter && lParameterId > dataElementNode.ParameterId)
				{
					return -1;
				}
				for (int i = 0; i < parent.ChildCount; i++)
				{
					dataElementNode = parent.GetChild(i) as DataElementNode;
					if (dataElementNode != null && dataElementNode.IsParameter && dataElementNode.ParameterId > lParameterId)
					{
						return i;
					}
				}
			}
			return -1;
		}

		internal static int GetIndexInsertDevDesc(long lIndexInDevDesc, ITreeTableNode parent)
		{
			if (lIndexInDevDesc >= 0)
			{
				for (int i = 0; i < parent.ChildCount; i++)
				{
					DataElementNode dataElementNode = parent.GetChild(i) as DataElementNode;
					if (dataElementNode != null && dataElementNode.IsParameter && dataElementNode.ParameterIndexInDevDesc > lIndexInDevDesc)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private void AddParameterSubNode(IDeviceObject device, IParameterSet parameterSet, IParameterSetProvider parameterSetProvider, IParameter parameter, DataElementNode node, IDataElement dataElement, LList<string> pathlist, HideParameterDelegate paramFilter, bool bIsFlat, ref bool bHasMatch)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected O, but got Unknown
			bool flag = false;
			Debug.Assert(node != null);
			Debug.Assert(dataElement != null);
			LList<string> val = null;
			if (pathlist != null)
			{
				val = new LList<string>();
				val.AddRange((IEnumerable<string>)pathlist);
			}
			if (dataElement is IDataElement6 && (int)((IDataElement6)((dataElement is IDataElement6) ? dataElement : null)).GetAccessRight(false) == 0)
			{
				return;
			}
			IDataElement2 val2 = (IDataElement2)(object)((dataElement is IDataElement2) ? dataElement : null);
			DataElementNode dataElementNode = new DataElementNode(this, this, node, parameterSetProvider, val2, dataElement.Identifier);
			SetPositionNode(dataElementNode, val2.LanguageModelPositionId);
			SetPositionNode(dataElementNode, val2.EditorPositionId);
			flag = MatchSearchText(device, parameterSet, parameter, dataElement);
			if (dataElement.HasSubElements)
			{
				bool bHasMatch2 = false;
				string[] array = null;
				if (paramFilter != null)
				{
					val.Add("");
					array = new string[val.Count];
					val.CopyTo(array);
				}
				foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
				{
					IDataElement val3 = item;
					if (val != null)
					{
						val[val.Count - 1]= val3.Identifier;
						array[val.Count - 1] = val3.Identifier;
					}
					if (paramFilter == null || !paramFilter.Invoke((int)parameter.Id, array))
					{
						AddParameterSubNode(device, parameterSet, parameterSetProvider, parameter, dataElementNode, val3, val, paramFilter, bIsFlat, ref bHasMatch2);
					}
				}
				flag = ((_variableFilter != null && !_variableFilter.MatchSubElements) ? (flag && bHasMatch2) : (flag || bHasMatch2));
			}
			bHasMatch |= flag;
			if (flag)
			{
				if (!bIsFlat)
				{
					node.AddDataElementNode(dataElementNode);
				}
				else if (!dataElement.HasSubElements)
				{
					base.UnderlyingModel.AddRootNode((ITreeTableNode)(object)dataElementNode);
				}
			}
		}

		private void ChangeDecimalPlacesRecursive(IParameterTreeNode node, int iDecimalPlaces)
		{
			foreach (IParameterTreeNode childNode in node.ChildNodes)
			{
				ChangeDecimalPlacesRecursive(childNode, iDecimalPlaces);
			}
			if (node is DataElementNode)
			{
				(node as DataElementNode).DecimalPlaces = iDecimalPlaces;
			}
		}

		private void ChangeConverterRecursive(IParameterTreeNode node, IConverterToIEC converter)
		{
			foreach (IParameterTreeNode childNode in node.ChildNodes)
			{
				ChangeConverterRecursive(childNode, converter);
			}
			if (node is DataElementNode)
			{
				(node as DataElementNode).ConverterToIec = converter;
			}
		}

		private IConverterToIEC GetConverter(int nDisplayMode)
		{
			if (nDisplayMode == GlobalOptionsHelper.DISPLAYMODE_BINARY)
			{
				return _binaryConverter;
			}
			if (nDisplayMode == GlobalOptionsHelper.DISPLAYMODE_HEXADECIMAL)
			{
				return _hexadecimalConverter;
			}
			return _decimalConverter;
		}

		private void OnOptionChanged(object sender, OptionEventArgs e)
		{
			if (e.OptionKey != null && e.OptionKey.Name == GlobalOptionsHelper.SUB_KEY)
			{
				IConverterToIEC converter = GetConverter(GlobalOptionsHelper.DisplayMode);
				Debug.Assert(converter != null);
				for (int i = 0; i < base.Sentinel.ChildCount; i++)
				{
					ChangeDecimalPlacesRecursive(base.Sentinel.GetChild(i) as IParameterTreeNode, GlobalOptionsHelper.DecimalPlaces);
					ChangeConverterRecursive(base.Sentinel.GetChild(i) as IParameterTreeNode, converter);
				}
			}
		}

		internal void Sort(int column, SortOrder order)
		{
			_sortColumn = column;
			_sortOrder = order;
			Refill(_modelView, _paramFilter);
			if (_sortOrder != 0)
			{
				base.UnderlyingModel.Sort((ITreeTableNode)null, true, (IComparer)new ParameterListComparer(SortColumn, SortOrder));
			}
		}

		private static bool ChangeForceValueServiceInvoke(IChangeForceValueService cfs, string stExpression, ICompiledType cType, string stCurrentValue, ref object oPreparedValue, bool bForced, Guid application)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (cfs is IChangeForceValueService2)
			{
				return ((IChangeForceValueService2)cfs).Invoke(stExpression, cType, stCurrentValue, ref oPreparedValue, bForced, application);
			}
			return cfs.Invoke(stExpression, ((object)cType).ToString(), stCurrentValue, ref oPreparedValue, bForced);
		}

		internal static void ChangeForcedValue(TreeTableViewEditEventArgs e)
		{
			DataElementNode dataElementNode = e.Node.View.GetModelNode(e.Node) as DataElementNode;
			if (dataElementNode == null || !dataElementNode.OnlineVarRef.Forced)
			{
				return;
			}
			IChangeForceValueService val = APEnvironment.TryCreateChangeForceValueService();
			if (val == null)
			{
				return;
			}
			object oPreparedValue = dataElementNode.OnlineVarRef.PreparedValue;
			while (ChangeForceValueServiceInvoke(val, ((IExprement)dataElementNode.OnlineVarRef.Expression).ToString(), dataElementNode.OnlineVarRef.Expression.Type, dataElementNode.OnlineVarRef.Value.ToString(), ref oPreparedValue, dataElementNode.OnlineVarRef.Forced, dataElementNode.PlcNode.OnlineApplication))
			{
				try
				{
					dataElementNode.OnlineVarRef.PreparedValue=(oPreparedValue);
					((CancelEventArgs)(object)e).Cancel = true;
					return;
				}
				catch
				{
					string text = string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WrongValueType"), dataElementNode.OnlineVarRef.Expression.Type);
					APEnvironment.MessageService.Error(text, "WrongValueType", Array.Empty<object>());
				}
			}
		}

		internal void StoreExpandedNodes()
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			if (_bIsInRestore)
			{
				return;
			}
			LList<string> val = ((!string.IsNullOrEmpty(_stSearchText)) ? _storedExpandedNodes : ((VariableFilter != null) ? Helper.GetExpandedObjects() : new LList<string>()));
			foreach (TreeTableViewNode node in _view.Nodes)
			{
				TreeTableViewNode viewNode = node;
				GetExpandedNodes(val, viewNode);
			}
			if (string.IsNullOrEmpty(_stSearchText))
			{
				Helper.SetExpandedObjects(val);
			}
		}

		private void GetExpandedNodes(LList<string> storedExpandedNodes, TreeTableViewNode viewNode)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			IParameterTreeNode parameterTreeNode = _view.GetModelNode(viewNode) as IParameterTreeNode;
			if (parameterTreeNode == null || !((ITreeTableNode)parameterTreeNode).HasChildren)
			{
				return;
			}
			if (viewNode.Expanded)
			{
				if (!storedExpandedNodes.Contains(parameterTreeNode.DevPath))
				{
					storedExpandedNodes.Add(parameterTreeNode.DevPath);
				}
			}
			else
			{
				storedExpandedNodes.Remove(parameterTreeNode.DevPath);
			}
			foreach (TreeTableViewNode node in viewNode.Nodes)
			{
				TreeTableViewNode viewNode2 = node;
				GetExpandedNodes(storedExpandedNodes, viewNode2);
			}
		}

		private void CollectNodes(LDictionary<string, IParameterTreeNode> dictNodes, IParameterTreeNode node)
		{
			dictNodes.Add(node.DevPath, node);
			foreach (IParameterTreeNode childNode in node.ChildNodes)
			{
				CollectNodes(dictNodes, childNode);
			}
		}

		private void RestoreExpandedNodes()
		{
			try
			{
				_view.BeginUpdate();
				LList<string> val = ((!string.IsNullOrEmpty(_stSearchText)) ? _storedExpandedNodes : Helper.GetExpandedObjects());
				_bIsInRestore = true;
				if (val.Count == 0)
				{
					if (string.IsNullOrEmpty(_stSearchText))
					{
						ExpandNodesDefault((ICollection)_view.Nodes);
					}
					else
					{
						_view.ExpandAll();
					}
					return;
				}
				_view.CollapseAll();
				LDictionary<string, IParameterTreeNode> val2 = new LDictionary<string, IParameterTreeNode>();
				for (int i = 0; i < base.UnderlyingModel.Sentinel.ChildCount; i++)
				{
					IParameterTreeNode node = base.UnderlyingModel.Sentinel.GetChild(i) as IParameterTreeNode;
					CollectNodes(val2, node);
				}
				IParameterTreeNode parameterTreeNode = default(IParameterTreeNode);
				foreach (string item in val)
				{
					if (val2.TryGetValue(item, out parameterTreeNode))
					{
						TreeTableViewNode viewNode = _view.GetViewNode((ITreeTableNode)parameterTreeNode);
						if (viewNode != null)
						{
							viewNode.Expand();
						}
					}
				}
			}
			finally
			{
				_view.EndUpdate();
				_bIsInRestore = false;
			}
		}

		private void ExpandNodesDefault(ICollection nodes)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			foreach (TreeTableViewNode node in nodes)
			{
				TreeTableViewNode val = node;
				ITreeTableNode modelNode = _view.GetModelNode(val);
				if (!(modelNode is DataElementNode))
				{
					val.Expand();
					ExpandNodesDefault((ICollection)val.Nodes);
				}
				else if (CheckMapping((ICollection)(modelNode as DataElementNode).DataElement.SubElements))
				{
					val.Expand();
					ExpandNodesDefault((ICollection)val.Nodes);
				}
			}
		}

		internal bool CheckMapping(ICollection subElements)
		{
			bool bHasDefaultMapping = false;
			return CheckMapping(subElements, ref bHasDefaultMapping);
		}

		internal bool CheckMapping(ICollection subElements, ref bool bHasDefaultMapping)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			foreach (IDataElement subElement in subElements)
			{
				IDataElement val = subElement;
				foreach (IVariableMapping item in (IEnumerable)val.IoMapping.VariableMappings)
				{
					IVariableMapping val2 = item;
					if (val2 is IVariableMapping2 && !string.IsNullOrEmpty(((IVariableMapping2)((val2 is IVariableMapping2) ? val2 : null)).DefaultVariable))
					{
						bHasDefaultMapping = true;
					}
					if (!string.IsNullOrEmpty(val2.Variable))
					{
						return true;
					}
				}
				if (CheckMapping((ICollection)val.SubElements, ref bHasDefaultMapping))
				{
					return true;
				}
			}
			return false;
		}

		internal bool MatchSearchText(IDeviceObject device, IParameterSet parameterSet, IParameter parameter, IDataElement dataElement)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(_stSearchText))
			{
				if (_stSearchText.StartsWith("%") && dataElement.IoMapping.IecAddress.IndexOf(_stSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
				foreach (IVariableMapping item in (IEnumerable)dataElement.IoMapping.VariableMappings)
				{
					string text = item.Variable;
					if (APEnvironment.LocalizationManagerOrNull != null && _parameterSetProvider.LocalizationActive)
					{
						text = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text, (LocalizationContent)2);
					}
					if (text.IndexOf(_stSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return true;
					}
				}
				if (dataElement.VisibleName.IndexOf(_stSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
				string text2 = dataElement.Description;
				if (APEnvironment.LocalizationManagerOrNull != null && _parameterSetProvider.LocalizationActive)
				{
					text2 = APEnvironment.LocalizationManagerOrNull.GetLocalizedExpression(text2, (LocalizationContent)8);
				}
				if (text2.IndexOf(_stSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
				return false;
			}
			if (_variableFilter == null)
			{
				return true;
			}
			return _variableFilter.FilterChannel(device, parameterSet, parameter, dataElement);
		}
	}
}
