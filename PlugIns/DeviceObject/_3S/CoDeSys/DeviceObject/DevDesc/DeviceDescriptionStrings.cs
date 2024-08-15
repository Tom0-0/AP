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
	public class DeviceDescriptionStrings
	{
		private DeviceDescriptionStringsLanguage[] languageField;

		private string namespaceField;

		[XmlElement("Language")]
		public DeviceDescriptionStringsLanguage[] Language
		{
			get
			{
				return languageField;
			}
			set
			{
				languageField = value;
			}
		}

		[XmlAttribute(DataType = "NCName")]
		public string @namespace
		{
			get
			{
				return namespaceField;
			}
			set
			{
				namespaceField = value;
			}
		}
	}
}
