using System;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{4C6B6EFB-8A8B-449B-ABC7-AC6C7997B407}")]
	[StorageVersion("3.5.6.0")]
	public class ArrayType : TypeDefinition, ITypeArray, ITypeDefinition
	{
		[DefaultSerialization("FirstDimension")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private ArrayTypeBorder _firstDimension;

		[DefaultSerialization("SecondDimension")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private ArrayTypeBorder _secondDimension;

		[DefaultSerialization("ThirdDimension")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private ArrayTypeBorder _thirdDimension;

		[DefaultSerialization("Dimensions")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(0)]
		private int _iDimensions;

		[DefaultSerialization("BaseType")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(/*Could not decode attribute arguments.*/)]
		[StorageDefaultValue(null)]
		private string _stBaseType;

		public int Dimensions
		{
			get
			{
				return _iDimensions;
			}
			set
			{
				if (value < 1 || value > 3)
				{
					throw new ArgumentException("dimension must be within 1 to 3");
				}
				_iDimensions = value;
				if (_iDimensions > 0 && _firstDimension == null)
				{
					_firstDimension = new ArrayTypeBorder();
				}
				if (_iDimensions > 1 && _secondDimension == null)
				{
					_secondDimension = new ArrayTypeBorder();
				}
				if (_iDimensions > 2 && _thirdDimension == null)
				{
					_thirdDimension = new ArrayTypeBorder();
				}
			}
		}

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

		public ITypeArrayBorders FirstDimension => (ITypeArrayBorders)(object)_firstDimension;

		public ITypeArrayBorders SecondDimension => (ITypeArrayBorders)(object)_secondDimension;

		public ITypeArrayBorders ThirdDimension => (ITypeArrayBorders)(object)_thirdDimension;

		public ArrayType()
		{
		}

		public ArrayType(XmlReader reader, TypeList typeList)
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
					case "FirstDimension":
						_iDimensions++;
						_firstDimension = new ArrayTypeBorder(reader, typeList);
						break;
					case "SecondDimension":
						_iDimensions++;
						_secondDimension = new ArrayTypeBorder(reader, typeList);
						break;
					case "ThirdDimension":
						_iDimensions++;
						_thirdDimension = new ArrayTypeBorder(reader, typeList);
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
