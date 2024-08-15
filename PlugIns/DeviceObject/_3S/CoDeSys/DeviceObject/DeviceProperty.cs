using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{CDADFC2B-8598-4621-AD56-5B1DF7DB910F}")]
	[StorageVersion("3.3.2.0")]
	public class DeviceProperty : GenericObject2, IDeviceProperty, IObjectProperty, IGenericObject, IArchivable, ICloneable, IComparable
	{
		public static readonly Guid My_Guid = new Guid("{CDADFC2B-8598-4621-AD56-5B1DF7DB910F}");

		[DefaultSerialization("DeviceIdentification")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		private IDeviceIdentification _devId;

		public static Guid Guid => My_Guid;

		public IDeviceIdentification DeviceIdentification => _devId;

		public DeviceProperty()
			: this()
		{
		}

		public DeviceProperty(IDeviceIdentification devId)
			: this()
		{
			if (devId is IModuleIdentification)
			{
				_devId = (IDeviceIdentification)(object)new ModuleIdentification((IModuleIdentification)(object)((devId is IModuleIdentification) ? devId : null));
			}
			else
			{
				_devId = (IDeviceIdentification)(object)new DeviceIdentification(devId);
			}
		}

		internal DeviceProperty(DeviceProperty original)
			: this()
		{
			_devId = original._devId;
		}

		public override object Clone()
		{
			return new DeviceProperty(this);
		}
	}
}
