
using _3S.CoDeSys.Controls.Controls;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
    partial class CommentValueDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommentValueDialog));
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._treeTableView = new _3S.CoDeSys.Controls.Controls.TreeTableView();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            resources.ApplyResources(this._okButton, "_okButton");
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Name = "_okButton";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            resources.ApplyResources(this._cancelButton, "_cancelButton");
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _treeTableView
            // 
            this._treeTableView.AllowColumnReorder = false;
            this._treeTableView.AutoRestoreSelection = false;
            this._treeTableView.BackColor = System.Drawing.SystemColors.Window;
            this._treeTableView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._treeTableView.DoNotShrinkColumnsAutomatically = true;
            this._treeTableView.ForceFocusOnClick = false;
            this._treeTableView.GridLines = false;
            this._treeTableView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._treeTableView.HideSelection = false;
            this._treeTableView.ImmediateEdit = false;
            this._treeTableView.Indent = 20;
            this._treeTableView.KeepColumnWidthsAdjusted = true;
            resources.ApplyResources(this._treeTableView, "_treeTableView");
            this._treeTableView.Model = null;
            this._treeTableView.MultiSelect = true;
            this._treeTableView.Name = "_treeTableView";
            this._treeTableView.NoSearchStrings = false;
            this._treeTableView.OnlyWhenFocused = false;
            this._treeTableView.OpenEditOnDblClk = true;
            this._treeTableView.ReadOnly = false;
            this._treeTableView.Scrollable = true;
            this._treeTableView.ShowLines = true;
            this._treeTableView.ShowPlusMinus = true;
            this._treeTableView.ShowRootLines = true;
            this._treeTableView.ToggleOnDblClk = false;
            this._treeTableView.SelectionChanged += new System.EventHandler(this._treeTableView_SelectionChanged);
            // 
            // CommentValueDialog
            // 
            this.AcceptButton = this._okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._treeTableView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommentValueDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}