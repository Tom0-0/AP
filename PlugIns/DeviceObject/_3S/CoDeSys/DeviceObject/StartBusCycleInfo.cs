namespace _3S.CoDeSys.DeviceObject
{
	internal class StartBusCycleInfo
	{
		private int _nModuleId;

		private string _stTask;

		private bool _bBeforeReadInputs;

		private bool _bAfterWriteOutputs;

		private bool _bExternEvent;

		public int ModuleId => _nModuleId;

		public string Task => _stTask;

		public bool AfterWriteOutputs => _bAfterWriteOutputs;

		public bool BeforeReadInputs => _bBeforeReadInputs;

		public bool ExternEvent => _bExternEvent;

		public StartBusCycleInfo(int nModuleId, string stTask, bool bBeforeReadInputs, bool bAfterWriteOutputs, bool ExternEvent)
		{
			_nModuleId = nModuleId;
			_stTask = stTask;
			_bBeforeReadInputs = bBeforeReadInputs;
			_bAfterWriteOutputs = bAfterWriteOutputs;
			_bExternEvent = ExternEvent;
		}
	}
}
