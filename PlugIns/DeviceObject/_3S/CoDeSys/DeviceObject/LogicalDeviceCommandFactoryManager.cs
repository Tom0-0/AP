using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class LogicalDeviceCommandFactoryManager : FactoryManagerBase<ILogicalDeviceCommandFactory, IMetaObject>
	{
		private static LogicalDeviceCommandFactoryManager s_instance;

		public static LogicalDeviceCommandFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new LogicalDeviceCommandFactoryManager();
				}
				return s_instance;
			}
		}

		public ILogicalDeviceCommandFactory GetFactory(IMetaObject mo)
		{
			return base.GetFactory((IMetaObject[])(object)new IMetaObject[1] { mo });
		}

		protected override int GetMatch(ILogicalDeviceCommandFactory factory, IMetaObject[] objects)
		{
			return factory.GetMatch(objects[0]);
		}

		private LogicalDeviceCommandFactoryManager()
			: base(APEnvironment.CreateLogicalDeviceCommandFactory())
		{
		}
	}
}
