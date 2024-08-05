using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.BrowserCommands;
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Common;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceEditor.CustomizedOnline;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.LegacyOnlineManager;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_io_mapping.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceEditor.Editor.chm::/I_O_Mapping.htm")]
	public class IoMappingEditorPage : UserControl, IEditorPage3, IEditorPage2, IEditorPage, IEditorPageAppearance2, IEditorPageAppearance, IPrintableEx, IVisibleEditor, IRefactoringCommandContext, IEditorBasedFindReplace, IBrowserCommandsTarget
	{
		private IContainer components;

		private bool _bInEnableWatchListRange;

		private Panel _panelDescription;

		private GroupBox _grpBoxTask;

		private ComboBox _cbBusCycle;

		private Label label4;

		private Label _explanationLabel2;

		private PictureBox _pictureBox1;

		private PictureBox _pictureBox2;

		private Label _explanationLabel1;

		private Panel _panelWarning;

		private Label label3;

		private PictureBox _pbWarning;

		private Panel _panelChannels;

		private TreetableViewWithColumnStorage _mappingTreeTableView;

		private TextBox _descriptionTextBox;

		private Panel panel2;

		private Panel _panelBusCycle;

		private Button _btResetDefault;

		private IOMappingEditor _editor;

		private ComboBox _comboUpdateVariables;

		private Label label5;

		private Guid additionalContextMenuGuid = Guid.Empty;

		private Guid COMMAND_BOOTPROJECT_ACTIVE_APP = new Guid("{cf46872e-7cf6-484e-acee-f8b9da763449}");

		private Guid COMMAND_ONLINECHANGE_ACTIVE_APP = new Guid("{F6B049E7-6C27-4736-9A45-4CC93C3E8C9D}");

		private Guid COMMAND_DOWNLOAD_ACTIVE_APP = new Guid("{CC9AD1E5-FE78-4dc6-A4BE-B1991BF068EA}");

		private ToolStrip _toolStrip;

		private ToolStripLabel toolStripLabel1;

		private ToolStripTextBox _tbFilter;

		private ToolStripLabel toolStripLabel2;

		private ToolStripComboBox _comboFilter;

		private Guid COMMAND_ONLINECHANGE_SELECTED_APP = new Guid("{C6FCCD24-B0DB-4EAB-A4B4-C9068340FAC2}");

		private bool _bDoRefill;

		private Timer Timer;

		private ToolStripButton _btAddFB;

		private ToolStripButton _btGoIoChannel;

		private long _delayTicks;

		private bool _bInit;

		private IDeviceObject _plcDevice;

		private IDriverInfo5 _driverInfo;

		private bool _bCycleTaskEnable;

		private bool _bHasLogicalDevices;

		private bool _bDuringHandleCreated;

		private bool bGetParameterInProgress;

		private Timer _collapseTimer;

		private int iLastColumnIndex = -1;

		private bool _bReload;

		private bool _bIsHidden = true;

		public static readonly Guid GUID_EDITCUT = new Guid("{586FB001-60CA-4dd1-A2F9-F9319EE13C95}");

		public static readonly Guid GUID_EDITCOPY = new Guid("{E76B8E0B-9247-41e7-93D5-80FE36AF9449}");

		public static readonly Guid GUID_EDITPASTE = new Guid("{73A7678B-2707-44f6-963B-8A4B3C05A331}");

		public static readonly Guid GUID_EDITDELETE = new Guid("{C5AAECF0-F55A-4864-871E-4584D1CCC9AF}");

		public static readonly Guid GUID_GOTODEFINITION = new Guid("{d19a9182-fc62-4ea5-bd01-cad58e092281}");

		public static readonly Guid GUID_CROSSREFERENCES = new Guid("{2D499D13-AF93-43b4-AFDA-E98349935B51}");

		public static readonly Guid GUID_EDITUNDO = new Guid("{9ECCAF22-3293-4165-943E-88C2C40B4A58}");

		public static readonly Guid GUID_EDITREDO = new Guid("{871B29A1-9E9F-47f9-A5CE-D56C40976743}");

		public static readonly Guid GUID_REFACTORING = new Guid("{EEA6D3F9-7B35-4843-8D5B-911B5E6D2DAB}");

		public static readonly Guid GUID_EDITSELECTALL = new Guid("{1C36CA5E-E26D-4edc-9AB7-C7D87690C328}");

		internal IUndoManager UndoMgr => _editor.EditorUndoMgr;

		public bool CycleTaskEnable => _bCycleTaskEnable;

		public IOMappingEditor Editor => _editor;

		public bool HasLogicalDevices => _bHasLogicalDevices;

		internal bool ChannelPanelVisible
		{
			get
			{
				return _panelChannels.Visible;
			}
			set
			{
				_panelChannels.Visible = value;
				_panelBusCycle.Dock = ((!value) ? DockStyle.Top : DockStyle.Bottom);
			}
		}

		private ParameterTreeTableModel Model => _editor.ChannelsModel;

		internal TreeTableView MappingTreeTableViewCellValue => (TreeTableView)(object)_mappingTreeTableView;

		public string PageName => _editor.PageName;

		public Icon Icon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceEditor.Resources.IOMapping.ico");

		public Control Control => this;

		internal TreetableViewWithColumnStorage MappingTreeTableView => _mappingTreeTableView;

		public string PageIdentifier => PageIdentifierString;

		public static string PageIdentifierString => "IOMappingEditor";

		public bool IsHidden
		{
			get
			{
				return _bIsHidden;
			}
			set
			{
				_bIsHidden = value;
				if (_bIsHidden)
				{
					Model.ReleaseMonitoring(bClose: false);
				}
				else
				{
					EnableMonitoringRange();
				}
				if (!_bIsHidden)
				{
					UpdateView();
				}
			}
		}

		private bool CanUndo => UndoMgr.CanUndo;

		private bool CanRedo => UndoMgr.CanRedo;

		public bool HasOnlineMode => true;

		private IFbInstance5 MappedFBInstance
		{
			get
			{
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Expected O, but got Unknown
				if (((TreeTableView)_mappingTreeTableView).FocusedNode != null)
				{
					DataElementNode dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(((TreeTableView)_mappingTreeTableView).FocusedNode) as DataElementNode;
					if (dataElementNode != null && dataElementNode.DataElement != null && ((ICollection)dataElementNode.DataElement.IoMapping.VariableMappings).Count > 0 && dataElementNode.DataElement.IoMapping.VariableMappings[0] is IVariableMapping3)
					{
						IVariableMapping obj = dataElementNode.DataElement.IoMapping.VariableMappings[0];
						string ioChannelFBInstance = ((IVariableMapping3)((obj is IVariableMapping3) ? obj : null)).IoChannelFBInstance;
						IOMappingEditor editor = _editor;
						object obj2;
						if (editor == null)
						{
							obj2 = null;
						}
						else
						{
							IParameterSetProvider parameterSetProvider = editor.GetParameterSetProvider();
							if (parameterSetProvider == null)
							{
								obj2 = null;
							}
							else
							{
								IIoProvider ioProvider = parameterSetProvider.GetIoProvider(bToModify: false);
								obj2 = ((ioProvider != null) ? ioProvider.DriverInfo : null);
							}
						}
						if (obj2 != null)
						{
							foreach (IRequiredLib item in (IEnumerable)_editor.GetParameterSetProvider().GetIoProvider(bToModify: false).DriverInfo
								.RequiredLibs)
							{
								foreach (IFbInstance item2 in (IEnumerable)item.FbInstances)
								{
									IFbInstance val = item2;
									if (val is IFbInstance5 && ((IFbInstance5)((val is IFbInstance5) ? val : null)).BaseName == ioChannelFBInstance)
									{
										return (IFbInstance5)(object)((val is IFbInstance5) ? val : null);
									}
								}
							}
						}
					}
				}
				return null;
			}
		}

		public IoMappingEditorPage()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			InitializeComponent();
			_toolStrip.Renderer = (ToolStripRenderer)new ProfessionalToolStripRenderer();
			_bReload = true;
		}

		public IoMappingEditorPage(IOMappingEditor editor)
			: this()
		{
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Expected O, but got Unknown
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Expected O, but got Unknown
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Expected O, but got Unknown
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Expected O, but got Unknown
			_editor = editor;
			LSortedList<string, IIoMappingEditorFilterFactory> val = new LSortedList<string, IIoMappingEditorFilterFactory>();
			if (_editor is ConnectorIoMappingEditor)
			{
				int connectorId = (_editor as ConnectorIoMappingEditor).ConnectorId;
				(_editor as ConnectorIoMappingEditor).ConnectorEditorFrame.GetConnector(connectorId, false);
			}
			foreach (IIoMappingEditorFilterFactory ioMappingFilter in APEnvironment.IoMappingFilters)
			{
				if (ioMappingFilter.ShowFilter(_editor.GetDeviceObject(bToModify: false), editor.GetParameterSetProvider().GetParameterSet(bToModify: false)) && !val.ContainsKey(ioMappingFilter.FilterName))
				{
					val.Add(ioMappingFilter.FilterName, ioMappingFilter);
				}
			}
			_comboFilter.Items.Clear();
			FilterItem item = new FilterItem(null);
			_comboFilter.Items.Add(item);
			foreach (IIoMappingEditorFilterFactory value3 in val.Values)
			{
				FilterItem item2 = new FilterItem(value3);
				_comboFilter.Items.Add(item2);
			}
			_comboFilter.SelectedIndex = 0;
			try
			{
				object customizationValue = DeviceEditorFactory.GetCustomizationValue(DeviceEditorFactory.stAdditionalContextMenue);
				if (customizationValue is Guid)
				{
					additionalContextMenuGuid = (Guid)customizationValue;
				}
			}
			catch
			{
			}
			_mappingTreeTableView.ObjectGuid = _editor.ObjectGuid;
			int a = -1;
			if (_editor is ConnectorIoMappingEditor)
			{
				a = (_editor as ConnectorIoMappingEditor).ConnectorId;
			}
			_mappingTreeTableView.IdentificationGuid = new Guid((uint)a, 40444, 19448, 178, 100, 76, 244, 205, 124, 188, 31);
			((TreeTableView)_mappingTreeTableView).Model=((ITreeTableModel)(object)_editor.ChannelsModel);
			if (editor.ChannelsModel != null)
			{
				_panelChannels.Visible = ((AbstractTreeTableModel)_editor.ChannelsModel).Sentinel.HasChildren;
				_panelWarning.Visible = _panelChannels.Visible;
				_panelDescription.Visible = _panelChannels.Visible;
			}
			if (!_panelChannels.Visible)
			{
				_panelBusCycle.Dock = DockStyle.Top;
			}
			_bInit = true;
			IDeviceObject val2 = null;
			if (_editor is ConnectorIoMappingEditor)
			{
				ConnectorIoMappingEditor connectorIoMappingEditor = (ConnectorIoMappingEditor)_editor;
				IConnector connector = connectorIoMappingEditor.ConnectorEditorFrame.GetConnector(connectorIoMappingEditor.ConnectorId, false);
				val2 = connector.GetDeviceObject();
				if (typeof(IConnector6).IsAssignableFrom(((object)connector).GetType()))
				{
					IConnector6 val3 = (IConnector6)connector;
					if (val3 != null)
					{
						LoadAlwaysMapping(val3);
						_plcDevice = _editor.GetHost();
					}
					if (val3 is IConnector9)
					{
						_comboUpdateVariables.Enabled = !((IConnector9)((val3 is IConnector9) ? val3 : null)).AlwaysMappingDisabled;
					}
				}
				ref IDriverInfo5 driverInfo = ref _driverInfo;
				IDriverInfo driverInfo2 = connectorIoMappingEditor.GetDriverInfo(bToModify: false);
				driverInfo = (IDriverInfo5)(object)((driverInfo2 is IDriverInfo5) ? driverInfo2 : null);
			}
			if (_editor is DeviceIoMappingEditor)
			{
				DeviceIoMappingEditor deviceIoMappingEditor = _editor as DeviceIoMappingEditor;
				if (deviceIoMappingEditor != null)
				{
					val2 = deviceIoMappingEditor.DeviceEditorFrame.GetDeviceObject(false);
					if (val2 != null)
					{
						LoadAlwaysMapping(val2.DeviceParameterSet);
					}
				}
			}
			if (_driverInfo != null)
			{
				_bCycleTaskEnable = false;
				foreach (IRequiredLib item3 in (IEnumerable)((IDriverInfo)_driverInfo).RequiredLibs)
				{
					foreach (IFbInstance item4 in (IEnumerable)item3.FbInstances)
					{
						IFbInstance val4 = item4;
						if (!(val4.Instance.Variable != ""))
						{
							continue;
						}
						foreach (ICyclicCall item5 in (IEnumerable)val4.CyclicCalls)
						{
							ICyclicCall val5 = item5;
							if (val5.Task == "#buscycletask" || val5.Task == "#userdeftask")
							{
								_bCycleTaskEnable = true;
							}
						}
					}
				}
				if (((IDriverInfo)_driverInfo).NeedsBusCycle)
				{
					_bCycleTaskEnable = true;
				}
				if (_driverInfo is IDriverInfo8 && (_driverInfo as IDriverInfo8).NeedsBusCycleBeforeRead)
				{
					_bCycleTaskEnable = true;
				}
			}
			if (_bCycleTaskEnable)
			{
				_panelBusCycle.Visible = true;
				FillBusCycleList(_plcDevice, _driverInfo);
			}
			else
			{
				_panelBusCycle.Visible = false;
			}
			if (IOMappingEditor.EnableLogicalDevices && val2 != null && val2 is ILogicalDevice)
			{
				ILogicalDevice val6 = (ILogicalDevice)(object)((val2 is ILogicalDevice) ? val2 : null);
				if (val6.IsPhysical && val6.MappedDevices != null)
				{
					foreach (IMappedDevice item6 in (IEnumerable)val6.MappedDevices)
					{
						IMappedDevice val7 = item6;
						LogicalIOControl value = new LogicalIOControl(_editor, val7.Index)
						{
							Dock = DockStyle.Bottom
						};
						panel2.Controls.Add(value);
						_bHasLogicalDevices = true;
					}
				}
				if (val6.IsLogical)
				{
					PhysicalIOControl value2 = new PhysicalIOControl(_editor, 0)
					{
						Dock = DockStyle.Bottom
					};
					panel2.Controls.Add(value2);
					_bHasLogicalDevices = true;
				}
			}
			_bInit = false;
		}

		private void LoadAlwaysMapping(object loadobject)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Invalid comparison between Unknown and I4
			AlwaysMappingMode val = (AlwaysMappingMode)0;
			bool flag = false;
			if (loadobject is IParameterSet)
			{
				flag = ParameterSet4_GetAlwaysMapping((IParameterSet)((loadobject is IParameterSet) ? loadobject : null));
				if (loadobject is IParameterSet5)
				{
					val = ((IParameterSet5)((loadobject is IParameterSet5) ? loadobject : null)).AlwaysMappingMode;
				}
				else if (_comboUpdateVariables.Items.Count > 1)
				{
					_comboUpdateVariables.Items.RemoveAt(2);
				}
			}
			if (loadobject is IConnector6)
			{
				flag = ((IConnector6)((loadobject is IConnector6) ? loadobject : null)).AlwaysMapping;
				if (loadobject is IConnector11)
				{
					val = ((IConnector11)((loadobject is IConnector11) ? loadobject : null)).AlwaysMappingMode;
				}
				else if (_comboUpdateVariables.Items.Count > 1)
				{
					_comboUpdateVariables.Items.RemoveAt(2);
				}
			}
			if (flag)
			{
				if ((int)val != 0)
				{
					if ((int)val == 1)
					{
						_comboUpdateVariables.SelectedIndex = 2;
					}
				}
				else
				{
					_comboUpdateVariables.SelectedIndex = 1;
				}
			}
			else
			{
				_comboUpdateVariables.SelectedIndex = 0;
			}
		}

		private void SaveAlwaysMapping(object saveobject)
		{
			switch (_comboUpdateVariables.SelectedIndex)
			{
			case 0:
				if (saveobject is IParameterSet)
				{
					ParameterSet4_SetAlwaysMapping((IParameterSet)((saveobject is IParameterSet) ? saveobject : null), value: false);
				}
				if (saveobject is IConnector6)
				{
					((IConnector6)((saveobject is IConnector6) ? saveobject : null)).AlwaysMapping=(false);
				}
				break;
			case 1:
				if (saveobject is IParameterSet)
				{
					ParameterSet4_SetAlwaysMapping((IParameterSet)((saveobject is IParameterSet) ? saveobject : null), value: true);
				}
				if (saveobject is IParameterSet5)
				{
					((IParameterSet5)((saveobject is IParameterSet5) ? saveobject : null)).AlwaysMappingMode=((AlwaysMappingMode)0);
				}
				if (saveobject is IConnector6)
				{
					((IConnector6)((saveobject is IConnector6) ? saveobject : null)).AlwaysMapping=(true);
				}
				if (saveobject is IConnector11)
				{
					((IConnector11)((saveobject is IConnector11) ? saveobject : null)).AlwaysMappingMode=((AlwaysMappingMode)0);
				}
				break;
			case 2:
				if (saveobject is IParameterSet)
				{
					ParameterSet4_SetAlwaysMapping((IParameterSet)((saveobject is IParameterSet) ? saveobject : null), value: true);
				}
				if (saveobject is IParameterSet5)
				{
					((IParameterSet5)((saveobject is IParameterSet5) ? saveobject : null)).AlwaysMappingMode=((AlwaysMappingMode)1);
				}
				if (saveobject is IConnector6)
				{
					((IConnector6)((saveobject is IConnector6) ? saveobject : null)).AlwaysMapping=(true);
				}
				if (saveobject is IConnector11)
				{
					((IConnector11)((saveobject is IConnector11) ? saveobject : null)).AlwaysMappingMode=((AlwaysMappingMode)1);
				}
				break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Timer.Tick -= Timer_Tick;
				if (_collapseTimer != null)
				{
					_collapseTimer.Tick -= _collapseTimer_Tick;
				}
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			if (CustomizedOnlineHelper.HasCustomizedOnlineFunctionality(_plcDevice))
			{
				ILegacyOnlineManager legacyOnlineMgrOrNull = APEnvironment.LegacyOnlineMgrOrNull;
				if (legacyOnlineMgrOrNull != null)
				{
					legacyOnlineMgrOrNull.AfterDeviceLogin+=(new AfterLoginEventHandler(OnCustomizedOnlineManagerAfterDeviceLogin));
					legacyOnlineMgrOrNull.AfterDeviceLogout+=(new AfterLogoutEventHandler(OnCustomizedOnlineManagerAfterDeviceLogout));
					if (legacyOnlineMgrOrNull.IsOnline(((IObject)_plcDevice).MetaObject.ObjectGuid))
					{
						OnOnlineStateChanged(this, new EventArgs());
					}
				}
			}
			if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager7)
			{
				((ICommandManager3)(ICommandManager7)((IEngine)APEnvironment.Engine).CommandManager).CommandExecuted+=((EventHandler<CommandExecutedEventArgs>)OnCommandExecuted);
			}
			EnableMonitoringRange();
			try
			{
				_bDuringHandleCreated = true;
				if (_editor.OnlineState.OnlineApplication != Guid.Empty)
				{
					OnOnlineStateChanged(this, new EventArgs());
				}
			}
			finally
			{
				_bDuringHandleCreated = false;
			}
			base.OnHandleCreated(e);
			if (((TreeTableView)_mappingTreeTableView).Model != null)
			{
				((ParameterTreeTableModel)(object)((TreeTableView)_mappingTreeTableView).Model).AttachEventHandlers();
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			Timer.Stop();
			Timer.Tick -= Timer_Tick;
			if (_collapseTimer != null)
			{
				_collapseTimer.Tick -= _collapseTimer_Tick;
			}
			if (((TreeTableView)_mappingTreeTableView).Model != null)
			{
				((ParameterTreeTableModel)(object)((TreeTableView)_mappingTreeTableView).Model).ReleaseMonitoring(bClose: true);
				((ParameterTreeTableModel)(object)((TreeTableView)_mappingTreeTableView).Model).DetachEventHandlers();
			}
			ILegacyOnlineManager legacyOnlineMgrOrNull = APEnvironment.LegacyOnlineMgrOrNull;
			if (legacyOnlineMgrOrNull != null)
			{
				legacyOnlineMgrOrNull.AfterDeviceLogin-=(new AfterLoginEventHandler(OnCustomizedOnlineManagerAfterDeviceLogin));
				legacyOnlineMgrOrNull.AfterDeviceLogout-=(new AfterLogoutEventHandler(OnCustomizedOnlineManagerAfterDeviceLogout));
			}
			if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager7)
			{
				((ICommandManager3)(ICommandManager7)((IEngine)APEnvironment.Engine).CommandManager).CommandExecuted-=((EventHandler<CommandExecutedEventArgs>)OnCommandExecuted);
			}
			base.OnHandleDestroyed(e);
		}

		private void OnCommandExecuted(object sender, CommandExecutedEventArgs e)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && _editor != null && _editor.OnlineState.OnlineApplication == ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication && (((AbstractCommandExecutionEventArgs)e).CommandGuid == COMMAND_BOOTPROJECT_ACTIVE_APP || ((AbstractCommandExecutionEventArgs)e).CommandGuid == COMMAND_DOWNLOAD_ACTIVE_APP || ((AbstractCommandExecutionEventArgs)e).CommandGuid == COMMAND_ONLINECHANGE_ACTIVE_APP || ((AbstractCommandExecutionEventArgs)e).CommandGuid == COMMAND_ONLINECHANGE_SELECTED_APP || (((AbstractCommandExecutionEventArgs)e).CustomizedCommand != null && (((AbstractCommandExecutionEventArgs)e).CustomizedCommand.OriginalCommandGuid == COMMAND_BOOTPROJECT_ACTIVE_APP || ((AbstractCommandExecutionEventArgs)e).CustomizedCommand.OriginalCommandGuid == COMMAND_DOWNLOAD_ACTIVE_APP || ((AbstractCommandExecutionEventArgs)e).CustomizedCommand.OriginalCommandGuid == COMMAND_ONLINECHANGE_ACTIVE_APP || ((AbstractCommandExecutionEventArgs)e).CustomizedCommand.OriginalCommandGuid == COMMAND_ONLINECHANGE_SELECTED_APP))) && e.Exception == null && !e.ExcecutionCancelled)
			{
				Model.ReleaseMonitoring(bClose: false);
				OnOnlineStateChanged(sender, (EventArgs)(object)e);
			}
		}

		private void OnCustomizedOnlineManagerAfterDeviceLogout(object sender, LegacyOnlineEventArgs e)
		{
			OnOnlineStateChanged(this, new EventArgs());
			((ParameterTreeTableModel)(object)((TreeTableView)_mappingTreeTableView).Model).ReleaseMonitoring(bClose: true);
		}

		private void OnCustomizedOnlineManagerAfterDeviceLogin(object sender, LegacyOnlineEventArgs e)
		{
			OnOnlineStateChanged(this, new EventArgs());
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		internal void OnModuleStatusChanged(object sender, IModuleStatusEventArgs e)
		{
			UpdateModuleStatus(e.ProjectHandle, e.ObjectGuid, e.ConnectorId);
		}

		internal void UpdateModuleStatus(int nProjectHandle, Guid ObjectGuid, int nConnectorId)
		{
			ModuleStatus val = default(ModuleStatus);
			if (_editor.IsApplicationOnline && APEnvironment.DeviceController.GetModuleStatus(nProjectHandle, ObjectGuid, nConnectorId, out val))
			{
				if (((int)val & 0x100) == 256 || ((int)val & 0x200) == 512 || ((int)val & 0xF1) != 241)
				{
					_panelWarning.Visible = true;
				}
				else
				{
					_panelWarning.Visible = false;
				}
			}
		}

		internal void OnOnlineStateChanged(object sender, EventArgs e)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (_editor.LogicalDeviceIsMapped || Model == null || Model.IsExcludedFromBuild)
			{
				return;
			}
			Model.OnlineApplication = _editor.OnlineState.OnlineApplication;
			EnableMonitoringRange();
			ILegacyOnlineManager legacyOnlineMgrOrNull = APEnvironment.LegacyOnlineMgrOrNull;
			bool flag = legacyOnlineMgrOrNull != null;
			if (_editor.OnlineState.OnlineApplication != Guid.Empty || (flag && _plcDevice != null && legacyOnlineMgrOrNull.IsOnline(((IObject)_plcDevice).MetaObject.ObjectGuid)))
			{
				if (!_bDuringHandleCreated)
				{
					_mappingTreeTableView.SaveTreetableColumnsWidth();
				}
				Model.ShowOnlineValue(bIsMappingEditor: true, !flag, bShowCurrentValueColumn: true);
				_comboUpdateVariables.Enabled = false;
				_btResetDefault.Enabled = false;
				_cbBusCycle.Enabled = false;
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				int nConnectorId = 0;
				ConnectorIoMappingEditor connectorIoMappingEditor = _editor as ConnectorIoMappingEditor;
				if (connectorIoMappingEditor != null)
				{
					nConnectorId = connectorIoMappingEditor.ConnectorId;
				}
				UpdateModuleStatus(handle, _editor.ObjectGuid, nConnectorId);
			}
			else
			{
				CheckFocuseNode(bCancel: true);
				_mappingTreeTableView.SaveTreetableColumnsWidth();
				Model.HideOnlineValue();
				if (_editor is ConnectorIoMappingEditor)
				{
					ConnectorIoMappingEditor connectorIoMappingEditor2 = (ConnectorIoMappingEditor)_editor;
					IConnector connector = connectorIoMappingEditor2.ConnectorEditorFrame.GetConnector(connectorIoMappingEditor2.ConnectorId, false);
					if (connector is IConnector9)
					{
						_comboUpdateVariables.Enabled = !((IConnector9)((connector is IConnector9) ? connector : null)).AlwaysMappingDisabled;
					}
					else
					{
						_comboUpdateVariables.Enabled = true;
					}
				}
				if (_editor is DeviceIoMappingEditor)
				{
					_comboUpdateVariables.Enabled = true;
				}
				_panelWarning.Visible = false;
				_btResetDefault.Enabled = true;
				_cbBusCycle.Enabled = true;
			}
			_mappingTreeTableView.RestoreExpandedNodes(bExpandIfNoData: false);
			AdjustColumnWidths(bForceAdjust: false);
		}

		private void EnableMonitoringRange()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (!base.IsHandleCreated || _bInEnableWatchListRange)
			{
				return;
			}
			try
			{
				_bInEnableWatchListRange = true;
				if (_editor.OnlineState.OnlineApplication != Guid.Empty || CustomizedOnlineHelper.HasCustomizedOnlineFunctionality(_plcDevice))
				{
					TreeTableViewNode val = ((TreeTableView)_mappingTreeTableView).TopNode;
					LList<DataElementNode> val2 = new LList<DataElementNode>();
					while (val != null && val.Displayed)
					{
						DataElementNode dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(val) as DataElementNode;
						if (dataElementNode != null)
						{
							val2.Add(dataElementNode);
						}
						val = val.NextVisibleNode;
					}
					DataElementNode[] array = new DataElementNode[val2.Count];
					val2.CopyTo(array);
					if (Model != null)
					{
						Model.EnableMonitoring(array);
					}
				}
				else if (Model != null)
				{
					Model.EnableMonitoring(null);
				}
			}
			finally
			{
				_bInEnableWatchListRange = false;
			}
		}

		private void AdjustColumnWidths(bool bForceAdjust)
		{
			int num;
			int num2;
			using (Graphics graphics = CreateGraphics())
			{
				num = graphics.MeasureString("BCDRTBCDRTBCDRT", ((Control)(object)_mappingTreeTableView).Font).ToSize().Width;
				num += ((TreeTableView)_mappingTreeTableView).Indent * 3;
				num2 = graphics.MeasureString("M %QX0000.1", ((Control)(object)_mappingTreeTableView).Font).ToSize().Width;
			}
			if (bForceAdjust || !_mappingTreeTableView.RestoreTreetableColumnWidth(0))
			{
				if (((TreeTableView)_mappingTreeTableView).DoNotShrinkColumnsAutomatically)
				{
					if (((TreeTableView)_mappingTreeTableView).Columns[0].Width < num)
					{
						_mappingTreeTableView.SetColumnWidth(0, num, bForceAdjust);
					}
				}
				else
				{
					_mappingTreeTableView.SetColumnWidth(0, num, bForceAdjust);
				}
			}
			for (int i = 1; i < ((TreeTableView)_mappingTreeTableView).Columns.Count; i++)
			{
				_mappingTreeTableView.AdjustColumnWidth(i, bConsiderHeaderText: true, bSave: true, bForceAdjust);
			}
			if (((TreeTableView)_mappingTreeTableView).Columns.Count > 3 && ((TreeTableView)_mappingTreeTableView).Columns[3].Width < num2)
			{
				_mappingTreeTableView.SetColumnWidth(3, num2, bForceAdjust);
			}
		}

		internal void CheckFocuseNode(bool bCancel)
		{
			if (_mappingTreeTableView != null && ((TreeTableView)_mappingTreeTableView).FocusedNode != null)
			{
				TreeTableViewNode focusedNode = ((TreeTableView)_mappingTreeTableView).FocusedNode;
				int num = default(int);
				if (focusedNode != null && focusedNode.IsFocused(out num) && focusedNode.IsEditing(num))
				{
					focusedNode.EndEdit(num, bCancel);
				}
			}
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IoMappingEditorPage));
            this._panelDescription = new System.Windows.Forms.Panel();
            this._explanationLabel2 = new System.Windows.Forms.Label();
            this._pictureBox1 = new System.Windows.Forms.PictureBox();
            this._pictureBox2 = new System.Windows.Forms.PictureBox();
            this._explanationLabel1 = new System.Windows.Forms.Label();
            this._panelBusCycle = new System.Windows.Forms.Panel();
            this._grpBoxTask = new System.Windows.Forms.GroupBox();
            this._cbBusCycle = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._panelWarning = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this._pbWarning = new System.Windows.Forms.PictureBox();
            this._panelChannels = new System.Windows.Forms.Panel();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this._tbFilter = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this._comboFilter = new System.Windows.Forms.ToolStripComboBox();
            this._btAddFB = new System.Windows.Forms.ToolStripButton();
            this._btGoIoChannel = new System.Windows.Forms.ToolStripButton();
            this.label5 = new System.Windows.Forms.Label();
            this._comboUpdateVariables = new System.Windows.Forms.ComboBox();
            this._btResetDefault = new System.Windows.Forms.Button();
            this._descriptionTextBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this._panelDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox2)).BeginInit();
            this._panelBusCycle.SuspendLayout();
            this._grpBoxTask.SuspendLayout();
            this._panelWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarning)).BeginInit();
            this._panelChannels.SuspendLayout();
            this._toolStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _panelDescription
            // 
            this._panelDescription.Controls.Add(this._explanationLabel2);
            this._panelDescription.Controls.Add(this._pictureBox1);
            this._panelDescription.Controls.Add(this._pictureBox2);
            this._panelDescription.Controls.Add(this._explanationLabel1);
            this._panelDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelDescription.Location = new System.Drawing.Point(0, 335);
            this._panelDescription.Name = "_panelDescription";
            this._panelDescription.Size = new System.Drawing.Size(700, 32);
            this._panelDescription.TabIndex = 11;
            // 
            // _explanationLabel2
            // 
            this._explanationLabel2.AutoSize = true;
            this._explanationLabel2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._explanationLabel2.Location = new System.Drawing.Point(209, 6);
            this._explanationLabel2.Name = "_explanationLabel2";
            this._explanationLabel2.Size = new System.Drawing.Size(132, 13);
            this._explanationLabel2.TabIndex = 5;
            this._explanationLabel2.Text = "= Map to existing variable";
            // 
            // _pictureBox1
            // 
            this._pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("_pictureBox1.Image")));
            this._pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._pictureBox1.Location = new System.Drawing.Point(3, 6);
            this._pictureBox1.Name = "_pictureBox1";
            this._pictureBox1.Size = new System.Drawing.Size(16, 16);
            this._pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pictureBox1.TabIndex = 2;
            this._pictureBox1.TabStop = false;
            // 
            // _pictureBox2
            // 
            this._pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("_pictureBox2.Image")));
            this._pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._pictureBox2.Location = new System.Drawing.Point(187, 6);
            this._pictureBox2.Name = "_pictureBox2";
            this._pictureBox2.Size = new System.Drawing.Size(16, 16);
            this._pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pictureBox2.TabIndex = 4;
            this._pictureBox2.TabStop = false;
            // 
            // _explanationLabel1
            // 
            this._explanationLabel1.AutoSize = true;
            this._explanationLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._explanationLabel1.Location = new System.Drawing.Point(25, 6);
            this._explanationLabel1.Name = "_explanationLabel1";
            this._explanationLabel1.Size = new System.Drawing.Size(115, 13);
            this._explanationLabel1.TabIndex = 3;
            this._explanationLabel1.Text = "= Create new variable";
            // 
            // _panelBusCycle
            // 
            this._panelBusCycle.Controls.Add(this._grpBoxTask);
            this._panelBusCycle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelBusCycle.Location = new System.Drawing.Point(0, 367);
            this._panelBusCycle.Name = "_panelBusCycle";
            this._panelBusCycle.Size = new System.Drawing.Size(700, 58);
            this._panelBusCycle.TabIndex = 6;
            // 
            // _grpBoxTask
            // 
            this._grpBoxTask.Controls.Add(this._cbBusCycle);
            this._grpBoxTask.Controls.Add(this.label4);
            this._grpBoxTask.Location = new System.Drawing.Point(3, 6);
            this._grpBoxTask.Name = "_grpBoxTask";
            this._grpBoxTask.Size = new System.Drawing.Size(305, 44);
            this._grpBoxTask.TabIndex = 14;
            this._grpBoxTask.TabStop = false;
            this._grpBoxTask.Text = "Bus Cycle Options";
            // 
            // _cbBusCycle
            // 
            this._cbBusCycle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cbBusCycle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbBusCycle.FormattingEnabled = true;
            this._cbBusCycle.Location = new System.Drawing.Point(114, 15);
            this._cbBusCycle.Name = "_cbBusCycle";
            this._cbBusCycle.Size = new System.Drawing.Size(173, 21);
            this._cbBusCycle.TabIndex = 13;
            this._cbBusCycle.SelectedIndexChanged += new System.EventHandler(this._cbBusCycle_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(7, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Bus cycle task";
            // 
            // _panelWarning
            // 
            this._panelWarning.Controls.Add(this.label3);
            this._panelWarning.Controls.Add(this._pbWarning);
            this._panelWarning.Dock = System.Windows.Forms.DockStyle.Top;
            this._panelWarning.Location = new System.Drawing.Point(0, 0);
            this._panelWarning.Name = "_panelWarning";
            this._panelWarning.Size = new System.Drawing.Size(700, 44);
            this._panelWarning.TabIndex = 13;
            this._panelWarning.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(41, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(318, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "The bus is not running. The shown values are perhaps not actual";
            // 
            // _pbWarning
            // 
            this._pbWarning.Image = ((System.Drawing.Image)(resources.GetObject("_pbWarning.Image")));
            this._pbWarning.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._pbWarning.Location = new System.Drawing.Point(3, 3);
            this._pbWarning.Name = "_pbWarning";
            this._pbWarning.Size = new System.Drawing.Size(32, 32);
            this._pbWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pbWarning.TabIndex = 1;
            this._pbWarning.TabStop = false;
            // 
            // _panelChannels
            // 
            this._panelChannels.AutoSize = true;
            this._panelChannels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._panelChannels.Controls.Add(this._toolStrip);
            this._panelChannels.Controls.Add(this.label5);
            this._panelChannels.Controls.Add(this._comboUpdateVariables);
            this._panelChannels.Controls.Add(this._btResetDefault);
            this._panelChannels.Controls.Add(this._descriptionTextBox);
            this._panelChannels.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelChannels.Location = new System.Drawing.Point(0, 44);
            this._panelChannels.Name = "_panelChannels";
            this._panelChannels.Size = new System.Drawing.Size(700, 291);
            this._panelChannels.TabIndex = 9;
            // 
            // _toolStrip
            // 
            this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this._tbFilter,
            this.toolStripLabel2,
            this._comboFilter,
            this._btAddFB,
            this._btGoIoChannel});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(700, 25);
            this._toolStrip.TabIndex = 11;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(32, 22);
            this.toolStripLabel1.Text = "Find";
            // 
            // _tbFilter
            // 
            this._tbFilter.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this._tbFilter.Name = "_tbFilter";
            this._tbFilter.Size = new System.Drawing.Size(200, 25);
            this._tbFilter.TextChanged += new System.EventHandler(this._tbFilter_TextChanged);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabel2.Text = "Filter";
            // 
            // _comboFilter
            // 
            this._comboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._comboFilter.Name = "_comboFilter";
            this._comboFilter.Size = new System.Drawing.Size(260, 25);
            this._comboFilter.SelectedIndexChanged += new System.EventHandler(this._comboFilter_SelectedIndexChanged);
            // 
            // _btAddFB
            // 
            this._btAddFB.Enabled = false;
            this._btAddFB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btAddFB.Name = "_btAddFB";
            this._btAddFB.Size = new System.Drawing.Size(152, 21);
            this._btAddFB.Text = "Add FB for IO Channel...";
            this._btAddFB.Click += new System.EventHandler(this._btAddFB_Click);
            // 
            // _btGoIoChannel
            // 
            this._btGoIoChannel.Enabled = false;
            this._btGoIoChannel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btGoIoChannel.Name = "_btGoIoChannel";
            this._btGoIoChannel.Size = new System.Drawing.Size(97, 21);
            this._btGoIoChannel.Text = "Go to Instance";
            this._btGoIoChannel.Click += new System.EventHandler(this._btGoIoChannel_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(283, 267);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Always update variables";
            // 
            // _comboUpdateVariables
            // 
            this._comboUpdateVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._comboUpdateVariables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._comboUpdateVariables.DropDownWidth = 280;
            this._comboUpdateVariables.FormattingEnabled = true;
            this._comboUpdateVariables.Items.AddRange(new object[] {
            "Use parent device setting",
            "Enabled 1 (use bus cycle task if not used in any task)",
            "Enabled 2 (always in bus cycle task)"});
            this._comboUpdateVariables.Location = new System.Drawing.Point(417, 264);
            this._comboUpdateVariables.Name = "_comboUpdateVariables";
            this._comboUpdateVariables.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._comboUpdateVariables.Size = new System.Drawing.Size(280, 21);
            this._comboUpdateVariables.TabIndex = 2;
            this._comboUpdateVariables.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // _btResetDefault
            // 
            this._btResetDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btResetDefault.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._btResetDefault.Location = new System.Drawing.Point(153, 262);
            this._btResetDefault.Name = "_btResetDefault";
            this._btResetDefault.Size = new System.Drawing.Size(124, 23);
            this._btResetDefault.TabIndex = 9;
            this._btResetDefault.Text = "Reset Mapping";
            this._btResetDefault.UseVisualStyleBackColor = true;
            this._btResetDefault.Click += new System.EventHandler(this._btResetDefault_Click);
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._descriptionTextBox.Location = new System.Drawing.Point(3, 264);
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.ReadOnly = true;
            this._descriptionTextBox.Size = new System.Drawing.Size(144, 21);
            this._descriptionTextBox.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this._panelChannels);
            this.panel2.Controls.Add(this._panelDescription);
            this.panel2.Controls.Add(this._panelWarning);
            this.panel2.Controls.Add(this._panelBusCycle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(700, 425);
            this.panel2.TabIndex = 8;
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            // 
            // IoMappingEditorPage
            // 
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(600, 300);
            this.AutoSize = true;
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Name = "IoMappingEditorPage";
            this.Size = new System.Drawing.Size(700, 425);
            this._panelDescription.ResumeLayout(false);
            this._panelDescription.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox2)).EndInit();
            this._panelBusCycle.ResumeLayout(false);
            this._grpBoxTask.ResumeLayout(false);
            this._grpBoxTask.PerformLayout();
            this._panelWarning.ResumeLayout(false);
            this._panelWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbWarning)).EndInit();
            this._panelChannels.ResumeLayout(false);
            this._panelChannels.PerformLayout();
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private void OnMappingTreeBeforeEdit(object sender, TreeTableViewEditEventArgs e)
		{
			if (bGetParameterInProgress)
			{
				((CancelEventArgs)(object)e).Cancel = true;
				return;
			}
			try
			{
				bGetParameterInProgress = true;
				int indexOfColumn = Model.GetIndexOfColumn(7);
				if (e.ColumnIndex == indexOfColumn)
				{
					ParameterTreeTableModel.ChangeForcedValue(e);
					return;
				}
				indexOfColumn = Model.GetIndexOfColumn(6);
				if (e.ColumnIndex != indexOfColumn && _editor.GetParameterSetProvider().GetParameterSet(bToModify: true) == null)
				{
					((CancelEventArgs)(object)e).Cancel = true;
				}
			}
			finally
			{
				bGetParameterInProgress = false;
			}
		}

		private void OnMappingTreeSelectionChanged(object sender, EventArgs e)
		{
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			if (selectedNodes != null && selectedNodes.Count > 0)
			{
				bool enabled = true;
				string text = string.Empty;
				ChannelType channelType = (ChannelType)0;
				foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
				{
					if (item.Value is DataElementNode)
					{
						if (channelType == ChannelType.None)
						{
							channelType = (item.Value as DataElementNode).Parameter.ChannelType;
						}
						else if (channelType != (item.Value as DataElementNode).Parameter.ChannelType)
						{
							enabled = false;
							break;
						}
						if ((item.Value as DataElementNode).DataElement is IDataElement2 dataElement && dataElement.HasBaseType)
						{
							if (string.IsNullOrEmpty(text))
							{
								text = dataElement.BaseType;
							}
							else if (string.Compare(text, dataElement.BaseType, StringComparison.InvariantCultureIgnoreCase) != 0)
							{
								enabled = false;
								break;
							}
							continue;
						}
						enabled = false;
						break;
					}
					enabled = false;
					break;
				}
				_btAddFB.Enabled = enabled;
			}
			else
			{
				_btAddFB.Enabled = false;
			}
			if (((TreeTableView)_mappingTreeTableView).FocusedNode != null)
			{
				DataElementNode dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(((TreeTableView)_mappingTreeTableView).FocusedNode) as DataElementNode;
				SectionNode sectionNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(((TreeTableView)_mappingTreeTableView).FocusedNode) as SectionNode;
				_btGoIoChannel.Enabled = MappedFBInstance != null;
				if (dataElementNode != null && dataElementNode.DataElement != null)
				{
					_descriptionTextBox.Text = dataElementNode.DataElement.Description;
					return;
				}
				if (sectionNode != null)
				{
					_descriptionTextBox.Text = sectionNode.Section.Description;
					return;
				}
			}
			else
			{
				_btGoIoChannel.Enabled = false;
			}
			_descriptionTextBox.Text = string.Empty;
		}

		private void OnMappingTreeScroll(object sender, EventArgs e)
		{
			EnableMonitoringRange();
		}

		private void OnMappingTreeSizeChanged(object sender, EventArgs e)
		{
			EnableMonitoringRange();
		}

		private void ExpandCollapse()
		{
			if (_collapseTimer == null)
			{
				_collapseTimer = new Timer();
				_collapseTimer.Interval = 100;
				_collapseTimer.Start();
				_collapseTimer.Tick += _collapseTimer_Tick;
			}
			else
			{
				_collapseTimer.Stop();
				_collapseTimer.Start();
			}
		}

		private void _collapseTimer_Tick(object sender, EventArgs e)
		{
			_collapseTimer.Stop();
			bool doNotShrinkColumnsAutomatically = ((TreeTableView)_mappingTreeTableView).DoNotShrinkColumnsAutomatically;
			try
			{
				((TreeTableView)_mappingTreeTableView).DoNotShrinkColumnsAutomatically=(true);
				AdjustColumnWidths(bForceAdjust: true);
			}
			finally
			{
				((TreeTableView)_mappingTreeTableView).DoNotShrinkColumnsAutomatically=(doNotShrinkColumnsAutomatically);
			}
			EnableMonitoringRange();
			_editor.ChannelsModel.View = (TreeTableView)(object)_mappingTreeTableView;
			_editor.ChannelsModel.StoreExpandedNodes();
		}

		private void OnMappingTreeAfterCollapse(object sender, TreeTableViewEventArgs e)
		{
			ExpandCollapse();
		}

		private void OnMappingTreeAfterExpand(object sender, TreeTableViewEventArgs e)
		{
			ExpandCollapse();
		}

		private bool HasMapping(ref bool bHasDefaultMapping)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			try
			{
				if (_editor != null)
				{
					if (_editor.ChannelsModel != null)
					{
						IParameterSetProvider parameterSetProvider = _editor.GetParameterSetProvider();
						if (parameterSetProvider != null)
						{
							IParameterSet parameterSet = parameterSetProvider.GetParameterSet(bToModify: false);
							if (parameterSet != null)
							{
								LList<IDataElement> val = new LList<IDataElement>();
								foreach (IParameter item in (IEnumerable)parameterSet)
								{
									IParameter val2 = item;
									if ((int)val2.ChannelType != 0)
									{
										val.Add((IDataElement)(object)val2);
									}
								}
								if (_editor.ChannelsModel.CheckMapping((ICollection)val, ref bHasDefaultMapping))
								{
									result = true;
									return result;
								}
								return result;
							}
							return result;
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		private void _mappingTreeTableView_AfterEditAccepted(object sender, TreeTableViewEditEventArgs e)
		{
			iLastColumnIndex = e.ColumnIndex;
			if (e.ColumnIndex != 3)
			{
				return;
			}
			if (_editor is ConnectorIoMappingEditor)
			{
				IConnectorEditorFrame frame = _editor.GetFrame();
				if (frame != null)
				{
					IDeviceObject associatedDeviceObject = frame.GetAssociatedDeviceObject(false);
					if (associatedDeviceObject != null)
					{
						((ILanguageModelManager21)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(((IObject)associatedDeviceObject).MetaObject.ProjectHandle, ((IObject)associatedDeviceObject).MetaObject.ObjectGuid);
					}
					((IEditorView)frame).Editor.Save(true);
				}
			}
			if (!(_editor is DeviceIoMappingEditor))
			{
				return;
			}
			DeviceIoMappingEditor deviceIoMappingEditor = _editor as DeviceIoMappingEditor;
			if (deviceIoMappingEditor != null && deviceIoMappingEditor.DeviceEditorFrame != null)
			{
				IDeviceObject deviceObject = deviceIoMappingEditor.DeviceEditorFrame.GetDeviceObject(false);
				if (deviceObject != null)
				{
					((ILanguageModelManager21)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(((IObject)deviceObject).MetaObject.ProjectHandle, ((IObject)deviceObject).MetaObject.ObjectGuid);
				}
				((IEditorView)deviceIoMappingEditor.DeviceEditorFrame).Editor.Save(true);
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			if (_bInit)
			{
				return;
			}
			if (_editor is ConnectorIoMappingEditor)
			{
				ConnectorIoMappingEditor connectorIoMappingEditor = (ConnectorIoMappingEditor)_editor;
				IConnector connector = connectorIoMappingEditor.ConnectorEditorFrame.GetConnector(connectorIoMappingEditor.ConnectorId, true);
				if (connector == null)
				{
					try
					{
						_bInit = true;
						LoadAlwaysMapping(connectorIoMappingEditor.ConnectorEditorFrame.GetConnector(connectorIoMappingEditor.ConnectorId, false));
					}
					finally
					{
						_bInit = false;
					}
				}
				if (connector != null && typeof(IConnector6).IsAssignableFrom(((object)connector).GetType()))
				{
					IConnector6 val = (IConnector6)connector;
					if (val != null)
					{
						SaveAlwaysMapping(val);
						IMetaObject metaObject = ((IIoProvider)((connector is IIoProvider) ? connector : null)).GetMetaObject();
						if (metaObject != null)
						{
							((ILanguageModelManager21)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(metaObject.ProjectHandle, metaObject.ObjectGuid);
						}
					}
				}
			}
			if (!(_editor is DeviceIoMappingEditor))
			{
				return;
			}
			DeviceIoMappingEditor deviceIoMappingEditor = _editor as DeviceIoMappingEditor;
			if (deviceIoMappingEditor == null)
			{
				return;
			}
			IDeviceObject deviceObject = deviceIoMappingEditor.DeviceEditorFrame.GetDeviceObject(true);
			if (deviceObject == null)
			{
				try
				{
					_bInit = true;
					IDeviceObject deviceObject2 = deviceIoMappingEditor.DeviceEditorFrame.GetDeviceObject(false);
					LoadAlwaysMapping(deviceObject2.DeviceParameterSet);
				}
				finally
				{
					_bInit = false;
				}
			}
			if (deviceObject != null)
			{
				SaveAlwaysMapping(deviceObject.DeviceParameterSet);
				IMetaObject metaObject2 = ((IObject)deviceObject).MetaObject;
				if (metaObject2 != null)
				{
					((ILanguageModelManager21)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(metaObject2.ProjectHandle, metaObject2.ObjectGuid);
				}
			}
		}

		private bool ParameterSet4_GetAlwaysMapping(IParameterSet parameterSet)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (parameterSet is IParameterSet4)
			{
				return ((IParameterSet4)parameterSet).AlwaysMapping;
			}
			if (parameterSet is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)parameterSet).IsFunctionAvailable("GetAlwaysMapping"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				return XmlConvert.ToBoolean(((IGenericInterfaceExtensionProvider)parameterSet).CallFunction("GetAlwaysMapping", xmlDocument).DocumentElement.InnerText);
			}
			return false;
		}

		private void ParameterSet4_SetAlwaysMapping(IParameterSet parameterSet, bool value)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (parameterSet is IParameterSet4)
			{
				((IParameterSet4)parameterSet).AlwaysMapping=(value);
			}
			else if (parameterSet is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)parameterSet).IsFunctionAvailable("SetAlwaysMapping"))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
				xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("value"));
				xmlDocument.DocumentElement["value"].InnerText = XmlConvert.ToString(value);
				((IGenericInterfaceExtensionProvider)parameterSet).CallFunction("SetAlwaysMapping", xmlDocument);
			}
		}

		public void SetReload()
		{
			_bReload = true;
		}

		private void UpdateView()
		{
			if (_bReload)
			{
				_bReload = false;
				Timer.Tick -= Timer_Tick;
				Timer.Tick += Timer_Tick;
				if (_editor is ConnectorIoMappingEditor)
				{
					(_editor as ConnectorIoMappingEditor).Reload();
				}
				if (_editor is DeviceIoMappingEditor)
				{
					(_editor as DeviceIoMappingEditor).Reload();
				}
				AdjustColumnWidths(bForceAdjust: false);
				OnOnlineStateChanged(this, new EventArgs());
			}
			else
			{
				try
				{
					((TreeTableView)_mappingTreeTableView).BeginUpdate();
					if (_editor is ConnectorIoMappingEditor)
					{
						(_editor as ConnectorIoMappingEditor).Paint();
					}
					if (_editor is DeviceIoMappingEditor)
					{
						(_editor as DeviceIoMappingEditor).Paint();
					}
				}
				catch
				{
				}
				finally
				{
					((TreeTableView)_mappingTreeTableView).EndUpdate();
				}
			}
			if (_editor?.InstancesModel != null)
			{
				_editor.InstancesModel.Refresh();
			}
		}

		public void GetSelection(out long lPosition, out short sOffset, out int nLength)
		{
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			lPosition = -1L;
			sOffset = -1;
			nLength = 0;
			if (((TreeTableView)_mappingTreeTableView).FocusedNode == null)
			{
				return;
			}
			TreeTableViewNode focusedNode = ((TreeTableView)_mappingTreeTableView).FocusedNode;
			DataElementNode dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(((TreeTableView)_mappingTreeTableView).FocusedNode) as DataElementNode;
			if (focusedNode == null || dataElementNode == null)
			{
				return;
			}
			IDataElement dataElement = dataElementNode.DataElement;
			IDataElement2 val = (IDataElement2)(object)((dataElement is IDataElement2) ? dataElement : null);
			if (val == null)
			{
				return;
			}
			int num = -1;
			lPosition = val.EditorPositionId;
			if (!focusedNode.IsFocused(out num))
			{
				return;
			}
			if (focusedNode.IsEditing(num) && focusedNode.View != null && ((ContainerControl)(object)focusedNode.View).ActiveControl is ISingleLineIECTextEditor)
			{
				Control control = ((ContainerControl)(object)focusedNode.View).ActiveControl;
				ISingleLineIECTextEditor val2 = (ISingleLineIECTextEditor)(object)((control is ISingleLineIECTextEditor) ? control : null);
				if (val2 != null)
				{
					sOffset = (short)val2.SelectionStart;
					nLength = val2.SelectionLength;
					return;
				}
			}
			switch (num)
			{
			case 0:
				if (((IDataElement)val).IoMapping == null || ((IDataElement)val).IoMapping.VariableMappings == null)
				{
					break;
				}
				{
					IEnumerator enumerator = ((IEnumerable)((IDataElement)val).IoMapping.VariableMappings).GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							IVariableMapping val3 = (IVariableMapping)enumerator.Current;
							if (val3.CreateVariable)
							{
								sOffset = 0;
							}
							else
							{
								sOffset = (short)(val3.Variable.IndexOf('.') + 1);
							}
							nLength = val3.Variable.Length - sOffset;
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				break;
			case 3:
				sOffset = 0;
				nLength = ((IDataElement)val).IoMapping.IecAddress.Length;
				lPosition |= 2147483648L;
				break;
			}
		}

		internal static LList<Guid> GetTasks(IMetaObject metaHost)
		{
			LList<Guid> val = new LList<Guid>();
			if (metaHost != null)
			{
				LList<Guid> obj = new LList<Guid>();
				DeviceHelper.CollectObjectGuids(obj, metaHost.ProjectHandle, metaHost.SubObjectGuids, typeof(IDeviceApplication), bRecursive: false, bWithHidden: false);
				if (obj.Count == 1)
				{
					DeviceHelper.CollectObjectGuids(val, metaHost.ProjectHandle, metaHost.SubObjectGuids, typeof(ITaskObject), bRecursive: true, bWithHidden: false);
				}
				else if (metaHost.Object is IDeviceObject2)
				{
					IObject @object = metaHost.Object;
					IDriverInfo driverInfo = ((IDeviceObject2)((@object is IDeviceObject2) ? @object : null)).DriverInfo;
					Guid ioApplication = ((IDriverInfo2)((driverInfo is IDriverInfo2) ? driverInfo : null)).IoApplication;
					if (ioApplication != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaHost.ProjectHandle, ioApplication))
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaHost.ProjectHandle, ioApplication);
						LList<Guid> val2 = new LList<Guid>();
						DeviceHelper.CollectObjectGuids(val2, metaHost.ProjectHandle, metaObjectStub.SubObjectGuids, typeof(ITaskConfigObject), bRecursive: false, bWithHidden: false);
						if (val2.Count > 0)
						{
							foreach (Guid item in val2)
							{
								IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaHost.ProjectHandle, item);
								DeviceHelper.CollectObjectGuids(val, metaHost.ProjectHandle, metaObjectStub2.SubObjectGuids, typeof(ITaskObject), bRecursive: false, bWithHidden: false);
							}
							return val;
						}
					}
				}
			}
			return val;
		}

		private void FillBusCycleList(IDeviceObject plcdevice, IDriverInfo5 driverinfo)
		{
			LList<Guid> tasks = GetTasks(((IObject)plcdevice).MetaObject);
			LList<BusTaskItem> val = new LList<BusTaskItem>();
			BusTaskItem busTaskItem = null;
			foreach (Guid item in tasks)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)plcdevice).MetaObject.ProjectHandle, item);
				if (typeof(ITaskObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					BusTaskItem busTaskItem2 = new BusTaskItem(metaObjectStub.ObjectGuid, metaObjectStub.Name);
					val.Add(busTaskItem2);
					if (driverinfo != null && (driverinfo.BusCycleTaskGuid == metaObjectStub.ObjectGuid || ((IDriverInfo)driverinfo).BusCycleTask == metaObjectStub.Name))
					{
						busTaskItem = busTaskItem2;
					}
				}
			}
			val.Sort();
			BusTaskItem busTaskItem3 = new BusTaskItem(Guid.Empty, Strings.BusCycleTaskFromParent);
			val.Insert(0, busTaskItem3);
			if (busTaskItem == null)
			{
				busTaskItem = busTaskItem3;
			}
			_cbBusCycle.Items.Clear();
			_cbBusCycle.Items.AddRange(val.ToArray());
			_cbBusCycle.SelectedItem = busTaskItem;
		}

		private void _cbBusCycle_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_bInit && _plcDevice != null && _driverInfo != null)
			{
				Guid objectGuid = ((BusTaskItem)_cbBusCycle.SelectedItem).ObjectGuid;
				if (_driverInfo.BusCycleTaskGuid != objectGuid || (Guid.Empty == objectGuid && !string.IsNullOrEmpty(((IDriverInfo)_driverInfo).BusCycleTask)))
				{
					SaveSettings();
				}
			}
		}

		private void SaveSettings()
		{
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Expected O, but got Unknown
			bool bInit = _bInit;
			try
			{
				_bInit = true;
				IDriverInfo5 val = null;
				if (_editor is ConnectorIoMappingEditor)
				{
					ConnectorIoMappingEditor connectorIoMappingEditor = (ConnectorIoMappingEditor)_editor;
					if (connectorIoMappingEditor != null)
					{
						IDriverInfo driverInfo = connectorIoMappingEditor.GetDriverInfo(bToModify: true);
						val = (IDriverInfo5)(object)((driverInfo is IDriverInfo5) ? driverInfo : null);
					}
				}
				if (val != null)
				{
					val.BusCycleTaskGuid=(((BusTaskItem)_cbBusCycle.SelectedItem).ObjectGuid);
					((IDriverInfo)val).BusCycleTask=(((BusTaskItem)_cbBusCycle.SelectedItem).ToString());
					if (val.BusCycleTaskGuid != Guid.Empty)
					{
						((IDriverInfo)val).BusCycleTask=(((BusTaskItem)_cbBusCycle.SelectedItem).ToString());
					}
					else
					{
						((IDriverInfo)val).BusCycleTask=(string.Empty);
					}
					if (_editor is ConnectorIoMappingEditor)
					{
						ConnectorIoMappingEditor connectorIoMappingEditor2 = (ConnectorIoMappingEditor)_editor;
						if (connectorIoMappingEditor2 != null)
						{
							((IEditorView)connectorIoMappingEditor2.GetFrame()).Editor.Save(true);
							ref IDriverInfo5 driverInfo2 = ref _driverInfo;
							IDriverInfo driverInfo3 = connectorIoMappingEditor2.GetDriverInfo(bToModify: false);
							driverInfo2 = (IDriverInfo5)(object)((driverInfo3 is IDriverInfo5) ? driverInfo3 : null);
							_plcDevice = _editor.GetHost();
						}
					}
					int projectHandle = ((IObject)_plcDevice).MetaObject.ProjectHandle;
					IMetaObject metaObject = ((IObject)_plcDevice).MetaObject;
					LList<Guid> obj = new LList<Guid>();
					DeviceHelper.CollectObjectGuids(obj, metaObject.ProjectHandle, metaObject.SubObjectGuids, typeof(IApplicationObject), bRecursive: true, bWithHidden: false);
					foreach (Guid item in obj)
					{
						((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ClearDownloadContext(item);
					}
					LList<Guid> obj2 = new LList<Guid>();
					DeviceHelper.CollectObjectGuids(obj2, metaObject.ProjectHandle, metaObject.SubObjectGuids, typeof(ITaskConfigObject), bRecursive: true, bWithHidden: false);
					foreach (Guid item2 in obj2)
					{
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, item2);
						if (objectToRead != null && objectToRead.Object is ITaskConfigObject)
						{
							ITaskConfigObject val2 = (ITaskConfigObject)objectToRead.Object;
							((ILanguageModelManager21)APEnvironment.LanguageModelMgr).PutLanguageModel((ILanguageModelProvider)(object)val2, true);
						}
					}
				}
				else if (_bCycleTaskEnable)
				{
					FillBusCycleList(_plcDevice, _driverInfo);
				}
			}
			finally
			{
				_bInit = bInit;
			}
		}

		public void RefillBusCycleList()
		{
			if (_bInit)
			{
				return;
			}
			try
			{
				_bInit = true;
				if (_bCycleTaskEnable && _plcDevice != null && ((IObject)_plcDevice).MetaObject != null)
				{
					FillBusCycleList(_plcDevice, _driverInfo);
				}
			}
			finally
			{
				_bInit = false;
			}
		}

		private void ResetSubNodes(ITreeTableNode node, bool bEmptyMapping)
		{
			if (node == null)
			{
				return;
			}
			if (node is DataElementNode)
			{
				SetMappingToDefault(node as DataElementNode, bEmptyMapping);
			}
			if (!(node is SectionNode))
			{
				return;
			}
			foreach (IParameterTreeNode childNode in (node as SectionNode).ChildNodes)
			{
				ResetSubNodes((ITreeTableNode)childNode, bEmptyMapping);
			}
		}

		private void _btResetDefault_Click(object sender, EventArgs e)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Invalid comparison between Unknown and I4
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			bool bHasDefaultMapping = false;
			if (!HasMapping(ref bHasDefaultMapping))
			{
				return;
			}
			bool flag = false;
			bool bEmptyMapping = false;
			if (bHasDefaultMapping)
			{
				string[] array = new string[2]
				{
					Strings.ChoiceDefaultVariable,
					Strings.ChoiceEmptyVariable
				};
				int num = APEnvironment.MessageService.MultipleChoicePromptWithDetails(Strings.PromptClearMapping, array, 0, true, (EventHandler)null, (EventArgs)null, "PromptDeleteMappings", Array.Empty<object>());
				if (num >= 0)
				{
					flag = true;
					if (num == 1)
					{
						bEmptyMapping = true;
					}
				}
			}
			else if ((int)((IEngine)APEnvironment.Engine).MessageService.Prompt(Strings.PromptClearMapping, (PromptChoice)2, (PromptResult)2) == 2)
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = false;
			try
			{
				UndoMgr.BeginCompoundAction("delete mappings");
				if (_editor.GetParameterSetProvider().GetParameterSet(bToModify: true) != null)
				{
					foreach (TreeTableViewNode node in ((TreeTableView)_mappingTreeTableView).Nodes)
					{
						TreeTableViewNode val = node;
						ResetSubNodes(((TreeTableView)_mappingTreeTableView).GetModelNode(val), bEmptyMapping);
					}
				}
				flag2 = true;
			}
			finally
			{
				UndoMgr.EndCompoundAction();
				if (!flag2)
				{
					try
					{
						UndoMgr.Undo();
					}
					catch
					{
					}
				}
			}
		}

		internal void SetMappingToDefault(DataElementNode node, bool bEmptyMapping)
		{
			if (node.HasChildren)
			{
				for (int i = 0; i < node.ChildCount; i++)
				{
					DataElementNode dataElementNode = node.GetChild(i) as DataElementNode;
					if (dataElementNode != null)
					{
						SetMappingToDefault(dataElementNode, bEmptyMapping);
					}
				}
			}
			if (node.DataElement == null || node.DataElement.IoMapping == null)
			{
				return;
			}
			try
			{
				if (((ICollection)node.DataElement.IoMapping.VariableMappings).Count > 0)
				{
					MappingAction mappingAction = new MappingAction(node, string.Empty, bEmptyMapping);
					UndoMgr.AddAction((IUndoableAction)(object)mappingAction);
					mappingAction.Redo();
				}
			}
			catch
			{
			}
		}

		public IPrintPainterEx CreatePrintPainter(long nPosition, int nLength)
		{
			return (IPrintPainterEx)(object)new DeviceParameterPrintPainter(_editor, null);
		}

		internal bool TrySelect(long lPosition, int nOffset, int nLength)
		{
			if ((_editor.MappingPage.CycleTaskEnable || (_editor.InstancesModel != null && ((DefaultTreeTableModel)_editor.InstancesModel).Sentinel.HasChildren) || ((AbstractTreeTableModel)_editor.ChannelsModel).Sentinel.HasChildren) && IOMappingEditor.EnableLogicalDevices)
			{
				IDeviceObject deviceObject = _editor.GetDeviceObject(bToModify: false);
				if (deviceObject != null && deviceObject is ILogicalDevice && ((ILogicalDevice)((deviceObject is ILogicalDevice) ? deviceObject : null)).LanguageModelPositionId == lPosition)
				{
					((Control)(object)_mappingTreeTableView)?.Focus();
					return true;
				}
			}
			return false;
		}

		internal LDictionary<TreeTableViewNode, ITreeTableNode2> GetSelectedNodes()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			LDictionary<TreeTableViewNode, ITreeTableNode2> val = new LDictionary<TreeTableViewNode, ITreeTableNode2>();
			foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)((TreeTableView)_mappingTreeTableView).SelectedNodes)
			{
				TreeTableViewNode val2 = item;
				ITreeTableNode modelNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(val2);
				ITreeTableNode2 val3 = (ITreeTableNode2)(object)((modelNode is ITreeTableNode2) ? modelNode : null);
				if (val3 != null)
				{
					val.Add(val2, val3);
				}
			}
			return val;
		}

		private void Cut()
		{
			Copy();
			Delete();
		}

		private void Copy()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			string text = string.Empty;
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			bool flag = true;
			int nModelColumnIndex = -1;
			((TreeTableView)_mappingTreeTableView).FocusedNode.IsFocused(out nModelColumnIndex);
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (nModelColumnIndex == Model.GetIndexOfColumn(0))
				{
					IconLabelTreeTableViewCellData iconLabelTreeTableViewCellData = item.Key.CellValues[nModelColumnIndex] as IconLabelTreeTableViewCellData;
					if (!string.IsNullOrEmpty(iconLabelTreeTableViewCellData.Label.ToString()))
					{
						if (!flag)
						{
							text += "\n";
						}
						text += iconLabelTreeTableViewCellData.Label.ToString();
						flag = false;
					}
				}
				if (nModelColumnIndex == Model.GetIndexOfColumn(6) || nModelColumnIndex == Model.GetIndexOfColumn(7))
				{
					string text2 = item.Key.CellValues[nModelColumnIndex].ToString();
					if (!string.IsNullOrEmpty(text2))
					{
						if (!flag)
						{
							text += "\n";
						}
						text += text2;
						flag = false;
					}
				}
				if (nModelColumnIndex != Model.GetIndexOfColumn(10))
				{
					continue;
				}
				string text3 = item.Key.CellValues[nModelColumnIndex] as string;
				if (!string.IsNullOrEmpty(text3))
				{
					if (!flag)
					{
						text += "\n";
					}
					text += text3;
					flag = false;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				Clipboard.SetText(text);
			}
		}

		private void Paste()
		{
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> selectedNode in GetSelectedNodes())
			{
				if (selectedNode.Value is DataElementNode node && !DataElementNode.CheckForMultipleMapping(node, bNoMessage: false))
				{
					return;
				}
			}
			string text = Clipboard.GetText();
			if (text != string.Empty)
			{
				SetVariable(text);
			}
		}

		private void Delete()
		{
			SetVariable(string.Empty);
		}

		private void SetVariable(string stText)
		{
			string[] array = stText.Split('\n');
			int num = 0;
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			int nModelColumnIndex = -1;
			using (LDictionary<TreeTableViewNode, ITreeTableNode2>.Enumerator enumerator = selectedNodes.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.Key.IsFocused(out nModelColumnIndex))
				{
				}
			}
			if (nModelColumnIndex == Model.GetIndexOfColumn(7))
			{
				foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
				{
					if (item.Value is DataElementNode dataElementNode)
					{
						if (array.Length > num)
						{
							dataElementNode.SetValue(nModelColumnIndex, array[num]);
						}
						else
						{
							dataElementNode.SetValue(nModelColumnIndex, string.Empty);
						}
						num++;
					}
				}
			}
			if (nModelColumnIndex == -1 || !(Model.OnlineApplication == Guid.Empty))
			{
				return;
			}
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item2 in selectedNodes)
			{
				if (item2.Value is DataElementNode dataElementNode2 && dataElementNode2.PlcNode.OnlineApplication == Guid.Empty && dataElementNode2.ParameterSetProvider.GetParameterSet(bToModify: true) != null)
				{
					if (array.Length > num)
					{
						dataElementNode2.SetValue(nModelColumnIndex, array[num]);
					}
					else
					{
						dataElementNode2.SetValue(nModelColumnIndex, string.Empty);
					}
					num++;
				}
			}
		}

		private void Undo()
		{
			if (CanUndo)
			{
				UndoMgr.Undo();
			}
		}

		private void Redo()
		{
			if (CanRedo)
			{
				UndoMgr.Redo();
			}
		}

		public void Mark(long nPosition, int nLength, object tag)
		{
		}

		public void UnmarkAll(object tag)
		{
		}

		public void Select(long nPosition, int nLength)
		{
		}

		private bool MenuEnabled(bool bEditable)
		{
			bool result = false;
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			if (selectedNodes.Count > 0)
			{
				result = true;
			}
			int nModelColumnIndex = -1;
			if (_mappingTreeTableView.FocusedNode != null)
			{
				_mappingTreeTableView.FocusedNode.IsFocused(out nModelColumnIndex);
			}
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (nModelColumnIndex != Model.GetIndexOfColumn(10))
				{
					if (nModelColumnIndex == Model.GetIndexOfColumn(6))
					{
						if (bEditable || !(item.Value is DataElementNode) || !((item.Value as DataElementNode).DataElement as IDataElement2).HasBaseType)
						{
							result = false;
						}
					}
					else if (nModelColumnIndex == Model.GetIndexOfColumn(7))
					{
						if (!(item.Value is DataElementNode) || !((item.Value as DataElementNode).DataElement as IDataElement2).HasBaseType)
						{
							result = false;
						}
					}
					else if (!(item.Value is DataElementNode) || (bEditable && !(item.Value as DataElementNode).VariableEditable()))
					{
						result = false;
					}
				}
				if (nModelColumnIndex != Model.GetIndexOfColumn(0) && nModelColumnIndex != Model.GetIndexOfColumn(10) && nModelColumnIndex != Model.GetIndexOfColumn(6) && nModelColumnIndex != Model.GetIndexOfColumn(7))
				{
					result = false;
				}
			}
			return result;
		}

		public bool CanExecuteStandardCommand(Guid commandGuid)
		{
			if (((TreeTableView)_mappingTreeTableView).FocusedNode != null)
			{
				TreeTableViewNode focusedNode = ((TreeTableView)_mappingTreeTableView).FocusedNode;
				if (((TreeTableView)_mappingTreeTableView).GetModelNode(focusedNode) is DataElementNode)
				{
					ICommandManager commandManager = ((IEngine)APEnvironment.Engine).CommandManager;
					ICommandManager3 val = (ICommandManager3)(object)((commandManager is ICommandManager3) ? commandManager : null);
					if (val != null && ((ICommandManager)val).GetCommand(commandGuid) != null && val.GetCommandCategory(commandGuid) == additionalContextMenuGuid)
					{
						return true;
					}
				}
			}
			if (commandGuid == GUID_EDITCUT || commandGuid == GUID_EDITPASTE || commandGuid == GUID_EDITDELETE)
			{
				return MenuEnabled(bEditable: true);
			}
			if (commandGuid == GUID_EDITCOPY)
			{
				return MenuEnabled(bEditable: false);
			}
			if (commandGuid == GUID_EDITUNDO)
			{
				return CanUndo;
			}
			if (commandGuid == GUID_EDITREDO)
			{
				return CanRedo;
			}
			if (commandGuid == GUID_GOTODEFINITION || commandGuid == GUID_CROSSREFERENCES)
			{
				return true;
			}
			if (commandGuid == GUID_EDITSELECTALL)
			{
				return true;
			}
			if (commandGuid == GUID_REFACTORING)
			{
				LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
				if (selectedNodes.Count == 1)
				{
					foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
					{
						if (!(item.Value is DataElementNode dataElementNode) || dataElementNode.DataElement == null || dataElementNode.DataElement.IoMapping.VariableMappings.Count <= 0)
						{
							continue;
						}
						foreach (IVariableMapping variableMapping in dataElementNode.DataElement.IoMapping.VariableMappings)
						{
							if (variableMapping.CreateVariable && !string.IsNullOrEmpty(variableMapping.Variable) && dataElementNode.GetRefactoringContext(out var _, out var _) == RefactoringContextType.Variable)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public void ExecuteStandardCommand(Guid commandGuid)
		{
			if (commandGuid == GUID_EDITCUT)
			{
				Cut();
			}
			else if (commandGuid == GUID_EDITCOPY)
			{
				Copy();
			}
			else if (commandGuid == GUID_EDITPASTE)
			{
				Paste();
			}
			else if (commandGuid == GUID_EDITDELETE)
			{
				Delete();
			}
			else if (commandGuid == GUID_EDITREDO)
			{
				Redo();
			}
			else if (commandGuid == GUID_EDITUNDO)
			{
				Undo();
			}
			else if (commandGuid == GUID_EDITSELECTALL)
			{
				for (TreeTableViewNode val = ((TreeTableView)_mappingTreeTableView).TopNode; val != null; val = val.NextVisibleNode)
				{
					val.Selected=(true);
				}
			}
		}

		private void _mappingTreeTableView_MouseUp(object sender, MouseEventArgs e)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			if (e.Button == MouseButtons.Right && ((TreeTableView)_mappingTreeTableView).FocusedNode != null)
			{
				_=((TreeTableView)_mappingTreeTableView).FocusedNode;
				try
				{
					ContextMenuFilterCallback val = new ContextMenuFilterCallback(CanExecuteStandardCommand);
					((IEngine)APEnvironment.Engine).Frame.DisplayContextMenu(Guid.Empty, (Guid[])null, val, (Control)(object)_mappingTreeTableView, e.Location);
				}
				catch
				{
				}
			}
		}

		private void _mappingTreeTableView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Return || ((TreeTableView)_mappingTreeTableView).FocusedNode == null || iLastColumnIndex < 0)
			{
				return;
			}
			TreeTableViewNode val = ((TreeTableView)_mappingTreeTableView).FocusedNode;
			val.Selected=(false);
			if (val.NextVisibleNode != null)
			{
				val = val.NextVisibleNode;
			}
			else
			{
				while (val.PrevVisibleNode != null)
				{
					val = val.PrevVisibleNode;
				}
			}
			DataElementNode dataElementNode = null;
			do
			{
				dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(val) as DataElementNode;
				if (dataElementNode != null && (iLastColumnIndex != Model.GetIndexOfColumn(0) || DataElementNode.CheckForMultipleMapping(dataElementNode, bNoMessage: true)) && dataElementNode.IsEditable(iLastColumnIndex))
				{
					break;
				}
				val = val.NextVisibleNode;
			}
			while (val != null);
			if (val != null && val != ((TreeTableView)_mappingTreeTableView).FocusedNode)
			{
				val.Selected=(true);
				val.Focus(iLastColumnIndex);
				val.EnsureVisible(iLastColumnIndex);
			}
			iLastColumnIndex = -1;
		}

		public RefactoringContextType GetRefactoringContext(out Guid objectGuid, out string stVariableName)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (((TreeTableView)_mappingTreeTableView).FocusedNode != null)
			{
				DataElementNode dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(((TreeTableView)_mappingTreeTableView).FocusedNode) as DataElementNode;
				if (dataElementNode != null)
				{
					return dataElementNode.GetRefactoringContext(out objectGuid, out stVariableName);
				}
			}
			stVariableName = string.Empty;
			objectGuid = Guid.Empty;
			return (RefactoringContextType)0;
		}

		public bool UndoableReplace(long nPosition, int nLength, string stReplacement)
		{
			if (UndoMgr != null)
			{
				long lPosition = default(long);
				short startIndex = default(short);
				PositionHelper.SplitPosition(nPosition, out lPosition, out startIndex);
				try
				{
					DataElementNode dataElementNode = _editor?.ChannelsModel?.GetNodeByPosition(lPosition);
					if (dataElementNode != null)
					{
						string text = string.Empty;
						IVariableMappingCollection variableMappings = dataElementNode.DataElement.IoMapping.VariableMappings;
						if (((ICollection)variableMappings).Count > 0)
						{
							IVariableMapping obj = variableMappings[0];
							text = ((obj is IVariableMapping2) ? obj : null).Variable;
						}
						string text2 = text.Remove(startIndex, nLength);
						text2 = text2.Insert(startIndex, stReplacement);
						MappingAction mappingAction = new MappingAction(dataElementNode, text2, text);
						UndoMgr.AddAction((IUndoableAction)(object)mappingAction);
						mappingAction.Redo();
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		public bool GetSelectionForBrowserCommands(out string expression, out Guid objectGuid, out IPreCompileContext pcc)
		{
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Expected O, but got Unknown
			expression = null;
			objectGuid = Guid.Empty;
			pcc = null;
			if (((TreeTableView)_mappingTreeTableView).FocusedNode != null)
			{
				try
				{
					_=((TreeTableView)_mappingTreeTableView).FocusedNode;
					DataElementNode dataElementNode = ((TreeTableView)_mappingTreeTableView).GetModelNode(((TreeTableView)_mappingTreeTableView).FocusedNode) as DataElementNode;
					if (dataElementNode != null && dataElementNode.DataElement.IoMapping != null && ((ICollection)dataElementNode.DataElement.IoMapping.VariableMappings).Count > 0)
					{
						expression = dataElementNode.DataElement.IoMapping.VariableMappings[0]
							.Variable;
						objectGuid = Editor.ObjectGuid;
						if (expression.Contains("."))
						{
							int num = expression.IndexOf('.') + 1;
							string text = expression.Substring(0, num - 1).ToLowerInvariant();
							expression = expression.Substring(num);
							int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
							foreach (Guid application in dataElementNode.PlcNode.Applications)
							{
								if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, application) && ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, application).Name.ToLowerInvariant() == text)
								{
									pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(application);
								}
							}
						}
						else
						{
							IDeviceObject host = Editor.GetHost();
							if (host != null)
							{
								IDriverInfo4 val = (IDriverInfo4)((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
								if (val != null)
								{
									pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IDriverInfo2)val).IoApplication);
								}
							}
						}
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		public bool GetSelectionForCrossReferenceCommand(out string expression, out Guid objectGuid)
		{
			expression = string.Empty;
			objectGuid = Guid.Empty;
			return false;
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			if (_bDoRefill && DateTime.Now.Ticks - _delayTicks > 2500000)
			{
				_bDoRefill = false;
				SetVariableFilter(_comboFilter.SelectedItem as FilterItem);
				EnableMonitoringRange();
			}
			ILegacyOnlineManager legacyOnlineMgrOrNull = APEnvironment.LegacyOnlineMgrOrNull;
			bool flag = legacyOnlineMgrOrNull != null;
			if (!(_editor.OnlineState.OnlineApplication != Guid.Empty) && (!flag || _plcDevice == null || !legacyOnlineMgrOrNull.IsOnline(((IObject)_plcDevice).MetaObject.ObjectGuid)))
			{
				bool bHasDefaultMapping = false;
				_btResetDefault.Enabled = HasMapping(ref bHasDefaultMapping);
			}
			_btGoIoChannel.Enabled = MappedFBInstance != null;
		}

		private void _comboFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_editor != null && _editor.ChannelsModel != null && !_bIsHidden)
			{
				SetVariableFilter(_comboFilter.SelectedItem as FilterItem);
				EnableMonitoringRange();
			}
		}

		private void SetVariableFilter(FilterItem filter)
		{
			if (filter == null)
			{
				return;
			}
			for (int i = 0; i < _comboFilter.Items.Count; i++)
			{
				if (_comboFilter.Items[i] as FilterItem == filter)
				{
					_comboFilter.SelectedIndex = i;
				}
			}
			if (_editor.ChannelsModel != null)
			{
				_editor.ChannelsModel.VariableFilter = filter.MappingFilter;
				_editor.ChannelsModel.Refill();
			}
		}

		private void _tbFilter_TextChanged(object sender, EventArgs e)
		{
			bool enabled = string.IsNullOrEmpty(_tbFilter.Text);
			_comboFilter.Enabled = enabled;
			if (!_tbFilter.Text.StartsWith("%") || _tbFilter.Text.Length != 1)
			{
				_editor.ChannelsModel.SearchText = _tbFilter.Text;
				_delayTicks = DateTime.Now.Ticks;
				_bDoRefill = true;
			}
		}

		public void SetLocalizedObject(IMetaObject obj, bool bLocalizationActive)
		{
			_editor.InstancesModel.LocalizationActive = bLocalizationActive;
		}

		public bool IsComment(long nPositionCombined, string stText)
		{
			return false;
		}

		private void _btAddFB_Click(object sender, EventArgs e)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Invalid comparison between Unknown and I4
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			LDictionary<TreeTableViewNode, ITreeTableNode2> selectedNodes = GetSelectedNodes();
			if (selectedNodes == null || selectedNodes.Count <= 0)
			{
				return;
			}
			ChannelType val = (ChannelType)0;
			string text = string.Empty;
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item in selectedNodes)
			{
				if (item.Value is DataElementNode)
				{
					val = (item.Value as DataElementNode).Parameter.ChannelType;
					text = ((item.Value as DataElementNode).DataElement as IDataElement2).BaseType;
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			IDeviceObject host = _editor.GetHost();
			int projectHandle = ((IObject)host).MetaObject.ProjectHandle;
			if (host == null)
			{
				return;
			}
			IDriverInfo driverInfo = ((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
			Guid ioApplication = ((IDriverInfo2)((driverInfo is IDriverInfo4) ? driverInfo : null)).IoApplication;
			if (!(ioApplication != Guid.Empty))
			{
				return;
			}
			FBCreateForm fBCreateForm = new FBCreateForm(projectHandle, ioApplication, text, (int)val != 1);
			if (fBCreateForm.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			FBCreateNode selectedFB = fBCreateForm.SelectedFB;
			foreach (KeyValuePair<TreeTableViewNode, ITreeTableNode2> item2 in selectedNodes)
			{
				if (item2.Value is DataElementNode node)
				{
					MaintenanceFBAction maintenanceFBAction = new MaintenanceFBAction(_editor, node, selectedFB, projectHandle, ioApplication);
					UndoMgr.AddAction(maintenanceFBAction);
					maintenanceFBAction.Redo();
				}
			}
		}

		private void _btGoIoChannel_Click(object sender, EventArgs e)
		{
			IFbInstance5 mappedFBInstance = MappedFBInstance;
			if (mappedFBInstance != null)
			{
				IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(((IObject)_editor.GetDeviceObject(bToModify: false)).MetaObject);
				if (editors != null && editors.Length != 0 && editors[0] is IEditorView)
				{
					IEditor obj = editors[0];
					((IEditorView)((obj is IEditorView) ? obj : null)).Select(mappedFBInstance.LanguageModelPositionId, 0);
				}
			}
		}
	}
}
