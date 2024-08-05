using System.ComponentModel;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceCommunicationEditor;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_dlg_options_device_editor.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceEditor.Options.chm::/core_deviceeditor_options_home.htm")]
	public class OptionControl : UserControl
	{
		private TabControl tabControl;

		private TabPage _viewTabPage;

		private CheckBox _showGenericConfig;

		private CheckBox _ckbCrossReferences;

		private CheckBox _ckbShowAllSyncFiles;

		private CheckBox _ckbShowAccessRights;

		private CheckBox _ckBHorizontalTabs;

		private ComboBox _comboComPageMode;

		private Label label1;

		private Container components;

		public OptionControl()
		{
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected I4, but got Unknown
			InitializeComponent();
			_showGenericConfig.Checked = OptionsHelper.ShowGenericConfiguration;
			if (OptionsHelper.IsOemValueAvailable("DeviceEditor", "ShowGenericEditor", out var value))
			{
				_showGenericConfig.Checked = (bool)value;
				_showGenericConfig.Enabled = false;
			}
			_ckbCrossReferences.Checked = OptionsHelper.CreateCrossReferences;
			_ckbShowAllSyncFiles.Checked = OptionsHelper.ShowAllSyncFiles;
			_ckbShowAccessRights.Checked = OptionsHelper.ShowAccessRightsPage;
			_ckBHorizontalTabs.Checked = OptionsHelper.UseHorizontalTabPages;
			_ckBHorizontalTabs.Enabled = !OptionsHelper.OEMUseClassicDeviceEditorAvailable;
			_comboComPageMode.Items.Add(Strings.ComPageMode_Simple);
			_comboComPageMode.Items.Add(Strings.ComPageMode_Classic);
			if (APEnvironment.ComPageCustomizerOrNull != null && !string.IsNullOrEmpty(APEnvironment.ComPageCustomizerOrNull.ComPageName))
			{
				_comboComPageMode.Items.Add(APEnvironment.ComPageCustomizerOrNull.ComPageName);
			}
			_comboComPageMode.SelectedIndex = 0;
			int num = (int)OptionsHelper.ComPageMode;
			if (num > 0 && num < _comboComPageMode.Items.Count)
			{
				_comboComPageMode.SelectedIndex = num;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.OptionControl));
			tabControl = new System.Windows.Forms.TabControl();
			_viewTabPage = new System.Windows.Forms.TabPage();
			label1 = new System.Windows.Forms.Label();
			_comboComPageMode = new System.Windows.Forms.ComboBox();
			_ckBHorizontalTabs = new System.Windows.Forms.CheckBox();
			_ckbShowAccessRights = new System.Windows.Forms.CheckBox();
			_ckbShowAllSyncFiles = new System.Windows.Forms.CheckBox();
			_ckbCrossReferences = new System.Windows.Forms.CheckBox();
			_showGenericConfig = new System.Windows.Forms.CheckBox();
			tabControl.SuspendLayout();
			_viewTabPage.SuspendLayout();
			SuspendLayout();
			tabControl.Controls.Add(_viewTabPage);
			resources.ApplyResources(tabControl, "tabControl");
			tabControl.Name = "tabControl";
			tabControl.SelectedIndex = 0;
			_viewTabPage.Controls.Add(label1);
			_viewTabPage.Controls.Add(_comboComPageMode);
			_viewTabPage.Controls.Add(_ckBHorizontalTabs);
			_viewTabPage.Controls.Add(_ckbShowAccessRights);
			_viewTabPage.Controls.Add(_ckbShowAllSyncFiles);
			_viewTabPage.Controls.Add(_ckbCrossReferences);
			_viewTabPage.Controls.Add(_showGenericConfig);
			resources.ApplyResources(_viewTabPage, "_viewTabPage");
			_viewTabPage.Name = "_viewTabPage";
			resources.ApplyResources(label1, "label1");
			label1.Name = "label1";
			_comboComPageMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_comboComPageMode.FormattingEnabled = true;
			resources.ApplyResources(_comboComPageMode, "_comboComPageMode");
			_comboComPageMode.Name = "_comboComPageMode";
			resources.ApplyResources(_ckBHorizontalTabs, "_ckBHorizontalTabs");
			_ckBHorizontalTabs.Name = "_ckBHorizontalTabs";
			resources.ApplyResources(_ckbShowAccessRights, "_ckbShowAccessRights");
			_ckbShowAccessRights.Name = "_ckbShowAccessRights";
			resources.ApplyResources(_ckbShowAllSyncFiles, "_ckbShowAllSyncFiles");
			_ckbShowAllSyncFiles.Name = "_ckbShowAllSyncFiles";
			resources.ApplyResources(_ckbCrossReferences, "_ckbCrossReferences");
			_ckbCrossReferences.Name = "_ckbCrossReferences";
			resources.ApplyResources(_showGenericConfig, "_showGenericConfig");
			_showGenericConfig.Name = "_showGenericConfig";
			base.Controls.Add(tabControl);
			resources.ApplyResources(this, "$this");
			base.Name = "OptionControl";
			tabControl.ResumeLayout(false);
			_viewTabPage.ResumeLayout(false);
			_viewTabPage.PerformLayout();
			ResumeLayout(false);
		}

		public bool Save(ref string stMessage, ref Control failedControl)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Invalid comparison between Unknown and I4
			bool flag = false;
			if (OptionsHelper.ShowGenericConfiguration != _showGenericConfig.Checked)
			{
				OptionsHelper.ShowGenericConfiguration = _showGenericConfig.Checked;
				flag = true;
			}
			if (OptionsHelper.CreateCrossReferences != _ckbCrossReferences.Checked)
			{
				OptionsHelper.CreateCrossReferences = _ckbCrossReferences.Checked;
			}
			if ((int)OptionsHelper.ComPageMode != _comboComPageMode.SelectedIndex)
			{
				OptionsHelper.ComPageMode = (AvailableComPageModez)_comboComPageMode.SelectedIndex;
				flag = true;
			}
			if (OptionsHelper.ShowAllSyncFiles != _ckbShowAllSyncFiles.Checked)
			{
				OptionsHelper.ShowAllSyncFiles = _ckbShowAllSyncFiles.Checked;
				flag = true;
			}
			if (OptionsHelper.ShowAccessRightsPage != _ckbShowAccessRights.Checked)
			{
				OptionsHelper.ShowAccessRightsPage = _ckbShowAccessRights.Checked;
				flag = true;
			}
			if (OptionsHelper.UseHorizontalTabPages != _ckBHorizontalTabs.Checked)
			{
				OptionsHelper.UseHorizontalTabPages = _ckBHorizontalTabs.Checked;
				flag = true;
			}
			if (flag)
			{
				IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors();
				foreach (IEditor val in editors)
				{
					if (val is DeviceEditor)
					{
						val.Save(true);
						val.Reload();
					}
				}
			}
			return true;
		}
	}
}
