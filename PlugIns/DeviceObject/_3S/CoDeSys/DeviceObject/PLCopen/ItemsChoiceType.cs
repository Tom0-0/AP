using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(IncludeInSchema = false)]
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
