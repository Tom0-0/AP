using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.OnlineCommands
{
    internal abstract class OptionsHelper
    {
        private const string DECIMAL_PLACES = "DecimalPlaces";

        internal const int DECIMAL_PLACES_VARIABLE = -1;

        private static readonly string DISPLAY_MODE = "DisplayMode";

        private static readonly string ONLINE_VIEW_INFO_TABLE = "OnlineViewInfoTable";

        private static readonly string SUB_KEY = "{EFD5A0D8-B437-41b2-87B6-1D745A74AD55}";

        public static readonly int DISPLAYMODE_BINARY = 0;

        public static readonly int DISPLAYMODE_DECIMAL = 1;

        public static readonly int DISPLAYMODE_HEXADECIMAL = 2;

        internal static readonly string SUB_KEY_BP = "{BD190F99-8911-498E-B67E-01CE13F46FDA}";

        private static readonly string RESTORE_BPS_AFTER_RESET = "RestoreBreakpointsAfterReset";

        private static IOptionKey OptionKeyBP => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY_BP);

        public static bool RestoreBreakpointsAfterReset
        {
            get
            {
                if (OptionKeyBP.HasValue(RESTORE_BPS_AFTER_RESET, typeof(bool)))
                {
                    return (bool)OptionKeyBP[RESTORE_BPS_AFTER_RESET];
                }
                return true;
            }
        }

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
            set
            {
                OptionKey["DecimalPlaces"] = (object)value;
            }
        }

        public static OnlineViewInfoTable OnlineViewInfoTable
        {
            get
            {
                OnlineViewInfoTable onlineViewInfoTable = null;
                if (OptionKey.HasValue(ONLINE_VIEW_INFO_TABLE, typeof(OnlineViewInfoTable)))
                {
                    onlineViewInfoTable = (OnlineViewInfoTable)OptionKey[ONLINE_VIEW_INFO_TABLE];
                }
                if (onlineViewInfoTable == null)
                {
                    onlineViewInfoTable = new OnlineViewInfoTable();
                }
                return onlineViewInfoTable;
            }
            set
            {
                OptionKey[ONLINE_VIEW_INFO_TABLE] = (object)value;
            }
        }

        public static bool UseHmiPerspective
        {
            get
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
                {
                    IOptionKey val = APEnvironment.OptionStorage.GetRootKey((OptionRoot)0).CreateSubKey("{01976BC1-BA98-4A4D-98A8-EADC3C949CF8}}");
                    if (val.HasValue("UseHmiPerspective", typeof(bool)))
                    {
                        return (bool)val["UseHmiPerspective"];
                    }
                    return false;
                }
                IOptionKey val2 = APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey("{01976BC1-BA98-4A4D-98A8-EADC3C949CF8}}");
                if (val2.HasValue("UseHmiPerspective", typeof(bool)))
                {
                    return (bool)val2["UseHmiPerspective"];
                }
                return false;
            }
        }

        private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);
    }
}
