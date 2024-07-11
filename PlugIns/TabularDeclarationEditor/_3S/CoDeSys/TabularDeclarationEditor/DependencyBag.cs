using _3S.CoDeSys.AutoDeclare;
using _3S.CoDeSys.Bookmarks;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.ComponentModel;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.IECTextEditor;
using _3S.CoDeSys.InputAssistant;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.TabularDeclarationEditor.Comment;
using _3S.CoDeSys.WatchList;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal class DependencyBag : IDependencyInjectable
	{
		[InjectSingleInstance]
		public ISingleInstanceProvider<IUndoManager> UndoMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true, Optional = true)]
		public ISharedSingleInstanceProvider<IFeatureSettingsManager> FeatureSettingsMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IStructuredTypesInputAssistantCategory> StructuredTypesInputAssistantCategoryProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInputAssistantService> InputAssistantServiceProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IStandardTypesInputAssistantCategory> StandardTypesInputAssistantCategoryProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IObjectManager> ObjectMgrProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IArchiveWriter> BinaryArchiveWriterProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IArchiveReader> BinaryArchiveReaderProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IRefactoringService> RefactoringServiceProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IWatchListView> WatchListViewProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IArrayTypeDialogService> ArrayTypeDialogServiceProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IInitValueDialogService> InitValueDialogServiceProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IBookmarkManager> BookmarkMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IOptionStorage> OptionStorageProvider { get; private set; }

		[InjectSingleInstance]
		public ISingleInstanceProvider<IIECTextEditor> IECTextEditorProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IEngine> EngineProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<ILanguageModelManager> LanguageModelMgrProvider { get; private set; }

		[InjectSingleInstance(Shared = true)]
		public ISharedSingleInstanceProvider<IMessageServiceWithKeys> MessageServiceProvider { get; private set; }

		//[InjectSpecificInstance("{1688D38A-35E3-4560-B57E-4061CF00C64F}")]
		//public ISingleInstanceProvider<IInitValueDialogService2> CommentDialogServiceProvider { get; private set; }

		//[InjectSpecificInstance("{2873410F-1484-48B5-9D66-A062964B6EDF}")]
		//public ISingleInstanceProvider<IMonitoringRangeSetup> MonitoringRangeSetupProvider { get; private set; }

		public DependencyBag()
		{
			ComponentModel.Singleton.InjectDependencies((IDependencyInjectable)(object)this, GetType());
		}

		public void InjectionComplete()
		{
		}
	}
}
