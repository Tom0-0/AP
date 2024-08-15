using System;
using System.Collections.Generic;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
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
	internal static class APEnvironment
	{
		private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

		public static IMessageServiceWithKeys MessageService => s_bag.Value.MessageServiceProvider.Value;

		public static IWinFormWrapper FrameForm => s_bag.Value.FrameFormProxy.FrameForm;

		public static IOnlineManager3 OnlineMgr => s_bag.Value.OnlineMgrProvider.Value;

		public static IFeatureSettingsManager FeatureSettingsMgrOrNull => s_bag.Value.FeatureSettingsMgrProvider.Value;

		public static DeviceManager DeviceMgr => s_bag.Value.DeviceMgrProvider.Value;

		public static DeviceController DeviceController => s_bag.Value.DeviceControllerProvider.Value;

		public static ITargetSettingsManager TargetSettingsMgr => s_bag.Value.TargetSettingsMgrProvider.Value;

		public static ITargetSettingsProvider TargetSettingsProvider => s_bag.Value.TargetSettingsProviderProvider.Value;

		public static IObjectManager8 ObjectMgr => s_bag.Value.ObjectMgrProvider.Value;

		public static IOnlineUIServices5 OnlineUIMgr => s_bag.Value.OnlineUIMgrProvider.Value;

		public static Guid PlaceholderResolutionTypeGuid => s_bag.Value.PlaceholderResolutionProvider.TypeGuid;

		public static Guid LicenseLibPlaceholderResolutionTypeGuid => s_bag.Value.LicenseLibPlaceholderResolutionTypeInfo.TypeGuid;

		public static IManagedLibraryManager ManagedLibraryMgr => s_bag.Value.ManagedLibraryMgrProvider.Value;

		public static ILibraryLoader13 LibraryLoader => s_bag.Value.LibraryLoaderProvider.Value;

		public static IRefactoringService RefactoringServiceOrNull => s_bag.Value.RefactoringServiceProvider.Value;

		public static IMessageStorage MessageStorage => s_bag.Value.MessageStorageProvider.Value;

		public static ISimulationManager SimulationMgrOrNull => s_bag.Value.SimulationMgrProvider.Value;

		public static IOptionStorage OptionStorage => s_bag.Value.OptionStorageProvider.Value;

		public static IEngine5 Engine => s_bag.Value.EngineProvider.Value;

		public static IFdtIntegration2 FdtIntegrationOrNull => s_bag.Value.FdtIntegrationProvider.Value;

		public static ILanguageModelManager17 LanguageModelMgr => s_bag.Value.LanguageModelMgrProvider.Value;

		public static ICompilerVersionManager6 CompilerVersionMgr => s_bag.Value.CompilerVersionMgrProvider.Value;

		public static IMessageCategory CompilerMessageCategory => s_bag.Value.CompilerMessageCategoryProvider.Value;

		public static IDeviceRepository8 DeviceRepository => s_bag.Value.DeviceRepositoryProvider.Value;

		public static IEnumerable<IUpdateDeviceParametersFactory> UpdateDeviceParametersFactories => s_bag.Value.UpdateDeviceParametersFactoriesProvider.Value;

		public static IEnumerable<IUpdateConnectorsFactory> UpdateConnectorsFactories => s_bag.Value.UpdateConnectorsFactoriesProvider.Value;

		public static IEnumerable<ISortTaskMapFactory> SortTaskMapFactories => s_bag.Value.SortTaskMapFactoriesProvider.Value;

		public static Type SafetyEL6900TypeOrNull => s_bag.Value.SafetyEL6900TypeInfo.Type;

		public static bool ExistsSafetyEL6900 => SafetyEL6900TypeOrNull != null;

		public static Type SafetyTypeOrNull => s_bag.Value.SafetyTypeInfo.Type;

		public static bool ExistsSafety => SafetyTypeOrNull != null;

		public static IEnumerable<IFbInstanceFactory> FbInstanceFactories => s_bag.Value.FbInstanceFactoriesProvider.Value;

		public static IEnumerable<IDeviceObjectFindReplaceFactory> DeviceObjectFindReplaceFactories => s_bag.Value.DeviceObjectFindReplaceFactoriesProvider.Value;

		public static IIECTextComparer IECTextComparer => s_bag.Value.IECTextComparerProvider.Value;

		public static IDpiAdapter DpiAdapter => s_bag.Value.DpiAdapterProvider.Value;

		public static ILanguageModelUtilities LanguageModelUtilities => s_bag.Value.LanguageModelUtilitiesProvider.Value;

		public static IEnumerable<IHiddenObjectAdorner> HiddenObjectAdorners => s_bag.Value.HiddenObjectAdornersProvider.Value;

		public static ILocalizationManager LocalizationManagerOrNull => s_bag.Value.LocalizationManagerProvider.Value;

		public static ILMServiceProvider LMServiceProvider => s_bag.Value.LMServiceProviderProvider.Value;

		public static ITextVarDeclObject CreateVarDeclObject()
		{
			return s_bag.Value.VarDeclObjectProvider.Create();
		}

		public static IUndoManager CreateUndoMgr()
		{
			return s_bag.Value.UndoMgrProvider.Create();
		}

		public static IApplicationObject CreateApplicationObject()
		{
			return s_bag.Value.ApplicationObjectProvider.Create();
		}

		public static IApplicationObject CreateHiddenApplicationObject()
		{
			return s_bag.Value.HiddenApplicationObjectProvider.Create();
		}

		public static IObjectFactory CreateDeviceObjectFactory()
		{
			return s_bag.Value.DeviceObjectFactoryProvider.Create();
		}

		public static ILoginService2 CreateLoginServiceWrapper()
		{
			return s_bag.Value.LoginServiceWrapperProvider.Create();
		}

		public static ILibraryPlaceholderResolution CreatePlaceholderResolution()
		{
			return s_bag.Value.PlaceholderResolutionProvider.Create();
		}

		public static IPOUObject CreatePOUObject()
		{
			return s_bag.Value.POUObjectProvider.Create();
		}

		public static ICrossReferenceService2 CreateCrossReferenceService()
		{
			return s_bag.Value.CrossReferenceServiceProvider.Create();
		}

		public static ISTImplementationObject CreateSTImplementationObject()
		{
			return s_bag.Value.STImplementationObjectProvider.Create();
		}

		public static ITaskConfigObject CreateTaskConfigObject()
		{
			return s_bag.Value.TaskConfigObjectProvider.Create();
		}

		public static IGenericInterfaceExtensionProvider TryCreateGenericInterfaceExtensionProviderImpl()
		{
			return s_bag.Value.GenericInterfaceExtensionProviderImplProvider.Create();
		}

		public static ITaskObject CreateTaskObject()
		{
			return s_bag.Value.TaskObjectProvider.Create();
		}

		public static IDeviceCatalogue CreateFirstDeviceCatalogue()
		{
			return s_bag.Value.DeviceCataloguesProvider.CreateFirst();
		}

		public static IEnumerable<IUpdateDeviceIDProvider> CreateUpdateDeviceIDProviders()
		{
			return s_bag.Value.UpdateDeviceIDProvidersProvider.Create();
		}

		public static IAddressAssignmentStrategy TryCreateAddressAssignmentStrategy(Guid typeGuid)
		{
			return s_bag.Value.AnyAddressAssignmentStrategyProvider.TryCreate(typeGuid);
		}

		public static IEnumerable<IDeviceObjectComparerFactory> CreateDeviceObjectComparerFactories()
		{
			return s_bag.Value.DeviceObjectComparerFactoriesProvider.Create();
		}

		public static IEnumerable<IDeviceObjectEmbeddedDiffViewerFactory> CreateDeviceObjectEmbeddedDiffViewerFactories()
		{
			return s_bag.Value.DeviceObjectEmbeddedDiffViewerFactoriesProvider.Create();
		}

		public static IEnumerable<IDiffViewerDeviceParameterFilterFactory> CreateDiffViewerDeviceParameterFilterFactories()
		{
			return s_bag.Value.DiffViewerDeviceParameterFilterFactoriesProvider.Create();
		}

		public static IEnumerable<IHideDeviceDisableFactory> CreateHideDeviceDisableFactories()
		{
			return s_bag.Value.HideDeviceDisableFactoriesProvider.Create();
		}

		public static Type TryGetType(Guid typeGuid)
		{
			return s_bag.Value.AnyTypeInfo.TryGetType(typeGuid);
		}

		public static IIECEmbeddedDiffViewer CreateIECEmbeddedDiffViewer()
		{
			return s_bag.Value.IECEmbeddedDiffViewerProvider.Create();
		}

		public static IArchiveReader CreateBinaryArchiveReader()
		{
			return s_bag.Value.BinaryArchiveReaderProvider.Create();
		}

		public static IEnumerable<IConfigVersionUpdateEnvironmentFactory> CreateCheckUpdateVersionFactories()
		{
			return s_bag.Value.CheckUpdateVersionFactoriesProvider.Create();
		}

		public static IBuildProperty CreateBuildProperty()
		{
			return s_bag.Value.BuildPropertyProvider.Create();
		}

		public static IObjectFactory TryCreateObjectFactory(Guid typeGuid)
		{
			return s_bag.Value.AnyObjectFactoryProvider.TryCreate(typeGuid);
		}

		public static IEnumerable<ILogicalDeviceCommandFactory> CreateLogicalDeviceCommandFactory()
		{
			return s_bag.Value.LogicalDeviceCommandFactoryProvider.Create();
		}
	}
}
