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
	public class FBInstanceTypeCyclicCall
	{
		private string methodnameField;

		private FBInstanceTypeCyclicCallWhentocall whentocallField;

		private string taskField;

		[XmlAttribute]
		public string methodname
		{
			get
			{
				return methodnameField;
			}
			set
			{
				methodnameField = value;
			}
		}

		[XmlAttribute]
		public FBInstanceTypeCyclicCallWhentocall whentocall
		{
			get
			{
				return whentocallField;
			}
			set
			{
				whentocallField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("#eachtask")]
		public string task
		{
			get
			{
				return taskField;
			}
			set
			{
				taskField = value;
			}
		}

		public FBInstanceTypeCyclicCall()
		{
			taskField = "#eachtask";
		}
	}
}
