using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class UnplugSlotAction : AbstractSlotAction, IUndoableAction
	{
		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "UnplugSlotAction");

		internal UnplugSlotAction(int nProjectHandle, Guid guidSlot, DeviceObject device)
		{
			Init(nProjectHandle, guidSlot, device, string.Empty);
		}

		public object Undo()
		{
			return Plug();
		}

		public object Redo()
		{
			return Unplug();
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
