using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceObjectCompareFactoryManager : FactoryManagerBase<IDeviceObjectComparerFactory, IObject>
	{
		private static DeviceObjectCompareFactoryManager s_instance;

		public static DeviceObjectCompareFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new DeviceObjectCompareFactoryManager();
				}
				return s_instance;
			}
		}

		public IDeviceObjectComparerFactory GetFactory(IObject leftObject, IObject rightObject)
		{
			return GetFactory((IObject[])(object)new IObject[2] { leftObject, rightObject });
		}

		protected override int GetMatch(IDeviceObjectComparerFactory factory, IObject[] objects)
		{
			return factory.GetMatch(objects[0], objects[1]);
		}

		private DeviceObjectCompareFactoryManager()
			: base(APEnvironment.CreateDeviceObjectComparerFactories())
		{
		}
	}
}
