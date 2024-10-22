using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{03B12D85-7991-4556-8C0E-C0B3063EB6C4}")]
	[StorageVersion("3.5.4.0")]
	internal class StorableScanInformation : GenericObject2, IScanInformation
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationDeviceName")]
		[StorageVersion("3.5.4.0")]
		private string _stDeviceName = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationIPAddressAndPort")]
		[StorageVersion("3.5.4.0")]
		private string _stIPAddressAndPort = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationTargetID")]
		[StorageVersion("3.5.4.0")]
		private string _stTargetID = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationTargetName")]
		[StorageVersion("3.5.4.0")]
		private string _stTargetName = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationTargetType")]
		[StorageVersion("3.5.4.0")]
		private string _stTargetType = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationTargetVendor")]
		[StorageVersion("3.5.4.0")]
		private string _stTargetVendor = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ScanInformationTargetVersion")]
		[StorageVersion("3.5.4.0")]
		private string _stTargetVersion = string.Empty;

		public string DeviceName
		{
			get
			{
				return _stDeviceName;
			}
			set
			{
				_stDeviceName = value;
			}
		}

		public string IPAddressAndPort
		{
			get
			{
				return _stIPAddressAndPort;
			}
			set
			{
				_stIPAddressAndPort = value;
			}
		}

		public string TargetID
		{
			get
			{
				return _stTargetID;
			}
			set
			{
				_stTargetID = value;
			}
		}

		public string TargetName
		{
			get
			{
				return _stTargetName;
			}
			set
			{
				_stTargetName = value;
			}
		}

		public string TargetType
		{
			get
			{
				return _stTargetType;
			}
			set
			{
				_stTargetType = value;
			}
		}

		public string TargetVendor
		{
			get
			{
				return _stTargetVendor;
			}
			set
			{
				_stTargetVendor = value;
			}
		}

		public string TargetVersion
		{
			get
			{
				return _stTargetVersion;
			}
			set
			{
				_stTargetVersion = value;
			}
		}

		public StorableScanInformation()
		{
		}
	}
}
