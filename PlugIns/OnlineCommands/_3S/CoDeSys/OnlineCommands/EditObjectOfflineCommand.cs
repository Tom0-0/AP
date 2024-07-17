#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.OnlineUI;
using System;
using System.Diagnostics;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{72A2F7C2-A650-4feb-ADA4-A8A9BEFF1291}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_edit_object_offline.htm")]
    [AssociatedOnlineHelpTopic("core.objectcommands.objects.chm::/edit_object_offline.htm")]
    public class EditObjectOfflineCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "object", "editoffline" };

        private static readonly Guid GUID_OBJECTCOMMANDCATEGORY = new Guid("{02ABB186-7154-49dd-9B61-6357AC0D60EE}");

        public Guid Category => GUID_OBJECTCOMMANDCATEGORY;

        public string Name => Strings.EditObjectOffline_Name;

        public string Description => Strings.EditObjectOffline_Description;

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
            INavigatorControl navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl) : null;
            if (!bContextMenu || (bContextMenu && navigatorControl != null))
            {
                IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
                return primaryProject != null && primaryProject.SelectedSVNodes != null && primaryProject.SelectedSVNodes.Length == 1 && OnlineCommandHelper.HasInstancePath(primaryProject.SelectedSVNodes[0].GetMetaObjectStub(), Guid.Empty);
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            //IL_0044: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length != 0)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 0);
            }
            try
            {
                OnlineState onlineState = default(OnlineState);
                onlineState.OnlineApplication = Guid.Empty;
                onlineState.InstancePath = null;
                APEnvironment.OnlineUIMgr.BeginPreselectOnlineState(onlineState);
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                Debug.Assert(primaryProject != null);
                IMetaObjectStub metaObjectStub = primaryProject.SelectedSVNodes[0].GetMetaObjectStub();
                Debug.Assert(metaObjectStub != null);
                Guid defaultEditorViewFactory = ((IEngine)APEnvironment.Engine).Frame.ViewFactoryManager.GetDefaultEditorViewFactory(metaObjectStub.ObjectType, metaObjectStub.EmbeddedObjectTypes);
                ((IEngine)APEnvironment.Engine).Frame.OpenEditorView(metaObjectStub, defaultEditorViewFactory, (string)null);
            }
            finally
            {
                APEnvironment.OnlineUIMgr.EndPreselectOnlineState();
            }
        }
    }
}
