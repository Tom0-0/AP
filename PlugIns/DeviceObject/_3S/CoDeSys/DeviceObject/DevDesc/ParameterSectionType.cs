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
    public class ParameterSectionType
    {
        private StringRefType nameField;

        private StringRefType descriptionField;

        private object[] itemsField;

        private CustomType customField;

        public StringRefType Name
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

        [XmlElement("Parameter", typeof(ParameterType))]
        [XmlElement("ParameterSection", typeof(ParameterSectionType))]
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
    }
}
