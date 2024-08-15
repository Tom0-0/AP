using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
	[XmlRoot(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd", IsNullable = false)]
	public class DeviceDescription
	{
		private DeviceDescriptionTypes typesField;

		private DeviceDescriptionParameterFilterFlags parameterFilterFlagsField;

		private DeviceDescriptionStrings[] stringsField;

		private DeviceDescriptionFiles filesField;

		private DeviceDescriptionDevice deviceField;

		private DeviceDescriptionModule[] modulesField;

		private XmlElement[] anyField;

		private string importPluginVersionField;

		private string externalConverterPluginVersionField;

		private DeviceDescriptionAdditionalFileToImport[] additionalFileToImportField;

		public DeviceDescriptionTypes Types
		{
			get
			{
				return typesField;
			}
			set
			{
				typesField = value;
			}
		}

		public DeviceDescriptionParameterFilterFlags ParameterFilterFlags
		{
			get
			{
				return parameterFilterFlagsField;
			}
			set
			{
				parameterFilterFlagsField = value;
			}
		}

		[XmlElement("Strings")]
		public DeviceDescriptionStrings[] Strings
		{
			get
			{
				return stringsField;
			}
			set
			{
				stringsField = value;
			}
		}

		public DeviceDescriptionFiles Files
		{
			get
			{
				return filesField;
			}
			set
			{
				filesField = value;
			}
		}

		public DeviceDescriptionDevice Device
		{
			get
			{
				return deviceField;
			}
			set
			{
				deviceField = value;
			}
		}

		[XmlArrayItem("Module", IsNullable = false)]
		public DeviceDescriptionModule[] Modules
		{
			get
			{
				return modulesField;
			}
			set
			{
				modulesField = value;
			}
		}

		[XmlAnyElement]
		public XmlElement[] Any
		{
			get
			{
				return anyField;
			}
			set
			{
				anyField = value;
			}
		}

		[DefaultValue("3.4.3.0")]
		public string ImportPluginVersion
		{
			get
			{
				return importPluginVersionField;
			}
			set
			{
				importPluginVersionField = value;
			}
		}

		[DefaultValue("3.4.3.0")]
		public string ExternalConverterPluginVersion
		{
			get
			{
				return externalConverterPluginVersionField;
			}
			set
			{
				externalConverterPluginVersionField = value;
			}
		}

		[XmlElement("AdditionalFileToImport")]
		public DeviceDescriptionAdditionalFileToImport[] AdditionalFileToImport
		{
			get
			{
				return additionalFileToImportField;
			}
			set
			{
				additionalFileToImportField = value;
			}
		}

		public DeviceDescription()
		{
			importPluginVersionField = "3.4.3.0";
			externalConverterPluginVersionField = "3.4.3.0";
		}
	}
}
