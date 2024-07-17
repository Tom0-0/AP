using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{FCD3D68C-509A-4C36-8925-D9422C68C437}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_unforce_all_values_of_application.htm")]
    [AssociatedOnlineHelpTopic("core.onlinecommands.online.chm::/unforce_values.htm")]
    public class UnforceSelectedApplicationCommand : IStandardCommand, ICommand, IHasContextlessName
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "unforceselectedaplication" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Name;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceSelectedAppCommand_Description");

        public string Name
        {
            get
            {
                string devicePrefixedApplicationName = OnlineCommandHelper.GetDevicePrefixedApplicationName(ForceSelectedApplicationCommand.DetermineSelectedApplication());
                if (string.IsNullOrEmpty(devicePrefixedApplicationName))
                {
                    return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceSelectedAppCommand_ContextlessName");
                }
                return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceSelectedAppCommand_Name"), devicePrefixedApplicationName);
            }
        }

        public bool Enabled
        {
            get
            {
                Guid guid = ForceSelectedApplicationCommand.DetermineSelectedApplication();
                if (OnlineCommandHelper.IsApplicationOnline(guid) && OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)4, guid))
                {
                    return true;
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string ContextlessName => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnforceSelectedAppCommand_ContextlessName");

        public string[] CreateBatchArguments()
        {
            if (_bIsExecutionInProgress)
            {
                return null;
            }
            Guid guid = ForceSelectedApplicationCommand.DetermineSelectedApplication();
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

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_0039: Unknown result type (might be due to invalid IL or missing references)
            //IL_0056: Unknown result type (might be due to invalid IL or missing references)
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
                try
                {
                    _bIsExecutionInProgress = true;
                    OnlineCommandHelper.UnforceSelectedApplication(result);
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
