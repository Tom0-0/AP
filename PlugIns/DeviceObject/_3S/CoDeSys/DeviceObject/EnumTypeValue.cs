using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{5FF413DF-BA7B-469E-9123-4B8553B1F5C2}")]
	[StorageVersion("3.5.6.0")]
	public class EnumTypeValue : GenericObject2, ITypeEnumComponent
	{
		[DefaultSerialization("Value")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue("")]
		private string _stValue = "";

		[DefaultSerialization("Identifier")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue("")]
		private string _stIdentifier = "";

		[DefaultSerialization("Name")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private IStringRef _name;

		[DefaultSerialization("Description")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private IStringRef _description;

		public string Value
		{
			get
			{
				return _stValue;
			}
			set
			{
				_stValue = value;
			}
		}

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

		internal StringRef Name => _name as StringRef;

		internal StringRef Description => _description as StringRef;

		IStringRef Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
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

		public EnumTypeValue()
			: this()
		{
		}

		public EnumTypeValue(XmlReader reader, TypeList typeList)
			: this()
		{
			if (reader.IsEmptyElement)
			{
				return;
			}
			_name = (IStringRef)(object)new StringRef();
			_description = (IStringRef)(object)new StringRef();
			_stIdentifier = reader.GetAttribute("identifier");
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
					case "Value":
						_stValue = reader.ReadElementString();
						break;
					case "VisibleName":
						_name = (IStringRef)(object)ParameterDataCache.AddStringRef(new StringRef(reader));
						break;
					case "Description":
						_description = (IStringRef)(object)ParameterDataCache.AddStringRef(new StringRef(reader));
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
