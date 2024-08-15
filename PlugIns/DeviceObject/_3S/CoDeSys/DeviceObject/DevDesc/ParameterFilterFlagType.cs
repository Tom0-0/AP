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
	public class ParameterFilterFlagType
	{
		private StringRefType[] displayNameField;

		private string nameField;

		[XmlElement("DisplayName")]
		public StringRefType[] DisplayName
		{
			get
			{
				return displayNameField;
			}
			set
			{
				displayNameField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string name
		{
			get
			{
				return nameField;
			}
			set
			{
				nameField = value;
			}
		}
	}
}
