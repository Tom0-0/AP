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
	public class DriverInfoType
	{
		private DriverInfoTypeRequiredCyclicTask[] requiredCyclicTaskField;

		private DriverInfoTypeRequiredExternalEventTask[] requiredExternalEventTaskField;

		private DriverInfoTypeRequiredLib[] requiredLibField;

		private DriverInfoTypeScan scanField;

		private DriverInfoTypeRequiredLibEx[] requiredLibExField;

		private bool needsBusCycleField;

		private StopResetBehaviourType stopResetBehaviourField;

		private bool stopResetBehaviourFieldSpecified;

		private string stopResetBehaviourUserProgramField;

		private bool updateIosInStopField;

		private string defaultBusCycleTaskField;

		private bool useSlowestTaskField;

		private bool needsBusCycleBeforeReadInputsField;

		private bool enableDiagnosisField;

		private DriverInfoTypeCreateAdditionalParameters createAdditionalParametersField;

		private DriverInfoTypeDiagnosisCheckbox diagnosisCheckboxField;

		[XmlElement("RequiredCyclicTask")]
		public DriverInfoTypeRequiredCyclicTask[] RequiredCyclicTask
		{
			get
			{
				return requiredCyclicTaskField;
			}
			set
			{
				requiredCyclicTaskField = value;
			}
		}

		[XmlElement("RequiredExternalEventTask")]
		public DriverInfoTypeRequiredExternalEventTask[] RequiredExternalEventTask
		{
			get
			{
				return requiredExternalEventTaskField;
			}
			set
			{
				requiredExternalEventTaskField = value;
			}
		}

		[XmlElement("RequiredLib")]
		public DriverInfoTypeRequiredLib[] RequiredLib
		{
			get
			{
				return requiredLibField;
			}
			set
			{
				requiredLibField = value;
			}
		}

		public DriverInfoTypeScan Scan
		{
			get
			{
				return scanField;
			}
			set
			{
				scanField = value;
			}
		}

		[XmlElement("RequiredLibEx")]
		public DriverInfoTypeRequiredLibEx[] RequiredLibEx
		{
			get
			{
				return requiredLibExField;
			}
			set
			{
				requiredLibExField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool needsBusCycle
		{
			get
			{
				return needsBusCycleField;
			}
			set
			{
				needsBusCycleField = value;
			}
		}

		[XmlAttribute]
		public StopResetBehaviourType StopResetBehaviour
		{
			get
			{
				return stopResetBehaviourField;
			}
			set
			{
				stopResetBehaviourField = value;
			}
		}

		[XmlIgnore]
		public bool StopResetBehaviourSpecified
		{
			get
			{
				return stopResetBehaviourFieldSpecified;
			}
			set
			{
				stopResetBehaviourFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string StopResetBehaviourUserProgram
		{
			get
			{
				return stopResetBehaviourUserProgramField;
			}
			set
			{
				stopResetBehaviourUserProgramField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool UpdateIosInStop
		{
			get
			{
				return updateIosInStopField;
			}
			set
			{
				updateIosInStopField = value;
			}
		}

		[XmlAttribute]
		public string defaultBusCycleTask
		{
			get
			{
				return defaultBusCycleTaskField;
			}
			set
			{
				defaultBusCycleTaskField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool useSlowestTask
		{
			get
			{
				return useSlowestTaskField;
			}
			set
			{
				useSlowestTaskField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool needsBusCycleBeforeReadInputs
		{
			get
			{
				return needsBusCycleBeforeReadInputsField;
			}
			set
			{
				needsBusCycleBeforeReadInputsField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool enableDiagnosis
		{
			get
			{
				return enableDiagnosisField;
			}
			set
			{
				enableDiagnosisField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(DriverInfoTypeCreateAdditionalParameters.Hidden)]
		public DriverInfoTypeCreateAdditionalParameters createAdditionalParameters
		{
			get
			{
				return createAdditionalParametersField;
			}
			set
			{
				createAdditionalParametersField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(DriverInfoTypeDiagnosisCheckbox.Enabled)]
		public DriverInfoTypeDiagnosisCheckbox diagnosisCheckbox
		{
			get
			{
				return diagnosisCheckboxField;
			}
			set
			{
				diagnosisCheckboxField = value;
			}
		}

		public DriverInfoType()
		{
			needsBusCycleField = false;
			updateIosInStopField = false;
			useSlowestTaskField = false;
			needsBusCycleBeforeReadInputsField = false;
			enableDiagnosisField = false;
			createAdditionalParametersField = DriverInfoTypeCreateAdditionalParameters.Hidden;
			diagnosisCheckboxField = DriverInfoTypeDiagnosisCheckbox.Enabled;
		}
	}
}
