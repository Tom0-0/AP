#define DEBUG
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{5637CD44-CB3D-4f7c-BA9A-80E81482CD7B}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_source_upload.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Source_upload.htm")]
    internal class SourceUploadCömmand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "sourceupload" };

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => Strings.SourceUploadCommand_Name;

        public string Description => Strings.SourceUploadCommand_Description;

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled => true;

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments()
        {
            return new string[0];
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
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
                new SourceUpload().Upload();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }
    }
}
