using System;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{33f58abf-62ae-4277-a80b-74c57ea00c50}")]
	[StorageVersion("3.3.0.0")]
	public class ParameterSection : GenericObject2, IParameterSection3, IParameterSection2, IParameterSection
	{
		private ParameterSet _owner;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		private StringRef _name = new StringRef();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Description")]
		[StorageVersion("3.3.0.0")]
		private StringRef _description = new StringRef();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ParentId")]
		[StorageVersion("3.3.0.0")]
		private int _nParentSectionId = -1;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("SectionId")]
		[StorageVersion("3.3.0.0")]
		private int _nSectionId = -1;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("SubSections")]
		[StorageVersion("3.3.0.0")]
		[StorageSaveAsNonGenericCollection("3.3.0.0-3.5.0.255")]
		private LList<IParameterSection> _alSubSectionsLazy;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("CustomItems")]
		[StorageVersion("3.3.0.0")]
		private CustomItemList _customItems;

		public string Name
		{
			get
			{
				string @default = _name.Default;
				try
				{
					IStringTable stringTable = _owner.StringTable;
					IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
					if (val != null)
					{
						val.ResolveString(_name.Namespace, _name.Identifier, _name.Default, out @default);
						return @default;
					}
					return @default;
				}
				catch
				{
					return @default;
				}
			}
		}

		public string Description
		{
			get
			{
				string @default = _description.Default;
				try
				{
					IStringTable stringTable = _owner.StringTable;
					IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
					if (val != null)
					{
						val.ResolveString(_description.Namespace, _description.Identifier, _description.Default, out @default);
						return @default;
					}
					return @default;
				}
				catch
				{
					return @default;
				}
			}
		}

		public IParameterSection Section
		{
			get
			{
				if (_nParentSectionId < 0)
				{
					return null;
				}
				return (IParameterSection)(object)_owner.GetSection(_nParentSectionId);
			}
		}

		public ICustomItemList2 CustomItems
		{
			get
			{
				if (_customItems == null)
				{
					_customItems = new CustomItemList();
				}
				return (ICustomItemList2)(object)_customItems;
			}
		}

		public IParameterSection2[] SubSections
		{
			get
			{
				if (_alSubSectionsLazy == null)
				{
					return (IParameterSection2[])(object)new IParameterSection2[0];
				}
				IParameterSection2[] array = (IParameterSection2[])(object)new IParameterSection2[_alSubSectionsLazy.Count];
				_alSubSectionsLazy.CopyTo((IParameterSection[])(object)array);
				return array;
			}
		}

		public int Id => _nSectionId;

		internal StringRef GetDescription => _description;

		public ParameterSection()
			: base()
		{
		}

		internal ParameterSection(XmlElement xeSection, TypeList typeList, int nParentSectionId, ParameterSet owner, bool bUpdate, ref long lIndex, LList<long> liParamIds)
			: this()
		{
			_owner = owner;
			_nSectionId = _owner.CreateSectionId();
			Import(xeSection, typeList, nParentSectionId, bUpdate, ref lIndex, liParamIds);
		}

		internal ParameterSection(StringRef name, StringRef description, int nParentSectionId, ParameterSet owner)
			: this()
		{
			if (name != null)
			{
				_name = name;
			}
			if (description != null)
			{
				_description = description;
			}
			_owner = owner;
			_nParentSectionId = nParentSectionId;
			_nSectionId = owner.CreateSectionId();
		}

		public override int GetHashCode()
		{
			return _nSectionId;
		}

		public override bool Equals(object obj)
		{
			ParameterSection parameterSection = obj as ParameterSection;
			if (parameterSection == null)
			{
				return false;
			}
			return _nSectionId == parameterSection._nSectionId;
		}

		public IParameterSection2 AddSubSection(IStringRef name, IStringRef description, int nIndex)
		{
			ParameterSection parameterSection = new ParameterSection(new StringRef(name), new StringRef(description), _nSectionId, _owner);
			if (_alSubSectionsLazy == null)
			{
				_alSubSectionsLazy = new LList<IParameterSection>();
			}
			if (nIndex < 0)
			{
				_alSubSectionsLazy.Add((IParameterSection)(object)parameterSection);
			}
			else
			{
				_alSubSectionsLazy.Insert(nIndex, (IParameterSection)(object)parameterSection);
			}
			_owner.ParameterSectionAdded(parameterSection);
			return (IParameterSection2)(object)parameterSection;
		}

		public void RemoveSubSection(IParameterSection section)
		{
			ParameterSection parameterSection = section as ParameterSection;
			if (section == null)
			{
				throw new ArgumentException("Invalid section");
			}
			if (_alSubSectionsLazy != null)
			{
				_alSubSectionsLazy.Remove((IParameterSection)(object)parameterSection);
				_owner.ParameterSectionRemoved(parameterSection);
			}
		}

		public void SetName(IStringRef name)
		{
			_name = new StringRef(name);
			_owner.RaiseSectionChanged((IParameterSection3)(object)this);
		}

		public void SetDescription(IStringRef description)
		{
			_description = new StringRef(description);
			_owner.RaiseSectionChanged((IParameterSection3)(object)this);
		}

		internal void SetOwner(ParameterSet owner)
		{
			_owner = owner;
			if (_alSubSectionsLazy == null)
			{
				return;
			}
			foreach (ParameterSection item in _alSubSectionsLazy)
			{
				item.SetOwner(owner);
			}
		}

		internal StringRef GetName()
		{
			return _name;
		}

		internal void Update(XmlElement xeSection, TypeList types, int nParentId, ref long lIndex, LList<long> liParamIds)
		{
			Import(xeSection, types, nParentId, bUpdate: true, ref lIndex, liParamIds);
		}

		private void Import(XmlElement xeSection, TypeList typeList, int nParentSectionId, bool bUpdate, ref long lIndex, LList<long> liParamIds)
		{
			_nParentSectionId = nParentSectionId;
			foreach (XmlNode childNode in xeSection.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				XmlElement xmlElement = (XmlElement)childNode;
				switch (xmlElement.Name)
				{
				case "Name":
					_name = ParameterDataCache.AddStringRef(new StringRef(xmlElement));
					break;
				case "Description":
					_description = ParameterDataCache.AddStringRef(new StringRef(xmlElement));
					break;
				case "Custom":
					_customItems = new CustomItemList(xmlElement);
					break;
				case "Parameter":
					_owner.ReadParameter(xmlElement, typeList, _nSectionId, bUpdate, ref lIndex, liParamIds);
					break;
				case "ParameterSection":
					if (_alSubSectionsLazy == null)
					{
						_alSubSectionsLazy = new LList<IParameterSection>();
					}
					_owner.ReadSection(xmlElement, typeList, _nSectionId, _alSubSectionsLazy, bUpdate, ref lIndex, liParamIds);
					break;
				}
			}
		}

		internal static XmlElement GetNameNode(XmlElement xeSection)
		{
			foreach (XmlNode childNode in xeSection.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)childNode;
					if (xmlElement.Name == "Name")
					{
						return xmlElement;
					}
				}
			}
			return null;
		}
	}
}
