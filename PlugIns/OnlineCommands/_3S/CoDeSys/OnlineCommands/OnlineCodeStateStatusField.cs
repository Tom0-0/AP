using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{D065F7D0-98D8-4c38-81E7-A0D7A6EB93DC}")]
    public class OnlineCodeStateStatusField : ITextStatusField, IStatusField
    {
        public string Text
        {
            get
            {
                //IL_0000: Unknown result type (might be due to invalid IL or missing references)
                //IL_0005: Unknown result type (might be due to invalid IL or missing references)
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_001c: Expected I4, but got Unknown
                OnlineCodeState internalCodeState = InternalCodeStateProvider.InternalCodeState;
                return (int)internalCodeState switch
                {
                    3 => string.Empty,
                    0 => Strings.CodeUnchanged,
                    1 => Strings.CodeNeedsOnlineChange,
                    2 => Strings.CodeNeedsFullDownload,
                    _ => string.Empty,
                };
            }
        }

        public Color ForeColor
        {
            get
            {
                //IL_0000: Unknown result type (might be due to invalid IL or missing references)
                //IL_0005: Unknown result type (might be due to invalid IL or missing references)
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_0008: Invalid comparison between Unknown and I4
                //IL_000a: Unknown result type (might be due to invalid IL or missing references)
                //IL_000c: Invalid comparison between Unknown and I4
                OnlineCodeState internalCodeState = InternalCodeStateProvider.InternalCodeState;
                if ((int)internalCodeState == 1 || (int)internalCodeState == 2)
                {
                    return InternalCodeStateProvider.ChangedCodeTextColor;
                }
                return InternalCodeStateProvider.UnchangedCodeTextColor;
            }
        }

        public Color BackColor
        {
            get
            {
                //IL_0000: Unknown result type (might be due to invalid IL or missing references)
                //IL_0005: Unknown result type (might be due to invalid IL or missing references)
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_0008: Invalid comparison between Unknown and I4
                //IL_000a: Unknown result type (might be due to invalid IL or missing references)
                //IL_000c: Invalid comparison between Unknown and I4
                OnlineCodeState internalCodeState = InternalCodeStateProvider.InternalCodeState;
                if ((int)internalCodeState == 1 || (int)internalCodeState == 2)
                {
                    return InternalCodeStateProvider.ChangedCodeBackgroundColor;
                }
                return InternalCodeStateProvider.UnchangedCodeBackgroundColor;
            }
        }

        public int Width => 240;

        public bool Visible
        {
            get
            {
                //IL_0000: Unknown result type (might be due to invalid IL or missing references)
                //IL_0006: Invalid comparison between Unknown and I4
                if ((int)InternalCodeStateProvider.InternalCodeState == 3)
                {
                    return false;
                }
                return true;
            }
        }

        public ICommand DoubleClickCommand => null;
    }
}
