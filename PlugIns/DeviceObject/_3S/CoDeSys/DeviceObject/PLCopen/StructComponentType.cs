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
	public class StructComponentType
	{
		private ValueType defaultField;

		private string visibleNameField;

		private string unitField;

		private string descriptionField;

		private string filterFlagsField;

		private CustomType customField;

		private string identifierField;

		private string typeField;

		public ValueType Default
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

		public string Unit
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

		[XmlAttribute(DataType = "NMTOKEN")]
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
