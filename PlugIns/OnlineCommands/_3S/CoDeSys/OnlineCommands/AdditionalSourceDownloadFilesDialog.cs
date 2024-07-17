using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_dlg_project_settings_source_code.htm")]
    [AssociatedOnlineHelpTopic("core.filecommands.file.chm::/project_settings.htm")]
    public class AdditionalSourceDownloadFilesDialog : Form
    {
        private ArchiveModel _model;

        private IContainer components;

        private TreeTableView _contentView;

        private Button _cancelButton;

        private Button _okButton;

        public AdditionalSourceDownloadFilesDialog()
        {
            InitializeComponent();
        }

        internal void Initialize(Guid[] selectedCategories)
        {
            _model = new ArchiveModel();
            _model.Initialize(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, selectedCategories);
            _contentView.Model = ((ITreeTableModel)(object)_model);
        }

        internal Guid[] GetSelectedCategories()
        {
            List<Guid> list = new List<Guid>();
            for (int i = 0; i < ((DefaultTreeTableModel)_model).Sentinel.ChildCount; i++)
            {
                ArchiveCategoryNode archiveCategoryNode = ((DefaultTreeTableModel)_model).Sentinel.GetChild(i) as ArchiveCategoryNode;
                if (archiveCategoryNode != null && archiveCategoryNode.Checked)
                {
                    list.Add(archiveCategoryNode.CategoryGuid);
                }
            }
            return list.ToArray();
        }

        private void _contentView_MouseDown(object sender, MouseEventArgs e)
        {
            //IL_00cc: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d3: Expected O, but got Unknown
            TreeTableViewNode nodeAt = _contentView.GetNodeAt(e.X, e.Y);
            if (nodeAt == null || e.Button != MouseButtons.Left)
            {
                return;
            }
            Rectangle bounds = nodeAt.GetBounds(0, (CellBoundsPortion)2);
            if (!new Rectangle(bounds.X, bounds.Y, 20, bounds.Height).Contains(e.X, e.Y))
            {
                return;
            }
            try
            {
                _contentView.BeginUpdate();
                ArchiveCategoryNode archiveCategoryNode = _contentView.GetModelNode(nodeAt) as ArchiveCategoryNode;
                if (archiveCategoryNode == null)
                {
                    return;
                }
                bool @checked = (archiveCategoryNode.Checked = archiveCategoryNode != null && !archiveCategoryNode.Checked);
                if (!nodeAt.Selected)
                {
                    return;
                }
                foreach (TreeTableViewNode item in (TreeTableViewNodeCollection)_contentView.SelectedNodes)
                {
                    TreeTableViewNode val = item;
                    archiveCategoryNode = _contentView.GetModelNode(val) as ArchiveCategoryNode;
                    if (archiveCategoryNode != null)
                    {
                        archiveCategoryNode.Checked = @checked;
                    }
                }
            }
            finally
            {
                _contentView.EndUpdate();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.AdditionalSourceDownloadFilesDialog));
            _contentView = new TreeTableView();
            _cancelButton = new System.Windows.Forms.Button();
            _okButton = new System.Windows.Forms.Button();
            SuspendLayout();
            ((System.Windows.Forms.Control)(object)_contentView).BackColor = System.Drawing.SystemColors.Window;
            _contentView.BorderStyle = (System.Windows.Forms.BorderStyle.Fixed3D);
            _contentView.DoNotShrinkColumnsAutomatically = (false);
            _contentView.GridLines = (false);
            _contentView.HeaderStyle = (System.Windows.Forms.ColumnHeaderStyle.None);
            _contentView.HideSelection = (false);
            _contentView.ImmediateEdit = (false);
            _contentView.Indent = (20);
            _contentView.KeepColumnWidthsAdjusted = (true);
            resources.ApplyResources(_contentView, "_contentView");
            _contentView.Model = ((ITreeTableModel)null);
            _contentView.MultiSelect = (true);
            ((System.Windows.Forms.Control)(object)_contentView).Name = "_contentView";
            _contentView.OpenEditOnDblClk = (false);
            _contentView.ReadOnly = (false);
            _contentView.Scrollable = (true);
            _contentView.ShowLines = (false);
            _contentView.ShowPlusMinus = (false);
            _contentView.ShowRootLines = (false);
            ((System.Windows.Forms.Control)(object)_contentView).MouseDown += new System.Windows.Forms.MouseEventHandler(_contentView_MouseDown);
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(_cancelButton, "_cancelButton");
            _cancelButton.Name = "_cancelButton";
            _cancelButton.UseVisualStyleBackColor = true;
            _okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(_okButton, "_okButton");
            _okButton.Name = "_okButton";
            _okButton.UseVisualStyleBackColor = true;
            base.AcceptButton = _okButton;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _cancelButton;
            base.Controls.Add(_okButton);
            base.Controls.Add(_cancelButton);
            base.Controls.Add((System.Windows.Forms.Control)(object)_contentView);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "AdditionalSourceDownloadFilesDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ResumeLayout(false);
        }
    }
}
