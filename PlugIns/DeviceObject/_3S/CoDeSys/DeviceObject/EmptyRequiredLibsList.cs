using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	internal class EmptyRequiredLibsList : IRequiredLibsList, ICollection, IEnumerable
	{
		private IRequiredLib[] _emptyList = (IRequiredLib[])(object)new IRequiredLib[0];

		public IRequiredLib this[int nIndex]
		{
			get
			{
				throw new IndexOutOfRangeException();
			}
		}

		public bool IsSynchronized => false;

		public int Count => 0;

		public object SyncRoot => this;

		public void CopyTo(Array array, int index)
		{
		}

		public IEnumerator GetEnumerator()
		{
			return _emptyList.GetEnumerator();
		}
	}
}
