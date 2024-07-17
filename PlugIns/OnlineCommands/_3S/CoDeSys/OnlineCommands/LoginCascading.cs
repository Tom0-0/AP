using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{F3D51D16-1CD3-4C92-9D92-612482FE3F55}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_login_all_applications.htm")]
    public class LoginCascading : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "loginCascading" };

        public bool Enabled
        {
            get
            {
                if (OnlineCommandHelper.GetDeviceApplicationGuidOfSelectedDevice() == Guid.Empty)
                {
                    return false;
                }
                Func<Guid, bool> predicate = (Guid g) => OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)8, g) && OnlineCommandHelper.CanLogin(g);
                return OnlineCommandHelper.GetDeviceAppSubApplicationsOfSelectedDevice().Any(predicate);
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public Guid Category => OnlineCommandCategory.Guid;

        public string Description => Strings.LoginCascadingCommand_Descption;

        public Icon LargeIcon => SmallIcon;

        public string Name => Strings.LoginCascadingCommand_Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.OnlineCommands.Resources.LoginSmall.ico");

        public string ToolTipText => Name;

        public bool InExecution { get; set; }

        public string[] CreateBatchArguments()
        {
            if (InExecution)
            {
                return null;
            }
            Guid deviceApplicationGuidOfSelectedDevice = OnlineCommandHelper.GetDeviceApplicationGuidOfSelectedDevice();
            if (deviceApplicationGuidOfSelectedDevice == Guid.Empty)
            {
                throw new InvalidOperationException("No device application selected.");
            }
            return new string[1] { deviceApplicationGuidOfSelectedDevice.ToString() };
        }

        public void ExecuteBatch(string[] arguments)
        {
            if (InExecution)
            {
                return;
            }
            IList<Guid> list = OnlineCommandHelper.CollectCascadingAppsFromArguments(arguments);
            try
            {
                ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
                InExecution = true;
                for (int i = 0; i < list.Count; i++)
                {
                    ILoginService2 val = APEnvironment.CreateLoginServiceWrapper();
                    try
                    {
                        if (OnlineCommandHelper.CanLogin(list[i]))
                        {
                            Guid guid = list[i];
                            List<Guid> list2 = new List<Guid>(1) { guid };
                            val.ConfigureLogin((object)this, (IList<Guid>)list2, (LoginServiceFlags)1, (OnlineChangeDialogHandlerDelegate)null, (LoginDialogHandlerDelegate)null);
                            if (!((ILoginService)val).BeginLogin((object)this))
                            {
                                return;
                            }
                            APEnvironment.OnlineUIMgr.Login(guid);
                        }
                    }
                    catch (CancelledByUserException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (Exception ex3)
                    {
                        APEnvironment.MessageService.Error(ex3.Message, "ErrorLoginSelectedApp", Array.Empty<object>());
                        return;
                    }
                    finally
                    {
                        try
                        {
                            ((ILoginService)val).EndLogin((object)this);
                        }
                        catch
                        {
                        }
                        if (list.Count > 1 && i < list.Count - 1)
                        {
                            int nDelayMs = 300;
                            IOnlineDevice onlineDeviceForApplicationFast = OnlineCommandHelper.GetOnlineDeviceForApplicationFast(list[i]);
                            IOnlineDevice15 val3 = (IOnlineDevice15)(object)((onlineDeviceForApplicationFast is IOnlineDevice15) ? onlineDeviceForApplicationFast : null);
                            if (val3 != null)
                            {
                                nDelayMs = (int)(val3.MonitoringIntervalTicks / 10000) + 100;
                            }
                            OnlineCommandHelper.ActiveWaitWithTimeout(nDelayMs);
                        }
                    }
                }
            }
            finally
            {
                InExecution = false;
            }
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                return Enabled;
            }
            return false;
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }
    }
}
