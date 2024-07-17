using _3S.CoDeSys.Core.Online;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    public class CannotRemoveDeviceOrApplicationWhichIsOnlineException : OnlineManagerException
    {
        private int _nProjectHandle;

        private Guid _objectGuidToRemove;

        private Guid _objectGuidOnline;

        public int ProjectHandle => _nProjectHandle;

        public Guid ObjectGuidToRemove => _objectGuidToRemove;

        public Guid ObjectGuidOnline => _objectGuidOnline;

        public CannotRemoveDeviceOrApplicationWhichIsOnlineException(string stMessage, int nProjectHandle, Guid objectGuidToRemove, Guid objectGuidOnline)
            : base(stMessage)
        {
            _nProjectHandle = nProjectHandle;
            _objectGuidToRemove = objectGuidToRemove;
            _objectGuidOnline = objectGuidOnline;
        }
    }
}
