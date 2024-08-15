using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{8c994079-f3c6-4484-9643-9258628feecd}")]
	[StorageVersion("3.3.0.0")]
	public class Parameter : GenericObject2, IParameter21, IParameter20, IParameter19, IParameter18, IParameter17, IParameter16, IParameter15, IParameter14, IParameter13, IParameter12, IParameter11, IParameter10, IParameter9, IParameter8, IParameter7, IParameter6, IParameter5, IParameter4, IParameter3, IParameter2, IParameter, IDataElement, IComparable, IDataElementParent, IEnumerationDataElement, IDataElement12, IDataElement11, IDataElement10, IDataElement9, IDataElement8, IDataElement7, IDataElement6, IDataElement5, IDataElement4, IDataElement3, IDataElement2, IGenericInterfaceExtensionProvider
	{
		[DefaultDuplication((DuplicationMethod)1)]
		[DefaultSerialization("Id")]
		[StorageVersion("3.3.0.0")]
		private uint _uiId = uint.MaxValue;

		[DefaultDuplication(0)]
		[DefaultSerialization("OfflineAccess")]
		[StorageVersion("3.3.0.0")]
		private AccessRight _accessRightOffline = (AccessRight)3;

		[DefaultDuplication(0)]
		[DefaultSerialization("OnlineAccess")]
		[StorageVersion("3.3.0.0")]
		private AccessRight _accessRightOnline = (AccessRight)3;

		[DefaultDuplication(0)]
		[DefaultSerialization("Download")]
		[StorageVersion("3.3.0.0")]
		private bool _bDownload = true;

		[DefaultDuplication(0)]
		[DefaultSerialization("Functional")]
		[StorageVersion("3.3.0.0")]
		private bool _bFunctional;

		[DefaultDuplication(0)]
		[DefaultSerialization("ChannelType")]
		[StorageVersion("3.3.0.0")]
		private ChannelType _channelType;

		[DefaultDuplication(0)]
		[DefaultSerialization("DiagType")]
		[StorageVersion("3.3.0.0")]
		private DiagType _diagType;

		[DefaultDuplication((DuplicationMethod)1)]
		[DefaultSerialization("CreateDownloadStructure")]
		[StorageVersion("3.3.0.0")]
		private bool _bCreateDownloadStructure = true;

		[DefaultDuplication((DuplicationMethod)1)]
		[DefaultSerialization("DalaElement")]
		[StorageVersion("3.3.0.0")]
		private DataElementBase _dataElement;

		[DefaultDuplication((DuplicationMethod)1)]
		[DefaultSerialization("SectionId")]
		[StorageVersion("3.3.0.0")]
		private int _nParentSectionId = -1;

		[DefaultDuplication(0)]
		[DefaultSerialization("FileType")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue(false)]
		private bool _bIsFileType;

		[DefaultDuplication(0)]
		[DefaultSerialization("ParamType")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue("")]
		private string _stParamType = string.Empty;

		[DefaultDuplication(0)]
		[DefaultSerialization("DriverSpecific")]
		[StorageVersion("3.5.1.0")]
		[StorageDefaultValue("")]
		private string _stDriverSpecific = string.Empty;

		[DefaultDuplication(0)]
		[DefaultSerialization("ConstantValue")]
		[StorageVersion("3.5.2.0")]
		[StorageDefaultValue(false)]
		private bool _bConstantValue;

		[DefaultDuplication(0)]
		[DefaultSerialization("OnlineHelpUrl")]
		[StorageVersion("3.5.2.0")]
		[StorageDefaultValue("")]
		private string _stOnlineHelpUrl = string.Empty;

		[DefaultDuplication(0)]
		[DefaultSerialization("AlwaysMappingMode")]
		[StorageVersion("3.5.5.0")]
		[StorageDefaultValue(AlwaysMappingMode.OnlyIfUnused)]
		private AlwaysMappingMode _alwaysMappingMode;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("InstanceVariable")]
		[StorageVersion("3.5.7.0")]
		[StorageDefaultValue("")]
		private string _stInstanceVariable = string.Empty;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("PreparedValueAccess")]
		[StorageVersion("3.5.9.0")]
		[StorageDefaultValue(false)]
		protected bool _preparedValueAccess;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("UseRefactoring")]
		[StorageVersion("3.5.11.0")]
		[StorageDefaultValue(false)]
		protected bool _useRefactoring;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("DisableMapping")]
		[StorageVersion("3.5.12.20")]
		[StorageDefaultValue(false)]
		protected bool _disableMapping;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("IsBidirectionalOutput")]
		[StorageVersion("3.5.15.0")]
		[StorageDefaultValue(false)]
		private bool _bIsBidirectionalOutput;

		private ParameterSet _owner;

		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AlwaysMapping")]
		[StorageVersion("3.3.0.0")]
		private bool _bAlwaysMapping;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("CreateInHostConnector")]
		[StorageVersion("3.3.0.0")]
		private bool _bCreateInHostConnector;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("CreateInChildConnector")]
		[StorageVersion("3.3.0.0")]
		private bool _bCreateInChildConnector;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("OnlineParameter")]
		[StorageVersion("3.3.0.0")]
		private bool _bOnlineParameter;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("NoManualAddress")]
		[StorageVersion("3.4.0.0")]
		[StorageDefaultValue(false)]
		private bool _bNoManualAddress;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("OnlineChangeEnabled")]
		[StorageVersion("3.4.3.10")]
		[StorageDefaultValue(false)]
		private bool _bOnlineChangeEnable;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("TraceSystemRecord")]
		[StorageVersion("3.4.4.0")]
		[StorageDefaultValue(false)]
		private bool _bTraceSystemRecord;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("IndexInDevDesc")]
		[StorageVersion("3.3.0.0")]
		private long _lIndexInDevDesc = -1L;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("MapOnlyNew")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(false)]
		private bool _bMapOnlyNew;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("LogicalParameter")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(false)]
		private bool _bLogicalParameter;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("MapSize")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(0u)]
		private uint _uiMapSize;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("MapOffset")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(0u)]
		private uint _uiMapOffset;

		protected string _stParamName = string.Empty;

		protected string _stLastDownloadedValue = string.Empty;

		private string _stInstanceVariableWithDeviceName = string.Empty;

		public string ParamType => _stParamType;

		public bool IsExternalDatatype
		{
			get
			{
				bool result = false;
				if (!string.IsNullOrEmpty(_stParamType))
				{
					string[] array = _stParamType.Split(':');
					if (array.Length > 1)
					{
						result = array[0].Trim().ToLowerInvariant().Equals("external");
					}
				}
				return result;
			}
		}

		public IDataElement DataElement => (IDataElement)(object)this;

		public DataElementBase DataElementBase => _dataElement;

		public IIoProvider IoProvider
		{
			get
			{
				if (_owner != null)
				{
					return _owner.IoProvider;
				}
				return null;
			}
		}

		public bool IsParameter => true;

		public long Id
		{
			get
			{
				return _uiId;
			}
			internal set
			{
				_uiId = (uint)value;
			}
		}

		public bool Download => _bDownload;

		public ChannelType ChannelType => _channelType;

		public IParameterSection Section
		{
			get
			{
				if (_owner == null)
				{
					return null;
				}
				return (IParameterSection)(object)_owner.GetSection(_nParentSectionId);
			}
		}

		public DiagType DiagType
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _diagType;
			}
			set
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				if (value != _diagType)
				{
					_diagType = value;
					Notify((IDataElement)(object)this, new string[0]);
				}
			}
		}

		public bool CreateDownloadStructure
		{
			get
			{
				return _bCreateDownloadStructure;
			}
			set
			{
				_bCreateDownloadStructure = value;
			}
		}

		public string Identifier => _dataElement.Identifier;

		public string VisibleName => _dataElement.VisibleName;

		public string Description => _dataElement.Description;

		public string UserComment
		{
			get
			{
				return _dataElement.UserComment;
			}
			set
			{
				_dataElement.UserComment = value;
			}
		}

		public bool HasSubElements => _dataElement.HasSubElements;

		public bool IsRangeType => _dataElement.IsRangeType;

		public bool IsEnumeration => _dataElement.IsEnumeration;

		public IDataElementCollection SubElements => _dataElement.SubElements;

		public IDataElement this[string stIdentifier] => _dataElement[stIdentifier];

		public string MinValue => _dataElement.MinValue;

		public string MaxValue => _dataElement.MaxValue;

		public string DefaultValue => _dataElement.DefaultValue;

		public string Value
		{
			get
			{
				return _dataElement.Value;
			}
			set
			{
				_dataElement.Value = value;
			}
		}

		public IEnumerationValue EnumerationValue
		{
			get
			{
				return _dataElement.EnumerationValue;
			}
			set
			{
				_dataElement.EnumerationValue = value;
			}
		}

		public string BaseType => _dataElement.BaseType;

		public bool HasBaseType => _dataElement.HasBaseType;

		public ICustomItemList CustomItems => _dataElement.CustomItems;

		public string[] FilterFlags => _dataElement.FilterFlags;

		public string Unit => _dataElement.Unit;

		public IIoMapping IoMapping => _dataElement.IoMapping;

		public bool CanWatch
		{
			get
			{
				if (IsExternalDatatype)
				{
					return false;
				}
				return _dataElement.CanWatch;
			}
		}

		public IEnumerationValue[] EnumerationValues
		{
			get
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (_dataElement is IEnumerationDataElement)
				{
					return ((IEnumerationDataElement)_dataElement).EnumerationValues;
				}
				throw new InvalidOperationException("Property only available for enumeration types");
			}
		}

		public long LanguageModelPositionId => _dataElement.LanguageModelPositionId;

		public long EditorPositionId => _dataElement.EditorPositionId;

		internal ParameterSet Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}

		public bool BlobInit
		{
			get
			{
				if (CustomItems != null && ((ICollection)CustomItems).Count > 0)
				{
					foreach (CustomItem item in (IEnumerable)CustomItems)
					{
						if (item.Name == "NO_BLOB_INIT")
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public bool AlwaysMapping
		{
			get
			{
				return _bAlwaysMapping;
			}
			set
			{
				_bAlwaysMapping = value;
			}
		}

		public bool CreateInHostConnector
		{
			get
			{
				return _bCreateInHostConnector;
			}
			set
			{
				_bCreateInHostConnector = value;
			}
		}

		public bool CreateInChildConnector
		{
			get
			{
				return _bCreateInChildConnector;
			}
			set
			{
				_bCreateInChildConnector = value;
			}
		}

		[DefaultSerialization("OnlineChangeEnable")]
		[StorageVersion("3.4.3.10")]
		[StorageDefaultValue(false)]
		protected bool OnlineChangeEnable
		{
			get
			{
				return _bOnlineChangeEnable;
			}
			set
			{
				_bOnlineChangeEnable = value;
			}
		}

		public bool NoManualAddress
		{
			get
			{
				return _bNoManualAddress;
			}
			set
			{
				_bNoManualAddress = value;
			}
		}

		public string IecType
		{
			get
			{
				if (_dataElement is DataElementStructType)
				{
					return ((DataElementStructType)_dataElement).IecType;
				}
				if (_dataElement is DataElementUnionType)
				{
					return ((DataElementUnionType)_dataElement).IecType;
				}
				return string.Empty;
			}
			set
			{
				if (_dataElement is DataElementStructType)
				{
					((DataElementStructType)_dataElement).IecType = value;
				}
				if (_dataElement is DataElementUnionType)
				{
					((DataElementUnionType)_dataElement).IecType = value;
				}
			}
		}

		public bool OnlineParameter
		{
			get
			{
				return _bOnlineParameter;
			}
			set
			{
				_bOnlineParameter = value;
			}
		}

		public long IndexInDevDesc
		{
			get
			{
				return _lIndexInDevDesc;
			}
			set
			{
				_lIndexInDevDesc = value;
			}
		}

		public bool IsFileType => _bIsFileType;

		public IDeviceObject GetAssociatedDeviceObject
		{
			get
			{
				if (_owner != null)
				{
					return (IDeviceObject)(object)_owner.Device;
				}
				return null;
			}
		}

		public IConnector GetAssociatedConnector
		{
			get
			{
				if (_owner != null)
				{
					IIoProvider ioProvider = _owner.IoProvider;
					return (IConnector)(object)((ioProvider is IConnector) ? ioProvider : null);
				}
				return null;
			}
		}

		public bool IsUnion => _dataElement is DataElementUnionType;

		public bool LogicalParameter
		{
			get
			{
				return _bLogicalParameter;
			}
			internal set
			{
				_bLogicalParameter = value;
			}
		}

		public string OnlineHelpUrl => _stOnlineHelpUrl;

		public bool MapOnlyNew => _bMapOnlyNew;

		public uint LogicalMapSize => _uiMapSize;

		public uint LogicalMapOffset => _uiMapOffset;

		public bool OnlineChangeEnabled
		{
			get
			{
				return _bOnlineChangeEnable;
			}
			set
			{
				_bOnlineChangeEnable = value;
			}
		}

		public string DownloadedValue
		{
			get
			{
				return _stLastDownloadedValue;
			}
			set
			{
				_stLastDownloadedValue = value;
			}
		}

		public bool TraceSystemRecord => _bTraceSystemRecord;

		public bool BidirectionalOutput
		{
			get
			{
				return _bIsBidirectionalOutput;
			}
			set
			{
				_bIsBidirectionalOutput = value;
				if (_bIsBidirectionalOutput && _owner != null)
				{
					_owner.HasBidirectionalOutputs = true;
				}
			}
		}

		internal string ParamName => _stParamName;

		public bool IsFunctional => _bFunctional;

		public bool IsConstantValue => _bConstantValue;

		public bool IsInstanceVariable => !string.IsNullOrEmpty(_stInstanceVariable);

		public bool ConstantValue
		{
			get
			{
				return _bConstantValue;
			}
			set
			{
				_bConstantValue = value;
			}
		}

		public string DriverSpecific
		{
			get
			{
				return _stDriverSpecific;
			}
			set
			{
				_stDriverSpecific = value;
			}
		}

		public AlwaysMappingMode AlwaysMappingMode
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _alwaysMappingMode;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_alwaysMappingMode = value;
			}
		}

		internal string FbInstanceVariableWithDeviceName => _stInstanceVariableWithDeviceName;

		public string WatchFbInstanceVariable
		{
			get
			{
				if (_dataElement.HasBaseType)
				{
					return _stInstanceVariableWithDeviceName;
				}
				return string.Empty;
			}
			internal set
			{
				_stInstanceVariableWithDeviceName = value;
			}
		}

		public string VisibleNameDefault => _dataElement.VisibleNameDefault;

		public string DescriptionDefault => _dataElement.DescriptionDefault;

		public string UnitDefault => _dataElement.UnitDefault;

		public IStringRef VisibleNameStringRef => _dataElement.VisibleNameStringRef;

		public IStringRef DescriptionStringRef => _dataElement.DescriptionStringRef;

		public IStringRef UnitStringRef => _dataElement.UnitStringRef;

		public string FbInstanceVariable
		{
			get
			{
				if (!string.IsNullOrEmpty(_stInstanceVariable) && _owner != null && _owner.createInstanceVariable)
				{
					return _stInstanceVariable;
				}
				return string.Empty;
			}
			set
			{
				_stInstanceVariable = value;
			}
		}

		public bool PreparedValueAccess
		{
			get
			{
				return _preparedValueAccess;
			}
			set
			{
				_preparedValueAccess = value;
			}
		}

		public bool UseRefactoring
		{
			get
			{
				return _useRefactoring;
			}
			set
			{
				_useRefactoring = value;
			}
		}

		public bool DisableMapping
		{
			get
			{
				return _disableMapping;
			}
			set
			{
				_disableMapping = value;
			}
		}

		public Parameter()
			: base()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		internal Parameter(long lId)
			: this()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			_uiId = (uint)lId;
		}

		internal Parameter(long lId, StringRef name, StringRef description, AccessRight onlineaccess, AccessRight offlineaccess, ChannelType channeltype, string stType, TypeList typeList, ParameterSet owner, bool bCreateBitChannels)
			: this()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			_owner = owner;
			_uiId = (uint)lId;
			_accessRightOnline = onlineaccess;
			_accessRightOffline = offlineaccess;
			_channelType = channeltype;
			DataElementFactory dataElementFactory = new DataElementFactory(typeList);
			LList<ValueElement> val = new LList<ValueElement>();
			val.Add(new ValueElement());
			if ((int)_channelType != 0 && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0) && stType.ToLowerInvariant() == "std:bool")
			{
				stType = "std:BIT";
				DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.InformationParameterType, _uiId), (Severity)8);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
			}
			_stParamType = stType;
			_dataElement = dataElementFactory.Create(_uiId.ToString(), val, stType, name, new StringRef(), description, new string[0], this, null, bUpdate: false, bCreateBitChannels);
		}

		internal Parameter(XmlElement xeNode, TypeList typeList, int nParentSectionId, ParameterSet owner)
			: this()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			_owner = owner;
			Import(xeNode, typeList, nParentSectionId, bUpdate: false);
		}

		internal Parameter(Parameter original)
			: this()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			MergeOrNewParameter(original, bMerge: false);
		}

		public void Merge(Parameter original)
		{
			MergeOrNewParameter(original, bMerge: true);
		}

		internal void RecursiveMergeParameter(DataElementBase newdataelement, DataElementBase olddataelement)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			if (olddataelement.GetDescription != null)
			{
				newdataelement.SetDescription((IStringRef)(object)olddataelement.GetDescription);
			}
			if (newdataelement.HasSubElements)
			{
				foreach (DataElementBase item in (IEnumerable)newdataelement.SubElements)
				{
					if (olddataelement.SubElements.Contains(item.Identifier))
					{
						DataElementBase dataElementBase2 = olddataelement.SubElements[item.Identifier] as DataElementBase;
						if (dataElementBase2 != null)
						{
							RecursiveMergeParameter(item, dataElementBase2);
						}
					}
				}
			}
			if (((int)_accessRightOffline & 2) != 0 || (int)_channelType != 0)
			{
				newdataelement.Merge(olddataelement);
			}
		}

		internal void MergeOrNewParameter(Parameter original, bool bMerge)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			_uiId = original._uiId;
			_accessRightOffline = original._accessRightOffline;
			_accessRightOnline = original._accessRightOnline;
			_bDownload = original._bDownload;
			_bFunctional = original._bFunctional;
			_channelType = original._channelType;
			_diagType = original._diagType;
			_bCreateDownloadStructure = original._bCreateDownloadStructure;
			_bIsFileType = original._bIsFileType;
			_stParamType = original._stParamType;
			_stDriverSpecific = original._stDriverSpecific;
			_bConstantValue = original._bConstantValue;
			_stOnlineHelpUrl = original._stOnlineHelpUrl;
			_bIsBidirectionalOutput = original._bIsBidirectionalOutput;
			if (!bMerge)
			{
				_dataElement = (DataElementBase)((GenericObject)original._dataElement).Clone();
			}
			else
			{
				DataElementBase dataElementBase = (DataElementBase)((GenericObject)original._dataElement).Clone();
				dataElementBase.Parent = this;
				RecursiveMergeParameter(dataElementBase, _dataElement);
				_dataElement = dataElementBase;
			}
			_dataElement.Parent = this;
			_nParentSectionId = original._nParentSectionId;
			_bAlwaysMapping = original._bAlwaysMapping;
			_bCreateInHostConnector = original._bCreateInHostConnector;
			_bOnlineParameter = original._bOnlineParameter;
			_bCreateInChildConnector = original._bCreateInChildConnector;
			_lIndexInDevDesc = original._lIndexInDevDesc;
			_bNoManualAddress = original._bNoManualAddress;
			_bMapOnlyNew = original._bMapOnlyNew;
			_bLogicalParameter = original._bLogicalParameter;
			_uiMapOffset = original._uiMapOffset;
			_uiMapSize = original._uiMapSize;
			_bOnlineChangeEnable = original._bOnlineChangeEnable;
			_stParamName = original._stParamName;
			_stLastDownloadedValue = original._stParamName;
			_bTraceSystemRecord = original._bTraceSystemRecord;
			_alwaysMappingMode = original._alwaysMappingMode;
			_stInstanceVariable = original._stInstanceVariable;
			_stInstanceVariableWithDeviceName = original._stInstanceVariableWithDeviceName;
			_preparedValueAccess = original._preparedValueAccess;
			_useRefactoring = original._useRefactoring;
			_disableMapping = original._disableMapping;
		}

		internal void Update(XmlElement xeParam, TypeList types, int nParentSectionId)
		{
			Import(xeParam, types, nParentSectionId, bUpdate: true);
		}

		private void Import(XmlElement xeNode, TypeList typeList, int nParentSectionId, bool bUpdate)
		{
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			StringRef description = new StringRef();
			StringRef visibleName = new StringRef();
			StringRef unit = new StringRef();
			string text = null;
			CustomItemList customItemList = null;
			LList<ValueElement> val = new LList<ValueElement>();
			LList<ValueElement> val2 = new LList<ValueElement>();
			_nParentSectionId = nParentSectionId;
			_uiId = ParseUint(xeNode.GetAttribute("ParameterId"), 0u);
			string text2 = (_stParamType = xeNode.GetAttribute("type"));
			foreach (XmlNode childNode in xeNode.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					XmlElement xmlElement = (XmlElement)childNode;
					switch (xmlElement.Name)
					{
					case "Attributes":
						ReadAttributesNode(xmlElement, bUpdate);
						break;
					case "Default":
					{
						ValueElement valueElement2 = new ValueElement(xmlElement, typeList, null);
						val.Add(valueElement2);
						break;
					}
					case "Name":
						visibleName = ParameterDataCache.AddStringRef(new StringRef(xmlElement));
						break;
					case "Unit":
						unit = ParameterDataCache.AddStringRef(new StringRef(xmlElement));
						break;
					case "Description":
						description = ParameterDataCache.AddStringRef(new StringRef(xmlElement));
						break;
					case "FilterFlags":
						text = xmlElement.InnerText;
						break;
					case "Custom":
						customItemList = new CustomItemList(xmlElement);
						break;
					case "DefaultMapping":
					{
						ValueElement valueElement = new ValueElement(xmlElement, typeList, null);
						val2.Add(valueElement);
						break;
					}
					}
				}
			}
			if (((int)_accessRightOffline & 2) == 0 && (int)_channelType == 0)
			{
				bUpdate = false;
			}
			string[] filterFlags = text?.Split(' ');
			DataElementFactory dataElementFactory = new DataElementFactory(typeList);
			if ((int)_channelType != 0 && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0) && text2.ToLowerInvariant() == "std:bool")
			{
				text2 = "std:BIT";
				DeviceMessage deviceMessage = new DeviceMessage(string.Format(Strings.InformationParameterType, _uiId), (Severity)8);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
			}
			_dataElement = dataElementFactory.Create(_uiId.ToString(), val, text2, visibleName, unit, description, filterFlags, this, _dataElement, bUpdate, _owner.CreateBitChannels);
			if (customItemList != null)
			{
				_dataElement.SetCustomItems(customItemList);
			}
			if (val2 == null || val2.Count <= 0)
			{
				return;
			}
			int num = 0;
			foreach (ValueElement item in val2)
			{
				try
				{
					if (_dataElement is DataElementArrayType)
					{
						if (((ICollection)_dataElement.SubElements).Count > num)
						{
							SetDefaultMapping(item, _dataElement.SubElements[num]);
						}
						else if (((ICollection)_dataElement.SubElements).Count == num)
						{
							SetDefaultMapping(item, (IDataElement)(object)_dataElement);
						}
					}
					else
					{
						SetDefaultMapping(item, (IDataElement)(object)this);
					}
				}
				catch
				{
				}
				num++;
			}
		}

		private void SetDefaultMapping(ValueElement defaultmapping, IDataElement dataelement)
		{
			if (defaultmapping == null || dataelement == null)
			{
				return;
			}
			bool flag = ((!defaultmapping.Value.Contains(".")) ? true : false);
			if (defaultmapping.Value != string.Empty)
			{
				if (dataelement is DataElementBase)
				{
					(dataelement as DataElementBase).Mapping = new IoMapping();
				}
				if (dataelement is Parameter)
				{
					_dataElement.Mapping = new IoMapping();
				}
				IVariableMappingCollection variableMappings = dataelement.IoMapping.VariableMappings;
				if (((ICollection)variableMappings).Count == 0)
				{
					string value = defaultmapping.Value;
					variableMappings.AddMapping(value, flag);
					IVariableMapping obj = variableMappings[0];
					IVariableMapping2 val = (IVariableMapping2)(object)((obj is IVariableMapping2) ? obj : null);
					if (val != null)
					{
						val.DefaultVariable=(value);
					}
				}
			}
			if (defaultmapping.SubElements.Count <= 0 || ((ICollection)dataelement.SubElements).Count <= 0)
			{
				return;
			}
			foreach (ValueElement subElement in defaultmapping.SubElements)
			{
				if (dataelement.SubElements.Contains(subElement.Name))
				{
					SetDefaultMapping(subElement, dataelement.SubElements[subElement.Name]);
				}
			}
		}

		public override object Clone()
		{
			Parameter parameter = new Parameter(this);
			((GenericObject)parameter).AfterClone();
			parameter.Owner = _owner;
			return parameter;
		}

		public void Notify(IDataElement dataelement, string[] path)
		{
			if (_owner == null)
			{
				throw new InvalidOperationException("Cannot notify a change whithout a parent");
			}
			if (dataelement == _dataElement)
			{
				dataelement = (IDataElement)(object)this;
				path = null;
			}
			_owner.Notify((IParameter)(object)this, dataelement, path);
		}

		public long GetBitOffset(IDataElement child)
		{
			return 0L;
		}

		public long GetBitOffset()
		{
			return 0L;
		}

		public override void AfterDeserialize()
		{
			_dataElement.Parent = this;
		}

		public override void AfterClone()
		{
			_dataElement.Parent = this;
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

		public void SetParameterId(long lParameterId)
		{
			if ((uint)lParameterId != _uiId && _owner != null)
			{
				if (_owner.GetParameter(lParameterId) != null)
				{
					throw new ArgumentException("A parameter with that id already exists");
				}
				ParameterSet owner = _owner;
				owner.RemoveParameter(_uiId);
				_uiId = (uint)lParameterId;
				owner.AddParameter(this);
			}
		}

		public void SetName(IStringRef name)
		{
			_dataElement.SetName(name);
		}

		public void SetDescription(IStringRef description)
		{
			_dataElement.SetDescription(description);
		}

		public void MoveToSection(IParameterSection section)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			int nParentSectionId;
			if (section == null)
			{
				nParentSectionId = -1;
			}
			else
			{
				ParameterSection parameterSection = (ParameterSection)(object)section;
				if (_owner.GetSection(parameterSection.Id) == null)
				{
					throw new ArgumentException("Invalid section");
				}
				nParentSectionId = parameterSection.Id;
			}
			IParameterSection3 oldSection = (IParameterSection3)Section;
			_nParentSectionId = nParentSectionId;
			_owner.RaiseParameterMoved((IParameter)(object)this, oldSection);
		}

		public void SetDownload(bool bDownload)
		{
			_bDownload = bDownload;
		}

		public string GetTypeString()
		{
			return _dataElement.GetTypeString();
		}

		public long GetBitSize()
		{
			return _dataElement.GetBitSize();
		}

		public Parameter GetParameter()
		{
			return this;
		}

		public IOnlineVarRef CreateWatch()
		{
			return _dataElement.CreateWatch();
		}

		public IOnlineVarRef2 CreateWatch(bool bClientControlled)
		{
			return _dataElement.CreateWatch(bClientControlled);
		}

		public int GetValueEnumerationIndex()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_dataElement is IEnumerationDataElement)
			{
				return ((IEnumerationDataElement)_dataElement).GetValueEnumerationIndex();
			}
			throw new InvalidOperationException("Method only available for enumeration types");
		}

		public string GetValueForEnumeration(IEnumerationValue enumValue)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_dataElement is IEnumerationDataElement)
			{
				return ((IEnumerationDataElement)_dataElement).GetValueForEnumeration(enumValue);
			}
			throw new InvalidOperationException("Method only available for enumeration types");
		}

		public IEnumerationValue GetEnumerationValue(object baseValue, out int nIndex)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_dataElement is IEnumerationDataElement)
			{
				return ((IEnumerationDataElement)_dataElement).GetEnumerationValue(baseValue, out nIndex);
			}
			throw new InvalidOperationException("Method only available for enumeration types");
		}

		public override int CompareTo(object obj)
		{
			Parameter parameter = obj as Parameter;
			if (parameter != null)
			{
				return _uiId.CompareTo(parameter._uiId);
			}
			if (obj is uint)
			{
				return _uiId.CompareTo((uint)obj);
			}
			throw new ArgumentException("Cannot compare this object to an object of type '" + obj.GetType().ToString() + "'");
		}

		public override bool Equals(object obj)
		{
			if (obj is uint)
			{
				return _uiId == (uint)obj;
			}
			if (obj is Parameter)
			{
				return _uiId == ((Parameter)obj).Id;
			}
			throw new ArgumentException("Cannot compare this object to an object of type '" + obj.GetType().ToString() + "'");
		}

		public override int GetHashCode()
		{
			return _uiId.GetHashCode();
		}

		internal void SetSectionId(int nSectionId)
		{
			_nParentSectionId = nSectionId;
		}

		internal void SetOwner(ParameterSet owner)
		{
			_owner = owner;
		}

		internal void UpdateLanguageModelGuids(bool bUpgrade)
		{
			_dataElement.UpdateLanguageModelGuids(bUpgrade, string.Empty);
		}

		internal void SetPositionIds(IUniqueIdGenerator idGenerator)
		{
			_dataElement.SetPositionIds(idGenerator);
		}

		internal bool AddToLanguageModel(LanguageModelContainer lmcontainer, string stParamName, string stBaseName, string stBaseNameNoBlob, string stSeparator, bool bHide, bool isGlobalType, uint nNumParam)
		{
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Invalid comparison between Unknown and I4
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Invalid comparison between Unknown and I4
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Expected I4, but got Unknown
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Invalid comparison between Unknown and I4
			string typeName = _dataElement.GetTypeName("T" + stParamName);
			stParamName = "p" + nNumParam.ToString("x");
			_stParamName = stParamName;
			bool isExternalDatatype = IsExternalDatatype;
			bool blobInit = BlobInit;
			if (_dataElement is DataElementStructType)
			{
				if (!((DataElementStructType)_dataElement).HasIecType && !isGlobalType)
				{
					bool flag = false;
					foreach (StructDefinition structType in lmcontainer.StructTypes)
					{
						if (structType._stName == typeName)
						{
							flag = true;
							break;
						}
					}
					if (lmcontainer.lmNew != null)
					{
						ILMDataType[] dataTypes = lmcontainer.lmNew.DataTypes;
						for (int i = 0; i < dataTypes.Length; i++)
						{
							if (dataTypes[i].Name == typeName)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						_dataElement.AddTypeDefs(typeName, lmcontainer, bHide);
					}
				}
			}
			else if (_dataElement is DataElementUnionType)
			{
				if (!((DataElementUnionType)_dataElement).HasIecType && !isGlobalType)
				{
					bool flag2 = false;
					foreach (StructDefinition structType2 in lmcontainer.StructTypes)
					{
						if (structType2._stName == typeName)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						_dataElement.AddTypeDefs(typeName, lmcontainer, bHide);
					}
				}
			}
			else
			{
				_dataElement.AddTypeDefs(typeName, lmcontainer, bHide);
			}
			string text = _dataElement.GetInitialization(out var bDefault, ((int)_channelType == 2) | ((int)_channelType == 3), bCreateDefaultValue: false);
			if (_bOnlineChangeEnable && string.IsNullOrEmpty(text) && _dataElement.HasBaseType)
			{
				TypeClass val = (TypeClass)_dataElement.GetTypeId();
				switch ((int)val - 2)
				{
				default:
					if ((int)val == 37)
					{
						text = "LTIME#0ms";
					}
					break;
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
					text = "0";
					break;
				case 14:
					text = "''";
					break;
				case 15:
					text = "\"\"";
					break;
				case 16:
					text = "t#0ms";
					break;
				}
			}
			if (_bIsFileType)
			{
				text = ResolveFileParameter(text);
			}
			bDefault = bDefault || text == null || text == string.Empty;
			long num = 0L;
			if (!isExternalDatatype)
			{
				num = _dataElement.GetBitSize();
			}
			if (bDefault && !_bCreateDownloadStructure)
			{
				text = null;
			}
			else
			{
				IExpression val2 = null;
				ILanguageModelBuilder lmBuilder = lmcontainer.lmBuilder;
				ILanguageModelBuilder3 val3 = (ILanguageModelBuilder3)(object)((lmBuilder is ILanguageModelBuilder3) ? lmBuilder : null);
				if (!string.IsNullOrEmpty(text) && lmcontainer.lmBuilder != null)
				{
					val2 = ((!(lmcontainer.lmBuilder is ILanguageModelBuilder2)) ? lmcontainer.lmBuilder.ParseExpression(text) : (lmcontainer.lmBuilder as ILanguageModelBuilder2).ParseInitialisation(text));
				}
				if (val3 != null)
				{
					ICompiledType val4 = Types.ParseType(typeName);
					if (blobInit)
					{
						lmcontainer.seqParamStruct.AddStatement(val3.CreatePragmaStatement2((IExprementPosition)null, "attribute 'noinit'"));
						IVariableDeclarationStatement val5 = ((ILanguageModelBuilder)val3).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, stParamName, val4);
						lmcontainer.seqParamStruct.AddStatement((IStatement)(object)val5);
					}
					else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
					{
						IVariableDeclarationStatement val6 = ((ILanguageModelBuilder)val3).CreateVariableDeclarationStatement((IExprementPosition)null, stParamName, val4, val2, (IDirectVariable)null);
						lmcontainer.seqParamVarDeclNoBlobInit.AddStatement((IStatement)(object)val6);
						val2 = null;
						text = null;
					}
					else
					{
						IVariableDeclarationStatement val7 = ((ILanguageModelBuilder)val3).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, stParamName, val4);
						lmcontainer.seqParamVarDeclNoBlobInit.AddStatement((IStatement)(object)val7);
					}
				}
				else
				{
					lmcontainer.sbParamStruct.AppendFormat("{{attribute 'noinit'}}{0}:{1};\n", stParamName, typeName);
				}
				if (val2 != null)
				{
					IVariableExpression val8 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, stParamName);
					IAssignmentExpression item = lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val8, val2);
					if (blobInit)
					{
						lmcontainer.assInitValues.Add(item);
					}
					else
					{
						lmcontainer.assInitValuesNoBlobInit.Add(item);
					}
				}
				else if (!string.IsNullOrEmpty(text))
				{
					if (lmcontainer.sbInitValues.Length > 0)
					{
						lmcontainer.sbInitValues.Append(",\n");
					}
					lmcontainer.sbInitValues.AppendFormat("{0}:={1}", stParamName, text);
				}
			}
			if (_bCreateDownloadStructure)
			{
				if (lmcontainer.lmBuilder != null)
				{
					List<IAssignmentExpression> list = new List<IAssignmentExpression>();
					IVariableExpression val9 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwParameterId");
					IVariableExpression val10 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwValue");
					IVariableExpression val11 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "wType");
					IVariableExpression val12 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "wLen");
					IVariableExpression val13 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwFlags");
					ILiteralExpression val14 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)Id, (TypeClass)4);
					ILiteralExpression val15 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)_dataElement.GetTypeId(), (TypeClass)3);
					ILiteralExpression val16 = null;
					IVariableExpression2 val17 = lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, stParamName);
					IExpression val18 = null;
					if (!isExternalDatatype && num != -1)
					{
						if (num > 65535)
						{
							val16 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)(num & 0xFFFF), (TypeClass)3);
							val18 = (IExpression)(object)lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)((num & 0xFFFF0000u) | 0x32), (TypeClass)4);
						}
						else
						{
							val16 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, (ulong)num, (TypeClass)3);
						}
					}
					IExpression val19 = null;
					if (blobInit)
					{
						IVariableExpression2 val20 = lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, stBaseName);
						val19 = (IExpression)(object)lmcontainer.lmBuilder.CreateCompoAccessExpression((IExprementPosition)null, (IExpression)(object)val20, val17);
					}
					else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
					{
						val19 = (IExpression)(object)val17;
					}
					else
					{
						IVariableExpression2 val21 = lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, stBaseNameNoBlob);
						val19 = (IExpression)(object)lmcontainer.lmBuilder.CreateCompoAccessExpression((IExprementPosition)null, (IExpression)(object)val21, val17);
					}
					IOperatorExpression val22 = lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)33, val19);
					list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val9, (IExpression)(object)val14));
					list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val10, (IExpression)(object)val22));
					list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val11, (IExpression)(object)val15));
					if (val16 != null)
					{
						list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val12, (IExpression)(object)val16));
					}
					else
					{
						ILanguageModelBuilder lmBuilder2 = lmcontainer.lmBuilder;
						ILanguageModelBuilder3 val23 = (ILanguageModelBuilder3)(object)((lmBuilder2 is ILanguageModelBuilder3) ? lmBuilder2 : null);
						if (val23 != null)
						{
							ICompiledType val24 = Types.ParseType(typeName);
							ITypeExpression val25 = ((ILanguageModelBuilder)val23).CreateTypeExpression((IExprementPosition)null, val24);
							IOperatorExpression val26 = lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)36, (IExpression)(object)val25);
							ILiteralExpression val27 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 8L, (TypeClass)3);
							IOperatorExpression val28 = lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)159, (IExpression)(object)val26, (IExpression)(object)val27);
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)6, (ushort)0))
							{
								ICastExpression val29 = lmcontainer.lmBuilder.CreateCastExpression((IExprementPosition)null, ((ILanguageModelBuilder)val23).CreateSimpleType((TypeClass)3), (IExpression)(object)val28);
								list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val12, (IExpression)(object)val29));
								ITypeExpression val30 = ((ILanguageModelBuilder)val23).CreateTypeExpression((IExprementPosition)null, val24);
								IOperatorExpression val31 = lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)36, (IExpression)(object)val30);
								ILiteralExpression val32 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 8L, (TypeClass)3);
								IOperatorExpression val33 = lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)159, (IExpression)(object)val31, (IExpression)(object)val32);
								ILiteralExpression val34 = lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 4294901760L, (TypeClass)4);
								IOperatorExpression val35 = lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)127, (IExpression)(object)val33, (IExpression)(object)val34);
								ILiteralExpression val36 = ((!BidirectionalOutput) ? lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 50L, (TypeClass)3) : lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 306uL, (TypeClass)4));
								val18 = (IExpression)(object)lmcontainer.lmBuilder.CreateOperatorExpression((IExprementPosition)null, (Operator)129, (IExpression)(object)val35, (IExpression)(object)val36);
							}
							else
							{
								list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val12, (IExpression)(object)val28));
							}
						}
					}
					if (val18 == null)
					{
						val18 = (IExpression)(object)((!BidirectionalOutput) ? lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 50uL, (TypeClass)4) : lmcontainer.lmBuilder.CreateLiteralExpression((IExprementPosition)null, 306uL, (TypeClass)4));
					}
					list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val13, val18));
					if (!string.IsNullOrEmpty(DriverSpecific))
					{
						IVariableExpression val37 = (IVariableExpression)(object)lmcontainer.lmBuilder.CreateVariableExpression((IExprementPosition)null, "dwDriverSpecific");
						IExpression val38 = lmcontainer.lmBuilder.ParseExpression(DriverSpecific);
						list.Add(lmcontainer.lmBuilder.CreateAssignmentExpression((IExprementPosition)null, (IExpression)(object)val37, val38));
					}
					lmcontainer.struinitParameters.Add(lmcontainer.lmBuilder.CreateStructureInitialisation((IExprementPosition)null, list));
				}
				else
				{
					lmcontainer.sbParameters.Append(stSeparator);
					string text2 = stBaseName + "." + stParamName;
					if (!string.IsNullOrEmpty(DriverSpecific))
					{
						lmcontainer.sbParameters.AppendFormat("(dwParameterId:={0}, dwValue:=ADR({1}), wType:={2}, wLen := {3}, dwFlags:=50, dwDriverSpecific:={4})", Id, text2, _dataElement.GetTypeId(), num, DriverSpecific);
					}
					else
					{
						lmcontainer.sbParameters.AppendFormat("(dwParameterId:={0}, dwValue:=ADR({1}), wType:={2}, wLen := {3}, dwFlags:=50)", Id, text2, _dataElement.GetTypeId(), num);
					}
				}
				return true;
			}
			if (!bDefault)
			{
				lmcontainer.AddParameterInitialization(_uiId, stBaseName + "." + stParamName);
				return true;
			}
			return false;
		}

		internal string ResolveFileParameter(string stValue)
		{
			string text = string.Empty;
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject != null)
			{
				string directoryName = Path.GetDirectoryName(primaryProject.Path);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(primaryProject.Path);
				text = stValue.Replace("$PROJECTPATH$", directoryName);
				text = text.Replace("$PROJECT$", fileNameWithoutExtension);
				if (stValue.Contains("$DEVICEPATH$") && _owner != null && _owner.Device != null)
				{
					IDeviceObject2 device = _owner.Device;
					IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((device is IDeviceObject5) ? device : null)).DeviceIdentificationNoSimulation;
					IRepositorySource val = default(IRepositorySource);
					((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceIdentificationNoSimulation, out val);
					directoryName = new Uri(val.LocationUrl).LocalPath;
					string text2 = DeviceObjectHelper.EscapeString(deviceIdentificationNoSimulation.Id);
					string text3 = DeviceObjectHelper.EscapeString(deviceIdentificationNoSimulation.Version);
					directoryName = Path.Combine(directoryName, deviceIdentificationNoSimulation.Type + "\\" + text2 + "\\" + text3);
					text = text.Replace("$DEVICEPATH$", directoryName);
				}
			}
			return text;
		}

		internal void AddFixedUpdates(ConnectorMap cm, FixedTaskUpdates fixedTaskUpdates, string stTask, Hashtable htStartAddresses)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Invalid comparison between Unknown and I4
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Invalid comparison between Unknown and I4
			if ((int)ChannelType == 1 || (int)ChannelType == 2 || (int)ChannelType == 3)
			{
				ChannelMap channelMap = new ChannelMap(Id, GetBitSize(), (int)ChannelType == 1, (int)ChannelType == 3, bAlwaysMapping: false, (IDataElement)(object)this, (AlwaysMappingMode)0);
				channelMap.ParamBitoffset = 0;
				channelMap.Comment = Description;
				IDirectVariable iecAddress = ((IoMapping)(object)IoMapping).GetIecAddress(htStartAddresses);
				fixedTaskUpdates.Add(new FixedTaskUpdate(nBitOffset: (iecAddress.Components.Length == 2) ? iecAddress.Components[1] : 0, connector: cm, channel: channelMap, stAddress: IoMapping.IecAddress, stTask: stTask, dataElement: (IDataElement)(object)_dataElement));
			}
		}

		internal void AddMapping(ConnectorMap cm, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Invalid comparison between Unknown and I4
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Invalid comparison between Unknown and I4
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			if ((!_disableMapping && _bDownload) || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)0))
			{
				if (_bAlwaysMapping)
				{
					bAlwaysMapping = true;
					mappingMode = _alwaysMappingMode;
				}
				if ((int)ChannelType == 1)
				{
					_dataElement.AddMapping(cm, 0L, Id, bInput: true, bReadOnly: false, stBaseName, bAlwaysMapping, mappingMode, string.Empty, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				}
				else if ((int)ChannelType == 2)
				{
					_dataElement.AddMapping(cm, 0L, Id, bInput: false, bReadOnly: false, stBaseName, bAlwaysMapping, mappingMode, string.Empty, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				}
				else if ((int)ChannelType == 3)
				{
					_dataElement.AddMapping(cm, 0L, Id, bInput: false, bReadOnly: true, stBaseName, bAlwaysMapping, mappingMode, string.Empty, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				}
			}
		}

		internal void AddChannel(AddrToChannelMap map, ConnectorMap connectorMap, ICompileContext comcon, DataElementBase dataElement, long lStartBit, Hashtable htStartAddresses, bool bSkipCheckOverlap)
		{
			if (!dataElement.HasBaseType)
			{
				AddChannels(map, connectorMap, dataElement, comcon, lStartBit, htStartAddresses, bSkipCheckOverlap);
				return;
			}
			IDirectVariable iecAddress = ((IoMapping)(object)dataElement.IoMapping).GetIecAddress(htStartAddresses);
			bool flag = default(bool);
			IDataLocation val = comcon.LocateAddress(out flag, iecAddress);
			long lBitOffset = new BitDataLocation(val).BitOffset - lStartBit;
			if (iecAddress != null && _bDownload)
			{
				map.AddChannel(connectorMap, _uiId, lBitOffset, val, iecAddress, dataElement.BaseType, (IDataElement)(object)dataElement, htStartAddresses, bSkipCheckOverlap);
			}
		}

		internal void AddChannels(AddrToChannelMap map, ConnectorMap connectorMap, DataElementBase baseElement, ICompileContext comcon, long lStartBit, Hashtable htStartAddresses, bool bSkipCheckOverlap)
		{
			if (baseElement is DataElementUnionType)
			{
				long num = 0L;
				DataElementBase dataElementBase = null;
				foreach (DataElementBase item in (IEnumerable)baseElement.SubElements)
				{
					long bitSize = item.GetBitSize();
					if (bitSize > num)
					{
						num = bitSize;
						dataElementBase = item;
					}
				}
				if (dataElementBase != null)
				{
					AddChannel(map, connectorMap, comcon, dataElementBase, lStartBit, htStartAddresses, bSkipCheckOverlap);
				}
				return;
			}
			foreach (DataElementBase item2 in (IEnumerable)baseElement.SubElements)
			{
				AddChannel(map, connectorMap, comcon, item2, lStartBit, htStartAddresses, bSkipCheckOverlap);
			}
		}

		internal void AddChannels(AddrToChannelMap map, ConnectorMap connectorMap, ICompileContext comcon, Hashtable htStartAddresses, bool bSkipCheckOverlap)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			if ((int)ChannelType != 1 && (int)ChannelType != 2 && (int)ChannelType != 3)
			{
				return;
			}
			string text = string.Empty;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) && _dataElement is DataElementArrayType && ((ICollection)_dataElement.SubElements).Count > 0 && (_dataElement.SubElements[0] as DataElementBase).HasBaseType)
			{
				text = _dataElement.SubElements[0].BaseType;
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)1, (ushort)20) && !_dataElement.HasBaseType && string.IsNullOrEmpty(text))
			{
				IDirectVariable iecAddress = ((IoMapping)(object)_dataElement.IoMapping).GetIecAddress(htStartAddresses);
				bool flag = default(bool);
				BitDataLocation bitDataLocation = new BitDataLocation(comcon.LocateAddress(out flag, iecAddress));
				AddChannels(map, connectorMap, _dataElement, comcon, bitDataLocation.BitOffset, htStartAddresses, bSkipCheckOverlap);
				return;
			}
			IoMapping ioMapping = (IoMapping)(object)_dataElement.IoMapping;
			IDirectVariable iecAddress2 = ioMapping.GetIecAddress(htStartAddresses);
			if (iecAddress2 != null)
			{
				if (_bDownload && !_disableMapping)
				{
					string empty = string.Empty;
					empty = ((!_dataElement.HasBaseType) ? text : _dataElement.BaseType);
					bool flag2 = default(bool);
					IDataLocation val = comcon.LocateAddress(out flag2, iecAddress2);
					if (val != null)
					{
						map.AddChannel(connectorMap, _uiId, 0L, val, iecAddress2, empty, (IDataElement)(object)_dataElement, htStartAddresses, bSkipCheckOverlap);
					}
				}
				return;
			}
			throw new ArgumentException(string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ErrorAddressNotValid"), ioMapping.IecAddress));
		}

		internal void SetIoProvider(IIoProvider ioProvider)
		{
			_dataElement.SetIoProvider(ioProvider);
		}

		private void ReadAttributesNode(XmlElement xeNode, bool bUpdate)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			if (bUpdate)
			{
				_accessRightOffline = (AccessRight)3;
				_accessRightOnline = (AccessRight)3;
				_bDownload = true;
				_bFunctional = false;
			}
			string attribute = xeNode.GetAttribute("onlineaccess");
			_accessRightOnline = ParseAccessRight(attribute, _accessRightOnline);
			attribute = xeNode.GetAttribute("offlineaccess");
			_accessRightOffline = ParseAccessRight(attribute, _accessRightOffline);
			attribute = xeNode.GetAttribute("download");
			_bDownload = ParseBool(attribute, _bDownload);
			attribute = xeNode.GetAttribute("functional");
			_bFunctional = ParseBool(attribute, _bFunctional);
			if (_bFunctional && _owner != null)
			{
				_owner.HasFunctionalParams = true;
			}
			attribute = xeNode.GetAttribute("channel");
			_channelType = ParseChannelType(attribute, (ChannelType)0, out _diagType, out _bIsFileType);
			if (_bIsFileType && _owner != null)
			{
				_owner.HasFileParams = true;
			}
			attribute = xeNode.GetAttribute("bidirectionaloutput");
			_bIsBidirectionalOutput = ParseBool(attribute, _bIsBidirectionalOutput);
			if (_bIsBidirectionalOutput && _owner != null)
			{
				_owner.HasBidirectionalOutputs = true;
			}
			attribute = xeNode.GetAttribute("createDownloadStructure");
			_bCreateDownloadStructure = ParseBool(attribute, bDefault: true);
			attribute = xeNode.GetAttribute("alwaysmapping");
			_bAlwaysMapping = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("createInHostConnector");
			_bCreateInHostConnector = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("onlineparameter");
			_bOnlineParameter = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("createInChildConnector");
			_bCreateInChildConnector = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("noManualAddress");
			_bNoManualAddress = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("TraceSystemRecord");
			_bTraceSystemRecord = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("OnlineChangeEnabled");
			_bOnlineChangeEnable = ParseBool(attribute, bDefault: false);
			attribute = xeNode.GetAttribute("constantValue");
			_bConstantValue = ParseBool(attribute, bDefault: false);
			_stOnlineHelpUrl = xeNode.GetAttribute("onlineHelpUrl");
			attribute = xeNode.GetAttribute("driverSpecific");
			if (!string.IsNullOrEmpty(attribute))
			{
				_stDriverSpecific = attribute;
				_owner.HasInstanceVariable = true;
			}
			attribute = xeNode.GetAttribute("alwaysmappingMode");
			if (!string.IsNullOrEmpty(attribute))
			{
				_alwaysMappingMode = DeviceObjectHelper.ParseAlwaysMappingMode(attribute, (AlwaysMappingMode)0);
			}
			attribute = xeNode.GetAttribute("instanceVariable");
			if (!string.IsNullOrEmpty(attribute))
			{
				_stInstanceVariable = attribute;
			}
			attribute = xeNode.GetAttribute("preparedValueAccess");
			if (!string.IsNullOrEmpty(attribute))
			{
				_preparedValueAccess = ParseBool(attribute, bDefault: false);
			}
			attribute = xeNode.GetAttribute("useRefactoring");
			if (!string.IsNullOrEmpty(attribute))
			{
				_useRefactoring = ParseBool(attribute, bDefault: false);
			}
			attribute = xeNode.GetAttribute("disableMapping");
			if (!string.IsNullOrEmpty(attribute))
			{
				_disableMapping = ParseBool(attribute, bDefault: false);
			}
			if (bUpdate)
			{
				_bMapOnlyNew = ParseBool(xeNode.GetAttribute("mapOnlyNew"), _bMapOnlyNew);
				_bLogicalParameter = ParseBool(xeNode.GetAttribute("logicalParameter"), _bLogicalParameter);
				_uiMapSize = ParseUint(xeNode.GetAttribute("mapSize"), _uiMapSize);
				_uiMapOffset = ParseUint(xeNode.GetAttribute("mapOffset"), _uiMapOffset);
			}
			else
			{
				_bMapOnlyNew = ParseBool(xeNode.GetAttribute("mapOnlyNew"), bDefault: false);
				_bLogicalParameter = ParseBool(xeNode.GetAttribute("logicalParameter"), bDefault: false);
				_uiMapSize = ParseUint(xeNode.GetAttribute("mapSize"), 0u);
				_uiMapOffset = ParseUint(xeNode.GetAttribute("mapOffset"), 0u);
			}
		}

		internal static AccessRight ParseAccessRight(string st, AccessRight arDefault)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (st == null)
			{
				return arDefault;
			}
			return (AccessRight)(st switch
			{
				"read" => (AccessRight)1, 
				"write" => (AccessRight)2, 
				"readwrite" => (AccessRight)3, 
				"none" => 0, 
				_ => arDefault, 
			});
		}

		private bool ParseBool(string st, bool bDefault)
		{
			if (st == null)
			{
				return bDefault;
			}
			st = st.ToLowerInvariant();
			return st switch
			{
				"true" => true, 
				"1" => true, 
				"false" => false, 
				"0" => false, 
				_ => bDefault, 
			};
		}

		private uint ParseUint(string st, uint uiDefault)
		{
			if (string.IsNullOrEmpty(st))
			{
				return uiDefault;
			}
			uint.TryParse(st, out var result);
			return result;
		}

		private ChannelType ParseChannelType(string st, ChannelType ctDefault, out DiagType diagType, out bool bIsFileType)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			bIsFileType = false;
			diagType = (DiagType)0;
			if (st == null)
			{
				return ctDefault;
			}
			switch (st.ToLowerInvariant())
			{
			case "":
			case "none":
				return (ChannelType)0;
			case "input":
				return (ChannelType)1;
			case "output":
				return (ChannelType)2;
			case "output_readonly":
				return (ChannelType)3;
			case "diag":
				diagType = (DiagType)1;
				return (ChannelType)0;
			case "diagack":
				diagType = (DiagType)2;
				return (ChannelType)0;
			case "file":
				bIsFileType = true;
				return (ChannelType)0;
			default:
			{
				DeviceMessage deviceMessage = new DeviceMessage($"Invalid channel type <{st}>", (Severity)2);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
				return (ChannelType)0;
			}
			}
		}

		public void AttachToEvent(string stEvent, GenericEventDelegate callback)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.AttachToEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		public void DetachFromEvent(string stEvent, GenericEventDelegate callback)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.DetachFromEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		public void RaiseEvent(string stEvent, XmlDocument eventData)
		{
			if (_baseGenericInterfaceExtensionProvider != null)
			{
				_baseGenericInterfaceExtensionProvider.RaiseEvent(stEvent, eventData);
				return;
			}
			throw new NotImplementedException();
		}

		public bool IsFunctionAvailable(string stFunction)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			if (stFunction == "GetOnlineParameter" || stFunction == "SetOnlineParameter")
			{
				return true;
			}
			return false;
		}

		public XmlDocument CallFunction(string stFunction, XmlDocument functionData)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			if (functionData == null)
			{
				throw new ArgumentNullException("functionData");
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateElement("Output"));
			if (!(stFunction == "GetOnlineParameter"))
			{
				if (stFunction == "SetOnlineParameter")
				{
					OnlineParameter = XmlConvert.ToBoolean(functionData.DocumentElement["value"].InnerText);
				}
			}
			else
			{
				xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(OnlineParameter);
			}
			return xmlDocument;
		}

		public void SetAccessRight(bool bOnline, AccessRight access)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (bOnline)
			{
				_accessRightOnline = access;
			}
			else
			{
				_accessRightOffline = access;
			}
			Notify((IDataElement)(object)this, new string[0]);
		}

		public void SetMinValue(string stValue)
		{
			_dataElement.SetMinValue(stValue);
		}

		public void SetMaxValue(string stValue)
		{
			_dataElement.SetMaxValue(stValue);
		}

		public void SetDefaultValue(string stValue)
		{
			_dataElement.SetDefaultValue(stValue);
		}

		public void SetLogicalParameter(bool bIsLogícal)
		{
			_bLogicalParameter = bIsLogícal;
		}

		public void SetUnit(IStringRef unit)
		{
			_dataElement.SetUnit(unit);
		}

		public void SetLanguageModelPositionId(long lPositionId)
		{
			if (_dataElement != null)
			{
				_dataElement.SetLanguageModelPositionId(lPositionId);
			}
		}

		public void SetEditorPositionId(long lPositionId)
		{
			if (_dataElement != null)
			{
				_dataElement.SetEditorPositionId(lPositionId);
			}
		}
	}
}
