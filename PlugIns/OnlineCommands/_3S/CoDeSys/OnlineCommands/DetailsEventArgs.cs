using _3S.CoDeSys.Core.Online;
using System;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class DetailsEventArgs : EventArgs
    {
        internal readonly bool ApplicationExistsOnDevice;

        internal readonly IOnlineApplication OnlineApplication;

        internal DetailsEventArgs(bool bApplicationExistsOnDevice, IOnlineApplication onlineApplication)
        {
            ApplicationExistsOnDevice = bApplicationExistsOnDevice;
            OnlineApplication = onlineApplication;
        }
    }
}
