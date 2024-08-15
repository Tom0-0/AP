#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{4322e2a3-55e5-4e47-9fcd-b70eb1e6bfee}")]
	[StorageVersion("3.3.0.0")]
	internal class DataElementEnumType : DataElementBase, IEnumerationDataElement
	{
		[DefaultSerialization("Values")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private EnumValue[] _values = new EnumValue[0];

		[DefaultSerialization("BaseType")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private DataElementBase _baseType;

		public IEnumerationValue[] EnumerationValues => (IEnumerationValue[])(object)_values;

		public override string BaseType => _baseType.BaseType;

		public override bool HasBaseType => true;

		public override string DefaultValue => _baseType.DefaultValue;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				int valueEnumerationIndex = GetValueEnumerationIndex();
				if (valueEnumerationIndex < 0)
				{
					return (IEnumerationValue)(object)EnumValue.CreateDummyValue(Value);
				}
				return (IEnumerationValue)(object)_values[valueEnumerationIndex];
			}
			set
			{
				EnumValue enumValue = value as EnumValue;
				if (enumValue == null)
				{
					throw new ArgumentException("Invalid enumeration value");
				}
				Value = enumValue.Value;
			}
		}

		public override bool CanWatch => _baseType.CanWatch;

		public override bool HasSubElements => false;

		public override bool IsEnumeration => true;

		public override bool IsRangeType => false;

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override IDataElement this[string stIdentifier]
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

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

		public override bool SupportsSetValue => true;

		internal override IDataElementParent Parent
		{
			set
			{
				base.Parent = value;
				_baseType.Parent = value;
				EnumValue[] values = _values;
				for (int i = 0; i < values.Length; i++)
				{
					values[i].Parent = this;
				}
			}
		}

		public DataElementEnumType()
		{
		}

		internal DataElementEnumType(DataElementEnumType original)
			: base(original)
		{
			_values = (EnumValue[])original._values.Clone();
			_baseType = (DataElementBase)((GenericObject)original._baseType).Clone();
			_baseType.SetLanguageModelPositionId(base.LanguageModelPositionId);
			_baseType.SetEditorPositionId(base.EditorPositionId);
		}

		public override object Clone()
		{
			DataElementEnumType dataElementEnumType = new DataElementEnumType(this);
			((GenericObject)dataElementEnumType).AfterClone();
			return dataElementEnumType;
		}

		public int GetValueEnumerationIndex()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			IComparable comparable = null;
			IConverterFromIEC converterFromIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
			object obj = null;
			int result = -1;
			try
			{
				try
				{
					ICompiledType val = Types.ParseType(BaseType);
					string text = ((val == null || (int)((IType)val).Class != 0) ? (BaseType.ToUpperInvariant() + "#" + Value) : ((!(Value == "0") && !(Value == "1")) ? Value : (BaseType.ToUpperInvariant() + "#" + Value)));
					TypeClass val2 = default(TypeClass);
					converterFromIEC.GetLiteralValue(text, out obj, out val2);
					comparable = obj as IComparable;
				}
				catch
				{
				}
				for (int i = 0; i < _values.Length; i++)
				{
					bool flag = false;
					try
					{
						if (comparable != null && _values[i].ComparableValue != null)
						{
							if (comparable.CompareTo(_values[i].ComparableValue) == 0)
							{
								result = i;
								return result;
							}
						}
						else
						{
							flag = true;
						}
					}
					catch
					{
						flag = true;
					}
					if (flag && _values[i].Value == Value)
					{
						result = i;
						return result;
					}
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		public string GetValueForEnumeration(IEnumerationValue value)
		{
			if (value is EnumValue)
			{
				return ((EnumValue)(object)value).Value;
			}
			return "";
		}

		public IEnumerationValue GetEnumerationValue(object baseValue, out int nIndex)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Invalid comparison between Unknown and I4
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Invalid comparison between Unknown and I4
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			IConverterFromIEC converterFromIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
			try
			{
				object obj = default(object);
				TypeClass val = default(TypeClass);
				converterFromIEC.GetLiteralValue(Convert.ToString(baseValue), out obj, out val);
				if ((int)val == 29)
				{
					converterFromIEC.GetLiteralValue(_baseType.BaseType.ToUpperInvariant() + "#" + Convert.ToString(obj), out obj, out val);
				}
				object obj2 = default(object);
				TypeClass val2 = default(TypeClass);
				for (int i = 0; i < _values.Length; i++)
				{
					converterFromIEC.GetLiteralValue(_values[i].Value, out obj2, out val2);
					if ((int)val2 == 29)
					{
						converterFromIEC.GetLiteralValue(_baseType.BaseType.ToUpperInvariant() + "#" + Convert.ToString(obj2), out obj2, out val2);
					}
					if (val2 == val && object.Equals(obj2, obj))
					{
						nIndex = i;
						return (IEnumerationValue)(object)_values[i];
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			nIndex = -1;
			return null;
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
		}

		internal override void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			EnumType enumType = (EnumType)definition;
			_values = new EnumValue[enumType.Values.Count];
			for (int i = 0; i < enumType.Values.Count; i++)
			{
				_values[i] = new EnumValue((EnumTypeValue)(object)enumType.Values[i]);
				if (bUpdate && _baseType != null && _baseType.Value == _values[i].Value)
				{
					flag = true;
				}
			}
			if (bUpdate && !flag)
			{
				bUpdate = false;
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			ValueElement valueElement;
			if (defaultValue != null && defaultValue.Count > 0)
			{
				valueElement = TranslateDefault(defaultValue[0]);
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
			LList<ValueElement> val = new LList<ValueElement>();
			val.Add(valueElement);
			_baseType = factory.Create(stIdentifier, val, enumType.BaseType, visibleName, unit, description, filterFlags, this, _baseType, bUpdate, bCreateBitChannels: false);
		}

		internal void Import(EnumValue[] values, string baseTypeName, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			if (bUpdate && _baseType != null && _baseType.Value == _baseType.DefaultValue)
			{
				bUpdate = false;
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			_values = values;
			ValueElement valueElement = ((defaultValue == null || defaultValue.Count <= 0) ? new ValueElement() : TranslateDefault(defaultValue[0]));
			LList<ValueElement> val = new LList<ValueElement>();
			val.Add(valueElement);
			_baseType = factory.Create(stIdentifier, val, baseTypeName, visibleName, unit, description, filterFlags, this, _baseType, bUpdate, bCreateBitChannels: false);
		}

		internal void Import2(EnumValue[] values, LList<ValueElement> defaultValue, DataElementBase baseType)
		{
			_values = values;
			ValueElement valueElement = ((defaultValue == null || defaultValue.Count <= 0) ? new ValueElement() : TranslateDefault(defaultValue[0]));
			new LList<ValueElement>().Add(valueElement);
			_baseType = baseType;
		}

		internal ValueElement TranslateDefault(ValueElement defaultValue)
		{
			string value = defaultValue.Value;
			EnumValue[] values = _values;
			foreach (EnumValue enumValue in values)
			{
				if (enumValue.Identifier == value)
				{
					return new ValueElement(enumValue.Value);
				}
			}
			return defaultValue;
		}

		internal override void SetDefault(ValueElement defaultValue)
		{
			_baseType.SetDefault(TranslateDefault(defaultValue));
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
			return "Enumeration of " + _baseType.GetTypeString();
		}

		internal override ushort GetTypeId()
		{
			return _baseType.GetTypeId();
		}

		public override void SetValue(string stValue)
		{
			_baseType.SetValue(stValue);
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on enum types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on enum types");
		}

		public override void SetDefaultValue(string stValue)
		{
			_baseType.SetDefaultValue(stValue);
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			_baseType.Mapping = base.Mapping;
			_baseType.AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, bAlwaysMapping, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
			_baseType.UpdateLanguageModelGuids(bUpgrade, stPath);
		}

		public override long GetBitOffset(IDataElement child)
		{
			throw new InvalidOperationException("Enum types do not have children.");
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		public override bool Merge(DataElementBase element)
		{
			bool result = false;
			try
			{
				if (element.HasBaseType)
				{
					if (element.BaseType.Equals(BaseType, StringComparison.InvariantCultureIgnoreCase))
					{
						if (GetEnumerationValue(element.Value, out var _) != null)
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
	}
}
