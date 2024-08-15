#define DEBUG
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.DeviceObject
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_device_tree_device_editor.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/Device_Device_tree.htm")]
	[AssociatedOnlineHelpTopic("core.frame.overview.chm::/Device,_Device_tree.htm")]
	public class WizardPage : SelectDeviceTypeControl, IValidateableControl, IWizardPageHasDefaultSize
	{
		private Container components;

		protected override bool InDesigner => base.DesignMode;

		Size IWizardPageHasDefaultSize.DefaultSize => new Size(550, 400);

		public WizardPage()
		{
			InitializeComponent();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			if (base.Visible && !base.DesignMode)
			{
				UpdateContext();
			}
			base.OnVisibleChanged(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!base.DesignMode)
			{
				UpdateContext();
			}
			base.OnLoad(e);
		}

		private void UpdateContext()
		{
			if (base.Catalogue.Filter == null)
			{
				DeviceObjectHelper.UpdateCatalogueContext(base.Catalogue);
			}
		}

		public bool Validate(ref string stMessage, ref Control failedControl)
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			Debug.Assert(primaryProject != null);
			if (base.Catalogue.SelectedDevice == null)
			{
				stMessage = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageErrorNoDevice");
				failedControl = base.Catalogue.Control;
				return false;
			}
			if (!DeviceObjectHelper.CheckUniqueIdentifier(primaryProject.Handle, base.NameControl.Text, DeviceCommandHelper.GetSelectedStub()))
			{
				stMessage = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WizardPageNameNotUnique");
				failedControl = base.NameControl;
				return false;
			}
			return true;
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceObject.WizardPage));
			SuspendLayout();
			base.Name = "WizardPage";
			componentResourceManager.ApplyResources(this, "$this");
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
