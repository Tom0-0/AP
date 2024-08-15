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
	public class ConnectorType
	{
		private StringRefType interfaceNameField;

		private object[] itemsField;

		private object[] hostParameterSetField;

		private DriverInfoType driverInfoField;

		private CustomType customField;

		private string[] appearanceField;

		private ConnectorTypeClient clientField;

		private ConnectorTypeAdditionalInterface[] additionalInterfaceField;

		private ConnectorTypeConstraint[] constraintField;

		private DeviceRefType[] allowOnlyField;

		private int moduleTypeField;

		private ConnectorTypeRole roleField;

		private string interfaceField;

		private int connectorIdField;

		private int hostpathField;

		private bool explicitField;

		private bool alwaysmappingField;

		private ConnectorTypeAlwaysmappingMode alwaysmappingModeField;

		private bool alwaysmappingDisabledField;

		private bool hideInStatusPageField;

		private bool updateAllowedField;

		private string fixedInputAddressField;

		private string fixedOutputAddressField;

		private bool downloadParamsDevDescOrderField;

		private uint initialStatusFlagField;

		private bool useBlobInitConstField;

		public StringRefType InterfaceName
		{
			get
			{
				return interfaceNameField;
			}
			set
			{
				interfaceNameField = value;
			}
		}

		[XmlElement("Fixed", typeof(ConnectorTypeFixed))]
		[XmlElement("Slot", typeof(ConnectorTypeSlot))]
		[XmlElement("Var", typeof(ConnectorTypeVar))]
		public object[] Items
		{
			get
			{
				return itemsField;
			}
			set
			{
				itemsField = value;
			}
		}

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

		[XmlArrayItem("ShowEditor", IsNullable = false)]
		public string[] Appearance
		{
			get
			{
				return appearanceField;
			}
			set
			{
				appearanceField = value;
			}
		}

		public ConnectorTypeClient Client
		{
			get
			{
				return clientField;
			}
			set
			{
				clientField = value;
			}
		}

		[XmlElement("AdditionalInterface")]
		public ConnectorTypeAdditionalInterface[] AdditionalInterface
		{
			get
			{
				return additionalInterfaceField;
			}
			set
			{
				additionalInterfaceField = value;
			}
		}

		[XmlElement("Constraint")]
		public ConnectorTypeConstraint[] Constraint
		{
			get
			{
				return constraintField;
			}
			set
			{
				constraintField = value;
			}
		}

		[XmlElement("AllowOnly")]
		public DeviceRefType[] AllowOnly
		{
			get
			{
				return allowOnlyField;
			}
			set
			{
				allowOnlyField = value;
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

		[XmlAttribute]
		[DefaultValue(false)]
		public bool @explicit
		{
			get
			{
				return explicitField;
			}
			set
			{
				explicitField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool alwaysmapping
		{
			get
			{
				return alwaysmappingField;
			}
			set
			{
				alwaysmappingField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(ConnectorTypeAlwaysmappingMode.OnlyIfUnused)]
		public ConnectorTypeAlwaysmappingMode alwaysmappingMode
		{
			get
			{
				return alwaysmappingModeField;
			}
			set
			{
				alwaysmappingModeField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool alwaysmappingDisabled
		{
			get
			{
				return alwaysmappingDisabledField;
			}
			set
			{
				alwaysmappingDisabledField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool hideInStatusPage
		{
			get
			{
				return hideInStatusPageField;
			}
			set
			{
				hideInStatusPageField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(true)]
		public bool updateAllowed
		{
			get
			{
				return updateAllowedField;
			}
			set
			{
				updateAllowedField = value;
			}
		}

		[XmlAttribute]
		public string fixedInputAddress
		{
			get
			{
				return fixedInputAddressField;
			}
			set
			{
				fixedInputAddressField = value;
			}
		}

		[XmlAttribute]
		public string fixedOutputAddress
		{
			get
			{
				return fixedOutputAddressField;
			}
			set
			{
				fixedOutputAddressField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool downloadParamsDevDescOrder
		{
			get
			{
				return downloadParamsDevDescOrderField;
			}
			set
			{
				downloadParamsDevDescOrderField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(typeof(uint), "1")]
		public uint initialStatusFlag
		{
			get
			{
				return initialStatusFlagField;
			}
			set
			{
				initialStatusFlagField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool useBlobInitConst
		{
			get
			{
				return useBlobInitConstField;
			}
			set
			{
				useBlobInitConstField = value;
			}
		}

		public ConnectorType()
		{
			roleField = ConnectorTypeRole.parent;
			connectorIdField = -1;
			hostpathField = -1;
			explicitField = false;
			alwaysmappingField = false;
			alwaysmappingModeField = ConnectorTypeAlwaysmappingMode.OnlyIfUnused;
			alwaysmappingDisabledField = false;
			hideInStatusPageField = false;
			updateAllowedField = true;
			downloadParamsDevDescOrderField = false;
			initialStatusFlagField = 1u;
			useBlobInitConstField = false;
		}
	}
}
