using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[XmlInclude(typeof(StructdefType))]
	[XmlInclude(typeof(ArraydefType))]
	[XmlInclude(typeof(RangedefType))]
	[XmlInclude(typeof(EnumdefType))]
	[XmlInclude(typeof(BitfielddefType))]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class TypedefType
	{
		private string nameField;

		[XmlAttribute(DataType = "NCName")]
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
