using System;
using _3S.CoDeSys.AutoDeclare;
using _3S.CoDeSys.Bookmarks;
using _3S.CoDeSys.Core;
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
	internal static class APEnvironment
	{
		private static Lazy<DependencyBag> s_bag = new Lazy<DependencyBag>(() => new DependencyBag());

		public static IFeatureSettingsManager FeatureSettingsMgrOrNull => s_bag.Value.FeatureSettingsMgrProvider.Value;

		public static IObjectManager ObjectMgr => s_bag.Value.ObjectMgrProvider.Value;

		public static IRefactoringService RefactoringService => s_bag.Value.RefactoringServiceProvider.Value;

		public static IBookmarkManager BookmarkMgr => s_bag.Value.BookmarkMgrProvider.Value;

		public static IOptionStorage OptionStorage => s_bag.Value.OptionStorageProvider.Value;

		public static IEngine Engine => s_bag.Value.EngineProvider.Value;

		public static ILanguageModelManager LanguageModelMgr => s_bag.Value.LanguageModelMgrProvider.Value;

		public static IMessageServiceWithKeys MessageService => s_bag.Value.MessageServiceProvider.Value;

		public static IUndoManager CreateUndoMgr()
		{
			return s_bag.Value.UndoMgrProvider.Create();
		}

		public static IStructuredTypesInputAssistantCategory CreateStructuredTypesInputAssistantCategory()
		{
			return s_bag.Value.StructuredTypesInputAssistantCategoryProvider.Create();
		}

		public static IInputAssistantService CreateInputAssistantService()
		{
			return s_bag.Value.InputAssistantServiceProvider.Create();
		}

		public static IStandardTypesInputAssistantCategory CreateStandardTypesInputAssistantCategory()
		{
			return s_bag.Value.StandardTypesInputAssistantCategoryProvider.Create();
		}

		public static IArchiveWriter CreateBinaryArchiveWriter()
		{
			return s_bag.Value.BinaryArchiveWriterProvider.Create();
		}

		public static IArchiveReader CreateBinaryArchiveReader()
		{
			return s_bag.Value.BinaryArchiveReaderProvider.Create();
		}

		public static IWatchListView CreateWatchListView()
		{
			return s_bag.Value.WatchListViewProvider.Create();
		}

		public static IArrayTypeDialogService CreateArrayTypeDialogService()
		{
			return s_bag.Value.ArrayTypeDialogServiceProvider.Create();
		}

		public static IInitValueDialogService CreateInitValueDialogService()
		{
			return s_bag.Value.InitValueDialogServiceProvider.Create();
		}

		public static IIECTextEditor CreateIECTextEditor()
		{
			return s_bag.Value.IECTextEditorProvider.Create();
		}

        //public static IInitValueDialogService2 CreateCommentDialogService()
        //{
        //	return s_bag.Value.CommentDialogServiceProvider.Create();
        //}

        //internal static IMonitoringRangeSetup CreateMonitoringRangeSetup()
        //{
        //    return APEnvironment.s_bag.Value.MonitoringRangeSetupProvider.Create();
        //}
    }
}
