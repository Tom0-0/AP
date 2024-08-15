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
    public class DeviceDescriptionDeviceAttribute
    {
        private string nameField;

        private string valueField;

        [XmlAttribute(DataType = "NMTOKEN")]
        public string name
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

        [XmlText]
        public string Value
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
    }
}
