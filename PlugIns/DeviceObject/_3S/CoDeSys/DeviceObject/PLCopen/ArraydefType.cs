using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.3038")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class ArraydefType : TypedefType
	{
		private DimensionType firstDimensionField;

		private DimensionType secondDimensionField;

		private DimensionType thirdDimensionField;

		private XmlElement[] anyField;

		private string basetypeField;

		public DimensionType FirstDimension
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
	}
}
