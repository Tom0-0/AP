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
	public class ConnectorTypeVar
	{
		private ConnectorTypeVarDefaultModule[] defaultModuleField;

		private string maxField;

		private bool subdevicesCollapsedField;

		[XmlElement("DefaultModule")]
		public ConnectorTypeVarDefaultModule[] DefaultModule
		{
			get
			{
				return defaultModuleField;
			}
			set
			{
				defaultModuleField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("unbounded")]
		public string max
		{
			get
			{
				return maxField;
			}
			set
			{
				maxField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool subdevicesCollapsed
		{
			get
			{
				return subdevicesCollapsedField;
			}
			set
			{
				subdevicesCollapsedField = value;
			}
		}

		public ConnectorTypeVar()
		{
			maxField = "unbounded";
			subdevicesCollapsedField = false;
		}
	}
}
