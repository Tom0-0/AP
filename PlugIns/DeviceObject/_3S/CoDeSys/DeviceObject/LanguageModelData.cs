using System;

namespace _3S.CoDeSys.DeviceObject
{
    internal class LanguageModelData
    {
        private int _nProjectHandle;

        private bool _bIsTaskLanguageModel;

        private Guid _objectGuid;

        private bool _bIsPlc;

        internal int ProjectHandle => _nProjectHandle;

        internal Guid ObjectGuid => _objectGuid;

        internal bool IsTaskLanguageModel => _bIsTaskLanguageModel;

        internal bool IsPlc => _bIsPlc;

        internal LanguageModelData(int nProjectHandle, Guid ObjectGuid, bool bIsTaskLanguageModel, bool bIsPlc)
        {
            _nProjectHandle = nProjectHandle;
            _bIsTaskLanguageModel = bIsTaskLanguageModel;
            _objectGuid = ObjectGuid;
            _bIsPlc = bIsPlc;
        }
    }
}
