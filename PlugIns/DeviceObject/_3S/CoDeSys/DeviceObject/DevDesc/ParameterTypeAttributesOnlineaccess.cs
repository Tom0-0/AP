using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[XmlType(AnonymousType = true, Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	public enum ParameterTypeAttributesOnlineaccess
	{
		none,
		read,
		write,
		readwrite
	}
}
