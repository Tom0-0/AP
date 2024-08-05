using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class PhysicalIOControl : UserControl
	{
		private int _iMappedIndex = -1;

		private IOMappingEditor _editor;

		private bool _bInit;

		private IContainer components;

		private GroupBox _grpPhysicallMapping;

		private ComboBox _cbMapping;

		private Button _btReset;

		public PhysicalIOControl(IOMappingEditor editor, int iMappedIndex)
		{
			InitializeComponent();
			_editor = editor;
			_iMappedIndex = iMappedIndex;
			RefillMappings();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			RefillMappings();
			base.OnPaint(e);
		}

		private void _btReset_Click(object sender, EventArgs e)
		{
			SaveMappedDevice(_editor.GetDeviceObject(bToModify: true), _iMappedIndex, string.Empty);
			if (_editor.GetFrame() != null)
			{
				((IEditorView)_editor.GetFrame()).Editor.Save(true);
			}
			RefillMappings();
		}

		private void _cbMapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_bInit && _cbMapping.SelectedItem != null)
			{
				SaveMappedDevice(_editor.GetDeviceObject(bToModify: true), _iMappedIndex, _cbMapping.SelectedItem.ToString());
				if (_editor.GetFrame() != null)
				{
					((IEditorView)_editor.GetFrame()).Editor.Save(true);
				}
				RefillMappings();
			}
		}

		internal static void SaveMappedDevice(IDeviceObject device, int iMappedIndex, string stMappedDev)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Expected O, but got Unknown
			if (device == null || !(device is ILogicalDevice))
			{
				return;
			}
			ILogicalDevice val = (ILogicalDevice)(object)((device is ILogicalDevice) ? device : null);
			if (val.MappedDevices == null)
			{
				return;
			}
			IMappedDevice val2 = val.MappedDevices[iMappedIndex];
			Guid empty = Guid.Empty;
			if (val2.IsMapped)
			{
				empty = val2.GetMappedDevice;
				if (empty != Guid.Empty)
				{
					IMetaObject val3 = null;
					try
					{
						val3 = ((!val.IsPhysical) ? ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(((IObject)device).MetaObject.ProjectHandle, empty) : ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IObject)device).MetaObject.ProjectHandle, empty));
						if (val3 != null && val3.Object is ILogicalDevice)
						{
							IObject @object = val3.Object;
							ILogicalDevice val4 = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
							foreach (IMappedDevice item in (IEnumerable)val4.MappedDevices)
							{
								IMappedDevice val5 = item;
								if (val5.MappedDevice == ((IObject)device).MetaObject.Name)
								{
									if (val4.IsPhysical)
									{
										val5.MappedDevice=(string.Empty);
									}
									if (val.IsPhysical)
									{
										val2.MappedDevice=(string.Empty);
									}
								}
							}
						}
					}
					catch
					{
					}
					finally
					{
						if (val3 != null && val3.IsToModify)
						{
							IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(val3.ProjectHandle, val3.ObjectGuid);
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)((editors != null && editors.Length != 0) ? editors[0] : null));
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(stMappedDev))
			{
				empty = Guid.Empty;
				foreach (KeyValuePair<string, Guid> matchingDevice in val2.MatchingDevices)
				{
					if (matchingDevice.Key == stMappedDev)
					{
						empty = matchingDevice.Value;
					}
				}
				if (!(empty != Guid.Empty))
				{
					return;
				}
				IMetaObject val6 = null;
				try
				{
					val6 = ((!val.IsPhysical) ? ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(((IObject)device).MetaObject.ProjectHandle, empty) : ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IObject)device).MetaObject.ProjectHandle, empty));
					if (val6 == null || !(val6.Object is ILogicalDevice))
					{
						return;
					}
					IObject object2 = val6.Object;
					ILogicalDevice val7 = (ILogicalDevice)(object)((object2 is ILogicalDevice) ? object2 : null);
					foreach (IMappedDevice item2 in (IEnumerable)val7.MappedDevices)
					{
						IMappedDevice val8 = item2;
						foreach (KeyValuePair<string, Guid> matchingDevice2 in val8.MatchingDevices)
						{
							if (matchingDevice2.Key == ((IObject)device).MetaObject.Name)
							{
								if (val7.IsPhysical)
								{
									val8.MappedDevice=(((IObject)device).MetaObject.Name);
								}
								if (val.IsPhysical)
								{
									val2.MappedDevice=(stMappedDev);
								}
							}
						}
					}
				}
				catch
				{
				}
				finally
				{
					if (val6 != null && val6.IsToModify)
					{
						IEditor[] editors2 = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(val6.ProjectHandle, val6.ObjectGuid);
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val6, true, (object)((editors2 != null && editors2.Length != 0) ? editors2[0] : null));
					}
				}
			}
			else if (val.IsPhysical)
			{
				val2.MappedDevice=(stMappedDev);
			}
		}

		private void _cbMapping_DropDown(object sender, EventArgs e)
		{
			RefillMappings();
		}

		private void RefillMappings()
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			bool bInit = _bInit;
			try
			{
				_bInit = true;
				_cbMapping.Items.Clear();
				IDeviceObject deviceObject = _editor.GetDeviceObject(bToModify: false);
				if (deviceObject == null || !(deviceObject is ILogicalDevice))
				{
					return;
				}
				IMappedDevice val = ((ILogicalDevice)((deviceObject is ILogicalDevice) ? deviceObject : null)).MappedDevices[_iMappedIndex];
				if (_editor.MappingPage != null && _editor.MappingPage.MappingTreeTableView != null)
				{
					bool flag = false;
					foreach (IMappedDevice item in (IEnumerable)((ILogicalDevice)((deviceObject is ILogicalDevice) ? deviceObject : null)).MappedDevices)
					{
						if (item.IsMapped)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						((Control)(object)_editor.MappingPage.MappingTreeTableView).Enabled = false;
						((Control)(object)_editor.MappingPage.MappingTreeTableView).Parent.Enabled = false;
						((Control)(object)_editor.MappingPage.MappingTreeTableView).BackColor = SystemColors.ControlLight;
					}
					else
					{
						((Control)(object)_editor.MappingPage.MappingTreeTableView).Enabled = true;
						((Control)(object)_editor.MappingPage.MappingTreeTableView).Parent.Enabled = true;
						((Control)(object)_editor.MappingPage.MappingTreeTableView).BackColor = SystemColors.Window;
					}
				}
				SortedList<string, Guid> matchingDevices = val.MatchingDevices;
				if (val.IsMapped)
				{
					int selectedIndex = _cbMapping.Items.Add(val.MappedDevice);
					_cbMapping.SelectedIndex = selectedIndex;
					_cbMapping.Enabled = false;
					return;
				}
				_cbMapping.Enabled = true;
				foreach (KeyValuePair<string, Guid> item2 in matchingDevices)
				{
					_cbMapping.Items.Add(item2.Key);
				}
			}
			finally
			{
				_bInit = bInit;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.PhysicalIOControl));
			_grpPhysicallMapping = new System.Windows.Forms.GroupBox();
			_btReset = new System.Windows.Forms.Button();
			_cbMapping = new System.Windows.Forms.ComboBox();
			_grpPhysicallMapping.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_grpPhysicallMapping, "_grpPhysicallMapping");
			_grpPhysicallMapping.Controls.Add(_btReset);
			_grpPhysicallMapping.Controls.Add(_cbMapping);
			_grpPhysicallMapping.Name = "_grpPhysicallMapping";
			_grpPhysicallMapping.TabStop = false;
			resources.ApplyResources(_btReset, "_btReset");
			_btReset.Name = "_btReset";
			_btReset.UseVisualStyleBackColor = true;
			_btReset.Click += new System.EventHandler(_btReset_Click);
			resources.ApplyResources(_cbMapping, "_cbMapping");
			_cbMapping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_cbMapping.FormattingEnabled = true;
			_cbMapping.Name = "_cbMapping";
			_cbMapping.DropDown += new System.EventHandler(_cbMapping_DropDown);
			_cbMapping.SelectedIndexChanged += new System.EventHandler(_cbMapping_SelectedIndexChanged);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			base.Controls.Add(_grpPhysicallMapping);
			base.Name = "PhysicalIOControl";
			_grpPhysicallMapping.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
