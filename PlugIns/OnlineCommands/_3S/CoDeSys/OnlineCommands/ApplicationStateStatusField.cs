using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Views;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{0D56E415-C96C-4c71-AFFA-D1CC10779A87}")]
    public class ApplicationStateStatusField : ITextStatusField, IStatusField
    {
        private ApplicationState ApplicationState
        {
            get
            {
                //IL_0054: Unknown result type (might be due to invalid IL or missing references)
                if (!(OnlineCommandHelper.ActiveAppObjectGuid != Guid.Empty) || !((IObjectManager2)APEnvironment.ObjectMgr).IsObjectLoaded(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, OnlineCommandHelper.ActiveAppObjectGuid))
                {
                    return (ApplicationState)0;
                }
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(OnlineCommandHelper.ActiveAppObjectGuid);
                if (application == null || !application.IsLoggedIn)
                {
                    return (ApplicationState)0;
                }
                return application.ApplicationState;
            }
        }

        public string Text
        {
            get
            {

                ApplicationState applicationState = ApplicationState;
                switch ((int)applicationState)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), applicationState.ToString());
                    default:
                        return string.Empty;
                }
            }
        }

        public int Width => 96;

        public ICommand DoubleClickCommand => null;

        public Color BackColor
        {
            get
            {
                //IL_0001: Unknown result type (might be due to invalid IL or missing references)
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_0007: Unknown result type (might be due to invalid IL or missing references)
                //IL_0025: Expected I4, but got Unknown
                ApplicationState applicationState = ApplicationState;
                switch ((int)applicationState)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return Color.Red;
                    case 1:
                        return Color.LightGreen;
                    default:
                        return Color.Transparent;
                }
            }
        }

        public Color ForeColor => Color.Black;

        public bool Visible => (int)ApplicationState > 0;
    }
}
