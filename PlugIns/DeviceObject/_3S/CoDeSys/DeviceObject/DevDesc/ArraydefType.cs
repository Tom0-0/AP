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
    [XmlType(Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
    public class ArraydefType
    {
        private ArraydefTypeFirstDimension firstDimensionField;

        private DimensionType secondDimensionField;

        private DimensionType thirdDimensionField;

        private XmlElement[] anyField;

        private string basetypeField;

        private string nameField;

        public ArraydefTypeFirstDimension FirstDimension
        {
            get
            {
                return firstDimensionField;
            }
            set
            {
                firstDimensionField = value;
            }
        }

        public DimensionType SecondDimension
        {
            get
            {
                return secondDimensionField;
            }
            set
            {
                secondDimensionField = value;
            }
        }

        public DimensionType ThirdDimension
        {
            get
            {
                return thirdDimensionField;
            }
            set
            {
                thirdDimensionField = value;
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

        [XmlAttribute(DataType = "NMTOKEN")]
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
