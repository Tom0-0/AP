using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.WatchList;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    public abstract class AbstractDisplayModeCommand : IToggleCommand, ICommand
    {
        private int _displayMode;

        private string[] _batchCommand;

        public string[] BatchCommand => _batchCommand;

        public Guid Category => OnlineCommandCategory.Guid;

        public bool Checked => OptionsHelper.DisplayMode == _displayMode;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Description");

        public bool Enabled => true;

        public Icon LargeIcon => SmallIcon;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Name");

        public bool RadioCheck => true;

        public Icon SmallIcon => null;

        public string ToolTipText => Name;

        protected AbstractDisplayModeCommand(int displayMode, string @short)
        {
            _displayMode = displayMode;
            _batchCommand = new string[3] { "online", "display", @short };
        }

        public void AddedToUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            return new string[0];
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001c: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length != 0)
            {
                throw new BatchTooManyArgumentsException(_batchCommand, arguments.Length, 0);
            }
            OptionsHelper.DisplayMode = _displayMode;
        }

        public bool IsVisible(bool bContextMenu)
        {
            //IL_0036: Unknown result type (might be due to invalid IL or missing references)
            if (!bContextMenu)
            {
                return true;
            }
            if (((IEngine)APEnvironment.Engine).Frame.ActiveView is IWatchListView)
            {
                return true;
            }
            IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
            IHasOnlineMode val = (IHasOnlineMode)(object)((activeView is IHasOnlineMode) ? activeView : null);
            if (val != null && val.OnlineState.OnlineApplication != Guid.Empty)
            {
                return true;
            }
            return false;
        }

        public void RemovedFromUI()
        {
        }
    }
}
