using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class BitfielddefType : TypedefType
	{
		private StructComponentType[] componentField;

		private string basetypeField;

		[XmlElement("Component")]
		public StructComponentType[] Component
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

		[XmlAttribute(DataType = "NMTOKEN")]
		public string basetype
		{
			get
			{
				return basetypeField;
			}
			set
			{
				basetypeField = value;
			}
		}
	}
}
