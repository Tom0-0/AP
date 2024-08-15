using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class VariableCrossRef
	{
		private VariableDeclaration _varDecl;

		private ICrossReference _crossRef;

		private IVariable _var;

		private int _signId;

		public VariableDeclaration VariableDeclaration => _varDecl;

		public ICrossReference CrossReference => _crossRef;

		public IVariable Variable => _var;

		public int SignatureId => _signId;

		public VariableCrossRef(VariableDeclaration varDecl, ICrossReference crossRef, IVariable var, int signId)
		{
			_varDecl = varDecl;
			_crossRef = crossRef;
			_var = var;
			_signId = signId;
		}
	}
}
