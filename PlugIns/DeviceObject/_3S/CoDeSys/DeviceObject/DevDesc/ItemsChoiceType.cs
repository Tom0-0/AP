using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd", IncludeInSchema = false)]
	public enum ItemsChoiceType
	{
		ArrayType,
		BitfieldType,
		EnumType,
		RangeType,
		StructType,
		UnionType
	}
}
