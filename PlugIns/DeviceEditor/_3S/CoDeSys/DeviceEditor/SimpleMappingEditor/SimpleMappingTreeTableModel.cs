#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	internal class SimpleMappingTreeTableModel : AbstractTreeTableModel, IParameterTreeTable
	{
		private IUndoManager _undoMgr;

		private List<int> _columnMap = new List<int>();

		private TreeTableView _view;

		private ISVNode _startNode;

		private IIoMappingEditorFilterFactory _variableFilter;

		private LList<DeviceParameterSetProvider> _liProviders = new LList<DeviceParameterSetProvider>();

		private string _stSearchText = string.Empty;

		private bool _bLocalization;

		private LDictionary<string, IParameterTreeNode> _DevPath2Node = new LDictionary<string, IParameterTreeNode>();

		private bool _bDefaultColumnForInputsEditable;

		private LList<DataElementNode> _liMonitoringNodes = new LList<DataElementNode>();

		private bool _bIsStoring;

		private bool _bIsInRefill;

		private Timer _retrigger = new Timer();

		private bool _bIsInRestore;

		private LList<string> _storedExpandedNodes = new LList<string>();

		private IConverterToIEC _binaryConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)0);

		private IConverterToIEC _decimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)1);

		private IConverterToIEC _hexadecimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)2);

		public TreeTableView View => _view;

		public bool DefaultColumnForInputsEditable => _bDefaultColumnForInputsEditable;

		public LDictionary<string, IParameterTreeNode> DevPath2Node => _DevPath2Node;

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
				Clear();
				_variableFilter = value;
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

		public bool IsStoring
		{
			get
			{
				return _bIsStoring;
			}
			set
			{
				_bIsStoring = value;
			}
		}

		public bool IsInRefill => _bIsInRefill;

		public bool Localization
		{
			get
			{
				return _bLocalization;
			}
			set
			{
				_bLocalization = value;
			}
		}

		public WeakMulticastDelegate ConfigurationNodeChanged
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SimpleMappingTreeTableModel(TreeTableView view, bool bLocalization)
			: base()
		{
			_view = view;
			_bLocalization = bLocalization;
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
			base.UnderlyingModel.AddColumn(_3S.CoDeSys.DeviceEditor.Strings.ColumnNameVariable, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyIconLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false), (ITreeTableViewEditor)(object)new MappingVariableEditor(), true);
			_columnMap.Add(0);
			DefaultTreeTableModel underlyingModel = base.UnderlyingModel;
			string columnNameChannel = _3S.CoDeSys.DeviceEditor.Strings.ColumnNameChannel;
			ITreeTableViewRenderer obj2;
			if (base.UnderlyingModel.ColumnCount != 0)
			{
				ITreeTableViewRenderer val = (ITreeTableViewRenderer)(object)new MyLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false);
				obj2 = val;
			}
			else
			{
				obj2 = MyIconLabelTreeTableViewRenderer.NormalString;
			}
			underlyingModel.AddColumn(columnNameChannel, HorizontalAlignment.Left, obj2, (base.UnderlyingModel.ColumnCount == 0) ? IconTextBoxTreeTableViewEditor.TextBox : TextBoxTreeTableViewEditor.TextBox, false);
			_columnMap.Add(2);
			base.UnderlyingModel.AddColumn(_3S.CoDeSys.DeviceEditor.Strings.ColumnNameAddress, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyIconLabelTreeTableViewRenderer(() => _stSearchText.StartsWith("%") ? _stSearchText : string.Empty, bPathEllipses: false), MyIconTextBoxTreeTableViewEditor.TextBox, true);
			_columnMap.Add(3);
			base.UnderlyingModel.AddColumn(_3S.CoDeSys.DeviceEditor.Strings.ColumnNameType, HorizontalAlignment.Left, MyLabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			_columnMap.Add(4);
			base.UnderlyingModel.AddColumn(_3S.CoDeSys.DeviceEditor.Strings.ColumnNameDescription, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new MyLabelTreeTableViewRenderer(() => _stSearchText, bPathEllipses: false), MyTextBoxTreeTableViewEditor.TextBox, true);
			_columnMap.Add(10);
		}

		public void AttachEventHandlers()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			APEnvironment.OptionStorage.OptionChanged+=(new OptionEventHandler(OnOptionChanged));
		}

		public void DetachEventHandlers()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			Helper.SaveColumdWidths(this);
			StopMonitoring(bForceClose: true);
			APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
		}

		private void OnOptionChanged(object sender, OptionEventArgs e)
		{
			if (e.OptionKey != null && e.OptionKey.Name == GlobalOptionsHelper.SUB_KEY)
			{
				IConverterToIEC converter = GetConverter(GlobalOptionsHelper.DisplayMode);
				Debug.Assert(converter != null);
				for (int i = 0; i < base.Sentinel.ChildCount; i++)
				{
					ChangeConverterRecursive(base.Sentinel.GetChild(i) as IParameterTreeNode, converter);
				}
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

		internal void SetOffOnline(Guid applicationGuid)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			if (base.Sentinel.ChildCount == 0)
			{
				HideOnlineValue();
			}
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				PLCNode pLCNode = base.Sentinel.GetChild(i) as PLCNode;
				if (pLCNode == null)
				{
					continue;
				}
				IDeviceObject host = pLCNode.GetHost();
				IDriverInfo4 val = (IDriverInfo4)((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
				if (val == null || (!(applicationGuid == ((IDriverInfo2)val).IoApplication) && !(applicationGuid == Guid.Empty)))
				{
					continue;
				}
				try
				{
					IOnlineApplication val2 = null;
					if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IObject)host).MetaObject.ProjectHandle, ((IDriverInfo2)val).IoApplication))
					{
						val2 = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(((IDriverInfo2)val).IoApplication);
					}
					if (val2 != null && val2.IsLoggedIn)
					{
						pLCNode.OnlineApplication = val2.ApplicationGuid;
						ShowOnlineValue();
					}
					else
					{
						pLCNode.OnlineApplication = Guid.Empty;
						HideOnlineValue();
					}
				}
				catch
				{
				}
			}
			StartStopMonitoring();
		}

		public void ShowOnlineValue()
		{
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				PLCNode pLCNode = base.Sentinel.GetChild(i) as PLCNode;
				if (pLCNode == null || !(pLCNode.OnlineApplication != Guid.Empty) || GetIndexOfColumn(6) >= 0)
				{
					continue;
				}
				try
				{
					_view.BeginUpdate();
					if (Helper.GetColumnWidths().Count > 0)
					{
						Helper.SaveColumdWidths(this);
					}
					StoreExpandedNodes();
					int num = GetIndexOfColumn(5);
					if (num < 0)
					{
						num = base.UnderlyingModel.ColumnCount;
					}
					_columnMap.Insert(num, 6);
					if (!pLCNode.IsConfigModeOnlineApplication)
					{
						base.UnderlyingModel.InsertColumn(num, _3S.CoDeSys.DeviceEditor.Strings.ColumnNameCurrentValue, HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithForceIndicator, TextBoxTreeTableViewEditor.TextBox, false);
					}
					else
					{
						base.UnderlyingModel.InsertColumn(num, _3S.CoDeSys.DeviceEditor.Strings.ColumnNameCurrentValue, HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)ValueDataRenderer.WithoutForceIndicator, (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: false), true);
					}
					if (!pLCNode.IsConfigModeOnlineApplication)
					{
						num++;
						_columnMap.Insert(num, 7);
						base.UnderlyingModel.InsertColumn(num, _3S.CoDeSys.DeviceEditor.Strings.ColumnNamePreparedValue, HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)new GenericValueTreeTableViewRenderer(), (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: true), true);
					}
					if (!_bIsInRefill)
					{
						RestoreExpandedNodes();
					}
					Helper.RestoreColumnWidths(this);
				}
				finally
				{
					_view.EndUpdate();
				}
			}
		}

		public void StopMonitoring(bool bForceClose)
		{
			LList<DataElementNode> obj = new LList<DataElementNode>();
			obj.AddRange((IEnumerable<DataElementNode>)_liMonitoringNodes);
			foreach (DataElementNode item in obj)
			{
				bool flag = true;
				if (!bForceClose && item.PlcNode.OnlineApplication != Guid.Empty)
				{
					TreeTableViewNode viewNode = _view.GetViewNode((ITreeTableNode)(object)item);
					if (viewNode != null && viewNode.Displayed)
					{
						flag = false;
					}
				}
				if (flag || bForceClose)
				{
					if (item.MonitoringEnabled)
					{
						item.MonitoringEnabled = false;
						item.ReleaseMonitoring(bClose: true);
					}
					_liMonitoringNodes.Remove(item);
				}
			}
		}

		public void StartStopMonitoring()
		{
			if (_bIsInRestore || GetIndexOfColumn(6) < 0)
			{
				return;
			}
			StopMonitoring(bForceClose: false);
			for (TreeTableViewNode val = _view.TopNode; val != null; val = val.NextVisibleNode)
			{
				ITreeTableNode modelNode = _view.GetModelNode(val);
				if (modelNode is DataElementNode && val.Displayed)
				{
					DataElementNode dataElementNode = modelNode as DataElementNode;
					if (dataElementNode.PlcNode.OnlineApplication != Guid.Empty && !_liMonitoringNodes.Contains(dataElementNode))
					{
						dataElementNode.StartMonitoring();
						dataElementNode.MonitoringEnabled = true;
						if (dataElementNode.MonitoringEnabled)
						{
							_liMonitoringNodes.Add(dataElementNode);
							RaiseChanged(dataElementNode);
						}
					}
				}
			}
		}

		public void HideOnlineValue()
		{
			bool flag = true;
			for (int i = 0; i < base.Sentinel.ChildCount; i++)
			{
				PLCNode pLCNode = base.Sentinel.GetChild(i) as PLCNode;
				if (pLCNode != null && pLCNode.OnlineApplication != Guid.Empty)
				{
					flag = false;
				}
			}
			if (!flag || GetIndexOfColumn(6) < 0)
			{
				return;
			}
			try
			{
				_view.BeginUpdate();
				Helper.SaveColumdWidths(this);
				StoreExpandedNodes();
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
				if (!_bIsInRefill)
				{
					RestoreExpandedNodes();
				}
				Helper.RestoreColumnWidths(this);
			}
			finally
			{
				_view.EndUpdate();
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

		public void RaiseChanged(IParameterTreeNode node)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
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
		}

		public void StorePendingChanges()
		{
			try
			{
				_bIsStoring = true;
				foreach (DeviceParameterSetProvider liProvider in _liProviders)
				{
					liProvider.StoreObject();
				}
			}
			finally
			{
				_bIsStoring = false;
			}
		}

		public void Clear()
		{
			base.UnderlyingModel.ClearRootNodes();
		}

		public void Refill(ISVNode startNode)
		{
			string text = null;
			_startNode = startNode;
			if (_bIsStoring)
			{
				return;
			}
			_liProviders.Clear();
			_DevPath2Node.Clear();
			try
			{
				_bIsInRefill = true;
				TreeTableViewNode topNode = _view.TopNode;
				if (topNode != null)
				{
					text = (_view.GetModelNode(topNode) as IParameterTreeNode).DevPath;
				}
				_view.BeginUpdate();
				base.UnderlyingModel.ClearRootNodes();
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
				{
					int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
					LSortedList<string, PLCNode> val = new LSortedList<string, PLCNode>();
					bool flag = false;
					if (startNode == null)
					{
						ISVNode[] children = ((IObjectManager)APEnvironment.ObjectMgr).GetStructuredView(handle, Guid.Empty).Children;
						foreach (ISVNode val2 in children)
						{
							IMetaObjectStub mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, val2.ObjectGuid);
							Debug.Assert(mos != null);
							if (!(mos.ParentObjectGuid != Guid.Empty) && !typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) && !APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)) && !typeof(ITransientObject).IsAssignableFrom(mos.ObjectType) && typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType))
							{
								PLCNode pLCNode = new PLCNode(this, val2);
								val.Add(mos.Name, pLCNode);
								flag |= pLCNode.DefaultColumnAvailable;
							}
						}
					}
					else
					{
						PLCNode pLCNode2 = new PLCNode(this, startNode);
						val.Add(startNode.Name, pLCNode2);
						flag |= pLCNode2.DefaultColumnAvailable;
					}
					int indexOfColumn = GetIndexOfColumn(5);
					if (flag && indexOfColumn == -1)
					{
						base.UnderlyingModel.InsertColumn(4, _3S.CoDeSys.DeviceEditor.Strings.ColumnNameDefault, HorizontalAlignment.Right, (ITreeTableViewRenderer)(object)new GenericValueTreeTableViewRenderer(), (ITreeTableViewEditor)(object)new GenericValueTreeTableViewEditor(bAllowNullValues: true), true);
						_columnMap.Insert(4, 5);
					}
					if (!flag && indexOfColumn != -1)
					{
						base.UnderlyingModel.RemoveColumn(indexOfColumn);
						_columnMap.Remove(5);
					}
					foreach (PLCNode value in val.Values)
					{
						base.UnderlyingModel.AddRootNode((ITreeTableNode)(object)value);
						_DevPath2Node[value.DevPath]= (IParameterTreeNode)value;
						AddDevices(value.ISVNode, value, value, out var bHasParameter);
						if (!bHasParameter)
						{
							base.UnderlyingModel.RemoveRootNode((ITreeTableNode)(object)value);
						}
					}
				}
			}
			finally
			{
				RestoreExpandedNodes();
				SetOffOnline(Guid.Empty);
				Helper.RestoreColumnWidths(this);
				Helper.SaveColumdWidths(this);
				_view.EndUpdate();
				_bIsInRefill = false;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			IParameterTreeNode parameterTreeNode = default(IParameterTreeNode);
			_DevPath2Node.TryGetValue(text, out parameterTreeNode);
			if (parameterTreeNode != null)
			{
				TreeTableViewNode viewNode = _view.GetViewNode((ITreeTableNode)parameterTreeNode);
				if (viewNode != null)
				{
					_view.TopNode=(viewNode);
				}
			}
		}

		private bool CheckMapping(ICollection subElements)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			foreach (IDataElement subElement in subElements)
			{
				IDataElement val = subElement;
				foreach (IVariableMapping item in (IEnumerable)val.IoMapping.VariableMappings)
				{
					if (!string.IsNullOrEmpty(item.Variable))
					{
						return true;
					}
				}
				if (CheckMapping((ICollection)val.SubElements))
				{
					return true;
				}
			}
			return false;
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

		internal void StoreExpandedNodes(TreeTableViewEventArgs e)
		{
			if (_retrigger.Enabled)
			{
				_retrigger.Stop();
				_retrigger.Tick -= _retrigger_Tick;
			}
			_retrigger.Interval = 1000;
			_retrigger.Tick += _retrigger_Tick;
			_retrigger.Start();
		}

		private void _retrigger_Tick(object sender, EventArgs e)
		{
			_retrigger.Stop();
			_retrigger.Tick -= _retrigger_Tick;
			StoreExpandedNodes();
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

		private void CollectNodes(LDictionary<string, IParameterTreeNode> dictNodes, IParameterTreeNode node)
		{
			dictNodes[node.DevPath]= node;
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

		private void AddDevices(ISVNode node, DeviceNode parent, IPlcNode plcNode, out bool bHasParameter)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Expected O, but got Unknown
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Invalid comparison between Unknown and I4
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Expected O, but got Unknown
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Expected O, but got Unknown
			bHasParameter = false;
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(node.ProjectHandle, node.ObjectGuid);
			if (objectToRead.Object is IDeviceObject)
			{
				IObject @object = objectToRead.Object;
				IDeviceObject9 val = (IDeviceObject9)(object)((@object is IDeviceObject9) ? @object : null);
				bool flag = false;
				foreach (IMappedDevice item in (IEnumerable)((ILogicalDevice)((val is ILogicalDevice) ? val : null)).MappedDevices)
				{
					if (item.IsMapped)
					{
						flag = true;
					}
				}
				if (val is ILogicalDevice2 && ((ILogicalDevice2)((val is ILogicalDevice2) ? val : null)).MappingPossible)
				{
					flag = false;
				}
				foreach (IConnector item2 in (IEnumerable)((IDeviceObject)val).Connectors)
				{
					IConnector6 val2 = (IConnector6)item2;
					if (val2.AllowedPages != null && val2.AllowedPages.Length != 0 && !val2.AllowedPages.Contains(IoMappingEditorPage.PageIdentifierString, StringComparer.InvariantCultureIgnoreCase))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					bool bShowParamsInDevDescOrder = false;
					if (val != null && val.ShowParamsInDevDescOrder)
					{
						bShowParamsInDevDescOrder = true;
					}
					AddParameters(plcNode, node, parent, ((IDeviceObject)val).DeviceParameterSet, -1, bShowParamsInDevDescOrder, ref bHasParameter);
					Guid parentObjectGuid = ((IObject)val).MetaObject.ParentObjectGuid;
					foreach (IConnector item3 in (IEnumerable)((IDeviceObject)val).Connectors)
					{
						IConnector val3 = item3;
						if (val3.IsExplicit || ((int)val3.ConnectorRole == 1 && !IsConnectorToParent(val3, parentObjectGuid)))
						{
							continue;
						}
						if ((int)val3.ConnectorRole == 0)
						{
							bool flag2 = true;
							foreach (IConnector item4 in (IEnumerable)((IDeviceObject)val).Connectors)
							{
								IConnector val4 = item4;
								if (val4.ConnectorId == val3.HostPath && !IsConnectorToParent(val4, parentObjectGuid))
								{
									flag2 = false;
									break;
								}
							}
							if (!flag2)
							{
								continue;
							}
						}
						AddParameters(plcNode, node, parent, val3.HostParameterSet, val3.ConnectorId, bShowParamsInDevDescOrder, ref bHasParameter);
					}
				}
			}
			LSortedList<int, ISVNode> val5 = new LSortedList<int, ISVNode>();
			ISVNode[] children = node.Children;
			foreach (ISVNode val6 in children)
			{
				val5.Add(val6.Index, val6);
			}
			foreach (ISVNode value in val5.Values)
			{
				IMetaObjectStub mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(node.ProjectHandle, value.ObjectGuid);
				if (typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) || APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)) || typeof(ITransientObject).IsAssignableFrom(mos.ObjectType) || (!typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType) && !typeof(IExplicitConnector).IsAssignableFrom(mos.ObjectType)))
				{
					continue;
				}
				DeviceNode deviceNode = new DeviceNode(this, value, parent);
				_DevPath2Node[deviceNode.DevPath]= (IParameterTreeNode)deviceNode;
				AddDevices(value, deviceNode, plcNode, out var bHasParameter2);
				if (bHasParameter2)
				{
					bHasParameter = true;
					if (parent != null)
					{
						parent?.ChildNodes.Add(deviceNode);
					}
					base.RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)(object)parent, ((ITreeTableNode)parent).GetIndex((ITreeTableNode)(object)deviceNode), (ITreeTableNode)(object)deviceNode));
				}
			}
		}

		private bool IsConnectorToParent(IConnector connector, Guid guidParent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (IAdapter item in (IEnumerable)connector.Adapters)
			{
				Guid[] modules = item.Modules;
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i] == guidParent)
					{
						return true;
					}
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
					if (APEnvironment.LocalizationManagerOrNull != null && _bLocalization)
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
				if (APEnvironment.LocalizationManagerOrNull != null && _bLocalization)
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

		private void AddParameters(IPlcNode plcNode, ISVNode node, IDeviceNode parent, IParameterSet paramset, int nConnectorId, bool bShowParamsInDevDescOrder, ref bool bHasParameter)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Expected O, but got Unknown
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Expected O, but got Unknown
			bool flag = false;
			LSortedList<long, IParameter> val = new LSortedList<long, IParameter>();
			if (paramset != null)
			{
				foreach (IParameter item in (IEnumerable)paramset)
				{
					IParameter val2 = item;
					if (!ParameterTreeTableModel.ShouldInsertParameter(val2, ParameterTreeTableModelView.IOMappingsOffline, null))
					{
						continue;
					}
					if (val2 is IParameter6 && bShowParamsInDevDescOrder)
					{
						long num = (int)((IParameter6)((val2 is IParameter6) ? val2 : null)).IndexInDevDesc;
						if (val.ContainsKey(num))
						{
							num = val.Keys[val.Count - 1] + 1;
						}
						val.Add(num, val2);
					}
					else
					{
						val.Add(val2.Id, val2);
					}
				}
			}
			if (val.Count <= 0)
			{
				return;
			}
			DeviceParameterSetProvider deviceParameterSetProvider = new DeviceParameterSetProvider(this, node.ProjectHandle, node.ObjectGuid, nConnectorId);
			deviceParameterSetProvider.LocalizationActive = _bLocalization;
			_liProviders.Add(deviceParameterSetProvider);
			foreach (KeyValuePair<long, IParameter> item2 in val)
			{
				flag = MatchSearchText(deviceParameterSetProvider.GetDevice(), paramset, item2.Value, (IDataElement)(object)item2.Value);
				DataElementNode dataElementNode = new DataElementNode(this, plcNode, (ITreeTableNode)(parent as IParameterTreeNode), deviceParameterSetProvider, item2.Value, GetConverter(GlobalOptionsHelper.DisplayMode));
				_DevPath2Node[dataElementNode.DevPath]= (IParameterTreeNode)dataElementNode;
				if (((IDataElement)item2.Value).HasSubElements)
				{
					bool bHasMatch = false;
					foreach (IDataElement item3 in (IEnumerable)((IDataElement)item2.Value).SubElements)
					{
						IDataElement dataElement = item3;
						AddParameterSubNode(plcNode, deviceParameterSetProvider, item2.Value, dataElementNode, dataElement, ref bHasMatch);
					}
					flag = ((_variableFilter != null && !_variableFilter.MatchSubElements) ? (flag && bHasMatch) : (flag || bHasMatch));
				}
				if (flag)
				{
					bHasParameter = true;
					SectionNode orCreateSectionNode = parent.GetOrCreateSectionNode(item2.Value.Section, parent, parent as IParameterTreeNode);
					if (orCreateSectionNode != null)
					{
						dataElementNode.Parent = (ITreeTableNode)(object)orCreateSectionNode;
						orCreateSectionNode.AddDataElementNode(dataElementNode);
					}
					else if (parent is DeviceNode)
					{
						(parent as DeviceNode).ChildNodes.Add(dataElementNode);
						base.RaiseInserted(new TreeTableModelEventArgs((ITreeTableNode)(parent as IParameterTreeNode), ((ITreeTableNode)(parent as IParameterTreeNode)).GetIndex((ITreeTableNode)(object)dataElementNode), (ITreeTableNode)(object)dataElementNode));
					}
				}
			}
		}

		private void AddParameterSubNode(IPlcNode plcNode, IParameterSetProvider parameterSetProvider, IParameter parameter, DataElementNode node, IDataElement dataElement, ref bool bHasMatch)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Expected O, but got Unknown
			bool flag = false;
			Debug.Assert(node != null);
			Debug.Assert(dataElement != null);
			if (dataElement is IDataElement6 && (int)((IDataElement6)((dataElement is IDataElement6) ? dataElement : null)).GetAccessRight(false) == 0)
			{
				return;
			}
			IDataElement2 dataelement = (IDataElement2)(object)((dataElement is IDataElement2) ? dataElement : null);
			DataElementNode dataElementNode = new DataElementNode(this, plcNode, node, parameterSetProvider, dataelement, dataElement.Identifier);
			_DevPath2Node[dataElementNode.DevPath]= (IParameterTreeNode)dataElementNode;
			flag = MatchSearchText(parameterSetProvider.GetDevice(), parameterSetProvider.GetParameterSet(bToModify: false), parameter, dataElement);
			if (dataElement.HasSubElements)
			{
				bool bHasMatch2 = false;
				foreach (IDataElement item in (IEnumerable)dataElement.SubElements)
				{
					IDataElement dataElement2 = item;
					AddParameterSubNode(plcNode, parameterSetProvider, parameter, dataElementNode, dataElement2, ref bHasMatch2);
				}
				flag = ((_variableFilter != null && !_variableFilter.MatchSubElements) ? (flag && bHasMatch2) : (flag || bHasMatch2));
			}
			bHasMatch |= flag;
			if (flag)
			{
				node.AddDataElementNode(dataElementNode);
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
	}
}
