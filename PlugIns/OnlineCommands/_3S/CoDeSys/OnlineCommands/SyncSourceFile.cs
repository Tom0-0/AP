using _3S.CoDeSys.Core.Online;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    public class SyncSourceFile : ISyncOnlineFile2, ISyncOnlineFile
    {
        private ISyncOnlineFileProvider _provider;

        private Guid _guidDeviceObject = Guid.Empty;

        private Version _version;

        public ISyncOnlineFileProvider Provider => _provider;

        public string Name => "Archive.prj";

        public string RtsFileName => "Archive.prj";

        public string Description => Strings.SourceDownload_SyncFileDescription;

        public string RtsFilePath => string.Empty;

        public string HostFilePath => string.Empty;

        public Guid DeviceObject => _guidDeviceObject;

        public Guid Application => Guid.Empty;

        public bool IsDynamicStream => true;

        public SyncOnlineFileTiming Timing => (SyncOnlineFileTiming)1;

        public object Tag => null;

        public bool UseLegacyHostPath => _version < SourceTransferHelper.RTS_VERSION_3580;

        public string LegacyHostPath => string.Empty;

        public bool AlreadyUpToDate
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public SyncSourceFile(ISyncOnlineFileProvider provider, Guid guidDeviceObject, Version version)
        {
            _provider = provider;
            _guidDeviceObject = guidDeviceObject;
            _version = version;
        }
    }
}
