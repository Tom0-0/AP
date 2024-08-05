#define DEBUG
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_information.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceEditor.Editor.chm::/Information.htm")]
	public class DeviceInformationControl : UserControl, IEditorPage, IDeviceEditor, IBaseDeviceEditor, IEditorPageAppearance2, IEditorPageAppearance, IPrintableEx
	{
		private Container components;

		private RichTextBox _richTextBox;

		private StatusPage _statusPage;

		private Button _btHelp;

		private Panel panel1;

		private Panel panel3;

		private Panel _panelImage;

		private TaskUpdatePage _taskPage;

		private GroupBox _imageGroupBox;

		private PictureBox _imagePictureBox;

		private GroupBox _generalGroupBox;

		private PictureBox _iconPictureBox;

		private IDeviceEditorFrame _editorFrame;

		private static readonly Guid GUID_ONLINEHELPSERVICE = new Guid("{EF0330CE-5108-4f66-BA7A-100F9D4AF87D}");

		public string PageName => Strings.PageNameInformation;

		public Icon Icon => SystemIcons.Information;

		public Control Control => this;

		public IEditorPage[] Pages
		{
			get
			{
				LList<IEditorPage> val = new LList<IEditorPage>();
				if (_taskPage != null)
				{
					val.Add((IEditorPage)(object)_taskPage);
				}
				if (_statusPage != null)
				{
					val.Add((IEditorPage)(object)_statusPage);
				}
				val.Add((IEditorPage)(object)this);
				IEditorPage[] array = (IEditorPage[])(object)new IEditorPage[val.Count];
				val.CopyTo(array);
				return array;
			}
		}

		public IDeviceEditorFrame DeviceEditorFrame
		{
			get
			{
				return _editorFrame;
			}
			set
			{
				_editorFrame = value;
			}
		}

		public bool HideGenericEditor => true;

		public string PageIdentifier => "DeviceInformation";

		public bool HasOnlineMode => true;

		public DeviceInformationControl(StatusPage statusPage)
		{
			_statusPage = statusPage;
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.DeviceInformationControl));
			_imageGroupBox = new System.Windows.Forms.GroupBox();
			_imagePictureBox = new System.Windows.Forms.PictureBox();
			_generalGroupBox = new System.Windows.Forms.GroupBox();
			_btHelp = new System.Windows.Forms.Button();
			_richTextBox = new System.Windows.Forms.RichTextBox();
			_iconPictureBox = new System.Windows.Forms.PictureBox();
			panel1 = new System.Windows.Forms.Panel();
			panel3 = new System.Windows.Forms.Panel();
			_panelImage = new System.Windows.Forms.Panel();
			_imageGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).BeginInit();
			_generalGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).BeginInit();
			panel1.SuspendLayout();
			panel3.SuspendLayout();
			_panelImage.SuspendLayout();
			SuspendLayout();
			resources.ApplyResources(_imageGroupBox, "_imageGroupBox");
			_imageGroupBox.Controls.Add(_imagePictureBox);
			_imageGroupBox.Name = "_imageGroupBox";
			_imageGroupBox.TabStop = false;
			resources.ApplyResources(_imagePictureBox, "_imagePictureBox");
			_imagePictureBox.Name = "_imagePictureBox";
			_imagePictureBox.TabStop = false;
			resources.ApplyResources(_generalGroupBox, "_generalGroupBox");
			_generalGroupBox.Controls.Add(_btHelp);
			_generalGroupBox.Controls.Add(_richTextBox);
			_generalGroupBox.Controls.Add(_iconPictureBox);
			_generalGroupBox.Name = "_generalGroupBox";
			_generalGroupBox.TabStop = false;
			resources.ApplyResources(_btHelp, "_btHelp");
			_btHelp.Name = "_btHelp";
			_btHelp.UseVisualStyleBackColor = true;
			_btHelp.Click += new System.EventHandler(_btHelp_Click);
			resources.ApplyResources(_richTextBox, "_richTextBox");
			_richTextBox.BackColor = System.Drawing.Color.White;
			_richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_richTextBox.Name = "_richTextBox";
			_richTextBox.ReadOnly = true;
			resources.ApplyResources(_iconPictureBox, "_iconPictureBox");
			_iconPictureBox.Name = "_iconPictureBox";
			_iconPictureBox.TabStop = false;
			panel1.Controls.Add(panel3);
			panel1.Controls.Add(_panelImage);
			resources.ApplyResources(panel1, "panel1");
			panel1.Name = "panel1";
			resources.ApplyResources(panel3, "panel3");
			panel3.Controls.Add(_generalGroupBox);
			panel3.Name = "panel3";
			resources.ApplyResources(_panelImage, "_panelImage");
			_panelImage.Controls.Add(_imageGroupBox);
			_panelImage.Name = "_panelImage";
			resources.ApplyResources(this, "$this");
			base.Controls.Add(panel1);
			base.Name = "DeviceInformationControl";
			_imageGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_imagePictureBox).EndInit();
			_generalGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)_iconPictureBox).EndInit();
			panel1.ResumeLayout(false);
			panel3.ResumeLayout(false);
			_panelImage.ResumeLayout(false);
			ResumeLayout(false);
		}

		public void Reload()
		{
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			if (_editorFrame is DeviceEditor && !(((IEditorView)_editorFrame).Editor as DeviceEditor).OpenView)
			{
				return;
			}
			IDeviceObject deviceObject = _editorFrame.GetDeviceObject(false);
			if (deviceObject != null)
			{
				IDeviceInfo deviceInfo = deviceObject.DeviceInfo;
				string text = GetCategoriesString(deviceObject.DeviceInfo.Categories);
				string familiesString = GetFamiliesString(deviceObject.DeviceInfo);
				if (!string.IsNullOrEmpty(familiesString))
				{
					text = (string.IsNullOrEmpty(text) ? familiesString : (text + ", " + familiesString));
				}
				AddRichText(_richTextBox, Strings.InformationName, (deviceInfo.Name != null) ? deviceInfo.Name : string.Empty);
				AddRichText(_richTextBox, Strings.InformationVendor, (deviceInfo.Vendor != null) ? deviceInfo.Vendor : string.Empty);
				AddRichText(_richTextBox, Strings.InformationGroups, text);
				IDeviceObject5 val = (IDeviceObject5)(object)((deviceObject is IDeviceObject5) ? deviceObject : null);
				IDeviceIdentification val2 = ((val == null) ? deviceObject.DeviceIdentification : val.DeviceIdentificationNoSimulation);
				AddRichText(_richTextBox, Strings.InformationType, val2.Type.ToString());
				AddRichText(_richTextBox, Strings.InformationID, val2.Id);
				if (val2 is IModuleIdentification)
				{
					AddRichText(_richTextBox, Strings.InformationModuleID, ((IModuleIdentification)((val2 is IModuleIdentification) ? val2 : null)).ModuleId);
				}
				string text2 = val2.Version;
				if (Version.TryParse(text2, out var result))
				{
					text2 = result.ToString();
				}
				AddRichText(_richTextBox, Strings.InformationVersion, text2);
				if (!string.IsNullOrEmpty(deviceInfo.OrderNumber))
				{
					AddRichText(_richTextBox, Strings.InformationModelNumber, (deviceInfo.OrderNumber != null) ? deviceInfo.OrderNumber : string.Empty);
				}
				AddRichText(_richTextBox, Strings.InformationDescription, (deviceInfo.Description != null) ? deviceInfo.Description : string.Empty);
				object value = default(object);
				TypeClass val3 = default(TypeClass);
				foreach (IConnector item in (IEnumerable)deviceObject.Connectors)
				{
					IParameter parameter = item.HostParameterSet.GetParameter(1879052288L);
					if (parameter == null || string.IsNullOrEmpty(((IDataElement)parameter).Value))
					{
						continue;
					}
					IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
					try
					{
						converterFromIEC.GetInteger(((IDataElement)parameter).Value, out value, out val3);
						byte[] bytes = BitConverter.GetBytes(Convert.ToUInt32(value));
						if (bytes.Length == 4)
						{
							Version version = new Version(bytes[3], bytes[2], bytes[1], bytes[0]);
							AddRichText(_richTextBox, Strings.InformationConfigVersion, version.ToString());
						}
					}
					catch
					{
					}
				}
				Image image = deviceObject.DeviceInfo.Image;
				if (image != null)
				{
					_imagePictureBox.Image = image;
					if (image.Height < _imagePictureBox.Size.Height && image.Width < _imagePictureBox.Size.Width)
					{
						_imagePictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
					}
					else
					{
						_imagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
					}
					_imagePictureBox.Visible = true;
				}
				else
				{
					_panelImage.Visible = false;
				}
				if (deviceObject.AllowsDirectCommunication && ((IEditorView)_editorFrame).Editor.GetObjectToRead().Object is IDeviceObject)
				{
					_taskPage = new TaskUpdatePage();
					_taskPage.DeviceEditor = _editorFrame as DeviceEditor;
				}
			}
			if (deviceObject is IDeviceObject12 && string.IsNullOrEmpty(((IDeviceObject12)((deviceObject is IDeviceObject12) ? deviceObject : null)).OnlineHelpUrl))
			{
				_btHelp.Visible = false;
			}
		}

		private void AddRichText(RichTextBox textBox, string stCaption, string stValue)
		{
			using Font selectionFont = new Font(Font, FontStyle.Bold);
			textBox.SelectionFont = selectionFont;
			textBox.SelectedText = stCaption + " ";
			textBox.SelectionFont = Font;
			textBox.SelectedText = stValue + "\n";
		}

		public void Save(bool bCommit)
		{
		}

		internal static string GetCategoriesString(int[] categories)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			Debug.Assert(categories != null);
			LStringBuilder val = new LStringBuilder();
			bool flag = true;
			foreach (int num in categories)
			{
				if (!flag)
				{
					val.Append(", ");
				}
				try
				{
					IDeviceCategory deviceCategory = APEnvironment.DeviceRepository.GetDeviceCategory(num);
					if (deviceCategory != null)
					{
						val.Append(deviceCategory.Name);
						flag = false;
					}
				}
				catch
				{
				}
			}
			return ((object)val).ToString();
		}

		internal static string GetFamiliesString(IDeviceInfo deviceInfo)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			LStringBuilder val = new LStringBuilder();
			if (deviceInfo.Families != null && deviceInfo.Families.Length != 0)
			{
				bool flag = true;
				string[] families = deviceInfo.Families;
				foreach (string text in families)
				{
					IDeviceFamily deviceFamily = APEnvironment.DeviceRepository.GetDeviceFamily(text);
					if (deviceFamily != null)
					{
						if (!flag)
						{
							val.Append(", ");
						}
						val.Append(deviceFamily.Name);
						flag = false;
					}
				}
			}
			return ((object)val).ToString();
		}

		public HideParameterDelegate GetParameterFilter()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return new HideParameterDelegate(HideParameter);
		}

		public bool HideParameter(int nParameterId, string[] componentPath)
		{
			return true;
		}

		public IPrintPainterEx CreatePrintPainter(long nPosition, int nLength)
		{
			return (IPrintPainterEx)(object)new DeviceInformationPrintPainter(_editorFrame.GetDeviceObject(false));
		}

		internal static IOnlineHelpService CreateOnlineHelpService()
		{
			Guid typeGuid = GUID_ONLINEHELPSERVICE;
			if (APEnvironment.Engine.OEMCustomization.HasValue("Frame", "OnlineHelpService"))
			{
				typeGuid = (Guid)APEnvironment.Engine.OEMCustomization.GetValue("Frame", "OnlineHelpService");
			}
			return APEnvironment.CreateOnlineHelpService(typeGuid);
		}

		private void _btHelp_Click(object sender, EventArgs e)
		{
			IDeviceObject deviceObject = _editorFrame.GetDeviceObject(false);
			IDeviceObject12 val = (IDeviceObject12)(object)((deviceObject is IDeviceObject12) ? deviceObject : null);
			if (val != null && !string.IsNullOrEmpty(val.OnlineHelpUrl))
			{
				if (val.OnlineHelpUrl.ToLowerInvariant().StartsWith("http://"))
				{
					Process.Start(val.OnlineHelpUrl);
					return;
				}
				IOnlineHelpService obj = CreateOnlineHelpService();
				Debug.Assert(obj != null);
				obj.ShowTopic(val.OnlineHelpUrl);
			}
		}
	}
}
