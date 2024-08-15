using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.FdtIntegration;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.OnlineCommands;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.POUObject;
using _3S.CoDeSys.ProjectCompare;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Simulation;
using _3S.CoDeSys.STObject;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.VarDeclObject;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DependencyBag : IDependencyInjectable
	{
		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOnlineManager3> OnlineMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ITextVarDeclObject> VarDeclObjectProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IUndoManager> UndoMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IApplicationObject> ApplicationObjectProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IApplicationObject> HiddenApplicationObjectProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IFeatureSettingsManager> FeatureSettingsMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<DeviceManager> DeviceMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IObjectFactory> DeviceObjectFactoryProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<DeviceController> DeviceControllerProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ITargetSettingsManager> TargetSettingsMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ITargetSettingsProvider> TargetSettingsProviderProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IObjectManager8> ObjectMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOnlineUIServices5> OnlineUIMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ILoginService2> LoginServiceWrapperProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ILibraryPlaceholderResolution> PlaceholderResolutionProvider { get; private set; }

		[InjectSingleTypeInformation(Optional = true)]
		public ISpecificTypeInformation LicenseLibPlaceholderResolutionTypeInfo { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IManagedLibraryManager> ManagedLibraryMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILibraryLoader13> LibraryLoaderProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IRefactoringService> RefactoringServiceProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IPOUObject> POUObjectProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMessageStorage> MessageStorageProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ICrossReferenceService2> CrossReferenceServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<ISimulationManager> SimulationMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ISTImplementationObject> STImplementationObjectProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOptionStorage> OptionStorageProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ITaskConfigObject> TaskConfigObjectProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IEngine5> EngineProvider { get; private set; }

		[InjectSingleInstance(Optional = true)]
		public ISingleInstanceProvider<IGenericInterfaceExtensionProvider> GenericInterfaceExtensionProviderImplProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IFdtIntegration2> FdtIntegrationProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ITaskObject> TaskObjectProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILanguageModelManager17> LanguageModelMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ICompilerVersionManager6> CompilerVersionMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMessageCategory> CompilerMessageCategoryProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IDeviceRepository8> DeviceRepositoryProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMessageServiceWithKeys> MessageServiceProvider { get; private set; }

		[InjectFrameForm]
		public IWinFormWrapperProxy FrameFormProxy { get; private set; }

		[InjectMultipleInstances]
		public IMultipleInstancesProvider<IDeviceCatalogue> DeviceCataloguesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IUpdateDeviceIDProvider> UpdateDeviceIDProvidersProvider { get; private set; }

		[InjectMultipleInstances(Optional = true, Shared = true)]
		public ISharedMultipleInstancesProvider<IUpdateDeviceParametersFactory> UpdateDeviceParametersFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true, Shared = true)]
		public ISharedMultipleInstancesProvider<IUpdateConnectorsFactory> UpdateConnectorsFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true, Shared = true)]
		public ISharedMultipleInstancesProvider<ISortTaskMapFactory> SortTaskMapFactoriesProvider { get; private set; }

		[InjectSingleTypeInformation(Optional = true)]
		public ISpecificTypeInformation SafetyEL6900TypeInfo { get; private set; }

		[InjectSingleTypeInformation(Optional = true)]
		public ISpecificTypeInformation SafetyTypeInfo { get; private set; }

		[InjectMultipleInstances(Optional = true, Shared = true)]
		public ISharedMultipleInstancesProvider<IFbInstanceFactory> FbInstanceFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true, Shared = true)]
		public ISharedMultipleInstancesProvider<IDeviceObjectFindReplaceFactory> DeviceObjectFindReplaceFactoriesProvider { get; private set; }

		[InjectAnyInstance]
		public IAnyInstanceProvider<IAddressAssignmentStrategy> AnyAddressAssignmentStrategyProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IDeviceObjectComparerFactory> DeviceObjectComparerFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IDeviceObjectEmbeddedDiffViewerFactory> DeviceObjectEmbeddedDiffViewerFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IDiffViewerDeviceParameterFilterFactory> DiffViewerDeviceParameterFilterFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IHideDeviceDisableFactory> HideDeviceDisableFactoriesProvider { get; private set; }

		[InjectAnyTypeInformation]
		public IAnyTypeInformation AnyTypeInfo { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IIECTextComparer> IECTextComparerProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IIECEmbeddedDiffViewer> IECEmbeddedDiffViewerProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IDpiAdapter> DpiAdapterProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILanguageModelUtilities> LanguageModelUtilitiesProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IHiddenObjectAdorner> HiddenObjectAdornersProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IArchiveReader> BinaryArchiveReaderProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IConfigVersionUpdateEnvironmentFactory> CheckUpdateVersionFactoriesProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IBuildProperty> BuildPropertyProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<ILocalizationManager> LocalizationManagerProvider { get; private set; }

		[InjectAnyInstance]
		public IAnyInstanceProvider<IObjectFactory> AnyObjectFactoryProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILMServiceProvider> LMServiceProviderProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<ILogicalDeviceCommandFactory> LogicalDeviceCommandFactoryProvider { get; private set; }

		public DependencyBag()
		{
			ComponentModel.Singleton.InjectDependencies((IDependencyInjectable)(object)this, GetType());
		}

		public void InjectionComplete()
		{
		}
	}
}
