using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceEditor
{
	internal abstract class GlobalOptionsHelper
	{
		private static IConverterToIEC s_binaryConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)0);

		private static IConverterToIEC s_decimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)1);

		private static IConverterToIEC s_hexadecimalConverter = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)2);

		private static readonly string DISPLAY_MODE = "DisplayMode";

		public static readonly string SUB_KEY = "{EFD5A0D8-B437-41b2-87B6-1D745A74AD55}";

		public static readonly int DISPLAYMODE_BINARY = 0;

		public static readonly int DISPLAYMODE_DECIMAL = 1;

		public static readonly int DISPLAYMODE_HEXADECIMAL = 2;

		private const string DECIMAL_PLACES = "DecimalPlaces";

		internal const int DECIMAL_PLACES_VARIABLE = -1;

		public static int DisplayMode
		{
			get
			{
				if (OptionKey.HasValue(DISPLAY_MODE, typeof(int)))
				{
					return (int)OptionKey[DISPLAY_MODE];
				}
				return 1;
			}
			set
			{
				OptionKey[DISPLAY_MODE]= (object)value;
			}
		}

		internal static int DecimalPlaces
		{
			get
			{
				if (OptionKey.HasValue("DecimalPlaces", typeof(int)))
				{
					return (int)OptionKey["DecimalPlaces"];
				}
				return -1;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);

		public static IConverterToIEC GetCurrentConverterToIEC()
		{
			int displayMode = DisplayMode;
			if (displayMode == DISPLAYMODE_BINARY)
			{
				return s_binaryConverter;
			}
			if (displayMode == DISPLAYMODE_HEXADECIMAL)
			{
				return s_hexadecimalConverter;
			}
			return s_decimalConverter;
		}
	}
}
