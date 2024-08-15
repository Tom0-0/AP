using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject.LogicalDevice;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7E96B4F7-B1A1-4097-943D-86B81F4ABC23}")]
	public class LogicalIODeviceFactory : IObjectFactory
	{
		private WizardPage _wizardPage;

		public Guid Namespace => DeviceObject.GUID_DEVICENAMESPACE;

		public string Name => LogicalIOStrings.LogicalIOName;

		public string Description => LogicalIOStrings.LogicalIODescription;

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.DeviceObject.Resources.LogicalIconSmall.ico");

		public Icon LargeIcon => SmallIcon;

		public Control WizardPage => GetWizardPage(bReload: true);

		public Control ObjectNameControl => GetWizardPage(bReload: false).NameControl;

		public string ObjectBaseName => "LogicalIO";

		public Type ObjectType => typeof(LogicalIODevice);

		public Type[] EmbeddedObjectTypes => new Type[0];

		public IObject Create()
		{
			APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(_wizardPage.SelectedDevice);
			IDeviceIdentification selectedDevice = _wizardPage.SelectedDevice;
			IModuleIdentification val = (IModuleIdentification)(object)((selectedDevice is IModuleIdentification) ? selectedDevice : null);
			if (val == null)
			{
				return Create(new string[3]
				{
					selectedDevice.Type.ToString(),
					selectedDevice.Id,
					selectedDevice.Version
				});
			}
			return Create(new string[4]
			{
				selectedDevice.Type.ToString(),
				selectedDevice.Id,
				selectedDevice.Version,
				val.ModuleId
			});
		}

		public void ObjectCreated(int nProjectHandle, Guid guidObject)
		{
		}

		public IObject Create(string[] stBatchArguments)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			if (stBatchArguments.Length < 3)
			{
				throw new BatchTooFewArgumentsException(new string[0], stBatchArguments.Length, 3);
			}
			if (stBatchArguments.Length > 5)
			{
				throw new BatchTooManyArgumentsException(new string[0], stBatchArguments.Length, 5);
			}
			int type;
			try
			{
				type = int.Parse(stBatchArguments[0]);
			}
			catch
			{
				throw new BatchWrongArgumentTypeException(new string[0], 0, "int");
			}
			CreateDeviceFlags createDeviceFlags = CreateDeviceFlags.None;
			if (stBatchArguments.Length == 3)
			{
				createDeviceFlags = CreateDeviceFlags.None;
			}
			else if (stBatchArguments.Length == 4)
			{
				createDeviceFlags = CreateDeviceFlags.UseModuleId;
			}
			else if (stBatchArguments.Length == 5)
			{
				try
				{
					createDeviceFlags = (CreateDeviceFlags)uint.Parse(stBatchArguments[4]);
				}
				catch
				{
					throw new BatchWrongArgumentTypeException(new string[0], 4, "uint");
				}
			}
			DeviceIdentification deviceIdentification = ((stBatchArguments.Length != 3 && !(stBatchArguments[3] == "") && (createDeviceFlags & CreateDeviceFlags.UseModuleId) != 0) ? new ModuleIdentification
			{
				ModuleId = stBatchArguments[3]
			} : new DeviceIdentification());
			deviceIdentification.Type = type;
			deviceIdentification.Id = stBatchArguments[1];
			deviceIdentification.Version = stBatchArguments[2];
			if ((createDeviceFlags & (CreateDeviceFlags.HiddenDevice | CreateDeviceFlags.TransientDevice)) == 0)
			{
				return (IObject)(object)new LogicalIODevice(deviceIdentification);
			}
			throw new DeviceObjectException((DeviceObjectExeptionReason)9, "Unknown combination of CreateDeviceFlags");
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject != null)
			{
				if (ReflectionHelper.CheckImplementsInterface((object)parentObject, typeof(ILogicalObject).FullName))
				{
					return true;
				}
				return false;
			}
			return false;
		}

		protected WizardPage GetWizardPage(bool bReload)
		{
			if (_wizardPage == null)
			{
				_wizardPage = new WizardPage();
				_wizardPage.Catalogue.Filter=((IDeviceCatalogueFilter)(object)new LogicalDeviceFilter());
			}
			if (_wizardPage != null && bReload)
			{
				string text = string.Empty;
				if (_wizardPage.NameControl != null)
				{
					text = _wizardPage.NameControl.Text;
				}
				_wizardPage.Catalogue.BeginUpdate();
				_wizardPage.Catalogue.EndUpdate();
				if (!string.IsNullOrEmpty(text))
				{
					_wizardPage.NameControl.Text = text;
				}
			}
			return _wizardPage;
		}
	}
}
