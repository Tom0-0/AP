using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.FdtIntegration;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{02867743-ec58-47a2-ae84-4eeaa6617bb0}")]
	public class DeviceEditorFactory : IModelessEditorViewFactory, IEditorViewFactory, IHasDynamicIcons
	{
		internal static string stDeviceSection = "DeviceEditor";

		internal static string stAdditionalContextMenue = "AdditionalContextMenu";

		internal static string stHideGenericEditorButtons = "HideGenericEditorButtons";

		internal static string stUseClassicDeviceEditor = "UseClassicDeviceEditor";

		internal static string stDefaultColumnForInputsEditable = "DefaultColumnForInputsEditable";

		private static Icon _icon = null;

		private static Icon _smallIcon = null;

		private static Icon _largeIcon = null;

		private static Icon _safetyIcon = null;

		private static Icon _smallSafetyIcon = null;

		private static Icon _largeSafetyIcon = null;

		public string Name => "Device Editor";

		public string Description => "";

		public Icon SmallIcon
		{
			get
			{
				if (_icon == null)
				{
					LoadIcons();
				}
				return _smallIcon;
			}
		}

		public Icon LargeIcon
		{
			get
			{
				if (_icon == null)
				{
					LoadIcons();
				}
				return _largeIcon;
			}
		}

		public IEditorView Create()
		{
			return (IEditorView)(object)new DeviceEditor();
		}

		public bool AcceptsObjectType(Type objectType, Type[] embeddedObjectTypes)
		{
			if (!typeof(IDeviceObject).IsAssignableFrom(objectType))
			{
				return typeof(IExplicitConnector).IsAssignableFrom(objectType);
			}
			return true;
		}

		internal static object GetCustomizationValue(string stKey)
		{
			try
			{
				if (APEnvironment.Engine.OEMCustomization.HasValue(stDeviceSection, stKey))
				{
					return APEnvironment.Engine.OEMCustomization.GetValue(stDeviceSection, stKey);
				}
			}
			catch
			{
			}
			return null;
		}

		public static Icon GetSmallIcon()
		{
			if (_icon == null)
			{
				LoadIcons();
			}
			return _smallIcon;
		}

		public static Icon GetLargeIcon()
		{
			if (_icon == null)
			{
				LoadIcons();
			}
			return _largeIcon;
		}

		private static void LoadIcons()
		{
			_icon = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceEditorFactory), "_3S.CoDeSys.DeviceEditor.Resources.DeviceIcon.ico");
			_smallIcon = new Icon(_icon, new Size(16, 16));
			_largeIcon = new Icon(_icon, new Size(32, 32));
		}

		public static Icon GetSmallSafetyIcon()
		{
			if (_safetyIcon == null)
			{
				LoadSafetyIcons();
			}
			return _smallSafetyIcon;
		}

		public static Icon GetLargeSafetyIcon()
		{
			if (_safetyIcon == null)
			{
				LoadSafetyIcons();
			}
			return _largeSafetyIcon;
		}

		private static void LoadSafetyIcons()
		{
			_safetyIcon = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(DeviceEditorFactory), "_3S.CoDeSys.DeviceEditor.Resources.Safety_Small.ico");
			_smallSafetyIcon = new Icon(_safetyIcon, new Size(16, 16));
			_largeSafetyIcon = new Icon(_safetyIcon, new Size(32, 32));
		}

		Icon IHasDynamicIcons.GetSmallIcon(IMetaObjectStub mos)
		{
			if (((IObjectManager2)APEnvironment.ObjectMgr).IsObjectLoaded(mos.ProjectHandle, mos.ObjectGuid))
			{
				return GetSmallIcon(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid));
			}
			return GetSmallIcon();
		}

		internal static Icon GetSmallIcon(IMetaObject meta)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			try
			{
				if (meta.Object is IDeviceObject)
				{
					IDeviceObject val = (IDeviceObject)meta.Object;
					if (((IObject)val).Namespace != Guid.Empty)
					{
						if (APEnvironment.DeviceRepository.GetDevice(val.DeviceIdentification) == null)
						{
							return Strings.Unknown;
						}
						IFdtIntegration fdtIntegrationOrNull = APEnvironment.FdtIntegrationOrNull;
						if (fdtIntegrationOrNull != null && fdtIntegrationOrNull.IsRequiredDtmMissing(meta.ProjectHandle, val))
						{
							return Strings.Unknown;
						}
					}
					Icon icon = val.DeviceInfo.Icon;
					if (icon != null)
					{
						if (icon.Height == 16 && icon.Width == 16)
						{
							return icon;
						}
						return new Icon(icon, 16, 16);
					}
					int[] categories = val.DeviceInfo.Categories;
					for (int i = 0; i < categories.Length; i++)
					{
						if (categories[i] == 4098)
						{
							return GetSmallSafetyIcon();
						}
					}
				}
				else if (meta.Object is IExplicitConnector)
				{
					return Strings.MasterConnectorIconSmall;
				}
			}
			catch
			{
			}
			return GetSmallIcon();
		}

		Icon IHasDynamicIcons.GetLargeIcon(IMetaObjectStub mos)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType))
				{
					Icon icon = ((IDeviceObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid).Object).DeviceInfo.Icon;
					if (icon != null)
					{
						return new Icon(icon, 32, 32);
					}
				}
				else if (typeof(IExplicitConnector).IsAssignableFrom(mos.ObjectType))
				{
					return Strings.MasterConnectorIconSmall;
				}
			}
			catch
			{
			}
			return GetLargeIcon();
		}
	}
}
