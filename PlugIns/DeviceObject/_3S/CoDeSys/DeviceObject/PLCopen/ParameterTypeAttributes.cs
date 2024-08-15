using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	public class ParameterTypeAttributes
	{
		private XmlElement[] anyField;

		private bool alwaysmappingField;

		private ParameterTypeAttributesAlwaysmappingMode alwaysmappingModeField;

		private bool downloadField;

		private bool functionalField;

		private ParameterTypeAttributesChannel channelField;

		private bool createDownloadStructureField;

		private bool createInHostConnectorField;

		private bool createInChildConnectorField;

		private bool onlineparameterField;

		private AccessRightType offlineaccessField;

		private AccessRightType onlineaccessField;

		private string instanceVariableField;

		private string driverSpecificField;

		private bool preparedValueAccessField;

		private bool useRefactoringField;

		private bool disableMappingField;

		private bool bidirectionaloutputField;

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
		[DefaultValue(ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused)]
		public ParameterTypeAttributesAlwaysmappingMode alwaysmappingMode
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
		[DefaultValue(true)]
		public bool download
		{
			get
			{
				return downloadField;
			}
			set
			{
				downloadField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool functional
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

		[XmlAttribute]
		[DefaultValue(ParameterTypeAttributesChannel.none)]
		public ParameterTypeAttributesChannel channel
		{
			get
			{
				return channelField;
			}
			set
			{
				channelField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(true)]
		public bool createDownloadStructure
		{
			get
			{
				return createDownloadStructureField;
			}
			set
			{
				createDownloadStructureField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool createInHostConnector
		{
			get
			{
				return createInHostConnectorField;
			}
			set
			{
				createInHostConnectorField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool createInChildConnector
		{
			get
			{
				return createInChildConnectorField;
			}
			set
			{
				createInChildConnectorField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool onlineparameter
		{
			get
			{
				return onlineparameterField;
			}
			set
			{
				onlineparameterField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(AccessRightType.readwrite)]
		public AccessRightType offlineaccess
		{
			get
			{
				return offlineaccessField;
			}
			set
			{
				offlineaccessField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(AccessRightType.readwrite)]
		public AccessRightType onlineaccess
		{
			get
			{
				return onlineaccessField;
			}
			set
			{
				onlineaccessField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("")]
		public string instanceVariable
		{
			get
			{
				return instanceVariableField;
			}
			set
			{
				instanceVariableField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("")]
		public string driverSpecific
		{
			get
			{
				return driverSpecificField;
			}
			set
			{
				driverSpecificField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool preparedValueAccess
		{
			get
			{
				return preparedValueAccessField;
			}
			set
			{
				preparedValueAccessField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool useRefactoring
		{
			get
			{
				return useRefactoringField;
			}
			set
			{
				useRefactoringField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool disableMapping
		{
			get
			{
				return disableMappingField;
			}
			set
			{
				disableMappingField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool bidirectionaloutput
		{
			get
			{
				return bidirectionaloutputField;
			}
			set
			{
				bidirectionaloutputField = value;
			}
		}

		public ParameterTypeAttributes()
		{
			alwaysmappingField = false;
			alwaysmappingModeField = ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused;
			downloadField = true;
			functionalField = false;
			channelField = ParameterTypeAttributesChannel.none;
			createDownloadStructureField = true;
			createInHostConnectorField = false;
			createInChildConnectorField = false;
			onlineparameterField = false;
			offlineaccessField = AccessRightType.readwrite;
			onlineaccessField = AccessRightType.readwrite;
			instanceVariableField = "";
			driverSpecificField = "";
			preparedValueAccessField = false;
			useRefactoringField = false;
			disableMappingField = false;
			bidirectionaloutputField = false;
		}
	}
}
