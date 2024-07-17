#define DEBUG
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.LibManObject;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_log.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/Log_device.htm")]
    public class UnknownLoginErrorDialog : Form
    {
        private Guid _appGuid;

        private GlobalInitExceptionInformation _downloadException;

        private IContainer components;

        private TextBox _logMessageField;

        private Button _gotoSourceButton;

        private Label label1;

        private TextBox _sourcePositionField;

        private Label label2;

        private Button _closeButton;

        private TextBox _libNameField;

        private Label label3;

        public UnknownLoginErrorDialog()
        {
            InitializeComponent();
            base.FormClosing += AbortPendingRequests;
        }

        public void AbortPendingRequests(object sender, FormClosingEventArgs e)
        {
            if (_downloadException != null)
            {
                _downloadException.AbortPendingRequests();
            }
        }

        internal void Initialize(Guid appGuid)
        {
            _appGuid = appGuid;
            _downloadException = new GlobalInitExceptionInformation(_appGuid);
            _downloadException.GetLastExceptionFromDevice(CBGotExceptionFromDevice);
        }

        private void CBGotExceptionFromDevice()
        {
            _logMessageField.Text = _downloadException.LastExceptionFromDevice;
            _sourcePositionField.Text = _downloadException.POUName;
            _libNameField.Text = _downloadException.LibraryName;
            if (_downloadException.SourcePosition != null && POUIsNotInCompiledLib(_downloadException.SourcePosition))
            {
                _gotoSourceButton.Enabled = true;
            }
        }

        private bool POUIsNotInCompiledLib(ISourcePosition sourcePosition)
        {
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(sourcePosition.ProjectHandle, sourcePosition.ObjectGuid);
            Debug.Assert(metaObjectStub != null);
            if (typeof(IEmptyObject).IsAssignableFrom(metaObjectStub.ObjectType))
            {
                return false;
            }
            return true;
        }

        private void btnGotoEditor_Click(object sender, EventArgs e)
        {
            OpenEditorView(_downloadException.SourcePosition.ProjectHandle, _downloadException.SourcePosition.ObjectGuid, _downloadException.SourcePosition.PositionCombination);
            Close();
        }

        private static bool OpenEditorView(int nProjectHandle, Guid objectGuid, long nPosition)
        {
            //IL_0056: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, objectGuid);
                Debug.Assert(metaObjectStub != null);
                Guid defaultEditorViewFactory = ((IEngine)APEnvironment.Engine).Frame.ViewFactoryManager.GetDefaultEditorViewFactory(metaObjectStub.ObjectType, metaObjectStub.EmbeddedObjectTypes);
                IEditorView val = ((IEngine)APEnvironment.Engine).Frame.OpenEditorView(metaObjectStub, defaultEditorViewFactory, (string)null);
                if (val != null)
                {
                    if (val is IHasModifiableBreakpoints)
                    {
                        ((IHasModifiableBreakpoints)val).SelectPositionForBreakpoint(nPosition, 0);
                    }
                    else
                    {
                        val.Select(nPosition, 0);
                    }
                }
                else
                {
                    APEnvironment.MessageService.Warning(Strings.NoSourceCodeAvailable, "NoSourceCodeAvailable", Array.Empty<object>());
                }
                return val != null;
            }
            catch (InvalidObjectGuidException)
            {
                return false;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.UnknownLoginErrorDialog));
            _logMessageField = new System.Windows.Forms.TextBox();
            _gotoSourceButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            _sourcePositionField = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            _closeButton = new System.Windows.Forms.Button();
            _libNameField = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            resources.ApplyResources(_logMessageField, "_logMessageField");
            _logMessageField.Name = "_logMessageField";
            _logMessageField.ReadOnly = true;
            resources.ApplyResources(_gotoSourceButton, "_gotoSourceButton");
            _gotoSourceButton.Name = "_gotoSourceButton";
            _gotoSourceButton.UseVisualStyleBackColor = true;
            _gotoSourceButton.Click += new System.EventHandler(btnGotoEditor_Click);
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            resources.ApplyResources(_sourcePositionField, "_sourcePositionField");
            _sourcePositionField.Name = "_sourcePositionField";
            _sourcePositionField.ReadOnly = true;
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            resources.ApplyResources(_closeButton, "_closeButton");
            _closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _closeButton.Name = "_closeButton";
            _closeButton.UseVisualStyleBackColor = true;
            resources.ApplyResources(_libNameField, "_libNameField");
            _libNameField.Name = "_libNameField";
            _libNameField.ReadOnly = true;
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _closeButton;
            base.Controls.Add(label3);
            base.Controls.Add(_libNameField);
            base.Controls.Add(_closeButton);
            base.Controls.Add(label2);
            base.Controls.Add(_sourcePositionField);
            base.Controls.Add(label1);
            base.Controls.Add(_gotoSourceButton);
            base.Controls.Add(_logMessageField);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "UnknownLoginErrorDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
