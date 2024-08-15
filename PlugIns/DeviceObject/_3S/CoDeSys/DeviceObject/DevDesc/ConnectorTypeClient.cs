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
    public class ConnectorTypeClient
    {
        private string nameField;

        private uint maxinputsizeField;

        private bool maxinputsizeFieldSpecified;

        private uint maxoutputsizeField;

        private bool maxoutputsizeFieldSpecified;

        private uint maxinoutputsizeField;

        private bool maxinoutputsizeFieldSpecified;

        private string clienttypeguidField;

        private string clientconnectorinterfaceField;

        private int clientconnectoridField;

        [XmlAttribute]
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

        [XmlAttribute]
        public uint maxinputsize
        {
            get
            {
                return maxinputsizeField;
            }
            set
            {
                maxinputsizeField = value;
            }
        }

        [XmlIgnore]
        public bool maxinputsizeSpecified
        {
            get
            {
                return maxinputsizeFieldSpecified;
            }
            set
            {
                maxinputsizeFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public uint maxoutputsize
        {
            get
            {
                return maxoutputsizeField;
            }
            set
            {
                maxoutputsizeField = value;
            }
        }

        [XmlIgnore]
        public bool maxoutputsizeSpecified
        {
            get
            {
                return maxoutputsizeFieldSpecified;
            }
            set
            {
                maxoutputsizeFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public uint maxinoutputsize
        {
            get
            {
                return maxinoutputsizeField;
            }
            set
            {
                maxinoutputsizeField = value;
            }
        }

        [XmlIgnore]
        public bool maxinoutputsizeSpecified
        {
            get
            {
                return maxinoutputsizeFieldSpecified;
            }
            set
            {
                maxinoutputsizeFieldSpecified = value;
            }
        }

        [XmlAttribute]
        public string clienttypeguid
        {
            get
            {
                return clienttypeguidField;
            }
            set
            {
                clienttypeguidField = value;
            }
        }

        [XmlAttribute]
        public string clientconnectorinterface
        {
            get
            {
                return clientconnectorinterfaceField;
            }
            set
            {
                clientconnectorinterfaceField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(-1)]
        public int clientconnectorid
        {
            get
            {
                return clientconnectoridField;
            }
            set
            {
                clientconnectoridField = value;
            }
        }

        public ConnectorTypeClient()
        {
            clientconnectoridField = -1;
        }
    }
}
