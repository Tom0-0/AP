using System;

namespace _3S.CoDeSys.DeviceObject
{
    internal class DeviceInfo
    {
        private Guid deviceGuid;

        private int nProjectHandle;

        public int ProjectHandle
        {
            get
            {
                return nProjectHandle;
            }
            set
            {
                nProjectHandle = value;
            }
        }

        internal DeviceInfo(Guid deviceGuid)
        {
            this.deviceGuid = deviceGuid;
        }
    }
}
