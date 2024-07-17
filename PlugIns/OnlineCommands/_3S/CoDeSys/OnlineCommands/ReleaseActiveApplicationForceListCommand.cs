#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{E7EE1AC1-2E71-4465-AA6B-3A808CD4D78D}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_unforce_all_values.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Release_Forcelist_for__Application_application_path.htm")]
    public class ReleaseActiveApplicationForceListCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "releaseforcelistactiveapp" };

        private bool _bIsExecutionInProgress;

        public string ToolTipText => Description;

        public Icon LargeIcon => SmallIcon;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ReleasetForceListActiveApplicationCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ReleaseForceListActiveApplicationCommand_Name");

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)4))
                {
                    return OnlineCommandHelper.CanReleaseForceValues(activeAppObjectGuid);
                }
                return false;
            }
        }

        public Icon SmallIcon => null;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

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
            try
            {
                _bIsExecutionInProgress = true;
                if (OnlineCommandHelper.PromptExecuteOperation_ActiveApplication((ICommand)(object)this, bPromptInNormalMode: false, null))
                {
                    return new string[0];
                }
                return null;
            }
            finally
            {
                _bIsExecutionInProgress = false;
            }
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)4);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                if (arguments == null)
                {
                    throw new ArgumentNullException("arguments");
                }
                if (arguments.Length != 0)
                {
                    throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
                }
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid != Guid.Empty && OnlineCommandHelper.CanReleaseForceValues(activeAppObjectGuid))
                {
                    IOnlineApplication application = OnlineCommandHelper.GetApplication(activeAppObjectGuid);
                    if (application != null)
                    {
                        application.ReleaseForceValues();
                    }
                }
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
            }
        }
    }
}
