#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{1389E61C-BE22-4DF5-A5CE-9DC3E72421DE}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_active_app_selector.htm")]
    public class ActiveApplicationSelectorTextCommand : ITextCommand3, ITextCommand2, ITextCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "activeapplicationselector" };

        public DropDownListStyle DropDownListStyle => (DropDownListStyle)2;

        public string[] ListItems
        {
            get
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
                {
                    IEnumerable<IMetaObjectStub> allApplications = GetAllApplications();
                    if (allApplications.Count() == 0)
                    {
                        return new string[0];
                    }
                    int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                    List<string> list = new List<string>();
                    foreach (IMetaObjectStub item in allApplications)
                    {
                        list.Add(GetQualifiedName(handle, item.ObjectGuid));
                    }
                    return list.ToArray();
                }
                return new string[0];
            }
        }

        public string CurrentText
        {
            get
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject == null || primaryProject.ActiveApplication == Guid.Empty)
                {
                    return string.Empty;
                }
                return GetQualifiedName(primaryProject.Handle, primaryProject.ActiveApplication);
            }
        }

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => Strings.CmdActiveApplicationSelector_Name;

        public string Description => Name;

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled => true;

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments(string stText)
        {
            return new string[1] { stText };
        }

        public string[] CreateBatchArguments(string stText, bool bInvokedByContextMenu)
        {
            return CreateBatchArguments(stText);
        }

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (!bContextMenu)
            {
                return ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0047: Unknown result type (might be due to invalid IL or missing references)
            //IL_0067: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 1);
            }
            if (((IEngine)APEnvironment.Engine).Frame == null || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                throw new BatchInteractiveException(BatchCommand);
            }
            Guid guid = Guid.Empty;
            IEnumerable<IMetaObjectStub> allApplications = GetAllApplications();
            if (allApplications.Count() == 0)
            {
                throw new BatchInteractiveException(BatchCommand);
            }
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            foreach (IMetaObjectStub item in allApplications)
            {
                if (GetQualifiedName(handle, item.ObjectGuid) == arguments[0])
                {
                    guid = item.ObjectGuid;
                    break;
                }
            }
            if (guid != Guid.Empty)
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                Debug.Assert(primaryProject != null);
                primaryProject.ActiveApplication = (guid);
            }
        }

        private IEnumerable<IMetaObjectStub> GetAllApplications()
        {
            int nPrimHandle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nPrimHandle);
            Guid[] array = allObjects;
            foreach (Guid guid in array)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nPrimHandle, guid);
                if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    yield return metaObjectStub;
                }
            }
        }

        private string GetQualifiedName(int nProjectHandle, Guid appGuid)
        {
            return ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(nProjectHandle, appGuid);
        }
    }
}
