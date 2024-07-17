using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_dlg_properties_options_controller.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/interactive_login.htm")]
    public class InteractiveLoginForm : Form
    {
        private Timer _timerProgress;

        private int _nTimeoutSeconds = -1;

        private bool _bCancelled;

        private string _stID = string.Empty;

        private InteractiveLoginMode _mode;

        private bool _bHandleDestroyed;

        private IContainer components;

        private TextBox _tbID;

        private Label _lbMessage;

        private ProgressBar _progress;

        private Button _btnCancel;

        private Button _btnOK;

        private TextBox _tbDeviceName;

        private Label label1;

        private ErrorProvider _error;

        private Label _lbConfirmation;

        private Label _lbProgressLabel;

        private PictureBox _pictureBox;

        internal string ID => _stID;

        internal bool IsCancelledByUser => _bCancelled;

        internal bool IsHandleDestroyed => _bHandleDestroyed;

        public InteractiveLoginForm(InteractiveLoginMode mode, int nTimeoutSeconds, string stDescription, string stDeviceName)
        {
            //IL_0065: Unknown result type (might be due to invalid IL or missing references)
            //IL_0066: Unknown result type (might be due to invalid IL or missing references)
            //IL_0078: Unknown result type (might be due to invalid IL or missing references)
            //IL_007e: Invalid comparison between Unknown and I4
            InitializeComponent();
            _pictureBox.Image = Bitmap.FromHicon(SystemIcons.Information.Handle);
            _progress.Style = ProgressBarStyle.Continuous;
            _nTimeoutSeconds = nTimeoutSeconds;
            _lbMessage.Text = stDescription;
            _tbDeviceName.Text = stDeviceName;
            _mode = mode;
            _btnOK.Enabled = false;
            if ((int)_mode == 1)
            {
                _lbProgressLabel.Text = Strings.InteractiveLoginEnterID;
                _tbID.Width = _lbMessage.Width;
                _progress.Visible = false;
            }
            else
            {
                _progress.Location = _tbID.Location;
                _progress.Width = _lbMessage.Width;
                _progress.Visible = true;
                _tbID.Visible = false;
                if (nTimeoutSeconds < 10 || nTimeoutSeconds > 1800)
                {
                    _progress.Visible = false;
                    _lbProgressLabel.Visible = false;
                }
                else
                {
                    _lbProgressLabel.Text = string.Format(Strings.InteractiveLoginTimeout, nTimeoutSeconds.ToString());
                    _progress.Maximum = nTimeoutSeconds;
                    _progress.Step = 1;
                }
            }
            if (_lbMessage.Size.Height > _lbMessage.MinimumSize.Height)
            {
                base.Size = new Size(base.Size.Width, base.Size.Height + _lbMessage.Height - _lbMessage.MinimumSize.Height);
            }
        }

        internal void ConfirmLogin(bool bSuccess, Exception ex)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Invalid comparison between Unknown and I4
            if ((int)_mode != 2)
            {
                return;
            }
            if (bSuccess)
            {
                _lbConfirmation.Text = Strings.InteractiveLoginConfirmed;
                _lbConfirmation.Visible = true;
                _btnOK.Enabled = true;
                _progress.Value = _progress.Maximum;
                _progress.Enabled = false;
                return;
            }
            if (ex != null)
            {
                _lbConfirmation.ForeColor = Color.Red;
                _lbConfirmation.Text = ex.Message;
                _lbConfirmation.Visible = true;
            }
            _btnCancel.Enabled = true;
            _btnOK.Enabled = false;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            //IL_0008: Unknown result type (might be due to invalid IL or missing references)
            //IL_000e: Invalid comparison between Unknown and I4
            base.OnHandleCreated(e);
            if ((int)_mode == 2)
            {
                _timerProgress = new Timer();
                _timerProgress.Interval = 1000;
                _timerProgress.Tick += _timer_Tick;
                _timerProgress.Start();
            }
            base.FormClosing += InteractiveLoginForm_FormClosing;
        }

        private void InteractiveLoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && base.DialogResult != DialogResult.OK)
            {
                _bCancelled = true;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.FormClosing -= InteractiveLoginForm_FormClosing;
            if (_timerProgress != null)
            {
                _timerProgress.Stop();
                _timerProgress.Dispose();
                _timerProgress = null;
            }
            base.OnHandleDestroyed(e);
            _bHandleDestroyed = true;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_progress.Visible && _progress.Value < _nTimeoutSeconds)
                {
                    _progress.PerformStep();
                }
            }
            catch
            {
            }
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            _bCancelled = true;
            _btnCancel.Enabled = false;
            _btnOK.Enabled = false;
            _tbID.Enabled = false;
            Close();
        }

        private void _tbID_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_tbID.Text) && _tbID.Text.Length < 256)
            {
                _stID = _tbID.Text;
                _error.SetError(_tbID, string.Empty);
                _btnOK.Enabled = true;
            }
            else
            {
                _error.SetError(_tbID, Strings.InvalidValue);
                _btnOK.Enabled = false;
            }
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            Close();
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.InteractiveLoginForm));
            _tbID = new System.Windows.Forms.TextBox();
            _lbMessage = new System.Windows.Forms.Label();
            _progress = new System.Windows.Forms.ProgressBar();
            _btnCancel = new System.Windows.Forms.Button();
            _btnOK = new System.Windows.Forms.Button();
            _tbDeviceName = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            _error = new System.Windows.Forms.ErrorProvider(components);
            _lbConfirmation = new System.Windows.Forms.Label();
            _lbProgressLabel = new System.Windows.Forms.Label();
            _pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)_error).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
            SuspendLayout();
            resources.ApplyResources(_tbID, "_tbID");
            _tbID.Name = "_tbID";
            _tbID.TextChanged += new System.EventHandler(_tbID_TextChanged);
            resources.ApplyResources(_lbMessage, "_lbMessage");
            _lbMessage.BackColor = System.Drawing.SystemColors.Control;
            _lbMessage.Name = "_lbMessage";
            resources.ApplyResources(_progress, "_progress");
            _progress.Name = "_progress";
            resources.ApplyResources(_btnCancel, "_btnCancel");
            _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _btnCancel.Name = "_btnCancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new System.EventHandler(_btnCancel_Click);
            resources.ApplyResources(_btnOK, "_btnOK");
            _btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            _btnOK.Name = "_btnOK";
            _btnOK.UseVisualStyleBackColor = true;
            _btnOK.Click += new System.EventHandler(_btnOK_Click);
            resources.ApplyResources(_tbDeviceName, "_tbDeviceName");
            _tbDeviceName.BackColor = System.Drawing.SystemColors.Control;
            _tbDeviceName.Name = "_tbDeviceName";
            _tbDeviceName.ReadOnly = true;
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            _error.ContainerControl = this;
            resources.ApplyResources(_lbConfirmation, "_lbConfirmation");
            _lbConfirmation.ForeColor = System.Drawing.Color.Blue;
            _lbConfirmation.Name = "_lbConfirmation";
            resources.ApplyResources(_lbProgressLabel, "_lbProgressLabel");
            _lbProgressLabel.Name = "_lbProgressLabel";
            resources.ApplyResources(_pictureBox, "_pictureBox");
            _pictureBox.Name = "_pictureBox";
            _pictureBox.TabStop = false;
            base.AcceptButton = _btnOK;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.Control;
            base.CancelButton = _btnCancel;
            base.Controls.Add(_pictureBox);
            base.Controls.Add(_lbProgressLabel);
            base.Controls.Add(_lbConfirmation);
            base.Controls.Add(_tbDeviceName);
            base.Controls.Add(label1);
            base.Controls.Add(_btnOK);
            base.Controls.Add(_btnCancel);
            base.Controls.Add(_progress);
            base.Controls.Add(_tbID);
            base.Controls.Add(_lbMessage);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "InteractiveLoginForm";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)_error).EndInit();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
