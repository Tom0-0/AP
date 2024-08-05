using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class DeviceEditorFactoryManager : FactoryManagerBase<IDeviceEditorFactory, IDeviceObject>
	{
		private static DeviceEditorFactoryManager s_instance;

		public static DeviceEditorFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new DeviceEditorFactoryManager();
				}
				return s_instance;
			}
		}

		protected override int GetMatch(IDeviceEditorFactory factory, IDeviceObject obj)
		{
			return factory.GetMatch(obj);
		}

		private DeviceEditorFactoryManager()
			: base(APEnvironment.DeviceEditorFactories)
		{
		}
	}
}
