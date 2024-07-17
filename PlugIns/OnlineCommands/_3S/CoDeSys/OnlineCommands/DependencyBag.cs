using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
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

namespace _3S.CoDeSys.OnlineCommands
{
    internal class DependencyBag : IDependencyInjectable
    {
        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IOnlineManager11> OnlineMgrProvider { get; private set; }

        [InjectSingleInstance]
        public ISingleInstanceProvider<ISelectDeviceService> SelectDeviceServiceProvider { get; private set; }

        [InjectSingleInstance]
        public ISingleInstanceProvider<IApplicationObject> HiddenAndTransientApplicationObjectProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ITargetSettingsManager> TargetSettingsMgrProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ITargetSettingsProvider> TargetSettingsProviderProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IObjectManager6> ObjectMgrProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<OnlineUIManager> OnlineUIMgrProvider { get; private set; }

        [InjectSingleInstance]
        public ISingleInstanceProvider<ILoginService2> LoginServiceWrapperProvider { get; private set; }

        [InjectSingleInstance(Shared = true, Optional = true)]
        public ISharedSingleInstanceProvider<IUserManagement> UserManagementProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IMessageStorage> MessageStorageProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IBPManager6> BPMgrProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IOptionStorage> OptionStorageProvider { get; private set; }

        [InjectSingleInstance]
        public ISingleInstanceProvider<IProjectArchiveService> ProjectArchiveServiceProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IEngine10> EngineProvider { get; private set; }

        [InjectSingleInstance(Optional = true)]
        public ISingleInstanceProvider<IGenericInterfaceExtensionProvider> GenericInterfaceExtensionProviderImplProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ILanguageModelManager30> LanguageModelMgrProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ILMServiceProvider> LMServiceProviderProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ICompilerVersionManager6> CompilerVersionMgrProvider { get; private set; }

        [InjectFrameForm]
        public IWinFormWrapperProxy FrameFormProxy { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IMessageServiceWithKeys> MessageServiceProvider { get; private set; }

        [InjectMultipleInstances(Optional = true, Shared = true)]
        public ISharedMultipleInstancesProvider<IProjectArchiveCategory> ProjectArchiveCategoriesProvider { get; private set; }

        [InjectMultipleInstances(Optional = true)]
        public IMultipleInstancesProvider<IDeviceFeatureChecker> DeviceFeatureCheckersProvider { get; private set; }

        [InjectMultipleInstances(Optional = true)]
        public IMultipleInstancesProvider<ICustomSelectDeviceServiceProvider> CustomSelectDeviceServiceProvidersProvider { get; private set; }

        [InjectMultipleInstances(Optional = true, Shared = true)]
        public ISharedMultipleInstancesProvider<IMultipleDownloadExtensionProvider> MultipleDownloadExtensionProvidersProvider { get; private set; }

        [InjectActiveProfile]
        public IProfileInformation ProfileInformation { get; private set; }

        [InjectMultipleInstances(Optional = true)]
        public IMultipleInstancesProvider<IMultipleDownloadReorganizer> MultipleDownloadReorganizersProvider { get; private set; }

        [InjectAnyInstance]
        public IAnyInstanceProvider<IProjectArchiveCategory> AnyProjectArchiveCategoryProvider { get; private set; }

        [InjectMultipleInstances(Shared = true, Optional = true)]
        public ISharedMultipleInstancesProvider<IHiddenObjectAdorner> HiddenObjectAdornersProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<ISecurityManager2> SecurityMgrProvider { get; private set; }

        [InjectSingleInstance]
        public ISingleInstanceProvider<IMessageCategory> BuildMessageCategoryProvider { get; private set; }

        [InjectSingleInstance(Shared = true, Optional = true)]
        public ISharedSingleInstanceProvider<IOutdatedDevdescHandler> OutdatedDevdescHandlerProvider { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IX509UIService4> X509UIService4Provider { get; private set; }

        [InjectMultipleInstances(Shared = true, Optional = true)]
        public ISharedMultipleInstancesProvider<IMultipleDownloadApplicationHandler> MultipleDownloadApplicationHandlerProvider { get; private set; }

        [InjectSingleInstance]
        public ISingleInstanceProvider<IExtractProjectArchiveNotifyHandler3> ExtractProjectArchiveNotifyHandlerProvider { get; private set; }

        [InjectSingleTypeInformation]
        public ISpecificTypeInformation LogicalAppPropertyTypeInfo { get; private set; }

        [InjectSingleInstance(Shared = true)]
        public ISharedSingleInstanceProvider<IDpiAdapter> DpiAdapterProvider { get; private set; }

        public DependencyBag()
        {
            ComponentModel.Singleton.InjectDependencies((IDependencyInjectable)(object)this, GetType());
        }

        public void InjectionComplete()
        {
        }
    }
}
