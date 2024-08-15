#define DEBUG
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.DeviceObject
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_add_device.htm")]
	[AssociatedOnlineHelpTopic("core.deviceobject.devices.chm::/add_device.htm")]
	public class DeviceFormEx : Form
	{
		protected IDeviceCatalogue _deviceCatalogue;

		protected IInteractiveDeviceCommand _selectedCommand;

		private IContainer components;

		protected TextBox _objectNameTextBox;

		protected Button _closeButton;

		protected Button _addDeviceButton;

		protected RadioButton _plugDeviceRadioButton;

		protected RadioButton _insertDeviceRadioButton;

		protected RadioButton _appendDeviceRadioButton;

		protected SplitContainer _splitContainer;

		protected Panel _deviceListPanel;

		protected PictureBox _iconPictureBox;

		protected PictureBox _imagePictureBox;

		protected RichTextBox _richTextBox;

		protected RadioButton _replaceDeviceRadioButton;

		protected Label _insertLocationLabel2;

		protected Label _insertLocationLabel1;

		protected ErrorProvider errorProvider1;

		protected CheckBox _ckbUpdateAll;

		public DeviceFormEx()
		{
			InitializeComponent();
		}

		protected void UpdateControlStates()
		{
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Expected O, but got Unknown
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Expected O, but got Unknown
			IDeviceDescription val = ((_deviceCatalogue != null) ? _deviceCatalogue.SelectedDevice : null);
			if (val != null)
			{
				_iconPictureBox.Visible = true;
				Icon icon = val.DeviceInfo.Icon;
				if (icon != null)
				{
					_iconPictureBox.Image = icon.ToBitmap();
				}
				else
				{
					_iconPictureBox.Image = Bitmap.FromHicon(base.Icon.Handle);
				}
				_imagePictureBox.Visible = true;
				Image image = val.DeviceInfo.Image;
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
				_richTextBox.Clear();
				_richTextBox.SelectionAlignment = HorizontalAlignment.Left;
				IDeviceIdentification deviceIdentification = val.DeviceIdentification;
				IDeviceInfo deviceInfo = val.DeviceInfo;
				try
				{
					DeviceObjectFactory deviceObjectFactory = new DeviceObjectFactory();
					IDeviceObject val2;
					if (val.DeviceIdentification is IModuleIdentification)
					{
						string moduleId = ((IModuleIdentification)deviceIdentification).ModuleId;
						val2 = (IDeviceObject)deviceObjectFactory.Create(new string[4]
						{
							deviceIdentification.Type.ToString(),
							deviceIdentification.Id,
							deviceIdentification.Version,
							moduleId
						});
					}
					else
					{
						val2 = (IDeviceObject)deviceObjectFactory.Create(new string[3]
						{
							deviceIdentification.Type.ToString(),
							deviceIdentification.Id,
							deviceIdentification.Version
						});
					}
					if (val2 != null)
					{
						deviceInfo = val2.DeviceInfo;
					}
				}
				catch
				{
				}
				AddRichText(_richTextBox, Strings.InformationName, deviceInfo.Name);
				AddRichText(_richTextBox, Strings.InformationVendor, deviceInfo.Vendor);
				AddRichText(_richTextBox, Strings.InformationGroups, GetGroupsString(val, _deviceCatalogue.VendorSpecificStructure));
				string text = deviceIdentification.Version;
				if (Version.TryParse(text, out var result))
				{
					text = result.ToString();
				}
				AddRichText(_richTextBox, Strings.InformationVersion, text);
				AddRichText(_richTextBox, Strings.InformationModelNumber, deviceInfo.OrderNumber);
				AddRichText(_richTextBox, Strings.InformationDescription, deviceInfo.Description);
				_addDeviceButton.Enabled = true;
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
					.Length == 1)
				{
					if (_appendDeviceRadioButton.Checked)
					{
						_insertLocationLabel1.Text = Strings.AddDeviceFormEx_AppendDevice;
					}
					else if (_insertDeviceRadioButton.Checked)
					{
						_insertLocationLabel1.Text = Strings.AddDeviceFormEx_InsertDevice;
					}
					else if (_plugDeviceRadioButton.Checked)
					{
						_insertLocationLabel1.Text = Strings.AddDeviceFormEx_PlugDevice;
					}
					else if (_replaceDeviceRadioButton.Checked)
					{
						_insertLocationLabel1.Text = Strings.AddDeviceFormEx_ReplaceDevice;
					}
					else
					{
						_insertLocationLabel1.Text = string.Empty;
					}
					_insertLocationLabel2.Text = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[0].Name;
				}
				else
				{
					_insertLocationLabel1.Text = Strings.AddDeviceFormEx_Toplevel;
					_insertLocationLabel2.Text = string.Empty;
				}
			}
			else
			{
				_iconPictureBox.Visible = false;
				_imagePictureBox.Visible = false;
				_richTextBox.Clear();
				_richTextBox.SelectionAlignment = HorizontalAlignment.Center;
				using (Font selectionFont = new Font(Font, FontStyle.Italic))
				{
					_richTextBox.SelectionFont = selectionFont;
					_richTextBox.SelectedText = Strings.InformationNothingSelected;
				}
				_addDeviceButton.Enabled = false;
				_insertLocationLabel1.Text = string.Empty;
				_insertLocationLabel2.Text = string.Empty;
			}
			if (_selectedCommand != null)
			{
				string text3 = (Text = _selectedCommand.Name.Trim('.'));
				_addDeviceButton.Text = text3;
			}
		}

		private void AddRichText(RichTextBox textBox, string stCaption, string stValue)
		{
			using Font selectionFont = new Font(Font, FontStyle.Bold);
			textBox.SelectionFont = selectionFont;
			textBox.SelectedText = stCaption + " ";
			textBox.SelectionFont = Font;
			textBox.SelectedText = stValue + "\n";
		}

		private static string GetGroupsString(IDeviceDescription deviceDescription, bool bVendorSpecific)
		{
			Debug.Assert(deviceDescription != null);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			if (bVendorSpecific)
			{
				string[] families = deviceDescription.DeviceInfo.Families;
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
				int[] categories = deviceDescription.DeviceInfo.Categories;
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

		protected void OnDeviceCatalogueSelectionChanged(object sender, EventArgs e)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Expected O, but got Unknown
			IDeviceDescription selectedDevice = _deviceCatalogue.SelectedDevice;
			string value = null;
			if (selectedDevice != null)
			{
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				IDeviceIdentification deviceIdentification = selectedDevice.DeviceIdentification;
				IDeviceInfo deviceInfo = selectedDevice.DeviceInfo;
				try
				{
					DeviceObjectFactory deviceObjectFactory = new DeviceObjectFactory();
					IDeviceObject val;
					if (selectedDevice.DeviceIdentification is IModuleIdentification)
					{
						string moduleId = ((IModuleIdentification)deviceIdentification).ModuleId;
						val = (IDeviceObject)deviceObjectFactory.Create(new string[4]
						{
							deviceIdentification.Type.ToString(),
							deviceIdentification.Id,
							deviceIdentification.Version,
							moduleId
						});
					}
					else
					{
						val = (IDeviceObject)deviceObjectFactory.Create(new string[3]
						{
							deviceIdentification.Type.ToString(),
							deviceIdentification.Id,
							deviceIdentification.Version
						});
					}
					if (val != null)
					{
						deviceInfo = val.DeviceInfo;
					}
				}
				catch
				{
				}
				if (_selectedCommand is PlugDeviceCmd)
				{
					IDeviceObject selectedDevice2 = DeviceCommandHelper.GetSelectedDevice();
					if (selectedDevice2 != null)
					{
						if (((object)selectedDevice.DeviceIdentification).Equals((object)selectedDevice2.DeviceIdentification))
						{
							value = ((IObject)selectedDevice2).MetaObject.Name;
						}
						if (selectedDevice2 is SlotDeviceObject)
						{
							IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, ((IObject)selectedDevice2).MetaObject.ParentObjectGuid).Object;
							IDeviceObject val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
							if (val2 != null)
							{
								foreach (IConnector item in (IEnumerable)val2.Connectors)
								{
									foreach (IAdapter item2 in (IEnumerable)item.Adapters)
									{
										IAdapter val3 = item2;
										int num = Array.IndexOf(val3.Modules, ((IObject)selectedDevice2).MetaObject.ObjectGuid);
										if (!(val3 is SlotAdapter) || num < 0 || (val3 as SlotAdapter).SlotNames == null)
										{
											continue;
										}
										string slotName = (val3 as SlotAdapter).GetSlotName(num);
										if (!slotName.StartsWith("<"))
										{
											slotName = DeviceObjectHelper.BuildIecIdentifier(slotName);
											IMetaObjectStub host = null;
											if (!DeviceObjectHelper.IsDeviceWithPlcLogic((IDeviceDescription10)(object)((selectedDevice is IDeviceDescription10) ? selectedDevice : null)))
											{
												host = DeviceCommandHelper.GetHostFromSelectedStub();
											}
											value = DeviceObjectHelper.CreateUniqueIdentifier(handle, slotName, Guid.Empty, host, bCheckAll: false, bCheckLogical: false);
										}
										else
										{
											value = slotName;
										}
									}
								}
							}
						}
					}
				}
				if (string.IsNullOrEmpty(value))
				{
					IMetaObjectStub host2 = null;
					if (!DeviceObjectHelper.IsDeviceWithPlcLogic((IDeviceDescription10)(object)((selectedDevice is IDeviceDescription10) ? selectedDevice : null)))
					{
						host2 = DeviceCommandHelper.GetHostFromSelectedStub();
					}
					string stBaseName = DeviceObjectHelper.CreateInstanceNameBase(deviceInfo);
					value = DeviceObjectHelper.CreateUniqueIdentifier(handle, stBaseName, Guid.Empty, host2, bCheckAll: false, bCheckLogical: false);
				}
			}
			else
			{
				value = string.Empty;
			}
			if (_selectedCommand != null && _selectedCommand.GetFixObjectName() == null)
			{
				_objectNameTextBox.Text = value;
			}
			UpdateControlStates();
		}

		protected bool IsValidIdentifier(string st)
		{
			IScanner obj = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(st, true, true, true, true);
			Debug.Assert(obj != null);
			IToken val = default(IToken);
			return obj.Match((TokenType)13, true, out val) > 0;
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
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.DeviceFormEx));
			_splitContainer = new System.Windows.Forms.SplitContainer();
			_deviceListPanel = new System.Windows.Forms.Panel();
			_richTextBox = new System.Windows.Forms.RichTextBox();
			_imagePictureBox = new System.Windows.Forms.PictureBox();
			_iconPictureBox = new System.Windows.Forms.PictureBox();
			_replaceDeviceRadioButton = new System.Windows.Forms.RadioButton();
			_plugDeviceRadioButton = new System.Windows.Forms.RadioButton();
			_insertDeviceRadioButton = new System.Windows.Forms.RadioButton();
			_appendDeviceRadioButton = new System.Windows.Forms.RadioButton();
			_insertLocationLabel2 = new System.Windows.Forms.Label();
			_insertLocationLabel1 = new System.Windows.Forms.Label();
			_objectNameTextBox = new System.Windows.Forms.TextBox();
			_closeButton = new System.Windows.Forms.Button();
			_addDeviceButton = new System.Windows.Forms.Button();
			errorProvider1 = new System.Windows.Forms.ErrorProvider(components);
			_ckbUpdateAll = new System.Windows.Forms.CheckBox();
			System.Windows.Forms.Label label = new System.Windows.Forms.Label();
			System.Windows.Forms.GroupBox groupBox = new System.Windows.Forms.GroupBox();
			System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
			System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
			System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
			_splitContainer.Panel1.SuspendLayout();
			_splitContainer.Panel2.SuspendLayout();
			_splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).BeginInit();
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).BeginInit();
			groupBox.SuspendLayout();
			panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
			SuspendLayout();
			componentResourceManager.ApplyResources(label, "label1");
			label.Name = "label1";
			componentResourceManager.ApplyResources(_splitContainer, "_splitContainer");
			_splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_splitContainer.Name = "_splitContainer";
			_splitContainer.Panel1.Controls.Add(_deviceListPanel);
			componentResourceManager.ApplyResources(_splitContainer.Panel1, "_splitContainer.Panel1");
			_splitContainer.Panel2.Controls.Add(_richTextBox);
			_splitContainer.Panel2.Controls.Add(_imagePictureBox);
			_splitContainer.Panel2.Controls.Add(_iconPictureBox);
			componentResourceManager.ApplyResources(_deviceListPanel, "_deviceListPanel");
			_deviceListPanel.Name = "_deviceListPanel";
			componentResourceManager.ApplyResources(_richTextBox, "_richTextBox");
			_richTextBox.BackColor = System.Drawing.SystemColors.Control;
			_richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_richTextBox.Name = "_richTextBox";
			_richTextBox.ReadOnly = true;
			componentResourceManager.ApplyResources(_imagePictureBox, "_imagePictureBox");
			_imagePictureBox.Name = "_imagePictureBox";
			_imagePictureBox.TabStop = false;
			componentResourceManager.ApplyResources(_iconPictureBox, "_iconPictureBox");
			_iconPictureBox.Name = "_iconPictureBox";
			_iconPictureBox.TabStop = false;
			componentResourceManager.ApplyResources(groupBox, "groupBox4");
			groupBox.Controls.Add(_ckbUpdateAll);
			groupBox.Controls.Add(_replaceDeviceRadioButton);
			groupBox.Controls.Add(_plugDeviceRadioButton);
			groupBox.Controls.Add(_insertDeviceRadioButton);
			groupBox.Controls.Add(_appendDeviceRadioButton);
			groupBox.Name = "groupBox4";
			groupBox.TabStop = false;
			componentResourceManager.ApplyResources(_replaceDeviceRadioButton, "_replaceDeviceRadioButton");
			_replaceDeviceRadioButton.Name = "_replaceDeviceRadioButton";
			_replaceDeviceRadioButton.TabStop = true;
			_replaceDeviceRadioButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_plugDeviceRadioButton, "_plugDeviceRadioButton");
			_plugDeviceRadioButton.Name = "_plugDeviceRadioButton";
			_plugDeviceRadioButton.TabStop = true;
			_plugDeviceRadioButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_insertDeviceRadioButton, "_insertDeviceRadioButton");
			_insertDeviceRadioButton.Name = "_insertDeviceRadioButton";
			_insertDeviceRadioButton.TabStop = true;
			_insertDeviceRadioButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_appendDeviceRadioButton, "_appendDeviceRadioButton");
			_appendDeviceRadioButton.Name = "_appendDeviceRadioButton";
			_appendDeviceRadioButton.TabStop = true;
			_appendDeviceRadioButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(panel, "panel1");
			panel.BackColor = System.Drawing.SystemColors.Info;
			panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			panel.Controls.Add(label2);
			panel.Controls.Add(label3);
			panel.Controls.Add(_insertLocationLabel2);
			panel.Controls.Add(_insertLocationLabel1);
			panel.Name = "panel1";
			componentResourceManager.ApplyResources(label2, "label2");
			label2.Name = "label2";
			componentResourceManager.ApplyResources(label3, "label4");
			label3.ForeColor = System.Drawing.Color.Blue;
			label3.Name = "label4";
			componentResourceManager.ApplyResources(_insertLocationLabel2, "_insertLocationLabel2");
			_insertLocationLabel2.Name = "_insertLocationLabel2";
			componentResourceManager.ApplyResources(_insertLocationLabel1, "_insertLocationLabel1");
			_insertLocationLabel1.Name = "_insertLocationLabel1";
			componentResourceManager.ApplyResources(_objectNameTextBox, "_objectNameTextBox");
			_objectNameTextBox.Name = "_objectNameTextBox";
			componentResourceManager.ApplyResources(_closeButton, "_closeButton");
			_closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_closeButton.Name = "_closeButton";
			_closeButton.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(_addDeviceButton, "_addDeviceButton");
			_addDeviceButton.Name = "_addDeviceButton";
			_addDeviceButton.UseVisualStyleBackColor = true;
			errorProvider1.ContainerControl = this;
			componentResourceManager.ApplyResources(_ckbUpdateAll, "_ckbUpdateAll");
			_ckbUpdateAll.Name = "_ckbUpdateAll";
			_ckbUpdateAll.UseVisualStyleBackColor = true;
			base.AcceptButton = _addDeviceButton;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = _closeButton;
			base.Controls.Add(panel);
			base.Controls.Add(_splitContainer);
			base.Controls.Add(groupBox);
			base.Controls.Add(_addDeviceButton);
			base.Controls.Add(_closeButton);
			base.Controls.Add(label);
			base.Controls.Add(_objectNameTextBox);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DeviceFormEx";
			base.ShowInTaskbar = false;
			_splitContainer.Panel1.ResumeLayout(false);
			_splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
			_splitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).EndInit();
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).EndInit();
			groupBox.ResumeLayout(false);
			groupBox.PerformLayout();
			panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
