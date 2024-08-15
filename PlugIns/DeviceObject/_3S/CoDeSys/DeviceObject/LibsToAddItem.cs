namespace _3S.CoDeSys.DeviceObject
{
	internal class LibsToAddItem
	{
		public string stLibName = string.Empty;

		public string stLibVersion = string.Empty;

		public string stLibVendor = string.Empty;

		public string stPlaceholder = string.Empty;

		public bool bLoadAsSystemLibrary;

		public bool bLoadAsPlaceHolder;

		public bool bInsertLibrary;

		public IIoProvider ioProvider;

		public IRequiredLib requiredLib;
	}
}
