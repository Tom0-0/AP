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
	public class DeviceDescriptionDeviceDependentDevice
	{
		private DeviceDescriptionDeviceDependentDeviceDeviceIdentification[] deviceIdentificationField;

		private bool autoInsertDependentDevicesField;

		[XmlElement("DeviceIdentification")]
		public DeviceDescriptionDeviceDependentDeviceDeviceIdentification[] DeviceIdentification
		{
			get
			{
				return deviceIdentificationField;
			}
			set
			{
				deviceIdentificationField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool AutoInsertDependentDevices
		{
			get
			{
				return autoInsertDependentDevicesField;
			}
			set
			{
				autoInsertDependentDevicesField = value;
			}
		}

		public DeviceDescriptionDeviceDependentDevice()
		{
			autoInsertDependentDevicesField = false;
		}
	}
}
