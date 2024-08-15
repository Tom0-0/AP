using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.TargetSettings;

namespace _3S.CoDeSys.DeviceObject
{
	public class ScanImportDeviceForm : DeviceFormEx, ICreateDeviceDialog
	{
		private bool _bAddingDevice;

		private static bool s_bOneInstanceVisible;

		private InitialAddDeviceMode mode;

		private IDeviceCatalogueFilter customFilter;

		private IMetaObject target;

		private AddDeviceCmd addCmd;

		private InsertDeviceCmd insertCmd;

		private ReplaceDeviceCmd replaceCmd;

		private PlugDeviceCmd plugCmd;

		private Guid createdDevice;

		public IDeviceDescription SelectedDeviceDescription => _deviceCatalogue.SelectedDevice;

		internal InitialAddDeviceMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}

		internal static bool OneInstanceVisible => s_bOneInstanceVisible;

		public IDeviceCatalogue DeviceCatalogue => _deviceCatalogue;

		public IDeviceCatalogueFilter CustomFilter
		{
			get
			{
				return customFilter;
			}
			set
			{
				customFilter = value;
			}
		}

		public Guid CreatedDevice => createdDevice;

		public event EventHandler DeviceCreated;

		public ScanImportDeviceForm()
		{
			CreateCommands();
			_replaceDeviceRadioButton.CheckedChanged += _replaceDeviceRadioButton_CheckedChanged;
			_plugDeviceRadioButton.CheckedChanged += _plugDeviceRadioButton_CheckedChanged;
			_insertDeviceRadioButton.CheckedChanged += _insertDeviceRadioButton_CheckedChanged;
			_appendDeviceRadioButton.CheckedChanged += _appendDeviceRadioButton_CheckedChanged;
			_objectNameTextBox.TextChanged += _objectNameTextBox_TextChanged;
			_closeButton.Click += _closeButton_Click;
			_addDeviceButton.Click += _addDeviceButton_Click;
			Initialize(InitialAddDeviceMode.Append);
		}

