using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{A634937E-E915-467d-9324-96CF67E2F426}")]
	[StorageVersion("3.3.0.0")]
	public class DeviceConstraint : GenericObject2, IDeviceConstraint
	{
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("devIdent")]
		[StorageVersion("3.3.0.0")]
		private DeviceIdentification _devIdent;

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("MaxNumber")]
		[StorageVersion("3.3.0.0")]
		private uint _uiMaxNumber;

		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("CheckRecursive")]
		[StorageVersion("3.3.0.0")]
		private bool _bCheckRecursive;

		public bool CheckRecursive
		{
			get
			{
				return _bCheckRecursive;
			}
			set
			{
				_bCheckRecursive = value;
			}
		}

		public uint MaxNumber
		{
			get
			{
				return _uiMaxNumber;
			}
			set
			{
				_uiMaxNumber = value;
			}
		}

		public DeviceIdentification DevIdent => _devIdent;

		public IDeviceIdentification DeviceIdentification
		{
			get
			{
				return (IDeviceIdentification)(object)_devIdent;
			}
			set
			{
				_devIdent = value as DeviceIdentification;
			}
		}

		public DeviceConstraint()
			: this()
		{
		}

		public DeviceConstraint(DeviceIdentification devIdent, uint uiMaxNumber, bool bCheckRecursive)
			: this()
		{
			_devIdent = devIdent;
			_uiMaxNumber = uiMaxNumber;
			_bCheckRecursive = bCheckRecursive;
		}
	}
}
