using _3S.CoDeSys.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    public class OutdatedDevdescCustomControl : UserControl, ICustomControlProvider
    {
        private const string ID_IGNORE_IN_FUTURE = "Checkbox_IgnoreInFuture";

        private IContainer components;

        private CheckBox _chkRemember;

        internal bool IgnoreInTheFuture => _chkRemember.Checked;

        public Control ControlPanel => this;

        public IList<string> SubControlIDs => new List<string> { "Checkbox_IgnoreInFuture" };

        public Control ControlToSelect => _chkRemember;

        public OutdatedDevdescCustomControl()
        {
            InitializeComponent();
            _chkRemember.Checked = false;
        }

        public string GetValue(string stSubControlID, out IList<string> AllowedValues)
        {
            AllowedValues = new List<string>();
            if (stSubControlID == "Checkbox_IgnoreInFuture")
            {
                AllowedValues.Add("TRUE");
                AllowedValues.Add("FALSE");
                if (_chkRemember.Checked)
                {
                    return "TRUE";
                }
                return "FALSE";
            }
            throw new ArgumentException("Invalid Control ID: " + stSubControlID);
        }

        public void SetValue(string stSubControlID, string stValue)
        {
            bool flag = false;
            if (stSubControlID == "Checkbox_IgnoreInFuture")
            {
                if (stValue == "TRUE")
                {
                    _chkRemember.Checked = true;
                }
                else if (stValue == "FALSE")
                {
                    _chkRemember.Checked = false;
                }
                else
                {
                    flag = true;
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.OutdatedDevdescCustomControl));
            _chkRemember = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            resources.ApplyResources(_chkRemember, "_chkRemember");
            _chkRemember.Name = "_chkRemember";
            _chkRemember.UseVisualStyleBackColor = true;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(_chkRemember);
            base.Name = "OutdatedDevdescCustomControl";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
