using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{16A5DBB8-7A28-40A5-9593-5410DDEA1E2A}")]
    public class OfflineCodeStateStatusField : IIconStatusField, IStatusField, IHasToolTip
    {
        private static readonly Icon s_statusIcon = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(OfflineCodeStateStatusField), "_3S.CoDeSys.OnlineCommands.Resources.OnlineChangeState.ico");

        private static readonly Icon s_statusIcon_DL = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(OfflineCodeStateStatusField), "_3S.CoDeSys.OnlineCommands.Resources.OnlineChangeState_DL.ico");

        private static readonly Icon s_statusIcon_OLC = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(OfflineCodeStateStatusField), "_3S.CoDeSys.OnlineCommands.Resources.OnlineChangeState_OLC.ico");

        private static readonly Icon s_statusIcon_Up2Date = ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(OfflineCodeStateStatusField), "_3S.CoDeSys.OnlineCommands.Resources.OnlineChangeState_Up2date.ico");

        public Icon Icon
        {
            get
            {
                InternalOfflineCodeStateProvider.OnIdleDoUpdate();
                return InternalOfflineCodeStateProvider.InternalCodeState switch
                {
                    OfflineCodeState.NO_CODE_GENERATED => s_statusIcon,
                    OfflineCodeState.CODE_UP_TO_DATE => s_statusIcon_Up2Date,
                    OfflineCodeState.ONLINECHANGE_POSSIBLE => s_statusIcon_OLC,
                    OfflineCodeState.DOWNLOAD_NEEDED => s_statusIcon_DL,
                    _ => null,
                };
            }
        }

        public string ToolTipText => InternalOfflineCodeStateProvider.InternalCodeState switch
        {
            OfflineCodeState.UNKNOWN => string.Empty,
            OfflineCodeState.NO_CODE_GENERATED => Strings.OfflineCodeState_NoCodeGenerated,
            OfflineCodeState.CODE_UP_TO_DATE => Strings.OfflineCodeState_Unchanged,
            OfflineCodeState.ONLINECHANGE_POSSIBLE => Strings.OfflineCodeState_OnlineChangable,
            OfflineCodeState.DOWNLOAD_NEEDED => Strings.OfflineCodeState_DownloadNeeded,
            _ => string.Empty,
        };

        public Color ForeColor => InternalOfflineCodeStateProvider.UnchangedCodeTextColor;

        public Color BackColor => InternalOfflineCodeStateProvider.UnchangedCodeBackgroundColor;

        public int Width => 24;

        public bool Visible
        {
            get
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty && InternalOfflineCodeStateProvider.InternalCodeState != OfflineCodeState.UNKNOWN)
                {
                    return true;
                }
                return false;
            }
        }

        public ICommand DoubleClickCommand => ((IEngine)APEnvironment.Engine).CommandManager.GetCommand(new Guid("{6FF9FFDB-8572-4B73-96F3-9F9404D11BCF}"));
    }
}
