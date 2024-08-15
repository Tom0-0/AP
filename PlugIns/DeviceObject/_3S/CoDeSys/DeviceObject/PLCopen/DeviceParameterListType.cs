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
    public class DeviceParameterListType : ParameterListType
    {
        private string editorNameField;

        public string EditorName
        {
            get
            {
                return editorNameField;
            }
            set
            {
                editorNameField = value;
            }
        }
    }
}
