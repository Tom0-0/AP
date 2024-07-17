using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{C7A2DCEA-D175-45b1-A85B-4D589BCDBCBE}")]
    public class LoginActiveApplicationCommand : IStandardCommand, ICommand, IHasContextlessName, IHasAssociatedOnlineHelpTopic
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "loginactiveapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LoginActiveApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LoginActiveApplicationCommand_Name");

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (!_bIsExecutionInProgress && activeAppObjectGuid != Guid.Empty && OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)8))
                {
                    return OnlineCommandHelper.CanLogin(activeAppObjectGuid);
                }
                return false;
            }
        }

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(LoginActiveApplicationCommand), "_3S.CoDeSys.OnlineCommands.Resources.LoginSmall.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LoginActiveApplicationCommand_ContextlessName");

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            //IL_00cb: Unknown result type (might be due to invalid IL or missing references)
            //IL_00dd: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f4: Unknown result type (might be due to invalid IL or missing references)
            //IL_0115: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            try
            {
                _bIsExecutionInProgress = true;
                string stActualNodeName = string.Empty;
                ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
                Cursor current = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                if (OnlineCommandHelper.VerifyRealDeviceName(OnlineCommandHelper.ActiveAppObjectGuid, bIsActiveApplication: true, out stActualNodeName))
                {
                    IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                    string stCustomMessage = string.Empty;
                    if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("OnlineCommands", "SecureOnlineModeLoginString"))
                    {
                        stCustomMessage = ((IEngine3)APEnvironment.Engine).OEMCustomization.GetStringValue("OnlineCommands", "SecureOnlineModeLoginString");
                    }
                    else if (activeDevice != null && activeDevice.CommunicationSettings.Address != null)
                    {
                        string arg = activeDevice.CommunicationSettings.Address.ToString();
                        if (activeDevice.CommunicationSettings is ICommunicationSettings5 && ((ICommunicationSettings5)activeDevice.CommunicationSettings).ScanInformation != null && ((ICommunicationSettings5)activeDevice.CommunicationSettings).ScanInformation.IPAddressAndPort != null && ((ICommunicationSettings5)activeDevice.CommunicationSettings).ScanInformation.IPAddressAndPort != string.Empty)
                        {
                            arg = ((ICommunicationSettings5)activeDevice.CommunicationSettings).ScanInformation.IPAddressAndPort;
                        }
                        stCustomMessage = string.Format(Strings.PromptExecuteLogin, stActualNodeName, arg);
                    }
                    if (OnlineCommandHelper.PromptExecuteOperation_ActiveApplication((ICommand)(object)this, bPromptInNormalMode: false, stCustomMessage))
                    {
                        Cursor.Current = current;
                        return new string[0];
                    }
                }
                Cursor.Current = current;
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
            return null;
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)8);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return;
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length != 0)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
            }
            ILoginService2 val = null;
            try
            {
                _bIsExecutionInProgress = true;
                ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanLogin(activeAppObjectGuid))
                {
                    val = APEnvironment.CreateLoginServiceWrapper();
                    List<Guid> list = new List<Guid>(1) { activeAppObjectGuid };
                    val.ConfigureLogin((object)this, (IList<Guid>)list, (LoginServiceFlags)1, (OnlineChangeDialogHandlerDelegate)null, (LoginDialogHandlerDelegate)null);
                    if (((ILoginService)val).BeginLogin((object)this))
                    {
                        APEnvironment.OnlineUIMgr.Login(activeAppObjectGuid);
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
                APEnvironment.MessageService.Error(ex3.Message, "ErrorLoginActiveApp", Array.Empty<object>());
            }
            finally
            {
                if (val != null)
                {
                    try
                    {
                        ((ILoginService)val).EndLogin((object)this);
                    }
                    catch
                    {
                    }
                }
                _bIsExecutionInProgress = false;
            }
        }

        public string GetOnlineHelpKeyword()
        {
            return null;
        }

        public string GetOnlineHelpUrl()
        {
            return "codesys.chm::/_cds_cmd_login.htm";
        }
    }
}
