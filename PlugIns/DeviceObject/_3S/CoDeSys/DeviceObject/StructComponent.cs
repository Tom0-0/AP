using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{d5ab7fee-aed9-42c7-a744-05883206a781}")]
	[StorageVersion("3.5.6.0")]
	public class StructComponent : GenericObject2, ITypeComponent
	{
		[DefaultSerialization("Type")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private string _stType;

		[DefaultSerialization("Identifier")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private string _stIdentifier;

		[DefaultSerialization("Default")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private ValueElement _default;

		[DefaultSerialization("VisibleName")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private IStringRef _visibleName;

		[DefaultSerialization("Description")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private IStringRef _description;

		[DefaultSerialization("Unit")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private IStringRef _unit;

		[DefaultSerialization("CustomItems")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private CustomItemList _customItems;

		[DefaultSerialization("AccessRightOffline")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(/*Could not decode attribute arguments.*/)]
		private AccessRight _accessRightOffline = (AccessRight)3;

		[DefaultSerialization("AccessRightOnline")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(/*Could not decode attribute arguments.*/)]
		private AccessRight _accessRightOnline = (AccessRight)3;

		public string Identifier
		{
			get
			{
				return _stIdentifier;
			}
			set
			{
				_stIdentifier = value;
			}
		}

		public string Type
		{
			get
			{
				return _stType;
			}
			set
			{
				_stType = value;
			}
		}

		internal ValueElement Default => _default;

		internal StringRef VisibleName
		{
			get
			{
				return _visibleName as StringRef;
			}
			set
			{
				_visibleName = (IStringRef)(object)value;
			}
		}

		internal StringRef Description => _description as StringRef;

		internal StringRef Unit => _unit as StringRef;

		internal CustomItemList CustomItems => _customItems;

		public string DefaultValue
		{
			get
			{
				return _default.Value;
			}
			set
			{
				if (_default == null)
				{
					_default = new ValueElement(value);
				}
				else
				{
					_default.Value = value;
				}
			}
		}

		public AccessRight OfflineAccess
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _accessRightOffline;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_accessRightOffline = value;
			}
		}

		public AccessRight OnlineAccess
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _accessRightOnline;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_accessRightOnline = value;
			}
		}

		IStringRef VisibleName
		{
			get
			{
				return _visibleName;
			}
			set
			{
				_visibleName = value;
			}
		}

		IStringRef Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		IStringRef Unit
		{
			get
			{
				return _unit;
			}
			set
			{
				_unit = value;
			}
		}

		public StructComponent()
			: this()
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)


		public StructComponent(XmlElement xeNode, TypeList typeList)
			: this()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			_visibleName = (IStringRef)(object)new StringRef();
			_description = (IStringRef)(object)new StringRef();
			_unit = (IStringRef)(object)new StringRef();
			_customItems = new CustomItemList();
			_stIdentifier = xeNode.GetAttribute("identifier");
			_stType = xeNode.GetAttribute("type");
			string attribute = xeNode.GetAttribute("onlineaccess");
			_accessRightOnline = Parameter.ParseAccessRight(attribute, _accessRightOnline);
			attribute = xeNode.GetAttribute("offlineaccess");
			_accessRightOffline = Parameter.ParseAccessRight(attribute, _accessRightOffline);
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)childNode;
					switch (xmlElement.Name)
					{
					case "Default":
						_default = new ValueElement(xmlElement, typeList, null);
						break;
					case "VisibleName":
						_visibleName = (IStringRef)(object)ParameterDataCache.AddStringRef(new StringRef(xmlElement));
						break;
					case "Description":
						_description = (IStringRef)(object)ParameterDataCache.AddStringRef(new StringRef(xmlElement));
						break;
					case "Unit":
						_unit = (IStringRef)(object)ParameterDataCache.AddStringRef(new StringRef(xmlElement));
						break;
					case "Custom":
						_customItems = new CustomItemList(xmlElement);
						break;
					}
				}
			}
		}

		internal StructComponent(string identifier, string type)
			: this()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			_stIdentifier = identifier;
			_stType = type;
			_default = new ValueElement();
			_visibleName = (IStringRef)(object)new StringRef();
			_description = (IStringRef)(object)new StringRef();
			_unit = (IStringRef)(object)new StringRef();
			_customItems = new CustomItemList();
		}

		internal StructComponent(string identifier, string type, ValueElement defaultValue, AccessRight accessRightOffline, AccessRight accessRightOnline)
			: this()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			_stIdentifier = identifier;
			_stType = type;
			_default = defaultValue;
			_accessRightOffline = accessRightOffline;
			_accessRightOnline = accessRightOnline;
			_visibleName = (IStringRef)(object)new StringRef();
			_description = (IStringRef)(object)new StringRef();
			_unit = (IStringRef)(object)new StringRef();
			_customItems = new CustomItemList();
		}

		internal AccessRight GetAccessRight(bool bOnline)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			if (bOnline)
			{
				return _accessRightOnline;
			}
			return _accessRightOffline;
		}
	}
}
