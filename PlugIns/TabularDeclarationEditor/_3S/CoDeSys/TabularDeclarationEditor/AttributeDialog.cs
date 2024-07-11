#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_edit_declaration_header.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/core_vardeclobject_editor_home.htm")]
	[AssociatedOnlineHelpTopic("core.VarDeclObject.Editor.chm::/home.htm")]
	public class AttributeDialog : Form
	{
		private IContainer components;

		private Button _cancelButton;

		private Button _okButton;

		private TextBox _textBox;

		internal string Result => _textBox.Text;

		public AttributeDialog()
		{
			InitializeComponent();
		}

		internal void Initialize(string attributes, bool bReadOnly)
		{
			_textBox.Text = attributes;
			_textBox.ReadOnly = bReadOnly;
			_okButton.Enabled = !bReadOnly;
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Invalid comparison between Unknown and I4
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Invalid comparison between Unknown and I4
			LStringBuilder val = new LStringBuilder();
			string[] array = _textBox.Text.Split('\r', '\n');
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text.Trim()))
				{
					val.AppendLine("{" + text + "}");
				}
			}
			IScanner val2 = APEnvironment.LanguageModelMgr.CreateScanner(((object)val).ToString(), true, false, true, false);
			Debug.Assert(val2 != null);
			IToken val3 = default(IToken);
			while ((int)val2.GetNext(out val3) != 21)
			{
				if ((int)val3.Type != 4)
				{
					APEnvironment.Engine.MessageService.Error(string.Format(Resources.InvalidTokenInAttributes, val2.GetTokenText(val3)));
					base.DialogResult = DialogResult.None;
					break;
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.AttributeDialog));
			_cancelButton = new System.Windows.Forms.Button();
			_okButton = new System.Windows.Forms.Button();
			_textBox = new System.Windows.Forms.TextBox();
			System.Windows.Forms.Label label = new System.Windows.Forms.Label();
			SuspendLayout();
			resources.ApplyResources(label, "label1");
			label.Name = "label1";
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.Name = "_cancelButton";
			_cancelButton.UseVisualStyleBackColor = true;
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(_okButton, "_okButton");
			_okButton.Name = "_okButton";
			_okButton.UseVisualStyleBackColor = true;
			_okButton.Click += new System.EventHandler(_okButton_Click);
			_textBox.AcceptsReturn = true;
			resources.ApplyResources(_textBox, "_textBox");
			_textBox.Name = "_textBox";
			base.AcceptButton = _okButton;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = _cancelButton;
			base.Controls.Add(_textBox);
			base.Controls.Add(label);
			base.Controls.Add(_okButton);
			base.Controls.Add(_cancelButton);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AttributeDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
