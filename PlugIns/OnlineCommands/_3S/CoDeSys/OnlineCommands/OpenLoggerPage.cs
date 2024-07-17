#define DEBUG
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceEditor;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{D0B4FF00-397F-48ac-82B6-2C2FD4B63579}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_edt_device_log.htm")]
    [AssociatedOnlineHelpTopic("codesys.chm::/Log_device.htm")]
    public class OpenLoggerPage : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "open_logger_page" };

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => Strings.OpenLoggerPage_Name;

        public string Description => Strings.OpenLoggerPage_Descr;

        public string ToolTipText => Description;

        public Icon SmallIcon => null;

        public Icon LargeIcon => null;

        public bool Enabled => OnlineCommandHelper.GetActiveDevice() != null;

        public string[] BatchCommand => BATCH_COMMAND;

        public void AddedToUI()
        {
        }

        public void RemovedFromUI()
        {
        }

        public bool IsVisible(bool bContextMenu)
        {
            if (((IEngine)APEnvironment.Engine).Frame != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                return ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication != Guid.Empty;
            }
            return false;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_001e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0036: Unknown result type (might be due to invalid IL or missing references)
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length > 1)
            {
                throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 0);
            }
            if (((IEngine)APEnvironment.Engine).Frame == null)
            {
                throw new BatchInteractiveException(BatchCommand);
            }
            IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
            if (activeDevice != null)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IObject)activeDevice).MetaObject.ProjectHandle, ((IObject)activeDevice).MetaObject.ObjectGuid);
                Debug.Assert(metaObjectStub != null);
                Guid defaultEditorViewFactory = GetDefaultEditorViewFactory(metaObjectStub);
                IEditorView val = ((IEngine)APEnvironment.Engine).Frame.OpenEditorView(metaObjectStub, defaultEditorViewFactory, (string)null);
                if (val != null && val.Editor != null && val.Editor is IEditorTabManager)
                {
                    IEditor editor = val.Editor;
                    ((IEditorTabManager)((editor is IEditorTabManager) ? editor : null)).SelectEditorTab(GetLogPageName());
                }
            }
        }

        private string GetLogPageName()
        {
            try
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;
                if (ComponentManager.Singleton.SameStartupCultureAsOS && !cultureInfo.Equals(ComponentManager.Singleton.SpecificStartupCulture))
                {
                    cultureInfo = ComponentManager.Singleton.SpecificStartupCulture;
                }
                switch (cultureInfo.Name.Substring(0, 2).ToUpperInvariant())
                {
                    case "DE":
                    case "EN":
                    case "IT":
                        return "Log";
                    case "ES":
                        return "Registro";
                    case "FR":
                        return "Journal";
                    case "ZH":
                        return "日志";
                    default:
                        return "Log";
                }
            }
            catch
            {
            }
            return "Log";
        }

        private static Guid GetDefaultEditorViewFactory(IMetaObjectStub mos)
        {
            Debug.Assert(mos != null);
            Debug.Assert(((IEngine)APEnvironment.Engine).Frame != null);
            Type objectType = mos.ObjectType;
            Type[] embeddedObjectTypes = mos.EmbeddedObjectTypes;
            return ((IEngine)APEnvironment.Engine).Frame.ViewFactoryManager.GetDefaultEditorViewFactory(objectType, embeddedObjectTypes);
        }

        public string[] CreateBatchArguments()
        {
            return new string[0];
        }
    }
}
