using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	[TypeGuid("{4a4f68bb-7214-449d-b4c0-ae5cfed68f3f}")]
	public class DeviceParameterEditorFactory : IDeviceEditorFactory
	{
		public int GetMatch(IDeviceObject deviceObject)
		{
			if (deviceObject != null && deviceObject.DeviceParameterSet != null)
			{
				return 0;
			}
			return -1;
		}

		public IDeviceEditor Create(IDeviceObject deviceObject, HideParameterDelegate paramFilter)
		{
			return (IDeviceEditor)(object)new DeviceParameterEditor(paramFilter);
		}
	}
}
