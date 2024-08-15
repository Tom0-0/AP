using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{5F7CD0AA-E287-4730-A875-DC2A483D8D61}")]
	[StorageVersion("3.5.6.0")]
	public class ArrayTypeBorder : GenericObject2, ITypeArrayBorders
	{
		[DefaultSerialization("LowerBorder")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private string _stLowerBorder;

		[DefaultSerialization("UpperBorder")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private string _stUpperBorder;

		public string LowerBorder
		{
			get
			{
				return _stLowerBorder;
			}
			set
			{
				_stLowerBorder = value;
			}
		}

		public string UpperBorder
		{
			get
			{
				return _stUpperBorder;
			}
			set
			{
				_stUpperBorder = value;
			}
		}

		public ArrayTypeBorder()
			: this()
		{
		}

		public ArrayTypeBorder(XmlReader reader, TypeList typeList)
			: this()
		{
			if (reader.IsEmptyElement)
			{
				return;
			}
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.Name;
					if (!(name == "LowerBorder"))
					{
						if (name == "UpperBorder")
						{
							_stUpperBorder = reader.ReadElementString();
						}
						else
						{
							reader.Skip();
						}
					}
					else
					{
						_stLowerBorder = reader.ReadElementString();
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.Read();
		}

		public ArrayTypeBorder(string lowerBorder, string upperBorder)
			: this()
		{
			_stLowerBorder = lowerBorder;
			_stUpperBorder = upperBorder;
		}
	}
}
