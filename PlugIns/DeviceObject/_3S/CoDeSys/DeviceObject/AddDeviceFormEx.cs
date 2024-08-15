using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class AddDeviceFormEx : DeviceFormEx, IHasAssociatedOnlineHelpTopic
	{
		private bool _bAddingDevice;

		private static bool s_bOneInstanceVisible;

		private static AddDeviceFormEx s_instance;

		private bool _bUpdateSelectedCommand;

		private IUndoManager2 _undoMgr;

		private bool _bUndoMgrActive;

		internal static bool OneInstanceVisible => s_bOneInstanceVisible;

		internal static AddDeviceFormEx GetInstance => s_instance;

		public AddDeviceFormEx()
		{
			_replaceDeviceRadioButton.CheckedChanged += _replaceDeviceRadioButton_CheckedChanged;
			_plugDeviceRadioButton.CheckedChanged += _plugDeviceRadioButton_CheckedChanged;
			_insertDeviceRadioButton.CheckedChanged += _insertDeviceRadioButton_CheckedChanged;
			_appendDeviceRadioButton.CheckedChanged += _appendDeviceRadioButton_CheckedChanged;
			_objectNameTextBox.TextChanged += _objectNameTextBox_TextChanged;
			_closeButton.Click += _closeButton_Click;
			_addDeviceButton.Click += _addDeviceButton_Click;
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
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			base.Location = UserOptionsHelper.AddDeviceWindowLocation;
			base.Size = UserOptionsHelper.AddDeviceWindowSize;
			UIHelper.AssureFormIsVisibleOnScreen((Form)this);
			if (UserOptionsHelper.AddDeviceWindowContainerHeight != 0)
			{
				_splitContainer.SplitterDistance = UserOptionsHelper.AddDeviceWindowContainerHeight;
			}
			base.OnLoad(e);
			if (UserOptionsHelper.AddDeviceWindowContainerHeight == 0)
			{
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
			}
			if (!base.DesignMode)
			{
				_deviceCatalogue = APEnvironment.CreateFirstDeviceCatalogue();
				APEnvironment.DpiAdapter.AdaptControl(_deviceCatalogue.Control);
				_deviceCatalogue.Control.Dock = DockStyle.Fill;
				_deviceListPanel.Controls.Add(_deviceCatalogue.Control);
				_deviceCatalogue.SelectionChanged+=((EventHandler)base.OnDeviceCatalogueSelectionChanged);
				if (_deviceCatalogue is IDeviceCatalogue2)
				{
					((IDeviceCatalogue2)_deviceCatalogue).CatalogueTreeDoubleClick+=((EventHandler)OnDeviceCatalogueDoubleClick);
				}
				if (_deviceCatalogue is IDeviceCatalogue7)
				{
					((IDeviceCatalogue7)_deviceCatalogue).EnableDrag=(true);
				}
				UpdateSelectedCommandAndDeviceCatalogue();
			}
			if (UserOptionsHelper.AddDeviceWindowContainerHeight == 0 && _splitContainer.Height - 172 >= 0)
			{
				_splitContainer.SplitterDistance = _splitContainer.Height - 172;
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);
			UserOptionsHelper.AddDeviceWindowLocation = base.Location;
			if (base.WindowState == FormWindowState.Normal)
			{
				UserOptionsHelper.AddDeviceWindowSize = base.Size;
			}
			else
			{
				UserOptionsHelper.AddDeviceWindowSize = base.RestoreBounds.Size;
			}
			UserOptionsHelper.AddDeviceWindowContainerHeight = _splitContainer.SplitterDistance;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			base.OnHandleCreated(e);
			if (s_bOneInstanceVisible)
			{
				throw new InvalidOperationException("Only one instance of dialog must appear at the same time.");
			}
			s_bOneInstanceVisible = true;
			s_instance = this;
			((IEngine)APEnvironment.Engine).Projects.PrimaryProjectSwitched+=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
			((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectionChanged+=(new ProjectChangedEventHandler(OnGlobalObjectSelectionChanged));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved+=(new ObjectRemovedEventHandler(ObjectMgr_ObjectRemoved));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded+=(new ObjectEventHandler(ObjectMgr_ObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRenamed+=(new ObjectRenamedEventHandler(ObjectMgr_ObjectRenamed));
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			base.OnHandleDestroyed(e);
			s_bOneInstanceVisible = false;
			s_instance = null;
			((IEngine)APEnvironment.Engine).Projects.PrimaryProjectSwitched-=(new PrimaryProjectSwitchedEventHandler(OnPrimaryProjectSwitched));
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectionChanged-=(new ProjectChangedEventHandler(OnGlobalObjectSelectionChanged));
			}
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoved-=(new ObjectRemovedEventHandler(ObjectMgr_ObjectRemoved));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectAdded-=(new ObjectEventHandler(ObjectMgr_ObjectAdded));
			((IObjectManager)APEnvironment.ObjectMgr).ObjectRenamed-=(new ObjectRenamedEventHandler(ObjectMgr_ObjectRenamed));
			if (_undoMgr != null)
			{
				_undoMgr.AfterEndCompoundAction-=((EventHandler)_undoMgr_AfterEndCompoundAction);
			}
		}

		private void UpdateSelectedCommandAndDeviceCatalogue()
		{
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_bUpdateSelectedCommand)
				{
					return;
				}
				_bUpdateSelectedCommand = true;
				_appendDeviceRadioButton.Enabled = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(AddDeviceCmd))).Enabled;
				_insertDeviceRadioButton.Enabled = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(InsertDeviceCmd))).Enabled;
				_plugDeviceRadioButton.Enabled = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(PlugDeviceCmd))).Enabled;
				_replaceDeviceRadioButton.Enabled = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(ReplaceDeviceCmd))).Enabled;
				if ((_appendDeviceRadioButton.Checked && !_appendDeviceRadioButton.Enabled) || (_insertDeviceRadioButton.Checked && !_insertDeviceRadioButton.Enabled) || (_plugDeviceRadioButton.Checked && !_plugDeviceRadioButton.Enabled) || (_replaceDeviceRadioButton.Checked && !_replaceDeviceRadioButton.Enabled))
				{
					if (_appendDeviceRadioButton.Enabled)
					{
						_appendDeviceRadioButton.Checked = true;
					}
					else if (_insertDeviceRadioButton.Enabled)
					{
						_insertDeviceRadioButton.Checked = true;
					}
					else if (_plugDeviceRadioButton.Enabled)
					{
						_plugDeviceRadioButton.Checked = true;
					}
				}
				if (_appendDeviceRadioButton.Checked)
				{
					_selectedCommand = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(AddDeviceCmd))) as IInteractiveDeviceCommand;
					goto IL_0272;
				}
				if (_insertDeviceRadioButton.Checked)
				{
					_selectedCommand = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(InsertDeviceCmd))) as IInteractiveDeviceCommand;
					goto IL_0272;
				}
				if (_replaceDeviceRadioButton.Checked)
				{
					_selectedCommand = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(ReplaceDeviceCmd))) as IInteractiveDeviceCommand;
					goto IL_0272;
				}
				if (_plugDeviceRadioButton.Checked)
				{
					_selectedCommand = ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(Common.GetTypeGuid(typeof(PlugDeviceCmd))) as IInteractiveDeviceCommand;
					goto IL_0272;
				}
				_selectedCommand = null;
				goto end_IL_0000;
				IL_0272:
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
				bool flag = false;
				if (_selectedCommand is ReplaceDeviceCmd && DeviceObjectHelper.GenerateCodeForLogicalDevices)
				{
					IDeviceObject selectedDevice = DeviceCommandHelper.GetSelectedDevice();
					ILogicalDevice val = (ILogicalDevice)(object)((selectedDevice is ILogicalDevice) ? selectedDevice : null);
					if (val != null && val.IsLogical && DeviceCommandHelper.IsUpdateForLogicalDevicesEnabled((IDeviceObject)(object)((val is IDeviceObject) ? val : null)))
					{
						flag = true;
					}
					if (val != null && val.IsPhysical && val.MappedDevices != null)
					{
						foreach (IMappedDevice item in (IEnumerable)val.MappedDevices)
						{
							if (item.IsMapped)
							{
								string[] array = new string[filter.Length + 1];
								filter.CopyTo(array, 0);
								array[filter.Length] = string.Concat("{", UpdatePhysicalDeviceFilter.TypeGuid, "}");
								filter = array;
								break;
							}
						}
					}
				}
				try
				{
					_deviceCatalogue.BeginUpdate();
					if (flag)
					{
						_deviceCatalogue.Filter=((IDeviceCatalogueFilter)(object)new LogicalIoFilter(filter));
					}
					else
					{
						_deviceCatalogue.Filter=(_deviceCatalogue.CreateChildConnectorFilter(filter));
					}
					_deviceCatalogue.Context=(context);
					if (_deviceCatalogue is IDeviceCatalogue6)
					{
						if (_appendDeviceRadioButton.Checked)
						{
							IDeviceCatalogue deviceCatalogue = _deviceCatalogue;
							((IDeviceCatalogue6)((deviceCatalogue is IDeviceCatalogue6) ? deviceCatalogue : null)).AddDeviceContext=((AddDeviceContext)1);
						}
						else if (_insertDeviceRadioButton.Checked)
						{
							IDeviceCatalogue deviceCatalogue2 = _deviceCatalogue;
							((IDeviceCatalogue6)((deviceCatalogue2 is IDeviceCatalogue6) ? deviceCatalogue2 : null)).AddDeviceContext=((AddDeviceContext)3);
						}
						else if (_replaceDeviceRadioButton.Checked)
						{
							IDeviceCatalogue deviceCatalogue3 = _deviceCatalogue;
							((IDeviceCatalogue6)((deviceCatalogue3 is IDeviceCatalogue6) ? deviceCatalogue3 : null)).AddDeviceContext=((AddDeviceContext)2);
						}
						else if (_plugDeviceRadioButton.Checked)
						{
							IDeviceCatalogue deviceCatalogue4 = _deviceCatalogue;
							((IDeviceCatalogue6)((deviceCatalogue4 is IDeviceCatalogue6) ? deviceCatalogue4 : null)).AddDeviceContext=((AddDeviceContext)4);
						}
						else
						{
							IDeviceCatalogue deviceCatalogue5 = _deviceCatalogue;
							((IDeviceCatalogue6)((deviceCatalogue5 is IDeviceCatalogue6) ? deviceCatalogue5 : null)).AddDeviceContext=((AddDeviceContext)0);
						}
					}
					if (_deviceCatalogue is IDeviceCatalogue3)
					{
						IDeviceCatalogue deviceCatalogue6 = _deviceCatalogue;
						((IDeviceCatalogue3)((deviceCatalogue6 is IDeviceCatalogue3) ? deviceCatalogue6 : null)).AllowOnly=(AllowOnlyDevices);
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
									IDeviceObject selectedDevice2 = DeviceCommandHelper.GetSelectedDevice();
									if (selectedDevice2 != null)
									{
										IDeviceCatalogue deviceCatalogue7 = _deviceCatalogue;
										((IDeviceCatalogue3)((deviceCatalogue7 is IDeviceCatalogue3) ? deviceCatalogue7 : null)).VersionMatch=(((IDeviceObject5)((selectedDevice2 is IDeviceObject5) ? selectedDevice2 : null)).DeviceIdentificationNoSimulation);
									}
								}
								else
								{
									IDeviceCatalogue deviceCatalogue8 = _deviceCatalogue;
									((IDeviceCatalogue3)((deviceCatalogue8 is IDeviceCatalogue3) ? deviceCatalogue8 : null)).VersionMatch=((IDeviceIdentification)null);
								}
							}
						}
						else
						{
							IDeviceCatalogue deviceCatalogue9 = _deviceCatalogue;
							((IDeviceCatalogue3)((deviceCatalogue9 is IDeviceCatalogue3) ? deviceCatalogue9 : null)).VersionMatch=((IDeviceIdentification)null);
						}
					}
				}
				finally
				{
					_deviceCatalogue.EndUpdate();
					UpdateControlStates();
				}
				if (_selectedCommand is ReplaceDeviceCmd)
				{
					IList<IDeviceObject> selectedDevices = DeviceCommandHelper.GetSelectedDevices();
					if (selectedDevices != null)
					{
						DeviceIdentification deviceIdentification = null;
						foreach (IDeviceObject item2 in selectedDevices)
						{
							IDeviceObject5 val2 = (IDeviceObject5)(object)((item2 is IDeviceObject5) ? item2 : null);
							if (val2 == null)
							{
								continue;
							}
							DeviceIdentification deviceIdentification2 = val2.DeviceIdentificationNoSimulation as DeviceIdentification;
							if (deviceIdentification2 != null)
							{
								DeviceIdentification deviceIdentification3 = null;
								deviceIdentification3 = ((!(deviceIdentification2 is ModuleIdentification)) ? new DeviceIdentification((IDeviceIdentification)(object)deviceIdentification2) : new ModuleIdentification((IModuleIdentification)(object)(deviceIdentification2 as ModuleIdentification)));
								deviceIdentification3.Version = "*";
								if (deviceIdentification == null)
								{
									deviceIdentification = deviceIdentification3;
								}
								else if (!((object)deviceIdentification).Equals((object)deviceIdentification3))
								{
									deviceIdentification = null;
									break;
								}
							}
						}
						if (deviceIdentification != null)
						{
							IDeviceCatalogue deviceCatalogue10 = _deviceCatalogue;
							((IDeviceCatalogue4)((deviceCatalogue10 is IDeviceCatalogue4) ? deviceCatalogue10 : null)).SelectedDeviceIds=((IDeviceIdentification[])(object)new IDeviceIdentification[1] { deviceIdentification });
						}
					}
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
				end_IL_0000:;
			}
			finally
			{
				_bUpdateSelectedCommand = false;
			}
		}

		private void SetCompound(int nProjectHandle)
		{
			ref IUndoManager2 undoMgr = ref _undoMgr;
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(nProjectHandle);
			undoMgr = (IUndoManager2)(object)((undoManager is IUndoManager2) ? undoManager : null);
			if (_undoMgr != null && (((IUndoManager)_undoMgr).InCompoundAction || ((IUndoManager)_undoMgr).InRedo || ((IUndoManager)_undoMgr).InUndo))
			{
				if (!_bUndoMgrActive)
				{
					_undoMgr.AfterEndCompoundAction+=((EventHandler)_undoMgr_AfterEndCompoundAction);
					_bUndoMgrActive = true;
				}
			}
			else
			{
				UpdateSelectedCommandAndDeviceCatalogue();
			}
		}

		private void _undoMgr_AfterEndCompoundAction(object sender, EventArgs e)
		{
			_bUndoMgrActive = false;
			_undoMgr.AfterEndCompoundAction-=((EventHandler)_undoMgr_AfterEndCompoundAction);
			UpdateSelectedCommandAndDeviceCatalogue();
		}

		private void ObjectMgr_ObjectRemoved(object sender, ObjectRemovedEventArgs e)
		{
			if (e.MetaObject != null)
			{
				SetCompound(e.MetaObject.ProjectHandle);
			}
		}

		private void ObjectMgr_ObjectAdded(object sender, ObjectEventArgs e)
		{
			SetCompound(e.ProjectHandle);
		}

		private void ObjectMgr_ObjectRenamed(object sender, ObjectRenamedEventArgs e)
		{
			SetCompound(e.ProjectHandle);
		}

		private void OnPrimaryProjectSwitched(IProject oldProject, IProject newProject)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			if (oldProject != null)
			{
				oldProject.SelectionChanged-=(new ProjectChangedEventHandler(OnGlobalObjectSelectionChanged));
			}
			Close();
		}

		private void OnGlobalObjectSelectionChanged(IProject project)
		{
			if (!_bAddingDevice)
			{
				UpdateSelectedCommandAndDeviceCatalogue();
			}
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
				UpdateSelectedCommandAndDeviceCatalogue();
			}
		}

		private void _insertDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_insertDeviceRadioButton.Checked)
			{
				UpdateSelectedCommandAndDeviceCatalogue();
			}
		}

		private void _plugDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (_plugDeviceRadioButton.Checked)
			{
				UpdateSelectedCommandAndDeviceCatalogue();
			}
		}

		private void _replaceDeviceRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			_ckbUpdateAll.Visible = _replaceDeviceRadioButton.Checked;
			if (_replaceDeviceRadioButton.Checked)
			{
				UpdateSelectedCommandAndDeviceCatalogue();
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
			IProgressCallback val = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
			try
			{
				IDeviceDescription selectedDevice = _deviceCatalogue.SelectedDevice;
				_bAddingDevice = true;
				Cursor = Cursors.WaitCursor;
				base.Enabled = false;
				string[] array = _selectedCommand.CreateBatchArguments(selectedDevice.DeviceIdentification, _objectNameTextBox.Text, _ckbUpdateAll.Checked);
				if (array != null)
				{
					_selectedCommand.OverridableExecuteBatch(array);
				}
				if (!(selectedDevice is IDeviceDescription9) || !((IDeviceDescription9)((selectedDevice is IDeviceDescription9) ? selectedDevice : null)).AutoInsertDependentDevices)
				{
					return;
				}
				IDeviceIdentification[] dependentDevices = ((IDeviceDescription8)((selectedDevice is IDeviceDescription9) ? selectedDevice : null)).DependentDevices;
				foreach (IDeviceIdentification val2 in dependentDevices)
				{
					IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val2);
					if (device != null)
					{
						int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
						string stBaseName = DeviceObjectHelper.CreateInstanceNameBase(device.DeviceInfo);
						string stName = DeviceObjectHelper.CreateUniqueIdentifier(handle, stBaseName, DeviceCommandHelper.GetHostFromSelectedStub());
						array = _selectedCommand.CreateBatchArguments(val2, stName);
						if (array != null)
						{
							_selectedCommand.OverridableExecuteBatch(array);
						}
					}
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
				UpdateSelectedCommandAndDeviceCatalogue();
				Focus();
				if (val != null)
				{
					val.Finish();
				}
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
			if (text != null && _objectNameTextBox.Enabled)
			{
				errorProvider1.SetError(_objectNameTextBox, text);
				_addDeviceButton.Enabled = false;
				return;
			}
			if (((_deviceCatalogue != null) ? _deviceCatalogue.SelectedDevice : null) != null)
			{
				if (_deviceCatalogue is IDeviceCatalogue7)
				{
					IDeviceCatalogue deviceCatalogue = _deviceCatalogue;
					((IDeviceCatalogue7)((deviceCatalogue is IDeviceCatalogue7) ? deviceCatalogue : null)).DeviceNameForDrag=(_objectNameTextBox.Text);
				}
				_addDeviceButton.Enabled = true;
			}
			errorProvider1.SetError(_objectNameTextBox, null);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.AddDeviceFormEx));
			((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
			_splitContainer.Panel1.SuspendLayout();
			_splitContainer.Panel2.SuspendLayout();
			_splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).BeginInit();
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).BeginInit();
			((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
			SuspendLayout();
			componentResourceManager.ApplyResources(_imagePictureBox, "_imagePictureBox");
			componentResourceManager.ApplyResources(_richTextBox, "_richTextBox");
			componentResourceManager.ApplyResources(_insertLocationLabel2, "_insertLocationLabel2");
			componentResourceManager.ApplyResources(_insertLocationLabel1, "_insertLocationLabel1");
			componentResourceManager.ApplyResources(this, "$this");
			base.Name = "AddDeviceFormEx";
			base.Controls.SetChildIndex(_objectNameTextBox, 0);
			base.Controls.SetChildIndex(_closeButton, 0);
			base.Controls.SetChildIndex(_addDeviceButton, 0);
			base.Controls.SetChildIndex(_splitContainer, 0);
			_splitContainer.Panel1.ResumeLayout(false);
			_splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
			_splitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).EndInit();
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).EndInit();
			((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
