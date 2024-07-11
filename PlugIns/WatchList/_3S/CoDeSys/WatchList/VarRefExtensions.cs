using System;
using System.Linq;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	public static class VarRefExtensions
	{
		public static string GetQualifiedPath(this IVarRef varRef)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Invalid comparison between Unknown and I4
			string applicationName = Common.GetApplicationName(varRef.ApplicationGuid);
			string text = ((IExprement)varRef.WatchExpression).ToString();
			IVarRef obj = ((varRef is IVarRef3) ? varRef : null);
			object obj2;
			if (obj == null)
			{
				obj2 = null;
			}
			else
			{
				IExpression instancePathExpression = ((IVarRef3)obj).InstancePathExpression;
				obj2 = ((instancePathExpression != null) ? ((IExprement)instancePathExpression).ToString() : null);
			}
			string text2 = (string)obj2;
			IPreCompileContext precompileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(varRef.ApplicationGuid);
			GetAccessContext(varRef, out var staticInstancePath, out var objectGuid);
			IIdentifierInfo val = precompileContext.GetIdentifierInfo(objectGuid ?? Guid.Empty, text).First();
			if (string.IsNullOrEmpty(text2))
			{
				text2 = staticInstancePath;
			}
			else if (val.Variable != null && val.Signature != null && (int)val.Signature.POUType == 118)
			{
				text2 = text2 + "." + staticInstancePath;
			}
			if (!IsRooted(precompileContext, text, objectGuid ?? Guid.Empty))
			{
				text = text2 + "." + text;
			}
			return applicationName + "." + text;
		}

		private static bool IsRooted(IPreCompileContext precomcon, string watchExpression, Guid accessObjectGuid)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Invalid comparison between Unknown and I4
			string text = watchExpression.Split('.').First();
			IIdentifierInfo val = precomcon.GetIdentifierInfo(accessObjectGuid, text).First();
			switch (text)
			{
			default:
				if ((val.Variable != null || val.Signature != null || val.Scope != null) && val.Scope == null && (val.Variable != null || val.Signature == null))
				{
					if (val.Variable != null && val.Variable.HasFlag((VarFlag)8192) && val.Signature != null && (int)val.Signature.POUType == 109)
					{
						return !val.Signature.HasAttribute(CompileAttributes.ATTRIBUTE_QUALIFIED_ONLY);
					}
					return false;
				}
				break;
			case "":
			case "__POOL":
			case "__SYSTEM":
				break;
			}
			return true;
		}

		private static void GetAccessContext(IVarRef varRef, out string staticInstancePath, out Guid? objectGuid)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			ICompileContext compileContext = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(varRef.ApplicationGuid);
			objectGuid = null;
			staticInstancePath = null;
			if (varRef is IVarRef2 && compileContext != null)
			{
				int signatureId = ((IVarRef2)varRef).SignatureId;
				ISignature signatureById = compileContext.GetSignatureById(signatureId);
				ICompiledPOU compiledPOUById = compileContext.GetCompiledPOUById(signatureId);
				if (compiledPOUById != null)
				{
					ICompiledPOU obj = ((compiledPOUById is ICompiledPOU4) ? compiledPOUById : null);
					objectGuid = ((obj != null) ? new Guid?(((ICompiledPOU4)obj).ObjectGuid) : null);
				}
				else if (signatureById != null)
				{
					objectGuid = signatureById.ObjectGuid;
				}
				if (signatureById != null)
				{
					staticInstancePath = signatureById.OrgName;
				}
			}
		}
	}
}
