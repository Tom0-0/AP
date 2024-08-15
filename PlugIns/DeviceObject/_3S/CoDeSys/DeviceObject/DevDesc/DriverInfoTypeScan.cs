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
	public class DriverInfoTypeScan
	{
		private bool supportedField;

		private bool nominateField;

		private bool identifyField;

		private bool uploadDescriptionField;

		[XmlAttribute]
		[DefaultValue(false)]
		public bool supported
		{
			get
			{
				return supportedField;
			}
			set
			{
				supportedField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool nominate
		{
			get
			{
				return nominateField;
			}
			set
			{
				nominateField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool identify
		{
			get
			{
				return identifyField;
			}
			set
			{
				identifyField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool uploadDescription
		{
			get
			{
				return uploadDescriptionField;
			}
			set
			{
				uploadDescriptionField = value;
			}
		}

		public DriverInfoTypeScan()
		{
			supportedField = false;
			nominateField = false;
			identifyField = false;
			uploadDescriptionField = false;
		}
	}
}
