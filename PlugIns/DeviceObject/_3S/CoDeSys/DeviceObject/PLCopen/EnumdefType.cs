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
	public class EnumdefType : TypedefType
	{
		private EnumdefTypeEnum[] enumField;

		private string basetypeField;

		[XmlElement("Enum")]
		public EnumdefTypeEnum[] Enum
		{
			get
			{
				return enumField;
			}
			set
			{
				enumField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		[DefaultValue("std.INT")]
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

		public EnumdefType()
		{
			basetypeField = "std.INT";
		}
	}
}
