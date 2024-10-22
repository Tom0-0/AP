#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{a1cc20b9-d6c2-4409-b035-4ba930bd50e1}")]
	[StorageVersion("3.3.0.0")]
	public abstract class ConnectorBase : GenericObject2, IConnector13, IConnector12, IConnector11, IConnector10, IConnector9, IConnector8, IConnector7, IConnector6, IConnector5, IConnector4, IConnector3, IConnector2, IConnector, IIoProvider2, IIoProvider
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ModuleType")]
		[StorageVersion("3.3.0.0")]
		protected ushort _usModuleType;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Interface")]
		[StorageVersion("3.3.0.0")]
		protected string _stInterface;

		[DefaultDuplication(DuplicationMethod.Deep)]
		internal StringRef _stVisibleInterfaceName;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Role")]
		[StorageVersion("3.3.0.0")]
		protected ConnectorRole _role;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("ConnectorId")]
		[StorageVersion("3.3.0.0")]
		protected int _nConnectorId = -1;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Hostpath")]
		[StorageVersion("3.3.0.0")]
		protected int _nHostpath = -1;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Adapters")]
		[StorageVersion("3.3.0.0")]
		protected AdapterList _adapters = new AdapterList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("HostParameterSet")]
		[StorageVersion("3.3.0.0")]
		protected ParameterSet _hostParameterSet = new ParameterSet();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("BusCycleTaskGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidBusCycleTask = Guid.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("DriverInfo")]
		[StorageVersion("3.3.0.0")]
		protected DriverInfo _driverInfo = new DriverInfo();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("CustomItems")]
		[StorageVersion("3.3.0.0")]
		protected CustomItemList _customItems = new CustomItemList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ModuleId")]
		[StorageVersion("3.3.0.0")]
		protected int _nModuleId = -1;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IoUpdateTask")]
		[StorageVersion("3.3.0.0")]
		protected string _stIoUpdateTask;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("AdditionalInterfaces")]
		[StorageVersion("3.3.0.0")]
		protected ArrayList _alAdditionalInterfaces = new ArrayList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Constraints")]
		[StorageVersion("3.3.0.0")]
		protected ArrayList _alConstraints = new ArrayList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("hideInStatusPage")]
		[StorageVersion("3.3.0.0")]
		[StorageDefaultValue(false)]
		protected bool _bHideInStatusPage;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("updateAllowed")]
		[StorageVersion("3.3.0.20")]
		[StorageDefaultValue(true)]
		protected bool _bUpdateAllowed = true;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("fixedInputAddress")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue("")]
		protected string _stFixedInputAddress = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("fixedOutputAddress")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue("")]
		protected string _stFixedOutputAddress = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("DownloadParamsDevDescOrder")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue(false)]
		protected bool _bDownloadParamsDevDescOrder;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("AllowOnlyDevices")]
		[StorageVersion("3.3.2.0")]
		[StorageIgnorable]
		protected ArrayList _alAllowOnlyDevices = new ArrayList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("InitialStatusFlag")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(0u)]
		protected uint _uiInitialStatusFlag;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("UseBlobInitConst")]
		[StorageVersion("3.5.2.0")]
		[StorageDefaultValue(false)]
		protected bool _bUseBlobInitConst;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AlwaysMappingMode")]
		[StorageVersion("3.5.5.0")]
		[StorageDefaultValue(0U)]
		protected AlwaysMappingMode _alwaysMappingMode;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ConnectorGroup")]
		[StorageVersion("3.5.8.0")]
		[StorageDefaultValue(0u)]
		protected uint _uiConnectorGroup;

		[DefaultDuplication(DuplicationMethod.Deep)]
		protected Guid _guidExpectedConnectorObject = Guid.Empty;

		private IoProviderBase _xxxioProviderBase;

		public const int DYNAMIC_HOSTPATH = -1;

		internal IAddressAssignmentStrategy _strategy;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AllowedPages")]
		[StorageVersion("3.3.0.0")]
		protected string[] _stAllowedPages = new string[0];

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AlwaysMapping")]
		[StorageVersion("3.3.0.0")]
		protected bool _bAllwaysMapping;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Client")]
		[StorageVersion("3.3.0.0")]
		protected string _stClient = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("MaxInputSize")]
		[StorageVersion("3.3.0.0")]
		protected uint _nMaxInputSize;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("MaxOutputSize")]
		[StorageVersion("3.3.0.0")]
		protected uint _nMaxOutputSize;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("MaxInOutputSize")]
		[StorageVersion("3.3.0.0")]
		protected uint _nMaxInOutputSize;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("ClientTypeGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidClientTypeGuid = Guid.Empty;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("ClientConnectorInterface")]
		[StorageVersion("3.3.0.0")]
		protected string _stClientConnectorInterface = string.Empty;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("ClientConnectorId")]
		[StorageVersion("3.3.0.0")]
		protected int _nClientConnectorId = -1;

		private Guid _guidPackMode;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("AlwaysMappingDisabled")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(false)]
		protected bool _bAllwaysMappingDisabled;

		[DefaultSerialization("VisibleInterfaceName")]
		[StorageVersion("3.3.0.0")]
		protected StringRef VisibleInterfaceNameSerialization
		{
			get
			{
				return _stVisibleInterfaceName;
			}
			set
			{
				_stVisibleInterfaceName = ParameterDataCache.AddStringRef(value);
			}
		}

		public abstract bool IsExplicit { get; }

		public abstract bool Disabled { get; }

		public abstract bool Excluded { get; }

		internal bool CreateBitChannels
		{
			set
			{
				if (_hostParameterSet != null)
				{
					_hostParameterSet.CreateBitChannels = value;
				}
			}
		}

		public int ModuleType => _usModuleType;

		public string Interface
		{
			get
			{
				return _stInterface;
			}
			set
			{
				_stInterface = value;
			}
		}

		public string VisibleInterfaceName
		{
			get
			{
				string @default = _stVisibleInterfaceName.Default;
				ParameterSet parameterSet = ParameterSet as ParameterSet;
				if (parameterSet != null)
				{
					try
					{
						IStringTable stringTable = parameterSet.StringTable;
						IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
						if (val == null)
						{
							return @default;
						}
						val.ResolveString(_stVisibleInterfaceName.Namespace, _stVisibleInterfaceName.Identifier, _stVisibleInterfaceName.Default, out @default);
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

		public ConnectorRole ConnectorRole => _role;

		public IAdapterList Adapters
		{
			get
			{
				return (IAdapterList)(object)_adapters;
			}
			internal set
			{
				_adapters = value as AdapterList;
			}
		}

		public IParameterSet HostParameterSet => (IParameterSet)(object)_hostParameterSet;

		public Guid BusCycleTaskGuid
		{
			get
			{
				return _guidBusCycleTask;
			}
			set
			{
				_guidBusCycleTask = value;
			}
		}

		public int HostPath => _nHostpath;

		public int ConnectorId
		{
			get
			{
				return _nConnectorId;
			}
			set
			{
				_nConnectorId = value;
				_hostParameterSet.ConnectorId = _nConnectorId;
			}
		}

		public ICustomItemList CustomItems => (ICustomItemList)(object)_customItems;

		public IDriverInfo DriverInfo => (IDriverInfo)(object)_driverInfo;

		public IParameterSet ParameterSet => HostParameterSet;

		int IIoProvider.TypeId => _usModuleType;

		IIoProvider IIoProvider.Parent => GetIoProviderParent();

		IIoProvider[] IIoProvider.Children
		{
			get
			{
				LList<IIoProvider> val = default(LList<IIoProvider>);
				if (DeviceObjectHelper.IsInLateLanguageModel && DeviceObjectHelper.IoProviderChildrens.TryGetValue((IIoProvider)(object)this, out val))
				{
					return val.ToArray();
				}
				LList<IIoProvider> ioProviderChildren = GetIoProviderChildren();
				LList<IIoProvider> val2 = new LList<IIoProvider>(ioProviderChildren.Count);
				foreach (IIoProvider item in ioProviderChildren)
				{
					bool flag = false;
					if (item != null)
					{
						if (DeviceObjectHelper.IsExcludedFromBuild(item.GetMetaObject()))
						{
							flag = true;
						}
						if (!item.Excluded && !flag)
						{
							val2.Add(item);
						}
					}
				}
				if (DeviceObjectHelper.IsInLateLanguageModel)
				{
					DeviceObjectHelper.IoProviderChildrens[(IIoProvider)(object)this]= val2;
				}
				IIoProvider[] array = (IIoProvider[])(object)new IIoProvider[val2.Count];
				val2.CopyTo(array, 0);
				return array;
			}
		}

		public string IoUpdateTask
		{
			get
			{
				if (_stIoUpdateTask == string.Empty)
				{
					return null;
				}
				return _stIoUpdateTask;
			}
			set
			{
				if (value == string.Empty)
				{
					_stIoUpdateTask = null;
				}
				else
				{
					_stIoUpdateTask = value;
				}
			}
		}

		public IAddressAssignmentStrategy Strategy
		{
			get
			{
				if (_strategy != null)
				{
					return _strategy;
				}
				IIoProvider ioProviderParent = GetIoProviderParent();
				if (ioProviderParent != null)
				{
					_strategy = ioProviderParent.Strategy;
					return _strategy;
				}
				if (_strategy == null)
				{
					IDeviceObject deviceObject = GetDeviceObject();
					if (deviceObject is SlotDeviceObject)
					{
						deviceObject = (deviceObject as SlotDeviceObject).GetDeviceObject();
					}
					if (deviceObject is DeviceObject)
					{
						_strategy = (deviceObject as DeviceObject).Strategy;
					}
				}
				return _strategy;
			}
			set
			{
				_strategy = value;
			}
		}

		public int ModuleId
		{
			get
			{
				return _nModuleId;
			}
			set
			{
				_nModuleId = value;
			}
		}

		internal StringRef VisibleInterfaceNameAsStringRef => _stVisibleInterfaceName;

		public bool NeedsBusCycle
		{
			get
			{
				return _driverInfo.NeedsBusCycle;
			}
			set
			{
				_driverInfo.NeedsBusCycle = value;
			}
		}

		public virtual int SubObjectsCount
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				if ((int)_role == 1)
				{
					return 0;
				}
				int num = 0;
				foreach (IAdapterBase item in (IEnumerable)Adapters)
				{
					num += item.SubObjectsCount;
				}
				return num;
			}
		}

		private IoProviderBase IoProviderBase
		{
			get
			{
				if (_xxxioProviderBase == null)
				{
					_xxxioProviderBase = new IoProviderBase();
					UpdateAddresses();
				}
				return _xxxioProviderBase;
			}
		}

		public string[] AllowedPages => _stAllowedPages;

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

		public string Client => _stClient;

		public uint MaxInputSize => _nMaxInputSize;

		public uint MaxOutputSize => _nMaxOutputSize;

		public uint MaxInOutputSize => _nMaxInOutputSize;

		public Guid ClientTypeGuid => _guidClientTypeGuid;

		public string ClientConnectorInterface => _stClientConnectorInterface;

		public int ClientConnectorId => _nClientConnectorId;

		public ArrayList Contraints => _alConstraints;

		public ArrayList AdditionalInterfaces
		{
			get
			{
				return _alAdditionalInterfaces;
			}
			internal set
			{
				_alAdditionalInterfaces = value;
			}
		}

		public bool HideInStatusPage => _bHideInStatusPage;

		public bool UpdateAllowed => _bUpdateAllowed;

		public string FixedInputAddress => _stFixedInputAddress;

		public string FixedOutputAddress => _stFixedOutputAddress;

		public bool DownloadParamsDevDescOrder => _bDownloadParamsDevDescOrder;

		public IDeviceIdentification[] AllowOnlyDevices
		{
			get
			{
				IDeviceIdentification[] array = (IDeviceIdentification[])(object)new IDeviceIdentification[_alAllowOnlyDevices.Count];
				_alAllowOnlyDevices.CopyTo(array);
				return array;
			}
			set
			{
				_alAllowOnlyDevices.Clear();
				_alAllowOnlyDevices.AddRange(value);
			}
		}

		internal uint InitialStatusFlag => _uiInitialStatusFlag;

		public bool AlwaysMappingDisabled
		{
			get
			{
				return _bAllwaysMappingDisabled;
			}
			set
			{
				_bAllwaysMappingDisabled = value;
			}
		}

		public bool UseBlobInitConst
		{
			get
			{
				return _bUseBlobInitConst;
			}
			set
			{
				_bUseBlobInitConst = value;
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

		public uint ConnectorGroup
		{
			get
			{
				return _uiConnectorGroup;
			}
			set
			{
				_uiConnectorGroup = value;
			}
		}

		public IList<IDeviceConstraint> DeviceConstraints
		{
			get
			{
				LList<IDeviceConstraint> val = new LList<IDeviceConstraint>();
				foreach (DeviceConstraint alConstraint in _alConstraints)
				{
					val.Add((IDeviceConstraint)(object)alConstraint);
				}
				return (IList<IDeviceConstraint>)val;
			}
		}

		public abstract IIoProvider GetIoProviderParent();

		public abstract LList<IIoProvider> GetIoProviderChildren();

		public abstract IDeviceObject GetDeviceObject();

		public abstract bool IoProviderEquals(IIoProvider provider);

		public abstract string GetIoBaseName();

		public abstract IMetaObject GetMetaObject();

		public abstract IIoModuleIterator CreateModuleIterator();

		public ConnectorBase()
			: base()
		{
		}

		protected ConnectorBase(ConnectorBase original)
			: this()
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			_usModuleType = original._usModuleType;
			_stInterface = original._stInterface;
			_stVisibleInterfaceName = (StringRef)((GenericObject)original._stVisibleInterfaceName).Clone();
			_role = original._role;
			_nConnectorId = original._nConnectorId;
			_nHostpath = original._nHostpath;
			_adapters = (AdapterList)((GenericObject)original._adapters).Clone();
			_hostParameterSet = (ParameterSet)((GenericObject)original._hostParameterSet).Clone();
			_guidBusCycleTask = original._guidBusCycleTask;
			_driverInfo = (DriverInfo)((GenericObject)original._driverInfo).Clone();
			_customItems = (CustomItemList)((GenericObject)original._customItems).Clone();
			_nModuleId = original._nModuleId;
			_guidExpectedConnectorObject = original._guidExpectedConnectorObject;
			_stIoUpdateTask = original._stIoUpdateTask;
			_stAllowedPages = original._stAllowedPages;
			_bAllwaysMapping = original._bAllwaysMapping;
			_stClient = original._stClient;
			_nMaxOutputSize = original._nMaxOutputSize;
			_nMaxInputSize = original._nMaxInputSize;
			_nMaxInOutputSize = original._nMaxInOutputSize;
			_nClientConnectorId = original._nClientConnectorId;
			_stClientConnectorInterface = original._stClientConnectorInterface;
			_guidClientTypeGuid = original._guidClientTypeGuid;
			_alAdditionalInterfaces = (ArrayList)original._alAdditionalInterfaces.Clone();
			_alConstraints = original._alConstraints;
			_bHideInStatusPage = original._bHideInStatusPage;
			_bUpdateAllowed = original._bUpdateAllowed;
			_stFixedInputAddress = original._stFixedInputAddress;
			_stFixedOutputAddress = original._stFixedOutputAddress;
			_bDownloadParamsDevDescOrder = original._bDownloadParamsDevDescOrder;
			_alAllowOnlyDevices = original._alAllowOnlyDevices;
			_bAllwaysMappingDisabled = original._bAllwaysMappingDisabled;
			_uiInitialStatusFlag = original._uiInitialStatusFlag;
			_bUseBlobInitConst = original._bUseBlobInitConst;
			_alwaysMappingMode = original._alwaysMappingMode;
			_uiConnectorGroup = original._uiConnectorGroup;
		}

		public abstract override object Clone();

		internal static ConnectorRole ParseConnectorRole(string stRole)
		{
			if (!"child".Equals(stRole))
			{
				return (ConnectorRole)0;
			}
			return (ConnectorRole)1;
		}

		internal void GetDevices(AdapterList adapters, LList<object> alFixed, LList<object> alSlots, LList<object> alVars)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			foreach (IAdapter adapter in adapters)
			{
				IAdapter val = adapter;
				LList<object> val2;
				if (val is FixedAdapter)
				{
					val2 = alFixed;
				}
				else if (val is SlotAdapter)
				{
					val2 = alSlots;
				}
				else
				{
					if (!(val is VarAdapter))
					{
						continue;
					}
					val2 = alVars;
				}
				Guid[] modules = val.Modules;
				foreach (Guid guid in modules)
				{
					if (!(guid == Guid.Empty))
					{
						try
						{
							IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(GetMetaObject().ProjectHandle, guid);
							val2.Add((object)objectToRead.Object);
						}
						catch (Exception)
						{
						}
					}
				}
			}
		}

		private void InsertDevicesInUpdateList(int nProjectHandle, bool bVersionUpgrade, AdapterList alAdapters, LList<DeviceIdUpdate> devicesToUpdate)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Expected O, but got Unknown
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			foreach (IAdapter alAdapter in alAdapters)
			{
				IAdapter val = alAdapter;
				if (val is FixedAdapter)
				{
					FixedAdapter fixedAdapter = (FixedAdapter)(object)val;
					for (int i = 0; i < fixedAdapter.DefaultModules.Length; i++)
					{
						DeviceIdentification deviceIdentification = fixedAdapter.DefaultModules[i];
						if (fixedAdapter.ModuleGuidsPlain[i] != Guid.Empty && deviceIdentification != null)
						{
							IModuleIdentification val2 = (IModuleIdentification)(object)((deviceIdentification is IModuleIdentification) ? deviceIdentification : null);
							if (val2 != null)
							{
								devicesToUpdate.Add(new DeviceIdUpdate(nProjectHandle, fixedAdapter.ModuleGuidsPlain[i], (IDeviceIdentification)(object)val2));
							}
							else
							{
								devicesToUpdate.Add(new DeviceIdUpdate(nProjectHandle, fixedAdapter.ModuleGuidsPlain[i], (IDeviceIdentification)(object)deviceIdentification));
							}
						}
					}
					continue;
				}
				if (val is SlotAdapter)
				{
					SlotAdapter slotAdapter = (SlotAdapter)(object)val;
					for (int j = 0; j < slotAdapter.ModulesCount; j++)
					{
						IDeviceIdentification val3 = slotAdapter.GetDefaultDevice(j);
						if (slotAdapter.ModuleGuidsPlain[j] != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(nProjectHandle, slotAdapter.ModuleGuidsPlain[j]))
						{
							IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, slotAdapter.ModuleGuidsPlain[j]);
							if (objectToRead.Object is IDeviceObject5)
							{
								IObject @object = objectToRead.Object;
								IDeviceObject5 val4 = (IDeviceObject5)(object)((@object is IDeviceObject5) ? @object : null);
								if (val3 == null)
								{
									val3 = val4.DeviceIdentificationNoSimulation;
								}
								else
								{
									IDeviceIdentification deviceIdentificationNoSimulation = val4.DeviceIdentificationNoSimulation;
									if (deviceIdentificationNoSimulation is IModuleIdentification)
									{
										if (val3.Type == deviceIdentificationNoSimulation.Type && val3.Id.Equals(deviceIdentificationNoSimulation.Id) && (bVersionUpgrade || val3.Version.Equals(deviceIdentificationNoSimulation.Version)))
										{
											val3 = deviceIdentificationNoSimulation;
										}
									}
									else
									{
										val3 = deviceIdentificationNoSimulation;
									}
								}
							}
						}
						if (slotAdapter.ModuleGuidsPlain[j] != Guid.Empty && val3 != null)
						{
							IModuleIdentification val5 = (IModuleIdentification)(object)((val3 is IModuleIdentification) ? val3 : null);
							if (val5 != null)
							{
								devicesToUpdate.Add(new DeviceIdUpdate(nProjectHandle, slotAdapter.ModuleGuidsPlain[j], (IDeviceIdentification)(object)val5));
							}
						}
					}
					continue;
				}
				Guid[] modules = val.Modules;
				foreach (Guid guid in modules)
				{
					try
					{
						if (guid != Guid.Empty)
						{
							IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, guid);
							IModuleIdentification val6 = null;
							if (objectToRead2.Object is IDeviceObject)
							{
								object obj = (object)(IDeviceObject)objectToRead2.Object;
								IDeviceIdentification deviceIdentificationNoSimulation2 = ((IDeviceObject5)((obj is IDeviceObject5) ? obj : null)).DeviceIdentificationNoSimulation;
								val6 = (IModuleIdentification)(object)((deviceIdentificationNoSimulation2 is IModuleIdentification) ? deviceIdentificationNoSimulation2 : null);
							}
							if (objectToRead2.Object is IExplicitConnector)
							{
								IDeviceObject deviceObject = ((IConnector)(IExplicitConnector)objectToRead2.Object).GetDeviceObject();
								IDeviceIdentification deviceIdentificationNoSimulation3 = ((IDeviceObject5)((deviceObject is IDeviceObject5) ? deviceObject : null)).DeviceIdentificationNoSimulation;
								val6 = (IModuleIdentification)(object)((deviceIdentificationNoSimulation3 is IModuleIdentification) ? deviceIdentificationNoSimulation3 : null);
							}
							if (val6 != null)
							{
								devicesToUpdate.Add(new DeviceIdUpdate(objectToRead2.ProjectHandle, objectToRead2.ObjectGuid, (IDeviceIdentification)(object)val6));
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		internal void Update(XmlNode node, TypeList types, LList<Guid> droppedDevices, LList<DeviceIdUpdate> devicesToUpdate, IDeviceIdentification deviceId, bool bVersionUpgrade, bool bCreateBitChannels)
		{
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Expected O, but got Unknown
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Expected O, but got Unknown
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Expected O, but got Unknown
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			bool flag = _usModuleType < 32768 || bVersionUpgrade;
			AdapterList adapters = (AdapterList)((GenericObject)_adapters).Clone();
			if (flag)
			{
				NeedsBusCycle = false;
			}
			Import(node, types, deviceId, flag, bCreateBitChannels);
			LList<object> val = new LList<object>();
			LList<object> val2 = new LList<object>();
			LList<object> val3 = new LList<object>();
			GetDevices(adapters, val, val2, val3);
			foreach (IAdapterBase adapter in _adapters)
			{
				if (adapter is FixedAdapter)
				{
					adapter.UpdateModules(val, (IConnector7)(object)this);
					if (val2.Count > 0)
					{
						adapter.UpdateModules(val2, (IConnector7)(object)this);
					}
				}
				else if (adapter is SlotAdapter)
				{
					adapter.UpdateModules(val2, (IConnector7)(object)this);
					if (val.Count > 0)
					{
						adapter.UpdateModules(val, (IConnector7)(object)this);
					}
				}
				else if (adapter is VarAdapter)
				{
					adapter.UpdateModules(val3, (IConnector7)(object)this);
				}
			}
			foreach (IAdapterBase adapter2 in _adapters)
			{
				if (adapter2 is VarAdapter && val.Count > 0)
				{
					adapter2.UpdateModules(val, (IConnector7)(object)this);
				}
			}
			foreach (IDeviceObject item in val)
			{
				IDeviceObject val4 = item;
				droppedDevices.Add(((IObject)val4).MetaObject.ObjectGuid);
			}
			foreach (IDeviceObject item2 in val2)
			{
				IDeviceObject val5 = item2;
				droppedDevices.Add(((IObject)val5).MetaObject.ObjectGuid);
			}
			foreach (IDeviceObject item3 in val3)
			{
				IDeviceObject val6 = item3;
				droppedDevices.Add(((IObject)val6).MetaObject.ObjectGuid);
			}
			IMetaObject metaObject = GetMetaObject();
			if (metaObject != null)
			{
				LList<DeviceIdUpdate> val7 = new LList<DeviceIdUpdate>();
				InsertDevicesInUpdateList(metaObject.ProjectHandle, bVersionUpgrade, _adapters, val7);
				foreach (DeviceIdUpdate item4 in val7)
				{
					IDeviceIdentification deviceID = null;
					IDeviceIdentification val8 = null;
					val8 = (IDeviceIdentification)(object)((!(item4.DeviceId is IModuleIdentification)) ? ((ModuleIdentification)(object)deviceId) : new ModuleIdentification
					{
						Id = deviceId.Id,
						Type = deviceId.Type,
						Version = deviceId.Version,
						ModuleId = ((IModuleIdentification)item4.DeviceId).ModuleId
					});
					if (APEnvironment.DeviceMgr.TryGetTargetDeviceID(item4.ProjectHandle, item4.ObjectGuid, val8, out deviceID))
					{
						devicesToUpdate.Add(new DeviceIdUpdate(item4.ProjectHandle, item4.ObjectGuid, deviceID));
					}
					else if (item4.DeviceId is IModuleIdentification)
					{
						DeviceIdUpdate deviceIdUpdate = new DeviceIdUpdate(item4.ProjectHandle, item4.ObjectGuid, val8);
						devicesToUpdate.Add(deviceIdUpdate);
					}
					else
					{
						devicesToUpdate.Add(item4);
					}
				}
			}
			base.AfterClone();
		}

		internal void Import(XmlNode node, TypeList types, IDeviceIdentification deviceId, bool bUpdate, bool bCreateBitChannels)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(node is XmlElement);
			XmlElement xmlElement = (XmlElement)node;
			_stVisibleInterfaceName = null;
			_usModuleType = ushort.Parse(xmlElement.GetAttribute(ConnectorTagAttributes.ModuleType));
			_stInterface = xmlElement.GetAttribute(ConnectorTagAttributes.Interface);
			_role = ParseConnectorRole(xmlElement.GetAttribute(ConnectorTagAttributes.Role));
			string attribute = xmlElement.GetAttribute(ConnectorTagAttributes.AlwaysMapping);
			string attribute2 = xmlElement.GetAttribute(ConnectorTagAttributes.AlwaysMappingDisabled);
			if (!bUpdate)
			{
				_bAllwaysMapping = DeviceObjectHelper.ParseBool(attribute, bDefault: false);
				_bAllwaysMappingDisabled = DeviceObjectHelper.ParseBool(attribute2, bDefault: false);
				attribute = xmlElement.GetAttribute("alwaysmappingMode");
				if (!string.IsNullOrEmpty(attribute))
				{
					_alwaysMappingMode = DeviceObjectHelper.ParseAlwaysMappingMode(attribute, (AlwaysMappingMode)0);
				}
			}
			else
			{
				_bAllwaysMapping = DeviceObjectHelper.ParseBool(attribute, _bAllwaysMapping);
				_bAllwaysMappingDisabled = DeviceObjectHelper.ParseBool(attribute2, _bAllwaysMappingDisabled);
				attribute = xmlElement.GetAttribute("alwaysmappingMode");
				if (!string.IsNullOrEmpty(attribute))
				{
					_alwaysMappingMode = DeviceObjectHelper.ParseAlwaysMappingMode(attribute, _alwaysMappingMode);
				}
			}
			attribute = xmlElement.GetAttribute(ConnectorTagAttributes.InitialStatusFlag);
			if (!bUpdate)
			{
				_uiInitialStatusFlag = DeviceObjectHelper.ParseUInt(attribute, 1u);
			}
			else
			{
				_uiInitialStatusFlag = DeviceObjectHelper.ParseUInt(attribute, _uiInitialStatusFlag);
			}
			_nConnectorId = DeviceObjectHelper.ParseInt(xmlElement.GetAttribute(ConnectorTagAttributes.ConnectorId), -1);
			if ((int)_role == 0)
			{
				_nHostpath = DeviceObjectHelper.ParseInt(xmlElement.GetAttribute("hostpath"), -1);
				if (_nConnectorId == _nHostpath && _nHostpath != -1)
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)7, Strings.ErrorWrongHostPath);
				}
			}
			else
			{
				_nHostpath = -1;
			}
			attribute = xmlElement.GetAttribute(ConnectorTagAttributes.UseBlobInitConstFlag);
			_bUseBlobInitConst = DeviceObjectHelper.ParseBool(attribute, bDefault: false);
			_adapters = new AdapterList();
			_alAdditionalInterfaces.Clear();
			_alConstraints.Clear();
			_alAllowOnlyDevices.Clear();
			attribute = xmlElement.GetAttribute(ConnectorTagAttributes.HideInStatusPage);
			_bHideInStatusPage = DeviceObjectHelper.ParseBool(attribute, bDefault: false);
			attribute = xmlElement.GetAttribute(ConnectorTagAttributes.UpdateAllowed);
			_bUpdateAllowed = DeviceObjectHelper.ParseBool(attribute, bDefault: true);
			_stFixedInputAddress = xmlElement.GetAttribute(ConnectorTagAttributes.FixedInputAddress);
			_stFixedOutputAddress = xmlElement.GetAttribute(ConnectorTagAttributes.FixedOutputAddress);
			_bDownloadParamsDevDescOrder = DeviceObjectHelper.ParseBool(xmlElement.GetAttribute(ConnectorTagAttributes.DownloadParamsDevDescOrder), bDefault: false);
			bool flag = false;
			if (bUpdate)
			{
				_stAllowedPages = new string[0];
			}
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				XmlNodeReader xmlNodeReader = new XmlNodeReader(childNode);
				xmlNodeReader.Read();
				switch (childNode.Name)
				{
				case "InterfaceName":
					_stVisibleInterfaceName = ParameterDataCache.AddStringRef(new StringRef((XmlReader)xmlNodeReader));
					break;
				case "Var":
					_adapters.Add((IAdapter)(object)new VarAdapter(xmlNodeReader, deviceId));
					break;
				case "Fixed":
					_adapters.Add((IAdapter)(object)new FixedAdapter(xmlNodeReader, deviceId));
					break;
				case "Slot":
				case "Slots":
					_adapters.Add((IAdapter)(object)new SlotAdapter(xmlNodeReader, deviceId));
					break;
				case "HostParameterSet":
					if (bUpdate)
					{
						_hostParameterSet.Update(childNode, types, _nConnectorId, bCreateBitChannels);
					}
					else
					{
						_hostParameterSet = new ParameterSet(childNode, types, _nConnectorId, bCreateBitChannels);
					}
					break;
				case "DriverInfo":
					flag = true;
					if (bUpdate)
					{
						_driverInfo.Import(childNode, bUpdate: true);
					}
					else
					{
						_driverInfo = new DriverInfo(childNode);
					}
					break;
				case "Custom":
					_customItems = new CustomItemList((XmlElement)childNode);
					break;
				case "Appearance":
				{
					ArrayList arrayList = new ArrayList();
					foreach (XmlNode childNode2 in childNode.ChildNodes)
					{
						if (childNode2.Name == "ShowEditor")
						{
							arrayList.Add(childNode2.InnerText);
						}
					}
					_stAllowedPages = new string[arrayList.Count];
					for (int i = 0; i < arrayList.Count; i++)
					{
						_stAllowedPages[i] = (string)arrayList[i];
					}
					break;
				}
				case "Client":
				{
					XmlElement xmlElement2 = (XmlElement)childNode;
					_stClient = xmlElement2.GetAttribute("name");
					uint.TryParse(xmlElement2.GetAttribute("maxinputsize"), out _nMaxInputSize);
					uint.TryParse(xmlElement2.GetAttribute("maxoutputsize"), out _nMaxOutputSize);
					uint.TryParse(xmlElement2.GetAttribute("maxinoutputsize"), out _nMaxInOutputSize);
					int.TryParse(xmlElement2.GetAttribute("clientconnectorid"), out _nClientConnectorId);
					_stClientConnectorInterface = xmlElement2.GetAttribute("clientconnectorinterface");
					string attribute4 = xmlElement2.GetAttribute("clienttypeguid");
					try
					{
						if (!string.IsNullOrEmpty(attribute4))
						{
							_guidClientTypeGuid = new Guid(attribute4);
						}
					}
					catch
					{
					}
					break;
				}
				case "AdditionalInterface":
				{
					string attribute3 = xmlNodeReader.GetAttribute(ConnectorTagAttributes.Interface);
					_alAdditionalInterfaces.Add(attribute3);
					break;
				}
				case "Constraint":
				{
					uint uiMaxNumber = DeviceObjectHelper.ParseUInt(xmlNodeReader.GetAttribute("MaxNumber"), 0u);
					bool bCheckRecursive = DeviceObjectHelper.ParseBool(xmlNodeReader.GetAttribute("CheckRecursive"), bDefault: false);
					DeviceConstraint value2 = new DeviceConstraint(DeviceRefHelper.ReadDeviceRef(xmlNodeReader, deviceId), uiMaxNumber, bCheckRecursive);
					_alConstraints.Add(value2);
					break;
				}
				case "AllowOnly":
				{
					DeviceIdentification value = DeviceRefHelper.ReadDeviceRef(xmlNodeReader, deviceId);
					_alAllowOnlyDevices.Add(value);
					break;
				}
				}
			}
			if (bUpdate && !flag)
			{
				_driverInfo = new DriverInfo();
			}
			if (_stVisibleInterfaceName == null)
			{
				_stVisibleInterfaceName = new StringRef("", "", _stInterface);
			}
			if (_hostParameterSet != null)
			{
				_hostParameterSet.SetIoProvider((IIoProvider)(object)this);
				_hostParameterSet.CreateBitChannels = bCreateBitChannels;
			}
			_driverInfo.SetIoProvider((IIoProvider)(object)this);
		}

		public void SetUserBaseAddress(DirectVariableLocation location, string stBaseAddress)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			IoProviderBase.SetUserBaseAddress(location, stBaseAddress);
		}

		public string GetUserBaseAddress(DirectVariableLocation location)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return IoProviderBase.GetUserBaseAddress(location);
		}

		public IDirectVariable GetNextUnusedAddress(DirectVariableLocation location)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return IoProviderBase.GetNextUnusedAddress(location);
		}

		public void SetNextUnusedAddress(IDirectVariable addr)
		{
			IoProviderBase.SetNextUnusedAddress(addr);
		}

		public string GetParamsListName()
		{
			return GetIoBaseName() + "_HPS_" + ConnectorId;
		}

		public int GetGranularity(DirectVariableLocation location)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return IoProviderBase.GetGranularity(location);
		}

		public void SetGranularity(DirectVariableLocation location, int nGranularity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			IoProviderBase.SetGranularity(location, nGranularity);
		}

		public IDeviceObject GetHost()
		{
			IIoProvider ioProviderParent = GetIoProviderParent();
			if (ioProviderParent == null)
			{
				return null;
			}
			return ioProviderParent.GetHost();
		}

		public void GetModuleAddress(out uint uiModuleType, out uint uiInstance)
		{
			IoProviderBase.GetModuleAddress((IIoProvider)(object)this, out uiModuleType, out uiInstance);
		}

		public void UpdateAddresses()
		{
			if (_stFixedInputAddress != string.Empty)
			{
				IoProviderBase.SetUserBaseAddress((DirectVariableLocation)1, _stFixedInputAddress);
			}
			if (_stFixedOutputAddress != string.Empty)
			{
				IoProviderBase.SetUserBaseAddress((DirectVariableLocation)2, _stFixedOutputAddress);
			}
			if (_xxxioProviderBase == null)
			{
				_xxxioProviderBase = new IoProviderBase();
			}
			Strategy.UpdateAddresses((IIoProvider)(object)this);
		}

		public IMetaObject GetApplication()
		{
			IIoProvider ioProviderParent = GetIoProviderParent();
			if (ioProviderParent != null)
			{
				return ioProviderParent.GetApplication();
			}
			return null;
		}

		public IMetaObject GetPlcLogic()
		{
			IIoProvider ioProviderParent = GetIoProviderParent();
			if (ioProviderParent != null)
			{
				return ioProviderParent.GetPlcLogic();
			}
			return null;
		}

		public override void AfterDeserialize()
		{
			base.AfterDeserialize();
			_driverInfo.SetIoProvider((IIoProvider)(object)this);
			if (_hostParameterSet == null)
			{
				_hostParameterSet = new ParameterSet();
			}
			_hostParameterSet.ConnectorId = ConnectorId;
			_hostParameterSet.SetIoProvider((IIoProvider)(object)this);
		}

		public override void AfterClone()
		{
			base.AfterClone();
			_driverInfo.SetIoProvider((IIoProvider)(object)this);
			if (_hostParameterSet != null)
			{
				_hostParameterSet.ConnectorId = ConnectorId;
				_hostParameterSet.SetIoProvider((IIoProvider)(object)this);
			}
		}

		internal virtual void PreparePaste()
		{
			foreach (IAdapterBase item in (IEnumerable)Adapters)
			{
				item.PreparePaste();
			}
		}

		public virtual void OnDeviceRenamed(string stOldDeviceName)
		{
			IDeviceObject deviceObject = GetDeviceObject();
			IMetaObject moDevice = ((deviceObject != null) ? ((IObject)deviceObject).MetaObject : null);
			IMetaObject application = GetApplication();
			if (_driverInfo != null)
			{
				_driverInfo.OnDeviceRenamed(application, moDevice, stOldDeviceName);
			}
		}

		public virtual void UpdateChildObjects(int nProjectHandle, Guid guidDevice, ref int nModuleOffset, bool bUpdate, bool bCreateBitChannels, bool bVersionUpgrade, LList<DeviceIdUpdate> liDevicesToUpdate = null)
		{
			foreach (IAdapter item in (IEnumerable)Adapters)
			{
				IAdapter val = item;
				if (val is VarAdapter && !bUpdate)
				{
					VarAdapter varAdapter = (VarAdapter)(object)val;
					if (varAdapter.DefaultModules != null && varAdapter.DefaultModules.Length != 0)
					{
						for (int i = 0; i < varAdapter.DefaultModules.Length; i++)
						{
							nModuleOffset++;
							DeviceIdentification deviceIdentification = varAdapter.DefaultModules[i];
							DeviceObject deviceObject;
							try
							{
								deviceObject = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification);
							}
							catch (DeviceNotFoundException ex)
							{
								ex.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, Interface);
								throw;
							}
							deviceObject.ConnectorIDForChild = ConnectorId;
							if (deviceObject.DeviceInfo == null)
							{
								continue;
							}
							string baseName = DeviceObjectHelper.GetBaseName(deviceIdentification.BaseName, DeviceObjectHelper.CreateInstanceNameBase(deviceObject.DeviceInfo));
							baseName = DeviceObjectHelper.BuildIecIdentifier(baseName);
							string text = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, baseName, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
							Guid guidObject = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)deviceObject, text, nModuleOffset - 1);
							DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guidObject);
							if (varAdapter.SubDevicesCollapsed)
							{
								INavigatorControl3 navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null;
								if (navigatorControl != null)
								{
									navigatorControl.Collapse(guidDevice);
								}
							}
						}
					}
					else
					{
						nModuleOffset += val.ModulesCount;
					}
				}
				else if (val is FixedAdapter)
				{
					FixedAdapter fixedAdapter = (FixedAdapter)(object)val;
					for (int j = 0; j < fixedAdapter.DefaultModules.Length; j++)
					{
						nModuleOffset++;
						DeviceIdentification deviceIdentification2 = fixedAdapter.DefaultModules[j];
						DeviceObject deviceObject2;
						try
						{
							deviceObject2 = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification2);
						}
						catch (DeviceNotFoundException ex2)
						{
							ex2.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, Interface);
							throw;
						}
						if (fixedAdapter.ModuleGuidsPlain[j] != Guid.Empty)
						{
							if (!bUpdate)
							{
								continue;
							}
							IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, fixedAdapter.ModuleGuidsPlain[j]);
							try
							{
								if (objectToModify != null && objectToModify is IMetaObject3 && objectToModify.Object is SlotDeviceObject && (objectToModify.Object as SlotDeviceObject).HasDevice)
								{
									DeviceObject device = (objectToModify.Object as SlotDeviceObject).GetDevice();
									((IMetaObject2)((objectToModify is IMetaObject3) ? objectToModify : null)).ReplaceObject((IObject)(object)device);
								}
								if (objectToModify == null || !(objectToModify.Object is DeviceObject))
								{
									continue;
								}
								DeviceObject deviceObject3 = objectToModify.Object as DeviceObject;
								LList<DeviceIdUpdate> devicesToUpdate = new LList<DeviceIdUpdate>();
								deviceObject3.SetDeviceIdentification((IDeviceIdentification)(object)deviceIdentification2, bUpdate: true, devicesToUpdate);
								bool flag = false;
								foreach (IConnector item2 in (IEnumerable)deviceObject3.Connectors)
								{
									IConnector val3 = item2;
									foreach (IConnector item3 in (IEnumerable)deviceObject2.Connectors)
									{
										IConnector val4 = item3;
										if (!DeviceManager.CheckMatchInterface((IConnector7)(object)((val4 is IConnector7) ? val4 : null), (IConnector7)(object)((val3 is IConnector7) ? val3 : null)) || val4.ModuleType != val3.ModuleType || val4.ConnectorId != val3.ConnectorId || val4.ConnectorRole != val3.ConnectorRole)
										{
											continue;
										}
										flag = true;
										ParameterSet parameterSet = val3.HostParameterSet as ParameterSet;
										parameterSet.UpdateParameters((IParameterSet)(object)(val4.HostParameterSet as ParameterSet), bWithAddParameter: false);
										foreach (Parameter item4 in parameterSet)
										{
											item4.UpdateLanguageModelGuids(bUpdate);
										}
										parameterSet.SetIoProvider((IIoProvider)(object)((val3 is IIoProvider) ? val3 : null));
										break;
									}
								}
								if (flag)
								{
									continue;
								}
								foreach (IConnector item5 in (IEnumerable)deviceObject3.Connectors)
								{
									IConnector val5 = item5;
									foreach (IConnector item6 in (IEnumerable)deviceObject2.Connectors)
									{
										IConnector val6 = item6;
										if (!DeviceManager.CheckMatchInterface((IConnector7)(object)((val6 is IConnector7) ? val6 : null), (IConnector7)(object)((val5 is IConnector7) ? val5 : null)))
										{
											continue;
										}
										ParameterSet parameterSet2 = val5.HostParameterSet as ParameterSet;
										parameterSet2.UpdateParameters((IParameterSet)(object)(val6.HostParameterSet as ParameterSet), bWithAddParameter: false);
										foreach (Parameter item7 in parameterSet2)
										{
											item7.UpdateLanguageModelGuids(bUpdate);
										}
										parameterSet2.SetIoProvider((IIoProvider)(object)((val5 is IIoProvider) ? val5 : null));
									}
								}
							}
							finally
							{
								if (objectToModify != null)
								{
									((IObjectManager)APEnvironment.ObjectMgr).SetObject(objectToModify, true, (object)null);
								}
							}
							continue;
						}
						deviceObject2.ConnectorIDForChild = ConnectorId;
						if (deviceObject2.DeviceInfo == null)
						{
							continue;
						}
						string baseName2 = DeviceObjectHelper.GetBaseName(deviceIdentification2.BaseName, DeviceObjectHelper.CreateInstanceNameBase(deviceObject2.DeviceInfo));
						baseName2 = DeviceObjectHelper.BuildIecIdentifier(baseName2);
						string text2 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, baseName2, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
						Guid guid = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)deviceObject2, text2, nModuleOffset - 1);
						DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guid);
						if (fixedAdapter.SubDevicesCollapsed)
						{
							INavigatorControl3 navigatorControl = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null;
							if (navigatorControl != null)
							{
								navigatorControl.Collapse(guidDevice);
							}
						}
					}
				}
				else if (val is SlotAdapter)
				{
					SlotAdapter slotAdapter = (SlotAdapter)(object)val;
					for (int k = 0; k < slotAdapter.ModulesCount; k++)
					{
						nModuleOffset++;
						SlotDeviceObject slotDeviceObject = ((!slotAdapter.HiddenSlot) ? new SlotDeviceObject(Interface, _alAdditionalInterfaces, slotAdapter.AllowEmptySlot) : new HiddenSlotDeviceObject(Interface, _alAdditionalInterfaces, slotAdapter.AllowEmptySlot));
						DeviceIdentification deviceIdentification3 = (DeviceIdentification)(object)slotAdapter.GetDefaultDevice(k);
						bool flag2 = false;
						if (slotAdapter.ModuleGuidsPlain[k] != Guid.Empty)
						{
							bool flag3 = false;
							IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, slotAdapter.ModuleGuidsPlain[k]);
							if (objectToRead != null && objectToRead.Object is SlotDeviceObject)
							{
								bool flag4 = false;
								IDeviceObject device2 = (IDeviceObject)(object)(objectToRead.Object as SlotDeviceObject).GetDevice();
								if (device2 != null)
								{
									foreach (Connector item8 in (IEnumerable)device2.Connectors)
									{
										if (DeviceManager.CheckMatchInterface((IConnector7)(object)item8, (IConnector7)(object)this))
										{
											flag4 = true;
										}
									}
									if (!flag4)
									{
										flag2 = true;
									}
								}
							}
							if (bUpdate && !flag2)
							{
								objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, slotAdapter.ModuleGuidsPlain[k]);
								try
								{
									if (objectToRead != null && objectToRead is IMetaObject3 && objectToRead.Object is DeviceObject)
									{
										DeviceObject deviceObject4 = objectToRead.Object as DeviceObject;
										((IMetaObject2)((objectToRead is IMetaObject3) ? objectToRead : null)).ReplaceObject((IObject)(object)slotDeviceObject);
										try
										{
											bool flag5 = false;
											foreach (Connector item9 in (IEnumerable)deviceObject4.Connectors)
											{
												if ((int)item9.ConnectorRole == 1 && DeviceManager.CheckMatchInterface((IConnector7)(object)item9, (IConnector7)(object)this))
												{
													flag5 = true;
												}
											}
											if (flag5)
											{
												slotDeviceObject.PlugDevice(deviceObject4);
											}
											else
											{
												DeviceObject device3 = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification3);
												slotDeviceObject.PlugDevice(device3);
											}
										}
										catch
										{
										}
									}
									if (objectToRead != null && objectToRead.Object is SlotDeviceObject)
									{
										SlotDeviceObject slotDeviceObject2 = objectToRead.Object as SlotDeviceObject;
										IDeviceObject val8 = (IDeviceObject)(object)slotDeviceObject;
										if (slotDeviceObject2.HasDevice)
										{
											try
											{
												bool flag6 = false;
												if (liDevicesToUpdate != null)
												{
													foreach (DeviceIdUpdate item10 in liDevicesToUpdate)
													{
														if (item10.ObjectGuid == objectToRead.ObjectGuid)
														{
															val8 = (IDeviceObject)(object)new DeviceObject(bCreateBitChannels, item10.DeviceId);
															flag6 = true;
															break;
														}
													}
												}
												if (!flag6)
												{
													if (((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(slotDeviceObject2.DeviceIdentificationNoSimulation) == null)
													{
														continue;
													}
													val8 = (IDeviceObject)(object)new DeviceObject(bCreateBitChannels, slotDeviceObject2.DeviceIdentificationNoSimulation);
												}
											}
											catch
											{
												continue;
											}
											DeviceObject device4 = slotDeviceObject2.GetDevice();
											bool flag7 = false;
											foreach (Connector item11 in (IEnumerable)((IDeviceObject)device4).Connectors)
											{
												foreach (Connector item12 in (IEnumerable)val8.Connectors)
												{
													if (DeviceManager.CheckMatchInterface((IConnector7)(object)item11, (IConnector7)(object)item12))
													{
														flag7 = true;
													}
												}
											}
											if (!flag7)
											{
												flag3 = true;
											}
										}
										foreach (Connector item13 in (IEnumerable)slotDeviceObject2.SlotConnectors)
										{
											foreach (Connector item14 in (IEnumerable)val8.Connectors)
											{
												if (item13.Interface == item14.Interface && item13.ConnectorId == item14.ConnectorId)
												{
													item13._alAdditionalInterfaces = item14._alAdditionalInterfaces;
													foreach (string alAdditionalInterface in _alAdditionalInterfaces)
													{
														if (!item13._alAdditionalInterfaces.Contains(alAdditionalInterface))
														{
															item13._alAdditionalInterfaces.Add(alAdditionalInterface);
														}
													}
												}
												else
												{
													if (item13.ModuleType != item14.ModuleType || item13.ConnectorId != item14.ConnectorId)
													{
														continue;
													}
													item13.Interface = item14.Interface;
													item13._alAdditionalInterfaces = item14._alAdditionalInterfaces;
													flag3 = true;
													foreach (IConnector item15 in (IEnumerable)slotDeviceObject2.Connectors)
													{
														IConnector val9 = item15;
														if (val9.Interface == item14.Interface)
														{
															flag3 = false;
														}
														if (!(val9 is IConnector7) || !DeviceManager.CheckMatchInterface((IConnector7)(object)((val9 is IConnector7) ? val9 : null), (IConnector7)(object)item13))
														{
															continue;
														}
														flag3 = false;
														ParameterSet parameterSet3 = val9.HostParameterSet as ParameterSet;
														parameterSet3.UpdateParameters((IParameterSet)(object)(item14.HostParameterSet as ParameterSet), bWithAddParameter: true);
														foreach (Parameter item16 in parameterSet3)
														{
															item16.UpdateLanguageModelGuids(bUpdate);
														}
														parameterSet3.SetIoProvider((IIoProvider)(object)((val9 is IIoProvider) ? val9 : null));
													}
												}
											}
										}
										foreach (IConnector item17 in (IEnumerable)slotDeviceObject2.Connectors)
										{
											IConnector val10 = item17;
											foreach (IConnector item18 in (IEnumerable)val8.Connectors)
											{
												IConnector val11 = item18;
												if (!(val10.Interface == val11.Interface))
												{
													continue;
												}
												ParameterSet parameterSet4 = val10.HostParameterSet as ParameterSet;
												parameterSet4.UpdateParameters((IParameterSet)(object)(val11.HostParameterSet as ParameterSet), bWithAddParameter: true);
												foreach (Parameter item19 in parameterSet4)
												{
													item19.UpdateLanguageModelGuids(bUpdate);
												}
												parameterSet4.SetIoProvider((IIoProvider)(object)((val10 is IIoProvider) ? val10 : null));
											}
										}
									}
								}
								finally
								{
									if (objectToRead != null)
									{
										((IObjectManager)APEnvironment.ObjectMgr).SetObject(objectToRead, true, (object)null);
									}
								}
								if (flag3 && deviceIdentification3 != null)
								{
									DeviceObject device5;
									try
									{
										device5 = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification3);
									}
									catch (DeviceNotFoundException ex3)
									{
										ex3.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, Interface);
										throw;
									}
									DeviceCommandHelper.PlugDeviceIntoSlot(nProjectHandle, slotAdapter.ModuleGuidsPlain[k], device5, objectToRead.Name);
								}
							}
							if (!flag2)
							{
								continue;
							}
						}
						if (flag2)
						{
							try
							{
								((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(nProjectHandle, slotAdapter.ModuleGuidsPlain[k]);
							}
							catch
							{
							}
						}
						string text4;
						if (deviceIdentification3 != null)
						{
							DeviceObject deviceObject5;
							try
							{
								deviceObject5 = new DeviceObject(bCreateBitChannels, (IDeviceIdentification)(object)deviceIdentification3);
							}
							catch (DeviceNotFoundException ex4)
							{
								ex4.stErrorContext = string.Format(Strings.DeviceForInterfaceNotFound, Interface);
								throw;
							}
							if (deviceObject5.DeviceInfo == null)
							{
								continue;
							}
							string baseName3 = DeviceObjectHelper.GetBaseName(deviceIdentification3.BaseName, DeviceObjectHelper.CreateInstanceNameBase(deviceObject5.DeviceInfo));
							baseName3 = DeviceObjectHelper.BuildIecIdentifier(baseName3);
							text4 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, baseName3, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
							slotDeviceObject.PlugDevice(deviceObject5);
						}
						else
						{
							string slotName = slotAdapter.GetSlotName(k);
							if (!slotName.StartsWith("<"))
							{
								slotName = DeviceObjectHelper.BuildIecIdentifier(slotName);
								text4 = DeviceObjectHelper.CreateUniqueIdentifier(nProjectHandle, slotName, DeviceObjectHelper.GetHostStub(nProjectHandle, guidDevice));
							}
							else
							{
								text4 = slotName;
							}
						}
						slotDeviceObject.ConnectorIDForChild = ConnectorId;
						Guid guid2 = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidDevice, Guid.NewGuid(), (IObject)(object)slotDeviceObject, text4, nModuleOffset - 1);
						if (deviceIdentification3 != null)
						{
							DeviceObjectFactory.ObjectCreatedStatic(nProjectHandle, guid2);
						}
						if (slotAdapter.SubDevicesCollapsed)
						{
							INavigatorControl3 val12 = (APEnvironment.Engine.Frame != null) ? (APEnvironment.Engine.Frame.ActiveView as INavigatorControl3) : null;
							if (val12 != null)
							{
								val12.Collapse(guid2);
							}
						}
					}
				}
				else
				{
					nModuleOffset += val.ModulesCount;
				}
			}
		}

		public bool CheckConstraints(IObject childDevice)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			if (_alConstraints.Count > 0 && childDevice is DeviceObject)
			{
				return CheckConstraints((IDeviceObjectBase)(childDevice as DeviceObject));
			}
			if (_alAllowOnlyDevices != null && _alAllowOnlyDevices.Count > 0 && childDevice is DeviceObject)
			{
				bool flag = true;
				DeviceObject deviceObject = childDevice as DeviceObject;
				foreach (IDeviceIdentification alAllowOnlyDevice in _alAllowOnlyDevices)
				{
					IDeviceIdentification val = alAllowOnlyDevice;
					if (val.Id == deviceObject.DeviceIdentification.Id && val.Type == deviceObject.DeviceIdentification.Type && (val.Version == string.Empty || val.Version == deviceObject.DeviceIdentification.Version))
					{
						flag = false;
					}
				}
				if (flag)
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)6, Strings.Error_Device_Insert);
				}
			}
			return true;
		}

		public bool CheckConstraints(IDeviceObjectBase device)
		{
			return CheckConstraints(device, bRecursion: false);
		}

		public bool CheckConstraints(IDeviceObjectBase device, bool bRecursion)
		{
			DeviceIdentification devIdent = ((IDeviceObject5)device).DeviceIdentificationNoSimulation as DeviceIdentification;
			Guid deviceGuid = Guid.Empty;
			if (((IObject)device).MetaObject != null)
			{
				deviceGuid = ((IObject)device).MetaObject.ObjectGuid;
			}
			return CheckConstraints((IDeviceIdentification)(object)devIdent, bRecursion, deviceGuid, bCheck: false);
		}

		public bool CheckConstraints(IDeviceIdentification devIdent, bool bRecursion, Guid deviceGuid, bool bCheck)
		{
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			if (_alConstraints.Count > 0 && devIdent != null)
			{
				foreach (DeviceConstraint alConstraint in _alConstraints)
				{
					if (!alConstraint.DevIdent.Equals(devIdent, alConstraint.DevIdent.Version == string.Empty))
					{
						continue;
					}
					int num = 0;
					IIoModuleIterator val = CreateModuleIterator();
					if (!val.MoveToFirstChild())
					{
						continue;
					}
					try
					{
						int num2 = 1;
						do
						{
							if (val.Current.IsConnector && val.Current is IoModuleChildConnectorReference)
							{
								IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(val.Current.ProjectHandle, val.Current.ObjectGuid);
								if (objectToRead != null && objectToRead.Object is IDeviceObjectBase && (deviceGuid == Guid.Empty || objectToRead.ObjectGuid != deviceGuid))
								{
									DeviceIdentification deviceIdentification = ((IDeviceObject5)(IDeviceObjectBase)objectToRead.Object).DeviceIdentificationNoSimulation as DeviceIdentification;
									if (deviceIdentification != null && deviceIdentification.Equals(devIdent, alConstraint.DevIdent.Version == string.Empty))
									{
										num++;
									}
								}
							}
							if (alConstraint.CheckRecursive && val.MoveToFirstChild())
							{
								num2++;
								continue;
							}
							while (!val.MoveToNextSibling() && num2 > 0)
							{
								val.MoveToParent();
								num2--;
							}
						}
						while (num2 > 0);
					}
					catch
					{
					}
					if (!bRecursion)
					{
						if (num >= alConstraint.MaxNumber)
						{
							if (bCheck)
							{
								return false;
							}
							throw new DeviceObjectException((DeviceObjectExeptionReason)4, Strings.Error_Constraint_Reached);
						}
					}
					else if (num > alConstraint.MaxNumber)
					{
						if (bCheck)
						{
							return false;
						}
						throw new DeviceObjectException((DeviceObjectExeptionReason)4, Strings.Error_Constraint_Reached);
					}
				}
			}
			return true;
		}

		public virtual bool InsertChild(int nInsertPosition, IObject childDevice, Guid newChildGuid)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			CheckConstraints(childDevice);
			if ((int)ConnectorRole == 1)
			{
				return false;
			}
			Connector connector = GetIoProviderParent() as Connector;
			if (connector != null && (int)connector.ConnectorRole == 1 && newChildGuid != Guid.Empty)
			{
				foreach (IAdapter item in (IEnumerable)connector.Adapters)
				{
					IAdapter val = item;
					if (!(val is SlotAdapter))
					{
						continue;
					}
					Guid[] modules = ((SlotAdapter)(object)val).Modules;
					for (int i = 0; i < modules.Length; i++)
					{
						if (modules[i] == Guid.Empty)
						{
							return false;
						}
					}
				}
			}
			if (!(childDevice is IDeviceObject2))
			{
				return false;
			}
			int num = nInsertPosition;
			foreach (IAdapterBase item2 in (IEnumerable)Adapters)
			{
				if (!(item2 is FixedAdapter) && !(item2 is SlotAdapter))
				{
					continue;
				}
				for (int num2 = nInsertPosition; num2 >= 0; num2--)
				{
					if (item2.CanAddModule(num2, childDevice))
					{
						if (childDevice.MetaObject != null)
						{
							IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(childDevice.MetaObject.ProjectHandle, childDevice.MetaObject.ObjectGuid);
							return DoInsertDevice(num2, item2, metaObjectStub);
						}
						return true;
					}
				}
				nInsertPosition -= ((IAdapter)item2).ModulesCount;
				if (nInsertPosition < 0)
				{
					break;
				}
			}
			nInsertPosition = num;
			foreach (IAdapterBase item3 in (IEnumerable)Adapters)
			{
				for (int num3 = nInsertPosition; num3 >= 0; num3--)
				{
					if (item3.CanAddModule(num3, childDevice))
					{
						if (childDevice.MetaObject != null)
						{
							IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(childDevice.MetaObject.ProjectHandle, childDevice.MetaObject.ObjectGuid);
							return DoInsertDevice(num3, item3, metaObjectStub2);
						}
						return true;
					}
				}
				nInsertPosition -= ((IAdapter)item3).ModulesCount;
				if (nInsertPosition < 0)
				{
					break;
				}
			}
			return false;
		}

		public virtual void RemoveChild(Guid guidChild)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			if ((int)_role == 1)
			{
				return;
			}
			foreach (IAdapter item in (IEnumerable)Adapters)
			{
				IAdapter val = item;
				if (val is VarAdapter)
				{
					((VarAdapter)(object)val).Remove(guidChild);
				}
				else if (val is SlotAdapter)
				{
					((SlotAdapter)(object)val).RemoveDevice(guidChild);
				}
				else if (val is FixedAdapter)
				{
					((FixedAdapter)(object)val).RemoveDevice(guidChild);
				}
			}
		}

		internal virtual void UpdatePasteModuleGuid(Guid oldGuid, Guid newGuid)
		{
			foreach (IAdapterBase item in (IEnumerable)Adapters)
			{
				item.UpdatePasteModuleGuid(oldGuid, newGuid);
			}
		}

		internal virtual bool IsExpectedModule(Guid guidModule)
		{
			foreach (IAdapterBase item in (IEnumerable)Adapters)
			{
				if (item.IsExpectedModule(guidModule))
				{
					return true;
				}
			}
			return false;
		}

		internal virtual bool SetExpectedModule(Guid guidModule)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			foreach (IAdapterBase item in (IEnumerable)Adapters)
			{
				if (!item.SetExpectedModule(guidModule))
				{
					continue;
				}
				if ((int)_role == 0)
				{
					IMetaObject val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(GetMetaObject().ProjectHandle, guidModule);
					try
					{
						((IDeviceObject2)val.Object).SetParent(GetMetaObject().ObjectGuid, -1);
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
						val = null;
					}
					catch
					{
						if (val != null)
						{
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
						}
					}
				}
				return true;
			}
			return false;
		}

		private bool DoInsertDevice(int nInsertPosition, IAdapterBase adapter, IMetaObjectStub mosChild)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			createInChildParameters(mosChild);
			IMetaObject val = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(mosChild.ProjectHandle, mosChild.ObjectGuid);
				if (val != null && val.IsToModify)
				{
					Debug.Assert(val.Object is IDeviceObject2);
					IDeviceObject2 val2 = (IDeviceObject2)val.Object;
					int emptyChildConnectorForInterface = DeviceObjectHelper.GetEmptyChildConnectorForInterface(Interface, val2);
					if (emptyChildConnectorForInterface < 0)
					{
						foreach (string alAdditionalInterface in _alAdditionalInterfaces)
						{
							emptyChildConnectorForInterface = DeviceObjectHelper.GetEmptyChildConnectorForInterface(alAdditionalInterface, val2);
							if (emptyChildConnectorForInterface >= 0)
							{
								break;
							}
						}
					}
					if (emptyChildConnectorForInterface >= 0)
					{
						val2.SetParent(GetMetaObject().ObjectGuid, emptyChildConnectorForInterface);
						adapter.AddModule(nInsertPosition, mosChild);
						if (val2 is IDeviceObject5)
						{
							IDeviceProperty val3 = (IDeviceProperty)(object)new DeviceProperty(((IDeviceObject5)((val2 is IDeviceObject5) ? val2 : null)).DeviceIdentificationNoSimulation);
							val.AddProperty((IObjectProperty)(object)val3);
						}
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
						val = null;
						createHostChildParameters(mosChild);
						return true;
					}
				}
				return false;
			}
			finally
			{
				if (val != null && val.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
				}
			}
		}

		public virtual int GetChildIndex(Guid subObjectGuid, out int nNumSubmoduls)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			int num = 0;
			nNumSubmoduls = SubObjectsCount;
			if ((int)_role == 1)
			{
				return -1;
			}
			foreach (IAdapterBase item in (IEnumerable)Adapters)
			{
				Guid[] modules = ((IAdapter)item).Modules;
				int num2 = Array.IndexOf(modules, subObjectGuid);
				if (num2 < 0)
				{
					num += modules.Length;
					continue;
				}
				return num + num2;
			}
			return -1;
		}

		public void createInChildParameters(IMetaObjectStub mosChild)
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Invalid comparison between Unknown and I4
			bool flag = false;
			foreach (Parameter item in (IEnumerable)ParameterSet)
			{
				if (item.CreateInChildConnector)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			IMetaObject val = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(mosChild.ProjectHandle, mosChild.ObjectGuid);
				if (val == null || !val.IsToModify)
				{
					return;
				}
				IObject @object = val.Object;
				IDeviceObject2 val2 = (IDeviceObject2)(object)((@object is IDeviceObject2) ? @object : null);
				if (val2 == null)
				{
					return;
				}
				foreach (Connector item2 in (IEnumerable)((IDeviceObject)val2).Connectors)
				{
					if ((int)item2.ConnectorRole != 1)
					{
						continue;
					}
					foreach (Parameter item3 in (IEnumerable)ParameterSet)
					{
						if (!item3.CreateInChildConnector)
						{
							continue;
						}
						ParameterSet parameterSet = item2.ParameterSet as ParameterSet;
						if (parameterSet == null || parameterSet.Contains(item3.Id))
						{
							continue;
						}
						if (item3.Section is ParameterSection)
						{
							ParameterSection section = item3.Section as ParameterSection;
							if (!parameterSet.ContainsParameterSection(section))
							{
								parameterSet.AddParameterSection(section, -1);
							}
						}
						parameterSet.AddParameter(item3);
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (val != null && val.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
				}
			}
		}

		public void createHostChildParameters(IMetaObjectStub mosChild)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			createInChildParameters(mosChild);
			if (!typeof(IDeviceObject2).IsAssignableFrom(mosChild.ObjectType))
			{
				return;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mosChild.ProjectHandle, mosChild.ObjectGuid);
			if (objectToRead == null)
			{
				return;
			}
			IObject @object = objectToRead.Object;
			foreach (IConnector item in (IEnumerable)((IDeviceObject)((@object is IDeviceObject2) ? @object : null)).Connectors)
			{
				foreach (Parameter item2 in (IEnumerable)item.HostParameterSet)
				{
					if (!item2.CreateInHostConnector)
					{
						continue;
					}
					ParameterSet parameterSet = HostParameterSet as ParameterSet;
					if (parameterSet == null || ParameterSet.Contains(item2.Id))
					{
						continue;
					}
					if (item2.Section is ParameterSection)
					{
						ParameterSection section = item2.Section as ParameterSection;
						if (!parameterSet.ContainsParameterSection(section))
						{
							parameterSet.AddParameterSection(section, -1);
						}
					}
					parameterSet.AddParameter(item2);
				}
			}
		}

		internal long GetPackMode()
		{
			if (Guid.Empty == _guidPackMode)
			{
				_guidPackMode = default(Guid);
			}
			long packMode = DeviceObjectHelper.GetPackMode(_guidPackMode);
			if (packMode != 0L)
			{
				return packMode;
			}
			packMode = DeviceObjectHelper.GetPackMode(GetHost());
			DeviceObjectHelper.StorePackMode(_guidPackMode, packMode);
			return packMode;
		}

		public bool GetChangedParametersInSet(out List<IParameter> changedParameters)
		{
			return LanguageModelHelper.FindChangedParametersInSet((IIoProvider)(object)this, out changedParameters);
		}

		public void AddDeviceConstraint(bool CheckRecursive, uint MaxNumber, IDeviceIdentification devId)
		{
			DeviceConstraint value = new DeviceConstraint(devId as DeviceIdentification, MaxNumber, CheckRecursive);
			_alConstraints.Add(value);
		}
	}
}
