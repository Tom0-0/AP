using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core;
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
	internal static class APEnvironment
	{
		private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

		public static IMessageServiceWithKeys MessageService => s_bag.Value.MessageServiceProvider.Value;

		public static IOnlineManager8 OnlineMgr => s_bag.Value.OnlineMgrProvider.Value;

		public static IFeatureSettingsManager FeatureSettingsMgrOrNull => s_bag.Value.FeatureSettingsMgrProvider.Value;

		public static IDeviceManager10 DeviceMgr => s_bag.Value.DeviceMgrProvider.Value;

		public static IDeviceController DeviceController => s_bag.Value.DeviceControllerProvider.Value;

		public static ITargetSettingsManager TargetSettingsMgr => s_bag.Value.TargetSettingsMgrProvider.Value;

		public static ITargetSettingsProvider TargetSettingsProvider => s_bag.Value.TargetSettingsProviderProvider.Value;

		public static IObjectManager8 ObjectMgr => s_bag.Value.ObjectMgrProvider.Value;

		public static IRefactoringService RefactoringService => s_bag.Value.RefactoringServiceProvider.Value;

		public static IOptionStorage OptionStorage => s_bag.Value.OptionStorageProvider.Value;

		public static IEngine3 Engine => s_bag.Value.EngineProvider.Value;

		public static IFdtIntegration FdtIntegrationOrNull => s_bag.Value.FdtIntegrationProvider.Value;

		public static ILanguageModelManager22 LanguageModelMgr => s_bag.Value.LanguageModelMgrProvider.Value;

		public static IDeviceRepository DeviceRepository => s_bag.Value.DeviceRepositoryProvider.Value;

		public static IEnumerable<IGeneratedObjectProtector> GeneratedObjectProtectors => s_bag.Value.GeneratedObjectProtectorsProvider.Value;

		public static IEnumerable<IConnectorEditorFactory> ConnectorEditorFactories => s_bag.Value.ConnectorEditorFactoriesProvider.Value;

		public static IEnumerable<IDeviceCommunicationEditorFactory> DeviceCommunicationEditorFactories => s_bag.Value.DeviceCommunicationEditorFactoriesProvider.Value;

		public static IEnumerable<IDeviceEditorFactory> DeviceEditorFactories => s_bag.Value.DeviceEditorFactoriesProvider.Value;

		public static IEnumerable<IAppearancePageFactory> AppearancePageFactories => s_bag.Value.AppearancePageFactoriesProvider.Value;

		public static ILegacyOnlineManager LegacyOnlineMgrOrNull => s_bag.Value.LegacyOnlineMgrProvider.Value;

		public static IDpiAdapter DpiAdapter => s_bag.Value.DpiAdapterProvider.Value;

		public static IEnumerable<IHiddenObjectAdorner> HiddenObjectAdorners => s_bag.Value.HiddenObjectAdornersProvider.Value;

		public static IEnumerable<IIoMappingEditorFilterFactory> IoMappingFilters => s_bag.Value.IoMappingFiltersProvider.Value;

		public static ILocalizationManager LocalizationManagerOrNull => s_bag.Value.LocalizationManagerProvider.Value;

		public static ICompilerVersionManager6 CompilerVersionMgr => s_bag.Value.CompilerVersionMgrProvider.Value;

		public static IComPageCustomizer ComPageCustomizerOrNull => s_bag.Value.ComPageCustomizerProvider.Value;

		public static IEnumerable<ICustomizedLibManEditorViewFactorySelector> CustomizedLibManEditorViewFactorySelectors => s_bag.Value.CustomizedLibManEditorViewFactorySelectorsProvider.Value;

		public static ILibraryLoader13 LibraryLoader => s_bag.Value.LibraryLoaderProvider.Value;

		public static IEnumerable<IDeviceParameterHideColumnFactory> ParameterColumnHideFactories => s_bag.Value.ParameterColumnHideFactoriesProvider.Value;

		public static IUndoManager CreateUndoMgr()
		{
			return s_bag.Value.UndoMgrProvider.Create();
		}

		public static IVariablesInputAssistantCategory CreateVariablesInputAssistantCategory()
		{
			return s_bag.Value.VariablesInputAssistantCategoryProvider.Create();
		}

		public static IInputAssistantService2 CreateInputAssistantService()
		{
			return s_bag.Value.InputAssistantServiceProvider.Create();
		}

		public static ISingleLineIECTextEditor5 CreateSingleLineIECTextEditor()
		{
			return s_bag.Value.SingleLineIECTextEditorProvider.Create();
		}

		public static IOnlineHelpService CreateOnlineHelpService(Guid typeGuid)
		{
			return s_bag.Value.AnyOnlineHelpServiceProvider.Create(typeGuid);
		}

		public static IEnumerable<IStatusPageAdditionalControlFactory> CreateStatusPageAdditionalControlFactories()
		{
			return s_bag.Value.StatusPageAdditionalControlFactoriesProvider.Create();
		}

		public static IChangeForceValueService TryCreateChangeForceValueService()
		{
			return s_bag.Value.ChangeForceValueServiceProvider.Create();
		}

		public static IEnumerable<IDeviceParameterPrintFactory> CreateDeviceParameterPrintFactories()
		{
			return s_bag.Value.DeviceParameterPrintFactoriesProvider.Create();
		}

		public static IWatchListView3 CreateWatchListView()
		{
			return s_bag.Value.WatchListViewProvider.Create();
		}

		public static IEnumerable<ILibManEditorViewFactory> CreateLibManEditorViewFactories()
		{
			return s_bag.Value.LibManEditorViewFactoriesProvider.Create();
		}
	}
}
