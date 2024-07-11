using System;
using _3S.CoDeSys.LanguageModelUtilities;

namespace _3S.CoDeSys.WatchList
{
	public class EvaluationContext : IEvaluationContext
	{
		private Guid _app = Guid.Empty;

		private Guid _scope = Guid.Empty;

		public Guid ApplicationGuid => _app;

		public Guid ScopeIdentification => _scope;

		public EvaluationContext(Guid guidApplication, Guid guidScope)
		{
			_app = guidApplication;
			_scope = guidScope;
		}
	}
}
