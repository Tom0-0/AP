using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_dlg_project_settings_source_code.htm")]
    public class SourceDownloadOptionControl : UserControl
    {
        private Guid[] _content;

        private bool _bWasCompileInfoPartOfSource;

        private IContainer components;

        private GroupBox Timing;

        private RadioButton _implicitlyAtDownloadRB;

        private RadioButton _onlyOnDemandRB;

        private RadioButton _implicitlyAtCreatingBootprojectRB;

        private RadioButton _promptAtDownloadRB;

        private GroupBox DestinationDevice;

        private ComboBox _selectedDeviceComboBox;

        private GroupBox Content;

        private Button _additionalFilesButton;

        private CheckBox _cbCompact;

        private RadioButton _implitBootAppAndDownloadRB;

        public SourceDownloadOptionControl()
        {
            InitializeComponent();
        }

        public bool Save(ref string stMessage, ref Control failedControl)
        {
            ProjectOptionsHelper.SourceDownloadContent2 = _content;
            if (_implicitlyAtDownloadRB.Checked)
            {
                ProjectOptionsHelper.SourceDownloadTiming2 = 1;
            }
            else if (_promptAtDownloadRB.Checked)
            {
                ProjectOptionsHelper.SourceDownloadTiming2 = 2;
            }
            else if (_implicitlyAtCreatingBootprojectRB.Checked)
            {
                ProjectOptionsHelper.SourceDownloadTiming2 = 3;
            }
            else if (_onlyOnDemandRB.Checked)
            {
                ProjectOptionsHelper.SourceDownloadTiming2 = 4;
            }
            else if (_implitBootAppAndDownloadRB.Checked)
            {
                ProjectOptionsHelper.SourceDownloadTiming2 = 5;
            }
            ProjectOptionsHelper.SourceDownloadCompact = _cbCompact.Checked;
            ProjectOptionsHelper.SourceDownloadDevice = ((DeviceComboBoxItem)_selectedDeviceComboBox.SelectedItem).ObjectGuid;
            if (_bWasCompileInfoPartOfSource && !ProjectOptionsHelper.IsCompileInformationIncludedInSource(bWarnUserAndOptionallyChangeSourceContent: false))
            {
                ProjectOptionsHelper.AlreadyWarnedOfMissingDownloadInfo = false;
            }
            return true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
            _selectedDeviceComboBox.Items.Add(new DeviceComboBoxItem(Guid.Empty, Strings.SourceDownload_AllDevices));
            Guid[] array = allObjects;
            foreach (Guid guid in array)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
                if (!(metaObjectStub.ParentObjectGuid != Guid.Empty) && typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid).Object;
                    IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
                    if (val == null || OnlineFeatureHelper.CheckSelectedDevice((OnlineFeatureEnum)1, val))
                    {
                        _selectedDeviceComboBox.Items.Add(new DeviceComboBoxItem(guid, metaObjectStub.Name));
                    }
                }
            }
            _selectedDeviceComboBox.SelectedItem = new DeviceComboBoxItem(ProjectOptionsHelper.SourceDownloadDevice, "");
            switch (ProjectOptionsHelper.SourceDownloadTiming2)
            {
                case 1:
                    _implicitlyAtDownloadRB.Checked = true;
                    break;
                case 2:
                    _promptAtDownloadRB.Checked = true;
                    break;
                case 3:
                    _implicitlyAtCreatingBootprojectRB.Checked = true;
                    break;
                case 4:
                    _onlyOnDemandRB.Checked = true;
                    break;
                case 5:
                    _implitBootAppAndDownloadRB.Checked = true;
                    break;
            }
            _cbCompact.Checked = ProjectOptionsHelper.SourceDownloadCompact;
            _content = ProjectOptionsHelper.SourceDownloadContent2;
            _bWasCompileInfoPartOfSource = ProjectOptionsHelper.IsCompileInformationIncludedInSource(bWarnUserAndOptionallyChangeSourceContent: false);
        }

        private void _additionalFilesButton_Click(object sender, EventArgs e)
        {
            AdditionalSourceDownloadFilesDialog additionalSourceDownloadFilesDialog = new AdditionalSourceDownloadFilesDialog();
            additionalSourceDownloadFilesDialog.Initialize(_content);
            if (additionalSourceDownloadFilesDialog.ShowDialog(this) == DialogResult.OK)
            {
                _content = additionalSourceDownloadFilesDialog.GetSelectedCategories();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.SourceDownloadOptionControl));
            Timing = new System.Windows.Forms.GroupBox();
            _implitBootAppAndDownloadRB = new System.Windows.Forms.RadioButton();
            _onlyOnDemandRB = new System.Windows.Forms.RadioButton();
            _implicitlyAtCreatingBootprojectRB = new System.Windows.Forms.RadioButton();
            _promptAtDownloadRB = new System.Windows.Forms.RadioButton();
            _implicitlyAtDownloadRB = new System.Windows.Forms.RadioButton();
            DestinationDevice = new System.Windows.Forms.GroupBox();
            _selectedDeviceComboBox = new System.Windows.Forms.ComboBox();
            Content = new System.Windows.Forms.GroupBox();
            _cbCompact = new System.Windows.Forms.CheckBox();
            _additionalFilesButton = new System.Windows.Forms.Button();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            Timing.SuspendLayout();
            DestinationDevice.SuspendLayout();
            Content.SuspendLayout();
            SuspendLayout();
            resources.ApplyResources(label, "label1");
            label.Name = "label1";
            Timing.Controls.Add(_implitBootAppAndDownloadRB);
            Timing.Controls.Add(_onlyOnDemandRB);
            Timing.Controls.Add(_implicitlyAtCreatingBootprojectRB);
            Timing.Controls.Add(_promptAtDownloadRB);
            Timing.Controls.Add(_implicitlyAtDownloadRB);
            resources.ApplyResources(Timing, "Timing");
            Timing.Name = "Timing";
            Timing.TabStop = false;
            resources.ApplyResources(_implitBootAppAndDownloadRB, "_implitBootAppAndDownloadRB");
            _implitBootAppAndDownloadRB.Name = "_implitBootAppAndDownloadRB";
            _implitBootAppAndDownloadRB.TabStop = true;
            _implitBootAppAndDownloadRB.UseVisualStyleBackColor = true;
            resources.ApplyResources(_onlyOnDemandRB, "_onlyOnDemandRB");
            _onlyOnDemandRB.Name = "_onlyOnDemandRB";
            _onlyOnDemandRB.TabStop = true;
            _onlyOnDemandRB.UseVisualStyleBackColor = true;
            resources.ApplyResources(_implicitlyAtCreatingBootprojectRB, "_implicitlyAtCreatingBootprojectRB");
            _implicitlyAtCreatingBootprojectRB.Name = "_implicitlyAtCreatingBootprojectRB";
            _implicitlyAtCreatingBootprojectRB.TabStop = true;
            _implicitlyAtCreatingBootprojectRB.UseVisualStyleBackColor = true;
            resources.ApplyResources(_promptAtDownloadRB, "_promptAtDownloadRB");
            _promptAtDownloadRB.Name = "_promptAtDownloadRB";
            _promptAtDownloadRB.TabStop = true;
            _promptAtDownloadRB.UseVisualStyleBackColor = true;
            resources.ApplyResources(_implicitlyAtDownloadRB, "_implicitlyAtDownloadRB");
            _implicitlyAtDownloadRB.Name = "_implicitlyAtDownloadRB";
            _implicitlyAtDownloadRB.TabStop = true;
            _implicitlyAtDownloadRB.UseVisualStyleBackColor = true;
            DestinationDevice.Controls.Add(_selectedDeviceComboBox);
            resources.ApplyResources(DestinationDevice, "DestinationDevice");
            DestinationDevice.Name = "DestinationDevice";
            DestinationDevice.TabStop = false;
            _selectedDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _selectedDeviceComboBox.FormattingEnabled = true;
            resources.ApplyResources(_selectedDeviceComboBox, "_selectedDeviceComboBox");
            _selectedDeviceComboBox.Name = "_selectedDeviceComboBox";
            _selectedDeviceComboBox.Sorted = true;
            Content.Controls.Add(_cbCompact);
            Content.Controls.Add(label);
            Content.Controls.Add(_additionalFilesButton);
            resources.ApplyResources(Content, "Content");
            Content.Name = "Content";
            Content.TabStop = false;
            resources.ApplyResources(_cbCompact, "_cbCompact");
            _cbCompact.Name = "_cbCompact";
            _cbCompact.UseVisualStyleBackColor = true;
            resources.ApplyResources(_additionalFilesButton, "_additionalFilesButton");
            _additionalFilesButton.Name = "_additionalFilesButton";
            _additionalFilesButton.UseVisualStyleBackColor = true;
            _additionalFilesButton.Click += new System.EventHandler(_additionalFilesButton_Click);
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(DestinationDevice);
            base.Controls.Add(Content);
            base.Controls.Add(Timing);
            base.Name = "SourceDownloadOptionControl";
            Timing.ResumeLayout(false);
            Timing.PerformLayout();
            DestinationDevice.ResumeLayout(false);
            Content.ResumeLayout(false);
            Content.PerformLayout();
            ResumeLayout(false);
        }
    }
}
