using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{4da2c5bd-5291-436b-b8b0-6a4480112db7}")]
	[StorageVersion("3.3.0.0")]
	public enum InstanceLocation
	{
		GVL,
		Retain,
		Input,
		Output
	}
}
