#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{abc12bfe-e34e-4b2c-a058-42c6e7e03a13}")]
	[StorageVersion("3.3.0.0")]
	public class ParameterSet : GenericObject2, IParameterSet6, IParameterSet5, IParameterSet4, IParameterSet3, IParameterSet2, IParameterSet, IDataElementCollection, ICollection, IEnumerable, IGenericInterfaceExtensionProvider
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageSaveAsNonGenericCollection("3.3.0.0-3.5.0.255")]
		[DefaultSerialization("Params")]
		[StorageVersion("3.3.0.0")]
		private LList<Parameter> _alParams = new LList<Parameter>();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("LmGuid")]
		[StorageVersion("3.3.0.0")]
		private Guid _guidLanguageModel = Guid.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Sections")]
		[StorageVersion("3.3.0.0")]
		[StorageSaveAsNonGenericCollection("3.3.0.0-3.5.0.255")]
		private LList<IParameterSection> _alSections;

		private IDeviceObject2 _deviceObject;

		private IIoProvider _ioProvider;

		private int _connectorid = -1;

		private int _nLastSectionId = -1;

		private LDictionary<int, ParameterSection> _htIdToSection;

		private LDictionary<long, uint> _dictParamIdToIndex = new LDictionary<long, uint>();

		private bool _bHasFileParams;

		private bool _bHasFunctionalParams;

		private bool _bHasInstanceVariable;

		private bool _bHasBidirectionalOutputs;

		private WeakMulticastDelegate _ehAdded;

		private WeakMulticastDelegate _ehRemoved;

		private WeakMulticastDelegate _ehChanged;

		private WeakMulticastDelegate _ehMoved;

		private WeakMulticastDelegate _ehSectionChanged;

		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		private string _stParamsListName;

		private Parameter _searchParameter = new Parameter(0L);

		internal bool _bCreateInstanceVariable;

		internal bool _bCreateInstanceVariableSet;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AlwaysMapping")]
		[StorageVersion("3.3.0.0")]
		protected bool _bAllwaysMapping;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("EditorName")]
		[StorageVersion("3.3.0.0")]
		protected StringRef _stEditorName;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AlwaysMappingMode")]
		[StorageVersion("3.5.5.0")]
		[StorageDefaultValue(AlwaysMappingMode.OnlyIfUnused)]
		protected AlwaysMappingMode _alwaysMappingMode;

		private ParameterSetTransactionHandler _transactionHandler;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("CreateBitChannels")]
		[StorageVersion("3.5.0.0")]
		[StorageDefaultValue(false)]
		private bool _bCreateBitChannels;

		private uint _nNumParams;

		private bool _bHasIoConfigAddress;

		internal LDictionary<long, uint> ParamIdToIndex => _dictParamIdToIndex;

		public IIoProvider IoProvider => _ioProvider;

		internal string ParamsListName
		{
			get
			{
				if (string.IsNullOrEmpty(_stParamsListName))
				{
					if (_ioProvider is Connector)
					{
						_stParamsListName = (_ioProvider as Connector).GetParamsListName();
					}
					if (_ioProvider is DeviceObject)
					{
						_stParamsListName = (_ioProvider as DeviceObject).GetParamsListName();
					}
					if (_ioProvider is ExplicitConnector)
					{
						_stParamsListName = (_ioProvider as ExplicitConnector).GetParamsListName();
					}
				}
				return _stParamsListName;
			}
		}

		public IStringTable StringTable
		{
			get
			{
				IDeviceObject2 deviceObject = _deviceObject;
				IDeviceObject6 val = (IDeviceObject6)(object)((deviceObject is IDeviceObject6) ? deviceObject : null);
				if (_deviceObject == null && IoProvider is ExplicitConnector)
				{
					IDeviceObject deviceObject2 = (IoProvider as ExplicitConnector).GetDeviceObject();
					val = (IDeviceObject6)(object)((deviceObject2 is IDeviceObject6) ? deviceObject2 : null);
				}
				if (val != null)
				{
					return (IStringTable)(object)val.StringTable;
				}
				return (IStringTable)(object)new StringTable();
			}
		}

		public IDataElement this[int nIndex] => (IDataElement)(object)_alParams[nIndex];

		public IDataElement this[string stIdentifier]
		{
			get
			{
				try
				{
					uint num = uint.Parse(stIdentifier);
					IDataElement parameter = (IDataElement)(object)GetParameter(num);
					if (parameter == null)
					{
						throw new ArgumentException("No element of name '" + stIdentifier + "' found.");
					}
					return parameter;
				}
				catch
				{
					throw new ArgumentException("No element of name '" + stIdentifier + "' found.");
				}
			}
		}

		public int Count => _alParams.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => _alParams;

		internal Guid LanguageModelGuid => _guidLanguageModel;

		internal IDeviceObject2 Device
		{
			get
			{
				return _deviceObject;
			}
			set
			{
				_deviceObject = value;
			}
		}

		internal bool createInstanceVariable
		{
			get
			{
				if (_ioProvider != null && !_bCreateInstanceVariableSet)
				{
					for (IIoProvider val = _ioProvider; val != null; val = val.Parent)
					{
						IDeviceIdentification val2 = null;
						IMetaObject metaObject = val.GetMetaObject();
						if (metaObject.Object is DeviceObject)
						{
							val2 = (metaObject.Object as DeviceObject).DeviceIdentificationNoSimulation;
						}
						if (metaObject.Object is ExplicitConnector)
						{
							IDeviceObject deviceObject = (metaObject.Object as ExplicitConnector).GetDeviceObject();
							val2 = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
						}
						if (metaObject.Object is SlotDeviceObject)
						{
							val2 = (metaObject.Object as SlotDeviceObject).DeviceIdentificationNoSimulation;
						}
						if (val2 != null)
						{
							ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val2);
							_bCreateInstanceVariable |= LocalTargetSettings.CreateInstanceVariables.GetBoolValue(targetSettingsById);
							if (_bCreateInstanceVariable)
							{
								break;
							}
						}
					}
					_bCreateInstanceVariableSet = true;
				}
				return _bCreateInstanceVariable;
			}
		}

		internal int ConnectorId
		{
			get
			{
				return _connectorid;
			}
			set
			{
				_connectorid = value;
			}
		}

		public bool AlwaysMapping
		{
			get
			{
				return _bAllwaysMapping;
			}
			set
			{
				_bAllwaysMapping = value;
			}
		}

		public string EditorName
		{
			get
			{
				if (_stEditorName != null)
				{
					return _stEditorName.Default;
				}
				return string.Empty;
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

		internal bool HasFileParams
		{
			get
			{
				return _bHasFileParams;
			}
			set
			{
				_bHasFileParams = value;
			}
		}

		internal bool HasFunctionalParams
		{
			get
			{
				return _bHasFunctionalParams;
			}
			set
			{
				_bHasFunctionalParams = value;
			}
		}

		internal bool HasInstanceVariable
		{
			get
			{
				return _bHasInstanceVariable;
			}
			set
			{
				_bHasInstanceVariable = value;
			}
		}

		internal bool HasBidirectionalOutputs
		{
			get
			{
				return _bHasBidirectionalOutputs;
			}
			set
			{
				_bHasBidirectionalOutputs = value;
			}
		}

		public bool CreateBitChannels
		{
			get
			{
				return _bCreateBitChannels;
			}
			set
			{
				_bCreateBitChannels = value;
			}
		}

		public bool HasIoConfigAddress
		{
			get
			{
				return _bHasIoConfigAddress;
			}
			set
			{
				_bHasIoConfigAddress = value;
			}
		}

		public uint NumParams
		{
			get
			{
				return _nNumParams;
			}
			set
			{
				_nNumParams = value;
			}
		}

		public event ParameterEventHandler ParameterAdded
		{
			add
			{
				_ehAdded = WeakMulticastDelegate.CombineUnique(_ehAdded, (Delegate)(object)value);
			}
			remove
			{
				_ehAdded = WeakMulticastDelegate.Remove(_ehAdded, (Delegate)(object)value);
			}
		}

		public event ParameterEventHandler ParameterRemoved
		{
			add
			{
				_ehRemoved = WeakMulticastDelegate.CombineUnique(_ehRemoved, (Delegate)(object)value);
			}
			remove
			{
				_ehRemoved = WeakMulticastDelegate.Remove(_ehRemoved, (Delegate)(object)value);
			}
		}

		public event ParameterChangedEventHandler ParameterChanged
		{
			add
			{
				_ehChanged = WeakMulticastDelegate.CombineUnique(_ehChanged, (Delegate)(object)value);
			}
			remove
			{
				_ehChanged = WeakMulticastDelegate.Remove(_ehChanged, (Delegate)(object)value);
			}
		}

		public event ParameterMovedEventHandler ParameterMoved
		{
			add
			{
				_ehMoved = WeakMulticastDelegate.CombineUnique(_ehMoved, (Delegate)(object)value);
			}
			remove
			{
				_ehMoved = WeakMulticastDelegate.Remove(_ehMoved, (Delegate)(object)value);
			}
		}

		public event ParameterSectionChangedEventHandler ParameterSectionChanged
		{
			add
			{
				_ehSectionChanged = WeakMulticastDelegate.CombineUnique(_ehSectionChanged, (Delegate)(object)value);
			}
			remove
			{
				_ehSectionChanged = WeakMulticastDelegate.Remove(_ehSectionChanged, (Delegate)(object)value);
			}
		}

		public ParameterSet()
			: base()
		{
			_transactionHandler = new ParameterSetTransactionHandler(this);
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		internal ParameterSet(XmlNode node, TypeList typeList, int connectorid, bool bCreateBitChannels)
			: this()
		{
			_transactionHandler = new ParameterSetTransactionHandler(this);
			_bCreateBitChannels = bCreateBitChannels;
			Import(node, typeList, connectorid, bUpdate: false);
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		internal void Update(XmlNode node, TypeList types, int nConnectorId, bool bCreateBitChannels)
		{
			_bCreateBitChannels = bCreateBitChannels;
			Import(node, types, nConnectorId, bUpdate: true);
		}

		internal void ReadParameter(XmlElement xeParam, TypeList types, int nParentSectionId, bool bUpdate, ref long lIndex, LList<long> liParamIds)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			try
			{
				Parameter parameter = new Parameter(xeParam, types, nParentSectionId, this);
				if (parameter.IsExternalDatatype && (int)parameter.ChannelType != 0)
				{
					throw new ArgumentException("I/O channels with external datatype are not supported.");
				}
				liParamIds.Add(parameter.Id);
				if (bUpdate)
				{
					foreach (IUpdateDeviceParametersFactory parameterUpdateFactory in DeviceObjectHelper.GetParameterUpdateFactories((IDeviceObject)(object)Device, ConnectorId))
					{
						IUpdateDeviceParametersFactory val = parameterUpdateFactory;
						if (Contains(parameter.Id))
						{
							if (!val.MergeParameter((IParameterSet)(object)this, GetParameter(parameter.Id), (IParameter)(object)parameter))
							{
								return;
							}
						}
						else if (!val.AddParameter((IParameterSet)(object)this, (IParameter)(object)parameter))
						{
							return;
						}
					}
				}
				if (bUpdate && Contains(parameter.Id))
				{
					parameter = (Parameter)(object)GetParameter(parameter.Id);
					parameter.IndexInDevDesc = lIndex;
					parameter.Update(xeParam, types, nParentSectionId);
				}
				else
				{
					int num = _alParams.BinarySearch(parameter);
					if (num >= 0)
					{
						throw new ArgumentException("Parameter with that id does already exist");
					}
					num = ~num;
					_alParams.Insert(num, parameter);
					if (!DeviceManager.IsDuringPLCOpenImport)
					{
						parameter.IndexInDevDesc = lIndex;
					}
				}
			}
			catch (Exception ex)
			{
				string attribute = xeParam.GetAttribute("ParameterId");
				DeviceMessage deviceMessage = new DeviceMessage($"Could not create parameter with id {attribute}: {ex.Message}", (Severity)2);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
			}
			lIndex++;
		}

		internal void ReadSection(XmlElement xeSection, TypeList types, int nParentSectionId, LList<IParameterSection> alSections, bool bUpdate, ref long lIndex, LList<long> liParamIds)
		{
			try
			{
				bool flag = false;
				if (bUpdate)
				{
					XmlElement nameNode = ParameterSection.GetNameNode(xeSection);
					if (nameNode != null)
					{
						foreach (ParameterSection alSection in alSections)
						{
							if (alSection.GetName().TestEquals(nameNode))
							{
								alSection.Update(xeSection, types, nParentSectionId, ref lIndex, liParamIds);
								flag = true;
							}
						}
					}
				}
				if (!flag)
				{
					ParameterSection parameterSection2 = new ParameterSection(xeSection, types, nParentSectionId, this, bUpdate, ref lIndex, liParamIds);
					if (_htIdToSection != null)
					{
						AddIdToSectionMapping(parameterSection2);
					}
					alSections.Add((IParameterSection)(object)parameterSection2);
				}
			}
			catch (Exception ex)
			{
				DeviceMessage deviceMessage = new DeviceMessage($"Could not create parameter section: {ex.Message}", (Severity)2);
				APEnvironment.MessageStorage.AddMessage((IMessageCategory)(object)DeviceMessageCategory.Instance, (IMessage)(object)deviceMessage);
			}
		}

		private void Import(XmlNode node, TypeList types, int nConnectorId, bool bUpdate)
		{
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Expected O, but got Unknown
			LList<long> val = new LList<long>();
			if (!bUpdate)
			{
				_alParams.Clear();
			}
			ConnectorId = nConnectorId;
			long lIndex = 0L;
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				XmlElement xmlElement = (XmlElement)childNode;
				switch (xmlElement.Name)
				{
				case "EditorName":
					_stEditorName = ParameterDataCache.AddStringRef(new StringRef(xmlElement));
					break;
				case "Parameter":
					ReadParameter(xmlElement, types, -1, bUpdate, ref lIndex, val);
					break;
				case "ParameterSection":
					if (_alSections == null)
					{
						_alSections = new LList<IParameterSection>();
					}
					ReadSection(xmlElement, types, -1, _alSections, bUpdate, ref lIndex, val);
					break;
				}
			}
			if (bUpdate)
			{
				for (int i = 0; i < _alParams.Count; i++)
				{
					Parameter parameter = _alParams[i];
					if (val.Contains(parameter.Id) || parameter.IndexInDevDesc == -1)
					{
						continue;
					}
					bool flag = false;
					foreach (IUpdateDeviceParametersFactory parameterUpdateFactory in DeviceObjectHelper.GetParameterUpdateFactories((IDeviceObject)(object)Device, ConnectorId))
					{
						IUpdateDeviceParametersFactory val2 = parameterUpdateFactory;
						if (val2 is IUpdateDeviceParametersFactory2 && !((IUpdateDeviceParametersFactory2)((val2 is IUpdateDeviceParametersFactory2) ? val2 : null)).RemoveParameter((IParameterSet)(object)this, (IParameter)(object)parameter))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						_alParams.Remove(parameter);
						i--;
					}
				}
			}
			_alParams.Sort();
		}

		public IParameter AddParameter(long lId, string stName, AccessRight onlineaccess, AccessRight offlineaccess, ChannelType channeltype, string stType)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			TypeList typeList = ((Device == null || !(Device is DeviceObject)) ? new TypeList() : ((DeviceObject)(object)Device).Types);
			StringRef name = new StringRef("", "", stName);
			StringRef description = new StringRef();
			Parameter parameter = new Parameter(lId, name, description, onlineaccess, offlineaccess, channeltype, stType, typeList, this, CreateBitChannels);
			AddParameter(parameter);
			return (IParameter)(object)parameter;
		}

		internal void UpdateAddresses()
		{
			if (_ioProvider != null)
			{
				_ioProvider.Strategy.UpdateAddresses(_ioProvider);
			}
		}

		public void RemoveParameter(long lId)
		{
			this._searchParameter.Id = lId;
			int num = this._alParams.BinarySearch(this._searchParameter);
			if (num >= 0)
			{
				Parameter parameter = this._alParams[num];
				this._alParams.RemoveAt(num);
				if (parameter != null)
				{
					if (parameter.DataElementBase is DataElementStructType || parameter.DataElementBase is DataElementUnionType)
					{
						IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
						if (primaryProject != null)
						{
							DeviceObjectHelper.ClearGlobalDataTypes(primaryProject.Handle, parameter.DataElementBase);
							if (this._deviceObject is DeviceObject)
							{
								DeviceObject deviceObject = (this._deviceObject as DeviceObject).GetHostDeviceObject() as DeviceObject;
								if (deviceObject != null)
								{
									foreach (KeyValuePair<string, LDictionary<IDataElement, Guid>> keyValuePair in deviceObject.GlobalDataTypes)
									{
										keyValuePair.Value.Remove(parameter.DataElementBase);
									}
								}
							}
						}
					}
					this.OnParameterRemoved(new ParameterEventArgs(this._connectorid, parameter));
					parameter.SetOwner(null);
				}
			}
		}

		internal bool ContainsParameterSection(ParameterSection section)
		{
			if (_alSections != null)
			{
				return _alSections.Contains((IParameterSection)(object)section);
			}
			return false;
		}

		internal void AddParameter(Parameter param)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			int num = _alParams.BinarySearch(param);
			if (num >= 0)
			{
				throw new ArgumentException("Parameter with that id does already exist");
			}
			if (Device == null)
			{
				throw new ArgumentException("Parameter can not be saved. Referenced device is null.");
			}
			if (param.IsExternalDatatype && (int)param.ChannelType != 0)
			{
				throw new ArgumentException("I/O channels with external datatype are not supported.");
			}
			num = ~num;
			_alParams.Insert(num, param);
			param.SetPositionIds(((IObject)Device).UniqueIdGenerator);
			param.SetOwner(this);
			param.UpdateLanguageModelGuids(bUpgrade: false);
			if (_ioProvider != null)
			{
				param.SetIoProvider(_ioProvider);
				if ((int)param.ChannelType != 0)
				{
					_transactionHandler.UpdateAddresses();
				}
			}
			OnParameterAdded(new ParameterEventArgs(_connectorid, (IParameter)(object)param));
		}

		public void Notify(IParameter parameter, IDataElement dataelement, string[] path)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			OnParameterChanged(new ParameterChangedEventArgs(_connectorid, parameter, dataelement, path));
		}

		public void RaiseParameterMoved(IParameter parameter, IParameterSection3 oldSection)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			OnParameterMoved(new ParameterMovedEventArgs(_connectorid, parameter, oldSection));
		}

		public void RaiseSectionChanged(IParameterSection3 section)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			OnParameterSectionChanged(new ParameterSectionChangedEventArgs(section));
		}

		public IParameter GetParameter(long lParameterId)
		{
			_searchParameter.Id = lParameterId;
			int num = _alParams.BinarySearch(_searchParameter);
			if (num < 0)
			{
				return null;
			}
			return (IParameter)(object)_alParams[num];
		}

		public bool Contains(long lParameterId)
		{
			_searchParameter.Id = lParameterId;
			return _alParams.BinarySearch(_searchParameter) >= 0;
		}

		public override void AfterDeserialize()
		{
			foreach (Parameter alParam in _alParams)
			{
				alParam.SetOwner(this);
			}
			if (_alSections == null)
			{
				return;
			}
			foreach (ParameterSection alSection in _alSections)
			{
				alSection.SetOwner(this);
			}
		}

		public override void AfterClone()
		{
			foreach (Parameter alParam in _alParams)
			{
				alParam.SetOwner(this);
			}
			if (_alSections == null)
			{
				return;
			}
			foreach (ParameterSection alSection in _alSections)
			{
				alSection.SetOwner(this);
			}
		}

		protected virtual void OnParameterAdded(ParameterEventArgs e)
		{
			if (_ehAdded != null)
			{
				_ehAdded.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterRemoved(ParameterEventArgs e)
		{
			if (_ehRemoved != null)
			{
				_ehRemoved.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterMoved(ParameterMovedEventArgs e)
		{
			if (_ehMoved != null)
			{
				_ehMoved.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterChanged(ParameterChangedEventArgs e)
		{
			if (_ehChanged != null)
			{
				_ehChanged.Invoke(new object[2] { this, e });
			}
		}

		protected virtual void OnParameterSectionChanged(ParameterSectionChangedEventArgs e)
		{
			if (_ehSectionChanged != null)
			{
				_ehSectionChanged.Invoke(new object[2] { this, e });
			}
		}

		public IParameter AddParameter(long lId, IStringRef name, IStringRef description, AccessRight onlineaccess, AccessRight offlineaccess, ChannelType channeltype, string stType, IParameterSection section)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			int num;
			if (section == null)
			{
				num = -1;
			}
			else
			{
				num = ((section as ParameterSection) ?? throw new ArgumentException("Invalid section)")).Id;
				if (GetSection(num) == null)
				{
					throw new ArgumentException("Invalid section)");
				}
			}
			Parameter parameter = new Parameter(typeList: (Device == null || !(Device is DeviceObject)) ? new TypeList() : ((DeviceObject)(object)Device).Types, lId: lId, name: new StringRef(name), description: new StringRef(description), onlineaccess: onlineaccess, offlineaccess: offlineaccess, channeltype: channeltype, stType: stType, owner: this, bCreateBitChannels: CreateBitChannels);
			parameter.SetSectionId(num);
			AddParameter(parameter);
			return (IParameter)(object)parameter;
		}

		public IParameter AddParameter(XmlElement xeNode, IParameterSection section)
		{
			int num;
			if (section == null)
			{
				num = -1;
			}
			else
			{
				num = ((section as ParameterSection) ?? throw new ArgumentException("Invalid section)")).Id;
				if (GetSection(num) == null)
				{
					throw new ArgumentException("Invalid section)");
				}
			}
			TypeList typeList = ((Device == null || !(Device is DeviceObject)) ? new TypeList() : ((DeviceObject)(object)Device).Types);
			Parameter parameter = new Parameter(xeNode, typeList, num, this);
			parameter.SetSectionId(num);
			AddParameter(parameter);
			return (IParameter)(object)parameter;
		}

		public IParameterSection AddParameterSection(XmlElement xeNode)
		{
			long lIndex = 0L;
			LList<long> liParamIds = new LList<long>();
			LList<IParameterSection> val = new LList<IParameterSection>();
			TypeList types = ((Device == null || !(Device is DeviceObject)) ? new TypeList() : ((DeviceObject)(object)Device).Types);
			ReadSection(xeNode, types, -1, val, bUpdate: false, ref lIndex, liParamIds);
			_alSections.AddRange((IEnumerable<IParameterSection>)val);
			if (val.Count > 0)
			{
				return val[0];
			}
			return null;
		}

		public IParameterSection2[] GetTopLevelSections()
		{
			if (_alSections == null)
			{
				return (IParameterSection2[])(object)new IParameterSection2[0];
			}
			IParameterSection2[] array = (IParameterSection2[])(object)new IParameterSection2[_alSections.Count];
			_alSections.CopyTo((IParameterSection[])(object)array);
			return array;
		}

		public IParameterSection2 AddParameterSection(IStringRef name, IStringRef description, int nIndex)
		{
			StringRef name2 = new StringRef(name);
			StringRef description2 = new StringRef(description);
			ParameterSection parameterSection = new ParameterSection(name2, description2, -1, this);
			AddParameterSection(parameterSection, nIndex);
			return (IParameterSection2)(object)parameterSection;
		}

		internal void AddParameterSection(ParameterSection section, int nIndex)
		{
			if (_alSections == null)
			{
				_alSections = new LList<IParameterSection>();
			}
			if (nIndex < 0)
			{
				_alSections.Add((IParameterSection)(object)section);
			}
			else
			{
				_alSections.Insert(nIndex, (IParameterSection)(object)section);
			}
			if (_htIdToSection != null)
			{
				AddIdToSectionMapping(section);
			}
		}

		public void RemoveParameterSection(IParameterSection section)
		{
			if (_alSections == null)
			{
				throw new ArgumentException("Invalid section");
			}
			if (section != null && section.Section != null)
			{
				(section.Section as ParameterSection).RemoveSubSection(section);
				return;
			}
			int num = _alSections.IndexOf(section);
			if (num < 0)
			{
				throw new ArgumentException("Invalid section");
			}
			ParameterSection section2 = (ParameterSection)(object)_alSections[num];
			_alSections.RemoveAt(num);
			RemoveIdToSectionMapping(section2);
		}

		public bool Contains(string stIdentifier)
		{
			try
			{
				uint num = uint.Parse(stIdentifier);
				return Contains(num);
			}
			catch
			{
				return false;
			}
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_alParams).CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _alParams.GetEnumerator();
		}

		internal void SetIoProvider(IIoProvider ioProvider)
		{
			_ioProvider = ioProvider;
			foreach (Parameter alParam in _alParams)
			{
				if (alParam.IsFileType)
				{
					_bHasFileParams = true;
				}
				if (alParam.IsFunctional)
				{
					_bHasFunctionalParams = true;
				}
				if (alParam.IsInstanceVariable)
				{
					_bHasInstanceVariable = true;
				}
				if (alParam.BidirectionalOutput)
				{
					_bHasBidirectionalOutputs = true;
				}
				alParam.SetIoProvider(ioProvider);
			}
		}

		internal void UpdateLanguageModelGuids(bool bUpgrade)
		{
			if (!bUpgrade || _guidLanguageModel == Guid.Empty)
			{
				_guidLanguageModel = LanguageModelHelper.CreateDeterministicGuid(_ioProvider.GetMetaObject().ObjectGuid, ParamsListName);
			}
			foreach (Parameter alParam in _alParams)
			{
				alParam.UpdateLanguageModelGuids(bUpgrade);
			}
		}

		internal void SetPositionIds(IUniqueIdGenerator idGenerator)
		{
			foreach (Parameter alParam in _alParams)
			{
				alParam.SetPositionIds(idGenerator);
			}
		}

		internal ParameterSection GetSection(int nId)
		{
			if (_alSections == null || _alSections.Count == 0)
			{
				return null;
			}
			if (_htIdToSection == null)
			{
				InitIdToSectionMap();
			}
			ParameterSection result = default(ParameterSection);
			_htIdToSection.TryGetValue(nId, out result);
			return result;
		}

		internal int CreateSectionId()
		{
			int i;
			for (i = _nLastSectionId + 1; GetSection(i) != null; i++)
			{
			}
			_nLastSectionId = i;
			return i;
		}

		internal void ParameterSectionAdded(ParameterSection section)
		{
			if (_htIdToSection != null)
			{
				AddIdToSectionMapping(section);
			}
		}

		internal void ParameterSectionRemoved(ParameterSection section)
		{
			if (_htIdToSection != null)
			{
				RemoveIdToSectionMapping(section);
			}
		}

		private void InitIdToSectionMap()
		{
			_htIdToSection = new LDictionary<int, ParameterSection>();
			_nLastSectionId = -1;
			foreach (ParameterSection alSection in _alSections)
			{
				AddIdToSectionMapping(alSection);
			}
		}

		private void AddIdToSectionMapping(ParameterSection section)
		{
			_htIdToSection[section.Id]= section;
			_nLastSectionId = Math.Max(section.Id, _nLastSectionId);
			IParameterSection2[] subSections = section.SubSections;
			for (int i = 0; i < subSections.Length; i++)
			{
				ParameterSection section2 = (ParameterSection)(object)subSections[i];
				AddIdToSectionMapping(section2);
			}
		}

		private void RemoveIdToSectionMapping(ParameterSection section)
		{
			if (_htIdToSection != null && _htIdToSection.ContainsKey(section.Id))
			{
				_htIdToSection.Remove(section.Id);
			}
			IParameterSection2[] subSections = section.SubSections;
			for (int i = 0; i < subSections.Length; i++)
			{
				ParameterSection section2 = (ParameterSection)(object)subSections[i];
				RemoveIdToSectionMapping(section2);
			}
		}

		public void BeginUpdate()
		{
			_transactionHandler.BeginUpdate();
		}

		public void EndUpdate()
		{
			_transactionHandler.EndUpdate();
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
			switch (stFunction)
			{
			case "GetAlwaysMapping":
			case "SetAlwaysMapping":
			case "EditorName":
				return true;
			default:
				return false;
			}
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
			switch (stFunction)
			{
			case "GetAlwaysMapping":
				xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(AlwaysMapping);
				break;
			case "SetAlwaysMapping":
				AlwaysMapping = XmlConvert.ToBoolean(functionData.DocumentElement["value"].InnerText);
				break;
			case "EditorName":
				xmlDocument.DocumentElement.InnerText = EditorName;
				break;
			}
			return xmlDocument;
		}

		public void UpdateParameters(IParameterSet newParameterSet, bool bWithAddParameter)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Expected O, but got Unknown
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			LList<object> parameterUpdateFactories = DeviceObjectHelper.GetParameterUpdateFactories((IDeviceObject)(object)_deviceObject, _connectorid);
			if (_ioProvider is IConnector)
			{
				foreach (IUpdateDeviceParametersFactory item in parameterUpdateFactories)
				{
					IDeviceObject2 deviceObject = _deviceObject;
					IIoProvider ioProvider = _ioProvider;
					item.StartUpdateParameters((IDeviceObject)(object)deviceObject, (IConnector)(object)((ioProvider is IConnector) ? ioProvider : null));
				}
			}
			for (int i = 0; i < ((ICollection)newParameterSet).Count; i++)
			{
				Parameter parameter = ((IDataElementCollection)newParameterSet)[i] as Parameter;
				if (parameter.Section is ParameterSection)
				{
					ParameterSection section = parameter.Section as ParameterSection;
					if (!ContainsParameterSection(section) && bWithAddParameter)
					{
						AddParameterSection(section, -1);
					}
				}
				if (Contains(parameter.Id))
				{
					Parameter parameter2 = GetParameter(parameter.Id) as Parameter;
					Debug.Assert(parameter2 != null);
					bool flag = false;
					foreach (IUpdateDeviceParametersFactory item2 in parameterUpdateFactories)
					{
						if (!item2.MergeParameter((IParameterSet)(object)this, (IParameter)(object)parameter2, (IParameter)(object)parameter))
						{
							flag = true;
						}
					}
					if (!flag)
					{
						parameter2.Merge(parameter);
						if (parameter2.Section != null && parameter.Section != null && parameter2.Section.Name != parameter.Section.Name)
						{
							parameter2.MoveToSection(parameter.Section);
						}
					}
				}
				else
				{
					if (!bWithAddParameter)
					{
						continue;
					}
					bool flag2 = false;
					foreach (IUpdateDeviceParametersFactory item3 in parameterUpdateFactories)
					{
						IUpdateDeviceParametersFactory val2 = item3;
						if (Contains(parameter.Id) && !val2.AddParameter((IParameterSet)(object)this, (IParameter)(object)parameter))
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						AddParameter(parameter);
					}
				}
			}
			if (!(_ioProvider is IConnector))
			{
				return;
			}
			foreach (IUpdateDeviceParametersFactory item4 in parameterUpdateFactories)
			{
				IDeviceObject2 deviceObject2 = _deviceObject;
				IIoProvider ioProvider2 = _ioProvider;
				item4.EndUpdateParameters((IDeviceObject)(object)deviceObject2, (IConnector)(object)((ioProvider2 is IConnector) ? ioProvider2 : null));
			}
		}
	}
}
