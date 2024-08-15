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
    public class ParameterType
    {
        private ParameterTypeAttributes attributesField;

        private ValueType[] valueField;

        private string nameField;

        private string unitField;

        private string descriptionField;

        private ValueType[] mappingField;

        private XmlElement[] anyField;

        private CustomType customField;

        private uint parameterIdField;

        private string typeField;

        private long indexInDevDescField;

        private bool indexInDevDescFieldSpecified;

        private string fixedAddressField;

        public ParameterTypeAttributes Attributes
        {
            get
            {
                return attributesField;
            }
            set
            {
                attributesField = value;
            }
        }

        [XmlElement("Value")]
        public ValueType[] Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }

        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        public string Unit
        {
            get
            {
                return unitField;
            }
            set
            {
                unitField = value;
            }
        }

        public string Description
        {
            get
            {
                return descriptionField;
            }
            set
            {
                descriptionField = value;
            }
        }

        [XmlElement("Mapping")]
        public ValueType[] Mapping
        {
            get
            {
                return mappingField;
            }
            set
            {
                mappingField = value;
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
        public uint ParameterId
        {
            get
            {
                return parameterIdField;
            }
            set
            {
                parameterIdField = value;
            }
        }

        [XmlAttribute]
        public string type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }

        [XmlAttribute]
        public long IndexInDevDesc
        {
            get
            {
                return indexInDevDescField;
            }
            set
            {
                indexInDevDescField = value;
            }
        }

        [XmlIgnore]
        public bool IndexInDevDescSpecified
        {
            get
            {
                return indexInDevDescFieldSpecified;
            }
            set
            {
                indexInDevDescFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public string FixedAddress
        {
            get
            {
                return fixedAddressField;
            }
            set
            {
                fixedAddressField = value;
            }
        }
    }
}
