using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.WatchList
{
	public class VariableVisitor : IVariableVisitor
	{
		private string _stSigntureIDtoMatch = string.Empty;

		private LList<string> _matches = new LList<string>();

		public LList<string> Matches
		{
			get
			{
				return _matches;
			}
			set
			{
				_matches = value;
			}
		}

		public VariableVisitor(string stSigntureIDtoMatch)
		{
			_stSigntureIDtoMatch = stSigntureIDtoMatch;
		}

		public void visit(IVariableExpression variable, AccessFlag access, IPrecompileScope5 scope)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			IVariable val = default(IVariable);
			ISignature val2 = default(ISignature);
			IPrecompileScope val3 = default(IPrecompileScope);
			if (((IPrecompileScope)scope).FindDeclaration(variable.Name, out val, out val2, out val3) && val2 != null && val != null && val.Type is IUserdefType2 && ((IExprement)((IUserdefType2)val.Type).NameExpression).ToString().ToLowerInvariant() == _stSigntureIDtoMatch.ToLowerInvariant())
			{
				_matches.Add(val2.OrgName + "." + variable.Name);
			}
		}
	}
}
