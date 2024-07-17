using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_login.htm")]
    [AssociatedOnlineHelpTopic("core.frame.overview.chm::/code_generation_and_online_change.htm")]
    public class ApplicationInfoDialogWithContent : Form
    {
        private Guid _appGuid = Guid.Empty;

        private IApplicationContent _appcontentPLC;

        private IContainer components;

        private Button _closeButton;

        private TabControl _tcApplicationInformation;

        private TabPage _tpApplicationInfo;

        private TabPage _tpApplicationContent;

        private TextBox _descriptionPLCTextBox;

        private TextBox _descriptionIDETextBox;

        private TextBox _versionPLCTextBox;

        private TextBox _versionIDETextBox;

        private TextBox _authorPLCTextBox;

        private TextBox _authorIDETextBox;

        private TextBox _ideVersionPLCTextBox;

        private TextBox _ideVersionIDETextBox;

        private TextBox _lastModificationPLCTextBox;

        private TextBox _lastModificationIDETextBox;

        private TextBox _projectNamePLCTextBox;

        private TextBox _projectNameIDETextBox;

        private Label label2;

        private Label label1;

        private Panel _panel1;

        private Label _rightLabel;

        private Label _leftLabel;

        private LinkLabel _linkGenerateCode;

        private TabPage _tpChanges;

        private TreeTableView _ttvChanges;

        public ApplicationInfoDialogWithContent()
        {
            InitializeComponent();
        }

        internal void Initialize(string stProjectNameIDE, string stProjectNamePLC, string stLastModificationIDE, string stLastModificationPLC, string stIDEVersionIDE, string stIDEVersionPLC, string stAuthorIDE, string stAuthorPLC, string stVersionIDE, string stVersionPLC, string stDescriptionIDE, string stDescriptionPLC, IEnumerable<IChangedLMObject> changedObjects, IApplicationContent appcontentIDE, IApplicationContent appcontentPLC, ICompileContext9 comcon, Guid appGuid, string stApplicationName)
        {
            _projectNameIDETextBox.Text = stProjectNameIDE;
            _projectNamePLCTextBox.Text = stProjectNamePLC;
            _lastModificationIDETextBox.Text = stLastModificationIDE;
            _lastModificationPLCTextBox.Text = stLastModificationPLC;
            _ideVersionIDETextBox.Text = stIDEVersionIDE;
            _ideVersionPLCTextBox.Text = stIDEVersionPLC;
            _authorIDETextBox.Text = stAuthorIDE;
            _authorPLCTextBox.Text = stAuthorPLC;
            _versionIDETextBox.Text = stVersionIDE;
            _versionPLCTextBox.Text = stVersionPLC;
            _descriptionIDETextBox.Text = stDescriptionIDE;
            _descriptionPLCTextBox.Text = stDescriptionPLC;
            if (appcontentPLC != null)
            {
                ApplicationContentDiffView applicationContentDiffView = new ApplicationContentDiffView();
                applicationContentDiffView.Initialize(appcontentIDE, appcontentPLC, comcon);
                if (!SetEmbeddedDiffViewer(applicationContentDiffView))
                {
                    return;
                }
            }
            bool flag = false;
            if (comcon != null)
            {
                flag = APEnvironment.LMServiceProvider.CompileService.IsUpToDate(((ICompileContextCommon)comcon).ApplicationGuid);
            }
            _linkGenerateCode.Visible = !flag && appcontentPLC != null && comcon != null;
            if (!flag || appcontentPLC == null || appcontentIDE == null)
            {
                _tcApplicationInformation.TabPages.Remove(_tpApplicationContent);
            }
            _appGuid = appGuid;
            _appcontentPLC = appcontentPLC;
            _leftLabel.Text = string.Format(Strings.ApplicationContent_Title_IDE, stApplicationName);
            _rightLabel.Text = string.Format(Strings.ApplicationContent_Title_PLC, stApplicationName);
            _leftLabel.Width = (base.Width - 3 * _leftLabel.Left) / 2;
            _rightLabel.Width = (base.Width - 3 * _leftLabel.Left) / 2;
            _rightLabel.Left = _leftLabel.Right + _leftLabel.Left;
            ChangeTreeTableModel changeTreeTableModel = new ChangeTreeTableModel(changedObjects);
            _ttvChanges.Model = ((ITreeTableModel)(object)changeTreeTableModel);
            _ttvChanges.Columns[0].Width = 360;
            _ttvChanges.Columns[1].Width = 400;
            _ttvChanges.ExpandAll();
            if (changeTreeTableModel.Empty)
            {
                _tcApplicationInformation.TabPages.Remove(_tpChanges);
            }
        }

        private bool SetEmbeddedDiffViewer(ApplicationContentDiffView pdv)
        {
            if (pdv != null)
            {
                pdv.EmbeddedControl.Dock = DockStyle.Fill;
                _panel1.Controls.Clear();
                _panel1.Controls.Add(pdv.EmbeddedControl);
                pdv.UpdateContents(bOpposeChanges: true);
                pdv.EmbeddedControl.Focus();
            }
            return true;
        }

        private void FillContentInfo(string stProjectNamePLC, IApplicationContent appcontentIDE, IApplicationContent appcontentPLC)
        {
            if (appcontentPLC == null)
            {
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Application <<{0}>> contains:\r\n", stProjectNamePLC);
            stringBuilder.AppendFormat("{0} POUs:\r\n", appcontentPLC.POUs.Length + appcontentPLC.FBs.Length);
            IPOUInfoStruct[] pOUs = appcontentPLC.POUs;
            foreach (IPOUInfoStruct pouinfo in pOUs)
            {
                POUInfoText(stringBuilder, pouinfo, appcontentPLC, appcontentIDE);
            }
            for (int j = 0; j < appcontentPLC.FBs.Length; j++)
            {
                IFBInfoStruct pouinfo2 = appcontentPLC.FBs[j];
                POUInfoText(stringBuilder, (IPOUInfoStruct)(object)pouinfo2, appcontentPLC, appcontentIDE);
                IMethodInfoStruct[] methods = appcontentPLC.Methods;
                foreach (IMethodInfoStruct val in methods)
                {
                    if (val.ParentPOUIndex == j)
                    {
                        stringBuilder.Append("\t");
                        POUInfoText(stringBuilder, (IPOUInfoStruct)(object)val, appcontentPLC, appcontentIDE);
                    }
                }
            }
            stringBuilder.AppendFormat("{0} Data Types:\r\n", appcontentPLC.DUTs.Length);
            IDUTInfoStruct[] dUTs = appcontentPLC.DUTs;
            foreach (IDUTInfoStruct val2 in dUTs)
            {
                if (appcontentIDE == null)
                {
                    stringBuilder.AppendFormat("\t{0}, (Checksum Interface: 0x{1:X})\r\n", ((ICompiledElementInfoStruct)val2).Name, val2.CRCInterface);
                    continue;
                }
                IDUTInfoStruct val3 = null;
                IDUTInfoStruct[] dUTs2 = appcontentIDE.DUTs;
                foreach (IDUTInfoStruct val4 in dUTs2)
                {
                    if (((ICompiledElementInfoStruct)val4).Name == ((ICompiledElementInfoStruct)val2).Name)
                    {
                        val3 = val4;
                        break;
                    }
                }
                if (val3 == null)
                {
                    stringBuilder.AppendFormat("\t{0}, (not found in reference context)\r\n", ((ICompiledElementInfoStruct)val2).Name);
                }
                else if (val3.CRCInterface != val2.CRCInterface)
                {
                    stringBuilder.AppendFormat("\t{0}, (changed)\r\n", ((ICompiledElementInfoStruct)val2).Name);
                }
                else
                {
                    stringBuilder.AppendFormat("\t{0}, (unchanged)\r\n", ((ICompiledElementInfoStruct)val2).Name);
                }
            }
            stringBuilder.AppendFormat("{0} Global Variable lists:\r\n", appcontentPLC.GVLs.Length);
            IGVLInfoStruct[] gVLs = appcontentPLC.GVLs;
            foreach (IGVLInfoStruct val5 in gVLs)
            {
                if (appcontentIDE == null)
                {
                    stringBuilder.AppendFormat("\t{0}, (Checksum Interface: 0x{1:X})\r\n", ((ICompiledElementInfoStruct)val5).Name, val5.CRCInterface);
                    continue;
                }
                IGVLInfoStruct val6 = null;
                IGVLInfoStruct[] gVLs2 = appcontentIDE.GVLs;
                foreach (IGVLInfoStruct val7 in gVLs2)
                {
                    if (((ICompiledElementInfoStruct)val7).Name == ((ICompiledElementInfoStruct)val5).Name)
                    {
                        val6 = val7;
                        break;
                    }
                }
                if (val6 == null)
                {
                    stringBuilder.AppendFormat("\t{0}, (not found in reference context)\r\n", ((ICompiledElementInfoStruct)val5).Name);
                }
                else if (val6.CRCInterface != val5.CRCInterface)
                {
                    stringBuilder.AppendFormat("\t{0}, (changed)\r\n", ((ICompiledElementInfoStruct)val5).Name);
                }
                else
                {
                    stringBuilder.AppendFormat("\t{0}, (unchanged)\r\n", ((ICompiledElementInfoStruct)val5).Name);
                }
            }
        }

        private void POUInfoText(StringBuilder stb, IPOUInfoStruct pouinfo, IApplicationContent appContent, IApplicationContent appContentProject)
        {
            stb.AppendFormat("\t{0}, ", ((ICompiledElementInfoStruct)pouinfo).Name);
            if (appContentProject == null)
            {
                stb.AppendFormat("(Checksum Code: 0x{0:X}, Checksum Interface: 0x{1:X})\r\n", pouinfo.CRCCode, pouinfo.CRCInterface);
                return;
            }
            IPOUInfoStruct val = null;
            if (pouinfo is IFBInfoStruct)
            {
                IFBInfoStruct[] fBs = appContentProject.FBs;
                foreach (IFBInfoStruct val2 in fBs)
                {
                    if (((ICompiledElementInfoStruct)val2).Name == ((ICompiledElementInfoStruct)pouinfo).Name)
                    {
                        val = (IPOUInfoStruct)(object)val2;
                        break;
                    }
                }
            }
            else if (pouinfo is IMethodInfoStruct)
            {
                string name = ((ICompiledElementInfoStruct)appContent.FBs[((IMethodInfoStruct)((pouinfo is IMethodInfoStruct) ? pouinfo : null)).ParentPOUIndex]).Name;
                IMethodInfoStruct[] methods = appContentProject.Methods;
                foreach (IMethodInfoStruct val3 in methods)
                {
                    string name2 = ((ICompiledElementInfoStruct)appContentProject.FBs[val3.ParentPOUIndex]).Name;
                    if (((ICompiledElementInfoStruct)val3).Name == ((ICompiledElementInfoStruct)pouinfo).Name && name == name2)
                    {
                        val = (IPOUInfoStruct)(object)val3;
                        break;
                    }
                }
            }
            else
            {
                IPOUInfoStruct[] pOUs = appContentProject.POUs;
                foreach (IPOUInfoStruct val4 in pOUs)
                {
                    if (((ICompiledElementInfoStruct)val4).Name == ((ICompiledElementInfoStruct)pouinfo).Name)
                    {
                        val = val4;
                        break;
                    }
                }
            }
            if (val != null)
            {
                if (val.CRCCode == pouinfo.CRCCode && val.CRCInterface == pouinfo.CRCInterface)
                {
                    stb.Append("(unchanged)");
                }
                else
                {
                    stb.Append("(");
                    if (val.CRCInterface != pouinfo.CRCInterface)
                    {
                        stb.Append("interface changed");
                    }
                    else
                    {
                        stb.Append("interface unchanged");
                    }
                    if (val.CRCCode != pouinfo.CRCCode)
                    {
                        stb.Append(", code changed");
                    }
                    else
                    {
                        stb.Append(", code unchanged");
                    }
                    stb.Append(")");
                }
            }
            else
            {
                stb.Append("(not found in reference context)");
            }
            stb.Append("\r\n");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (base.Owner != null)
            {
                base.Location = new Point(base.Owner.Left + base.Owner.Width / 2 - base.Width / 2, base.Owner.Bottom + 2);
            }
            new SourceDownloadOptionControl();
        }

        private void _tpApplicationContent_Click(object sender, EventArgs e)
        {
        }

        private void _linkGenerateCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateCode(_appGuid, false, false))
            {
                ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(_appGuid);
                ICompileContext9 val = (ICompileContext9)(object)((compileContext is ICompileContext9) ? compileContext : null);
                if (val != null)
                {
                    IApplicationContent applicationContent = val.GetApplicationContent();
                    ApplicationContentDiffView applicationContentDiffView = new ApplicationContentDiffView();
                    applicationContentDiffView.Initialize(applicationContent, _appcontentPLC, val);
                    SetEmbeddedDiffViewer(applicationContentDiffView);
                    _tcApplicationInformation.TabPages.Add(_tpApplicationContent);
                    _linkGenerateCode.Visible = false;
                }
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            base.OnSizeChanged(e);
            _leftLabel.Width = (base.Width - 3 * _leftLabel.Left) / 2;
            _rightLabel.Width = (base.Width - 3 * _leftLabel.Left) / 2;
            _rightLabel.Left = _leftLabel.Right + _leftLabel.Left;
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
            //IL_003c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0046: Expected O, but got Unknown
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.ApplicationInfoDialogWithContent));
            _closeButton = new System.Windows.Forms.Button();
            _tcApplicationInformation = new System.Windows.Forms.TabControl();
            _tpChanges = new System.Windows.Forms.TabPage();
            _ttvChanges = new TreeTableView();
            _tpApplicationInfo = new System.Windows.Forms.TabPage();
            _descriptionPLCTextBox = new System.Windows.Forms.TextBox();
            _descriptionIDETextBox = new System.Windows.Forms.TextBox();
            _versionPLCTextBox = new System.Windows.Forms.TextBox();
            _versionIDETextBox = new System.Windows.Forms.TextBox();
            _authorPLCTextBox = new System.Windows.Forms.TextBox();
            _authorIDETextBox = new System.Windows.Forms.TextBox();
            _ideVersionPLCTextBox = new System.Windows.Forms.TextBox();
            _ideVersionIDETextBox = new System.Windows.Forms.TextBox();
            _lastModificationPLCTextBox = new System.Windows.Forms.TextBox();
            _lastModificationIDETextBox = new System.Windows.Forms.TextBox();
            _projectNamePLCTextBox = new System.Windows.Forms.TextBox();
            _projectNameIDETextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            _tpApplicationContent = new System.Windows.Forms.TabPage();
            _rightLabel = new System.Windows.Forms.Label();
            _leftLabel = new System.Windows.Forms.Label();
            _panel1 = new System.Windows.Forms.Panel();
            _linkGenerateCode = new System.Windows.Forms.LinkLabel();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label4 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label5 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label6 = new System.Windows.Forms.Label();
            _tcApplicationInformation.SuspendLayout();
            _tpChanges.SuspendLayout();
            _tpApplicationInfo.SuspendLayout();
            _tpApplicationContent.SuspendLayout();
            SuspendLayout();
            resources.ApplyResources(label, "label8");
            label.Name = "label8";
            resources.ApplyResources(label2, "label7");
            label2.Name = "label7";
            resources.ApplyResources(label3, "label6");
            label3.Name = "label6";
            resources.ApplyResources(label4, "label5");
            label4.Name = "label5";
            resources.ApplyResources(label5, "label4");
            label5.Name = "label4";
            resources.ApplyResources(label6, "label3");
            label6.Name = "label3";
            _closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(_closeButton, "_closeButton");
            _closeButton.Name = "_closeButton";
            _closeButton.UseVisualStyleBackColor = true;
            _tcApplicationInformation.Controls.Add(_tpChanges);
            _tcApplicationInformation.Controls.Add(_tpApplicationInfo);
            _tcApplicationInformation.Controls.Add(_tpApplicationContent);
            resources.ApplyResources(_tcApplicationInformation, "_tcApplicationInformation");
            _tcApplicationInformation.Name = "_tcApplicationInformation";
            _tcApplicationInformation.SelectedIndex = 0;
            _tpChanges.Controls.Add((System.Windows.Forms.Control)(object)_ttvChanges);
            resources.ApplyResources(_tpChanges, "_tpChanges");
            _tpChanges.Name = "_tpChanges";
            _tpChanges.UseVisualStyleBackColor = true;
            _ttvChanges.AllowColumnReorder = (false);
            resources.ApplyResources(_ttvChanges, "_ttvChanges");
            _ttvChanges.AutoRestoreSelection = (false);
            ((System.Windows.Forms.Control)(object)_ttvChanges).BackColor = System.Drawing.SystemColors.Window;
            _ttvChanges.BorderStyle = (System.Windows.Forms.BorderStyle.FixedSingle);
            _ttvChanges.DoNotShrinkColumnsAutomatically = (false);
            _ttvChanges.ForceFocusOnClick = (false);
            _ttvChanges.GridLines = (true);
            _ttvChanges.HeaderStyle = (System.Windows.Forms.ColumnHeaderStyle.Nonclickable);
            _ttvChanges.HideSelection = (false);
            _ttvChanges.ImmediateEdit = (false);
            _ttvChanges.Indent = (20);
            _ttvChanges.KeepColumnWidthsAdjusted = (false);
            _ttvChanges.Model = ((ITreeTableModel)null);
            _ttvChanges.MultiSelect = (false);
            ((System.Windows.Forms.Control)(object)_ttvChanges).Name = "_ttvChanges";
            _ttvChanges.NoSearchStrings = (false);
            _ttvChanges.OnlyWhenFocused = (false);
            _ttvChanges.OpenEditOnDblClk = (false);
            _ttvChanges.ReadOnly = (false);
            _ttvChanges.Scrollable = (true);
            _ttvChanges.ShowLines = (true);
            _ttvChanges.ShowPlusMinus = (true);
            _ttvChanges.ShowRootLines = (true);
            _ttvChanges.ToggleOnDblClk = (false);
            _tpApplicationInfo.BackColor = System.Drawing.SystemColors.Control;
            _tpApplicationInfo.Controls.Add(label);
            _tpApplicationInfo.Controls.Add(_descriptionPLCTextBox);
            _tpApplicationInfo.Controls.Add(_descriptionIDETextBox);
            _tpApplicationInfo.Controls.Add(label2);
            _tpApplicationInfo.Controls.Add(_versionPLCTextBox);
            _tpApplicationInfo.Controls.Add(_versionIDETextBox);
            _tpApplicationInfo.Controls.Add(label3);
            _tpApplicationInfo.Controls.Add(_authorPLCTextBox);
            _tpApplicationInfo.Controls.Add(_authorIDETextBox);
            _tpApplicationInfo.Controls.Add(label4);
            _tpApplicationInfo.Controls.Add(_ideVersionPLCTextBox);
            _tpApplicationInfo.Controls.Add(_ideVersionIDETextBox);
            _tpApplicationInfo.Controls.Add(label5);
            _tpApplicationInfo.Controls.Add(_lastModificationPLCTextBox);
            _tpApplicationInfo.Controls.Add(_lastModificationIDETextBox);
            _tpApplicationInfo.Controls.Add(label6);
            _tpApplicationInfo.Controls.Add(_projectNamePLCTextBox);
            _tpApplicationInfo.Controls.Add(_projectNameIDETextBox);
            _tpApplicationInfo.Controls.Add(this.label2);
            _tpApplicationInfo.Controls.Add(label1);
            resources.ApplyResources(_tpApplicationInfo, "_tpApplicationInfo");
            _tpApplicationInfo.Name = "_tpApplicationInfo";
            resources.ApplyResources(_descriptionPLCTextBox, "_descriptionPLCTextBox");
            _descriptionPLCTextBox.Name = "_descriptionPLCTextBox";
            _descriptionPLCTextBox.ReadOnly = true;
            resources.ApplyResources(_descriptionIDETextBox, "_descriptionIDETextBox");
            _descriptionIDETextBox.Name = "_descriptionIDETextBox";
            _descriptionIDETextBox.ReadOnly = true;
            _versionPLCTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_versionPLCTextBox, "_versionPLCTextBox");
            _versionPLCTextBox.Name = "_versionPLCTextBox";
            _versionPLCTextBox.ReadOnly = true;
            _versionIDETextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_versionIDETextBox, "_versionIDETextBox");
            _versionIDETextBox.Name = "_versionIDETextBox";
            _versionIDETextBox.ReadOnly = true;
            _authorPLCTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_authorPLCTextBox, "_authorPLCTextBox");
            _authorPLCTextBox.Name = "_authorPLCTextBox";
            _authorPLCTextBox.ReadOnly = true;
            _authorIDETextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_authorIDETextBox, "_authorIDETextBox");
            _authorIDETextBox.Name = "_authorIDETextBox";
            _authorIDETextBox.ReadOnly = true;
            _ideVersionPLCTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_ideVersionPLCTextBox, "_ideVersionPLCTextBox");
            _ideVersionPLCTextBox.Name = "_ideVersionPLCTextBox";
            _ideVersionPLCTextBox.ReadOnly = true;
            _ideVersionIDETextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_ideVersionIDETextBox, "_ideVersionIDETextBox");
            _ideVersionIDETextBox.Name = "_ideVersionIDETextBox";
            _ideVersionIDETextBox.ReadOnly = true;
            _lastModificationPLCTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_lastModificationPLCTextBox, "_lastModificationPLCTextBox");
            _lastModificationPLCTextBox.Name = "_lastModificationPLCTextBox";
            _lastModificationPLCTextBox.ReadOnly = true;
            _lastModificationIDETextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_lastModificationIDETextBox, "_lastModificationIDETextBox");
            _lastModificationIDETextBox.Name = "_lastModificationIDETextBox";
            _lastModificationIDETextBox.ReadOnly = true;
            _projectNamePLCTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_projectNamePLCTextBox, "_projectNamePLCTextBox");
            _projectNamePLCTextBox.Name = "_projectNamePLCTextBox";
            _projectNamePLCTextBox.ReadOnly = true;
            _projectNameIDETextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_projectNameIDETextBox, "_projectNameIDETextBox");
            _projectNameIDETextBox.Name = "_projectNameIDETextBox";
            _projectNameIDETextBox.ReadOnly = true;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            _tpApplicationContent.BackColor = System.Drawing.SystemColors.Control;
            _tpApplicationContent.Controls.Add(_rightLabel);
            _tpApplicationContent.Controls.Add(_leftLabel);
            _tpApplicationContent.Controls.Add(_panel1);
            resources.ApplyResources(_tpApplicationContent, "_tpApplicationContent");
            _tpApplicationContent.Name = "_tpApplicationContent";
            _tpApplicationContent.Click += new System.EventHandler(_tpApplicationContent_Click);
            _rightLabel.AutoEllipsis = true;
            _rightLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(_rightLabel, "_rightLabel");
            _rightLabel.Name = "_rightLabel";
            _leftLabel.AutoEllipsis = true;
            _leftLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(_leftLabel, "_leftLabel");
            _leftLabel.Name = "_leftLabel";
            _panel1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(_panel1, "_panel1");
            _panel1.Name = "_panel1";
            resources.ApplyResources(_linkGenerateCode, "_linkGenerateCode");
            _linkGenerateCode.Name = "_linkGenerateCode";
            _linkGenerateCode.TabStop = true;
            _linkGenerateCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(_linkGenerateCode_LinkClicked);
            base.AcceptButton = _closeButton;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _closeButton;
            base.Controls.Add(_linkGenerateCode);
            base.Controls.Add(_tcApplicationInformation);
            base.Controls.Add(_closeButton);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ApplicationInfoDialogWithContent";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            _tcApplicationInformation.ResumeLayout(false);
            _tpChanges.ResumeLayout(false);
            _tpChanges.PerformLayout();
            _tpApplicationInfo.ResumeLayout(false);
            _tpApplicationInfo.PerformLayout();
            _tpApplicationContent.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
