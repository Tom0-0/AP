using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
    public class DeviceParameterFactoryManager : FactoryManagerBase<IDiffViewerDeviceParameterFilterFactory, IMetaObject>
    {
        private static DeviceParameterFactoryManager s_instance;

        public static DeviceParameterFactoryManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new DeviceParameterFactoryManager();
                }
                return s_instance;
            }
        }

        public IDiffViewerDeviceParameterFilterFactory GetFactory(IMetaObject mo)
        {
            return base.GetFactory((IMetaObject[])(object)new IMetaObject[1] { mo });
        }

        protected override int GetMatch(IDiffViewerDeviceParameterFilterFactory factory, IMetaObject[] objects)
        {
            return factory.GetMatch(objects[0]);
        }

        private DeviceParameterFactoryManager()
            : base(APEnvironment.CreateDiffViewerDeviceParameterFilterFactories())
        {
        }
    }
}
