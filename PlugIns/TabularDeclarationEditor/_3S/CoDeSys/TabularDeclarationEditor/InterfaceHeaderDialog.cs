#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_edit_declaration_header.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/core_vardeclobject_editor_home.htm")]
	[AssociatedOnlineHelpTopic("core.VarDeclObject.Editor.chm::/home.htm")]
	public class InterfaceHeaderDialog : Form
	{
		private TabularDeclarationModel _model;

		private int _nProjectHandle;

		private Guid _objectGuid;

		private bool _bReadOnly;

		private string _stChangedName;

		private string _stChangedExtends;

		private string _stChangedComment;

		private string _stChangedAttributes;

		private string _stAttributes;

		private IContainer components;

		private Label _interfaceLabel;

		private Button _extendsButton;

		private Label _extendsLabel;

		private TextBox _extendsTextBox;

		private TextBox _nameTextBox;

		private Button _cancelButton;

		private Button _okButton;

		private TextBox _commentTextBox;

		private GroupBox _commentGroupBox;

		private Button _attributesButton;

		private CheckBox _renameWithRefactoringCheckBox;

		internal string ChangedName => _stChangedName;

		internal string ChangedExtends => _stChangedExtends;

		internal string ChangedComment => _stChangedComment;

		internal string ChangedAttributes => _stChangedAttributes;

		public bool RenameWithRefactoring => _renameWithRefactoringCheckBox.Checked;

		public InterfaceHeaderDialog()
		{
			InitializeComponent();
		}

		internal void Initialize(TabularDeclarationModel model, int nProjectHandle, Guid objectGuid, bool bReadOnly)
		{
			_model = model;
			_nProjectHandle = nProjectHandle;
			_objectGuid = objectGuid;
			_bReadOnly = bReadOnly;
			_nameTextBox.Text = _model.Header.Name;
			_extendsTextBox.Text = _model.Header.Extends;
			_commentTextBox.Text = _model.Header.Comment;
			_stAttributes = _model.Header.Attributes;
			_renameWithRefactoringCheckBox.Checked = UserOptions.RenameWithRefactoring;
			UpdateControlStates();
		}

		private void UpdateControlStates()
		{
			_nameTextBox.ReadOnly = _bReadOnly;
			_extendsTextBox.ReadOnly = _bReadOnly;
			_extendsButton.Enabled = !_bReadOnly;
			_commentTextBox.ReadOnly = _bReadOnly;
			_okButton.Enabled = !_bReadOnly;
		}

		private void _extendsButton_Click(object sender, EventArgs e)
		{
			try
			{
				IInputAssistantService val = APEnvironment.CreateInputAssistantService();
				Debug.Assert(val != null);
				IInputAssistantCategory[] array = (IInputAssistantCategory[])(object)new IInputAssistantCategory[1];
				IStructuredTypesInputAssistantCategory val2 = APEnvironment.CreateStructuredTypesInputAssistantCategory();
				val2.POUTypes=((Operator[])(object)new Operator[1] { (Operator)119 });
				((IInputAssistantCategory)val2).Name=(Resources.Interfaces);
				array[0] = (IInputAssistantCategory)(object)val2;
				int num = -1;
				Guid applicationGuid = Common.GetApplicationGuid(_nProjectHandle, _objectGuid);
				if (applicationGuid != Guid.Empty)
				{
					num = APEnvironment.Engine.Projects.PrimaryProject.Handle;
				}
				string text = val.Invoke(array, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, num, applicationGuid, (IWin32Window)this);
				if (text != null)
				{
					_extendsTextBox.Text = text;
				}
			}
			catch
			{
			}
		}

		private void _attributesButton_Click(object sender, EventArgs e)
		{
			AttributeDialog attributeDialog = new AttributeDialog();
			attributeDialog.Initialize(_stAttributes, _bReadOnly);
			if (attributeDialog.ShowDialog(this) == DialogResult.OK)
			{
				_stAttributes = attributeDialog.Result;
			}
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			if (!Common.CheckIdentifier(_nameTextBox.Text.Trim()))
			{
				APEnvironment.Engine.MessageService.Error(Resources.NameMustBeIdentifier);
				_nameTextBox.Focus();
				base.DialogResult = DialogResult.None;
				return;
			}
			string text = _extendsTextBox.Text.Trim();
			if (text.Length > 0 && !Common.CheckQualifiedIdentifierList(text))
			{
				APEnvironment.Engine.MessageService.Error(Resources.ExtendsMustBeQualifiedIdentifierList);
				_extendsTextBox.Focus();
				base.DialogResult = DialogResult.None;
				return;
			}
			if (_nameTextBox.Text != _model.Header.Name)
			{
				_stChangedName = _nameTextBox.Text;
			}
			if (_extendsTextBox.Text != _model.Header.Extends)
			{
				_stChangedExtends = _extendsTextBox.Text;
			}
			if (_commentTextBox.Text != _model.Header.Comment)
			{
				_stChangedComment = _commentTextBox.Text;
			}
			if (_stAttributes != _model.Header.Attributes)
			{
				_stChangedAttributes = _stAttributes;
			}
		}

		private void RenameWithRefactoringCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			UserOptions.RenameWithRefactoring = _renameWithRefactoringCheckBox.Checked;
		}

		private void _nameTextBox_TextChanged(object sender, EventArgs e)
		{
			_renameWithRefactoringCheckBox.Enabled = _nameTextBox.Text != _model.Header.Name;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.InterfaceHeaderDialog));
			_interfaceLabel = new System.Windows.Forms.Label();
			_extendsButton = new System.Windows.Forms.Button();
			_extendsLabel = new System.Windows.Forms.Label();
			_extendsTextBox = new System.Windows.Forms.TextBox();
			_nameTextBox = new System.Windows.Forms.TextBox();
			_cancelButton = new System.Windows.Forms.Button();
			_okButton = new System.Windows.Forms.Button();
			_commentTextBox = new System.Windows.Forms.TextBox();
			_commentGroupBox = new System.Windows.Forms.GroupBox();
			_attributesButton = new System.Windows.Forms.Button();
			_renameWithRefactoringCheckBox = new System.Windows.Forms.CheckBox();
			System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
			groupBox.SuspendLayout();
			_commentGroupBox.SuspendLayout();
			SuspendLayout();
			groupBox.Controls.Add(_interfaceLabel);
			groupBox.Controls.Add(_extendsButton);
			groupBox.Controls.Add(_extendsLabel);
			groupBox.Controls.Add(_extendsTextBox);
			groupBox.Controls.Add(_nameTextBox);
			resources.ApplyResources(groupBox, "groupBox1");
			groupBox.Name = "groupBox1";
			groupBox.TabStop = false;
			resources.ApplyResources(_interfaceLabel, "_interfaceLabel");
			_interfaceLabel.Name = "_interfaceLabel";
			resources.ApplyResources(_extendsButton, "_extendsButton");
			_extendsButton.Name = "_extendsButton";
			_extendsButton.UseVisualStyleBackColor = true;
			_extendsButton.Click += new System.EventHandler(_extendsButton_Click);
			resources.ApplyResources(_extendsLabel, "_extendsLabel");
			_extendsLabel.Name = "_extendsLabel";
			resources.ApplyResources(_extendsTextBox, "_extendsTextBox");
			_extendsTextBox.Name = "_extendsTextBox";
			resources.ApplyResources(_nameTextBox, "_nameTextBox");
			_nameTextBox.Name = "_nameTextBox";
			_nameTextBox.TextChanged += new System.EventHandler(_nameTextBox_TextChanged);
			resources.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			_cancelButton.UseVisualStyleBackColor = true;
			resources.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.UseVisualStyleBackColor = true;
			_okButton.Click += new System.EventHandler(_okButton_Click);
			_commentTextBox.AcceptsReturn = true;
			_commentTextBox.AcceptsTab = true;
			resources.ApplyResources(_commentTextBox, "_commentTextBox");
			_commentTextBox.Name = "_commentTextBox";
			_commentGroupBox.Controls.Add(_commentTextBox);
			resources.ApplyResources(_commentGroupBox, "_commentGroupBox");
			_commentGroupBox.Name = "_commentGroupBox";
			_commentGroupBox.TabStop = false;
			resources.ApplyResources(_attributesButton, "_attributesButton");
			_attributesButton.Name = "_attributesButton";
			_attributesButton.UseVisualStyleBackColor = true;
			_attributesButton.Click += new System.EventHandler(_attributesButton_Click);
			resources.ApplyResources(_renameWithRefactoringCheckBox, "_renameWithRefactoringCheckBox");
			_renameWithRefactoringCheckBox.Name = "_renameWithRefactoringCheckBox";
			_renameWithRefactoringCheckBox.UseVisualStyleBackColor = true;
			_renameWithRefactoringCheckBox.CheckedChanged += new System.EventHandler(RenameWithRefactoringCheckBoxCheckedChanged);
			base.AcceptButton = _okButton;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = _cancelButton;
			base.Controls.Add(_renameWithRefactoringCheckBox);
			base.Controls.Add(groupBox);
			base.Controls.Add(_cancelButton);
			base.Controls.Add(_okButton);
			base.Controls.Add(_commentGroupBox);
			base.Controls.Add(_attributesButton);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "InterfaceHeaderDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			groupBox.ResumeLayout(false);
			groupBox.PerformLayout();
			_commentGroupBox.ResumeLayout(false);
			_commentGroupBox.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
