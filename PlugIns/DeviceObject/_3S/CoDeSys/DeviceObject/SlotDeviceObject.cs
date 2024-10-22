#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{085766fd-043e-4545-8e8d-d651d56d5d3b}")]
	[StorageVersion("3.3.0.0")]
	public class SlotDeviceObject : DeviceObjectBase, IDeviceObjectBase, IDeviceObject16, IDeviceObject15, IDeviceObject14, IDeviceObject13, IDeviceObject12, IDeviceObject11, IDeviceObject10, IDeviceObject9, IDeviceObject8, IDeviceObject7, IDeviceObject6, IDeviceObject5, IDeviceObject4, IDeviceObject3, IDeviceObject2, IDeviceObject, IObject, IGenericObject, IArchivable, ICloneable, IComparable, ILanguageModelProvider3, ILanguageModelProvider2, ILanguageModelProvider, ILanguageModelProviderWithDependencies, IOrderedSubObjects, IHasAssociatedOnlineHelpTopic, IKnowMyOrderedSubObjectsInAdvance, ILogicalDevice2, ILogicalDevice, ILanguageModelProviderBuildPropertiesControl, ISlotDeviceObject2, ISlotDeviceObject, IStructuredLanguageModelProvider
	{
		private const string DEFAULTCONNECTOR_XML = "    <Connector connectorId=\"1\" explicit=\"false\" hostpath=\"-1\" interface=\"{0}\" moduleType=\"0\" role=\"child\">\r\n \t\t<Slot allowEmpty=\"false\" count=\"1\"/>\r\n        {1}\r\n\t\t</Connector>\r\n";

		private static EmptyDriverInfo s_driverInfo = new EmptyDriverInfo();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Device")]
		[StorageVersion("3.3.0.0")]
		protected DeviceObject _device;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ConnectorList")]
		[StorageVersion("3.3.0.0")]
		protected ConnectorList _connectors = new ConnectorList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("AllowEmpty")]
		[StorageVersion("3.3.0.0")]
		protected bool _bAllowEmpty = true;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("showParamsInDevDescOrder")]
		[StorageVersion("3.3.1.0")]
		[StorageDefaultValue(false)]
		protected bool _bShowParamsInDevDescOrder;

		private IMetaObject _metaObject;

		private Guid _guidObject;

		private int _nConnectorIDForChild = -1;

		private SlotDeviceInfo _slotDeviceInfo = new SlotDeviceInfo();

		private CommunicationSettings _communicationSettings = new CommunicationSettings();

		private ParameterSet _deviceParameterSet = new ParameterSet();

		private DeviceAttributes _attributes = new DeviceAttributes();

		private CustomItemList _customItems = new CustomItemList();

		private DeviceIdentification _identification = new DeviceIdentification(0, "0000 0000", "3.0.0.0", null);

		private bool _bIsInUpdate;

		internal int ConnectorIDForChild
		{
			get
			{
				return _nConnectorIDForChild;
			}
			set
			{
				_nConnectorIDForChild = value;
			}
		}

		public bool SimulationMode
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		internal IConnectorCollection SlotConnectors => (IConnectorCollection)(object)_connectors;

		public override IConnectorCollection Connectors
		{
			get
			{
				if (_device == null)
				{
					return (IConnectorCollection)(object)_connectors;
				}
				return _device.Connectors;
			}
		}

		public override IConnectorCollection ConnectorsWithGroups
		{
			get
			{
				if (_device == null)
				{
					return (IConnectorCollection)(object)_connectors;
				}
				return _device.ConnectorsWithGroups;
			}
		}

		public IDeviceInfo DeviceInfo => (IDeviceInfo)(object)_slotDeviceInfo;

		public bool Disable
		{
			get
			{
				if (_device == null)
				{
					return false;
				}
				return _device.Disable;
			}
			set
			{
				if (_device != null)
				{
					_device.Disable = value;
				}
			}
		}

		public ICommunicationSettings CommunicationSettings
		{
			get
			{
				if (_device == null)
				{
					return (ICommunicationSettings)(object)_communicationSettings;
				}
				return _device.CommunicationSettings;
			}
		}

		public override IParameterSet DeviceParameterSet
		{
			get
			{
				if (_device == null)
				{
					return (IParameterSet)(object)_deviceParameterSet;
				}
				return _device.DeviceParameterSet;
			}
		}

		public IDeviceAttributesCollection Attributes
		{
			get
			{
				if (_device == null)
				{
					return (IDeviceAttributesCollection)(object)_attributes;
				}
				return _device.Attributes;
			}
		}

		public ICustomItemList CustomItems
		{
			get
			{
				if (_device == null)
				{
					return (ICustomItemList)(object)_customItems;
				}
				return _device.CustomItems;
			}
		}

		public IDeviceIdentification DeviceIdentification
		{
			get
			{
				if (_device == null)
				{
					return (IDeviceIdentification)(object)_identification;
				}
				return _device.DeviceIdentification;
			}
			set
			{
				if (_device == null)
				{
					throw new InvalidOperationException("Cannot set the identification of an empty slot");
				}
				_device.DeviceIdentification = value;
			}
		}

		public IDeviceIdentification DeviceIdentificationNoSimulation => DeviceIdentification;

		public bool Exclude
		{
			get
			{
				if (_device == null)
				{
					return false;
				}
				return _device.Exclude;
			}
			set
			{
				if (_device != null)
				{
					_device.Exclude = value;
				}
			}
		}

		public bool AllowsDirectCommunication
		{
			get
			{
				if (_device == null)
				{
					return false;
				}
				return _device.AllowsDirectCommunication;
			}
		}

		public IDriverInfo DriverInfo
		{
			get
			{
				if (_device == null)
				{
					return (IDriverInfo)(object)s_driverInfo;
				}
				return _device.DriverInfo;
			}
		}

		public Guid[] FunctionalChildrenTypeGuids
		{
			get
			{
				if (_device == null)
				{
					return new Guid[0];
				}
				return _device.FunctionalChildrenTypeGuids;
			}
			set
			{
				if (_device == null)
				{
					throw new InvalidOperationException("Cannot modify the list of functional children of an empty slot");
				}
				_device.FunctionalChildrenTypeGuids = value;
			}
		}

		public bool CanRename
		{
			get
			{
				if (_device != null)
				{
					return _device.CanRename;
				}
				return false;
			}
		}

		public Guid Namespace
		{
			get
			{
				if (_device == null)
				{
					return Guid.Empty;
				}
				return _device.Namespace;
			}
		}

		public IMetaObject MetaObject
		{
			get
			{
				return _metaObject;
			}
			set
			{
				if (_metaObject != null && value == null)
				{
					_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
				}
				else
				{
					_metaObject = value;
				}
				if (_metaObject != null)
				{
					_nProjectHandle = _metaObject.ProjectHandle;
					_guidObject = _metaObject.ObjectGuid;
				}
				if (_device != null)
				{
					_device.MetaObject = _metaObject;
				}
			}
		}

		public IEmbeddedObject[] EmbeddedObjects
		{
			get
			{
				if (_device == null)
				{
					return null;
				}
				return _device.EmbeddedObjects;
			}
		}

		public IUniqueIdGenerator UniqueIdGenerator
		{
			get
			{
				if (_device == null)
				{
					return null;
				}
				return _device.UniqueIdGenerator;
			}
		}

		public IStringTable2 StringTable
		{
			get
			{
				if (_device == null)
				{
					return (IStringTable2)(object)new StringTable();
				}
				return _device.StringTable;
			}
		}

		public IStringTable3 StringTable3
		{
			get
			{
				if (_device == null)
				{
					return (IStringTable3)(object)new StringTable();
				}
				return _device.StringTable3;
			}
		}

		public bool IsInUpdate
		{
			get
			{
				return _bIsInUpdate;
			}
			internal set
			{
				_bIsInUpdate = value;
			}
		}

		public bool HasDevice => _device != null;

		internal bool AllowEmpty => _bAllowEmpty;

		public bool InheritedDisable
		{
			get
			{
				if (_device != null)
				{
					return Disable | _device.InheritedDisable;
				}
				return Disable;
			}
		}

		public Guid[] ObjectsToUpdate
		{
			get
			{
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				if (_device == null)
				{
					IIoProvider val = (IIoProvider)(object)(Connector)(object)_connectors[0];
					while (val.Parent != null)
					{
						val = val.Parent;
					}
					if (val is IDeviceObject)
					{
						return new Guid[1] { ((IObject)(IDeviceObject)val).MetaObject.ObjectGuid };
					}
					return new Guid[0];
				}
				return _device.ObjectsToUpdate;
			}
		}

		public string UserManagement
		{
			get
			{
				if (_device == null)
				{
					return null;
				}
				return ((IDeviceObject6)_device).UserManagement;
			}
			set
			{
				if (_device != null)
				{
					((IDeviceObject6)_device).UserManagement=(value);
				}
			}
		}

		public string RightsManagement
		{
			get
			{
				if (_device == null)
				{
					return null;
				}
				return ((IDeviceObject6)_device).RightsManagement;
			}
			set
			{
				if (_device != null)
				{
					((IDeviceObject6)_device).RightsManagement=(value);
				}
			}
		}

		public bool ShowParamsInDevDescOrder => _bShowParamsInDevDescOrder;

		public bool NeedsContextForLanguageModelProvision => false;

		public bool IsPhysical => !(_device is LogicalIODevice);

		public bool IsLogical => _device is LogicalIODevice;

		public IMappedDeviceList MappedDevices
		{
			get
			{
				if (_device != null)
				{
					return _device.MappedDevices;
				}
				return (IMappedDeviceList)(object)new LogicalDeviceList();
			}
		}

		public long LanguageModelPositionId
		{
			get
			{
				if (_device != null)
				{
					return _device.LanguageModelPositionId;
				}
				return -1L;
			}
		}

		public bool MappingPossible
		{
			get
			{
				if (_device != null)
				{
					return _device.MappingPossible;
				}
				return false;
			}
			set
			{
				if (_device != null)
				{
					_device.MappingPossible = value;
				}
			}
		}

		public bool ShowPropertiesDialog => ExcludeFromBuildEnabled;

		public bool ExcludeFromBuildEnabled
		{
			get
			{
				if (AllowsDirectCommunication)
				{
					return false;
				}
				return true;
			}
		}

		public bool ExternalEnabled => false;

		public bool EnableSystemCallEnabled => false;

		public bool LinkAlwaysEnabled => false;

		public bool CompilerDefinesEnabled => false;

		public bool CreateBitChannels
		{
			get
			{
				if (_device != null)
				{
					return _device.CreateBitChannels;
				}
				return false;
			}
			set
			{
				if (_device != null)
				{
					_device.CreateBitChannels = value;
				}
			}
		}

		public string OnlineHelpUrl
		{
			get
			{
				if (_device != null)
				{
					return _device.OnlineHelpUrl;
				}
				return null;
			}
		}

		public InteractiveLoginMode LoginMode => (InteractiveLoginMode)0;

		public ITypeList TypeDefinitions
		{
			get
			{
				if (_device != null)
				{
					return _device.TypeDefinitions;
				}
				return null;
			}
		}

		public bool UseDeviceApplicationStructure
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new InvalidOperationException(Strings.ErrorDoesNotSupportDeviceApplicationStructure);
				}
			}
		}

		public bool AllowSymbolicVarAccessInSyncWithIecCycle
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException("Slot devices do not allow symbolic var access in sync with the IEC cycle...");
			}
		}

		public SlotDeviceObject()
		{
		}

		internal SlotDeviceObject(string stInterfaceType, ArrayList alAdditionalInterfaces, bool bAllowEmpty)
		{
			string text = string.Empty;
			StringWriter stringWriter;
			foreach (string alAdditionalInterface in alAdditionalInterfaces)
			{
				stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteString(alAdditionalInterface);
				xmlTextWriter.Close();
				text += string.Format("<AdditionalInterface interface={0}/>\n\r", "\"" + stringWriter.ToString() + "\"");
			}
			XmlDocument xmlDocument = new XmlDocument();
			stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter2 = new XmlTextWriter(stringWriter);
			xmlTextWriter2.WriteString(stInterfaceType);
			xmlTextWriter2.Close();
			xmlDocument.LoadXml($"    <Connector connectorId=\"1\" explicit=\"false\" hostpath=\"-1\" interface=\"{stringWriter.ToString()}\" moduleType=\"0\" role=\"child\">\r\n \t\t<Slot allowEmpty=\"false\" count=\"1\"/>\r\n        {text}\r\n\t\t</Connector>\r\n");
			Connector connector = new Connector(xmlDocument.DocumentElement, new TypeList(), (IDeviceIdentification)(object)_identification, bCreateBitChannels: false);
			_connectors.Add(connector);
			connector.Device = this;
			_bAllowEmpty = bAllowEmpty;
		}

		internal SlotDeviceObject(SlotDeviceObject original)
		{
			if (original._device != null)
			{
				_device = (DeviceObject)((GenericObject)original._device).Clone();
			}
			else
			{
				_device = null;
			}
			_connectors = (ConnectorList)((GenericObject)original._connectors).Clone();
			_bAllowEmpty = original._bAllowEmpty;
			_bIsInUpdate = original._bIsInUpdate;
			_bShowParamsInDevDescOrder = original._bShowParamsInDevDescOrder;
			_nConnectorIDForChild = original._nConnectorIDForChild;
		}

		public override object Clone()
		{
			SlotDeviceObject slotDeviceObject = new SlotDeviceObject(this);
			((GenericObject)slotDeviceObject).AfterClone();
			return slotDeviceObject;
		}

		public void InsertDevice(int nInsertPosition, IDeviceObject device)
		{
		}

		public string[] GetPossibleInterfacesForInsert(int nInsertPosition)
		{
			if (_device == null)
			{
				return new string[0];
			}
			return _device.GetPossibleInterfacesForInsert(nInsertPosition);
		}

		public IIoModuleIterator CreateModuleIterator()
		{
			return (IIoModuleIterator)(object)new IoModuleIterator(new IoModuleDeviceReference(_guidObject, _nProjectHandle));
		}

		public int CheckRelationships(IObject parentObject, IObject[] childObjects)
		{
			if (_device == null)
			{
				if (childObjects.Length == 0)
				{
					return 0;
				}
				return 1;
			}
			return _device.CheckRelationships(parentObject, childObjects);
		}

		public bool AcceptsChildObject(Type childObjectType)
		{
			if (_device == null)
			{
				return false;
			}
			return _device.AcceptsChildObject(childObjectType);
		}

		public void HandleRenamed()
		{
			if (_device != null)
			{
				_device.HandleRenamed();
			}
		}

		public string GetPositionText(long nPosition)
		{
			if (_device == null)
			{
				return null;
			}
			return _device.GetPositionText(nPosition);
		}

		public bool CheckName(string stName)
		{
			if (_device == null)
			{
				return true;
			}
			if (stName.StartsWith("<") && stName.EndsWith(">"))
			{
				return true;
			}
			return _device.CheckName(stName);
		}

		public string GetContentString(ref long nPosition, ref int nLength, bool bWord)
		{
			if (_device == null)
			{
				return null;
			}
			return _device.GetContentString(ref nPosition, ref nLength, bWord);
		}

		public bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject == null)
			{
				return false;
			}
			if (!(parentObject is IDeviceObject) && !(parentObject is IConnector))
			{
				return false;
			}
			return true;
		}

		public XmlNode GetFunctionalSection()
		{
			if (_device != null)
			{
				return _device.GetFunctionalSection();
			}
			return null;
		}

		public void SetParent(Guid guidParent, int nLocalConnectorId)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, guidParent);
			if (_device != null)
			{
				_device.SetParent(guidParent, nLocalConnectorId);
			}
			((IAdapterBase)((Connector)(object)_connectors[0]).Adapters[0]).AddModule(0, metaObjectStub);
		}

		public bool CheckLanguageModelGuids()
		{
			if (_device == null)
			{
				return true;
			}
			return _device.CheckLanguageModelGuids();
		}

		public IMetaObject GetMetaObject()
		{
			if (_metaObject != null)
			{
				return _metaObject;
			}
			return ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidObject);
		}

		public void UpdateLanguageModelGuids(bool bUpgrade)
		{
			if (_device != null)
			{
				_device.UpdateLanguageModelGuids(bUpgrade);
			}
		}

		public void OnAfterAdded()
		{
			if (_device != null)
			{
				_device.OnAfterAdded();
			}
		}

		public void OnAfterCreated()
		{
			if (_device != null)
			{
				_device.OnAfterCreated();
			}
		}

		public void AddLateLanguageModel(int nProjectHandle, AddLanguageModelEventArgs e)
		{
			if (_device != null)
			{
				_device.AddLateLanguageModel(nProjectHandle, e);
			}
		}

		public void PreparePaste(bool bOnlyChildConnectors = false)
		{
			if (_device != null)
			{
				_device.PreparePaste();
			}
			((Connector)(object)_connectors[0]).PreparePaste();
		}

		public void UpdatePasteModuleGuid(Guid oldGuid, Guid newGuid)
		{
			if (_device != null)
			{
				_device.UpdatePasteModuleGuid(oldGuid, newGuid);
			}
			((Connector)(object)_connectors[0]).UpdatePasteModuleGuid(oldGuid, newGuid);
		}

		public override void AfterClone()
		{
			base.AfterClone();
			_slotDeviceInfo.DeviceObject = (IDeviceObject2)(object)_device;
			((Connector)(object)_connectors[0]).Device = this;
		}

		public override void AfterDeserialize()
		{
			base.AfterDeserialize();
			_slotDeviceInfo.DeviceObject = (IDeviceObject2)(object)_device;
			((Connector)(object)_connectors[0]).Device = this;
		}

		public void OnRenamed(string stOldDeviceName)
		{
			if (_device != null)
			{
				_device.OnRenamed(stOldDeviceName);
			}
		}

		public string[] GetPossibleInterfacesForPlug()
		{
			IConnector obj = _connectors[0];
			IConnector7 val = (IConnector7)(object)((obj is IConnector7) ? obj : null);
			if (val == null)
			{
				return new string[1] { _connectors[0].Interface };
			}
			IIoProvider parent = ((IIoProvider)((val is IIoProvider) ? val : null)).Parent;
			IConnector7 val2 = (IConnector7)(object)((parent is IConnector7) ? parent : null);
			if (val2 != null)
			{
				val = val2;
			}
			ArrayList arrayList = new ArrayList();
			if (val.AdditionalInterfaces.Count > 0)
			{
				arrayList.AddRange(val.AdditionalInterfaces);
				string[] array = new string[arrayList.Count + 1];
				array[0] = _connectors[0].Interface;
				arrayList.CopyTo(array, 1);
				return array;
			}
			return new string[1] { ((IConnector)val).Interface };
		}

		internal DeviceObject GetDevice()
		{
			return _device;
		}

		public IDeviceObject GetDeviceObject()
		{
			return (IDeviceObject)(object)_device;
		}

		internal IDeviceObject GetOwnerDevice()
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
			IObject @object = objectToRead.Object;
			IExplicitConnector val = (IExplicitConnector)(object)((@object is IExplicitConnector) ? @object : null);
			if (val != null)
			{
				return ((IConnector)val).GetDeviceObject();
			}
			IObject object2 = objectToRead.Object;
			return (IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null);
		}

		internal void AfterPlugDevice()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Invalid comparison between Unknown and I4
			if (_device == null)
			{
				Debug.Fail("No device plugged into slot.");
				throw new InvalidOperationException("No device plugged into slot.");
			}
			DeviceObject.UpdateChildObjects(_device, bUpdate: false, bVersionUpgrade: false);
			foreach (IConnector item in (IEnumerable)_device.Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole == 1)
				{
					IIoProvider parent = ((IIoProvider)((val is IIoProvider) ? val : null)).Parent;
					if (parent != null && parent is Connector && _device.MetaObject != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_device.ProjectHandle, _device.MetaObject.ObjectGuid))
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_device.ProjectHandle, _device.MetaObject.ObjectGuid);
						(parent as Connector).createHostChildParameters(metaObjectStub);
					}
				}
			}
		}

		internal void PlugDevice(DeviceObject device)
		{
			PlugDevice(device, bCheckForConstraints: false);
		}

		internal void PlugDevice(DeviceObject device, bool bCheckForConstraints)
		{
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Expected O, but got Unknown
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Invalid comparison between Unknown and I4
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Invalid comparison between Unknown and I4
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Expected O, but got Unknown
			if (!bCheckForConstraints)
			{
				if (_device != null)
				{
					throw new InvalidOperationException("Unplug the existing device before plugging in a new device");
				}
				_device = device;
				_slotDeviceInfo.DeviceObject = (IDeviceObject2)(object)_device;
				_bShowParamsInDevDescOrder = _device.ShowParamsInDevDescOrder;
			}
			bool flag = false;
			IMetaObject val = null;
			try
			{
				if (_metaObject != null)
				{
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
					if (metaObjectStub != null && typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
					}
					if (metaObjectStub != null && typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, _metaObject.ParentObjectGuid);
					}
				}
			}
			catch
			{
			}
			if (val != null && val.Object is ExplicitConnector)
			{
				ExplicitConnector explicitConnector = val.Object as ExplicitConnector;
				if (explicitConnector.Contraints.Count > 0)
				{
					explicitConnector.CheckConstraints((IDeviceObjectBase)device);
				}
			}
			if (val != null && val.Object is DeviceObject)
			{
				foreach (Connector item in (IEnumerable)(val.Object as DeviceObject).Connectors)
				{
					if (item.Contraints.Count <= 0)
					{
						continue;
					}
					if ((int)item.ConnectorRole == 0 && item.Interface == _connectors[0].Interface)
					{
						foreach (IAdapter item2 in (IEnumerable)item.Adapters)
						{
							if (item2 is SlotAdapter)
							{
								item.CheckConstraints((IDeviceObjectBase)device);
							}
						}
					}
					if ((int)item.ConnectorRole == 1)
					{
						item.CheckConstraints((IDeviceObjectBase)device);
					}
				}
			}
			if (bCheckForConstraints)
			{
				return;
			}
			foreach (Connector item3 in (IEnumerable)device.Connectors)
			{
				if ((int)item3.ConnectorRole != 1 || !DeviceManager.CheckMatchInterface((IConnector7)(object)item3, this._connectors[0] as IConnector7))
				{
					continue;
				}
				foreach (IAdapter item4 in (IEnumerable)item3.Adapters)
				{
					SlotAdapter slotAdapter = item4 as SlotAdapter;
					if (slotAdapter != null)
					{
						flag = true;
						if (_metaObject != null)
						{
							slotAdapter.SetDevice(_metaObject.ParentObjectGuid, 0);
							_metaObject.RemoveProperty(DeviceProperty.My_Guid);
							IDeviceProperty val2 = (IDeviceProperty)(object)new DeviceProperty(((IDeviceObject5)device).DeviceIdentificationNoSimulation);
							_metaObject.AddProperty((IObjectProperty)(object)val2);
						}
						break;
					}
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException("Device cannot be plugged into this slot.");
			}
			if (_metaObject != null)
			{
				_device.UpdateDependentObjects(bAfterDeserialized: true);
				_device.MetaObject = _metaObject;
				_device.OnAfterAdded();
			}
		}

		internal void UnplugDevice()
		{
			_device = null;
			if (_metaObject != null)
			{
				_metaObject.RemoveProperty(DeviceProperty.My_Guid);
				IDeviceProperty val = (IDeviceProperty)(object)new DeviceProperty((IDeviceIdentification)(object)_identification);
				_metaObject.AddProperty((IObjectProperty)(object)val);
				((ILanguageModelManager)APEnvironment.LanguageModelMgr).RemoveLanguageModelOfObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
			}
		}

		internal void BeforeUnplugDevice()
		{
			Debug.Assert(_metaObject != null);
			Debug.Assert(!_metaObject.IsToModify);
			int projectHandle = _metaObject.ProjectHandle;
			Guid[] subObjectGuids = _metaObject.SubObjectGuids;
			Guid[] array = subObjectGuids;
			foreach (Guid guid in array)
			{
				IMetaObject val = null;
				try
				{
					val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(projectHandle, guid);
				}
				finally
				{
					if (val != null && val.IsToModify)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
					}
				}
			}
			array = subObjectGuids;
			foreach (Guid guid2 in array)
			{
				((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(projectHandle, guid2);
			}
		}

		public string GetLanguageModel()
		{
			if (_device == null)
			{
				return null;
			}
			return _device.GetLanguageModel();
		}

		bool IOrderedSubObjects.AcceptsChildObject(Type childObjectType, int nIndex)
		{
			if (_device == null)
			{
				return false;
			}
			return _device.AcceptsChildObject(childObjectType, nIndex);
		}

		public void AddChild(Guid subObjectGuid, int nIndex)
		{
			if (_device == null)
			{
				throw new InvalidOperationException("Cannot add a child to an empty slot");
			}
			_device.AddChild(subObjectGuid, nIndex);
		}

		public int GetChildIndex(Guid subObjectGuid)
		{
			if (_device == null)
			{
				return -1;
			}
			return _device.GetChildIndex(subObjectGuid);
		}

		public void RemoveChild(IMetaObject moRemoved)
		{
			if (_device != null)
			{
				_device.RemoveChild(moRemoved);
			}
		}

		public List<IIOTaskUsage> GetTaskMappings(Guid appGuid)
		{
			if (_device != null)
			{
				IDeviceObject hostDeviceObject = _device.GetHostDeviceObject();
				return ((IDeviceObject10)((hostDeviceObject is IDeviceObject10) ? hostDeviceObject : null)).GetTaskMappings(appGuid);
			}
			return null;
		}

		public ILanguageModel GetStructuredLanguageModel(ILanguageModelBuilder lmbuilder)
		{
			List<List<string>> codeTables = null;
			ILanguageModel val = lmbuilder.CreateLanguageModel(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty);
			string text = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0)) ? GetLanguageModel2(out codeTables) : GetLanguageModel2(lmbuilder, val, out codeTables));
			ILanguageModel val2 = lmbuilder.CreateLanguageModelOfXml(text, codeTables);
			if (val2 != null)
			{
				ILMPOU[] pous = val.Pous;
				foreach (ILMPOU val3 in pous)
				{
					val2.AddPou(val3);
				}
				ILMGlobVarlist[] globalVariableLists = val.GlobalVariableLists;
				foreach (ILMGlobVarlist val4 in globalVariableLists)
				{
					val2.AddGlobalVariableList(val4);
				}
				ILMDataType[] dataTypes = val.DataTypes;
				foreach (ILMDataType val5 in dataTypes)
				{
					val2.AddDataType(val5);
				}
			}
			return val2;
		}

		public string GetLanguageModel2(out List<List<string>> codeTables)
		{
			if (_device == null)
			{
				codeTables = null;
				return null;
			}
			return _device.GetLanguageModel2(out codeTables);
		}

		private string GetLanguageModel2(ILanguageModelBuilder lmb, ILanguageModel lmnew, out List<List<string>> codeTables)
		{
			if (_device == null)
			{
				codeTables = null;
				return null;
			}
			return _device.GetLanguageModel2(lmb, lmnew, out codeTables);
		}

		public void UpdateScanInformation(IScanInformation scaninfo)
		{
			throw new InvalidOperationException("Slot devices do not support storing of scan information...");
		}

		public void SetLoginMode(InteractiveLoginMode mode)
		{
			throw new InvalidOperationException("Slot devices do not support interactive login mode...");
		}

		public string GetOnlineHelpKeyword()
		{
			return null;
		}

		public string GetOnlineHelpUrl()
		{
			return OnlineHelpUrl;
		}

		public int GetEnvisionedIndexOf(int nProjectHandle, Guid objectGuid)
		{
			return GetChildIndex(objectGuid);
		}

		public Guid GetBusTask(out long lCycleTime)
		{
			if (_device != null)
			{
				return _device.GetBusTask(out lCycleTime);
			}
			lCycleTime = 0L;
			return Guid.Empty;
		}
	}
}
