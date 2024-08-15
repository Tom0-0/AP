using System.Collections.Generic;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{14F385F5-DDD2-452A-A093-499275FA05DE}")]
	[StorageVersion("3.5.6.0")]
	public class EnumType : TypeDefinition, ITypeEnum, ITypeDefinition
	{
		[DefaultSerialization("Values")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private LList<ITypeEnumComponent> _liComponents;

		[DefaultSerialization("BaseType")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private string _stBaseType;

		public LList<ITypeEnumComponent> Values => _liComponents;

		public string BaseType
		{
			get
			{
				return _stBaseType;
			}
			set
			{
				_stBaseType = value;
			}
		}

		public IList<ITypeEnumComponent> TypeComponents => (IList<ITypeEnumComponent>)_liComponents;

		public EnumType()
		{
		}

		public EnumType(XmlReader reader, TypeList typeList)
			: base(typeList, reader)
		{
			if (reader.IsEmptyElement)
			{
				return;
			}
			_liComponents = new LList<ITypeEnumComponent>();
			_stBaseType = reader.GetAttribute("basetype");
			if (string.IsNullOrEmpty(_stBaseType))
			{
				_stBaseType = "std:INT";
			}
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.Name;
					if (name == "Enum")
					{
						EnumTypeValue enumTypeValue = new EnumTypeValue(reader, typeList);
						_liComponents.Add((ITypeEnumComponent)(object)enumTypeValue);
					}
					else
					{
						reader.Skip();
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.Read();
		}

		public ITypeEnumComponent AddEnum(string stIdentifier, IStringRef visibleName, string stValue)
		{
			EnumTypeValue enumTypeValue = new EnumTypeValue();
			enumTypeValue.Identifier = stIdentifier;
			enumTypeValue.Value = stValue;
			((ITypeEnumComponent)enumTypeValue).Name=(visibleName);
			if (_liComponents == null)
			{
				_liComponents = new LList<ITypeEnumComponent>();
			}
			_liComponents.Add((ITypeEnumComponent)(object)enumTypeValue);
			return (ITypeEnumComponent)(object)enumTypeValue;
		}
	}
}
