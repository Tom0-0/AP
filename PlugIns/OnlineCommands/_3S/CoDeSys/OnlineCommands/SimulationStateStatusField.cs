using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Drawing;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{3BE85B50-4F01-4504-A546-54D477689813}")]
    public class SimulationStateStatusField : ITextStatusField, IStatusField
    {
        private static bool s_bSimulationMode = false;

        private static bool s_bAttachedToEvents = false;

        private static Guid s_guidDeviceObjectToListen = Guid.Empty;

        private static bool s_bReentranceBreak = false;

        private static int nTest = 80;

        private bool SimulationMode
        {
            get
            {
                bAttachToEvents();
                return s_bSimulationMode;
            }
        }

        public string Text => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "Simulation_Text");

        public Color ForeColor => SystemColors.ControlText;

        public Color BackColor => Color.Red;

        public int Width => nTest;

        public bool Visible => SimulationMode;

        public ICommand DoubleClickCommand => null;

        private void bAttachToEvents()
        {
            //IL_0016: Unknown result type (might be due to invalid IL or missing references)
            //IL_0020: Expected O, but got Unknown
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0036: Expected O, but got Unknown
            //IL_0042: Unknown result type (might be due to invalid IL or missing references)
            //IL_004c: Expected O, but got Unknown
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0067: Expected O, but got Unknown
            //IL_0073: Unknown result type (might be due to invalid IL or missing references)
            //IL_007d: Expected O, but got Unknown
            //IL_0089: Unknown result type (might be due to invalid IL or missing references)
            //IL_0093: Expected O, but got Unknown
            if (!s_bAttachedToEvents)
            {
                ((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoving += (new ObjectCancelEventHandler(ObjectMgr_ObjectRemoving));
                ((IObjectManager)APEnvironment.ObjectMgr).ObjectLoaded += (new ObjectEventHandler(ObjectMgr_ObjectLoaded));
                ((IObjectManager)APEnvironment.ObjectMgr).ObjectModified += (new ObjectModifiedEventHandler(ObjectMgr_ObjectModified));
                ((IEngine)APEnvironment.Engine).Projects.PrimaryProjectSwitched += (new PrimaryProjectSwitchedEventHandler(Projects_PrimaryProjectSwitched));
                ((IObjectManager)APEnvironment.ObjectMgr).ProjectClosing += (new ProjectClosingEventHandler(ObjectMgr_ProjectClosing));
                APEnvironment.OptionStorage.OptionChanged += (new OptionEventHandler(OptionStorage_OptionChanged));
                s_bAttachedToEvents = true;
            }
        }

        private void OptionStorage_OptionChanged(object sender, OptionEventArgs e)
        {
            newProject_ActiveApplicationChanged(null);
        }

        private void ObjectMgr_ProjectClosing(object sender, ProjectClosingEventArgs e)
        {
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && e.ProjectHandle == ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle)
            {
                DetachFromDeviceObject();
            }
        }

        private void ObjectMgr_ObjectLoaded(object sender, ObjectEventArgs e)
        {
            if (s_guidDeviceObjectToListen == Guid.Empty)
            {
                newProject_ActiveApplicationChanged(null);
            }
        }

        private void ObjectMgr_ObjectModified(object sender, ObjectModifiedEventArgs e)
        {
            _ = s_guidDeviceObjectToListen;
            if (e.ObjectGuid == s_guidDeviceObjectToListen)
            {
                newProject_ActiveApplicationChanged(null);
            }
        }

        private void ObjectMgr_ObjectRemoving(object sender, ObjectCancelEventArgs e)
        {
            _ = s_guidDeviceObjectToListen;
            if (e.ObjectGuid == s_guidDeviceObjectToListen)
            {
                DetachFromDeviceObject();
            }
        }

        private void Projects_PrimaryProjectSwitched(IProject oldProject, IProject newProject)
        {
            //IL_000b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0015: Expected O, but got Unknown
            //IL_0026: Unknown result type (might be due to invalid IL or missing references)
            //IL_0030: Expected O, but got Unknown
            if (oldProject != null)
            {
                oldProject.ActiveApplicationChanged -= (new ProjectChangedEventHandler(newProject_ActiveApplicationChanged));
                DetachFromDeviceObject();
            }
            if (newProject != null)
            {
                newProject.ActiveApplicationChanged += (new ProjectChangedEventHandler(newProject_ActiveApplicationChanged));
                newProject_ActiveApplicationChanged(newProject);
            }
        }

        private void DetachFromDeviceObject()
        {
            s_guidDeviceObjectToListen = Guid.Empty;
            s_bSimulationMode = false;
        }

        private void newProject_ActiveApplicationChanged(IProject project)
        {
            if (s_bReentranceBreak)
            {
                return;
            }
            try
            {
                s_bReentranceBreak = true;
                s_bSimulationMode = false;
                if (!(OnlineCommandHelper.ActiveAppObjectGuid != Guid.Empty) || !((IObjectManager2)APEnvironment.ObjectMgr).IsObjectLoaded(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, OnlineCommandHelper.ActiveAppObjectGuid))
                {
                    return;
                }
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, OnlineCommandHelper.ActiveAppObjectGuid);
                if (objectToRead == null)
                {
                    return;
                }
                IObject @object = objectToRead.Object;
                IApplicationObject val = (IApplicationObject)(object)((@object is IApplicationObject) ? @object : null);
                if (val == null || ((IOnlineApplicationObject)val).DeviceGuid == Guid.Empty)
                {
                    return;
                }
                IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, ((IOnlineApplicationObject)val).DeviceGuid);
                if (objectToRead2 != null)
                {
                    IObject object2 = objectToRead2.Object;
                    IDeviceObject5 val2 = (IDeviceObject5)(object)((object2 is IDeviceObject5) ? object2 : null);
                    if (val2 != null)
                    {
                        DetachFromDeviceObject();
                        s_bSimulationMode = ((IDeviceObject4)val2).SimulationMode;
                        s_guidDeviceObjectToListen = objectToRead2.ObjectGuid;
                    }
                }
            }
            finally
            {
                s_bReentranceBreak = false;
            }
        }
    }
}
