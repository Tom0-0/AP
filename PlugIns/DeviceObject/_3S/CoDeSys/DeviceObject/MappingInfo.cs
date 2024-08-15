namespace _3S.CoDeSys.DeviceObject
{
	internal class MappingInfo : ITaskMappingInfo
	{
		private IIoProvider _ioProvider;

		private int _iModuleIndex;

		public IIoProvider IoProvider => _ioProvider;

		public int ModuleIndex => _iModuleIndex;

		public MappingInfo(IIoProvider ioProvider, int iModuleIndex)
		{
			_ioProvider = ioProvider;
			_iModuleIndex = iModuleIndex;
		}
	}
}
