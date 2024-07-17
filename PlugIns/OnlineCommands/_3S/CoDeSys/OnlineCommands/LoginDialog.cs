#define DEBUG
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_login.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/login_to.htm")]
    public class LoginDialog : Form
    {
        private string _stUsername;

        private string _stPassword;

        private IContainer components;

        private Button _okButton;

        private Button _cancelButton;

        private Label _lblUserName;

        private TextBox _outputUserNameTextBox;

        private TextBox _outputPasswordTextBox;

        private Label _lblPassword;

        private Label _lblDeviceName;

        private TextBox _deviceNameTextBox;

        private TextBox _deviceAddressTextBox;

        private Label _lblDeviceAddress;

        private Label _lbOperation;

        private Label _lbOperationInfo;

        public string Username => _stUsername;

        public string Password => _stPassword;

        public LoginDialog()
        {
            InitializeComponent();
        }

        public void Initialize(IProvideDeviceCredentialsArgs args)
        {
            //IL_0055: Unknown result type (might be due to invalid IL or missing references)
            //IL_005b: Expected O, but got Unknown
            //IL_00b4: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c9: Unknown result type (might be due to invalid IL or missing references)
            //IL_017f: Unknown result type (might be due to invalid IL or missing references)
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (args.DeviceObjectProjectHandle >= 0 && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(args.DeviceObjectProjectHandle, args.DeviceObjectGuid))
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(args.DeviceObjectProjectHandle, args.DeviceObjectGuid);
                Debug.Assert(objectToRead != null);
                IDeviceObject val = (IDeviceObject)objectToRead.Object;
                _deviceNameTextBox.Text = $"{objectToRead.Name} ({val.DeviceInfo.Name})";
            }
            _deviceAddressTextBox.Text = ((args.Address != null) ? args.Address.ToString() : string.Empty);
            bool visible = false;
            if (args is IProvideDeviceCredentialsArgs2 && !string.IsNullOrEmpty(((IProvideDeviceCredentialsArgs2)args).ObjectName))
            {
                long num = ((IProvideDeviceCredentialsArgs2)args).ObjectRights;
                IList<string> list = new List<string>();
                if ((num & 8) == 8)
                {
                    list.Add(Strings.UserOperation_AddRemove);
                }
                if ((num & 4) == 4)
                {
                    list.Add(Strings.UserOperation_Execute);
                }
                if ((num & 2) == 2)
                {
                    list.Add(Strings.UserOperation_Modify);
                }
                if ((num & 1) == 1)
                {
                    list.Add(Strings.UserOperation_View);
                }
                if (list.Count > 0)
                {
                    visible = true;
                    string text = "";
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        text = text + list[i] + "  |  ";
                    }
                    text += list[list.Count - 1];
                    string text2 = ((IProvideDeviceCredentialsArgs2)args).ObjectName;
                    if (text2.Length > 50)
                    {
                        text2 = text2.Substring(0, 50) + "(...)";
                    }
                    if (text2.Contains("/"))
                    {
                        _lbOperation.Text = Strings.Operation + ":" + Environment.NewLine + Strings.Directory + ":";
                    }
                    else
                    {
                        _lbOperation.Text = Strings.Operation + ":" + Environment.NewLine + Strings.Object + ":";
                    }
                    text = text + Environment.NewLine + "\"" + text2 + "\"" + Environment.NewLine;
                    _lbOperationInfo.Text = text;
                }
            }
            _lbOperationInfo.Visible = visible;
            _lbOperation.Visible = visible;
        }

        public void Initialize(bool firstCall)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControlStates();
        }

        private void UpdateControlStates()
        {
            _okButton.Enabled = _outputUserNameTextBox.Text.Trim().Length > 0;
        }

        private void _outputUserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void _outputPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _stUsername = _outputUserNameTextBox.Text;
            _stPassword = _outputPasswordTextBox.Text;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.LoginDialog));
            _okButton = new System.Windows.Forms.Button();
            _cancelButton = new System.Windows.Forms.Button();
            _lblUserName = new System.Windows.Forms.Label();
            _outputUserNameTextBox = new System.Windows.Forms.TextBox();
            _outputPasswordTextBox = new System.Windows.Forms.TextBox();
            _lblPassword = new System.Windows.Forms.Label();
            _lblDeviceName = new System.Windows.Forms.Label();
            _deviceNameTextBox = new System.Windows.Forms.TextBox();
            _deviceAddressTextBox = new System.Windows.Forms.TextBox();
            _lblDeviceAddress = new System.Windows.Forms.Label();
            _lbOperation = new System.Windows.Forms.Label();
            _lbOperationInfo = new System.Windows.Forms.Label();
            System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            resources.ApplyResources(pictureBox, "pictureBox1");
            pictureBox.Name = "pictureBox1";
            pictureBox.TabStop = false;
            resources.ApplyResources(label, "label1");
            label.Name = "label1";
            resources.ApplyResources(_okButton, "_okButton");
            _okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            _okButton.Name = "_okButton";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(_okButton_Click);
            resources.ApplyResources(_cancelButton, "_cancelButton");
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _cancelButton.Name = "_cancelButton";
            _cancelButton.UseVisualStyleBackColor = true;
            resources.ApplyResources(_lblUserName, "_lblUserName");
            _lblUserName.Name = "_lblUserName";
            resources.ApplyResources(_outputUserNameTextBox, "_outputUserNameTextBox");
            _outputUserNameTextBox.Name = "_outputUserNameTextBox";
            _outputUserNameTextBox.TextChanged += new System.EventHandler(_outputUserNameTextBox_TextChanged);
            resources.ApplyResources(_outputPasswordTextBox, "_outputPasswordTextBox");
            _outputPasswordTextBox.Name = "_outputPasswordTextBox";
            _outputPasswordTextBox.UseSystemPasswordChar = true;
            resources.ApplyResources(_lblPassword, "_lblPassword");
            _lblPassword.Name = "_lblPassword";
            resources.ApplyResources(_lblDeviceName, "_lblDeviceName");
            _lblDeviceName.Name = "_lblDeviceName";
            resources.ApplyResources(_deviceNameTextBox, "_deviceNameTextBox");
            _deviceNameTextBox.Name = "_deviceNameTextBox";
            _deviceNameTextBox.ReadOnly = true;
            _deviceNameTextBox.TabStop = false;
            resources.ApplyResources(_deviceAddressTextBox, "_deviceAddressTextBox");
            _deviceAddressTextBox.Name = "_deviceAddressTextBox";
            _deviceAddressTextBox.ReadOnly = true;
            _deviceAddressTextBox.TabStop = false;
            resources.ApplyResources(_lblDeviceAddress, "_lblDeviceAddress");
            _lblDeviceAddress.Name = "_lblDeviceAddress";
            resources.ApplyResources(_lbOperation, "_lbOperation");
            _lbOperation.Name = "_lbOperation";
            resources.ApplyResources(_lbOperationInfo, "_lbOperationInfo");
            _lbOperationInfo.Name = "_lbOperationInfo";
            base.AcceptButton = _okButton;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _cancelButton;
            base.Controls.Add(_lbOperationInfo);
            base.Controls.Add(_lbOperation);
            base.Controls.Add(_lblDeviceAddress);
            base.Controls.Add(_deviceAddressTextBox);
            base.Controls.Add(_deviceNameTextBox);
            base.Controls.Add(_lblDeviceName);
            base.Controls.Add(label);
            base.Controls.Add(pictureBox);
            base.Controls.Add(_outputPasswordTextBox);
            base.Controls.Add(_lblPassword);
            base.Controls.Add(_outputUserNameTextBox);
            base.Controls.Add(_lblUserName);
            base.Controls.Add(_cancelButton);
            base.Controls.Add(_okButton);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "LoginDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
