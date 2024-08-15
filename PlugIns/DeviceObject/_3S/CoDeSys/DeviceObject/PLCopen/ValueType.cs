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
	public class ValueType
	{
		private ValueTypeElement[] elementField;

		private CustomType customField;

		private string[] textField;

		private string nameField;

		private string visiblenameField;

		private AccessRightType offlineaccessField;

		private AccessRightType onlineaccessField;

		private string descField;

		[XmlElement("Element")]
		public ValueTypeElement[] Element
		{
			get
			{
				return elementField;
			}
			set
			{
				elementField = value;
			}
		}

		public CustomType Custom
		{
			get
			{
				return customField;
			}
			set
			{
				customField = value;
			}
		}

		[XmlText]
		public string[] Text
		{
			get
			{
				return textField;
			}
			set
			{
				textField = value;
			}
		}

		[XmlAttribute(DataType = "NCName")]
		public string name
		{
			get
			{
				return nameField;
			}
			set
			{
				nameField = value;
			}
		}

		[XmlAttribute]
		public string visiblename
		{
			get
			{
				return visiblenameField;
			}
			set
			{
				visiblenameField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(AccessRightType.readwrite)]
		public AccessRightType offlineaccess
		{
			get
			{
				return offlineaccessField;
			}
			set
			{
				offlineaccessField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(AccessRightType.readwrite)]
		public AccessRightType onlineaccess
		{
			get
			{
				return onlineaccessField;
			}
			set
			{
				onlineaccessField = value;
			}
		}

		[XmlAttribute]
		public string desc
		{
			get
			{
				return descField;
			}
			set
			{
				descField = value;
			}
		}

		public ValueType()
		{
			offlineaccessField = AccessRightType.readwrite;
			onlineaccessField = AccessRightType.readwrite;
		}
	}
}
