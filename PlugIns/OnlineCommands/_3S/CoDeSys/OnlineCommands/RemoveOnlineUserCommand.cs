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
    [TypeGuid("{81FC1DBD-081A-4027-AE70-182C55192798}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_remove_online_user.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/online_remove_online_user.htm")]
    public class RemoveOnlineUserCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "removeonlineuser" };

        private static readonly Version RTS_VERSION_351200 = new Version("3.5.12.0");

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "RemoveOnlineUserCommand_Name");

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "RemoveOnlineUserCommand_Description");

        public string ToolTipText => Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.RemoveOnlineUser.ico");

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
            //IL_00ac: Unknown result type (might be due to invalid IL or missing references)
            //IL_010d: Unknown result type (might be due to invalid IL or missing references)
            IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
            if (activeDevice == null)
            {
                return;
            }
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
            IOnlineDevice11 val = (IOnlineDevice11)(object)((onlineDevice is IOnlineDevice11) ? onlineDevice : null);
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
            try
            {
                ((IDeviceUserManagement)val2).Upload();
                if (((IDeviceUserManagement)val2).UserList != null && ((IDeviceUserManagement)val2).UserList.Count > 0)
                {
                    string[] array = new string[((IDeviceUserManagement)val2).UserList.Count];
                    for (int i = 0; i < ((IDeviceUserManagement)val2).UserList.Count; i++)
                    {
                        array[i] = ((IDeviceUserManagement)val2).UserList[i].Name;
                    }
                    string text = null;
                    bool flag = true;
                    if (val is IOnlineDevice17)
                    {
                        flag = ((IOnlineDevice17)val).RuntimeSystemVersion < RTS_VERSION_351200;
                    }
                    string text2 = ((IX509UIService3)APEnvironment.X509UIService4).ServeUserRemovalDialog(Strings.RemoveOnlineUserCommand_Dialog_Caption, array, flag, out text);
                    if (!string.IsNullOrEmpty(text2) && (!(((IOnlineDevice6)val).LoggedOnUsername == text2) || (int)APEnvironment.MessageService.Prompt(string.Format(Strings.Prompt_UserMgmt_DeleteCurrentUser, text2), (PromptChoice)1, (PromptResult)1, "Prompt_DeleteCurrentOnlineUser", Array.Empty<object>()) == 0))
                    {
                        val2.RemoveOnlineUser(text2, text);
                        if (((IOnlineDevice6)val).LoggedOnUsername == text2)
                        {
                            val.ForceDisconnect();
                        }
                    }
                }
                else
                {
                    APEnvironment.MessageService.Information(Strings.Error_NoUserInformationFound, "RemoveOnlineUser_NoInfo", Array.Empty<object>());
                }
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "ErrorRemoveOnlineUser", Array.Empty<object>());
            }
        }
    }
}
