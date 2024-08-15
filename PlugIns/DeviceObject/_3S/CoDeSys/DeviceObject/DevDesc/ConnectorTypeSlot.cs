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
	public class ConnectorTypeSlot
	{
		private ConnectorTypeSlotDefaultModule[] defaultModuleField;

		private string[] slotNameField;

		private int countField;

		private bool allowEmptyField;

		private bool subdevicesCollapsedField;

		private uint startSlotNumberField;

		private bool startSlotNumberFieldSpecified;

		private string hiddenSlotField;

		[XmlElement("DefaultModule")]
		public ConnectorTypeSlotDefaultModule[] DefaultModule
		{
			get
			{
				return defaultModuleField;
			}
			set
			{
				defaultModuleField = value;
			}
		}

		[XmlElement("SlotName")]
		public string[] SlotName
		{
			get
			{
				return slotNameField;
			}
			set
			{
				slotNameField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(1)]
		public int count
		{
			get
			{
				return countField;
			}
			set
			{
				countField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(true)]
		public bool allowEmpty
		{
			get
			{
				return allowEmptyField;
			}
			set
			{
				allowEmptyField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool subdevicesCollapsed
		{
			get
			{
				return subdevicesCollapsedField;
			}
			set
			{
				subdevicesCollapsedField = value;
			}
		}

		[XmlAttribute]
		public uint startSlotNumber
		{
			get
			{
				return startSlotNumberField;
			}
			set
			{
				startSlotNumberField = value;
			}
		}

		[XmlIgnore]
		public bool startSlotNumberSpecified
		{
			get
			{
				return startSlotNumberFieldSpecified;
			}
			set
			{
				startSlotNumberFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string hiddenSlot
		{
			get
			{
				return hiddenSlotField;
			}
			set
			{
				hiddenSlotField = value;
			}
		}

		public ConnectorTypeSlot()
		{
			countField = 1;
			allowEmptyField = true;
			subdevicesCollapsedField = false;
		}
	}
}
