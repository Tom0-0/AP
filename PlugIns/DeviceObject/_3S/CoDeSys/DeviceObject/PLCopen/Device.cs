using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "", IsNullable = false)]
	public class Device
	{
		private DeviceType deviceTypeField;

		private DeviceTypes typesField;

		private int fixedIndexField;

		public DeviceType DeviceType
		{
			get
			{
				return deviceTypeField;
			}
			set
			{
				deviceTypeField = value;
			}
		}

		public DeviceTypes Types
		{
			get
			{
				return typesField;
			}
			set
			{
				typesField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(-1)]
		public int fixedIndex
		{
			get
			{
				return fixedIndexField;
			}
			set
			{
				fixedIndexField = value;
			}
		}

		public Device()
		{
			fixedIndexField = -1;
		}
	}
}
