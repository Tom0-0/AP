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
	public class DeviceDescriptionTypes
	{
		private object[] itemsField;

		private ItemsChoiceType[] itemsElementNameField;

		private string namespaceField;

		[XmlElement("ArrayType", typeof(ArraydefType))]
		[XmlElement("BitfieldType", typeof(BitfielddefType))]
		[XmlElement("EnumType", typeof(EnumdefType))]
		[XmlElement("RangeType", typeof(RangedefType))]
		[XmlElement("StructType", typeof(StructdefType))]
		[XmlElement("UnionType", typeof(StructdefType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		public object[] Items
		{
			get
			{
				return itemsField;
			}
			set
			{
				itemsField = value;
			}
		}

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType[] ItemsElementName
		{
			get
			{
				return itemsElementNameField;
			}
			set
			{
				itemsElementNameField = value;
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
