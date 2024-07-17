using _3S.CoDeSys.Core.Messages;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    public class DownloadMessage : IMessage
    {
        private string _stMessage;

        public short Length => 0;

        public long Position => -1L;

        public Severity Severity => (Severity)1;

        public Guid ObjectGuid => Guid.Empty;

        public short PositionOffset => 0;

        public string Text => _stMessage;

        public int ProjectHandle => -1;

        public DownloadMessage(string stMessage)
        {
            _stMessage = stMessage;
        }
    }
}
