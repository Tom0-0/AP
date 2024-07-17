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
    [TypeGuid("{300006BF-C49B-4702-8DD1-CA470CF06DDC}")]
    public class DeviceOperatingModeStatusField : IOwnerDrawnStatusField, IStatusField
    {
        private DeviceOperatingMode _currentOperatingMode;

        private bool _bNeedsUpdate;

        private static Brush s_brushGreen;

        private DeviceOperatingMode OperatingMode
        {
            get
            {
                //IL_0060: Unknown result type (might be due to invalid IL or missing references)
                //IL_0065: Unknown result type (might be due to invalid IL or missing references)
                //IL_006b: Unknown result type (might be due to invalid IL or missing references)
                //IL_007b: Unknown result type (might be due to invalid IL or missing references)
                //IL_0080: Unknown result type (might be due to invalid IL or missing references)
                //IL_0085: Unknown result type (might be due to invalid IL or missing references)
                //IL_008b: Unknown result type (might be due to invalid IL or missing references)
                //IL_0092: Unknown result type (might be due to invalid IL or missing references)
                //IL_009b: Unknown result type (might be due to invalid IL or missing references)
                //IL_00a8: Unknown result type (might be due to invalid IL or missing references)
                _bNeedsUpdate = false;
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && ((IObjectManager2)APEnvironment.ObjectMgr).IsObjectLoaded(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, activeAppObjectGuid) && ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(activeAppObjectGuid) != null)
                {
                    IOnlineDevice onlineDeviceForApplicationFast = OnlineCommandHelper.GetOnlineDeviceForApplicationFast(activeAppObjectGuid);
                    if (onlineDeviceForApplicationFast is IOnlineDevice17 && onlineDeviceForApplicationFast.IsConnected)
                    {
                        if (((IOnlineDevice17)onlineDeviceForApplicationFast).OperatingMode != _currentOperatingMode)
                        {
                            _bNeedsUpdate = true;
                            _currentOperatingMode = ((IOnlineDevice17)onlineDeviceForApplicationFast).OperatingMode;
                        }
                        return _currentOperatingMode;
                    }
                }
                if ((int)_currentOperatingMode != 0)
                {
                    _currentOperatingMode = (DeviceOperatingMode)0;
                    _bNeedsUpdate = true;
                }
                return _currentOperatingMode;
            }
        }

        public int Width => 26;

        public ICommand DoubleClickCommand
        {
            get
            {
                //IL_000d: Unknown result type (might be due to invalid IL or missing references)
                //IL_0012: Unknown result type (might be due to invalid IL or missing references)
                //IL_0013: Unknown result type (might be due to invalid IL or missing references)
                //IL_0015: Unknown result type (might be due to invalid IL or missing references)
                //IL_0027: Expected I4, but got Unknown
                if (((IEngine)APEnvironment.Engine).Frame != null)
                {
                    DeviceOperatingMode currentOperatingMode = _currentOperatingMode;
                    switch ((int)currentOperatingMode - 1)
                    {
                        case 0:
                            APEnvironment.MessageService.Information(Strings.CmdModeDebug_Info, "CmdModeDebug_Info", Array.Empty<object>());
                            break;
                        case 1:
                            APEnvironment.MessageService.Information(Strings.CmdModeLock_Info, "CmdModeLock_Info", Array.Empty<object>());
                            break;
                        case 2:
                            APEnvironment.MessageService.Information(Strings.CmdModeOperational_Info, "CmdModeOperational_Info", Array.Empty<object>());
                            break;
                    }
                }
                return null;
            }
        }

        public Color BackColor => Color.Transparent;

        public Color ForeColor => Color.Transparent;

        public bool Visible => (int)OperatingMode > 0;

        private Icon Icon
        {
            get
            {
                //IL_0001: Unknown result type (might be due to invalid IL or missing references)
                //IL_0006: Unknown result type (might be due to invalid IL or missing references)
                //IL_0007: Unknown result type (might be due to invalid IL or missing references)
                //IL_0009: Unknown result type (might be due to invalid IL or missing references)
                //IL_001b: Expected I4, but got Unknown
                DeviceOperatingMode currentOperatingMode = _currentOperatingMode;
                return ((int)currentOperatingMode - 1) switch
                {
                    1 => Strings.CmdLocked,
                    2 => Strings.CmdLocked,
                    _ => Strings.CmdDebug,
                };
            }
        }

        public bool NeedsRedraw => _bNeedsUpdate;

        public DeviceOperatingModeStatusField()
        {
            if (s_brushGreen == null)
            {
                s_brushGreen = new SolidBrush(Color.LightGreen);
            }
        }

        public void Draw(Graphics g, Rectangle rect)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Invalid comparison between Unknown and I4
            if ((int)_currentOperatingMode == 3)
            {
                g.FillRectangle(s_brushGreen, new Rectangle(rect.X + 1, rect.Y, 19, 18));
            }
            g.DrawIconUnstretched(Icon, new Rectangle(rect.X + 2, rect.Y, 16, 16));
        }
    }
}
