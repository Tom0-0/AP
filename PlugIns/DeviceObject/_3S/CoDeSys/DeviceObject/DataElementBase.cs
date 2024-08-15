using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{524c2a10-46b1-4f99-a7ca-6e8dd7cf499f}")]
	[StorageVersion("3.3.0.0")]
	public abstract class DataElementBase : GenericObject2, IDataElement12, IDataElement11, IDataElement10, IDataElement9, IDataElement8, IDataElement7, IDataElement6, IDataElement5, IDataElement4, IDataElement3, IDataElement2, IDataElement, IDataElementParent
	{
		[DefaultDuplication(0)]
		[DefaultSerialization("Identifier")]
		[StorageVersion("3.3.0.0")]
		protected string _stIdentifier = "<unknown>";

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected StringRef _visibleName = new StringRef();

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected StringRef _description;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("UserComment")]
		[StorageVersion("3.3.0.0")]
		protected string _stUserComment = "";

		protected static DataElementCollection emptyDataElementCollection = new DataElementCollection();

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected DataElementCollection _subElements = new DataElementCollection();

		protected static IoMapping emptyIoMapping = new IoMapping();

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected IoMapping _ioMapping;

		protected static CustomItemList emptyCustomItemList = new CustomItemList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected CustomItemList _customItemList;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("FilterFlags")]
		[StorageVersion("3.3.0.0")]
		protected string[] _filterFlags = new string[0];

		protected static StringRef emptyUnitSerialization = new StringRef();

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected StringRef _unit;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("PositionId")]
		[StorageVersion("3.3.0.0")]
		protected long _lLanguageModelPositionId = -1L;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("EditorPositionId")]
		[StorageVersion("3.3.0.0")]
		protected long _lEditorPositionId = -1L;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("OfflineAccess")]
		[StorageVersion("3.4.0.0")]
		[StorageDefaultValue(AccessRight.ReadWrite)]
		protected AccessRight _accessRightOffline = (AccessRight)3;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("OnlineAccess")]
		[StorageVersion("3.4.0.0")]
		[StorageDefaultValue(AccessRight.ReadWrite)]
		protected AccessRight _accessRightOnline = (AccessRight)3;

		private IDataElementParent _parent;

		private ushort _usType = ushort.MaxValue;

		[DefaultSerialization("VisibleName")]
		[StorageVersion("3.3.0.0")]
		protected StringRef VisibleNameSerialization
		{
			get
			{
				if (_visibleName == null)
				{
					return new StringRef();
				}
				return _visibleName;
			}
			set
			{
				_visibleName = ParameterDataCache.AddStringRef(value);
			}
		}

		[DefaultSerialization("Description")]
		[StorageVersion("3.3.0.0")]
		protected StringRef Descriptionerialization
		{
			get
			{
				if (_description == null)
				{
					return new StringRef();
				}
				return _description;
			}
			set
			{
				if (value != null && (value.Namespace != string.Empty || value.Default != string.Empty))
				{
					_description = ParameterDataCache.AddStringRef(value);
				}
			}
		}

		[DefaultSerialization("SubElements")]
		[StorageVersion("3.3.0.0")]
		protected DataElementCollection SubElementserialization
		{
			get
			{
				if (_subElements == null)
				{
					return emptyDataElementCollection;
				}
				return _subElements;
			}
			set
			{
				if (value != null && value.Count > 0)
				{
					_subElements = value;
				}
			}
		}

		[DefaultSerialization("IoMapping")]
		[StorageVersion("3.3.0.0")]
		protected IoMapping IoMappingSerialization
		{
			get
			{
				if (_ioMapping == null)
				{
					return emptyIoMapping;
				}
				return _ioMapping;
			}
			set
			{
				if (value != null && (((ICollection)value.VariableMappings).Count > 0 || !value.AutomaticIecAddress))
				{
					_ioMapping = value;
				}
			}
		}

		[DefaultSerialization("CustomItems")]
		[StorageVersion("3.3.0.0")]
		protected CustomItemList CustomItemsSerialization
		{
			get
			{
				if (_customItemList == null)
				{
					return emptyCustomItemList;
				}
				return _customItemList;
			}
			set
			{
				if (value != null && value.Count > 0)
				{
					_customItemList = value;
				}
			}
		}

		[DefaultSerialization("Unit")]
		[StorageVersion("3.3.0.0")]
		protected StringRef UnitSerialization
		{
			get
			{
				if (_unit == null)
				{
					return emptyUnitSerialization;
				}
				return _unit;
			}
			set
			{
				if (value != null)
				{
					_unit = ParameterDataCache.AddStringRef(value);
				}
			}
		}

		public IDataElement DataElement => (IDataElement)(object)this;

		public IIoProvider IoProvider
		{
			get
			{
				if (_parent != null)
				{
					return _parent.IoProvider;
				}
				return null;
			}
		}

		public bool IsParameter => false;

		public string Identifier => _stIdentifier;

		public string VisibleName
		{
			get
			{
				if (_visibleName == null)
				{
					return string.Empty;
				}
				string text = _visibleName.Default;
				Parameter parameter = _parent as Parameter;
				if (parameter == null)
				{
					parameter = GetParameter();
				}
				if (parameter != null && parameter.Owner != null)
				{
					IStringTable stringTable = parameter.Owner.StringTable;
					IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
					if (val != null)
					{
						val.ResolveString(_visibleName.Namespace, _visibleName.Identifier, _visibleName.Default, out text);
					}
				}
				if (_parent is DataElementCollection && (_parent as DataElementCollection).Parent is DataElementArrayType)
				{
					DataElementArrayType dataElementArrayType = (_parent as DataElementCollection).Parent as DataElementArrayType;
					if (text == dataElementArrayType.VisibleName)
					{
						string[] array = _stIdentifier.Substring(dataElementArrayType.Identifier.Length).Split('_');
						text += "[";
						if (dataElementArrayType.Dimension1 != null && array.Length > 1)
						{
							text += array[1];
						}
						if (dataElementArrayType.Dimension2 != null && array.Length > 2)
						{
							text = text + "," + array[2];
						}
						if (dataElementArrayType.Dimension3 != null && array.Length > 3)
						{
							text = text + "," + array[3];
						}
						text += "]";
					}
				}
				return text;
			}
		}

		public string Description
		{
			get
			{
				if (_description == null)
				{
					return string.Empty;
				}
				string @default = _description.Default;
				Parameter parameter = _parent as Parameter;
				if (parameter == null)
				{
					parameter = GetParameter();
				}
				if (parameter != null && parameter.Owner != null)
				{
					IStringTable stringTable = parameter.Owner.StringTable;
					IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
					if (val != null)
					{
						val.ResolveString(_description.Namespace, _description.Identifier, _description.Default, out @default);
					}
				}
				return @default;
			}
		}

		public string UserComment
		{
			get
			{
				return _stUserComment;
			}
			set
			{
				string text = "";
				if (value != null)
				{
					text = value;
				}
				if (_stUserComment != text)
				{
					_stUserComment = text;
					Notify((IDataElement)(object)this, new string[0]);
				}
			}
		}

		public IDataElementCollection SubElements
		{
			get
			{
				if (_subElements == null)
				{
					return (IDataElementCollection)(object)new DataElementCollection();
				}
				return (IDataElementCollection)(object)_subElements;
			}
		}

		public ICustomItemList CustomItems
		{
			get
			{
				if (_customItemList == null)
				{
					_customItemList = new CustomItemList();
				}
				return (ICustomItemList)(object)_customItemList;
			}
		}

		public string[] FilterFlags => _filterFlags;

		public string Unit
		{
			get
			{
				if (_unit == null)
				{
					return string.Empty;
				}
				string @default = _unit.Default;
				Parameter parameter = _parent as Parameter;
				if (parameter == null)
				{
					parameter = GetParameter();
				}
				if (parameter != null && parameter.Owner != null)
				{
					try
					{
						IStringTable stringTable = parameter.Owner.StringTable;
						IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
						if (val == null)
						{
							return @default;
						}
						val.ResolveString(_unit.Namespace, _unit.Identifier, _unit.Default, out @default);
						return @default;
					}
					catch
					{
						return @default;
					}
				}
				return @default;
			}
		}

		public IIoMapping IoMapping
		{
			get
			{
				if (_ioMapping == null)
				{
					return (IIoMapping)(object)new IoMapping();
				}
				return (IIoMapping)(object)_ioMapping;
			}
		}

		internal IoMapping Mapping
		{
			get
			{
				if (_ioMapping == null)
				{
					return new IoMapping();
				}
				return _ioMapping;
			}
			set
			{
				_ioMapping = value;
			}
		}

		public abstract bool HasSubElements { get; }

		public abstract bool IsRangeType { get; }

		public abstract bool IsEnumeration { get; }

		public abstract IDataElement this[string stIdentifier] { get; }

		public abstract string MinValue { get; }

		public abstract string MaxValue { get; }

		public abstract string DefaultValue { get; }

		public abstract string Value { get; set; }

		public abstract IEnumerationValue EnumerationValue { get; set; }

		public abstract string BaseType { get; }

		public abstract bool CanWatch { get; }

		public abstract bool HasBaseType { get; }

		public abstract bool SupportsSetValue { get; }

		internal virtual IDataElementParent Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		public long LanguageModelPositionId => _lLanguageModelPositionId;

		public long EditorPositionId => _lEditorPositionId;

		internal string PositionPragma
		{
			get
			{
				long languageModelPositionId = LanguageModelPositionId;
				if (languageModelPositionId == -1)
				{
					return string.Empty;
				}
				return "{p " + languageModelPositionId + "}";
			}
		}

		protected DataElementCollection SubElementCollection
		{
			get
			{
				return _subElements;
			}
			set
			{
				_subElements = value;
			}
		}

		public bool IsUnion => this is DataElementUnionType;

		internal StringRef GetVisibleName => _visibleName;

		internal StringRef GetDescription => _description;

		internal StringRef GetUnit => _unit;

		internal IStringRef DescriptionRef
		{
			get
			{
				return (IStringRef)(object)_description;
			}
			set
			{
				_description = ParameterDataCache.AddStringRef(new StringRef(value));
			}
		}

		public string WatchFbInstanceVariable
		{
			get
			{
				string text = string.Empty;
				if (HasBaseType)
				{
					Parameter parameter = GetParameter();
					string fbInstanceVariableWithDeviceName = parameter.FbInstanceVariableWithDeviceName;
					if (!string.IsNullOrEmpty(fbInstanceVariableWithDeviceName))
					{
						DataElementBase dataElementBase = this;
						while (dataElementBase != null)
						{
							IDataElementParent parent = dataElementBase.Parent;
							if (parent is DataElementCollection)
							{
								parent = (parent as DataElementCollection).Parent;
							}
							if (dataElementBase is DataElementBitFieldComponent)
							{
								text = "." + parent.GetBitOffset((IDataElement)(object)this) + text;
							}
							else if (parent is DataElementArrayType)
							{
								DataElementArrayType dataElementArrayType = parent as DataElementArrayType;
								string[] array = _stIdentifier.Substring(dataElementArrayType.Identifier.Length).Split('_');
								string text2 = "[";
								if (dataElementArrayType.Dimension1 != null && array.Length > 1)
								{
									text2 += array[1];
								}
								if (dataElementArrayType.Dimension2 != null && array.Length > 2)
								{
									text2 = text2 + "," + array[2];
								}
								if (dataElementArrayType.Dimension3 != null && array.Length > 3)
								{
									text2 = text2 + "," + array[3];
								}
								text2 += "]";
								text += text2;
							}
							else if (dataElementBase.Identifier != parameter.Identifier)
							{
								text = "." + dataElementBase.Identifier + text;
							}
							dataElementBase = ((!(parent is DataElementBase)) ? null : (parent as DataElementBase));
						}
						text = fbInstanceVariableWithDeviceName + text;
					}
				}
				return text;
			}
		}

		public string VisibleNameDefault => _visibleName.Default;

		public string DescriptionDefault => _description.Default;

		public string UnitDefault => _unit.Default;

		public IStringRef VisibleNameStringRef => (IStringRef)(object)_visibleName;

		public IStringRef DescriptionStringRef => (IStringRef)(object)_description;

		public IStringRef UnitStringRef => (IStringRef)(object)_unit;

		public DataElementBase()
			: base()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			if (SubElementCollection != null)
			{
				SubElementCollection.Parent = this;
			}
			if (_ioMapping != null)
			{
				_ioMapping.Parent = this;
			}
		}

		internal void ImportBase(string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, CustomItemList customItemList, bool bUpdate, IDataElementParent parent)
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			_stIdentifier = stIdentifier;
			_visibleName = ParameterDataCache.AddStringRef(visibleName);
			_unit = ParameterDataCache.AddStringRef(unit);
			if (!bUpdate)
			{
				_description = ParameterDataCache.AddStringRef(description);
			}
			else if (string.IsNullOrEmpty(_description?.Default))
			{
				_description = ParameterDataCache.AddStringRef(description);
			}
			_filterFlags = filterFlags;
			if (customItemList.Count > 0)
			{
				_customItemList = customItemList;
			}
			if (SubElementCollection != null)
			{
				SubElementCollection.Parent = this;
			}
			if (_ioMapping != null)
			{
				_ioMapping.Parent = this;
			}
			if (parent != null && parent.DataElement != null && parent.DataElement is IDataElement6)
			{
				ref AccessRight accessRightOffline = ref _accessRightOffline;
				IDataElement dataElement = parent.DataElement;
				accessRightOffline = ((IDataElement6)((dataElement is IDataElement6) ? dataElement : null)).GetAccessRight(false);
				ref AccessRight accessRightOnline = ref _accessRightOnline;
				IDataElement dataElement2 = parent.DataElement;
				accessRightOnline = ((IDataElement6)((dataElement2 is IDataElement6) ? dataElement2 : null)).GetAccessRight(true);
			}
		}

		internal DataElementBase(DataElementBase element)
			: this()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			_stIdentifier = element._stIdentifier;
			_visibleName = ParameterDataCache.AddStringRef(element._visibleName);
			if (element._description != null)
			{
				_description = ParameterDataCache.AddStringRef(element._description);
			}
			_stUserComment = element._stUserComment;
			if (element._subElements != null)
			{
				_subElements = (DataElementCollection)((GenericObject)element._subElements).Clone();
			}
			if (_subElements != null)
			{
				_subElements.Parent = this;
			}
			if (element._ioMapping != null)
			{
				_ioMapping = (IoMapping)((GenericObject)element._ioMapping).Clone();
			}
			if (element._customItemList != null)
			{
				_customItemList = (CustomItemList)((GenericObject)element._customItemList).Clone();
			}
			if (element._filterFlags != null)
			{
				_filterFlags = (string[])element._filterFlags.Clone();
			}
			else
			{
				_filterFlags = null;
			}
			if (element._unit != null)
			{
				_unit = ParameterDataCache.AddStringRef(element._unit);
			}
			_lLanguageModelPositionId = element._lLanguageModelPositionId;
			_lEditorPositionId = element._lEditorPositionId;
			_accessRightOffline = element._accessRightOffline;
			_accessRightOnline = element._accessRightOnline;
			_usType = element._usType;
		}

		public void Notify(IDataElement dataelement, string[] path)
		{
			if (_parent == null)
			{
				throw new InvalidOperationException("Cannot notify a change whithout a parent");
			}
			if (!_parent.IsParameter)
			{
				string[] array = new string[path.Length + 1];
				array[0] = Identifier;
				path.CopyTo(array, 1);
				_parent.Notify(dataelement, array);
			}
			else
			{
				_parent.Notify(dataelement, path);
			}
		}

		public abstract long GetBitOffset(IDataElement child);

		public abstract long GetBitOffset();

		internal bool CheckDefaultValueInitialisation(bool bIsOutput, ref bool bCreateDefaultValue)
		{
			if (bIsOutput && !bCreateDefaultValue && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)14, (ushort)0))
			{
				if (_ioMapping?.VariableMappings != null && ((ICollection)_ioMapping.VariableMappings).Count > 0)
				{
					bCreateDefaultValue = _ioMapping.VariableMappings[0].CreateVariable;
				}
				return bCreateDefaultValue;
			}
			return true;
		}

		public override void AfterDeserialize()
		{
			if (SubElementCollection != null)
			{
				SubElementCollection.Parent = this;
			}
			if (_ioMapping != null)
			{
				_ioMapping.Parent = this;
			}
		}

		public override void AfterClone()
		{
			if (SubElementCollection != null)
			{
				SubElementCollection.Parent = this;
			}
			if (_ioMapping != null)
			{
				_ioMapping.Parent = this;
			}
		}

		public abstract string GetTypeString();

		public abstract long GetBitSize();

		public abstract IOnlineVarRef CreateWatch();

		public abstract IOnlineVarRef2 CreateWatch(bool bClientControlled);

		public abstract void SetValue(string stValue);

		public abstract void SetMinValue(string stValue);

		public abstract void SetMaxValue(string stValue);

		public abstract void SetDefaultValue(string stValue);

		public void SetName(IStringRef name)
		{
			_visibleName = ParameterDataCache.AddStringRef(new StringRef(name));
			Notify((IDataElement)(object)this, new string[0]);
		}

		public void SetDescription(IStringRef description)
		{
			_description = ParameterDataCache.AddStringRef(new StringRef(description));
			Notify((IDataElement)(object)this, new string[0]);
		}

		public void SetUnit(IStringRef unit)
		{
			_unit = ParameterDataCache.AddStringRef(new StringRef(unit));
			Notify((IDataElement)(object)this, new string[0]);
		}

		internal abstract void Import(TypeDefinition definition, DataElementFactory dataElementFactory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent);

		internal abstract void SetDefault(ValueElement defaultValue);

		internal abstract string GetTypeName(string stBaseName);

		internal abstract string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue);

		internal abstract void UpdateLanguageModelGuids(bool bUpgrade, string stPath);

		internal abstract void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide);

		internal abstract void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield);

		internal abstract ushort GetTypeId();

		internal void SetPositionIds(IUniqueIdGenerator idGenerator)
		{
			_lLanguageModelPositionId = idGenerator.GetNext(true);
			_lEditorPositionId = idGenerator.GetNext(true);
			if (_subElements == null)
			{
				return;
			}
			foreach (DataElementBase subElement in _subElements)
			{
				subElement.SetPositionIds(idGenerator);
			}
		}

		internal void SetIoProvider(IIoProvider ioProvider)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (_ioMapping == null && _parent != null)
			{
				Parameter parameter = GetParameter();
				if (parameter != null && (int)parameter.ChannelType != 0)
				{
					_ioMapping = new IoMapping();
				}
			}
			if (_ioMapping != null)
			{
				_ioMapping.SetIoProvider(ioProvider);
				_ioMapping.Parent = this;
			}
			if (_subElements == null)
			{
				return;
			}
			foreach (DataElementBase subElement in _subElements)
			{
				subElement.SetIoProvider(ioProvider);
			}
		}

		internal void SetCustomItems(CustomItemList customItems)
		{
			_customItemList = customItems;
		}

		public Parameter GetParameter()
		{
			if (_parent == null)
			{
				return null;
			}
			return _parent.GetParameter();
		}

		protected ushort GetStandardTypeId(string stType, ushort usDefault)
		{
			if (_usType == ushort.MaxValue)
			{
				_usType = Types.GetTypeId(stType, usDefault);
			}
			return _usType;
		}

		public void AddMappingUnused(ConnectorMap cm, ChannelMap channel)
		{
			if (((ICollection)_ioMapping.VariableMappings).Count == 0 && channel.IecAddress != null)
			{
				string empty = string.Empty;
				empty = ((channel.LanguageModelPositionId == -1) ? (cm.ObjectGuid.ToString() + ((object)channel.IecAddress).ToString()) : (cm.ObjectGuid.ToString() + "_" + channel.LanguageModelPositionId + ((object)channel.IecAddress).ToString()));
				empty = "map_" + empty.Replace("%", "_").Replace(".", "_").Replace("-", "_");
				VariableMapping variableMapping = new VariableMapping(channel.LanguageModelPositionId, empty, bCreateVariable: true, bIsUnusedMapping: true);
				variableMapping.Parent = this;
				variableMapping.IoProvider = IoProvider;
				channel.AddVariableMapping(variableMapping);
			}
		}

		internal bool CheckCrossRefs(ICompileContext comcon, DataElementBase elem, IDirectVariable channelvar, DirectVarCrossRefList crossrefs)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected I4, but got Unknown
			if (channelvar != null)
			{
				bool flag = default(bool);
				IDataLocation locBase = comcon.LocateAddress(out flag, channelvar);
				if (!flag)
				{
					BitDataLocation bitDataLocation = new BitDataLocation(locBase);
					foreach (DirectVarCrossRef crossRef in crossrefs.GetCrossRefs())
					{
						if (crossRef.DirectVariable.IsEqual(channelvar))
						{
							return true;
						}
						if (crossRef.DirectVariable.Location != channelvar.Location)
						{
							continue;
						}
						IDataLocation locBase2 = comcon.LocateAddress(out flag, crossRef.DirectVariable);
						if (!flag)
						{
							BitDataLocation bitDataLocation2 = new BitDataLocation(locBase2);
							int num = 0;
							DirectVariableSize size = crossRef.DirectVariable.Size;
							switch ((int)size - 1)
							{
							case 1:
								num = 8;
								break;
							case 2:
								num = 16;
								break;
							case 3:
								num = 32;
								break;
							case 4:
								num = 64;
								break;
							case 0:
								num = 1;
								break;
							}
							long bitOffset = bitDataLocation2.BitOffset;
							long num2 = bitOffset + num;
							long bitOffset2 = bitDataLocation.BitOffset;
							long num3 = bitDataLocation.BitOffset + elem.GetBitSize();
							if ((bitOffset <= bitOffset2 && num2 > bitOffset2) || (bitOffset < num3 && num2 >= num3))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		internal bool IsBitMapping(ICompileContext comcon, IDirectVariable channelvar, DataElementBase elem, DirectVarCrossRefList crossrefs)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Invalid comparison between Unknown and I4
			if (crossrefs.Count == 0)
			{
				return false;
			}
			long bitSize = elem.GetBitSize();
			if (bitSize != 1)
			{
				bool flag = default(bool);
				IDataLocation locBase = comcon.LocateAddress(out flag, channelvar);
				if (!flag)
				{
					BitDataLocation bitDataLocation = new BitDataLocation(locBase);
					foreach (DirectVarCrossRef crossRef in crossrefs.GetCrossRefs())
					{
						if ((int)crossRef.DirectVariable.Size == 1 && crossRef.BitDataLocation != null)
						{
							long bitOffset = crossRef.BitDataLocation.BitOffset;
							long num = bitOffset + 1;
							long bitOffset2 = bitDataLocation.BitOffset;
							long num2 = bitDataLocation.BitOffset + bitSize;
							if (bitOffset >= bitOffset2 && num <= num2)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		internal bool IsSubElementMapping(ICompileContext comcon, IDirectVariable channelvar, DataElementBase elem, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses)
		{
			if (elem.HasSubElements)
			{
				foreach (DataElementBase item in (IEnumerable)elem.SubElements)
				{
					IDirectVariable iecAddress = (item.IoMapping as IoMapping).GetIecAddress(htStartAddresses);
					if (directVarCRefs != null && iecAddress != null)
					{
						DirectVarCrossRefList[] inputs = directVarCRefs.Inputs;
						foreach (DirectVarCrossRefList crossrefs in inputs)
						{
							if (CheckCrossRefs(comcon, item, iecAddress, crossrefs))
							{
								return true;
							}
						}
						inputs = directVarCRefs.Outputs;
						foreach (DirectVarCrossRefList crossrefs2 in inputs)
						{
							if (CheckCrossRefs(comcon, item, iecAddress, crossrefs2))
							{
								return true;
							}
						}
					}
					if (IsSubElementMapping(comcon, iecAddress, item, directVarCRefs, htStartAddresses))
					{
						return true;
					}
				}
			}
			else if (directVarCRefs != null)
			{
				DirectVarCrossRefList[] inputs = directVarCRefs.Inputs;
				foreach (DirectVarCrossRefList crossrefs3 in inputs)
				{
					if (IsBitMapping(comcon, channelvar, elem, crossrefs3))
					{
						return true;
					}
				}
				inputs = directVarCRefs.Outputs;
				foreach (DirectVarCrossRefList crossrefs4 in inputs)
				{
					if (IsBitMapping(comcon, channelvar, elem, crossrefs4))
					{
						return true;
					}
				}
			}
			if (((ICollection)elem.IoMapping.VariableMappings).Count > 0)
			{
				return true;
			}
			return false;
		}

		public AccessRight GetAccessRight(bool bOnline)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			if (bOnline)
			{
				return _accessRightOnline;
			}
			return _accessRightOffline;
		}

		public void SetAccessRight(bool bOnline, AccessRight access)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (bOnline)
			{
				_accessRightOnline = access;
			}
			else
			{
				_accessRightOffline = access;
			}
		}

		public void SetLanguageModelPositionId(long lPositionId)
		{
			_lLanguageModelPositionId = lPositionId;
		}

		public void SetEditorPositionId(long lPositionId)
		{
			_lEditorPositionId = lPositionId;
		}

		public virtual bool Merge(DataElementBase element)
		{
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			bool result = false;
			try
			{
				bool flag = false;
				if (HasBaseType && element.HasBaseType)
				{
					if (element.BaseType.Equals(BaseType, StringComparison.InvariantCultureIgnoreCase))
					{
						flag = true;
					}
				}
				else if ((element.Parent?.IsParameter ?? false) && (Parent?.IsParameter ?? false))
				{
					if (element.Parent is IParameter10 && Parent is IParameter10 && (element.Parent as IParameter10).ParamType.Equals((this.Parent as IParameter10).ParamType))
					{
						flag = true;
					}
				}
				else if (element.Identifier.Equals(Identifier))
				{
					flag = true;
				}
				if (flag)
				{
					Mapping = (IoMapping)((GenericObject)element.Mapping).Clone();
					Parameter parameter = GetParameter();
					if (parameter != null && (int)parameter.ChannelType != 0 && element.GetDescription != null)
					{
						SetDescription((IStringRef)(object)element.GetDescription);
					}
					if (((ICollection)element.CustomItems).Count > 0)
					{
						SetCustomItems(element.CustomItems as CustomItemList);
					}
					if (SupportsSetValue)
					{
						if (element is DataElementBitFieldType)
						{
							SetValue((element as DataElementBitFieldType).GetInitialization(out var _, bIsOutput: false, bCreateDefaultValue: false));
						}
						else
						{
							SetValue(element.Value);
						}
					}
					result = true;
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}
	}
}
