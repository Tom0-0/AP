#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.DeviceObject
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_add_device.htm")]
	[AssociatedOnlineHelpTopic("core.deviceobject.devices.chm::/add_device.htm")]
	public class AddDeviceForm : Form
	{
		private IDeviceCatalogue _catalogue;

		private Label _nameLabel;

		private GroupBox _deviceGroupBox;

		private Label _noImageFoundLabel;

		private Panel _deviceListPanel;

		private PictureBox _imagePictureBox;

		private PictureBox _iconPictureBox;

		private Button _okButton;

		private Button _cancelButton;

		private TextBox _objectNameTextBox;

		private TextBox _orderNumberTextBox;

		private TextBox _versionTextBox;

		private TextBox _vendorTextBox;

		private TextBox _nameTextBox;

		private TextBox _descriptionTextBox;

		private Label _descriptionLegendLabel;

		private Label _orderNumberLegendLabel;

		private Label _versionLegendLabel;

		private Label _vendorLegendLabel;

		private Label _nameLegendLabel;

		private TextBox _groupsTextBox;

		private Label _groupsLegendLabel;

		private Label _noDeviceSelectedLabel;

		private Container components;

		private bool _bFixObjectName;

		internal IDeviceDescription SelectedDevice => _catalogue.SelectedDevice;

		internal string ObjectName => _objectNameTextBox.Text;

		public AddDeviceForm()
		{
			InitializeComponent();
			if (!base.DesignMode)
			{
				_catalogue = APEnvironment.CreateFirstDeviceCatalogue();
				_catalogue.Control.Dock = DockStyle.Fill;
				_deviceListPanel.Controls.Add(_catalogue.Control);
				_catalogue.SelectionChanged+=((EventHandler)OnCatalogueSelectionChanged);
			}
		}

		internal void Initialize(string[] filter, IDeviceIdentification context, string stDeviceName)
		{
			Initialize(filter, context);
			_objectNameTextBox.Text = stDeviceName;
			_objectNameTextBox.Enabled = false;
			_bFixObjectName = true;
		}

		internal void Initialize(string[] filter, IDeviceIdentification context)
		{
			_objectNameTextBox.Enabled = true;
			_bFixObjectName = false;
			_catalogue.BeginUpdate();
			try
			{
				_catalogue.Filter=(_catalogue.CreateChildConnectorFilter(filter));
				_catalogue.Context=(context);
			}
			finally
			{
				_catalogue.EndUpdate();
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

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.DesignMode)
			{
				UpdateControlStates();
			}
		}

		private void UpdateControlStates()
		{
			IDeviceDescription selectedDevice = _catalogue.SelectedDevice;
			if (selectedDevice != null)
			{
				_iconPictureBox.Visible = true;
				Icon icon = selectedDevice.DeviceInfo.Icon;
				if (icon != null)
				{
					_iconPictureBox.Image = icon.ToBitmap();
				}
				else
				{
					_iconPictureBox.Image = Bitmap.FromHicon(base.Icon.Handle);
				}
				_nameLegendLabel.Visible = true;
				_vendorLegendLabel.Visible = true;
				_groupsLegendLabel.Visible = true;
				_versionLegendLabel.Visible = true;
				_orderNumberLegendLabel.Visible = true;
				_descriptionLegendLabel.Visible = true;
				_nameTextBox.Visible = true;
				_nameTextBox.Text = selectedDevice.DeviceInfo.Name;
				_vendorTextBox.Visible = true;
				_vendorTextBox.Text = selectedDevice.DeviceInfo.Vendor;
				_groupsTextBox.Visible = true;
				_groupsTextBox.Text = GetGroupsString(selectedDevice, _catalogue.VendorSpecificStructure);
				_versionTextBox.Visible = true;
				_versionTextBox.Text = selectedDevice.DeviceIdentification.Version;
				_orderNumberTextBox.Visible = true;
				_orderNumberTextBox.Text = selectedDevice.DeviceInfo.OrderNumber;
				_descriptionTextBox.Visible = true;
				_descriptionTextBox.Text = selectedDevice.DeviceInfo.Description;
				_imagePictureBox.Visible = true;
				Image image = selectedDevice.DeviceInfo.Image;
				if (image == null)
				{
					image = ((IEngine)APEnvironment.Engine).ResourceManager.GetBitmap(GetType(), "_3S.CoDeSys.DeviceObject.Resources.DefaultDeviceBitmap.bmp");
				}
				_imagePictureBox.Image = image;
				if (image.Height < _imagePictureBox.Size.Height && image.Width < _imagePictureBox.Size.Width)
				{
					_imagePictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
				}
				else
				{
					_imagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				}
				_noImageFoundLabel.Visible = false;
				_noDeviceSelectedLabel.Visible = false;
				_okButton.Enabled = true;
				_objectNameTextBox.Enabled = !_bFixObjectName;
			}
			else
			{
				_iconPictureBox.Visible = false;
				_nameLegendLabel.Visible = false;
				_vendorLegendLabel.Visible = false;
				_groupsLegendLabel.Visible = false;
				_versionLegendLabel.Visible = false;
				_orderNumberLegendLabel.Visible = false;
				_descriptionLegendLabel.Visible = false;
				_nameTextBox.Visible = false;
				_vendorTextBox.Visible = false;
				_groupsTextBox.Visible = false;
				_versionTextBox.Visible = false;
				_orderNumberTextBox.Visible = false;
				_descriptionTextBox.Visible = false;
				_imagePictureBox.Visible = false;
				_noImageFoundLabel.Visible = false;
				_noDeviceSelectedLabel.Visible = true;
				_okButton.Enabled = false;
				_objectNameTextBox.Enabled = false;
			}
		}

		private static string GetGroupsString(IDeviceDescription device, bool bVendorSpecific)
		{
			Debug.Assert(device != null);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			if (bVendorSpecific)
			{
				string[] families = device.DeviceInfo.Families;
				if (families != null)
				{
					string[] array = families;
					foreach (string text in array)
					{
						if (!flag)
						{
							stringBuilder.Append(", ");
						}
						try
						{
							IDeviceFamily deviceFamily = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDeviceFamily(text);
							if (deviceFamily != null)
							{
								stringBuilder.Append(deviceFamily.Name);
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			else
			{
				int[] categories = device.DeviceInfo.Categories;
				if (categories != null)
				{
					int[] array2 = categories;
					foreach (int num in array2)
					{
						if (!flag)
						{
							stringBuilder.Append(", ");
						}
						try
						{
							IDeviceCategory deviceCategory = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDeviceCategory(num);
							if (deviceCategory != null)
							{
								stringBuilder.Append(deviceCategory.Name);
								flag = false;
							}
						}
						catch
						{
						}
					}
				}
			}
			return stringBuilder.ToString().Trim(' ', ',');
		}

		private static bool IsValidIdentifier(string st)
		{
			IScanner obj = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(st, false, false, false, false);
			Debug.Assert(obj != null);
			IToken val = default(IToken);
			return obj.Match((TokenType)13, true, out val) > 0;
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.AddDeviceForm));
			_nameLabel = new System.Windows.Forms.Label();
			_objectNameTextBox = new System.Windows.Forms.TextBox();
			_deviceGroupBox = new System.Windows.Forms.GroupBox();
			_orderNumberTextBox = new System.Windows.Forms.TextBox();
			_versionTextBox = new System.Windows.Forms.TextBox();
			_groupsTextBox = new System.Windows.Forms.TextBox();
			_vendorTextBox = new System.Windows.Forms.TextBox();
			_nameTextBox = new System.Windows.Forms.TextBox();
			_descriptionTextBox = new System.Windows.Forms.TextBox();
			_descriptionLegendLabel = new System.Windows.Forms.Label();
			_orderNumberLegendLabel = new System.Windows.Forms.Label();
			_versionLegendLabel = new System.Windows.Forms.Label();
			_vendorLegendLabel = new System.Windows.Forms.Label();
			_groupsLegendLabel = new System.Windows.Forms.Label();
			_nameLegendLabel = new System.Windows.Forms.Label();
			_iconPictureBox = new System.Windows.Forms.PictureBox();
			_deviceListPanel = new System.Windows.Forms.Panel();
			_imagePictureBox = new System.Windows.Forms.PictureBox();
			_noImageFoundLabel = new System.Windows.Forms.Label();
			_noDeviceSelectedLabel = new System.Windows.Forms.Label();
			_okButton = new System.Windows.Forms.Button();
			_cancelButton = new System.Windows.Forms.Button();
			_deviceGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).BeginInit();
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).BeginInit();
			SuspendLayout();
			componentResourceManager.ApplyResources(_nameLabel, "_nameLabel");
			_nameLabel.Name = "_nameLabel";
			componentResourceManager.ApplyResources(_objectNameTextBox, "_objectNameTextBox");
			_objectNameTextBox.Name = "_objectNameTextBox";
			_objectNameTextBox.TextChanged += new System.EventHandler(OnObjectNameTextBoxTextChanged);
			componentResourceManager.ApplyResources(_deviceGroupBox, "_deviceGroupBox");
			_deviceGroupBox.Controls.Add(_orderNumberTextBox);
			_deviceGroupBox.Controls.Add(_versionTextBox);
			_deviceGroupBox.Controls.Add(_groupsTextBox);
			_deviceGroupBox.Controls.Add(_vendorTextBox);
			_deviceGroupBox.Controls.Add(_nameTextBox);
			_deviceGroupBox.Controls.Add(_descriptionTextBox);
			_deviceGroupBox.Controls.Add(_descriptionLegendLabel);
			_deviceGroupBox.Controls.Add(_orderNumberLegendLabel);
			_deviceGroupBox.Controls.Add(_versionLegendLabel);
			_deviceGroupBox.Controls.Add(_vendorLegendLabel);
			_deviceGroupBox.Controls.Add(_groupsLegendLabel);
			_deviceGroupBox.Controls.Add(_nameLegendLabel);
			_deviceGroupBox.Controls.Add(_iconPictureBox);
			_deviceGroupBox.Controls.Add(_deviceListPanel);
			_deviceGroupBox.Controls.Add(_imagePictureBox);
			_deviceGroupBox.Controls.Add(_noImageFoundLabel);
			_deviceGroupBox.Controls.Add(_noDeviceSelectedLabel);
			_deviceGroupBox.Name = "_deviceGroupBox";
			_deviceGroupBox.TabStop = false;
			componentResourceManager.ApplyResources(_orderNumberTextBox, "_orderNumberTextBox");
			_orderNumberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_orderNumberTextBox.Name = "_orderNumberTextBox";
			_orderNumberTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_versionTextBox, "_versionTextBox");
			_versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_versionTextBox.Name = "_versionTextBox";
			_versionTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_groupsTextBox, "_groupsTextBox");
			_groupsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_groupsTextBox.Name = "_groupsTextBox";
			_groupsTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_vendorTextBox, "_vendorTextBox");
			_vendorTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_vendorTextBox.Name = "_vendorTextBox";
			_vendorTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_nameTextBox, "_nameTextBox");
			_nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_nameTextBox.Name = "_nameTextBox";
			_nameTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_descriptionTextBox, "_descriptionTextBox");
			_descriptionTextBox.Name = "_descriptionTextBox";
			_descriptionTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_descriptionLegendLabel, "_descriptionLegendLabel");
			_descriptionLegendLabel.Name = "_descriptionLegendLabel";
			componentResourceManager.ApplyResources(_orderNumberLegendLabel, "_orderNumberLegendLabel");
			_orderNumberLegendLabel.Name = "_orderNumberLegendLabel";
			componentResourceManager.ApplyResources(_versionLegendLabel, "_versionLegendLabel");
			_versionLegendLabel.Name = "_versionLegendLabel";
			componentResourceManager.ApplyResources(_vendorLegendLabel, "_vendorLegendLabel");
			_vendorLegendLabel.Name = "_vendorLegendLabel";
			componentResourceManager.ApplyResources(_groupsLegendLabel, "_groupsLegendLabel");
			_groupsLegendLabel.Name = "_groupsLegendLabel";
			componentResourceManager.ApplyResources(_nameLegendLabel, "_nameLegendLabel");
			_nameLegendLabel.Name = "_nameLegendLabel";
			componentResourceManager.ApplyResources(_iconPictureBox, "_iconPictureBox");
			_iconPictureBox.Name = "_iconPictureBox";
			_iconPictureBox.TabStop = false;
			componentResourceManager.ApplyResources(_deviceListPanel, "_deviceListPanel");
			_deviceListPanel.Name = "_deviceListPanel";
			componentResourceManager.ApplyResources(_imagePictureBox, "_imagePictureBox");
			_imagePictureBox.Name = "_imagePictureBox";
			_imagePictureBox.TabStop = false;
			componentResourceManager.ApplyResources(_noImageFoundLabel, "_noImageFoundLabel");
			_noImageFoundLabel.Name = "_noImageFoundLabel";
			componentResourceManager.ApplyResources(_noDeviceSelectedLabel, "_noDeviceSelectedLabel");
			_noDeviceSelectedLabel.Name = "_noDeviceSelectedLabel";
			componentResourceManager.ApplyResources(_okButton, "_okButton");
			_okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			_okButton.Name = "_okButton";
			_okButton.Click += new System.EventHandler(_okButton_Click);
			componentResourceManager.ApplyResources(_cancelButton, "_cancelButton");
			_cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cancelButton.Name = "_cancelButton";
			base.AcceptButton = _okButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.CancelButton = _cancelButton;
			base.Controls.Add(_cancelButton);
			base.Controls.Add(_okButton);
			base.Controls.Add(_deviceGroupBox);
			base.Controls.Add(_objectNameTextBox);
			base.Controls.Add(_nameLabel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddDeviceForm";
			base.ShowInTaskbar = false;
			base.Shown += new System.EventHandler(AddDeviceForm_Shown);
			_deviceGroupBox.ResumeLayout(false);
			_deviceGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).EndInit();
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		private void OnCatalogueSelectionChanged(object sender, EventArgs e)
		{
			IDeviceDescription selectedDevice = _catalogue.SelectedDevice;
			string text;
			if (selectedDevice != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				string stBaseName = DeviceObjectHelper.CreateInstanceNameBase(selectedDevice.DeviceInfo);
				text = DeviceObjectHelper.CreateUniqueIdentifier(handle, stBaseName, DeviceCommandHelper.GetHostFromSelectedStub());
			}
			else
			{
				text = "";
			}
			if (!_bFixObjectName)
			{
				_objectNameTextBox.Text = text;
			}
			UpdateControlStates();
		}

		private void OnObjectNameTextBoxTextChanged(object sender, EventArgs e)
		{
			UpdateControlStates();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			string text = null;
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			Debug.Assert(primaryProject != null);
			if (_objectNameTextBox.Text == "")
			{
				text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameEmpty");
			}
			else if (!IsValidIdentifier(_objectNameTextBox.Text))
			{
				text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameInvalid");
			}
			else if (!_bFixObjectName && !DeviceObjectHelper.CheckUniqueIdentifier(primaryProject.Handle, _objectNameTextBox.Text, DeviceCommandHelper.GetSelectedStub()))
			{
				text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameNotUnique");
			}
			if (text != null)
			{
				APEnvironment.MessageService.Error(text, text, Array.Empty<object>());
				base.DialogResult = DialogResult.None;
				_objectNameTextBox.Focus();
			}
		}

		private void AddDeviceForm_Shown(object sender, EventArgs e)
		{
			_okButton.Text = Text.Replace("...", "");
		}
	}
}
