using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal static class UserOptions
	{
		private static readonly string DECLARATION_EDITOR = "DeclarationEditor";

		private static readonly string DEFAULT = "Default";

		private static readonly string GLOBAL_RECENT = "GlobalRecent";

		private static readonly string RENAME_WITH_REFACTORING = "RenameWithRefactoring";

		private static readonly string SUB_KEY = "{4F069A5B-FC8E-4c6e-B205-91817EF9E25D}";

		internal const int DECLARATION_EDITOR_TEXTUAL_ONLY = 0;

		internal const int DECLARATION_EDITOR_TABULAR_ONLY = 1;

		internal const int DECLARATION_EDITOR_SWITCHABLE = 2;

		internal const int DEFAULT_TEXTUAL = 0;

		internal const int DEFAULT_TABULAR = 1;

		internal const int DEFAULT_REMEMBER_PER_OBJECT = 2;

		internal const int DEFAULT_REMEMBER_GLOBALLY = 3;

		internal const int GLOBAL_RECENT_NOT_SET = 0;

		internal const int GLOBAL_RECENT_TEXTUAL = 1;

		internal const int GLOBAL_RECENT_TABULAR = 2;

		internal static int DeclarationEditor
		{
			get
			{
				if (OptionKey.HasValue(DECLARATION_EDITOR, typeof(int)))
				{
					return (int)OptionKey[DECLARATION_EDITOR];
				}
				return 2;
			}
			set
			{
				OptionKey[DECLARATION_EDITOR] = (object)value;
			}
		}

		internal static int Default
		{
			get
			{
				if (OptionKey.HasValue(DEFAULT, typeof(int)))
				{
					return (int)OptionKey[DEFAULT];
				}
				return 3;
			}
			set
			{
				OptionKey[DEFAULT]= (object)value;
			}
		}

		internal static int GlobalRecent
		{
			get
			{
				if (OptionKey.HasValue(GLOBAL_RECENT, typeof(int)))
				{
					return (int)OptionKey[GLOBAL_RECENT];
				}
				return 0;
			}
			set
			{
				OptionKey[GLOBAL_RECENT] = (object)value;
			}
		}

		internal static bool RenameWithRefactoring
		{
			get
			{
				if (OptionKey.HasValue(RENAME_WITH_REFACTORING, typeof(bool)))
				{
					return (bool)OptionKey[RENAME_WITH_REFACTORING];
				}
				return true;
			}
			set
			{
				OptionKey[RENAME_WITH_REFACTORING] = (object)value;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY);
	}
}
