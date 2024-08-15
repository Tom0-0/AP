using _3S.CoDeSys.Core.Components;

namespace _3S.CoDeSys.DeviceObject
{
	public abstract class DeviceObjectManager : ISystemInstanceRequiresInitialization
	{
		public void OnAllSystemInstancesAvailable()
		{
			RegisterObjectEvents();
		}

		protected abstract void RegisterObjectEvents();
	}
}
