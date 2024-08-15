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
