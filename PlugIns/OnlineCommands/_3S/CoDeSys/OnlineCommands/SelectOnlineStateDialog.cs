#define DEBUG
using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.OnlineUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_using_monitoring_in_pous.htm")]
    [AssociatedOnlineHelpTopic("core.frame.userinterface.chm::/user_interface_in_online_mode.htm")]
    public class SelectOnlineStateDialog : Form
    {
        private IMetaObjectStub _mos;

        private string[] _instancePaths;

        private OnlineState _onlineState;

        private IInstanceFormatter _formatter;

        private Label _applicationLabel;

        private TreeTableView _instanceTreeTableView;

        private Button _okButton;

        private Button _cancelButton;

        private RadioButton _onlineModeRadioButton;

        private RadioButton _offlineModeRadioButton;

        private Label _dividerLabel;

        private Label _instanceLabel;

        private CheckBox _implementationCheckBox;

        private ComboBox _applicationComboBox;

        private TextBoxCue _findWhatTextBox;

        private IContainer components;

        private string _stImplemenation;

        public OnlineState OnlineState => _onlineState;

        public SelectOnlineStateDialog()
        {
            InitializeComponent();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
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
            ((Control)(object)_instanceTreeTableView).Enabled = false;
            _implementationCheckBox.Enabled = false;
            _offlineModeRadioButton.Checked = true;
        }

        private void UpdateControlStates()
        {
            _applicationLabel.Enabled = _onlineModeRadioButton.Checked;
            _applicationComboBox.Enabled = _onlineModeRadioButton.Checked;
            _instanceLabel.Enabled = _onlineModeRadioButton.Checked && !_implementationCheckBox.Checked;
            ((Control)(object)_instanceTreeTableView).Enabled = _onlineModeRadioButton.Checked && !_implementationCheckBox.Checked;
            _implementationCheckBox.Enabled = _onlineModeRadioButton.Checked;
            _okButton.Enabled = _offlineModeRadioButton.Checked || (_applicationComboBox.SelectedItem != null && (((TreeTableViewNodeCollection)_instanceTreeTableView.SelectedNodes).Count > 0 || _implementationCheckBox.Checked));
            if (_onlineModeRadioButton.Checked)
            {
                if (_applicationComboBox.SelectedItem != null)
                {
                    _okButton.Enabled = ((TreeTableViewNodeCollection)_instanceTreeTableView.SelectedNodes).Count > 0 || _implementationCheckBox.Checked;
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._onlineModeRadioButton = new System.Windows.Forms.RadioButton();
            this._applicationLabel = new System.Windows.Forms.Label();
            this._instanceTreeTableView = new _3S.CoDeSys.Controls.Controls.TreeTableView();
            this._offlineModeRadioButton = new System.Windows.Forms.RadioButton();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._dividerLabel = new System.Windows.Forms.Label();
            this._instanceLabel = new System.Windows.Forms.Label();
            this._implementationCheckBox = new System.Windows.Forms.CheckBox();
            this._applicationComboBox = new System.Windows.Forms.ComboBox();
            this._findWhatTextBox = new _3S.CoDeSys.Controls.Controls.TextBoxCue();
            this.SuspendLayout();
            // 
            // _onlineModeRadioButton
            // 
            this._onlineModeRadioButton.Checked = true;
            this._onlineModeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._onlineModeRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this._onlineModeRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._onlineModeRadioButton.Location = new System.Drawing.Point(16, 16);
            this._onlineModeRadioButton.Name = "_onlineModeRadioButton";
            this._onlineModeRadioButton.Size = new System.Drawing.Size(96, 16);
            this._onlineModeRadioButton.TabIndex = 0;
            this._onlineModeRadioButton.TabStop = true;
            this._onlineModeRadioButton.Text = "O&nline mode";
            this._onlineModeRadioButton.CheckedChanged += new System.EventHandler(this.OnOnlineModeRadioButtonCheckedChanged);
            // 
            // _applicationLabel
            // 
            this._applicationLabel.AutoSize = true;
            this._applicationLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._applicationLabel.Location = new System.Drawing.Point(32, 40);
            this._applicationLabel.Name = "_applicationLabel";
            this._applicationLabel.Size = new System.Drawing.Size(95, 13);
            this._applicationLabel.TabIndex = 1;
            this._applicationLabel.Text = "&Device/Application";
            // 
            // _instanceTreeTableView
            // 
            this._instanceTreeTableView.AllowColumnReorder = true;
            this._instanceTreeTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._instanceTreeTableView.AutoRestoreSelection = false;
            this._instanceTreeTableView.AutoSize = true;
            this._instanceTreeTableView.AutoScroll = true;
            this._instanceTreeTableView.BackColor = System.Drawing.SystemColors.Window;
            this._instanceTreeTableView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._instanceTreeTableView.DoNotShrinkColumnsAutomatically = false;
            this._instanceTreeTableView.ForceFocusOnClick = false;
            this._instanceTreeTableView.GridLines = false;
            this._instanceTreeTableView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._instanceTreeTableView.HideSelection = false;
            this._instanceTreeTableView.ImmediateEdit = false;
            this._instanceTreeTableView.Indent = 20;
            this._instanceTreeTableView.KeepColumnWidthsAdjusted = true;
            this._instanceTreeTableView.Location = new System.Drawing.Point(35, 135);
            this._instanceTreeTableView.Model = null;
            this._instanceTreeTableView.MultiSelect = false;
            this._instanceTreeTableView.Name = "_instanceTreeTableView";
            this._instanceTreeTableView.NoSearchStrings = false;
            this._instanceTreeTableView.OnlyWhenFocused = false;
            this._instanceTreeTableView.OpenEditOnDblClk = false;
            this._instanceTreeTableView.ReadOnly = false;
            this._instanceTreeTableView.Scrollable = true;
            this._instanceTreeTableView.ShowLines = false;
            this._instanceTreeTableView.ShowPlusMinus = false;
            this._instanceTreeTableView.ShowRootLines = false;
            this._instanceTreeTableView.Size = new System.Drawing.Size(467, 188);
            this._instanceTreeTableView.TabIndex = 5;
            this._instanceTreeTableView.ToggleOnDblClk = false;
            this._instanceTreeTableView.SelectionChanged += new System.EventHandler(this.OnInstanceTreeTableViewSelectionChanged);
            this._instanceTreeTableView.DoubleClick += new System.EventHandler(this.OnInstanceTreeTableViewDoubleClick);
            // 
            // _offlineModeRadioButton
            // 
            this._offlineModeRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._offlineModeRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._offlineModeRadioButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this._offlineModeRadioButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._offlineModeRadioButton.Location = new System.Drawing.Point(16, 367);
            this._offlineModeRadioButton.Name = "_offlineModeRadioButton";
            this._offlineModeRadioButton.Size = new System.Drawing.Size(96, 16);
            this._offlineModeRadioButton.TabIndex = 7;
            this._offlineModeRadioButton.Text = "O&ffline mode";
            this._offlineModeRadioButton.CheckedChanged += new System.EventHandler(this.OnOfflineModeRadioButtonCheckedChanged);
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._okButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._okButton.Location = new System.Drawing.Point(336, 412);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(80, 24);
            this._okButton.TabIndex = 8;
            this._okButton.Text = "OK";
            this._okButton.Click += new System.EventHandler(this.OnOkButtonClick);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._cancelButton.Location = new System.Drawing.Point(422, 412);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(80, 24);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "Cancel";
            // 
            // _dividerLabel
            // 
            this._dividerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._dividerLabel.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this._dividerLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._dividerLabel.Location = new System.Drawing.Point(0, 399);
            this._dividerLabel.Name = "_dividerLabel";
            this._dividerLabel.Size = new System.Drawing.Size(520, 2);
            this._dividerLabel.TabIndex = 7;
            // 
            // _instanceLabel
            // 
            this._instanceLabel.AutoSize = true;
            this._instanceLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._instanceLabel.Location = new System.Drawing.Point(32, 93);
            this._instanceLabel.Name = "_instanceLabel";
            this._instanceLabel.Size = new System.Drawing.Size(120, 13);
            this._instanceLabel.TabIndex = 3;
            this._instanceLabel.Text = "Function Block &Instance";
            // 
            // _implementationCheckBox
            // 
            this._implementationCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._implementationCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._implementationCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._implementationCheckBox.Location = new System.Drawing.Point(35, 329);
            this._implementationCheckBox.Name = "_implementationCheckBox";
            this._implementationCheckBox.Size = new System.Drawing.Size(104, 16);
            this._implementationCheckBox.TabIndex = 6;
            this._implementationCheckBox.Text = "I&mplementation";
            this._implementationCheckBox.CheckedChanged += new System.EventHandler(this.OnImplementationCheckBoxCheckedChanged);
            // 
            // _applicationComboBox
            // 
            this._applicationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._applicationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._applicationComboBox.FormattingEnabled = true;
            this._applicationComboBox.Location = new System.Drawing.Point(35, 56);
            this._applicationComboBox.Name = "_applicationComboBox";
            this._applicationComboBox.Size = new System.Drawing.Size(467, 21);
            this._applicationComboBox.TabIndex = 2;
            this._applicationComboBox.SelectedIndexChanged += new System.EventHandler(this._applicationComboBox_SelectedIndexChanged);
            // 
            // _findWhatTextBox
            // 
            this._findWhatTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._findWhatTextBox.CueText = "Enter text to filter instances";
            this._findWhatTextBox.Location = new System.Drawing.Point(35, 110);
            this._findWhatTextBox.Name = "_findWhatTextBox";
            this._findWhatTextBox.ShowEvenIfFocused = true;
            this._findWhatTextBox.Size = new System.Drawing.Size(467, 21);
            this._findWhatTextBox.TabIndex = 10;
            this._findWhatTextBox.TextChanged += new System.EventHandler(this._findWhatTextBox_TextChanged);
            // 
            // SelectOnlineStateDialog
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(514, 448);
            this.Controls.Add(this._findWhatTextBox);
            this.Controls.Add(this._applicationComboBox);
            this.Controls.Add(this._implementationCheckBox);
            this.Controls.Add(this._dividerLabel);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._offlineModeRadioButton);
            this.Controls.Add(this._instanceTreeTableView);
            this.Controls.Add(this._applicationLabel);
            this.Controls.Add(this._onlineModeRadioButton);
            this.Controls.Add(this._instanceLabel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 480);
            this.Name = "SelectOnlineStateDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Online State";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void OnInstanceTreeTableViewSelectionChanged(object sender, EventArgs e)
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
                    if (!OnlineCommandHelper.IsPoolPOU(_mos))
                    {
                        _onlineState.InstancePath = _stImplemenation;
                    }
                    else
                    {
                        _onlineState.InstancePath += ((IObjectManager)APEnvironment.ObjectMgr).GetDottedFullName(_mos.ProjectHandle, _mos.ObjectGuid);
                    }
                }
                else
                {
                    Debug.Assert(((TreeTableViewNodeCollection)_instanceTreeTableView.SelectedNodes).Count > 0);
                    TreeTableViewNode val = ((TreeTableViewNodeCollection)_instanceTreeTableView.SelectedNodes)[0];
                    InstanceTreeTableNode instanceTreeTableNode = _instanceTreeTableView.GetModelNode(val) as InstanceTreeTableNode;
                    Debug.Assert(instanceTreeTableNode != null);
                    _onlineState.InstancePath += instanceTreeTableNode.Instance;
                }
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

        private void OnImplementationCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void OnOfflineModeRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void OnInstanceTreeTableViewDoubleClick(object sender, EventArgs e)
        {
            OnOkButtonClick(sender, e);
            base.DialogResult = DialogResult.OK;
            Close();
        }

        private void _applicationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplicationComboBoxItem item = _applicationComboBox.SelectedItem as ApplicationComboBoxItem;
            UpdateModel(item, ((Control)(object)_findWhatTextBox).Text);
            UpdateControlStates();
        }

        private void _findWhatTextBox_TextChanged(object sender, EventArgs e)
        {
            bool flag = false;
            if (_instanceTreeTableView.Nodes.Count == 0)
            {
                flag = true;
            }
            ApplicationComboBoxItem item = _applicationComboBox.SelectedItem as ApplicationComboBoxItem;
            UpdateModel(item, ((Control)(object)_findWhatTextBox).Text);
            if (flag && _instanceTreeTableView.Nodes.Count > 0 && _implementationCheckBox.Checked)
            {
                _implementationCheckBox.Checked = false;
            }
        }

        private void UpdateModel(ApplicationComboBoxItem item, string filter)
        {
            if (item != null)
            {
                InstanceTreeTableModel instanceTreeTableModel = new InstanceTreeTableModel(_instancePaths, item.Application, item.ApplicationGuid, _mos.Name, _formatter, filter);
                ((DefaultTreeTableModel)instanceTreeTableModel).Sort((ITreeTableNode)null, true, (IComparer)null);
                _instanceTreeTableView.Model = ((ITreeTableModel)(object)instanceTreeTableModel);
                if (_instanceTreeTableView.Nodes.Count > 0)
                {
                    _instanceTreeTableView.Nodes[0].Selected = (true);
                }
                else
                {
                    _implementationCheckBox.Checked = true;
                }
            }
            else
            {
                _instanceTreeTableView.Model = ((ITreeTableModel)null);
            }
        }
    }
}
