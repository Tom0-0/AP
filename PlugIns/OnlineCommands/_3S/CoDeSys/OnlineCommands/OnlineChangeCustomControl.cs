using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Online;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    public class OnlineChangeCustomControl : UserControl, ICustomControlProvider
    {
        private BootProjectTransferMode _transferMode;

        private bool _bTempDownloadBootApp = true;

        private bool _bDuringReload = true;

        private bool _bInitialOnlineChange;

        private bool _bInitialFullDownload = true;

        private bool _bFirstRadioPress = true;

        private const string ID_BOOT_APP = "Checkbox_UpdateBootApp";

        private const string ID_RADIO = "Radiobuttons_OnlineChangeMode";

        private const string VALUE_RADIO1 = "OnlineChange";

        private const string VALUE_RADIO2 = "FullDownload";

        private const string VALUE_RADIO3 = "LoginWithoutChange";

        private IContainer components;

        private RadioButton _radio1;

        private RadioButton _radio3;

        private RadioButton _radio2;

        private CheckBox _chkBootApp;

        private GroupBox groupBox1;

        internal bool DownloadBootApp => _chkBootApp.Checked;

        internal int SelectedOnlineChangeOption
        {
            get
            {
                if (_radio1.Checked)
                {
                    return 0;
                }
                if (_radio2.Checked)
                {
                    return 1;
                }
                return 2;
            }
        }

        public Control ControlPanel => this;

        public IList<string> SubControlIDs => new List<string> { "Checkbox_UpdateBootApp", "Radiobuttons_OnlineChangeMode" };

        public Control ControlToSelect => _radio1;

        public OnlineChangeCustomControl()
        {
            InitializeComponent();
        }

        internal void Initialize(string[] radioStrings, bool bInitialOnlineChange, bool bInitialFullDownload, BootProjectTransferMode transferMode)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0045: Unknown result type (might be due to invalid IL or missing references)
            if (radioStrings == null || radioStrings.Length < 2 || radioStrings.Length > 3)
            {
                throw new ArgumentException("Invalid string array. Expecting two or three strings!");
            }
            _transferMode = transferMode;
            _chkBootApp.Checked = (_bTempDownloadBootApp = bInitialOnlineChange);
            _bInitialOnlineChange = bInitialOnlineChange;
            _bInitialFullDownload = bInitialFullDownload;
            if ((int)transferMode == 0)
            {
                _chkBootApp.Enabled = false;
            }
            _radio1.Text = radioStrings[0];
            _radio2.Text = radioStrings[1];
            if (radioStrings.Length == 3)
            {
                _radio3.Text = radioStrings[2];
                _radio3.Visible = true;
            }
            else
            {
                _radio3.Visible = false;
            }
            _radio1.Checked = true;
            _bDuringReload = false;
        }

        private void _radio1_CheckedChanged(object sender, EventArgs e)
        {
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            if (!_bDuringReload && _radio1.Checked)
            {
                if (_bFirstRadioPress)
                {
                    _chkBootApp.Checked = _bInitialOnlineChange;
                    _bTempDownloadBootApp = _bInitialOnlineChange;
                }
                else
                {
                    _chkBootApp.Checked = _bTempDownloadBootApp;
                }
                if ((int)_transferMode != 0)
                {
                    _chkBootApp.Enabled = true;
                }
                else
                {
                    _chkBootApp.Checked = _bInitialOnlineChange;
                }
            }
        }

        private void _radio2_CheckedChanged(object sender, EventArgs e)
        {
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            if (!_bDuringReload && _radio2.Checked)
            {
                if (_bFirstRadioPress)
                {
                    _chkBootApp.Checked = _bInitialFullDownload;
                    _bTempDownloadBootApp = _bInitialFullDownload;
                }
                else
                {
                    _chkBootApp.Checked = _bTempDownloadBootApp;
                }
                if ((int)_transferMode != 0)
                {
                    _chkBootApp.Enabled = true;
                }
                else
                {
                    _chkBootApp.Checked = _bInitialFullDownload;
                }
            }
        }

        private void _radio3_CheckedChanged(object sender, EventArgs e)
        {
            if (!_bDuringReload && _radio3.Checked)
            {
                _bTempDownloadBootApp = _chkBootApp.Checked;
                _chkBootApp.Enabled = false;
                _chkBootApp.Checked = false;
            }
        }

        private void _chkBootApp_Click(object sender, EventArgs e)
        {
            if (!_bDuringReload && _chkBootApp.Enabled)
            {
                _bFirstRadioPress = false;
                _bTempDownloadBootApp = _chkBootApp.Checked;
            }
        }

        public string GetValue(string stSubControlID, out IList<string> AllowedValues)
        {
            AllowedValues = new List<string>();
            if (!(stSubControlID == "Checkbox_UpdateBootApp"))
            {
                if (stSubControlID == "Radiobuttons_OnlineChangeMode")
                {
                    AllowedValues.Add("OnlineChange");
                    AllowedValues.Add("FullDownload");
                    if (_radio3.Visible)
                    {
                        AllowedValues.Add("LoginWithoutChange");
                    }
                    if (_radio1.Checked)
                    {
                        return "OnlineChange";
                    }
                    if (_radio2.Checked)
                    {
                        return "FullDownload";
                    }
                    return "LoginWithoutChange";
                }
                throw new ArgumentException("Invalid Control ID: " + stSubControlID);
            }
            AllowedValues.Add("TRUE");
            AllowedValues.Add("FALSE");
            if (_chkBootApp.Checked)
            {
                return "TRUE";
            }
            return "FALSE";
        }

        public void SetValue(string stSubControlID, string stValue)
        {
            bool flag = false;
            if (!(stSubControlID == "Checkbox_UpdateBootApp"))
            {
                if (stSubControlID == "Radiobuttons_OnlineChangeMode")
                {
                    switch (stValue)
                    {
                        case "OnlineChange":
                            _radio1.Checked = true;
                            break;
                        case "FullDownload":
                            _radio2.Checked = true;
                            break;
                        case "LoginWithoutChange":
                            if (_radio3.Visible)
                            {
                                _radio3.Checked = true;
                                break;
                            }
                            goto default;
                        default:
                            flag = true;
                            break;
                    }
                }
                else
                {
                    flag = true;
                }
            }
            else if (stValue == "TRUE")
            {
                _chkBootApp.Checked = true;
            }
            else if (stValue == "FALSE")
            {
                _chkBootApp.Checked = false;
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                throw new ArgumentException("Invalid value '" + stValue + "' for Control ID: " + stSubControlID);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.OnlineChangeCustomControl));
            _radio1 = new System.Windows.Forms.RadioButton();
            _radio3 = new System.Windows.Forms.RadioButton();
            _radio2 = new System.Windows.Forms.RadioButton();
            _chkBootApp = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            resources.ApplyResources(_radio1, "_radio1");
            _radio1.Name = "_radio1";
            _radio1.TabStop = true;
            _radio1.UseVisualStyleBackColor = true;
            _radio1.CheckedChanged += new System.EventHandler(_radio1_CheckedChanged);
            resources.ApplyResources(_radio3, "_radio3");
            _radio3.Name = "_radio3";
            _radio3.TabStop = true;
            _radio3.UseVisualStyleBackColor = true;
            _radio3.CheckedChanged += new System.EventHandler(_radio3_CheckedChanged);
            resources.ApplyResources(_radio2, "_radio2");
            _radio2.Name = "_radio2";
            _radio2.TabStop = true;
            _radio2.UseVisualStyleBackColor = true;
            _radio2.CheckedChanged += new System.EventHandler(_radio2_CheckedChanged);
            resources.ApplyResources(_chkBootApp, "_chkBootApp");
            _chkBootApp.Name = "_chkBootApp";
            _chkBootApp.UseVisualStyleBackColor = true;
            _chkBootApp.Click += new System.EventHandler(_chkBootApp_Click);
            groupBox1.Controls.Add(_radio1);
            groupBox1.Controls.Add(_chkBootApp);
            groupBox1.Controls.Add(_radio3);
            groupBox1.Controls.Add(_radio2);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(groupBox1);
            base.Name = "OnlineChangeCustomControl";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }
    }
}
