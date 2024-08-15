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
	public class DeviceDescriptionDeviceDeviceParameterSet : ParameterListType
	{
		private StringRefType editorNameField;

		private bool downloadParamsDevDescOrderField;

		private string fixedInputAddressField;

		private string fixedOutputAddressField;

		public StringRefType EditorName
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

		[XmlAttribute]
		[DefaultValue(false)]
		public bool downloadParamsDevDescOrder
		{
			get
			{
				return downloadParamsDevDescOrderField;
			}
			set
			{
				downloadParamsDevDescOrderField = value;
			}
		}

		[XmlAttribute]
		public string fixedInputAddress
		{
			get
			{
				return fixedInputAddressField;
			}
			set
			{
				fixedInputAddressField = value;
			}
		}

		[XmlAttribute]
		public string fixedOutputAddress
		{
			get
			{
				return fixedOutputAddressField;
			}
			set
			{
				fixedOutputAddressField = value;
			}
		}

		public DeviceDescriptionDeviceDeviceParameterSet()
		{
			downloadParamsDevDescOrderField = false;
		}
	}
}
