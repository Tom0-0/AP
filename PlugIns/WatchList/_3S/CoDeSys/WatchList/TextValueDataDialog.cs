using System;
using System.ComponentModel;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.WatchList
{
	[AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/_cds_cmd_force_values.htm")]
	[AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Force_Values.htm")]
	public class TextValueDataDialog : Form
	{
		private string _stExpression;

		private ICompiledType _type;

		private string _stCurrentValue;

		private object _unforce;

		private string _stPreparedValue;

		private bool _bUseForUnforcedVariables;

		private bool _bIsEnum;

		private Guid _application;

		private WatchListNode _node;

		private static IType s_stringType;

		private static IType s_wstringType;

		private static IType s_boolType;

		private TextBox _expressionTextBox;

		private TextBox _typeTextBox;

		private TextBox _currentValueTextBox;

		private TextBox _prepareValueTextBox;

		private ComboBox _prepareValueComboBox;

		private Button _okButton;

		private Button _cancelButton;

		private TableLayoutPanel tableLayoutPanel1;

		private Label label1;

		private Label label2;

		private Label label3;

		private GroupBox _groupBox;

		private RadioButton _removePreparationRadioButton;

		private RadioButton _unforceRadioButton;

		private RadioButton _prepareValueRadioButton;

		private RadioButton _unforceAndRestoreRadioButton;

		private Panel _panelPrepareValue;

		private Container components;

		public bool UsageForUnforcedVariables
		{
			get
			{
				return _bUseForUnforcedVariables;
			}
			set
			{
				_bUseForUnforcedVariables = value;
			}
		}

		public object Unforce => _unforce;

		public string PreparedValue => _stPreparedValue;

		private static IType StringType
		{
			get
			{
				if (s_stringType == null)
				{
					IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner("STRING", false, false, false, false);
					s_stringType = (IType)(object)((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val).ParseTypeDeclaration();
				}
				return s_stringType;
			}
		}

		private static IType WStringType
		{
			get
			{
				if (s_wstringType == null)
				{
					IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner("WSTRING", false, false, false, false);
					s_wstringType = (IType)(object)((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val).ParseTypeDeclaration();
				}
				return s_wstringType;
			}
		}

		private static IType BoolType
		{
			get
			{
				if (s_boolType == null)
				{
					IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner("BOOL", false, false, false, false);
					s_boolType = (IType)(object)((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val).ParseTypeDeclaration();
				}
				return s_boolType;
			}
		}

		public TextValueDataDialog(bool bIsEnum, WatchListNode node)
		{
			InitializeComponent();
			_bIsEnum = bIsEnum;
			_node = node;
			_prepareValueComboBox = new ComboBox();
			_prepareValueTextBox = new TextBox();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Initialize(string stExpression, ICompiledType cType, string stCurrentValue, object unforce, string stPreparedValue, Guid application)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			if (stExpression == null)
			{
				throw new ArgumentNullException("stExpression");
			}
			if (cType == null)
			{
				throw new ArgumentNullException("cType");
			}
			if (stCurrentValue == null)
			{
				throw new ArgumentNullException("stCurrentValue");
			}
			if (stPreparedValue == null)
			{
				throw new ArgumentNullException("stPreparedValue");
			}
			if (unforce != null && unforce != PreparedValues.Unforce && unforce != PreparedValues.UnforceAndRestore)
			{
				throw new ArgumentException("unforce must be one of the following values: null, 'Unforce', or 'UnforceAndRestore'.");
			}
			if (cType != null && (int)((IType)cType).Class == 0)
			{
				bool result = false;
				if (bool.TryParse(stCurrentValue, out result))
				{
					stCurrentValue = stCurrentValue.ToUpperInvariant();
					if (stPreparedValue == string.Empty)
					{
						stPreparedValue = (!result).ToString().ToUpperInvariant();
					}
				}
			}
			_stExpression = stExpression;
			_stCurrentValue = stCurrentValue;
			_unforce = unforce;
			_stPreparedValue = stPreparedValue;
			_application = application;
			_type = cType;
		}

		protected override void OnLoad(EventArgs e)
		{
			CenterToScreen();
			_expressionTextBox.Text = _stExpression;
			_typeTextBox.Text = ((_type != null) ? ((object)_type).ToString() : "?");
			_currentValueTextBox.Text = _stCurrentValue;
			if (_bIsEnum && Common.LoadEnumValuesToComboBoxControl(_prepareValueComboBox, _type, _application))
			{
				_panelPrepareValue.Controls.Add(_prepareValueComboBox);
				_prepareValueComboBox.SelectedItem = _prepareValueComboBox.Items[0];
				_prepareValueComboBox.DropDownStyle = ComboBoxStyle.DropDown;
				_prepareValueComboBox.Width = _panelPrepareValue.Width;
			}
			else
			{
				_panelPrepareValue.Controls.Add(_prepareValueTextBox);
				_prepareValueTextBox.Width = _panelPrepareValue.Width;
				_prepareValueTextBox.Text = _stPreparedValue;
				_bIsEnum = false;
			}
			if (_unforce == PreparedValues.Unforce)
			{
				_unforceRadioButton.Checked = true;
			}
			else if (_unforce == PreparedValues.UnforceAndRestore)
			{
				_unforceAndRestoreRadioButton.Checked = true;
			}
			else
			{
				_prepareValueRadioButton.Checked = true;
			}
			UpdateControlStates();
			if (_prepareValueRadioButton.Checked)
			{
				if (_bIsEnum)
				{
					_prepareValueComboBox.Select();
				}
				else
				{
					_prepareValueTextBox.Select();
				}
			}
		}

		private void UpdateControlStates()
		{
			if (_prepareValueRadioButton.Checked)
			{
				_prepareValueTextBox.Enabled = true;
				_prepareValueComboBox.Enabled = true;
			}
			else
			{
				_prepareValueTextBox.Enabled = false;
				_prepareValueComboBox.Enabled = false;
			}
			if (string.IsNullOrEmpty(_stPreparedValue))
			{
				_removePreparationRadioButton.Enabled = false;
			}
			else
			{
				_removePreparationRadioButton.Enabled = true;
			}
			if (_bUseForUnforcedVariables)
			{
				_unforceAndRestoreRadioButton.Enabled = false;
				_unforceRadioButton.Enabled = false;
			}
			else
			{
				_unforceAndRestoreRadioButton.Enabled = true;
				_unforceRadioButton.Enabled = true;
			}
		}

		private void OnUnforceRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (_unforceRadioButton.Checked)
			{
				UpdateControlStates();
			}
		}

		private void OnUnforceAndRestoreRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (_unforceAndRestoreRadioButton.Checked)
			{
				UpdateControlStates();
			}
		}

		private void OnRemovePrepareRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (_removePreparationRadioButton.Checked)
			{
				UpdateControlStates();
			}
		}

		private void OnPrepareValueRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if (_prepareValueRadioButton.Checked)
			{
				UpdateControlStates();
				if (_bIsEnum)
				{
					_prepareValueComboBox.Select();
				}
				else
				{
					_prepareValueTextBox.Select();
				}
			}
		}

		private void OnPrepareExpressionTextBoxTextChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		private void OnOkButtonClick(object sender, EventArgs e)
		{
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Invalid comparison between Unknown and I4
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Invalid comparison between Unknown and I4
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Invalid comparison between Unknown and I4
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Invalid comparison between Unknown and I4
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Invalid comparison between Unknown and I4
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Invalid comparison between Unknown and I4
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Invalid comparison between Unknown and I4
			if (_prepareValueRadioButton.Checked)
			{
				try
				{
					IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
					ICompiledType type = _type;
					if (type != null)
					{
						object obj = default(object);
						TypeClass val = default(TypeClass);
						if (_bIsEnum && _prepareValueComboBox.Text != null)
						{
							string text = _prepareValueComboBox.Text.Trim();
							if (decimal.TryParse(text, out var _))
							{
								APEnvironment.MonitoringUtilities.CheckValidValue(type.BaseType, text, _application);
								converterFromIEC.GetLiteralValue(_prepareValueComboBox.Text.Trim(), out obj, out val);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj, (IType)(object)_type, Guid.Empty, (ByteOrder)0);
								_stPreparedValue = text;
							}
							else
							{
								APEnvironment.MonitoringUtilities.CheckValidValue(type, text, _application);
								_stPreparedValue = text;
							}
						}
						else
						{
							APEnvironment.MonitoringUtilities.CheckValidValue(type, _prepareValueTextBox.Text.Trim(), _application);
							converterFromIEC.GetLiteralValue(_prepareValueTextBox.Text, out obj, out val);
							((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj, (IType)(object)_type, Guid.Empty, (ByteOrder)0);
							_stPreparedValue = _prepareValueTextBox.Text;
						}
					}
				}
				catch (Exception ex)
				{
					bool flag = true;
					if (!(ex is OverflowException))
					{
						if ((int)((IType)_type).Class == 28)
						{
							try
							{
								IConverterFromIEC converterFromIEC2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
								object obj2 = default(object);
								TypeClass val2 = default(TypeClass);
								if (_bIsEnum)
								{
									converterFromIEC2.GetLiteralValue("'" + _prepareValueComboBox.Text + "'", out obj2, out val2);
									((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj2, StringType, Guid.Empty, (ByteOrder)0);
									_stPreparedValue = _prepareValueComboBox.Text;
								}
								else
								{
									converterFromIEC2.GetLiteralValue("'" + _prepareValueTextBox.Text + "'", out obj2, out val2);
									((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj2, StringType, Guid.Empty, (ByteOrder)0);
									_stPreparedValue = "'" + _prepareValueTextBox.Text + "'";
								}
								flag = false;
							}
							catch
							{
							}
						}
						else if ((int)((IType)_type).Class == 6 || (int)((IType)_type).Class == 7 || (int)((IType)_type).Class == 9 || (int)((IType)_type).Class == 8)
						{
							string stConvertedValue = string.Empty;
							if (WatchListNodeUtils.TryToConvertBinHexValue(_prepareValueTextBox.Text, null, ((IType)_type).Class, out stConvertedValue))
							{
								_stPreparedValue = stConvertedValue;
								flag = false;
							}
						}
						else if ((int)((IType)_type).Class == 0 && (_prepareValueTextBox.Text.Trim() == "0" || _prepareValueTextBox.Text.Trim() == "1"))
						{
							try
							{
								object obj4 = default(object);
								TypeClass val3 = default(TypeClass);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC().GetLiteralValue((_prepareValueTextBox.Text.Trim() == "1") ? "TRUE" : "FALSE", out obj4, out val3);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj4, BoolType, Guid.Empty, (ByteOrder)0);
								_stPreparedValue = ((_prepareValueTextBox.Text.Trim() == "1") ? "TRUE" : "FALSE");
								flag = false;
							}
							catch
							{
							}
						}
						else if ((int)((IType)_type).Class == 16 && _prepareValueTextBox.Text.IndexOf('\'') == -1)
						{
							try
							{
								object obj6 = default(object);
								TypeClass val4 = default(TypeClass);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC().GetLiteralValue("'" + _prepareValueTextBox.Text + "'", out obj6, out val4);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj6, StringType, Guid.Empty, (ByteOrder)0);
								_stPreparedValue = "'" + _prepareValueTextBox.Text + "'";
								flag = false;
							}
							catch
							{
							}
						}
						else if ((int)((IType)_type).Class == 17 && _prepareValueTextBox.Text.IndexOf('"') == -1)
						{
							try
							{
								object obj8 = default(object);
								TypeClass val5 = default(TypeClass);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC().GetLiteralValue("\"" + _prepareValueTextBox.Text + "\"", out obj8, out val5);
								((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ConvertToRaw(obj8, WStringType, Guid.Empty, (ByteOrder)0);
								_stPreparedValue = "\"" + _prepareValueTextBox.Text + "\"";
								flag = false;
							}
							catch
							{
							}
						}
					}
					if (flag)
					{
						string text2 = ((!_bIsEnum || _prepareValueComboBox.Text == null) ? string.Format(Strings.IncompatibleValue, _prepareValueTextBox.Text, ((object)_type).ToString()) : string.Format(Strings.IncompatibleValue, _prepareValueComboBox.Text, ((object)_type).ToString()));
						((IEngine)APEnvironment.Engine).MessageService.Error(text2);
						base.DialogResult = DialogResult.None;
						if (_bIsEnum)
						{
							_prepareValueComboBox.Select();
						}
						else
						{
							_prepareValueTextBox.Select();
						}
						return;
					}
				}
			}
			else
			{
				_stPreparedValue = null;
			}
			if (_unforceRadioButton.Checked)
			{
				_unforce = PreparedValues.Unforce;
			}
			else if (_unforceAndRestoreRadioButton.Checked)
			{
				_unforce = PreparedValues.UnforceAndRestore;
			}
			else
			{
				_unforce = null;
			}
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.WatchList.TextValueDataDialog));
			_expressionTextBox = new System.Windows.Forms.TextBox();
			_typeTextBox = new System.Windows.Forms.TextBox();
			_currentValueTextBox = new System.Windows.Forms.TextBox();
			_okButton = new System.Windows.Forms.Button();
			_cancelButton = new System.Windows.Forms.Button();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			_groupBox = new System.Windows.Forms.GroupBox();
			_removePreparationRadioButton = new System.Windows.Forms.RadioButton();
			_unforceRadioButton = new System.Windows.Forms.RadioButton();
			_prepareValueRadioButton = new System.Windows.Forms.RadioButton();
			_unforceAndRestoreRadioButton = new System.Windows.Forms.RadioButton();
			_panelPrepareValue = new System.Windows.Forms.Panel();
			tableLayoutPanel1.SuspendLayout();
			_groupBox.SuspendLayout();
			SuspendLayout();
			componentResourceManager.ApplyResources(_expressionTextBox, "_expressionTextBox");
			_expressionTextBox.Name = "_expressionTextBox";
			_expressionTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_typeTextBox, "_typeTextBox");
			_typeTextBox.Name = "_typeTextBox";
			_typeTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_currentValueTextBox, "_currentValueTextBox");
			_currentValueTextBox.Name = "_currentValueTextBox";
			_currentValueTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.Click += new System.EventHandler(OnOkButtonClick);
			componentResourceManager.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Controls.Add(label2, 0, 1);
			tableLayoutPanel1.Controls.Add(_expressionTextBox, 1, 0);
			tableLayoutPanel1.Controls.Add(_currentValueTextBox, 1, 2);
			tableLayoutPanel1.Controls.Add(_typeTextBox, 1, 1);
			tableLayoutPanel1.Controls.Add(label3, 0, 2);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			componentResourceManager.ApplyResources(label1, "label1");
			label1.Name = "label1";
			componentResourceManager.ApplyResources(label2, "label2");
			label2.Name = "label2";
			componentResourceManager.ApplyResources(label3, "label3");
			label3.Name = "label3";
			componentResourceManager.ApplyResources(_groupBox, "_groupBox");
			_groupBox.Controls.Add(_removePreparationRadioButton);
			_groupBox.Controls.Add(_unforceRadioButton);
			_groupBox.Controls.Add(_prepareValueRadioButton);
			_groupBox.Controls.Add(_unforceAndRestoreRadioButton);
			_groupBox.Controls.Add(_panelPrepareValue);
			_groupBox.Name = "_groupBox";
			_groupBox.TabStop = false;
			componentResourceManager.ApplyResources(_removePreparationRadioButton, "_removePreparationRadioButton");
			_removePreparationRadioButton.Name = "_removePreparationRadioButton";
			componentResourceManager.ApplyResources(_unforceRadioButton, "_unforceRadioButton");
			_unforceRadioButton.Name = "_unforceRadioButton";
			componentResourceManager.ApplyResources(_prepareValueRadioButton, "_prepareValueRadioButton");
			_prepareValueRadioButton.Name = "_prepareValueRadioButton";
			componentResourceManager.ApplyResources(_unforceAndRestoreRadioButton, "_unforceAndRestoreRadioButton");
			_unforceAndRestoreRadioButton.Name = "_unforceAndRestoreRadioButton";
			componentResourceManager.ApplyResources(_panelPrepareValue, "_panelPrepareValue");
			_panelPrepareValue.Name = "_panelPrepareValue";
			base.AcceptButton = _okButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.CancelButton = _cancelButton;
			base.Controls.Add(_groupBox);
			base.Controls.Add(_cancelButton);
			base.Controls.Add(_okButton);
			base.Controls.Add(tableLayoutPanel1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TextValueDataDialog";
			base.ShowInTaskbar = false;
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			_groupBox.ResumeLayout(false);
			_groupBox.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
