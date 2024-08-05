using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceCommunicationEditor;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.FdtIntegration;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.LegacyOnlineManager;
using _3S.CoDeSys.LibManEditor;
using _3S.CoDeSys.LibManObject;
using _3S.CoDeSys.OnlineUI;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.WatchList;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DependencyBag : IDependencyInjectable
	{
		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOnlineManager8> OnlineMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IUndoManager> UndoMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IFeatureSettingsManager> FeatureSettingsMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IDeviceManager10> DeviceMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IDeviceController> DeviceControllerProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ITargetSettingsManager> TargetSettingsMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ITargetSettingsProvider> TargetSettingsProviderProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IVariablesInputAssistantCategory> VariablesInputAssistantCategoryProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInputAssistantService2> InputAssistantServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IObjectManager8> ObjectMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IRefactoringService> RefactoringServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOptionStorage> OptionStorageProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ISingleLineIECTextEditor5> SingleLineIECTextEditorProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IEngine3> EngineProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IFdtIntegration> FdtIntegrationProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILanguageModelManager22> LanguageModelMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IDeviceRepository> DeviceRepositoryProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMessageServiceWithKeys> MessageServiceProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IGeneratedObjectProtector> GeneratedObjectProtectorsProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IConnectorEditorFactory> ConnectorEditorFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IDeviceCommunicationEditorFactory> DeviceCommunicationEditorFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IDeviceEditorFactory> DeviceEditorFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IAppearancePageFactory> AppearancePageFactoriesProvider { get; private set; }

		[InjectAnyInstance]
		public IAnyInstanceProvider<IOnlineHelpService> AnyOnlineHelpServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<ILegacyOnlineManager> LegacyOnlineMgrProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IStatusPageAdditionalControlFactory> StatusPageAdditionalControlFactoriesProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IDpiAdapter> DpiAdapterProvider { get; private set; }

		[InjectSingleInstance(Optional = true)]
		public ISingleInstanceProvider<IChangeForceValueService> ChangeForceValueServiceProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IHiddenObjectAdorner> HiddenObjectAdornersProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IIoMappingEditorFilterFactory> IoMappingFiltersProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<ILocalizationManager> LocalizationManagerProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<IDeviceParameterPrintFactory> DeviceParameterPrintFactoriesProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ICompilerVersionManager6> CompilerVersionMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IComPageCustomizer> ComPageCustomizerProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IWatchListView3> WatchListViewProvider { get; private set; }

		[InjectMultipleInstances(Optional = true)]
		public IMultipleInstancesProvider<ILibManEditorViewFactory> LibManEditorViewFactoriesProvider { get; private set; }

		[InjectMultipleInstances(Optional = true, Shared = true)]
		public ISharedMultipleInstancesProvider<ICustomizedLibManEditorViewFactorySelector> CustomizedLibManEditorViewFactorySelectorsProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILibraryLoader13> LibraryLoaderProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IDeviceParameterHideColumnFactory> ParameterColumnHideFactoriesProvider { get; private set; }

		public DependencyBag()
		{
			ComponentModel.Singleton.InjectDependencies((IDependencyInjectable)(object)this, GetType());
		}

		public void InjectionComplete()
		{
		}
	}
}
