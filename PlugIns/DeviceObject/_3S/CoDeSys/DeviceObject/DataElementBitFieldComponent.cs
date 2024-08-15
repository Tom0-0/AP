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
	[TypeGuid("{1944b378-8867-41f4-b1f4-6e5b1e6547ba}")]
	[StorageVersion("3.3.0.0")]
	public class DataElementBitFieldComponent : DataElementBase, IEnumerationDataElement
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Value")]
		[StorageVersion("3.3.0.0")]
		protected string _stValue = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected string _stType = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Default")]
		[StorageVersion("3.3.0.0")]
		protected string _stDefault = "";

		[DefaultSerialization("BaseType")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private DataElementBase _baseType;

		[DefaultSerialization("Type")]
		[StorageVersion("3.3.0.0")]
		protected string TypeSerialization
		{
			get
			{
				return _stType;
			}
			set
			{
				_stType = string.Intern(value);
			}
		}

		internal override IDataElementParent Parent
		{
			set
			{
				base.Parent = value;
				if (_baseType != null)
				{
					_baseType.Parent = value;
				}
			}
		}

		public override bool HasSubElements => false;

		public override bool IsRangeType => false;

		public override bool IsEnumeration
		{
			get
			{
				if (_baseType == null)
				{
					return false;
				}
				return _baseType.IsEnumeration;
			}
		}

		public override IDataElement this[string stIdentifier]
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on DataElementBitFieldComponent types");
			}
		}

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on DataElementBitFieldComponent types");
			}
		}

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on DataElementBitFieldComponent types");
			}
		}

		public override string DefaultValue => _stDefault;

		public override string Value
		{
			get
			{
				if (_baseType is DataElementEnumType)
				{
					return _baseType.Value;
				}
				return _stValue;
			}
			set
			{
				if (_baseType is DataElementEnumType)
				{
					if (_baseType.Value != value)
					{
						_baseType.Value = value;
						Notify((IDataElement)(object)this, new string[0]);
					}
				}
				else if (_stValue != value)
				{
					_stValue = value;
					Notify((IDataElement)(object)this, new string[0]);
				}
			}
		}

		public override bool SupportsSetValue => true;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				if (_baseType is DataElementEnumType)
				{
					return _baseType.EnumerationValue;
				}
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
			set
			{
				if (_baseType is DataElementEnumType)
				{
					_baseType.EnumerationValue = value;
					return;
				}
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
		}

		public override string BaseType => _stType;

		public override bool CanWatch
		{
			get
			{
				if (_baseType != null)
				{
					return _baseType.CanWatch;
				}
				return false;
			}
		}

		public override bool HasBaseType => true;

		public IEnumerationValue[] EnumerationValues
		{
			get
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (_baseType is IEnumerationDataElement)
				{
					return ((IEnumerationDataElement)_baseType).EnumerationValues;
				}
				throw new InvalidOperationException("Property only available for enumeration types");
			}
		}

		public DataElementBitFieldComponent()
		{
		}

		internal DataElementBitFieldComponent(DataElementBitFieldComponent original)
			: base(original)
		{
			_stValue = original._stValue;
			_stType = string.Intern(original._stType);
			_stDefault = original._stDefault;
			if (original._baseType != null)
			{
				_baseType = (DataElementBase)((GenericObject)original._baseType).Clone();
			}
		}

		public override object Clone()
		{
			DataElementBitFieldComponent dataElementBitFieldComponent = new DataElementBitFieldComponent(this);
			((GenericObject)dataElementBitFieldComponent).AfterClone();
			return dataElementBitFieldComponent;
		}

		public override void SetValue(string stValue)
		{
			_stValue = stValue;
		}

		public override string GetTypeString()
		{
			return _stType;
		}

		internal override ushort GetTypeId()
		{
			throw new InvalidOperationException("GetTypeId not supported for class DataElementBitFieldComponent");
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
			return (IOnlineVarRef2)(object)new DataElementOnlineVarRef(metaObject.ProjectHandle, metaObject.ObjectGuid, nConnectorId, parameter.Id, nBitOffset, "BIT", bClientControlled);
		}

		internal override void Import(TypeDefinition definition, DataElementFactory dataElementFactory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			throw new NotSupportedException("");
		}

		internal void Import(DataElementSimpleType elem, StringRef visibleName, StringRef unit, StringRef description, CustomItemList customItems, bool bUpdate, IDataElementParent parent)
		{
			ImportBase(elem.Identifier, visibleName, unit, description, elem.FilterFlags, customItems, bUpdate, parent);
			if (elem.BaseType != "BOOL" && elem.BaseType != "SAFEBOOL")
			{
				throw new ArgumentException("Invalid Bitfieldcomponent. Only BOOL types allowed.");
			}
			_stType = string.Intern(elem.BaseType);
			if (elem.DefaultValue != null)
			{
				_stDefault = elem.DefaultValue;
			}
			if (!bUpdate)
			{
				_stValue = _stDefault;
			}
			_baseType = elem;
		}

		internal void Import(DataElementEnumType elem, StringRef visibleName, StringRef unit, StringRef description, CustomItemList customItems, bool bUpdate, IDataElementParent parent)
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			ImportBase(elem.Identifier, visibleName, unit, description, elem.FilterFlags, customItems, bUpdate, parent);
			if (elem.BaseType != "BOOL" && elem.BaseType != "SAFEBOOL")
			{
				throw new ArgumentException("Invalid Bitfieldcomponent. Only BOOL types allowed.");
			}
			_stType = elem.BaseType;
			if (elem.DefaultValue != null)
			{
				_stDefault = elem.DefaultValue;
			}
			if (!bUpdate)
			{
				_stValue = _stDefault;
			}
			_accessRightOffline = elem.GetAccessRight(bOnline: false);
			_accessRightOnline = elem.GetAccessRight(bOnline: true);
			_baseType = elem;
		}

		internal override void SetDefault(ValueElement defaultValue)
		{
			if (defaultValue != null && defaultValue.Value != null)
			{
				_stDefault = defaultValue.Value;
				_stValue = _stDefault;
			}
		}

		internal override string GetTypeName(string stBaseName)
		{
			return _stType;
		}

		internal override string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue)
		{
			bDefault = _stDefault != null && _stDefault.Equals(_stValue);
			if (!CheckDefaultValueInitialisation(bIsOutput, ref bCreateDefaultValue))
			{
				return string.Empty;
			}
			if (_baseType is DataElementEnumType)
			{
				bDefault = _baseType.DefaultValue != null && _baseType.DefaultValue.Equals(_baseType.Value);
				return _baseType.Value;
			}
			return _stValue;
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			if (((ICollection)base.IoMapping.VariableMappings).Count == 0 && !bAlwaysMapping)
			{
				return;
			}
			ChannelMap channelMap = new ChannelMap(lParameterId, (ushort)GetBitSize(), bInput, bReadOnly, bAlwaysMapping, (IDataElement)(object)this, mappingMode);
			IDirectVariable val = _ioMapping.GetIecAddress(htStartAddresses);
			if (bMotorolaBitfield)
			{
				IDataElement val2 = null;
				DataElementCollection dataElementCollection = _baseType.Parent as DataElementCollection;
				if (dataElementCollection != null)
				{
					IDataElementParent parent = dataElementCollection.Parent;
					val2 = (IDataElement)((parent is IDataElement) ? parent : null);
				}
				if (val2 != null)
				{
					FixedIecAddress fixedIecAddress = new FixedIecAddress();
					fixedIecAddress.Location = val.Location;
					fixedIecAddress.Size = val.Size;
					fixedIecAddress.Components = new int[val.Components.Length];
					for (int i = 0; i < val.Components.Length; i++)
					{
						fixedIecAddress.Components[i] = val.Components[i];
					}
					long bitSize = val2.GetBitSize();
					if (bitSize == 16 || bitSize == 32 || bitSize == 64)
					{
						int num = (int)(bitSize / 8);
						fixedIecAddress.Components[0] += num - 1 - val.Components[0] % num * 2;
						lOffset += (num - 1 - lOffset / 8 % num * 2) * 8;
					}
					val = (IDirectVariable)(object)fixedIecAddress;
				}
			}
			channelMap.IecAddress = val;
			channelMap.ParamBitoffset = (ushort)lOffset;
			channelMap.Type = _stType;
			channelMap.ParentType = stParentType;
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

		public override long GetBitSize()
		{
			return 1L;
		}

		public override long GetBitOffset(IDataElement child)
		{
			throw new InvalidOperationException("Bitfield components do not have children.");
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		public int GetValueEnumerationIndex()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_baseType is IEnumerationDataElement)
			{
				return ((IEnumerationDataElement)_baseType).GetValueEnumerationIndex();
			}
			throw new InvalidOperationException("Method only available for enumeration types");
		}

		public string GetValueForEnumeration(IEnumerationValue enumValue)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_baseType is IEnumerationDataElement)
			{
				return ((IEnumerationDataElement)_baseType).GetValueForEnumeration(enumValue);
			}
			throw new InvalidOperationException("Method only available for enumeration types");
		}

		public IEnumerationValue GetEnumerationValue(object baseValue, out int nIndex)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_baseType is IEnumerationDataElement)
			{
				return ((IEnumerationDataElement)_baseType).GetEnumerationValue(baseValue, out nIndex);
			}
			throw new InvalidOperationException("Method only available for enumeration types");
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on DataElementBitFieldComponent types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on DataElementBitFieldComponent types");
		}

		public override void SetDefaultValue(string stValue)
		{
			if (_stDefault != stValue)
			{
				_stDefault = stValue;
				Notify((IDataElement)(object)this, new string[0]);
			}
		}
	}
}
