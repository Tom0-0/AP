using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
    [Serializable]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class DimensionType
    {
        private int lowerBorderField;

        private int upperBorderField;

        public int LowerBorder
        {
            get
            {
                return lowerBorderField;
            }
            set
            {
                lowerBorderField = value;
            }
        }

        public int UpperBorder
        {
            get
            {
                return upperBorderField;
            }
            set
            {
                upperBorderField = value;
            }
        }
    }
}
