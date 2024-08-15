using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class EnumdefTypeEnum
	{
		private ValueType valueField;

		private string visibleNameField;

		private string descriptionField;

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

		public string VisibleName
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

		public string Description
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
