using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DirectVarCrossRefList
	{
		private LList<DirectVarCrossRef> _alCrossRefs = new LList<DirectVarCrossRef>();

		private LDictionary<IDirectVariable, DirectVarCrossRef> _ldic = new LDictionary<IDirectVariable, DirectVarCrossRef>();

		public int Count => _alCrossRefs.Count;

		public void Add(DirectVarCrossRef cref)
		{
			_alCrossRefs.Add(cref);
			_ldic[cref.DirectVariable]= cref;
		}

		public void Remove(DirectVarCrossRef cref)
		{
			_alCrossRefs.Remove(cref);
			_ldic.Remove(cref.DirectVariable);
		}

		public LList<DirectVarCrossRef> GetCrossRefs()
		{
			return _alCrossRefs;
		}

		public bool Contains(IDirectVariable var)
		{
			return _ldic.ContainsKey(var);
		}

		public bool TryGetValue(IDirectVariable var, out DirectVarCrossRef cref)
		{
			return _ldic.TryGetValue(var, out cref);
		}
	}
}
