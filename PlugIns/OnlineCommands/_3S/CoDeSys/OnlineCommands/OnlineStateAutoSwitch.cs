using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.OnlineUI;

namespace _3S.CoDeSys.OnlineCommands
{
    internal class OnlineStateAutoSwitch
    {
        private IEditorView _view;

        private OnlineState _oldOnlineState;

        private OnlineState _newOnlineState;

        public IEditorView View => _view;

        public OnlineState OldOnlineState => _oldOnlineState;

        public OnlineState NewOnlineState => _newOnlineState;

        public OnlineStateAutoSwitch(IEditorView view, OnlineState oldOnlineState, OnlineState newOnlineState)
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            //IL_000f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0015: Unknown result type (might be due to invalid IL or missing references)
            //IL_0016: Unknown result type (might be due to invalid IL or missing references)
            _view = view;
            _oldOnlineState = oldOnlineState;
            _newOnlineState = newOnlineState;
        }
    }
}
