using System;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceObjectDiffViewerFactoryManager : FactoryManagerBase<IDeviceObjectEmbeddedDiffViewerFactory, Tuple<int, Guid>>
	{
		private static DeviceObjectDiffViewerFactoryManager s_instance;

		public static DeviceObjectDiffViewerFactoryManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new DeviceObjectDiffViewerFactoryManager();
				}
				return s_instance;
			}
		}

		public IDeviceObjectEmbeddedDiffViewerFactory GetFactory(int nLeftProjectHandle, Guid leftObjectGuid, int nRightProjectHandle, Guid rightObjectGuid)
		{
			return GetFactory(new Tuple<int, Guid>[2]
			{
				new Tuple<int, Guid>(nLeftProjectHandle, leftObjectGuid),
				new Tuple<int, Guid>(nRightProjectHandle, rightObjectGuid)
			});
		}

		protected override int GetMatch(IDeviceObjectEmbeddedDiffViewerFactory factory, Tuple<int, Guid>[] objects)
		{
			return factory.GetMatch(objects[0].Item1, objects[0].Item2, objects[1].Item1, objects[1].Item2);
		}

		private DeviceObjectDiffViewerFactoryManager()
			: base(APEnvironment.CreateDeviceObjectEmbeddedDiffViewerFactories())
		{
		}
	}
}
