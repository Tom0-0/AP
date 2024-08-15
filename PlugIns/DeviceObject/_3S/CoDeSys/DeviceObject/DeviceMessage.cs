using _3S.CoDeSys.Core.Messages;
using System;

namespace _3S.CoDeSys.DeviceObject
{
    public class DeviceMessage : IMessage
    {
        private string _stMessage;

        private Severity _severity;

        private Guid _ObjectGuid = Guid.Empty;

        private long _lPosition = -1L;

        public int ProjectHandle => -1;

        public Guid ObjectGuid => _ObjectGuid;

        public long Position => _lPosition;

        public short PositionOffset => 0;

        public short Length => 0;

        public string Text => _stMessage;

        public Severity Severity => _severity;

        public DeviceMessage(string stMessage, Severity severity)
        {
            //IL_0021: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            _stMessage = stMessage;
            _severity = severity;
        }

        public DeviceMessage(string stMessage, Severity severity, Guid objectGuid, long lPosition)
        {
            //IL_0021: Unknown result type (might be due to invalid IL or missing references)
            //IL_0022: Unknown result type (might be due to invalid IL or missing references)
            _stMessage = stMessage;
            _severity = severity;
            _ObjectGuid = objectGuid;
            _lPosition = lPosition;
        }
    }
}
