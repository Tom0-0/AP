using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{a3038281-adaa-4e84-ab41-76b98e75f28d}")]
	public class ConnectorParameterEditorFactory : IConnectorEditorFactory
	{
		public int GetMatch(IDeviceObject deviceObject, IConnector connector)
		{
			return 0;
		}

		public IConnectorEditor Create(IDeviceObject deviceObject, IConnector connector, HideParameterDelegate paramFilter)
		{
			return (IConnectorEditor)(object)new ConnectorParameterEditor(paramFilter);
		}
	}
}
