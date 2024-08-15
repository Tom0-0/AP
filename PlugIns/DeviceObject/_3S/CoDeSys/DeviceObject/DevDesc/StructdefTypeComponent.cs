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
    public class StructdefTypeComponent
    {
        private ValueType defaultField;

        private StringRefType visibleNameField;

        private StringRefType unitField;

        private StringRefType descriptionField;

        private string filterFlagsField;

        private CustomType customField;

        private string identifierField;

        private string typeField;

        private StructdefTypeComponentOnlineaccess onlineaccessField;

        private StructdefTypeComponentOfflineaccess offlineaccessField;

        public ValueType Default
        {
            get
            {
                return defaultField;
            }
            set
            {
                defaultField = value;
            }
        }

        public StringRefType VisibleName
        {
            get
            {
                return visibleNameField;
            }
            set
            {
                visibleNameField = value;
            }
        }

        public StringRefType Unit
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

        public StringRefType Description
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

        [XmlElement(DataType = "NMTOKENS")]
        public string FilterFlags
        {
            get
            {
                return filterFlagsField;
            }
            set
            {
                filterFlagsField = value;
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

        [XmlAttribute(DataType = "NCName")]
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
        [DefaultValue(StructdefTypeComponentOnlineaccess.readwrite)]
        public StructdefTypeComponentOnlineaccess onlineaccess
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
        [DefaultValue(StructdefTypeComponentOfflineaccess.readwrite)]
        public StructdefTypeComponentOfflineaccess offlineaccess
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

        public StructdefTypeComponent()
        {
            onlineaccessField = StructdefTypeComponentOnlineaccess.readwrite;
            offlineaccessField = StructdefTypeComponentOfflineaccess.readwrite;
        }
    }
}
