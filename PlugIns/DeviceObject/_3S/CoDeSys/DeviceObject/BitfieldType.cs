using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{EE0F00BA-4CFA-4C1D-A1AC-F72BF89031B6}")]
	[StorageVersion("3.5.6.0")]
	public class BitfieldType : TypeDefinition, ITypeBitField, ITypeDefinition
	{
		[DefaultSerialization("BaseType")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue("")]
		private string _stBaseType = "";

		[DefaultSerialization("Components")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private LList<ITypeComponent> _liComponents;

		public IList Components => (IList)_liComponents;

		public string BaseType
		{
			get
			{
				return _stBaseType;
			}
			set
			{
				_stBaseType = value;
				if (_stBaseType == null)
				{
					_stBaseType = "";
					return;
				}
				_stBaseType = _stBaseType.Trim();
				string[] array = _stBaseType.Split(':');
				if (array.Length == 2 && array[0] == "std")
				{
					_stBaseType = array[1];
				}
			}
		}

		public IList<ITypeComponent> TypeComponents => (IList<ITypeComponent>)_liComponents;

		public BitfieldType()
		{
		}

		public BitfieldType(XmlElement xeNode, TypeList typeList)
			: base(typeList, xeNode)
		{
			_liComponents = new LList<ITypeComponent>();
			BaseType = xeNode.GetAttribute("basetype");
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)childNode;
					string name = xmlElement.Name;
					if (name == "Component")
					{
						_liComponents.Add((ITypeComponent)(object)new StructComponent(xmlElement, typeList));
					}
				}
			}
		}

		public ITypeComponent AddComponent(string stIdentifier, string stType, IStringRef visibleName, string stDefaultValue)
		{
			stType = stType.Trim().ToUpperInvariant();
			if (stType != "STD:BOOL" && stType != "STD:SAFEBOOL")
			{
				throw new ArgumentException("Invalid Bitfieldcomponent. Only BOOL types allowed.");
			}
			StructComponent structComponent = new StructComponent(stIdentifier, stType);
			((ITypeComponent)structComponent).VisibleName=(visibleName);
			structComponent.DefaultValue = stDefaultValue;
			if (_liComponents == null)
			{
				_liComponents = new LList<ITypeComponent>();
			}
			_liComponents.Add((ITypeComponent)(object)structComponent);
			return (ITypeComponent)(object)structComponent;
		}
	}
}
