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
	[TypeGuid("{c2659c15-b6d3-41a4-88b2-50a445913e2e}")]
	[StorageVersion("3.3.0.0")]
	internal class DataElementRangeType : DataElementBase
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Min")]
		[StorageVersion("3.3.0.0")]
		private string _stMin = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Max")]
		[StorageVersion("3.3.0.0")]
		private string _stMax = "";

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("SimpleType")]
		[StorageVersion("3.3.0.0")]
		private DataElementBase _baseType;

		public override bool IsRangeType => true;

		public override string MinValue => _stMin;

		public override string MaxValue => _stMax;

		internal override IDataElementParent Parent
		{
			set
			{
				base.Parent = value;
				_baseType.Parent = value;
			}
		}

		public override bool CanWatch => _baseType.CanWatch;

		public override string BaseType => _baseType.BaseType;

		public override bool HasBaseType => true;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				return _baseType.EnumerationValue;
			}
			set
			{
				_baseType.EnumerationValue = value;
			}
		}

		public override bool HasSubElements => _baseType.HasSubElements;

		public override bool IsEnumeration => _baseType.IsEnumeration;

		public override IDataElement this[string stIdentifier] => _baseType[stIdentifier];

		public override string DefaultValue => _baseType.DefaultValue;

		public override string Value
		{
			get
			{
				return _baseType.Value;
			}
			set
			{
				_baseType.Value = value;
			}
		}

		public override bool SupportsSetValue => _baseType.SupportsSetValue;

		public DataElementRangeType()
		{
		}

		internal DataElementRangeType(DataElementRangeType original)
			: base(original)
		{
			_stMin = original._stMin;
			_stMax = original._stMax;
			_baseType = (DataElementBase)((GenericObject)original._baseType).Clone();
		}

		public override object Clone()
		{
			DataElementRangeType dataElementRangeType = new DataElementRangeType(this);
			((GenericObject)dataElementRangeType).AfterClone();
			return dataElementRangeType;
		}

		public override long GetBitSize()
		{
			return _baseType.GetBitSize();
		}

		public override IOnlineVarRef CreateWatch()
		{
			return _baseType.CreateWatch();
		}

		public override IOnlineVarRef2 CreateWatch(bool bClientControlled)
		{
			return _baseType.CreateWatch(bClientControlled);
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
			_baseType.UpdateLanguageModelGuids(bUpgrade, stPath);
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
			_baseType.AddTypeDefs(stTypeName, lmcontainer, bHide);
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (((ICollection)base.IoMapping.VariableMappings).Count == 0 && !bAlwaysMapping)
			{
				return;
			}
			ChannelMap channelMap = new ChannelMap(lParameterId, (ushort)GetBitSize(), bInput, bReadOnly, bAlwaysMapping, (IDataElement)(object)this, mappingMode);
			channelMap.IecAddress = _ioMapping.GetIecAddress(htStartAddresses);
			channelMap.ParamBitoffset = (ushort)lOffset;
			channelMap.Type = GetTypeString();
			channelMap.LanguageModelPositionId = base.LanguageModelPositionId;
			channelMap.Comment = base.Description;
			if (bAlwaysMapping)
			{
				AddMappingUnused(cm, channelMap);
			}
			foreach (VariableMapping item in (IEnumerable)base.IoMapping.VariableMappings)
			{
				channelMap.AddVariableMapping(item);
			}
			cm.AddChannelMap(channelMap);
		}

		internal override string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue)
		{
			CheckDefaultValueInitialisation(bIsOutput, ref bCreateDefaultValue);
			return _baseType.GetInitialization(out bDefault, bIsOutput, bCreateDefaultValue);
		}

		internal override string GetTypeName(string stBaseName)
		{
			return _baseType.GetTypeName(stBaseName);
		}

		public override string GetTypeString()
		{
			return $"{_baseType.GetTypeString()}({MinValue}..{MaxValue})";
		}

		internal override ushort GetTypeId()
		{
			return _baseType.GetTypeId();
		}

		public override void SetValue(string stValue)
		{
			_baseType.SetValue(stValue);
		}

		public override long GetBitOffset(IDataElement child)
		{
			throw new InvalidOperationException("RangeTypes do not have children.");
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		internal override void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			RangeType rangeType = (RangeType)definition;
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			_stMin = rangeType.Min;
			_stMax = rangeType.Max;
			ValueElement valueElement;
			if (defaultValue != null && defaultValue.Count > 0)
			{
				valueElement = defaultValue[0];
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			else
			{
				valueElement = new ValueElement();
			}
			if (string.IsNullOrEmpty(valueElement.Value) && !string.IsNullOrEmpty(rangeType.Default))
			{
				valueElement.Value = rangeType.Default;
			}
			LList<ValueElement> val = new LList<ValueElement>();
			val.Add(valueElement);
			_baseType = factory.Create(stIdentifier, val, rangeType.BaseType, visibleName, unit, description, filterFlags, this, _baseType, bUpdate, bCreateBitChannels: false);
		}

		internal void Import(string min, string max, string baseTypeName, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			_stMin = min;
			_stMax = max;
			ValueElement valueElement = ((defaultValue == null || defaultValue.Count <= 0) ? new ValueElement() : defaultValue[0]);
			LList<ValueElement> val = new LList<ValueElement>();
			val.Add(valueElement);
			_baseType = factory.Create(stIdentifier, val, baseTypeName, visibleName, unit, description, filterFlags, this, _baseType, bUpdate, bCreateBitChannels: false);
		}

		internal override void SetDefault(ValueElement defaultValue)
		{
			_baseType.SetDefault(defaultValue);
		}

		public override void SetMinValue(string stValue)
		{
			if (_stMin != stValue)
			{
				_stMin = stValue;
				Notify((IDataElement)(object)this, new string[0]);
			}
		}

		public override void SetMaxValue(string stValue)
		{
			if (_stMax != stValue)
			{
				_stMax = stValue;
				Notify((IDataElement)(object)this, new string[0]);
			}
		}

		public override void SetDefaultValue(string stValue)
		{
			_baseType.SetDefaultValue(stValue);
		}

		public override bool Merge(DataElementBase element)
		{
			bool result = false;
			if (element.HasBaseType && element.BaseType.Equals(BaseType, StringComparison.InvariantCultureIgnoreCase))
			{
				IConverterFromIEC converterFromIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
				try
				{
					object obj = default(object);
					TypeClass val = default(TypeClass);
					converterFromIEC.GetLiteralValue(BaseType + "#" + MinValue, out obj, out val);
					object obj2 = default(object);
					converterFromIEC.GetLiteralValue(BaseType + "#" + MaxValue, out obj2, out val);
					object obj3 = default(object);
					converterFromIEC.GetLiteralValue(BaseType + "#" + element.Value, out obj3, out val);
					if (obj3 is IComparable)
					{
						IComparable comparable = obj3 as IComparable;
						if (comparable.CompareTo(obj) >= 0)
						{
							if (comparable.CompareTo(obj2) <= 0)
							{
								result = base.Merge(element);
								return result;
							}
							return result;
						}
						return result;
					}
					return result;
				}
				catch
				{
					return result;
				}
			}
			return result;
		}
	}
}
