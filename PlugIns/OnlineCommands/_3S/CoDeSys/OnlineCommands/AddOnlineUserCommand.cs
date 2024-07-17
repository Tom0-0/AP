using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Security;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{0DE2BE1F-B649-4B00-A3E9-AA1AC6209806}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_add_online_user.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/online_add_online_user.htm")]
    public class AddOnlineUserCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "addonlineuser" };

        private static readonly string ADMIN = "Administrator";

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "AddOnlineUserCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "AddOnlineUserCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.AddOnlineUser.ico");

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
                if (onlineDevice == null)
                {
                    return false;
                }
                if (onlineDevice.IsConnected)
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
            string text = string.Empty;
            string text2 = null;
            bool flag = true;
            bool flag2 = false;
            int num = 0;
            string[] array = new string[1] { ADMIN };
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = true;
            DeviceUserManagementFlags result = ~DeviceUserManagementFlags.Owner;
            if (arguments.Length != 0 && arguments[0] == "EnforceActivateUserMgmt")
            {
                flag3 = true;
                if (arguments.Length == 4)
                {
                    flag5 = false;
                    flag4 = true;
                    text = arguments[1];
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new ArgumentException("First argument should be the username and can not be null or empty");
                    }
                    text2 = arguments[2] ?? "";
                    if (!Enum.TryParse<DeviceUserManagementFlags>(arguments[3], out result))
                    {
                        throw new ArgumentException(string.Format("Fourth argument should be the flags and could not be parsed as '{0}'.", "DeviceUserManagementFlags"));
                    }
                }
            }
            IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
            if (activeDevice == null)
            {
                return;
            }
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
            IOnlineDevice6 val = (IOnlineDevice6)(object)((onlineDevice is IOnlineDevice6) ? onlineDevice : null);
            if (val == null)
            {
                return;
            }
            IDeviceUserManagement obj = ((IOnlineDevice5)val).CreateUserManagement();
            IDeviceUserManagement2 val2 = (IDeviceUserManagement2)(object)((obj is IDeviceUserManagement2) ? obj : null);
            if (val2 == null)
            {
                return;
            }
            bool flag6 = true;
            IOnlineDevice17 val3 = (IOnlineDevice17)(object)((val is IOnlineDevice17) ? val : null);
            if (val3 != null && val3.RuntimeSystemVersion >= OnlineCommandHelper.RTS_VERSION_35160 && val2 is IDeviceUserManagement4)
            {
                flag6 = false;
            }
            if (flag5)
            {
                if (!flag6 && val is IOnlineDevice11 && val.LoggedOnUsername == "")
                {
                    if (2 != (int)APEnvironment.MessageService.Prompt(Strings.Prompt_ActivateUserManagement_new, (PromptChoice)2, (PromptResult)2, "Prompt_ActivateUserManagement_New", Array.Empty<object>()))
                    {
                        if (flag3)
                        {
                            throw new CancelledByUserException();
                        }
                        return;
                    }
                    flag4 = true;
                }
                string text3 = default(string);
                text = ((IX509UIService3)APEnvironment.X509UIService4).ServeUserConfigurationDialog(text, array, Strings.AddOnlineUserCommand_Dialog_Caption, true, true, ref flag, ref flag2, out text2, out text3, out num);
                if (string.IsNullOrEmpty(text))
                {
                    if (flag3)
                    {
                        throw new CancelledByUserException();
                    }
                    return;
                }
            }
            object obj2 = null;
            try
            {
                if (!(val is IOnlineDevice11) || !(val.LoggedOnUsername == "") || !flag6)
                {
                    goto IL_01f8;
                }
                if (2 != (int)APEnvironment.MessageService.Prompt(Strings.Prompt_ActivateUserManagement, (PromptChoice)2, (PromptResult)2, "Prompt_ActivateUserManagement", Array.Empty<object>()))
                {
                    return;
                }
                ((IDeviceUserManagement)val2).Upload();
                ((IDeviceUserManagement)val2).Download();
                ((IOnlineDevice11)val).ForceDisconnect();
                obj2 = new object();
                ((IOnlineDevice3)val).SharedConnect(obj2);
                goto IL_01f8;
            IL_01f8:
                if (val2 is IDeviceUserManagement3)
                {
                    if (flag5)
                    {
                        if (!flag)
                        {
                            result = ~DeviceUserManagementFlags.PasswordEditable;
                        }
                        if (flag2)
                        {
                            result = ~DeviceUserManagementFlags.PasswordUpToDate;
                        }
                    }
                    ((IDeviceUserManagement3)val2).AddOnlineUser(text, text2, result);
                    if (!flag4 && !flag6 && val2 is IDeviceUserManagement4)
                    {
                        ((IDeviceUserManagement)val2).Upload();
                        IDeviceGroup val4 = ((IDeviceUserManagement)(IDeviceUserManagement4)val2).GroupList[ADMIN];
                        val4.UserMembers.Add(text);
                        ((IDeviceUserManagement4)val2).SetGroupConfig(ADMIN, val4.GroupMembers, val4.UserMembers);
                    }
                }
                else
                {
                    val2.AddOnlineUser(text, text2);
                }
                if (flag6 && val != null && val.LoggedOnUsername == "")
                {
                    string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "AddOnlineUserCommand_LogoutMessage");
                    if (!flag5)
                    {
                        throw new DeviceLogoutAndLoginAsUserException(@string);
                    }
                    APEnvironment.MessageService.Prompt(@string, (PromptChoice)0, (PromptResult)0, "AddOnlineUserCommand_LogoutMessage", Array.Empty<object>());
                }
            }
            catch (Exception ex)
            {
                if (flag5)
                {
                    APEnvironment.MessageService.Error(ex.Message, "ErrorAddOnlineUser", Array.Empty<object>());
                    return;
                }
                throw;
            }
            finally
            {
                if (val != null && ((IOnlineDevice)val).IsConnected && obj2 != null)
                {
                    try
                    {
                        ((IOnlineDevice3)val).SharedDisconnect(obj2);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
