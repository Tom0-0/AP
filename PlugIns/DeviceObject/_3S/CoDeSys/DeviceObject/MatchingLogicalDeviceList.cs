using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7D152438-0516-4bcd-BBD9-7ECC295A171C}")]
	[StorageVersion("3.4.1.0")]
	public class MatchingLogicalDeviceList : GenericObject2
	{
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("InnerList")]
		[StorageVersion("3.4.1.0")]
		private ArrayList _alMatchingLogicalDevices = new ArrayList();

		internal MatchingLogicalDevice this[int nIndex] => (MatchingLogicalDevice)_alMatchingLogicalDevices[nIndex];

		public int Count => _alMatchingLogicalDevices.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => _alMatchingLogicalDevices.SyncRoot;

		public MatchingLogicalDeviceList()
			: this()
		{
		}

		private MatchingLogicalDeviceList(MatchingLogicalDeviceList original)
			: this()
		{
			_alMatchingLogicalDevices = new ArrayList(original._alMatchingLogicalDevices.Count);
			foreach (MatchingLogicalDevice alMatchingLogicalDevice in original._alMatchingLogicalDevices)
			{
				_alMatchingLogicalDevices.Add(((GenericObject)alMatchingLogicalDevice).Clone());
			}
		}

		public override object Clone()
		{
			MatchingLogicalDeviceList matchingLogicalDeviceList = new MatchingLogicalDeviceList(this);
			((GenericObject)matchingLogicalDeviceList).AfterClone();
			return matchingLogicalDeviceList;
		}

		public void Add(MatchingLogicalDevice logicalDevice)
		{
			_alMatchingLogicalDevices.Add(logicalDevice);
		}

		public void AddRange(IList list)
		{
			foreach (MatchingLogicalDevice item in list)
			{
				Add(item);
			}
		}

		public void CopyTo(Array array, int index)
		{
			_alMatchingLogicalDevices.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alMatchingLogicalDevices.GetEnumerator();
		}
	}
}
