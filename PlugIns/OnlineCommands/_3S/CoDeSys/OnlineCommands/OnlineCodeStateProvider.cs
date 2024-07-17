using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Utilities;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{19CF7C55-FD84-4AC2-B26D-38716156A138}")]
    public class OnlineCodeStateProvider : IOnlineCodeStateProvider2, IOnlineCodeStateProvider
    {
        public OnlineCodeState CodeState => InternalCodeStateProvider.InternalCodeState;

        public event OnlineCodeStateChangedHandler OnlineCodeStateChanged;

        public event LoginWithOutdatedCodeHandler LoginWithOutdatedCodeDetected;

        public OnlineCodeStateProvider()
        {
            //IL_000d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0017: Expected O, but got Unknown
            //IL_001e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0028: Expected O, but got Unknown
            InternalCodeStateProvider.InternalCodeStateChanged += new OnlineCodeStateChangedHandler(OnInternalCodeStateChanged);
            InternalCodeStateProvider.InternalLoginWithOutdatedCodeDetected += new LoginWithOutdatedCodeHandler(OnInternalLoginWithOutdatedCodeDetected);
        }

        private void OnInternalLoginWithOutdatedCodeDetected(object sender, EventArgs e)
        {
            if ((object)this.LoginWithOutdatedCodeDetected != null)
            {
                this.LoginWithOutdatedCodeDetected.Invoke((object)this, e);
            }
        }

        private void OnInternalCodeStateChanged(object sender, OnlineCodeStateEventArgs e)
        {
            if ((object)this.OnlineCodeStateChanged != null)
            {
                this.OnlineCodeStateChanged.Invoke((object)this, e);
            }
        }

        public void SetCodeStatusFieldColors(Color changedCodeText, Color changedCodeBackground, Color unchangedCodeText, Color unchangedCodeBackground)
        {
            InternalCodeStateProvider.ChangedCodeBackgroundColor = changedCodeBackground;
            InternalCodeStateProvider.ChangedCodeTextColor = changedCodeText;
            InternalCodeStateProvider.UnchangedCodeBackgroundColor = unchangedCodeBackground;
            InternalCodeStateProvider.UnchangedCodeTextColor = unchangedCodeText;
        }

        public DateTime GetLastModificationTime()
        {
            DateTime result = InternalCodeStateProvider.ORIGIN;
            try
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
                {
                    result = AuthFile.GetLastWriteTime(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path);
                    if (InternalCodeStateProvider.LastModificationTime != InternalCodeStateProvider.ORIGIN)
                    {
                        result = InternalCodeStateProvider.LastModificationTime;
                        return result;
                    }
                    return result;
                }
                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
