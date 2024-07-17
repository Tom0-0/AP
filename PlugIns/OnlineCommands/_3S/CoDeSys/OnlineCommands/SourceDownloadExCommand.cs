#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{9904594C-7A74-4da7-9EFB-1918CAD98B3F}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_source_download.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Source_download.htm")]
    public class SourceDownloadExCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "sourcedownloadex" };

        public string ToolTipText => Name;

        public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SourceDownloadExCommand_Description");

        public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "SourceDownloadExCommand_Name");

        public bool Enabled
        {
            get
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
                {
                    return false;
                }
                return true;
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
            return !bContextMenu;
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
            try
            {
                new SourceDownloadEx().SelectDeviceAndTriggerDownload();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
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
