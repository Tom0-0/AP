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
	public class ConnectorType
	{
		private object[] hostParameterSetField;

		private CustomType customField;

		private int moduleTypeField;

		private ConnectorTypeRole roleField;

		private string interfaceField;

		private int connectorIdField;

		private int hostpathField;

		[XmlArrayItem("Parameter", typeof(ParameterType), IsNullable = false)]
		[XmlArrayItem("ParameterSection", typeof(ParameterSectionType), IsNullable = false)]
		public object[] HostParameterSet
		{
			get
			{
				return hostParameterSetField;
			}
			set
			{
				hostParameterSetField = value;
			}
		}

		public CustomType Custom
		{
			get
			{
				return customField;
			}
			set
			{
				customField = value;
			}
		}

		[XmlAttribute]
		public int moduleType
		{
			get
			{
				return moduleTypeField;
			}
			set
			{
				moduleTypeField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(ConnectorTypeRole.parent)]
		public ConnectorTypeRole role
		{
			get
			{
				return roleField;
			}
			set
			{
				roleField = value;
			}
		}

		[XmlAttribute]
		public string @interface
		{
			get
			{
				return interfaceField;
			}
			set
			{
				interfaceField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(-1)]
		public int connectorId
		{
			get
			{
				return connectorIdField;
			}
			set
			{
				connectorIdField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(-1)]
		public int hostpath
		{
			get
			{
				return hostpathField;
			}
			set
			{
				hostpathField = value;
			}
		}

		public ConnectorType()
		{
			roleField = ConnectorTypeRole.parent;
			connectorIdField = -1;
			hostpathField = -1;
		}
	}
}
