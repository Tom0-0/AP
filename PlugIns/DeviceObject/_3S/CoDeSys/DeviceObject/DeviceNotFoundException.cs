using System;
using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
	public class DeviceNotFoundException : Exception
	{
		public string stErrorContext = string.Empty;

		public IDeviceIdentification DeviceId;

		public override string Message
		{
			get
			{
				string text = "\n" + string.Format(Strings.DeviceIdentificationString, DeviceId.Type, DeviceId.Id, DeviceId.Version);
				if (stErrorContext.Equals(string.Empty))
				{
					return Strings.DeviceNotFound + text;
				}
				return stErrorContext + text;
			}
		}

		public DeviceNotFoundException(IDeviceIdentification DeviceId)
		{
			this.DeviceId = DeviceId;
		}
	}
}
