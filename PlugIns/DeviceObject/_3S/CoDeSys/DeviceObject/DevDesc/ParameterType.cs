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
	[XmlType(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	public class ParameterType
	{
		private ParameterTypeAttributes attributesField;

		private ValueType[] defaultField;

		private StringRefType nameField;

		private StringRefType unitField;

		private StringRefType descriptionField;

		private string filterFlagsField;

		private CustomType customField;

		private ValueType[] defaultMappingField;

		private XmlElement[] anyField;

		private uint parameterIdField;

		private string typeField;

		public ParameterTypeAttributes Attributes
		{
			get
			{
				return attributesField;
			}
			set
			{
				attributesField = value;
			}
		}

		[XmlElement("Default")]
		public ValueType[] Default
		{
			get
			{
				return defaultField;
			}
			set
			{
				defaultField = value;
			}
		}

		public StringRefType Name
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

		public StringRefType Unit
		{
			get
			{
				return unitField;
			}
			set
			{
				unitField = value;
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

		[XmlElement(DataType = "NMTOKENS")]
		public string FilterFlags
		{
			get
			{
				return filterFlagsField;
			}
			set
			{
				filterFlagsField = value;
			}
		}

		public CustomType Custom
		{
			get
			{
				return customField;
			}
			set
			{
				customField = value;
			}
		}

		[XmlElement("DefaultMapping")]
		public ValueType[] DefaultMapping
		{
			get
			{
				return defaultMappingField;
			}
			set
			{
				defaultMappingField = value;
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

		[XmlAttribute]
		public uint ParameterId
		{
			get
			{
				return parameterIdField;
			}
			set
			{
				parameterIdField = value;
			}
		}

		[XmlAttribute]
		public string type
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
	}
}
