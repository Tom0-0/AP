using System;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.DeviceCommunicationEditor;

namespace _3S.CoDeSys.DeviceEditor
{
	internal abstract class OptionsHelper
	{
		public static readonly string SHOW_GENERIC_CONFIGURATION = "ShowGenericConfiguration";

		public static readonly string CREATE_CROSS_REFERENCES = "CreateCrossReferences";

		private static readonly string USE_CLASSIC_COM_PAGE = "UseClassicComPageMode";

		private static readonly string COM_PAGE_MODE = "ComPageMode";

		private static readonly string SHOW_ALL_SYNC_FILES = "ShowAllSyncFiles";

		private static readonly string SHOW_ACCESS_RIGHTS_PAGE = "ShowAccessRightsPage";

		private static readonly string USE_HORIZONTAL_TAB_PAGES = "UseClassicDeviceEditor";

		public static readonly string SUB_KEY = "{5F370A46-A40D-41dd-9B9E-8094E2158F4C}";

		public static bool ShowGenericConfiguration
		{
			get
			{
				if (IsOemValueAvailable("DeviceEditor", "ShowGenericEditor", out var value) && value is bool)
				{
					return (bool)value;
				}
				if (OptionKey.HasValue(SHOW_GENERIC_CONFIGURATION, typeof(bool)))
				{
					return (bool)OptionKey[SHOW_GENERIC_CONFIGURATION];
				}
				return false;
			}
			set
			{
				OptionKey[SHOW_GENERIC_CONFIGURATION]= (object)value;
			}
		}

		public static bool CreateCrossReferences
		{
			get
			{
				if (OptionKey.HasValue(CREATE_CROSS_REFERENCES, typeof(bool)))
				{
					return (bool)OptionKey[CREATE_CROSS_REFERENCES];
				}
				return false;
			}
			set
			{
				OptionKey[CREATE_CROSS_REFERENCES]= (object)value;
			}
		}

		public static bool UseClassicComPage
		{
			get
			{
				if (OptionKey.HasValue(USE_CLASSIC_COM_PAGE, typeof(bool)))
				{
					return (bool)OptionKey[USE_CLASSIC_COM_PAGE];
				}
				bool result = false;
				object value = null;
				if (IsOemValueAvailable("DeviceEditor", "UseClassicComPage", out value) && value is bool)
				{
					result = (bool)value;
				}
				return result;
			}
			set
			{
				OptionKey[USE_CLASSIC_COM_PAGE]= (object)value;
			}
		}

		public static AvailableComPageModez ComPageMode
		{
			get
			{
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				if (OptionKey.HasValue(COM_PAGE_MODE, typeof(int)))
				{
					int num = (int)OptionKey[COM_PAGE_MODE];
					if (num >= 0 && num < Enum.GetValues(typeof(AvailableComPageModez)).Length)
					{
						if (APEnvironment.ComPageCustomizerOrNull == null && num == 2)
						{
							ComPageMode = (AvailableComPageModez)0;
							return (AvailableComPageModez)0;
						}
						return (AvailableComPageModez)num;
					}
				}
				object value = null;
				if (IsOemValueAvailable("DeviceEditor", "UseClassicComPage", out value) && value is bool && (bool)value)
				{
					return (AvailableComPageModez)1;
				}
				if (APEnvironment.ComPageCustomizerOrNull != null && !string.IsNullOrEmpty(APEnvironment.ComPageCustomizerOrNull.ComPageName))
				{
					ComPageMode = APEnvironment.ComPageCustomizerOrNull.DefaultMode;
					return APEnvironment.ComPageCustomizerOrNull.DefaultMode;
				}
				if (!UseClassicComPage)
				{
					return (AvailableComPageModez)0;
				}
				return (AvailableComPageModez)1;
			}
			set
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Expected I4, but got Unknown
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Invalid comparison between Unknown and I4
				OptionKey[COM_PAGE_MODE]= (object)(int)value;
				if ((int)value == 1)
				{
					UseClassicComPage = true;
				}
			}
		}

		public static bool UseHorizontalTabPages
		{
			get
			{
				bool result = false;
				object value = null;
				if (IsOemValueAvailable("DeviceEditor", "UseClassicDeviceEditor", out value) && value is bool)
				{
					result = (bool)value;
				}
				else if (OptionKey.HasValue(USE_HORIZONTAL_TAB_PAGES, typeof(bool)))
				{
					result = (bool)OptionKey[USE_HORIZONTAL_TAB_PAGES];
				}
				return result;
			}
			set
			{
				OptionKey[USE_HORIZONTAL_TAB_PAGES]= (object)value;
			}
		}

		public static bool OEMUseClassicDeviceEditorAvailable
		{
			get
			{
				object value = null;
				return IsOemValueAvailable("DeviceEditor", "UseClassicDeviceEditor", out value);
			}
		}

		public static bool ShowAllSyncFiles
		{
			get
			{
				if (OptionKey.HasValue(SHOW_ALL_SYNC_FILES, typeof(bool)))
				{
					return (bool)OptionKey[SHOW_ALL_SYNC_FILES];
				}
				return false;
			}
			set
			{
				OptionKey[SHOW_ALL_SYNC_FILES]= (object)value;
			}
		}

		public static bool ShowAccessRightsPage
		{
			get
			{
				if (OptionKey.HasValue(SHOW_ACCESS_RIGHTS_PAGE, typeof(bool)))
				{
					return (bool)OptionKey[SHOW_ACCESS_RIGHTS_PAGE];
				}
				return false;
			}
			set
			{
				OptionKey[SHOW_ACCESS_RIGHTS_PAGE]= (object)value;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY);

		internal static bool IsOemValueAvailable(string stSection, string stKey, out object value)
		{
			value = null;
			if (APEnvironment.Engine.OEMCustomization.HasValue(stSection, stKey))
			{
				value = APEnvironment.Engine.OEMCustomization.GetValue(stSection, stKey);
				return value != null;
			}
			return false;
		}
	}
}