		internal void Initialize(InitialAddDeviceMode mode)
		{
			switch (mode)
			{
			default:
				_appendDeviceRadioButton.Checked = true;
				break;
			case InitialAddDeviceMode.Insert:
				_insertDeviceRadioButton.Checked = true;
				break;
			case InitialAddDeviceMode.Plug:
				_plugDeviceRadioButton.Checked = true;
				break;
			case InitialAddDeviceMode.Replace:
				_replaceDeviceRadioButton.Checked = true;
				break;
			}
			if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("DeviceObject", "HideUpdateDeviceRadioButton") && ((IEngine3)APEnvironment.Engine).OEMCustomization.GetBoolValue("DeviceObject", "HideUpdateDeviceRadioButton"))
			{
				_replaceDeviceRadioButton.Visible = false;
				if (mode == InitialAddDeviceMode.Replace)
				{
					throw new InvalidOperationException("Cannot invoke this dialog because the 'Update Device' radio button is disabled by OEMCustomization.");
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Rectangle bounds = base.Bounds;
			try
			{
				bounds.Y = 3 * SystemInformation.CaptionHeight;
				bounds.Height = Screen.FromControl(this).WorkingArea.Height - 6 * SystemInformation.CaptionHeight;
			}
			catch
			{
			}
			base.Bounds = bounds;
			CenterToParent();
			if (!base.DesignMode)
			{
				InitDeviceCatalogue();
				UpdateSelectedCommand();
			}
			if (_splitContainer.Height - 172 >= 0)
			{
				_splitContainer.SplitterDistance = _splitContainer.Height - 172;
			}
		}

		private void InitDeviceCatalogue()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (_deviceCatalogue == null)
			{
				_deviceCatalogue = APEnvironment.CreateFirstDeviceCatalogue();
				_deviceCatalogue.Control.Dock = DockStyle.Fill;
				_deviceListPanel.Controls.Add(_deviceCatalogue.Control);
				_deviceCatalogue.SelectionChanged+=((EventHandler)base.OnDeviceCatalogueSelectionChanged);
				if (_deviceCatalogue is IDeviceCatalogue2)
				{
					((IDeviceCatalogue2)_deviceCatalogue).CatalogueTreeDoubleClick+=((EventHandler)OnDeviceCatalogueDoubleClick);
				}
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (s_bOneInstanceVisible)
			{
				throw new InvalidOperationException("Only one instance of dialog must appear at the same time.");
			}
			s_bOneInstanceVisible = true;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			base.OnHandleDestroyed(e);
			s_bOneInstanceVisible = false;
			if (_deviceCatalogue != null && _deviceCatalogue is IDeviceCatalogue2)
			{
				((IDeviceCatalogue2)_deviceCatalogue).CatalogueTreeDoubleClick-=((EventHandler)OnDeviceCatalogueDoubleClick);
			}
		}

		public bool GetDeviceCatalogConfig(IMetaObject target, out string[] filter, out IDeviceIdentification context, out IDeviceIdentification[] AllowOnlyDevices)
		{
			filter = null;
			context = null;
			AllowOnlyDevices = null;
			if (_selectedCommand == null || !_selectedCommand.GetFilterAndContext(out filter, out context, out AllowOnlyDevices) || !_selectedCommand.Selectable)
			{
				filter = new string[0];
			}
			return true;
		}

		public void ChangeTarget(IMetaObject target)
		{
			this.target = target;
			UpdateSelectedCommand();
		}

		private void CreateCommands()
		{
			addCmd = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(AddDeviceCmd))) as AddDeviceCmd;
			insertCmd = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(InsertDeviceCmd))) as InsertDeviceCmd;
			replaceCmd = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(ReplaceDeviceCmd))) as ReplaceDeviceCmd;
			plugCmd = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(PlugDeviceCmd))) as PlugDeviceCmd;
		}

		private void UpdateSelectedCommand()
		{
			if (_bAddingDevice)
			{
				return;
			}
			if (_appendDeviceRadioButton.Checked)
			{
				_selectedCommand = addCmd;
			}
			else if (_insertDeviceRadioButton.Checked)
			{
				_selectedCommand = insertCmd;
			}
			else if (_replaceDeviceRadioButton.Checked)
			{
				_selectedCommand = replaceCmd;
			}
			else
			{
				if (!_plugDeviceRadioButton.Checked)
				{
					_selectedCommand = null;
					return;
				}
				_selectedCommand = plugCmd;
			}
			if (_deviceCatalogue == null || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return;
			}
			string[] filter = null;
			IDeviceIdentification context = null;
			IDeviceIdentification[] AllowOnlyDevices = null;
			if (_selectedCommand == null || !_selectedCommand.GetFilterAndContext(out filter, out context, out AllowOnlyDevices) || !_selectedCommand.Selectable)
			{
				filter = new string[0];
			}
			try
			{
				_deviceCatalogue.BeginUpdate();
				CompositeDeviceCatalogFilter compositeDeviceCatalogFilter = new CompositeDeviceCatalogFilter();
				compositeDeviceCatalogFilter.childConnectorFilter = _deviceCatalogue.CreateChildConnectorFilter(filter);
				compositeDeviceCatalogFilter.customFilter = CustomFilter;
				_deviceCatalogue.Filter=((IDeviceCatalogueFilter)(object)compositeDeviceCatalogFilter);
				_deviceCatalogue.Context=(context);
				if (_deviceCatalogue is IDeviceCatalogue3)
				{
					IDeviceCatalogue deviceCatalogue = _deviceCatalogue;
					((IDeviceCatalogue3)((deviceCatalogue is IDeviceCatalogue3) ? deviceCatalogue : null)).AllowOnly=(AllowOnlyDevices);
				}
				if (_deviceCatalogue is IDeviceCatalogue3)
				{
					if (_selectedCommand is ReplaceDeviceCmd)
					{
						IDeviceObject hostFromSelectedDevice = DeviceCommandHelper.GetHostFromSelectedDevice();
						if (hostFromSelectedDevice != null)
						{
							ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)((hostFromSelectedDevice is IDeviceObject5) ? hostFromSelectedDevice : null)).DeviceIdentificationNoSimulation);
							if (LocalTargetSettings.UpdateOnlyDeviceVersion.GetBoolValue(targetSettingsById))
							{
								IDeviceObject selectedDevice = DeviceCommandHelper.GetSelectedDevice();
								IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((selectedDevice is IDeviceObject5) ? selectedDevice : null)).DeviceIdentificationNoSimulation;
								if (selectedDevice != null)
								{
									IDeviceCatalogue deviceCatalogue2 = _deviceCatalogue;
									((IDeviceCatalogue3)((deviceCatalogue2 is IDeviceCatalogue3) ? deviceCatalogue2 : null)).VersionMatch=(deviceIdentificationNoSimulation);
								}
							}
							else
							{
								IDeviceCatalogue deviceCatalogue3 = _deviceCatalogue;
								((IDeviceCatalogue3)((deviceCatalogue3 is IDeviceCatalogue3) ? deviceCatalogue3 : null)).VersionMatch=((IDeviceIdentification)null);
							}
						}
					}
					else
					{
						IDeviceCatalogue deviceCatalogue4 = _deviceCatalogue;
						((IDeviceCatalogue3)((deviceCatalogue4 is IDeviceCatalogue3) ? deviceCatalogue4 : null)).VersionMatch=((IDeviceIdentification)null);
					}
				}
			}
			finally
			{
				_deviceCatalogue.EndUpdate();
				UpdateControlStates();
			}
			if (_selectedCommand != null && _selectedCommand.GetFixObjectName() != null)
			{
				_objectNameTextBox.Text = _selectedCommand.GetFixObjectName();
				_objectNameTextBox.Enabled = false;
			}
			else
			{
				_objectNameTextBox.Enabled = true;
			}
			_objectNameTextBox_TextChanged(_objectNameTextBox, new EventArgs());
		}

		private void OnDeviceCatalogueDoubleClick(object sender, EventArgs e)
		{
			if (_deviceCatalogue.SelectedDevice != null)
			{
				_addDeviceButton_Click(this, EventArgs.Empty);
			}
		}

		private void _appendDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_appendDeviceRadioButton.Checked)
			{
				mode = InitialAddDeviceMode.Append;
				UpdateSelectedCommand();
			}
		}

		private void _insertDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_insertDeviceRadioButton.Checked)
			{
				mode = InitialAddDeviceMode.Insert;
				UpdateSelectedCommand();
			}
		}

		private void _plugDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_plugDeviceRadioButton.Checked)
			{
				mode = InitialAddDeviceMode.Plug;
				UpdateSelectedCommand();
			}
		}

		private void _replaceDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_replaceDeviceRadioButton.Checked)
			{
				mode = InitialAddDeviceMode.Replace;
				UpdateSelectedCommand();
			}
		}

		private void _closeButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void _addDeviceButton_Click(object sender, EventArgs e)
		{
			if (_selectedCommand == null || _deviceCatalogue.SelectedDevice == null)
			{
				return;
			}
			try
			{
				_bAddingDevice = true;
				Cursor = Cursors.WaitCursor;
				base.Enabled = false;
				_=_deviceCatalogue.SelectedDevice;
				string[] array = _selectedCommand.CreateBatchArguments(_deviceCatalogue.SelectedDevice.DeviceIdentification, _objectNameTextBox.Text);
				if (array != null)
				{
					APEnvironment.OnlineUIMgr.BeginAllowOnlineModification();
					try
					{
						_selectedCommand.OverridableExecuteBatch(array);
					}
					finally
					{
						APEnvironment.OnlineUIMgr.EndAllowOnlineModification();
					}
				}
				if (_selectedCommand is AddDeviceCmd)
				{
					AbstractDeviceCmd abstractDeviceCmd = _selectedCommand as AbstractDeviceCmd;
					createdDevice = abstractDeviceCmd.DeviceGuid;
				}
				else if (_selectedCommand is InsertDeviceCmd)
				{
					AbstractDeviceCmd abstractDeviceCmd2 = _selectedCommand as AbstractDeviceCmd;
					createdDevice = abstractDeviceCmd2.DeviceGuid;
				}
				else if (_selectedCommand is PlugDeviceCmd)
				{
					createdDevice = target.ObjectGuid;
				}
				else if (_selectedCommand is ReplaceDeviceCmd)
				{
					createdDevice = target.ObjectGuid;
				}
				CreateCommands();
				_=((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				if (this.DeviceCreated != null)
				{
					this.DeviceCreated(this, new EventArgs());
				}
			}
			catch (Exception ex)
			{
				APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
			}
			finally
			{
				base.Enabled = true;
				Cursor = Cursors.Default;
				_bAddingDevice = false;
				Close();
			}
		}

		public string GetOnlineHelpKeyword()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			if (_selectedCommand == null)
			{
				return null;
			}
			return _selectedCommand.GetOnlineHelpUrl();
		}

		private void _objectNameTextBox_TextChanged(object sender, EventArgs e)
		{
			string text = null;
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (_objectNameTextBox.Text == "")
			{
				text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameEmpty");
			}
			else if (!IsValidIdentifier(_objectNameTextBox.Text))
			{
				text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameInvalid");
			}
			else if (!_replaceDeviceRadioButton.Checked && !DeviceObjectHelper.CheckUniqueIdentifier(primaryProject.Handle, _objectNameTextBox.Text, DeviceCommandHelper.GetSelectedStub()))
			{
				text = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameNotUnique");
			}
			if (text != null)
			{
				errorProvider1.SetError(_objectNameTextBox, text);
				_addDeviceButton.Enabled = false;
			}
			else
			{
				_addDeviceButton.Enabled = true;
				errorProvider1.SetError(_objectNameTextBox, null);
			}
		}
	}
}
