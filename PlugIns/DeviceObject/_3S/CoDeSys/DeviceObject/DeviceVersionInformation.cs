using System;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceVersionInformation : IComparable
	{
		internal Version DeviceVersion;

		internal bool NewConfigurationFormat;

		public int CompareTo(object obj)
		{
			return DeviceVersion.CompareTo((obj as DeviceVersionInformation).DeviceVersion);
		}
	}
}
