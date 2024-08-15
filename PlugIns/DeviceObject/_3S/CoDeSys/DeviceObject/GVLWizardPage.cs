using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.DeviceObject
{
    public class GVLWizardPage : UserControl, IValidateableControl
    {
        private TextBox _nameTextBox;

        private Label _nameLabel;

        private Container components;

        public Control ObjectNameControl => _nameTextBox;

        public GVLWizardPage()
        {
            InitializeComponent();
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
            System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager(typeof(_3S.CoDeSys.DeviceObject.GVLWizardPage));
            _nameTextBox = new System.Windows.Forms.TextBox();
            _nameLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            _nameTextBox.AccessibleDescription = resourceManager.GetString("_nameTextBox.AccessibleDescription");
            _nameTextBox.AccessibleName = resourceManager.GetString("_nameTextBox.AccessibleName");
            _nameTextBox.Anchor = (System.Windows.Forms.AnchorStyles)resourceManager.GetObject("_nameTextBox.Anchor");
            _nameTextBox.AutoSize = (bool)resourceManager.GetObject("_nameTextBox.AutoSize");
            _nameTextBox.BackgroundImage = (System.Drawing.Image)resourceManager.GetObject("_nameTextBox.BackgroundImage");
            _nameTextBox.Dock = (System.Windows.Forms.DockStyle)resourceManager.GetObject("_nameTextBox.Dock");
            _nameTextBox.Enabled = (bool)resourceManager.GetObject("_nameTextBox.Enabled");
            _nameTextBox.Font = (System.Drawing.Font)resourceManager.GetObject("_nameTextBox.Font");
            _nameTextBox.ImeMode = (System.Windows.Forms.ImeMode)resourceManager.GetObject("_nameTextBox.ImeMode");
            _nameTextBox.Location = (System.Drawing.Point)resourceManager.GetObject("_nameTextBox.Location");
            _nameTextBox.MaxLength = (int)resourceManager.GetObject("_nameTextBox.MaxLength");
            _nameTextBox.Multiline = (bool)resourceManager.GetObject("_nameTextBox.Multiline");
            _nameTextBox.Name = "_nameTextBox";
            _nameTextBox.PasswordChar = (char)resourceManager.GetObject("_nameTextBox.PasswordChar");
            _nameTextBox.RightToLeft = (System.Windows.Forms.RightToLeft)resourceManager.GetObject("_nameTextBox.RightToLeft");
            _nameTextBox.ScrollBars = (System.Windows.Forms.ScrollBars)resourceManager.GetObject("_nameTextBox.ScrollBars");
            _nameTextBox.Size = (System.Drawing.Size)resourceManager.GetObject("_nameTextBox.Size");
            _nameTextBox.TabIndex = (int)resourceManager.GetObject("_nameTextBox.TabIndex");
            _nameTextBox.Text = resourceManager.GetString("_nameTextBox.Text");
            _nameTextBox.TextAlign = (System.Windows.Forms.HorizontalAlignment)resourceManager.GetObject("_nameTextBox.TextAlign");
            _nameTextBox.Visible = (bool)resourceManager.GetObject("_nameTextBox.Visible");
            _nameTextBox.WordWrap = (bool)resourceManager.GetObject("_nameTextBox.WordWrap");
            _nameLabel.AccessibleDescription = resourceManager.GetString("_nameLabel.AccessibleDescription");
            _nameLabel.AccessibleName = resourceManager.GetString("_nameLabel.AccessibleName");
            _nameLabel.Anchor = (System.Windows.Forms.AnchorStyles)resourceManager.GetObject("_nameLabel.Anchor");
            _nameLabel.AutoSize = (bool)resourceManager.GetObject("_nameLabel.AutoSize");
            _nameLabel.Dock = (System.Windows.Forms.DockStyle)resourceManager.GetObject("_nameLabel.Dock");
            _nameLabel.Enabled = (bool)resourceManager.GetObject("_nameLabel.Enabled");
            _nameLabel.Font = (System.Drawing.Font)resourceManager.GetObject("_nameLabel.Font");
            _nameLabel.Image = (System.Drawing.Image)resourceManager.GetObject("_nameLabel.Image");
            _nameLabel.ImageAlign = (System.Drawing.ContentAlignment)resourceManager.GetObject("_nameLabel.ImageAlign");
            _nameLabel.ImageIndex = (int)resourceManager.GetObject("_nameLabel.ImageIndex");
            _nameLabel.ImeMode = (System.Windows.Forms.ImeMode)resourceManager.GetObject("_nameLabel.ImeMode");
            _nameLabel.Location = (System.Drawing.Point)resourceManager.GetObject("_nameLabel.Location");
            _nameLabel.Name = "_nameLabel";
            _nameLabel.RightToLeft = (System.Windows.Forms.RightToLeft)resourceManager.GetObject("_nameLabel.RightToLeft");
            _nameLabel.Size = (System.Drawing.Size)resourceManager.GetObject("_nameLabel.Size");
            _nameLabel.TabIndex = (int)resourceManager.GetObject("_nameLabel.TabIndex");
            _nameLabel.Text = resourceManager.GetString("_nameLabel.Text");
            _nameLabel.TextAlign = (System.Drawing.ContentAlignment)resourceManager.GetObject("_nameLabel.TextAlign");
            _nameLabel.Visible = (bool)resourceManager.GetObject("_nameLabel.Visible");
            base.AccessibleDescription = resourceManager.GetString("$this.AccessibleDescription");
            base.AccessibleName = resourceManager.GetString("$this.AccessibleName");
            AutoScroll = (bool)resourceManager.GetObject("$this.AutoScroll");
            base.AutoScrollMargin = (System.Drawing.Size)resourceManager.GetObject("$this.AutoScrollMargin");
            base.AutoScrollMinSize = (System.Drawing.Size)resourceManager.GetObject("$this.AutoScrollMinSize");
            BackgroundImage = (System.Drawing.Image)resourceManager.GetObject("$this.BackgroundImage");
            base.Controls.Add(_nameTextBox);
            base.Controls.Add(_nameLabel);
            base.Enabled = (bool)resourceManager.GetObject("$this.Enabled");
            Font = (System.Drawing.Font)resourceManager.GetObject("$this.Font");
            base.ImeMode = (System.Windows.Forms.ImeMode)resourceManager.GetObject("$this.ImeMode");
            base.Location = (System.Drawing.Point)resourceManager.GetObject("$this.Location");
            base.Name = "GVLWizardPage";
            RightToLeft = (System.Windows.Forms.RightToLeft)resourceManager.GetObject("$this.RightToLeft");
            base.Size = (System.Drawing.Size)resourceManager.GetObject("$this.Size");
            ResumeLayout(false);
        }

        public bool Validate(ref string stMessage, ref Control failedControl)
        {
            if (!DeviceObject.IsIdentifier(_nameTextBox.Text.Trim()))
            {
                stMessage = LogicalIOStrings.NameMustBeIdentifier;
                failedControl = _nameTextBox;
                return false;
            }
            return true;
        }
    }
}
