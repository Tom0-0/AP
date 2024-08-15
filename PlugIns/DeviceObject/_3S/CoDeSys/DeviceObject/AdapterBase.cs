using System.Collections;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class AdapterBase
	{
		internal static void CheckAndChangeConnectorInChild(IDeviceObject device, IConnector7 conUpdate)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			if (device == null || conUpdate == null || (int)((IConnector)conUpdate).ConnectorRole != 0)
			{
				return;
			}
			Connector activeChildConnector = DeviceManager.GetActiveChildConnector(device);
			if (activeChildConnector == null || DeviceManager.CheckMatchInterface(conUpdate, (IConnector7)(object)activeChildConnector))
			{
				return;
			}
			foreach (IConnector7 item in (IEnumerable)device.Connectors)
			{
				IConnector7 val = item;
				if ((int)((IConnector)val).ConnectorRole == 1 && DeviceManager.CheckMatchInterface(conUpdate, val))
				{
					HostpathUpdate.ChangeActiveChildConnector(((IObject)device).MetaObject.ProjectHandle, ((IObject)device).MetaObject.ObjectGuid, ((IConnector)val).ConnectorId);
					break;
				}
			}
		}
	}
}
