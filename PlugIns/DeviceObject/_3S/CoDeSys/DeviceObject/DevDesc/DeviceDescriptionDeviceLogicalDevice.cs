using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	public class DeviceDescriptionDeviceLogicalDevice
	{
		private DeviceDescriptionDeviceLogicalDeviceMatchingLogicalDevice[] matchingLogicalDeviceField;

		private ushort insertAutoIndexField;

		private bool insertAutoIndexFieldSpecified;

		[XmlElement("MatchingLogicalDevice")]
		public DeviceDescriptionDeviceLogicalDeviceMatchingLogicalDevice[] MatchingLogicalDevice
		{
			get
			{
				return matchingLogicalDeviceField;
			}
			set
			{
				matchingLogicalDeviceField = value;
			}
		}

		[XmlAttribute]
		public ushort insertAutoIndex
		{
			get
			{
				return insertAutoIndexField;
			}
			set
			{
				insertAutoIndexField = value;
			}
		}

		[XmlIgnore]
		public bool insertAutoIndexSpecified
		{
			get
			{
				return insertAutoIndexFieldSpecified;
			}
			set
			{
				insertAutoIndexFieldSpecified = value;
			}
		}
	}
}
