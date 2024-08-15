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
	public class FileType
	{
		private string itemField;

		private FileTypeFileref filerefField;

		private string identifierField;

		[XmlElement("LocalFile")]
		public string Item
		{
			get
			{
				return itemField;
			}
			set
			{
				itemField = value;
			}
		}

		[XmlAttribute]
		public FileTypeFileref fileref
		{
			get
			{
				return filerefField;
			}
			set
			{
				filerefField = value;
			}
		}

		[XmlAttribute(DataType = "NCName")]
		public string identifier
		{
			get
			{
				return identifierField;
			}
			set
			{
				identifierField = value;
			}
		}
	}
}
