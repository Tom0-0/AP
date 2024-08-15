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
	[XmlType(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	public class DeviceIdentificationType
	{
		private int typeField;

		private string idField;

		private string versionField;

		public int Type
		{
			get
			{
				return typeField;
			}
			set
			{
				typeField = value;
			}
		}

		public string Id
		{
			get
			{
				return idField;
			}
			set
			{
				idField = value;
			}
		}

		public string Version
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
