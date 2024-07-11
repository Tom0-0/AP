using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	public class OptionControl : UserControl
	{
		private IContainer components;

		private RadioButton _switchableRadioButton;

		private RadioButton _tabularOnlyRadioButton;

		private RadioButton _textualOnlyRadioButton;

		private GroupBox _defaultGroupBox;

		private RadioButton _defaultRecentGloballyRadioButton;

		private RadioButton _defaultRecentPerObjectRadioButton;

		private RadioButton _defaultTabularRadioButton;

		private RadioButton _defaultTextualRadioButton;

		public OptionControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			switch (UserOptions.DeclarationEditor)
			{
			case 0:
				_textualOnlyRadioButton.Checked = true;
				break;
			case 1:
				_tabularOnlyRadioButton.Checked = true;
				break;
			default:
				_switchableRadioButton.Checked = true;
				break;
			}
			switch (UserOptions.Default)
			{
			case 0:
				_defaultTextualRadioButton.Checked = true;
				break;
			case 1:
				_defaultTabularRadioButton.Checked = true;
				break;
			case 2:
				_defaultRecentPerObjectRadioButton.Checked = true;
				break;
			default:
				_defaultRecentGloballyRadioButton.Checked = true;
				break;
			}
		}

		private void _textualOnlyRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_textualOnlyRadioButton.Checked)
			{
				UserOptions.DeclarationEditor = 0;
				_defaultGroupBox.Visible = false;
			}
		}

		private void _tabularOnlyRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_tabularOnlyRadioButton.Checked)
			{
				UserOptions.DeclarationEditor = 1;
				_defaultGroupBox.Visible = false;
			}
		}

		private void _switchableRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_switchableRadioButton.Checked)
			{
				UserOptions.DeclarationEditor = 2;
				_defaultGroupBox.Visible = true;
			}
		}

		private void _defaultTextualRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_defaultTextualRadioButton.Checked)
			{
				UserOptions.Default = 0;
			}
		}

		private void _defaultTabularRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_defaultTabularRadioButton.Checked)
			{
				UserOptions.Default = 1;
			}
		}

		private void _defaultRecentPerObjectRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_defaultRecentPerObjectRadioButton.Checked)
			{
				UserOptions.Default = 2;
			}
		}

		private void _defaultRecentGloballyRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_defaultRecentGloballyRadioButton.Checked)
			{
				UserOptions.Default = 3;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.OptionControl));
			_switchableRadioButton = new System.Windows.Forms.RadioButton();
			_tabularOnlyRadioButton = new System.Windows.Forms.RadioButton();
			_textualOnlyRadioButton = new System.Windows.Forms.RadioButton();
			_defaultGroupBox = new System.Windows.Forms.GroupBox();
			_defaultRecentGloballyRadioButton = new System.Windows.Forms.RadioButton();
			_defaultRecentPerObjectRadioButton = new System.Windows.Forms.RadioButton();
			_defaultTabularRadioButton = new System.Windows.Forms.RadioButton();
			_defaultTextualRadioButton = new System.Windows.Forms.RadioButton();
			_defaultGroupBox.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_switchableRadioButton, "_switchableRadioButton");
			_switchableRadioButton.Name = "_switchableRadioButton";
			_switchableRadioButton.TabStop = true;
			_switchableRadioButton.UseVisualStyleBackColor = true;
			_switchableRadioButton.CheckedChanged += new System.EventHandler(_switchableRadioButton_CheckedChanged);
			resources.ApplyResources(_tabularOnlyRadioButton, "_tabularOnlyRadioButton");
			_tabularOnlyRadioButton.Name = "_tabularOnlyRadioButton";
			_tabularOnlyRadioButton.TabStop = true;
			_tabularOnlyRadioButton.UseVisualStyleBackColor = true;
			_tabularOnlyRadioButton.CheckedChanged += new System.EventHandler(_tabularOnlyRadioButton_CheckedChanged);
			resources.ApplyResources(_textualOnlyRadioButton, "_textualOnlyRadioButton");
			_textualOnlyRadioButton.Name = "_textualOnlyRadioButton";
			_textualOnlyRadioButton.TabStop = true;
			_textualOnlyRadioButton.UseVisualStyleBackColor = true;
			_textualOnlyRadioButton.CheckedChanged += new System.EventHandler(_textualOnlyRadioButton_CheckedChanged);
			_defaultGroupBox.Controls.Add(_defaultRecentGloballyRadioButton);
			_defaultGroupBox.Controls.Add(_defaultRecentPerObjectRadioButton);
			_defaultGroupBox.Controls.Add(_defaultTabularRadioButton);
			_defaultGroupBox.Controls.Add(_defaultTextualRadioButton);
			resources.ApplyResources(_defaultGroupBox, "_defaultGroupBox");
			_defaultGroupBox.Name = "_defaultGroupBox";
			_defaultGroupBox.TabStop = false;
			resources.ApplyResources(_defaultRecentGloballyRadioButton, "_defaultRecentGloballyRadioButton");
			_defaultRecentGloballyRadioButton.Name = "_defaultRecentGloballyRadioButton";
			_defaultRecentGloballyRadioButton.TabStop = true;
			_defaultRecentGloballyRadioButton.UseVisualStyleBackColor = true;
			_defaultRecentGloballyRadioButton.CheckedChanged += new System.EventHandler(_defaultRecentGloballyRadioButton_CheckedChanged);
			resources.ApplyResources(_defaultRecentPerObjectRadioButton, "_defaultRecentPerObjectRadioButton");
			_defaultRecentPerObjectRadioButton.Name = "_defaultRecentPerObjectRadioButton";
			_defaultRecentPerObjectRadioButton.TabStop = true;
			_defaultRecentPerObjectRadioButton.UseVisualStyleBackColor = true;
			_defaultRecentPerObjectRadioButton.CheckedChanged += new System.EventHandler(_defaultRecentPerObjectRadioButton_CheckedChanged);
			resources.ApplyResources(_defaultTabularRadioButton, "_defaultTabularRadioButton");
			_defaultTabularRadioButton.Name = "_defaultTabularRadioButton";
			_defaultTabularRadioButton.TabStop = true;
			_defaultTabularRadioButton.UseVisualStyleBackColor = true;
			_defaultTabularRadioButton.CheckedChanged += new System.EventHandler(_defaultTabularRadioButton_CheckedChanged);
			resources.ApplyResources(_defaultTextualRadioButton, "_defaultTextualRadioButton");
			_defaultTextualRadioButton.Name = "_defaultTextualRadioButton";
			_defaultTextualRadioButton.TabStop = true;
			_defaultTextualRadioButton.UseVisualStyleBackColor = true;
			_defaultTextualRadioButton.CheckedChanged += new System.EventHandler(_defaultTextualRadioButton_CheckedChanged);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(_defaultGroupBox);
			base.Controls.Add(_switchableRadioButton);
			base.Controls.Add(_tabularOnlyRadioButton);
			base.Controls.Add(_textualOnlyRadioButton);
			base.Name = "OptionControl";
			_defaultGroupBox.ResumeLayout(false);
			_defaultGroupBox.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
