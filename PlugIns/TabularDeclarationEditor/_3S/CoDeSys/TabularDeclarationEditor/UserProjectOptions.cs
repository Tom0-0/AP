using System;
using System.Collections;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal static class UserProjectOptions
	{
		private static readonly string TABULAR_TABLE = "TabularTable";

		private static readonly string SUB_KEY = "{24A961FA-0FF2-409d-9BAD-0085C603946D}";

		private static Hashtable Table
		{
			get
			{
				if (OptionKey.HasValue(TABULAR_TABLE, typeof(Hashtable)))
				{
					return (Hashtable)OptionKey[TABULAR_TABLE];
				}
				return null;
			}
			set
			{
				OptionKey[TABULAR_TABLE] = (object)value;
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)5).CreateSubKey(SUB_KEY);

		internal static SwitchableDeclarationEditorMode GetMode(Guid objectGuid)
		{
			Hashtable hashtable = Table;
			if (hashtable == null)
			{
				hashtable = (Table = new Hashtable());
			}
			if (hashtable.ContainsKey(objectGuid))
			{
				if ((bool)hashtable[objectGuid])
				{
					return (SwitchableDeclarationEditorMode)1;
				}
				return (SwitchableDeclarationEditorMode)0;
			}
			return (SwitchableDeclarationEditorMode)0;
		}

		internal static void SetMode(Guid objectGuid, SwitchableDeclarationEditorMode mode)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			Hashtable hashtable = Table;
			if (hashtable == null)
			{
				hashtable = (Table = new Hashtable());
			}
			hashtable[objectGuid] = (int)mode == 1;
		}
	}
}
