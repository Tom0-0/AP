using System;
using System.Collections;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DataElementOnlineVarRefCollection : ICollection, IEnumerable
	{
		private LList<DataElementOnlineVarRef> _htVarRefs = new LList<DataElementOnlineVarRef>();

		public int Count => _htVarRefs.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => this;

		public void Add(DataElementOnlineVarRef ovr)
		{
			_htVarRefs.Add(ovr);
		}

		public void Remove(DataElementOnlineVarRef ovr)
		{
			_htVarRefs.Remove(ovr);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_htVarRefs).CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _htVarRefs.GetEnumerator();
		}
	}
}
