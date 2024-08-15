#define DEBUG
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{21493CF5-00AC-4f1c-BE98-9483051AB70D}")]
	[StorageVersion("3.4.1.0")]
	public class MatchingLogicalDevice : GenericObject2
	{
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[DefaultSerialization("DeviceIdentification")]
		[StorageVersion("3.4.1.0")]
		private IDeviceIdentification _devId;

		public IDeviceIdentification DeviceIdentification => _devId;

		public MatchingLogicalDevice()
			: this()
		{
		}

		internal MatchingLogicalDevice(XmlNode node)
			: this()
		{
			Import(node);
		}

		internal void Import(XmlNode node)
		{
			Debug.Assert(node is XmlElement);
			_devId = (IDeviceIdentification)(object)DeviceRefHelper.ReadDeviceRef(node, null);
		}
	}
}
