using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	internal class HashtableReader
	{
		private Hashtable _hashtable;

		public Hashtable UnreadValues => _hashtable;

		internal HashtableReader(Hashtable hashtable)
		{
			_hashtable = hashtable;
		}

		public object Read(object key)
		{
			object result = _hashtable[key];
			_hashtable.Remove(key);
			return result;
		}

		public bool Contains(object key)
		{
			return _hashtable.ContainsKey(key);
		}
	}
}
