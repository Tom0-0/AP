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
	public class StructdefType : TypedefType
	{
		private StructdefTypeComponent[] componentField;

		private string iecTypeField;

		private string iecTypeLibField;

		[XmlElement("Component")]
		public StructdefTypeComponent[] Component
		{
			get
			{
				return componentField;
			}
			set
			{
				componentField = value;
			}
		}

		[XmlAttribute]
		public string iecType
		{
			get
			{
				return iecTypeField;
			}
			set
			{
				iecTypeField = value;
			}
		}

		[XmlAttribute]
		public string iecTypeLib
		{
			get
			{
				return iecTypeLibField;
			}
			set
			{
				iecTypeLibField = value;
			}
		}
	}
}
