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
	[TypeGuid("{08a69302-82ff-4bf9-a55d-a6014ee63990}")]
	[StorageVersion("3.3.0.0")]
	internal class DataElementSimpleType : DataElementBase
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Value")]
		[StorageVersion("3.3.0.0")]
		protected string _stValue = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Type")]
		[StorageVersion("3.3.0.0")]
		protected string _stType = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Default")]
		[StorageVersion("3.3.0.0")]
		protected string _stDefault = "";

		public override bool SupportsSetValue => true;

		public override bool HasSubElements => false;

		public override bool IsRangeType => false;

		public override bool IsEnumeration => false;

		public override IDataElement this[string stIdentifier]
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
		}

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
		}

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
		}

		public override string DefaultValue => _stDefault;

		public override string Value
		{
			get
			{
				return _stValue;
			}
			set
			{
				if (_stValue != value)
				{
					_stValue = value;
					Notify((IDataElement)(object)this, new string[0]);
				}
			}
		}

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on simple types");
			}
		}

		public override string BaseType => _stType;

		public override bool HasBaseType => true;

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
				return false;
			}
		}

		public DataElementSimpleType()
		{
		}

		internal DataElementSimpleType(DataElementSimpleType original)
			: base(original)
		{
			_stValue = original._stValue;
			_stType = original._stType;
			_stDefault = original._stDefault;
		}

		public override object Clone()
		{
			DataElementSimpleType dataElementSimpleType = new DataElementSimpleType(this);
			((GenericObject)dataElementSimpleType).AfterClone();
			return dataElementSimpleType;
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
			return GetStandardTypeId(_stType, ushort.MaxValue);
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

		internal override void Import(TypeDefinition definition, DataElementFactory dataElementFactory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			throw new InvalidOperationException("Not supported for DataElementSimpleType");
		}

		internal void Import(string stType, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			_stType = string.Intern(stType);
			if (defaultValue != null && defaultValue.Count > 0 && defaultValue[0].Value != null)
			{
				_stDefault = defaultValue[0].Value;
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			if (!bUpdate)
			{
				_stValue = _stDefault;
			}
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
			if (_stType == "BIT")
			{
				return "BOOL";
			}
			if (_stType == "SAFEBIT")
			{
				return "SAFEBOOL";
			}
			return _stType;
		}

		internal override string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue)
		{
			bDefault = _stDefault != null && _stDefault.Trim().Equals(_stValue);
			if (!CheckDefaultValueInitialisation(bIsOutput, ref bCreateDefaultValue))
			{
				return string.Empty;
			}
			if (_stValue == null || _stValue.Trim() == "")
			{
				return "";
			}
			return base.PositionPragma + _stValue;
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			if (((ICollection)base.IoMapping.VariableMappings).Count == 0 && bAlwaysMapping)
			{
				IDirectVariable iecAddress = _ioMapping.GetIecAddress(htStartAddresses);
				if (IsSubElementMapping(comcon, iecAddress, this, directVarCRefs, htStartAddresses))
				{
					IAddressAssignmentStrategy strategy = _ioMapping.Strategy;
					if (strategy != null && strategy is IAddressAssignmentStrategy2)
					{
						for (int i = 0; i < GetBitSize(); i++)
						{
							ChannelMap channelMap = new ChannelMap(lParameterId, 1L, bInput, bReadOnly, bAlwaysMapping, (IDataElement)(object)this, mappingMode);
							IDirectVariable val2 = (channelMap.IecAddress = ((IAddressAssignmentStrategy2)((strategy is IAddressAssignmentStrategy2) ? strategy : null)).CalcBitAddress(iecAddress, i));
							channelMap.ParamBitoffset = (ushort)i;
							channelMap.Type = "BIT";
							channelMap.LanguageModelPositionId = base.LanguageModelPositionId;
							AddMappingUnused(cm, channelMap);
							cm.AddChannelMap(channelMap);
						}
						return;
					}
				}
			}
			if (((ICollection)base.IoMapping.VariableMappings).Count == 0 && !bAlwaysMapping)
			{
				return;
			}
			ChannelMap channelMap2 = new ChannelMap(lParameterId, (ushort)GetBitSize(), bInput, bReadOnly, bAlwaysMapping, (IDataElement)(object)this, mappingMode);
			channelMap2.IecAddress = _ioMapping.GetIecAddress(htStartAddresses);
			channelMap2.ParamBitoffset = (ushort)lOffset;
			channelMap2.Type = GetTypeName("");
			channelMap2.LanguageModelPositionId = base.LanguageModelPositionId;
			channelMap2.Comment = base.Description;
			if (bAlwaysMapping)
			{
				AddMappingUnused(cm, channelMap2);
			}
			foreach (VariableMapping item in (IEnumerable)base.IoMapping.VariableMappings)
			{
				channelMap2.AddVariableMapping(item);
			}
			cm.AddChannelMap(channelMap2);
		}

		public override long GetBitOffset(IDataElement child)
		{
			throw new InvalidOperationException("Simple types do not have children.");
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		public override long GetBitSize()
		{
			return Types.GetBitSize(_stType);
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on simple types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on simple types");
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
