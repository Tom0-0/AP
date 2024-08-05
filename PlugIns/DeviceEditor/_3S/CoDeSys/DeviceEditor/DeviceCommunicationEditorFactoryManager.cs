using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class DeviceCommunicationEditorFactoryManager : FactoryManagerBase<IDeviceCommunicationEditorFactory, IDeviceObject>
	{
		private static DeviceCommunicationEditorFactoryManager s_instance;

		public static DeviceCommunicationEditorFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new DeviceCommunicationEditorFactoryManager();
				}
				return s_instance;
			}
		}

		protected override int GetMatch(IDeviceCommunicationEditorFactory factory, IDeviceObject obj)
		{
			return factory.GetMatch(obj);
		}

		private DeviceCommunicationEditorFactoryManager()
			: base(APEnvironment.DeviceCommunicationEditorFactories)
		{
		}
	}
}
