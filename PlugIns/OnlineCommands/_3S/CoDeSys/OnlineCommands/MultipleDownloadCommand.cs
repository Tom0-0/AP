#define DEBUG
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.OnlineUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace _3S.CoDeSys.OnlineCommands
{
    [TypeGuid("{FE09B175-5F71-413a-B5ED-253520184B64}")]
    [AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_multiple_download.htm")]
    [AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Multiple_Download.htm")]
    public class MultipleDownloadCommand : IStandardCommand, ICommand
    {
        private class MyMessageService : IMessageService4, IMessageService3, IMessageService2, IMessageService, IGenericInterfaceExtensionProvider
        {
            private IMessageService _originalMessageService;

            private OnlineChangeOption _onlineChange;

            private bool _bInitializePersistentVariables;

            private bool _bDeleteOldApplications;

            private bool _bDoNotReleaseForcedVariables;

            private Guid _currentApplicationGuid;

            private bool _bCancellationRequested;

            private MultipleDownloadResult[] _results;

            private IGenericInterfaceExtensionProvider _basicGenericInterfaceExtensionProvider;

            internal bool CancellationRequested => _bCancellationRequested;

            internal MyMessageService(IMessageService originalMessageService, OnlineChangeOption onlineChange, bool bInitializePersistentVariables, bool bDeleteOldApplications, bool bDoNotReleaseForcedVariables, MultipleDownloadResult[] results)
            {
                //IL_002b: Unknown result type (might be due to invalid IL or missing references)
                //IL_002c: Unknown result type (might be due to invalid IL or missing references)
                if (originalMessageService == null)
                {
                    throw new ArgumentNullException("originalMessageService");
                }
                if (results == null)
                {
                    throw new ArgumentNullException("results");
                }
                _originalMessageService = originalMessageService;
                _onlineChange = onlineChange;
                _bInitializePersistentVariables = bInitializePersistentVariables;
                _bDeleteOldApplications = bDeleteOldApplications;
                _bDoNotReleaseForcedVariables = bDoNotReleaseForcedVariables;
                _results = results;
                _basicGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
            }

            internal void SetCurrentApplication(Guid currentApplicationGuid)
            {
                _currentApplicationGuid = currentApplicationGuid;
            }

            public void Error(string stMessage, string stMessageKey, params object[] messageArguments)
            {
                ErrorWithKey(stMessage, stMessageKey);
            }

            public void Warning(string stMessage, string stMessageKey, params object[] messageArguments)
            {
                Warning(stMessage);
            }

            public void Information(string stMessage, string stMessageKey, params object[] messageArguments)
            {
            }

            public PromptResult Prompt(string stMessage, PromptChoice choice, PromptResult defaultResult, string stMessageKey, params object[] messageArguments)
            {
                //IL_0003: Unknown result type (might be due to invalid IL or missing references)
                //IL_0004: Unknown result type (might be due to invalid IL or missing references)
                return InternalPrompt(stMessageKey, defaultResult);
            }

            public PromptResult Prompt(string stMessage, object[] items, PromptChoice choice, PromptResult defaultResult, out bool[] selection, string stMessageKey, params object[] messageArguments)
            {
                //IL_002a: Unknown result type (might be due to invalid IL or missing references)
                //IL_002c: Unknown result type (might be due to invalid IL or missing references)
                selection = new bool[(items != null) ? items.Length : 0];
                for (int i = 0; i < selection.Length; i++)
                {
                    selection[i] = true;
                }
                return InternalPrompt(stMessageKey, defaultResult);
            }

            public PromptResult Prompt(string stMessage, PromptChoice choice, PromptResult defaultResult, string stStoreResultText, string stStoredResultKey, string stMessageKey, params object[] messageArguments)
            {
                //IL_0003: Unknown result type (might be due to invalid IL or missing references)
                //IL_0004: Unknown result type (might be due to invalid IL or missing references)
                return InternalPrompt(stMessageKey, defaultResult);
            }

            public PromptResult Prompt(string stMessage, PromptChoice choice, PromptResult defaultResult, string stStoreResultText, string stStoredResultKey)
            {
                //IL_0000: Unknown result type (might be due to invalid IL or missing references)
                return defaultResult;
            }

            public void ResetStoredResult(string stStoredResultKey)
            {
            }

            public void Error(string stMessage)
            {
                ErrorWithKey(stMessage, "");
            }

            private void ErrorWithKey(string stMessage, string messageKey)
            {
                stMessage = string.Format(Strings.MultipleDownload_GenericErrorMessage, OnlineCommandHelper.GetDevicePrefixedApplicationName(this._currentApplicationGuid), stMessage);
                MultipleDownloadCommand.StoreResult(this._results, this._currentApplicationGuid, MultipleDownloadResultState.Error, stMessage);
                PromptResult promptResult;
                if (this._originalMessageService is IMessageService3)
                {
                    if (messageKey == "ErrorGeneric_LoginWithParameters03")
                    {
                        promptResult = (this._originalMessageService as IMessageService3).Prompt(stMessage, PromptChoice.YesNo, PromptResult.No, messageKey, Array.Empty<object>());
                    }
                    else
                    {
                        promptResult = (this._originalMessageService as IMessageService3).Prompt(stMessage, PromptChoice.YesNo, PromptResult.No, "Prompt_MultipleDownload_GenericErrorMessage", Array.Empty<object>());
                    }
                }
                else
                {
                    promptResult = this._originalMessageService.Prompt(stMessage, PromptChoice.YesNo, PromptResult.No);
                }
                if (promptResult != PromptResult.Yes)
                {
                    this._bCancellationRequested = true;
                }
            }

            public void Warning(string stMessage)
            {
                stMessage = string.Format(Strings.MultipleDownload_GenericWarningMessage, OnlineCommandHelper.GetDevicePrefixedApplicationName(this._currentApplicationGuid), stMessage);
                PromptResult promptResult;
                if (this._originalMessageService is IMessageService3)
                {
                    promptResult = (this._originalMessageService as IMessageService3).Prompt(stMessage, PromptChoice.YesNo, PromptResult.No, "Prompt_MultipleDownload_GenericWarningMessage", Array.Empty<object>());
                }
                else
                {
                    promptResult = this._originalMessageService.Prompt(stMessage, PromptChoice.YesNo, PromptResult.No);
                }
                if (promptResult != PromptResult.Yes)
                {
                    this._bCancellationRequested = true;
                }
            }

            public void Information(string stMessage)
            {
            }

            public PromptResult Prompt(string stMessage, PromptChoice choice, PromptResult defaultResult)
            {
                //IL_0000: Unknown result type (might be due to invalid IL or missing references)
                return defaultResult;
            }

            public PromptResult Prompt(string stMessage, object[] items, PromptChoice choice, PromptResult defaultResult, out bool[] selection)
            {
                //IL_0027: Unknown result type (might be due to invalid IL or missing references)
                selection = new bool[(items != null) ? items.Length : 0];
                for (int i = 0; i < selection.Length; i++)
                {
                    selection[i] = true;
                }
                return defaultResult;
            }

            private PromptResult InternalPrompt(string stMessageKey, PromptResult defaultResult)
            {
                //IL_0348: Unknown result type (might be due to invalid IL or missing references)
                //IL_034e: Invalid comparison between Unknown and I4
                //IL_037b: Unknown result type (might be due to invalid IL or missing references)
                //IL_03b3: Unknown result type (might be due to invalid IL or missing references)
                switch (stMessageKey)
                {
                    case "PersistenVariablesChanged":
                        if (_bInitializePersistentVariables)
                        {
                            return (PromptResult)2;
                        }
                        return (PromptResult)3;
                    case "QueryCreateDevice":
                    case "QueryCreateApplication":
                    case "QueryDownload":
                        StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)0, null);
                        return (PromptResult)2;
                    case "NoDownloadOnBP":
                        return (PromptResult)2;
                    case "QueryDownloadDevice":
                    case "QueryDownloadAlways":
                    case "QueryNoOnlChange":
                    case "QueryNoOnlChangeWithWarning":
                    case "QueryProgramChanged":
                    case "QueryProgramChangedWithWarning":
                        if ((int)_onlineChange != 2)
                        {
                            StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)3, null);
                            return (PromptResult)2;
                        }
                        StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)4, null);
                        return (PromptResult)3;
                    case "QueryOnlChange":
                        if ((int)_onlineChange != 0)
                        {
                            StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)2, null);
                            return (PromptResult)2;
                        }
                        return (PromptResult)3;
                    case "QueryCompileErrors":
                        return (PromptResult)3;
                    case "PromptDeletePLCApplicationsCancelDownload":
                    case "PromptDeletePLCApplications":
                        if (_bDeleteOldApplications)
                        {
                            return (PromptResult)0;
                        }
                        return (PromptResult)1;
                    case "MultipleDownload_PromptContinue":
                    case "MultipleDownload_PromptContinue01":
                    case "MultipleDownload_PromptContinue02":
                    case "MultipleDownload_PromptContinue03":
                    case "MultipleDownload_PromptContinue04":
                    case "MultipleDownload_PromptContinue05":
                    case "MultipleDownload_PromptContinue06":
                        if (_bCancellationRequested)
                        {
                            return (PromptResult)3;
                        }
                        return (PromptResult)2;
                    default:
                        return defaultResult;
                }
            }

            private int InternalPrompt(string stMessageKey)
            {
                //IL_0020: Unknown result type (might be due to invalid IL or missing references)
                //IL_0025: Unknown result type (might be due to invalid IL or missing references)
                //IL_0026: Unknown result type (might be due to invalid IL or missing references)
                //IL_003c: Expected I4, but got Unknown
                //IL_00a1: Unknown result type (might be due to invalid IL or missing references)
                //IL_00a6: Unknown result type (might be due to invalid IL or missing references)
                //IL_00a7: Unknown result type (might be due to invalid IL or missing references)
                //IL_00b9: Expected I4, but got Unknown
                OnlineChangeOption onlineChange;
                if (!(stMessageKey == "QueryOnlChange2"))
                {
                    if (stMessageKey == "InfoNoOnlineChangeWithForcedVars")
                    {
                        if (_bDoNotReleaseForcedVariables)
                        {
                            StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)5, Strings.MultipleDownload_SkippedDueToForcedVariables);
                            return -1;
                        }
                        onlineChange = _onlineChange;
                        switch ((int)onlineChange)
                        {
                            case 1:
                            case 2:
                                StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)2, null);
                                return 0;
                            case 0:
                                StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)3, null);
                                return 1;
                            default:
                                return -1;
                        }
                    }
                    return -1;
                }
                onlineChange = _onlineChange;
                switch ((int)onlineChange)
                {
                    case 1:
                    case 2:
                        StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)2, null);
                        return 0;
                    case 0:
                        StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)3, null);
                        return 1;
                    case 3:
                        StoreResult(_results, _currentApplicationGuid, (MultipleDownloadResultState)5, null);
                        return -1;
                    default:
                        return -1;
                }
            }

            public void RaiseEvent(string stEvent, XmlDocument eventData)
            {
                if (_basicGenericInterfaceExtensionProvider != null)
                {
                    _basicGenericInterfaceExtensionProvider.RaiseEvent(stEvent, eventData);
                    return;
                }
                throw new NotImplementedException();
            }

            public void AttachToEvent(string stEvent, GenericEventDelegate callback)
            {
                if (_basicGenericInterfaceExtensionProvider != null)
                {
                    _basicGenericInterfaceExtensionProvider.AttachToEvent(stEvent, callback);
                    return;
                }
                throw new NotImplementedException();
            }

            public void DetachFromEvent(string stEvent, GenericEventDelegate callback)
            {
                if (_basicGenericInterfaceExtensionProvider != null)
                {
                    _basicGenericInterfaceExtensionProvider.DetachFromEvent(stEvent, callback);
                    return;
                }
                throw new NotImplementedException();
            }

            public bool IsFunctionAvailable(string stFunction)
            {
                if (stFunction == null)
                {
                    throw new ArgumentNullException("stFunction");
                }
                if (stFunction == "MultipleChoicePrompt")
                {
                    return true;
                }
                return false;
            }

            public XmlDocument CallFunction(string stFunction, XmlDocument functionData)
            {
                if (stFunction == null)
                {
                    throw new ArgumentNullException("stFunction");
                }
                if (functionData == null)
                {
                    throw new ArgumentNullException("functionData");
                }
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateElement("Output"));
                if (stFunction == "MultipleChoicePrompt")
                {
                    string innerText = functionData.DocumentElement["stMessage"].InnerText;
                    List<string> list = new List<string>();
                    foreach (XmlNode childNode in functionData.DocumentElement["options"].ChildNodes)
                    {
                        if (childNode is XmlElement)
                        {
                            list.Add(((XmlElement)childNode).InnerText);
                        }
                    }
                    int nInitialSelection = XmlConvert.ToInt32(functionData.DocumentElement["nInitialSelection"].InnerText);
                    bool bCancellable = XmlConvert.ToBoolean(functionData.DocumentElement["bCancellable"].InnerText);
                    string innerText2 = functionData.DocumentElement["stMessageKey"].InnerText;
                    List<string> list2 = new List<string>();
                    foreach (XmlNode childNode2 in functionData.DocumentElement["messageArguments"].ChildNodes)
                    {
                        if (childNode2 is XmlElement)
                        {
                            list2.Add(((XmlElement)childNode2).InnerText);
                        }
                    }
                    int num = MultipleChoicePrompt(innerText, list.ToArray(), nInitialSelection, bCancellable, innerText2, list2.ToArray());
                    xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(num);
                    return xmlDocument;
                }
                throw new InvalidOperationException("Unknown function.");
            }

            public int MultipleChoicePrompt(string stMessage, string[] options, int nInitialSelection, bool bCancellable, string stMessageKey, params object[] messageArguments)
            {
                return InternalPrompt(stMessageKey);
            }
        }

        private class MyMessageCategory : IMessageCategory
        {
            internal static readonly IMessageCategory SINGLETON = (IMessageCategory)(object)new MyMessageCategory();

            public string Text => Strings.MultipleDownload_MessageCategory;

            public Icon Icon => null;
        }

        private class MyMessage : IMessage
        {
            private string _stText;

            private Severity _severity;

            public int ProjectHandle => -1;

            public Guid ObjectGuid => Guid.Empty;

            public long Position => -1L;

            public short PositionOffset => 0;

            public short Length => 0;

            public string Text => _stText;

            public Severity Severity => _severity;

            internal MyMessage(string stText, Severity severity)
            {
                //IL_000e: Unknown result type (might be due to invalid IL or missing references)
                //IL_000f: Unknown result type (might be due to invalid IL or missing references)
                _stText = stText;
                _severity = severity;
            }
        }

        private Dictionary<string, IMultipleDownloadExtension> _pendingExtensions = new Dictionary<string, IMultipleDownloadExtension>();

        private static readonly string[] BATCH_COMMAND = new string[2] { "online", "multidownload" };

        private static readonly Guid GUID_BUILDMESSAGECATEGORY = new Guid("{97F48D64-A2A3-4856-B640-75C046E37EA9}");

        internal static readonly string DUMMY = "<DUMMY>";

        internal static readonly string SELECTED = "SELECTED";

        internal static readonly string UNSELECTED = "UNSELECTED";

        internal static readonly string ARG_ONLINE_CHANGE_TRY = "online-change=try";

        internal static readonly string ARG_ONLINE_CHANGE_FORCE = "online-change=force";

        internal static readonly string ARG_ONLINE_CHANGE_NEVER = "online-change=never";

        internal static readonly string ARG_ONLINE_CHANGE_KEEP = "online-change=keep";

        public Guid Category => OnlineCommandCategory.Guid;

        public string Name => Strings.MultipleDownloadCommand_Name;

        public string Description => Strings.MultipleDownloadCommand_Description;

        public string ToolTipText => Name;

        public Icon SmallIcon => null;

        public Icon LargeIcon => SmallIcon;

        public bool Enabled
        {
            get
            {
                if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
                {
                    return false;
                }
                return OnlineFeatureHelper.ApplicationCount > 0;
            }
        }

        public string[] BatchCommand => BATCH_COMMAND;

        public string[] CreateBatchArguments()
        {
            //IL_015a: Unknown result type (might be due to invalid IL or missing references)
            //IL_015f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0161: Unknown result type (might be due to invalid IL or missing references)
            //IL_0174: Expected I4, but got Unknown
            _pendingExtensions.Clear();
            MultipleDownloadDialog multipleDownloadDialog = new MultipleDownloadDialog();
            if (multipleDownloadDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) == DialogResult.OK)
            {
                List<Guid> list = new List<Guid>();
                List<IMultipleDownloadExtension> list2 = new List<IMultipleDownloadExtension>();
                IMultipleDownloadItem[] selectedItems = multipleDownloadDialog.SelectedItems;
                foreach (IMultipleDownloadItem multipleDownloadItem in selectedItems)
                {
                    Guid item = Guid.Empty;
                    IMultipleDownloadExtension item2 = null;
                    if (multipleDownloadItem is ApplicationMultipleDownloadItem)
                    {
                        item = ((ApplicationMultipleDownloadItem)multipleDownloadItem).ApplicationGuid;
                    }
                    else if (multipleDownloadItem is ExtensionMultipleDownloadItem)
                    {
                        item2 = ((ExtensionMultipleDownloadItem)multipleDownloadItem).Extension;
                    }
                    list.Add(item);
                    list2.Add(item2);
                }
                if (!OnlineCommandHelper.PromptExecuteOperation_MultipleApplications((ICommand)(object)this, list, bPromptInNormalMode: false))
                {
                    return null;
                }
                List<string> list3 = new List<string>();
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] != Guid.Empty)
                    {
                        string devicePrefixedApplicationName = OnlineCommandHelper.GetDevicePrefixedApplicationName(list[j]);
                        list3.Add(devicePrefixedApplicationName);
                    }
                    else if (list2[j] != null)
                    {
                        string text = $"{list2[j].Provider.Name}.{list2[j].Name}";
                        string arg = Uri.EscapeDataString(text);
                        _pendingExtensions[text] = list2[j];
                        list3.Add($"extension={arg}");
                    }
                }
                OnlineChangeOption onlineChangeOption = multipleDownloadDialog.OnlineChangeOption;
                switch ((int)onlineChangeOption)
                {
                    case 1:
                        list3.Add(ARG_ONLINE_CHANGE_TRY);
                        break;
                    case 2:
                        list3.Add(ARG_ONLINE_CHANGE_FORCE);
                        break;
                    case 0:
                        list3.Add(ARG_ONLINE_CHANGE_NEVER);
                        break;
                }
                if (multipleDownloadDialog.InitPersistentVars)
                {
                    list3.Add("init-persistent-vars");
                }
                if (multipleDownloadDialog.DeleteOldApplications)
                {
                    list3.Add("delete-old-apps");
                }
                if (multipleDownloadDialog.StartAllApplications)
                {
                    list3.Add("start-all-apps");
                }
                if (multipleDownloadDialog.DoNotReleaseForcedVariables)
                {
                    list3.Add("do-not-release-forced-variables");
                }
                return list3.ToArray();
            }
            return null;
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
            //IL_002e: Unknown result type (might be due to invalid IL or missing references)
            //IL_00f8: Unknown result type (might be due to invalid IL or missing references)
            //IL_011d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0142: Unknown result type (might be due to invalid IL or missing references)
            //IL_0167: Unknown result type (might be due to invalid IL or missing references)
            //IL_02f7: Unknown result type (might be due to invalid IL or missing references)
            //IL_0346: Unknown result type (might be due to invalid IL or missing references)
            //IL_0377: Unknown result type (might be due to invalid IL or missing references)
            //IL_0409: Unknown result type (might be due to invalid IL or missing references)
            //IL_0433: Unknown result type (might be due to invalid IL or missing references)
            //IL_0493: Unknown result type (might be due to invalid IL or missing references)
            //IL_0508: Unknown result type (might be due to invalid IL or missing references)
            //IL_050e: Invalid comparison between Unknown and I4
            //IL_05b7: Unknown result type (might be due to invalid IL or missing references)
            //IL_05bd: Invalid comparison between Unknown and I4
            //IL_067b: Unknown result type (might be due to invalid IL or missing references)
            //IL_06a5: Unknown result type (might be due to invalid IL or missing references)
            //IL_078e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0794: Invalid comparison between Unknown and I4
            //IL_07c3: Unknown result type (might be due to invalid IL or missing references)
            //IL_07da: Unknown result type (might be due to invalid IL or missing references)
            //IL_0825: Unknown result type (might be due to invalid IL or missing references)
            //IL_082f: Expected O, but got Unknown
            //IL_087b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0881: Invalid comparison between Unknown and I4
            //IL_092c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0932: Invalid comparison between Unknown and I4
            //IL_0977: Unknown result type (might be due to invalid IL or missing references)
            //IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0a47: Unknown result type (might be due to invalid IL or missing references)
            //IL_0b71: Unknown result type (might be due to invalid IL or missing references)
            //IL_0bad: Unknown result type (might be due to invalid IL or missing references)
            //IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
            //IL_0bfb: Invalid comparison between Unknown and I4
            //IL_0ca2: Unknown result type (might be due to invalid IL or missing references)
            //IL_0ca8: Invalid comparison between Unknown and I4
            //IL_0d81: Unknown result type (might be due to invalid IL or missing references)
            //IL_0d87: Invalid comparison between Unknown and I4
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            List<Guid> list = new List<Guid>();
            List<IMultipleDownloadExtension> list2 = new List<IMultipleDownloadExtension>();
            bool flag = false;
            bool flag2 = false;
            bool bDoNotReleaseForcedVariables = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            OnlineChangeOption val = (OnlineChangeOption)1;
            foreach (string text in arguments)
            {
                switch (text)
                {
                    case "init-persistent-vars":
                        flag = true;
                        continue;
                    case "delete-old-apps":
                        flag2 = true;
                        continue;
                    case "start-all-apps":
                        flag3 = true;
                        continue;
                    case "throw-if-aborted":
                        flag4 = true;
                        continue;
                    case "omit-result-dialog":
                        flag5 = true;
                        continue;
                    case "stay-logged-in":
                        flag6 = true;
                        continue;
                    case "do-not-release-forced-variables":
                        bDoNotReleaseForcedVariables = true;
                        continue;
                }
                if (text.Replace(" ", "") == ARG_ONLINE_CHANGE_TRY)
                {
                    val = (OnlineChangeOption)1;
                    continue;
                }
                if (text.Replace(" ", "") == ARG_ONLINE_CHANGE_FORCE)
                {
                    val = (OnlineChangeOption)2;
                    continue;
                }
                if (text.Replace(" ", "") == ARG_ONLINE_CHANGE_NEVER)
                {
                    val = (OnlineChangeOption)0;
                    continue;
                }
                if (text.Replace(" ", "") == ARG_ONLINE_CHANGE_KEEP)
                {
                    val = (OnlineChangeOption)3;
                    continue;
                }
                IMultipleDownloadExtension item = null;
                Guid empty = Guid.Empty;
                if (text.StartsWith("extension="))
                {
                    string stName = Uri.UnescapeDataString(text.Substring("extension=".Length));
                    item = GetExtension(stName);
                    if (item == null)
                    {
                        APEnvironment.MessageService.Error(string.Format(Strings.SpecifiedDeviceNotFound, text), "SpecifiedDeviceNotFound01", Array.Empty<object>());
                        return;
                    }
                    list.Add(empty);
                    list2.Add(item);
                }
                else
                {
                    empty = GetApplicationGuid(text);
                    if (empty == Guid.Empty)
                    {
                        APEnvironment.MessageService.Error(string.Format(Strings.SpecifiedDeviceNotFound, text), "SpecifiedDeviceNotFound02", Array.Empty<object>());
                        return;
                    }
                    list.Add(empty);
                    list2.Add(item);
                }
            }
            foreach (IMultipleDownloadApplicationHandler item2 in APEnvironment.MultipleDownloadApplicationHandler)
            {
                item2.Initialize(arguments, list.ToArray());
            }
            _pendingExtensions.Clear();
            InternalCodeStateProvider.StopOnlineCodeStateUpdates();
            ((IEngine)APEnvironment.Engine).EditorManager.SaveAllEditors(true);
            MultipleDownloadResult[] array = new MultipleDownloadResult[list.Count];
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j] != Guid.Empty)
                {
                    array[j] = default(MultipleDownloadResult);
                    array[j].SetApplication(list[j]);
                    array[j].ErrorMessage = null;
                    array[j].State = (MultipleDownloadResultState)6;
                    continue;
                }
                if (list2[j] != null)
                {
                    array[j] = default(MultipleDownloadResult);
                    array[j].SetExtension(list2[j]);
                    array[j].ErrorMessage = null;
                    array[j].State = (MultipleDownloadResultState)6;
                    continue;
                }
                throw new InvalidOperationException("Inconsistent download extensions and applications!");
            }
            MultipleDownloadContext multipleDownloadContext = new MultipleDownloadContext(list, list2, flag, flag2, flag3, bDoNotReleaseForcedVariables, flag4, flag5, val);
            IProgressCallback val2 = ((IEngine)APEnvironment.Engine).StartLengthyOperation();
            IMessageService messageService = ((IEngine)APEnvironment.Engine).MessageService;
            val2.Abortable = (true);
            try
            {
                val2.NextTask(Strings.MultipleDownload_BuildingTaskName, list.Count, Strings.MultipleDownload_BuildingTaskUnit);
                MultipleDownloadResultState state = default(MultipleDownloadResultState);
                for (int k = 0; k < list.Count; k++)
                {
                    bool flag7 = false;
                    foreach (IMultipleDownloadApplicationHandler item3 in APEnvironment.MultipleDownloadApplicationHandler)
                    {
                        bool flag8 = false;
                        string stErrorMessage = "";
                        if (item3.PrepareAndCheck(val2, list[k], out flag8, out state, out stErrorMessage))
                        {
                            flag7 = true;
                            if (!flag8)
                            {
                                StoreResult(array, list[k], state, stErrorMessage);
                                break;
                            }
                            StoreResult(array, list[k], state, stErrorMessage);
                            list2.RemoveAt(k);
                            list.RemoveAt(k--);
                        }
                    }
                    if (flag7)
                    {
                        continue;
                    }
                    if (list[k] != Guid.Empty)
                    {
                        if (val2.Aborting)
                        {
                            break;
                        }
                        val2.TaskProgress(OnlineCommandHelper.GetDevicePrefixedApplicationName(list[k]));
                        if ((int)val == 0)
                        {
                            ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).ClearDownloadContext(list[k]);
                        }
                        if (!((ILanguageModelManager21)APEnvironment.LanguageModelMgr).Compile(list[k]))
                        {
                            string text2 = string.Format(Strings.MultipleDownload_BuildFailed, OnlineCommandHelper.GetDevicePrefixedApplicationName(list[k]));
                            StoreResult(array, list[k], (MultipleDownloadResultState)5, text2);
                            if ((int)APEnvironment.MessageService.Prompt(text2 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue", Array.Empty<object>()) != 2)
                            {
                                throw new OperationCanceledException();
                            }
                            list2.RemoveAt(k);
                            list.RemoveAt(k--);
                        }
                        continue;
                    }
                    if (list2[k] != null)
                    {
                        if (val2.Aborting)
                        {
                            break;
                        }
                        val2.TaskProgress(list2[k].Name);
                        try
                        {
                            list2[k].Build((IMultipleDownloadContext)(object)multipleDownloadContext);
                        }
                        catch
                        {
                            string text3 = string.Format(Strings.MultipleDownload_BuildFailed, list2[k].Name);
                            StoreResult(array, list2[k], (MultipleDownloadResultState)5, text3);
                            if ((int)APEnvironment.MessageService.Prompt(text3 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue01", Array.Empty<object>()) == 2)
                            {
                                list2.RemoveAt(k);
                                list.RemoveAt(k--);
                                continue;
                            }
                            throw new OperationCanceledException();
                        }
                        continue;
                    }
                    throw new InvalidOperationException("Inconsistent download extensions and applications!");
                }
                if (val2.Aborting)
                {
                    throw new OperationCanceledException();
                }
                Dictionary<IOnlineDevice, Exception> dictionary = new Dictionary<IOnlineDevice, Exception>();
                val2.NextTask(Strings.MultipleDownload_ConnectingTaskName, list.Count, Strings.MultipleDownload_ConnectingTaskUnit);
                MultipleDownloadResultState state2 = default(MultipleDownloadResultState);
                for (int l = 0; l < list.Count; l++)
                {
                    bool flag9 = false;
                    foreach (IMultipleDownloadApplicationHandler item4 in APEnvironment.MultipleDownloadApplicationHandler)
                    {
                        bool flag10 = false;
                        string stErrorMessage2 = "";
                        if (item4.TestConnection(val2, list[l], out flag10, out state2, out stErrorMessage2))
                        {
                            flag9 = true;
                            if (!flag10)
                            {
                                StoreResult(array, list[l], state2, stErrorMessage2);
                                break;
                            }
                            StoreResult(array, list[l], state2, stErrorMessage2);
                            list2.RemoveAt(l);
                            list.RemoveAt(l--);
                        }
                    }
                    if (flag9)
                    {
                        continue;
                    }
                    if (list[l] != Guid.Empty)
                    {
                        if (val2.Aborting)
                        {
                            break;
                        }
                        val2.TaskProgress(OnlineCommandHelper.GetDevicePrefixedApplicationName(list[l]));
                        IOnlineDevice onlineDeviceForApplication = OnlineCommandHelper.GetOnlineDeviceForApplication(list[l]);
                        try
                        {
                            if (onlineDeviceForApplication.IsConnected)
                            {
                                continue;
                            }
                            if (dictionary.ContainsKey(onlineDeviceForApplication))
                            {
                                Exception ex = dictionary[onlineDeviceForApplication];
                                if (ex != null)
                                {
                                    string text4 = string.Format(Strings.MultipleDownload_ConnectFailed, OnlineCommandHelper.GetDevicePrefixedApplicationName(list[l]), ex.Message);
                                    StoreResult(array, list[l], (MultipleDownloadResultState)5, text4);
                                    if ((int)APEnvironment.MessageService.Prompt(text4 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue02", Array.Empty<object>()) != 2)
                                    {
                                        throw new OperationCanceledException();
                                    }
                                    list2.RemoveAt(l);
                                    list.RemoveAt(l--);
                                }
                            }
                            else if (onlineDeviceForApplication is IOnlineDevice3)
                            {
                                ((IOnlineDevice3)onlineDeviceForApplication).SharedConnect((object)this);
                                dictionary[onlineDeviceForApplication] = null;
                                ((IOnlineDevice3)onlineDeviceForApplication).SharedDisconnect((object)this);
                            }
                            else
                            {
                                onlineDeviceForApplication.Connect();
                                dictionary[onlineDeviceForApplication] = null;
                                onlineDeviceForApplication.Disconnect();
                            }
                        }
                        catch (Exception ex2)
                        {
                            Exception ex4 = (dictionary[onlineDeviceForApplication] = ex2);
                            string arg = ex4.Message;
                            if (ex4 is TargetMismatchException2)
                            {
                                arg = OnlineCommandHelper.GetDetailsFromTargetMismatchException2((TargetMismatchException2)ex4);
                            }
                            string text5 = string.Format(Strings.MultipleDownload_ConnectFailed, OnlineCommandHelper.GetDevicePrefixedApplicationName(list[l]), arg);
                            StoreResult(array, list[l], (MultipleDownloadResultState)5, text5);
                            if ((int)APEnvironment.MessageService.Prompt(text5 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue03", Array.Empty<object>()) == 2)
                            {
                                list2.RemoveAt(l);
                                list.RemoveAt(l--);
                                continue;
                            }
                            throw new OperationCanceledException();
                        }
                    }
                    else
                    {
                        if (list2[l] == null)
                        {
                            continue;
                        }
                        if (val2.Aborting)
                        {
                            break;
                        }
                        val2.TaskProgress(list2[l].Name);
                        try
                        {
                            list2[l].TestConnection((IMultipleDownloadContext)(object)multipleDownloadContext);
                        }
                        catch
                        {
                            string text6 = string.Format(Strings.MultipleDownload_ConnectFailed, list2[l].Name);
                            StoreResult(array, list2[l], (MultipleDownloadResultState)5, text6);
                            if ((int)APEnvironment.MessageService.Prompt(text6 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue04", Array.Empty<object>()) == 2)
                            {
                                list2.RemoveAt(l);
                                list.RemoveAt(l--);
                                continue;
                            }
                            throw new OperationCanceledException();
                        }
                    }
                }
                if (val2.Aborting)
                {
                    throw new OperationCanceledException();
                }
                MyMessageService myMessageService = new MyMessageService(messageService, val, flag, flag2, bDoNotReleaseForcedVariables, array);
                ((IEngine2)APEnvironment.Engine).SetMessageService((IMessageService)(object)myMessageService);
                val2.NextTask(Strings.MultipleDownload_ProcessingApplicationsTaskName, list.Count, Strings.MultipleDownload_ProcessingApplicationsTaskUnit);
                MultipleDownloadResultState state3 = default(MultipleDownloadResultState);
                for (int m = 0; m < list.Count; m++)
                {
                    bool flag11 = false;
                    IEnumerable<IMultipleDownloadApplicationHandler> multipleDownloadApplicationHandler = APEnvironment.MultipleDownloadApplicationHandler;
                    if (multipleDownloadApplicationHandler != null && multipleDownloadApplicationHandler.Count() > 0)
                    {
                        ((IEngine2)APEnvironment.Engine).SetMessageService(messageService);
                    }
                    foreach (IMultipleDownloadApplicationHandler item5 in APEnvironment.MultipleDownloadApplicationHandler)
                    {
                        bool flag12 = false;
                        string stErrorMessage3 = "";
                        if (item5.DoDownload(val2, list[m], out flag12, out state3, out stErrorMessage3))
                        {
                            flag11 = true;
                            if (!flag12)
                            {
                                StoreResult(array, list[m], state3, stErrorMessage3);
                                break;
                            }
                            StoreResult(array, list[m], state3, stErrorMessage3);
                            list2.RemoveAt(m);
                            list.RemoveAt(m--);
                        }
                    }
                    IEnumerable<IMultipleDownloadApplicationHandler> multipleDownloadApplicationHandler2 = APEnvironment.MultipleDownloadApplicationHandler;
                    if (multipleDownloadApplicationHandler2 != null && multipleDownloadApplicationHandler2.Count() > 0)
                    {
                        ((IEngine2)APEnvironment.Engine).SetMessageService((IMessageService)(object)myMessageService);
                    }
                    if (flag11)
                    {
                        continue;
                    }
                    if (list[m] != Guid.Empty)
                    {
                        if (val2.Aborting || myMessageService.CancellationRequested)
                        {
                            break;
                        }
                        val2.TaskProgress(OnlineCommandHelper.GetDevicePrefixedApplicationName(list[m]));
                        myMessageService.SetCurrentApplication(list[m]);
                        StoreResult(array, list[m], (MultipleDownloadResultState)1, null);
                        try
                        {
                            bool flag13 = false;
                            if (OnlineCommandHelper.IsLoggedIn(list[m]))
                            {
                                flag13 = true;
                                OnlineCommandHelper.Logout(list[m]);
                            }
                            ClearBuildErrors();
                            OnlineCommandHelper.Login(list[m]);
                            APEnvironment.OnlineUIMgr.Login(list[m]);
                            if (CountBuildErrors(out var stLastBuildError) > 0)
                            {
                                StoreResult(array, list[m], (MultipleDownloadResultState)5, stLastBuildError);
                            }
                            if (flag3)
                            {
                                IOnlineApplication application = OnlineCommandHelper.GetApplication(list[m]);
                                if (application != null && (int)application.ApplicationState == 0)
                                {
                                    TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, 1200);
                                    DateTime now = DateTime.Now;
                                    while ((int)application.ApplicationState == 0)
                                    {
                                        Application.DoEvents();
                                        if (DateTime.Now - now > timeSpan)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (OnlineCommandHelper.CanStart(list[m]))
                                {
                                    OnlineCommandHelper.Start(list[m]);
                                }
                            }
                            if (array.Any((MultipleDownloadResult result) => (int)result.State == 5) && (int)val == 3)
                            {
                                APEnvironment.MessageService.Error(string.Format(Strings.LoginSvc_Error_LoginFailed, OnlineCommandHelper.GetApplicationNameByGuid(list[m])), "ErrorLoginActiveApp", Array.Empty<object>());
                            }
                            if (!flag13 && OnlineCommandHelper.IsLoggedIn(list[m]) && !flag6)
                            {
                                OnlineCommandHelper.Logout(list[m]);
                            }
                        }
                        catch (Exception ex5)
                        {
                            string text7 = string.Format(Strings.MultipleDownload_ProcessingApplicationFailed, OnlineCommandHelper.GetDevicePrefixedApplicationName(list[m]), ex5.Message);
                            StoreResult(array, list[m], (MultipleDownloadResultState)5, text7);
                            if ((int)APEnvironment.MessageService.Prompt(text7 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue05", Array.Empty<object>()) == 2)
                            {
                                list2.RemoveAt(m);
                                list.RemoveAt(m--);
                                continue;
                            }
                            throw new OperationCanceledException();
                        }
                    }
                    else
                    {
                        if (list2[m] == null)
                        {
                            continue;
                        }
                        if (val2.Aborting || myMessageService.CancellationRequested)
                        {
                            break;
                        }
                        val2.TaskProgress(list2[m].Name);
                        try
                        {
                            bool flag14 = list2[m].PerformDownload((IMultipleDownloadContext)(object)multipleDownloadContext);
                            StoreResult(array, list2[m], (MultipleDownloadResultState)((!flag14) ? 1 : 3), null);
                        }
                        catch (Exception ex6)
                        {
                            string text8 = string.Format(Strings.MultipleDownload_ProcessingExtensionFailed, list2[m].Name, ex6.Message);
                            StoreResult(array, list2[m], (MultipleDownloadResultState)5, text8);
                            if ((int)APEnvironment.MessageService.Prompt(text8 + Strings.MultipleDownload_PromptContinue, (PromptChoice)2, (PromptResult)3, "MultipleDownload_PromptContinue06", Array.Empty<object>()) == 2)
                            {
                                list2.RemoveAt(m);
                                list.RemoveAt(m--);
                                continue;
                            }
                            throw new OperationCanceledException();
                        }
                    }
                }
                IEnumerable<IMultipleDownloadApplicationHandler> multipleDownloadApplicationHandler3 = APEnvironment.MultipleDownloadApplicationHandler;
                if (multipleDownloadApplicationHandler3 != null && multipleDownloadApplicationHandler3.Count() > 0)
                {
                    ((IEngine2)APEnvironment.Engine).SetMessageService(messageService);
                }
                foreach (IMultipleDownloadApplicationHandler item6 in APEnvironment.MultipleDownloadApplicationHandler)
                {
                    item6.PostProcessing();
                }
                IEnumerable<IMultipleDownloadApplicationHandler> multipleDownloadApplicationHandler4 = APEnvironment.MultipleDownloadApplicationHandler;
                if (multipleDownloadApplicationHandler4 != null && multipleDownloadApplicationHandler4.Count() > 0)
                {
                    ((IEngine2)APEnvironment.Engine).SetMessageService((IMessageService)(object)myMessageService);
                }
                if (val2.Aborting || myMessageService.CancellationRequested)
                {
                    throw new OperationCanceledException();
                }
            }
            catch (OperationCanceledException)
            {
                if (flag4)
                {
                    throw;
                }
            }
            catch (CancelledByUserException)
            {
                if (flag4)
                {
                    throw;
                }
            }
            finally
            {
                InternalCodeStateProvider.StartOnlineCodeStateUpdates();
                if (val2 != null)
                {
                    val2.Finish();
                }
                ((IEngine2)APEnvironment.Engine).SetMessageService(messageService);
                if (((IEngine)APEnvironment.Engine).Frame != null && !flag5)
                {
                    MultipleDownloadResultDialog multipleDownloadResultDialog = new MultipleDownloadResultDialog();
                    multipleDownloadResultDialog.Initialize(array);
                    multipleDownloadResultDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm);
                }
            }
        }

        private static void ClearBuildErrors()
        {
            if (APEnvironment.MessageStorage.Categories == null)
            {
                return;
            }
            IMessageCategory[] categories = APEnvironment.MessageStorage.Categories;
            foreach (IMessageCategory val in categories)
            {
                if (TypeGuidAttribute.FromObject((object)val).Guid == GUID_BUILDMESSAGECATEGORY)
                {
                    APEnvironment.MessageStorage.ClearMessages(val);
                    break;
                }
            }
        }

        private static int CountBuildErrors(out string stLastBuildError)
        {
            //IL_005f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0065: Invalid comparison between Unknown and I4
            //IL_0069: Unknown result type (might be due to invalid IL or missing references)
            //IL_006f: Invalid comparison between Unknown and I4
            stLastBuildError = null;
            if (APEnvironment.MessageStorage.Categories != null)
            {
                IMessageCategory[] categories = APEnvironment.MessageStorage.Categories;
                foreach (IMessageCategory val in categories)
                {
                    if (!(TypeGuidAttribute.FromObject((object)val).Guid == GUID_BUILDMESSAGECATEGORY))
                    {
                        continue;
                    }
                    IMessage[] messages = APEnvironment.MessageStorage.GetMessages(val);
                    int num = 0;
                    if (messages != null)
                    {
                        IMessage[] array = messages;
                        foreach (IMessage val2 in array)
                        {
                            if ((int)val2.Severity == 2 || (int)val2.Severity == 1)
                            {
                                num++;
                                stLastBuildError = val2.Text;
                            }
                        }
                    }
                    return num;
                }
            }
            return 0;
        }

        private static void StoreResult(MultipleDownloadResult[] results, Guid applicationGuid, MultipleDownloadResultState state, string stErrorMessage)
        {
            //IL_0045: Unknown result type (might be due to invalid IL or missing references)
            //IL_0046: Unknown result type (might be due to invalid IL or missing references)
            Debug.Assert(results != null);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].Item is ApplicationMultipleDownloadItem && ((ApplicationMultipleDownloadItem)results[i].Item).ApplicationGuid == applicationGuid)
                {
                    results[i].State = state;
                    results[i].ErrorMessage = stErrorMessage;
                    break;
                }
            }
        }

        private static void StoreResult(MultipleDownloadResult[] results, IMultipleDownloadExtension extension, MultipleDownloadResultState state, string stErrorMessage)
        {
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0050: Unknown result type (might be due to invalid IL or missing references)
            Debug.Assert(results != null);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].Item is ExtensionMultipleDownloadItem && ((ExtensionMultipleDownloadItem)results[i].Item).Extension.Name == extension.Name)
                {
                    results[i].State = state;
                    results[i].ErrorMessage = stErrorMessage;
                    break;
                }
            }
        }

        private static Guid GetApplicationGuid(string stName)
        {
            Debug.Assert(stName != null);
            string[] array = stName.Split('.');
            if (array == null || array.Length == 0)
            {
                return Guid.Empty;
            }
            string b = array[array.Length - 1];
            Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle);
            foreach (Guid guid in allObjects)
            {
                IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, guid);
                if (string.Equals(metaObjectStub.Name, b, StringComparison.OrdinalIgnoreCase) && typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType) && string.Equals(OnlineCommandHelper.GetDevicePrefixedApplicationName(guid), stName, StringComparison.OrdinalIgnoreCase))
                {
                    return guid;
                }
            }
            return Guid.Empty;
        }

        private IMultipleDownloadExtension GetExtension(string stName)
        {
            Debug.Assert(stName != null);
            if (_pendingExtensions.TryGetValue(stName, out var value))
            {
                return value;
            }
            string[] array = stName.Split(new char[1] { '.' }, 2);
            if (array == null || array.Length < 2)
            {
                return null;
            }
            string text = array[0];
            string text2 = array[1];
            foreach (IMultipleDownloadExtensionProvider multipleDownloadExtensionProvider in APEnvironment.MultipleDownloadExtensionProviders)
            {
                if (!(multipleDownloadExtensionProvider.Name == text))
                {
                    continue;
                }
                IMultipleDownloadExtension[] extensions = multipleDownloadExtensionProvider.GetExtensions();
                if (extensions == null)
                {
                    continue;
                }
                IMultipleDownloadExtension[] array2 = extensions;
                foreach (IMultipleDownloadExtension val in array2)
                {
                    if (val.Name == text2)
                    {
                        return val;
                    }
                }
            }
            return null;
        }
    }
}
