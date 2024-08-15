using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{959f677a-b4e4-4930-8d88-b3b6b116d4d6}")]
	[StorageVersion("3.3.0.0")]
	public class ModuleIdentification : DeviceIdentification, IModuleIdentification, IDeviceIdentification
	{
		[DefaultSerialization("ModuleId")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected string _stModuleId;

		public string ModuleId
		{
			get
			{
				return _stModuleId;
			}
			set
			{
				_stModuleId = value;
			}
		}

		internal ModuleIdentification(IModuleIdentification moduleId)
			: base((IDeviceIdentification)(object)moduleId)
		{
			_stModuleId = moduleId.ModuleId;
		}

		public ModuleIdentification()
		{
		}

		public override object Clone()
		{
			ModuleIdentification moduleIdentification = new ModuleIdentification((IModuleIdentification)(object)this);
			((GenericObject)moduleIdentification).AfterClone();
			return moduleIdentification;
		}

		public override bool Equals(object obj, bool bIgnoreVersion)
		{
			return Equals(obj, bIgnoreVersion, bIgnoreModuleId: false);
		}

		public override bool Equals(object obj, bool bIgnoreVersion, bool bIgnoreModuleId)
		{
			IModuleIdentification val = (IModuleIdentification)((obj is IModuleIdentification) ? obj : null);
			if (val == null)
			{
				return false;
			}
			if (!bIgnoreModuleId && string.Compare(val.ModuleId, _stModuleId, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return false;
			}
			return base.Equals(obj, bIgnoreVersion, bIgnoreModuleId);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj, bIgnoreVersion: false, bIgnoreModuleId: false);
		}

		public override int GetHashCode()
		{
			if (_stModuleId != null)
			{
				return base.GetHashCode() ^ _stModuleId.GetHashCode();
			}
			return base.GetHashCode();
		}
	}
}
