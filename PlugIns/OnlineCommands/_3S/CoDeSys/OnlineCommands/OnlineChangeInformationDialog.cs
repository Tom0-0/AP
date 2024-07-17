using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_login.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/login_to.htm")]
    public class OnlineChangeInformationDialog : Form
    {
        private ICompileContext7 _comcon;

        private IOnlineChangeDetails2 _ocd;

        private IContainer components;

        private Button _closeButton;

        private TextBox _descriptionIDETextBox;

        private TextBox _tbNumOfChangedInterfaces;

        private TextBox _tbNumOfChangedPOUs;

        private TextBox _tbNumOfVarsAffected;

        private TextBox _tbInterfacesToTest;

        private TextBox _tbNumLocationChanged;

        private TextBox _tbNumInitialized;

        private TextBox _tbToCopy;

        private TextBox _tbVFTableChanged;

        private TextBox _tbReinit;

        private TextBox _tbInformation;

        private TextBox _tbInstImplItf;

        private TextBox _tbTotalRelinkTests;

        private Button _btnSave;

        public OnlineChangeInformationDialog()
        {
            InitializeComponent();
        }

        internal void DumpOCDetailsToFile()
        {
            //IL_0040: Unknown result type (might be due to invalid IL or missing references)
            //IL_0046: Expected O, but got Unknown
            //IL_0227: Unknown result type (might be due to invalid IL or missing references)
            //IL_0265: Unknown result type (might be due to invalid IL or missing references)
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Textfile|*.txt";
            saveFileDialog.Title = "Save Onlinechange Details to a file";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                TextualOnlineChangeDetails textualOnlineChangeDetails = GenerateOCDetails(bTruncated: false);
                LStringBuilder val = new LStringBuilder();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblNumChangedItfs,
                    textualOnlineChangeDetails.changedsignatures.Count
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblNumChangedPOUs,
                    textualOnlineChangeDetails.cpouschanged.Count
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblNumAffectedVars,
                    ((IOnlineChangeDetails)_ocd).VariablesAffected.Count
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblChangedLoc,
                    textualOnlineChangeDetails.nCountLocationChanged
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblToInit,
                    textualOnlineChangeDetails.nCountInitialised
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblToReinit,
                    textualOnlineChangeDetails.nCountReinit
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblToCopy,
                    textualOnlineChangeDetails.nCountCopied
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblChangedVFT,
                    textualOnlineChangeDetails.nCountVFInitialised
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblNumItfTest,
                    _ocd.InterfacesToTest.Count
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblMovedInst,
                    ((IOnlineChangeDetails3)_ocd).InstancesToMove.Count
                });
                val.AppendLine();
                val.AppendFormat("{0}: {1}", new object[2]
                {
                    Strings.LblTotalRelinkTests,
                    ((IOnlineChangeDetails3)_ocd).TotalNumberOfRelinkTests
                });
                val.AppendLine();
                AuthFile.AppendAllText(saveFileDialog.FileName, ((object)val).ToString());
                AuthFile.AppendAllText(saveFileDialog.FileName, ((object)textualOnlineChangeDetails.stbInformation).ToString());
                AuthFile.AppendAllText(saveFileDialog.FileName, ((object)textualOnlineChangeDetails.stbInformationText).ToString());
            }
        }

        internal TextualOnlineChangeDetails GenerateOCDetails(bool bTruncated)
        {
            int num = 0;
            TextualOnlineChangeDetails textualOnlineChangeDetails = new TextualOnlineChangeDetails();
            IList<ICompiledPOU4> list;
            if (this._comcon is ICompileContext10)
            {
                list = (this._comcon as ICompileContext10).GetCompiledPOUsToCompileEx();
            }
            else
            {
                list = this._comcon.GetCompiledPOUsToCompile();
            }
            IList<ISignature4> list2;
            if (this._comcon is ICompileContext10)
            {
                list2 = (this._comcon as ICompileContext10).GetAllSignaturesFlatEx();
            }
            else
            {
                list2 = this._comcon.GetAllSignaturesFlat();
            }
            List<IVariableInfo> list3 = new List<IVariableInfo>();
            int num2 = 0;
            IScope scope = this._comcon.CreateGlobalIScope();
            foreach (IVariableInfo variableInfo in this._ocd.VariablesAffected)
            {
                ISignature signatureById = this._comcon.GetSignatureById(variableInfo.SignatureId);
                IVariable variable = signatureById[variableInfo.VariableId];
                if (!signatureById.Name.Contains("__") && !variable.Name.Contains("__"))
                {
                    list3.Add(variableInfo);
                    if ((variableInfo.Flags & VarFlag.LocationChanged) == VarFlag.LocationChanged)
                    {
                        textualOnlineChangeDetails.nCountLocationChanged++;
                    }
                    if ((variableInfo.Flags & VarFlag.OnlChangeReInit) == VarFlag.OnlChangeReInit)
                    {
                        textualOnlineChangeDetails.nCountReinit++;
                    }
                    else if ((variableInfo.Flags & VarFlag.OnlChangeInit) == VarFlag.OnlChangeInit)
                    {
                        textualOnlineChangeDetails.nCountInitialised++;
                    }
                    if ((variableInfo.Flags & VarFlag.OnlChangeCopy) == VarFlag.OnlChangeCopy)
                    {
                        textualOnlineChangeDetails.nCountCopied++;
                        IVariable variable2 = scope[variableInfo.SignatureId][variableInfo.VariableId];
                        IScope scope2 = this._comcon.CreateIScope(variableInfo.SignatureId);
                        num2 += variable2.CompiledType.Size(scope2);
                    }
                    if (((long)variableInfo.Flags & 0x80000000u) == 2147483648u)
                    {
                        textualOnlineChangeDetails.nCountVFInitialised++;
                    }
                }
            }
            IOnlineChangeDetails3 onlineChangeDetails = this._ocd as IOnlineChangeDetails3;
            if (list3.Count > 0)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.ListOfVariables);
            }
            num = 0;
            while (num < list3.Count && num < 100)
            {
                IVariableInfo variableInfo2 = list3[num];
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("(");
                bool flag = false;
                if ((variableInfo2.Flags & VarFlag.LocationChanged) == VarFlag.LocationChanged)
                {
                    stringBuilder.Append(Strings.AffectionLocationChanged);
                    flag = true;
                }
                if ((variableInfo2.Flags & VarFlag.OnlChangeReInit) == VarFlag.OnlChangeReInit)
                {
                    if (flag)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(Strings.AffectionReInitialized);
                    flag = true;
                }
                else if ((variableInfo2.Flags & VarFlag.OnlChangeInit) == VarFlag.OnlChangeInit)
                {
                    if (flag)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(Strings.AffectionInitialized);
                    flag = true;
                }
                if ((variableInfo2.Flags & VarFlag.OnlChangeCopy) == VarFlag.OnlChangeCopy)
                {
                    if (flag)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(Strings.AffectionCopied);
                    flag = true;
                }
                if (((long)variableInfo2.Flags & 0x80000000u) == 2147483648u)
                {
                    if (flag)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(Strings.AffectionVFTableInitialized);
                    flag = true;
                }
                if (variableInfo2.Flags == VarFlag.None)
                {
                    if (flag)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(Strings.AffectionInitNewVariables);
                }
                stringBuilder.Append(")");
                ISignature signatureById2 = this._comcon.GetSignatureById(variableInfo2.SignatureId);
                IVariable variable3 = signatureById2[variableInfo2.VariableId];
                textualOnlineChangeDetails.stbInformation.AppendFormat("    - {0}.{1}  {2}", new object[]
                {
                    signatureById2.OrgName,
                    variable3.OrgName,
                    stringBuilder.ToString()
                });
                textualOnlineChangeDetails.stbInformation.AppendLine();
                num++;
            }
            if (num < list3.Count && num == 101)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.VariableListExceeded);
            }
            if (list3.Count > 0)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine("");
            }
            foreach (ISignature4 signature in list2)
            {
                if (signature.GetFlag(SignatureFlag.OnlineChanged) && !signature.Name.Contains("__"))
                {
                    textualOnlineChangeDetails.changedsignatures.Add(signature);
                }
            }
            if (textualOnlineChangeDetails.changedsignatures.Count > 0)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.ListOfInterfaces);
            }
            num = 0;
            while (num < textualOnlineChangeDetails.changedsignatures.Count && num < 100)
            {
                ISignature4 signature2 = textualOnlineChangeDetails.changedsignatures[num];
                string text;
                if (signature2.ParentSignatureId != -1)
                {
                    text = this._comcon.GetSignatureById(signature2.ParentSignatureId).OrgName + "." + signature2.OrgName;
                }
                else
                {
                    text = signature2.OrgName;
                }
                textualOnlineChangeDetails.stbInformation.AppendFormat("    - {0}", new object[]
                {
                    text
                });
                textualOnlineChangeDetails.stbInformation.AppendLine();
                num++;
            }
            if (num < textualOnlineChangeDetails.changedsignatures.Count && num == 101)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.InterfaceListExceeded);
            }
            for (int i = 0; i < list.Count; i++)
            {
                ICompiledPOU compiledPOU = list[i];
                if (!compiledPOU.GetFlag(CompiledPOUFlags.Blob) && !compiledPOU.GetFlag(CompiledPOUFlags.ConstBlob) && ((!compiledPOU.GetFlag(CompiledPOUFlags.Generated) && !compiledPOU.Name.Contains("__")) || !(compiledPOU.Name != "__MAIN")) && compiledPOU.GetFlag(CompiledPOUFlags.ToCompile))
                {
                    textualOnlineChangeDetails.cpouschanged.Add(compiledPOU);
                }
            }
            if (textualOnlineChangeDetails.cpouschanged.Count > 0)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine("");
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.ListOfChangedPOUs);
            }
            num = 0;
            while (num < textualOnlineChangeDetails.cpouschanged.Count && num < 100)
            {
                ICompiledPOU compiledPOU2 = textualOnlineChangeDetails.cpouschanged[num];
                ISignature signatureById3 = this._comcon.GetSignatureById(compiledPOU2.SignatureId);
                if (signatureById3 != null)
                {
                    Guid objectGuid = signatureById3.ObjectGuid;
                    string text2;
                    if (signatureById3.ParentSignatureId != -1)
                    {
                        ISignature signatureById4 = this._comcon.GetSignatureById(signatureById3.ParentSignatureId);
                        if (signatureById3.Name == "__MAIN")
                        {
                            text2 = signatureById4.OrgName;
                            Guid objectGuid2 = signatureById4.ObjectGuid;
                        }
                        else
                        {
                            text2 = signatureById4.OrgName + "." + signatureById3.OrgName;
                        }
                    }
                    else
                    {
                        text2 = signatureById3.OrgName;
                    }
                    textualOnlineChangeDetails.stbInformation.AppendFormat("    - {0}", new object[]
                    {
                        text2
                    });
                    textualOnlineChangeDetails.stbInformation.AppendLine();
                }
                num++;
            }
            if (num < textualOnlineChangeDetails.changedsignatures.Count && num == 101)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.POUListExceeded);
            }
            if (this._ocd.InterfacesToTest.Count > 0)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine("");
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.ListOfInterfaceReferences);
            }
            num = 0;
            foreach (KeyValuePair<string, IVariableInfo> keyValuePair in onlineChangeDetails.InterfacesToRelink)
            {
                IVariableInfo value = keyValuePair.Value;
                ISignature signatureById5 = this._comcon.GetSignatureById(value.SignatureId);
                IVariable variable4 = signatureById5[value.VariableId];
                textualOnlineChangeDetails.stbInformation.AppendFormat("    - {2} ({0}.{1})", new object[]
                {
                    signatureById5.OrgName,
                    variable4.OrgName,
                    keyValuePair.Key
                });
                textualOnlineChangeDetails.stbInformation.AppendLine();
                if (bTruncated && num++ > 100)
                {
                    break;
                }
            }
            if (bTruncated && num < textualOnlineChangeDetails.changedsignatures.Count && num == 101)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.InterfaceReferenceListExceeded);
            }
            if (onlineChangeDetails.InstancesToMove.Count > 0)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine("");
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.ListOfInstancesImplementingInterfaces);
            }
            num = 0;
            foreach (KeyValuePair<string, IVariableInfo> keyValuePair2 in onlineChangeDetails.InstancesToMove)
            {
                IVariableInfo value2 = keyValuePair2.Value;
                ISignature signatureById6 = this._comcon.GetSignatureById(value2.SignatureId);
                IVariable variable5 = signatureById6[value2.VariableId];
                textualOnlineChangeDetails.stbInformation.AppendFormat("    - {2} ({0}.{1})", new object[]
                {
                    signatureById6.OrgName,
                    variable5.OrgName,
                    keyValuePair2.Key
                });
                textualOnlineChangeDetails.stbInformation.AppendLine();
                if (bTruncated && num++ > 100)
                {
                    break;
                }
            }
            if (bTruncated && num < textualOnlineChangeDetails.changedsignatures.Count && num == 101)
            {
                textualOnlineChangeDetails.stbInformation.AppendLine(Strings.InterfaceReferenceListExceeded);
            }
            if (textualOnlineChangeDetails.nCountCopied > 0)
            {
                textualOnlineChangeDetails.stbInformationText.AppendLine(string.Format(Strings.OnlineChangeinformation_LocationChanged, num2));
            }
            if (textualOnlineChangeDetails.nCountReinit > 0)
            {
                textualOnlineChangeDetails.stbInformationText.AppendLine(Strings.OnlineChangeinformation_ReInit);
            }
            IOnlineApplication application = APEnvironment.OnlineMgr.GetApplication(this._comcon.ApplicationGuid);
            if (application != null && application.ApplicationState == ApplicationState.run)
            {
                if (textualOnlineChangeDetails.nCountReinit > 0 || textualOnlineChangeDetails.nCountCopied > 0)
                {
                    textualOnlineChangeDetails.stbInformationText.AppendLine(Strings.OnlineChangeinformation_General_Warning);
                }
                else
                {
                    textualOnlineChangeDetails.stbInformationText.AppendLine(Strings.OnlineChangeinformation_General_OK);
                }
            }
            return textualOnlineChangeDetails;
        }

        internal void Initialize(IOnlineChangeDetails2 ocd, ICompileContext7 comconNew)
        {
            //IL_00ef: Unknown result type (might be due to invalid IL or missing references)
            //IL_0117: Unknown result type (might be due to invalid IL or missing references)
            _ocd = ocd;
            _comcon = comconNew;
            TextualOnlineChangeDetails textualOnlineChangeDetails = GenerateOCDetails(bTruncated: true);
            _tbNumOfChangedInterfaces.Text = textualOnlineChangeDetails.changedsignatures.Count.ToString();
            _tbNumOfVarsAffected.Text = ((IOnlineChangeDetails)ocd).VariablesAffected.Count.ToString();
            _tbNumLocationChanged.Text = textualOnlineChangeDetails.nCountLocationChanged.ToString();
            _tbReinit.Text = textualOnlineChangeDetails.nCountReinit.ToString();
            _tbNumInitialized.Text = textualOnlineChangeDetails.nCountInitialised.ToString();
            _tbToCopy.Text = textualOnlineChangeDetails.nCountCopied.ToString();
            _tbVFTableChanged.Text = textualOnlineChangeDetails.nCountVFInitialised.ToString();
            _tbInterfacesToTest.Text = _ocd.InterfacesToTest.Count.ToString();
            _tbInstImplItf.Text = ((IOnlineChangeDetails3)_ocd).InstancesToMove.Count.ToString();
            _tbTotalRelinkTests.Text = ((IOnlineChangeDetails3)_ocd).TotalNumberOfRelinkTests.ToString();
            _descriptionIDETextBox.Text = ((object)textualOnlineChangeDetails.stbInformation).ToString();
            _tbNumOfChangedPOUs.Text = textualOnlineChangeDetails.cpouschanged.Count.ToString();
            _tbInformation.Text = ((object)textualOnlineChangeDetails.stbInformationText).ToString();
        }

        private void SaveToFile_Click(object sender, EventArgs e)
        {
            DumpOCDetailsToFile();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.OnlineCommands.OnlineChangeInformationDialog));
            _closeButton = new System.Windows.Forms.Button();
            _descriptionIDETextBox = new System.Windows.Forms.TextBox();
            _tbNumOfChangedInterfaces = new System.Windows.Forms.TextBox();
            _tbNumOfChangedPOUs = new System.Windows.Forms.TextBox();
            _tbNumOfVarsAffected = new System.Windows.Forms.TextBox();
            _tbInterfacesToTest = new System.Windows.Forms.TextBox();
            _tbNumLocationChanged = new System.Windows.Forms.TextBox();
            _tbNumInitialized = new System.Windows.Forms.TextBox();
            _tbToCopy = new System.Windows.Forms.TextBox();
            _tbVFTableChanged = new System.Windows.Forms.TextBox();
            _tbReinit = new System.Windows.Forms.TextBox();
            _tbInformation = new System.Windows.Forms.TextBox();
            _tbInstImplItf = new System.Windows.Forms.TextBox();
            _tbTotalRelinkTests = new System.Windows.Forms.TextBox();
            _btnSave = new System.Windows.Forms.Button();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label3 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label4 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label5 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label6 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label7 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label8 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label9 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label10 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label11 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label12 = new System.Windows.Forms.Label();
            System.Windows.Forms.Label label13 = new System.Windows.Forms.Label();
            SuspendLayout();
            resources.ApplyResources(label, "label8");
            label.Name = "label8";
            resources.ApplyResources(label2, "label3");
            label2.Name = "label3";
            resources.ApplyResources(label3, "label1");
            label3.Name = "label1";
            resources.ApplyResources(label4, "label2");
            label4.Name = "label2";
            resources.ApplyResources(label5, "label4");
            label5.Name = "label4";
            resources.ApplyResources(label6, "label5");
            label6.Name = "label5";
            resources.ApplyResources(label7, "label6");
            label7.Name = "label6";
            resources.ApplyResources(label8, "label7");
            label8.Name = "label7";
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            resources.ApplyResources(label11, "label11");
            label11.Name = "label11";
            resources.ApplyResources(label12, "label12");
            label12.Name = "label12";
            resources.ApplyResources(label13, "label13");
            label13.Name = "label13";
            _closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(_closeButton, "_closeButton");
            _closeButton.Name = "_closeButton";
            _closeButton.UseVisualStyleBackColor = true;
            resources.ApplyResources(_descriptionIDETextBox, "_descriptionIDETextBox");
            _descriptionIDETextBox.Name = "_descriptionIDETextBox";
            _descriptionIDETextBox.ReadOnly = true;
            _tbNumOfChangedInterfaces.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbNumOfChangedInterfaces, "_tbNumOfChangedInterfaces");
            _tbNumOfChangedInterfaces.Name = "_tbNumOfChangedInterfaces";
            _tbNumOfChangedInterfaces.ReadOnly = true;
            _tbNumOfChangedPOUs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbNumOfChangedPOUs, "_tbNumOfChangedPOUs");
            _tbNumOfChangedPOUs.Name = "_tbNumOfChangedPOUs";
            _tbNumOfChangedPOUs.ReadOnly = true;
            _tbNumOfVarsAffected.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbNumOfVarsAffected, "_tbNumOfVarsAffected");
            _tbNumOfVarsAffected.Name = "_tbNumOfVarsAffected";
            _tbNumOfVarsAffected.ReadOnly = true;
            _tbInterfacesToTest.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbInterfacesToTest, "_tbInterfacesToTest");
            _tbInterfacesToTest.Name = "_tbInterfacesToTest";
            _tbInterfacesToTest.ReadOnly = true;
            _tbNumLocationChanged.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbNumLocationChanged, "_tbNumLocationChanged");
            _tbNumLocationChanged.Name = "_tbNumLocationChanged";
            _tbNumLocationChanged.ReadOnly = true;
            _tbNumInitialized.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbNumInitialized, "_tbNumInitialized");
            _tbNumInitialized.Name = "_tbNumInitialized";
            _tbNumInitialized.ReadOnly = true;
            _tbToCopy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbToCopy, "_tbToCopy");
            _tbToCopy.Name = "_tbToCopy";
            _tbToCopy.ReadOnly = true;
            _tbVFTableChanged.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbVFTableChanged, "_tbVFTableChanged");
            _tbVFTableChanged.Name = "_tbVFTableChanged";
            _tbVFTableChanged.ReadOnly = true;
            _tbReinit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbReinit, "_tbReinit");
            _tbReinit.Name = "_tbReinit";
            _tbReinit.ReadOnly = true;
            resources.ApplyResources(_tbInformation, "_tbInformation");
            _tbInformation.Name = "_tbInformation";
            _tbInformation.ReadOnly = true;
            _tbInstImplItf.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbInstImplItf, "_tbInstImplItf");
            _tbInstImplItf.Name = "_tbInstImplItf";
            _tbInstImplItf.ReadOnly = true;
            _tbTotalRelinkTests.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(_tbTotalRelinkTests, "_tbTotalRelinkTests");
            _tbTotalRelinkTests.Name = "_tbTotalRelinkTests";
            _tbTotalRelinkTests.ReadOnly = true;
            resources.ApplyResources(_btnSave, "_btnSave");
            _btnSave.Name = "_btnSave";
            _btnSave.UseVisualStyleBackColor = true;
            _btnSave.Click += new System.EventHandler(SaveToFile_Click);
            base.AcceptButton = _closeButton;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = _closeButton;
            base.Controls.Add(_btnSave);
            base.Controls.Add(_tbTotalRelinkTests);
            base.Controls.Add(_tbInstImplItf);
            base.Controls.Add(label13);
            base.Controls.Add(label12);
            base.Controls.Add(label11);
            base.Controls.Add(_tbInformation);
            base.Controls.Add(_tbReinit);
            base.Controls.Add(label10);
            base.Controls.Add(_tbVFTableChanged);
            base.Controls.Add(_tbToCopy);
            base.Controls.Add(_tbNumInitialized);
            base.Controls.Add(label9);
            base.Controls.Add(label8);
            base.Controls.Add(label7);
            base.Controls.Add(_tbNumLocationChanged);
            base.Controls.Add(label6);
            base.Controls.Add(_tbInterfacesToTest);
            base.Controls.Add(label5);
            base.Controls.Add(_tbNumOfVarsAffected);
            base.Controls.Add(label4);
            base.Controls.Add(_tbNumOfChangedPOUs);
            base.Controls.Add(label3);
            base.Controls.Add(label2);
            base.Controls.Add(_tbNumOfChangedInterfaces);
            base.Controls.Add(label);
            base.Controls.Add(_descriptionIDETextBox);
            base.Controls.Add(_closeButton);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "OnlineChangeInformationDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
