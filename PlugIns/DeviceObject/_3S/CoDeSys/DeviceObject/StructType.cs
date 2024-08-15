using System.Collections;
using System.Collections.Generic;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{05875a90-2017-464a-b9d5-85b2bb0751a1}")]
	[StorageVersion("3.5.6.0")]
	public class StructType : TypeDefinition, ITypeStructureUnion, ITypeDefinition
	{
		[DefaultSerialization("Components")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private LList<ITypeComponent> _liComponents;

		[DefaultSerialization("IecType")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private string _stIecType;

		[DefaultSerialization("IecTypeLib")]
		[StorageVersion("3.5.6.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private string _stIecTypeLib;

		public string IecType
		{
			get
			{
				return _stIecType;
			}
			set
			{
				_stIecType = value;
			}
		}

		public string IecTypeLib
		{
			get
			{
				return _stIecTypeLib;
			}
			set
			{
				_stIecTypeLib = value;
			}
		}

		public IList Components => (IList)_liComponents;

		public IList<ITypeComponent> TypeComponents => (IList<ITypeComponent>)_liComponents;

		public StructType()
		{
		}

		public StructType(XmlElement xeNode, TypeList typeList)
			: base(typeList, xeNode)
		{
			_stIecType = xeNode.GetAttribute("iecType");
			_stIecTypeLib = xeNode.GetAttribute("iecTypeLib");
			_liComponents = new LList<ITypeComponent>();
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					string name = childNode.Name;
					if (name == "Component")
					{
						_liComponents.Add((ITypeComponent)(object)new StructComponent((XmlElement)childNode, typeList));
					}
				}
			}
		}

		public bool HasComponent(string stIdentifier)
		{
			foreach (StructComponent liComponent in _liComponents)
			{
				if (stIdentifier == liComponent.Identifier)
				{
					return true;
				}
			}
			return false;
		}

		public ITypeComponent AddComponent(string stIdentifier, string stType, IStringRef visibleName, string stDefaultValue)
		{
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
