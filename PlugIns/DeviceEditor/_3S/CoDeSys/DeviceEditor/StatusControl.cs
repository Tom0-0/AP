using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class StatusControl : UserControl, IVisibleEditor
	{
		private Guid _guidObject;

		private int _nConnectorId;

		private StatusPage _page;

		private IOnlineVarRef _paramAckVarRef;

		private IOnlineDevice2 _onlineDevice;

		private bool _bHideDiagParameter;

		private bool _bHideDiagAckParameter;

		private bool _bDiagParameterAvailable;

		private bool _bDiagAckParameterAvailable;

		private bool _bIsHidden = true;

		private IContainer components;

		private Panel _panelDiag;

		private TreeTableView _messageTree;

		private Label label2;

		private Button _btnAck;

		private Panel panel2;

		private Label _labelName;

		private Label label1;

		private TextBox _tbStatus;

		internal bool DiagPanelHidden => !_panelDiag.Visible;

		internal bool DiagParameterAvailable => _bDiagParameterAvailable;

		internal bool DiagAckParameterAvailable => _bDiagAckParameterAvailable;

		public bool IsHidden
		{
			get
			{
				return _bIsHidden;
			}
			set
			{
				_bIsHidden = value;
				UpdateModuleState();
			}
		}

		public StatusControl()
		{
			InitializeComponent();
		}

		public StatusControl(StatusPage page, IDeviceObject device)
			: this()
		{
			_labelName.Text = device.DeviceInfo.Name;
			_page = page;
			_guidObject = ((IObject)device).MetaObject.ObjectGuid;
			_nConnectorId = -1;
			try
			{
				ref IOnlineDevice2 onlineDevice = ref _onlineDevice;
				IOnlineDevice onlineDevice2 = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(_guidObject);
				onlineDevice = (IOnlineDevice2)(object)((onlineDevice2 is IOnlineDevice2) ? onlineDevice2 : null);
			}
			catch
			{
				_onlineDevice = null;
			}
			Init();
		}

		public StatusControl(StatusPage page, IConnector connector, Guid guidObject, bool bHideDiagParameter, bool bHideDiagAckParameter)
			: this()
		{
			_labelName.Text = connector.VisibleInterfaceName;
			_page = page;
			_guidObject = guidObject;
			_nConnectorId = connector.ConnectorId;
			_bHideDiagAckParameter = bHideDiagAckParameter;
			_bHideDiagParameter = bHideDiagParameter;
			try
			{
				if (connector is IConnector3)
				{
					IConnector3 val = (IConnector3)(object)((connector is IConnector3) ? connector : null);
					if (val != null)
					{
						IDeviceObject host = val.GetHost();
						if (host != null && ((IObject)host).MetaObject != null)
						{
							ref IOnlineDevice2 onlineDevice = ref _onlineDevice;
							IOnlineDevice onlineDevice2 = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)host).MetaObject.ObjectGuid);
							onlineDevice = (IOnlineDevice2)(object)((onlineDevice2 is IOnlineDevice2) ? onlineDevice2 : null);
						}
					}
				}
			}
			catch
			{
				_onlineDevice = null;
			}
			Init();
		}

		public void ReleaseMonitoring()
		{
			if (_paramAckVarRef != null)
			{
				_paramAckVarRef.Release();
				_paramAckVarRef = null;
			}
			((StatusTreeTableModel)(object)_messageTree.Model)?.ReleaseMonitoring();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			base.OnHandleCreated(e);
			APEnvironment.OptionStorage.OptionChanged+=(new OptionEventHandler(OnOptionChanged));
			APEnvironment.DeviceController.ModuleStatusChanged+=(new ModuleStatusEventHandler(OnModuleStatusChanged));
			UpdateModuleState();
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			base.OnHandleDestroyed(e);
			APEnvironment.OptionStorage.OptionChanged-=(new OptionEventHandler(OnOptionChanged));
			APEnvironment.DeviceController.ModuleStatusChanged-=(new ModuleStatusEventHandler(OnModuleStatusChanged));
			UpdateModuleState();
			ReleaseMonitoring();
		}

		private void Init()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			GetDiagnosisParameters(out var msg, out var ack);
			if (_bHideDiagParameter)
			{
				msg = null;
			}
			if (_bHideDiagAckParameter)
			{
				ack = null;
			}
			_bDiagAckParameterAvailable = ack != null;
			_bDiagParameterAvailable = msg != null;
			if (ack != null || msg != null)
			{
				StatusTreeTableModel statusTreeTableModel = new StatusTreeTableModel(_messageTree);
				statusTreeTableModel.Reload(msg, GlobalOptionsHelper.GetCurrentConverterToIEC());
				try
				{
					if (ack != null && ack is IDataElement2 && ((IDataElement2)ack).CanWatch)
					{
						_paramAckVarRef = ((IDataElement2)ack).CreateWatch();
						if (_paramAckVarRef != null)
						{
							_paramAckVarRef.SuspendMonitoring();
						}
					}
					else
					{
						_btnAck.Enabled = false;
					}
				}
				catch
				{
					_paramAckVarRef = null;
				}
				if (msg == null && ack == null)
				{
					_panelDiag.Hide();
				}
				_messageTree.Model=((ITreeTableModel)(object)statusTreeTableModel);
				if (_messageTree.Nodes.Count > 0)
				{
					_messageTree.Nodes[0].Expand();
				}
			}
			else
			{
				_panelDiag.Hide();
			}
		}

		private void OnButtonAckClick(object sender, EventArgs e)
		{
			try
			{
				if (_paramAckVarRef == null || _onlineDevice == null)
				{
					return;
				}
				_paramAckVarRef.PreparedValue=((object)true);
				IOnlineVarRef[] preparedVarRefs = _onlineDevice.PreparedVarRefs;
				foreach (IOnlineVarRef val in preparedVarRefs)
				{
					if (((object)val.Expression).Equals((object)_paramAckVarRef.Expression))
					{
						_onlineDevice.WriteVariables((IOnlineVarRef[])(object)new IOnlineVarRef[1] { val });
						break;
					}
				}
			}
			catch (Exception ex)
			{
				if (!(ex is NoMonitoringValueException))
				{
					((IEngine)APEnvironment.Engine).MessageService.Error("Service failed: " + ex.Message);
				}
			}
		}

		private void AdaptGlobalDisplayMode()
		{
			StatusTreeTableModel statusTreeTableModel = (StatusTreeTableModel)(object)_messageTree.Model;
			if (statusTreeTableModel != null)
			{
				IConverterToIEC currentConverterToIEC = GlobalOptionsHelper.GetCurrentConverterToIEC();
				statusTreeTableModel.SetConverterToIec(currentConverterToIEC);
			}
		}

		private void OnModuleStatusChanged(object sender, IModuleStatusEventArgs e)
		{
			if (e.ObjectGuid == _guidObject && e.ConnectorId == _nConnectorId)
			{
				UpdateModuleState();
			}
		}

		private void OnOptionChanged(object sender, OptionEventArgs e)
		{
			if (e.OptionKey != null && e.OptionKey.Name == GlobalOptionsHelper.SUB_KEY)
			{
				AdaptGlobalDisplayMode();
			}
		}

		private void UpdateModuleState()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Invalid comparison between Unknown and I4
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Invalid comparison between Unknown and I4
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Invalid comparison between Unknown and I4
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Invalid comparison between Unknown and I4
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Invalid comparison between Unknown and I4
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
				{
					return;
				}
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				Color backColor = Color.Salmon;
				StatusTreeTableModel statusTreeTableModel = (StatusTreeTableModel)(object)_messageTree.Model;
				ModuleStatus val = default(ModuleStatus);
				if (APEnvironment.DeviceController.GetModuleStatus(handle, _guidObject, _nConnectorId, out val))
				{
					if (((int)val & 1) == 0)
					{
						_tbStatus.Text = Strings.StatusNotEnabled;
					}
					else if (((int)val & 0x10) == 0)
					{
						_tbStatus.Text = Strings.StatusNoDriver;
					}
					else if (((int)val & 0x20) == 0)
					{
						_tbStatus.Text = Strings.StatusNoModule;
					}
					else if (((int)val & 0x40) == 0)
					{
						_tbStatus.Text = Strings.StatusNoConfig;
					}
					else if (((int)val & 0x80) == 0)
					{
						if (((int)val & 0x800) != 0)
						{
							backColor = SystemColors.Control;
							_tbStatus.Text = Strings.StatusRedundancyPassive;
						}
						else
						{
							_tbStatus.Text = Strings.StatusNotRunning;
						}
					}
					else if (((int)val & 0x100) == 256)
					{
						_tbStatus.Text = Strings.StatusBusError;
					}
					else if (((int)val & 0x200) == 512)
					{
						_tbStatus.Text = Strings.StatusModuleError;
					}
					else if (((int)val & 0x400) == 1024)
					{
						_tbStatus.Text = Strings.StatusDiagnosisAvailable;
					}
					else if (((int)val & 8) == 8)
					{
						_tbStatus.Text = Strings.StatusNoLicense;
					}
					else if (((int)val & 0x800) == 2048)
					{
						backColor = SystemColors.Control;
						_tbStatus.Text = Strings.StatusRedundancyPassive;
					}
					else
					{
						backColor = SystemColors.Control;
						_tbStatus.Text = Strings.StatusRunning;
					}
					if (((int)val & 0x1000) != 0)
					{
						TextBox tbStatus = _tbStatus;
						tbStatus.Text = tbStatus.Text + " | " + Strings.StatusCleared;
					}
					if (statusTreeTableModel != null)
					{
						if (_bIsHidden)
						{
							statusTreeTableModel.HideValueColumn();
							if (_paramAckVarRef != null)
							{
								_paramAckVarRef.SuspendMonitoring();
							}
						}
						else
						{
							statusTreeTableModel.ShowValueColumn();
							if (_paramAckVarRef != null)
							{
								_paramAckVarRef.ResumeMonitoring();
							}
						}
					}
				}
				else
				{
					statusTreeTableModel?.HideValueColumn();
					backColor = SystemColors.Control;
					_tbStatus.Text = "n/a";
				}
				_tbStatus.BackColor = backColor;
				if (_messageTree.Nodes.Count > 0)
				{
					_messageTree.Nodes[0].Expand();
				}
			}
			catch (Exception ex)
			{
				_tbStatus.Text = ex.Message;
			}
		}

		public void GetDiagnosisParameters(out IParameter msg, out IParameter ack)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between Unknown and I4
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			msg = null;
			ack = null;
			IParameterSet parameterSet = GetParameterSet();
			if (parameterSet == null)
			{
				return;
			}
			foreach (IParameter item in (IEnumerable)parameterSet)
			{
				IParameter val = item;
				IParameter2 val2 = (IParameter2)(object)((val is IParameter2) ? val : null);
				if (val2 != null)
				{
					if ((int)val2.DiagType == 1 && msg == null)
					{
						msg = val;
					}
					else if ((int)val2.DiagType == 2 && ack == null)
					{
						ack = val;
					}
				}
			}
		}

		public IParameterSet GetParameterSet()
		{
			if (_page == null || _page.DeviceEditor == null)
			{
				return null;
			}
			if (_nConnectorId == -1)
			{
				IDeviceObject deviceObject = _page.DeviceEditor.GetDeviceObject(bToModify: false);
				if (deviceObject != null)
				{
					return deviceObject.DeviceParameterSet;
				}
				return null;
			}
			IConnector connector = ((IConnectorEditorFrame)_page.DeviceEditor).GetConnector(_nConnectorId, false);
			if (connector != null)
			{
				return connector.HostParameterSet;
			}
			return null;
		}

		private void _messageTree_BeforeCollapse(object sender, TreeTableViewCancelEventArgs e)
		{
			if (e.Node.Parent == null)
			{
				((CancelEventArgs)(object)e).Cancel = true;
			}
		}

		private void _messageTree_SizeChanged(object sender, EventArgs e)
		{
			if (_messageTree.Model != null)
			{
				((StatusTreeTableModel)(object)_messageTree.Model).AdjustColumnWidth();
			}
		}

		private void _messageTree_AfterExpand(object sender, TreeTableViewEventArgs e)
		{
			if (_messageTree.Model != null)
			{
				((StatusTreeTableModel)(object)_messageTree.Model).AdjustColumnWidth();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Expected O, but got Unknown
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Expected O, but got Unknown
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Expected O, but got Unknown
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.StatusControl));
			_panelDiag = new System.Windows.Forms.Panel();
			_messageTree = new TreeTableView();
			label2 = new System.Windows.Forms.Label();
			_btnAck = new System.Windows.Forms.Button();
			panel2 = new System.Windows.Forms.Panel();
			_labelName = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			_tbStatus = new System.Windows.Forms.TextBox();
			_panelDiag.SuspendLayout();
			panel2.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_panelDiag, "_panelDiag");
			_panelDiag.Controls.Add((System.Windows.Forms.Control)(object)_messageTree);
			_panelDiag.Controls.Add(label2);
			_panelDiag.Controls.Add(_btnAck);
			_panelDiag.Name = "_panelDiag";
			_messageTree.AllowColumnReorder=(false);
			resources.ApplyResources(_messageTree, "_messageTree");
			_messageTree.AutoRestoreSelection=(false);
			((System.Windows.Forms.Control)(object)_messageTree).BackColor = System.Drawing.SystemColors.Window;
			_messageTree.BorderStyle=(System.Windows.Forms.BorderStyle.Fixed3D);
			_messageTree.DoNotShrinkColumnsAutomatically=(false);
			_messageTree.ForceFocusOnClick=(false);
			_messageTree.GridLines=(false);
			_messageTree.HeaderStyle=(System.Windows.Forms.ColumnHeaderStyle.None);
			_messageTree.ImmediateEdit=(false);
			_messageTree.Indent=(20);
			_messageTree.KeepColumnWidthsAdjusted=(false);
			_messageTree.Model=((ITreeTableModel)null);
			_messageTree.MultiSelect=(false);
			((System.Windows.Forms.Control)(object)_messageTree).Name = "_messageTree";
			_messageTree.NoSearchStrings=(false);
			_messageTree.OnlyWhenFocused=(false);
			_messageTree.OpenEditOnDblClk=(false);
			_messageTree.ReadOnly=(false);
			_messageTree.Scrollable=(true);
			_messageTree.ShowLines=(true);
			_messageTree.ShowPlusMinus=(true);
			_messageTree.ShowRootLines=(false);
			_messageTree.SmallImageList=((System.Windows.Forms.ImageList)null);
			_messageTree.ToggleOnDblClk=(false);
			_messageTree.BeforeCollapse+=(new TreeTableViewCancelEventHandler(_messageTree_BeforeCollapse));
			_messageTree.AfterCollapse+=(new TreeTableViewEventHandler(_messageTree_AfterExpand));
			_messageTree.AfterExpand+=(new TreeTableViewEventHandler(_messageTree_AfterExpand));
			((System.Windows.Forms.Control)(object)_messageTree).SizeChanged += new System.EventHandler(_messageTree_SizeChanged);
			resources.ApplyResources(label2, "label2");
			label2.Name = "label2";
			resources.ApplyResources(_btnAck, "_btnAck");
			_btnAck.Name = "_btnAck";
			_btnAck.UseVisualStyleBackColor = true;
			_btnAck.Click += new System.EventHandler(OnButtonAckClick);
			panel2.Controls.Add(_labelName);
			panel2.Controls.Add(label1);
			panel2.Controls.Add(_tbStatus);
			resources.ApplyResources(panel2, "panel2");
			panel2.Name = "panel2";
			resources.ApplyResources(_labelName, "_labelName");
			_labelName.Name = "_labelName";
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			resources.ApplyResources(_tbStatus, "_tbStatus");
			_tbStatus.BackColor = System.Drawing.SystemColors.Control;
			_tbStatus.Name = "_tbStatus";
			_tbStatus.ReadOnly = true;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			base.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			base.Controls.Add(_panelDiag);
			base.Controls.Add(panel2);
			MinimumSize = new System.Drawing.Size(360, 4);
			base.Name = "StatusControl";
			_panelDiag.ResumeLayout(false);
			_panelDiag.PerformLayout();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
