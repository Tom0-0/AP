using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{EC1F8A3A-98C6-404c-883E-CE9E54B42542}")]
	[StorageVersion("3.4.1.0")]
	public class LogicalDeviceList : GenericObject2, IMappedDeviceList, ICollection, IEnumerable
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("InnerList")]
		[StorageVersion("3.4.1.0")]
		private ArrayList _alLogicalDevices = new ArrayList();

		IMappedDevice IMappedDeviceList.this[int nIndex]
		{
			get
			{
				if (nIndex >= 0 && nIndex < this._alLogicalDevices.Count)
				{
					return (IMappedDevice)this._alLogicalDevices[nIndex];
				}
				return null;
			}
		}

		public int Count => _alLogicalDevices.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => _alLogicalDevices.SyncRoot;


		public LogicalDeviceList()
			: base()
		{
		}

		private LogicalDeviceList(LogicalDeviceList original)
			: this()
		{
			_alLogicalDevices = new ArrayList(original._alLogicalDevices.Count);
			foreach (LogicalMappedDevice alLogicalDevice in original._alLogicalDevices)
			{
				_alLogicalDevices.Add(((GenericObject)alLogicalDevice).Clone());
			}
		}

		public override object Clone()
		{
			LogicalDeviceList logicalDeviceList = new LogicalDeviceList(this);
			((GenericObject)logicalDeviceList).AfterClone();
			return logicalDeviceList;
		}

		public void Add(LogicalMappedDevice logicalDevice)
		{
			_alLogicalDevices.Add(logicalDevice);
		}

		public void AddRange(IList list)
		{
			foreach (LogicalMappedDevice item in list)
			{
				Add(item);
			}
		}

		public void CopyTo(Array array, int index)
		{
			_alLogicalDevices.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alLogicalDevices.GetEnumerator();
		}
	}
}
