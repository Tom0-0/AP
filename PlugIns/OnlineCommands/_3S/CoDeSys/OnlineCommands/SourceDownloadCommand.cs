#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{42545C70-5715-41de-B98D-CC3FEC79F536}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_source_download.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Source_download.htm")]
    public class SourceDownloadCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "sourcedownload" };

        public string ToolTipText => Name;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SourceDownloadCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SourceDownloadCommand_Name");

        public bool Enabled
        {
            get
            {
                Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
                if (activeAppObjectGuid == Guid.Empty)
                {
                    return false;
                }
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                if (activeDevice == null)
                {
                    return false;
                }
                if (!OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)1))
                {
                    return false;
                }
                ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(activeDevice.DeviceIdentification);
                if (!LocalTargetSettings.SourceDownloadAllowed.GetBoolValue(targetSettingsById))
                {
                    return false;
                }
                if (!ProjectOptionsHelper.OnlyOnDemand)
                {
                    return false;
                }
                if (ProjectOptionsHelper.GetDestinationDevice != Guid.Empty && ProjectOptionsHelper.GetDestinationDevice != ((IObject)activeDevice).MetaObject.ObjectGuid)
                {
                    return false;
                }
                return OnlineCommandHelper.GetApplication(activeAppObjectGuid).IsLoggedIn;
            }
        }

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public Guid Category => OnlineCommandCategory.Guid;

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments()
        {
            return new string[0];
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)1);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length != 0)
            {
                throw new BatchTooManyArgumentsException(BATCH_COMMAND, arguments.Length, 0);
            }
            if (OnlineFeatureHelper.CheckActiveApplication((OnlineFeatureEnum)1))
            {
                try
                {
                    new SourceDownload().DownloadFromUserCommand();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw ex;
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
