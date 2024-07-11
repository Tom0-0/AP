using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	internal static class VisibilityUtil
	{
		internal static bool IsHiddenSignature(ISignature signature, GUIHidingFlags flags)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).IsHiddenSignature(signature, flags);
		}

		internal static bool IsHiddenVariable(ISignature signature, IVariable var, GUIHidingFlags flags)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return APEnvironment.LanguageModelMgr.IsHiddenVariable((ISignature6)(object)((signature is ISignature6) ? signature : null), var, flags);
		}
	}
}
