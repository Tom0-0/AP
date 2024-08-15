using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	public class EnumdefTypeEnum
	{
		private ValueType valueField;

		private StringRefType visibleNameField;

		private StringRefType descriptionField;

		private XmlElement[] anyField;

		private string identifierField;

		public ValueType Value
		{
			get
			{
				return valueField;
			}
			set
			{
				valueField = value;
			}
		}

		public StringRefType VisibleName
		{
			get
			{
				return visibleNameField;
			}
			set
			{
				visibleNameField = value;
			}
		}

		public StringRefType Description
		{
			get
			{
				return descriptionField;
			}
			set
			{
				descriptionField = value;
			}
		}

		[XmlAnyElement]
		public XmlElement[] Any
		{
			get
			{
				return anyField;
			}
			set
			{
				anyField = value;
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
