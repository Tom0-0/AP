using System.Drawing;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Messages;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{40f0865b-0ef6-4fd7-a75c-f44c58bd9f5e}")]
	public class DeviceMessageCategory : IMessageCategory
	{
		private static DeviceMessageCategory s_instance;

		public static DeviceMessageCategory Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new DeviceMessageCategory();
				}
				return s_instance;
			}
		}

		public string Text => "Devices";

		public Icon Icon => null;
	}
}
