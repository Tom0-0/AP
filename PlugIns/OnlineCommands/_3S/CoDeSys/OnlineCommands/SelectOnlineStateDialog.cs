using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.OnlineUI;

namespace _3S.CoDeSys.OnlineCommands
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_using_monitoring_in_pous.htm")]
	[AssociatedOnlineHelpTopic("core.frame.userinterface.chm::/user_interface_in_online_mode.htm")]
	public class SelectOnlineStateDialog : Form
	{
		private IInstanceFormatter _formatter;

		private string[] _instancePaths;

		private IMetaObjectStub _mos;

		private OnlineState _onlineState;

		private string _stImplemenation;

		private IContainer components;

		private ComboBox _applicationComboBox;

		private Label _applicationLabel;

		private Button _cancelButton;

		private Label _dividerLabel;

		private CheckBox _implementationCheckBox;

		private Label _instanceLabel;

		private TreeTableView _instanceTreeTableView;

		private RadioButton _offlineModeRadioButton;

		private Button _okButton;

		private RadioButton _onlineModeRadioButton;

		public OnlineState OnlineState => _onlineState;

		public SelectOnlineStateDialog()
		{
			InitializeComponent();
		}

		private void _applicationComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplicationComboBoxItem applicationComboBoxItem = _applicationComboBox.SelectedItem as ApplicationComboBoxItem;
			if (applicationComboBoxItem != null)
			{
				InstanceTreeTableModel instanceTreeTableModel = new InstanceTreeTableModel(_instancePaths, applicationComboBoxItem.Application, applicationComboBoxItem.ApplicationGuid, _mos.Name, _formatter);
				instanceTreeTableModel.Sort(null, bRecursive: true, null);
				_instanceTreeTableView.Model = instanceTreeTableModel;
				if (_instanceTreeTableView.Nodes.Count > 0)
				{
					_instanceTreeTableView.Nodes[0].Selected = true;
				}
				else
				{
					_implementationCheckBox.Checked = true;
				}
			}
			else
			{
				_instanceTreeTableView.Model = null;
			}
			UpdateControlStates();
		}

		public void Initialize(IMetaObjectStub mos, string[] instancePaths, IInstanceFormatter formatter, string stImplemenation)
		{
			if (mos == null)
			{
				throw new ArgumentNullException("mos");
			}
			if (instancePaths == null)
			{
				throw new ArgumentNullException("instancePaths");
			}
			_formatter = formatter;
			_mos = mos;
			_instancePaths = instancePaths;
			_stImplemenation = stImplemenation;
		}

		private void OnImplementationCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		private void OnInstanceTreeTableViewDoubleClick(object sender, EventArgs e)
		{
			OnOkButtonClick(sender, e);
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void OnInstanceTreeTableViewSelectionChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (_instancePaths == null)
			{
				throw new InvalidOperationException("This dialog has not been initialized yet.");
			}
			base.OnLoad(e);
			string fullName = ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(_mos.ProjectHandle, _mos.ObjectGuid);
			Text = $"{Text} - {fullName}";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string[] instancePaths = _instancePaths;
			for (int i = 0; i < instancePaths.Length; i++)
			{
				Common.SplitInstancePath(instancePaths[i], out var stDevice, out var stApplication, out var applicationGuid);
				string text;
				bool bDevice;
				if (stApplication == null)
				{
					text = stDevice;
					bDevice = true;
				}
				else
				{
					text = stDevice + "." + stApplication;
					bDevice = false;
				}
				if (!dictionary.ContainsKey(text))
				{
					ApplicationComboBoxItem item = new ApplicationComboBoxItem(applicationGuid, text, bDevice);
					_applicationComboBox.Items.Add(item);
					dictionary[text] = null;
				}
			}
			if (_applicationComboBox.Items.Count > 0)
			{
				_applicationComboBox.SelectedIndex = 0;
				return;
			}
			_onlineModeRadioButton.Enabled = false;
			_applicationLabel.Enabled = false;
			_applicationComboBox.Enabled = false;
			_instanceLabel.Enabled = false;
			_instanceTreeTableView.Enabled = false;
			_implementationCheckBox.Enabled = false;
			_offlineModeRadioButton.Checked = true;
		}

		private void OnOfflineModeRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		private void OnOkButtonClick(object sender, EventArgs e)
		{
			if (_onlineModeRadioButton.Checked)
			{
				ApplicationComboBoxItem applicationComboBoxItem = _applicationComboBox.SelectedItem as ApplicationComboBoxItem;
				_onlineState.OnlineApplication = applicationComboBoxItem.ApplicationGuid;
				_onlineState.InstancePath = applicationComboBoxItem.Application + ".";
				if (_implementationCheckBox.Checked)
				{
					_onlineState.InstancePath = _stImplemenation;
					return;
				}
				TreeTableViewNode viewNode = _instanceTreeTableView.SelectedNodes[0];
				InstanceTreeTableNode instanceTreeTableNode = _instanceTreeTableView.GetModelNode(viewNode) as InstanceTreeTableNode;
				_onlineState.InstancePath += instanceTreeTableNode.Instance;
			}
			else
			{
				_onlineState.OnlineApplication = Guid.Empty;
				_onlineState.InstancePath = null;
			}
		}

		private void OnOnlineModeRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		private void UpdateControlStates()
		{
			_applicationLabel.Enabled = _onlineModeRadioButton.Checked;
			_applicationComboBox.Enabled = _onlineModeRadioButton.Checked;
			_instanceLabel.Enabled = _onlineModeRadioButton.Checked && !_implementationCheckBox.Checked;
			_instanceTreeTableView.Enabled = _onlineModeRadioButton.Checked && !_implementationCheckBox.Checked;
			_implementationCheckBox.Enabled = _onlineModeRadioButton.Checked;
			_okButton.Enabled = _offlineModeRadioButton.Checked || (_applicationComboBox.SelectedItem != null && (_instanceTreeTableView.SelectedNodes.Count > 0 || _implementationCheckBox.Checked));
			if (_onlineModeRadioButton.Checked)
			{
				if (_applicationComboBox.SelectedItem != null)
				{
					_okButton.Enabled = _instanceTreeTableView.SelectedNodes.Count > 0 || _implementationCheckBox.Checked;
				}
				else
				{
					_okButton.Enabled = false;
				}
			}
			else
			{
				_okButton.Enabled = true;
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
			_onlineModeRadioButton = new System.Windows.Forms.RadioButton();
			_applicationLabel = new System.Windows.Forms.Label();
			_instanceTreeTableView = new _3S.CoDeSys.Controls.Controls.TreeTableView();
			_offlineModeRadioButton = new System.Windows.Forms.RadioButton();
			_okButton = new System.Windows.Forms.Button();
			_cancelButton = new System.Windows.Forms.Button();
			_dividerLabel = new System.Windows.Forms.Label();
			_instanceLabel = new System.Windows.Forms.Label();
			_implementationCheckBox = new System.Windows.Forms.CheckBox();
			_applicationComboBox = new System.Windows.Forms.ComboBox();
			SuspendLayout();
			_onlineModeRadioButton.Checked = true;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.SelectOnlineStateDialog));
			resources.ApplyResources(_onlineModeRadioButton, "_onlineModeRadioButton");
			_onlineModeRadioButton.Name = "_onlineModeRadioButton";
			_onlineModeRadioButton.TabStop = true;
			_onlineModeRadioButton.CheckedChanged += new System.EventHandler(OnOnlineModeRadioButtonCheckedChanged);
			resources.ApplyResources(_applicationLabel, "_applicationLabel");
			_applicationLabel.Name = "_applicationLabel";
			resources.ApplyResources(_instanceTreeTableView, "_instanceTreeTableView");
			_instanceTreeTableView.AllowColumnReorder = true;
			_instanceTreeTableView.BackColor = System.Drawing.SystemColors.Window;
			_instanceTreeTableView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_instanceTreeTableView.DoNotShrinkColumnsAutomatically = true;
			_instanceTreeTableView.GridLines = false;
			_instanceTreeTableView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			_instanceTreeTableView.HideSelection = false;
			_instanceTreeTableView.ImmediateEdit = false;
			_instanceTreeTableView.Indent = 20;
			_instanceTreeTableView.KeepColumnWidthsAdjusted = true;
			_instanceTreeTableView.Model = null;
			_instanceTreeTableView.MultiSelect = false;
			_instanceTreeTableView.Name = "_instanceTreeTableView";
			_instanceTreeTableView.OpenEditOnDblClk = false;
			_instanceTreeTableView.ReadOnly = false;
			_instanceTreeTableView.Scrollable = true;
			_instanceTreeTableView.ShowLines = false;
			_instanceTreeTableView.ShowPlusMinus = false;
			_instanceTreeTableView.ShowRootLines = false;
			_instanceTreeTableView.SelectionChanged += new System.EventHandler(OnInstanceTreeTableViewSelectionChanged);
			_instanceTreeTableView.DoubleClick += new System.EventHandler(OnInstanceTreeTableViewDoubleClick);
			resources.ApplyResources(_offlineModeRadioButton, "_offlineModeRadioButton");
			_offlineModeRadioButton.Name = "_offlineModeRadioButton";
			_offlineModeRadioButton.CheckedChanged += new System.EventHandler(OnOfflineModeRadioButtonCheckedChanged);
			resources.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.Click += new System.EventHandler(OnOkButtonClick);
			resources.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			resources.ApplyResources(_dividerLabel, "_dividerLabel");
			_dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_dividerLabel.Name = "_dividerLabel";
			resources.ApplyResources(_instanceLabel, "_instanceLabel");
			_instanceLabel.Name = "_instanceLabel";
			resources.ApplyResources(_implementationCheckBox, "_implementationCheckBox");
			_implementationCheckBox.Name = "_implementationCheckBox";
			_implementationCheckBox.CheckedChanged += new System.EventHandler(OnImplementationCheckBoxCheckedChanged);
			resources.ApplyResources(_applicationComboBox, "_applicationComboBox");
			_applicationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_applicationComboBox.FormattingEnabled = true;
			_applicationComboBox.Name = "_applicationComboBox";
			_applicationComboBox.SelectedIndexChanged += new System.EventHandler(_applicationComboBox_SelectedIndexChanged);
			base.AcceptButton = _okButton;
			resources.ApplyResources(this, "$this");
			base.CancelButton = _cancelButton;
			base.Controls.Add(_applicationComboBox);
			base.Controls.Add(_implementationCheckBox);
			base.Controls.Add(_dividerLabel);
			base.Controls.Add(_cancelButton);
			base.Controls.Add(_okButton);
			base.Controls.Add(_offlineModeRadioButton);
			base.Controls.Add(_instanceTreeTableView);
			base.Controls.Add(_applicationLabel);
			base.Controls.Add(_onlineModeRadioButton);
			base.Controls.Add(_instanceLabel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SelectOnlineStateDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
