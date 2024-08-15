using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{E666D3B8-B36C-47B7-9743-2EB12FA30294}")]
	[StorageVersion("3.5.15.0")]
	public class HiddenSlotDeviceObject : SlotDeviceObject, IHiddenObject
	{
		public HiddenSlotDeviceObject()
		{
		}

		internal HiddenSlotDeviceObject(string stInterfaceType, ArrayList alAdditionalInterfaces, bool bAllowEmpty)
			: base(stInterfaceType, alAdditionalInterfaces, bAllowEmpty)
		{
		}

		private HiddenSlotDeviceObject(HiddenSlotDeviceObject hdo)
			: base(hdo)
		{
		}

		public override object Clone()
		{
			HiddenSlotDeviceObject hiddenSlotDeviceObject = new HiddenSlotDeviceObject(this);
			((GenericObject)hiddenSlotDeviceObject).AfterClone();
			return hiddenSlotDeviceObject;
		}
	}
}
