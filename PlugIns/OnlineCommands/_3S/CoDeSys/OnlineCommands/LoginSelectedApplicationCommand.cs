using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{D796BF41-64F2-400d-BFF7-6AC496BA2FFC}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_login.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/login_to.htm")]
    public class LoginSelectedApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "loginmultiapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LoginSelectedApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LoginSelectedApplicationCommand_Name");

        public bool Enabled => OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)8);

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(LoginActiveApplicationCommand), "_3S.CoDeSys.OnlineCommands.Resources.LoginSmall.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LoginSelectedApplicationCommand_ContextlessName");

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            Cursor current = Cursor.Current;
            try
            {
                _bIsExecutionInProgress = true;
                string stActualNodeName = string.Empty;
                HashSet<Guid> hashSet = new HashSet<Guid>();
                ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
                Cursor.Current = Cursors.WaitCursor;
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
                    .Length >= 1)
                {
                    for (int i = 0; i < ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
                        .Length; i++)
                    {
                        Guid objectGuid = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[i].ObjectGuid;
                        if (OnlineCommandHelper.IsApplicationOnline(objectGuid) || !OnlineCommandHelper.VerifyRealDeviceName(objectGuid, bIsActiveApplication: false, out stActualNodeName))
                        {
                            continue;
                        }
                        IOnlineDevice onlineDeviceForApplication = OnlineCommandHelper.GetOnlineDeviceForApplication(objectGuid);
                        IOnlineDevice4 val = (IOnlineDevice4)(object)((onlineDeviceForApplication is IOnlineDevice4) ? onlineDeviceForApplication : null);
                        string stCustomMessage = string.Empty;
                        if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("OnlineCommands", "SecureOnlineModeLoginString"))
                        {
                            stCustomMessage = ((IEngine3)APEnvironment.Engine).OEMCustomization.GetStringValue("OnlineCommands", "SecureOnlineModeLoginString");
                        }
                        else if (val != null)
                        {
                            IDeviceAddress deviceAddressInUse = val.GetDeviceAddressInUse();
                            if (deviceAddressInUse != null)
                            {
                                stCustomMessage = string.Format(Strings.PromptExecuteLogin, stActualNodeName, deviceAddressInUse.ToString());
                            }
                        }
                        if (OnlineCommandHelper.PromptExecuteOperation_SpecificApplication(objectGuid, (ICommand)(object)this, bPromptInNormalMode: false, stCustomMessage))
                        {
                            hashSet.Add(objectGuid);
                        }
                    }
                    List<Guid> list = new List<Guid>();
                    foreach (Guid item in hashSet)
                    {
                        Guid deviceAppGuidOfApplication = OnlineCommandHelper.GetDeviceAppGuidOfApplication(item);
                        if (deviceAppGuidOfApplication != Guid.Empty && !hashSet.Contains(deviceAppGuidOfApplication))
                        {
                            list.Add(item);
                        }
                    }
                    foreach (Guid item2 in list)
                    {
                        hashSet.Add(item2);
                    }
                    if (hashSet.Count > 0)
                    {
                        return hashSet.Select((Guid g) => g.ToString()).ToArray();
                    }
                }
            }
            finally
            {
                Cursor.Current = current;
                _bIsExecutionInProgress = false;
            }
            return null;
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                INavigatorControl navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null;
                return navigatorControl != null && OnlineCommandHelper.CanLoginToAny(navigatorControl.SelectedSVNodes) && OnlineFeatureHelper.CheckSelectedApplications(OnlineFeatureEnum.CoreApplicationHandling);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            if (_bIsExecutionInProgress)
            {
                return;
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            Guid result = Guid.Empty;
            if (arguments.Length == 1 && !Guid.TryParse(arguments[0], out result))
            {
                throw new ArgumentException("argument[0]: Expected a Guid!");
            }
            List<Guid> list = new List<Guid>();
            if (arguments.Length >= 1)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (Guid.TryParse(arguments[i], out result))
                    {
                        list.Add(result);
                    }
                }
            }
            ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
            ILoginService2 val = APEnvironment.CreateLoginServiceWrapper();
            for (int j = 0; j < list.Count; j++)
            {
                try
                {
                    _bIsExecutionInProgress = true;
                    if (OnlineCommandHelper.CanLogin(list[j]))
                    {
                        List<Guid> list2 = new List<Guid>(1);
                        list2.Add(list[j]);
                        val.ConfigureLogin((object)this, (IList<Guid>)list2, (LoginServiceFlags)1, (OnlineChangeDialogHandlerDelegate)null, (LoginDialogHandlerDelegate)null);
                        if (((ILoginService)val).BeginLogin((object)this))
                        {
                            APEnvironment.OnlineUIMgr.Login(list[j]);
                        }
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
                    if (list.Count > 1 && j < list.Count - 1)
                    {
                        int nDelayMs = 300;
                        IOnlineDevice onlineDeviceForApplicationFast = OnlineCommandHelper.GetOnlineDeviceForApplicationFast(list[j]);
                        IOnlineDevice15 val3 = (IOnlineDevice15)(object)((onlineDeviceForApplicationFast is IOnlineDevice15) ? onlineDeviceForApplicationFast : null);
                        if (val3 != null)
                        {
                            nDelayMs = (int)(val3.MonitoringIntervalTicks / 10000) + 100;
                        }
                        OnlineCommandHelper.ActiveWaitWithTimeout(nDelayMs);
                    }
                    _bIsExecutionInProgress = false;
                }
            }
        }
    }
}
