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
	public class DriverInfoTypeRequiredLib
	{
		private FBInstanceType[] fBInstanceField;

		private string libnameField;

		private string vendorField;

		private string versionField;

		private string identifierField;

		private string placeholderlibField;

		private bool loadAsSystemLibraryField;

		[XmlElement("FBInstance")]
		public FBInstanceType[] FBInstance
		{
			get
			{
				return fBInstanceField;
			}
			set
			{
				fBInstanceField = value;
			}
		}

		[XmlAttribute]
		public string libname
		{
			get
			{
				return libnameField;
			}
			set
			{
				libnameField = value;
			}
		}

		[XmlAttribute]
		public string vendor
		{
			get
			{
				return vendorField;
			}
			set
			{
				vendorField = value;
			}
		}

		[XmlAttribute]
		public string version
		{
			get
			{
				return versionField;
			}
			set
			{
				versionField = value;
			}
		}

		[XmlAttribute]
		public string identifier
		{
			get
			{
				return identifierField;
			}
			set
			{
				identifierField = value;
			}
		}

		[XmlAttribute]
		public string placeholderlib
		{
			get
			{
				return placeholderlibField;
			}
			set
			{
				placeholderlibField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(true)]
		public bool loadAsSystemLibrary
		{
			get
			{
				return loadAsSystemLibraryField;
			}
			set
			{
				loadAsSystemLibraryField = value;
			}
		}

		public DriverInfoTypeRequiredLib()
		{
			loadAsSystemLibraryField = true;
		}
	}
}
