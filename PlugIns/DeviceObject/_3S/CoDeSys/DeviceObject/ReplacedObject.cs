using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ReplacedObject
	{
		private Guid _guidObject;

		private DeviceObject _oldObject;

		private DeviceObject _newObject;

		private SlotDeviceObject _oldSlotObject;

		private SlotDeviceObject _newSlotObject;

		internal Guid ObjectGuid => _guidObject;

		internal DeviceObject OldObject => _oldObject;

		internal DeviceObject NewObject => _newObject;

		internal SlotDeviceObject OldSlotObject => _oldSlotObject;

		internal SlotDeviceObject NewSlotObject => _newSlotObject;

		internal ReplacedObject(Guid guid, DeviceObject oldObject, DeviceObject newObject)
		{
			_guidObject = guid;
			_oldObject = (DeviceObject)((GenericObject)oldObject).Clone();
			_newObject = (DeviceObject)((GenericObject)newObject).Clone();
		}

		internal ReplacedObject(Guid guid, SlotDeviceObject oldObject, SlotDeviceObject newObject)
		{
			_guidObject = guid;
			_oldSlotObject = (SlotDeviceObject)((GenericObject)oldObject).Clone();
			_newSlotObject = (SlotDeviceObject)((GenericObject)newObject).Clone();
		}
	}
}
