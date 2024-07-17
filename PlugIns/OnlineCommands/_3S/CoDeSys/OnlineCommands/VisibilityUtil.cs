using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.OnlineCommands
{
    internal static class VisibilityUtil
    {
        internal static bool IsHiddenSignature(ISignature signature)
        {
            return APEnvironment.LanguageModelMgr.IsHiddenSignature(signature as ISignature6, GUIHidingFlags.AllEvaluation);
        }

        internal static bool IsHiddenVariable(ISignature signature, IVariable var, GUIHidingFlags flags)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            return ((ILanguageModelManager22)APEnvironment.LanguageModelMgr).IsHiddenVariable((ISignature6)(object)((signature is ISignature6) ? signature : null), var, flags);
        }
    }
}
