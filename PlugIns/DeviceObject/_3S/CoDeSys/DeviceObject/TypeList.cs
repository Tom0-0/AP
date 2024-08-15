using System;
using System.Collections.Generic;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{c7b9b7d0-843f-41bd-b2d1-138bde58cf35}")]
	[StorageVersion("3.5.6.0")]
	public class TypeList : GenericObject2, ITypeList
	{
		private IDeviceIdentification _devId;

		[DefaultSerialization("Types")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication((DuplicationMethod)1)]
		[StorageDefaultValue(null)]
		private LDictionary<string, ITypeDefinition> _dictTypes;

		public IDeviceIdentification DeviceIdentification => _devId;

		internal LDictionary<string, ITypeDefinition> TypeMap
		{
			get
			{
				if (_dictTypes != null)
				{
					return _dictTypes;
				}
				return new LDictionary<string, ITypeDefinition>();
			}
			set
			{
				_dictTypes = value;
			}
		}

		public IDictionary<string, ITypeDefinition> Types => (IDictionary<string, ITypeDefinition>)TypeMap;

		public TypeList()
			: base()
		{
		}

		public void ReadTypes(XmlReader reader, IDeviceIdentification devId)
		{
			if (reader.IsEmptyElement)
			{
				return;
			}
			_devId = devId;
			string attribute = reader.GetAttribute("namespace");
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					TypeDefinition typeDefinition = null;
					switch (reader.Name)
					{
					case "StructType":
					{
						XmlDocument xmlDocument3 = new XmlDocument();
						xmlDocument3.LoadXml(reader.ReadOuterXml());
						typeDefinition = new StructType(xmlDocument3.DocumentElement, this);
						break;
					}
					case "UnionType":
					{
						XmlDocument xmlDocument2 = new XmlDocument();
						xmlDocument2.LoadXml(reader.ReadOuterXml());
						typeDefinition = new UnionType(xmlDocument2.DocumentElement, this);
						break;
					}
					case "EnumType":
						typeDefinition = new EnumType(reader, this);
						break;
					case "RangeType":
						typeDefinition = new RangeType(reader, this);
						break;
					case "BitfieldType":
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(reader.ReadOuterXml());
						typeDefinition = new BitfieldType(xmlDocument.DocumentElement, this);
						break;
					}
					case "ArrayType":
						typeDefinition = new ArrayType(reader, this);
						break;
					default:
						reader.Skip();
						break;
					}
					if (typeDefinition != null)
					{
						if (_dictTypes == null)
						{
							_dictTypes = new LDictionary<string, ITypeDefinition>();
						}
						_dictTypes[attribute + ":" + typeDefinition.Name]= (ITypeDefinition)(object)typeDefinition;
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.Read();
		}

		public TypeDefinition GetTypeDefinition(string stName)
		{
			ITypeDefinition val = null;
			if (_dictTypes != null)
			{
				_dictTypes.TryGetValue(stName, out val);
			}
			return val as TypeDefinition;
		}

		public override void AfterDeserialize()
		{
			base.AfterDeserialize();
			if (this._dictTypes != null)
			{
				foreach (string text in this._dictTypes.Keys)
				{
					string[] array = text.Split(new char[]
					{
						':'
					});
					if (array.Length > 1)
					{
						TypeDefinition typeDefinition = this._dictTypes[text] as TypeDefinition;
						typeDefinition.SetTypeList(this, array[1]);
						typeDefinition.CreatedType = true;
					}
				}
			}
		}

		public ITypeStructureUnion AddStructureType(string stNamespace, string stIdentifier)
		{
			StructType structType = new StructType();
			structType.SetTypeList(this, stIdentifier);
			structType.CreatedType = true;
			if (_dictTypes == null)
			{
				_dictTypes = new LDictionary<string, ITypeDefinition>();
			}
			_dictTypes[stNamespace + ":" + structType.Name]= (ITypeDefinition)(object)structType;
			return (ITypeStructureUnion)(object)structType;
		}

		public ITypeStructureUnion AddUnionType(string stNamespace, string stIdentifier)
		{
			UnionType unionType = new UnionType();
			unionType.SetTypeList(this, stIdentifier);
			unionType.CreatedType = true;
			if (_dictTypes == null)
			{
				_dictTypes = new LDictionary<string, ITypeDefinition>();
			}
			_dictTypes[stNamespace + ":" + unionType.Name]= (ITypeDefinition)(object)unionType;
			return (ITypeStructureUnion)(object)unionType;
		}

		public ITypeBitField AddBitfieldType(string stNamespace, string stIdentifier, string stBasetype)
		{
			BitfieldType bitfieldType = new BitfieldType();
			bitfieldType.SetTypeList(this, stIdentifier);
			bitfieldType.BaseType = stBasetype;
			bitfieldType.CreatedType = true;
			if (_dictTypes == null)
			{
				_dictTypes = new LDictionary<string, ITypeDefinition>();
			}
			_dictTypes[stNamespace + ":" + bitfieldType.Name]= (ITypeDefinition)(object)bitfieldType;
			return (ITypeBitField)(object)bitfieldType;
		}

		public ITypeRange AddRangeType(string stNamespace, string stIdentifier, string stBasetype)
		{
			RangeType rangeType = new RangeType();
			rangeType.SetTypeList(this, stIdentifier);
			rangeType.BaseType = stBasetype;
			rangeType.CreatedType = true;
			if (_dictTypes == null)
			{
				_dictTypes = new LDictionary<string, ITypeDefinition>();
			}
			_dictTypes[stNamespace + ":" + rangeType.Name]= (ITypeDefinition)(object)rangeType;
			return (ITypeRange)(object)rangeType;
		}

		public ITypeArray AddArrayType(string stNamespace, string stIdentifier, string stBasetype)
		{
			ArrayType arrayType = new ArrayType();
			arrayType.SetTypeList(this, stIdentifier);
			arrayType.BaseType = stBasetype;
			arrayType.CreatedType = true;
			if (_dictTypes == null)
			{
				_dictTypes = new LDictionary<string, ITypeDefinition>();
			}
			_dictTypes[stNamespace + ":" + arrayType.Name]= (ITypeDefinition)(object)arrayType;
			return (ITypeArray)(object)arrayType;
		}

		public ITypeEnum AddEnumType(string stNamespace, string stIdentifier, string stBasetype)
		{
			EnumType enumType = new EnumType();
			enumType.SetTypeList(this, stIdentifier);
			enumType.BaseType = stBasetype;
			enumType.CreatedType = true;
			if (_dictTypes == null)
			{
				_dictTypes = new LDictionary<string, ITypeDefinition>();
			}
			_dictTypes[stNamespace + ":" + enumType.Name]= (ITypeDefinition)(object)enumType;
			return (ITypeEnum)(object)enumType;
		}
	}
}
