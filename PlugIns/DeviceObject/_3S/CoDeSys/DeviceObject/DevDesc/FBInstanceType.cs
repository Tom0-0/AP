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
	public class FBInstanceType
	{
		private FBInstanceTypeInitialize initializeField;

		private FBInstanceTypeCyclicCall[] cyclicCallField;

		private string fbnameField;

		private string basenameField;

		private FBInstanceTypeLocation locationField;

		private bool locationFieldSpecified;

		private string fbnamediagField;

		public FBInstanceTypeInitialize Initialize
		{
			get
			{
				return initializeField;
			}
			set
			{
				initializeField = value;
			}
		}

		[XmlElement("CyclicCall")]
		public FBInstanceTypeCyclicCall[] CyclicCall
		{
			get
			{
				return cyclicCallField;
			}
			set
			{
				cyclicCallField = value;
			}
		}

		[XmlAttribute]
		public string fbname
		{
			get
			{
				return fbnameField;
			}
			set
			{
				fbnameField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("$(DeviceName)")]
		public string basename
		{
			get
			{
				return basenameField;
			}
			set
			{
				basenameField = value;
			}
		}

		[XmlAttribute]
		public FBInstanceTypeLocation location
		{
			get
			{
				return locationField;
			}
			set
			{
				locationField = value;
			}
		}

		[XmlIgnore]
		public bool locationSpecified
		{
			get
			{
				return locationFieldSpecified;
			}
			set
			{
				locationFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string fbnamediag
		{
			get
			{
				return fbnamediagField;
			}
			set
			{
				fbnamediagField = value;
			}
		}

		public FBInstanceType()
		{
			basenameField = "$(DeviceName)";
		}
	}
}
