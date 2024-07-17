using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.OnlineUI;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{92BD0446-3916-48CC-A50B-9E445EA4F21D}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_force_all_values_of_application.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/force_values.htm")]
    public class ForceSelectedApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "forceselectedaplication" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceSelectedAppCommand_Description");

        public string Name
        {
            get
            {
                string devicePrefixedApplicationName = OnlineCommandHelper.GetDevicePrefixedApplicationName(DetermineSelectedApplication());
                if (string.IsNullOrEmpty(devicePrefixedApplicationName))
                {
                    return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceSelectedAppCommand_ContextlessName");
                }
                return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceSelectedAppCommand_Name"), devicePrefixedApplicationName);
            }
        }

        public bool Enabled
        {
            get
            {
                Guid guid = DetermineSelectedApplication();
                if (OnlineCommandHelper.CanWriteOrForceValuesSelectedApp(guid) && OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)4, guid))
                {
                    return true;
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceSelectedAppCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            Guid guid = DetermineSelectedApplication();
            if (OnlineCommandHelper.IsApplicationOnline(guid) && OnlineCommandHelper.PromptExecuteOperation_SpecificApplication(guid, (ICommand)(object)this, bPromptInNormalMode: false, null))
            {
                return new string[1] { guid.ToString() };
            }
            return null;
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (bContextMenu)
            {
                return Enabled;
            }
            return false;
        }

        internal static Guid DetermineSelectedApplication()
        {
            //IL_0050: Unknown result type (might be due to invalid IL or missing references)
            //IL_0055: Unknown result type (might be due to invalid IL or missing references)
            Guid result = Guid.Empty;
            if (((IEngine)APEnvironment.Engine).Frame != null)
            {
                if (!(((IEngine)APEnvironment.Engine).Frame.ActiveView is INavigatorControl))
                {
                    if (((IEngine)APEnvironment.Engine).Frame.ActiveView is IHasOnlineMode)
                    {
                        result = ((IHasOnlineMode)((IEngine)APEnvironment.Engine).Frame.ActiveView).OnlineState.OnlineApplication;
                    }
                }
                else if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes
                    .Length == 1)
                {
                    result = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes[0].ObjectGuid;
                }
            }
            return result;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_0039: Unknown result type (might be due to invalid IL or missing references)
            //IL_0056: Unknown result type (might be due to invalid IL or missing references)
            //IL_0071: Unknown result type (might be due to invalid IL or missing references)
            if (!_bIsExecutionInProgress)
            {
                if (arguments == null)
                {
                    throw new ArgumentNullException("arguments");
                }
                if (arguments.Length == 0)
                {
                    throw new BatchTooFewArgumentsException(BATCH_COMMAND, arguments.Length, 1);
                }
                if (arguments.Length > 1)
                {
                    throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 1);
                }
                if (!Guid.TryParse(arguments[0], out var result))
                {
                    throw new BatchWrongArgumentTypeException(BATCH_COMMAND, 0, "Guid");
                }
                IOnlineApplication application = OnlineCommandHelper.GetApplication(result);
                if (application == null)
                {
                    throw new BatchWrongArgumentTypeException(BATCH_COMMAND, 0, "Guid for Application Object");
                }
                try
                {
                    _bIsExecutionInProgress = true;
                    OnlineCommandHelper.WriteOrForceValues(bForce: true, (IOnlineApplication[])(object)new IOnlineApplication[1] { application });
                }
                finally
                {
                    _bIsExecutionInProgress = false;
                }
            }
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }
    }
}
