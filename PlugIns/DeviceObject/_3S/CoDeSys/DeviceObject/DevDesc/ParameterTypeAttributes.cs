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
    public class ParameterTypeAttributes
    {
        private XmlElement[] anyField;

        private bool alwaysmappingField;

        private ParameterTypeAttributesAlwaysmappingMode alwaysmappingModeField;

        private ParameterTypeAttributesOnlineaccess onlineaccessField;

        private ParameterTypeAttributesOfflineaccess offlineaccessField;

        private bool downloadField;

        private bool functionalField;

        private ParameterTypeAttributesChannel channelField;

        private bool channelFieldSpecified;

        private bool createDownloadStructureField;

        private bool createInHostConnectorField;

        private bool createInChildConnectorField;

        private bool onlineparameterField;

        private bool noManualAddressField;

        private bool logicalParameterField;

        private bool mapOnlyNewField;

        private ulong mapSizeField;

        private ulong mapOffsetField;

        private bool onlineChangeEnabledField;

        private bool traceSystemRecordField;

        private bool constantValueField;

        private string onlineHelpUrlField;

        private string driverSpecificField;

        private string instanceVariableField;

        private bool preparedValueAccessField;

        private bool useRefactoringField;

        private bool disableMappingField;

        private bool bidirectionaloutputField;

        private ParameterTypeAttributesChannelCompatible channelFieldComp;

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
        [DefaultValue(ParameterTypeAttributesOnlineaccess.readwrite)]
        public ParameterTypeAttributesOnlineaccess onlineaccess
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
        [DefaultValue(ParameterTypeAttributesOfflineaccess.readwrite)]
        public ParameterTypeAttributesOfflineaccess offlineaccess
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

        [XmlIgnore]
        public bool channelSpecified
        {
            get
            {
                return channelFieldSpecified;
            }
            set
            {
                channelFieldSpecified = value;
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
        [DefaultValue(false)]
        public bool noManualAddress
        {
            get
            {
                return noManualAddressField;
            }
            set
            {
                noManualAddressField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool logicalParameter
        {
            get
            {
                return logicalParameterField;
            }
            set
            {
                logicalParameterField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool mapOnlyNew
        {
            get
            {
                return mapOnlyNewField;
            }
            set
            {
                mapOnlyNewField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(typeof(ulong), "0")]
        public ulong mapSize
        {
            get
            {
                return mapSizeField;
            }
            set
            {
                mapSizeField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(typeof(ulong), "0")]
        public ulong mapOffset
        {
            get
            {
                return mapOffsetField;
            }
            set
            {
                mapOffsetField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool OnlineChangeEnabled
        {
            get
            {
                return onlineChangeEnabledField;
            }
            set
            {
                onlineChangeEnabledField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool TraceSystemRecord
        {
            get
            {
                return traceSystemRecordField;
            }
            set
            {
                traceSystemRecordField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool constantValue
        {
            get
            {
                return constantValueField;
            }
            set
            {
                constantValueField = value;
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

        [XmlAttribute("channel")]
        public ParameterTypeAttributesChannelCompatible channelAttr
        {
            get
            {
                return channelFieldComp;
            }
            set
            {
                channelFieldComp = value;
            }
        }

        [XmlIgnore]
        public bool channelAttrSpecified
        {
            get
            {
                return channelFieldSpecified;
            }
            set
            {
                channelFieldSpecified = value;
            }
        }

        public ParameterTypeAttributes()
        {
            alwaysmappingField = false;
            alwaysmappingModeField = ParameterTypeAttributesAlwaysmappingMode.OnlyIfUnused;
            onlineaccessField = ParameterTypeAttributesOnlineaccess.readwrite;
            offlineaccessField = ParameterTypeAttributesOfflineaccess.readwrite;
            downloadField = true;
            functionalField = false;
            createDownloadStructureField = true;
            createInHostConnectorField = false;
            createInChildConnectorField = false;
            onlineparameterField = false;
            noManualAddressField = false;
            logicalParameterField = false;
            mapOnlyNewField = false;
            mapSizeField = 0uL;
            mapOffsetField = 0uL;
            onlineChangeEnabledField = false;
            traceSystemRecordField = false;
            constantValueField = false;
            driverSpecificField = "";
            instanceVariableField = "";
            preparedValueAccessField = false;
            useRefactoringField = false;
            disableMappingField = false;
            bidirectionaloutputField = false;
        }
    }
}
