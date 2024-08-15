using System;

namespace _3S.CoDeSys.DeviceObject
{
    internal class EmptyDriverInfo : IDriverInfo
    {
        private static EmptyRequiredLibsList s_emptyRequiredLibs = new EmptyRequiredLibsList();

        public bool NeedsBusCycle
        {
            get
            {
                return false;
            }
            set
            {
                throw new InvalidOperationException("EmptyDriverInfo is readonly");
            }
        }

        public string BusCycleTask
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("EmptyDriverInfo is readonly");
            }
        }

        public IRequiredLibsList RequiredLibs => (IRequiredLibsList)(object)s_emptyRequiredLibs;
    }
}
