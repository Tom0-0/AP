using System.ComponentModel;
using System.Windows.Forms;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_online_configuration_mode.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/online_config_mode.htm")]
	public class ConfigModeForm : Form
	{
		private IContainer components;

		private Button _btCancel;

		private Label _lblApplications;

		private ListView _lvApplications;

		private Button _btParamMode;

		private Button _btAppMode;

		public ConfigModeForm()
		{
			InitializeComponent();
		}

		public void Applications(LList<string> liApplications)
		{
			foreach (string liApplication in liApplications)
			{
				ListViewItem listViewItem = new ListViewItem();
				listViewItem.Text = liApplication;
				_lvApplications.Items.Add(listViewItem);
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.ConfigModeForm));
			_btCancel = new System.Windows.Forms.Button();
			_lblApplications = new System.Windows.Forms.Label();
			_lvApplications = new System.Windows.Forms.ListView();
			_btParamMode = new System.Windows.Forms.Button();
			_btAppMode = new System.Windows.Forms.Button();
			SuspendLayout();
			componentResourceManager.ApplyResources(_btCancel, "_btCancel");
			_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_btCancel.Name = "_btCancel";
			_btCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_lblApplications, "_lblApplications");
			_lblApplications.Name = "_lblApplications";
			componentResourceManager.ApplyResources(_lvApplications, "_lvApplications");
			_lvApplications.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			_lvApplications.MultiSelect = false;
			_lvApplications.Name = "_lvApplications";
			_lvApplications.ShowGroups = false;
			_lvApplications.UseCompatibleStateImageBehavior = false;
			_lvApplications.View = System.Windows.Forms.View.List;
			componentResourceManager.ApplyResources(_btParamMode, "_btParamMode");
			_btParamMode.DialogResult = System.Windows.Forms.DialogResult.Yes;
			_btParamMode.Name = "_btParamMode";
			_btParamMode.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_btAppMode, "_btAppMode");
			_btAppMode.DialogResult = System.Windows.Forms.DialogResult.No;
			_btAppMode.Name = "_btAppMode";
			_btAppMode.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ControlBox = false;
			base.Controls.Add(_btAppMode);
			base.Controls.Add(_btParamMode);
			base.Controls.Add(_lvApplications);
			base.Controls.Add(_lblApplications);
			base.Controls.Add(_btCancel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ConfigModeForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
