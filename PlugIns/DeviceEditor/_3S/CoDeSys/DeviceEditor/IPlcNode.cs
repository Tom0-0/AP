using System;
using System.Collections.Generic;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal interface IPlcNode : IDeviceNode
	{
		IList<Guid> Applications { get; }

		Guid OnlineApplication { get; }

		bool IsConfigModeOnlineApplication { get; }

		bool UnionRootEditable { get; }

		bool MotorolaBitfields { get; }

		bool BitfieldMappable { get; }

		bool BaseTypeMappable { get; }

		bool AlwaysMapToNew { get; }

		bool ManualAddress { get; }

		bool MultipleMappableAllowed { get; }

		bool DefaultColumnAvailable { get; }

		IDeviceObject PlcDevice { get; }
	}
}
