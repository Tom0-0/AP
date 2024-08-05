using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class LogicalIOControl : UserControl
	{
		private int _iMappedIndex = -1;

		private IOMappingEditor _editor;

		private bool _bInit;

		private IContainer components;

		private GroupBox _grpLogicalMapping;

		private ComboBox _cbMapping;

		private Button _btReset;

		public LogicalIOControl(IOMappingEditor editor, int iMappedIndex)
		{
			InitializeComponent();
			_editor = editor;
			_iMappedIndex = iMappedIndex;
			RefillMappings();
		}

		private void _btReset_Click(object sender, EventArgs e)
		{
			PhysicalIOControl.SaveMappedDevice(_editor.GetDeviceObject(bToModify: true), _iMappedIndex, string.Empty);
			if (_editor.GetFrame() != null)
			{
				_editor.GetFrame().Editor.Save(bCommit: true);
				if (_editor?.GetFrame() is IParameterSetEditorFrame2 parameterSetEditorFrame)
				{
					parameterSetEditorFrame.UpdatePages(_editor as IBaseDeviceEditor);
				}
			}
			RefillMappings();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			RefillMappings();
			base.OnPaint(e);
		}

		private void _cbMapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_bInit || _cbMapping.SelectedItem == null)
			{
				return;
			}
			PhysicalIOControl.SaveMappedDevice(_editor.GetDeviceObject(bToModify: true), _iMappedIndex, _cbMapping.SelectedItem.ToString());
			if (_editor.GetFrame() != null)
			{
				_editor.GetFrame().Editor.Save(bCommit: true);
				if (_editor?.GetFrame() is IParameterSetEditorFrame2 parameterSetEditorFrame)
				{
					parameterSetEditorFrame.UpdatePages(_editor as IBaseDeviceEditor);
				}
			}
			RefillMappings();
		}

		private void _cbMapping_DropDown(object sender, EventArgs e)
		{
			RefillMappings();
		}

		private void RefillMappings()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
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
					if (deviceObject is ILogicalDevice2 && ((ILogicalDevice2)((deviceObject is ILogicalDevice2) ? deviceObject : null)).MappingPossible)
					{
						flag = false;
					}
					if (flag)
					{
						((Control)(object)_editor.MappingPage.MappingTreeTableView).Enabled = false;
						if (((Control)(object)_editor.MappingPage.MappingTreeTableView).Parent != null)
						{
							((Control)(object)_editor.MappingPage.MappingTreeTableView).Parent.Enabled = false;
						}
						((Control)(object)_editor.MappingPage.MappingTreeTableView).BackColor = SystemColors.ControlLight;
					}
					else
					{
						((Control)(object)_editor.MappingPage.MappingTreeTableView).Enabled = true;
						if (((Control)(object)_editor.MappingPage.MappingTreeTableView).Parent != null)
						{
							((Control)(object)_editor.MappingPage.MappingTreeTableView).Parent.Enabled = true;
						}
						((Control)(object)_editor.MappingPage.MappingTreeTableView).BackColor = SystemColors.Window;
					}
				}
				IMappedDevice val = ((ILogicalDevice)((deviceObject is ILogicalDevice) ? deviceObject : null)).MappedDevices[_iMappedIndex];
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.LogicalIOControl));
			_grpLogicalMapping = new System.Windows.Forms.GroupBox();
			_btReset = new System.Windows.Forms.Button();
			_cbMapping = new System.Windows.Forms.ComboBox();
			_grpLogicalMapping.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_grpLogicalMapping, "_grpLogicalMapping");
			_grpLogicalMapping.Controls.Add(_btReset);
			_grpLogicalMapping.Controls.Add(_cbMapping);
			_grpLogicalMapping.Name = "_grpLogicalMapping";
			_grpLogicalMapping.TabStop = false;
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
			base.Controls.Add(_grpLogicalMapping);
			base.Name = "LogicalIOControl";
			_grpLogicalMapping.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
