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
    public class ConnectorTypeConstraint : DeviceRefType
    {
        private uint maxNumberField;

        private bool checkRecursiveField;

        [XmlAttribute]
        public uint MaxNumber
        {
            get
            {
                return maxNumberField;
            }
            set
            {
                maxNumberField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool CheckRecursive
        {
            get
            {
                return checkRecursiveField;
            }
            set
            {
                checkRecursiveField = value;
            }
        }

        public ConnectorTypeConstraint()
        {
            checkRecursiveField = false;
        }
    }
}
