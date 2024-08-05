using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_dlg_properties_options_controller.htm")]
	public class DevicePropertiesEditor : UserControl, IPropertiesEditorView, IEditor
	{
		private IEditor _editor;

		private bool _bReloadActive;

		private bool _bNeedSaving;

		private bool _bWinkAllowed;

		private bool _bPressKeyAllowed;

		private bool _bEnterIdAllowed;

		private bool _bAllowSyncVarAccessWarned;

		private InteractiveLoginMode _currentMode;

		private InteractiveLoginMode _newMode;

		private IContainer components;

		private NumericUpDown _numUpDown;

		private Label label1;

		private RadioButton _btnNone;

		private RadioButton _btnEnterID;

		private RadioButton _btnPressKey;

		private GroupBox groupBox1;

		private RadioButton _btnWink;

		private CheckBox _cbAllowSyncVarAccess;

		private TextBox _tbAllowSyncVarAccess;

		private GroupBox groupBox2;

		public Control Control => this;

		public int ProjectHandle => _editor.ProjectHandle;

		public Guid ObjectGuid => _editor.ObjectGuid;

		public DevicePropertiesEditor()
		{
			InitializeComponent();
		}

		public void Initialize(IEditor editor)
		{
			if (editor == null)
			{
				throw new ArgumentNullException("editor");
			}
			_editor = editor;
		}

		public void SetObject(int nProjectHandle, Guid objectGuid)
		{
		}

		public void Reload()
		{
			_bReloadActive = true;
			_btnNone.Checked = true;
			_btnEnterID.Enabled = false;
			_btnNone.Enabled = false;
			_btnPressKey.Enabled = false;
			_btnWink.Enabled = false;
			_cbAllowSyncVarAccess.Enabled = false;
			try
			{
				IMetaObject objectToRead = GetObjectToRead();
				IObject @object = objectToRead.Object;
				IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (val != null && val.CommunicationSettings is ICommunicationSettings4)
				{
					IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(objectToRead.ObjectGuid);
					_numUpDown.Value = ((ICommunicationSettings4)val.CommunicationSettings).MonitoringIntervalMsec;
					if (onlineDevice != null && onlineDevice.IsConnected)
					{
						_numUpDown.Enabled = false;
					}
					else
					{
						_numUpDown.Enabled = true;
					}
				}
				else
				{
					_numUpDown.Value = 200m;
					_numUpDown.Enabled = false;
				}
				if (val is IDeviceObject13 && val.DeviceIdentification != null && APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentification) != null)
				{
					_currentMode = ((IDeviceObject13)val).LoginMode;
					_=((IDeviceObject6)val).StringTable;
					string empty = string.Empty;
					((IDeviceObject6)val).StringTable.ResolveString("security", "interactivelogin_id_string", "", out empty);
					_bEnterIdAllowed = !string.IsNullOrEmpty(empty);
					((IDeviceObject6)val).StringTable.ResolveString("security", "interactivelogin_key_press", "", out empty);
					_bPressKeyAllowed = !string.IsNullOrEmpty(empty);
					((IDeviceObject6)val).StringTable.ResolveString("security", "interactivelogin_blink", "", out empty);
					_bWinkAllowed = !string.IsNullOrEmpty(empty);
					_btnEnterID.Enabled = _bEnterIdAllowed;
					_btnPressKey.Enabled = _bPressKeyAllowed;
					_btnWink.Enabled = _bWinkAllowed;
					_btnNone.Enabled = true;
					if (((int)_currentMode & 2) > 0)
					{
						_btnPressKey.Checked = true;
					}
					else if (((int)_currentMode & 1) > 0)
					{
						_btnEnterID.Checked = true;
					}
					else if (((int)_currentMode & 4) > 0)
					{
						_btnWink.Checked = true;
					}
					else
					{
						_btnNone.Checked = true;
					}
				}
				if (val is IDeviceObject15)
				{
					_cbAllowSyncVarAccess.Checked = ((IDeviceObject15)val).AllowSymbolicVarAccessInSyncWithIecCycle;
					_cbAllowSyncVarAccess.Enabled = true;
				}
			}
			catch
			{
				_numUpDown.Value = 200m;
				_numUpDown.Enabled = false;
			}
			_bReloadActive = false;
		}

		public void Save(bool bCommit)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (!(_bNeedSaving && bCommit))
			{
				return;
			}
			_bNeedSaving = false;
			try
			{
				IObject @object = GetObjectToModify().Object;
				IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (val != null && val.CommunicationSettings is ICommunicationSettings4)
				{
					((ICommunicationSettings4)val.CommunicationSettings).MonitoringIntervalMsec=((int)_numUpDown.Value);
				}
				if (val is IDeviceObject13 && _newMode != _currentMode)
				{
					((IDeviceObject13)val).SetLoginMode(_newMode);
				}
				if (val is IDeviceObject15)
				{
					((IDeviceObject15)val).AllowSymbolicVarAccessInSyncWithIecCycle=(_cbAllowSyncVarAccess.Checked);
				}
			}
			catch (Exception ex)
			{
				((IEngine)APEnvironment.Engine).MessageService.Error(ex.Message);
			}
		}

		public IMetaObject GetObjectToRead()
		{
			return _editor.GetObjectToRead();
		}

		public IMetaObject GetObjectToModify()
		{
			return _editor.GetObjectToModify();
		}

		private void _numUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (!_bReloadActive)
			{
				_bNeedSaving = true;
			}
		}

		private void _btnNone_CheckedChanged(object sender, EventArgs e)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (!_bReloadActive)
			{
				_bNeedSaving = true;
				_newMode = (InteractiveLoginMode)0;
			}
		}

		private void _btnEnterID_CheckedChanged(object sender, EventArgs e)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (!_bReloadActive)
			{
				_bNeedSaving = true;
				_newMode = (InteractiveLoginMode)1;
			}
		}

		private void _btnPressKey_CheckedChanged(object sender, EventArgs e)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (!_bReloadActive)
			{
				_bNeedSaving = true;
				_newMode = (InteractiveLoginMode)2;
			}
		}

		private void _btnWink_CheckedChanged(object sender, EventArgs e)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (!_bReloadActive)
			{
				_bNeedSaving = true;
				_newMode = (InteractiveLoginMode)4;
			}
		}

		private void cbAllowSyncVarAccess_CheckedChanged(object sender, EventArgs e)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (!_bReloadActive)
			{
				if (!_bAllowSyncVarAccessWarned && (int)APEnvironment.MessageService.Prompt(Strings.Prompt_EnableSymbolicVarAccessInSync, (PromptChoice)1, (PromptResult)1, "AllowSymbcoliVarAccessInCycle", Array.Empty<object>()) != 0)
				{
					_bReloadActive = true;
					_cbAllowSyncVarAccess.Checked = !_cbAllowSyncVarAccess.Checked;
					_bReloadActive = false;
				}
				else
				{
					_bNeedSaving = true;
					_bAllowSyncVarAccessWarned = true;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.DevicePropertiesEditor));
			_numUpDown = new System.Windows.Forms.NumericUpDown();
			label1 = new System.Windows.Forms.Label();
			_btnNone = new System.Windows.Forms.RadioButton();
			_btnEnterID = new System.Windows.Forms.RadioButton();
			_btnPressKey = new System.Windows.Forms.RadioButton();
			groupBox1 = new System.Windows.Forms.GroupBox();
			_btnWink = new System.Windows.Forms.RadioButton();
			_cbAllowSyncVarAccess = new System.Windows.Forms.CheckBox();
			_tbAllowSyncVarAccess = new System.Windows.Forms.TextBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)_numUpDown).BeginInit();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_numUpDown, "_numUpDown");
			_numUpDown.Increment = new decimal(new int[4] { 10, 0, 0, 0 });
			_numUpDown.Maximum = new decimal(new int[4] { 1000, 0, 0, 0 });
			_numUpDown.Minimum = new decimal(new int[4] { 10, 0, 0, 0 });
			_numUpDown.Name = "_numUpDown";
			_numUpDown.ReadOnly = true;
			_numUpDown.Value = new decimal(new int[4] { 200, 0, 0, 0 });
			_numUpDown.ValueChanged += new System.EventHandler(_numUpDown_ValueChanged);
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			resources.ApplyResources(_btnNone, "_btnNone");
			_btnNone.Name = "_btnNone";
			_btnNone.TabStop = true;
			_btnNone.UseVisualStyleBackColor = true;
			_btnNone.CheckedChanged += new System.EventHandler(_btnNone_CheckedChanged);
			resources.ApplyResources(_btnEnterID, "_btnEnterID");
			_btnEnterID.Name = "_btnEnterID";
			_btnEnterID.TabStop = true;
			_btnEnterID.UseVisualStyleBackColor = true;
			_btnEnterID.CheckedChanged += new System.EventHandler(_btnEnterID_CheckedChanged);
			resources.ApplyResources(_btnPressKey, "_btnPressKey");
			_btnPressKey.Name = "_btnPressKey";
			_btnPressKey.TabStop = true;
			_btnPressKey.UseVisualStyleBackColor = true;
			_btnPressKey.CheckedChanged += new System.EventHandler(_btnPressKey_CheckedChanged);
			resources.ApplyResources(groupBox1, "groupBox1");
			groupBox1.Controls.Add(_btnWink);
			groupBox1.Controls.Add(_btnNone);
			groupBox1.Controls.Add(_btnPressKey);
			groupBox1.Controls.Add(_btnEnterID);
			groupBox1.Name = "groupBox1";
			groupBox1.TabStop = false;
			resources.ApplyResources(_btnWink, "_btnWink");
			_btnWink.Name = "_btnWink";
			_btnWink.TabStop = true;
			_btnWink.UseVisualStyleBackColor = true;
			_btnWink.CheckedChanged += new System.EventHandler(_btnWink_CheckedChanged);
			resources.ApplyResources(_cbAllowSyncVarAccess, "_cbAllowSyncVarAccess");
			_cbAllowSyncVarAccess.Name = "_cbAllowSyncVarAccess";
			_cbAllowSyncVarAccess.UseVisualStyleBackColor = true;
			_cbAllowSyncVarAccess.CheckedChanged += new System.EventHandler(cbAllowSyncVarAccess_CheckedChanged);
			_tbAllowSyncVarAccess.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(_tbAllowSyncVarAccess, "_tbAllowSyncVarAccess");
			_tbAllowSyncVarAccess.Name = "_tbAllowSyncVarAccess";
			_tbAllowSyncVarAccess.ReadOnly = true;
			_tbAllowSyncVarAccess.TabStop = false;
			resources.ApplyResources(groupBox2, "groupBox2");
			groupBox2.Controls.Add(_cbAllowSyncVarAccess);
			groupBox2.Controls.Add(_tbAllowSyncVarAccess);
			groupBox2.Name = "groupBox2";
			groupBox2.TabStop = false;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			base.Controls.Add(groupBox2);
			base.Controls.Add(groupBox1);
			base.Controls.Add(label1);
			base.Controls.Add(_numUpDown);
			ForeColor = System.Drawing.SystemColors.ControlText;
			base.Name = "DevicePropertiesEditor";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)_numUpDown).EndInit();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
