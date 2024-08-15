using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
    public class DeviceLanguageModelEventArgsImpl : DeviceLanguageModelEventArgs
    {
        public ArrayList LmContributions => _alLanguageModels;

        public DeviceLanguageModelEventArgsImpl(int nProjectHandle, Guid guidDeviceObject, Guid guidPlcLogic, Guid guidApp)
            : base(nProjectHandle, guidDeviceObject, guidPlcLogic, guidApp)
        {
        }
    }
}
