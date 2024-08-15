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
    public class DeviceType
    {
        private DeviceIdentificationType itemField;

        private DeviceTypeConnector[] connectorField;

        private ConnectorGroupType[] connectorGroupField;

        private DeviceParameterListType deviceParameterSetField;

        private bool disableField;

        private bool excludeField;

        private bool excludeFromBuildField;

        [XmlElement("DeviceIdentification", typeof(DeviceIdentificationType))]
        [XmlElement("ModuleIdentification", typeof(ModuleIdentificationType))]
        public DeviceIdentificationType Item
        {
            get
            {
                return itemField;
            }
            set
            {
                itemField = value;
            }
        }

        [XmlElement("Connector")]
        public DeviceTypeConnector[] Connector
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

        public DeviceParameterListType DeviceParameterSet
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

        [XmlAttribute]
        [DefaultValue(false)]
        public bool Disable
        {
            get
            {
                return disableField;
            }
            set
            {
                disableField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool Exclude
        {
            get
            {
                return excludeField;
            }
            set
            {
                excludeField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool ExcludeFromBuild
        {
            get
            {
                return excludeFromBuildField;
            }
            set
            {
                excludeFromBuildField = value;
            }
        }

        public DeviceType()
        {
            disableField = false;
            excludeField = false;
            excludeFromBuildField = false;
        }
    }
}
