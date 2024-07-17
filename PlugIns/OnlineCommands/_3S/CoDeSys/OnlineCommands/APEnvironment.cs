using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.Security;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceEditor;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectArchive;
using _3S.CoDeSys.UserManagement;
using System;
using System.Collections.Generic;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class APEnvironment
    {
        private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

        public static IWinFormWrapper FrameForm => s_bag.Value.FrameFormProxy.FrameForm;

        public static IMessageServiceWithKeys MessageService => s_bag.Value.MessageServiceProvider.Value;

        public static string ProfileName => s_bag.Value.ProfileInformation.ProfileName;

        public static IOnlineManager11 OnlineMgr => s_bag.Value.OnlineMgrProvider.Value;

        public static ITargetSettingsManager TargetSettingsMgr => s_bag.Value.TargetSettingsMgrProvider.Value;

        public static ITargetSettingsProvider TargetSettingsProvider => s_bag.Value.TargetSettingsProviderProvider.Value;

        public static IObjectManager6 ObjectMgr => s_bag.Value.ObjectMgrProvider.Value;

        public static OnlineUIManager OnlineUIMgr => s_bag.Value.OnlineUIMgrProvider.Value;

        public static IUserManagement UserManagementOrNull => s_bag.Value.UserManagementProvider.Value;

        public static IMessageStorage MessageStorage => s_bag.Value.MessageStorageProvider.Value;

        public static IBPManager6 BPMgr => s_bag.Value.BPMgrProvider.Value;

        public static IOptionStorage OptionStorage => s_bag.Value.OptionStorageProvider.Value;

        public static IEngine10 Engine => s_bag.Value.EngineProvider.Value;

        public static ILanguageModelManager30 LanguageModelMgr => s_bag.Value.LanguageModelMgrProvider.Value;

        public static ILMServiceProvider LMServiceProvider => s_bag.Value.LMServiceProviderProvider.Value;

        public static ICompilerVersionManager6 CompilerVersionMgr => s_bag.Value.CompilerVersionMgrProvider.Value;

        public static IEnumerable<IProjectArchiveCategory> ProjectArchiveCategories => s_bag.Value.ProjectArchiveCategoriesProvider.Value;

        public static IEnumerable<IMultipleDownloadExtensionProvider> MultipleDownloadExtensionProviders => s_bag.Value.MultipleDownloadExtensionProvidersProvider.Value;

        public static IEnumerable<IHiddenObjectAdorner> HiddenObjectAdorners => s_bag.Value.HiddenObjectAdornersProvider.Value;

        public static ISecurityManager2 SecurityMgr => s_bag.Value.SecurityMgrProvider.Value;

        public static IOutdatedDevdescHandler OutdatedDevdescHandlerOrNull => s_bag.Value.OutdatedDevdescHandlerProvider.Value;

        public static IX509UIService4 X509UIService4 => s_bag.Value.X509UIService4Provider.Value;

        public static IEnumerable<IMultipleDownloadApplicationHandler> MultipleDownloadApplicationHandler => s_bag.Value.MultipleDownloadApplicationHandlerProvider.Value;

        public static Type LogicalAppPropertyType => s_bag.Value.LogicalAppPropertyTypeInfo.Type;

        public static IDpiAdapter DpiAdapter => s_bag.Value.DpiAdapterProvider.Value;

        public static IMessageCategory CreateBuildMessageCategory()
        {
            return s_bag.Value.BuildMessageCategoryProvider.Create();
        }

        public static ISelectDeviceService CreateSelectDeviceService()
        {
            return s_bag.Value.SelectDeviceServiceProvider.Create();
        }

        public static IApplicationObject CreateHiddenAndTransientApplicationObject()
        {
            return s_bag.Value.HiddenAndTransientApplicationObjectProvider.Create();
        }

        public static ILoginService2 CreateLoginServiceWrapper()
        {
            return s_bag.Value.LoginServiceWrapperProvider.Create();
        }

        public static IProjectArchiveService CreateProjectArchiveService()
        {
            return s_bag.Value.ProjectArchiveServiceProvider.Create();
        }

        public static IGenericInterfaceExtensionProvider TryCreateGenericInterfaceExtensionProviderImpl()
        {
            return s_bag.Value.GenericInterfaceExtensionProviderImplProvider.Create();
        }

        public static IEnumerable<IDeviceFeatureChecker> CreateDeviceFeatureCheckers()
        {
            return s_bag.Value.DeviceFeatureCheckersProvider.Create();
        }

        public static ICustomSelectDeviceServiceProvider TryCreateFirstCustomSelectDeviceServiceProvider()
        {
            return s_bag.Value.CustomSelectDeviceServiceProvidersProvider.CreateFirst();
        }

        public static IMultipleDownloadReorganizer TryCreateFirstMultipleDownloadReorganizer()
        {
            return s_bag.Value.MultipleDownloadReorganizersProvider.CreateFirst();
        }

        public static IProjectArchiveCategory CreateProjectArchiveCategory(Guid typeGuid)
        {
            return s_bag.Value.AnyProjectArchiveCategoryProvider.Create(typeGuid);
        }

        public static IExtractProjectArchiveNotifyHandler3 CreateExtractProjectArchiveNotifyHandler()
        {
            return s_bag.Value.ExtractProjectArchiveNotifyHandlerProvider.Create();
        }
    }
}
