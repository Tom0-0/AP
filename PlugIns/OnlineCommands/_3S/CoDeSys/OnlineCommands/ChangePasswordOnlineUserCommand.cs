using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Security;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{B2D833CC-C429-4305-8A62-4EFFEEA604E5}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_change_password_online_user.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/online_change_password_online_user.htm")]
    public class ChangePasswordOnlineUserCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "changepasswordonlineuser" };

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ChangePasswordOnlineUserCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ChangePasswordOnlineUserCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.ChangePasswordOnlineUser.ico");

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                if (activeDevice == null)
                {
                    return false;
                }
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
                IOnlineDevice6 val = (IOnlineDevice6)(object)((onlineDevice is IOnlineDevice6) ? onlineDevice : null);
                if (val == null)
                {
                    return false;
                }
                if (((IOnlineDevice)val).IsConnected && !string.IsNullOrEmpty(val.LoggedOnUsername))
                {
                    return true;
                }
                return false;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments()
        {
            return new string[0];
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                return false;
            }
            return true;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0083: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a0: Unknown result type (might be due to invalid IL or missing references)
            //IL_019d: Unknown result type (might be due to invalid IL or missing references)
            //IL_01a8: Unknown result type (might be due to invalid IL or missing references)
            IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
            if (activeDevice == null)
            {
                return;
            }
            string empty = string.Empty;
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
            IOnlineDevice6 val = (IOnlineDevice6)(object)((onlineDevice is IOnlineDevice6) ? onlineDevice : null);
            if (val == null)
            {
                return;
            }
            empty = val.LoggedOnUsername;
            IDeviceUserManagement obj = ((IOnlineDevice5)val).CreateUserManagement();
            IDeviceUserManagement4 val2 = (IDeviceUserManagement4)(object)((obj is IDeviceUserManagement4) ? obj : null);
            if (val2 == null)
            {
                return;
            }
            bool flag = true;
            bool flag2 = false;
            try
            {
                ((IDeviceUserManagement)val2).Upload();
                if (((IDeviceUserManagement)val2).UserList != null)
                {
                    foreach (IDeviceUser item in (IEnumerable<IDeviceUser>)((IDeviceUserManagement)val2).UserList)
                    {
                        if (item.Name == empty)
                        {
                            flag = ((Enum)item.Flags).HasFlag((Enum)(object)(DeviceUserManagementFlags)8192);
                            flag2 = !((Enum)item.Flags).HasFlag((Enum)(object)(DeviceUserManagementFlags)4096);
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            string text = null;
            string text2 = null;
            int num = 0;
            bool flag3 = false;
            string text3 = null;
            string[] array = null;
            bool flag4 = false;
            IOnlineDevice onlineDevice2 = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
            IOnlineDevice17 val3 = (IOnlineDevice17)(object)((onlineDevice2 is IOnlineDevice17) ? onlineDevice2 : null);
            if (val3 != null && val3.RuntimeSystemVersion >= OnlineCommandHelper.RTS_VERSION_35160)
            {
                flag4 = true;
            }
            empty = ((!flag4) ? ((IX509UIService3)APEnvironment.X509UIService4).ServeUserConfigurationDialog(empty, array, Strings.ChangePasswordOnlineUserCommand_Dialog_Caption, false, false, ref flag, ref flag2, out text, out text3, out num) : APEnvironment.X509UIService4.ServeUserConfigurationDialog(empty, array, Strings.ChangePasswordOnlineUserCommand_Dialog_Caption, false, false, true, ref flag, ref flag2, out text, out text2, out text3, out num, out flag3));
            if (string.IsNullOrEmpty(empty))
            {
                return;
            }
            try
            {
                if (flag4)
                {
                    val2.ChangePasswordOnlineUser(empty, text, text2);
                }
                else
                {
                    ((IDeviceUserManagement2)val2).ChangePasswordOnlineUser(empty, text);
                }
                if (val is IOnlineDevice19)
                {
                    ((IOnlineDevice19)val).ResetLoggedOnUser();
                    ((IOnlineDevice11)(IOnlineDevice19)val).ForceDisconnect();
                }
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "ErrorChangePassword", Array.Empty<object>());
            }
        }
    }
}
