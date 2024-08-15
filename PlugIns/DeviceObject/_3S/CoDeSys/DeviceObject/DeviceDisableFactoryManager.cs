using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceDisableFactoryManager : FactoryManagerBase<IHideDeviceDisableFactory, IMetaObject>
	{
		private static DeviceDisableFactoryManager s_instance;

		public static DeviceDisableFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new DeviceDisableFactoryManager();
				}
				return s_instance;
			}
		}

		public IHideDeviceDisableFactory GetFactory(IMetaObject mo)
		{
			return base.GetFactory((IMetaObject[])(object)new IMetaObject[1] { mo });
		}

		protected override int GetMatch(IHideDeviceDisableFactory factory, IMetaObject[] objects)
		{
			return factory.GetMatch(objects[0]);
		}

		private DeviceDisableFactoryManager()
			: base(APEnvironment.CreateHideDeviceDisableFactories())
		{
		}
	}
}
