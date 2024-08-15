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
    public class DriverInfoTypeRequiredExternalEventTask
    {
        private string[] taskpouField;

        private string tasknameField;

        private string eventField;

        private string cycletimeField;

        private bool implicitField;

        private bool withinspstimeslicingField;

        private bool watchdogEnabledField;

        private string watchdogTimeField;

        private string watchdogTimeUnitField;

        private string watchdogSensitivityField;

        [XmlElement("taskpou")]
        public string[] taskpou
        {
            get
            {
                return taskpouField;
            }
            set
            {
                taskpouField = value;
            }
        }

        [XmlAttribute]
        public string taskname
        {
            get
            {
                return tasknameField;
            }
            set
            {
                tasknameField = value;
            }
        }

        [XmlAttribute]
        public string @event
        {
            get
            {
                return eventField;
            }
            set
            {
                eventField = value;
            }
        }

        [XmlAttribute]
        public string cycletime
        {
            get
            {
                return cycletimeField;
            }
            set
            {
                cycletimeField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool @implicit
        {
            get
            {
                return implicitField;
            }
            set
            {
                implicitField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(true)]
        public bool withinspstimeslicing
        {
            get
            {
                return withinspstimeslicingField;
            }
            set
            {
                withinspstimeslicingField = value;
            }
        }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool watchdogEnabled
        {
            get
            {
                return watchdogEnabledField;
            }
            set
            {
                watchdogEnabledField = value;
            }
        }

        [XmlAttribute]
        public string watchdogTime
        {
            get
            {
                return watchdogTimeField;
            }
            set
            {
                watchdogTimeField = value;
            }
        }

        [XmlAttribute]
        public string watchdogTimeUnit
        {
            get
            {
                return watchdogTimeUnitField;
            }
            set
            {
                watchdogTimeUnitField = value;
            }
        }

        [XmlAttribute]
        public string watchdogSensitivity
        {
            get
            {
                return watchdogSensitivityField;
            }
            set
            {
                watchdogSensitivityField = value;
            }
        }

        public DriverInfoTypeRequiredExternalEventTask()
        {
            implicitField = false;
            withinspstimeslicingField = true;
            watchdogEnabledField = false;
        }
    }
}
