using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_status.htm")]
	[AssociatedOnlineHelpTopic("core.DeviceEditor.Editor.chm::/Status.htm")]
	public class StatusPage : UserControl, IEditorPage, IEditorPageAppearance2, IEditorPageAppearance, IVisibleEditor
	{
		private DeviceEditor _editor;

		private IEnumerable<IStatusPageAdditionalControlFactory> _factories = APEnvironment.CreateStatusPageAdditionalControlFactories();

		private bool _bIsHidden = true;

		private IContainer components;

		private TableLayoutPanel _panelMain;

		public DeviceEditor DeviceEditor
		{
			get
			{
				return _editor;
			}
			set
			{
				_editor = value;
			}
		}

		public int NumberPanels => _panelMain.Controls.Count;

		public string PageName => Strings.PageNameStatus;

		public Icon Icon => null;

		public Control Control => this;

		public string PageIdentifier => "StatusPage";

		public bool HasOnlineMode => true;

		public bool IsHidden
		{
			get
			{
				return _bIsHidden;
			}
			set
			{
				_bIsHidden = value;
				foreach (Control control in _panelMain.Controls)
				{
					if (control is IVisibleEditor)
					{
						((IVisibleEditor)((control is IVisibleEditor) ? control : null)).IsHidden=(value);
					}
				}
			}
		}

		public StatusPage()
		{
			InitializeComponent();
		}

		public void SetDevice(IDeviceObject device)
		{
			StatusControl statusControl = new StatusControl(this, device);
			if (statusControl.DiagPanelHidden)
			{
				statusControl.Dock = DockStyle.Top;
			}
			else
			{
				statusControl.Dock = DockStyle.Fill;
			}
			_panelMain.Controls.Add(statusControl);
			statusControl.BringToFront();
		}

		public void AddConnector(IConnector connector, Guid guidObject, ref bool bShowOnlyAdditionalControl, ref bool bShowOnlyDiagConnector)
		{
			bool bHideDiagParameter = false;
			bool bHideDiagAckParameter = false;
			int num = 0;
			foreach (IStatusPageAdditionalControlFactory factory in _factories)
			{
				try
				{
					if (!factory.GetMatch(connector.GetDeviceObject(), connector))
					{
						continue;
					}
					IStatusPageAdditionalControl addtionalControl = factory.GetAddtionalControl(connector.GetDeviceObject(), connector);
					if (addtionalControl is IStatusPageAdditionalControl2)
					{
						bHideDiagParameter = ((IStatusPageAdditionalControl2)((addtionalControl is IStatusPageAdditionalControl2) ? addtionalControl : null)).HideDiagParameter;
						bHideDiagAckParameter = ((IStatusPageAdditionalControl2)((addtionalControl is IStatusPageAdditionalControl2) ? addtionalControl : null)).HideDiagAckParameter;
						bShowOnlyAdditionalControl = ((IStatusPageAdditionalControl2)((addtionalControl is IStatusPageAdditionalControl2) ? addtionalControl : null)).ShowOnlyAdditionalControl;
						bShowOnlyDiagConnector = ((IStatusPageAdditionalControl2)((addtionalControl is IStatusPageAdditionalControl2) ? addtionalControl : null)).ShowOnlyDiagConnector;
					}
					if (bShowOnlyAdditionalControl | bShowOnlyDiagConnector)
					{
						_panelMain.Controls.Clear();
					}
					if (addtionalControl != null)
					{
						addtionalControl.ConnectorEditorFrame=((IConnectorEditorFrame)(object)_editor);
						addtionalControl.ConnectorId=(connector.ConnectorId);
						UserControl control = addtionalControl.GetControl();
						if (bShowOnlyAdditionalControl)
						{
							control.Dock = DockStyle.Fill;
						}
						else
						{
							control.Dock = DockStyle.Top;
						}
						_panelMain.Controls.Add(control, 0, num++);
						control.BringToFront();
					}
				}
				catch
				{
				}
			}
			if (bShowOnlyAdditionalControl)
			{
				return;
			}
			bool flag = false;
			IConnector7 val = (IConnector7)(object)((connector is IConnector7) ? connector : null);
			if (val != null)
			{
				flag = val.HideInStatusPage;
			}
			if (flag)
			{
				return;
			}
			StatusControl statusControl = new StatusControl(this, connector, guidObject, bHideDiagParameter, bHideDiagAckParameter);
			if (!bShowOnlyDiagConnector || statusControl.DiagAckParameterAvailable || statusControl.DiagParameterAvailable)
			{
				if (statusControl.DiagPanelHidden)
				{
					statusControl.Dock = DockStyle.Top;
				}
				else
				{
					statusControl.Dock = DockStyle.Fill;
				}
				_panelMain.Controls.Add(statusControl);
				statusControl.BringToFront();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(_3S.CoDeSys.DeviceEditor.StatusPage));
			_panelMain = new System.Windows.Forms.TableLayoutPanel();
			SuspendLayout();
			resources.ApplyResources(_panelMain, "_panelMain");
			_panelMain.Name = "_panelMain";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			base.Controls.Add(_panelMain);
			base.Name = "StatusPage";
			ResumeLayout(false);
		}
	}
}
