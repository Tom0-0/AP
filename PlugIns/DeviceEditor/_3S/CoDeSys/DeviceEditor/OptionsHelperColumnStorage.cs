using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.DeviceEditor
{
	internal abstract class OptionsHelperColumnStorage
	{
		private static readonly string COLUMN_STORAGEDATA = "DeviceEditorColumnStorageData";

		private static readonly string SUB_KEY = "{4C23F411-95D2-4B2F-916D-F4069AD219F7}";

		public static DeviceEditorColumnStorage StorageData
		{
			get
			{
				try
				{
					if (OptionKey.HasValue(COLUMN_STORAGEDATA, typeof(DeviceEditorColumnStorage)))
					{
						return (DeviceEditorColumnStorage)OptionKey[COLUMN_STORAGEDATA];
					}
				}
				catch
				{
				}
				return null;
			}
			set
			{
				OptionKey[COLUMN_STORAGEDATA]= (object)value;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);
	}
}
