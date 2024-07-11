using System;
using System.Collections.Generic;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;

namespace _3S.CoDeSys.WatchList
{
	internal static class APEnvironment
	{
		private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

		public static IEngine3 Engine => s_bag.Value.EngineProvider.Value;

		public static IOptionStorage OptionStorage => s_bag.Value.OptionStorageProvider.Value;

		public static IOnlineManager6 OnlineMgr => s_bag.Value.OnlineMgrProvider.Value;

		public static ILanguageModelManager22 LanguageModelMgr => s_bag.Value.LanguageModelMgrProvider.Value;

		public static IObjectManager ObjectMgr => s_bag.Value.ObjectMgrProvider.Value;

		public static IWinFormWrapper FrameForm => s_bag.Value.FrameFormProxy.FrameForm;

		public static ITargetSettingsManager TargetSettingsMgr => s_bag.Value.TargetSettingsMgrProvider.Value;

		public static IMessageServiceWithKeys MessageService => s_bag.Value.MessageServiceProvider.Value;

		public static IInputAssistantArgumentsFormatter STActionArgumentsFormatter => s_bag.Value.STActionArgumentsFormatterProvider.Value;

		public static IInputAssistantArgumentsFormatter STFunctionArgumentsFormatter => s_bag.Value.STFunctionArgumentsFormatterProvider.Value;

		public static IInputAssistantArgumentsFormatter STFunctionBlockArgumentsFormatter => s_bag.Value.STFunctionBlockArgumentsFormatterProvider.Value;

		public static IInputAssistantArgumentsFormatter STMethodArgumentsFormatter => s_bag.Value.STMethodArgumentsFormatterProvider.Value;

		public static IInputAssistantArgumentsFormatter STProgramArgumentsFormatter => s_bag.Value.STProgramArgumentsFormatterProvider.Value;

		public static ITargetSettingsProvider TargetSettingsProvider => s_bag.Value.TargetSettingsProviderProvider.Value;

		public static IRefactoringService RefactoringServiceOrNull => s_bag.Value.RefactoringServiceProvider.Value;

		public static ILanguageModelUtilities2 LanguageModelUtilities => s_bag.Value.LanguageModelUtilitiesProvider.Value;

		public static IBPManager4 BPMgr => s_bag.Value.BPMgrProvider.Value;

		public static IMonitoringUtilities MonitoringUtilities => s_bag.Value.MonitoringUtilitiesProvider.Value;

		public static IEnumerable<IHiddenObjectAdorner> HiddenObjectAdorners => s_bag.Value.HiddenObjectAdornersProvider.Value;

		public static ILocalizationManager LocalizationManagerOrNull => s_bag.Value.LocalizationManagerProvider.Value;

		public static ILMServiceProvider LMServiceProvider => s_bag.Value.LMServiceProviderProvider.Value;

		public static ICompilerVersionManager6 CompilerVersionMgr => s_bag.Value.CompilerVersionMgrProvider.Value;

		public static ISingleLineIECTextEditor CreateSingleLineIECTextEditor()
		{
			return s_bag.Value.SingleLineIECTextEditorProvider.Create();
		}

		public static IInputAssistantCategory CreateWatchVariablesInputAssistantCategory()
		{
			return s_bag.Value.WatchVariablesInputAssistantCategoryProvider.Create();
		}

		public static IInputAssistantService CreateInputAssistantService()
		{
			return s_bag.Value.InputAssistantServiceProvider.Create();
		}

		public static IToolTipService CreateToolTipService()
		{
			return s_bag.Value.ToolTipServiceProvider.Create();
		}

		public static IInterfaceOnlineVarRef CreateInterfaceOnlineVarRef()
		{
			return s_bag.Value.InterfaceOnlineVarRefProvider.Create();
		}

		public static IPointerOnlineVarRef CreatePointerOnlineVarRef()
		{
			return s_bag.Value.PointerOnlineVarRefProvider.Create();
		}

		public static IInterfaceMonitoringHelper CreateInterfaceMonitoringHelper()
		{
			return s_bag.Value.InterfaceMonitoringHelperProvider.Create();
		}

		public static IAddressWatchVarDescription CreateAddressWatchVarDescription()
		{
			return s_bag.Value.AddressWatchVarDescriptionProvider.Create();
		}

		public static IDynamicInterfaceInstanceWatchVarDescription CreateDynamicInterfaceInstanceWatchVarDescription()
		{
			return s_bag.Value.DynamicInterfaceInstanceWatchVarDescriptionProvider.Create();
		}

		public static IPointerInstanceWatchVarDescription CreatePointerInstanceWatchVarDescription()
		{
			return s_bag.Value.PointerInstanceWatchVarDescriptionProvider.Create();
		}

		public static ITemporaryWatchVarOnlineVarRef CreateTemporaryWatchVarOnlineVarRef()
		{
			return s_bag.Value.TemporaryWatchVarOnlineVarRefProvider.Create();
		}
	}
}
