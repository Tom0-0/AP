using System;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class BusTaskItem : IComparable
	{
		private Guid _ObjectGuid;

		private string _stName;

		internal Guid ObjectGuid => _ObjectGuid;

		internal BusTaskItem(Guid ObjectGuid, string stName)
		{
			_stName = stName;
			_ObjectGuid = ObjectGuid;
		}

		public override string ToString()
		{
			return _stName;
		}

		public int CompareTo(object obj)
		{
			return _stName.CompareTo(((BusTaskItem)obj)._stName);
		}
	}
}
