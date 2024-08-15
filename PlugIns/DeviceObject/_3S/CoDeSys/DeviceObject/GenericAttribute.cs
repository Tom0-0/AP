using System;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class GenericAttribute : ICheckedAttribute, IAttribute
	{
		private string _name;

		private string _description;

		private Version _requiredCompilerVersion;

		private AttributeScope _scope;

		public string Description => _description;

		public Version RequiredCompilerVersion => _requiredCompilerVersion;

		public AttributeScope Scope => _scope;

		public string Name => _name;

		public GenericAttribute(string name, string description, Version requiredCompilerVersion, AttributeScope scope)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			_name = name;
			_description = description;
			_requiredCompilerVersion = requiredCompilerVersion;
			_scope = scope;
		}

		public bool CheckValue(string value, AttributeScope scope, ISignature signature, IVariable variable, out string error)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Invalid comparison between Unknown and I4
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Invalid comparison between Unknown and I4
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(',');
				IToken val = default(IToken);
				foreach (string text in array)
				{
					((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(text, false, false, false, false).GetNext(out val);
					if ((int)val.Type != 13 && (int)val.Type != 7)
					{
						error = string.Format(Strings.AttributeIgnoreInvalid, value);
						return false;
					}
				}
			}
			error = string.Empty;
			return true;
		}
	}
}
