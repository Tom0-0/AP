using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceBuffer : IDeviceManagerBuffer
	{
		private Predicate<IDeviceManagerInfo> match;

		private LDictionary<Guid, int> devices = new LDictionary<Guid, int>();

		public ICollection<Guid> DeviceGuids => (ICollection<Guid>)devices.Keys;

		internal DeviceBuffer(Predicate<IDeviceManagerInfo> match)
		{
			this.match = match;
		}

		internal void NofifyUpdate(DeviceManagerInfo device, int projectHandle)
		{
			Guid objectGuid = device.ObjectGuid;
			if (match((IDeviceManagerInfo)(object)device))
			{
				if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle == projectHandle)
				{
					devices[objectGuid]= projectHandle;
				}
			}
			else if (devices.ContainsKey(objectGuid))
			{
				devices.Remove(objectGuid);
			}
		}

		internal void NofifyProjectSwitch(int projectHandle)
		{
			LDictionary<Guid, int> ldictionary = new LDictionary<Guid, int>();
			foreach (KeyValuePair<Guid, int> keyValuePair in this.devices)
			{
				if (keyValuePair.Value == projectHandle)
				{
					ldictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			this.devices = ldictionary;
		}

		internal void NofifyRemoved(Guid guid)
		{
			if (devices.ContainsKey(guid))
			{
				devices.Remove(guid);
			}
		}

		internal void NofifyClear()
		{
			devices.Clear();
		}
	}
}
