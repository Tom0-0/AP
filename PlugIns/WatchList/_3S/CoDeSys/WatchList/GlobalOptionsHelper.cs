using System;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.WatchList
{
	internal abstract class GlobalOptionsHelper
	{
		private const string DECIMAL_PLACES = "DecimalPlaces";

		internal const int DECIMAL_PLACES_VARIABLE = -1;

		private static readonly string DISPLAY_MODE = "DisplayMode";

		private static readonly string COLUMN_WIDTHS = "ColumnWidths";

		private static readonly string SUB_KEY = "{EFD5A0D8-B437-41b2-87B6-1D745A74AD55}";

		public static readonly int DISPLAYMODE_BINARY = 0;

		public static readonly int DISPLAYMODE_DECIMAL = 1;

		public static readonly int DISPLAYMODE_HEXADECIMAL = 2;

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
				OptionKey[DISPLAY_MODE] = (object)value;
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

		public static void SaveColumnWidths(Guid persistentGuid, int[] columnWidths)
		{
			if (columnWidths == null)
			{
				throw new ArgumentNullException("nColumnWidths");
			}
			OptionKey.CreateSubKey(persistentGuid.ToString())[COLUMN_WIDTHS] = (object)columnWidths;
		}

		public static int[] LoadColumnWidths(Guid persistentGuid)
		{
			if (OptionKey.HasSubKey(persistentGuid.ToString()))
			{
				IOptionKey val = OptionKey.OpenSubKey(persistentGuid.ToString());
				if (val.HasValue(COLUMN_WIDTHS, typeof(int[])))
				{
					return (int[])val[COLUMN_WIDTHS];
				}
			}
			return null;
		}
	}
}
