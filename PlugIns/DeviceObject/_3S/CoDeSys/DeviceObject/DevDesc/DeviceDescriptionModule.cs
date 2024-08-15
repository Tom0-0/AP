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
	public class DeviceDescriptionModule
	{
		private string moduleIdField;

		private DeviceDescriptionDeviceDeviceInfo deviceInfoField;

		private DeviceDescriptionDeviceConnector[] connectorField;

		private ConnectorGroupType[] connectorGroupField;

		private DeviceDescriptionDeviceDeviceParameterSet deviceParameterSetField;

		private DriverInfoType driverInfoField;

		private object[] functionalField;

		private DeviceDescriptionDeviceExtendedSettings extendedSettingsField;

		private CustomType customField;

		private DeviceDescriptionDeviceLogicalDevice[] logicalDeviceField;

		private string[] supportedLogicalBusSystemsField;

		private DeviceDescriptionDeviceDependentDevice dependentDeviceField;

		private XmlElement[] anyField;

		private bool hideInCatalogueField;

		private bool showParamsInDevDescOrderField;

		private string onlineHelpUrlField;

		public string ModuleId
		{
			get
			{
				return moduleIdField;
			}
			set
			{
				moduleIdField = value;
			}
		}

		public DeviceDescriptionDeviceDeviceInfo DeviceInfo
		{
			get
			{
				return deviceInfoField;
			}
			set
			{
				deviceInfoField = value;
			}
		}

		[XmlElement("Connector")]
		public DeviceDescriptionDeviceConnector[] Connector
		{
			get
			{
				return connectorField;
			}
			set
			{
				connectorField = value;
			}
		}

		[XmlElement("ConnectorGroup")]
		public ConnectorGroupType[] ConnectorGroup
		{
			get
			{
				return connectorGroupField;
			}
			set
			{
				connectorGroupField = value;
			}
		}

		public DeviceDescriptionDeviceDeviceParameterSet DeviceParameterSet
		{
			get
			{
				return deviceParameterSetField;
			}
			set
			{
				deviceParameterSetField = value;
			}
		}

		public DriverInfoType DriverInfo
		{
			get
			{
				return driverInfoField;
			}
			set
			{
				driverInfoField = value;
			}
		}

		[XmlArrayItem("Attribute", typeof(DeviceDescriptionDeviceAttribute), IsNullable = false)]
		[XmlArrayItem("ChildObject", typeof(ChildObjectType), IsNullable = false)]
		public object[] Functional
		{
			get
			{
				return functionalField;
			}
			set
			{
				functionalField = value;
			}
		}

		public DeviceDescriptionDeviceExtendedSettings ExtendedSettings
		{
			get
			{
				return extendedSettingsField;
			}
			set
			{
				extendedSettingsField = value;
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

		[XmlElement("LogicalDevice")]
		public DeviceDescriptionDeviceLogicalDevice[] LogicalDevice
		{
			get
			{
				return logicalDeviceField;
			}
			set
			{
				logicalDeviceField = value;
			}
		}

		[XmlElement("SupportedLogicalBusSystems")]
		public string[] SupportedLogicalBusSystems
		{
			get
			{
				return supportedLogicalBusSystemsField;
			}
			set
			{
				supportedLogicalBusSystemsField = value;
			}
		}

		public DeviceDescriptionDeviceDependentDevice DependentDevice
		{
			get
			{
				return dependentDeviceField;
			}
			set
			{
				dependentDeviceField = value;
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

		[XmlAttribute]
		[DefaultValue(false)]
		public bool hideInCatalogue
		{
			get
			{
				return hideInCatalogueField;
			}
			set
			{
				hideInCatalogueField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool showParamsInDevDescOrder
		{
			get
			{
				return showParamsInDevDescOrderField;
			}
			set
			{
				showParamsInDevDescOrderField = value;
			}
		}

		[XmlAttribute]
		public string onlineHelpUrl
		{
			get
			{
				return onlineHelpUrlField;
			}
			set
			{
				onlineHelpUrlField = value;
			}
		}

		public DeviceDescriptionModule()
		{
			hideInCatalogueField = false;
			showParamsInDevDescOrderField = false;
		}
	}
}
