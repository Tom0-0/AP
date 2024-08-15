using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{1da2f14a-1c63-4268-9171-3b6869b8ccd6}")]
	[StorageVersion("3.3.0.0")]
	public class AdapterList : GenericObject2, IAdapterList, ICollection, IEnumerable
	{
		[DefaultSerialization("Adapters")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private ArrayList _alAdapters = new ArrayList();

		public IAdapter this[int nIndex] => (IAdapter)_alAdapters[nIndex];

		public int Count => _alAdapters.Count;

		public bool IsSynchronized => _alAdapters.IsSynchronized;

		public object SyncRoot => _alAdapters.SyncRoot;

		public AdapterList()
			: this()
		{
		}

		private AdapterList(AdapterList original)
			: this()
		{
			_alAdapters = new ArrayList(original._alAdapters.Count);
			foreach (IAdapterBase alAdapter in original._alAdapters)
			{
				_alAdapters.Add(alAdapter.Clone());
			}
		}

		public override object Clone()
		{
			AdapterList adapterList = new AdapterList(this);
			((GenericObject)adapterList).AfterClone();
			return adapterList;
		}

		public void Add(IAdapter adapter)
		{
			_alAdapters.Add(adapter);
		}

		public void CopyTo(Array array, int index)
		{
			_alAdapters.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alAdapters.GetEnumerator();
		}
	}
}
