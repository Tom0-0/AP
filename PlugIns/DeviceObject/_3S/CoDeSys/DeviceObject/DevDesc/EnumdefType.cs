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
    public class EnumdefType : TypedefType
    {
        private EnumdefTypeEnum[] enumField;

        private string basetypeField;

        [XmlElement("Enum")]
        public EnumdefTypeEnum[] Enum
        {
            get
            {
                return enumField;
            }
            set
            {
                enumField = value;
            }
        }

        [XmlAttribute(DataType = "NMTOKEN")]
        [DefaultValue("std.INT")]
        public string basetype
        {
            get
            {
                return basetypeField;
            }
            set
            {
                basetypeField = value;
            }
        }

        public EnumdefType()
        {
            basetypeField = "std.INT";
        }
    }
}
