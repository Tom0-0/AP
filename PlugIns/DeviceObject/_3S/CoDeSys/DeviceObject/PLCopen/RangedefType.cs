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
	public class RangedefType : TypedefType
	{
		private string minField;

		private string maxField;

		private string defaultField;

		private string basetypeField;

		public string Min
		{
			get
			{
				return minField;
			}
			set
			{
				minField = value;
			}
		}

		public string Max
		{
			get
			{
				return maxField;
			}
			set
			{
				maxField = value;
			}
		}

		public string Default
		{
			get
			{
				return defaultField;
			}
			set
			{
				defaultField = value;
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
	}
}
