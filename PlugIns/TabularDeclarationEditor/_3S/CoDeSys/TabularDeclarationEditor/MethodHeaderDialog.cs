#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_edit_declaration_header.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/core_vardeclobject_editor_home.htm")]
	[AssociatedOnlineHelpTopic("core.VarDeclObject.Editor.chm::/home.htm")]
	public class MethodHeaderDialog : Form
	{
		private TabularDeclarationModel _model;

		private int _nProjectHandle;

		private Guid _objectGuid;

		private bool _bReadOnly;

		private string _stChangedName;

		private string _stChangedReturnType;

		private string _stChangedComment;

		private string _stChangedAttributes;

		private string _stAttributes;

		private IContainer components;

		private Label _methodLabel;

		private Button _returnTypeButton;

		private Label _returnTypeLabel;

		private TextBox _returnTypeTextBox;

		private TextBox _nameTextBox;

		private GroupBox _commentGroupBox;

		private TextBox _commentTextBox;

		private Button _okButton;

		private Button _cancelButton;

		private Button _attributesButton;

		private CheckBox _renameWithRefactoringCheckBox;

		public string ChangedName => _stChangedName;

		public string ChangedReturnType => _stChangedReturnType;

		public string ChangedComment => _stChangedComment;

		public string ChangedAttributes => _stChangedAttributes;

		public bool RenameWithRefactoring => _renameWithRefactoringCheckBox.Checked;

		public MethodHeaderDialog()
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
			_returnTypeTextBox.Text = _model.Header.ReturnType;
			_commentTextBox.Text = _model.Header.Comment;
			_stAttributes = _model.Header.Attributes;
			_renameWithRefactoringCheckBox.Checked = UserOptions.RenameWithRefactoring;
			UpdateControlStates();
		}

		private void UpdateControlStates()
		{
			_nameTextBox.ReadOnly = _bReadOnly;
			_returnTypeTextBox.ReadOnly = _bReadOnly;
			_returnTypeButton.Enabled = !_bReadOnly;
			_commentTextBox.ReadOnly = _bReadOnly;
			_okButton.Enabled = !_bReadOnly;
		}

		private void _returnTypeButton_Click(object sender, EventArgs e)
		{
			try
			{
				IInputAssistantService obj = APEnvironment.CreateInputAssistantService();
				Debug.Assert(obj != null);
				IInputAssistantCategory[] array = (IInputAssistantCategory[])(object)new IInputAssistantCategory[2]
				{
					(IInputAssistantCategory)APEnvironment.CreateStandardTypesInputAssistantCategory(),
					(IInputAssistantCategory)APEnvironment.CreateStructuredTypesInputAssistantCategory()
				};
				int num = -1;
				Guid applicationGuid = Common.GetApplicationGuid(_nProjectHandle, _objectGuid);
				if (applicationGuid != Guid.Empty)
				{
					num = APEnvironment.Engine.Projects.PrimaryProject.Handle;
				}
				string text = obj.Invoke(array, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, num, applicationGuid, (IWin32Window)this);
				if (text != null)
				{
					_returnTypeTextBox.Text = text;
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
			string text = _returnTypeTextBox.Text.Trim();
			if (text.Length > 0 && !Common.CheckType(text))
			{
				APEnvironment.Engine.MessageService.Error(Resources.InvalidDataType);
				_returnTypeTextBox.Focus();
				base.DialogResult = DialogResult.None;
				return;
			}
			if (_nameTextBox.Text != _model.Header.Name)
			{
				_stChangedName = _nameTextBox.Text;
			}
			if (_returnTypeTextBox.Text != _model.Header.ReturnType)
			{
				_stChangedReturnType = _returnTypeTextBox.Text;
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

		private void NameTextBoxTextChanged(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.MethodHeaderDialog));
			_methodLabel = new System.Windows.Forms.Label();
			_returnTypeButton = new System.Windows.Forms.Button();
			_returnTypeLabel = new System.Windows.Forms.Label();
			_returnTypeTextBox = new System.Windows.Forms.TextBox();
			_nameTextBox = new System.Windows.Forms.TextBox();
			_commentGroupBox = new System.Windows.Forms.GroupBox();
			_commentTextBox = new System.Windows.Forms.TextBox();
			_okButton = new System.Windows.Forms.Button();
			_cancelButton = new System.Windows.Forms.Button();
			_attributesButton = new System.Windows.Forms.Button();
			_renameWithRefactoringCheckBox = new System.Windows.Forms.CheckBox();
			System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
			groupBox.SuspendLayout();
			_commentGroupBox.SuspendLayout();
			SuspendLayout();
			groupBox.Controls.Add(_methodLabel);
			groupBox.Controls.Add(_returnTypeButton);
			groupBox.Controls.Add(_returnTypeLabel);
			groupBox.Controls.Add(_returnTypeTextBox);
			groupBox.Controls.Add(_nameTextBox);
			resources.ApplyResources(groupBox, "groupBox1");
			groupBox.Name = "groupBox1";
			groupBox.TabStop = false;
			resources.ApplyResources(_methodLabel, "_methodLabel");
			_methodLabel.Name = "_methodLabel";
			resources.ApplyResources(_returnTypeButton, "_returnTypeButton");
			_returnTypeButton.Name = "_returnTypeButton";
			_returnTypeButton.UseVisualStyleBackColor = true;
			_returnTypeButton.Click += new System.EventHandler(_returnTypeButton_Click);
			resources.ApplyResources(_returnTypeLabel, "_returnTypeLabel");
			_returnTypeLabel.Name = "_returnTypeLabel";
			resources.ApplyResources(_returnTypeTextBox, "_returnTypeTextBox");
			_returnTypeTextBox.Name = "_returnTypeTextBox";
			resources.ApplyResources(_nameTextBox, "_nameTextBox");
			_nameTextBox.Name = "_nameTextBox";
			_nameTextBox.TextChanged += new System.EventHandler(NameTextBoxTextChanged);
			_commentGroupBox.Controls.Add(_commentTextBox);
			resources.ApplyResources(_commentGroupBox, "_commentGroupBox");
			_commentGroupBox.Name = "_commentGroupBox";
			_commentGroupBox.TabStop = false;
			_commentTextBox.AcceptsReturn = true;
			_commentTextBox.AcceptsTab = true;
			resources.ApplyResources(_commentTextBox, "_commentTextBox");
			_commentTextBox.Name = "_commentTextBox";
			resources.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.UseVisualStyleBackColor = true;
			_okButton.Click += new System.EventHandler(_okButton_Click);
			resources.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			_cancelButton.UseVisualStyleBackColor = true;
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
			base.Controls.Add(_commentGroupBox);
			base.Controls.Add(_okButton);
			base.Controls.Add(_cancelButton);
			base.Controls.Add(_attributesButton);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MethodHeaderDialog";
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
