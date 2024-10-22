using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{7C60CB9A-E1CD-4631-A77F-6E8B48601074}")]
	[StorageVersion("3.5.6.0")]
	public class RangeType : TypeDefinition, ITypeRange, ITypeDefinition
	{
		[DefaultSerialization("BaseType")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private string _stBaseType;

		[DefaultSerialization("Min")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("0")]
		private string _stMin = "0";

		[DefaultSerialization("Max")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("0")]
		private string _stMax = "0";

		[DefaultSerialization("Default")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("0")]
		private string _stDefault = "0";

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

		public string Min
		{
			get
			{
				return _stMin;
			}
			set
			{
				_stMin = value;
			}
		}

		public string Max
		{
			get
			{
				return _stMax;
			}
			set
			{
				_stMax = value;
			}
		}

		public string Default
		{
			get
			{
				return _stDefault;
			}
			set
			{
				_stDefault = value;
			}
		}

		public RangeType()
		{
		}

		public RangeType(XmlReader reader, TypeList typeList)
			: base(typeList, reader)
		{
			if (reader.IsEmptyElement)
			{
				return;
			}
			_stBaseType = reader.GetAttribute("basetype");
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
					case "Min":
						_stMin = reader.ReadElementString();
						break;
					case "Max":
						_stMax = reader.ReadElementString();
						break;
					case "Default":
						_stDefault = reader.ReadElementString();
						break;
					default:
						reader.Skip();
						break;
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.Read();
		}
	}
}
