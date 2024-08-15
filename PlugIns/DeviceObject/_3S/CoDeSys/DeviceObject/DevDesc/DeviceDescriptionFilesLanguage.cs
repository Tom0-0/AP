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
	public class DeviceDescriptionFilesLanguage
	{
		private FileType[] fileField;

		private string langField;

		[XmlElement("File")]
		public FileType[] File
		{
			get
			{
				return fileField;
			}
			set
			{
				fileField = value;
			}
		}

		[XmlAttribute(DataType = "language")]
		public string lang
		{
			get
			{
				return langField;
			}
			set
			{
				langField = value;
			}
		}
	}
}
