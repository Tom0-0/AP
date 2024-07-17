using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_multiple_download.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Multiple_Download.htm")]
    public class MultipleDownloadResultDialog : Form
    {
        private IContainer components;

        private TreeTableView _resultView;

        private Button _closeButton;

        public MultipleDownloadResultDialog()
        {
            InitializeComponent();
        }

        internal void Initialize(MultipleDownloadResult[] results)
        {
            _resultView.Model = ((ITreeTableModel)(object)new MultipleDownloadResultModel(results));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            for (int i = 0; i < _resultView.Columns.Count; i++)
            {
                _resultView.AdjustColumnWidth(i, false);
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
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_001a: Expected O, but got Unknown
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.MultipleDownloadResultDialog));
            _resultView = new TreeTableView();
            _closeButton = new System.Windows.Forms.Button();
            SuspendLayout();
            ((System.Windows.Forms.Control)(object)_resultView).BackColor = System.Drawing.SystemColors.Window;
            _resultView.BorderStyle = (System.Windows.Forms.BorderStyle.Fixed3D);
            _resultView.DoNotShrinkColumnsAutomatically = (false);
            _resultView.GridLines = (true);
            _resultView.HeaderStyle = (System.Windows.Forms.ColumnHeaderStyle.None);
            _resultView.HideSelection = (false);
            _resultView.ImmediateEdit = (false);
            _resultView.Indent = (20);
            _resultView.KeepColumnWidthsAdjusted = (true);
            resources.ApplyResources(_resultView, "_resultView");
            _resultView.Model = ((ITreeTableModel)null);
            _resultView.MultiSelect = (true);
            ((System.Windows.Forms.Control)(object)_resultView).Name = "_resultView";
            _resultView.OpenEditOnDblClk = (false);
            _resultView.Scrollable = (true);
            _resultView.ShowLines = (false);
            _resultView.ShowPlusMinus = (false);
            _resultView.ShowRootLines = (false);
            _closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(_closeButton, "_closeButton");
            _closeButton.Name = "_closeButton";
            _closeButton.UseVisualStyleBackColor = true;
            base.AcceptButton = _closeButton;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _closeButton;
            base.Controls.Add(_closeButton);
            base.Controls.Add((System.Windows.Forms.Control)(object)_resultView);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "MultipleDownloadResultDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ResumeLayout(false);
        }
    }
}
