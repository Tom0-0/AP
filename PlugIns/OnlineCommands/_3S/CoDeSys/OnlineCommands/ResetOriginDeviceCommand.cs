using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{ec20a2f0-fa6f-488d-8b1b-3fe5fafac6b5}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_reset_origin_device.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Reset_Origin_Device.htm")]
    public class ResetOriginDeviceCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "resetorigindevice" };

        private static readonly Version MinimumVersionAdvancedReset = new Version("3.5.16.20");

        private static readonly Version MinimumVersionKeepUserMgr = new Version("3.5.12.0");

        private static readonly string SUPPORT_KEEPING_USER_MGR = "SupportKeepingUserManagement";

        private static readonly string SUPPORT_ADVANCED_RESET = "SupportAdvancedReset";

        private static readonly string RESET_ALL = "ResetAll";

        private static readonly string KEEP_USER_MGR = "KeepUsrMgr";

        internal static readonly string DELETE_ITEM = "DELETE";

        internal static readonly string KEEP_ITEM = "KEEP";

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name
        {
            get
            {
                IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
                return string.Format(arg0: (selectedDevice == null) ? Strings.None : ((IObject)selectedDevice).MetaObject.Name.ToString(), format: Strings.ResetOriginDeviceCmd_Name);
            }
        }

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ResetOriginDeviceCmd_Description");

        public string ToolTipText => Description;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                if (((APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null) == null)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckActiveApplication(OnlineFeatureEnum.ConnectDevice))
                {
                    return false;
                }
                IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
                return selectedDevice != null && selectedDevice.AllowsDirectCommunication;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments()
        {
            //IL_00ba: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d4: Unknown result type (might be due to invalid IL or missing references)
            //IL_00da: Invalid comparison between Unknown and I4
            //IL_013d: Unknown result type (might be due to invalid IL or missing references)
            //IL_015c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0176: Unknown result type (might be due to invalid IL or missing references)
            //IL_017c: Invalid comparison between Unknown and I4
            IOnlineDevice20 val = null;
            IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
            if (selectedDevice != null)
            {
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)selectedDevice).MetaObject.ObjectGuid);
                val = (IOnlineDevice20)(object)((onlineDevice is IOnlineDevice20) ? onlineDevice : null);
            }
            if (val == null || ((IOnlineDevice17)val).RuntimeSystemVersion < MinimumVersionKeepUserMgr)
            {
                if (OnlineCommandHelper.PromptExecuteOperation_SelectedDevice((ICommand)(object)this, bPromptInNormalMode: true))
                {
                    return new string[0];
                }
                return null;
            }
            if ((((IOnlineDevice17)val).RuntimeSystemVersion >= MinimumVersionKeepUserMgr && ((IOnlineDevice17)val).RuntimeSystemVersion < MinimumVersionAdvancedReset) || !(val is IOnlineDeviceWithAdvancedServices))
            {
                IList<IResetOriginConfigurationItem> list = new List<IResetOriginConfigurationItem>();
                IResetOriginConfigurationItem val2 = (IResetOriginConfigurationItem)(object)new ResetOriginConfigurationItem();
                val2.Description = (Strings.UserManagement);
                val2.CanDelete = (true);
                val2.Delete = (true);
                list.Add(val2);
                ResetOriginCustomControl resetOriginCustomControl = new ResetOriginCustomControl(list);
                if ((int)((IMessageService6)((IEngine)APEnvironment.Engine).MessageService).PromptWithCustomControls(Strings.Prompt_BasicResetOrigin, (ICustomControlProvider)(object)resetOriginCustomControl, (PromptChoice)2, (PromptResult)2, (EventHandler)null, (EventArgs)null, "PromptExecuteOperation", Array.Empty<object>()) == 2)
                {
                    if (!(resetOriginCustomControl.GetValue(list[0].Description, out var _) == KEEP_ITEM))
                    {
                        return new string[2] { SUPPORT_KEEPING_USER_MGR, RESET_ALL };
                    }
                    return new string[2] { SUPPORT_KEEPING_USER_MGR, KEEP_USER_MGR };
                }
                return null;
            }
            object obj = new object();
            try
            {
                ((IOnlineDevice3)val).SharedConnect(obj);
                IList<IResetOriginConfigurationItem> list2 = ((IOnlineDeviceWithAdvancedServices)val).ReadResetOriginConfiguration();
                ResetOriginCustomControl resetOriginCustomControl2 = new ResetOriginCustomControl(list2);
                if ((int)((IMessageService6)((IEngine)APEnvironment.Engine).MessageService).PromptWithCustomControls(Strings.Prompt_AdvancedResetOrigin, (ICustomControlProvider)(object)resetOriginCustomControl2, (PromptChoice)2, (PromptResult)2, (EventHandler)null, (EventArgs)null, "PromptExecuteOperation", Array.Empty<object>()) == 2)
                {
                    string text = string.Empty;
                    string text2 = string.Empty;
                    for (int i = 0; i < list2.Count; i++)
                    {
                        if (text2 != string.Empty)
                        {
                            text2 += ",";
                        }
                        text2 += list2[i].Description;
                        if (text != string.Empty)
                        {
                            text += ",";
                        }
                        IList<string> AllowedValues2;
                        string value = resetOriginCustomControl2.GetValue(list2[i].Description, out AllowedValues2);
                        if (value == KEEP_ITEM)
                        {
                            text += "1";
                            continue;
                        }
                        if (value == DELETE_ITEM)
                        {
                            text += "0";
                            continue;
                        }
                        throw new InvalidOperationException("Data inconsistency, reset origin operation aborted!");
                    }
                    return new string[3] { SUPPORT_ADVANCED_RESET, text, text2 };
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                try
                {
                    ((IOnlineDevice3)val).SharedDisconnect(obj);
                }
                catch
                {
                }
            }
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu && Enabled)
            {
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)6);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0068: Unknown result type (might be due to invalid IL or missing references)
            //IL_0079: Unknown result type (might be due to invalid IL or missing references)
            //IL_0137: Unknown result type (might be due to invalid IL or missing references)
            //IL_0147: Expected O, but got Unknown
            //IL_01a8: Unknown result type (might be due to invalid IL or missing references)
            //IL_01b7: Unknown result type (might be due to invalid IL or missing references)
            //IL_01cd: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                return;
            }
            IDeviceObject selectedDevice = OnlineCommandHelper.GetSelectedDevice();
            if (selectedDevice == null)
            {
                return;
            }
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)selectedDevice).MetaObject.ObjectGuid);
            IOnlineDevice7 val = (IOnlineDevice7)(object)((onlineDevice is IOnlineDevice7) ? onlineDevice : null);
            if (val == null)
            {
                return;
            }
            object obj = new object();
            try
            {
                ((IOnlineDevice3)val).SharedConnect(obj);
                if (arguments.Length == 0)
                {
                    val.ResetOrigin();
                }
                else if (arguments[0] == SUPPORT_KEEPING_USER_MGR)
                {
                    if (arguments[1] == RESET_ALL)
                    {
                        ((IOnlineDevice20)val).ResetOrigin((ResetOriginConfigurations)0);
                    }
                    else
                    {
                        ((IOnlineDevice20)val).ResetOrigin((ResetOriginConfigurations)1);
                    }
                }
                else if (arguments[0] == SUPPORT_ADVANCED_RESET)
                {
                    if (!(val is IOnlineDeviceWithAdvancedServices))
                    {
                        throw new InvalidOperationException("Advanced reset origin can only be used if the online device implements 'IAdvancedOnlineServiceProvider'!");
                    }
                    string[] array = arguments[1].Split(',');
                    string[] array2 = arguments[2].Split(',');
                    if (array.Length != array2.Length)
                    {
                        throw new ArgumentException("Inconsistent arguments 1 and 2. The number of commas need to be identical");
                    }
                    try
                    {
                        IList<IResetOriginConfigurationItem> list = new List<IResetOriginConfigurationItem>();
                        for (int i = 0; i < array.Length; i++)
                        {
                            ResetOriginConfigurationItem resetOriginConfigurationItem = new ResetOriginConfigurationItem();
                            resetOriginConfigurationItem.Description = array2[i];
                            resetOriginConfigurationItem.Delete = array[i] == "0";
                            list.Add((IResetOriginConfigurationItem)(object)resetOriginConfigurationItem);
                        }
                        ((IOnlineDeviceWithAdvancedServices)val).ResetOrigin(list);
                    }
                    catch (OnlineManager2Exception val2)
                    {
                        OnlineManager2Exception val3 = val2;
                        string text = Strings.ResetOriginFailed + Environment.NewLine;
                        for (int j = 0; j < array.Length; j++)
                        {
                            if (!(array[j] == "1") && ((val3.ErrorCode >> j) & 1) == 1)
                            {
                                text = text + Environment.NewLine + array2[j];
                            }
                        }
                        throw new OnlineManager2Exception(text, val3.ErrorCode);
                    }
                }
                if (val is IOnlineDevice19)
                {
                    ((IOnlineDevice19)val).ResetLoggedOnUser();
                }
            }
            finally
            {
                try
                {
                    if (val is IOnlineDevice11)
                    {
                        ((IOnlineDevice11)val).ForceDisconnect();
                    }
                    else
                    {
                        ((IOnlineDevice3)val).SharedDisconnect(obj);
                    }
                }
                catch
                {
                }
            }
        }
    }
}
