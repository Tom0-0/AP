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
	public class StructdefType : TypedefType
	{
		private StructComponentType[] componentField;

		private string iecTypeField;

		private string iecTypeLibField;

		[XmlElement("Component")]
		public StructComponentType[] Component
		{
			get
			{
				return componentField;
			}
			set
			{
				componentField = value;
			}
		}

		[XmlAttribute]
		public string iecType
		{
			get
			{
				return iecTypeField;
			}
			set
			{
				iecTypeField = value;
			}
		}

		[XmlAttribute]
		public string iecTypeLib
		{
			get
			{
				return iecTypeLibField;
			}
			set
			{
				iecTypeLibField = value;
			}
		}
	}
}
