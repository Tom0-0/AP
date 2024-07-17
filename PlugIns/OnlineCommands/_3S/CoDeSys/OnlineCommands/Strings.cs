using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace _3S.CoDeSys.OnlineCommands
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class Strings
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new ResourceManager("_3S.CoDeSys.OnlineCommands.Strings", typeof(Strings).Assembly);
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string Aborting => ResourceManager.GetString("Aborting", resourceCulture);

        internal static string AddOnlineUserCommand_Description => ResourceManager.GetString("AddOnlineUserCommand_Description", resourceCulture);

        internal static string AddOnlineUserCommand_Dialog_Caption => ResourceManager.GetString("AddOnlineUserCommand_Dialog_Caption", resourceCulture);

        internal static string AddOnlineUserCommand_LogoutMessage => ResourceManager.GetString("AddOnlineUserCommand_LogoutMessage", resourceCulture);

        internal static string AddOnlineUserCommand_Name => ResourceManager.GetString("AddOnlineUserCommand_Name", resourceCulture);

        internal static string AffectionCopied => ResourceManager.GetString("AffectionCopied", resourceCulture);

        internal static string AffectionInitialized => ResourceManager.GetString("AffectionInitialized", resourceCulture);

        internal static string AffectionInitNewVariables => ResourceManager.GetString("AffectionInitNewVariables", resourceCulture);

        internal static string AffectionLocationChanged => ResourceManager.GetString("AffectionLocationChanged", resourceCulture);

        internal static string AffectionReInitialized => ResourceManager.GetString("AffectionReInitialized", resourceCulture);

        internal static string AffectionVFTableInitialized => ResourceManager.GetString("AffectionVFTableInitialized", resourceCulture);

        internal static string ApplicationContent_Title_IDE => ResourceManager.GetString("ApplicationContent_Title_IDE", resourceCulture);

        internal static string ApplicationContent_Title_PLC => ResourceManager.GetString("ApplicationContent_Title_PLC", resourceCulture);

        internal static string ApplicationList => ResourceManager.GetString("ApplicationList", resourceCulture);

        internal static string ApplicationListEmpty => ResourceManager.GetString("ApplicationListEmpty", resourceCulture);

        internal static string ApplyToAll => ResourceManager.GetString("ApplyToAll", resourceCulture);

        internal static string BreakpointsDisablingWarning => ResourceManager.GetString("BreakpointsDisablingWarning", resourceCulture);

        internal static string CannotAddOnlineObject => ResourceManager.GetString("CannotAddOnlineObject", resourceCulture);

        internal static string CannotReadApplicationInfoFromPLC => ResourceManager.GetString("CannotReadApplicationInfoFromPLC", resourceCulture);

        internal static string CannotRemoveDeviceOrApplicationWhichIsOnlineException => ResourceManager.GetString("CannotRemoveDeviceOrApplicationWhichIsOnlineException", resourceCulture);

        internal static string CannotRemoveOnlineObject => ResourceManager.GetString("CannotRemoveOnlineObject", resourceCulture);

        internal static string CannotRenameOnlineObject => ResourceManager.GetString("CannotRenameOnlineObject", resourceCulture);

        internal static string Change_BaseExpressionChanged => ResourceManager.GetString("Change_BaseExpressionChanged", resourceCulture);

        internal static string Change_BlobInitChanged => ResourceManager.GetString("Change_BlobInitChanged", resourceCulture);

        internal static string Change_CheckFunction => ResourceManager.GetString("Change_CheckFunction", resourceCulture);

        internal static string Change_CompileOptionsChanged => ResourceManager.GetString("Change_CompileOptionsChanged", resourceCulture);

        internal static string Change_DeviceApplication => ResourceManager.GetString("Change_DeviceApplication", resourceCulture);

        internal static string Change_ExternalSignatureFlagChanged => ResourceManager.GetString("Change_ExternalSignatureFlagChanged", resourceCulture);

        internal static string Change_GlobalInitInCycle => ResourceManager.GetString("Change_GlobalInitInCycle", resourceCulture);

        internal static string Change_InhibitOnlineChange => ResourceManager.GetString("Change_InhibitOnlineChange", resourceCulture);

        internal static string Change_InhibitOnlineChangeOnCodeChanges => ResourceManager.GetString("Change_InhibitOnlineChangeOnCodeChanges", resourceCulture);

        internal static string Change_InterfaceSequenceChanged => ResourceManager.GetString("Change_InterfaceSequenceChanged", resourceCulture);

        internal static string Change_InterfacesRemoved => ResourceManager.GetString("Change_InterfacesRemoved", resourceCulture);

        internal static string Change_MemorySettingsChanged => ResourceManager.GetString("Change_MemorySettingsChanged", resourceCulture);

        internal static string Change_NameExpressionChanged => ResourceManager.GetString("Change_NameExpressionChanged", resourceCulture);

        internal static string Change_NewCheckFunctionInserted => ResourceManager.GetString("Change_NewCheckFunctionInserted", resourceCulture);

        internal static string Change_ParentContextChanged => ResourceManager.GetString("Change_ParentContextChanged", resourceCulture);

        internal static string Change_ParentContextNull => ResourceManager.GetString("Change_ParentContextNull", resourceCulture);

        internal static string Change_PrecomTypeMayLeaveDeadRefs => ResourceManager.GetString("Change_PrecomTypeMayLeaveDeadRefs", resourceCulture);

        internal static string Change_SignatureChangedByChecksumAttribute => ResourceManager.GetString("Change_SignatureChangedByChecksumAttribute", resourceCulture);

        internal static string Change_TaskListChanged => ResourceManager.GetString("Change_TaskListChanged", resourceCulture);

        internal static string Change_TaskLocalGvl => ResourceManager.GetString("Change_TaskLocalGvl", resourceCulture);

        internal static string Change_VariableChanged => ResourceManager.GetString("Change_VariableChanged", resourceCulture);

        internal static string Change_VariableDeleted => ResourceManager.GetString("Change_VariableDeleted", resourceCulture);

        internal static string Change_VariableInserted => ResourceManager.GetString("Change_VariableInserted", resourceCulture);

        internal static string ChangePasswordOnlineUserCommand_Description => ResourceManager.GetString("ChangePasswordOnlineUserCommand_Description", resourceCulture);

        internal static string ChangePasswordOnlineUserCommand_Dialog_Caption => ResourceManager.GetString("ChangePasswordOnlineUserCommand_Dialog_Caption", resourceCulture);

        internal static string ChangePasswordOnlineUserCommand_Name => ResourceManager.GetString("ChangePasswordOnlineUserCommand_Name", resourceCulture);

        internal static string CmdActiveApplicationSelector_Name => ResourceManager.GetString("CmdActiveApplicationSelector_Name", resourceCulture);

        internal static Icon CmdDebug => (Icon)ResourceManager.GetObject("CmdDebug", resourceCulture);

        internal static Icon CmdLocked => (Icon)ResourceManager.GetObject("CmdLocked", resourceCulture);

        internal static string CmdModeDebug_Descr => ResourceManager.GetString("CmdModeDebug_Descr", resourceCulture);

        internal static string CmdModeDebug_Info => ResourceManager.GetString("CmdModeDebug_Info", resourceCulture);

        internal static string CmdModeLock_Info => ResourceManager.GetString("CmdModeLock_Info", resourceCulture);

        internal static string CmdModeLocked_Descr => ResourceManager.GetString("CmdModeLocked_Descr", resourceCulture);

        internal static string CmdModeOperational_Descr => ResourceManager.GetString("CmdModeOperational_Descr", resourceCulture);

        internal static string CmdModeOperational_Info => ResourceManager.GetString("CmdModeOperational_Info", resourceCulture);

        internal static Icon CmdOperational => (Icon)ResourceManager.GetObject("CmdOperational", resourceCulture);

        internal static Icon CmdOperationalStatusField => (Icon)ResourceManager.GetObject("CmdOperationalStatusField", resourceCulture);

        internal static string CmdWink_Descr => ResourceManager.GetString("CmdWink_Descr", resourceCulture);

        internal static string CmdWink_Name => ResourceManager.GetString("CmdWink_Name", resourceCulture);

        internal static Icon CmdWinkIcon => (Icon)ResourceManager.GetObject("CmdWinkIcon", resourceCulture);

        internal static string CodeNeedsFullDownload => ResourceManager.GetString("CodeNeedsFullDownload", resourceCulture);

        internal static string CodeNeedsOnlineChange => ResourceManager.GetString("CodeNeedsOnlineChange", resourceCulture);

        internal static string CodeUnchanged => ResourceManager.GetString("CodeUnchanged", resourceCulture);

        internal static string Col_Description => ResourceManager.GetString("Col_Description", resourceCulture);

        internal static string Col_Pou => ResourceManager.GetString("Col_Pou", resourceCulture);

        internal static string Col_PreventsOnlineChange => ResourceManager.GetString("Col_PreventsOnlineChange", resourceCulture);

        internal static string CommandDeleteApplicationFromDevice => ResourceManager.GetString("CommandDeleteApplicationFromDevice", resourceCulture);

        internal static string ConnectToDeviceDescription => ResourceManager.GetString("ConnectToDeviceDescription", resourceCulture);

        internal static string ConnectToDeviceName => ResourceManager.GetString("ConnectToDeviceName", resourceCulture);

        internal static string core_dump_creating => ResourceManager.GetString("core_dump_creating", resourceCulture);

        internal static string core_dump_loaded => ResourceManager.GetString("core_dump_loaded", resourceCulture);

        internal static string CreateBootApplicationCommand_Description => ResourceManager.GetString("CreateBootApplicationCommand_Description", resourceCulture);

        internal static string CreateBootApplicationCommand_Name => ResourceManager.GetString("CreateBootApplicationCommand_Name", resourceCulture);

        internal static string CreateBootAppTitle => ResourceManager.GetString("CreateBootAppTitle", resourceCulture);

        internal static string CurrentState => ResourceManager.GetString("CurrentState", resourceCulture);

        internal static string debug_step => ResourceManager.GetString("debug_step", resourceCulture);

        internal static string Directory => ResourceManager.GetString("Directory", resourceCulture);

        internal static string DisconnectFromDeviceDescription => ResourceManager.GetString("DisconnectFromDeviceDescription", resourceCulture);

        internal static string DisconnectFromDeviceName => ResourceManager.GetString("DisconnectFromDeviceName", resourceCulture);

        internal static string DisplayModeBinaryCommand_Description => ResourceManager.GetString("DisplayModeBinaryCommand_Description", resourceCulture);

        internal static string DisplayModeBinaryCommand_Name => ResourceManager.GetString("DisplayModeBinaryCommand_Name", resourceCulture);

        internal static string DisplayModeDecimalCommand_Description => ResourceManager.GetString("DisplayModeDecimalCommand_Description", resourceCulture);

        internal static string DisplayModeDecimalCommand_Name => ResourceManager.GetString("DisplayModeDecimalCommand_Name", resourceCulture);

        internal static string DisplayModeHexadecimalCommand_Description => ResourceManager.GetString("DisplayModeHexadecimalCommand_Description", resourceCulture);

        internal static string DisplayModeHexadecimalCommand_Name => ResourceManager.GetString("DisplayModeHexadecimalCommand_Name", resourceCulture);

        internal static string DoNotWarnAgain => ResourceManager.GetString("DoNotWarnAgain", resourceCulture);

        internal static string download => ResourceManager.GetString("download", resourceCulture);

        internal static string DownloadActiveApplicationCommand_Description => ResourceManager.GetString("DownloadActiveApplicationCommand_Description", resourceCulture);

        internal static string DownloadActiveApplicationCommand_Name => ResourceManager.GetString("DownloadActiveApplicationCommand_Name", resourceCulture);

        internal static string DownloadMessageCategory => ResourceManager.GetString("DownloadMessageCategory", resourceCulture);

        internal static string DownloadOnDeviceCommand_Description => ResourceManager.GetString("DownloadOnDeviceCommand_Description", resourceCulture);

        internal static string DownloadOnDeviceCommand_Name => ResourceManager.GetString("DownloadOnDeviceCommand_Name", resourceCulture);

        internal static string EditObjectOffline_Description => ResourceManager.GetString("EditObjectOffline_Description", resourceCulture);

        internal static string EditObjectOffline_Name => ResourceManager.GetString("EditObjectOffline_Name", resourceCulture);

        internal static string EmptyDownloadTime => ResourceManager.GetString("EmptyDownloadTime", resourceCulture);

        internal static string Err_SourceDownload_ArchiveTooBig => ResourceManager.GetString("Err_SourceDownload_ArchiveTooBig", resourceCulture);

        internal static string Err_SourceDownload_NoSpaceOnTarget => ResourceManager.GetString("Err_SourceDownload_NoSpaceOnTarget", resourceCulture);

        internal static string Err_TSPreserveApp_NoChildApp => ResourceManager.GetString("Err_TSPreserveApp_NoChildApp", resourceCulture);

        internal static string Error_ConflictingEncryptionSettings => ResourceManager.GetString("Error_ConflictingEncryptionSettings", resourceCulture);

        internal static string Error_NoUserInformationFound => ResourceManager.GetString("Error_NoUserInformationFound", resourceCulture);

        internal static string exception => ResourceManager.GetString("exception", resourceCulture);

        internal static string executionpoints_active => ResourceManager.GetString("executionpoints_active", resourceCulture);

        internal static string FileTransferDownload => ResourceManager.GetString("FileTransferDownload", resourceCulture);

        internal static string FileTransferUpload => ResourceManager.GetString("FileTransferUpload", resourceCulture);

        internal static string FilterCreateBootApp => ResourceManager.GetString("FilterCreateBootApp", resourceCulture);

        internal static string flow_active => ResourceManager.GetString("flow_active", resourceCulture);

        internal static string FlowControlActiveApplicationCommand_ContextlessName => ResourceManager.GetString("FlowControlActiveApplicationCommand_ContextlessName", resourceCulture);

        internal static string FlowControlActiveApplicationCommand_Description => ResourceManager.GetString("FlowControlActiveApplicationCommand_Description", resourceCulture);

        internal static string FlowControlActiveApplicationCommand_Name => ResourceManager.GetString("FlowControlActiveApplicationCommand_Name", resourceCulture);

        internal static string FlowControlWarning => ResourceManager.GetString("FlowControlWarning", resourceCulture);

        internal static string force_active => ResourceManager.GetString("force_active", resourceCulture);

        internal static string ForceAllApplicationsCommand_ContextlessName => ResourceManager.GetString("ForceAllApplicationsCommand_ContextlessName", resourceCulture);

        internal static string ForceAllApplicationsCommand_Description => ResourceManager.GetString("ForceAllApplicationsCommand_Description", resourceCulture);

        internal static string ForceAllApplicationsCommand_Name => ResourceManager.GetString("ForceAllApplicationsCommand_Name", resourceCulture);

        internal static string ForceFailedForSomeVariables => ResourceManager.GetString("ForceFailedForSomeVariables", resourceCulture);

        internal static string ForceSelectedAppCommand_ContextlessName => ResourceManager.GetString("ForceSelectedAppCommand_ContextlessName", resourceCulture);

        internal static string ForceSelectedAppCommand_Description => ResourceManager.GetString("ForceSelectedAppCommand_Description", resourceCulture);

        internal static string ForceSelectedAppCommand_Name => ResourceManager.GetString("ForceSelectedAppCommand_Name", resourceCulture);

        internal static string ForceValuesCommand_ContextlessName => ResourceManager.GetString("ForceValuesCommand_ContextlessName", resourceCulture);

        internal static string ForceValuesCommand_Description => ResourceManager.GetString("ForceValuesCommand_Description", resourceCulture);

        internal static string ForceValuesCommand_Name => ResourceManager.GetString("ForceValuesCommand_Name", resourceCulture);

        internal static string GenerateCodeFailed => ResourceManager.GetString("GenerateCodeFailed", resourceCulture);

        internal static string halt_on_bp => ResourceManager.GetString("halt_on_bp", resourceCulture);

        internal static string HaltOnExceptionCommand_Description => ResourceManager.GetString("HaltOnExceptionCommand_Description", resourceCulture);

        internal static string HaltOnExceptionCommand_Name => ResourceManager.GetString("HaltOnExceptionCommand_Name", resourceCulture);

        internal static string InteractiveLoginConfirmed => ResourceManager.GetString("InteractiveLoginConfirmed", resourceCulture);

        internal static string InteractiveLoginEnterID => ResourceManager.GetString("InteractiveLoginEnterID", resourceCulture);

        internal static string InteractiveLoginName => ResourceManager.GetString("InteractiveLoginName", resourceCulture);

        internal static string InteractiveLoginPressKey => ResourceManager.GetString("InteractiveLoginPressKey", resourceCulture);

        internal static string InteractiveLoginTimeout => ResourceManager.GetString("InteractiveLoginTimeout", resourceCulture);

        internal static string InterfaceListExceeded => ResourceManager.GetString("InterfaceListExceeded", resourceCulture);

        internal static string InterfaceReferenceListExceeded => ResourceManager.GetString("InterfaceReferenceListExceeded", resourceCulture);

        internal static string InvalidCommunicationSettings => ResourceManager.GetString("InvalidCommunicationSettings", resourceCulture);

        internal static string InvalidParameters => ResourceManager.GetString("InvalidParameters", resourceCulture);

        internal static string InvalidValue => ResourceManager.GetString("InvalidValue", resourceCulture);

        internal static string LblChangedLoc => ResourceManager.GetString("LblChangedLoc", resourceCulture);

        internal static string LblChangedVFT => ResourceManager.GetString("LblChangedVFT", resourceCulture);

        internal static string LblMovedInst => ResourceManager.GetString("LblMovedInst", resourceCulture);

        internal static string LblNumAffectedVars => ResourceManager.GetString("LblNumAffectedVars", resourceCulture);

        internal static string LblNumChangedItfs => ResourceManager.GetString("LblNumChangedItfs", resourceCulture);

        internal static string LblNumChangedPOUs => ResourceManager.GetString("LblNumChangedPOUs", resourceCulture);

        internal static string LblNumItfTest => ResourceManager.GetString("LblNumItfTest", resourceCulture);

        internal static string LblToCopy => ResourceManager.GetString("LblToCopy", resourceCulture);

        internal static string LblToInit => ResourceManager.GetString("LblToInit", resourceCulture);

        internal static string LblToReinit => ResourceManager.GetString("LblToReinit", resourceCulture);

        internal static string LblTotalRelinkTests => ResourceManager.GetString("LblTotalRelinkTests", resourceCulture);

        internal static string ListOfChangedPOUs => ResourceManager.GetString("ListOfChangedPOUs", resourceCulture);

        internal static string ListOfInstancesImplementingInterfaces => ResourceManager.GetString("ListOfInstancesImplementingInterfaces", resourceCulture);

        internal static string ListOfInterfaceReferences => ResourceManager.GetString("ListOfInterfaceReferences", resourceCulture);

        internal static string ListOfInterfaces => ResourceManager.GetString("ListOfInterfaces", resourceCulture);

        internal static string ListOfVariables => ResourceManager.GetString("ListOfVariables", resourceCulture);

        internal static string LoginActiveApplicationCommand_ContextlessName => ResourceManager.GetString("LoginActiveApplicationCommand_ContextlessName", resourceCulture);

        internal static string LoginActiveApplicationCommand_Description => ResourceManager.GetString("LoginActiveApplicationCommand_Description", resourceCulture);

        internal static string LoginActiveApplicationCommand_Name => ResourceManager.GetString("LoginActiveApplicationCommand_Name", resourceCulture);

        internal static string LoginCascadingCommand_Descption => ResourceManager.GetString("LoginCascadingCommand_Descption", resourceCulture);

        internal static string LoginCascadingCommand_Name => ResourceManager.GetString("LoginCascadingCommand_Name", resourceCulture);

        internal static string LoginSelectedApplicationCommand_ContextlessName => ResourceManager.GetString("LoginSelectedApplicationCommand_ContextlessName", resourceCulture);

        internal static string LoginSelectedApplicationCommand_Description => ResourceManager.GetString("LoginSelectedApplicationCommand_Description", resourceCulture);

        internal static string LoginSelectedApplicationCommand_Name => ResourceManager.GetString("LoginSelectedApplicationCommand_Name", resourceCulture);

        internal static string LoginSvc_Error_CompileCodeFailed => ResourceManager.GetString("LoginSvc_Error_CompileCodeFailed", resourceCulture);

        internal static string LoginSvc_Error_ConflictingFlags => ResourceManager.GetString("LoginSvc_Error_ConflictingFlags", resourceCulture);

        internal static string LoginSvc_Error_DownloadNotAllowed => ResourceManager.GetString("LoginSvc_Error_DownloadNotAllowed", resourceCulture);

        internal static string LoginSvc_Error_GenerateCodeFailed => ResourceManager.GetString("LoginSvc_Error_GenerateCodeFailed", resourceCulture);

        internal static string LoginSvc_Error_InvalidOwner => ResourceManager.GetString("LoginSvc_Error_InvalidOwner", resourceCulture);

        internal static string LoginSvc_Error_LoginFailed => ResourceManager.GetString("LoginSvc_Error_LoginFailed", resourceCulture);

        internal static string LogoffUser_Description => ResourceManager.GetString("LogoffUser_Description", resourceCulture);

        internal static string LogoffUser_Name => ResourceManager.GetString("LogoffUser_Name", resourceCulture);

        internal static string LogoutActiveApplicationCommand_ContextlessName => ResourceManager.GetString("LogoutActiveApplicationCommand_ContextlessName", resourceCulture);

        internal static string LogoutActiveApplicationCommand_Description => ResourceManager.GetString("LogoutActiveApplicationCommand_Description", resourceCulture);

        internal static string LogoutActiveApplicationCommand_Name => ResourceManager.GetString("LogoutActiveApplicationCommand_Name", resourceCulture);

        internal static string LogoutCascadingCommand_Description => ResourceManager.GetString("LogoutCascadingCommand_Description", resourceCulture);

        internal static string LogoutCascadingCommand_Name => ResourceManager.GetString("LogoutCascadingCommand_Name", resourceCulture);

        internal static string LogoutSelectedApplicationCommand_ContextlessName => ResourceManager.GetString("LogoutSelectedApplicationCommand_ContextlessName", resourceCulture);

        internal static string LogoutSelectedApplicationCommand_Description => ResourceManager.GetString("LogoutSelectedApplicationCommand_Description", resourceCulture);

        internal static string LogoutSelectedApplicationCommand_Name => ResourceManager.GetString("LogoutSelectedApplicationCommand_Name", resourceCulture);

        internal static string MultipleDownload_BuildFailed => ResourceManager.GetString("MultipleDownload_BuildFailed", resourceCulture);

        internal static string MultipleDownload_BuildingTaskName => ResourceManager.GetString("MultipleDownload_BuildingTaskName", resourceCulture);

        internal static string MultipleDownload_BuildingTaskUnit => ResourceManager.GetString("MultipleDownload_BuildingTaskUnit", resourceCulture);

        internal static string MultipleDownload_ConnectFailed => ResourceManager.GetString("MultipleDownload_ConnectFailed", resourceCulture);

        internal static string MultipleDownload_ConnectingTaskName => ResourceManager.GetString("MultipleDownload_ConnectingTaskName", resourceCulture);

        internal static string MultipleDownload_ConnectingTaskUnit => ResourceManager.GetString("MultipleDownload_ConnectingTaskUnit", resourceCulture);

        internal static string MultipleDownload_GenericErrorMessage => ResourceManager.GetString("MultipleDownload_GenericErrorMessage", resourceCulture);

        internal static string MultipleDownload_GenericWarningMessage => ResourceManager.GetString("MultipleDownload_GenericWarningMessage", resourceCulture);

        internal static string MultipleDownload_MessageCategory => ResourceManager.GetString("MultipleDownload_MessageCategory", resourceCulture);

        internal static string MultipleDownload_ProcessingApplicationFailed => ResourceManager.GetString("MultipleDownload_ProcessingApplicationFailed", resourceCulture);

        internal static string MultipleDownload_ProcessingApplicationsTaskName => ResourceManager.GetString("MultipleDownload_ProcessingApplicationsTaskName", resourceCulture);

        internal static string MultipleDownload_ProcessingApplicationsTaskUnit => ResourceManager.GetString("MultipleDownload_ProcessingApplicationsTaskUnit", resourceCulture);

        internal static string MultipleDownload_ProcessingExtensionFailed => ResourceManager.GetString("MultipleDownload_ProcessingExtensionFailed", resourceCulture);

        internal static string MultipleDownload_PromptContinue => ResourceManager.GetString("MultipleDownload_PromptContinue", resourceCulture);

        internal static string MultipleDownload_SkippedDueToForcedVariables => ResourceManager.GetString("MultipleDownload_SkippedDueToForcedVariables", resourceCulture);

        internal static string MultipleDownloadCommand_Description => ResourceManager.GetString("MultipleDownloadCommand_Description", resourceCulture);

        internal static string MultipleDownloadCommand_Name => ResourceManager.GetString("MultipleDownloadCommand_Name", resourceCulture);

        internal static string MultipleDownloadResult_CancelledByUser => ResourceManager.GetString("MultipleDownloadResult_CancelledByUser", resourceCulture);

        internal static string MultipleDownloadResult_Created => ResourceManager.GetString("MultipleDownloadResult_Created", resourceCulture);

        internal static string MultipleDownloadResult_Downloaded => ResourceManager.GetString("MultipleDownloadResult_Downloaded", resourceCulture);

        internal static string MultipleDownloadResult_Error => ResourceManager.GetString("MultipleDownloadResult_Error", resourceCulture);

        internal static string MultipleDownloadResult_NotChanged => ResourceManager.GetString("MultipleDownloadResult_NotChanged", resourceCulture);

        internal static string MultipleDownloadResult_OnlineChanged => ResourceManager.GetString("MultipleDownloadResult_OnlineChanged", resourceCulture);

        internal static string MultipleDownloadResult_SkippedDueToImpossibleOnlineChange => ResourceManager.GetString("MultipleDownloadResult_SkippedDueToImpossibleOnlineChange", resourceCulture);

        internal static string NoDownloadOnBP => ResourceManager.GetString("NoDownloadOnBP", resourceCulture);

        internal static string None => ResourceManager.GetString("None", resourceCulture);

        internal static string NoSourceCodeAvailable => ResourceManager.GetString("NoSourceCodeAvailable", resourceCulture);

        internal static string Object => ResourceManager.GetString("Object", resourceCulture);

        internal static string Offline => ResourceManager.GetString("Offline", resourceCulture);

        internal static string OfflineCodeState_DownloadNeeded => ResourceManager.GetString("OfflineCodeState_DownloadNeeded", resourceCulture);

        internal static string OfflineCodeState_NoCodeGenerated => ResourceManager.GetString("OfflineCodeState_NoCodeGenerated", resourceCulture);

        internal static string OfflineCodeState_OnlineChangable => ResourceManager.GetString("OfflineCodeState_OnlineChangable", resourceCulture);

        internal static string OfflineCodeState_Unchanged => ResourceManager.GetString("OfflineCodeState_Unchanged", resourceCulture);

        internal static string online_change => ResourceManager.GetString("online_change", resourceCulture);

        internal static string OnlineChangeActiveApplicationCommand_ContextlessName => ResourceManager.GetString("OnlineChangeActiveApplicationCommand_ContextlessName", resourceCulture);

        internal static string OnlineChangeActiveApplicationCommand_Description => ResourceManager.GetString("OnlineChangeActiveApplicationCommand_Description", resourceCulture);

        internal static string OnlineChangeActiveApplicationCommand_Name => ResourceManager.GetString("OnlineChangeActiveApplicationCommand_Name", resourceCulture);

        internal static string OnlineChangeinformation_General_OK => ResourceManager.GetString("OnlineChangeinformation_General_OK", resourceCulture);

        internal static string OnlineChangeinformation_General_Warning => ResourceManager.GetString("OnlineChangeinformation_General_Warning", resourceCulture);

        internal static string OnlineChangeinformation_LocationChanged => ResourceManager.GetString("OnlineChangeinformation_LocationChanged", resourceCulture);

        internal static string OnlineChangeinformation_ReInit => ResourceManager.GetString("OnlineChangeinformation_ReInit", resourceCulture);

        internal static string OnlineChangeSelectedApplicationCommand_ContextlessName => ResourceManager.GetString("OnlineChangeSelectedApplicationCommand_ContextlessName", resourceCulture);

        internal static string OnlineChangeSelectedApplicationCommand_Description => ResourceManager.GetString("OnlineChangeSelectedApplicationCommand_Description", resourceCulture);

        internal static string OnlineChangeSelectedApplicationCommand_Name => ResourceManager.GetString("OnlineChangeSelectedApplicationCommand_Name", resourceCulture);

        internal static string OnlineCommandCategory_Description => ResourceManager.GetString("OnlineCommandCategory_Description", resourceCulture);

        internal static string OnlineCommandCategory_Name => ResourceManager.GetString("OnlineCommandCategory_Name", resourceCulture);

        internal static string OnlineToOneApplication => ResourceManager.GetString("OnlineToOneApplication", resourceCulture);

        internal static string OnlineToSomeApplications => ResourceManager.GetString("OnlineToSomeApplications", resourceCulture);

        internal static string OnlineUser_PasswordStrength_Better => ResourceManager.GetString("OnlineUser_PasswordStrength_Better", resourceCulture);

        internal static string OnlineUser_PasswordStrength_Good => ResourceManager.GetString("OnlineUser_PasswordStrength_Good", resourceCulture);

        internal static string OnlineUser_PasswordStrength_Medium => ResourceManager.GetString("OnlineUser_PasswordStrength_Medium", resourceCulture);

        internal static string OnlineUser_PasswordStrength_VeryGood => ResourceManager.GetString("OnlineUser_PasswordStrength_VeryGood", resourceCulture);

        internal static string OnlineUser_PasswordStrength_VeryWeak => ResourceManager.GetString("OnlineUser_PasswordStrength_VeryWeak", resourceCulture);

        internal static string OnlineUser_PasswordStrength_Weak => ResourceManager.GetString("OnlineUser_PasswordStrength_Weak", resourceCulture);

        internal static string OpenLoggerPage_Descr => ResourceManager.GetString("OpenLoggerPage_Descr", resourceCulture);

        internal static string OpenLoggerPage_Name => ResourceManager.GetString("OpenLoggerPage_Name", resourceCulture);

        internal static string OperatingModeDebug => ResourceManager.GetString("OperatingModeDebug", resourceCulture);

        internal static string OperatingModeLocked => ResourceManager.GetString("OperatingModeLocked", resourceCulture);

        internal static string OperatingModeOperational => ResourceManager.GetString("OperatingModeOperational", resourceCulture);

        internal static string Operation => ResourceManager.GetString("Operation", resourceCulture);

        internal static string OperationAborted_MissingCertificate => ResourceManager.GetString("OperationAborted_MissingCertificate", resourceCulture);

        internal static string OptionEditorDescription_Text => ResourceManager.GetString("OptionEditorDescription_Text", resourceCulture);

        internal static string OptionEditorName_Text => ResourceManager.GetString("OptionEditorName_Text", resourceCulture);

        internal static string PasswordRenewal => ResourceManager.GetString("PasswordRenewal", resourceCulture);

        internal static string POUListExceeded => ResourceManager.GetString("POUListExceeded", resourceCulture);

        internal static string program_loaded => ResourceManager.GetString("program_loaded", resourceCulture);

        internal static string ProgramNotChangedDownload => ResourceManager.GetString("ProgramNotChangedDownload", resourceCulture);

        internal static string ProgramNotChangedOnlChange => ResourceManager.GetString("ProgramNotChangedOnlChange", resourceCulture);

        internal static string Prompt_ActivateUserManagement => ResourceManager.GetString("Prompt_ActivateUserManagement", resourceCulture);

        internal static string Prompt_ActivateUserManagement_new => ResourceManager.GetString("Prompt_ActivateUserManagement_new", resourceCulture);

        internal static string Prompt_AdvancedResetOrigin => ResourceManager.GetString("Prompt_AdvancedResetOrigin", resourceCulture);

        internal static string Prompt_BasicResetOrigin => ResourceManager.GetString("Prompt_BasicResetOrigin", resourceCulture);

        internal static string Prompt_UserMgmt_DeleteCurrentUser => ResourceManager.GetString("Prompt_UserMgmt_DeleteCurrentUser", resourceCulture);

        internal static string PromptDeletePLCApplications => ResourceManager.GetString("PromptDeletePLCApplications", resourceCulture);

        internal static string PromptDeletePLCApplicationsCancelDownload => ResourceManager.GetString("PromptDeletePLCApplicationsCancelDownload", resourceCulture);

        internal static string PromptDeletePLCApplicationsOnlyOneAppAllowed => ResourceManager.GetString("PromptDeletePLCApplicationsOnlyOneAppAllowed", resourceCulture);

        internal static string PromptDeviceApplicationCreate => ResourceManager.GetString("PromptDeviceApplicationCreate", resourceCulture);

        internal static string PromptDeviceApplicationDelete => ResourceManager.GetString("PromptDeviceApplicationDelete", resourceCulture);

        internal static string PromptError => ResourceManager.GetString("PromptError", resourceCulture);

        internal static string PromptExecuteLogin => ResourceManager.GetString("PromptExecuteLogin", resourceCulture);

        internal static string PromptExecuteOperation => ResourceManager.GetString("PromptExecuteOperation", resourceCulture);

        internal static string PromptLogoutAppWithForcedValues => ResourceManager.GetString("PromptLogoutAppWithForcedValues", resourceCulture);

        internal static string PromptOpenExtractedProject => ResourceManager.GetString("PromptOpenExtractedProject", resourceCulture);

        internal static string PromptOutdatedDevdesc => ResourceManager.GetString("PromptOutdatedDevdesc", resourceCulture);

        internal static string PromptOverwrite => ResourceManager.GetString("PromptOverwrite", resourceCulture);

        internal static string PromptSaveProjectDuringSourceDownload => ResourceManager.GetString("PromptSaveProjectDuringSourceDownload", resourceCulture);

        internal static string QueryCompileErrors => ResourceManager.GetString("QueryCompileErrors", resourceCulture);

        internal static string QueryCreateApplication => ResourceManager.GetString("QueryCreateApplication", resourceCulture);

        internal static string QueryCreateDevice => ResourceManager.GetString("QueryCreateDevice", resourceCulture);

        internal static string QueryDownload => ResourceManager.GetString("QueryDownload", resourceCulture);

        internal static string QueryDownloadAlways => ResourceManager.GetString("QueryDownloadAlways", resourceCulture);

        internal static string QueryDownloadAlwaysWithWarning => ResourceManager.GetString("QueryDownloadAlwaysWithWarning", resourceCulture);

        internal static string QueryDownloadDevice => ResourceManager.GetString("QueryDownloadDevice", resourceCulture);

        internal static string QueryFinishCycleReset_Text => ResourceManager.GetString("QueryFinishCycleReset_Text", resourceCulture);

        internal static string QueryGenerateCode => ResourceManager.GetString("QueryGenerateCode", resourceCulture);

        internal static string QueryLocationChangedFullDownload => ResourceManager.GetString("QueryLocationChangedFullDownload", resourceCulture);

        internal static string QueryNoOnlChange => ResourceManager.GetString("QueryNoOnlChange", resourceCulture);

        internal static string QueryNoOnlChangeWithWarning => ResourceManager.GetString("QueryNoOnlChangeWithWarning", resourceCulture);

        internal static string QueryOnlChange => ResourceManager.GetString("QueryOnlChange", resourceCulture);

        internal static string QueryOnlChange2 => ResourceManager.GetString("QueryOnlChange2", resourceCulture);

        internal static string QueryOnlChange2_Login => ResourceManager.GetString("QueryOnlChange2_Login", resourceCulture);

        internal static string QueryOnlChange2_LoginDownload => ResourceManager.GetString("QueryOnlChange2_LoginDownload", resourceCulture);

        internal static string QueryOnlChange2_LoginOnlineChange => ResourceManager.GetString("QueryOnlChange2_LoginOnlineChange", resourceCulture);

        internal static string QueryOnlChange3_LoginDownload => ResourceManager.GetString("QueryOnlChange3_LoginDownload", resourceCulture);

        internal static string QueryOnlChange3_LoginOnlineChange => ResourceManager.GetString("QueryOnlChange3_LoginOnlineChange", resourceCulture);

        internal static string QueryOnlChangeWithForceVars => ResourceManager.GetString("QueryOnlChangeWithForceVars", resourceCulture);

        internal static string QueryOnlChangeWithForceVars_Login => ResourceManager.GetString("QueryOnlChangeWithForceVars_Login", resourceCulture);

        internal static string QueryOnlChangeWithForceVars_LoginDownload => ResourceManager.GetString("QueryOnlChangeWithForceVars_LoginDownload", resourceCulture);

        internal static string QueryOnlChangeWithForceVars_LoginOnlineChange => ResourceManager.GetString("QueryOnlChangeWithForceVars_LoginOnlineChange", resourceCulture);

        internal static string QueryOnlChangeWithWarning => ResourceManager.GetString("QueryOnlChangeWithWarning", resourceCulture);

        internal static string QueryProgramChanged => ResourceManager.GetString("QueryProgramChanged", resourceCulture);

        internal static string QueryProgramChangedWithWarning => ResourceManager.GetString("QueryProgramChangedWithWarning", resourceCulture);

        internal static string QuerySafeReferenceContextOffline => ResourceManager.GetString("QuerySafeReferenceContextOffline", resourceCulture);

        internal static string QueryTerminateCycleReset_Text => ResourceManager.GetString("QueryTerminateCycleReset_Text", resourceCulture);

        internal static string ReallyDeleteApplication => ResourceManager.GetString("ReallyDeleteApplication", resourceCulture);

        internal static string ReleaseForceListActiveApplicationCommand_Description => ResourceManager.GetString("ReleaseForceListActiveApplicationCommand_Description", resourceCulture);

        internal static string ReleaseForceListActiveApplicationCommand_Name => ResourceManager.GetString("ReleaseForceListActiveApplicationCommand_Name", resourceCulture);

        internal static string RemoveOnlineUserCommand_Description => ResourceManager.GetString("RemoveOnlineUserCommand_Description", resourceCulture);

        internal static string RemoveOnlineUserCommand_Dialog_Caption => ResourceManager.GetString("RemoveOnlineUserCommand_Dialog_Caption", resourceCulture);

        internal static string RemoveOnlineUserCommand_LogoutMessage => ResourceManager.GetString("RemoveOnlineUserCommand_LogoutMessage", resourceCulture);

        internal static string RemoveOnlineUserCommand_Name => ResourceManager.GetString("RemoveOnlineUserCommand_Name", resourceCulture);

        internal static string ResetColdCascadingCommand_Description => ResourceManager.GetString("ResetColdCascadingCommand_Description", resourceCulture);

        internal static string ResetColdCascadingCommand_Name => ResourceManager.GetString("ResetColdCascadingCommand_Name", resourceCulture);

        internal static string ResetColdSelectedApplicationCmd_Name => ResourceManager.GetString("ResetColdSelectedApplicationCmd_Name", resourceCulture);

        internal static string ResetOriginDeviceCmd_Description => ResourceManager.GetString("ResetOriginDeviceCmd_Description", resourceCulture);

        internal static string ResetOriginDeviceCmd_Name => ResourceManager.GetString("ResetOriginDeviceCmd_Name", resourceCulture);

        internal static string ResetOriginFailed => ResourceManager.GetString("ResetOriginFailed", resourceCulture);

        internal static string ResetOriginSelectedApplicationCmd_Description => ResourceManager.GetString("ResetOriginSelectedApplicationCmd_Description", resourceCulture);

        internal static string ResetOriginSelectedApplicationCmd_Name => ResourceManager.GetString("ResetOriginSelectedApplicationCmd_Name", resourceCulture);

        internal static string ResetWarmCascadingCommand_Description => ResourceManager.GetString("ResetWarmCascadingCommand_Description", resourceCulture);

        internal static string ResetWarmCascadingCommand_Name => ResourceManager.GetString("ResetWarmCascadingCommand_Name", resourceCulture);

        internal static string ResetWarmSelectedApplicationCmd_Name => ResourceManager.GetString("ResetWarmSelectedApplicationCmd_Name", resourceCulture);

        internal static string run => ResourceManager.GetString("run", resourceCulture);

        internal static string ShowCompileChangeDetails_Description => ResourceManager.GetString("ShowCompileChangeDetails_Description", resourceCulture);

        internal static string ShowCompileChangeDetails_Name => ResourceManager.GetString("ShowCompileChangeDetails_Name", resourceCulture);

        internal static string SignatureMismatch => ResourceManager.GetString("SignatureMismatch", resourceCulture);

        internal static string Simulation_Text => ResourceManager.GetString("Simulation_Text", resourceCulture);

        internal static string single_cycle => ResourceManager.GetString("single_cycle", resourceCulture);

        internal static string SingleCycleActiveApplicationCommand_Description => ResourceManager.GetString("SingleCycleActiveApplicationCommand_Description", resourceCulture);

        internal static string SingleCycleActiveApplicationCommand_Name => ResourceManager.GetString("SingleCycleActiveApplicationCommand_Name", resourceCulture);

        internal static string SourceDownload_AllDevices => ResourceManager.GetString("SourceDownload_AllDevices", resourceCulture);

        internal static string SourceDownload_SyncFileDescription => ResourceManager.GetString("SourceDownload_SyncFileDescription", resourceCulture);

        internal static string SourceDownload_SyncProviderName => ResourceManager.GetString("SourceDownload_SyncProviderName", resourceCulture);

        internal static string SourceDownloadCommand_Description => ResourceManager.GetString("SourceDownloadCommand_Description", resourceCulture);

        internal static string SourceDownloadCommand_Name => ResourceManager.GetString("SourceDownloadCommand_Name", resourceCulture);

        internal static string SourceDownloadExCommand_Description => ResourceManager.GetString("SourceDownloadExCommand_Description", resourceCulture);

        internal static string SourceDownloadExCommand_Name => ResourceManager.GetString("SourceDownloadExCommand_Name", resourceCulture);

        internal static string SourceDownloadNotSupported => ResourceManager.GetString("SourceDownloadNotSupported", resourceCulture);

        internal static string SourceDownloadOptionEditorDescription => ResourceManager.GetString("SourceDownloadOptionEditorDescription", resourceCulture);

        internal static string SourceDownloadOptionEditorName => ResourceManager.GetString("SourceDownloadOptionEditorName", resourceCulture);

        internal static string SourceDownloadPrompt_MessageBoxText => ResourceManager.GetString("SourceDownloadPrompt_MessageBoxText", resourceCulture);

        internal static string SourceProgressBytes => ResourceManager.GetString("SourceProgressBytes", resourceCulture);

        internal static string SourceProgressDownload => ResourceManager.GetString("SourceProgressDownload", resourceCulture);

        internal static string SourceProgressFinishing => ResourceManager.GetString("SourceProgressFinishing", resourceCulture);

        internal static string SourceProgressInitializing => ResourceManager.GetString("SourceProgressInitializing", resourceCulture);

        internal static string SourceProgressTransferringArchiveFile => ResourceManager.GetString("SourceProgressTransferringArchiveFile", resourceCulture);

        internal static string SourceProgressUpload => ResourceManager.GetString("SourceProgressUpload", resourceCulture);

        internal static string SourceTransferFailed => ResourceManager.GetString("SourceTransferFailed", resourceCulture);

        internal static string SourceUploadCommand_Description => ResourceManager.GetString("SourceUploadCommand_Description", resourceCulture);

        internal static string SourceUploadCommand_Name => ResourceManager.GetString("SourceUploadCommand_Name", resourceCulture);

        internal static string SourceUploadPrompt_NoUploadFile => ResourceManager.GetString("SourceUploadPrompt_NoUploadFile", resourceCulture);

        internal static string SpecifiedDeviceNotFound => ResourceManager.GetString("SpecifiedDeviceNotFound", resourceCulture);

        internal static string StartActiveApplicationCommand_ContextlessName => ResourceManager.GetString("StartActiveApplicationCommand_ContextlessName", resourceCulture);

        internal static string StartActiveApplicationCommand_Description => ResourceManager.GetString("StartActiveApplicationCommand_Description", resourceCulture);

        internal static string StartActiveApplicationCommand_Name => ResourceManager.GetString("StartActiveApplicationCommand_Name", resourceCulture);

        internal static string StartCascadingCommand_Description => ResourceManager.GetString("StartCascadingCommand_Description", resourceCulture);

        internal static string StartCascadingCommand_Name => ResourceManager.GetString("StartCascadingCommand_Name", resourceCulture);

        internal static string StartSelectedApplicationCommand_ContextlessName => ResourceManager.GetString("StartSelectedApplicationCommand_ContextlessName", resourceCulture);

        internal static string StartSelectedApplicationCommand_Description => ResourceManager.GetString("StartSelectedApplicationCommand_Description", resourceCulture);

        internal static string StartSelectedApplicationCommand_Name => ResourceManager.GetString("StartSelectedApplicationCommand_Name", resourceCulture);

        internal static string stop => ResourceManager.GetString("stop", resourceCulture);

        internal static string StopActiveApplicationCommand_ContextlessName => ResourceManager.GetString("StopActiveApplicationCommand_ContextlessName", resourceCulture);

        internal static string StopActiveApplicationCommand_Description => ResourceManager.GetString("StopActiveApplicationCommand_Description", resourceCulture);

        internal static string StopActiveApplicationCommand_Name => ResourceManager.GetString("StopActiveApplicationCommand_Name", resourceCulture);

        internal static string StopCascadingCommand_Description => ResourceManager.GetString("StopCascadingCommand_Description", resourceCulture);

        internal static string StopCascadingCommand_Name => ResourceManager.GetString("StopCascadingCommand_Name", resourceCulture);

        internal static string StopSelectedApplicationCommand_ContextlessName => ResourceManager.GetString("StopSelectedApplicationCommand_ContextlessName", resourceCulture);

        internal static string StopSelectedApplicationCommand_Description => ResourceManager.GetString("StopSelectedApplicationCommand_Description", resourceCulture);

        internal static string StopSelectedApplicationCommand_Name => ResourceManager.GetString("StopSelectedApplicationCommand_Name", resourceCulture);

        internal static string store_bootproject => ResourceManager.GetString("store_bootproject", resourceCulture);

        internal static string SwitchSimulationModeActiveCommand_ContextlessName => ResourceManager.GetString("SwitchSimulationModeActiveCommand_ContextlessName", resourceCulture);

        internal static string SwitchSimulationModeActiveCommand_Description => ResourceManager.GetString("SwitchSimulationModeActiveCommand_Description", resourceCulture);

        internal static string SwitchSimulationModeActiveCommand_Name => ResourceManager.GetString("SwitchSimulationModeActiveCommand_Name", resourceCulture);

        internal static string SwitchSimulationModeSelectionCommand_ContextlessName => ResourceManager.GetString("SwitchSimulationModeSelectionCommand_ContextlessName", resourceCulture);

        internal static string SwitchSimulationModeSelectionCommand_Description => ResourceManager.GetString("SwitchSimulationModeSelectionCommand_Description", resourceCulture);

        internal static string SwitchSimulationModeSelectionCommand_Name => ResourceManager.GetString("SwitchSimulationModeSelectionCommand_Name", resourceCulture);

        internal static string SystemApplication => ResourceManager.GetString("SystemApplication", resourceCulture);

        internal static string TargetMismatch_Id => ResourceManager.GetString("TargetMismatch_Id", resourceCulture);

        internal static string TargetMismatch_Type => ResourceManager.GetString("TargetMismatch_Type", resourceCulture);

        internal static string TargetMismatch_Version => ResourceManager.GetString("TargetMismatch_Version", resourceCulture);

        internal static string TooManyDecimalPlaces => ResourceManager.GetString("TooManyDecimalPlaces", resourceCulture);

        internal static string UnforceAllApplicationsCommand_ContextlessName => ResourceManager.GetString("UnforceAllApplicationsCommand_ContextlessName", resourceCulture);

        internal static string UnforceAllApplicationsCommand_Description => ResourceManager.GetString("UnforceAllApplicationsCommand_Description", resourceCulture);

        internal static string UnforceAllApplicationsCommand_Name => ResourceManager.GetString("UnforceAllApplicationsCommand_Name", resourceCulture);

        internal static string UnforceFailedForSomeVariables => ResourceManager.GetString("UnforceFailedForSomeVariables", resourceCulture);

        internal static string UnforceSelectedAppCommand_ContextlessName => ResourceManager.GetString("UnforceSelectedAppCommand_ContextlessName", resourceCulture);

        internal static string UnforceSelectedAppCommand_Description => ResourceManager.GetString("UnforceSelectedAppCommand_Description", resourceCulture);

        internal static string UnforceSelectedAppCommand_Name => ResourceManager.GetString("UnforceSelectedAppCommand_Name", resourceCulture);

        internal static string UnforceValuesCommand_ContextlessName => ResourceManager.GetString("UnforceValuesCommand_ContextlessName", resourceCulture);

        internal static string UnforceValuesCommand_Description => ResourceManager.GetString("UnforceValuesCommand_Description", resourceCulture);

        internal static string UnforceValuesCommand_Name => ResourceManager.GetString("UnforceValuesCommand_Name", resourceCulture);

        internal static string unknown => ResourceManager.GetString("unknown", resourceCulture);

        internal static string UnresolvedReference => ResourceManager.GetString("UnresolvedReference", resourceCulture);

        internal static string UnresolvedSystemApplication => ResourceManager.GetString("UnresolvedSystemApplication", resourceCulture);

        internal static string UserManagement => ResourceManager.GetString("UserManagement", resourceCulture);

        internal static string UserOperation_AddRemove => ResourceManager.GetString("UserOperation_AddRemove", resourceCulture);

        internal static string UserOperation_Execute => ResourceManager.GetString("UserOperation_Execute", resourceCulture);

        internal static string UserOperation_Modify => ResourceManager.GetString("UserOperation_Modify", resourceCulture);

        internal static string UserOperation_View => ResourceManager.GetString("UserOperation_View", resourceCulture);

        internal static string VariableListExceeded => ResourceManager.GetString("VariableListExceeded", resourceCulture);

        internal static string VersionMismatch => ResourceManager.GetString("VersionMismatch", resourceCulture);

        internal static string Warning_ApplicationLogin_DifferentDevice => ResourceManager.GetString("Warning_ApplicationLogin_DifferentDevice", resourceCulture);

        internal static string WarnUserOfMissingCompileInfo => ResourceManager.GetString("WarnUserOfMissingCompileInfo", resourceCulture);

        internal static string WriteAllApplicationsCommand_ContextlessName => ResourceManager.GetString("WriteAllApplicationsCommand_ContextlessName", resourceCulture);

        internal static string WriteAllApplicationsCommand_Description => ResourceManager.GetString("WriteAllApplicationsCommand_Description", resourceCulture);

        internal static string WriteAllApplicationsCommand_Name => ResourceManager.GetString("WriteAllApplicationsCommand_Name", resourceCulture);

        internal static string WriteFailedForSomeForcedVariables => ResourceManager.GetString("WriteFailedForSomeForcedVariables", resourceCulture);

        internal static string WriteFailedForSomeVariables => ResourceManager.GetString("WriteFailedForSomeVariables", resourceCulture);

        internal static string WriteSelectedAppCommand_ContextlessName => ResourceManager.GetString("WriteSelectedAppCommand_ContextlessName", resourceCulture);

        internal static string WriteSelectedAppCommand_Description => ResourceManager.GetString("WriteSelectedAppCommand_Description", resourceCulture);

        internal static string WriteSelectedAppCommand_Name => ResourceManager.GetString("WriteSelectedAppCommand_Name", resourceCulture);

        internal static string WriteValuesCommand_ContextlessName => ResourceManager.GetString("WriteValuesCommand_ContextlessName", resourceCulture);

        internal static string WriteValuesCommand_Description => ResourceManager.GetString("WriteValuesCommand_Description", resourceCulture);

        internal static string WriteValuesCommand_Name => ResourceManager.GetString("WriteValuesCommand_Name", resourceCulture);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings()
        {
        }
    }
}
