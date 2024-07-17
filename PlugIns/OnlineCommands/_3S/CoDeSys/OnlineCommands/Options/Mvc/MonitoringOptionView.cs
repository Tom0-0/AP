using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands.Options.Mvc
{
    public class MonitoringOptionView : UserControl, IMonitoringOptionModelListener
    {
        private IMonitoringOptionViewListener _viewListener;

        private bool _bFromController;

        private TextBox _tbDecimalPlaces;

        private IContainer components;

        private GroupBox _gbInteger;

        private RadioButton _rbHexadecimal;

        private RadioButton _rbDecimal;

        private RadioButton _rbBinary;

        private GroupBox _gbFloatingPoint;

        private Label _lblDecimalPlaces;

        private ErrorProvider _errorProvider;

        private Label _lblDigits;

        private NumericUpDown _nudDecimalPlaces;

        private Label _iconLabel;

        private ToolTip _toolTip1;

        public MonitoringOptionView()
        {
            InitializeComponent();
            if (2 <= _nudDecimalPlaces.Controls.Count)
            {
                _tbDecimalPlaces = _nudDecimalPlaces.Controls[1] as TextBox;
                if (_tbDecimalPlaces != null)
                {
                    _tbDecimalPlaces.TextChanged += _tbDecimalPlaces_TextChanged;
                }
            }
            _viewListener = new MonitoringOptionController();
            _viewListener.Initialize(this);
        }

        internal bool Save(ref string stMessage, ref Control failedControl)
        {
            EEditorPosition eFailedControl = EEditorPosition.Undefined;
            bool flag = _viewListener.Save(ref stMessage, ref eFailedControl);
            if (!flag && eFailedControl == EEditorPosition.DecimalPlaces)
            {
                failedControl = _nudDecimalPlaces;
            }
            return flag;
        }

        public void ShowError(EEditorPosition editorPosition, string stErrorMsg)
        {
            if (editorPosition == EEditorPosition.DecimalPlaces)
            {
                _errorProvider.SetError(_nudDecimalPlaces, stErrorMsg);
                _errorProvider.SetIconAlignment(_nudDecimalPlaces, ErrorIconAlignment.MiddleLeft);
            }
        }

        public void ShowIntegerDisplayMode(int iDisplayMode)
        {
            _bFromController = true;
            try
            {
                _rbBinary.Checked = OptionsHelper.DISPLAYMODE_BINARY == iDisplayMode;
                _rbDecimal.Checked = OptionsHelper.DISPLAYMODE_DECIMAL == iDisplayMode;
                _rbHexadecimal.Checked = OptionsHelper.DISPLAYMODE_HEXADECIMAL == iDisplayMode;
            }
            finally
            {
                _bFromController = false;
            }
        }

        public void ShowDecimalPlaces(int? iCountDecimalPlaces)
        {
            _bFromController = true;
            try
            {
                if (iCountDecimalPlaces.HasValue)
                {
                    _tbDecimalPlaces.Text = iCountDecimalPlaces.ToString();
                }
                else
                {
                    _tbDecimalPlaces.Text = string.Empty;
                }
            }
            finally
            {
                _bFromController = false;
            }
        }

        public void SetDecimalPlacesMinMax(int iMin, int iMax)
        {
            _bFromController = true;
            try
            {
                _nudDecimalPlaces.Minimum = iMin;
                _nudDecimalPlaces.Maximum = iMax;
            }
            finally
            {
                _bFromController = false;
            }
        }

        private void OnRadioButtonChecked(RadioButton rb, int iDisplayMode)
        {
            if (!_bFromController && rb.Checked)
            {
                _viewListener.DisplayModeChanged(iDisplayMode);
            }
        }

        private void _rbBinary_CheckedChanged(object sender, EventArgs e)
        {
            OnRadioButtonChecked(_rbBinary, OptionsHelper.DISPLAYMODE_BINARY);
        }

        private void _rbDecimal_CheckedChanged(object sender, EventArgs e)
        {
            OnRadioButtonChecked(_rbDecimal, OptionsHelper.DISPLAYMODE_DECIMAL);
        }

        private void _rbHexadecimal_CheckedChanged(object sender, EventArgs e)
        {
            OnRadioButtonChecked(_rbHexadecimal, OptionsHelper.DISPLAYMODE_HEXADECIMAL);
        }

        private void _tbDecimalPlaces_TextChanged(object sender, EventArgs e)
        {
            if (_bFromController)
            {
                return;
            }
            int? iCountDecimalPlaces = null;
            if (!string.IsNullOrEmpty(_tbDecimalPlaces.Text))
            {
                int result = 0;
                if (int.TryParse(_tbDecimalPlaces.Text, out result))
                {
                    iCountDecimalPlaces = result;
                }
            }
            _viewListener.DecimalPlacesChanged(iCountDecimalPlaces);
        }

        private void _nudDecimalPlaces_ValueChanged(object sender, EventArgs e)
        {
            if (_bFromController)
            {
                return;
            }
            int? iCountDecimalPlaces = null;
            if (!string.IsNullOrEmpty(_nudDecimalPlaces.Text))
            {
                int result = 0;
                if (int.TryParse(_nudDecimalPlaces.Text, out result))
                {
                    iCountDecimalPlaces = result;
                }
            }
            _viewListener.DecimalPlacesChanged(iCountDecimalPlaces);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.Options.Mvc.MonitoringOptionView));
            _gbFloatingPoint = new System.Windows.Forms.GroupBox();
            _iconLabel = new System.Windows.Forms.Label();
            _nudDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            _lblDigits = new System.Windows.Forms.Label();
            _lblDecimalPlaces = new System.Windows.Forms.Label();
            _gbInteger = new System.Windows.Forms.GroupBox();
            _rbHexadecimal = new System.Windows.Forms.RadioButton();
            _rbDecimal = new System.Windows.Forms.RadioButton();
            _rbBinary = new System.Windows.Forms.RadioButton();
            _errorProvider = new System.Windows.Forms.ErrorProvider(components);
            _toolTip1 = new System.Windows.Forms.ToolTip(components);
            _gbFloatingPoint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudDecimalPlaces).BeginInit();
            _gbInteger.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_errorProvider).BeginInit();
            SuspendLayout();
            _gbFloatingPoint.Controls.Add(_iconLabel);
            _gbFloatingPoint.Controls.Add(_nudDecimalPlaces);
            _gbFloatingPoint.Controls.Add(_lblDigits);
            _gbFloatingPoint.Controls.Add(_lblDecimalPlaces);
            resources.ApplyResources(_gbFloatingPoint, "_gbFloatingPoint");
            _gbFloatingPoint.Name = "_gbFloatingPoint";
            _gbFloatingPoint.TabStop = false;
            resources.ApplyResources(_iconLabel, "_iconLabel");
            _iconLabel.Name = "_iconLabel";
            _toolTip1.SetToolTip(_iconLabel, resources.GetString("_iconLabel.ToolTip"));
            resources.ApplyResources(_nudDecimalPlaces, "_nudDecimalPlaces");
            _nudDecimalPlaces.Name = "_nudDecimalPlaces";
            _nudDecimalPlaces.ValueChanged += new System.EventHandler(_nudDecimalPlaces_ValueChanged);
            resources.ApplyResources(_lblDigits, "_lblDigits");
            _lblDigits.Name = "_lblDigits";
            resources.ApplyResources(_lblDecimalPlaces, "_lblDecimalPlaces");
            _lblDecimalPlaces.Name = "_lblDecimalPlaces";
            _gbInteger.Controls.Add(_rbHexadecimal);
            _gbInteger.Controls.Add(_rbDecimal);
            _gbInteger.Controls.Add(_rbBinary);
            resources.ApplyResources(_gbInteger, "_gbInteger");
            _gbInteger.Name = "_gbInteger";
            _gbInteger.TabStop = false;
            resources.ApplyResources(_rbHexadecimal, "_rbHexadecimal");
            _rbHexadecimal.Name = "_rbHexadecimal";
            _rbHexadecimal.TabStop = true;
            _rbHexadecimal.UseVisualStyleBackColor = true;
            _rbHexadecimal.CheckedChanged += new System.EventHandler(_rbHexadecimal_CheckedChanged);
            resources.ApplyResources(_rbDecimal, "_rbDecimal");
            _rbDecimal.Name = "_rbDecimal";
            _rbDecimal.TabStop = true;
            _rbDecimal.UseVisualStyleBackColor = true;
            _rbDecimal.CheckedChanged += new System.EventHandler(_rbDecimal_CheckedChanged);
            resources.ApplyResources(_rbBinary, "_rbBinary");
            _rbBinary.Name = "_rbBinary";
            _rbBinary.TabStop = true;
            _rbBinary.UseVisualStyleBackColor = true;
            _rbBinary.CheckedChanged += new System.EventHandler(_rbBinary_CheckedChanged);
            _errorProvider.ContainerControl = this;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(_gbInteger);
            base.Controls.Add(_gbFloatingPoint);
            base.Name = "MonitoringOptionView";
            _gbFloatingPoint.ResumeLayout(false);
            _gbFloatingPoint.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudDecimalPlaces).EndInit();
            _gbInteger.ResumeLayout(false);
            _gbInteger.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_errorProvider).EndInit();
            ResumeLayout(false);
        }
    }
}
