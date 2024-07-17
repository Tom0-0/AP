using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceObject;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Xml;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{AB1F8435-CE2E-4a75-AF9E-26E3DFBD9E2C}")]
    [SuppressMessage("AP.Patterns", "Pat002", Justification = "No F1 help available")]
    public class LogoffUserCommand : IStandardCommand, ICommand
    {
        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "logoffuser" };

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => Strings.LogoffUser_Name;

        public string Description => Strings.LogoffUser_Description;

        public string ToolTipText => Name;

        public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(typeof(LogoffUserCommand), "_3S.CoDeSys.OnlineCommands.Resources.LogoffUserSmall.ico");

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                //IL_002f: Unknown result type (might be due to invalid IL or missing references)
                IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
                if (activeDevice == null)
                {
                    return false;
                }
                IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
                if (onlineDevice == null)
                {
                    return false;
                }
                if (onlineDevice is IOnlineDevice6 && string.IsNullOrEmpty(((IOnlineDevice6)onlineDevice).LoggedOnUsername))
                {
                    return false;
                }
                return true;
            }
        }

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
            if (bContextMenu)
            {
                return false;
            }
            return true;
        }

        public void ExecuteBatch(string[] arguments)
        {
            //IL_0064: Unknown result type (might be due to invalid IL or missing references)
            //IL_0081: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a5: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d2: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ff: Unknown result type (might be due to invalid IL or missing references)
            //IL_012c: Unknown result type (might be due to invalid IL or missing references)
            IDeviceObject activeDevice = OnlineCommandHelper.GetActiveDevice();
            if (activeDevice == null)
            {
                return;
            }
            IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
            if (onlineDevice == null)
            {
                return;
            }
            Guid activeAppObjectGuid = OnlineCommandHelper.ActiveAppObjectGuid;
            if (activeAppObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = OnlineCommandHelper.GetApplication(activeAppObjectGuid);
                if (application != null && application.IsLoggedIn)
                {
                    application.Logout();
                }
            }
            if (onlineDevice.IsConnected)
            {
                if (onlineDevice is IOnlineDevice11)
                {
                    ((IOnlineDevice11)onlineDevice).ForceDisconnect();
                }
                else
                {
                    onlineDevice.Disconnect();
                }
            }
            string value = null;
            if (onlineDevice is IOnlineDevice6)
            {
                value = ((IOnlineDevice6)onlineDevice).LoggedOnUsername;
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
            }
            else if (onlineDevice is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)onlineDevice).IsFunctionAvailable("GetLoggedOnUsername"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
                value = ((IGenericInterfaceExtensionProvider)onlineDevice).CallFunction("GetLoggedOnUsername", xmlDocument).DocumentElement.InnerText;
            }
            if (!string.IsNullOrEmpty(value) && onlineDevice is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)onlineDevice).IsFunctionAvailable("ResetLoggedOnUser"))
            {
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.AppendChild(xmlDocument2.CreateElement("Input"));
                ((IGenericInterfaceExtensionProvider)onlineDevice).CallFunction("ResetLoggedOnUser", xmlDocument2);
            }
        }
    }
}
