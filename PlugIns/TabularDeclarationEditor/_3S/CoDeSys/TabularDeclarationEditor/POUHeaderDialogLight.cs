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
	public class POUHeaderDialogLight : Form, IPOUHeaderDialog
	{
		private TabularDeclarationModel _model;

		private int _nProjectHandle;

		private Guid _objectGuid;

		private bool _bReadOnly;

		private string _stChangedKind;

		private string _stChangedName;

		private string _stChangedReturnType;

		private string _stChangedComment;

		private IContainer components;

		private Button _returnTypeButton;

		private Label _returnTypeLabel;

		private TextBox _returnTypeTextBox;

		private ComboBox _kindComboBox;

		private TextBox _nameTextBox;

		private Button _cancelButton;

		private Button _okButton;

		private TextBox _commentTextBox;

		private GroupBox _commentGroupBox;

		private CheckBox _renameWithRefactoringCheckBox;

		public string ChangedKind => _stChangedKind;

		public string ChangedName => _stChangedName;

		public string ChangedReturnType => _stChangedReturnType;

		public string ChangedComment => _stChangedComment;

		public string ChangedExtends => null;

		public string ChangedImplements => null;

		public string ChangedAttributes => null;

		public bool RenameWithRefactoring => _renameWithRefactoringCheckBox.Checked;

		public POUHeaderDialogLight()
		{
			InitializeComponent();
			_kindComboBox.Items.Add("PROGRAM");
			_kindComboBox.Items.Add("FUNCTION_BLOCK");
			_kindComboBox.Items.Add("FUNCTION");
		}

		public void Initialize(TabularDeclarationModel model, int nProjectHandle, Guid objectGuid, bool bReadOnly)
		{
			_model = model;
			_nProjectHandle = nProjectHandle;
			_objectGuid = objectGuid;
			_bReadOnly = bReadOnly;
			_kindComboBox.SelectedItem = _model.Header.Kind;
			_nameTextBox.Text = _model.Header.Name;
			if (_model.Header.Kind == "FUNCTION")
			{
				_returnTypeTextBox.Text = _model.Header.ReturnType;
			}
			_commentTextBox.Text = _model.Header.Comment;
			_renameWithRefactoringCheckBox.Checked = UserOptions.RenameWithRefactoring;
			UpdateControlStates();
		}

		private void UpdateControlStates()
		{
			if ((string)_kindComboBox.SelectedItem == "FUNCTION")
			{
				_returnTypeLabel.Visible = true;
				_returnTypeLabel.Text = Resources.ReturnType;
				_returnTypeTextBox.Visible = true;
				_returnTypeButton.Visible = true;
			}
			else
			{
				_returnTypeLabel.Visible = false;
				_returnTypeTextBox.Visible = false;
				_returnTypeButton.Visible = false;
			}
			_kindComboBox.Enabled = !_bReadOnly;
			_nameTextBox.ReadOnly = _bReadOnly;
			_returnTypeTextBox.ReadOnly = _bReadOnly;
			_returnTypeButton.Enabled = !_bReadOnly;
			_commentTextBox.ReadOnly = _bReadOnly;
			_okButton.Enabled = !_bReadOnly;
		}

		private void _kindComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
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

		private void _okButton_Click(object sender, EventArgs e)
		{
			if (!Common.CheckIdentifier(_nameTextBox.Text.Trim()))
			{
				APEnvironment.Engine.MessageService.Error(Resources.NameMustBeIdentifier);
				_nameTextBox.Focus();
				base.DialogResult = DialogResult.None;
				return;
			}
			if ((string)_kindComboBox.SelectedItem == "FUNCTION")
			{
				string text = _returnTypeTextBox.Text.Trim();
				if (text.Length == 0)
				{
					APEnvironment.Engine.MessageService.Error(Resources.EmptyReturnType);
					_returnTypeTextBox.Focus();
					base.DialogResult = DialogResult.None;
					return;
				}
				if (!Common.CheckType(text))
				{
					APEnvironment.Engine.MessageService.Error(Resources.InvalidDataType);
					_returnTypeTextBox.Focus();
					base.DialogResult = DialogResult.None;
					return;
				}
				foreach (TabularDeclarationBlock block in _model.List.Blocks)
				{
					if (block.Items.Count > 0 && block.Scope == ModelTokenType.VarTemp)
					{
						APEnvironment.Engine.MessageService.Error(Resources.CannotSwitchToFunctionVarTemp);
						_kindComboBox.Focus();
						base.DialogResult = DialogResult.None;
						return;
					}
				}
			}
			else if ((string)_kindComboBox.SelectedItem == "PROGRAM")
			{
				foreach (TabularDeclarationBlock block2 in _model.List.Blocks)
				{
					if (block2.Items.Count > 0 && block2.Scope == ModelTokenType.VarStat)
					{
						APEnvironment.Engine.MessageService.Error(Resources.CannotSwitchToProgramVarStat);
						_kindComboBox.Focus();
						base.DialogResult = DialogResult.None;
						return;
					}
				}
			}
			if ((string)_kindComboBox.SelectedItem != _model.Header.Kind)
			{
				_stChangedKind = (string)_kindComboBox.SelectedItem;
			}
			if (_nameTextBox.Text != _model.Header.Name)
			{
				_stChangedName = _nameTextBox.Text;
			}
			if ((string)_kindComboBox.SelectedItem == "PROGRAM")
			{
				if (!string.IsNullOrEmpty(_model.Header.ReturnType))
				{
					_stChangedReturnType = string.Empty;
				}
			}
			else if ((string)_kindComboBox.SelectedItem == "FUNCTION_BLOCK")
			{
				if (!string.IsNullOrEmpty(_model.Header.ReturnType))
				{
					_stChangedReturnType = string.Empty;
				}
			}
			else if ((string)_kindComboBox.SelectedItem == "FUNCTION" && _returnTypeTextBox.Text != _model.Header.ReturnType)
			{
				_stChangedReturnType = _returnTypeTextBox.Text;
			}
			if (_commentTextBox.Text != _model.Header.Comment)
			{
				_stChangedComment = _commentTextBox.Text;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.POUHeaderDialogLight));
			_returnTypeButton = new System.Windows.Forms.Button();
			_returnTypeLabel = new System.Windows.Forms.Label();
			_returnTypeTextBox = new System.Windows.Forms.TextBox();
			_kindComboBox = new System.Windows.Forms.ComboBox();
			_nameTextBox = new System.Windows.Forms.TextBox();
			_cancelButton = new System.Windows.Forms.Button();
			_okButton = new System.Windows.Forms.Button();
			_commentTextBox = new System.Windows.Forms.TextBox();
			_commentGroupBox = new System.Windows.Forms.GroupBox();
			_renameWithRefactoringCheckBox = new System.Windows.Forms.CheckBox();
			System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
			groupBox.SuspendLayout();
			_commentGroupBox.SuspendLayout();
			SuspendLayout();
			groupBox.Controls.Add(_returnTypeButton);
			groupBox.Controls.Add(_returnTypeLabel);
			groupBox.Controls.Add(_returnTypeTextBox);
			groupBox.Controls.Add(_kindComboBox);
			groupBox.Controls.Add(_nameTextBox);
			resources.ApplyResources(groupBox, "groupBox1");
			groupBox.Name = "groupBox1";
			groupBox.TabStop = false;
			resources.ApplyResources(_returnTypeButton, "_returnTypeButton");
			_returnTypeButton.Name = "_returnTypeButton";
			_returnTypeButton.UseVisualStyleBackColor = true;
			_returnTypeButton.Click += new System.EventHandler(_returnTypeButton_Click);
			resources.ApplyResources(_returnTypeLabel, "_returnTypeLabel");
			_returnTypeLabel.Name = "_returnTypeLabel";
			resources.ApplyResources(_returnTypeTextBox, "_returnTypeTextBox");
			_returnTypeTextBox.Name = "_returnTypeTextBox";
			_kindComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_kindComboBox.FormattingEnabled = true;
			resources.ApplyResources(_kindComboBox, "_kindComboBox");
			_kindComboBox.Name = "_kindComboBox";
			_kindComboBox.SelectedIndexChanged += new System.EventHandler(_kindComboBox_SelectedIndexChanged);
			resources.ApplyResources(_nameTextBox, "_nameTextBox");
			_nameTextBox.Name = "_nameTextBox";
			_nameTextBox.TextChanged += new System.EventHandler(NameTextBoxTextChanged);
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
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "POUHeaderDialogLight";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			groupBox.ResumeLayout(false);
			groupBox.PerformLayout();
			_commentGroupBox.ResumeLayout(false);
			_commentGroupBox.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		DialogResult IPOUHeaderDialog.ShowDialog(IWin32Window owner)
		{
			return ShowDialog(owner);
		}
	}
}
