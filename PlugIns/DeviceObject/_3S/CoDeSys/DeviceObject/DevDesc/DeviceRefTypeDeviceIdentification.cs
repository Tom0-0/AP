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
	public class DeviceRefTypeDeviceIdentification
	{
		private int deviceTypeField;

		private string deviceIdField;

		private string versionField;

		[XmlAttribute]
		public int deviceType
		{
			get
			{
				return deviceTypeField;
			}
			set
			{
				deviceTypeField = value;
			}
		}

		[XmlAttribute]
		public string deviceId
		{
			get
			{
				return deviceIdField;
			}
			set
			{
				deviceIdField = value;
			}
		}

		[XmlAttribute]
		public string version
		{
			get
			{
				return versionField;
			}
			set
			{
				versionField = value;
			}
		}
	}
}
