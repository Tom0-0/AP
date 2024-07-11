using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.ProjectLocalization;
using _3S.CoDeSys.Refactoring;

namespace _3S.CoDeSys.WatchList
{
	internal class DependencyBag : IDependencyInjectable
	{
		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IEngine3> EngineProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOptionStorage> OptionStorageProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOnlineManager6> OnlineMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILanguageModelManager22> LanguageModelMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IObjectManager> ObjectMgrProvider { get; private set; }

		[InjectFrameForm]
		public IWinFormWrapperProxy FrameFormProxy { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ITargetSettingsManager> TargetSettingsMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMessageServiceWithKeys> MessageServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IInputAssistantArgumentsFormatter> STActionArgumentsFormatterProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IInputAssistantArgumentsFormatter> STFunctionArgumentsFormatterProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IInputAssistantArgumentsFormatter> STFunctionBlockArgumentsFormatterProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IInputAssistantArgumentsFormatter> STMethodArgumentsFormatterProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IInputAssistantArgumentsFormatter> STProgramArgumentsFormatterProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ITargetSettingsProvider> TargetSettingsProviderProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IRefactoringService> RefactoringServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILanguageModelUtilities2> LanguageModelUtilitiesProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IBPManager4> BPMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ISingleLineIECTextEditor> SingleLineIECTextEditorProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInputAssistantCategory> WatchVariablesInputAssistantCategoryProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInputAssistantService> InputAssistantServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMonitoringUtilities> MonitoringUtilitiesProvider { get; private set; }

		[InjectMultipleInstances(Shared = true, Optional = true)]
		public ISharedMultipleInstancesProvider<IHiddenObjectAdorner> HiddenObjectAdornersProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IToolTipService> ToolTipServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<ILocalizationManager> LocalizationManagerProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILMServiceProvider> LMServiceProviderProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ICompilerVersionManager6> CompilerVersionMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInterfaceOnlineVarRef> InterfaceOnlineVarRefProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IPointerOnlineVarRef> PointerOnlineVarRefProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInterfaceMonitoringHelper> InterfaceMonitoringHelperProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IPointerInstanceWatchVarDescription> PointerInstanceWatchVarDescriptionProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IDynamicInterfaceInstanceWatchVarDescription> DynamicInterfaceInstanceWatchVarDescriptionProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IAddressWatchVarDescription> AddressWatchVarDescriptionProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<ITemporaryWatchVarOnlineVarRef> TemporaryWatchVarOnlineVarRefProvider { get; private set; }

		public DependencyBag()
		{
			ComponentModel.Singleton.InjectDependencies((IDependencyInjectable)(object)this, GetType());
		}

		public void InjectionComplete()
		{
		}
	}
}
