using System;
using System.ComponentModel;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
	public class SelectDeviceTypeControl : UserControl
	{
		private TextBox _nameTextBox;

		private Label label1;

		private Panel _cataloguePanel;

		private Label _divider;

		private Label label2;

		private Container components;

		private IDeviceCatalogue _catalogue;

		protected virtual bool InDesigner => base.DesignMode;

		internal Control NameControl => _nameTextBox;

		internal IDeviceCatalogue Catalogue => _catalogue;

		internal IDeviceIdentification SelectedDevice => _catalogue.SelectedDevice.DeviceIdentification;

		public SelectDeviceTypeControl()
		{
			InitializeComponent();
			if (!InDesigner)
			{
				_catalogue = APEnvironment.CreateFirstDeviceCatalogue();
				_catalogue.Control.Dock = DockStyle.Fill;
				_cataloguePanel.Controls.Add(_catalogue.Control);
				_catalogue.SelectionChanged+=((EventHandler)OnCatalogueSelectionChanged);
				_nameTextBox.Enabled = false;
				_nameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				_divider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.SelectDeviceTypeControl));
			_nameTextBox = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			_cataloguePanel = new System.Windows.Forms.Panel();
			_divider = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			SuspendLayout();
			_nameTextBox.AccessibleDescription = null;
			_nameTextBox.AccessibleName = null;
			componentResourceManager.ApplyResources(_nameTextBox, "_nameTextBox");
			_nameTextBox.BackgroundImage = null;
			_nameTextBox.Font = null;
			_nameTextBox.Name = "_nameTextBox";
			label1.AccessibleDescription = null;
			label1.AccessibleName = null;
			componentResourceManager.ApplyResources(label1, "label1");
			label1.Font = null;
			label1.Name = "label1";
			_cataloguePanel.AccessibleDescription = null;
			_cataloguePanel.AccessibleName = null;
			componentResourceManager.ApplyResources(_cataloguePanel, "_cataloguePanel");
			_cataloguePanel.BackgroundImage = null;
			_cataloguePanel.Font = null;
			_cataloguePanel.Name = "_cataloguePanel";
			_divider.AccessibleDescription = null;
			_divider.AccessibleName = null;
			componentResourceManager.ApplyResources(_divider, "_divider");
			_divider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_divider.Font = null;
			_divider.Name = "_divider";
			label2.AccessibleDescription = null;
			label2.AccessibleName = null;
			componentResourceManager.ApplyResources(label2, "label2");
			label2.Font = null;
			label2.Name = "label2";
			base.AccessibleDescription = null;
			base.AccessibleName = null;
			componentResourceManager.ApplyResources(this, "$this");
			BackgroundImage = null;
			base.Controls.Add(label2);
			base.Controls.Add(_cataloguePanel);
			base.Controls.Add(_nameTextBox);
			base.Controls.Add(label1);
			base.Controls.Add(_divider);
			Font = null;
			base.Name = "SelectDeviceTypeControl";
			ResumeLayout(false);
			PerformLayout();
		}

		private void OnCatalogueSelectionChanged(object sender, EventArgs e)
		{
			IDeviceDescription selectedDevice = _catalogue.SelectedDevice;
			if (selectedDevice != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				string stBaseName = DeviceObjectHelper.CreateInstanceNameBase(selectedDevice.DeviceInfo);
				string text = DeviceObjectHelper.CreateUniqueIdentifier(handle, stBaseName, DeviceCommandHelper.GetSelectedStub());
				_nameTextBox.Text = text;
				_nameTextBox.Enabled = true;
			}
			else
			{
				_nameTextBox.Text = "";
				_nameTextBox.Enabled = false;
			}
		}
	}
}
