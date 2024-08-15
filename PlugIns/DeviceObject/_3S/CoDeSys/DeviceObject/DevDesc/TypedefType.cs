using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
    [Serializable]
    [XmlInclude(typeof(RangedefType))]
    [XmlInclude(typeof(StructdefType))]
    [XmlInclude(typeof(EnumdefType))]
    [XmlInclude(typeof(BitfielddefType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
    public class TypedefType
    {
        private string nameField;

        [XmlAttribute(DataType = "NCName")]
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
    }
}
