#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Security;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceCommunicationEditor;
using _3S.CoDeSys.DeviceEditor;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.ProjectArchive;
using _3S.CoDeSys.ProjectInfoObject;
using _3S.CoDeSys.PropertyObject;
using _3S.CoDeSys.UserManagement;
using _3S.CoDeSys.Utilities;
using _3S.CoDeSys.VisualObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace _3S.CoDeSys.OnlineCommands
{
    internal abstract class OnlineCommandHelper
    {
        internal class UnknownLoginErrorEventArgs : EventArgs
        {
            public Guid ApplicationGuid;

            public UnknownLoginErrorEventArgs(Guid applicationGuid)
            {
                ApplicationGuid = applicationGuid;
            }
        }

        private class OnlineChangeEventArgs : EventArgs
        {
            public IOnlineChangeDetails2 _ocd;

            public ICompileContext7 _comconNew;

            public OnlineChangeEventArgs(IOnlineChangeDetails2 ocd, ICompileContext7 comconNew)
            {
                _ocd = ocd;
                _comconNew = comconNew;
            }
        }

        internal const int SAFETY_PLC = 4098;

        private static readonly Guid GUID_MESSAGEVIEWFACTORY = new Guid("{0EABD42C-7E07-400b-A13F-644ED86551BC}");

        private static readonly Guid GUID_APPLICATIONINFOPROPERTY = new Guid("{86B3EDF4-9DCC-4f07-85B0-CCB2D559C8B0}");

        private static readonly Guid GUID_COMPILE_INFO_ARCHIVE = new Guid("{B0B53F83-AF78-49aa-8133-0063F476BD7C}");

        private static object s_onlappInfoObject = null;

        private static object s_onlappContentObject = null;

        internal static readonly Version RTS_VERSION_35160 = new Version(3, 5, 16, 0);

        private static IGateway _enumGateway;

        public static Guid ActiveAppObjectGuid
        {
            get
            {
                try
                {
                    IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                    return (primaryProject != null) ? primaryProject.ActiveApplication : Guid.Empty;
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        public static string ActiveAppName
        {
            get
            {
                try
                {
                    IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                    if (primaryProject != null)
                    {
                        int handle = primaryProject.Handle;
                        Guid activeApplication = primaryProject.ActiveApplication;
                        if (activeApplication != Guid.Empty)
                        {
                            return ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(handle, activeApplication);
                        }
                    }
                    return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
                }
                catch
                {
                    return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
                }
            }
        }

        private static PromptResult Prompt(string stResourceKey, PromptChoice choice, PromptResult defaultResult, bool bApplicationExistsOnDevice, IOnlineApplication onlineApplicationForDetails, params object[] args)
        {
            string text = string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), stResourceKey), args);
            if (onlineApplicationForDetails != null && ((IEngine)APEnvironment.Engine).MessageService is IMessageService5)
            {
                return ((IMessageService5)((IEngine)APEnvironment.Engine).MessageService).PromptWithDetails(text, choice, defaultResult, (EventHandler)DetailsClickHandler, (EventArgs)new DetailsEventArgs(bApplicationExistsOnDevice, onlineApplicationForDetails), stResourceKey, args);
            }
            if (((IEngine)APEnvironment.Engine).MessageService is IMessageService3)
            {
                return ((IMessageService3)((IEngine)APEnvironment.Engine).MessageService).Prompt(text, choice, defaultResult, stResourceKey, args);
            }
            return APEnvironment.MessageService.Prompt(text, choice, defaultResult, stResourceKey, Array.Empty<object>());
        }

        private static PromptResult Prompt(string stResourceKey, object[] items, PromptChoice choice, PromptResult defaultResult, out bool[] selection, params object[] args)
        {
            string stMessage = string.Format(APEnvironment.Engine.ResourceManager.GetString(typeof(Strings), stResourceKey), args);
            IMessageService3 messageService = APEnvironment.Engine.MessageService as IMessageService3;
            if (messageService != null)
            {
                return messageService.Prompt(stMessage, items, choice, defaultResult, out selection, stResourceKey, args);
            }
            return APEnvironment.MessageService.Prompt(stMessage, items, choice, defaultResult, out selection, stResourceKey, Array.Empty<object>());
        }

        private static PromptOnlineChangeResult PromptOnlineChangeWithForcedVars(IOnlineApplication onlappl, out BootProjectTransferMode transferMode)
        {
            transferMode = (BootProjectTransferMode)0;
            if (((IEngine)APEnvironment.Engine).MessageService is IMessageService5)
            {
                string[] array = null;
                array = ((!CheckLoginWithoutChangeAvailability(onlappl.OnlineDevice)) ? new string[2]
                {
                    Strings.QueryOnlChangeWithForceVars_LoginOnlineChange,
                    Strings.QueryOnlChangeWithForceVars_LoginDownload
                } : new string[3]
                {
                    Strings.QueryOnlChangeWithForceVars_LoginOnlineChange,
                    Strings.QueryOnlChangeWithForceVars_LoginDownload,
                    Strings.QueryOnlChangeWithForceVars_Login
                });
                int num = -1;
                if (((IEngine)APEnvironment.Engine).MessageService is IMessageService6)
                {
                    bool bUpdateBootApplication = false;
                    bool bInitialOnlineChange = false;
                    bool bInitialFullDownload = false;
                    IOnlineApplicationObject onlineApplicationObject = GetOnlineApplicationObject0(onlappl.ApplicationGuid);
                    IOnlineApplicationObject5 val = (IOnlineApplicationObject5)(object)((onlineApplicationObject is IOnlineApplicationObject5) ? onlineApplicationObject : null);
                    if (val != null && val.TargetSupportsBootApplicationOnDownload)
                    {
                        transferMode = (BootProjectTransferMode)2;
                        bInitialOnlineChange = val.BootApplicationSettings.CreateBootApplicationOnOnlineChange;
                        bInitialFullDownload = val.BootApplicationSettings.CreateBootApplicationOnDownload;
                    }
                    num = PromptOnlineChangeWithCustomControls(Strings.QueryOnlChangeWithForceVars, array, "InfoNoOnlineChangeWithForcedVars_WithCustomControl", DetailsClickHandler, new DetailsEventArgs(bApplicationExistsOnDevice: true, onlappl), bInitialOnlineChange, bInitialFullDownload, transferMode, out bUpdateBootApplication);
                    if (transferMode != BootProjectTransferMode.UseProjectSetting)
                    {
                        if (bUpdateBootApplication)
                        {
                            transferMode = (BootProjectTransferMode)2;
                        }
                        else
                        {
                            transferMode = (BootProjectTransferMode)1;
                        }
                    }
                }
                else
                {
                    num = ((IMessageService5)((IEngine)APEnvironment.Engine).MessageService).MultipleChoicePromptWithDetails(Strings.QueryOnlChangeWithForceVars, array, 0, true, (EventHandler)DetailsClickHandler, (EventArgs)new DetailsEventArgs(bApplicationExistsOnDevice: true, onlappl), "InfoNoOnlineChangeWithForcedVars", Array.Empty<object>());
                }
                switch (num)
                {
                    case 0:
                        onlappl.ReleaseForceValues();
                        return (PromptOnlineChangeResult)0;
                    case 1:
                        onlappl.ReleaseForceValues();
                        return (PromptOnlineChangeResult)1;
                    case 2:
                        return (PromptOnlineChangeResult)2;
                    default:
                        return (PromptOnlineChangeResult)3;
                }
            }
            if (((IEngine)APEnvironment.Engine).MessageService is IMessageService4)
            {
                string[] array2 = null;
                if (CheckLoginWithoutChangeAvailability(onlappl.OnlineDevice))
                {
                    array2 = new string[3]
                    {
                        Strings.QueryOnlChangeWithForceVars_LoginOnlineChange,
                        Strings.QueryOnlChangeWithForceVars_LoginDownload,
                        Strings.QueryOnlChangeWithForceVars_Login
                    };
                }
                if (CheckLoginWithoutChangeAvailability(onlappl.OnlineDevice))
                {
                    array2 = new string[2]
                    {
                        Strings.QueryOnlChangeWithForceVars_LoginOnlineChange,
                        Strings.QueryOnlChangeWithForceVars_LoginDownload
                    };
                }
                switch (((IMessageService4)((IEngine)APEnvironment.Engine).MessageService).MultipleChoicePrompt(Strings.QueryOnlChangeWithForceVars, array2, 0, true, "InfoNoOnlineChangeWithForcedVars", Array.Empty<object>()))
                {
                    case 0:
                        onlappl.ReleaseForceValues();
                        return (PromptOnlineChangeResult)0;
                    case 1:
                        onlappl.ReleaseForceValues();
                        return (PromptOnlineChangeResult)1;
                    case 2:
                        return (PromptOnlineChangeResult)2;
                    default:
                        return (PromptOnlineChangeResult)3;
                }
            }
            if (((IEngine)APEnvironment.Engine).MessageService is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)((IEngine)APEnvironment.Engine).MessageService).IsFunctionAvailable("MultipleChoicePrompt"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("stMessage"));
                xmlDocument.DocumentElement["stMessage"].InnerText = Strings.QueryOnlChangeWithForceVars;
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("options"));
                XmlElement xmlElement = xmlDocument.CreateElement("option");
                xmlElement.InnerText = Strings.QueryOnlChangeWithForceVars_LoginOnlineChange;
                xmlDocument.DocumentElement["options"].AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("option");
                xmlElement.InnerText = Strings.QueryOnlChangeWithForceVars_LoginDownload;
                xmlDocument.DocumentElement["options"].AppendChild(xmlElement);
                if (CheckLoginWithoutChangeAvailability(onlappl.OnlineDevice))
                {
                    xmlElement = xmlDocument.CreateElement("option");
                    xmlElement.InnerText = Strings.QueryOnlChangeWithForceVars_Login;
                    xmlDocument.DocumentElement["options"].AppendChild(xmlElement);
                }
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("nInitialSelection"));
                xmlDocument.DocumentElement["nInitialSelection"].InnerText = XmlConvert.ToString(0);
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("bCancellable"));
                xmlDocument.DocumentElement["bCancellable"].InnerText = XmlConvert.ToString(value: true);
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("stMessageKey"));
                xmlDocument.DocumentElement["stMessageKey"].InnerText = "QueryOnlChange2";
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("messageArguments"));
                switch (XmlConvert.ToInt32(((IGenericInterfaceExtensionProvider)((IEngine)APEnvironment.Engine).MessageService).CallFunction("MultipleChoicePrompt", xmlDocument).DocumentElement.InnerText))
                {
                    case 0:
                        onlappl.ReleaseForceValues();
                        return (PromptOnlineChangeResult)0;
                    case 1:
                        onlappl.ReleaseForceValues();
                        return (PromptOnlineChangeResult)1;
                    case 2:
                        return (PromptOnlineChangeResult)2;
                    default:
                        return (PromptOnlineChangeResult)3;
                }
            }
            onlappl.ReleaseForceValues();
            return PromptOnlineChange(onlappl);
        }

        internal static PromptResult PromptOutdatedDevdesc(IOnlineDevice onlineDevice)
        {
            PromptResult val = (PromptResult)0;
            IOnlineDevice11 val2 = (IOnlineDevice11)(object)((onlineDevice is IOnlineDevice11) ? onlineDevice : null);
            if (val2 != null && ((IOnlineDevice)val2).IsConnected && !ProjectOptionsHelper.IgnoreOutdatedDevdescsInTheFuture)
            {
                try
                {
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IOnlineDevice10)val2).DeviceObjectGuid).Object;
                    IDeviceObject4 val3 = (IDeviceObject4)(object)((@object is IDeviceObject4) ? @object : null);
                    if (val3 == null)
                    {
                        return val;
                    }
                    if (val3.SimulationMode)
                    {
                        return val;
                    }
                    Version version = new Version(((IDeviceObject)val3).DeviceIdentification.Version);
                    string text = default(string);
                    IDeviceIdentification targetIdent = val2.GetTargetIdent(out text, out text, out text);
                    if (targetIdent == null)
                    {
                        return val;
                    }
                    if (string.IsNullOrEmpty(targetIdent.Version))
                    {
                        return val;
                    }
                    if (!(new Version(targetIdent.Version) > version))
                    {
                        return val;
                    }
                    string text2 = string.Format(Strings.PromptOutdatedDevdesc, ((IDeviceObject)val3).DeviceIdentification.Version, targetIdent.Version);
                    if (APEnvironment.OutdatedDevdescHandlerOrNull == null)
                    {
                        if (!(APEnvironment.MessageService is IMessageServiceWithKeys2))
                        {
                            val = APEnvironment.MessageService.Prompt(text2, (PromptChoice)1, (PromptResult)1, "PromptOutdatedDevdesc", Array.Empty<object>());
                            return val;
                        }
                        OutdatedDevdescCustomControl outdatedDevdescCustomControl = new OutdatedDevdescCustomControl();
                        val = ((IMessageServiceWithKeys2)APEnvironment.MessageService).PromptWithCustomControls(text2, (ICustomControlProvider)(object)outdatedDevdescCustomControl, (PromptChoice)1, (PromptResult)0, (EventHandler)null, (EventArgs)null, "PromptOutdatedDevdesc", Array.Empty<object>());
                        if ((int)val != 0)
                        {
                            return val;
                        }
                        ProjectOptionsHelper.IgnoreOutdatedDevdescsInTheFuture = outdatedDevdescCustomControl.IgnoreInTheFuture;
                        ProjectOptionsHelper.SaveOptions();
                        return val;
                    }
                    val = APEnvironment.OutdatedDevdescHandlerOrNull.HandleOutdatedDevdesc(text2, ((IDeviceObject)val3).DeviceIdentification, targetIdent);
                    return val;
                }
                catch
                {
                    return val;
                }
            }
            return val;
        }

        private static bool CheckLoginWithoutChangeAvailability(IOnlineDevice onlineDevice)
        {
            //IL_0034: Unknown result type (might be due to invalid IL or missing references)
            bool result = true;
            if (onlineDevice != null && onlineDevice is IOnlineDevice10 && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
            {
                try
                {
                    int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                    Guid deviceObjectGuid = ((IOnlineDevice10)onlineDevice).DeviceObjectGuid;
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, deviceObjectGuid).Object;
                    IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
                    if (val == null)
                    {
                        return result;
                    }
                    ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentification);
                    result = LocalTargetSettings.LoginWithOutdatedCodeAllowed.GetBoolValue(targetSettingsById);
                    return result;
                }
                catch
                {
                    return result;
                }
            }
            return result;
        }

        private static PromptOnlineChangeResult PromptOnlineChange(IOnlineApplication onlineApplicationForDetails)
        {
            //IL_0003: Unknown result type (might be due to invalid IL or missing references)
            BootProjectTransferMode transferMode;
            return PromptOnlineChange(onlineApplicationForDetails, out transferMode);
        }

        private static PromptOnlineChangeResult PromptOnlineChange(IOnlineApplication onlineApplicationForDetails, out BootProjectTransferMode transferMode)
        {
            transferMode = (BootProjectTransferMode)0;
            if (onlineApplicationForDetails != null && ((IEngine)APEnvironment.Engine).MessageService is IMessageService5)
            {
                IList<IOnlineApplicationObject> childApplicationObjects = GetChildApplicationObjects(onlineApplicationForDetails.ApplicationGuid);
                bool flag = false;
                if (childApplicationObjects.Count > 0)
                {
                    string[] array = onlineApplicationForDetails.OnlineDevice.ReadApplicationList();
                    foreach (IOnlineApplicationObject item in childApplicationObjects)
                    {
                        if (!typeof(IApplicationObject).IsAssignableFrom(((object)item).GetType()))
                        {
                            continue;
                        }
                        string[] array2 = array;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            if (string.Equals(array2[i], ((IOnlineApplicationObject2)((item is IOnlineApplicationObject2) ? item : null)).ApplicationName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                List<string> list = new List<string>();
                if (flag)
                {
                    list.Add(Strings.QueryOnlChange3_LoginOnlineChange);
                    list.Add(Strings.QueryOnlChange3_LoginDownload);
                }
                else
                {
                    list.Add(Strings.QueryOnlChange2_LoginOnlineChange);
                    list.Add(Strings.QueryOnlChange2_LoginDownload);
                }
                if (CheckLoginWithoutChangeAvailability(onlineApplicationForDetails.OnlineDevice))
                {
                    list.Add(Strings.QueryOnlChange2_Login);
                }
                int num = -1;
                if (((IEngine)APEnvironment.Engine).MessageService is IMessageService6)
                {
                    bool bUpdateBootApplication = false;
                    bool bInitialOnlineChange = false;
                    bool bInitialFullDownload = false;
                    IOnlineApplicationObject onlineApplicationObject = GetOnlineApplicationObject0(onlineApplicationForDetails.ApplicationGuid);
                    IOnlineApplicationObject5 val = (IOnlineApplicationObject5)(object)((onlineApplicationObject is IOnlineApplicationObject5) ? onlineApplicationObject : null);
                    if (val != null && val.TargetSupportsBootApplicationOnDownload)
                    {
                        transferMode = (BootProjectTransferMode)2;
                        bInitialOnlineChange = val.BootApplicationSettings.CreateBootApplicationOnOnlineChange;
                        bInitialFullDownload = val.BootApplicationSettings.CreateBootApplicationOnDownload;
                    }
                    num = PromptOnlineChangeWithCustomControls(Strings.QueryOnlChange2, list.ToArray(), "QueryOnlChange2_WithCustomControl", DetailsClickHandler, new DetailsEventArgs(bApplicationExistsOnDevice: true, onlineApplicationForDetails), bInitialOnlineChange, bInitialFullDownload, transferMode, out bUpdateBootApplication);
                    if (transferMode != BootProjectTransferMode.UseProjectSetting)
                    {
                        if (bUpdateBootApplication)
                        {
                            transferMode = (BootProjectTransferMode)2;
                        }
                        else
                        {
                            transferMode = (BootProjectTransferMode)1;
                        }
                    }
                }
                else
                {
                    num = ((IMessageService5)((IEngine)APEnvironment.Engine).MessageService).MultipleChoicePromptWithDetails(Strings.QueryOnlChange2, list.ToArray(), 0, true, (EventHandler)DetailsClickHandler, (EventArgs)new DetailsEventArgs(bApplicationExistsOnDevice: true, onlineApplicationForDetails), "QueryOnlChange2", Array.Empty<object>());
                }
                return (PromptOnlineChangeResult)(num switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 2,
                    _ => 3,
                });
            }
            if (((IEngine)APEnvironment.Engine).MessageService is IMessageService4)
            {
                string[] array3 = null;
                array3 = ((onlineApplicationForDetails == null) ? new string[3]
                {
                    Strings.QueryOnlChange2_LoginOnlineChange,
                    Strings.QueryOnlChange2_LoginDownload,
                    Strings.QueryOnlChange2_Login
                } : ((!CheckLoginWithoutChangeAvailability(onlineApplicationForDetails.OnlineDevice)) ? new string[2]
                {
                    Strings.QueryOnlChange2_LoginOnlineChange,
                    Strings.QueryOnlChange2_LoginDownload
                } : new string[3]
                {
                    Strings.QueryOnlChange2_LoginOnlineChange,
                    Strings.QueryOnlChange2_LoginDownload,
                    Strings.QueryOnlChange2_Login
                }));
                return (PromptOnlineChangeResult)(((IMessageService4)((IEngine)APEnvironment.Engine).MessageService).MultipleChoicePrompt(Strings.QueryOnlChange2, array3, 0, true, "QueryOnlChange2", Array.Empty<object>()) switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 2,
                    _ => 3,
                });
            }
            if (((IEngine)APEnvironment.Engine).MessageService is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)((IEngine)APEnvironment.Engine).MessageService).IsFunctionAvailable("MultipleChoicePrompt"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("stMessage"));
                xmlDocument.DocumentElement["stMessage"].InnerText = Strings.QueryOnlChange2;
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("options"));
                XmlElement xmlElement = xmlDocument.CreateElement("option");
                xmlElement.InnerText = Strings.QueryOnlChange2_LoginOnlineChange;
                xmlDocument.DocumentElement["options"].AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("option");
                xmlElement.InnerText = Strings.QueryOnlChange2_LoginDownload;
                xmlDocument.DocumentElement["options"].AppendChild(xmlElement);
                if (CheckLoginWithoutChangeAvailability(onlineApplicationForDetails.OnlineDevice))
                {
                    xmlElement = xmlDocument.CreateElement("option");
                    xmlElement.InnerText = Strings.QueryOnlChange2_Login;
                    xmlDocument.DocumentElement["options"].AppendChild(xmlElement);
                }
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("nInitialSelection"));
                xmlDocument.DocumentElement["nInitialSelection"].InnerText = XmlConvert.ToString(0);
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("bCancellable"));
                xmlDocument.DocumentElement["bCancellable"].InnerText = XmlConvert.ToString(value: true);
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("stMessageKey"));
                xmlDocument.DocumentElement["stMessageKey"].InnerText = "QueryOnlChange2";
                xmlDocument.DocumentElement.AppendChild(xmlDocument.CreateElement("messageArguments"));
                return (PromptOnlineChangeResult)(XmlConvert.ToInt32(((IGenericInterfaceExtensionProvider)((IEngine)APEnvironment.Engine).MessageService).CallFunction("MultipleChoicePrompt", xmlDocument).DocumentElement.InnerText) switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 2,
                    _ => 3,
                });
            }
            PromptResult val2 = Prompt("QueryOnlChange", (PromptChoice)3, (PromptResult)2, true, onlineApplicationForDetails);
            return (PromptOnlineChangeResult)(((int)val2 - 1) switch
            {
                1 => 0,
                2 => 2,
                _ => 3,
            });
        }

        private static int PromptOnlineChangeWithCustomControls(string stMessage, string[] stOptions, string stMessageKey, EventHandler eventHandler, EventArgs detailsEventArgs, bool bInitialOnlineChange, bool bInitialFullDownload, BootProjectTransferMode transferMode, out bool bUpdateBootApplication)
        {
            bUpdateBootApplication = true;
            int result = -1;
            OnlineChangeCustomControl onlineChangeCustomControl = new OnlineChangeCustomControl();
            onlineChangeCustomControl.Initialize(stOptions, bInitialOnlineChange, bInitialFullDownload, transferMode);
            if ((int)((IMessageService6)((IEngine)APEnvironment.Engine).MessageService).PromptWithCustomControls(Strings.QueryOnlChange2, (ICustomControlProvider)(object)onlineChangeCustomControl, (PromptChoice)1, (PromptResult)0, eventHandler, detailsEventArgs, stMessageKey, Array.Empty<object>()) == 0)
            {
                bUpdateBootApplication = onlineChangeCustomControl.DownloadBootApp;
                result = onlineChangeCustomControl.SelectedOnlineChangeOption;
            }
            return result;
        }

        public static string GetActiveDeviceName()
        {
            if (APEnvironment.ObjectMgr == null || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
            }
            if (ActiveAppObjectGuid == Guid.Empty)
            {
                return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
            }
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ActiveAppObjectGuid);
            if (objectToRead == null)
            {
                return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
            }
            IObject @object = objectToRead.Object;
            IApplicationObject val = (IApplicationObject)(object)((@object is IApplicationObject) ? @object : null);
            if (val == null)
            {
                return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
            }
            IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, ((IOnlineApplicationObject)val).DeviceGuid);
            if (objectToRead2 == null)
            {
                return ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "None");
            }
            return objectToRead2.Name;
        }

        public static IDeviceObject GetActiveDevice()
        {
            if (APEnvironment.ObjectMgr == null)
            {
                return null;
            }
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return null;
            }
            if (ActiveAppObjectGuid == Guid.Empty)
            {
                return null;
            }
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ActiveAppObjectGuid);
            if (objectToRead == null)
            {
                return null;
            }
            IObject @object = objectToRead.Object;
            IApplicationObject val = (IApplicationObject)(object)((@object is IApplicationObject) ? @object : null);
            if (val == null || ((IOnlineApplicationObject)val).DeviceGuid == Guid.Empty)
            {
                return null;
            }
            IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, ((IOnlineApplicationObject)val).DeviceGuid);
            if (objectToRead2 == null)
            {
                return null;
            }
            IObject object2 = objectToRead2.Object;
            return (IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null);
        }

        public static IDeviceObject GetSelectedDevice()
        {
            //IL_0087: Unknown result type (might be due to invalid IL or missing references)
            //IL_008d: Expected O, but got Unknown
            if (APEnvironment.Engine == null || APEnvironment.ObjectMgr == null)
            {
                return null;
            }
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return null;
            }
            ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
            if (selectedSVNodes == null || selectedSVNodes.Length != 1)
            {
                return null;
            }
            IMetaObjectStub metaObjectStub = selectedSVNodes[0].GetMetaObjectStub();
            if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
                Debug.Assert(objectToRead != null);
                return (IDeviceObject)objectToRead.Object;
            }
            return null;
        }

        internal static Guid GetSelectedObjectGuid<T>(out int projectHandle)
        {
            projectHandle = -1;
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject == null)
            {
                return Guid.Empty;
            }
            projectHandle = primaryProject.Handle;
            Guid result = Guid.Empty;
            if (primaryProject.SelectedSVNodes != null && primaryProject.SelectedSVNodes.Length == 1)
            {
                ISVNode val = primaryProject.SelectedSVNodes[0];
                if (typeof(T).IsAssignableFrom(val.ObjectType))
                {
                    result = val.ObjectGuid;
                }
            }
            return result;
        }

        internal static IEnumerable<Guid> GetSubObjects(int projectHandle, Guid parentGuid, Func<IMetaObjectStub, bool> predicate)
        {
            LQueue<Guid> guidsToTraverse = new LQueue<Guid>();
            guidsToTraverse.Enqueue(parentGuid);
            while (((IEnumerable<Guid>)guidsToTraverse).Any())
            {
                Guid guid = guidsToTraverse.Dequeue();
                if (guid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(projectHandle, guid))
                {
                    IMetaObjectStub mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, guid);
                    if (mos != null && predicate(mos))
                    {
                        yield return guid;
                    }
                    Guid[] subObjectGuids = mos.SubObjectGuids;
                    foreach (Guid guid2 in subObjectGuids)
                    {
                        guidsToTraverse.Enqueue(guid2);
                    }
                }
            }
        }

        internal static Guid GetSelectedApplicationGuid()
        {
            int projectHandle;
            return GetSelectedObjectGuid<IApplicationObject>(out projectHandle);
        }

        internal static Guid GetPLCLogicObjectGuidFromDeviceGuid(int projectHandle, Guid deviceGuid)
        {
            IEnumerable<Guid> subObjects = GetSubObjects(projectHandle, deviceGuid, (IMetaObjectStub mos) => typeof(IPlcLogicObject).IsAssignableFrom(mos.ObjectType));
            if (subObjects.Count() != 1)
            {
                return Guid.Empty;
            }
            return subObjects.First();
        }

        internal static Guid GetPLCLogicObjectGuidOfSelectedDevice(out int projectHandle)
        {
            Guid selectedObjectGuid = GetSelectedObjectGuid<IDeviceObject>(out projectHandle);
            if (selectedObjectGuid == Guid.Empty)
            {
                return Guid.Empty;
            }
            return GetPLCLogicObjectGuidFromDeviceGuid(projectHandle, selectedObjectGuid);
        }

        internal static Guid GetDeviceApplicationGuidOfSelectedDevice(out int projectHandle)
        {
            Guid pLCLogicObjectGuidOfSelectedDevice = GetPLCLogicObjectGuidOfSelectedDevice(out projectHandle);
            if (pLCLogicObjectGuidOfSelectedDevice == Guid.Empty)
            {
                return Guid.Empty;
            }
            Func<IMetaObjectStub, bool> predicate = (IMetaObjectStub mos) => typeof(IDeviceApplication).IsAssignableFrom(mos.ObjectType);
            return GetSubObjects(projectHandle, pLCLogicObjectGuidOfSelectedDevice, predicate).FirstOrDefault();
        }

        internal static Guid GetDeviceApplicationGuidOfSelectedDevice()
        {
            int projectHandle;
            return GetDeviceApplicationGuidOfSelectedDevice(out projectHandle);
        }

        internal static IEnumerable<Guid> GetDeviceAppSubApplicationsOfSelectedDevice()
        {
            int projectHandle;
            Guid deviceApplicationGuidOfSelectedDevice = GetDeviceApplicationGuidOfSelectedDevice(out projectHandle);
            if (!(deviceApplicationGuidOfSelectedDevice != Guid.Empty))
            {
                yield break;
            }
            foreach (Guid deviceAppSubApplication in GetDeviceAppSubApplications(projectHandle, deviceApplicationGuidOfSelectedDevice))
            {
                yield return deviceAppSubApplication;
            }
        }

        internal static IEnumerable<Guid> GetDeviceAppSubApplications(int projectHandle, Guid deviceAppGuid)
        {
            Func<IMetaObjectStub, bool> predicate = (IMetaObjectStub mos) => !typeof(IDeviceApplication).IsAssignableFrom(mos.ObjectType) && typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType);
            foreach (Guid subObject in GetSubObjects(projectHandle, deviceAppGuid, predicate))
            {
                yield return subObject;
            }
        }

        internal static Guid GetDeviceAppGuidOfApplication(Guid appGuid)
        {
            Guid result = Guid.Empty;
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            if (appGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, appGuid))
            {
                Guid parentObjectGuid = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, appGuid).ParentObjectGuid;
                if (parentObjectGuid != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, parentObjectGuid))
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, parentObjectGuid);
                    if (typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        result = parentObjectGuid;
                    }
                }
            }
            return result;
        }

        internal static void LogoutDeviceAppIfNecessary(Guid appGuid)
        {
            Guid deviceAppGuid = GetDeviceAppGuidOfApplication(appGuid);
            if (!(deviceAppGuid == Guid.Empty) && !(from app in GetAllOnlineApplications()
                                                    where app.ApplicationGuid != deviceAppGuid
                                                    where CanLogout(app.ApplicationGuid)
                                                    select app).Any() && CanLogout(deviceAppGuid))
            {
                try
                {
                    Logout(deviceAppGuid);
                }
                catch
                {
                }
            }
        }

        internal static Guid[] GetSelectedApplicationGuids()
        {
            List<Guid> list = new List<Guid>();
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject != null && primaryProject.SelectedSVNodes != null)
            {
                for (int i = 0; i < primaryProject.SelectedSVNodes.Length; i++)
                {
                    ISVNode val = primaryProject.SelectedSVNodes[i];
                    if (typeof(IApplicationObject).IsAssignableFrom(val.ObjectType))
                    {
                        list.Add(val.ObjectGuid);
                    }
                }
            }
            return list.ToArray();
        }

        internal static void RegisterEvents()
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0017: Expected O, but got Unknown
            //IL_0028: Unknown result type (might be due to invalid IL or missing references)
            //IL_0032: Expected O, but got Unknown
            //IL_003e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Expected O, but got Unknown
            try
            {
                ((IOnlineManager4)APEnvironment.OnlineMgr).SetProvideCredentialsHandler(new ProvideCredentialsHandler(OnProvideCredentials), false);
            }
            catch (InvalidOperationException)
            {
            }
            ((IObjectManager)APEnvironment.ObjectMgr).ProjectCreated += (new ProjectCreatedEventHandler(OnProjectCreated));
            ((IObjectManager)APEnvironment.ObjectMgr).ObjectRemoving += (new ObjectCancelEventHandler(OnObjectRemoving));
        }

        private static void OnObjectRemoving(object sender, ObjectCancelEventArgs e)
        {
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle != e.ProjectHandle)
            {
                return;
            }
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(e.ProjectHandle, e.ObjectGuid);
            Debug.Assert(metaObjectStub != null);
            List<Guid> list = new List<Guid>();
            CollectChildApplications(metaObjectStub, list);
            foreach (Guid item in list)
            {
                if (IsLoggedIn(item))
                {
                    string stMessage = string.Format(Strings.CannotRemoveDeviceOrApplicationWhichIsOnlineException, ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(e.ProjectHandle, e.ObjectGuid), ((IObjectManager)APEnvironment.ObjectMgr).GetFullName(e.ProjectHandle, item));
                    e.Cancel((Exception)(object)new CannotRemoveDeviceOrApplicationWhichIsOnlineException(stMessage, e.ProjectHandle, e.ObjectGuid, item));
                    break;
                }
            }
        }

        private static void OnProjectCreated(object sender, ProjectCreatedEventArgs e)
        {
            if (!APEnvironment.ProjectArchiveCategories.Any((IProjectArchiveCategory c) => TypeGuidAttribute.FromObject((object)c).Guid == GUID_COMPILE_INFO_ARCHIVE))
            {
                return;
            }
            Guid[] sourceDownloadContent = ProjectOptionsHelper.SourceDownloadContent2;
            if (sourceDownloadContent.Length == 0)
            {
                ProjectOptionsHelper.SourceDownloadContent2 = new Guid[1] { GUID_COMPILE_INFO_ARCHIVE };
                return;
            }
            List<Guid> list = new List<Guid>();
            Guid[] array = sourceDownloadContent;
            foreach (Guid guid in array)
            {
                if (guid == GUID_COMPILE_INFO_ARCHIVE)
                {
                    return;
                }
                list.Add(guid);
            }
            list.Add(GUID_COMPILE_INFO_ARCHIVE);
            ProjectOptionsHelper.SourceDownloadContent2 = list.ToArray();
        }

        private static void CollectChildApplications(IMetaObjectStub mos, List<Guid> appObjectGuids)
        {
            Debug.Assert(mos != null);
            Debug.Assert(appObjectGuids != null);
            if (typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType))
            {
                appObjectGuids.Add(mos.ObjectGuid);
            }
            Guid[] subObjectGuids = mos.SubObjectGuids;
            foreach (Guid guid in subObjectGuids)
            {
                CollectChildApplications(((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mos.ProjectHandle, guid), appObjectGuids);
            }
        }

        private static void OnProvideCredentials(object sender, IProvideCredentialsArgs pcArgs)
        {
            if ((int)pcArgs.SupportedCredentialTypes != 1)
            {
                return;
            }
            CredentialSourceKind val = (CredentialSourceKind)(-1);
            IProvideCredentialsArgs2 val2 = (IProvideCredentialsArgs2)(object)((pcArgs is IProvideCredentialsArgs2) ? pcArgs : null);
            if (val2 != null)
            {
                val = val2.PermittedSourceKinds;
            }
            IProvideDeviceCredentialsArgs val3 = (IProvideDeviceCredentialsArgs)(object)((pcArgs is IProvideDeviceCredentialsArgs) ? pcArgs : null);
            if (val3 == null)
            {
                pcArgs.Cancel((Exception)new OnlineManagerException("Authentication not supported"));
                return;
            }
            bool flag = true;
            IProvideCredentialsReusableArgs val4 = (IProvideCredentialsReusableArgs)(object)((pcArgs is IProvideCredentialsReusableArgs) ? pcArgs : null);
            if (val4 != null)
            {
                flag = val4.NumberOfReuses == 0;
            }
            if ((val3.SupportedCredentialTypes & CredentialsType.UserPassword) == CredentialsType.None)
            {
                _ = $"Authentication methods not supported (required: {(int)((IProvideCredentialsArgs)val3).SupportedCredentialTypes:X})";
                pcArgs.Cancel((Exception)new OnlineManagerException("Authentication method not supported"));
                return;
            }
            if (flag && ((IProvideCredentialsArgs)val3).Credentials == null && val3.DeviceObjectProjectHandle >= 0 && ((Enum)val).HasFlag((Enum)(object)(CredentialSourceKind)1))
            {
                IUserManagement userManagementOrNull = APEnvironment.UserManagementOrNull;
                if (userManagementOrNull != null)
                {
                    ISession session = userManagementOrNull.GetSession(val3.DeviceObjectProjectHandle);
                    ISession2 val5 = (ISession2)(object)((session is ISession2) ? session : null);
                    IUserList users = userManagementOrNull.GetUsers(val3.DeviceObjectProjectHandle);
                    if (val5 != null && users != null)
                    {
                        Guid loggedOnUserId = ((ISession)val5).LoggedOnUserId;
                        IUser val6 = users[loggedOnUserId];
                        string loggedOnPassword = val5.LoggedOnPassword;
                        if (val6 != null)
                        {
                            UserPasswordCredentials val7 = new UserPasswordCredentials();
                            val7.Username = (val6.Name);
                            val7.Password = (loggedOnPassword);
                            ((IProvideCredentialsArgs)val3).Credentials = ((Credentials)(object)val7);
                            if (val4 != null)
                            {
                                uint numberOfReuses = val4.NumberOfReuses;
                                val4.NumberOfReuses = (numberOfReuses + 1);
                            }
                            return;
                        }
                    }
                }
            }
            if (((Enum)val).HasFlag((Enum)(object)(CredentialSourceKind)2))
            {
                if (val2 != null && ((IProvideCredentialsArgs)val2).Credentials is UserPasswordCredentials && ((Enum)val).HasFlag((Enum)(object)(CredentialSourceKind)4))
                {
                    string username = ((UserPasswordCredentials)((IProvideCredentialsArgs)val2).Credentials).Username;
                    string password = null;
                    string oldPassword = null;
                    bool flag2 = true;
                    bool flag3 = true;
                    int num = 0;
                    bool flag4 = false;
                    string text = null;
                    string[] array = null;
                    bool flag5 = false;
                    IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(val3.DeviceObjectGuid);
                    IOnlineDevice17 val8 = (IOnlineDevice17)(object)((onlineDevice is IOnlineDevice17) ? onlineDevice : null);
                    if (val8 != null && val8.RuntimeSystemVersion >= RTS_VERSION_35160)
                    {
                        flag5 = true;
                    }
                    username = ((!flag5) ? ((IX509UIService3)APEnvironment.X509UIService4).ServeUserConfigurationDialog(username, array, Strings.PasswordRenewal, false, false, ref flag2, ref flag3, out password, out text, out num) : APEnvironment.X509UIService4.ServeUserConfigurationDialog(username, array, Strings.PasswordRenewal, false, false, true, ref flag2, ref flag3, out password, out oldPassword, out text, out num, out flag4));
                    if (string.IsNullOrEmpty(username))
                    {
                        pcArgs.Cancel((Exception)new CancelledByUserException());
                    }
                    else if (flag5)
                    {
                        UserPasswordCredentialsWithOldPw val9 = new UserPasswordCredentialsWithOldPw();
                        ((UserPasswordCredentials)val9).Username = (username);
                        ((UserPasswordCredentials)val9).Password = (password);
                        val9.OldPassword = (oldPassword);
                        ((IProvideCredentialsArgs)val3).Credentials = ((Credentials)(object)val9);
                    }
                    else
                    {
                        UserPasswordCredentials val10 = new UserPasswordCredentials();
                        val10.Username = (username);
                        val10.Password = (password);
                        ((IProvideCredentialsArgs)val3).Credentials = ((Credentials)(object)val10);
                    }
                    return;
                }
                using LoginDialog loginDialog = new LoginDialog();
                loginDialog.Initialize(val3);
                if (loginDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) != DialogResult.OK)
                {
                    pcArgs.Cancel((Exception)new CancelledByUserException());
                    return;
                }
                UserPasswordCredentials val11 = new UserPasswordCredentials();
                val11.Username = (loginDialog.Username);
                val11.Password = (loginDialog.Password);
                ((IProvideCredentialsArgs)val3).Credentials = ((Credentials)(object)val11);
            }
            else
            {
                pcArgs.Cancel((Exception)new OnlineManagerException($"No known permitted credential source kind: {val}"));
            }
        }

        public static IOnlineApplication GetApplication(Guid appObjectGuid)
        {
            return ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(appObjectGuid);
        }

        public static bool CanLogin(Guid appObjectGuid)
        {
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application != null)
                {
                    return !application.IsLoggedIn;
                }
                return false;
            }
            return false;
        }

        public static bool CanLoginToAny(ISVNode[] svNodes)
        {
            if (svNodes == null)
            {
                return false;
            }
            foreach (ISVNode val in svNodes)
            {
                if (typeof(IApplicationObject).IsAssignableFrom(val.ObjectType) && CanLogin(val.ObjectGuid))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanLogout(Guid appObjectGuid)
        {
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application != null)
                {
                    return application.IsLoggedIn;
                }
                return false;
            }
            return false;
        }

        public static bool CanLogoutFromAny(ISVNode[] svNodes)
        {
            if (svNodes == null)
            {
                return false;
            }
            foreach (ISVNode val in svNodes)
            {
                if (typeof(IApplicationObject).IsAssignableFrom(val.ObjectType) && CanLogout(val.ObjectGuid))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanOnlineChange(Guid appObjectGuid)
        {
            if (!OnlineChangeSupported(appObjectGuid))
            {
                return false;
            }
            if (!CanDownload(appObjectGuid))
            {
                return false;
            }
            bool bOnlineChangePossible = false;
            IsUpToDate(appObjectGuid, out bOnlineChangePossible);
            return bOnlineChangePossible;
        }

        public static bool CanDownload(Guid appObjectGuid)
        {
            bool bHaltOnBreakpoint;
            return CanDownload(appObjectGuid, out bHaltOnBreakpoint);
        }

        public static bool CanDownload(Guid appObjectGuid, out bool bHaltOnBreakpoint)
        {
            //IL_008b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0091: Invalid comparison between Unknown and I4
            //IL_009c: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a2: Invalid comparison between Unknown and I4
            bHaltOnBreakpoint = false;
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application == null)
                {
                    return false;
                }
                if (application.IsLoggedIn && application.CodeId == Guid.Empty && application.DataId == Guid.Empty)
                {
                    return true;
                }
                Guid codeId = application.CodeId;
                Guid dataId = application.DataId;
                Guid guid = default(Guid);
                Guid guid2 = default(Guid);
                ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetDownloadIds(appObjectGuid, out guid, out guid2);
                bool flag = codeId == guid && dataId == guid2 && application.IsLoggedIn;
                bHaltOnBreakpoint = flag && (int)application.ApplicationState == 3;
                if (flag)
                {
                    return (int)application.ApplicationState != 3;
                }
                return false;
            }
            return false;
        }

        public static bool CanCreateBootApplication(Guid appObjectGuid)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Invalid comparison between Unknown and I4
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0035: Invalid comparison between Unknown and I4
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application == null)
                {
                    return true;
                }
                if (!application.IsLoggedIn)
                {
                    return true;
                }
                if ((int)application.ApplicationState == -1)
                {
                    return true;
                }
                return (int)application.ApplicationState > 0;
            }
            return false;
        }

        public static bool CanStart(Guid appObjectGuid)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Invalid comparison between Unknown and I4
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0035: Invalid comparison between Unknown and I4
            //IL_0038: Unknown result type (might be due to invalid IL or missing references)
            //IL_003e: Invalid comparison between Unknown and I4
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application == null)
                {
                    return false;
                }
                if (!application.IsLoggedIn)
                {
                    return false;
                }
                if ((int)application.ApplicationState == -1)
                {
                    return true;
                }
                if ((int)application.ApplicationState != 2)
                {
                    return (int)application.ApplicationState == 3;
                }
                return true;
            }
            return false;
        }

        public static bool CanStartAny(ISVNode[] svNodes)
        {
            if (svNodes == null)
            {
                return false;
            }
            foreach (ISVNode val in svNodes)
            {
                if (typeof(IApplicationObject).IsAssignableFrom(val.ObjectType) && CanStart(val.ObjectGuid))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanSingleCycle(Guid appObjectGuid)
        {
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application == null)
                {
                    return false;
                }
                return application.IsLoggedIn;
            }
            return false;
        }

        public static bool CanStop(Guid appObjectGuid)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Invalid comparison between Unknown and I4
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0037: Unknown result type (might be due to invalid IL or missing references)
            //IL_003d: Invalid comparison between Unknown and I4
            //IL_0040: Unknown result type (might be due to invalid IL or missing references)
            //IL_0046: Invalid comparison between Unknown and I4
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application == null)
                {
                    return false;
                }
                if (!application.IsLoggedIn)
                {
                    return false;
                }
                if ((int)application.ApplicationState == -1)
                {
                    return true;
                }
                if ((int)application.ApplicationState != 0 && (int)application.ApplicationState != 2)
                {
                    return (int)application.ApplicationState != 3;
                }
                return false;
            }
            return false;
        }

        public static bool CanStopAny(ISVNode[] svNodes)
        {
            if (svNodes == null)
            {
                return false;
            }
            foreach (ISVNode val in svNodes)
            {
                if (typeof(IApplicationObject).IsAssignableFrom(val.ObjectType) && CanStop(val.ObjectGuid))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanReset(Guid appObjectGuid)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Invalid comparison between Unknown and I4
            //IL_002f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0035: Invalid comparison between Unknown and I4
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application == null)
                {
                    return false;
                }
                if (!application.IsLoggedIn)
                {
                    return false;
                }
                if ((int)application.ApplicationState == -1)
                {
                    return true;
                }
                return (int)application.ApplicationState > 0;
            }
            return false;
        }

        public static bool IsUpToDate(Guid appObjectGuid, out bool bOnlineChangePossible)
        {
            bOnlineChangePossible = true;
            if (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).IsUpToDate(appObjectGuid, out bOnlineChangePossible))
            {
                return false;
            }
            Guid parentObjectGuid = GetParentObjectGuid(appObjectGuid);
            if (parentObjectGuid == Guid.Empty)
            {
                return true;
            }
            IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(parentObjectGuid);
            if (precompileContext != null && !precompileContext.IsEmpty())
            {
                return IsUpToDate(parentObjectGuid, out bOnlineChangePossible);
            }
            return true;
        }

        public static bool IsUpToDate(Guid appObjectGuid)
        {
            if (!APEnvironment.LanguageModelMgr.IsUpToDate(appObjectGuid))
            {
                return false;
            }
            Guid parentObjectGuid = GetParentObjectGuid(appObjectGuid);
            if (parentObjectGuid == Guid.Empty)
            {
                return true;
            }
            IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(parentObjectGuid);
            if (precompileContext != null && !precompileContext.IsEmpty())
            {
                return IsUpToDate(parentObjectGuid);
            }
            return true;
        }

        public static bool CanWriteValues()
        {
            return CanWriteOrForceValues();
        }

        public static bool CanForceValues()
        {
            return CanWriteOrForceValues();
        }

        private static bool CanWriteOrForceValues()
        {
            try
            {
                IOnlineApplication[] allOnlineApplications = GetAllOnlineApplications();
                Debug.Assert(allOnlineApplications != null);
                IOnlineApplication[] array = allOnlineApplications;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].PreparedVarRefs.Length != 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public static bool CanWriteOrForceValuesSelectedApp(Guid appObjectGuid)
        {
            try
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                if (application != null && application.IsLoggedIn && application.PreparedVarRefs.Length != 0)
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public static bool CanReleaseForceValues(Guid appObjGuid)
        {
            return IsLoggedIn(appObjGuid);
        }

        public static bool IsLoggedInWithDownload(Guid appObjGuid)
        {
            //IL_0013: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                IOnlineApplication application = GetApplication(appObjGuid);
                if (application == null || !application.IsLoggedIn || (int)application.ApplicationState == 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool IsLoggedIn(Guid appObjGuid)
        {
            try
            {
                IOnlineApplication application = GetApplication(appObjGuid);
                if (application == null || !application.IsLoggedIn)
                {
                    return false;
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static bool HasTaskKillTargetSetting(Guid appObjectGuid)
        {
            //IL_0017: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            try
            {
                IOnlineApplicationObject onlineApplicationObject = GetOnlineApplicationObject0(appObjectGuid);
                IOnlineDevice5 val = (IOnlineDevice5)((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(onlineApplicationObject.DeviceGuid);
                if (val == null)
                {
                    return false;
                }
                return val.HasProperty((TargetProperties)1);
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
                return false;
            }
        }

        public static bool HasPreserveAppTargetSetting(Guid appObjectGuid)
        {
            //IL_002e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0034: Expected O, but got Unknown
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0055: Expected O, but got Unknown
            try
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject == null)
                {
                    return false;
                }
                IOnlineApplicationObject val = (IOnlineApplicationObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, appObjectGuid).Object;
                IDeviceObject val2 = (IDeviceObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, val.DeviceGuid).Object;
                ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val2.DeviceIdentification);
                return LocalTargetSettings.PreserveApplication.GetBoolValue(targetSettingsById);
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
                return false;
            }
        }

        public static bool HasMaxNumberOfAppsTargetSetting(Guid appObjectGuid, out int numberOfApps)
        {
            //IL_0031: Unknown result type (might be due to invalid IL or missing references)
            //IL_0037: Expected O, but got Unknown
            //IL_0052: Unknown result type (might be due to invalid IL or missing references)
            //IL_0058: Expected O, but got Unknown
            numberOfApps = -1;
            try
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject == null)
                {
                    return false;
                }
                IOnlineApplicationObject val = (IOnlineApplicationObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, appObjectGuid).Object;
                IDeviceObject val2 = (IDeviceObject)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, val.DeviceGuid).Object;
                ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val2.DeviceIdentification);
                numberOfApps = LocalTargetSettings.MaxNumberOfApps.GetIntValue(targetSettingsById);
                return true;
            }
            catch (Exception value)
            {
                Debug.WriteLine(value);
                return false;
            }
        }

        public static bool Login(Guid appObjectGuid)
        {
            return LoginWithParameters(appObjectGuid, (LoginServiceFlags)1, null, null);
        }

        internal static LoginDialogHandlerDelegate CreateImplicitDeviceApplicationLoginDelegate(Guid childApp, Guid parentAppGuid)
        {
            //IL_0019: Unknown result type (might be due to invalid IL or missing references)
            //IL_001f: Expected O, but got Unknown
            return (LoginDialogHandlerDelegate)delegate (Guid appguid, PromptChoice choice, LoginPromptType promptType, string[] options)
            {

                PromptResult val = (PromptResult)1;
                string stResourceKey = promptType.ToString();
                if ((int)promptType == 1)
                {
                    if (appguid == parentAppGuid)
                    {
                        IOnlineApplication application = GetApplication(childApp);
                        string applicationNameByGuid = GetApplicationNameByGuid(childApp);
                        return Prompt(stResourceKey, (PromptChoice)2, (PromptResult)2, false, application, applicationNameByGuid, options[1]);
                    }
                    if (appguid == childApp)
                    {
                        return (PromptResult)2;
                    }
                    return DefaultLoginDelegate(appguid, choice, promptType, options);
                }
                return DefaultLoginDelegate(appguid, choice, promptType, options);
            };
        }

        internal static PromptResult DefaultLoginDelegate(Guid applicationGuid, PromptChoice choice, LoginPromptType promptType, string[] options)
        {

            PromptResult val = (PromptResult)1;
            IOnlineApplication application = GetApplication(applicationGuid);
            string text = promptType.ToString();
            switch ((int)promptType)
            {
                case 0:
                    text = "Warning_ApplicationLogin_DifferentDevice";
                    if (((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("OnlineCommands", "WarningApplicationLoginDifferentDevice"))
                    {
                        return (PromptResult)(((IEngine3)APEnvironment.Engine).OEMCustomization.GetBoolValue("OnlineCommands", "WarningApplicationLoginDifferentDevice") ? 2 : 3);
                    }
                    return APEnvironment.MessageService.Prompt(options[0], (PromptChoice)2, (PromptResult)2, Strings.DoNotWarnAgain, options[1], text, Array.Empty<object>());
                case 1:
                    return Prompt(text, (PromptChoice)2, (PromptResult)2, false, application, options[0], options[1]);
                case 2:
                    return Prompt(text, (PromptChoice)3, (PromptResult)2, true, application, options[0]);
                case 3:
                    return Prompt(text, (PromptChoice)3, (PromptResult)2, true, application, options[0]);
                case 4:
                    return Prompt(text, (PromptChoice)3, (PromptResult)2, true, application);
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    {
                        s_onlappInfoObject = null;
                        s_onlappContentObject = null;
                        IOnlineApplication onlineApplicationForDetails = (PreloadAppInfoAndContent(application) ? application : null);
                        return Prompt(text, (PromptChoice)2, (PromptResult)2, true, onlineApplicationForDetails, options[0]);
                    }
                case 11:
                    return Prompt(text, (PromptChoice)2, (PromptResult)2, false, application, options[0]);
                case 12:
                    return Prompt(text, (PromptChoice)2, (PromptResult)2, false, application, options[0]);
                default:
                    throw new InvalidOperationException($"Unexpected LoginPromptType option: {text}");
            }
        }

        internal static PromptOnlineChangeResult DefaultOnlineChangeDelegate(Guid applicationGuid, OnlineChangePromptType promptType, out BootProjectTransferMode transferMode)
        {
            PromptOnlineChangeResult val = (PromptOnlineChangeResult)3;
            string arg = promptType.ToString();
            IOnlineApplication application = GetApplication(applicationGuid);
            if ((int)promptType != 0)
            {
                if ((int)promptType == 1)
                {
                    PreloadAppInfoAndContent(application);
                    return PromptOnlineChange(application, out transferMode);
                }
                throw new InvalidOperationException($"Unexpected OnlineChangePromptType option: {arg}");
            }
            PreloadAppInfoAndContent(application);
            return PromptOnlineChangeWithForcedVars(application, out transferMode);
        }

        public static bool MessageStorageContainsErrors()
        {
            return APEnvironment.MessageStorage.GetMessages(APEnvironment.CreateBuildMessageCategory(), (Severity)2).Length != 0;
        }

        public static bool LoginWithParameters(Guid appObjectGuid, LoginServiceFlags flags, OnlineChangeDialogHandlerDelegate dlgOnline, LoginDialogHandlerDelegate dlgPrompt, bool skipParentLogin = false)
        {

            ActivateMessageView();
            object obj = null;
            IOnlineDevice3 val = null;
            IOnlineApplication val2 = null;
            BootProjectTransferMode transferMode = (BootProjectTransferMode)0;
            bool flag = HasPreserveAppTargetSetting(appObjectGuid);
            bool flag2 = ((int)flags & 0x100) > 0;
            bool flag3 = ((int)flags & 0x200) > 0;
            int numberOfApps = 0;
            bool flag4 = HasMaxNumberOfAppsTargetSetting(appObjectGuid, out numberOfApps) && numberOfApps == 1;
            string text = string.Empty;
            Guid parentObjectGuid = GetParentObjectGuid(appObjectGuid);
            bool flag5 = false;
            if (dlgPrompt == null)
            {
                dlgPrompt = new LoginDialogHandlerDelegate(DefaultLoginDelegate);
            }
            if (dlgOnline == null)
            {
                dlgOnline = new OnlineChangeDialogHandlerDelegate(DefaultOnlineChangeDelegate);
            }
            try
            {
                if (!CanLogin(appObjectGuid))
                {
                    throw new InvalidOperationException("Login is not possible in this state.");
                }
                IOnlineDevice onlineDeviceForApplication = GetOnlineDeviceForApplication(appObjectGuid);
                val = (IOnlineDevice3)(object)((onlineDeviceForApplication is IOnlineDevice3) ? onlineDeviceForApplication : null);
                if (val != null)
                {
                    obj = new object();
                    val.SharedConnect(obj);
                    if (val is IOnlineDevice10 && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IOnlineDevice10)val).DeviceObjectGuid))
                    {
                        IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, ((IOnlineDevice10)val).DeviceObjectGuid);
                        text = objectToRead.Name;
                        if (objectToRead.Object is IDeviceObject4)
                        {
                            flag5 = ((IDeviceObject4)objectToRead.Object).SimulationMode;
                        }
                        if (!flag2 && val is IOnlineDevice11)
                        {
                            string text2 = default(string);
                            string text3 = default(string);
                            ((IOnlineDevice11)val).GetTargetIdent(out text2, out text2, out text3);
                            if (!string.IsNullOrEmpty(text3) && objectToRead.Object is IDeviceObject4 && !((IDeviceObject4)objectToRead.Object).SimulationMode && ((IDeviceObject)objectToRead.Object).CommunicationSettings != null && ((IDeviceObject)objectToRead.Object).CommunicationSettings is ICommunicationSettings2 && !string.IsNullOrEmpty(((ICommunicationSettings2)((IDeviceObject)objectToRead.Object).CommunicationSettings).Name))
                            {
                                string name = ((ICommunicationSettings2)((IDeviceObject)objectToRead.Object).CommunicationSettings).Name;
                                if (!string.IsNullOrEmpty(name) && text3 != name && text3.Length < 49 && name.Length < 49)
                                {
                                    string text4 = string.Format(Strings.Warning_ApplicationLogin_DifferentDevice, text3, name);
                                    if ((int)dlgPrompt.Invoke(appObjectGuid, (PromptChoice)2, (LoginPromptType)0, new string[2] { text4, text3 }) == 3)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                bool flag6 = IsPlcLogicGuid(appObjectGuid);
                string text5 = null;
                IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(parentObjectGuid);
                if (parentObjectGuid != Guid.Empty && precompileContext != null && !precompileContext.IsEmpty())
                {
                    if (flag)
                    {
                        throw new OnlineManagerException(string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "Err_TSPreserveApp_NoChildApp"), GetApplicationNameByGuid(appObjectGuid)));
                    }
                    bool flag7 = IsDeviceApplication(parentObjectGuid) && (Delegate)(object)dlgPrompt == (Delegate)new LoginDialogHandlerDelegate(DefaultLoginDelegate);
                    bool flag8 = IsLoggedInWithDownload(parentObjectGuid);
                    if (flag8 && flag7 && !IsUpToDate(appObjectGuid))
                    {
                        Logout(parentObjectGuid);
                        flag8 = false;
                    }
                    if (!skipParentLogin && !flag8)
                    {
                        if (flag7)
                        {
                            dlgPrompt = CreateImplicitDeviceApplicationLoginDelegate(appObjectGuid, parentObjectGuid);
                        }
                        if (!LoginWithParameters(parentObjectGuid, flags, dlgOnline, dlgPrompt))
                        {
                            return false;
                        }
                        if (!flag7)
                        {
                            Logout(parentObjectGuid);
                        }
                    }
                    text5 = GetParentName(appObjectGuid);
                }
                val2 = GetApplication(appObjectGuid);
                string applicationNameByGuid = GetApplicationNameByGuid(appObjectGuid);
                if (!flag2 && 1 == (int)PromptOutdatedDevdesc((IOnlineDevice)(object)val))
                {
                    return false;
                }
                bool flag9 = false;
                bool flag10 = false;
                if (val2 != null)
                {
                    bool flag11 = false;
                    try
                    {
                        InternalCodeStateProvider.StopOnlineCodeStateUpdates();
                        flag11 = val2.Login(true);
                    }
                    finally
                    {
                        InternalCodeStateProvider.StartOnlineCodeStateUpdates();
                    }
                    if (!ConfirmUpdateDeviceApplications(val2))
                    {
                        val2.Logout();
                        return false;
                    }
                    if (flag11 && !string.IsNullOrEmpty(applicationNameByGuid) && val is IOnlineDevice6 && ((IOnlineDevice)val).IsConnected)
                    {
                        string[] array = ((IOnlineDevice)(IOnlineDevice6)val).ReadApplicationList(0, int.MaxValue);
                        flag10 = true;
                        if (array != null && array.Length != 0)
                        {
                            for (int i = 0; i < array.Length; i++)
                            {
                                if (array[i] == applicationNameByGuid)
                                {
                                    flag10 = false;
                                }
                            }
                        }
                        if (flag10)
                        {
                            flag11 = false;
                        }
                    }
                    if (!flag11)
                    {
                        PromptResult val3 = (flag2 ? ((PromptResult)2) : ((!flag6) ? dlgPrompt.Invoke(appObjectGuid, (PromptChoice)2, (LoginPromptType)1, new string[2]
                        {
                            GetApplicationNameByGuid(appObjectGuid),
                            text
                        }) : dlgPrompt.Invoke(appObjectGuid, (PromptChoice)2, (LoginPromptType)11, new string[1] { GetApplicationNameByGuid(appObjectGuid) })));
                        if ((int)val3 != 2)
                        {
                            val2.Logout();
                            return false;
                        }
                        if (flag4 && !string.IsNullOrEmpty(applicationNameByGuid) && val is IOnlineDevice6 && ((IOnlineDevice)val).IsConnected)
                        {
                            string[] array2 = ((IOnlineDevice)(IOnlineDevice6)val).ReadApplicationList(0, int.MaxValue);
                            if (array2 != null && array2.Length == 1 && array2[0] != applicationNameByGuid && !string.IsNullOrEmpty(array2[0]))
                            {
                                ((IOnlineDevice)val).DeleteApplication(array2[0]);
                            }
                        }
                        val2.CreateAppOnDevice(text5);
                        if (flag10 && val2.IsLoggedIn)
                        {
                            val2.Logout();
                        }
                        try
                        {
                            InternalCodeStateProvider.StopOnlineCodeStateUpdates();
                            if (!val2.Login(true))
                            {
                                return false;
                            }
                        }
                        finally
                        {
                            InternalCodeStateProvider.StartOnlineCodeStateUpdates();
                        }
                        flag9 = true;
                    }
                }
                bool bOnlineChangePossible = false;
                bool flag12 = false;
                Guid codeId = val2.CodeId;
                Guid dataId = val2.DataId;
                ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ProcessQueuedLibraryPreCompileContexts(true);
                Guid guid = default(Guid);
                Guid guid2 = default(Guid);
                ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetDownloadIds(appObjectGuid, out guid, out guid2);
                Guid guid3 = default(Guid);
                Guid guid4 = default(Guid);
                ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetLastDownloadIds(appObjectGuid, out guid3, out guid4);
                bool flag13 = APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)40);
                bool flag14 = false;
                if (flag13)
                {
                    bool flag15 = IsUpToDate(appObjectGuid, out bOnlineChangePossible);
                    flag14 = codeId == guid3 && dataId == guid4 && flag15;
                }
                bool flag16 = codeId == guid && dataId == guid2;
                if (codeId != Guid.Empty && dataId != Guid.Empty && !flag16 && !flag14 && ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CheckAndLoadBootInfo(appObjectGuid, codeId, dataId))
                {
                    ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetDownloadIds(appObjectGuid, out guid, out guid2);
                    ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetLastDownloadIds(appObjectGuid, out guid3, out guid4);
                }
                bool flag17 = IsUpToDate(appObjectGuid, out bOnlineChangePossible);
                if (flag17 && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication == appObjectGuid)
                {
                    InternalCodeStateProvider.InternalCodeState = (OnlineCodeState)0;
                }
                if (!flag17 && bOnlineChangePossible)
                {
                    flag12 = AlwaysDownload(appObjectGuid) || ((int)flags & 4) > 0;
                }
                if (!bOnlineChangePossible && ((int)flags & 2) > 0)
                {
                    if (CanLogout(appObjectGuid))
                    {
                        Logout(appObjectGuid);
                    }
                    throw new LoginServiceException(string.Format(Strings.LoginSvc_Error_DownloadNotAllowed, applicationNameByGuid));
                }
                if (!codeId.Equals(Guid.Empty) && !codeId.Equals(Guid.Empty) && codeId.Equals(guid) && dataId.Equals(guid2) && flag17)
                {
                    ConfirmDeleteApplications(val2, bUserMayCancelDownload: false, flags);
                    return true;
                }
                if (flag5)
                {
                    transferMode = (BootProjectTransferMode)1;
                }
                if (flag6)
                {
                    if (flag9 || flag2)
                    {
                        Download(appObjectGuid, bOnlineChange: false, bCheck: false, flags, transferMode);
                    }
                    else if ((int)dlgPrompt.Invoke(appObjectGuid, (PromptChoice)2, (LoginPromptType)12, new string[2]
                    {
                        applicationNameByGuid,
                        Strings.QueryDownloadDevice
                    }) == 2)
                    {
                        Download(appObjectGuid, bOnlineChange: false, bCheck: false, flags, transferMode);
                    }
                    else
                    {
                        val2.Logout();
                    }
                }
                else if (codeId == Guid.Empty && dataId == Guid.Empty)
                {
                    if (flag9 || flag2)
                    {
                        Download(appObjectGuid, bOnlineChange: false, bCheck: false, flags, transferMode);
                    }
                    else if ((int)dlgPrompt.Invoke(appObjectGuid, (PromptChoice)2, (LoginPromptType)1, new string[2] { applicationNameByGuid, text }) == 2)
                    {
                        Download(appObjectGuid, bOnlineChange: false, bCheck: false, flags, transferMode);
                    }
                    else
                    {
                        val2.Logout();
                    }
                }
                else
                {
                    if ((int)val2.ApplicationState == 3)
                    {
                        PromptResult val4 = (PromptResult)2;
                        val4 = ((((int)flags & 0x40) > 0) ? ((PromptResult)1) : ((!flag2) ? dlgPrompt.Invoke(appObjectGuid, (PromptChoice)3, (LoginPromptType)2, new string[1] { applicationNameByGuid }) : ((PromptResult)2)));
                        if ((int)val4 != 2)
                        {
                            if ((int)val4 == 1)
                            {
                                val2.Logout();
                            }
                            return val2.IsLoggedIn;
                        }
                        val2.SingleCycle();
                    }
                    if (bOnlineChangePossible)
                    {
                        bOnlineChangePossible = ((IOnlineManager6)APEnvironment.OnlineMgr).CheckOnlineApplicationFeatureSupport((OnlineFeatureEnum)2, val2);
                    }
                    bool num = !flag17 && codeId.Equals(guid) && dataId.Equals(guid2) && bOnlineChangePossible && !flag12;
                    bool flag18 = flag17 && codeId.Equals(guid3) && dataId.Equals(guid4) && bOnlineChangePossible && !flag12;
                    if (num || (flag18 && flag13))
                    {
                        PromptOnlineChangeResult val5 = (PromptOnlineChangeResult)3;
                        val5 = ((((int)flags & 0x40) > 0 && OnlineApplicationIsRunning(val2)) ? ((PromptOnlineChangeResult)3) : (flag2 ? ((((int)flags & 2) > 0) ? ((PromptOnlineChangeResult)0) : ((((int)flags & 4) > 0) ? ((PromptOnlineChangeResult)1) : ((((int)flags & 1) <= 0) ? ((PromptOnlineChangeResult)0) : ((PromptOnlineChangeResult)0)))) : ((((IOnlineApplication5)((val2 is IOnlineApplication5) ? val2 : null)).GetForcedVarRefs().Length == 0) ? dlgOnline.Invoke(appObjectGuid, (OnlineChangePromptType)1, out transferMode) : dlgOnline.Invoke(appObjectGuid, (OnlineChangePromptType)0, out transferMode))));
                        if (flag5)
                        {
                            transferMode = (BootProjectTransferMode)1;
                        }
                        switch ((int)val5)
                        {
                            case 0:
                                LogoutChildApplications(val2);
                                Download(appObjectGuid, bOnlineChange: true, bCheck: false, flags, transferMode);
                                break;
                            case 1:
                                if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)2, (ushort)0) && (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateCode(appObjectGuid, false, false) || APEnvironment.MessageStorage.GetMessages(APEnvironment.CreateBuildMessageCategory(), (Severity)2).Length != 0))
                                {
                                    throw new GenerateCodeFailedException(Strings.GenerateCodeFailed);
                                }
                                if (!flag && val2.OnlineDevice != null)
                                {
                                    bool flag19 = false;
                                    if (val2 is IOnlineApplication4)
                                    {
                                        try
                                        {
                                            ((IOnlineApplication4)val2).ReinitAppOnDevice(text5, true);
                                            flag19 = true;
                                        }
                                        catch (OperationCanceledException)
                                        {
                                            if (val2.IsLoggedIn)
                                            {
                                                val2.Logout();
                                            }
                                            return false;
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    if (!flag19)
                                    {
                                        if (val2.OnlineDevice == null)
                                        {
                                            break;
                                        }
                                        val2.OnlineDevice.DeleteApplication(applicationNameByGuid);
                                        val2.CreateAppOnDevice(text5);
                                    }
                                    val2.Logout();
                                    val2.Login(false);
                                }
                                LogoutChildApplications(val2);
                                Download(appObjectGuid, bOnlineChange: false, bCheck: false, flags, transferMode);
                                break;
                            case 2:
                                InternalCodeStateProvider.RaiseLoginWithOutdatedCodeDetected();
                                break;
                            default:
                                val2.Logout();
                                break;
                        }
                    }
                    else if (flag18)
                    {
                        PromptResult val6 = (OnlineApplicationIsRunning(val2) ? ((((int)flags & 0x40) > 0) ? ((PromptResult)1) : ((!flag2) ? dlgPrompt.Invoke(appObjectGuid, (PromptChoice)3, (LoginPromptType)3, new string[1] { applicationNameByGuid }) : ((PromptResult)2))) : ((!flag2) ? dlgPrompt.Invoke(appObjectGuid, (PromptChoice)3, (LoginPromptType)4, new string[1] { applicationNameByGuid }) : ((PromptResult)2)));
                        if ((int)val6 == 2)
                        {
                            LogoutChildApplications(val2);
                            Download(appObjectGuid, bOnlineChange: true, bCheck: false, flags, transferMode);
                        }
                        else if ((int)val6 == 1)
                        {
                            val2.Logout();
                        }
                    }
                    else
                    {
                        LoginPromptType val7 = (IsDeviceApplication(appObjectGuid) ? ((LoginPromptType)8) : ((codeId.Equals(guid) && dataId.Equals(guid2) && !flag12) ? ((!OnlineApplicationIsRunning(val2)) ? ((LoginPromptType)6) : ((LoginPromptType)5)) : (flag12 ? ((!OnlineApplicationIsRunning(val2)) ? ((LoginPromptType)8) : ((LoginPromptType)7)) : ((!OnlineApplicationIsRunning(val2)) ? ((LoginPromptType)10) : ((LoginPromptType)9)))));
                        bool flag20 = false;
                        if ((!OnlineApplicationIsRunning(val2) || ((int)flags & 0x40) <= 0) && (flag2 || (int)dlgPrompt.Invoke(appObjectGuid, (PromptChoice)2, val7, new string[1] { applicationNameByGuid }) == 2))
                        {
                            if (!flag && val2.OnlineDevice != null)
                            {
                                bool flag21 = false;
                                if (val2 is IOnlineApplication4)
                                {
                                    if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)2, (ushort)10) && (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateCode(appObjectGuid, false, false) || APEnvironment.MessageStorage.GetMessages(APEnvironment.CreateBuildMessageCategory(), (Severity)2).Length != 0))
                                    {
                                        throw new GenerateCodeFailedException(Strings.GenerateCodeFailed);
                                    }
                                    try
                                    {
                                        ((IOnlineApplication4)val2).ReinitAppOnDevice(text5, true);
                                        flag21 = true;
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        if (val2.IsLoggedIn)
                                        {
                                            val2.Logout();
                                        }
                                        return false;
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                if (!flag21)
                                {
                                    if (val2.OnlineDevice == null)
                                    {
                                        return false;
                                    }
                                    val2.OnlineDevice.DeleteApplication(applicationNameByGuid);
                                    val2.CreateAppOnDevice(text5);
                                }
                                val2.Logout();
                                try
                                {
                                    InternalCodeStateProvider.StopOnlineCodeStateUpdates();
                                    val2.Login(false);
                                }
                                finally
                                {
                                    InternalCodeStateProvider.StartOnlineCodeStateUpdates();
                                }
                            }
                            LogoutChildApplications(val2);
                            Download(appObjectGuid, bOnlineChange: false, bCheck: false, flags, transferMode);
                        }
                        else
                        {
                            val2.Logout();
                        }
                    }
                }
                LoginIOMappingChildApplication(appObjectGuid, flags, dlgOnline);
                return val2.IsLoggedIn;
            }
            catch (InvalidOnlineConfigException val8)
            {
                InvalidOnlineConfigException val9 = val8;
                string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "InvalidCommunicationSettings");
                if (!flag2)
                {
                    PromptResult val10 = APEnvironment.MessageService.Prompt(@string, (PromptChoice)3, (PromptResult)2, "InvalidCommunicationSettings", Array.Empty<object>());
                    if ((int)val10 != 1)
                    {
                        int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                        IMetaObject val11 = FindDeviceObject(handle, appObjectGuid);
                        if (val11 != null)
                        {
                            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, val11.ObjectGuid);
                            IEditorView val12 = ((IEngine)APEnvironment.Engine).Frame.OpenEditorView(metaObjectStub, Guid.Empty, "");
                            if ((int)val10 == 2)
                            {
                                try
                                {
                                    IEditor editor = val12.Editor;
                                    IEditorFrameBase val13 = (IEditorFrameBase)(object)((editor is IEditorFrameBase) ? editor : null);
                                    if (val13 != null)
                                    {
                                        IEditorPage activeEditorPage = val13.GetActiveEditorPage();
                                        if (activeEditorPage != null)
                                        {
                                            ((IDeviceCommunicationEditor2)((activeEditorPage is IDeviceCommunicationEditor2) ? activeEditorPage : null)).TriggerActivePathWizard();
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    return false;
                }
                throw val9;
            }
            catch (CancelledByUserException)
            {
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                return false;
            }
            catch (InteractiveLoginFailedException val15)
            {
                InteractiveLoginFailedException val16 = val15;
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                if (!flag3 && !val16.ShouldBeHandledSilently)
                {
                    APEnvironment.MessageService.Error(((Exception)(object)val16).Message, "InteractiveLoginFailedException", Array.Empty<object>());
                }
                return false;
            }
            catch (TargetInvocationException ex6)
            {
                if (!flag3)
                {
                    if (ex6.InnerException != null)
                    {
                        APEnvironment.MessageService.Error(ex6.InnerException.Message, "TargetInvocationExceptionInner", Array.Empty<object>());
                    }
                    else
                    {
                        APEnvironment.MessageService.Error(ex6.Message, "TargetInvocationException", Array.Empty<object>());
                    }
                }
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                if (flag3)
                {
                    throw ex6;
                }
                return false;
            }
            catch (LoginServiceException val17)
            {
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                throw val17;
            }
            catch (UnresolvedReferencesException val18)
            {
                UnresolvedReferencesException val19 = val18;
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                if (flag3)
                {
                    throw val19;
                }
                return false;
            }
            catch (Exception ex7)
            {
                if (CanLogout(appObjectGuid))
                {
                    Logout(appObjectGuid);
                }
                string text6 = string.Empty;
                if (ex7 is TargetMismatchException2)
                {
                    string detailsFromTargetMismatchException = GetDetailsFromTargetMismatchException2((TargetMismatchException2)ex7);
                    if (flag3)
                    {
                        text6 = detailsFromTargetMismatchException.ToString();
                    }
                    else
                    {
                        APEnvironment.MessageService.Error(detailsFromTargetMismatchException.ToString(), "ErrorGeneric_LoginWithParameters01", Array.Empty<object>());
                    }
                }
                else if (flag3)
                {
                    text6 = ex7.Message;
                }
                else if (ex7.GetType() == typeof(OnlineManagerException) && ((IEngine)APEnvironment.Engine).MessageService is IMessageService5)
                {
                    IMessageService messageService = ((IEngine)APEnvironment.Engine).MessageService;
                    ((IMessageService5)((messageService is IMessageService5) ? messageService : null)).ErrorWithDetails(ex7.Message, (EventHandler)UnknownLoginErrorDetailsClickHandler, (EventArgs)new UnknownLoginErrorEventArgs(appObjectGuid), "UnknownErrorOnLogin", Array.Empty<object>());
                }
                else if (((IEngine)APEnvironment.Engine).MessageService is IMessageService3 && ex7.GetType() == typeof(OnlineManager2Exception) && ((OnlineManager2Exception)ex7).ErrorCode == 24)
                {
                    IMessageService messageService2 = ((IEngine)APEnvironment.Engine).MessageService;
                    ((IMessageService3)((messageService2 is IMessageService3) ? messageService2 : null)).Error(ex7.Message, "ErrorGeneric_LoginWithParameters03", Array.Empty<object>());
                }
                else
                {
                    APEnvironment.MessageService.Error(ex7.Message, "ErrorGeneric_LoginWithParameters02", Array.Empty<object>());
                }
                if (text6 != string.Empty)
                {
                    throw new LoginServiceException(text6);
                }
                return false;
            }
            finally
            {
                if (obj != null && val != null)
                {
                    try
                    {
                        val.SharedDisconnect(obj);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void LoginIOMappingChildApplication(Guid parentAppObjectGuid, LoginServiceFlags flags, OnlineChangeDialogHandlerDelegate dlgOnline)
        {
            //IL_0096: Unknown result type (might be due to invalid IL or missing references)
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            Guid[] subObjectGuids = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, parentAppObjectGuid).SubObjectGuids;
            foreach (Guid guid in subObjectGuids)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
                if (!typeof(IChildOnlineApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    continue;
                }
                IObjectProperty[] properties = metaObjectStub.Properties;
                for (int j = 0; j < properties.Length; j++)
                {
                    if (((object)properties[j]).GetType() == APEnvironment.LogicalAppPropertyType)
                    {
                        LoginDialogHandlerDelegate dlgPrompt = CreateImplicitDeviceApplicationLoginDelegate(metaObjectStub.ObjectGuid, parentAppObjectGuid);
                        LoginWithParameters(metaObjectStub.ObjectGuid, flags, dlgOnline, dlgPrompt, skipParentLogin: true);
                        break;
                    }
                }
            }
        }

        private static bool PreloadAppInfoAndContent(IOnlineApplication onlappl)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0044: Unknown result type (might be due to invalid IL or missing references)
            s_onlappInfoObject = null;
            s_onlappContentObject = null;
            if (onlappl == null)
            {
                return false;
            }
            try
            {
                if (onlappl is IOnlineApplication10)
                {
                    s_onlappInfoObject = ((IOnlineApplication10)onlappl).ReadApplicationInfo();
                    if (s_onlappInfoObject == null)
                    {
                        s_onlappInfoObject = new object();
                    }
                }
                if (onlappl is IOnlineApplication14)
                {
                    try
                    {
                        s_onlappContentObject = ((IOnlineApplication14)onlappl).ReadApplicationContent();
                    }
                    catch
                    {
                    }
                    if (s_onlappContentObject == null)
                    {
                        s_onlappContentObject = new object();
                    }
                }
                return s_onlappInfoObject is IOnlineApplicationInfo;
            }
            catch
            {
            }
            return false;
        }

        internal static void UnknownLoginErrorDetailsClickHandler(object sender, EventArgs e)
        {
            UnknownLoginErrorEventArgs unknownLoginErrorEventArgs = e as UnknownLoginErrorEventArgs;
            if (unknownLoginErrorEventArgs != null)
            {
                UnknownLoginErrorDialog unknownLoginErrorDialog = new UnknownLoginErrorDialog();
                unknownLoginErrorDialog.Initialize(unknownLoginErrorEventArgs.ApplicationGuid);
                unknownLoginErrorDialog.ShowDialog(sender as IWin32Window);
                if (sender is Form)
                {
                    (sender as Form).Close();
                }
            }
        }

        private static bool OnlineApplicationIsRunning(IOnlineApplication onlappl)
        {
            //IL_0034: Unknown result type (might be due to invalid IL or missing references)
            //IL_003c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0042: Invalid comparison between Unknown and I4
            //IL_0045: Unknown result type (might be due to invalid IL or missing references)
            //IL_004b: Invalid comparison between Unknown and I4
            //IL_004e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0054: Invalid comparison between Unknown and I4
            //IL_0057: Unknown result type (might be due to invalid IL or missing references)
            //IL_005d: Invalid comparison between Unknown and I4
            if (onlappl != null)
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 300);
                DateTime now = DateTime.Now;
                while ((int)onlappl.ApplicationState == 0)
                {
                    Application.DoEvents();
                    if (DateTime.Now - now > timeSpan)
                    {
                        break;
                    }
                }
                if ((int)onlappl.ApplicationState == 1 || (int)onlappl.ApplicationState == 5 || (int)onlappl.ApplicationState == 4 || (int)onlappl.ApplicationState == 5)
                {
                    return true;
                }
            }
            return false;
        }

        private static void GetSystemApplicationsRecursive(Hashtable htFound, Dictionary<string, string> dicTested, List<IPreCompileContext4> sysappsSorted, IPreCompileContext4 comcon, Guid guidApplication)
        {
            if (!string.IsNullOrEmpty(((IPreCompileContext)comcon).LibraryPath) && dicTested.ContainsKey(((IPreCompileContext)comcon).LibraryPath))
            {
                return;
            }
            dicTested.Add(((IPreCompileContext)comcon).LibraryPath, ((IPreCompileContext)comcon).LibraryPath);
            foreach (IPreCompileContext item in comcon.LibraryContextsWithResolvedPlaceholders(guidApplication))
            {
                if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)1, (ushort)20))
                {
                    GetSystemApplicationsRecursive(htFound, dicTested, sysappsSorted, (IPreCompileContext4)(object)((item is IPreCompileContext4) ? item : null), guidApplication);
                }
                if (item is IPreCompileContext4 && ((IPreCompileContext4)((item is IPreCompileContext4) ? item : null)).SystemApplication && !htFound.ContainsKey(((IPreCompileContext4)((item is IPreCompileContext4) ? item : null)).SystemApplicationName))
                {
                    if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)1, (ushort)20))
                    {
                        GetSystemApplicationsRecursive(htFound, dicTested, sysappsSorted, (IPreCompileContext4)(object)((item is IPreCompileContext4) ? item : null), guidApplication);
                    }
                    htFound[((IPreCompileContext4)((item is IPreCompileContext4) ? item : null)).SystemApplicationName] = item.LibraryPath;
                    sysappsSorted.Add((IPreCompileContext4)(object)((item is IPreCompileContext4) ? item : null));
                }
            }
        }

        private static void FindDeviceApplicationNames(LDictionary<string, string> devapps, IOnlineApplication app)
        {
            IPreCompileContext[] array = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AllPreCompileContexts(true, false);
            Debug.Assert(array != null);
            IMetaObject val = FindDeviceObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, app.ApplicationGuid);
            if (val == null)
            {
                return;
            }
            IPreCompileContext[] array2 = array;
            foreach (IPreCompileContext val2 in array2)
            {
                try
                {
                    Guid applicationGuid = ((ICompileContextCommon)val2).ApplicationGuid;
                    if (!(applicationGuid != Guid.Empty))
                    {
                        continue;
                    }
                    IMetaObject val3 = FindDeviceObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, applicationGuid);
                    if (val3 != null && val3.ObjectGuid == val.ObjectGuid)
                    {
                        IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, applicationGuid);
                        Debug.Assert(objectToRead != null);
                        if (objectToRead.Object is IOnlineApplicationObject2)
                        {
                            IObject @object = objectToRead.Object;
                            devapps[((IOnlineApplicationObject2)((@object is IOnlineApplicationObject2) ? @object : null)).ApplicationName] = (string)null;
                        }
                        else
                        {
                            devapps[objectToRead.Name] = (string)null;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private static bool ConfirmUpdateDeviceApplications(IOnlineApplication app)
        {
            //IL_00e7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ed: Invalid comparison between Unknown and I4
            //IL_0152: Unknown result type (might be due to invalid IL or missing references)
            //IL_0158: Invalid comparison between Unknown and I4
            LDictionary<string, string> obj = new LDictionary<string, string>();
            FindDeviceApplicationNames(obj, app);
            bool flag = obj.ContainsKey("DeviceApplication");
            new ArrayList();
            new List<string>();
            try
            {
                List<string> list = new List<string>();
                List<string> stDisplayNames = new List<string>();
                List<ApplicationState> states = new List<ApplicationState>();
                ReadApplicationStatesAndNames(app.OnlineDevice, list, stDisplayNames, states);
                string stApplicationName = GetApplicationNameByGuid(app.ApplicationGuid);
                bool num = stApplicationName == "DeviceApplication";
                bool flag2 = list.Where((string name) => name == "DeviceApplication").Count() > 0;
                bool flag3 = list.Where((string name) => name == stApplicationName).Count() > 0;
                if (num && list.Count > 0 && !flag2)
                {
                    string text = string.Join("\r\n", list);
                    if ((int)Prompt("PromptDeviceApplicationCreate", (PromptChoice)1, (PromptResult)0, true, app, text) == 1)
                    {
                        return false;
                    }
                    foreach (string item in list)
                    {
                        app.OnlineDevice.DeleteApplication(item);
                    }
                }
                else if (flag3 && flag2 && !flag)
                {
                    if ((int)Prompt("PromptDeviceApplicationDelete", (PromptChoice)1, (PromptResult)0, true, app, stApplicationName) == 1)
                    {
                        return false;
                    }
                    app.OnlineDevice.DeleteApplication(stApplicationName);
                }
            }
            catch
            {
            }
            return true;
        }

        private static bool ConfirmDeleteApplications(IOnlineApplication app, bool bUserMayCancelDownload, LoginServiceFlags flags)
        {

            IPreCompileContext[] array = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AllPreCompileContexts(true, false);
            Debug.Assert(array != null);
            Hashtable hashtable = new Hashtable();
            IMetaObject val = FindDeviceObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, app.ApplicationGuid);
            if (val == null)
            {
                return true;
            }
            IPreCompileContext[] array2 = array;
            foreach (IPreCompileContext val2 in array2)
            {
                try
                {
                    Guid applicationGuid = ((ICompileContextCommon)val2).ApplicationGuid;
                    if (!(applicationGuid != Guid.Empty))
                    {
                        continue;
                    }
                    IMetaObject val3 = FindDeviceObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, applicationGuid);
                    if (val3 != null && val3.ObjectGuid == val.ObjectGuid)
                    {
                        IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, applicationGuid);
                        Debug.Assert(objectToRead != null);
                        if (objectToRead.Object is IOnlineApplicationObject2)
                        {
                            IObject @object = objectToRead.Object;
                            hashtable[((IOnlineApplicationObject2)((@object is IOnlineApplicationObject2) ? @object : null)).ApplicationName] = null;
                        }
                        else
                        {
                            hashtable[objectToRead.Name] = null;
                        }
                    }
                }
                catch
                {
                }
            }
            Hashtable hashtable2 = new Hashtable();
            List<IPreCompileContext4> list = new List<IPreCompileContext4>();
            IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(app.ApplicationGuid);
            bool flag = precompileContext != null && precompileContext is IPreCompileContext4 && ((IPreCompileContext4)((precompileContext is IPreCompileContext4) ? precompileContext : null)).SupportSystemApplication;
            if (flag)
            {
                Dictionary<string, string> dicTested = new Dictionary<string, string>();
                GetSystemApplicationsRecursive(hashtable2, dicTested, list, (IPreCompileContext4)(object)((precompileContext is IPreCompileContext4) ? precompileContext : null), app.ApplicationGuid);
            }
            ArrayList arrayList = new ArrayList();
            new List<string>();
            try
            {
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                List<ApplicationState> list4 = new List<ApplicationState>();
                ReadApplicationStatesAndNames(app.OnlineDevice, list2, list3, list4);
                for (int j = 0; j < list2.Count; j++)
                {
                    if (!hashtable.ContainsKey(list2[j]) && (int)list4[j] != 255)
                    {
                        arrayList.Add(list3[j]);
                    }
                    if (hashtable2.ContainsKey(list2[j]))
                    {
                        hashtable2.Remove(list2[j]);
                    }
                }
                if (((int)flags & 0x80) > 0)
                {
                    foreach (ApplicationState item in list4)
                    {
                        if ((int)item == 4 || (int)item == 3 || (int)item == 5 || (int)item == 1)
                        {
                            return false;
                        }
                    }
                }
                arrayList.Sort();
                if (arrayList.Count > 0)
                {
                    string[] array3 = new string[arrayList.Count];
                    arrayList.CopyTo(array3);
                    bool[] selection = new bool[arrayList.Count];
                    string stResourceKey = "PromptDeletePLCApplications";
                    if (bUserMayCancelDownload)
                    {
                        stResourceKey = "PromptDeletePLCApplicationsCancelDownload";
                    }
                    bool flag2 = false;
                    bool flag3 = OnlineFeatureHelper.CheckSpecificApplication((OnlineFeatureEnum)18, app.ApplicationGuid);
                    if (((int)flags & 8) > 0)
                    {
                        flag2 = true;
                        for (int k = 0; k < arrayList.Count; k++)
                        {
                            selection[k] = true;
                        }
                    }
                    else
                    {
                        if (((int)flags & 0x100) != 0)
                        {
                            if (flag3)
                            {
                                return false;
                            }
                            return true;
                        }
                        if (flag3)
                        {
                            stResourceKey = string.Format(Strings.PromptDeletePLCApplicationsOnlyOneAppAllowed, array3[0]);
                            if ((int)APEnvironment.MessageService.Prompt(stResourceKey, (PromptChoice)1, (PromptResult)0, "PromptDeletePLCApplicationsOnlyOneAppAllowed", Array.Empty<object>()) != 0)
                            {
                                RemoveApplicationSilently(app);
                                return false;
                            }
                            selection[0] = true;
                            flag2 = true;
                        }
                        else
                        {
                            flag2 = (int)Prompt(stResourceKey, array3, (PromptChoice)1, (PromptResult)0, out selection) == 0;
                        }
                    }
                    if (!flag2)
                    {
                        return false;
                    }
                    for (int l = 0; l < array3.Length; l++)
                    {
                        try
                        {
                            if (selection[l])
                            {
                                if (array3[l].Contains(" "))
                                {
                                    app.OnlineDevice.DeleteApplication(array3[l].Substring(0, array3[l].IndexOf(" ")));
                                }
                                else
                                {
                                    app.OnlineDevice.DeleteApplication(array3[l]);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
            if (hashtable2.Values.Count > 0 && app is IOnlineApplication7 && flag)
            {
                foreach (IPreCompileContext4 item2 in list)
                {
                    if (hashtable2.ContainsKey(item2.SystemApplicationName))
                    {
                        DownloadSystemApplication(hashtable2[item2.SystemApplicationName] as string, app);
                    }
                }
            }
            return true;
        }

        internal static void DownloadSystemApplications(IOnlineApplication7 onlapp)
        {
            if (onlapp == null)
            {
                return;
            }
            Hashtable hashtable = new Hashtable();
            List<IPreCompileContext4> list = new List<IPreCompileContext4>();
            IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(((IOnlineApplication)onlapp).ApplicationGuid);
            if (precompileContext == null || !(precompileContext is IPreCompileContext4) || !((IPreCompileContext4)((precompileContext is IPreCompileContext4) ? precompileContext : null)).SupportSystemApplication)
            {
                return;
            }
            Dictionary<string, string> dicTested = new Dictionary<string, string>();
            GetSystemApplicationsRecursive(hashtable, dicTested, list, (IPreCompileContext4)(object)((precompileContext is IPreCompileContext4) ? precompileContext : null), ((IOnlineApplication)onlapp).ApplicationGuid);
            if (hashtable.Values.Count <= 0)
            {
                return;
            }
            foreach (IPreCompileContext4 item in list)
            {
                if (hashtable.ContainsKey(item.SystemApplicationName))
                {
                    DownloadSystemApplication(hashtable[item.SystemApplicationName] as string, (IOnlineApplication)(object)onlapp);
                }
            }
        }

        private static void DownloadSystemApplication(string stLibName, IOnlineApplication app)
        {
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, app.ApplicationGuid);
            IApplicationObject val = APEnvironment.CreateHiddenAndTransientApplicationObject();
            Guid guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid, Guid.NewGuid(), (IObject)(object)val, stLibName, -1);
            IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guid);
            IOnlineApplication7 val2 = (IOnlineApplication7)(object)((application is IOnlineApplication7) ? application : null);
            ((IOnlineApplication)val2).Login(true);
            try
            {
                ((IOnlineApplication)val2).CreateAppOnDevice((string)null);
                if (((IOnlineApplication)val2).IsLoggedIn)
                {
                    ((IOnlineApplication)val2).Logout();
                }
                ((IOnlineApplication)val2).Login(false);
                val2.DownloadSystemApplication(stLibName);
            }
            catch
            {
            }
            finally
            {
                ((IOnlineApplication)val2).Logout();
                ((IObjectManager)APEnvironment.ObjectMgr).RemoveObject(objectToRead.ProjectHandle, guid);
            }
        }

        private static void ReadApplicationStatesAndNames(IOnlineDevice onldev, List<string> stApps, List<string> stDisplayNames, List<ApplicationState> states)
        {
            //IL_00a2: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a4: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c1: Expected I4, but got Unknown
            //IL_00c1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c8: Invalid comparison between Unknown and I4
            ITaggedServiceWriter val = onldev.CreateService(2L, 37);
            IDataNodeWriter obj = ((IComplexNodeWriter)val).AddDataTag(1, ContentAlignment.Align40);
            obj.Write(0u);
            obj.Write(2147483647u);
            IServiceTagEnumerator enumerator = onldev.ExecuteService((IServiceWriter)(object)val).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IServiceTagReader current = enumerator.Current;
                    if (current.TagId == 129)
                    {
                        IServiceTagEnumerator enumerator2 = current.GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                IServiceTagReader current2 = enumerator2.Current;
                                string empty = string.Empty;
                                if (current2.TagId == 4)
                                {
                                    byte b = current2.ReadByte();
                                    empty = current2.ReadString(Encoding.ASCII);
                                    stApps.Add(empty);
                                    states.Add((ApplicationState)b);
                                    ApplicationState val2 = (ApplicationState)b;
                                    stDisplayNames.Add(((int)val2 - 1) switch
                                    {
                                        0 => empty + " (" + Strings.CurrentState + ": " + Strings.run + ")",
                                        1 => empty + " (" + Strings.CurrentState + ": " + Strings.stop + ")",
                                        2 => empty + " (" + Strings.CurrentState + ": " + Strings.halt_on_bp + ")",
                                        4 => empty + " (" + Strings.CurrentState + ": " + Strings.single_cycle + ")",
                                        3 => empty + " (" + Strings.CurrentState + ": " + Strings.debug_step + ")",
                                        _ => ((int)val2 != 255) ? (empty + " (" + Strings.CurrentState + ": " + Strings.unknown + ")") : (empty + " (" + Strings.CurrentState + ": " + Strings.SystemApplication + ")"),
                                    });
                                }
                                else
                                {
                                    current2.ReadBytes();
                                }
                            }
                        }
                        finally
                        {
                            (enumerator2 as IDisposable)?.Dispose();
                        }
                    }
                    else if (current.TagId == 65407)
                    {
                        string[] array = onldev.ReadApplicationList();
                        foreach (string item in array)
                        {
                            stApps.Add(item);
                            states.Add((ApplicationState)0);
                            stDisplayNames.Add(item);
                        }
                    }
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }

        private static void MyEnumDevicesCallback(ITargetDescription description, IAsyncResult asyncResult)
        {
            Debug.WriteLine($"Device in Network:{description.Name} ({description.Address.ToString()})");
        }

        private static void EndEnumDevicesCallback(IAsyncResult asyncResult)
        {
            if (_enumGateway != null)
            {
                _enumGateway.EndEnumDevices(asyncResult);
                _enumGateway.Disconnect();
                _enumGateway = null;
                Debug.WriteLine("EnumDevices finished");
            }
        }

        private static void LogoutChildApplications(IOnlineApplication onlineApplication)
        {
            if (onlineApplication == null || onlineApplication.OnlineDevice == null || !onlineApplication.OnlineDevice.IsConnected)
            {
                return;
            }
            IList<IOnlineApplicationObject> childApplicationObjects = GetChildApplicationObjects(onlineApplication.ApplicationGuid);
            if (childApplicationObjects.Count <= 0)
            {
                return;
            }
            string[] array = null;
            try
            {
                array = onlineApplication.OnlineDevice.ReadApplicationList();
            }
            catch
            {
                return;
            }
            foreach (IOnlineApplicationObject item in childApplicationObjects)
            {
                if (!typeof(IApplicationObject).IsAssignableFrom(((object)item).GetType()))
                {
                    continue;
                }
                string[] array2 = array;
                foreach (string a in array2)
                {
                    if (!(item is IOnlineApplicationObject2) || !string.Equals(a, ((IOnlineApplicationObject2)((item is IOnlineApplicationObject2) ? item : null)).ApplicationName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    try
                    {
                        if (CanLogout(item.ApplicationGuid))
                        {
                            Logout(item.ApplicationGuid);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void Logout(Guid appObjectGuid)
        {
            if (!CanLogout(appObjectGuid))
            {
                throw new InvalidOperationException("Logout is not possible in this state.");
            }
            GetApplication(appObjectGuid).Logout();
            Guid deviceAppGuidOfApplication = GetDeviceAppGuidOfApplication(appObjectGuid);
            if (deviceAppGuidOfApplication != Guid.Empty && CanLogout(deviceAppGuidOfApplication) && !GetDeviceAppSubApplications(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, deviceAppGuidOfApplication).Any((Guid g) => IsLoggedIn(g)))
            {
                GetApplication(deviceAppGuidOfApplication).Logout();
            }
        }

        public static void Start(Guid appObjectGuid)
        {
            if (!CanStart(appObjectGuid))
            {
                throw new InvalidOperationException("Start not possible in this state.");
            }
            GetApplication(appObjectGuid).Start();
        }

        public static void SingleCycle(Guid appObjectGuid)
        {
            if (!CanSingleCycle(appObjectGuid))
            {
                throw new InvalidOperationException("Single Cycle not possible in this state.");
            }
            GetApplication(appObjectGuid).SingleCycle();
        }

        public static void Stop(Guid appObjectGuid)
        {
            if (!CanStop(appObjectGuid))
            {
                throw new InvalidOperationException("Stop not possible in this state.");
            }
            GetApplication(appObjectGuid).Stop();
        }

        public static void Download(Guid appObjectGuid, bool bOnlineChange)
        {
            ActivateMessageView();
            try
            {
                Download(appObjectGuid, bOnlineChange, bCheck: true, (LoginServiceFlags)1, (BootProjectTransferMode)0);
            }
            catch (UnresolvedReferencesException)
            {
            }
        }

        internal static bool CheckMinimumRequirementsForSwitchingOperatingMode(Guid guidApplication)
        {
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && guidApplication != Guid.Empty)
            {
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guidApplication).Object;
                IOnlineApplicationObject val = (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                if (val != null)
                {
                    IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, val.DeviceGuid).Object;
                    IDeviceObject13 val2 = (IDeviceObject13)(object)((object2 is IDeviceObject13) ? object2 : null);
                    if (val2 == null)
                    {
                        return false;
                    }
                    if (((IDeviceObject)val2).DeviceIdentification.Type == 4098)
                    {
                        return false;
                    }
                    if (GetRuntimeVersion(((IDeviceObject)val2).DeviceIdentification) < new Version("3.5.5.0"))
                    {
                        return false;
                    }
                    if (!(((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(val.DeviceGuid) is IOnlineDevice17))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        internal static bool CanSwitchOperatingMode(DeviceOperatingMode mode, Guid guidApplication, out IOnlineDevice17 onldev)
        {
            //IL_00d9: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ef: Expected I4, but got Unknown
            //IL_00fb: Unknown result type (might be due to invalid IL or missing references)
            //IL_0101: Invalid comparison between Unknown and I4
            //IL_0104: Unknown result type (might be due to invalid IL or missing references)
            //IL_010a: Invalid comparison between Unknown and I4
            //IL_011b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0121: Invalid comparison between Unknown and I4
            onldev = null;
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && guidApplication != Guid.Empty)
            {
                int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guidApplication).Object;
                IOnlineApplicationObject val = (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                if (val != null)
                {
                    IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, val.DeviceGuid).Object;
                    IDeviceObject13 val2 = (IDeviceObject13)(object)((object2 is IDeviceObject13) ? object2 : null);
                    if (val2 == null)
                    {
                        return false;
                    }
                    if (((IDeviceObject)val2).DeviceIdentification.Type == 4098)
                    {
                        return false;
                    }
                    if (GetRuntimeVersion(((IDeviceObject)val2).DeviceIdentification) < new Version("3.5.5.0"))
                    {
                        return false;
                    }
                    IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(val.DeviceGuid);
                    IOnlineDevice17 val3 = (IOnlineDevice17)(object)((onlineDevice is IOnlineDevice17) ? onlineDevice : null);
                    if (val3 != null)
                    {
                        onldev = val3;
                        IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guidApplication);
                        if (application != null)
                        {
                            switch ((int)mode)
                            {
                                default:
                                    return false;
                                case 1:
                                    if (application.IsLoggedIn)
                                    {
                                        if ((int)val3.OperatingMode != 2)
                                        {
                                            return (int)val3.OperatingMode == 3;
                                        }
                                        return true;
                                    }
                                    return true;
                                case 2:
                                case 3:
                                    if (application.IsLoggedIn)
                                    {
                                        return (int)val3.OperatingMode == 1;
                                    }
                                    return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static void SetOperatingMode(DeviceOperatingMode mode, IOnlineDevice17 onldev)
        {
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || onldev == null)
            {
                throw new InvalidOperationException("Changing mode not possible in this state.");
            }
            object obj = new object();
            try
            {
                ((IOnlineDevice3)onldev).SharedConnect(obj);
                onldev.SwitchOperatingMode(mode);
            }
            finally
            {
                ((IOnlineDevice3)onldev).SharedDisconnect(obj);
            }
        }

        internal static Version GetRuntimeVersion(IDeviceIdentification ident)
        {
            ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(ident);
            if (targetSettingsById == null)
            {
                return new Version(0, 0, 0, 0);
            }
            string stringValue = LocalTargetSettings.RuntimeVersion.GetStringValue(targetSettingsById);
            try
            {
                return new Version(stringValue);
            }
            catch
            {
            }
            return new Version(0, 0, 0, 0);
        }

        private static IMetaObject FindDeviceObject(int nProjectHandle, Guid appObjectGuid)
        {
            if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, appObjectGuid))
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, appObjectGuid);
                while (!(objectToRead.Object is IDeviceObject))
                {
                    if (objectToRead.ParentObjectGuid == Guid.Empty)
                    {
                        return null;
                    }
                    objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, objectToRead.ParentObjectGuid);
                    if (objectToRead == null)
                    {
                        return null;
                    }
                }
                return objectToRead;
            }
            return null;
        }

        private static void OnlineChangeDetailsClickHandler(object sender, EventArgs e)
        {
            OnlineChangeEventArgs onlineChangeEventArgs = e as OnlineChangeEventArgs;
            if (onlineChangeEventArgs != null)
            {
                OnlineChangeInformationDialog onlineChangeInformationDialog = new OnlineChangeInformationDialog();
                onlineChangeInformationDialog.Initialize(onlineChangeEventArgs._ocd, onlineChangeEventArgs._comconNew);
                onlineChangeInformationDialog.ShowDialog(sender as IWin32Window);
            }
        }

        private static void Download(Guid appObjectGuid, bool bOnlineChange, bool bCheck, LoginServiceFlags flags, BootProjectTransferMode transferMode)
        {
            //IL_0057: Unknown result type (might be due to invalid IL or missing references)
            //IL_005d: Unknown result type (might be due to invalid IL or missing references)
            //IL_005f: Invalid comparison between Unknown and I4
            //IL_0066: Unknown result type (might be due to invalid IL or missing references)
            //IL_006c: Unknown result type (might be due to invalid IL or missing references)
            //IL_006e: Invalid comparison between Unknown and I4
            //IL_0073: Unknown result type (might be due to invalid IL or missing references)
            //IL_0075: Unknown result type (might be due to invalid IL or missing references)
            //IL_0077: Invalid comparison between Unknown and I4
            //IL_010a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0110: Invalid comparison between Unknown and I4
            //IL_0136: Unknown result type (might be due to invalid IL or missing references)
            //IL_0180: Unknown result type (might be due to invalid IL or missing references)
            //IL_018b: Unknown result type (might be due to invalid IL or missing references)
            //IL_018e: Invalid comparison between Unknown and I4
            //IL_0192: Unknown result type (might be due to invalid IL or missing references)
            //IL_01e3: Unknown result type (might be due to invalid IL or missing references)
            //IL_0202: Unknown result type (might be due to invalid IL or missing references)
            //IL_0208: Unknown result type (might be due to invalid IL or missing references)
            //IL_02c3: Expected O, but got Unknown
            //IL_02fd: Unknown result type (might be due to invalid IL or missing references)
            //IL_0303: Invalid comparison between Unknown and I4
            //IL_0318: Expected O, but got Unknown
            //IL_03aa: Expected O, but got Unknown
            //IL_0682: Unknown result type (might be due to invalid IL or missing references)
            if (bCheck && !CanDownload(appObjectGuid))
            {
                throw new InvalidOperationException("Download is not possible in this state.");
            }
            IOnlineApplication application = GetApplication(appObjectGuid);
            Debug.Assert(application != null);
            ((IObjectManager)APEnvironment.ObjectMgr).FinishLoadProject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle);
            IOnlineApplication11 val = (IOnlineApplication11)(object)((application is IOnlineApplication11) ? application : null);
            bool flag = true;
            bool flag2 = true;
            bool bSuppressAllDialogs = ((int)flags & 0x100) > 0;
            bool flag3 = ((int)flags & 0x200) > 0;
            bool bForceOnlineChange = ((int)flags & 2) > 0;
            EventHandler<BeforeGenerateRelinkCodeEventArgs> eventHandler = null;
            try
            {
                bool chanceledOnlineChange = false;
                eventHandler = delegate (object sender, BeforeGenerateRelinkCodeEventArgs e)
                {
                    OnBeforeGenerateRelinkCode(sender, e, bSuppressAllDialogs, bForceOnlineChange, out chanceledOnlineChange);
                };
                if (val != null && bOnlineChange && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)0, (ushort)0))
                {
                    ((ILanguageModelManager26)APEnvironment.LanguageModelMgr).BeforeGenerateRelinkCode += (eventHandler);
                    IOnlineChangeDetails val2 = default(IOnlineChangeDetails);
                    if (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GenerateOnlineChangeCode(appObjectGuid, false, out val2))
                    {
                        if (bSuppressAllDialogs || chanceledOnlineChange)
                        {
                            application.Logout();
                            return;
                        }
                        if ((int)Prompt("QueryCompileErrors", (PromptChoice)2, (PromptResult)2, true, application) == 3)
                        {
                            application.Logout();
                            return;
                        }
                        flag = false;
                        flag2 = false;
                    }
                }
            }
            finally
            {
                ((ILanguageModelManager26)APEnvironment.LanguageModelMgr).BeforeGenerateRelinkCode -= (eventHandler);
            }
            if (!bOnlineChange && !ConfirmDeleteApplications(application, bUserMayCancelDownload: true, flags))
            {
                if (application != null && application.IsLoggedIn)
                {
                    application.Logout();
                }
                return;
            }
            Guid guid = Guid.Empty;
            try
            {
                APEnvironment.MessageStorage.ClearMessages((IMessageCategory)(object)DownloadMessageCategory.Singleton);
                if (!flag2)
                {
                    return;
                }
                if (application.OnlineDevice is IOnlineDevice15)
                {
                    ((IOnlineDevice15)application.OnlineDevice).AreStatusAndMonitoringUpdatesSuspended = (true);
                }
                bool flag4 = (int)transferMode == 2;
                if ((int)transferMode == 0)
                {
                    IOnlineApplicationObject3 onlineApplicationObject = GetOnlineApplicationObject(application.ApplicationGuid);
                    IOnlineApplicationObject5 val3 = (IOnlineApplicationObject5)(object)((onlineApplicationObject is IOnlineApplicationObject5) ? onlineApplicationObject : null);
                    if (val3 != null && ((val3.BootApplicationSettings.CreateBootApplicationOnDownload && !bOnlineChange) || (val3.BootApplicationSettings.CreateBootApplicationOnOnlineChange && bOnlineChange)))
                    {
                        flag4 = true;
                    }
                }
                if (application.OnlineDevice is IOnlineDevice10)
                {
                    guid = ((IOnlineDevice10)application.OnlineDevice).DeviceObjectGuid;
                }
                if (application is IOnlineApplication19)
                {
                    APEnvironment.OnlineUIMgr.BeginAllowOnlineModification();
                    try
                    {
                        ((IOnlineApplication19)application).Download(bOnlineChange, transferMode);
                        if (flag4)
                        {
                            try
                            {
                                DownloadSystemApplications((IOnlineApplication7)(object)((application is IOnlineApplication7) ? application : null));
                            }
                            catch
                            {
                            }
                        }
                    }
                    finally
                    {
                        APEnvironment.OnlineUIMgr.EndAllowOnlineModification();
                    }
                }
                else
                {
                    APEnvironment.OnlineUIMgr.BeginAllowOnlineModification();
                    try
                    {
                        application.Download(bOnlineChange);
                        if (flag4)
                        {
                            try
                            {
                                DownloadSystemApplications((IOnlineApplication7)(object)((application is IOnlineApplication7) ? application : null));
                            }
                            catch
                            {
                            }
                        }
                    }
                    finally
                    {
                        APEnvironment.OnlineUIMgr.EndAllowOnlineModification();
                    }
                }
                if (guid == Guid.Empty || !SourceDownload.SourceCodesDownloadedToDevice.Contains(guid))
                {
                    try
                    {
                        new SourceDownload().DownloadFromProgramDownload();
                    }
                    catch
                    {
                    }
                }
            }
            catch (CancelledByUserException)
            {
                if (application.IsLoggedIn)
                {
                    application.Logout();
                }
            }
            catch (OperationCanceledByUserNotificationFromPlc)
            {
                if (application.IsLoggedIn)
                {
                    application.Logout();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (GenerateCodeFailedException val6)
            {
                GenerateCodeFailedException val7 = val6;
                if (flag3)
                {
                    RemoveApplicationSilently(application);
                    application.Logout();
                    throw val7;
                }
                if (bSuppressAllDialogs)
                {
                    RemoveApplicationSilently(application);
                    application.Logout();
                }
                else if (flag && (int)Prompt("QueryCompileErrors", (PromptChoice)2, (PromptResult)2, true, application) == 3)
                {
                    RemoveApplicationSilently(application);
                    application.Logout();
                }
            }
            catch (UnresolvedSystemApplicationsException val8)
            {
                UnresolvedSystemApplicationsException val9 = val8;
                if (!flag3)
                {
                    APEnvironment.MessageService.Error(((Exception)(object)val9).Message, "ErrorUnresolvedSystemApplicationsException", Array.Empty<object>());
                }
                string unresolvedSystemApplication = Strings.UnresolvedSystemApplication;
                APEnvironment.MessageStorage.ClearMessages((IMessageCategory)(object)DownloadMessageCategory.Singleton);
                if (val9.ApplicationNames != null)
                {
                    string[] applicationNames = val9.ApplicationNames;
                    foreach (string arg in applicationNames)
                    {
                        APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DownloadMessageCategory.Singleton, (IMessage)(object)new DownloadMessage(string.Format(unresolvedSystemApplication, arg)));
                    }
                }
                application.Logout();
                if (flag3)
                {
                    throw val9;
                }
            }
            catch (UnresolvedReferencesException val10)
            {
                UnresolvedReferencesException val11 = val10;
                string unresolvedReference = Strings.UnresolvedReference;
                string signatureMismatch = Strings.SignatureMismatch;
                string versionMismatch = Strings.VersionMismatch;
                APEnvironment.MessageStorage.ClearMessages((IMessageCategory)(object)DownloadMessageCategory.Singleton);
                UnresolvedReferencesException2 val12 = (UnresolvedReferencesException2)(object)((val11 is UnresolvedReferencesException2) ? val11 : null);
                if (val11.UnresolvedReferences != null)
                {
                    string[] applicationNames = val11.UnresolvedReferences;
                    foreach (string arg2 in applicationNames)
                    {
                        APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DownloadMessageCategory.Singleton, (IMessage)(object)new DownloadMessage(string.Format(unresolvedReference, arg2)));
                    }
                }
                if (val12 != null)
                {
                    if (val12.SignatureMismatches != null)
                    {
                        SignatureMismatch[] signatureMismatches = val12.SignatureMismatches;
                        foreach (SignatureMismatch val13 in signatureMismatches)
                        {
                            APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DownloadMessageCategory.Singleton, (IMessage)(object)new DownloadMessage(string.Format(signatureMismatch, val13.ExtRef, val13.SignatureRequested, val13.SignatureImplemented)));
                        }
                    }
                }
                else if (val11.SignatureMismatches != null)
                {
                    string[] applicationNames = val11.SignatureMismatches;
                    foreach (string arg3 in applicationNames)
                    {
                        APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DownloadMessageCategory.Singleton, (IMessage)(object)new DownloadMessage(string.Format(signatureMismatch, arg3, 0, 0)));
                    }
                }
                if (val12 != null && val12.VersionMismatches != null)
                {
                    VersionMismatch[] versionMismatches = val12.VersionMismatches;
                    foreach (VersionMismatch val14 in versionMismatches)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("{0}.{1}.{2}.{3}", (val14.VersionRequested >> 24) & 0xFFu, (val14.VersionRequested >> 16) & 0xFFu, (val14.VersionRequested >> 8) & 0xFFu, val14.VersionRequested & 0xFFu);
                        StringBuilder stringBuilder2 = new StringBuilder();
                        stringBuilder2.AppendFormat("{0}.{1}.{2}.{3}", (val14.VersionImplemented >> 24) & 0xFFu, (val14.VersionImplemented >> 16) & 0xFFu, (val14.VersionImplemented >> 8) & 0xFFu, val14.VersionImplemented & 0xFFu);
                        APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DownloadMessageCategory.Singleton, (IMessage)(object)new DownloadMessage(string.Format(versionMismatch, val14.ExtRef, stringBuilder, stringBuilder2)));
                    }
                }
                if (!flag3)
                {
                    APEnvironment.MessageService.Error(((Exception)(object)val11).Message, "ErrorUnresolvedReferencesException", Array.Empty<object>());
                }
                throw val11;
            }
            finally
            {
                if (SourceDownload.SourceCodesDownloadedToDevice.Contains(guid))
                {
                    SourceDownload.SourceCodesDownloadedToDevice.Remove(guid);
                }
                if (application.OnlineDevice is IOnlineDevice15)
                {
                    ((IOnlineDevice15)application.OnlineDevice).AreStatusAndMonitoringUpdatesSuspended = (false);
                }
            }
        }

        private static bool OnBeforeGenerateRelinkCode(object sender, BeforeGenerateRelinkCodeEventArgs e, bool bSuppressAllDialogs, bool bForceOnlineChange, out bool chanceledOC)
        {
            chanceledOC = false;
            if (e.OnlineChangeDetails != null && ((IOnlineChangeDetails)e.OnlineChangeDetails).VariablesAffected.Count > 0)
            {
                bool flag = WarnUserAboutOnlineChangeConsequences(GetApplication(((CompileEventArgs)e).ApplicationGuid), (IOnlineChangeDetails)(object)e.OnlineChangeDetails, bSuppressAllDialogs, bForceOnlineChange);
                chanceledOC = !flag;
                if (!flag)
                {
                    e.Cancel((Exception)new ApplicationException("The user cancelled the online change"));
                }
            }
            return true;
        }

        private static bool WarnUserAboutOnlineChangeConsequences(IOnlineApplication app, IOnlineChangeDetails ocd, bool bSuppressAllDialogs, bool bForceOnlineChange)
        {
            int num = 0;
            int num2 = 0;
            ICompileContext compileContext = APEnvironment.LanguageModelMgr.GetCompileContext(app.ApplicationGuid);
            ICompileContext7 compileContext2 = compileContext as ICompileContext7;
            foreach (IVariableInfo variableInfo in ocd.VariablesAffected)
            {
                if (compileContext != null)
                {
                    ISignature signature = compileContext.CreateGlobalIScope()[variableInfo.SignatureId];
                    IVariable var = null;
                    if (signature != null)
                    {
                        var = signature[variableInfo.VariableId];
                    }
                    if (VisibilityUtil.IsHiddenSignature(signature) || VisibilityUtil.IsHiddenVariable(signature, var, GUIHidingFlags.AllCommon))
                    {
                        continue;
                    }
                }
                if ((variableInfo.Flags & VarFlag.LocationChanged) == VarFlag.LocationChanged)
                {
                    num++;
                }
                else if ((variableInfo.Flags & VarFlag.OnlChangeReInit) == VarFlag.OnlChangeReInit)
                {
                    num2++;
                }
            }
            if (num2 > 0 || num > 0)
            {
                int num3 = 0;
                if (ocd is IOnlineChangeDetails2)
                {
                    num3 = (ocd as IOnlineChangeDetails2).InterfacesToTest.Count;
                }
                string stMessage = string.Format(Strings.QueryLocationChangedFullDownload, num, num2, num3);
                if (bSuppressAllDialogs && !bForceOnlineChange)
                {
                    app.Logout();
                    return false;
                }
                if (!bSuppressAllDialogs && !bForceOnlineChange)
                {
                    PromptResult promptResult;
                    if (APEnvironment.Engine.MessageService is IMessageService5 && compileContext2 != null && ocd is IOnlineChangeDetails2)
                    {
                        promptResult = (APEnvironment.Engine.MessageService as IMessageService5).PromptWithDetails(stMessage, PromptChoice.YesNo, PromptResult.Yes, new EventHandler(OnlineCommandHelper.OnlineChangeDetailsClickHandler), new OnlineCommandHelper.OnlineChangeEventArgs(ocd as IOnlineChangeDetails2, compileContext2), "QueryLocationChangedFullDownload", Array.Empty<object>());
                    }
                    else
                    {
                        promptResult = APEnvironment.MessageService.Prompt(stMessage, PromptChoice.YesNo, PromptResult.Yes, "QueryLocationChangedFullDownload", Array.Empty<object>());
                    }
                    if (promptResult != PromptResult.Yes)
                    {
                        app.Logout();
                        return false;
                    }
                }
            }
            return true;
        }

        private static void RemoveApplicationSilently(IOnlineApplication app)
        {
            try
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && app.OnlineDevice != null && app.OnlineDevice.IsConnected && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, app.ApplicationGuid))
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, app.ApplicationGuid);
                    if (app.OnlineDevice != null)
                    {
                        app.OnlineDevice.DeleteApplication(metaObjectStub.Name);
                    }
                }
            }
            catch
            {
            }
        }

        public static void WriteValues()
        {
            WriteOrForceValues(bForce: false);
        }

        public static void ForceValues()
        {
            WriteOrForceValues(bForce: true);
        }

        private static void WriteOrForceValues(bool bForce)
        {
            IOnlineApplication[] onlineApps = null;
            try
            {
                onlineApps = GetAllOnlineApplications();
            }
            catch (Exception ex)
            {
                APEnvironment.MessageService.Error(ex.Message, "WriteOrForceValuesFailed03", Array.Empty<object>());
            }
            WriteOrForceValues(bForce, onlineApps);
        }

        public static void WriteOrForceValues(bool bForce, IOnlineApplication[] onlineApps)
        {
            try
            {
                Debug.Assert(onlineApps != null);
                StringCollection stringCollection = new StringCollection();
                StringCollection stringCollection2 = new StringCollection();
                foreach (IOnlineApplication val in onlineApps)
                {
                    try
                    {
                        IOnlineVarRef[] preparedVarRefs = val.PreparedVarRefs;
                        if (preparedVarRefs == null)
                        {
                            continue;
                        }
                        if (bForce)
                        {
                            val.ForceVariables(preparedVarRefs);
                        }
                        else
                        {
                            val.WriteVariables(preparedVarRefs);
                        }
                        IOnlineVarRef[] array = preparedVarRefs;
                        foreach (IOnlineVarRef val2 in array)
                        {
                            if (val2.PreparedValue == null)
                            {
                                continue;
                            }
                            string value = ((IExprement)val2.Expression).ToString();
                            if (val2.Forced && !bForce)
                            {
                                if (!stringCollection2.Contains(value))
                                {
                                    stringCollection2.Add(value);
                                }
                            }
                            else if (!stringCollection.Contains(value))
                            {
                                stringCollection.Add(((IExprement)val2.Expression).ToString());
                            }
                        }
                    }
                    catch (CancelledByUserException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex2)
                    {
                        APEnvironment.MessageService.Error(ex2.Message, "WriteOrForceValuesFailed", Array.Empty<object>());
                    }
                }
                if (stringCollection.Count > 0)
                {
                    string text = ((!bForce) ? CreateFailedVarsMessages(stringCollection, "WriteFailedForSomeVariables") : CreateFailedVarsMessages(stringCollection, "ForceFailedForSomeVariables"));
                    APEnvironment.MessageService.Warning(text, "WriteOrForceValuesFailed01", Array.Empty<object>());
                }
                if (stringCollection2.Count > 0)
                {
                    string text2 = CreateFailedVarsMessages(stringCollection2, "WriteFailedForSomeForcedVariables");
                    APEnvironment.MessageService.Warning(text2, "WriteOrForceValuesFailed02", Array.Empty<object>());
                }
            }
            catch (CancelledByUserException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex4)
            {
                APEnvironment.MessageService.Error(ex4.Message, "WriteOrForceValuesFailed03", Array.Empty<object>());
            }
        }

        public static void ForceActiveApplication()
        {
            WriteOrForceActiveApplication(bForce: true);
        }

        public static void WriteActiveApplication()
        {
            WriteOrForceActiveApplication(bForce: false);
        }

        private static void GetAllPreparedVarReferences(LList<IOnlineVarRef> varrefs, IOnlineApplication onlineValuesApp)
        {
            varrefs.AddRange((IEnumerable<IOnlineVarRef>)onlineValuesApp.PreparedVarRefs);
            Guid parentObjectGuid = GetParentObjectGuid(onlineValuesApp.ApplicationGuid);
            if (parentObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(parentObjectGuid);
                if (application != null)
                {
                    GetAllPreparedVarReferences(varrefs, application);
                }
            }
        }

        private static void WriteOrForceApplication(bool bForce, IOnlineApplication onlineValuesApp)
        {
            if (onlineValuesApp == null || !onlineValuesApp.IsLoggedIn)
            {
                return;
            }
            StringCollection stringCollection = new StringCollection();
            StringCollection stringCollection2 = new StringCollection();
            LList<IOnlineVarRef> val = new LList<IOnlineVarRef>();
            GetAllPreparedVarReferences(val, onlineValuesApp);
            if (val != null)
            {
                if (bForce)
                {
                    onlineValuesApp.ForceVariables(val.ToArray());
                }
                else
                {
                    onlineValuesApp.WriteVariables(val.ToArray());
                }
                foreach (IOnlineVarRef item in val)
                {
                    if (item.PreparedValue == null)
                    {
                        continue;
                    }
                    IOnlineVarRef obj = ((item is IOnlineVarRef8) ? item : null);
                    string value = ((obj != null) ? ((IOnlineVarRef8)obj).DisplayExpression : null);
                    if (string.IsNullOrEmpty(value))
                    {
                        value = ((IExprement)item.Expression).ToString();
                    }
                    if (item.Forced && !bForce)
                    {
                        if (!stringCollection2.Contains(value))
                        {
                            stringCollection2.Add(value);
                        }
                    }
                    else if (!stringCollection.Contains(value))
                    {
                        stringCollection.Add(value);
                    }
                }
            }
            if (stringCollection.Count > 0)
            {
                string text = ((!bForce) ? CreateFailedVarsMessages(stringCollection, "WriteFailedForSomeVariables") : CreateFailedVarsMessages(stringCollection, "ForceFailedForSomeVariables"));
                APEnvironment.MessageService.Warning(text, "ForceOrWriteFailedActiveApp01", Array.Empty<object>());
            }
            if (stringCollection2.Count > 0)
            {
                string text2 = CreateFailedVarsMessages(stringCollection2, "WriteFailedForSomeForcedVariables");
                APEnvironment.MessageService.Warning(text2, "ForceOrWriteFailedActiveApp02", Array.Empty<object>());
            }
        }

        private static void WriteOrForceActiveApplication(bool bForce)
        {
            try
            {
                Guid activeApplication = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication;
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(activeApplication);
                WriteOrForceApplication(bForce, application);
            }
            catch (CancelledByUserException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex2)
            {
                APEnvironment.MessageService.Error(ex2.Message, "ForceOrWriteFailedActiveApp03", Array.Empty<object>());
            }
        }

        public static void UnforceApplication(IOnlineApplication onlappl, StringCollection stillForcedVars)
        {
            if (onlappl == null || !onlappl.IsLoggedIn)
            {
                return;
            }
            Guid parentObjectGuid = GetParentObjectGuid(onlappl.ApplicationGuid);
            if (parentObjectGuid != Guid.Empty)
            {
                UnforceApplication(((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(parentObjectGuid), stillForcedVars);
            }
            if (onlappl.ReleaseForceValues())
            {
                return;
            }
            IOnlineVarRef[] preparedVarRefs = onlappl.PreparedVarRefs;
            foreach (IOnlineVarRef val in preparedVarRefs)
            {
                if (val.PreparedValue != null)
                {
                    string value = ((IExprement)val.Expression).ToString();
                    if (!stillForcedVars.Contains(value))
                    {
                        stillForcedVars.Add(((IExprement)val.Expression).ToString());
                    }
                }
            }
        }

        public static void UnforceActiveApplication()
        {
            UnforceSelectedApplication(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.ActiveApplication);
        }

        public static void UnforceSelectedApplication(Guid guidSelectedApp)
        {
            IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(guidSelectedApp);
            StringCollection stringCollection = new StringCollection();
            try
            {
                UnforceApplication(application, stringCollection);
            }
            catch
            {
            }
            if (stringCollection.Count > 0)
            {
                string text = CreateFailedVarsMessages(stringCollection, "UnforceFailedForSomeVariables");
                APEnvironment.MessageService.Warning(text, "UnforceFailedActiveApp", Array.Empty<object>());
            }
        }

        public static void UnforceValues()
        {
            try
            {
                IOnlineApplication[] allOnlineApplications = GetAllOnlineApplications();
                Debug.Assert(allOnlineApplications != null);
                StringCollection stringCollection = new StringCollection();
                IOnlineApplication[] array = allOnlineApplications;
                foreach (IOnlineApplication val in array)
                {
                    try
                    {
                        if (val.ReleaseForceValues())
                        {
                            continue;
                        }
                        IOnlineVarRef[] preparedVarRefs = val.PreparedVarRefs;
                        foreach (IOnlineVarRef val2 in preparedVarRefs)
                        {
                            if (val2.PreparedValue != null)
                            {
                                string value = ((IExprement)val2.Expression).ToString();
                                if (!stringCollection.Contains(value))
                                {
                                    stringCollection.Add(((IExprement)val2.Expression).ToString());
                                }
                            }
                        }
                    }
                    catch (CancelledByUserException)
                    {
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex2)
                    {
                        APEnvironment.MessageService.Error(ex2.Message, "UnforceValuesFailed01", Array.Empty<object>());
                    }
                }
                if (stringCollection.Count > 0)
                {
                    string text = CreateFailedVarsMessages(stringCollection, "UnforceFailedForSomeVariables");
                    APEnvironment.MessageService.Warning(text, "UnforceValuesFailed02", Array.Empty<object>());
                }
            }
            catch (CancelledByUserException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex4)
            {
                APEnvironment.MessageService.Error(ex4.Message, "UnforceValuesFailed03", Array.Empty<object>());
            }
        }

        internal static bool CheckForcedValues(Guid appObjectGuid)
        {
            IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
            IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(primaryProject.Handle, appObjectGuid);
            Debug.Assert(metaObjectStub != null && typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType));
            IOnlineApplication5 onlineApplication = OnlineCommandHelper.GetApplication(appObjectGuid) as IOnlineApplication5;
            if (onlineApplication == null || !onlineApplication.IsLoggedIn || (onlineApplication.OperatingState & OperatingState.force_active) == OperatingState.none)
            {
                return true;
            }
            PromptResult promptResult = APEnvironment.MessageService.Prompt(Strings.PromptLogoutAppWithForcedValues, PromptChoice.YesNoCancel, PromptResult.Cancel, "onl_cmds_prompt_logout_forced_vars", Array.Empty<object>());
            if (promptResult == PromptResult.Yes)
            {
                onlineApplication.ReleaseForceValues();
                return true;
            }
            return promptResult == PromptResult.No;
        }

        private static string CreateFailedVarsMessages(StringCollection failedVars, string stMessageIdentifier)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            StringEnumerator enumerator = failedVars.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (num < 10)
                    {
                        stringBuilder.AppendFormat("     {0}\r\n", current);
                        num++;
                        continue;
                    }
                    stringBuilder.Append("     <...>\r\n");
                    break;
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
            return string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), stMessageIdentifier), stringBuilder.ToString());
        }

        internal static void ResetApplication(Guid guidApplication, ResetOption resettype)
        {
            IOnlineApplication application = OnlineCommandHelper.GetApplication(guidApplication);
            if (!application.IsLoggedIn)
            {
                return;
            }
            IBP[] breakpoints = APEnvironment.BPMgr.GetBreakpoints(guidApplication);
            LDictionary<IBP, bool> ldictionary = new LDictionary<IBP, bool>();
            foreach (IBP ibp in breakpoints)
            {
                if (ibp.IsEnabled)
                {
                    ldictionary.Add(ibp, (ibp as IBP6).Locked);
                }
                (ibp as IBP6).Locked = false;
                APEnvironment.BPMgr.DisableBreakpoint(ibp);
            }
            if (application.ApplicationState == ApplicationState.halt_on_bp)
            {
                if (OnlineCommandHelper.HasTaskKillTargetSetting(guidApplication))
                {
                    if (PromptResult.Yes == APEnvironment.MessageService.Prompt(Strings.QueryTerminateCycleReset_Text, PromptChoice.YesNo, PromptResult.Yes, "QueryTerminateCycleReset_Text", Array.Empty<object>()))
                    {
                        application.SingleCycle();
                    }
                }
                else
                {
                    application.SingleCycle();
                }
            }
            OnlineCommandHelper.ResetPreparedValues(application);
            application.Reset(resettype);
            if (OptionsHelper.RestoreBreakpointsAfterReset && resettype != ResetOption.Original)
            {
                foreach (IBP ibp2 in ldictionary.Keys)
                {
                    try
                    {
                        APEnvironment.BPMgr.EnableBreakpoint(ibp2);
                        if (ldictionary[ibp2])
                        {
                            (ibp2 as IBP6).Locked = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void ResetPreparedValues(IOnlineApplication app)
        {
            if (app != null)
            {
                IOnlineVarRef[] preparedVarRefs = app.PreparedVarRefs;
                for (int i = 0; i < preparedVarRefs.Length; i++)
                {
                    preparedVarRefs[i].PreparedValue = ((object)null);
                }
            }
        }

        internal static bool IsPlcLogicGuid(Guid appObjectGuid)
        {
            if (appObjectGuid != Guid.Empty)
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject != null)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(primaryProject.Handle, appObjectGuid);
                    Debug.Assert(metaObjectStub != null);
                    Type objectType = metaObjectStub.ObjectType;
                    Debug.Assert(objectType != null);
                    return typeof(IPlcLogicObject).IsAssignableFrom(objectType);
                }
            }
            return false;
        }

        internal static string GetParentName(Guid appObjectGuid)
        {
            Guid parentObjectGuid = GetParentObjectGuid(appObjectGuid);
            if (parentObjectGuid == Guid.Empty)
            {
                return null;
            }
            try
            {
                IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                if (primaryProject != null)
                {
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, parentObjectGuid).Object;
                    IOnlineApplicationObject val = (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                    if (val == null)
                    {
                        return null;
                    }
                    IOnlineApplicationObject2 val2 = (IOnlineApplicationObject2)(object)((val is IOnlineApplicationObject2) ? val : null);
                    if (val2 == null)
                    {
                        return val.Name;
                    }
                    return val2.ApplicationName;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        internal static IOnlineApplicationObject GetOnlineApplicationObject0(Guid appObjectGuid)
        {
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject != null)
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, appObjectGuid);
                Debug.Assert(objectToRead != null);
                IObject @object = objectToRead.Object;
                return (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
            }
            return null;
        }

        internal static IOnlineApplicationObject3 GetOnlineApplicationObject(Guid appObjectGuid)
        {
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject != null)
            {
                IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, appObjectGuid);
                Debug.Assert(objectToRead != null);
                IObject @object = objectToRead.Object;
                return (IOnlineApplicationObject3)(object)((@object is IOnlineApplicationObject3) ? @object : null);
            }
            return null;
        }

        internal static IList<IOnlineApplicationObject> GetChildApplicationObjects(Guid appObjectGuid)
        {
            List<IOnlineApplicationObject> list = new List<IOnlineApplicationObject>();
            try
            {
                Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle);
                foreach (Guid guid in allObjects)
                {
                    IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guid);
                    if (typeof(IOnlineApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guid).Object;
                        IOnlineApplicationObject val = (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                        if (val.ParentApplicationGuid == appObjectGuid)
                        {
                            list.Add(val);
                        }
                    }
                }
                return list;
            }
            catch
            {
                return list;
            }
        }

        internal static Guid GetParentObjectGuid(Guid appObjectGuid)
        {
            try
            {
                if (appObjectGuid != Guid.Empty)
                {
                    IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                    if (primaryProject != null)
                    {
                        IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, appObjectGuid);
                        Debug.Assert(objectToRead != null);
                        IObject @object = objectToRead.Object;
                        IOnlineApplicationObject val = (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                        if (val == null)
                        {
                            return Guid.Empty;
                        }
                        return val.ParentApplicationGuid;
                    }
                }
                return Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        internal static IOnlineApplicationObject OnlApplObject(Guid guid)
        {
            try
            {
                if (guid != Guid.Empty)
                {
                    IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                    if (primaryProject != null)
                    {
                        IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, guid).Object;
                        return (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        internal static Guid GetVisualManagerObjectFromApplication(int nProjectHandle, Guid appGuid)
        {
            try
            {
                if (appGuid != Guid.Empty)
                {
                    Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nProjectHandle);
                    foreach (Guid guid in allObjects)
                    {
                        IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
                        if (typeof(IVisualManagerObject).IsAssignableFrom(metaObjectStub.ObjectType) && metaObjectStub.ParentObjectGuid == appGuid)
                        {
                            return guid;
                        }
                    }
                }
                return Guid.Empty;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        internal static bool OnlineChangeSupported(Guid guidApplication)
        {
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return false;
            }
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            bool result = true;
            IMetaObject val = FindDeviceObject(handle, guidApplication);
            if (val == null)
            {
                return result;
            }
            if (IsDeviceApplication(guidApplication))
            {
                return false;
            }
            IObject @object = val.Object;
            IDeviceObject val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
            IDeviceIdentification deviceIdentification = val2.DeviceIdentification;
            foreach (IDeviceFeatureChecker item in APEnvironment.CreateDeviceFeatureCheckers())
            {
                try
                {
                    bool flag = false;
                    if (!item.CheckFeatureSupport(val2, (DeviceFeatureEnum)0, out flag))
                    {
                        if (flag)
                        {
                            return false;
                        }
                        result = false;
                        goto IL_00a4;
                    }
                }
                catch
                {
                }
            }
            goto IL_00a4;
        IL_00a4:
            ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(deviceIdentification);
            if (LocalTargetSettings.OnlineChangeSupported.GetBoolValue(targetSettingsById))
            {
                return !LocalTargetSettings.CompactDownload.GetBoolValue(targetSettingsById);
            }
            return false;
        }

        internal static bool AlwaysDownload(Guid guid)
        {
            if (!OnlineChangeSupported(guid))
            {
                return true;
            }
            IOnlineApplicationObject val = OnlApplObject(guid);
            if (val == null || !(val is IOnlineApplicationObject3))
            {
                return false;
            }
            return !((IOnlineApplicationObject3)((val is IOnlineApplicationObject3) ? val : null)).OnlineChangePossible;
        }

        internal static string GetApplicationNameByGuid(Guid guid)
        {
            try
            {
                if (guid != Guid.Empty)
                {
                    IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
                    if (primaryProject != null)
                    {
                        IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, guid).Object;
                        IOnlineApplicationObject val = (IOnlineApplicationObject)(object)((@object is IOnlineApplicationObject) ? @object : null);
                        IOnlineApplicationObject2 val2 = (IOnlineApplicationObject2)(object)((val is IOnlineApplicationObject2) ? val : null);
                        if (val2 == null)
                        {
                            return val.Name;
                        }
                        return val2.ApplicationName;
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string GetDevicePrefixedApplicationName(Guid objectGuid)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (objectGuid != Guid.Empty)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, objectGuid);
                Debug.Assert(metaObjectStub != null);
                if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    if (stringBuilder.Length == 0)
                    {
                        stringBuilder.Insert(0, metaObjectStub.Name);
                    }
                    else
                    {
                        stringBuilder.Insert(0, metaObjectStub.Name + ".");
                    }
                }
                objectGuid = metaObjectStub.ParentObjectGuid;
            }
            return stringBuilder.ToString();
        }

        internal static Guid GetGuidFromArguments(string stGuid)
        {
            try
            {
                return new Guid(stGuid);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        internal static IOnlineApplication[] GetAllOnlineApplications()
        {
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            ArrayList arrayList = new ArrayList();
            if (primaryProject != null)
            {
                Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(primaryProject.Handle);
                Debug.Assert(allObjects != null);
                Guid[] array = allObjects;
                foreach (Guid guid in array)
                {
                    try
                    {
                        IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(primaryProject.Handle, guid);
                        Debug.Assert(metaObjectStub != null);
                        if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                        {
                            IOnlineApplication application = GetApplication(guid);
                            if (application != null && application.IsLoggedIn)
                            {
                                arrayList.Add(application);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            IOnlineApplication[] array2 = (IOnlineApplication[])(object)new IOnlineApplication[arrayList.Count];
            arrayList.CopyTo(array2);
            return array2;
        }

        private static void ActivateMessageView()
        {
            if (((IEngine)APEnvironment.Engine).Frame != null)
            {
                IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews(GUID_MESSAGEVIEWFACTORY);
                Debug.Assert(views != null);
                if (views.Length != 0)
                {
                    ((IEngine)APEnvironment.Engine).Frame.ActiveView = (views[0]);
                }
            }
        }

        internal static IOnlineDevice GetOnlineDeviceForApplication(Guid applicationGuid)
        {
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, applicationGuid).Object;
            Guid deviceGuid = ((IOnlineApplicationObject)((@object is IOnlineApplicationObject) ? @object : null)).DeviceGuid;
            return ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(deviceGuid);
        }

        internal static IOnlineDevice GetOnlineDeviceForApplicationFast(Guid guidApp)
        {
            IOnlineDevice result = null;
            if (guidApp != Guid.Empty && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidApp))
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guidApp);
                while (metaObjectStub.ParentObjectGuid != Guid.Empty)
                {
                    metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, metaObjectStub.ParentObjectGuid);
                    if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                    {
                        result = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(metaObjectStub.ObjectGuid);
                        break;
                    }
                }
            }
            return result;
        }

        private static string GetCommandName(ICommand command)
        {
            //IL_0027: Unknown result type (might be due to invalid IL or missing references)
            if (((IEngine)APEnvironment.Engine).CommandManager is ICommandManager3)
            {
                Guid guid = TypeGuidAttribute.FromObject((object)command).Guid;
                return ((ICommandManager3)((IEngine)APEnvironment.Engine).CommandManager).GetCommandName(guid);
            }
            return command.Name;
        }

        internal static bool PromptExecuteOperation_ActiveApplication(ICommand command, bool bPromptInNormalMode, string stCustomMessage)
        {
            Debug.Assert(command != null);
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject == null)
            {
                return true;
            }
            if (IsApplicationInSecureOnlineMode(primaryProject.ActiveApplication))
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: true, stCustomMessage);
            }
            if (bPromptInNormalMode)
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: false, stCustomMessage);
            }
            return true;
        }

        internal static bool PromptExecuteOperation_SpecificApplication(Guid guidSelectedApplication, ICommand command, bool bPromptInNormalMode, string stCustomMessage)
        {
            Debug.Assert(command != null);
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return true;
            }
            if (guidSelectedApplication == Guid.Empty)
            {
                return true;
            }
            if (IsApplicationInSecureOnlineMode(guidSelectedApplication))
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: true, stCustomMessage);
            }
            if (bPromptInNormalMode)
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: false, stCustomMessage);
            }
            return true;
        }

        internal static bool PromptExecuteOperation_SelectedDevice(ICommand command, bool bPromptInNormalMode)
        {
            Debug.Assert(command != null);
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
            {
                return true;
            }
            if (IsDeviceInSecureOnlineMode(((IObject)GetSelectedDevice()).MetaObject.ObjectGuid))
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: true, null);
            }
            if (bPromptInNormalMode)
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: false, null);
            }
            return true;
        }

        internal static bool PromptExecuteOperation_MultipleApplications(ICommand command, IEnumerable<Guid> applicationGuids, bool bPromptInNormalMode)
        {
            Debug.Assert(command != null);
            Debug.Assert(applicationGuids != null);
            bool flag = false;
            bool flag2 = false;
            foreach (Guid applicationGuid in applicationGuids)
            {
                flag = true;
                if (IsApplicationInSecureOnlineMode(applicationGuid))
                {
                    flag2 = true;
                    break;
                }
            }
            if (!flag)
            {
                return true;
            }
            if (flag2)
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: true, null);
            }
            if (bPromptInNormalMode)
            {
                return PromptExecuteOperation(command, bSecureOnlineMode: false, null);
            }
            return true;
        }

        private static bool IsApplicationInSecureOnlineMode(Guid applicationGuid)
        {
            //IL_00b0: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f4: Unknown result type (might be due to invalid IL or missing references)
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject == null)
            {
                return false;
            }
            if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(primaryProject.Handle, applicationGuid))
            {
                return false;
            }
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, applicationGuid);
            Debug.Assert(objectToRead != null);
            IObject @object = objectToRead.Object;
            IApplicationObject val = (IApplicationObject)(object)((@object is IApplicationObject) ? @object : null);
            if (val == null)
            {
                return false;
            }
            if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(primaryProject.Handle, ((IOnlineApplicationObject)val).DeviceGuid))
            {
                return false;
            }
            IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, ((IOnlineApplicationObject)val).DeviceGuid);
            Debug.Assert(objectToRead2 != null);
            IObject object2 = objectToRead2.Object;
            IDeviceObject val2 = (IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null);
            if (val2 == null)
            {
                return false;
            }
            _3S.CoDeSys.DeviceObject.ICommunicationSettings communicationSettings = val2.CommunicationSettings;
            if (communicationSettings != null && communicationSettings is ICommunicationSettings2)
            {
                return ((ICommunicationSettings2)communicationSettings).SecureOnlineMode;
            }
            if (communicationSettings != null && communicationSettings is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)communicationSettings).IsFunctionAvailable("GetSecureOnlineMode"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
                return XmlConvert.ToBoolean(((IGenericInterfaceExtensionProvider)communicationSettings).CallFunction("GetSecureOnlineMode", xmlDocument).DocumentElement.InnerText);
            }
            return false;
        }

        private static bool IsDeviceInSecureOnlineMode(Guid deviceGuid)
        {
            //IL_0067: Unknown result type (might be due to invalid IL or missing references)
            //IL_007e: Unknown result type (might be due to invalid IL or missing references)
            //IL_00a8: Unknown result type (might be due to invalid IL or missing references)
            IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
            if (primaryProject == null)
            {
                return false;
            }
            if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(primaryProject.Handle, deviceGuid))
            {
                return false;
            }
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(primaryProject.Handle, deviceGuid);
            Debug.Assert(objectToRead != null);
            IObject @object = objectToRead.Object;
            IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
            if (val == null)
            {
                return false;
            }
            _3S.CoDeSys.DeviceObject.ICommunicationSettings communicationSettings = val.CommunicationSettings;
            if (communicationSettings != null && communicationSettings is ICommunicationSettings2)
            {
                return ((ICommunicationSettings2)communicationSettings).SecureOnlineMode;
            }
            if (communicationSettings != null && communicationSettings is IGenericInterfaceExtensionProvider && ((IGenericInterfaceExtensionProvider)communicationSettings).IsFunctionAvailable("GetSecureOnlineMode"))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateElement("Input"));
                return XmlConvert.ToBoolean(((IGenericInterfaceExtensionProvider)communicationSettings).CallFunction("GetSecureOnlineMode", xmlDocument).DocumentElement.InnerText);
            }
            return false;
        }

        private static bool PromptExecuteOperation(ICommand command, bool bSecureOnlineMode, string stCustomMessage)
        {
            //IL_003b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0043: Unknown result type (might be due to invalid IL or missing references)
            //IL_004e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0054: Invalid comparison between Unknown and I4
            Debug.Assert(command != null);
            string empty = string.Empty;
            empty = ((stCustomMessage == null || !(stCustomMessage != string.Empty)) ? string.Format(Strings.PromptExecuteOperation, GetCommandName(command)) : stCustomMessage);
            PromptResult val = (PromptResult)(bSecureOnlineMode ? 3 : 2);
            return (int)APEnvironment.MessageService.Prompt(empty, (PromptChoice)2, val, "PromptExecuteOperation", Array.Empty<object>()) == 2;
        }

        public static bool HasInstancePath(IMetaObjectStub mos, Guid onlineApplicationGuid)
        {
            //IL_006d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0073: Expected O, but got Unknown
            if (mos == null)
            {
                throw new ArgumentNullException("mos");
            }
            int num = ((((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null) ? ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle : (-1));
            if (num < 0)
            {
                return false;
            }
            ArrayList arrayList = new ArrayList();
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid);
            do
            {
                if (objectToRead.Object is IOnlineApplicationObject)
                {
                    IOnlineApplicationObject val = (IOnlineApplicationObject)objectToRead.Object;
                    arrayList.Add(val.ApplicationGuid);
                    break;
                }
                if (objectToRead.ParentObjectGuid == Guid.Empty)
                {
                    break;
                }
                objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid);
                Debug.Assert(objectToRead != null);
            }
            while (objectToRead != null);
            if (arrayList.Count == 0)
            {
                GetAllAppObjectGuids(num, arrayList);
            }
            for (int num2 = arrayList.Count - 1; num2 >= 0; num2--)
            {
                if (!IsApplicationOnline((Guid)arrayList[num2]))
                {
                    arrayList.RemoveAt(num2);
                }
            }
            IVariable[] array2 = default(IVariable[]);
            ISignature[] array3 = default(ISignature[]);
            foreach (Guid item in arrayList)
            {
                if (!(onlineApplicationGuid == Guid.Empty) && !(onlineApplicationGuid == item))
                {
                    continue;
                }
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(num, item);
                Debug.Assert(metaObjectStub != null);
                GetSignatureName(metaObjectStub);
                ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(item);
                ISignature parentsignature = null;
                ISignature val2 = ((referenceContextIfAvailable != null) ? FindSignature(referenceContextIfAvailable, mos, out parentsignature) : null);
                if (val2 != null)
                {
                    string[] array = null;
                    array = ((!(referenceContextIfAvailable is ICompileContext13)) ? referenceContextIfAvailable.InstancePaths(val2, out array2, out array3) : ((ICompileContext8)((referenceContextIfAvailable is ICompileContext13) ? referenceContextIfAvailable : null)).InstancePaths(val2, out array2, out array3, true, false, true));
                    if (array != null && array.Length != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ActiveWaitWithTimeout(int nDelayMs)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, nDelayMs);
            DateTime now = DateTime.Now;
            while (DateTime.Now - now < timeSpan)
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }

        public static bool IsUsedOnline(IMetaObjectStub mos)
        {
            if (mos == null)
            {
                throw new ArgumentNullException("mos");
            }
            if (typeof(ITransientObject).IsAssignableFrom(mos.ObjectType) && !typeof(IOnlineUsedTransientObject).IsAssignableFrom(mos.ObjectType))
            {
                return false;
            }
            bool flag = false;
            if (typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType))
            {
                flag = true;
            }
            if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null || mos.ProjectHandle != ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle)
            {
                return false;
            }
            Guid guid = Guid.Empty;
            for (IMetaObjectStub val = mos; val != null; val = ((!(val.ParentObjectGuid != Guid.Empty)) ? null : ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val.ProjectHandle, val.ParentObjectGuid)))
            {
                if (typeof(IApplicationObject).IsAssignableFrom(val.ObjectType))
                {
                    if (IsApplicationOnline(val.ObjectGuid))
                    {
                        return true;
                    }
                    if (guid == Guid.Empty)
                    {
                        guid = val.ObjectGuid;
                        break;
                    }
                }
            }
            for (IMetaObjectStub val = mos; val != null; val = ((!(val.ParentObjectGuid != Guid.Empty)) ? null : ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(val.ProjectHandle, val.ParentObjectGuid)))
            {
                if (typeof(IDeviceObject).IsAssignableFrom(val.ObjectType))
                {
                    List<Guid> list = new List<Guid>();
                    CollectChildApplications(val, list);
                    if (list != null)
                    {
                        foreach (Guid item in list)
                        {
                            if (IsApplicationOnline(item) && (item == guid || flag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            if (CollectInstancePaths(mos, Guid.Empty, out var result, out var _, out var _, out var _) && result != null && result.Length != 0)
            {
                return true;
            }
            return false;
        }

        public static bool CollectInstancePaths(IMetaObjectStub mos, Guid onlineApplicationGuid, out string[] result, out IList<Guid> guidApps, out string implementation, out ISignature parentsign)
        {
            //IL_007f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0086: Expected O, but got Unknown
            parentsign = null;
            implementation = null;
            guidApps = (IList<Guid>)new LList<Guid>();
            if (mos == null)
            {
                throw new ArgumentNullException("mos");
            }
            int num = ((((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null) ? ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle : (-1));
            if (num < 0)
            {
                result = null;
                return false;
            }
            ArrayList arrayList = new ArrayList();
            IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid);
            do
            {
                if (objectToRead.Object is IOnlineApplicationObject)
                {
                    IOnlineApplicationObject val = (IOnlineApplicationObject)objectToRead.Object;
                    arrayList.Add(val.ApplicationGuid);
                    break;
                }
                if (objectToRead.ParentObjectGuid == Guid.Empty)
                {
                    break;
                }
                objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid);
                Debug.Assert(objectToRead != null);
            }
            while (objectToRead != null);
            if (arrayList.Count == 0)
            {
                GetAllAppObjectGuids(num, arrayList);
            }
            for (int num2 = arrayList.Count - 1; num2 >= 0; num2--)
            {
                if (!IsApplicationOnline((Guid)arrayList[num2]))
                {
                    arrayList.RemoveAt(num2);
                }
            }
            StringCollection stringCollection = new StringCollection();
            new LList<Guid>();
            IVariable[] array2 = default(IVariable[]);
            ISignature[] array3 = default(ISignature[]);
            foreach (Guid item in arrayList)
            {
                if (!(onlineApplicationGuid == Guid.Empty) && !(onlineApplicationGuid == item))
                {
                    continue;
                }
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(num, item);
                Debug.Assert(metaObjectStub != null);
                string signatureName = GetSignatureName(metaObjectStub);
                ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(item);
                ISignature val2 = ((referenceContextIfAvailable != null) ? FindSignature(referenceContextIfAvailable, mos, out parentsign) : null);
                if (val2 != null)
                {
                    string[] array = null;
                    array = ((!(referenceContextIfAvailable is ICompileContext13)) ? referenceContextIfAvailable.InstancePaths(val2, out array2, out array3) : ((ICompileContext8)((referenceContextIfAvailable is ICompileContext13) ? referenceContextIfAvailable : null)).InstancePaths(val2, out array2, out array3, true, true, true));
                    if (array != null)
                    {
                        string[] array4 = array;
                        foreach (string arg in array4)
                        {
                            string value = $"{signatureName}.{arg}";
                            stringCollection.Add(value);
                            guidApps.Add(item);
                        }
                    }
                    string text = string.Empty;
                    if (referenceContextIfAvailable is ICompileContext8 && !string.IsNullOrEmpty(val2.LibraryPath))
                    {
                        text = ((ICompileContext8)((referenceContextIfAvailable is ICompileContext8) ? referenceContextIfAvailable : null)).GetLibraryNamespace(val2.LibraryPath);
                        if (!string.IsNullOrEmpty(text))
                        {
                            text += ".";
                        }
                    }
                    if (parentsign == null)
                    {
                        implementation = $"{signatureName}.{text}{val2.OrgName}";
                        continue;
                    }
                    implementation = $"{signatureName}.{text}{parentsign.OrgName}.{val2.OrgName}";
                }
                else if (typeof(IVisualObject).IsAssignableFrom(mos.ObjectType))
                {
                    string value2 = $"{signatureName}.{mos.Name}";
                    stringCollection.Add(value2);
                    guidApps.Add(Guid.Empty);
                }
                else if (typeof(IPropertyObject).IsAssignableFrom(mos.ObjectType))
                {
                    IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mos.ProjectHandle, mos.ParentObjectGuid);
                    string value3 = $"{signatureName}.{metaObjectStub2.Name}";
                    stringCollection.Add(value3);
                }
            }
            result = new string[stringCollection.Count];
            stringCollection.CopyTo(result, 0);
            return arrayList.Count > 0;
        }

        public static string GetSignatureName(IMetaObjectStub mos)
        {
            Debug.Assert(mos != null);
            string text = GetSignatureNameByObjectGuid(mos.ProjectHandle, mos.ObjectGuid);
            if (text == null)
            {
                text = mos.Name;
            }
            StringBuilder stringBuilder = new StringBuilder(text);
            if (typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType))
            {
                Guid deviceObjectGuid = GetDeviceObjectGuid(mos.ProjectHandle, mos.ObjectGuid);
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mos.ProjectHandle, deviceObjectGuid);
                Debug.Assert(metaObjectStub != null);
                stringBuilder.Insert(0, metaObjectStub.Name + ".");
            }
            else
            {
                while (mos.ParentObjectGuid != Guid.Empty)
                {
                    mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mos.ProjectHandle, mos.ParentObjectGuid);
                    Debug.Assert(mos != null);
                    if (typeof(IDeviceObject).IsAssignableFrom(mos.ObjectType) || typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType))
                    {
                        break;
                    }
                    stringBuilder.Insert(0, mos.Name + ".");
                }
            }
            return stringBuilder.ToString();
        }

        internal static ISelectDeviceService GetCustomSelectDeviceProvider()
        {
            ICustomSelectDeviceServiceProvider val = APEnvironment.TryCreateFirstCustomSelectDeviceServiceProvider();
            if (val != null)
            {
                return val.GetCustomSelectDeviceService();
            }
            return null;
        }

        private static void GetAllAppObjectGuids(int nProjectHandle, ArrayList collToFill)
        {
            if (collToFill == null)
            {
                throw new ArgumentNullException("collToFill");
            }
            collToFill.Clear();
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(nProjectHandle);
            Debug.Assert(allObjects != null);
            Guid[] array = allObjects;
            foreach (Guid guid in array)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, guid);
                Debug.Assert(metaObjectStub != null);
                if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    collToFill.Add(guid);
                }
            }
        }

        internal static bool IsApplicationOnline(Guid appObjectGuid)
        {
            try
            {
                IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(appObjectGuid);
                return application != null && application.IsLoggedIn;
            }
            catch
            {
                return false;
            }
        }

        private static Guid GetDeviceObjectGuid(int nProjectHandle, Guid appObjectGuid)
        {
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, appObjectGuid);
            Debug.Assert(metaObjectStub != null);
            while (metaObjectStub.ParentObjectGuid != Guid.Empty)
            {
                metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
                Debug.Assert(metaObjectStub != null);
                if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
                {
                    return metaObjectStub.ObjectGuid;
                }
            }
            return Guid.Empty;
        }

        private static string GetSignatureNameByObjectGuid(int nProjectHandle, Guid gdObject)
        {
            IProject projectByHandle = ((IEngine)APEnvironment.Engine).Projects.GetProjectByHandle(nProjectHandle);
            if (projectByHandle == null)
            {
                return null;
            }
            IPreCompileContext[] array = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).AllPreCompileContexts(true, true);
            foreach (IPreCompileContext val in array)
            {
                if (val.LibraryPath == projectByHandle.Id || (projectByHandle.Handle == ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle && val.LibraryPath == ""))
                {
                    ISignature signature = ((ICompileContextCommon)val).GetSignature(gdObject);
                    if (signature != null)
                    {
                        return signature.OrgName;
                    }
                }
            }
            return null;
        }

        private static ISignature FindSignature(ICompileContext comcon, IMetaObjectStub mos, out ISignature parentsignature)
        {
            Debug.Assert(comcon != null);
            Debug.Assert(mos != null);
            parentsignature = null;
            ISignature signature = GetSignature(comcon, mos);
            if (signature != null)
            {
                return signature;
            }
            Guid objectGuid = mos.ObjectGuid;
            while (mos.ParentObjectGuid != Guid.Empty)
            {
                mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mos.ProjectHandle, mos.ParentObjectGuid);
                ISignature signature2 = GetSignature(comcon, mos);
                if (signature2 == null)
                {
                    continue;
                }
                ISignature[] subSignatures = signature2.SubSignatures;
                if (subSignatures == null)
                {
                    continue;
                }
                ISignature[] array = subSignatures;
                foreach (ISignature val in array)
                {
                    if (val.ObjectGuid == objectGuid)
                    {
                        parentsignature = signature2;
                        return val;
                    }
                }
            }
            return null;
        }

        private static ISignature GetSignature(ICompileContext comcon, IMetaObjectStub mos)
        {
            //IL_0132: Unknown result type (might be due to invalid IL or missing references)
            IProject project = ((IEngine)APEnvironment.Engine).Projects.GetProjectByHandle(mos.ProjectHandle);
            if (project == null)
            {
                return null;
            }
            string projectId = string.Empty;
            if (!project.Primary)
            {
                if (project.Library)
                {
                    projectId = project.Id;
                }
                else
                {
                    IProjectInfoObject val = (from objectGuid in ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(project.Handle)
                                              select ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(project.Handle, objectGuid) into metaObjectStub
                                              where typeof(IProjectInfoObject).IsAssignableFrom(metaObjectStub.ObjectType)
                                              select ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid) into metaObjectStub
                                              select metaObjectStub.Object).OfType<IProjectInfoObject>().FirstOrDefault();
                    if (val == null)
                    {
                        return null;
                    }
                    string arg = val.GetValue("Title") as string;
                    Version arg2 = ((val.GetValue("Version") is IVersion) ? ((IVersion)val.GetValue("Version")).ToVersion() : null);
                    string arg3 = val.GetValue("Company") as string;
                    projectId = $"{arg}, {arg2} ({arg3})";
                }
            }
            ISignature[] allSignatures = ((ICompileContextCommon)comcon).AllSignatures;
            foreach (ISignature val2 in allSignatures)
            {
                if (val2.ObjectGuid == mos.ObjectGuid && IsLibraryPathMatch(val2.LibraryPath, projectId))
                {
                    return val2;
                }
            }
            return null;
        }

        private static bool IsLibraryPathMatch(string libraryPath, string projectId)
        {
            bool num = string.IsNullOrEmpty(libraryPath);
            bool flag = string.IsNullOrEmpty(projectId);
            if (num)
            {
                return flag;
            }
            if (flag)
            {
                return false;
            }
            return string.Equals(libraryPath, projectId, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCompilable(Guid appObjectGuid)
        {
            ((IObjectManager)APEnvironment.ObjectMgr).FinishLoadProject(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle);
            return ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).Compile(appObjectGuid);
        }

        private static void DetailsClickHandler(object sender, EventArgs e)
        {
            //IL_0057: Unknown result type (might be due to invalid IL or missing references)
            try
            {
                DetailsEventArgs detailsEventArgs = e as DetailsEventArgs;
                Debug.Assert(detailsEventArgs != null);
                if (detailsEventArgs.ApplicationExistsOnDevice)
                {
                    IApplicationInfoObjectProperty applicationInfoObjectProperty = GetApplicationInfoObjectProperty(detailsEventArgs.OnlineApplication.ApplicationGuid);
                    IProjectInfoObject projectInfoObject = GetProjectInfoObject();
                    object obj = s_onlappInfoObject;
                    IOnlineApplicationInfo val = (IOnlineApplicationInfo)((obj is IOnlineApplicationInfo) ? obj : null);
                    if (s_onlappInfoObject == null)
                    {
                        try
                        {
                            if (detailsEventArgs.OnlineApplication is IOnlineApplication10)
                            {
                                val = ((IOnlineApplication10)detailsEventArgs.OnlineApplication).ReadApplicationInfo();
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (val == null)
                    {
                        APEnvironment.MessageService.Error(Strings.CannotReadApplicationInfoFromPLC, "CannotReadApplicationInfoFromPLC01", Array.Empty<object>());
                        return;
                    }
                    int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
                    string text = ((!((IEngine3)APEnvironment.Engine).OEMCustomization.HasValue("Online", "ProjectNameIDE")) ? Path.GetFileNameWithoutExtension(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Path) : ((IEngine3)APEnvironment.Engine).OEMCustomization.GetStringValue("Online", "ProjectNameIDE"));
                    string stApplicationName = text;
                    IOnlineApplicationObject obj3 = OnlApplObject(detailsEventArgs.OnlineApplication.ApplicationGuid);
                    IOnlineApplicationObject2 val2 = (IOnlineApplicationObject2)(object)((obj3 is IOnlineApplicationObject2) ? obj3 : null);
                    if (val2 != null)
                    {
                        stApplicationName = val2.ApplicationName;
                    }
                    string projectName = val.ProjectName;
                    string emptyDownloadTime = Strings.EmptyDownloadTime;
                    string stLastModificationPLC = FormatTimestamp(val.LastModification);
                    string profileName = APEnvironment.ProfileName;
                    string profile = val.Profile;
                    string author = GetAuthor(applicationInfoObjectProperty, projectInfoObject);
                    string author2 = val.Author;
                    string version = GetVersion(applicationInfoObjectProperty, projectInfoObject);
                    string version2 = val.Version;
                    string description = GetDescription(applicationInfoObjectProperty, projectInfoObject);
                    string description2 = val.Description;
                    IOnlineApplication onlineApplication = detailsEventArgs.OnlineApplication;
                    IOnlineApplication14 val3 = (IOnlineApplication14)(object)((onlineApplication is IOnlineApplication14) ? onlineApplication : null);
                    object obj4 = s_onlappContentObject;
                    IApplicationContent val4 = (IApplicationContent)((obj4 is IApplicationContent) ? obj4 : null);
                    if (val3 != null && s_onlappContentObject == null)
                    {
                        try
                        {
                            val4 = val3.ReadApplicationContent();
                        }
                        catch (OnlineManagerException)
                        {
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                    ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(((IOnlineApplication)val3).ApplicationGuid);
                    ICompileContext9 val6 = (ICompileContext9)(object)((compileContext is ICompileContext9) ? compileContext : null);
                    ICompileContext referenceContextIfAvailable = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetReferenceContextIfAvailable(((IOnlineApplication)val3).ApplicationGuid);
                    ICompileContext9 val7 = (ICompileContext9)(object)((referenceContextIfAvailable is ICompileContext9) ? referenceContextIfAvailable : null);
                    IApplicationContent val8 = null;
                    if (val6 != null)
                    {
                        val8 = val6.GetApplicationContent();
                        if (val4 == null && val7 != null && val3 != null && val8 != null)
                        {
                            Guid codeId = ((IOnlineApplication)val3).CodeId;
                            Guid dataId = ((IOnlineApplication)val3).DataId;
                            Guid guid = default(Guid);
                            Guid guid2 = default(Guid);
                            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetDownloadIds(((IOnlineApplication)val3).ApplicationGuid, out guid, out guid2);
                            Guid guid3 = default(Guid);
                            Guid guid4 = default(Guid);
                            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompiledIds(((IOnlineApplication)val3).ApplicationGuid, out guid3, out guid4);
                            if (codeId == guid && dataId == guid2 && (guid4 != guid2 || guid3 != guid))
                            {
                                val4 = val7.GetApplicationContent();
                            }
                        }
                        else if (val4 != null && val8 == null && val7 != null)
                        {
                            val8 = val7.GetApplicationContent();
                        }
                    }
                    IEnumerable<IChangedLMObject> changedObjects = GetChangedObjects(detailsEventArgs.OnlineApplication.ApplicationGuid);
                    ApplicationInfoDialogWithContent applicationInfoDialogWithContent = new ApplicationInfoDialogWithContent();
                    applicationInfoDialogWithContent.Initialize(text, projectName, emptyDownloadTime, stLastModificationPLC, profileName, profile, author, author2, version, version2, description, description2, changedObjects, val8, val4, val6, ((IOnlineApplication)val3).ApplicationGuid, stApplicationName);
                    applicationInfoDialogWithContent.ShowDialog(sender as IWin32Window);
                    return;
                }
                try
                {
                    string[] array = null;
                    IOnlineApplicationInfo[] array2 = null;
                    IOnlineDevice onlineDevice = detailsEventArgs.OnlineApplication.OnlineDevice;
                    IOnlineDevice9 val9 = (IOnlineDevice9)(object)((onlineDevice is IOnlineDevice9) ? onlineDevice : null);
                    if (val9 != null)
                    {
                        ((IOnlineDevice8)val9).ReadApplicationList2(out array, out array2);
                    }
                    string text2;
                    if (array != null && array.Length != 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array2 != null && array2.Length > i && !string.IsNullOrEmpty(array2[i].Version))
                            {
                                stringBuilder.AppendLine(array[i] + ", V" + array2[i].Version.ToString());
                            }
                            else
                            {
                                stringBuilder.AppendLine(array[i]);
                            }
                        }
                        text2 = string.Format(Strings.ApplicationList, stringBuilder.ToString());
                    }
                    else
                    {
                        text2 = Strings.ApplicationListEmpty;
                    }
                    APEnvironment.MessageService.Information(text2, "InfoDetails", Array.Empty<object>());
                }
                catch (Exception ex2)
                {
                    APEnvironment.MessageService.Error(ex2.Message, "ErrorGettingDetails", Array.Empty<object>());
                }
            }
            catch
            {
                APEnvironment.MessageService.Error(Strings.CannotReadApplicationInfoFromPLC, "CannotReadApplicationInfoFromPLC02", Array.Empty<object>());
            }
        }

        internal static string GetAuthor(IApplicationInfoObjectProperty property, IProjectInfoObject pio)
        {
            string text = null;
            if (property != null && property.ContainsValue("Author"))
            {
                text = property.GetValue("Author") as string;
            }
            if (text == null && pio != null && pio.ContainsValue("Author"))
            {
                text = pio.GetValue("Author") as string;
            }
            if (text == null)
            {
                text = string.Empty;
            }
            return text;
        }

        internal static string GetVersion(IApplicationInfoObjectProperty property, IProjectInfoObject pio)
        {
            string text = null;
            if (property != null && property.ContainsValue("Version"))
            {
                text = property.GetValue("Version") as string;
            }
            if (text == null && pio != null && pio.ContainsValue("Version"))
            {
                object value = pio.GetValue("Version");
                IVersion val = (IVersion)((value is IVersion) ? value : null);
                if (val != null)
                {
                    text = ((object)val).ToString();
                }
            }
            if (text == null)
            {
                text = string.Empty;
            }
            return text;
        }

        internal static string GetDescription(IApplicationInfoObjectProperty property, IProjectInfoObject pio)
        {
            string text = null;
            if (property != null && property.ContainsValue("Description"))
            {
                text = property.GetValue("Description") as string;
            }
            if (text == null && pio != null && pio.ContainsValue("Description"))
            {
                text = pio.GetValue("Description") as string;
            }
            if (text == null)
            {
                text = string.Empty;
            }
            return text;
        }

        internal static string GetDetailsFromTargetMismatchException2(TargetMismatchException2 tmex2)
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            //IL_002a: Unknown result type (might be due to invalid IL or missing references)
            //IL_002c: Invalid comparison between Unknown and I4
            //IL_0062: Unknown result type (might be due to invalid IL or missing references)
            //IL_0068: Unknown result type (might be due to invalid IL or missing references)
            //IL_006a: Invalid comparison between Unknown and I4
            //IL_0096: Unknown result type (might be due to invalid IL or missing references)
            //IL_009c: Unknown result type (might be due to invalid IL or missing references)
            //IL_009e: Invalid comparison between Unknown and I4
            if (tmex2 == null)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(((Exception)(object)tmex2).Message);
            stringBuilder.AppendLine();
            if (((int)((TargetMismatchException)tmex2).GetMismatchReason & 1) > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat(Strings.TargetMismatch_Type, tmex2.DeviceIdSettings.Type, ((TargetMismatchException)tmex2).DeviceId.Type);
            }
            if (((int)((TargetMismatchException)tmex2).GetMismatchReason & 2) > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat(Strings.TargetMismatch_Id, tmex2.DeviceIdSettings.Id, ((TargetMismatchException)tmex2).DeviceId.Id);
            }
            if (((int)((TargetMismatchException)tmex2).GetMismatchReason & 4) > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat(Strings.TargetMismatch_Version, tmex2.DeviceIdSettings.Version, ((TargetMismatchException)tmex2).DeviceId.Version);
            }
            return stringBuilder.ToString();
        }

        internal static IProjectInfoObject GetProjectInfoObject()
        {
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
            foreach (Guid guid in allObjects)
            {
                if (typeof(IProjectInfoObject).IsAssignableFrom(((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid).ObjectType))
                {
                    IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, guid).Object;
                    return (IProjectInfoObject)(object)((@object is IProjectInfoObject) ? @object : null);
                }
            }
            return null;
        }

        internal static IApplicationInfoObjectProperty GetApplicationInfoObjectProperty(Guid appObjectGuid)
        {
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, appObjectGuid))
            {
                return null;
            }
            IObjectProperty property = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, appObjectGuid).GetProperty(GUID_APPLICATIONINFOPROPERTY);
            return (IApplicationInfoObjectProperty)(object)((property is IApplicationInfoObjectProperty) ? property : null);
        }

        internal static string FormatTimestamp(DateTime dt)
        {
            if (dt == DateTime.MinValue)
            {
                return string.Empty;
            }
            return dt.ToLongDateString() + " " + dt.ToShortTimeString();
        }

        internal static bool CanFlowControl(Guid appObjectGuid)
        {
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            //IL_002b: Invalid comparison between Unknown and I4
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                IOnlineApplication12 val = (IOnlineApplication12)(object)((application is IOnlineApplication12) ? application : null);
                if (val != null && ((IOnlineApplication)val).IsLoggedIn)
                {
                    return (int)((IOnlineApplication)val).ApplicationState != 3;
                }
                return false;
            }
            return false;
        }

        internal static void ToggleFlowControl(Guid appObjectGuid)
        {
            //IL_006b: Unknown result type (might be due to invalid IL or missing references)
            //IL_008c: Unknown result type (might be due to invalid IL or missing references)
            if (!(appObjectGuid != Guid.Empty))
            {
                return;
            }
            IOnlineApplication application = GetApplication(appObjectGuid);
            IOnlineApplication12 val = (IOnlineApplication12)(object)((application is IOnlineApplication12) ? application : null);
            if (val == null)
            {
                return;
            }
            if (!val.FlowControlEnabled)
            {
                bool flag = false;
                IBP[] breakpoints = ((IBPManager)APEnvironment.BPMgr).GetBreakpoints(appObjectGuid);
                for (int i = 0; i < breakpoints.Length; i++)
                {
                    if (breakpoints[i].IsEnabled)
                    {
                        flag = true;
                    }
                }
                if ((int)APEnvironment.MessageService.Prompt(Strings.FlowControlWarning, (PromptChoice)1, (PromptResult)0, "FlowControlWarning", Array.Empty<object>()) != 0 || (flag && (int)APEnvironment.MessageService.Prompt(Strings.BreakpointsDisablingWarning, (PromptChoice)1, (PromptResult)0, "BreakpointsDisablingWarning", Array.Empty<object>()) != 0))
                {
                    return;
                }
                breakpoints = ((IBPManager)APEnvironment.BPMgr).GetBreakpoints(appObjectGuid);
                foreach (IBP val2 in breakpoints)
                {
                    if (val2.IsEnabled)
                    {
                        ((IBPManager)APEnvironment.BPMgr).DisableBreakpoint(val2);
                    }
                }
            }
            val.FlowControlEnabled = (!val.FlowControlEnabled);
        }

        internal static bool IsFlowControlEnabled(Guid appObjectGuid)
        {
            if (appObjectGuid != Guid.Empty)
            {
                IOnlineApplication application = GetApplication(appObjectGuid);
                IOnlineApplication12 val = (IOnlineApplication12)(object)((application is IOnlineApplication12) ? application : null);
                if (val != null)
                {
                    return val.FlowControlEnabled;
                }
                return false;
            }
            return false;
        }

        internal static bool VerifyRealDeviceName(Guid guidSelectedApplication, bool bIsActiveApplication, out string stActualNodeName)
        {
            //IL_0094: Unknown result type (might be due to invalid IL or missing references)
            //IL_009e: Expected O, but got Unknown
            //IL_00ad: Unknown result type (might be due to invalid IL or missing references)
            stActualNodeName = string.Empty;
            object obj = null;
            IOnlineDevice11 val = null;
            if (bIsActiveApplication)
            {
                if (!IsApplicationInSecureOnlineMode(guidSelectedApplication))
                {
                    return true;
                }
                IDeviceObject activeDevice = GetActiveDevice();
                if (activeDevice != null)
                {
                    IOnlineDevice onlineDevice = ((IOnlineManager)APEnvironment.OnlineMgr).GetOnlineDevice(((IObject)activeDevice).MetaObject.ObjectGuid);
                    val = (IOnlineDevice11)(object)((onlineDevice is IOnlineDevice11) ? onlineDevice : null);
                }
            }
            else
            {
                try
                {
                    IOnlineDevice onlineDeviceForApplication = GetOnlineDeviceForApplication(guidSelectedApplication);
                    val = (IOnlineDevice11)(object)((onlineDeviceForApplication is IOnlineDevice11) ? onlineDeviceForApplication : null);
                }
                catch
                {
                    return false;
                }
            }
            if (val != null)
            {
                try
                {
                    if (!((IOnlineDevice)val).IsConnected)
                    {
                        obj = new object();
                        ((IOnlineDevice3)val).SharedConnect(obj);
                    }
                    string text = default(string);
                    val.GetTargetIdent(out text, out text, out stActualNodeName);
                }
                catch (Exception ex)
                {
                    string text2 = ex.Message;
                    if (ex is TargetMismatchException2)
                    {
                        text2 = GetDetailsFromTargetMismatchException2((TargetMismatchException2)ex);
                    }
                    else if (ex is InteractiveLoginFailedException && ((InteractiveLoginFailedException)ex).ShouldBeHandledSilently)
                    {
                        return false;
                    }
                    APEnvironment.MessageService.Error(text2, "ErrorGettingRealDeviceName", Array.Empty<object>());
                    return false;
                }
                finally
                {
                    if (obj != null && val != null && ((IOnlineDevice)val).IsConnected)
                    {
                        ((IOnlineDevice3)val).SharedDisconnect(obj);
                    }
                }
            }
            return true;
        }

        internal static string GetCreateDirForTemporaryFiles()
        {
            string text = null;
            try
            {
                text = Path.GetTempPath();
                text = Path.Combine(text, ((IEngine3)APEnvironment.Engine).OEMCustomization.ProductPathComponent);
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                return text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Application.StartupPath;
        }

        internal static IList<Guid> CollectCascadingAppsFromArguments(string[] arguments)
        {
            if (arguments == null || arguments.Length != 1)
            {
                throw new ArgumentNullException("arguments", "Exactly one argument expected (parent application GUID)");
            }
            Guid result = Guid.Empty;
            if (!Guid.TryParse(arguments[0], out result) || result == Guid.Empty)
            {
                throw new ArgumentNullException("arguments[0]", "Not a valid GUID.");
            }
            IList<IOnlineApplicationObject> childApplicationObjects = GetChildApplicationObjects(result);
            if (!childApplicationObjects.Any())
            {
                throw new InvalidOperationException("Not a valid parent application.");
            }
            return childApplicationObjects.Select((IOnlineApplicationObject app) => app.ApplicationGuid).ToList();
        }

        internal static bool IsDeviceApplication(Guid g)
        {
            if (g == Guid.Empty)
            {
                return false;
            }
            int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
            if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(handle, g))
            {
                return false;
            }
            IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, g);
            return typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType);
        }

        internal static bool IsPoolPOU(IMetaObjectStub mos)
        {
            bool result = false;
            bool flag = true;
            while (flag)
            {
                if (mos.ParentObjectGuid.Equals(Guid.Empty))
                {
                    result = true;
                    flag = false;
                }
                else if (!typeof(IApplicationObject).IsAssignableFrom(mos.ObjectType))
                {
                    mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(mos.ProjectHandle, mos.ParentObjectGuid);
                }
                else
                {
                    result = (flag = false);
                }
            }
            return result;
        }

        internal static IEnumerable<IChangedLMObject> GetChangedObjects(Guid gdApplication)
        {
            IEnumerable<IChangedLMObject> enumerable = null;
            if (Guid.Empty != gdApplication)
            {
                ILMCompiledApplicationQuery obj = APEnvironment.LMServiceProvider.DownloadedApplicationService.QueryCompiledApplicationSet(gdApplication);
                enumerable = ((obj != null) ? obj.FindChangedObjectsDetailed() : null);
                if (enumerable == null)
                {
                    ILMCompiledApplicationQuery obj2 = APEnvironment.LMServiceProvider.CompileService.QueryCompiledApplicationSet(gdApplication);
                    enumerable = ((obj2 != null) ? obj2.FindChangedObjectsDetailed() : null);
                }
            }
            return enumerable;
        }
    }
}
