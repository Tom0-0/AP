using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{05ee0b98-6639-4276-bc32-b75578b819ef}")]
	[StorageVersion("3.3.0.0")]
	public class DeviceIdentification : GenericObject2, IDeviceIdentification
	{
		[DefaultSerialization("Type")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected int _nType;

		[DefaultSerialization("Id")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected string _stId;

		[DefaultSerialization("Version")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected string _stVersion;

		[DefaultSerialization("BaseName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected string _stBaseName;

		public int Type
		{
			get
			{
				return _nType;
			}
			set
			{
				_nType = value;
			}
		}

		public string Id
		{
			get
			{
				return _stId;
			}
			set
			{
				_stId = value;
			}
		}

		public string Version
		{
			get
			{
				return _stVersion;
			}
			set
			{
				_stVersion = value;
			}
		}

		public string BaseName
		{
			get
			{
				if (_stBaseName == null)
				{
					return "$(DeviceName)";
				}
				return _stBaseName;
			}
			set
			{
				_stBaseName = value;
			}
		}

		public DeviceIdentification()
			: base()
		{
		}

		internal DeviceIdentification(int nType, string stId, string stVersion, string stBaseName)
			: this()
		{
			_nType = nType;
			_stId = stId;
			_stVersion = stVersion;
			_stBaseName = stBaseName;
		}

		internal DeviceIdentification(IDeviceIdentification di)
			: this()
		{
			_nType = di.Type;
			_stId = di.Id;
			_stVersion = di.Version;
			if (di is DeviceIdentification)
			{
				_stBaseName = ((DeviceIdentification)(object)di)._stBaseName;
			}
		}

		public override object Clone()
		{
			DeviceIdentification deviceIdentification = new DeviceIdentification((IDeviceIdentification)(object)this);
			((GenericObject)deviceIdentification).AfterClone();
			return deviceIdentification;
		}

		public virtual bool Equals(object obj, bool bIgnoreVersion)
		{
			return Equals(obj, bIgnoreVersion, bIgnoreModuleId: false);
		}

		public virtual bool Equals(object obj, bool bIgnoreVersion, bool bIgnoreModuleId)
		{
			IDeviceIdentification val = (IDeviceIdentification)((obj is IDeviceIdentification) ? obj : null);
			if (obj == null)
			{
				throw new ArgumentException("'obj' is null or not of type IDeviceIdentification");
			}
			if (val.Type != _nType)
			{
				return false;
			}
			if (string.Compare(val.Id, _stId, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return false;
			}
			if (!bIgnoreVersion && !val.Version.Equals(_stVersion))
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj, bIgnoreVersion: false, bIgnoreModuleId: false);
		}

		public override int GetHashCode()
		{
			if (_stId == null)
			{
				return 0;
			}
			return _nType ^ _stId.GetHashCode();
		}
	}
}
