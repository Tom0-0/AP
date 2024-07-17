using _3S.CoDeSys.OnlineUI;
using System;
using System.Collections.Generic;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class MultipleDownloadContext : IMultipleDownloadContext
    {
        private List<Guid> _applications;

        private List<IMultipleDownloadExtension> _extensions;

        private bool _bInitPersistentVars;

        private bool _bDeleteOldApps;

        private bool _bStartAllApps;

        private bool _bDoNotReleaseForcedVariables;

        private bool _bThrowIfAborted;

        private bool _bOmitResultDialog;

        private OnlineChangeOption _onlineChange;

        public Guid[] Applications => _applications.ToArray();

        public IMultipleDownloadExtension[] Extensions => _extensions.ToArray();

        public bool InitPersistentVars => _bInitPersistentVars;

        public bool DeleteOldApps => _bDeleteOldApps;

        public bool StartAllApps => _bStartAllApps;

        public bool ThrowIfAborted => _bThrowIfAborted;

        public bool OmitResultDialog => _bOmitResultDialog;

        public OnlineChangeOption OnlineChange => _onlineChange;

        internal MultipleDownloadContext(List<Guid> applications, List<IMultipleDownloadExtension> extensions, bool bInitPersistentVars, bool bDeleteOldApps, bool bStartAllApps, bool bDoNotReleaseForcedVariables, bool bThrowIfAborted, bool bOmitResultDialog, OnlineChangeOption onlineChange)
        {
            //IL_0044: Unknown result type (might be due to invalid IL or missing references)
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            _applications = applications;
            _extensions = extensions;
            _bInitPersistentVars = bInitPersistentVars;
            _bDeleteOldApps = bDeleteOldApps;
            _bStartAllApps = bStartAllApps;
            _bDoNotReleaseForcedVariables = bDoNotReleaseForcedVariables;
            _bThrowIfAborted = bThrowIfAborted;
            _bOmitResultDialog = bOmitResultDialog;
            _onlineChange = onlineChange;
        }
    }
}
