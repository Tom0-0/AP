using System;
using System.Collections;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class LogicalIoFilter : IDeviceCatalogueFilter
	{
		private string[] _filter;

		public LogicalIoFilter(string[] filter)
		{
			_filter = filter;
		}

		public override int GetHashCode()
		{
			int num = 305419896;
			if (_filter != null)
			{
				string[] filter = _filter;
				foreach (string text in filter)
				{
					num ^= text.GetHashCode();
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			LogicalIoFilter logicalIoFilter = (LogicalIoFilter)obj;
			return ArrayHelper.Equals((Array)_filter, (Array)logicalIoFilter._filter);
		}

		public bool Match(IDeviceDescription device)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (_filter == null)
			{
				return device.AllowTopLevel;
			}
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole != 0)
				{
					continue;
				}
				string[] filter = _filter;
				foreach (string value in filter)
				{
					if (val.Interface.Equals(value))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
