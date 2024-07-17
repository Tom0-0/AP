using _3S.CoDeSys.Controls.Collections;
using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    public class ApplicationContentDiffView : UserControl
    {
        private ApplicationContentViewModel _model;

        private IContainer components;

        private ToolTip _toolTip;

        private Label _infoLabel;

        private TreeTableView _diffView;

        public Control EmbeddedControl => this;

        public Control[] Panes => null;

        public bool CanNextDiff
        {
            get
            {
                ProjectDiffViewNode singleSelectedNode = SingleSelectedNode;
                if (singleSelectedNode != null)
                {
                    return _model.GetNextDiff(singleSelectedNode) != null;
                }
                return false;
            }
        }

        public bool CanPreviousDiff
        {
            get
            {
                ProjectDiffViewNode singleSelectedNode = SingleSelectedNode;
                if (singleSelectedNode != null)
                {
                    return _model.GetPreviousDiff(singleSelectedNode) != null;
                }
                return false;
            }
        }

        public int AdditionsCount => _model.AdditionsCount;

        public int DeletionsCount => _model.DeletionsCount;

        public int ChangesCount => _model.ChangesCount;

        private ProjectDiffViewNode SingleSelectedNode
        {
            get
            {
                if (((TreeTableViewNodeCollection)_diffView.SelectedNodes).Count == 1)
                {
                    return _diffView.GetModelNode(((TreeTableViewNodeCollection)_diffView.SelectedNodes)[0]) as ProjectDiffViewNode;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    TreeTableViewNode viewNode = _diffView.GetViewNode((ITreeTableNode)(object)value);
                    if (viewNode != null)
                    {
                        _diffView.DeselectAll();
                        viewNode.Selected = (true);
                        viewNode.Focus(0);
                        viewNode.EnsureVisible(0);
                    }
                }
            }
        }

        public ApplicationContentDiffView()
        {
            InitializeComponent();
        }

        internal void Initialize(IApplicationContent appcontent1, IApplicationContent appcontent2, ICompileContext9 comcon)
        {
            _model = new ApplicationContentViewModel(appcontent1, appcontent2, comcon);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_model != null)
            {
                _diffView.Model = ((ITreeTableModel)(object)_model);
            }
            ShowChanges();
        }

        private void ShowChanges()
        {
            if (_diffView.Nodes.Count <= 0)
            {
                return;
            }
            _diffView.Nodes[0].Selected = (true);
            _diffView.Nodes[0].Focus(0);
            for (ProjectDiffViewNode projectDiffViewNode = _diffView.GetModelNode(_diffView.Nodes[0]) as ProjectDiffViewNode; projectDiffViewNode != null; projectDiffViewNode = _model.GetNextDiff(projectDiffViewNode))
            {
                if (projectDiffViewNode.DiffState != 0 && projectDiffViewNode.Parent != null)
                {
                    _diffView.GetViewNode(projectDiffViewNode.Parent).Expand();
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _diffView.Clear();
            base.OnHandleDestroyed(e);
        }

        public void UpdateContents(bool bOpposeChanges)
        {
            _model.UpdateContents(_diffView);
            ((Control)(object)_diffView).Focus();
            ShowChanges();
        }

        public void Finish(bool bCommit)
        {
        }

        public void NextDiff()
        {
            SingleSelectedNode = _model.GetNextDiff(SingleSelectedNode);
        }

        public void PreviousDiff()
        {
            SingleSelectedNode = _model.GetPreviousDiff(SingleSelectedNode);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AdjustColumnWidths();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            AdjustColumnWidths();
        }

        private void AdjustColumnWidths()
        {
            if (base.IsHandleCreated)
            {
                int num = 0;
                _diffView.Columns[0].Width = (((Control)(object)_diffView).Width - num) / 2;
                _diffView.Columns[1].Width = num;
                _diffView.Columns[2].Width = (((Control)(object)_diffView).Width - num) / 2 - SystemInformation.VerticalScrollBarWidth - 2 * SystemInformation.Border3DSize.Width;
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
            //IL_0037: Unknown result type (might be due to invalid IL or missing references)
            //IL_0041: Expected O, but got Unknown
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.ApplicationContentDiffView));
            _toolTip = new System.Windows.Forms.ToolTip(components);
            _infoLabel = new System.Windows.Forms.Label();
            _diffView = new TreeTableView();
            SuspendLayout();
            _toolTip.ToolTipTitle = "Beispieltext.";
            _infoLabel.AutoEllipsis = true;
            resources.ApplyResources(_infoLabel, "_infoLabel");
            _infoLabel.Name = "_infoLabel";
            ((System.Windows.Forms.Control)(object)_diffView).BackColor = System.Drawing.SystemColors.Window;
            _diffView.BorderStyle = (System.Windows.Forms.BorderStyle.Fixed3D);
            resources.ApplyResources(_diffView, "_diffView");
            _diffView.DoNotShrinkColumnsAutomatically = (false);
            _diffView.GridLines = (false);
            _diffView.HeaderStyle = (System.Windows.Forms.ColumnHeaderStyle.None);
            _diffView.HideSelection = (false);
            _diffView.ImmediateEdit = (false);
            _diffView.Indent = (20);
            _diffView.KeepColumnWidthsAdjusted = (false);
            _diffView.Model = ((ITreeTableModel)null);
            _diffView.MultiSelect = (true);
            ((System.Windows.Forms.Control)(object)_diffView).Name = "_diffView";
            _diffView.OpenEditOnDblClk = (false);
            _diffView.ReadOnly = (false);
            _diffView.Scrollable = (true);
            _diffView.ShowLines = (true);
            _diffView.ShowPlusMinus = (true);
            _diffView.ShowRootLines = (true);
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add((System.Windows.Forms.Control)(object)_diffView);
            base.Controls.Add(_infoLabel);
            base.Name = "ApplicationContentDiffView";
            ResumeLayout(false);
        }
    }
}
