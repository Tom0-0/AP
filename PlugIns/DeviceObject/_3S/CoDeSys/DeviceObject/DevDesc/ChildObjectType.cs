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
	[XmlType(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	public class ChildObjectType
	{
		private string objectGuidField;

		private string objectNameField;

		private ChildObjectType[] childObjectField;

		private bool requiredField;

		public string ObjectGuid
		{
			get
			{
				return objectGuidField;
			}
			set
			{
				objectGuidField = value;
			}
		}

		public string ObjectName
		{
			get
			{
				return objectNameField;
			}
			set
			{
				objectNameField = value;
			}
		}

		[XmlElement("ChildObject")]
		public ChildObjectType[] ChildObject
		{
			get
			{
				return childObjectField;
			}
			set
			{
				childObjectField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool required
		{
			get
			{
				return requiredField;
			}
			set
			{
				requiredField = value;
			}
		}

		public ChildObjectType()
		{
			requiredField = false;
		}
	}
}
