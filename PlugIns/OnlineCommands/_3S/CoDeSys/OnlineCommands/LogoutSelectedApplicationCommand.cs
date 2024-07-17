#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{ad6955d2-2713-4828-9d6c-1c0cf20abdbe}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_logoff.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/logout_from.htm")]
    public class LogoutSelectedApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "logoutMultiapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LogoutSelectedApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LogoutSelectedApplicationCommand_Name");

        public bool Enabled => OnlineFeatureHelper.CheckSelectedApplications((OnlineFeatureEnum)8);

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(LoginActiveApplicationCommand), "_3S.CoDeSys.OnlineCommands.Resources.LogoutSmall.ico");

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "LogoutSelectedApplicationCommand_ContextlessName");

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public string[] CreateBatchArguments()
        {
            return new string[0];
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
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            if (_bIsExecutionInProgress)
            {
                return;
            }
            Guid guid = Guid.Empty;
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 1);
            }
            if (arguments.Length == 1)
            {
                guid = OnlineCommandHelper.GetGuidFromArguments(arguments[0]);
            }
            try
            {
                _bIsExecutionInProgress = true;
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                Debug.Assert(primaryProject != null);
                ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
                if (guid != Guid.Empty)
                {
                    try
                    {
                        if (OnlineCommandHelper.CanLogout(guid) && OnlineCommandHelper.CheckForcedValues(guid))
                        {
                            OnlineCommandHelper.Logout(guid);
                        }
                        OnlineCommandHelper.LogoutDeviceAppIfNecessary(guid);
                    }
                    catch (CancelledByUserException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex2)
                    {
                        APEnvironment.MessageService.Error(ex2.Message, "ErrorLogoutSelectedApp01", Array.Empty<object>());
                    }
                    return;
                }
                ISVNode[] array = selectedSVNodes;
                foreach (ISVNode val2 in array)
                {
                    try
                    {
                        if (!typeof(IApplicationObject).IsAssignableFrom(val2.GetMetaObjectStub().ObjectType))
                        {
                            continue;
                        }
                        guid = val2.ObjectGuid;
                        Debug.Assert(guid != Guid.Empty);
                        if (OnlineCommandHelper.CanLogout(guid))
                        {
                            if (OnlineCommandHelper.CheckForcedValues(guid))
                            {
                                OnlineCommandHelper.Logout(guid);
                            }
                            OnlineCommandHelper.LogoutDeviceAppIfNecessary(guid);
                        }
                    }
                    catch (CancelledByUserException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex4)
                    {
                        APEnvironment.MessageService.Error(ex4.Message, "ErrorLogoutSelectedApp02", Array.Empty<object>());
                    }
                }
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }
    }
}
