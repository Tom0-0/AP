using System;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal static class OptionHelper
	{
		internal static bool IsSwitchable()
		{
			return UserOptions.DeclarationEditor == 2;
		}

		internal static SwitchableDeclarationEditorMode GetMode(Guid objectGuid)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			switch (UserOptions.DeclarationEditor)
			{
			case 0:
				return (SwitchableDeclarationEditorMode)0;
			case 1:
				return (SwitchableDeclarationEditorMode)1;
			default:
				switch (UserOptions.Default)
				{
				case 0:
					return (SwitchableDeclarationEditorMode)0;
				case 1:
					return (SwitchableDeclarationEditorMode)1;
				case 2:
					return UserProjectOptions.GetMode(objectGuid);
				default:
				{
					int globalRecent = UserOptions.GlobalRecent;
					if ((uint)globalRecent > 1u && globalRecent == 2)
					{
						return (SwitchableDeclarationEditorMode)1;
					}
					return (SwitchableDeclarationEditorMode)0;
				}
				}
			}
		}

		internal static void SetMode(Guid objectGuid, SwitchableDeclarationEditorMode mode)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			switch (UserOptions.Default)
			{
			case 2:
				UserProjectOptions.SetMode(objectGuid, mode);
				break;
			case 3:
				UserOptions.GlobalRecent = (((int)mode != 1) ? 1 : 2);
				break;
			}
		}
	}
}
