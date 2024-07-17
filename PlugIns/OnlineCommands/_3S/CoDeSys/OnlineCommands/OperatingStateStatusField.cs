using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Views;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{79848691-21E2-4031-B4E3-336445711BC9}")]
    public class OperatingStateStatusField : ITextStatusField, IStatusField
    {
        private OperatingState OperatingState
        {
            get
            {
                //IL_002f: Unknown result type (might be due to invalid IL or missing references)
                if (OnlineCommandHelper.ActiveAppObjectGuid != Guid.Empty)
                {
                    IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(OnlineCommandHelper.ActiveAppObjectGuid);
                    if (application == null || !application.IsLoggedIn)
                    {
                        return (OperatingState)0;
                    }
                    return application.OperatingState;
                }
                return (OperatingState)0;
            }
        }

        public string Text
        {
            get
            {
                //IL_0009: Unknown result type (might be due to invalid IL or missing references)
                //IL_000e: Unknown result type (might be due to invalid IL or missing references)
                //IL_000f: Unknown result type (might be due to invalid IL or missing references)
                //IL_0015: Unknown result type (might be due to invalid IL or missing references)
                //IL_0050: Unknown result type (might be due to invalid IL or missing references)
                //IL_0052: Unknown result type (might be due to invalid IL or missing references)
                //IL_008b: Unknown result type (might be due to invalid IL or missing references)
                //IL_0091: Unknown result type (might be due to invalid IL or missing references)
                //IL_00ca: Unknown result type (might be due to invalid IL or missing references)
                //IL_00cc: Unknown result type (might be due to invalid IL or missing references)
                //IL_0105: Unknown result type (might be due to invalid IL or missing references)
                //IL_0107: Unknown result type (might be due to invalid IL or missing references)
                //IL_0140: Unknown result type (might be due to invalid IL or missing references)
                //IL_0142: Unknown result type (might be due to invalid IL or missing references)
                //IL_017b: Unknown result type (might be due to invalid IL or missing references)
                //IL_017e: Unknown result type (might be due to invalid IL or missing references)
                //IL_01b7: Unknown result type (might be due to invalid IL or missing references)
                //IL_01ba: Unknown result type (might be due to invalid IL or missing references)
                //IL_01f3: Unknown result type (might be due to invalid IL or missing references)
                //IL_01f9: Unknown result type (might be due to invalid IL or missing references)
                //IL_0232: Unknown result type (might be due to invalid IL or missing references)
                //IL_0238: Unknown result type (might be due to invalid IL or missing references)
                string text = string.Empty;
                bool flag = true;
                OperatingState operatingState = OperatingState;
                if (((int)operatingState & 0x80000) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "core_dump_creating");
                    flag = false;
                }
                else if (((int)operatingState & 1) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "program_loaded");
                    flag = false;
                }
                if (((int)operatingState & 0x20000) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "core_dump_loaded");
                    flag = false;
                }
                if (((int)operatingState & 2) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "download");
                    flag = false;
                }
                if (((int)operatingState & 4) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "online_change");
                    flag = false;
                }
                if (((int)operatingState & 8) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "store_bootproject");
                    flag = false;
                }
                if (((int)operatingState & 0x10) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "force_active");
                    flag = false;
                }
                if (((int)operatingState & 0x20) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "exception");
                    flag = false;
                }
                if (((int)operatingState & 0x4000) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "flow_active");
                    flag = false;
                }
                if (((int)operatingState & 0x40000) != 0)
                {
                    text = text + (flag ? string.Empty : " - ") + ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "executionpoints_active");
                    flag = false;
                }
                return text;
            }
        }

        public int Width => 240;

        public ICommand DoubleClickCommand
        {
            get
            {
                if (((int)OperatingState & 0x20) != 0)
                {
                    return (ICommand)(object)new OpenLoggerPage();
                }
                return null;
            }
        }

        public Color BackColor
        {
            get
            {
                if (((int)OperatingState & 0x20) != 0)
                {
                    if ((uint)DateTime.Now.Millisecond % 1000u <= 500)
                    {
                        return Color.Red;
                    }
                    return Color.Transparent;
                }
                return Color.Transparent;
            }
        }

        public Color ForeColor => SystemColors.ControlText;

        public bool Visible => (int)OperatingState > 0;
    }
}
