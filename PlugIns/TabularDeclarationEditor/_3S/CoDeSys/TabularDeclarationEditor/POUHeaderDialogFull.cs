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
	public class POUHeaderDialogFull : Form, IPOUHeaderDialog
	{
		private TabularDeclarationModel _model;

		private int _nProjectHandle;

		private Guid _objectGuid;

		private bool _bReadOnly;

		private string _stChangedKind;

		private string _stChangedName;

		private string _stChangedReturnType;

		private string _stChangedExtends;

		private string _stChangedImplements;

		private string _stChangedComment;

		private string _stChangedAttributes;

		private string _stAttributes;

		private IContainer components;

		private ComboBox _kindComboBox;

		private TextBox _nameTextBox;

		private TextBox _implementsTextBox;

		private TextBox _extendsOrReturnTypeTextBox;

		private Label _extendsOrReturnTypeLabel;

		private Button _implementsButton;

		private Button _extendsOrReturnTypeButton;

		private Label _implementsLabel;

		private GroupBox _commentGroupBox;

		private TextBox _commentTextBox;

		private Button _cancelButton;

		private Button _okButton;

		private Button _attributesButton;

		private CheckBox _renameWithRefactoringCheckBox;

		public string ChangedKind => _stChangedKind;

		public string ChangedName => _stChangedName;

		public string ChangedReturnType => _stChangedReturnType;

		public string ChangedExtends => _stChangedExtends;

		public string ChangedImplements => _stChangedImplements;

		public string ChangedComment => _stChangedComment;

		public string ChangedAttributes => _stChangedAttributes;

		public bool RenameWithRefactoring => _renameWithRefactoringCheckBox.Checked;

		public POUHeaderDialogFull()
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
				_extendsOrReturnTypeTextBox.Text = _model.Header.ReturnType;
			}
			else
			{
				_extendsOrReturnTypeTextBox.Text = _model.Header.Extends;
			}
			_implementsTextBox.Text = _model.Header.Implements;
			_commentTextBox.Text = _model.Header.Comment;
			_stAttributes = _model.Header.Attributes;
			_renameWithRefactoringCheckBox.Checked = UserOptions.RenameWithRefactoring;
			UpdateControlStates();
		}

		private void UpdateControlStates()
		{
			if ((string)_kindComboBox.SelectedItem == "FUNCTION_BLOCK")
			{
				_extendsOrReturnTypeLabel.Visible = true;
				_extendsOrReturnTypeLabel.Text = "EXTENDS";
				_extendsOrReturnTypeTextBox.Visible = true;
				_extendsOrReturnTypeButton.Visible = true;
				_implementsLabel.Visible = true;
				_implementsTextBox.Visible = true;
				_implementsButton.Visible = true;
			}
			else if ((string)_kindComboBox.SelectedItem == "FUNCTION")
			{
				_extendsOrReturnTypeLabel.Visible = true;
				_extendsOrReturnTypeLabel.Text = Resources.ReturnType;
				_extendsOrReturnTypeTextBox.Visible = true;
				_extendsOrReturnTypeButton.Visible = true;
				_implementsLabel.Visible = false;
				_implementsTextBox.Visible = false;
				_implementsButton.Visible = false;
			}
			else
			{
				_extendsOrReturnTypeLabel.Visible = false;
				_extendsOrReturnTypeTextBox.Visible = false;
				_extendsOrReturnTypeButton.Visible = false;
				_implementsLabel.Visible = false;
				_implementsTextBox.Visible = false;
				_implementsButton.Visible = false;
			}
			_kindComboBox.Enabled = !_bReadOnly;
			_nameTextBox.ReadOnly = _bReadOnly;
			_extendsOrReturnTypeTextBox.ReadOnly = _bReadOnly;
			_extendsOrReturnTypeButton.Enabled = !_bReadOnly;
			_implementsTextBox.ReadOnly = _bReadOnly;
			_implementsButton.Enabled = !_bReadOnly;
			_commentTextBox.ReadOnly = _bReadOnly;
			_okButton.Enabled = !_bReadOnly;
		}

		private void _kindComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		private void _extendsOrReturnTypeButton_Click(object sender, EventArgs e)
		{
			if ((string)_kindComboBox.SelectedItem == "FUNCTION_BLOCK")
			{
				try
				{
					IInputAssistantService val = APEnvironment.CreateInputAssistantService();
					Debug.Assert(val != null);
					IInputAssistantCategory[] array = (IInputAssistantCategory[])(object)new IInputAssistantCategory[1];
					IStructuredTypesInputAssistantCategory val2 = APEnvironment.CreateStructuredTypesInputAssistantCategory();
					val2.POUTypes=((Operator[])(object)new Operator[1] { (Operator)88 });
					((IInputAssistantCategory2)val2).SetFlag((CategoryFlags)2, false);
					((IInputAssistantCategory)val2).Name=(Resources.FunctionBlocks);
					array[0] = (IInputAssistantCategory)(object)val2;
					int num = -1;
					Guid applicationGuid = Common.GetApplicationGuid(_nProjectHandle, _objectGuid);
					if (applicationGuid != Guid.Empty)
					{
						num = _nProjectHandle;
					}
					string text = val.Invoke(array, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, num, applicationGuid, (IWin32Window)this);
					if (text != null)
					{
						_extendsOrReturnTypeTextBox.Text = text;
					}
				}
				catch
				{
				}
			}
			else
			{
				if (!((string)_kindComboBox.SelectedItem == "FUNCTION"))
				{
					return;
				}
				try
				{
					IInputAssistantService obj2 = APEnvironment.CreateInputAssistantService();
					Debug.Assert(obj2 != null);
					IInputAssistantCategory[] array2 = (IInputAssistantCategory[])(object)new IInputAssistantCategory[2]
					{
						(IInputAssistantCategory)APEnvironment.CreateStandardTypesInputAssistantCategory(),
						(IInputAssistantCategory)APEnvironment.CreateStructuredTypesInputAssistantCategory()
					};
					int num2 = -1;
					Guid applicationGuid2 = Common.GetApplicationGuid(_nProjectHandle, _objectGuid);
					if (applicationGuid2 != Guid.Empty)
					{
						num2 = APEnvironment.Engine.Projects.PrimaryProject.Handle;
					}
					string text2 = obj2.Invoke(array2, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, (IInputAssistantArgumentsFormatter)null, num2, applicationGuid2, (IWin32Window)this);
					if (text2 != null)
					{
						_extendsOrReturnTypeTextBox.Text = text2;
					}
				}
				catch
				{
				}
			}
		}

		private void _implementsButton_Click(object sender, EventArgs e)
		{
			try
			{
				IInputAssistantService val = APEnvironment.CreateInputAssistantService();
				Debug.Assert(val != null);
				IInputAssistantCategory[] array = (IInputAssistantCategory[])(object)new IInputAssistantCategory[1];
				IStructuredTypesInputAssistantCategory val2 = APEnvironment.CreateStructuredTypesInputAssistantCategory();
				val2.POUTypes=((Operator[])(object)new Operator[1] { (Operator)119 });
				((IInputAssistantCategory2)val2).SetFlag((CategoryFlags)2, false);
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
					if (_implementsTextBox.Text.Trim().Length > 0)
					{
						_implementsTextBox.Text = _implementsTextBox.Text + ", " + text;
					}
					else
					{
						_implementsTextBox.Text = text;
					}
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
			if ((string)_kindComboBox.SelectedItem == "FUNCTION_BLOCK")
			{
				string text = _extendsOrReturnTypeTextBox.Text.Trim();
				if (text.Length > 0 && !Common.CheckQualifiedIdentifier(text))
				{
					APEnvironment.Engine.MessageService.Error(Resources.ExtendsMustBeQualifiedIdentifier);
					_extendsOrReturnTypeTextBox.Focus();
					base.DialogResult = DialogResult.None;
					return;
				}
				string text2 = _implementsTextBox.Text.Trim();
				if (text2.Length > 0 && !Common.CheckQualifiedIdentifierList(text2))
				{
					APEnvironment.Engine.MessageService.Error(Resources.ImplementsMustBeQualifiedIdentifierList);
					_implementsTextBox.Focus();
					base.DialogResult = DialogResult.None;
					return;
				}
			}
			else if ((string)_kindComboBox.SelectedItem == "FUNCTION")
			{
				string text3 = _extendsOrReturnTypeTextBox.Text.Trim();
				if (text3.Length == 0)
				{
					APEnvironment.Engine.MessageService.Error(Resources.EmptyReturnType);
					_extendsOrReturnTypeTextBox.Focus();
					base.DialogResult = DialogResult.None;
					return;
				}
				if (!Common.CheckType(text3))
				{
					APEnvironment.Engine.MessageService.Error(Resources.InvalidDataType);
					_extendsOrReturnTypeTextBox.Focus();
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
				if (!string.IsNullOrEmpty(_model.Header.Extends))
				{
					_stChangedExtends = string.Empty;
				}
				if (!string.IsNullOrEmpty(_model.Header.Implements))
				{
					_stChangedImplements = string.Empty;
				}
				if (!string.IsNullOrEmpty(_model.Header.ReturnType))
				{
					_stChangedReturnType = string.Empty;
				}
			}
			else if ((string)_kindComboBox.SelectedItem == "FUNCTION_BLOCK")
			{
				if (_extendsOrReturnTypeTextBox.Text != _model.Header.Extends)
				{
					_stChangedExtends = _extendsOrReturnTypeTextBox.Text;
				}
				if (_implementsTextBox.Text != _model.Header.Implements)
				{
					_stChangedImplements = _implementsTextBox.Text;
				}
				if (!string.IsNullOrEmpty(_model.Header.ReturnType))
				{
					_stChangedReturnType = string.Empty;
				}
			}
			else if ((string)_kindComboBox.SelectedItem == "FUNCTION")
			{
				if (!string.IsNullOrEmpty(_model.Header.Extends))
				{
					_stChangedExtends = string.Empty;
				}
				if (!string.IsNullOrEmpty(_model.Header.Implements))
				{
					_stChangedImplements = string.Empty;
				}
				if (_extendsOrReturnTypeTextBox.Text != _model.Header.ReturnType)
				{
					_stChangedReturnType = _extendsOrReturnTypeTextBox.Text;
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.TabularDeclarationEditor.POUHeaderDialogFull));
			_implementsButton = new System.Windows.Forms.Button();
			_extendsOrReturnTypeButton = new System.Windows.Forms.Button();
			_implementsLabel = new System.Windows.Forms.Label();
			_extendsOrReturnTypeLabel = new System.Windows.Forms.Label();
			_implementsTextBox = new System.Windows.Forms.TextBox();
			_extendsOrReturnTypeTextBox = new System.Windows.Forms.TextBox();
			_kindComboBox = new System.Windows.Forms.ComboBox();
			_nameTextBox = new System.Windows.Forms.TextBox();
			_commentGroupBox = new System.Windows.Forms.GroupBox();
			_commentTextBox = new System.Windows.Forms.TextBox();
			_cancelButton = new System.Windows.Forms.Button();
			_okButton = new System.Windows.Forms.Button();
			_attributesButton = new System.Windows.Forms.Button();
			_renameWithRefactoringCheckBox = new System.Windows.Forms.CheckBox();
			System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
			groupBox.SuspendLayout();
			_commentGroupBox.SuspendLayout();
			SuspendLayout();
			groupBox.Controls.Add(_implementsButton);
			groupBox.Controls.Add(_extendsOrReturnTypeButton);
			groupBox.Controls.Add(_implementsLabel);
			groupBox.Controls.Add(_extendsOrReturnTypeLabel);
			groupBox.Controls.Add(_implementsTextBox);
			groupBox.Controls.Add(_extendsOrReturnTypeTextBox);
			groupBox.Controls.Add(_kindComboBox);
			groupBox.Controls.Add(_nameTextBox);
			resources.ApplyResources(groupBox, "groupBox1");
			groupBox.Name = "groupBox1";
			groupBox.TabStop = false;
			resources.ApplyResources(_implementsButton, "_implementsButton");
			_implementsButton.Name = "_implementsButton";
			_implementsButton.UseVisualStyleBackColor = true;
			_implementsButton.Click += new System.EventHandler(_implementsButton_Click);
			resources.ApplyResources(_extendsOrReturnTypeButton, "_extendsOrReturnTypeButton");
			_extendsOrReturnTypeButton.Name = "_extendsOrReturnTypeButton";
			_extendsOrReturnTypeButton.UseVisualStyleBackColor = true;
			_extendsOrReturnTypeButton.Click += new System.EventHandler(_extendsOrReturnTypeButton_Click);
			resources.ApplyResources(_implementsLabel, "_implementsLabel");
			_implementsLabel.Name = "_implementsLabel";
			resources.ApplyResources(_extendsOrReturnTypeLabel, "_extendsOrReturnTypeLabel");
			_extendsOrReturnTypeLabel.Name = "_extendsOrReturnTypeLabel";
			resources.ApplyResources(_implementsTextBox, "_implementsTextBox");
			_implementsTextBox.Name = "_implementsTextBox";
			resources.ApplyResources(_extendsOrReturnTypeTextBox, "_extendsOrReturnTypeTextBox");
			_extendsOrReturnTypeTextBox.Name = "_extendsOrReturnTypeTextBox";
			_kindComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_kindComboBox.FormattingEnabled = true;
			resources.ApplyResources(_kindComboBox, "_kindComboBox");
			_kindComboBox.Name = "_kindComboBox";
			_kindComboBox.SelectedIndexChanged += new System.EventHandler(_kindComboBox_SelectedIndexChanged);
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
			resources.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			_cancelButton.UseVisualStyleBackColor = true;
			resources.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.UseVisualStyleBackColor = true;
			_okButton.Click += new System.EventHandler(_okButton_Click);
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
			base.Controls.Add(_attributesButton);
			base.Controls.Add(_okButton);
			base.Controls.Add(_cancelButton);
			base.Controls.Add(_commentGroupBox);
			base.Controls.Add(groupBox);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "POUHeaderDialogFull";
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
