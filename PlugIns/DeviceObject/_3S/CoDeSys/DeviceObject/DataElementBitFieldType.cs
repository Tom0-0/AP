#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{bd387d12-7a68-42ff-9fa4-1e4e26e0dc37}")]
	[StorageVersion("3.3.0.0")]
	public class DataElementBitFieldType : DataElementBase
	{
		[DefaultSerialization("BaseType")]
		[StorageVersion("3.3.0.0")]
		private string _stBaseType = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected string BaseTypeSerialization
		{
			get
			{
				return _stBaseType;
			}
			set
			{
				_stBaseType = string.Intern(value);
			}
		}

		public override bool HasSubElements
		{
			get
			{
				if (base.SubElementCollection == null)
				{
					return false;
				}
				return base.SubElementCollection.Count > 0;
			}
		}

		public override bool IsRangeType => false;

		public override bool IsEnumeration => false;

		public override IDataElement this[string stIdentifier] => base.SubElementCollection[stIdentifier];

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on bitfield types");
			}
		}

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on bitfield types");
			}
		}

		public override string DefaultValue => "";

		public override string Value
		{
			get
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected O, but got Unknown
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				stringBuilder.Append("{");
				if (base.SubElementCollection != null)
				{
					foreach (IDataElement item in base.SubElementCollection)
					{
						IDataElement val = item;
						if (flag)
						{
							flag = false;
						}
						else
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(val.Value);
					}
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}
			set
			{
				if (base.SubElementCollection != null)
				{
					if (base.SubElementCollection.Count <= 0)
					{
						return;
					}
					IConverterFromIEC converterFromIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
					object value2 = null;
					try
					{
						TypeClass val = default(TypeClass);
						converterFromIEC.GetLiteralValue(value, out value2, out val);
						ulong num = Convert.ToUInt64(value2);
						for (int i = 0; i < base.SubElementCollection.Count; i++)
						{
							bool flag = (num & (ulong)(1 << i)) != 0;
							base.SubElementCollection[i].Value=(Convert.ToString(flag).ToUpperInvariant());
						}
						return;
					}
					catch
					{
					}
				}
				throw new InvalidOperationException("Operation not allowed on bitfield types");
			}
		}

		public override bool SupportsSetValue => true;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on bitfield types");
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on bitfield types");
			}
		}

		public override string BaseType => _stBaseType;

		public override bool HasBaseType
		{
			get
			{
				if (_stBaseType != null)
				{
					return _stBaseType != "";
				}
				return false;
			}
		}

		public override bool CanWatch
		{
			get
			{
				Parameter parameter = GetParameter();
				if (parameter != null)
				{
					if (parameter.Download && string.IsNullOrEmpty(parameter.FbInstanceVariable))
					{
						return !parameter.ParamType.ToLowerInvariant().StartsWith("external:");
					}
					return false;
				}
				return true;
			}
		}

		public DataElementBitFieldType()
		{
		}

		internal DataElementBitFieldType(DataElementBitFieldType original)
			: base(original)
		{
			_stBaseType = string.Intern(original._stBaseType);
		}

		public override object Clone()
		{
			DataElementBitFieldType dataElementBitFieldType = new DataElementBitFieldType(this);
			((GenericObject)dataElementBitFieldType).AfterClone();
			return dataElementBitFieldType;
		}

		public override void SetValue(string stValue)
		{
			Value = stValue;
		}

		public override string GetTypeString()
		{
			if (HasBaseType)
			{
				return BaseType;
			}
			return string.Empty;
		}

		internal override ushort GetTypeId()
		{
			if (HasBaseType)
			{
				return GetStandardTypeId(_stBaseType, 28);
			}
			return 28;
		}

		public override IOnlineVarRef CreateWatch()
		{
			return (IOnlineVarRef)(object)CreateWatch(bClientControlled: false);
		}

		public override IOnlineVarRef2 CreateWatch(bool bClientControlled)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			IIoProvider ioProvider = base.IoProvider;
			if (ioProvider == null)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)11, "Could not create watch: No IoProvider");
			}
			if (ioProvider.GetHost() == null)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)10, "Could not create watch: No host");
			}
			Parameter parameter = GetParameter();
			Debug.Assert(parameter != null);
			long bitOffset = Parent.GetBitOffset((IDataElement)(object)this);
			if (bitOffset > int.MaxValue)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)18, "Offset to large. Value cannot be monitored");
			}
			int nBitOffset = (int)bitOffset;
			int nConnectorId = ((ioProvider is IConnector) ? ((IConnector)ioProvider).ConnectorId : (-1));
			IMetaObject metaObject = ioProvider.GetMetaObject();
			return (IOnlineVarRef2)(object)new DataElementOnlineVarRef(metaObject.ProjectHandle, metaObject.ObjectGuid, nConnectorId, parameter.Id, nBitOffset, GetTypeString(), bClientControlled);
		}

		internal override string GetTypeName(string stBaseName)
		{
			return _stBaseType;
		}

		internal override string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue)
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Invalid comparison between Unknown and I4
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			CheckDefaultValueInitialisation(bIsOutput, ref bCreateDefaultValue);
			ulong num = 0uL;
			int num2 = 0;
			bDefault = false;
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					bool bDefault2;
					string initialization = item.GetInitialization(out bDefault2, bIsOutput, bCreateDefaultValue);
					bDefault &= bDefault2;
					if (initialization != null && initialization != string.Empty && (initialization.ToUpperInvariant() == "TRUE" || initialization == "1"))
					{
						num |= (ulong)(1L << num2);
					}
					num2++;
				}
			}
			if (num == 0L && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)2, (ushort)0))
			{
				return string.Empty;
			}
			IConverterToIEC converterToIEC = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)2);
			string result = string.Empty;
			try
			{
				TypeClass val = (TypeClass)GetTypeId();
				if ((int)val != 28)
				{
					result = converterToIEC.GetInteger((object)num, val);
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
		}

		public override long GetBitSize()
		{
			if (_stBaseType != "")
			{
				return Types.GetBitSize(_stBaseType);
			}
			long num = 0L;
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					num += item.GetBitSize();
				}
				return num;
			}
			return num;
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (((ICollection)base.IoMapping.VariableMappings).Count == 0 && bAlwaysMapping && IsSubElementMapping(comcon, _ioMapping.GetIecAddress(htStartAddresses), this, directVarCRefs, htStartAddresses))
			{
				flag = true;
			}
			if ((((ICollection)base.IoMapping.VariableMappings).Count > 0 || bAlwaysMapping) && _stBaseType.Length > 0 && !flag)
			{
				ChannelMap channelMap = new ChannelMap(lParameterId, (ushort)GetBitSize(), bInput, bReadOnly, bAlwaysMapping, (IDataElement)(object)this, mappingMode);
				channelMap.IecAddress = _ioMapping.GetIecAddress(htStartAddresses);
				channelMap.ParamBitoffset = (ushort)lOffset;
				channelMap.Type = _stBaseType;
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
			if (base.SubElementCollection == null)
			{
				return;
			}
			foreach (DataElementBase item2 in base.SubElementCollection)
			{
				bool bAlwaysMapping2 = flag;
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)30) && !flag && bAlwaysMapping && ((ICollection)item2.IoMapping.VariableMappings).Count > 0 && !item2.IoMapping.VariableMappings[0].CreateVariable)
				{
					bAlwaysMapping2 = true;
				}
				item2.AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, bAlwaysMapping2, mappingMode, _stBaseType, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				lOffset += item2.GetBitSize();
			}
		}

		public override long GetBitOffset(IDataElement child)
		{
			long num = Parent.GetBitOffset((IDataElement)(object)this);
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					if (item.Identifier.Equals(child.Identifier))
					{
						return num;
					}
					num += item.GetBitSize();
				}
			}
			throw new ArgumentException($"'{child.Identifier}' is not a child of this dataelement");
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		internal void Import(string baseTypeName, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			if (defaultValue != null && defaultValue.Count > 0)
			{
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			_stBaseType = baseTypeName;
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			long bitSize = Types.GetBitSize(_stBaseType);
			int num = 0;
			ulong num2 = 0uL;
			if (defaultValue != null && defaultValue.Count > 0)
			{
				if (!string.IsNullOrEmpty(defaultValue[0].Value))
				{
					try
					{
						object value = default(object);
						TypeClass val = default(TypeClass);
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC().GetLiteralValue(defaultValue[0].Value, out value, out val);
						num2 = Convert.ToUInt64(value);
					}
					catch
					{
					}
				}
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			for (long num3 = 0L; num3 < bitSize; num3++)
			{
				DataElementBitFieldComponent dataElementBitFieldComponent = null;
				string stIdentifier2 = stIdentifier + "_" + num3;
				if (bUpdate && dataElementCollection.Contains(stIdentifier2))
				{
					dataElementBitFieldComponent = (DataElementBitFieldComponent)(object)dataElementCollection[stIdentifier2];
				}
				string stDefault = "Bit" + num3;
				StringRef visibleName2 = new StringRef(string.Empty, string.Empty, stDefault);
				ValueElement valueElement = new ValueElement(((num2 & (ulong)(1 << num)) != 0L) ? "TRUE" : "FALSE");
				LList<ValueElement> val2 = new LList<ValueElement>();
				val2.Add(valueElement);
				string stType = "std:BOOL";
				if (_stBaseType.ToUpperInvariant().StartsWith("SAFE"))
				{
					stType = "std:SAFEBOOL";
				}
				if (dataElementBitFieldComponent != null)
				{
					DataElementBase dataElementBase = factory.Create(stIdentifier2, val2, stType, visibleName2, unit, description, filterFlags, this, dataElementBitFieldComponent, bUpdate: true, bCreateBitChannels: false);
					if (dataElementBase is DataElementSimpleType)
					{
						dataElementBitFieldComponent.Import((DataElementSimpleType)dataElementBase, visibleName2, unit, description, new CustomItemList(), bUpdate: true, this);
						base.SubElementCollection.Add((IDataElement)(object)dataElementBitFieldComponent);
					}
				}
				else
				{
					DataElementBase dataElementBase2 = factory.Create(stIdentifier2, val2, stType, visibleName2, unit, description, filterFlags, this, dataElementBitFieldComponent, bUpdate: false, bCreateBitChannels: false);
					if (dataElementBase2 is DataElementSimpleType)
					{
						DataElementBitFieldComponent dataElementBitFieldComponent2 = new DataElementBitFieldComponent();
						dataElementBitFieldComponent2.Import((DataElementSimpleType)dataElementBase2, visibleName2, unit, description, new CustomItemList(), bUpdate: false, this);
						base.SubElementCollection.Add((IDataElement)(object)dataElementBitFieldComponent2);
					}
				}
				num++;
			}
		}

		internal override void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			BitfieldType bitfieldType = (BitfieldType)definition;
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			if (defaultValue != null && defaultValue.Count > 0)
			{
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			_stBaseType = string.Intern(bitfieldType.BaseType);
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			foreach (StructComponent component in bitfieldType.Components)
			{
				DataElementBitFieldComponent dataElementBitFieldComponent = null;
				if (bUpdate && dataElementCollection.Contains(component.Identifier))
				{
					dataElementBitFieldComponent = (DataElementBitFieldComponent)(object)dataElementCollection[component.Identifier];
				}
				if (bUpdate && ((int)component.GetAccessRight(bOnline: false) & 2) == 0)
				{
					dataElementBitFieldComponent = null;
				}
				ValueElement valueElement = null;
				if (defaultValue != null && defaultValue.Count > 0)
				{
					valueElement = defaultValue[0].GetSubElement(component.Identifier);
				}
				if (valueElement == null)
				{
					valueElement = component.Default;
				}
				LList<ValueElement> val = new LList<ValueElement>();
				if (valueElement != null)
				{
					val.Add(valueElement);
				}
				DataElementBase dataElementBase = factory.Create(component.Identifier, val, component.Type, component.VisibleName, component.Unit, component.Description, filterFlags, this, null, bUpdate: false, bCreateBitChannels: false);
				if (!(dataElementBase is DataElementSimpleType) && !(dataElementBase is DataElementEnumType))
				{
					throw new ArgumentException("Invalid Bitfieldcomponent");
				}
				DataElementBitFieldComponent dataElementBitFieldComponent2;
				if (dataElementBitFieldComponent != null)
				{
					if (dataElementBase is DataElementSimpleType)
					{
						dataElementBitFieldComponent.Import((DataElementSimpleType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: true, this);
					}
					if (dataElementBase is DataElementEnumType)
					{
						dataElementBitFieldComponent.Import((DataElementEnumType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: true, this);
					}
					dataElementBitFieldComponent2 = dataElementBitFieldComponent;
				}
				else
				{
					dataElementBitFieldComponent2 = new DataElementBitFieldComponent();
					if (dataElementBase is DataElementSimpleType)
					{
						dataElementBitFieldComponent2.Import((DataElementSimpleType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: false, this);
					}
					if (dataElementBase is DataElementEnumType)
					{
						dataElementBitFieldComponent2.Import((DataElementEnumType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: false, this);
					}
				}
				if (valueElement == null || !valueElement.HasOfflineAccessRight)
				{
					dataElementBitFieldComponent2.SetAccessRight(bOnline: false, component.GetAccessRight(bOnline: false));
				}
				if (valueElement == null || !valueElement.HasOnlineAccessRight)
				{
					dataElementBitFieldComponent2.SetAccessRight(bOnline: true, component.GetAccessRight(bOnline: true));
				}
				base.SubElementCollection.Add((IDataElement)(object)dataElementBitFieldComponent2);
			}
		}

		internal void Import(LList<StructComponent> components, string baseTypeName, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			if (defaultValue != null && defaultValue.Count > 0)
			{
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			_stBaseType = string.Intern(baseTypeName);
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			foreach (StructComponent component in components)
			{
				DataElementBitFieldComponent dataElementBitFieldComponent = null;
				if (bUpdate && dataElementCollection.Contains(component.Identifier))
				{
					dataElementBitFieldComponent = (DataElementBitFieldComponent)(object)dataElementCollection[component.Identifier];
				}
				ValueElement valueElement = null;
				if (defaultValue != null && defaultValue.Count > 0)
				{
					valueElement = defaultValue[0].GetSubElement(component.Identifier);
				}
				if (valueElement == null)
				{
					valueElement = component.Default;
				}
				LList<ValueElement> val = new LList<ValueElement>();
				if (valueElement != null)
				{
					val.Add(valueElement);
				}
				DataElementBase dataElementBase = factory.Create(component.Identifier, val, component.Type, component.VisibleName, component.Unit, component.Description, filterFlags, this, null, bUpdate: false, bCreateBitChannels: false);
				if (!(dataElementBase is DataElementSimpleType) && !(dataElementBase is DataElementEnumType))
				{
					throw new ArgumentException("Invalid Bitfieldcomponent");
				}
				DataElementBitFieldComponent dataElementBitFieldComponent2;
				if (dataElementBitFieldComponent != null)
				{
					if (dataElementBase is DataElementSimpleType)
					{
						dataElementBitFieldComponent.Import((DataElementSimpleType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: true, this);
					}
					if (dataElementBase is DataElementEnumType)
					{
						dataElementBitFieldComponent.Import((DataElementEnumType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: true, this);
					}
					dataElementBitFieldComponent2 = dataElementBitFieldComponent;
				}
				else
				{
					dataElementBitFieldComponent2 = new DataElementBitFieldComponent();
					if (dataElementBase is DataElementSimpleType)
					{
						dataElementBitFieldComponent2.Import((DataElementSimpleType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: false, this);
					}
					if (dataElementBase is DataElementEnumType)
					{
						dataElementBitFieldComponent2.Import((DataElementEnumType)dataElementBase, component.VisibleName, component.Unit, component.Description, component.CustomItems, bUpdate: false, this);
					}
				}
				if (valueElement == null || !valueElement.HasOfflineAccessRight)
				{
					dataElementBitFieldComponent2.SetAccessRight(bOnline: false, component.GetAccessRight(bOnline: false));
				}
				if (valueElement == null || !valueElement.HasOnlineAccessRight)
				{
					dataElementBitFieldComponent2.SetAccessRight(bOnline: true, component.GetAccessRight(bOnline: true));
				}
				base.SubElementCollection.Add((IDataElement)(object)dataElementBitFieldComponent2);
			}
		}

		internal override void SetDefault(ValueElement defaultValue)
		{
			foreach (ValueElement subElement in defaultValue.SubElements)
			{
				((DataElementBase)(object)base.SubElementCollection[subElement.Name])?.SetDefault(subElement);
			}
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on bitfield types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on bitfield types");
		}

		public override void SetDefaultValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on bitfield types");
		}
	}
}
