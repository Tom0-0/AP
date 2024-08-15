using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class PlugSlotAction : AbstractSlotAction, IUndoableAction
	{
		public string Description => Strings.PlugSlotAction;

		internal PlugSlotAction(int nProjectHandle, Guid guidSlot, DeviceObject device, string stDeviceName)
		{
			Init(nProjectHandle, guidSlot, device, stDeviceName);
		}

		public object Redo()
		{
			return Plug();
		}

		public object Undo()
		{
			return Unplug();
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
