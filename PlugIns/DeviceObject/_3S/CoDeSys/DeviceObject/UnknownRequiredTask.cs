using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{B1141746-8805-4DDC-8DE5-C9828F7B3B59}")]
	[StorageVersion("3.3.0.0")]
	public class UnknownRequiredTask : RequiredTask
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		private string _stTypeOfTask;

		internal override string TypeOfTask => _stTypeOfTask;

		public UnknownRequiredTask()
		{
		}

		public UnknownRequiredTask(string stTypeOfTask)
		{
			_stTypeOfTask = stTypeOfTask;
		}
	}
}
