#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Compression.Checksums;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.Simulation;
using _3S.CoDeSys.TaskConfig;
using _3S.CoDeSys.TraceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{225bfe47-7336-4dbc-9419-4105a7c831fa}")]
	[StorageVersion("3.3.0.0")]
	public class DeviceObject : DeviceObjectBase, IDeviceObjectBase, IDeviceObject16, IDeviceObject15, IDeviceObject14, IDeviceObject13, IDeviceObject12, IDeviceObject11, IDeviceObject10, IDeviceObject9, IDeviceObject8, IDeviceObject7, IDeviceObject6, IDeviceObject5, IDeviceObject4, IDeviceObject3, IDeviceObject2, IDeviceObject, IObject, IGenericObject, IArchivable, ICloneable, IComparable, ILanguageModelProvider3, ILanguageModelProvider2, ILanguageModelProvider, ILanguageModelProviderWithDependencies, IOrderedSubObjects, IHasAssociatedOnlineHelpTopic, IKnowMyOrderedSubObjectsInAdvance, IIoProvider2, IIoProvider, IGenericInterfaceExtensionProvider, ILogicalDevice3, ILogicalDevice2, ILogicalDevice, ILanguageModelProviderBuildPropertiesControl, IStructuredLanguageModelProvider, IDeviceObjectPlaceholderResolver
	{
		[DefaultSerialization("UniqueIdGenerator")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected string UniqueIdGeneratorString
		{
			get
			{
				return this._idGenerator.StoreToString();
			}
			set
			{
				this._idGenerator.RestoreFromString(value);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x00024438 File Offset: 0x00023438
		// (set) Token: 0x060005E6 RID: 1510 RVA: 0x000244F8 File Offset: 0x000234F8
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("TypeList")]
		[StorageVersion("3.5.6.0")]
		[StorageDefaultValue(null)]
		protected TypeList TypeList
		{
			get
			{
				if (this._typeList != null && this._typeList.TypeMap != null)
				{
					TypeList typeList = new TypeList();
					typeList.TypeMap = new LDictionary<string, ITypeDefinition>();
					foreach (KeyValuePair<string, ITypeDefinition> keyValuePair in this._typeList.TypeMap)
					{
						if (keyValuePair.Value.CreatedType)
						{
							typeList.TypeMap[keyValuePair.Key] = keyValuePair.Value;
						}
					}
					if (typeList.TypeMap != null && typeList.TypeMap.Count > 0)
					{
						return typeList;
					}
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.LoadTypes();
					if (this._typeList.TypeMap == null)
					{
						this._typeList.TypeMap = new LDictionary<string, ITypeDefinition>();
					}
					foreach (KeyValuePair<string, ITypeDefinition> keyValuePair in value.TypeMap)
					{
						(keyValuePair.Value as TypeDefinition).CreatedType = true;
						this._typeList.TypeMap[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x000245A0 File Offset: 0x000235A0
		// (set) Token: 0x060005E8 RID: 1512 RVA: 0x000245A8 File Offset: 0x000235A8
		internal int ConnectorIDForChild
		{
			get
			{
				return this._nConnectorIDForChild;
			}
			set
			{
				this._nConnectorIDForChild = value;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x000245B1 File Offset: 0x000235B1
		internal Guid LmIoConfigGlobals
		{
			get
			{
				return this._guidLmIoConfigGlobals;
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000245BC File Offset: 0x000235BC
		static DeviceObject()
		{
			DeviceObjectHelper.CheckInstance();
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00024630 File Offset: 0x00023630
		public DeviceObject()
		{
			this._baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x00024765 File Offset: 0x00023765
		internal LDictionary<string, LDictionary<IDataElement, Guid>> GlobalDataTypes
		{
			get
			{
				return this._dictGlobalDataTypes;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x0002476D File Offset: 0x0002376D
		// (set) Token: 0x060005EE RID: 1518 RVA: 0x0002477A File Offset: 0x0002377A
		public bool SimulationMode
		{
			get
			{
				return this._communicationSettings.SimulationMode;
			}
			set
			{
				this._communicationSettings.SimulationMode = value;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x00024788 File Offset: 0x00023788
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x00024790 File Offset: 0x00023790
		public bool CreateBitChannels
		{
			get
			{
				return this._bCreateBitChannels;
			}
			set
			{
				if (this._connectors != null)
				{
					foreach (object obj in this._connectors)
					{
						((Connector)obj).CreateBitChannels = value;
					}
					this._bCreateBitChannels = value;
				}
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x000247F8 File Offset: 0x000237F8
		public string OnlineHelpUrl
		{
			get
			{
				if (!string.IsNullOrEmpty(this._stOnlineHelpUrl))
				{
					return this._stOnlineHelpUrl;
				}
				return null;
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x00024810 File Offset: 0x00023810
		public InteractiveLoginMode LoginMode
		{
			get
			{
				InteractiveLoginMode result;
				if (this._nInteractiveLoginMode == -1)
				{
					result = DeviceObjectHelper.ReadDefaultLoginModeFromTargetSettings(this.DeviceIdentification);
				}
				else
				{
					result = (InteractiveLoginMode)this._nInteractiveLoginMode;
				}
				return result;
			}
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0002483E File Offset: 0x0002383E
		public void SetLoginMode(InteractiveLoginMode mode)
		{
			if (!this._metaObject.IsToModify)
			{
				throw new InvalidOperationException("Need write access to set the login mode!");
			}
			this._nInteractiveLoginMode = (int)mode;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x00024860 File Offset: 0x00023860
		public void UpdateScanInformation(IScanInformation scaninfo)
		{
			if (!this._metaObject.IsToModify)
			{
				throw new InvalidOperationException("Need write access to create or update a scan information!");
			}
			if (this._communicationSettings != null)
			{
				ICommunicationSettings communicationSettings = this.CommunicationSettings;
				if (scaninfo == null)
				{
					this._communicationSettings.ScanInformation = null;
					return;
				}
				if (this._communicationSettings.ScanInformation == null)
				{
					this._communicationSettings.ScanInformation = new StorableScanInformation();
				}
				if (this._communicationSettings.ScanInformation != null)
				{
					this._communicationSettings.ScanInformation.DeviceName = scaninfo.DeviceName;
					this._communicationSettings.ScanInformation.IPAddressAndPort = scaninfo.IPAddressAndPort;
					this._communicationSettings.ScanInformation.TargetID = scaninfo.TargetID;
					this._communicationSettings.ScanInformation.TargetName = scaninfo.TargetName;
					this._communicationSettings.ScanInformation.TargetType = scaninfo.TargetType;
					this._communicationSettings.ScanInformation.TargetVendor = scaninfo.TargetVendor;
					this._communicationSettings.ScanInformation.TargetVersion = scaninfo.TargetVersion;
				}
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x0002496E File Offset: 0x0002396E
		public LList<LanguageStringRef> AdditionalStringTable
		{
			get
			{
				if (this._liAdditionalStringTable == null)
				{
					this._liAdditionalStringTable = new LList<LanguageStringRef>();
				}
				return this._liAdditionalStringTable;
			}
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0002498C File Offset: 0x0002398C
		internal DeviceObject(bool bCreateBitChannels, IDeviceIdentification deviceId)
		{
			this.InitDeviceObject(bCreateBitChannels, deviceId);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00024AC0 File Offset: 0x00023AC0
		internal void InitDeviceObject(bool bCreateBitChannels, IDeviceIdentification deviceId)
		{
			this._bCreateBitChannels = bCreateBitChannels;
			LList<DeviceIdUpdate> devicesToUpdate = new LList<DeviceIdUpdate>();
			this.SetDeviceIdentification(deviceId, false, devicesToUpdate);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00024AE4 File Offset: 0x00023AE4
		internal DeviceObject(IDeviceIdentification deviceId, bool noParameters)
		{
			LList<DeviceIdUpdate> devicesToUpdate = new LList<DeviceIdUpdate>();
			if (noParameters)
			{
				bool flag;
				this.SetDeviceIdentification2(deviceId, false, devicesToUpdate, out flag);
				return;
			}
			this.SetDeviceIdentification(deviceId, false, devicesToUpdate);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00024C2C File Offset: 0x00023C2C
		protected DeviceObject(DeviceObject original)
		{
			this.UniqueIdGeneratorString = original.UniqueIdGeneratorString;
			this._deviceId = (DeviceIdentification)original._deviceId.Clone();
			this._defaultDeviceInfo = (DefaultDeviceInfo)original._defaultDeviceInfo.Clone();
			this._deviceParameterSet = (ParameterSet)original._deviceParameterSet.Clone();
			this._bDisable = original._bDisable;
			this._bExclude = original._bExclude;
			this._alOldConnectors = new ArrayList(original._alOldConnectors.Count);
			foreach (object obj in original._alOldConnectors)
			{
				Connector connector = (Connector)obj;
				this._alOldConnectors.Add(connector.Clone());
			}
			this._connectors = (ConnectorList)original._connectors.Clone();
			this._attributes = (DeviceAttributes)original._attributes.Clone();
			this._communicationSettings = (CommunicationSettings)original._communicationSettings.Clone();
			this._ioProviderBase = (IoProviderBase)original._ioProviderBase.Clone();
			this._guidBusCycleTask = original._guidBusCycleTask;
			this._driverInfo = (DriverInfo)original._driverInfo.Clone();
			this._customItems = (CustomItemList)original._customItems.Clone();
			this._guidLmIoConfigGlobals = original._guidLmIoConfigGlobals;
			this._guidLmIoConfigVarConfig = original._guidLmIoConfigVarConfig;
			this._guidLmIoConfigErrorPou = original._guidLmIoConfigErrorPou;
			this._guidLmIoConfigGlobalsMapping = original._guidLmIoConfigGlobalsMapping;
			this._nModuleId = original._nModuleId;
			this._stIoUpdateTask = original._stIoUpdateTask;
			this._funcChildrenTypeGuids = new Guid[original._funcChildrenTypeGuids.Length];
			original._funcChildrenTypeGuids.CopyTo(this._funcChildrenTypeGuids, 0);
			this._bIsInUpdate = original._bIsInUpdate;
			this._bShowParamsInDevDescOrder = original._bShowParamsInDevDescOrder;
			this._stFixedInputAddress = original._stFixedInputAddress;
			this._stFixedOutputAddress = original._stFixedOutputAddress;
			this._bDownloadParamsDevDescOrder = original._bDownloadParamsDevDescOrder;
			this._nConnectorIDForChild = original._nConnectorIDForChild;
			this._logicalDevices = (LogicalDeviceList)original._logicalDevices.Clone();
			this._lLogicalLanguageModelPositionId = original._lLogicalLanguageModelPositionId;
			this._arSupportedLogicalBusSystems = (ArrayList)original._arSupportedLogicalBusSystems.Clone();
			this._bCreateBitChannels = original._bCreateBitChannels;
			this._stRightsManagement = original._stRightsManagement;
			this._stUserManagement = original._stUserManagement;
			this._bMappingPossible = original._bMappingPossible;
			this._stOnlineHelpUrl = original._stOnlineHelpUrl;
			this._nInteractiveLoginMode = original._nInteractiveLoginMode;
			if (original._liAdditionalStringTable != null)
			{
				this._liAdditionalStringTable = new LList<LanguageStringRef>();
				foreach (LanguageStringRef languageStringRef in original._liAdditionalStringTable)
				{
					this._liAdditionalStringTable.Add(languageStringRef.Clone() as LanguageStringRef);
				}
			}
			if (original._typeList != null && original._typeList.Types != null && original._typeList.Types.Count > 0)
			{
				this._typeList = original._typeList;
			}
			if (original._dictPlaceholderResolutions != null && original._dictPlaceholderResolutions.Count > 0)
			{
				this._dictPlaceholderResolutions = original._dictPlaceholderResolutions;
			}
			this._dictGlobalDataTypes = original._dictGlobalDataTypes;
			this._bUseDeviceApplicationStructure = original._bUseDeviceApplicationStructure;
			this._bAllowSymbolicVarAccessInSyncWithIecCycle = original._bAllowSymbolicVarAccessInSyncWithIecCycle;
			this._bHidePropertiesDialog = original._bHidePropertiesDialog;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x000250D4 File Offset: 0x000240D4
		public override object Clone()
		{
			DeviceObject deviceObject = new DeviceObject(this);
			deviceObject.AfterClone();
			return deviceObject;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x000250E4 File Offset: 0x000240E4
		internal static void UpdateChildObjects(IDeviceObjectBase device, bool bUpdate, bool bVersionUpgrade, LList<DeviceIdUpdate> liDevicesToUpdate = null)
		{
			if (device.MetaObject.IsToModify)
			{
				throw new InvalidOperationException("This operation may not be executed on a modifyable copy of a device object!");
			}
			int projectHandle = device.MetaObject.ProjectHandle;
			Guid objectGuid = device.MetaObject.ObjectGuid;
			bool flag;
			DeviceObject.CreateImplicitObjects(projectHandle, objectGuid, out flag, bUpdate);
			if (flag)
			{
				IUndoManager undoManager = APEnvironment.ObjectMgr.GetUndoManager(projectHandle);
				OnPlcCreatedAction onPlcCreatedAction = new OnPlcCreatedAction(projectHandle, objectGuid);
				undoManager.AddAction(onPlcCreatedAction);
				onPlcCreatedAction.Redo();
			}
			int num;
			if (device is DeviceObject)
			{
				DeviceObject.CreateTasks((DeviceObject)device);
				num = ((DeviceObject)device).GetNumberOfFunctionalChildren();
			}
			else if (device is SlotDeviceObject)
			{
				SlotDeviceObject slotDeviceObject = (SlotDeviceObject)device;
				if (slotDeviceObject.HasDevice)
				{
					DeviceObject.CreateTasks(slotDeviceObject.GetDevice());
					num = slotDeviceObject.GetDevice().GetNumberOfFunctionalChildren();
				}
				else
				{
					num = 0;
				}
			}
			else
			{
				num = 0;
			}
			foreach (object obj in device.Connectors)
			{
				Connector connector = (Connector)obj;
				if (connector.ConnectorRole == ConnectorRole.Parent)
				{
					bool flag2 = false;
					IIoProvider ioProviderParent = connector.GetIoProviderParent();
					if (ioProviderParent is Connector)
					{
						foreach (object obj2 in (ioProviderParent as Connector).Adapters)
						{
							IAdapter adapter = (IAdapter)obj2;
							if (adapter is SlotAdapter && adapter.ModulesCount == 1 && adapter.Modules[0] == Guid.Empty)
							{
								flag2 = true;
								break;
							}
						}
					}
					if (flag2)
					{
						continue;
					}
				}
				DeviceObject.CreateTasks(connector);
				if (connector.ConnectorRole == ConnectorRole.Parent)
				{
					if (device is LogicalIODevice && (device as LogicalIODevice).IsLogical)
					{
						break;
					}
					IIoProvider ioProvider = connector;
					if (ioProvider != null)
					{
						IIoProvider[] children = ioProvider.Children;
						for (int i = 0; i < children.Length; i++)
						{
							IMetaObject metaObject = children[i].GetMetaObject();
							if (metaObject != null)
							{
								IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObject.ProjectHandle, metaObject.ObjectGuid);
								connector.createHostChildParameters(metaObjectStub);
							}
						}
					}
					try
					{
						connector.UpdateChildObjects(projectHandle, objectGuid, ref num, bUpdate, device.CreateBitChannels, bVersionUpgrade, liDevicesToUpdate);
					}
					catch (Exception ex)
					{
						APEnvironment.MessageService.Error(ex.Message, "Exception", Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00025394 File Offset: 0x00024394
		internal void SetDeviceIdentification(IDeviceIdentification deviceId, bool bUpdate, LList<DeviceIdUpdate> devicesToUpdate)
		{
			bool flag;
			this.SetDeviceIdentification(deviceId, bUpdate, devicesToUpdate, out flag, out flag);
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x000253B0 File Offset: 0x000243B0
		internal void SetDeviceIdentification(IDeviceIdentification deviceId, bool bUpdate, LList<DeviceIdUpdate> devicesToUpdate, out bool bMustBeRemoved, out bool bVersionUpgrade)
		{
			bMustBeRemoved = false;
			if (this._deviceId == null && bUpdate)
			{
				throw new InvalidOperationException("Cannot update an uninitialized device object");
			}
			if (this._deviceId != null && !bUpdate)
			{
				throw new InvalidOperationException("Cannot change the device identification of a device. Use Update() instead");
			}
			if (this._deviceId != null)
			{
				bVersionUpgrade = this._deviceId.Equals(deviceId, true, true);
			}
			else
			{
				bVersionUpgrade = false;
			}
			if (!bVersionUpgrade && bUpdate)
			{
				DeviceObjectHelper.CollectTasksToRemove(this, this._metaObject);
				foreach (object obj in this._connectors)
				{
					DeviceObjectHelper.CollectTasksToRemove(((IConnector)obj) as IIoProvider, this._metaObject);
				}
				DeviceObjectHelper.DeleteUnusedTasks(PSChangeAction.Remove);
			}
			TypeList typeList = new TypeList();
			if (deviceId is IModuleIdentification)
			{
				this._deviceId = new ModuleIdentification((IModuleIdentification)deviceId);
			}
			else
			{
				this._deviceId = new DeviceIdentification(deviceId);
			}
			if (bUpdate && this._metaObject != null && this._metaObject.Object is IDeviceObjectBase)
			{
				this._metaObject.AddProperty(new DeviceProperty(this._deviceId));
			}
			IDeviceDescription device = APEnvironment.DeviceRepository.GetDevice(this._deviceId);
			if (device != null)
			{
				this._deviceInfo = device.DeviceInfo;
				this._defaultDeviceInfo = new DefaultDeviceInfo(this._deviceInfo);
			}
			XmlNode xmlGlobalNode = APEnvironment.DeviceRepository.GetXmlGlobalNode(this._deviceId, "Types");
			if (xmlGlobalNode != null)
			{
				XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlGlobalNode);
				xmlNodeReader.Read();
				typeList.ReadTypes(xmlNodeReader, this._deviceId);
			}
			XmlNode xmlDeviceNode = APEnvironment.DeviceRepository.GetXmlDeviceNode(this._deviceId);
			if (xmlDeviceNode == null && this._deviceId is IModuleIdentification)
			{
				bMustBeRemoved = true;
				return;
			}
			if (xmlDeviceNode == null)
			{
				throw new DeviceNotFoundException(this._deviceId);
			}
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this._deviceId);
			if (targetSettingsById != null)
			{
				this._bCreateBitChannels = targetSettingsById.GetBoolValue(LocalTargetSettings.CreateBitChannels.Path, this._bCreateBitChannels);
				this._communicationSettings.MonitoringIntervalMsec = LocalTargetSettings.MonitoringInterval.GetIntValue(targetSettingsById);
				if (LocalTargetSettings.PreselectEncryptedCommunication.GetBoolValue(targetSettingsById))
				{
					this._communicationSettings.IsCommunicationEncrypted = true;
				}
			}
			this.ReadDevice(xmlDeviceNode, typeList, bUpdate, bVersionUpgrade, devicesToUpdate, this._deviceId, this._bCreateBitChannels);
			this._typeList = typeList;
			if (this._deviceParameterSet != null)
			{
				this._deviceParameterSet.SetIoProvider(this);
			}
			this._driverInfo.SetIoProvider(this);
			this.SetPositionIds();
			this.UpdateDependentObjects(true);
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0002563C File Offset: 0x0002463C
		internal static DeviceObject Create(IDeviceIdentification deviceId, bool createBitChannels, XmlDocument doc, TypeList additionalTypes)
		{
			DeviceObject deviceObject = new DeviceObject();
			deviceObject.CreateBitChannels = createBitChannels;
			deviceObject.SetDeviceIdentification3(deviceId, doc, additionalTypes);
			return deviceObject;
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00025654 File Offset: 0x00024654
		internal void SetDeviceIdentification3(IDeviceIdentification deviceId, XmlDocument doc, TypeList additionalTypes)
		{
			bool bVersionUpgrade = this._deviceId != null && this._deviceId.Equals(deviceId, true);
			TypeList typeList = new TypeList();
			if (deviceId is IModuleIdentification)
			{
				this._deviceId = new ModuleIdentification((IModuleIdentification)deviceId);
			}
			else
			{
				this._deviceId = new DeviceIdentification(deviceId);
			}
			IDeviceDescription device = APEnvironment.DeviceRepository.GetDevice(this._deviceId);
			if (device != null)
			{
				this._deviceInfo = device.DeviceInfo;
				this._defaultDeviceInfo = new DefaultDeviceInfo(this._deviceInfo);
			}
			XmlNode xmlNode = doc.GetElementsByTagName("Types")[0];
			if (xmlNode != null)
			{
				XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlNode);
				xmlNodeReader.Read();
				typeList.ReadTypes(xmlNodeReader, this._deviceId);
				if (additionalTypes != null)
				{
					if (typeList.TypeMap == null)
					{
						typeList.TypeMap = new LDictionary<string, ITypeDefinition>();
					}
					foreach (KeyValuePair<string, ITypeDefinition> keyValuePair in additionalTypes.TypeMap)
					{
						typeList.TypeMap.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			XmlNode xmlNode2 = null;
			XmlNode xmlNode3 = doc.GetElementsByTagName("DeviceDescription")[0];
			if (deviceId is IModuleIdentification)
			{
				IModuleIdentification val = (IModuleIdentification)(object)((deviceId is IModuleIdentification) ? deviceId : null);
				foreach (XmlNode item in xmlNode3)
				{
					if (item.NodeType != XmlNodeType.Element || !(item.Name == "Modules"))
					{
						continue;
					}
					foreach (XmlNode item2 in item)
					{
						if (item2.NodeType != XmlNodeType.Element || !(item2.Name == "Module"))
						{
							continue;
						}
						foreach (XmlNode item3 in item2)
						{
							if (item3.NodeType == XmlNodeType.Element && item3.Name == "ModuleId" && (item3 as XmlElement).InnerText == val.ModuleId)
							{
								xmlNode2 = item2;
								break;
							}
						}
					}
				}
			}
			else
			{
				foreach (XmlNode item4 in xmlNode3)
				{
					if (item4.NodeType == XmlNodeType.Element && item4.Name == "Device")
					{
						xmlNode2 = (XmlElement)item4;
						break;
					}
				}
			}
			if (xmlNode2 == null && this._deviceId is IModuleIdentification)
			{
				return;
			}
			if (xmlNode2 == null)
			{
				throw new DeviceNotFoundException(this._deviceId);
			}
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this._deviceId);
			if (targetSettingsById != null)
			{
				this._bCreateBitChannels = targetSettingsById.GetBoolValue(LocalTargetSettings.CreateBitChannels.Path, this._bCreateBitChannels);
				this._communicationSettings.MonitoringIntervalMsec = LocalTargetSettings.MonitoringInterval.GetIntValue(targetSettingsById);
			}
			LList<DeviceIdUpdate> devicesToUpdate = new LList<DeviceIdUpdate>();
			this.ReadDevice(xmlNode2, typeList, false, bVersionUpgrade, devicesToUpdate, deviceId, this._bCreateBitChannels);
			this._typeList = typeList;
			if (this._deviceParameterSet != null)
			{
				this._deviceParameterSet.SetIoProvider(this);
			}
			this._driverInfo.SetIoProvider(this);
			this.SetPositionIds();
			this.UpdateDependentObjects(true);
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00025A6C File Offset: 0x00024A6C
		internal void SetDeviceIdentification2(IDeviceIdentification deviceId, bool bUpdate, LList<DeviceIdUpdate> devicesToUpdate, out bool bMustBeRemoved)
		{
			bMustBeRemoved = false;
			if (this._deviceId == null && bUpdate)
			{
				throw new InvalidOperationException("Cannot update an uninitialized device object");
			}
			if (this._deviceId != null && !bUpdate)
			{
				throw new InvalidOperationException("Cannot change the device identification of a device. Use Update() instead");
			}
			bool bVersionUpgrade = this._deviceId != null && this._deviceId.Equals(deviceId, true);
			TypeList typeList = new TypeList();
			if (deviceId is IModuleIdentification)
			{
				this._deviceId = new ModuleIdentification((IModuleIdentification)deviceId);
			}
			else
			{
				this._deviceId = new DeviceIdentification(deviceId);
			}
			IDeviceDescription device = APEnvironment.DeviceRepository.GetDevice(this._deviceId);
			if (device != null)
			{
				this._deviceInfo = device.DeviceInfo;
				this._defaultDeviceInfo = new DefaultDeviceInfo(this._deviceInfo);
			}
			XmlNode xmlGlobalNode = APEnvironment.DeviceRepository.GetXmlGlobalNode(this._deviceId, "Types");
			if (xmlGlobalNode != null)
			{
				XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlGlobalNode);
				xmlNodeReader.Read();
				typeList.ReadTypes(xmlNodeReader, this._deviceId);
			}
			XmlNode xmlDeviceNode = APEnvironment.DeviceRepository.GetXmlDeviceNode(this._deviceId);
			if (xmlDeviceNode == null && this._deviceId is IModuleIdentification)
			{
				bMustBeRemoved = true;
				return;
			}
			if (xmlDeviceNode == null)
			{
				throw new DeviceObjectException(DeviceObjectExeptionReason.DeviceNotFound, "Device not found");
			}
			this.RemoveParameterNodes(xmlDeviceNode);
			this.ReadDevice(xmlDeviceNode, typeList, bUpdate, bVersionUpgrade, devicesToUpdate, this._deviceId, this._bCreateBitChannels);
			this._typeList = typeList;
			if (this._deviceParameterSet != null)
			{
				this._deviceParameterSet.SetIoProvider(this);
			}
			this._driverInfo.SetIoProvider(this);
			this.SetPositionIds();
			this.UpdateDependentObjects(true);
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00025BE8 File Offset: 0x00024BE8
		private void RemoveParameterNodes(XmlNode node)
		{
			List<XmlNode> list = new List<XmlNode>();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "Parameter")
				{
					list.Add(xmlNode);
				}
			}
			foreach (XmlNode oldChild in list)
			{
				node.RemoveChild(oldChild);
			}
			foreach (object obj2 in node.ChildNodes)
			{
				XmlNode node2 = (XmlNode)obj2;
				this.RemoveParameterNodes(node2);
			}
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00025CE4 File Offset: 0x00024CE4
		internal IConnectorCollection ConnectorList(bool bAllGroups)
		{
			ConnectorList connectorList = new ConnectorList();
			foreach (Connector connector3 in _connectors)
			{
				if (connector3.ConnectorGroup != 0)
				{
					bool flag = false;
					foreach (Connector connector4 in _connectors)
					{
						if ((int)connector4.ConnectorRole == 1 || connector4.ConnectorGroup != connector3.ConnectorGroup)
						{
							continue;
						}
						foreach (IAdapter item in (IEnumerable)connector4.Adapters)
						{
							if (item.Modules.Length != 0)
							{
								connectorList.Add(connector4);
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (!flag && bAllGroups)
					{
						connectorList.Add(connector3);
					}
				}
				else
				{
					connectorList.Add(connector3);
				}
			}
			return (IConnectorCollection)(object)connectorList;
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000603 RID: 1539 RVA: 0x00025E34 File Offset: 0x00024E34
		internal ConnectorList RawConnectorList
		{
			get
			{
				return this._connectors;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x00025E34 File Offset: 0x00024E34
		public override IConnectorCollection ConnectorsWithGroups
		{
			get
			{
				return this._connectors;
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x00025E3C File Offset: 0x00024E3C
		public override IConnectorCollection Connectors
		{
			get
			{
				return this.ConnectorList(false);
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x00025E48 File Offset: 0x00024E48
		// (set) Token: 0x06000607 RID: 1543 RVA: 0x00025E74 File Offset: 0x00024E74
		public IDeviceIdentification DeviceIdentification
		{
			get
			{
				if (this.SimulationMode)
				{
					ISimulationManager simulationMgrOrNull = APEnvironment.SimulationMgrOrNull;
					if (simulationMgrOrNull != null)
					{
						return simulationMgrOrNull.SimulationDeviceIdentification;
					}
				}
				return this._deviceId;
			}
			set
			{
				LList<DeviceIdUpdate> devicesToUpdate = new LList<DeviceIdUpdate>();
				this.SetDeviceIdentification(value, false, devicesToUpdate);
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x00025E90 File Offset: 0x00024E90
		public IDeviceIdentification DeviceIdentificationNoSimulation
		{
			get
			{
				return this._deviceId;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000609 RID: 1545 RVA: 0x00025E98 File Offset: 0x00024E98
		public IDeviceInfo DeviceInfo
		{
			get
			{
				if (this._deviceInfo == null)
				{
					IDeviceDescription device = APEnvironment.DeviceRepository.GetDevice(this._deviceId);
					if (device == null)
					{
						return this._defaultDeviceInfo;
					}
					this._deviceInfo = device.DeviceInfo;
				}
				return this._deviceInfo;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00025EDA File Offset: 0x00024EDA
		public override IParameterSet DeviceParameterSet
		{
			get
			{
				return this._deviceParameterSet;
			}
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00025EE4 File Offset: 0x00024EE4
		public string[] GetPossibleInterfacesForInsert(int nInsertPosition)
		{
			if (this._metaObject == null)
			{
				return null;
			}
			int numberOfFunctionalChildren = this.GetNumberOfFunctionalChildren();
			if (nInsertPosition < numberOfFunctionalChildren)
			{
				return null;
			}
			nInsertPosition -= numberOfFunctionalChildren;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in this.ConnectorList(true))
			{
				Connector connector = (Connector)obj;
				if (connector.ConnectorRole != ConnectorRole.Child)
				{
					if (connector.HostPath != -1)
					{
						bool flag = false;
						Connector connectorById = this.GetConnectorById(connector.HostPath);
						if (connectorById != null)
						{
							foreach (object obj2 in connectorById.Adapters)
							{
								IAdapter adapter = (IAdapter)obj2;
								if (adapter is SlotAdapter && adapter.Modules.Length != 0 && adapter.Modules[0] != Guid.Empty)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					if (connector.IsExplicit)
					{
						nInsertPosition--;
					}
					else
					{
						foreach (object obj3 in connector.Adapters)
						{
							IAdapter adapter2 = (IAdapter)obj3;
							if (adapter2 is IVarAdapter)
							{
								IVarAdapter varAdapter = (IVarAdapter)adapter2;
								if (varAdapter.ModulesCount < varAdapter.MaxDevices)
								{
									arrayList.Add(connector.Interface);
									if (connector.AdditionalInterfaces.Count > 0)
									{
										arrayList.AddRange(connector.AdditionalInterfaces);
									}
								}
							}
							nInsertPosition -= adapter2.ModulesCount;
							if (nInsertPosition < 0)
							{
								break;
							}
						}
					}
					if (nInsertPosition < 0)
					{
						break;
					}
				}
			}
			string[] array = new string[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x00026110 File Offset: 0x00025110
		public ICustomItemList CustomItems
		{
			get
			{
				return this._customItems;
			}
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00026118 File Offset: 0x00025118
		protected bool IsFunctionalType(Type type)
		{
			return !typeof(IDeviceObject).IsAssignableFrom(type) && !typeof(IExplicitConnector).IsAssignableFrom(type) && !typeof(SlotDeviceObject).IsAssignableFrom(type);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00026154 File Offset: 0x00025154
		internal int GetNumberOfFunctionalChildren()
		{
			Debug.Assert(this._metaObject != null);
			int num = 0;
			foreach (Guid objectGuid in this._metaObject.SubObjectGuids)
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._metaObject.ProjectHandle, objectGuid);
				if (this.IsFunctionalType(metaObjectStub.ObjectType))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x000261BF File Offset: 0x000251BF
		public IDeviceAttributesCollection Attributes
		{
			get
			{
				return this._attributes;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x000261C8 File Offset: 0x000251C8
		public bool AllowsDirectCommunication
		{
			get
			{
				string text = this._attributes.GetAttribute("StdCommunicationLink");
				if (text == null)
				{
					return false;
				}
				text = text.Trim().ToUpperInvariant();
				return !text.Equals("false", StringComparison.InvariantCultureIgnoreCase) && !text.Equals("0");
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x00026215 File Offset: 0x00025215
		public ICommunicationSettings CommunicationSettings
		{
			get
			{
				return this._communicationSettings;
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0002621D File Offset: 0x0002521D
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x00026225 File Offset: 0x00025225
		public bool Disable
		{
			get
			{
				return this._bDisable;
			}
			set
			{
				this._bDisable = value;
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0002622E File Offset: 0x0002522E
		// (set) Token: 0x06000615 RID: 1557 RVA: 0x00026236 File Offset: 0x00025236
		public bool Exclude
		{
			get
			{
				return this._bExclude;
			}
			set
			{
				this._bExclude = value;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0002623F File Offset: 0x0002523F
		public IStringTable2 StringTable
		{
			get
			{
				if (this._stringTable == null)
				{
					this._stringTable = new StringTable(this);
				}
				return this._stringTable;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000617 RID: 1559 RVA: 0x0002623F File Offset: 0x0002523F
		public IStringTable3 StringTable3
		{
			get
			{
				if (this._stringTable == null)
				{
					this._stringTable = new StringTable(this);
				}
				return this._stringTable;
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x0002625B File Offset: 0x0002525B
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x00026263 File Offset: 0x00025263
		public bool IsInUpdate
		{
			get
			{
				return this._bIsInUpdate;
			}
			internal set
			{
				this._bIsInUpdate = value;
			}
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0002626C File Offset: 0x0002526C
		public void SetUserBaseAddress(DirectVariableLocation location, string stBaseAddress)
		{
			this._ioProviderBase.SetUserBaseAddress(location, stBaseAddress);
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0002627B File Offset: 0x0002527B
		public string GetUserBaseAddress(DirectVariableLocation location)
		{
			return this._ioProviderBase.GetUserBaseAddress(location);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00026289 File Offset: 0x00025289
		public IDirectVariable GetNextUnusedAddress(DirectVariableLocation location)
		{
			return this._ioProviderBase.GetNextUnusedAddress(location);
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00026297 File Offset: 0x00025297
		public void SetNextUnusedAddress(IDirectVariable addr)
		{
			this._ioProviderBase.SetNextUnusedAddress(addr);
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x00025EDA File Offset: 0x00024EDA
		public IParameterSet ParameterSet
		{
			get
			{
				return this._deviceParameterSet;
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x000262A5 File Offset: 0x000252A5
		int IIoProvider.TypeId
		{
			get
			{
				return this._deviceId.Type;
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x000262B2 File Offset: 0x000252B2
		public int GetGranularity(DirectVariableLocation location)
		{
			return this._ioProviderBase.GetGranularity(location);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x000262C0 File Offset: 0x000252C0
		public void SetGranularity(DirectVariableLocation location, int nGranularity)
		{
			this._ioProviderBase.SetGranularity(location, nGranularity);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x000262CF File Offset: 0x000252CF
		public string GetParamsListName()
		{
			return this.IoBaseName + "_DPS";
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x000262E1 File Offset: 0x000252E1
		// (set) Token: 0x06000624 RID: 1572 RVA: 0x000262FD File Offset: 0x000252FD
		public string IoUpdateTask
		{
			get
			{
				if (this._stIoUpdateTask == string.Empty)
				{
					return null;
				}
				return this._stIoUpdateTask;
			}
			set
			{
				if (value == string.Empty)
				{
					this._stIoUpdateTask = null;
					return;
				}
				this._stIoUpdateTask = value;
			}
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0002631B File Offset: 0x0002531B
		public void GetModuleAddress(out uint uiModuleType, out uint uiInstance)
		{
			this._ioProviderBase.GetModuleAddress(this, out uiModuleType, out uiInstance);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0002632B File Offset: 0x0002532B
		public IMetaObject GetMetaObject()
		{
			if (this._metaObject != null)
			{
				return this._metaObject;
			}
			if (this._guidObject != Guid.Empty)
			{
				return APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, this._guidObject);
			}
			return null;
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x00003E50 File Offset: 0x00002E50
		IIoProvider IIoProvider.Parent
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x00026368 File Offset: 0x00025368
		IIoProvider[] IIoProvider.Children
		{
			get
			{
				LList<IIoProvider> llist = new LList<IIoProvider>();
				foreach (object obj in this.Connectors)
				{
					Connector connector = (Connector)obj;
					if (connector.ConnectorRole == ConnectorRole.Parent && connector.HostPath == -1)
					{
						if (connector.IsExplicit)
						{
							IIoProvider connectorObject = connector.GetConnectorObject();
							if (connectorObject != null)
							{
								llist.Add(connectorObject);
							}
						}
						else
						{
							llist.Add(connector);
						}
					}
				}
				IIoProvider[] array = new IIoProvider[llist.Count];
				llist.CopyTo(array, 0);
				return array;
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00026410 File Offset: 0x00025410
		IDeviceObject IIoProvider.GetHost()
		{
			if (this.IsLogical && this._metaObject != null)
			{
				IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
				if (hostStub != null)
				{
					IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(hostStub.ProjectHandle, hostStub.ObjectGuid);
					if (objectToRead != null)
					{
						return objectToRead.Object as IDeviceObject;
					}
				}
			}
			return this;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00026474 File Offset: 0x00025474
		public bool IoProviderEquals(IIoProvider provider)
		{
			DeviceObject deviceObject = provider as DeviceObject;
			return deviceObject != null && deviceObject.MetaObject != null && deviceObject.MetaObject.ProjectHandle == this._nProjectHandle && deviceObject.MetaObject.ObjectGuid.Equals(this._guidObject);
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x000264C8 File Offset: 0x000254C8
		private bool SearchDeviceObject(ISVNode node, int nProjectHandle)
		{
			try
			{
				if (APEnvironment.ObjectMgr.ExistsObject(nProjectHandle, node.ObjectGuid))
				{
					Guid objectGuid = node.ObjectGuid;
					foreach (object obj in this.Connectors)
					{
						foreach (object obj2 in ((Connector)obj).Adapters)
						{
							Guid[] modules = ((IAdapter)obj2).Modules;
							for (int i = 0; i < modules.Length; i++)
							{
								if (modules[i] == objectGuid)
								{
									return true;
								}
							}
						}
					}
				}
				foreach (ISVNode node2 in node.Children)
				{
					if (this.SearchDeviceObject(node2, nProjectHandle))
					{
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x000265F4 File Offset: 0x000255F4
		internal IDeviceObject GetHostDeviceObject()
		{
			if (this._hostObjectGuid == Guid.Empty)
			{
				IMetaObjectStub plcDevice = APEnvironment.DeviceMgr.GetPlcDevice(this._nProjectHandle, this._guidObject);
				if (plcDevice != null)
				{
					this._hostObjectGuid = plcDevice.ObjectGuid;
				}
			}
			if (this._hostObjectGuid != Guid.Empty)
			{
				IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
				if (primaryProject != null)
				{
					int handle = primaryProject.Handle;
					if (APEnvironment.ObjectMgr.ExistsObject(handle, this._hostObjectGuid))
					{
						IObject @object = APEnvironment.ObjectMgr.GetObjectToRead(handle, this._hostObjectGuid).Object;
						if (@object is IDeviceObject)
						{
							return @object as IDeviceObject;
						}
					}
				}
			}
			if (this.MetaObject != null)
			{
				foreach (object obj in this.Connectors)
				{
					IConnector3 connector = (IConnector3)obj;
					if (connector != null)
					{
						IDeviceObject host = connector.GetHost();
						if (host != null)
						{
							this._hostObjectGuid = host.MetaObject.ObjectGuid;
							return host;
						}
					}
				}
				int projectHandle = this.MetaObject.ProjectHandle;
				Guid parentObjectGuid = this.MetaObject.ParentObjectGuid;
				if (this.MetaObject.ParentObjectGuid == Guid.Empty)
				{
					return this.MetaObject.Object as IDeviceObject;
				}
				while (parentObjectGuid != Guid.Empty)
				{
					if (!APEnvironment.ObjectMgr.ExistsObject(projectHandle, parentObjectGuid))
					{
						return null;
					}
					IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(projectHandle, parentObjectGuid);
					if (objectToRead == null)
					{
						return null;
					}
					if (objectToRead.Object is IDeviceObject && objectToRead.ParentObjectGuid == Guid.Empty)
					{
						this._hostObjectGuid = objectToRead.ObjectGuid;
						return objectToRead.Object as IDeviceObject;
					}
					parentObjectGuid = objectToRead.ParentObjectGuid;
				}
				goto IL_44E;
			}
			IProject primaryProject2 = APEnvironment.Engine.Projects.PrimaryProject;
			if (primaryProject2 != null)
			{
				int handle2 = primaryProject2.Handle;
				Guid structuredViewGuid = new Guid("{D9B2B2CC-EA99-4c3b-AA42-1E5C49E65B84}");
				IStructuredView structuredView = APEnvironment.ObjectMgr.GetStructuredView(primaryProject2.Handle, structuredViewGuid);
				foreach (ISVNode isvnode in structuredView.Children)
				{
					bool flag = false;
					foreach (object obj2 in this.Connectors)
					{
						Connector connector2 = (Connector)obj2;
						if (connector2.ConnectorRole != ConnectorRole.Parent)
						{
							foreach (object obj3 in connector2.Adapters)
							{
								IAdapter adapter = (IAdapter)obj3;
								if (adapter is SlotAdapter)
								{
									foreach (Guid guid in adapter.Modules)
									{
										if (guid != Guid.Empty)
										{
											ISVNode isvnode2 = structuredView.GetNode(guid);
											if (isvnode2 != null)
											{
												while (isvnode2.Parent != null)
												{
													isvnode2 = isvnode2.Parent;
												}
												if (isvnode2.ObjectGuid == isvnode.ObjectGuid)
												{
													flag = true;
													break;
												}
											}
										}
									}
									if (flag)
									{
										break;
									}
								}
							}
							if (flag)
							{
								break;
							}
						}
					}
					if (flag || this.SearchDeviceObject(isvnode, handle2))
					{
						Guid guid2 = isvnode.ObjectGuid;
						while (guid2 != Guid.Empty && typeof(IDeviceObject).IsAssignableFrom(isvnode.ObjectType))
						{
							if (!APEnvironment.ObjectMgr.ExistsObject(handle2, guid2) || !APEnvironment.ObjectMgr.IsObjectLoaded(handle2, guid2))
							{
								return null;
							}
							IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(handle2, guid2);
							Debug.Assert(metaObjectStub != null);
							if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && metaObjectStub.ParentObjectGuid == Guid.Empty)
							{
								if (isvnode.ObjectGuid == metaObjectStub.ObjectGuid)
								{
									return this;
								}
								return APEnvironment.ObjectMgr.GetObjectToRead(handle2, guid2).Object as IDeviceObject;
							}
							else
							{
								guid2 = metaObjectStub.ParentObjectGuid;
							}
						}
					}
				}
			}
		IL_44E:
			return null;
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x00026A7C File Offset: 0x00025A7C
		public IAddressAssignmentStrategy Strategy
		{
			get
			{
				IDeviceObject hostDeviceObject = this.GetHostDeviceObject();
				IAddressAssignmentStrategy addressAssignmentStrategy = null;
				if (hostDeviceObject != null && hostDeviceObject.MetaObject != null)
				{
					addressAssignmentStrategy = DeviceObjectHelper.GetStrategy(hostDeviceObject.MetaObject.ObjectGuid);
				}
				if (addressAssignmentStrategy == null)
				{
					try
					{
						int iMinStructureGranularity = -1;
						IDeviceIdentification id = this._deviceId;
						if (hostDeviceObject != null)
						{
							id = (hostDeviceObject as IDeviceObject5).DeviceIdentificationNoSimulation;
						}
						ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(id);
						if (targetSettingsById != null)
						{
							string stringValue = LocalTargetSettings.AddressAssignmentGuid.GetStringValue(targetSettingsById);
							if (stringValue != string.Empty)
							{
								addressAssignmentStrategy = APEnvironment.TryCreateAddressAssignmentStrategy(new Guid(stringValue));
								if (addressAssignmentStrategy != null)
								{
									return addressAssignmentStrategy;
								}
							}
							iMinStructureGranularity = LocalTargetSettings.MinimalStructureGranularity.GetIntValue(targetSettingsById);
							if (LocalTargetSettings.ByteAddressing.GetBoolValue(targetSettingsById))
							{
								addressAssignmentStrategy = new ByteAddressFlatAdressAssignmentStrategy(iMinStructureGranularity);
								return addressAssignmentStrategy;
							}
							bool flag = LocalTargetSettings.BitWordAddressing.GetBoolValue(targetSettingsById);
							if (!flag && hostDeviceObject is IDeviceObject2 && (hostDeviceObject as IDeviceObject2).DriverInfo is IDriverInfo2)
							{
								Guid ioApplication = ((hostDeviceObject as IDeviceObject2).DriverInfo as IDriverInfo2).IoApplication;
								if (ioApplication != Guid.Empty && APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, ioApplication))
								{
									ILMPreCompileSet preCompileSet = APEnvironment.LMServiceProvider.PreCompileService.GetPreCompileSet(ioApplication);
									if (preCompileSet != null)
									{
										flag = ((IMemorySettings3)preCompileSet.MemorySettings).BitWordAddressing;
									}
								}
							}
							if (flag)
							{
								addressAssignmentStrategy = new BitWordAddressFlatAdressAssignmentStrategy(iMinStructureGranularity);
								return addressAssignmentStrategy;
							}
						}
						else if (APEnvironment.Engine.OEMCustomization.HasValue("DeviceObject", "AddressAssignmentGuid"))
						{
							string stringValue2 = APEnvironment.Engine.OEMCustomization.GetStringValue("DeviceObject", "AddressAssignmentGuid");
							if (!string.IsNullOrEmpty(stringValue2))
							{
								Guid typeGuid = new Guid(stringValue2);
								try
								{
									addressAssignmentStrategy = APEnvironment.TryCreateAddressAssignmentStrategy(typeGuid);
									if (addressAssignmentStrategy != null)
									{
										return addressAssignmentStrategy;
									}
								}
								catch
								{
								}
							}
						}
						if (hostDeviceObject == null && targetSettingsById == null)
						{
							return new FlatAdressAssignmentStrategy(iMinStructureGranularity);
						}
						if (hostDeviceObject == null || hostDeviceObject.MetaObject == null || hostDeviceObject.MetaObject.Name == null)
						{
							return new FlatAdressAssignmentStrategy(iMinStructureGranularity);
						}
						addressAssignmentStrategy = new FlatAdressAssignmentStrategy(iMinStructureGranularity);
					}
					finally
					{
						if (addressAssignmentStrategy != null && hostDeviceObject != null && hostDeviceObject.MetaObject != null)
						{
							DeviceObjectHelper.SaveStrategy(hostDeviceObject.MetaObject.ObjectGuid, addressAssignmentStrategy);
						}
					}
					return addressAssignmentStrategy;
				}
				return addressAssignmentStrategy;
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x00026CD8 File Offset: 0x00025CD8
		// (set) Token: 0x0600062F RID: 1583 RVA: 0x00026CE5 File Offset: 0x00025CE5
		[DefaultSerialization("NeedsBusCycle")]
		[StorageVersion("3.3.0.0")]
		[Obsolete("Use DriverInfo.NeedsBusCycle instead")]
		public bool NeedsBusCycle
		{
			get
			{
				return this._driverInfo.NeedsBusCycle;
			}
			set
			{
				this._driverInfo.NeedsBusCycle = value;
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x00026CF3 File Offset: 0x00025CF3
		// (set) Token: 0x06000631 RID: 1585 RVA: 0x00026CFB File Offset: 0x00025CFB
		[Obsolete("Use DriverInfo.BusCycleTask instead")]
		public Guid BusCycleTaskGuid
		{
			get
			{
				return this._guidBusCycleTask;
			}
			set
			{
				this._guidBusCycleTask = value;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x00026D04 File Offset: 0x00025D04
		public IDriverInfo DriverInfo
		{
			get
			{
				return this._driverInfo;
			}
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00026D0C File Offset: 0x00025D0C
		public virtual IMetaObject GetApplication()
		{
			IPlcLogicObject plcLogicObject = this.GetPlcLogicObject();
			if (plcLogicObject == null)
			{
				return null;
			}
			IApplicationObject applicationObject = this.GetApplicationObject(plcLogicObject);
			if (applicationObject == null)
			{
				return null;
			}
			return applicationObject.MetaObject;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00026D38 File Offset: 0x00025D38
		public IMetaObject GetPlcLogic()
		{
			if (this.UseParentPLC)
			{
				return null;
			}
			IMetaObjectStub metaObjectStub;
			if (this.MetaObject == null)
			{
				metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, this._guidObject);
			}
			else
			{
				metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this.MetaObject.ProjectHandle, this.MetaObject.ObjectGuid);
			}
			foreach (Guid objectGuid in metaObjectStub.SubObjectGuids)
			{
				IMetaObjectStub metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(metaObjectStub.ProjectHandle, objectGuid);
				if (typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub2.ObjectType))
				{
					return APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub.ProjectHandle, objectGuid);
				}
			}
			return null;
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000635 RID: 1589 RVA: 0x0000464A File Offset: 0x0000364A
		bool IIoProvider.Disabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x0000464A File Offset: 0x0000364A
		bool IIoProvider.Excluded
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x00026DEC File Offset: 0x00025DEC
		// (set) Token: 0x06000638 RID: 1592 RVA: 0x00026DF4 File Offset: 0x00025DF4
		public int ModuleId
		{
			get
			{
				return this._nModuleId;
			}
			set
			{
				this._nModuleId = value;
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x00026DFD File Offset: 0x00025DFD
		// (set) Token: 0x0600063A RID: 1594 RVA: 0x00026E08 File Offset: 0x00025E08
		public IMetaObject MetaObject
		{
			get
			{
				return this._metaObject;
			}
			set
			{
				if (this._metaObject != null && value == null)
				{
					this._metaObject = APEnvironment.ObjectMgr.GetObjectToRead(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
				}
				else
				{
					this._metaObject = value;
				}
				if (this._metaObject != null)
				{
					this._guidObject = this._metaObject.ObjectGuid;
					this._nProjectHandle = this._metaObject.ProjectHandle;
					this.Strategy.UpdateAddresses(this);
					foreach (object obj in this.Connectors)
					{
						((Connector)obj).UpdateAddresses();
					}
					foreach (object obj2 in this._logicalDevices)
					{
						((LogicalMappedDevice)obj2).MetaObject = this._metaObject;
					}
				}
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x00026F1C File Offset: 0x00025F1C
		public Guid Namespace
		{
			get
			{
				return DeviceObject.GUID_DEVICENAMESPACE;
			}
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00026F23 File Offset: 0x00025F23
		public bool CheckName(string stName)
		{
			return DeviceObject.IsIdentifier(stName);
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x00026F2B File Offset: 0x00025F2B
		public bool CanRename
		{
			get
			{
				return !(this._deviceInfo is IDeviceInfo4) || !(this._deviceInfo as IDeviceInfo4).FixName;
			}
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00004152 File Offset: 0x00003152
		public void HandleRenamed()
		{
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00026F50 File Offset: 0x00025F50
		public string GetPositionText(long nPosition)
		{
			foreach (IDeviceObjectFindReplaceFactory deviceObjectFindReplaceFactory in APEnvironment.DeviceObjectFindReplaceFactories)
			{
				if (deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2 && this._metaObject != null && deviceObjectFindReplaceFactory.GetMatch(this._metaObject))
				{
					return (deviceObjectFindReplaceFactory as IDeviceObjectFindReplaceFactory2).GetPositionText(nPosition, this._metaObject);
				}
			}
			return null;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00026FCC File Offset: 0x00025FCC
		public string GetContentString(ref long nPosition, ref int nLength, bool bWord)
		{
			foreach (IDeviceObjectFindReplaceFactory deviceObjectFindReplaceFactory in APEnvironment.DeviceObjectFindReplaceFactories)
			{
				if (deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2 && this._metaObject != null && deviceObjectFindReplaceFactory.GetMatch(this._metaObject))
				{
					return (deviceObjectFindReplaceFactory as IDeviceObjectFindReplaceFactory2).GetContentString(ref nPosition, ref nLength, bWord, this._metaObject);
				}
			}
			string empty = string.Empty;
			return base.GetFindReplaceContentString(ref nPosition, ref nLength, bWord);
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000641 RID: 1601 RVA: 0x00003E50 File Offset: 0x00002E50
		public IEmbeddedObject[] EmbeddedObjects
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x00027058 File Offset: 0x00026058
		public IUniqueIdGenerator UniqueIdGenerator
		{
			get
			{
				return this._idGenerator;
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00027060 File Offset: 0x00026060
		public virtual bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject == null)
			{
				return this.AllowTopLevel;
			}
			return parentObject is IDeviceObject || parentObject is IConnector;
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0002707F File Offset: 0x0002607F
		public virtual bool AcceptsChildObject(Type childObjectType)
		{
			return this.AcceptsChildObject(childObjectType, -1);
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0002708C File Offset: 0x0002608C
		public virtual int CheckRelationships(IObject parentObject, IObject[] childObjects)
		{
			if (!this.AcceptsParentObject(parentObject))
			{
				return -1;
			}
			Hashtable hashtable = new Hashtable();
			foreach (IObject @object in childObjects)
			{
				hashtable.Add(@object.MetaObject.ObjectGuid, @object);
			}
			int num = 0;
			foreach (object obj in this.Connectors)
			{
				Connector connector = (Connector)obj;
				if (connector.ConnectorRole != ConnectorRole.Child)
				{
					if (connector.IsExplicit)
					{
						if (connector.ExplicitConnectorGuid != Guid.Empty)
						{
							num++;
							if (!hashtable.Contains(connector.ExplicitConnectorGuid))
							{
								return num;
							}
							hashtable.Remove(connector.ExplicitConnectorGuid);
						}
					}
					else
					{
						foreach (object obj2 in connector.Adapters)
						{
							IAdapter adapter = (IAdapter)obj2;
							if (adapter is FixedAdapter || adapter is SlotAdapter)
							{
								foreach (Guid guid in adapter.Modules)
								{
									num++;
									if (guid != Guid.Empty)
									{
										if (!hashtable.ContainsKey(guid) || !(hashtable[guid] is IDeviceObject))
										{
											return num;
										}
										hashtable.Remove(guid);
									}
								}
							}
							else if (adapter is VarAdapter)
							{
								foreach (Guid guid2 in adapter.Modules)
								{
									num++;
									hashtable.Remove(guid2);
								}
							}
						}
					}
				}
			}
			foreach (Guid guid3 in this._funcChildrenTypeGuids)
			{
				if (guid3 != Guid.Empty)
				{
					bool flag = false;
					foreach (object obj3 in hashtable.Values)
					{
						IObject object2 = (IObject)obj3;
						if (TypeGuidAttribute.FromObject(object2).Guid == guid3)
						{
							flag = true;
							hashtable.Remove(object2.MetaObject.ObjectGuid);
							break;
						}
					}
					if (!flag)
					{
						return num + 1;
					}
				}
			}
			foreach (object obj4 in hashtable.Values)
			{
				IObject object3 = (IObject)obj4;
				if (object3 is IDeviceObject || object3 is IExplicitConnector || object3 is SlotDeviceObject)
				{
					return num;
				}
			}
			return 0;
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00027400 File Offset: 0x00026400
		public int GetEnvisionedIndexOf(int nProjectHandle, Guid objectGuid)
		{
			return this.GetChildIndex(objectGuid);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0002740C File Offset: 0x0002640C
		public int GetChildIndex(Guid subObjectGuid)
		{
			if (!APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, subObjectGuid))
			{
				return -1;
			}
			IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, subObjectGuid);
			if (!this.IsFunctionalType(metaObjectStub.ObjectType))
			{
				if (this.nFunctionalChildren == -1)
				{
					this.nFunctionalChildren = this.GetNumberOfFunctionalChildren();
				}
				int num = this.nFunctionalChildren;
				foreach (object obj in this.Connectors)
				{
					int num2;
					int childIndex = ((Connector)obj).GetChildIndex(subObjectGuid, out num2);
					if (childIndex >= 0)
					{
						return num + childIndex;
					}
					num += num2;
				}
				return -1;
			}
			int num3 = 0;
			foreach (Guid guid in this.MetaObject.SubObjectGuids)
			{
				if (guid != subObjectGuid)
				{
					IMetaObjectStub metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(this.MetaObject.ProjectHandle, guid);
					if (this.IsFunctionalType(metaObjectStub2.ObjectType))
					{
						int num4 = metaObjectStub.Name.CompareTo(metaObjectStub2.Name);
						if (num4 > 0)
						{
							num3++;
						}
						else if (num4 == 0 && subObjectGuid.CompareTo(guid) > 0)
						{
							num3++;
						}
					}
				}
			}
			return num3;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00027570 File Offset: 0x00026570
		public bool AcceptsChildObject(Type childObjectType, int nIndex)
		{
			if (nIndex == -1)
			{
				nIndex = this._metaObject.SubObjectGuids.Length;
			}
			if (typeof(IDeviceObject).IsAssignableFrom(childObjectType))
			{
				return nIndex >= this.GetNumberOfFunctionalChildren();
			}
			if (typeof(ITraceObject).IsAssignableFrom(childObjectType) && this._metaObject.ParentObjectGuid == Guid.Empty)
			{
				ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this.DeviceIdentificationNoSimulation);
				return LocalTargetSettings.TraceManager.GetBoolValue(targetSettingsById);
			}
			return true;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00027600 File Offset: 0x00026600
		public void AddChild(Guid subObjectGuid, int nIndex)
		{
			IUndoManager undoManager = APEnvironment.ObjectMgr.GetUndoManager(this._metaObject.ProjectHandle);
			if (undoManager != null && undoManager.InUndo && undoManager.NextRedoAction == ReplaceDeviceAction.REPLACEACTION)
			{
				return;
			}
			if (!DeviceManager.DoNotSkipEvents && (APEnvironment.Engine.Projects.PrimaryProject == null || this._metaObject.ProjectHandle != APEnvironment.Engine.Projects.PrimaryProject.Handle))
			{
				return;
			}
			IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._metaObject.ProjectHandle, subObjectGuid);
			if (objectToRead != null)
			{
				this.InsertChild(nIndex, objectToRead.Object, objectToRead.ObjectGuid);
			}
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x000276A8 File Offset: 0x000266A8
		public void RemoveChild(IMetaObject moRemoved)
		{
			this.RemoveChild(moRemoved.ObjectGuid);
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x000276B8 File Offset: 0x000266B8
		internal void AddFunctionalParameter(StringBuilder sb, Parameter param)
		{
			sb.AppendFormat("{{attribute 'device_parameter' := '{0}'}}", param.Identifier);
			sb.AppendLine();
			if ((param.GetAccessRight(false) & AccessRight.Write) != AccessRight.Write)
			{
				sb.AppendLine("{attribute 'read_only'}");
			}
			if (param.GetAccessRight(false) == AccessRight.None)
			{
				sb.AppendLine("{attribute 'nowatch'}");
			}
			sb.Append(DeviceObjectHelper.BuildIecIdentifier(param.DataElementBase.GetVisibleName.Default));
			sb.Append(" : ");
			if (param.IsEnumeration && !string.IsNullOrEmpty(param.ParamType) && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 3, 0))
			{
				string[] array = param.ParamType.Split(new char[]
				{
					':'
				});
				sb.Append(array[1] + "Enum");
			}
			else
			{
				sb.Append(param.BaseType);
			}
			if (!string.IsNullOrWhiteSpace(param.Value))
			{
				sb.Append(" := ");
				sb.Append(param.Value);
			}
			sb.Append(";");
			sb.AppendLine();
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x000277CC File Offset: 0x000267CC
		internal string GetFunctionalSectionType(IParameterSection section)
		{
			string text = string.Empty;
			while (section != null)
			{
				text = DeviceObjectHelper.BuildIecIdentifier((section as ParameterSection).GetName().Default) + text;
				section = section.Section;
			}
			return text + "_Struct";
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00027814 File Offset: 0x00026814
		internal StringBuilder GetCreateFunctionalSections(LDictionary<IParameterSection, StringBuilder> dictSections, List<string> liData, IParameterSection section, out bool bCreated)
		{
			bCreated = false;
			StringBuilder stringBuilder = null;
			if (!dictSections.TryGetValue(section, out stringBuilder))
			{
				bCreated = true;
				stringBuilder = new StringBuilder();
				dictSections.Add(section, stringBuilder);
				string arg = DeviceObjectHelper.BuildIecIdentifier((section as ParameterSection).GetName().Default);
				stringBuilder.AppendFormat("TYPE {0}:\nSTRUCT\n", this.GetFunctionalSectionType(section));
				if (section.Section == null)
				{
					liData.Add(string.Format("{0} : {1};\n", arg, this.GetFunctionalSectionType(section)));
				}
				if (section.Section != null)
				{
					bool flag = false;
					StringBuilder createFunctionalSections = this.GetCreateFunctionalSections(dictSections, liData, section.Section, out flag);
					if (bCreated)
					{
						string arg2 = DeviceObjectHelper.BuildIecIdentifier((section as ParameterSection).GetName().Default);
						createFunctionalSections.Append(string.Format("{0} : {1};\n", arg2, this.GetFunctionalSectionType(section)));
					}
					bCreated = flag;
					section = section.Section;
				}
			}
			return stringBuilder;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x000278F0 File Offset: 0x000268F0
		private void AddLocalLanguageModel(ILanguageModelBuilder3 lmb, ILanguageModel lmnew, XmlWriter xmlWriter, IPlcLogicObject logic, IApplicationObject app, List<List<string>> codeTables, bool bCreateAdditionalParams, bool bNoIoDownload, bool bSkipAdditionalParamsForZeroParams)
		{
			try
			{
				if (logic != null && app != null)
				{
					DeviceObjectHelper.IsInLateLanguageModel = true;
					DeviceObjectHelper.IoProviderChildrens.Clear();
				}
				Guid objectGuid = this.MetaObject.ObjectGuid;
				objectGuid.ToString().Replace('-', '_');
				string stErrors = "";
				if (logic != null)
				{
					bool flag = true;
					DeviceObject deviceObject = this.GetHostDeviceObject() as DeviceObject;
					if (deviceObject != null)
					{
						ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)deviceObject).DeviceIdentificationNoSimulation);
						if (LocalTargetSettings.NoPrecompileMessages.GetBoolValue(targetSettingsById))
						{
							flag = false;
						}
					}
					long num = PositionHelper.CombinePosition(this._driverInfo.GetPositionId(2), 0);
					string str = "{p " + num + "}";
					if (app == null)
					{
						if (flag)
						{
							stErrors = str + "{error '" + Strings.ErrorMissingIoApplication + "' show_precompile}";
						}
					}
					else if (!this._driverInfo.IoApplicationSet || this._driverInfo.IoApplication == Guid.Empty)
					{
						if (flag)
						{
							stErrors = str + "{error '" + Strings.WarningNoIoApplication + "' show_precompile}";
						}
					}
					else
					{
						stErrors = "(* *)";
					}
				}
				if (!bNoIoDownload)
				{
					xmlWriter.WriteStartElement("language-model");
					Guid appGuid = Guid.Empty;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 2, 0, 3))
					{
						if (app != null && logic != null)
						{
							xmlWriter.WriteAttributeString("application-id", app.MetaObject.ObjectGuid.ToString());
							xmlWriter.WriteAttributeString("plclogic-id", logic.MetaObject.ObjectGuid.ToString());
							xmlWriter.WriteAttributeString("object-id", objectGuid.ToString());
							appGuid = app.MetaObject.ObjectGuid;
						}
						else
						{
							DeviceObject deviceObject2 = null;
							if (this.IsLogical && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
							{
								IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, this._guidObject);
								while (metaObjectStub != null)
								{
									if (!(metaObjectStub.ParentObjectGuid != Guid.Empty))
									{
										break;
									}
									metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, metaObjectStub.ParentObjectGuid);
									if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) && metaObjectStub.ParentObjectGuid == Guid.Empty)
									{
										deviceObject2 = (APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, metaObjectStub.ObjectGuid).Object as DeviceObject);
										break;
									}
								}
							}
							else
							{
								deviceObject2 = (this.GetHostDeviceObject() as DeviceObject);
							}
							if (deviceObject2 != null && deviceObject2.UseParentPLC && APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, deviceObject2.MetaObject.ParentObjectGuid))
							{
								IMetaObjectStub plcDevice = APEnvironment.DeviceMgr.GetPlcDevice(this._nProjectHandle, deviceObject2.MetaObject.ParentObjectGuid);
								if (plcDevice != null)
								{
									deviceObject2 = (APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, plcDevice.ObjectGuid).Object as DeviceObject);
								}
							}
							if (deviceObject2 != null)
							{
								logic = deviceObject2.GetPlcLogicObject();
								if (logic != null)
								{
									app = deviceObject2.GetDeviceApplicationObject(logic);
									if (app == null)
									{
										app = deviceObject2.GetApplicationObject(logic);
									}
									if (app != null)
									{
										xmlWriter.WriteAttributeString("application-id", app.MetaObject.ObjectGuid.ToString());
										xmlWriter.WriteAttributeString("plclogic-id", logic.MetaObject.ObjectGuid.ToString());
										xmlWriter.WriteAttributeString("object-id", objectGuid.ToString());
										appGuid = app.MetaObject.ObjectGuid;
									}
								}
							}
						}
					}
					bool bCreateAdditionalParams2 = bCreateAdditionalParams;
					if (bSkipAdditionalParamsForZeroParams && this._deviceParameterSet.Count == 0 && this._metaObject.ParentObjectGuid != Guid.Empty)
					{
						bCreateAdditionalParams2 = false;
					}
					LanguageModelHelper.GetLanguageModelForParameterSet(lmb, lmnew, this._deviceParameterSet, this.GetParamsListName(), xmlWriter, this.MetaObject.ObjectGuid, stErrors, codeTables, bCreateAdditionalParams2, this._bDownloadParamsDevDescOrder, appGuid, this._driverInfo._bUpdateIOsInStop);
					foreach (object obj in this.Connectors)
					{
						Connector connector = (Connector)obj;
						if (!connector.IsExplicit)
						{
							bCreateAdditionalParams2 = bCreateAdditionalParams;
							if (bSkipAdditionalParamsForZeroParams && connector.HostParameterSet.Count == 0)
							{
								bCreateAdditionalParams2 = false;
							}
							LanguageModelHelper.GetLanguageModelForParameterSet(lmb, lmnew, (ParameterSet)connector.HostParameterSet, connector.GetParamsListName(), xmlWriter, this._guidObject, string.Empty, codeTables, bCreateAdditionalParams2, connector.DownloadParamsDevDescOrder, appGuid, false);
						}
					}
					if (this._dictGlobalDataTypes.Count > 0 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 9, 0))
					{
						LanguageModelContainer languageModelContainer = new LanguageModelContainer();
						languageModelContainer.lmBuilder = lmb;
						languageModelContainer.lmNew = lmnew;
						foreach (KeyValuePair<string, LDictionary<IDataElement, Guid>> keyValuePair in this._dictGlobalDataTypes)
						{
							LSortedList<Guid, IDictionary<Guid, IDataElement>> lsortedList = new LSortedList<Guid, IDictionary<Guid, IDataElement>>();
							foreach (KeyValuePair<IDataElement, Guid> keyValuePair2 in keyValuePair.Value)
							{
								IDictionary<Guid, IDataElement> dictionary;
								if (!lsortedList.TryGetValue(keyValuePair2.Value, out dictionary))
								{
									if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 9, 20))
									{
										dictionary = new LSortedList<Guid, IDataElement>();
									}
									else
									{
										dictionary = new LDictionary<Guid, IDataElement>();
									}
									lsortedList.Add(keyValuePair2.Value, dictionary);
								}
								DataElementStructType dataElementStructType = keyValuePair2.Key as DataElementStructType;
								if (dataElementStructType != null && !dataElementStructType.HasIecType)
								{
									dictionary[dataElementStructType.LmStructType] = dataElementStructType;
								}
								DataElementUnionType dataElementUnionType = keyValuePair2.Key as DataElementUnionType;
								if (dataElementUnionType != null && !dataElementUnionType.HasIecType)
								{
									dictionary[dataElementUnionType.LmUnionType] = dataElementUnionType;
								}
							}
							foreach (IDictionary<Guid, IDataElement> dictionary2 in lsortedList.Values)
							{
								foreach (IDataElement dataElement in dictionary2.Values)
								{
									bool flag2 = false;
									foreach (StructDefinition structType in languageModelContainer.StructTypes)
									{
										if (structType._stName == keyValuePair.Key)
										{
											flag2 = true;
											break;
										}
									}
									if (languageModelContainer.lmNew != null)
									{
										ILMDataType[] dataTypes = languageModelContainer.lmNew.DataTypes;
										for (int i = 0; i < dataTypes.Length; i++)
										{
											if (dataTypes[i].Name == keyValuePair.Key)
											{
												flag2 = true;
												break;
											}
										}
									}
									if (!flag2)
									{
										(dataElement as DataElementBase).AddTypeDefs(keyValuePair.Key, languageModelContainer, true);
									}
									DeviceObjectHelper.ClearGlobalDataTypes(this._nProjectHandle, dataElement);
								}
							}
						}
						try
						{
							this.SetTaskLanguage(true);
							this._bRemoveLanguageModel = true;
							APEnvironment.LanguageModelMgr.PutLanguageModel(this, true);
						}
						finally
						{
							this.SetTaskLanguage(false);
							this._bRemoveLanguageModel = false;
						}
					}
					xmlWriter.WriteEndElement();
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 2, 2, 10) && app != null && logic != null)
				{
					LList<Parameter> llist = new LList<Parameter>();
					foreach (object obj2 in this._deviceParameterSet)
					{
						Parameter parameter = (Parameter)obj2;
						if (parameter.IsFunctional)
						{
							llist.Add(parameter);
						}
					}
					foreach (object obj3 in this.Connectors)
					{
						Connector connector2 = (Connector)obj3;
						if (!connector2.IsExplicit)
						{
							int num2;
							if (APEnvironment.ObjectMgr.IsLoadProjectFinished(this._nProjectHandle, out num2))
							{
								bool flag3 = false;
								bool flag4 = false;
								if (connector2.HostParameterSet.Count <= 0)
								{
									continue;
								}
								LateLanguageStartAddresses lateLanguageStartAddresses = new LateLanguageStartAddresses();
								foreach (Parameter item4 in DeviceObjectHelper.SortedParameterSet((IIoProvider)(object)connector2))
								{
									if (!flag3 && (int)item4.ChannelType == 1)
									{
										flag3 = true;
										lateLanguageStartAddresses.startInAddress = (item4.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses);
										DeviceObjectHelper.HashIecAddresses[connector2] = lateLanguageStartAddresses;
									}
									if (!flag4 && ((int)item4.ChannelType == 2 || (int)item4.ChannelType == 3))
									{
										flag4 = true;
										lateLanguageStartAddresses.startOutAddress = (item4.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses);
										DeviceObjectHelper.HashIecAddresses[connector2] = lateLanguageStartAddresses;
									}
									if (item4.IsFunctional)
									{
										llist.Add(item4);
									}
								}
								continue;
							}
							foreach (object obj5 in connector2.HostParameterSet)
							{
								Parameter parameter3 = (Parameter)obj5;
								if (parameter3.IsFunctional)
								{
									llist.Add(parameter3);
								}
							}
						}
					}
					if (llist.Count > 0 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 1, 0))
					{
						List<string> list = new List<string>();
						Guid objectGuid2 = app.MetaObject.ObjectGuid;
						ArrayList arrayList = new ArrayList();
						LDictionary<IParameterSection, StringBuilder> ldictionary = new LDictionary<IParameterSection, StringBuilder>();
						foreach (Parameter parameter4 in llist)
						{
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 3, 0))
							{
								IParameterSection section = parameter4.Section;
								if (section != null)
								{
									bool flag5;
									StringBuilder createFunctionalSections = this.GetCreateFunctionalSections(ldictionary, list, section, out flag5);
									this.AddFunctionalParameter(createFunctionalSections, parameter4);
								}
								if (parameter4.IsEnumeration && !string.IsNullOrEmpty(parameter4.ParamType))
								{
									string[] array = parameter4.ParamType.Split(new char[]
									{
										':'
									});
									string text = array[1];
									bool flag6 = false;
									foreach (StructDefinition item7 in arrayList)
									{
										if (item7._stName == text)
										{
											flag6 = true;
											break;
										}
									}
									if (!flag6)
									{
										ICompiledType compiledType = _3S.CoDeSys.DeviceObject.Types.ParseType(parameter4.BaseType);
										string text2 = text + "Enum";
										StringBuilder stringBuilder = new StringBuilder();
										stringBuilder.AppendLine("{attribute 'read_only'}");
										stringBuilder.AppendLine("{warning disable C0228}");
										stringBuilder.AppendFormat("{0} : {1};", array[1], text);
										stringBuilder.AppendLine();
										list.Add(stringBuilder.ToString());
										StringBuilder stringBuilder2 = new StringBuilder();
										stringBuilder2.AppendFormat("TYPE {0}:\nSTRUCT\n", text);
										foreach (EnumValue enumValue in parameter4.EnumerationValues)
										{
											stringBuilder2.AppendFormat("{0} : {1} := {2};", enumValue.Identifier, text2, enumValue.Value);
											stringBuilder2.AppendLine();
										}
										stringBuilder2.Append("END_STRUCT\nEND_TYPE\n");
										Guid guidStructDef = LanguageModelHelper.FindGuidStructure(text, objectGuid2);
										arrayList.Add(new StructDefinition(text, stringBuilder2.ToString(), guidStructDef, false));
										StringBuilder stringBuilder3 = new StringBuilder();
										stringBuilder3.AppendFormat("TYPE {0} : (", text2);
										bool flag7 = true;
										if (compiledType.Class != TypeClass.Bool)
										{
											foreach (EnumValue enumValue2 in parameter4.EnumerationValues)
											{
												if (!flag7)
												{
													stringBuilder3.Append(",");
												}
												flag7 = false;
												stringBuilder3.AppendFormat("{0} := {1}", enumValue2.Identifier, enumValue2.Value);
											}
										}
										else
										{
											foreach (EnumValue enumValue3 in parameter4.EnumerationValues)
											{
												bool flag8;
												if (bool.TryParse(enumValue3.Value, out flag8) && !flag8)
												{
													stringBuilder3.AppendFormat("{0}", enumValue3.Identifier);
													break;
												}
											}
											foreach (EnumValue enumValue4 in parameter4.EnumerationValues)
											{
												bool flag8;
												if (bool.TryParse(enumValue4.Value, out flag8) && flag8)
												{
													stringBuilder3.AppendFormat(", {0}", enumValue4.Identifier);
													break;
												}
											}
										}
										stringBuilder3.Append(") " + parameter4.BaseType + ";");
										stringBuilder3.Append("END_TYPE\n");
										Guid guidStructDef2 = LanguageModelHelper.FindGuidStructure(text2, objectGuid2);
										arrayList.Add(new StructDefinition(text2, stringBuilder3.ToString(), guidStructDef2, false));
									}
								}
								if (parameter4.HasBaseType && parameter4.Section == null)
								{
									StringBuilder stringBuilder4 = new StringBuilder();
									this.AddFunctionalParameter(stringBuilder4, parameter4);
									list.Add(stringBuilder4.ToString());
								}
							}
							else
							{
								StringBuilder stringBuilder5 = new StringBuilder();
								this.AddFunctionalParameter(stringBuilder5, parameter4);
								list.Add(stringBuilder5.ToString());
							}
						}
						foreach (KeyValuePair<IParameterSection, StringBuilder> keyValuePair3 in ldictionary)
						{
							string functionalSectionType = this.GetFunctionalSectionType(keyValuePair3.Key);
							keyValuePair3.Value.Append("END_STRUCT\nEND_TYPE\n");
							Guid guidStructDef3 = LanguageModelHelper.FindGuidStructure(functionalSectionType, objectGuid2);
							arrayList.Add(new StructDefinition(functionalSectionType, keyValuePair3.Value.ToString(), guidStructDef3, false));
						}
						Guid guidGvl = LanguageModelHelper.FindGuidStructure(this.MetaObject.Name, objectGuid2);
						xmlWriter.WriteStartElement("language-model");
						xmlWriter.WriteAttributeString("application-id", objectGuid2.ToString());
						xmlWriter.WriteAttributeString("plclogic-id", logic.MetaObject.ObjectGuid.ToString());
						xmlWriter.WriteAttributeString("object-id", guidGvl.ToString());
						LanguageModelHelper.AddGlobalVarListWithRetains(list, string.Empty, guidGvl, this.MetaObject.Name, xmlWriter, Guid.Empty, false, "{attribute 'qualified_only'}", codeTables, false);
						LanguageModelHelper.AddTypeDefs(arrayList, xmlWriter, this.MetaObject.ObjectGuid, codeTables);
						xmlWriter.WriteEndElement();
					}
				}
			}
			finally
			{
				DeviceObjectHelper.IsInLateLanguageModel = false;
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00028904 File Offset: 0x00027904
		private void AddIoLanguageModel(XmlWriter xmlWriter, ILanguageModelBuilder3 lmBuilder, ILanguageModel lmnew, IPlcLogicObject logic, IApplicationObject app, List<List<string>> codeTables, bool bCreateAdditionalParams, bool bMotorolaBitfield, bool bSkipAdditionalParamsForZeroParams, bool bLoadFinished, IDriverInfo4 driverInfo)
		{
			int num = 0;
			int num2 = 0;
			Guid objectGuid = this.MetaObject.ObjectGuid;
			IMetaObject metaObject = this.MetaObject;
			if (logic == null || app == null)
			{
				return;
			}
			LanguageModelHelper.ClearDiagnosisInstance();
			Guid objectGuid2 = logic.MetaObject.ObjectGuid;
			Guid objectGuid3 = app.MetaObject.ObjectGuid;
			Hashtable hashtable = new Hashtable();
			string text = "";
			DeviceLanguageModelEventArgsImpl deviceLanguageModelEventArgsImpl = new DeviceLanguageModelEventArgsImpl(this._nProjectHandle, objectGuid, objectGuid2, objectGuid3);
			APEnvironment.DeviceController.RaiseDeviceProvidingLanguageModel(deviceLanguageModelEventArgsImpl);
			if (deviceLanguageModelEventArgsImpl.LmContributions != null)
			{
				foreach (object obj in deviceLanguageModelEventArgsImpl.LmContributions)
				{
					string data = (string)obj;
					xmlWriter.WriteRaw(data);
				}
			}
			text = deviceLanguageModelEventArgsImpl.GetCompileMessages();
			bool bHideFbInstances = false;
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this.DeviceIdentificationNoSimulation);
			bHideFbInstances = LocalTargetSettings.HideIoFbInstances.GetBoolValue(targetSettingsById);
			LanguageModelContainer languageModelContainer = new LanguageModelContainer();
			if (lmnew != null && lmBuilder != null)
			{
				languageModelContainer.lmBuilder = lmBuilder;
				languageModelContainer.lmNew = lmnew;
				languageModelContainer.struinitModules = new List<IStructureInitialization>();
			}
			LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs = new LDictionary<IRequiredLib, IIoProvider>();
			LanguageModelHelper.AddModuleIoLanguageModel(this, null, "0", languageModelContainer, app.MetaObject, ref num, bCreateAdditionalParams, false, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
			foreach (KeyValuePair<IIoProvider, bool> keyValuePair in this.LogicalGVLProviders)
			{
				string stParentPointer = "0";
				IMetaObject metaObject2 = keyValuePair.Key.GetMetaObject();
				string text2;
				Guid logicalAppForDevice = LogicalIOHelper.GetLogicalAppForDevice(metaObject2.ProjectHandle, metaObject2.ObjectGuid, out text2);
				if (logicalAppForDevice != Guid.Empty)
				{
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(app.MetaObject.ProjectHandle, logicalAppForDevice);
					while (metaObjectStub.ParentObjectGuid != Guid.Empty)
					{
						metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(app.MetaObject.ProjectHandle, metaObjectStub.ParentObjectGuid);
						if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							break;
						}
					}
					int proxyModuleIndex = LogicalIOHelper.GetProxyModuleIndex(this, metaObjectStub.ObjectGuid);
					stParentPointer = string.Format("ADR(moduleList[{0}])", proxyModuleIndex);
				}
				if (keyValuePair.Value)
				{
					using (IEnumerator<IIoProvider> enumerator3 = DeviceObjectHelper.GetMappedIoProvider(keyValuePair.Key, true).GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							IIoProvider ioProvider = enumerator3.Current;
							LanguageModelHelper.AddModuleIoLanguageModel(ioProvider, stParentPointer, languageModelContainer, app.MetaObject, ref num, bCreateAdditionalParams, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
						}
						continue;
					}
				}
				LanguageModelHelper.AddModuleIoLanguageModel(keyValuePair.Key, stParentPointer, languageModelContainer, app.MetaObject, ref num, bCreateAdditionalParams, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
			}
			if (DeviceObjectHelper.AdditionalModules != null && DeviceObjectHelper.AdditionalModules.DeviceGuids != null)
			{
				foreach (Guid objectGuid4 in DeviceObjectHelper.AdditionalModules.DeviceGuids)
				{
					if (APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, objectGuid4))
					{
						IAdditionalModules additionalModules = APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, objectGuid4).Object as IAdditionalModules;
						if (additionalModules != null && languageModelContainer.lmBuilder != null && languageModelContainer.struinitModules != null)
						{
							IMetaObjectStub metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, objectGuid4);
							while (metaObjectStub2.ParentObjectGuid != Guid.Empty)
							{
								metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(app.MetaObject.ProjectHandle, metaObjectStub2.ParentObjectGuid);
								if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub2.ObjectType))
								{
									break;
								}
							}
							IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(this._nProjectHandle, metaObjectStub2.ObjectGuid);
							if (hostStub == null || !(hostStub.ObjectGuid != objectGuid))
							{
								int proxyModuleIndex2 = LogicalIOHelper.GetProxyModuleIndex(this, metaObjectStub2.ObjectGuid);
								string stExpression = string.Format("ADR(moduleList[{0}])", proxyModuleIndex2);
								uint num3 = 1U;
								foreach (IAdditionalModuleData additionalModuleData in additionalModules.AdditionalModules)
								{
									List<IAssignmentExpression> list = new List<IAssignmentExpression>();
									IVariableExpression expLeft = languageModelContainer.lmBuilder.CreateVariableExpression(null, "wType");
									IVariableExpression expLeft2 = languageModelContainer.lmBuilder.CreateVariableExpression(null, "dwFlags");
									IVariableExpression expLeft3 = languageModelContainer.lmBuilder.CreateVariableExpression(null, "dwNumOfParameters");
									IVariableExpression expLeft4 = languageModelContainer.lmBuilder.CreateVariableExpression(null, "pParameterList");
									IVariableExpression expLeft5 = languageModelContainer.lmBuilder.CreateVariableExpression(null, "pFather");
									ILiteralExpression expRight = languageModelContainer.lmBuilder.CreateLiteralExpression(null, (ulong)((long)additionalModuleData.ModuleType), TypeClass.Word);
									ILiteralExpression expRight2 = languageModelContainer.lmBuilder.CreateLiteralExpression(null, (ulong)num3, TypeClass.DWord);
									ILiteralExpression expRight3 = languageModelContainer.lmBuilder.CreateLiteralExpression(null, (ulong)((long)additionalModuleData.ParameterList.Count), TypeClass.DWord);
									IExpression expRight4 = languageModelContainer.lmBuilder.ParseExpression(string.Concat(new string[]
									{
										"ADR(",
										additionalModuleData.GVLInstance,
										".",
										additionalModuleData.GVLParameterInstance,
										")"
									}));
									IExpression expRight5 = languageModelContainer.lmBuilder.ParseExpression(stExpression);
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression(null, expLeft, expRight));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression(null, expLeft2, expRight2));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression(null, expLeft3, expRight3));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression(null, expLeft4, expRight4));
									list.Add(languageModelContainer.lmBuilder.CreateAssignmentExpression(null, expLeft5, expRight5));
									languageModelContainer.struinitModules.Add(languageModelContainer.lmBuilder.CreateStructureInitialisation(null, list));
									num++;
								}
							}
						}
					}
				}
			}
			bool flag = false;
			LList<IIoProvider> liIoProviders = new LList<IIoProvider>();
			ConnectorMapList connectorMapList = new ConnectorMapList();
			this.CollectMappings(this, connectorMapList, ref num2, this._driverInfo.PlcAlwaysMapping, this._driverInfo.PlcAlwaysMappingMode, hashtable, bMotorolaBitfield, liIoProviders, ref flag);
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 6, 0))
			{
				bool flag2 = false;
				DeviceObjectHelper.HasManualAddresses.TryGetValue(this._metaObject.ObjectGuid, out flag2);
				if ((flag || flag2) && this.Strategy is IAddressAssignmentStrategy3)
				{
					bool flag3 = (this.Strategy as IAddressAssignmentStrategy3).UpdateAddresses(liIoProviders, flag, flag2);
					DeviceObjectHelper.HasManualAddresses[this._metaObject.ObjectGuid] = flag3;
					if (flag3 || flag2)
					{
						connectorMapList = new ConnectorMapList();
						hashtable.Clear();
						this.CollectMappings(this, connectorMapList, ref num2, this._driverInfo.PlcAlwaysMapping, this._driverInfo.PlcAlwaysMappingMode, hashtable, bMotorolaBitfield, null, ref flag);
					}
				}
			}
			bool flag4;
			LanguageModelHelper.AddLibraries(this._metaObject.ObjectGuid, dictRequiredLibs, app.MetaObject, languageModelContainer, false, out flag4, this._driverInfo.EnableDiagnosis, this, bHideFbInstances, bLoadFinished);
			string arg;
			string arg2;
			if (!flag4 && !LanguageModelHelper.TryInsertLib(app.MetaObject, "IoStandard", "3.0.0.0", "System", out arg, out arg2, true, false, string.Empty))
			{
				languageModelContainer.AddCompilerMessage("error", "", 0L, objectGuid, string.Format(Strings.ErrorNoLib, arg, arg2));
			}
			xmlWriter.WriteStartElement("language-model");
			xmlWriter.WriteAttributeString("application-id", app.MetaObject.ObjectGuid.ToString());
			xmlWriter.WriteAttributeString("plclogic-id", logic.MetaObject.ObjectGuid.ToString());
			xmlWriter.WriteAttributeString("object-id", objectGuid.ToString());
			string item = string.Format("{{nobp}}{{messageguid '{0}'}}\n", app.MetaObject.ObjectGuid.ToString());
			List<string> list2 = null;
			ISequenceStatement sequenceStatement = null;
			ISequenceStatement seq = null;
			string text3 = null;
			string text4 = null;
			List<string> list3 = null;
			ISequenceStatement sequenceStatement2 = null;
			ILanguageModelBuilder3 languageModelBuilder = languageModelContainer.lmBuilder as ILanguageModelBuilder3;
			if (languageModelBuilder != null)
			{
				sequenceStatement = languageModelBuilder.CreateSequenceStatement(null);
				sequenceStatement2 = languageModelBuilder.CreateSequenceStatement(null);
				seq = languageModelBuilder.CreateSequenceStatement(null);
				IStatement state = languageModelBuilder.CreatePragmaStatement2(null, "attribute 'blobinit'");
				sequenceStatement.AddStatement(state);
				IStatement state2 = languageModelBuilder.CreatePragmaStatement2(null, "attribute 'hide'");
				sequenceStatement.AddStatement(state2);
				string stType = string.Format("ARRAY[0..{0}] OF IoConfigConnector", num);
				ICompiledType type = languageModelBuilder.ParseType(stType);
				List<IExpression> list4 = new List<IExpression>();
				foreach (IStructureInitialization item2 in languageModelContainer.struinitModules)
				{
					list4.Add(item2);
				}
				IArrayInitialization expInitial = languageModelBuilder.CreateArrayInitialisation(null, list4);
				IVariableDeclarationStatement state3 = languageModelBuilder.CreateVariableDeclarationStatement(null, "moduleList", type, expInitial, null);
				sequenceStatement.AddStatement(state3);
				LDictionary<Guid, string> ldictionary = new LDictionary<Guid, string>();
				ldictionary[app.MetaObject.ObjectGuid] = app.MetaObject.Name.ToUpperInvariant();
				DeviceObjectHelper.IoConfigPou[app.MetaObject.ObjectGuid] = languageModelBuilder.CreateSequenceStatement(null);
				if (app is IDeviceApplication)
				{
					foreach (Guid objectGuid5 in app.MetaObject.SubObjectGuids)
					{
						IMetaObjectStub metaObjectStub3 = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, objectGuid5);
						if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub3.ObjectType))
						{
							ldictionary[metaObjectStub3.ObjectGuid] = metaObjectStub3.Name.ToUpperInvariant();
							DeviceObjectHelper.IoConfigPou[metaObjectStub3.ObjectGuid] = languageModelBuilder.CreateSequenceStatement(null);
						}
					}
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
				{
					connectorMapList.GetMappedVariableDeclarations(seq, languageModelBuilder, ldictionary, app, sequenceStatement2, driverInfo);
				}
				else
				{
					connectorMapList.GetMappedVariableDeclarations(sequenceStatement, languageModelBuilder, ldictionary, app, sequenceStatement2, driverInfo);
				}
				using (LDictionary<Guid, string>.Enumerator enumerator7 = ldictionary.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						KeyValuePair<Guid, string> keyValuePair2 = enumerator7.Current;
						ISequenceStatement sequenceStatement3;
						if (DeviceObjectHelper.IoConfigPou.TryGetValue(keyValuePair2.Key, out sequenceStatement3))
						{
							Guid guid = Guid.Empty;
							if (app.MetaObject.ObjectGuid == keyValuePair2.Key)
							{
								guid = this._guidLmIoConfigErrorPou;
							}
							else
							{
								IPreCompileContext2 preCompileContext = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(keyValuePair2.Key);
								if (preCompileContext != null)
								{
									ISignature[] array = preCompileContext.FindSignature(PouDefinitions.ErrorPou_Name);
									if (array.Length != 0)
									{
										guid = array[0].ObjectGuid;
									}
								}
							}
							if (sequenceStatement3.Statements.Length != 0)
							{
								APEnvironment.LanguageModelMgr.PutLanguageModel(new IoConfigPou(keyValuePair2.Key, logic.MetaObject.ObjectGuid, objectGuid, guid, sequenceStatement3), true);
							}
							else if (guid != Guid.Empty)
							{
								APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(this._nProjectHandle, guid);
							}
						}
					}
					goto IL_B71;
				}
			}
			list2 = new List<string>();
			list2.Add(string.Format("{{attribute 'blobinit'}}{{attribute 'hide'}}\r\nmoduleList : ARRAY [0..{0}] OF IoConfigConnector := [", num));
			list2.AddRange(languageModelContainer.sbModuleList.StringList);
			list2.Add("];\n");
			list3 = connectorMapList.GetMappedVariableDeclarations(app.MetaObject.Name, out text3, out text4, false);
		IL_B71:
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 4, 1, 0))
			{
				Guid guid2 = Guid.Empty;
				IPreCompileContext2 preCompileContext2 = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(app.MetaObject.ObjectGuid);
				if (preCompileContext2 != null)
				{
					try
					{
						ISignature[] array2 = preCompileContext2.FindSignature(DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES);
						if (array2.Length != 0)
						{
							guid2 = array2[0].ObjectGuid;
						}
					}
					catch
					{
					}
				}
				if (this.DriverInfo is IDriverInfo7 && (this.DriverInfo as IDriverInfo7).CreateForceVariables)
				{
					string stAttributes = "{attribute 'global_init_slot' := '40100'}" + text3;
					int num4 = 0;
					List<string> forceVariableDeclarations = connectorMapList.GetForceVariableDeclarations(ref num4);
					if (guid2 == Guid.Empty)
					{
						guid2 = LanguageModelHelper.CreateDeterministicGuid(preCompileContext2.ApplicationGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES);
					}
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("\r\nIoConfig_Forces_Reset : BOOL;\r\n");
					forceVariableDeclarations.Add(stringBuilder.ToString());
					LanguageModelHelper.AddGlobalVarListWithRetains(forceVariableDeclarations, string.Empty, guid2, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES, xmlWriter, objectGuid, false, stAttributes, codeTables);
				}
				else if (guid2 != Guid.Empty)
				{
					APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(this._nProjectHandle, guid2);
				}
			}
			if (languageModelBuilder != null)
			{
				if (sequenceStatement.Statements.Length != 0)
				{
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
					{
						ISequenceStatement2 sequenceStatement4 = languageModelBuilder.CreateSequenceStatement(null);
						sequenceStatement4.AddStatement(languageModelBuilder.CreatePragmaStatement2(null, "attribute 'global_init_slot' := '39900'"));
						sequenceStatement4.AddStatement(languageModelBuilder.CreatePragmaStatement2(null, string.Concat(new string[]
						{
							"attribute '",
							CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG,
							"' := '",
							1073741824.ToString(),
							"'"
						})));
						if (this._guidLmIoConfigVarConfig == Guid.Empty)
						{
							IPreCompileContext2 preCompileContext3 = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(app.MetaObject.ObjectGuid);
							if (preCompileContext3 != null)
							{
								ISignature[] array3 = preCompileContext3.FindSignature(DeviceObject.GVL_IOCONFIG_GLOBALS_MODULELIST);
								if (array3.Length != 0)
								{
									this._guidLmIoConfigVarConfig = array3[0].ObjectGuid;
								}
								else
								{
									this._guidLmIoConfigVarConfig = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_MODULELIST);
								}
							}
						}
						ILMGlobVarlist ilmglobVarlist = languageModelBuilder.CreateGlobVarlist(DeviceObject.GVL_IOCONFIG_GLOBALS_MODULELIST, this._guidLmIoConfigVarConfig);
						IVariableDeclarationListStatement state4 = languageModelBuilder.CreateVariableDeclarationListStatement(null, VarFlag.Global, sequenceStatement);
						ISequenceStatement2 sequenceStatement5 = languageModelBuilder.CreateSequenceStatement(null);
						sequenceStatement5.AddStatement(sequenceStatement4);
						sequenceStatement5.AddStatement(state4);
						ilmglobVarlist.Interface = sequenceStatement5;
						ilmglobVarlist.ObjectGuid = objectGuid;
						ilmglobVarlist.InhibitOnlineChange = true;
						languageModelContainer.lmNew.AddGlobalVariableList(ilmglobVarlist);
					}
					ISequenceStatement2 sequenceStatement6 = languageModelBuilder.DuplicateExprement(sequenceStatement2) as ISequenceStatement2;
					if (sequenceStatement6.Statements.Length != 0)
					{
						sequenceStatement6.InsertStatement(0, languageModelBuilder.CreatePragmaStatement2(null, string.Concat(new string[]
						{
							"attribute '",
							CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG,
							"' := '",
							1073741824.ToString(),
							"'"
						})));
						sequenceStatement6.InsertStatement(0, languageModelBuilder.CreatePragmaStatement2(null, "attribute 'global_init_slot' := '40000'"));
					}
					else
					{
						sequenceStatement6.AddStatement(languageModelBuilder.CreatePragmaStatement2(null, "attribute 'global_init_slot' := '40000'"));
						sequenceStatement6.AddStatement(languageModelBuilder.CreatePragmaStatement2(null, string.Concat(new string[]
						{
							"attribute '",
							CompileAttributes.ATTRIBUTE_SIGNATURE_FLAG,
							"' := '",
							1073741824.ToString(),
							"'"
						})));
					}
					if (this._guidLmIoConfigGlobalsMapping == Guid.Empty)
					{
						IPreCompileContext2 preCompileContext4 = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(app.MetaObject.ObjectGuid);
						if (preCompileContext4 != null)
						{
							ISignature[] array4 = preCompileContext4.FindSignature(DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING);
							if (array4.Length != 0)
							{
								this._guidLmIoConfigGlobalsMapping = array4[0].ObjectGuid;
							}
							else
							{
								this._guidLmIoConfigGlobalsMapping = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING);
							}
						}
					}
					ILMGlobVarlist ilmglobVarlist2 = languageModelBuilder.CreateGlobVarlist(DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING, this._guidLmIoConfigGlobalsMapping);
					IVariableDeclarationListStatement state5;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
					{
						state5 = languageModelBuilder.CreateVariableDeclarationListStatement(null, VarFlag.Global, seq);
					}
					else
					{
						state5 = languageModelBuilder.CreateVariableDeclarationListStatement(null, VarFlag.Global, sequenceStatement);
					}
					ISequenceStatement2 sequenceStatement7 = languageModelBuilder.CreateSequenceStatement(null);
					sequenceStatement7.AddStatement(sequenceStatement6);
					sequenceStatement7.AddStatement(state5);
					ilmglobVarlist2.Interface = sequenceStatement7;
					ilmglobVarlist2.ObjectGuid = objectGuid;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
					{
						ilmglobVarlist2.InhibitOnlineChange = false;
					}
					else
					{
						ilmglobVarlist2.InhibitOnlineChange = true;
					}
					languageModelContainer.lmNew.AddGlobalVariableList(ilmglobVarlist2);
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
				{
					text3 = "{attribute 'global_init_slot' := '60000'}";
				}
				else
				{
					text3 = "{attribute 'global_init_slot' := '60000'}" + sequenceStatement2.ToString();
				}
				List<string> list5 = new List<string>();
				list5.AddRange(languageModelContainer.sbValues.StringList);
				list5.Add(languageModelContainer.sbMessages.ToString());
				list5.Add(item);
				list5.Add("{attribute 'hide'}\r\n bGlobalInitDone : BOOL := FALSE; \n");
				list5.Add("{attribute 'hide'}\r\n bIoConfigLateInitDone : BOOL := FALSE;\n");
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 3, 2, 10))
				{
					list5.Add(string.Format("{{attribute 'hide'}}\r\n bUpdateIoInStop : BOOL := {0};\n", this._driverInfo._bUpdateIOsInStop.ToString()));
					list5.Add(string.Format("{{attribute 'hide'}}\r\n bResetBehaviour : BYTE := {0};\n", ((byte)this._driverInfo._StopResetBehaviourSetting).ToString()));
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 4, 1, 0))
				{
					list5.Add("\r\n pIoConfigTaskMap: POINTER TO IoConfigTaskMap;\n");
					list5.Add("\r\n nIoConfigTaskMapCount: DINT;\n");
				}
				LanguageModelHelper.AddGlobalVarListWithRetains(list5, languageModelContainer.sbRetains.ToString(), this._guidLmIoConfigGlobals, DeviceObject.GVL_IOCONFIG_GLOBALS, xmlWriter, objectGuid, false, text3, codeTables);
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 10, 0) && app is IDeviceApplication)
				{
					LanguageModelHelper.AddDeviceApplicationExternalEventFb(xmlWriter, app.MetaObject.ObjectGuid);
				}
			}
			else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 2, 0, 4))
			{
				if ((list3 != null && list3.Count > 0) || (list2 != null && list2.Count > 0))
				{
					string stAttributes2 = "{attribute 'global_init_slot' := '40000'}" + text3;
					if (this._guidLmIoConfigGlobalsMapping == Guid.Empty)
					{
						IPreCompileContext2 preCompileContext5 = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(app.MetaObject.ObjectGuid);
						if (preCompileContext5 != null)
						{
							ISignature[] array5 = preCompileContext5.FindSignature(DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING);
							if (array5.Length != 0)
							{
								this._guidLmIoConfigGlobalsMapping = array5[0].ObjectGuid;
							}
							else
							{
								this._guidLmIoConfigGlobalsMapping = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING);
							}
						}
					}
					List<string> list6 = new List<string>();
					list6.AddRange(list2);
					list6.AddRange(list3);
					LanguageModelHelper.AddGlobalVarListWithRetains(list6, string.Empty, this._guidLmIoConfigGlobalsMapping, DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING, xmlWriter, objectGuid, false, stAttributes2, codeTables);
				}
				text3 = "{attribute 'global_init_slot' := '60000'}" + text3;
				List<string> list7 = new List<string>();
				list7.AddRange(languageModelContainer.sbValues.StringList);
				list7.Add(languageModelContainer.sbMessages.ToString());
				list7.Add(item);
				list7.Add("{attribute 'hide'}\r\n bGlobalInitDone : BOOL := FALSE; \n");
				list7.Add("{attribute 'hide'}\r\n bIoConfigLateInitDone : BOOL := FALSE;\n");
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 3, 2, 10))
				{
					list7.Add(string.Format("{{attribute 'hide'}}\r\n bUpdateIoInStop : BOOL := {0};\n", this._driverInfo._bUpdateIOsInStop.ToString()));
					list7.Add(string.Format("{{attribute 'hide'}}\r\n bResetBehaviour : BYTE := {0};\n", ((byte)this._driverInfo._StopResetBehaviourSetting).ToString()));
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 4, 1, 0))
				{
					list7.Add("\r\n pIoConfigTaskMap: POINTER TO IoConfigTaskMap;\n");
					list7.Add("\r\n nIoConfigTaskMapCount: DINT;\n");
				}
				LanguageModelHelper.AddGlobalVarListWithRetains(list7, languageModelContainer.sbRetains.ToString(), this._guidLmIoConfigGlobals, DeviceObject.GVL_IOCONFIG_GLOBALS, xmlWriter, objectGuid, false, text3, codeTables);
			}
			else
			{
				List<string> list8 = new List<string>();
				list8.AddRange(list2);
				list8.AddRange(list3);
				list8.AddRange(languageModelContainer.sbValues.StringList);
				list8.Add(languageModelContainer.sbMessages.ToString());
				list8.Add(item);
				list8.Add("{attribute 'hide'}\r\n bGlobalInitDone : BOOL := FALSE; \n");
				list8.Add("{attribute 'hide'}\r\n bIoConfigLateInitDone : BOOL := FALSE;\n");
				LanguageModelHelper.AddGlobalVarListWithRetains(list8, languageModelContainer.sbRetains.ToString(), this._guidLmIoConfigGlobals, DeviceObject.GVL_IOCONFIG_GLOBALS, xmlWriter, objectGuid, false, text3, null);
			}
			if (languageModelBuilder == null)
			{
				if (!string.IsNullOrEmpty(text4) || !string.IsNullOrEmpty(text) || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 3, 2, 0))
				{
					PouDefinitions.WriteIoConfigErrorPou(xmlWriter, this._guidLmIoConfigErrorPou, text4 + text);
				}
				else
				{
					APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(this._nProjectHandle, this._guidLmIoConfigErrorPou);
				}
			}
			xmlWriter.WriteEndElement();
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00029DD4 File Offset: 0x00028DD4
		public bool CheckLanguageModelGuids()
		{
			return this._guidLmIoConfigGlobals != Guid.Empty && this._guidLmIoConfigVarConfig != Guid.Empty && this._guidLmIoConfigErrorPou != Guid.Empty;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00029E0C File Offset: 0x00028E0C
		public void UpdateLanguageModelGuids(bool bUpgrade)
		{
			if (!bUpgrade)
			{
				LList<Guid> llist = new LList<Guid>();
				if (this._metaObject != null && this._metaObject.ParentObjectGuid == Guid.Empty)
				{
					llist.Add(this._guidLmIoConfigGlobals);
					llist.Add(this._guidLmIoConfigVarConfig);
					llist.Add(this._guidLmIoConfigErrorPou);
					llist.Add(this._guidLmIoConfigGlobalsMapping);
				}
				llist.Add(this._deviceParameterSet.LanguageModelGuid);
				foreach (object obj in this.Connectors)
				{
					Connector connector = (Connector)obj;
					llist.Add(((ParameterSet)connector.ParameterSet).LanguageModelGuid);
				}
				if (llist.IndexOf(Guid.Empty) == -1 && !DeviceObjectHelper.CheckLanguageModelGuids(llist))
				{
					bUpgrade = true;
				}
			}
			if (!bUpgrade || this._guidLmIoConfigGlobals == Guid.Empty)
			{
				this._guidLmIoConfigGlobals = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS);
			}
			if (!bUpgrade || this._guidLmIoConfigVarConfig == Guid.Empty)
			{
				this._guidLmIoConfigVarConfig = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_MODULELIST);
			}
			if (!bUpgrade || this._guidLmIoConfigErrorPou == Guid.Empty)
			{
				this._guidLmIoConfigErrorPou = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, PouDefinitions.ErrorPou_Name);
			}
			if (!bUpgrade || this._guidLmIoConfigGlobalsMapping == Guid.Empty)
			{
				this._guidLmIoConfigGlobalsMapping = LanguageModelHelper.CreateDeterministicGuid(this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_MAPPING);
			}
			this._deviceParameterSet.UpdateLanguageModelGuids(bUpgrade);
			foreach (object obj2 in this.Connectors)
			{
				((ParameterSet)((Connector)obj2).ParameterSet).UpdateLanguageModelGuids(bUpgrade);
			}
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0002A018 File Offset: 0x00029018
		internal void UpdateAllIoProviders()
		{
			this.UpdateIoProvider(this);
			foreach (object obj in this.Connectors)
			{
				Connector connector = (Connector)obj;
				if (!connector.IsExplicit)
				{
					this.UpdateIoProvider(connector);
				}
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0002A080 File Offset: 0x00029080
		private static void GetAllTasksNotExcluded(IMetaObject mo, ref List<ITaskObject> alTasks)
		{
			if (mo.SubObjectGuids == null || mo.SubObjectGuids.Length == 0)
			{
				return;
			}
			foreach (Guid objectGuid in mo.SubObjectGuids)
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(mo.ProjectHandle, objectGuid);
				if (metaObjectStub != null)
				{
					if (typeof(ITaskConfigObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						ITaskConfigObject taskConfigObject = APEnvironment.ObjectMgr.GetObjectToRead(mo.ProjectHandle, metaObjectStub.ObjectGuid).Object as ITaskConfigObject;
						if (taskConfigObject != null)
						{
							foreach (ITaskObject taskObject in taskConfigObject.Tasks)
							{
								if (!APEnvironment.LanguageModelMgr.IsExcludedFromBuild(taskObject.MetaObject.ProjectHandle, taskObject.MetaObject.ObjectGuid))
								{
									alTasks.Add(taskObject);
								}
							}
						}
					}
					else if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IPlcLogicObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(mo.ProjectHandle, metaObjectStub.ObjectGuid);
						if (objectToRead != null)
						{
							DeviceObject.GetAllTasksNotExcluded(objectToRead, ref alTasks);
						}
					}
				}
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x0002A1C0 File Offset: 0x000291C0
		internal static void CreateTasks(IIoProvider ioprovider)
		{
			bool flag = false;
			IObject @object;
			if (ioprovider == null)
			{
				@object = null;
			}
			else
			{
				IMetaObject metaObject = ioprovider.GetMetaObject();
				@object = ((metaObject != null) ? metaObject.Object : null);
			}
			IObject object2 = @object;
			if (object2 is DeviceObject && (object2 as DeviceObject).IsInUpdate)
			{
				flag = true;
			}
			if (object2 is SlotDeviceObject && (object2 as SlotDeviceObject).IsInUpdate)
			{
				flag = true;
			}
			IMetaObject application = ioprovider.GetApplication();
			if (application == null && ioprovider is DeviceObject)
			{
				foreach (object obj in (ioprovider as DeviceObject).Connectors)
				{
					application = ((IIoProvider)obj).GetApplication();
					if (application != null)
					{
						break;
					}
				}
			}
			IList requiredTasks = ((DriverInfo)ioprovider.DriverInfo).RequiredTasks;
			if (requiredTasks != null && requiredTasks.Count > 0)
			{
				IMetaObject metaObject2 = (application == null) ? null : DeviceObjectHelper.GetTaskConfig(application);
				if (application != null && metaObject2 == null)
				{
					try
					{
						ITaskConfigObject obj2 = APEnvironment.CreateTaskConfigObject();
						APEnvironment.ObjectMgr.AddObject(application.ProjectHandle, application.ObjectGuid, Guid.NewGuid(), obj2, "Task configuration", -1);
						metaObject2 = DeviceObjectHelper.GetTaskConfig(application);
					}
					catch
					{
					}
				}
				if (metaObject2 == null)
				{
					if (ioprovider is DeviceObject)
					{
						DeviceObject deviceObject = ioprovider as DeviceObject;
						if (deviceObject.MetaObject != null && deviceObject.MetaObject.ParentObjectGuid == Guid.Empty)
						{
							return;
						}
					}
					if (ioprovider is IConnector)
					{
						IMetaObject metaObject3 = ioprovider.GetMetaObject();
						if (metaObject3 != null && metaObject3.ParentObjectGuid == Guid.Empty)
						{
							return;
						}
					}
					DeviceMessage message = new DeviceMessage(string.Format(APEnvironment.Engine.ResourceManager.GetString(typeof(Strings), "ErrorNoTaskconfig"), (application == null) ? "" : application.Name), Severity.Error);
					APEnvironment.MessageStorage.AddMessage(DeviceMessageCategory.Instance, message);
					return;
				}
				bool flag2 = false;
				int num = 0;
				LList<int> llist = new LList<int>();
				IDeviceObject host = ioprovider.GetHost();
				if (host != null)
				{
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById((host as IDeviceObject5).DeviceIdentificationNoSimulation);
					flag2 = LocalTargetSettings.ProhibitDuplicatePriorities.GetBoolValue(targetSettingsById);
					num = LocalTargetSettings.MaxTaskPriority.GetIntValue(targetSettingsById);
					if (flag2)
					{
						List<ITaskObject> list = new List<ITaskObject>();
						DeviceObject.GetAllTasksNotExcluded(application, ref list);
						using (List<ITaskObject>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								int item;
								if (int.TryParse(enumerator2.Current.Priority.Trim(), out item))
								{
									llist.Add(item);
								}
							}
						}
					}
				}
				foreach (object obj3 in requiredTasks)
				{
					RequiredTask requiredTask = (RequiredTask)obj3;
					if ((!requiredTask.DeviceApplicationOnly || application.Object is IDeviceApplication) && (requiredTask.Implicit || !flag))
					{
						string text = requiredTask.TaskName;
						if (text.Contains("$(DeviceName)") && ioprovider != null)
						{
							IMetaObject metaObject4 = ioprovider.GetMetaObject();
							if (metaObject4 != null)
							{
								text = text.Replace("$(DeviceName)", metaObject4.Name);
							}
						}
						IMetaObject task = DeviceObjectHelper.GetTask(metaObject2, text);
						if (task != null)
						{
							ITaskObject taskObject = task.Object as ITaskObject;
							DeviceObject.CreateTaskPou(requiredTask.TaskPou, ioprovider, taskObject);
						}
						else
						{
							if (requiredTask is RequiredCyclicTask)
							{
								try
								{
									RequiredCyclicTask requiredCyclicTask = (RequiredCyclicTask)requiredTask;
									ITaskObject taskObject2 = APEnvironment.CreateTaskObject();
									taskObject2.Interval = requiredCyclicTask.CycleTime;
									taskObject2.KindOf = KindOfTask.Cyclic;
									if (flag2)
									{
										int num2 = requiredCyclicTask.Priority;
										while (llist.Contains(num2) && num2 < num)
										{
											num2++;
										}
										taskObject2.Priority = num2.ToString();
									}
									else
									{
										taskObject2.Priority = requiredCyclicTask.Priority.ToString();
									}
									taskObject2.Watchdog.Enabled = requiredCyclicTask.WatchdogEnabled;
									if (taskObject2.Watchdog.Enabled)
									{
										taskObject2.Watchdog.Sensitivity = requiredCyclicTask.WatchdogSensitivity;
										taskObject2.Watchdog.Time = requiredCyclicTask.WatchdogTime;
										taskObject2.Watchdog.TimeUnit = requiredCyclicTask.WatchdogTimeUnit;
									}
									DeviceObject.CreateTaskPou(requiredCyclicTask.TaskPou, ioprovider, taskObject2);
									ITaskObject2 taskObject3 = taskObject2 as ITaskObject2;
									if (taskObject3 != null)
									{
										taskObject3.Implicit = requiredCyclicTask.Implicit;
										taskObject3.WithinSPSTimeSlicing = requiredCyclicTask.WithinSPSTimeSlicing;
									}
									ITaskObject6 taskObject4 = taskObject2 as ITaskObject6;
									if (taskObject4 != null)
									{
										taskObject4.AllowEmpty = true;
									}
									APEnvironment.ObjectMgr.AddObject(metaObject2.ProjectHandle, metaObject2.ObjectGuid, Guid.NewGuid(), taskObject2, text, -1);
									DeviceMessage message2 = new DeviceMessage(string.Format(APEnvironment.Engine.ResourceManager.GetString(typeof(Strings), "InfoTaskCreated"), text), Severity.Information);
									APEnvironment.MessageStorage.AddMessage(DeviceMessageCategory.Instance, message2);
									continue;
								}
								catch (Exception ex)
								{
									DeviceMessage message3 = new DeviceMessage(string.Format(APEnvironment.Engine.ResourceManager.GetString(typeof(Strings), "ErrorCreateTaskFailure"), text, ex.Message), Severity.Information);
									APEnvironment.MessageStorage.AddMessage(DeviceMessageCategory.Instance, message3);
									continue;
								}
							}
							if (requiredTask is RequiredExternalEventTask)
							{
								try
								{
									RequiredExternalEventTask requiredExternalEventTask = (RequiredExternalEventTask)requiredTask;
									ITaskObject taskObject5 = APEnvironment.CreateTaskObject();
									taskObject5.ExternalEvent = requiredExternalEventTask.Event;
									taskObject5.KindOf = KindOfTask.ExternalEvent;
									if (flag2)
									{
										int num3 = requiredExternalEventTask.Priority;
										while (llist.Contains(num3) && num3 < num)
										{
											num3++;
										}
										taskObject5.Priority = num3.ToString();
									}
									else
									{
										taskObject5.Priority = requiredExternalEventTask.Priority.ToString();
									}
									if (!string.IsNullOrEmpty(requiredExternalEventTask.CycleTime))
									{
										taskObject5.Interval = requiredExternalEventTask.CycleTime;
									}
									taskObject5.Watchdog.Enabled = requiredExternalEventTask.WatchdogEnabled;
									if (taskObject5.Watchdog.Enabled)
									{
										taskObject5.Watchdog.Sensitivity = requiredExternalEventTask.WatchdogSensitivity;
										taskObject5.Watchdog.Time = requiredExternalEventTask.WatchdogTime;
										taskObject5.Watchdog.TimeUnit = requiredExternalEventTask.WatchdogTimeUnit;
									}
									DeviceObject.CreateTaskPou(requiredExternalEventTask.TaskPou, ioprovider, taskObject5);
									ITaskObject2 taskObject6 = taskObject5 as ITaskObject2;
									if (taskObject6 != null)
									{
										taskObject6.Implicit = requiredExternalEventTask.Implicit;
										taskObject6.WithinSPSTimeSlicing = requiredExternalEventTask.WithinSPSTimeSlicing;
									}
									ITaskObject6 taskObject7 = taskObject5 as ITaskObject6;
									if (taskObject7 != null)
									{
										taskObject7.AllowEmpty = true;
									}
									APEnvironment.ObjectMgr.AddObject(metaObject2.ProjectHandle, metaObject2.ObjectGuid, Guid.NewGuid(), taskObject5, text, -1);
									DeviceMessage message4 = new DeviceMessage(string.Format(APEnvironment.Engine.ResourceManager.GetString(typeof(Strings), "InfoTaskCreated"), text), Severity.Information);
									APEnvironment.MessageStorage.AddMessage(DeviceMessageCategory.Instance, message4);
								}
								catch (Exception ex2)
								{
									DeviceMessage message5 = new DeviceMessage(string.Format(APEnvironment.Engine.ResourceManager.GetString(typeof(Strings), "ErrorCreateTaskFailure"), text, ex2.Message), Severity.Information);
									APEnvironment.MessageStorage.AddMessage(DeviceMessageCategory.Instance, message5);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0002A970 File Offset: 0x00029970
		internal static void CreateTaskPou(ArrayList taskPou, IIoProvider ioprovider, ITaskObject taskObject)
		{
			foreach (object obj in taskPou)
			{
				string text = (string)obj;
				string text2 = text;
				if (text.Contains("$(DeviceName)") && ioprovider != null)
				{
					IMetaObject metaObject = ioprovider.GetMetaObject();
					if (metaObject != null)
					{
						text2 = text.Replace("$(DeviceName)", metaObject.Name);
					}
				}
				bool flag = true;
				foreach (IPouObject item2 in (IEnumerable)taskObject.POUs)
				{
					if (item2.Name == text2)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					if (taskObject.MetaObject != null && !taskObject.MetaObject.IsToModify)
					{
						IMetaObject metaObject2 = null;
						try
						{
							metaObject2 = APEnvironment.ObjectMgr.GetObjectToModify(taskObject.MetaObject);
							if (metaObject2 != null)
							{
								taskObject = (metaObject2.Object as ITaskObject);
								IPouObject pouObject = taskObject.CreatePOU(text2);
								if (pouObject != null)
								{
									taskObject.POUs.Add(pouObject);
								}
							}
							continue;
						}
						catch
						{
							continue;
						}
						finally
						{
							if (metaObject2 != null && metaObject2.IsToModify)
							{
								APEnvironment.ObjectMgr.SetObject(metaObject2, true, null);
							}
						}
					}
					IPouObject pouObject2 = taskObject.CreatePOU(text2);
					if (pouObject2 != null)
					{
						taskObject.POUs.Add(pouObject2);
					}
				}
			}
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0002AB34 File Offset: 0x00029B34
		internal void UpdateIoProvider(IIoProvider ioprovider)
		{
			DeviceObject.CreateTasks(ioprovider);
			IMetaObject application = ioprovider.GetApplication();
			IMetaObject metaObject = this.GetMetaObject();
			foreach (object obj in ioprovider.DriverInfo.RequiredLibs)
			{
				RequiredLib requiredLib = (RequiredLib)obj;
				if (!requiredLib.IsDiagnosisLib)
				{
					foreach (object obj2 in requiredLib.FbInstances)
					{
						((FBInstance)obj2).ResetInstanceName(metaObject, application, ioprovider.DriverInfo.RequiredLibs, ioprovider);
					}
				}
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0002AC08 File Offset: 0x00029C08
		public void OnAfterCreated()
		{
			Debug.Assert(this.MetaObject != null && this.MetaObject.IsToModify);
			IMetaObject plcLogic = this.GetPlcLogic();
			if (plcLogic != null)
			{
				foreach (Guid guid in plcLogic.SubObjectGuids)
				{
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogic.ProjectHandle, guid);
					if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						this._driverInfo.IoApplication = guid;
						break;
					}
				}
			}
			this.GetLanguageModel();
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0002AC97 File Offset: 0x00029C97
		public void OnAfterAdded()
		{
			this._bPastePrepared = false;
			if (this._metaObject == null || !this._metaObject.IsToModify)
			{
				Debug.Fail("This device object is not modifiable");
				throw new InvalidOperationException("This device object is not modifiable");
			}
			this.UpdateLanguageModelGuids(false);
			this.UpdateAllIoProviders();
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0002ACD8 File Offset: 0x00029CD8
		public void OnRenamed(string stOldDeviceName)
		{
			Debug.Assert(this.MetaObject != null);
			Debug.Assert(this.MetaObject.IsToModify);
			IMetaObject application = this.GetApplication();
			IMetaObject metaObject = this.MetaObject;
			this._driverInfo.OnDeviceRenamed(application, metaObject, stOldDeviceName);
			foreach (object obj in this._connectors)
			{
				((Connector)obj).OnDeviceRenamed(stOldDeviceName);
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0002AD6C File Offset: 0x00029D6C
		public void PreparePaste(bool bOnlyChildConnectors = false)
		{
			if (!this._bPastePrepared)
			{
				foreach (object obj in this.Connectors)
				{
					Connector connector = (Connector)obj;
					if (!bOnlyChildConnectors || connector.ConnectorRole == ConnectorRole.Child)
					{
						connector.PreparePaste();
					}
				}
				this._bPastePrepared = true;
			}
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0002ADE0 File Offset: 0x00029DE0
		public void UpdatePasteModuleGuid(Guid oldGuid, Guid newGuid)
		{
			foreach (object obj in this.Connectors)
			{
				((Connector)obj).UpdatePasteModuleGuid(oldGuid, newGuid);
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0002AE38 File Offset: 0x00029E38
		public string GetLanguageModel()
		{
			return this.GetLanguageModelDevice(null, null, null);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0002AE43 File Offset: 0x00029E43
		public string GetLanguageModel2(out List<List<string>> codeTables)
		{
			return this.GetLanguageModel2(null, null, out codeTables);
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0002AE4E File Offset: 0x00029E4E
		internal string GetLanguageModel2(ILanguageModelBuilder lmb, ILanguageModel lmnew, out List<List<string>> codeTables)
		{
			codeTables = null;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 3, 2, 0))
			{
				codeTables = new List<List<string>>();
			}
			return this.GetLanguageModelDevice(lmb as ILanguageModelBuilder3, lmnew, codeTables);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0002AE7C File Offset: 0x00029E7C
		private string GetLanguageModelDevice(ILanguageModelBuilder3 lmb, ILanguageModel lmnew, List<List<string>> codeTables)
		{
			if (DeviceObjectHelper.IsInListLanguageModel(this._guidObject) && (this._metaObject == null || this._metaObject.ParentObjectGuid != Guid.Empty))
			{
				return string.Empty;
			}
			if (!this.CheckLanguageModelGuids())
			{
				return string.Empty;
			}
			if (this is LogicalIODevice && this.DeviceIdentificationNoSimulation.Type != 152)
			{
				return string.Empty;
			}
			IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
			if (this._metaObject == null || primaryProject == null || primaryProject.Handle != this._metaObject.ProjectHandle)
			{
				return string.Empty;
			}
			if (this._bTaskLanguageModel)
			{
				return this.AddTaskLanguageModel(this.MetaObject.ProjectHandle, codeTables);
			}
			int num;
			if (!APEnvironment.ObjectMgr.IsLoadProjectFinished(this._nProjectHandle, out num))
			{
				bool flag = false;
				if (this._metaObject.Object is ILogicalDevice)
				{
					ILogicalDevice logicalDevice = this._metaObject.Object as ILogicalDevice;
					if (logicalDevice.IsPhysical)
					{
						foreach (IMappedDevice item in (IEnumerable)logicalDevice.MappedDevices)
						{
							if (!string.IsNullOrEmpty(item.MappedDevice))
							{
								flag = true;
								break;
							}
						}
					}
					else
					{
						flag = true;
					}
				}
			IL_143:
				if (flag || this._metaObject.ParentObjectGuid == Guid.Empty)
				{
					DeviceObjectHelper.AddObjectsToUpdate(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
					return string.Empty;
				}
			}
			if (this != null && ((ILogicalDevice)this).IsLogical)
			{
				foreach (object obj in ((ILogicalDevice)this).MappedDevices)
				{
					LogicalMappedDevice logicalMappedDevice = (LogicalMappedDevice)obj;
					if (logicalMappedDevice.IsMapped)
					{
						Guid getMappedDevice = logicalMappedDevice.GetMappedDevice;
						if (Guid.Empty != getMappedDevice && APEnvironment.ObjectMgr.ExistsObject(this._metaObject.ProjectHandle, getMappedDevice))
						{
							IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._metaObject.ProjectHandle, getMappedDevice);
							if (objectToRead.Object is LogicalExchangeGVLObject && (objectToRead.Object as LogicalExchangeGVLObject).UseCombinedType)
							{
								return string.Empty;
							}
						}
					}
				}
			}
			bool afterLoadFinished = DeviceObjectHelper.AfterLoadFinished;
			if (!DeviceObjectHelper.BeginGetLanguageModel(this.MetaObject.ObjectGuid))
			{
				return string.Empty;
			}
			string result;
			try
			{
				IApplicationObject applicationObject = null;
				IPlcLogicObject plcLogicObject = this.GetPlcLogicObject();
				if (plcLogicObject != null)
				{
					applicationObject = this.GetDeviceApplicationObject(plcLogicObject);
					if (applicationObject == null)
					{
						applicationObject = this.GetApplicationObject(plcLogicObject);
					}
				}
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteStartElement("language-model-list");
				if (plcLogicObject != null)
				{
					xmlTextWriter.WriteStartElement("language-model");
					xmlTextWriter.WriteAttributeString("plclogic-id", XmlConvert.ToString(plcLogicObject.MetaObject.ObjectGuid));
					xmlTextWriter.WriteStartElement("device");
					xmlTextWriter.WriteAttributeString("plclogic", this.MetaObject.Name);
					xmlTextWriter.WriteStartElement("device-identification");
					xmlTextWriter.WriteAttributeString("type", this._deviceId.Type.ToString());
					xmlTextWriter.WriteAttributeString("target-id", this._deviceId.Id);
					xmlTextWriter.WriteAttributeString("version", this._deviceId.Version);
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
				}
				bool bCreateAdditionalParams = false;
				bool bMotorolaBitfield = false;
				bool bSkipAdditionalParamsForZeroParams = false;
				IDeviceObject deviceObject = this.GetHostDeviceObject();
				IDriverInfo4 driverInfo = null;
				if (deviceObject != null)
				{
					DeviceObject deviceObject2 = deviceObject as DeviceObject;
					if (deviceObject2 == null)
					{
						SlotDeviceObject slotDeviceObject = deviceObject as SlotDeviceObject;
						deviceObject2 = ((slotDeviceObject != null) ? slotDeviceObject.GetDevice() : null);
					}
					if (deviceObject2 != null && deviceObject2.UseParentPLC)
					{
						IMetaObjectStub plcDevice = APEnvironment.DeviceMgr.GetPlcDevice(deviceObject2.ProjectHandle, deviceObject2.MetaObject.ParentObjectGuid);
						if (plcDevice != null)
						{
							IObject @object = APEnvironment.ObjectMgr.GetObjectToRead(deviceObject2.ProjectHandle, plcDevice.ObjectGuid).Object;
							if (@object is IDeviceObject)
							{
								deviceObject = (@object as IDeviceObject);
							}
						}
					}
					bCreateAdditionalParams = DeviceObjectHelper.EnableAdditionalParameters(deviceObject);
					bSkipAdditionalParamsForZeroParams = DeviceObjectHelper.SkipAdditionalParametersForEmptyConnectors(deviceObject);
					bMotorolaBitfield = DeviceObjectHelper.MotorolaBitfields(deviceObject);
					driverInfo = ((deviceObject as IDeviceObject2).DriverInfo as IDriverInfo4);
				}
				if (!this.NoIoDownload)
				{
					this.AddLocalLanguageModel(lmb, lmnew, xmlTextWriter, plcLogicObject, applicationObject, codeTables, bCreateAdditionalParams, false, bSkipAdditionalParamsForZeroParams);
					this.AddIoLanguageModel(xmlTextWriter, lmb, lmnew, plcLogicObject, applicationObject, codeTables, bCreateAdditionalParams, bMotorolaBitfield, bSkipAdditionalParamsForZeroParams, afterLoadFinished, driverInfo);
				}
				else
				{
					if (plcLogicObject == null && this._metaObject.ParentObjectGuid == Guid.Empty)
					{
						DeviceObjectHelper.FillIecAddresstable((IDeviceObject)(object)this);
					}
					if (applicationObject != null)
					{
						this.AddLocalLanguageModel(lmb, lmnew, xmlTextWriter, plcLogicObject, applicationObject, codeTables, bCreateAdditionalParams, true, bSkipAdditionalParamsForZeroParams);
						LanguageModelContainer lmcontainer = new LanguageModelContainer();
						int num2 = 0;
						LDictionary<IRequiredLib, IIoProvider> dictRequiredLibs = new LDictionary<IRequiredLib, IIoProvider>();
						LanguageModelHelper.AddModuleIoLanguageModel(this, "0", lmcontainer, applicationObject.MetaObject, ref num2, bCreateAdditionalParams, dictRequiredLibs, bSkipAdditionalParamsForZeroParams);
						foreach (Guid objectGuid in applicationObject.MetaObject.SubObjectGuids)
						{
							IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, objectGuid);
							if (typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								LanguageModelHelper.CollectLibsForLogicalIO(this._nProjectHandle, dictRequiredLibs, metaObjectStub.SubObjectGuids);
							}
						}
						LanguageModelHelper.AddLibraries(this._metaObject.ObjectGuid, dictRequiredLibs, applicationObject.MetaObject, lmcontainer, false, afterLoadFinished);
					}
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Close();
				result = stringWriter.ToString();
			}
			finally
			{
				if (this._metaObject != null && this._metaObject.ParentObjectGuid != Guid.Empty)
				{
					APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
				}
				DeviceObjectHelper.EndGetLanguageModel(this.MetaObject.ObjectGuid);
			}
			return result;
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0002B4AC File Offset: 0x0002A4AC
		internal void CollectMappings(IIoProvider ioProvider, ConnectorMapList connectorMapList, ref int nNumModules, bool bPlcAlwaysMapping, AlwaysMappingMode PlcMappingMode, Hashtable htStartAddresses, bool bMotorolaBitfield, LList<IIoProvider> liIoProviders, ref bool bHasManualAddress)
		{
			nNumModules++;
			IMetaObject metaObject = ioProvider.GetMetaObject();
			ConnectorMap connectorMap = new ConnectorMap(new MappingInfo(ioProvider, nNumModules - 1), ioProvider.ParameterSet, string.Format("ADR(moduleList[{0}])", nNumModules - 1), metaObject.ProjectHandle, metaObject.ObjectGuid, metaObject.Name, -1, false);
			bool flag = bPlcAlwaysMapping;
			AlwaysMappingMode alwaysMappingMode = PlcMappingMode;
			if (typeof(IConnector6).IsAssignableFrom(ioProvider.GetType()))
			{
				IConnector6 connector = (IConnector6)ioProvider;
				flag |= connector.AlwaysMapping;
				if (connector.AlwaysMapping)
				{
					alwaysMappingMode = (connector as IConnector11).AlwaysMappingMode;
				}
			}
			bool flag2 = false;
			if (DeviceObjectHelper.GenerateCodeForLogicalDevices && DeviceObjectHelper.GetMappedIoProvider(ioProvider, false).Count > 0 && (!(metaObject.Object is ILogicalDevice2) || !(metaObject.Object as ILogicalDevice2).MappingPossible))
			{
				flag2 = true;
			}
			if (!flag2)
			{
				bool flag3 = false;
				bool flag4 = false;
				if (ioProvider.ParameterSet.Count > 0)
				{
					IEnumerable enumerable = DeviceObjectHelper.SortedParameterSet(ioProvider);
					string stBaseName = ioProvider.GetParamsListName();
					foreach (object obj in enumerable)
					{
						Parameter parameter = (Parameter)obj;
						if (parameter.ChannelType != ChannelType.None)
						{
							if (htStartAddresses != null)
							{
								if (!flag3 && parameter.ChannelType == ChannelType.Input)
								{
									flag3 = true;
									LateLanguageStartAddresses lateLanguageStartAddresses;
									if (htStartAddresses.ContainsKey(ioProvider))
									{
										lateLanguageStartAddresses = (htStartAddresses[ioProvider] as LateLanguageStartAddresses);
									}
									else
									{
										lateLanguageStartAddresses = new LateLanguageStartAddresses();
										htStartAddresses.Add(ioProvider, lateLanguageStartAddresses);
									}
									lateLanguageStartAddresses.startInAddress = (parameter.IoMapping as IoMapping).GetIecAddress(htStartAddresses);
									DeviceObjectHelper.HashIecAddresses[ioProvider] = lateLanguageStartAddresses;
								}
								if (!flag4 && (parameter.ChannelType == ChannelType.Output || parameter.ChannelType == ChannelType.OutputReadOnly))
								{
									flag4 = true;
									LateLanguageStartAddresses lateLanguageStartAddresses2;
									if (htStartAddresses.ContainsKey(ioProvider))
									{
										lateLanguageStartAddresses2 = (htStartAddresses[ioProvider] as LateLanguageStartAddresses);
									}
									else
									{
										lateLanguageStartAddresses2 = new LateLanguageStartAddresses();
										htStartAddresses.Add(ioProvider, lateLanguageStartAddresses2);
									}
									lateLanguageStartAddresses2.startOutAddress = (parameter.IoMapping as IoMapping).GetIecAddress(htStartAddresses);
									DeviceObjectHelper.HashIecAddresses[ioProvider] = lateLanguageStartAddresses2;
								}
							}
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 9, 0) && parameter.HasSubElements && parameter.Owner != null)
							{
								DeviceObject deviceObject = null;
								if (parameter.DataElementBase is DataElementStructType && !(parameter.DataElementBase as DataElementStructType).HasIecType)
								{
									deviceObject = (parameter.Owner.IoProvider.GetHost() as DeviceObject);
								}
								if (parameter.DataElementBase is DataElementUnionType && !(parameter.DataElementBase as DataElementUnionType).HasIecType)
								{
									deviceObject = (parameter.Owner.IoProvider.GetHost() as DeviceObject);
								}
								if (deviceObject != null && (ioProvider.ParameterSet as ParameterSet).Device != null)
								{
									IDeviceIdentification deviceIdentificationNoSimulation = ((ioProvider.ParameterSet as ParameterSet).Device as IDeviceObject5).DeviceIdentificationNoSimulation;
									string str = LanguageModelHelper.CalcDataElementHashCode(parameter.DataElementBase).ToString("X");
									stBaseName = deviceIdentificationNoSimulation.Type.ToString() + "_" + str;
								}
							}
							parameter.AddMapping(connectorMap, stBaseName, flag, alwaysMappingMode, null, null, htStartAddresses, bMotorolaBitfield);
							if (!parameter.IoMapping.AutomaticIecAddress)
							{
								bHasManualAddress = true;
							}
						}
					}
				}
			}
			if (liIoProviders != null && ioProvider.ParameterSet.Count > 0)
			{
				liIoProviders.Add(ioProvider);
			}
			connectorMapList.Add(connectorMap);
			if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
			{
				flag = bPlcAlwaysMapping;
			}
			foreach (IIoProvider ioProvider2 in ioProvider.Children)
			{
				this.CollectMappings(ioProvider2, connectorMapList, ref nNumModules, flag, alwaysMappingMode, htStartAddresses, bMotorolaBitfield, liIoProviders, ref bHasManualAddress);
			}
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0002B884 File Offset: 0x0002A884
		public void SetTaskLanguage(bool bEnable)
		{
			this._bTaskLanguageModel = bEnable;
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0002B890 File Offset: 0x0002A890
		private string GetBeforeAfterPouName(DeviceObject.CYCLICCALLS iType)
		{
			string arg = string.Empty;
			switch (iType)
			{
				case DeviceObject.CYCLICCALLS.BeforeReadInputs:
					arg = "BeforeReadInputs";
					break;
				case DeviceObject.CYCLICCALLS.AfterReadInput:
					arg = "AfterReadInput";
					break;
				case DeviceObject.CYCLICCALLS.BeforeWriteOutputs:
					arg = "BeforeWriteOutputs";
					break;
				case DeviceObject.CYCLICCALLS.AfterWriteOutputs:
					arg = "AfterWriteOutputs";
					break;
				default:
					return string.Empty;
			}
			return string.Format("__IoConfig{0}_{1}_{{0}}", arg, this._guidObject.ToString().Replace("-", "_"));
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0002B90C File Offset: 0x0002A90C
		private Guid GetBeforeAfterPou(string stPouName, IPreCompileContext2 precomcon)
		{
			Guid result;
			try
			{
				ISignature[] array = precomcon.FindSignature(stPouName.ToUpperInvariant());
				Guid guid = Guid.Empty;
				if (array.Length != 0)
				{
					guid = array[0].ObjectGuid;
				}
				else
				{
					guid = LanguageModelHelper.CreateDeterministicGuid(precomcon.ApplicationGuid, stPouName);
				}
				result = guid;
			}
			catch
			{
				result = LanguageModelHelper.CreateDeterministicGuid(precomcon.ApplicationGuid, stPouName);
			}
			return result;
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x0002B970 File Offset: 0x0002A970
		private static bool GetTaskCycleTime(Guid taskGuid, out KindOfTask eType, out long lDuration)
		{
			lDuration = 0L;
			eType = KindOfTask.Cyclic;
			IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return false;
			}
			int handle = primaryProject.Handle;
			IConverterFromIEC converterFromIEC = APEnvironment.LanguageModelMgr.GetConverterFromIEC();
			if (APEnvironment.ObjectMgr.ExistsObject(handle, taskGuid))
			{
				IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(handle, taskGuid);
				if (objectToRead != null && objectToRead.Object is ITaskObject)
				{
					ITaskObject taskObject = objectToRead.Object as ITaskObject;
					IMetaObject objectToRead2 = APEnvironment.ObjectMgr.GetObjectToRead(handle, objectToRead.ParentObjectGuid);
					if (objectToRead2 != null && objectToRead2.Object is ITaskConfigObject)
					{
						ITaskChecker4 taskChecker = (objectToRead2.Object as ITaskConfigObject).Checker as ITaskChecker4;
						TimeSpan timeSpan;
						string text;
						if (taskChecker != null && taskChecker.HasKnownInterval(taskObject) && taskChecker.EvaluateTaskInterval(taskObject, out timeSpan, out text))
						{
							lDuration = timeSpan.Ticks / 10L;
							return true;
						}
					}
					eType = taskObject.KindOf;
					if (taskObject.KindOf == KindOfTask.Freewheeling)
					{
						long.TryParse(taskObject.Priority, out lDuration);
						return true;
					}
					if (taskObject.KindOf == KindOfTask.Cyclic)
					{
						try
						{
							if (taskObject.Interval.ToLowerInvariant().StartsWith("t#"))
							{
								lDuration = converterFromIEC.GetDuration(taskObject.Interval) * 1000L;
							}
							else
							{
								object value;
								TypeClass typeClass;
								converterFromIEC.GetInteger(taskObject.Interval, out value, out typeClass);
								lDuration = Convert.ToInt64(value);
								if (taskObject.IntervalUnit == "µs")
								{
									lDuration = lDuration;
								}
								if (taskObject.IntervalUnit == "ms")
								{
									lDuration *= 1000L;
								}
							}
						}
						catch
						{
						}
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0002BB24 File Offset: 0x0002AB24
		protected int GetBusTask(ITaskInfo[] taskinfos)
		{
			return DeviceObject.GetBusTask(taskinfos, this._driverInfo);
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0002BB34 File Offset: 0x0002AB34
		internal static int GetBusTaskChild(LDictionary<Guid, string> dictAllTask, ITaskInfo[] taskinfos, DriverInfo driverInfo)
		{
			if (driverInfo != null && taskinfos != null)
			{
				for (int i = 0; i < taskinfos.Length; i++)
				{
					if (taskinfos[i].TaskGuid == driverInfo.BusCycleTaskGuid || taskinfos[i].TaskName == driverInfo.BusCycleTask)
					{
						return i;
					}
				}
				if (driverInfo.BusCycleTaskGuid != Guid.Empty || !string.IsNullOrEmpty(driverInfo.BusCycleTask))
				{
					if (driverInfo.BusCycleTaskGuid != Guid.Empty)
					{
						if (dictAllTask.Keys.Contains(driverInfo.BusCycleTaskGuid))
						{
							return -1;
						}
					}
					else if (dictAllTask.Values.Contains(driverInfo.BusCycleTask))
					{
						return -1;
					}
				}
			}
			return -2;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0002BBE4 File Offset: 0x0002ABE4
		internal static int GetBusTask(ITaskInfo[] taskinfos, DriverInfo driverInfo)
		{
			int result = 0;
			bool flag = false;
			try
			{
				if (driverInfo != null && taskinfos != null)
				{
					for (int i = 0; i < taskinfos.Length; i++)
					{
						if (taskinfos[i].TaskGuid == driverInfo.BusCycleTaskGuid || taskinfos[i].TaskName == driverInfo.BusCycleTask)
						{
							return i;
						}
					}
					flag = driverInfo.UseSlowestTask;
				}
				long num = long.MaxValue;
				long num2 = long.MinValue;
				bool flag2 = false;
				bool flag3 = false;
				for (int j = 0; j < taskinfos.Length; j++)
				{
					KindOfTask kindOfTask;
					long num3;
					if (DeviceObject.GetTaskCycleTime(taskinfos[j].TaskGuid, out kindOfTask, out num3))
					{
						if (kindOfTask == KindOfTask.Freewheeling)
						{
							flag2 = true;
						}
						else
						{
							flag3 = true;
							if (!flag)
							{
								if (num3 < num)
								{
									num = num3;
									result = j;
								}
							}
							else if (num3 > num2)
							{
								num2 = num3;
								result = j;
							}
						}
					}
				}
				if (flag2 && !flag3)
				{
					for (int k = 0; k < taskinfos.Length; k++)
					{
						KindOfTask kindOfTask2;
						long num4;
						if (DeviceObject.GetTaskCycleTime(taskinfos[k].TaskGuid, out kindOfTask2, out num4) && kindOfTask2 == KindOfTask.Freewheeling)
						{
							if (!flag)
							{
								if (num4 < num)
								{
									num = num4;
									result = k;
								}
							}
							else if (num4 > num2)
							{
								num2 = num4;
								result = k;
							}
						}
					}
				}
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0002BD24 File Offset: 0x0002AD24
		internal static int GetBusTaskConfigMode(ITaskInfo[] taskinfos, DriverInfo driverInfo)
		{
			int result = -2;
			try
			{
				if (driverInfo != null && taskinfos != null)
				{
					for (int i = 0; i < taskinfos.Length; i++)
					{
						string text = taskinfos[i].TaskName;
						int num = text.IndexOf('_');
						if (num > 0)
						{
							text = text.Substring(num + 1);
						}
						if (text == driverInfo.BusCycleTask)
						{
							return i;
						}
					}
				}
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0002BD94 File Offset: 0x0002AD94
		public string AddTaskLanguageModel(int nProjectHandle, List<List<string>> codeTables)
		{
			if (this.NoIoDownload)
			{
				return string.Empty;
			}
			int num = 0;
			IPlcLogicObject plcLogicObject = this.GetPlcLogicObject();
			if (plcLogicObject == null)
			{
				return string.Empty;
			}
			IApplicationObject applicationObject = this.GetApplicationObject(plcLogicObject);
			if (applicationObject == null)
			{
				return string.Empty;
			}
			if (applicationObject != null && plcLogicObject != null)
			{
				IPreCompileContext2 preCompileContext = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(applicationObject.MetaObject.ObjectGuid);
				if (preCompileContext != null)
				{
					StringWriter stringWriter = new StringWriter();
					XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
					Guid objectGuid = this.MetaObject.ObjectGuid;
					xmlTextWriter.WriteStartElement("language-model");
					xmlTextWriter.WriteAttributeString("application-id", applicationObject.MetaObject.ObjectGuid.ToString());
					xmlTextWriter.WriteAttributeString("plclogic-id", plcLogicObject.MetaObject.ObjectGuid.ToString());
					xmlTextWriter.WriteAttributeString("object-id", objectGuid.ToString());
					ITaskInfo[] allTasks = preCompileContext.AllTasks;
					StringWriter stringWriter2 = new StringWriter();
					XmlWriter xmlWriter = new XmlTextWriter(stringWriter2);
					LateLanguageModel lateLanguageModel = new LateLanguageModel(allTasks);
					lateLanguageModel.CyclicCalls.Init(allTasks, this.GetBusTask(allTasks));
					LDictionary<Guid, string> ldictionary = new LDictionary<Guid, string>();
					DeviceObjectHelper.CollectObjectGuids(ldictionary, nProjectHandle, this._metaObject.SubObjectGuids, typeof(ITaskObject), true);
					LanguageModelHelper.FillLanguageModel(ldictionary, this, lateLanguageModel, ref num, preCompileContext, this._driverInfo.UpdateIOsInStop);
					bool flag = false;
					string text = string.Empty;
					for (int i = 0; i < allTasks.Length; i++)
					{
						if (APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, allTasks[i].TaskGuid))
						{
							IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, allTasks[i].TaskGuid);
							if (objectToRead != null && objectToRead.Object is ITaskObject)
							{
								ITaskObject taskObject = (ITaskObject)objectToRead.Object;
								if (taskObject.KindOf == KindOfTask.Cyclic || taskObject.KindOf == KindOfTask.Freewheeling || taskObject.KindOf == KindOfTask.ExternalEvent)
								{
									flag = true;
								}
							}
						}
						else
						{
							flag = true;
						}
					}
					if (!flag && allTasks.Length != 0)
					{
						text = "{error '" + Strings.ErrorOnlyEventTasks + "' show_compile}";
					}
					if (this._iLastNumberOfTasks > allTasks.Length)
					{
						for (int j = allTasks.Length; j < this._iLastNumberOfTasks; j++)
						{
							for (int k = 0; k < 6; k++)
							{
								string beforeAfterPouName = this.GetBeforeAfterPouName((DeviceObject.CYCLICCALLS)k);
								if (beforeAfterPouName != string.Empty)
								{
									string text2 = string.Format(beforeAfterPouName, j * 2);
									foreach (ISignature signature in preCompileContext.FindSignature(text2.ToUpperInvariant()))
									{
										APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature.ObjectGuid);
									}
								}
							}
						}
					}
					this._iLastNumberOfTasks = allTasks.Length;
					for (int m = 0; m < allTasks.Length; m++)
					{
						string text3 = lateLanguageModel.CyclicCalls.GetCalls(m, "BeforeReadInputs");
						string text4 = lateLanguageModel.CyclicCalls.GetCalls(m, "AfterReadInputs");
						string text5 = lateLanguageModel.CyclicCalls.GetCalls(m, "BeforeWriteOutputs");
						string text6 = lateLanguageModel.CyclicCalls.GetCalls(m, "AfterWriteOutputs");
						if (!this._driverInfo.UpdateIOsInStop && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
						{
							string str = "IF NOT bAppHalted OR udiState = 2 and udiOldState <> 2 OR (udOpState AND 16#20) <>  0  OR (udOpState AND 16#10000) <>  0 THEN\n\r";
							string str2 = "IF NOT bAppHalted THEN\n\r";
							string str3 = "END_IF\n\r";
							if (!string.IsNullOrEmpty(text3))
							{
								text3 = str2 + text3 + str3;
							}
							if (!string.IsNullOrEmpty(text4))
							{
								text4 = str2 + text4 + str3;
							}
							if (!string.IsNullOrEmpty(text5))
							{
								text5 = str + text5 + str3;
							}
							if (!string.IsNullOrEmpty(text6))
							{
								text6 = str + text6 + str3;
							}
						}
						string format = "\r\n{{implicit}}\r\nFUNCTION {0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpapplicationinfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n\tbAppHalted : BOOL;\r\nEND_VAR\r\n";
						string format2 = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nbAppHalted := (pApplicationInfo^.udState = 2) OR (pApplicationInfo^.udState = 3);\r\n\r\n{0}\r\n";
						string format3 = "\r\n{{implicit}}\r\nFUNCTION {0} : BOOL\r\nVAR_INPUT\r\n\tptaskinfo: POINTER TO _IMPLICIT_TASK_INFO;\r\n\tpapplicationinfo: POINTER TO _IMPLICIT_APPLICATION_INFO;\r\nEND_VAR\r\nVAR\r\n\tbAppHalted : BOOL;\r\n    udiState : UDINT;\r\n    udOpState : UDINT;\r\nEND_VAR\r\nVAR_STAT\r\n\tudiOldState: UDINT := 2;\r\nEND_VAR\r\n";
						string format4 = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState    := pApplicationInfo^.udOpState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nbAppHalted := (udiState = 2) OR (udiState = 3);\r\n{0}\r\nudiOldState := udiState;\r\n";
						string format5 = "\r\n{{IF defined (IoConfigLateInit)}}\r\nIF NOT bIoConfigLateInitDone THEN\r\n\tRETURN;\r\nEND_IF\r\n{{END_IF}}\r\n\r\nIF pApplicationInfo <> 0 THEN\r\n    udiState := pApplicationInfo^.udState;\r\n    udOpState    := pApplicationInfo^.udOpState;\r\nELSE\r\n    (* Reset *)\r\n    udiState := 2;\r\n    udiOldState := 0;\r\nEND_IF\r\n(* 2 = STOP, 3 = HALT_ON_BP *)\r\nbAppHalted := (udiState = 2);\r\n{0}\r\nudiOldState := udiState;\r\n";
						string text7 = string.Format(this.GetBeforeAfterPouName(DeviceObject.CYCLICCALLS.BeforeReadInputs), m * 2);
						if (text3 != string.Empty)
						{
							XmlAttribute[] attributes = new XmlAttribute[]
							{
								new XmlAttribute("slot", "99"),
								new XmlAttribute("task-id", allTasks[m].TaskGuid.ToString())
							};
							Guid beforeAfterPou = this.GetBeforeAfterPou(text7, preCompileContext);
							string stInterface;
							string stBody;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 7, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody = string.Format(format5, text3);
							}
							else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody = string.Format(format4, text3);
							}
							else
							{
								stInterface = string.Format(format, text7);
								stBody = string.Format(format2, text3);
							}
							PouDefinitions.WritePou(xmlWriter, beforeAfterPou, text7, stInterface, stBody, attributes);
						}
						else
						{
							foreach (ISignature signature2 in preCompileContext.FindSignature(text7.ToUpperInvariant()))
							{
								APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature2.ObjectGuid);
							}
						}
						text7 = string.Format(this.GetBeforeAfterPouName(DeviceObject.CYCLICCALLS.AfterReadInput), m * 2);
						if (text4 != string.Empty)
						{
							XmlAttribute[] attributes2 = new XmlAttribute[]
							{
								new XmlAttribute("slot", "101"),
								new XmlAttribute("task-id", allTasks[m].TaskGuid.ToString())
							};
							Guid beforeAfterPou2 = this.GetBeforeAfterPou(text7, preCompileContext);
							string stInterface;
							string stBody2;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 7, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody2 = string.Format(format5, text4);
							}
							else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody2 = string.Format(format4, text4);
							}
							else
							{
								stInterface = string.Format(format, text7);
								stBody2 = string.Format(format2, text4);
							}
							PouDefinitions.WritePou(xmlWriter, beforeAfterPou2, text7, stInterface, stBody2, attributes2);
						}
						else
						{
							foreach (ISignature signature3 in preCompileContext.FindSignature(text7.ToUpperInvariant()))
							{
								APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature3.ObjectGuid);
							}
						}
						text7 = string.Format(this.GetBeforeAfterPouName(DeviceObject.CYCLICCALLS.BeforeWriteOutputs), m * 2);
						if (text5 != string.Empty)
						{
							XmlAttribute[] attributes3 = new XmlAttribute[]
							{
								new XmlAttribute("slot", "59998"),
								new XmlAttribute("task-id", allTasks[m].TaskGuid.ToString())
							};
							Guid beforeAfterPou3 = this.GetBeforeAfterPou(text7, preCompileContext);
							string stInterface;
							string stBody3;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 7, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody3 = string.Format(format5, text5);
							}
							else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody3 = string.Format(format4, text5);
							}
							else
							{
								stInterface = string.Format(format, text7);
								stBody3 = string.Format(format2, text5);
							}
							PouDefinitions.WritePou(xmlWriter, beforeAfterPou3, text7, stInterface, stBody3, attributes3);
						}
						else
						{
							foreach (ISignature signature4 in preCompileContext.FindSignature(text7.ToUpperInvariant()))
							{
								APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature4.ObjectGuid);
							}
						}
						text7 = string.Format(this.GetBeforeAfterPouName(DeviceObject.CYCLICCALLS.AfterWriteOutputs), m * 2);
						if (text6 != string.Empty)
						{
							XmlAttribute[] attributes4 = new XmlAttribute[]
							{
								new XmlAttribute("slot", "60001"),
								new XmlAttribute("task-id", allTasks[m].TaskGuid.ToString())
							};
							Guid beforeAfterPou4 = this.GetBeforeAfterPou(text7, preCompileContext);
							string stInterface;
							string stBody4;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 7, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody4 = string.Format(format5, text6);
							}
							else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 5, 0))
							{
								stInterface = string.Format(format3, text7);
								stBody4 = string.Format(format4, text6);
							}
							else
							{
								stInterface = string.Format(format, text7);
								stBody4 = string.Format(format2, text6);
							}
							PouDefinitions.WritePou(xmlWriter, beforeAfterPou4, text7, stInterface, stBody4, attributes4);
						}
						else
						{
							foreach (ISignature signature5 in preCompileContext.FindSignature(text7.ToUpperInvariant()))
							{
								APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature5.ObjectGuid);
							}
						}
					}
					string text8 = "IoConfig_TaskError";
					if (text != string.Empty || (this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.ExecuteProgram && this._driverInfo.StopResetBehaviourUserProgram != string.Empty))
					{
						string format6 = "\r\n{{attribute 'signature_flag' := '1073741824'}}\r\n{{implicit}}\r\nPROGRAM {0}\r\nVAR\r\n    bDummy: BOOL := FALSE;\r\nEND_VAR\r\n";
						string format7 = "\r\n{0}\r\nIF bDummy THEN\r\n{1} // never call this function, just fake the optimizer to use this program\r\nEND_IF\r\n";
						Guid beforeAfterPou5 = this.GetBeforeAfterPou(text8, preCompileContext);
						string stInterface2 = string.Format(format6, text8);
						string arg = string.Empty;
						if (this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.ExecuteProgram && this._driverInfo.StopResetBehaviourUserProgram != string.Empty)
						{
							long num2 = PositionHelper.CombinePosition(this._driverInfo.GetPositionId(2), 0);
							string str4 = string.Empty;
							if (num2 != -1L)
							{
								str4 = "{p " + num2 + "}";
							}
							arg = str4 + string.Format("{{messageguid '{0}'}}\n", this._metaObject.ObjectGuid) + this._driverInfo.StopResetBehaviourUserProgram + "();";
						}
						string stBody5 = string.Format(format7, text, arg);
						PouDefinitions.WritePou(xmlTextWriter, beforeAfterPou5, text8, stInterface2, stBody5, new XmlAttribute[0]);
					}
					else
					{
						try
						{
							foreach (ISignature signature6 in preCompileContext.FindSignature(text8.ToUpperInvariant()))
							{
								APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature6.ObjectGuid);
							}
						}
						catch
						{
						}
					}
					xmlWriter.Close();
					stringWriter2.Close();
					xmlTextWriter.WriteRaw(stringWriter2.ToString());
					xmlTextWriter.WriteEndElement();
					if (this._bRemoveLanguageModel)
					{
						APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
					}
					return stringWriter.ToString();
				}
			}
			return string.Empty;
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0002C7E4 File Offset: 0x0002B7E4
		public List<IIOTaskUsage> GetTaskMappings(Guid appGuid)
		{
			List<IIOTaskUsage> list = null;
			if (this.AllowsDirectCommunication)
			{
				DeviceObject.ApplicationUsage applicationUsage;
				this._dictAppUsage.TryGetValue(appGuid, out applicationUsage);
				if (applicationUsage == null)
				{
					ICompileContext6 compileContext = APEnvironment.LanguageModelMgr.GetCompileContext(appGuid) as ICompileContext6;
					if (compileContext == null)
					{
						return null;
					}
					if (compileContext.GetMemoryManager(0) == null)
					{
						bool flag = false;
						IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, appGuid);
						while (metaObjectStub.ParentObjectGuid != Guid.Empty)
						{
							if (typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								flag = true;
								break;
							}
							metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, metaObjectStub.ParentObjectGuid);
						}
						if (!flag)
						{
							return null;
						}
					}
					applicationUsage = new DeviceObject.ApplicationUsage();
					this._dictAppUsage.Add(appGuid, applicationUsage);
					this.AddLateLanguageModel(this.MetaObject.ProjectHandle, new AddLanguageModelEventArgs(appGuid, null));
				}
				if (applicationUsage != null)
				{
					list = new List<IIOTaskUsage>();
					foreach (DeviceObject.TaskUsage taskUsage in applicationUsage.TaskUsage.Values)
					{
						list.AddRange(taskUsage.IoUsage);
					}
					list.Sort(new Comparison<IIOTaskUsage>(DeviceObject.CompareTaskUsage));
				}
			}
			return list;
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0002C930 File Offset: 0x0002B930
		internal void FillTaskUsageList(TaskMapList taskmaplist, LDictionary<Guid, DeviceObject.TaskUsage> dictTasks, LateLanguageModel lmLate, ITaskInfo taskInfo, ITaskInfo[] taskinfos)
		{
			foreach (KeyValuePair<ITaskMappingInfo, List<ITaskMapping>> keyValuePair in taskmaplist.Mappings)
			{
				ConnectorMap connectorMap = lmLate.ConnectorMapList.ConnectorMaps[keyValuePair.Key.ModuleIndex];
				foreach (ITaskMapping taskMapping in keyValuePair.Value)
				{
					Mapping mapping = (Mapping)taskMapping;
					if (mapping.DataElement != null && mapping.DataElement is DataElementBase)
					{
						Parameter parameter = (mapping.DataElement as DataElementBase).GetParameter();
						if (parameter != null)
						{
							IoTaskUsage ioTaskUsage = new IoTaskUsage(connectorMap.ObjectGuid, parameter, mapping);
							ioTaskUsage.UsedTasks.Add(taskInfo);
							ioTaskUsage.ConnectorIndex = (uint)connectorMap.MappingInfo.ModuleIndex;
							ioTaskUsage.ParameterIndex = (long)((ulong)mapping.ParameterId);
							if (connectorMap.BusTask >= 0)
							{
								ioTaskUsage.BusTask = taskinfos[connectorMap.BusTask];
							}
							if (mapping.MapToExisiting)
							{
								ioTaskUsage.MappedAddress = mapping.ExistingVar;
							}
							else if (mapping.BitSize == 1 && !mapping.IecVar.Contains("X"))
							{
								ioTaskUsage.MappedAddress = mapping.DataElement.IoMapping.IecAddress;
							}
							else
							{
								ioTaskUsage.MappedAddress = mapping.IecVar;
							}
							IDeviceObject9 deviceObject = parameter.GetAssociatedDeviceObject as IDeviceObject9;
							if (deviceObject != null && deviceObject.ShowParamsInDevDescOrder)
							{
								ioTaskUsage.ParameterIndex = parameter.IndexInDevDesc;
							}
							bool flag = false;
							int key = ioTaskUsage.DataElement.GetHashCode() ^ ioTaskUsage.MappedAddress.GetHashCode();
							DeviceObject.TaskUsage taskUsage;
							IIOTaskUsage iiotaskUsage;
							if (!dictTasks.TryGetValue(ioTaskUsage.ObjectGuid, out taskUsage))
							{
								taskUsage = new DeviceObject.TaskUsage();
								dictTasks.Add(ioTaskUsage.ObjectGuid, taskUsage);
							}
							else if (taskUsage.DataElementUsage.TryGetValue(key, out iiotaskUsage))
							{
								iiotaskUsage.UsedTasks.Add(taskInfo);
								continue;
							}
							if (!flag)
							{
								taskUsage.IoUsage.Add(ioTaskUsage);
								taskUsage.DataElementUsage[key] = ioTaskUsage;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0002CBA4 File Offset: 0x0002BBA4
		private static int CompareTaskUsage(IIOTaskUsage x, IIOTaskUsage y)
		{
			IoTaskUsage ioTaskUsage = x as IoTaskUsage;
			IoTaskUsage ioTaskUsage2 = y as IoTaskUsage;
			if (ioTaskUsage == null || ioTaskUsage2 == null)
			{
				return 0;
			}
			int num = ioTaskUsage.ConnectorIndex.CompareTo(ioTaskUsage2.ConnectorIndex);
			if (num != 0)
			{
				return num;
			}
			num = ioTaskUsage.ParameterIndex.CompareTo(ioTaskUsage2.ParameterIndex);
			if (num != 0)
			{
				return num;
			}
			return ioTaskUsage.Mapping.BitOffset.CompareTo(ioTaskUsage2.Mapping.BitOffset);
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0002CC20 File Offset: 0x0002BC20
		public void AddLateLanguageModel(int nProjectHandle, AddLanguageModelEventArgs e)
		{
			if (this.NoIoDownload)
			{
				return;
			}
			if (this is HiddenAndTransientDeviceObject)
			{
				return;
			}
			bool isInLateLanguageModel = DeviceObjectHelper.IsInLateLanguageModel;
			try
			{
				DeviceObjectHelper.IsInLateLanguageModel = true;
				DeviceObjectHelper.IoProviderChildrens.Clear();
				Hashtable htStartAddresses = new Hashtable();
				int num = 0;
				IPlcLogicObject plcLogicObject = this.GetPlcLogicObject();
				if (plcLogicObject != null)
				{
					bool flag = false;
					IApplicationObject deviceApplicationObject = this.GetDeviceApplicationObject(plcLogicObject);
					IApplicationObject applicationObject = null;
					if (deviceApplicationObject != null)
					{
						if (deviceApplicationObject.MetaObject.ObjectGuid == e.ApplicationGuid)
						{
							applicationObject = deviceApplicationObject;
							flag = true;
						}
						else
						{
							IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogicObject.MetaObject.ProjectHandle, e.ApplicationGuid);
							if (metaObjectStub.ParentObjectGuid == deviceApplicationObject.MetaObject.ObjectGuid)
							{
								applicationObject = (APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid).Object as IApplicationObject);
							}
						}
					}
					if (applicationObject == null)
					{
						applicationObject = this.GetApplicationObject(plcLogicObject);
					}
					if (applicationObject != null)
					{
						LDictionary<Guid, string> ldictionary = new LDictionary<Guid, string>();
						DeviceObjectHelper.CollectObjectGuids(ldictionary, nProjectHandle, this._metaObject.SubObjectGuids, typeof(ITaskObject), true);
						string text = string.Empty;
						bool flag2 = false;
						Guid guid = Guid.Empty;
						if (DeviceObjectHelper.GenerateCodeForLogicalDevices && this._metaObject != null)
						{
							IMetaObjectStub metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, e.ApplicationGuid);
							foreach (IObjectProperty objectProperty in metaObjectStub2.Properties)
							{
								if (objectProperty is ILogicalApplicationProperty)
								{
									StringBuilder stringBuilder = new StringBuilder();
									stringBuilder.Append("'");
									IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, e.ApplicationGuid);
									if (objectToRead.Object is IOnlineApplicationObject2)
									{
										stringBuilder.Append((objectToRead.Object as IOnlineApplicationObject2).ApplicationName);
									}
									while (metaObjectStub2.ParentObjectGuid != Guid.Empty)
									{
										metaObjectStub2 = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, metaObjectStub2.ParentObjectGuid);
										if (typeof(IOnlineApplicationObject2).IsAssignableFrom(metaObjectStub2.ObjectType))
										{
											IMetaObject objectToRead2 = APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, metaObjectStub2.ObjectGuid);
											stringBuilder.Insert(1, (objectToRead2.Object as IOnlineApplicationObject2).ApplicationName + ".");
										}
									}
									stringBuilder.Append("'");
									text = stringBuilder.ToString();
									applicationObject = (objectToRead.Object as IApplicationObject);
									flag2 = true;
									guid = (objectProperty as ILogicalApplicationProperty).LogicalApplication;
								}
							}
						}
						if (!(applicationObject.MetaObject.ObjectGuid != e.ApplicationGuid))
						{
							ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this.DeviceIdentificationNoSimulation);
							bool boolValue = LocalTargetSettings.CycleControlInIec.GetBoolValue(targetSettingsById);
							bool boolValue2 = LocalTargetSettings.MotorolaBitfields.GetBoolValue(targetSettingsById);
							bool flag3 = LocalTargetSettings.ShowMultipleTaskMappingsAsError.GetBoolValue(targetSettingsById);
							flag3 |= this._driverInfo.PlcCreateWarningsAsErros;
							bool boolValue3 = LocalTargetSettings.MapAlwaysIecAddress.GetBoolValue(targetSettingsById);
							string applicationName = DeviceObjectHelper.GetApplicationName(applicationObject);
							MemoryStream memoryStream = new MemoryStream();
							XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.Unicode);
							xmlTextWriter.WriteStartElement("language-model");
							xmlTextWriter.WriteAttributeString("application-id", applicationObject.MetaObject.ObjectGuid.ToString());
							xmlTextWriter.WriteAttributeString("plclogic-id", plcLogicObject.MetaObject.ObjectGuid.ToString());
							ICompileContext compileContext = APEnvironment.LanguageModelMgr.GetCompileContext(e.ApplicationGuid);
							Debug.Assert(compileContext != null);
							ICompileContext referenceContextIfAvailable = APEnvironment.LanguageModelMgr.GetReferenceContextIfAvailable(e.ApplicationGuid);
							ITaskInfo[] allTasks = compileContext.AllTasks;
							long num2 = 0L;
							long num3 = 0L;
							int num4 = 1000;
							int num5 = 1000;
							if (compileContext is ICompileContext12)
							{
								IDataManager4 dataManager = (compileContext as ICompileContext12).GetDataManager() as IDataManager4;
								if (((dataManager != null) ? dataManager.DataSegments : null) != null)
								{
									foreach (IDataSegment dataSegment in dataManager.DataSegments)
									{
										if ((dataSegment.Flags & DataSegmentFlags.Input) != DataSegmentFlags.None)
										{
											num2 = (long)(dataSegment.Address * 8);
											num4 = dataSegment.Size;
										}
										if ((dataSegment.Flags & DataSegmentFlags.Output) != DataSegmentFlags.None)
										{
											num3 = (long)(dataSegment.Address * 8);
											num5 = dataSegment.Size;
										}
									}
								}
							}
							AddrToChannelMap addrToChannelMap = new AddrToChannelMap(compileContext, num2, num3, num4, num5);
							LateLanguageModel lateLanguageModel = new LateLanguageModel(allTasks);
							lateLanguageModel.AddrToChannelMap = addrToChannelMap;
							int num6 = -1;
							if (DeviceObjectHelper.ConfigModeList.ContainsValue(e.ApplicationGuid))
							{
								num6 = DeviceObject.GetBusTaskConfigMode(allTasks, this._driverInfo);
							}
							if (num6 < 0)
							{
								num6 = this.GetBusTask(allTasks);
							}
							if (deviceApplicationObject != null && this._driverInfo.IoApplication != e.ApplicationGuid && !flag2)
							{
								num6 = -1;
							}
							lateLanguageModel.CyclicCalls.Init(allTasks, num6);
							DirectVarCrossRefsByTask directVarCrossRefsByTask = new DirectVarCrossRefsByTask(compileContext, allTasks.Length);
							LanguageModelHelper.FillLateLanguageModel(applicationObject, ldictionary, this, lateLanguageModel, ref num, compileContext, this._driverInfo.PlcAlwaysMapping, this._driverInfo.PlcAlwaysMappingMode, num6, directVarCrossRefsByTask, htStartAddresses, boolValue2, flag2, guid, false, flag3);
							bool flag4 = false;
							if (flag2 || (DeviceObjectHelper.GenerateCodeForLogicalDevices && LogicalIOHelper.GetLogicalApplications(this._nProjectHandle).Count > 0))
							{
								foreach (KeyValuePair<IIoProvider, bool> keyValuePair in this.LogicalGVLProviders)
								{
									bool flag5 = false;
									if (!flag2)
									{
										IMetaObject metaObject = keyValuePair.Key.GetMetaObject();
										string text2;
										guid = LogicalIOHelper.GetLogicalAppForDevice(metaObject.ProjectHandle, metaObject.ObjectGuid, out text2);
										Guid guid2 = guid;
										while (guid2 != Guid.Empty)
										{
											IMetaObjectStub metaObjectStub3 = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, guid2);
											if (metaObjectStub3 != null)
											{
												if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub3.ObjectType))
												{
													IMetaObject objectToRead3 = APEnvironment.ObjectMgr.GetObjectToRead(nProjectHandle, metaObjectStub3.ObjectGuid);
													if (objectToRead3 != null && objectToRead3.Object is DeviceObject)
													{
														ITargetSettings targetSettingsById2 = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById((objectToRead3.Object as IDeviceObject5).DeviceIdentificationNoSimulation);
														if (targetSettingsById2 != null && targetSettingsById2.Sections[LogicalIOHelper.stLogicalDeviceSection] != null)
														{
															flag5 = LocalTargetSettings.DisableChildApp.GetBoolValue(targetSettingsById2);
															flag4 = (flag4 || flag5);
															break;
														}
													}
												}
												guid2 = metaObjectStub3.ParentObjectGuid;
											}
										}
									}
									if (flag2 || flag5)
									{
										bool bPlcAlwaysMapping = this._driverInfo.PlcAlwaysMapping;
										if (!flag2)
										{
											bPlcAlwaysMapping = false;
										}
										if (keyValuePair.Value)
										{
											using (IEnumerator<IIoProvider> enumerator3 = DeviceObjectHelper.GetMappedIoProvider(keyValuePair.Key, true).GetEnumerator())
											{
												while (enumerator3.MoveNext())
												{
													IIoProvider ioProvider = enumerator3.Current;
													LanguageModelHelper.FillLateLanguageModel(null, ldictionary, ioProvider, lateLanguageModel, ref num, compileContext, bPlcAlwaysMapping, this._driverInfo.PlcAlwaysMappingMode, num6, directVarCrossRefsByTask, htStartAddresses, boolValue2, flag2, guid, flag5, flag3);
												}
												continue;
											}
										}
										LanguageModelHelper.FillLateLanguageModel(null, ldictionary, keyValuePair.Key, lateLanguageModel, ref num, compileContext, bPlcAlwaysMapping, this._driverInfo.PlcAlwaysMappingMode, num6, directVarCrossRefsByTask, htStartAddresses, boolValue2, flag2, guid, flag5, flag3);
									}
									else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 7, 0))
									{
										num++;
									}
								}
								if (DeviceObjectHelper.AdditionalModules != null && DeviceObjectHelper.AdditionalModules.DeviceGuids != null)
								{
									foreach (Guid guid3 in DeviceObjectHelper.AdditionalModules.DeviceGuids)
									{
										if (APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, guid3))
										{
											APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, guid3);
											IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(this._nProjectHandle, guid3);
											if ((hostStub == null || !(hostStub.ObjectGuid != this._metaObject.ObjectGuid)) && !flag4)
											{
												Guid guid4 = guid3;
												while (guid4 != Guid.Empty)
												{
													IMetaObjectStub metaObjectStub4 = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, guid4);
													if (metaObjectStub4 != null)
													{
														if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub4.ObjectType))
														{
															IMetaObject objectToRead4 = APEnvironment.ObjectMgr.GetObjectToRead(nProjectHandle, metaObjectStub4.ObjectGuid);
															if (objectToRead4 != null && objectToRead4.Object is DeviceObject)
															{
																ITargetSettings targetSettingsById3 = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById((objectToRead4.Object as IDeviceObject5).DeviceIdentificationNoSimulation);
																if (targetSettingsById3 != null && targetSettingsById3.Sections[LogicalIOHelper.stLogicalDeviceSection] != null)
																{
																	flag4 |= LocalTargetSettings.DisableChildApp.GetBoolValue(targetSettingsById3);
																	break;
																}
															}
														}
														guid4 = metaObjectStub4.ParentObjectGuid;
													}
												}
											}
										}
									}
									foreach (Guid guid5 in DeviceObjectHelper.AdditionalModules.DeviceGuids)
									{
										if (APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, guid5))
										{
											IMetaObject objectToRead5 = APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, guid5);
											IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(this._nProjectHandle, guid5);
											if (hostStub2 == null || !(hostStub2.ObjectGuid != this._metaObject.ObjectGuid))
											{
												IAdditionalModules additionalModules = objectToRead5.Object as IAdditionalModules;
												if (additionalModules != null)
												{
													foreach (IAdditionalModuleData additionalModuleData in additionalModules.AdditionalModules)
													{
														if (flag4 || flag2)
														{
															ConnectorMap connectorMap = new ConnectorMap(new MappingInfo(this, num), null, string.Format("ADR(moduleList[{0}])", num), this._nProjectHandle, guid5, objectToRead5.Name, num6, flag3);
															foreach (IAdditionalModuleParameter additionalModuleParameter in additionalModuleData.ParameterList)
															{
																if (additionalModuleParameter.ChannelType != ChannelType.None)
																{
																	ChannelMap channelMap = new ChannelMap(additionalModuleParameter.ParameterId, additionalModuleParameter.BitSize, additionalModuleParameter.ChannelType == ChannelType.Input, false, true, null, AlwaysMappingMode.OnlyIfUnused);
																	channelMap.Type = additionalModuleParameter.DataType;
																	channelMap.AddVariableMapping(new VariableMapping(-1L, additionalModuleParameter.Variable, flag2, true)
																	{
																		Parent = new DataElementSimpleType()
																	});
																	connectorMap.AddChannelMap(channelMap);
																}
															}
															lateLanguageModel.ConnectorMapList.Add(connectorMap);
														}
														if (flag4 || flag2 || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 13, 30) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 14, 0)) || APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 14, 10))
														{
															num++;
														}
													}
												}
											}
										}
									}
								}
							}
							DoubleAddressChecker doubleAddressChecker = new DoubleAddressChecker(num3, num5);
							foreach (ConnectorMap connectorMap2 in lateLanguageModel.ConnectorMapList.ConnectorMaps.Values)
							{
								if (connectorMap2.MappingInfo.IoProvider.TypeId != 152 && !connectorMap2.SkipOverlapCheck)
								{
									if (flag4)
									{
										IMetaObjectStub metaObjectStub5 = APEnvironment.ObjectMgr.GetMetaObjectStub(connectorMap2.ProjectHandle, connectorMap2.ObjectGuid);
										if (typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub5.ObjectType))
										{
											continue;
										}
									}
									foreach (object obj in connectorMap2.GetChannelMapList(false))
									{
										ChannelMap channelMap2 = (ChannelMap)obj;
										if (!channelMap2.IsInput && channelMap2.IecAddress != null)
										{
											bool flag6 = false;
											bool flag7;
											IDataLocation dataLocation = compileContext.LocateAddress(out flag7, channelMap2.IecAddress);
											if (dataLocation != null)
											{
												BitDataLocation bitDataLocation = new BitDataLocation(dataLocation);
												if (bitDataLocation.BitOffset % 8L == 0L && channelMap2.BitSize % 8U == 0U)
												{
													for (long num7 = bitDataLocation.BitOffset; num7 < bitDataLocation.BitOffset + (long)((ulong)channelMap2.BitSize); num7 += 8L)
													{
														if (!doubleAddressChecker.CheckByte(num7))
														{
															doubleAddressChecker.SetByte(num7);
														}
														else
														{
															flag6 = true;
														}
													}
												}
												else
												{
													for (long num8 = bitDataLocation.BitOffset; num8 < bitDataLocation.BitOffset + (long)((ulong)channelMap2.BitSize); num8 += 1L)
													{
														if (!doubleAddressChecker.CheckBit(num8))
														{
															doubleAddressChecker.SetBit(num8);
														}
														else
														{
															flag6 = true;
														}
													}
												}
												if (flag6 && connectorMap2.ObjectGuid != Guid.Empty)
												{
													IMetaObjectStub metaObjectStub6 = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, connectorMap2.ObjectGuid);
													string arg = string.Format(Strings.ErrorChannelAlreadyUsed, channelMap2.IecAddress.ToString());
													foreach (VariableMapping variableMapping in channelMap2.GetVariableMappings())
													{
														arg = string.Format(Strings.ErrorChannelAlreadyUsed, "$'" + variableMapping.Variable + "$'");
													}
													string text3 = string.Format("{{warning '{0}: {1}'}}\n", metaObjectStub6.Name, arg);
													text3 = string.Format("{{messageguid '{0}'}}\n", connectorMap2.ObjectGuid) + text3;
													if (channelMap2.LanguageModelPositionId != -1L)
													{
														text3 = string.Format("{{p {0} }}", channelMap2.LanguageModelPositionId) + text3;
													}
													lateLanguageModel.AddAfterUpdateConfigurationCode(text3);
												}
											}
										}
									}
								}
							}
							LateLanguageModel lateLanguageModel2 = new LateLanguageModel(allTasks);
							lateLanguageModel2.CyclicCalls.Init(allTasks, num6);
							IPreCompileContext2 preCompileContext = (IPreCompileContext2)APEnvironment.LanguageModelMgr.GetPrecompileContext(applicationObject.MetaObject.ObjectGuid);
							if (preCompileContext != null)
							{
								int num9 = 0;
								LanguageModelHelper.FillLanguageModel(ldictionary, this, lateLanguageModel2, ref num9, preCompileContext, true);
							}
							if (allTasks.Length == 0 && !flag)
							{
								string stCode = "{error '" + Strings.ErrorNoTask + "' show_compile}";
								lateLanguageModel.AddAfterUpdateConfigurationCode(stCode);
							}
							StringBuilder stringBuilder2 = new StringBuilder();
							StringBuilder stringBuilder3 = new StringBuilder();
							StringBuilder stringBuilder4 = new StringBuilder();
							StringBuilder stringBuilder5 = new StringBuilder();
							StringBuilder stringBuilder6 = null;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 11, 0) && this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.SetToDefault)
							{
								stringBuilder6 = new StringBuilder();
							}
							StringBuilder stringBuilder7 = null;
							LHashSet<string> lhashSet = new LHashSet<string>();
							int num10 = 0;
							if (this.DriverInfo is IDriverInfo7 && (this.DriverInfo as IDriverInfo7).CreateForceVariables && allTasks.Length > num6)
							{
								stringBuilder7 = new StringBuilder();
							}
							bool plcCreateWarningsAsErros = this._driverInfo.PlcCreateWarningsAsErros;
							StringBuilder stringBuilder8 = new StringBuilder();
							StringBuilder stringBuilder9 = new StringBuilder();
							StringBuilder stringBuilder10 = new StringBuilder();
							StringBuilder sbErrorMap = new StringBuilder();
							VariableCrossRefsByTask variableCrossRefsByTask = new VariableCrossRefsByTask(compileContext, lateLanguageModel.ConnectorMapList, this._metaObject, allTasks.Length, num6, stringBuilder5, flag2, directVarCrossRefsByTask, deviceApplicationObject != null, plcCreateWarningsAsErros);
							if (flag4)
							{
								variableCrossRefsByTask.AddLogicalGVL(compileContext, lateLanguageModel.ConnectorMapList, this._metaObject.Name, num6, stringBuilder5, applicationObject.MetaObject.Name);
							}
							Guid guid6;
							if (num6 >= 0 && allTasks.Length != 0)
							{
								guid6 = allTasks[num6].TaskGuid;
							}
							else
							{
								guid6 = Guid.Empty;
							}
							StringWriter stringWriter = new StringWriter();
							XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
							string stAdditionalCalls = string.Empty;
							string stAdditionalCalls2 = string.Empty;
							string stAdditionalCalls3 = string.Empty;
							if (flag2)
							{
								stAdditionalCalls = string.Format("IoMgrUpdateMapping2(ADR(iotaskmapproxy[0]), {0}, pszConfigApplication := {1});", allTasks.Length * 2, text);
							}
							if (this._driverInfo.EnableDiagnosis)
							{
								LDictionary<Guid, LanguageModelHelper.DiagnosisInstance> ldictionary2 = new LDictionary<Guid, LanguageModelHelper.DiagnosisInstance>();
								LanguageModelHelper.CollectDiagnosisInstances(ldictionary2, this, applicationObject.MetaObject);
								if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 3, 50))
								{
									LDictionary<Guid, LanguageModelHelper.DiagnosisInstance> ldictionary3 = new LDictionary<Guid, LanguageModelHelper.DiagnosisInstance>();
									foreach (KeyValuePair<Guid, LanguageModelHelper.DiagnosisInstance> keyValuePair2 in ldictionary2)
									{
										if (!ldictionary3.ContainsKey(keyValuePair2.Key))
										{
											ldictionary3.Add(keyValuePair2.Key, keyValuePair2.Value);
										}
										foreach (KeyValuePair<Guid, LanguageModelHelper.DiagnosisInstance> keyValuePair3 in ldictionary2)
										{
											if (keyValuePair2.Value.ObjectGuid == keyValuePair3.Value.ParentGuid)
											{
												ldictionary3.Add(keyValuePair3.Key, keyValuePair3.Value);
											}
										}
									}
									ldictionary2 = ldictionary3;
								}
								if (deviceApplicationObject == null || flag)
								{
									int num11 = 0;
									Guid[] array = new Guid[ldictionary2.Count];
									ldictionary2.Keys.CopyTo(array, 0);
									foreach (KeyValuePair<Guid, LanguageModelHelper.DiagnosisInstance> keyValuePair4 in ldictionary2)
									{
										LanguageModelHelper.DiagnosisInstance value = keyValuePair4.Value;
										if (value.Instance.FbName == "CAADiagDeviceDefault" || !string.IsNullOrEmpty(value.Instance.FbNameDiag) || value.RequiredLib != null)
										{
											lateLanguageModel.AddFBInit(string.Format("DeviceNodes[{0}] := {1};", num11++, value.Instance.Instance.Variable));
										}
										int num12 = 0;
										int num13 = 0;
										foreach (LanguageModelHelper.DiagnosisInstance diagnosisInstance in ldictionary2.Values)
										{
											Guid parentGuid = diagnosisInstance.ParentGuid;
											if (!(Guid.Empty == parentGuid) && parentGuid == keyValuePair4.Key)
											{
												if (num12 == 0)
												{
													num13 = Array.IndexOf<Guid>(array, diagnosisInstance.ObjectGuid);
												}
												num12++;
											}
										}
										StringBuilder stringBuilder11 = new StringBuilder();
										stringBuilder11.Append(string.Format("{0}.Initialize_Diag(ADR(modulelist[{1}]), {2}", value.Instance.Instance.Variable, value.ModuleIndex, num));
										if (value.ParentGuid != Guid.Empty)
										{
											int num14 = Array.IndexOf<Guid>(array, value.ParentGuid);
											stringBuilder11.Append(string.Format(", ADR(DeviceNodes[{0}])", num14));
										}
										else
										{
											stringBuilder11.Append(", 0");
										}
										if (num12 > 0)
										{
											stringBuilder11.Append(string.Format(", ADR(DeviceNodes[{0}]), {1});", num13, num12));
										}
										else
										{
											stringBuilder11.Append(", 0, 0);");
										}
										lateLanguageModel.AddFBInit(stringBuilder11.ToString());
									}
									if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 6, 40))
									{
										lateLanguageModel.AddFBInit("DED" + ".g_itfRootINodeInst := ADR(DeviceNodes[0]);");
										lateLanguageModel.AddFBInit(string.Format("DED" + ".g_itfRootNodeCount := {0};", array.Length));
									}
									else
									{
										lateLanguageModel.AddFBInit("g_itfRootINodeInst := ADR(DeviceNodes[0]);");
										lateLanguageModel.AddFBInit(string.Format("g_itfRootNodeCount := {0};", array.Length));
									}
								}
							}
							string stPastUpdateConfigInitialization = string.Empty;
							string text4 = string.Empty;
							if (deviceApplicationObject == null || flag)
							{
								stPastUpdateConfigInitialization = lateLanguageModel.GetAfterUpdateConfigurationCode();
								text4 = lateLanguageModel.GetFbInits();
							}
							string text5 = PouDefinitions.WriteGlobalInitPou(null, referenceContextIfAvailable, guid6, "ADR(moduleList[0])", num, allTasks.Length * 2, text4, stPastUpdateConfigInitialization, text, stAdditionalCalls, this.DeviceIdentificationNoSimulation);
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 4, 1, 0))
							{
								StringBuilder stringBuilder12 = new StringBuilder();
								stringBuilder12.Append(text5);
								stringBuilder12.AppendLine();
								stringBuilder12.AppendLine("pIoConfigTaskMap := ADR(iotaskmap[0]);");
								stringBuilder12.AppendFormat("nIoConfigTaskMapCount := {0};\r\n", allTasks.Length * 2);
								text5 = stringBuilder12.ToString();
							}
							bool flag8 = true;
							bool flag9 = true;
							int num15 = -1;
							LDictionary<Guid, DeviceObject.ApplicationUsage> ldictionary4 = new LDictionary<Guid, DeviceObject.ApplicationUsage>();
							foreach (KeyValuePair<Guid, DeviceObject.ApplicationUsage> keyValuePair5 in this._dictAppUsage)
							{
								if (APEnvironment.ObjectMgr.ExistsObject(nProjectHandle, keyValuePair5.Key))
								{
									ldictionary4.Add(keyValuePair5.Key, keyValuePair5.Value);
								}
							}
							this._dictAppUsage = ldictionary4;
							DoubleAddressTaskChecker checker = new DoubleAddressTaskChecker(num3, num5, allTasks.Length);
							DoubleAddressTaskChecker checker2 = new DoubleAddressTaskChecker(num2, num4, allTasks.Length);
							DeviceObject.ApplicationUsage applicationUsage;
							if (!this._dictAppUsage.TryGetValue(e.ApplicationGuid, out applicationUsage))
							{
								applicationUsage = new DeviceObject.ApplicationUsage();
								applicationUsage.Checker = checker;
								if (string.Compare(APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, e.ApplicationGuid).Name, OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION, StringComparison.InvariantCultureIgnoreCase) != 0)
								{
									this._dictAppUsage[e.ApplicationGuid] = applicationUsage;
								}
							}
							applicationUsage.TaskUsage.Clear();
							if (flag2 && APEnvironment.ObjectMgr.ExistsObject(nProjectHandle, guid))
							{
								IMetaObjectStub metaObjectStub7 = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, guid);
								while (metaObjectStub7.ParentObjectGuid != Guid.Empty)
								{
									metaObjectStub7 = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, metaObjectStub7.ParentObjectGuid);
									if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub7.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub7.ObjectType))
									{
										break;
									}
								}
								num15 = LogicalIOHelper.GetProxyModuleIndex(this, metaObjectStub7.ObjectGuid);
							}
							StringWriter stringWriter2 = new StringWriter();
							XmlWriter xmlWriter2 = new XmlTextWriter(stringWriter2);
							if (allTasks.Length == 0)
							{
								stringBuilder2.AppendLine("{attribute 'init_on_onlchange'}");
								stringBuilder2.AppendFormat("{{attribute 'hide'}}\r\n iotaskmap : ARRAY [0..0] OF IoConfigTaskMap;\n", Array.Empty<object>());
								if (flag2)
								{
									stringBuilder8.AppendLine("{attribute 'init_on_onlchange'}");
									stringBuilder8.AppendFormat("{{attribute 'hide'}}\r\n iotaskmapproxy : ARRAY [0..0] OF IoConfigTaskMap;\n", Array.Empty<object>());
								}
							}
							else
							{
								stringBuilder2.AppendLine("{attribute 'init_on_onlchange'}");
								stringBuilder2.AppendFormat("{{attribute 'blobinit'}}{{attribute 'hide'}}\r\n iotaskmap : ARRAY [0..{0}] OF IoConfigTaskMap := [\n", allTasks.Length * 2 - 1);
								if (flag2)
								{
									stringBuilder8.AppendLine("{attribute 'init_on_onlchange'}");
									stringBuilder8.AppendFormat("{{attribute 'blobinit'}}{{attribute 'hide'}}\r\n iotaskmapproxy : ARRAY [0..{0}] OF IoConfigTaskMap := [\n", allTasks.Length * 2 - 1);
								}
								for (int j = 0; j < allTasks.Length; j++)
								{
									StartBusCycleInfo[] busCycleInfos = addrToChannelMap.GetBusCycleInfos(allTasks[j].TaskName, j == num6);
									StartBusCycleInfo[] array2 = busCycleInfos;
									for (int i = 0; i < array2.Length; i++)
									{
										if (array2[i].ExternEvent)
										{
											string externalFBInstanceDeclaration = LanguageModelHelper.GetExternalFBInstanceDeclaration(allTasks[j].TaskName);
											lhashSet.Add(externalFBInstanceDeclaration);
										}
									}
									stAdditionalCalls2 = string.Empty;
									stAdditionalCalls3 = string.Empty;
									if (flag2)
									{
										stAdditionalCalls2 = string.Format("IoMgrReadInputs(ADR(iotaskmapproxy[{0}]));\r\n", 2 * j);
										stAdditionalCalls3 = string.Format("IoMgrWriteOutputs(ADR(iotaskmapproxy[{0}]));\r\n", 2 * j + 1);
									}
									LList<DirectVarCrossRef> crossRefsForTask = directVarCrossRefsByTask.GetCrossRefsForTask((byte)j, true);
									VariableCrossRef[] crossRefsForTask2 = variableCrossRefsByTask.GetCrossRefsForTask((byte)j, true);
									FixedTaskUpdate[] fixedUpdatesForTask = lateLanguageModel.FixedTaskUpdates.GetFixedUpdatesForTask((byte)j, true);
									TaskMapList taskMapList = new TaskMapList(this._dictAppUsage, e.ApplicationGuid);
									LanguageModelHelper.GetTaskMappings(j, addrToChannelMap, taskMapList, checker2, crossRefsForTask, crossRefsForTask2, fixedUpdatesForTask, compileContext, htStartAddresses, boolValue3, plcCreateWarningsAsErros);
									if (taskMapList.Mappings.Count > 0)
									{
										this.SortTaskMappings(taskMapList.Mappings);
										foreach (ISortTaskMapFactory sortTaskMapFactory in APEnvironment.SortTaskMapFactories)
										{
											sortTaskMapFactory.SortTaskMappings(this, taskMapList.Mappings, true, allTasks[j].TaskGuid);
										}
									}
									this.FillTaskUsageList(taskMapList, applicationUsage.TaskUsage, lateLanguageModel, allTasks[j], allTasks);
									string stLateInitCode = "";
									if (allTasks[j].TaskGuid == guid6)
									{
										stLateInitCode = text5;
									}
									PouDefinitions.WriteBeforeTaskPou(xmlWriter2, referenceContextIfAvailable, allTasks[j].TaskGuid, 2 * j, busCycleInfos, stLateInitCode, this._driverInfo.UpdateIOsInStop, boolValue, stAdditionalCalls2);
									if (boolValue3)
									{
										string text6 = string.Format(string.Format("__IoConfig{0}_{{0}}", "MappingTaskAfterReadInputs"), j * 2);
										Guid beforeAfterPou = this.GetBeforeAfterPou(text6, preCompileContext);
										if (taskMapList.MapToExisting.Count > 0)
										{
											XmlAttribute[] attributes = new XmlAttribute[]
											{
												new XmlAttribute("slot", "110"),
												new XmlAttribute("task-id", allTasks[j].TaskGuid.ToString())
											};
											string stInterface = string.Format("\r\n{{implicit}}\r\nFUNCTION {0} : BOOL\r\nVAR_INPUT\r\nEND_VAR", text6);
											StringBuilder stringBuilder13 = new StringBuilder();
											stringBuilder13.AppendLine("{implicit on}");
											foreach (string value2 in taskMapList.MapToExisting)
											{
												stringBuilder13.AppendLine(value2);
											}
											stringBuilder13.AppendLine("{implicit off}");
											PouDefinitions.WritePou(xmlWriter2, beforeAfterPou, text6, stInterface, stringBuilder13.ToString(), attributes);
										}
										else
										{
											foreach (ISignature signature in preCompileContext.FindSignature(text6.ToUpperInvariant()))
											{
												APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature.ObjectGuid);
											}
										}
									}
									LList<DirectVarCrossRef> crossRefsForTask3 = directVarCrossRefsByTask.GetCrossRefsForTask((byte)j, false);
									VariableCrossRef[] crossRefsForTask4 = variableCrossRefsByTask.GetCrossRefsForTask((byte)j, false);
									FixedTaskUpdate[] fixedUpdatesForTask2 = lateLanguageModel.FixedTaskUpdates.GetFixedUpdatesForTask((byte)j, false);
									TaskMapList taskMapList2 = new TaskMapList(this._dictAppUsage, e.ApplicationGuid);
									LanguageModelHelper.GetTaskMappings(j, addrToChannelMap, taskMapList2, checker, crossRefsForTask3, crossRefsForTask4, fixedUpdatesForTask2, compileContext, htStartAddresses, boolValue3, plcCreateWarningsAsErros);
									if (taskMapList2.Mappings.Count > 0)
									{
										this.SortTaskMappings(taskMapList2.Mappings);
										foreach (ISortTaskMapFactory sortTaskMapFactory2 in APEnvironment.SortTaskMapFactories)
										{
											sortTaskMapFactory2.SortTaskMappings(this, taskMapList2.Mappings, false, allTasks[j].TaskGuid);
										}
									}
									this.FillTaskUsageList(taskMapList2, applicationUsage.TaskUsage, lateLanguageModel, allTasks[j], allTasks);
									if (!flag2 && taskMapList2.Mappings.Count > 0)
									{
										foreach (KeyValuePair<ITaskMappingInfo, List<ITaskMapping>> keyValuePair6 in taskMapList2.Mappings)
										{
											ITaskMappingInfo key = keyValuePair6.Key;
											object obj2;
											if (key == null)
											{
												obj2 = null;
											}
											else
											{
												IIoProvider ioProvider2 = key.IoProvider;
												obj2 = ((ioProvider2 != null) ? ioProvider2.ParameterSet : null);
											}
											if (obj2 is ParameterSet && (keyValuePair6.Key.IoProvider.ParameterSet as ParameterSet).HasBidirectionalOutputs)
											{
												bool flag10 = false;
												foreach (KeyValuePair<ITaskMappingInfo, List<ITaskMapping>> keyValuePair7 in taskMapList.Mappings)
												{
													if (keyValuePair7.Key.ModuleIndex == keyValuePair6.Key.ModuleIndex)
													{
														flag10 = true;
														foreach (ITaskMapping taskMapping in keyValuePair6.Value)
														{
															Mapping mapping = taskMapping as Mapping;
															object obj3;
															if (mapping == null)
															{
																obj3 = null;
															}
															else
															{
																ParameterSet paramSet = mapping.ParamSet;
																obj3 = ((paramSet != null) ? paramSet.GetParameter((long)((ulong)(taskMapping as Mapping).ParameterId)) : null);
															}
															Parameter parameter = obj3 as Parameter;
															if (parameter != null && parameter.BidirectionalOutput)
															{
																keyValuePair7.Value.Add(taskMapping);
															}
														}
													}
												}
												if (!flag10)
												{
													List<ITaskMapping> list = new List<ITaskMapping>();
													foreach (ITaskMapping taskMapping2 in keyValuePair6.Value)
													{
														Mapping mapping2 = taskMapping2 as Mapping;
														object obj4;
														if (mapping2 == null)
														{
															obj4 = null;
														}
														else
														{
															ParameterSet paramSet2 = mapping2.ParamSet;
															obj4 = ((paramSet2 != null) ? paramSet2.GetParameter((long)((ulong)(taskMapping2 as Mapping).ParameterId)) : null);
														}
														Parameter parameter2 = obj4 as Parameter;
														if (parameter2 != null && parameter2.BidirectionalOutput)
														{
															list.Add(taskMapping2);
														}
													}
													if (list.Count > 0)
													{
														taskMapList.Mappings.Add(keyValuePair6.Key, list);
													}
												}
											}
										}
									}
									taskMapList.AddToLanguageModel(applicationName, j, 1, ref flag8, stringBuilder2, stringBuilder3, stringBuilder4, stringBuilder5, stringBuilder7, ref num10, compileContext, lateLanguageModel, flag2, false, 0, null);
									if (flag2 && num15 >= 0)
									{
										taskMapList.AddToLanguageModel(applicationName, j, 1, ref flag9, stringBuilder8, stringBuilder9, stringBuilder10, sbErrorMap, null, ref num10, compileContext, lateLanguageModel, flag2, true, num15, null);
									}
									taskMapList2.AddToLanguageModel(applicationName, j, 2, ref flag8, stringBuilder2, stringBuilder3, stringBuilder4, stringBuilder5, stringBuilder7, ref num10, compileContext, lateLanguageModel, flag2, false, 0, stringBuilder6);
									if (flag2 && num15 >= 0)
									{
										taskMapList2.AddToLanguageModel(applicationName, j, 2, ref flag9, stringBuilder8, stringBuilder9, stringBuilder10, sbErrorMap, null, ref num10, compileContext, lateLanguageModel, flag2, true, num15, null);
									}
									PouDefinitions.WriteAfterTaskPou(xmlWriter2, referenceContextIfAvailable, allTasks[j].TaskGuid, 2 * j + 1, busCycleInfos, this._driverInfo, boolValue, stAdditionalCalls3, addrToChannelMap.GetAllBusCycleInfos, allTasks[j].TaskName);
									if (boolValue3)
									{
										string text7 = string.Format(string.Format("__IoConfig{0}_{{0}}", "MappingTaskBeforeWriteOutputs"), j * 2);
										Guid beforeAfterPou2 = this.GetBeforeAfterPou(text7, preCompileContext);
										if (taskMapList2.MapToExisting.Count > 0)
										{
											XmlAttribute[] attributes2 = new XmlAttribute[]
											{
												new XmlAttribute("slot", "59990"),
												new XmlAttribute("task-id", allTasks[j].TaskGuid.ToString())
											};
											string stInterface2 = string.Format("\r\n{{implicit}}\r\nFUNCTION {0} : BOOL\r\nVAR_INPUT\r\nEND_VAR", text7);
											StringBuilder stringBuilder14 = new StringBuilder();
											stringBuilder14.AppendLine("{implicit on}");
											foreach (string value3 in taskMapList2.MapToExisting)
											{
												stringBuilder14.AppendLine(value3);
											}
											stringBuilder14.AppendLine("{implicit off}");
											PouDefinitions.WritePou(xmlWriter2, beforeAfterPou2, text7, stInterface2, stringBuilder14.ToString(), attributes2);
										}
										else
										{
											foreach (ISignature signature2 in preCompileContext.FindSignature(text7.ToUpperInvariant()))
											{
												APEnvironment.LanguageModelMgr.RemoveLanguageModelOfObject(nProjectHandle, signature2.ObjectGuid);
											}
										}
									}
									if ((this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.SetToDefault || (this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.ExecuteProgram && this._driverInfo.StopResetBehaviourUserProgram != string.Empty)) && !this.SimulationMode)
									{
										string calls = lateLanguageModel2.CyclicCalls.GetCalls(j, "BeforeWriteOutputs");
										string calls2 = lateLanguageModel2.CyclicCalls.GetCalls(j, "AfterWriteOutputs");
										PouDefinitions.WriteResetOutputsPou(xmlWriter2, referenceContextIfAvailable, Guid.NewGuid(), allTasks[j].TaskGuid, 2 * j + 1, this._driverInfo._StopResetBehaviourSetting, this._driverInfo._StopResetBehaviourUserProgram, addrToChannelMap.GetAllBusCycleInfos, calls, calls2, this._driverInfo.UpdateIOsInStop);
									}
								}
								stringBuilder2.Append("\n];\n");
								if (flag2)
								{
									stringBuilder8.Append("\n];\n");
								}
							}
							string text8 = string.Format("IoConfig_{0}_Mappings", applicationName);
							List<string> list2 = new List<string>();
							list2.Add(stringBuilder2.ToString());
							list2.Add(stringBuilder3.ToString());
							list2.Add(stringBuilder4.ToString());
							list2.Add(stringBuilder5.ToString());
							if (flag2 && num15 >= 0)
							{
								list2.Add(stringBuilder8.ToString());
								list2.Add(stringBuilder9.ToString());
								list2.Add(stringBuilder10.ToString());
							}
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 10, 0) && lhashSet.Count > 0)
							{
								StringBuilder stringBuilder15 = new StringBuilder();
								foreach (string value4 in lhashSet)
								{
									stringBuilder15.AppendLine(value4);
								}
								list2.Add(stringBuilder15.ToString());
							}
							string stAttributes = "{attribute 'generate_implicit_init_function'} {attribute 'init_on_onl_change'}";
							ISignature signature3 = null;
							if (referenceContextIfAvailable != null)
							{
								signature3 = referenceContextIfAvailable.GetSignature(text8);
							}
							Guid guidGvl;
							if (signature3 != null)
							{
								guidGvl = signature3.ObjectGuid;
							}
							else
							{
								guidGvl = LanguageModelHelper.CreateDeterministicGuid(preCompileContext.ApplicationGuid, text8);
							}
							LanguageModelHelper.AddGlobalVarList(list2, guidGvl, text8, xmlTextWriter, this.MetaObject.ObjectGuid, false, stAttributes, null);
							if (this.DriverInfo is IDriverInfo7 && (this.DriverInfo as IDriverInfo7).CreateForceVariables && allTasks.Length > num6)
							{
								string format = "\r\n{{attribute 'hide'}}\r\nTYPE {0}:\r\nSTRUCT\r\n\txIsInput: BIT;\r\n\txOldForce: BIT;\r\nEND_STRUCT\r\nEND_TYPE\r\n";
								PouDefinitions.WriteDatatype(xmlTextWriter, Guid.NewGuid(), this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES_FLAGS, string.Format(format, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES_FLAGS));
								string format2 = "\r\n{{attribute 'hide'}}\r\nTYPE {0}:\r\nSTRUCT\r\n\tForceFlags: {1};\r\n\tpbyIecAddress: POINTER TO BYTE;\r\n    wIecBitOffset : WORD;\r\n\tpbyValue: POINTER TO BYTE;\r\n\tpChannel: POINTER TO IoConfigChannelMap;\r\n\tpxForce : POINTER TO BOOL;\r\nEND_STRUCT\r\nEND_TYPE\r\n";
								PouDefinitions.WriteDatatype(xmlTextWriter, Guid.NewGuid(), this.MetaObject.ObjectGuid, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES_TYPE, string.Format(format2, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES_TYPE, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES_FLAGS));
								text8 = string.Format("IoConfig_{0}_ForceVars", applicationName);
								string format3 = "\r\n{{implicit}}\r\n{{attribute 'init_on_onlchange'}}\r\nPROGRAM {0}\r\nVAR\r\n    bInit: BOOL := FALSE;\r\n    diCount: DINT;\r\n    {{attribute 'noinit'}}\r\n    IoConfig_Forces_buffer : ARRAY [0..99] OF BYTE;\r\n    {{attribute 'noinit'}}\r\n    IoConfig_Forces : ARRAY [0..{2}] OF {1};\r\n\r\n    pForce: POINTER TO {1};\r\n    pbyDest: POINTER TO BYTE;\r\n\tpbyIecAddress: POINTER TO BYTE;\r\n\tk: word;\r\n\twSize: WORD;\r\n\twDestIndex : WORD;\t\r\n\tbyDestMask : BYTE;\t\r\n    bySrcMask : BYTE;\t\r\n\tpbySrc: POINTER TO BYTE;\r\nEND_VAR\r\n";
								string format4 = "\r\nIF NOT bInit THEN\r\n\tbInit := TRUE;\r\n    {0}\r\nEND_IF\r\nFOR diCount := 0 TO {1} DO\r\n    IF IoConfig_Forces_Reset THEN\r\n        IoConfig_Forces[diCount].pxForce^ := FALSE;\r\n    END_IF\r\n    pForce := ADR(IoConfig_Forces[diCount]);\r\n    IF pForce^.pChannel > 0 THEN\r\n\t    IF pForce^.pxForce^ THEN\r\n\t\t    IF NOT pForce^.ForceFlags.xOldForce THEN\r\n\t\t\t    pForce^.pbyIecAddress := pForce^.pChannel^.pbyIecAddress;\r\n                pForce^.wIecBitOffset := pForce^.pChannel^.wIecAddressBitOffset;\r\n\t\t\t    IF pForce^.ForceFlags.xIsInput THEN\r\n\t\t\t\t    IF pForce^.pChannel^.wSize/8 < SIZEOF(IoConfig_Forces_buffer) THEN\r\n\t\t\t\t\t    pForce^.pChannel^.pbyIecAddress := ADR(IoConfig_Forces_buffer);\r\n\t\t\t\t\t    pbyIecAddress := pForce^.pbyIecAddress;\r\n\t\t\t\t\t    wDestIndex := pForce^.pChannel^.wIecAddressBitOffset / 8; \r\n\t\t\t\t\t    IF (pForce^.pChannel^.wSize = 1) THEN\r\n\t\t\t\t\t\t    byDestMask := 1;\r\n\t\t\t\t\t\t    byDestMask := SHL(byDestMask, pForce^.pChannel^.wIecAddressBitOffset MOD 8);\r\n\t\t\t\t\t\t    IF (pForce^.pbyValue^) <> 0 THEN\r\n\t\t\t\t\t\t\t    pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] OR byDestMask);\t\t\t\t\r\n\t\t\t\t\t\t    ELSE\r\n\t\t\t\t\t\t\t    pbyIecAddress[wDestIndex] := (pbyIecAddress[wDestIndex] AND NOT byDestMask);\t\t\t\t\r\n\t\t\t\t\t\t    END_IF \t\t\t\r\n\t\t\t\t\t    ELSE\r\n\t\t\t\t\t\t    wSize := (pForce^.pChannel^.wSize / 8);\r\n\t\t\t\t\t\t    IF wSize > 0 THEN\r\n                                pbySrc := ADR(pForce^.pbyValue[wDestIndex]);\r\n\t\t\t\t\t\t\t    pbyDest := ADR(pbyIecAddress[wDestIndex]);\r\n\t\t\t\t\t\t\t    FOR k:=0 TO wSize - 1 DO\r\n\t\t\t\t\t\t\t\t    pbyDest[k] := pbySrc[k];\r\n\t\t\t\t\t\t\t    END_FOR\r\n\t\t\t\t\t\t    END_IF\r\n\t\t\t\t\t    END_IF\r\n\t\t\t\t    END_IF\r\n\t\t\t    ELSE\r\n\t\t\t        pForce^.pChannel^.pbyIecAddress := pForce^.pbyValue;\r\n                    pForce^.pChannel^.wIecAddressBitOffset := 0;\r\n\t\t\t    END_IF\r\n\t\t    END_IF\r\n\t    ELSE\r\n\t\t    IF pForce^.ForceFlags.xOldForce THEN\r\n\t\t\t    pForce^.pChannel^.pbyIecAddress := pForce^.pbyIecAddress;\r\n                pForce^.pChannel^.wIecAddressBitOffset := pForce^.wIecBitOffset;\r\n\t\t    END_IF\r\n\t    END_IF\r\n\t    pForce^.ForceFlags.xOldForce := pForce^.pxForce^;\r\n    END_IF\r\n\r\nEND_FOR\r\n\r\n";
								XmlAttribute[] attributes3 = new XmlAttribute[]
								{
									new XmlAttribute("slot", "90"),
									new XmlAttribute("task-id", allTasks[num6].TaskGuid.ToString())
								};
								string stInterface3 = string.Format(format3, text8, DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES_TYPE, num10 - 1);
								string stBody = string.Format(format4, stringBuilder7.ToString(), num10 - 1);
								PouDefinitions.WritePou(xmlTextWriter, Guid.NewGuid(), text8, stInterface3, stBody, attributes3);
							}
							xmlWriter2.Close();
							stringWriter2.Close();
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 11, 0) && this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.SetToDefault)
							{
								text4 += stringBuilder6.ToString();
							}
							PouDefinitions.WriteGlobalInitPou(xmlWriter, referenceContextIfAvailable, guid6, "ADR(moduleList[0])", num, allTasks.Length * 2, text4, stPastUpdateConfigInitialization, text, stAdditionalCalls, this.DeviceIdentificationNoSimulation);
							xmlWriter.Close();
							stringWriter.Close();
							xmlTextWriter.WriteRaw(stringWriter2.ToString());
							xmlTextWriter.WriteRaw(stringWriter.ToString());
							int iNumberOfResetCalls = 0;
							if ((this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.SetToDefault || (this._driverInfo.StopResetBehaviourSetting == StopResetBehaviour.ExecuteProgram && this._driverInfo.StopResetBehaviourUserProgram != string.Empty)) && !this.SimulationMode)
							{
								iNumberOfResetCalls = allTasks.Length;
							}
							xmlTextWriter.Flush();
							Crc32 crc = new Crc32();
							crc.Update(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
							if (deviceApplicationObject == null || flag)
							{
								PouDefinitions.WriteGlobalExitPou(xmlTextWriter, referenceContextIfAvailable, e.ApplicationGuid, iNumberOfResetCalls, crc.Value);
							}
							xmlTextWriter.WriteEndElement();
							if (e.LanguageModelList != null)
							{
								long num16 = 0L;
								if (compileContext != null)
								{
									ISignature signature4 = compileContext.GetSignature(PouDefinitions.GlobalExitPou_Name);
									if (signature4 != null)
									{
										string attributeValue = signature4.GetAttributeValue("crc_for_latelanguagemodel");
										if (!string.IsNullOrEmpty(attributeValue))
										{
											long.TryParse(attributeValue, out num16);
										}
									}
								}
								if (num16 != crc.Value)
								{
									Encoding unicode = Encoding.Unicode;
									xmlTextWriter.Flush();
									byte[] buffer = memoryStream.GetBuffer();
									string @string;
									if (buffer[0] == 255 && buffer[1] == 254)
									{
										@string = unicode.GetString(buffer, 2, (int)memoryStream.Length - 2);
									}
									else
									{
										@string = unicode.GetString(buffer, 0, (int)memoryStream.Length);
									}
									e.LanguageModelList.AddLanguageModel(@string);
								}
							}
							xmlTextWriter.Close();
							if (memoryStream != null)
							{
								memoryStream.Close();
							}
						}
					}
				}
			}
			finally
			{
				DeviceObjectHelper.IsInLateLanguageModel = isInLateLanguageModel;
			}
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0002F16C File Offset: 0x0002E16C
		public IApplicationObject GetDeviceApplicationObject(IMetaObject plcLogic)
		{
			if (this.UseParentPLC)
			{
				return null;
			}
			foreach (Guid objectGuid in plcLogic.SubObjectGuids)
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(plcLogic.ProjectHandle, objectGuid);
				if (typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return APEnvironment.ObjectMgr.GetObjectToRead(plcLogic.ProjectHandle, objectGuid).Object as IApplicationObject;
				}
			}
			return null;
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0002F1E8 File Offset: 0x0002E1E8
		public IApplicationObject GetDeviceApplicationObject(IPlcLogicObject logic)
		{
			if (logic != null && logic.MetaObject != null)
			{
				foreach (Guid objectGuid in logic.MetaObject.SubObjectGuids)
				{
					if (APEnvironment.ObjectMgr.ExistsObject(this.MetaObject.ProjectHandle, objectGuid))
					{
						IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this.MetaObject.ProjectHandle, objectGuid);
						if (typeof(IDeviceApplication).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							return APEnvironment.ObjectMgr.GetObjectToRead(metaObjectStub.ProjectHandle, objectGuid).Object as IApplicationObject;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0002F28C File Offset: 0x0002E28C
		public IApplicationObject GetApplicationObject(IPlcLogicObject logic)
		{
			Debug.Assert(logic != null);
			Guid[] subObjectGuids = logic.MetaObject.SubObjectGuids;
			Guid guid = DeviceObjectHelper.ConfigModeApplication(logic.MetaObject.ParentObjectGuid);
			if (guid != Guid.Empty && guid != DeviceObjectHelper.ParamModeGuid && APEnvironment.ObjectMgr.ExistsObject(this._nProjectHandle, guid))
			{
				return (IApplicationObject)APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, guid).Object;
			}
			foreach (Guid guid2 in subObjectGuids)
			{
				IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._nProjectHandle, guid2);
				if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._nProjectHandle, guid2);
					if (objectToRead.Object is IApplicationObject)
					{
						if (!this._driverInfo.IoApplicationSet)
						{
							return (IApplicationObject)objectToRead.Object;
						}
						if (guid2 == this._driverInfo.IoApplication || this._driverInfo.IoApplication == Guid.Empty)
						{
							return (IApplicationObject)objectToRead.Object;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0002F3C8 File Offset: 0x0002E3C8
		public ITaskConfigObject GetTaskConfigObject(IApplicationObject app)
		{
			Debug.Assert(app != null);
			Guid[] subObjectGuids = app.MetaObject.SubObjectGuids;
			int projectHandle = app.MetaObject.ProjectHandle;
			foreach (Guid objectGuid in subObjectGuids)
			{
				IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(projectHandle, objectGuid);
				if (objectToRead.Object is ITaskConfigObject)
				{
					return (ITaskConfigObject)objectToRead.Object;
				}
			}
			return null;
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0002F438 File Offset: 0x0002E438
		public IPlcLogicObject GetPlcLogicObject()
		{
			IMetaObject plcLogic = this.GetPlcLogic();
			if (plcLogic != null && plcLogic.Object is IPlcLogicObject)
			{
				return plcLogic.Object as IPlcLogicObject;
			}
			return null;
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000673 RID: 1651 RVA: 0x0002F46C File Offset: 0x0002E46C
		public virtual Guid[] ObjectsToUpdate
		{
			get
			{
				if (DeviceObjectHelper.IsUpdateObjectSuppressed)
				{
					return new Guid[0];
				}
				Hashtable hashtable = new Hashtable();
				object value = new object();
				int num;
				if (!APEnvironment.ObjectMgr.IsLoadProjectFinished(this._metaObject.ProjectHandle, out num))
				{
					IDeviceObject hostDeviceObject = this.GetHostDeviceObject();
					if (hostDeviceObject != null && hostDeviceObject.MetaObject != null)
					{
						DeviceObjectHelper.AddObjectsToUpdate(hostDeviceObject.MetaObject.ProjectHandle, hostDeviceObject.MetaObject.ObjectGuid);
					}
				}
				else
				{
					foreach (object obj in this.Connectors)
					{
						IIoProvider ioProvider = (Connector)obj;
						while (ioProvider.Parent != null)
						{
							ioProvider = ioProvider.Parent;
						}
						if (ioProvider is IDeviceObject)
						{
							IMetaObject metaObject = (ioProvider as IDeviceObject).MetaObject;
							if (!DeviceObjectHelper.IsInListLanguageModel(metaObject.ObjectGuid))
							{
								hashtable[metaObject.ObjectGuid] = value;
							}
						}
					}
				}
				hashtable.Remove(this._metaObject.ObjectGuid);
				Guid[] array = new Guid[hashtable.Keys.Count];
				hashtable.Keys.CopyTo(array, 0);
				return array;
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x0002F5B4 File Offset: 0x0002E5B4
		internal bool AllowTopLevel
		{
			get
			{
				foreach (IConnector item in (IEnumerable)Connectors)
				{
					if ((int)item.ConnectorRole == 1)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000675 RID: 1653 RVA: 0x0002F614 File Offset: 0x0002E614
		internal string IoBaseName
		{
			get
			{
				return LanguageModelHelper.GetIoBaseName(this);
			}
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0002F61C File Offset: 0x0002E61C
		internal bool ReadDevice(XmlNode xnDevice, TypeList types, bool bUpdate, bool bVersionUpgrade, LList<DeviceIdUpdate> devicesToUpdate, IDeviceIdentification deviceId, bool bCreateBitChannels)
		{
			int num = 0;
			uint num2 = 0U;
			LDictionary<uint, LList<XmlElement>> ldictionary = new LDictionary<uint, LList<XmlElement>>();
			LList<XmlElement> llist = new LList<XmlElement>();
			string attribute = ((XmlElement)xnDevice).GetAttribute("showParamsInDevDescOrder");
			this._bShowParamsInDevDescOrder = DeviceObjectHelper.ParseBool(attribute, false);
			attribute = ((XmlElement)xnDevice).GetAttribute("onlineHelpUrl");
			if (!string.IsNullOrEmpty(attribute))
			{
				this._stOnlineHelpUrl = attribute;
			}
			bool flag = false;
			bool flag2 = false;
			LogicalDeviceList logicalDeviceList = new LogicalDeviceList();
			this._arSupportedLogicalBusSystems.Clear();
			foreach (XmlNode childNode in xnDevice.ChildNodes)
			{
				if (childNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				switch (childNode.Name)
				{
					case "LogicalDevice":
						_lLogicalLanguageModelPositionId = ((DefaultUniqueIdGenerator)_idGenerator).GetNext(true);
						if (bUpdate)
						{
							logicalDeviceList.Add(new LogicalMappedDevice((XmlElement)childNode));
						}
						else
						{
							_logicalDevices.Add(new LogicalMappedDevice((XmlElement)childNode));
						}
						break;
					case "SupportedLogicalBusSystems":
						_arSupportedLogicalBusSystems.Add(childNode.InnerText);
						break;
					case "ConnectorGroup":
						{
							if (childNode.ChildNodes == null || childNode.ChildNodes.Count < 2)
							{
								throw new Exception("At least 2 connectors are required below ConnectorGroup");
							}
							LList<XmlElement> val3 = new LList<XmlElement>();
							foreach (XmlNode childNode2 in childNode.ChildNodes)
							{
								if ((int)ConnectorBase.ParseConnectorRole(((XmlElement)childNode2).GetAttribute(ConnectorTagAttributes.Role)) != 0)
								{
									throw new Exception("Only parent connectors are allowed inside a connector group");
								}
								if (DeviceObjectHelper.ParseBool(((XmlElement)childNode2).GetAttribute("explicit"), bDefault: false))
								{
									throw new Exception("explicit connector are not possible inside a connector group");
								}
								llist.Add((XmlElement)childNode2);
								val3.Add((XmlElement)childNode2);
							}
							num2++;
							ldictionary.Add(num2, val3);
							break;
						}
					case "Connector":
						llist.Add((XmlElement)childNode);
						break;
					case "DeviceParameterSet":
						_stFixedInputAddress = ((XmlElement)childNode).GetAttribute(ConnectorTagAttributes.FixedInputAddress);
						_stFixedOutputAddress = ((XmlElement)childNode).GetAttribute(ConnectorTagAttributes.FixedOutputAddress);
						_bDownloadParamsDevDescOrder = DeviceObjectHelper.ParseBool(((XmlElement)childNode).GetAttribute(ConnectorTagAttributes.DownloadParamsDevDescOrder), bDefault: false);
						if (bUpdate && bVersionUpgrade)
						{
							_deviceParameterSet.Update(childNode, types, -1, bCreateBitChannels);
						}
						else
						{
							_deviceParameterSet = new ParameterSet(childNode, types, -1, bCreateBitChannels);
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
					case "Functional":
						ReadFunctionalSection(childNode, bUpdate);
						break;
					case "Custom":
						flag2 = true;
						_customItems = new CustomItemList((XmlElement)childNode);
						break;
				}
			}
			if (!flag && bUpdate)
			{
				this._driverInfo = new DriverInfo();
			}
			if (!flag2 && bUpdate)
			{
				this._customItems = new CustomItemList();
			}
			if (bUpdate)
			{
				LogicalDeviceList logicalDeviceList2 = new LogicalDeviceList();
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(_logicalDevices);
				foreach (LogicalMappedDevice item in logicalDeviceList)
				{
					bool flag3 = false;
					foreach (LogicalMappedDevice item2 in arrayList)
					{
						bool flag4 = true;
						if (item2.MatchingLogicalDevices.Count == item.MatchingLogicalDevices.Count)
						{
							foreach (MatchingLogicalDevice matchingLogicalDevice in item2.MatchingLogicalDevices)
							{
								foreach (MatchingLogicalDevice matchingLogicalDevice2 in item.MatchingLogicalDevices)
								{
									if (!((object)matchingLogicalDevice2.DeviceIdentification).Equals((object)matchingLogicalDevice.DeviceIdentification))
									{
										flag4 = false;
									}
								}
							}
						}
						else
						{
							flag4 = false;
						}
						if (flag4)
						{
							logicalDeviceList2.Add(item2);
							flag3 = true;
							arrayList.Remove(item2);
							break;
						}
					}
					if (!flag3)
					{
						logicalDeviceList2.Add(item);
					}
				}
				_logicalDevices = logicalDeviceList2;
			}
			LList<Connector> llist3;
			if (bUpdate)
			{
				foreach (object obj6 in this.Connectors)
				{
					IConnector connector = (IConnector)obj6;
					foreach (object obj7 in DeviceObjectHelper.GetParameterUpdateFactories(this, connector.ConnectorId))
					{
						((IUpdateDeviceParametersFactory)obj7).StartUpdateParameters(this, connector);
					}
				}
				llist3 = this.UpdateConnectors(llist, types, bVersionUpgrade, this._deviceId, devicesToUpdate, bCreateBitChannels);
				using (IEnumerator<Connector> enumerator6 = llist3.GetEnumerator())
				{
					while (enumerator6.MoveNext())
					{
						Connector connector2 = enumerator6.Current;
						if (connector2.ConnectorRole == ConnectorRole.Parent)
						{
							connector2.ConnectorGroup = 0U;
							foreach (KeyValuePair<uint, LList<XmlElement>> keyValuePair in ldictionary)
							{
								foreach (XmlElement xmlElement in keyValuePair.Value)
								{
									int num4 = DeviceObjectHelper.ParseInt(xmlElement.GetAttribute(ConnectorTagAttributes.ConnectorId), -1);
									if (connector2.ConnectorId == num4)
									{
										connector2.ConnectorGroup = keyValuePair.Key;
									}
								}
							}
						}
						LList<object> parameterUpdateFactories = DeviceObjectHelper.GetParameterUpdateFactories(this, connector2.ConnectorId);
						if (connector2 != null)
						{
							connector2.Device = this;
						}
						foreach (object obj8 in parameterUpdateFactories)
						{
							((IUpdateDeviceParametersFactory)obj8).EndUpdateParameters(this, connector2);
						}
					}
					goto IL_803;
				}
			}
			llist3 = new LList<Connector>();
			foreach (XmlElement xmlElement2 in llist)
			{
				Connector connector3 = new Connector(xmlElement2, types, this._deviceId, bCreateBitChannels);
				foreach (KeyValuePair<uint, LList<XmlElement>> keyValuePair2 in ldictionary)
				{
					if (keyValuePair2.Value.Contains(xmlElement2))
					{
						connector3.ConnectorGroup = keyValuePair2.Key;
					}
				}
				llist3.Add(connector3);
			}
		IL_803:
			num = -1;
			foreach (Connector connector4 in llist3)
			{
				num = Math.Max(connector4.ConnectorId, num);
			}
			foreach (Connector connector5 in llist3)
			{
				if (connector5.ConnectorId < 0)
				{
					num++;
					connector5.ConnectorId = num;
				}
				this._connectors.Add(connector5);
			}
			return true;
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00030038 File Offset: 0x0002F038
		internal bool UpdateConnectors(string stOldInterface, string stNewInterface, ushort usOldModuleType, ushort usNewModuleType)
		{
			try
			{
				if (APEnvironment.DeviceMgr.GenericInterfaceExtensionProvider != null)
				{
					XmlDocument xmlDocument = new XmlDocument();
					XmlNode xmlNode = xmlDocument.AppendChild(xmlDocument.CreateElement("UpdateConnectors"));
					xmlNode.AppendChild(xmlDocument.CreateElement("OldInterface")).InnerText = stOldInterface;
					xmlNode.AppendChild(xmlDocument.CreateElement("NewInterface")).InnerText = stNewInterface;
					xmlNode.AppendChild(xmlDocument.CreateElement("OldModuleType")).InnerText = XmlConvert.ToString(usOldModuleType);
					xmlNode.AppendChild(xmlDocument.CreateElement("NewModuleType")).InnerText = XmlConvert.ToString(usNewModuleType);
					APEnvironment.DeviceMgr.RaiseEvent("UpdateConnectors", xmlDocument);
				}
			}
			catch
			{
				return false;
			}
			using (IEnumerator<IUpdateConnectorsFactory> enumerator = APEnvironment.UpdateConnectorsFactories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.UpdateConnectors(stOldInterface, stNewInterface, usOldModuleType, usNewModuleType))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00030140 File Offset: 0x0002F140
		private LList<Connector> UpdateConnectors(LList<XmlElement> connectortags, TypeList types, bool bVersionUpgrade, IDeviceIdentification deviceId, LList<DeviceIdUpdate> devicesToUpdate, bool bCreateBitChannels)
		{
			LList<Guid> llist = new LList<Guid>();
			LList<Connector> llist2 = new LList<Connector>();
			ConnectorList connectorList = (ConnectorList)this._connectors.Clone();
			for (int i = connectortags.Count - 1; i >= 0; i--)
			{
				XmlElement xmlElement = connectortags[i];
				ushort num;
				string text;
				ConnectorRole connectorRole;
				int num2;
				this.GetConnectorAttributes(xmlElement, out num, out text, out connectorRole, out num2);
				if (this._connectors.ContainsConnectorId(num2))
				{
					Connector connector = (Connector)this._connectors.GetById(num2);
					if (connector.ConnectorRole == connectorRole && DeviceManager.CompareInterfaces(connector.Interface, text) && connector.ModuleType == (int)num)
					{
						if (!this.UpdateConnectors(connector.Interface, text, (ushort)connector.ModuleType, num))
						{
							break;
						}
						connector.Update(xmlElement, types, llist, devicesToUpdate, this._deviceId, bVersionUpgrade, bCreateBitChannels);
						llist2.Insert(0, (Connector)connector.Clone());
						this._connectors.Remove(connector);
						connectortags.RemoveAt(i);
					}
				}
			}
			for (int j = connectortags.Count - 1; j >= 0; j--)
			{
				XmlElement xmlElement2 = connectortags[j];
				ushort num;
				string text;
				ConnectorRole connectorRole;
				int num2;
				this.GetConnectorAttributes(xmlElement2, out num, out text, out connectorRole, out num2);
				foreach (object obj in this._connectors)
				{
					Connector connector2 = (Connector)obj;
					if (connector2.ConnectorRole == connectorRole && DeviceManager.CompareInterfaces(connector2.Interface, text) && connector2.ModuleType == (int)num)
					{
						if (!this.UpdateConnectors(connector2.Interface, text, (ushort)connector2.ModuleType, num))
						{
							break;
						}
						connector2.Update(xmlElement2, types, llist, devicesToUpdate, this._deviceId, bVersionUpgrade, bCreateBitChannels);
						llist2.Insert(0, (Connector)connector2.Clone());
						this._connectors.Remove(connector2);
						connectortags.RemoveAt(j);
						break;
					}
				}
			}
			for (int k = connectortags.Count - 1; k >= 0; k--)
			{
				XmlElement xmlElement3 = connectortags[k];
				ushort num;
				string text;
				ConnectorRole connectorRole;
				int num2;
				this.GetConnectorAttributes(xmlElement3, out num, out text, out connectorRole, out num2);
				LDictionary<int, int> ldictionary = new LDictionary<int, int>();
				foreach (object obj2 in this._connectors)
				{
					Connector connector3 = (Connector)obj2;
					if (ldictionary.ContainsKey(connector3.ModuleType))
					{
						ldictionary[connector3.ModuleType] = ldictionary[connector3.ModuleType] + 1;
					}
					else
					{
						ldictionary[connector3.ModuleType] = 1;
					}
				}
				foreach (object obj3 in this._connectors)
				{
					Connector connector4 = (Connector)obj3;
					if (connector4.ConnectorRole == connectorRole && (DeviceManager.CompareInterfaces(connector4.Interface, text) || connector4.ModuleType == (int)num))
					{
						int num3;
						if (connector4.Interface != text && ldictionary.TryGetValue(connector4.ModuleType, out num3) && num3 > 1)
						{
							break;
						}
						if (!this.UpdateConnectors(connector4.Interface, text, (ushort)connector4.ModuleType, num))
						{
							break;
						}
						connector4.Update(xmlElement3, types, llist, devicesToUpdate, this._deviceId, bVersionUpgrade, bCreateBitChannels);
						llist2.Insert(0, (Connector)connector4.Clone());
						this._connectors.Remove(connector4);
						connectortags.RemoveAt(k);
						break;
					}
					else if (connector4.ConnectorRole == connectorRole && connector4.ConnectorId == num2 && connector4.Interface.Contains(".") && text.Contains("."))
					{
						string value = text.Substring(0, text.LastIndexOf('.'));
						if (connector4.Interface.StartsWith(value))
						{
							if (!this.UpdateConnectors(connector4.Interface, text, (ushort)connector4.ModuleType, num))
							{
								break;
							}
							connector4.Update(xmlElement3, types, llist, devicesToUpdate, this._deviceId, bVersionUpgrade, bCreateBitChannels);
							llist2.Insert(0, (Connector)connector4.Clone());
							this._connectors.Remove(connector4);
							connectortags.RemoveAt(k);
							break;
						}
					}
				}
			}
			foreach (XmlElement node in connectortags)
			{
				llist2.Add(new Connector(node, types, this._deviceId, bCreateBitChannels));
			}
			int nHostPath = -1;
			List<Connector> list = new List<Connector>();
			foreach (object obj4 in this._connectors)
			{
				Connector connector5 = (Connector)obj4;
				foreach (Connector connector6 in llist2)
				{
					if (DeviceManager.CheckMatchInterface(connector5, connector6))
					{
						if (!this.UpdateConnectors(connector5.Interface, connector6.Interface, (ushort)connector5.ModuleType, (ushort)connector6.ModuleType))
						{
							break;
						}
						LList<object> llist3 = new LList<object>();
						LList<object> llist4 = new LList<object>();
						LList<object> llist5 = new LList<object>();
						connector5.GetDevices(connector5.Adapters as AdapterList, llist3, llist4, llist5);
						foreach (object obj5 in connector6.Adapters)
						{
							IAdapterBase adapterBase = (IAdapterBase)obj5;
							if (adapterBase is FixedAdapter)
							{
								adapterBase.UpdateModules(llist3, connector6, true);
								if (llist4.Count > 0)
								{
									adapterBase.UpdateModules(llist4, connector6, true);
								}
							}
							else if (adapterBase is SlotAdapter)
							{
								adapterBase.UpdateModules(llist4, connector6, true);
								if (llist3.Count > 0)
								{
									adapterBase.UpdateModules(llist3, connector6, true);
								}
							}
							else if (adapterBase is VarAdapter)
							{
								adapterBase.UpdateModules(llist5, connector6, true);
								if (llist3.Count > 0)
								{
									adapterBase.UpdateModules(llist3, connector6, true);
								}
								if (llist4.Count > 0)
								{
									adapterBase.UpdateModules(llist4, connector6, true);
								}
							}
						}
						nHostPath = connector5.HostPath;
						foreach (object obj6 in llist3)
						{
							IDeviceObject deviceObject = (IDeviceObject)obj6;
							llist.Add(deviceObject.MetaObject.ObjectGuid);
						}
						foreach (object obj7 in llist4)
						{
							IDeviceObject deviceObject2 = (IDeviceObject)obj7;
							llist.Add(deviceObject2.MetaObject.ObjectGuid);
						}
						foreach (object obj8 in llist5)
						{
							IDeviceObject deviceObject3 = (IDeviceObject)obj8;
							llist.Add(deviceObject3.MetaObject.ObjectGuid);
						}
						list.Add(connector5);
						break;
					}
				}
			}
			foreach (Connector conRemove in list)
			{
				this._connectors.Remove(conRemove);
			}
			foreach (object obj9 in this._connectors)
			{
				Connector connector7 = (Connector)obj9;
				foreach (object obj10 in connector7.Adapters)
				{
					IAdapter adapter = (IAdapter)obj10;
					if ((adapter is FixedAdapter || adapter is SlotAdapter) && adapter.ModulesCount > 0)
					{
						nHostPath = connector7.HostPath;
						llist.AddRange(adapter.Modules);
					}
					if (adapter is IVarAdapter && adapter.ModulesCount > 0)
					{
						nHostPath = connector7.HostPath;
						llist.AddRange(adapter.Modules);
					}
				}
			}
			if (llist.Count > 0)
			{
				ErrorConnector errorConnector = new ErrorConnector(types, deviceId, nHostPath);
				errorConnector.AddModules(llist);
				llist2.Add(errorConnector);
			}
			this._connectors.Clear();
			return llist2;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00030B18 File Offset: 0x0002FB18
		private void GetConnectorAttributes(XmlElement elemConnector, out ushort usModuleType, out string stInterface, out ConnectorRole role, out int nConnectorId)
		{
			usModuleType = ushort.Parse(elemConnector.GetAttribute(ConnectorTagAttributes.ModuleType));
			stInterface = elemConnector.GetAttribute(ConnectorTagAttributes.Interface);
			role = ConnectorBase.ParseConnectorRole(elemConnector.GetAttribute(ConnectorTagAttributes.Role));
			nConnectorId = DeviceObjectHelper.ParseInt(elemConnector.GetAttribute(ConnectorTagAttributes.ConnectorId), -1);
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00030B6C File Offset: 0x0002FB6C
		internal void ReadFunctionalSection(XmlNode node, bool bUpdate)
		{
			this._attributes.Clear();
			foreach (object obj in node.ChildNodes)
			{
				XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
				if (xmlElement != null && xmlElement.Name == "Attribute")
				{
					string attribute = xmlElement.GetAttribute("name");
					string text = xmlElement.InnerText;
					if (text == null)
					{
						text = "";
					}
					if (attribute != "")
					{
						this._attributes.AddAttribute(attribute, text);
					}
				}
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x00030C1C File Offset: 0x0002FC1C
		public ITypeList TypeDefinitions
		{
			get
			{
				return this.Types;
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x00030C24 File Offset: 0x0002FC24
		internal TypeList Types
		{
			get
			{
				if (this._typeList == null)
				{
					this.LoadTypes();
				}
				return this._typeList;
			}
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00030C3C File Offset: 0x0002FC3C
		internal void InsertChild(int nIndex, IObject childDevice, Guid newObjectGuid)
		{
			if (this._metaObject == null)
			{
				throw new InvalidOperationException("Object not part of the project");
			}
			if (!this._metaObject.IsToModify)
			{
				throw new InvalidOperationException("Device is not modifiable");
			}
			if (this.IsFunctionalType(childDevice.GetType()))
			{
				return;
			}
			foreach (object obj in this._connectors)
			{
				Connector connector = (Connector)obj;
				if (childDevice.MetaObject != null)
				{
					if (connector.SetExpectedModule(childDevice.MetaObject.ObjectGuid))
					{
						return;
					}
				}
				else if (connector.IsExpectedModule(newObjectGuid))
				{
					return;
				}
			}
			int num;
			if (nIndex == -1)
			{
				if (childDevice.MetaObject != null)
				{
					num = this.MetaObject.SubObjectGuids.Length - 1;
				}
				else
				{
					num = this.MetaObject.SubObjectGuids.Length;
				}
			}
			else
			{
				num = nIndex;
			}
			int numberOfFunctionalChildren = this.GetNumberOfFunctionalChildren();
			if (num < numberOfFunctionalChildren)
			{
				throw new ArgumentException("Invalid insert position");
			}
			int num2 = -1;
			try
			{
				if (childDevice is DeviceObject)
				{
					num2 = (childDevice as DeviceObject).ConnectorIDForChild;
				}
				if (childDevice is SlotDeviceObject)
				{
					num2 = (childDevice as SlotDeviceObject).ConnectorIDForChild;
				}
			}
			catch
			{
			}
			num -= numberOfFunctionalChildren;
			foreach (object obj2 in this.ConnectorList(true))
			{
				Connector connector2 = (Connector)obj2;
				if ((num2 == -1 || connector2.ConnectorId == num2) && connector2.InsertChild(num, childDevice, newObjectGuid))
				{
					return;
				}
				num -= connector2.SubObjectsCount;
				if (num < 0)
				{
					break;
				}
			}
			throw new DeviceObjectException(DeviceObjectExeptionReason.DeviceInsertImpossible, Strings.Error_Device_Insert);
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00004152 File Offset: 0x00003152
		[Obsolete("Don't use anymore. See IDeviceObject.InsertDevice()")]
		public void InsertDevice(int nInsertPosition, IDeviceObject device)
		{
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x00030E00 File Offset: 0x0002FE00
		internal void RemoveChild(Guid guidChild)
		{
			foreach (object obj in this.Connectors)
			{
				Connector connector = (Connector)obj;
				connector.RemoveChild(guidChild);
				if (connector.ModulesCount == 0)
				{
					ParameterSet parameterSet = connector.ParameterSet as ParameterSet;
					for (int i = 0; i < parameterSet.Count; i++)
					{
						Parameter parameter = parameterSet[i] as Parameter;
						if (parameter.CreateInHostConnector && parameterSet != null)
						{
							parameterSet.RemoveParameter(parameter.Id);
							i = -1;
						}
					}
				}
			}
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00030EAC File Offset: 0x0002FEAC
		public void SetParent(Guid guidParent, int nConnectorId)
		{
			foreach (Connector connector in _connectors)
			{
				if (connector.SetExpectedModule(guidParent))
				{
					return;
				}
			}
			Connector connectorById = this.GetConnectorById(nConnectorId);
			if (connectorById == null || connectorById.ConnectorRole != ConnectorRole.Child)
			{
				throw new ArgumentException("Invalid connector id");
			}
			foreach (object obj in connectorById.Adapters)
			{
				IAdapter adapter = (IAdapter)obj;
				if (adapter is SlotAdapter)
				{
					IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._metaObject.ProjectHandle, guidParent);
					((SlotAdapter)adapter).AddModule(0, metaObjectStub);
					return;
				}
			}
			throw new ArgumentException("No empty slot available");
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x00030FA8 File Offset: 0x0002FFA8
		public IIoModuleIterator CreateModuleIterator()
		{
			return new IoModuleIterator(new IoModuleDeviceReference(this._metaObject.ObjectGuid, this._metaObject.ProjectHandle));
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x00030FCA File Offset: 0x0002FFCA
		// (set) Token: 0x06000683 RID: 1667 RVA: 0x00030FD2 File Offset: 0x0002FFD2
		public Guid[] FunctionalChildrenTypeGuids
		{
			get
			{
				return this._funcChildrenTypeGuids;
			}
			set
			{
				this._funcChildrenTypeGuids = value;
			}
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00030FDC File Offset: 0x0002FFDC
		private void SetPositionIds()
		{
			this._idGenerator.Use(4096L);
			this._driverInfo.SetPositionIds(this._idGenerator);
			this._deviceParameterSet.SetPositionIds(this._idGenerator);
			foreach (object obj in this._connectors)
			{
				((Connector)obj).SetPositionIds(this._idGenerator);
			}
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0003106C File Offset: 0x0003006C
		private XmlNode FindModuleNode(XmlNode node, string stModuleId)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "Module")
				{
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name == "ModuleId" && stModuleId.Equals(xmlNode2.InnerText))
						{
							return xmlNode;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x00031158 File Offset: 0x00030158
		private XmlNode GetDeviceNode()
		{
			return APEnvironment.DeviceRepository.GetXmlDeviceNode(this._deviceId);
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0003116C File Offset: 0x0003016C
		public XmlNode GetFunctionalSection()
		{
			XmlNode deviceNode = this.GetDeviceNode();
			if (deviceNode == null)
			{
				return null;
			}
			foreach (object obj in deviceNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "Functional")
				{
					return xmlNode;
				}
			}
			return null;
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x000311F0 File Offset: 0x000301F0
		public XmlNode GetUpdateSection()
		{
			XmlNode deviceNode = this.GetDeviceNode();
			if (deviceNode == null)
			{
				return null;
			}
			foreach (object obj in deviceNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "UpdateDevice")
				{
					return xmlNode;
				}
			}
			return null;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00031274 File Offset: 0x00030274
		public LList<IDeviceIdentification> GetUpdateDevices()
		{
			LList<IDeviceIdentification> llist = new LList<IDeviceIdentification>();
			XmlNode updateSection = this.GetUpdateSection();
			if (updateSection != null)
			{
				foreach (object obj in updateSection.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "DeviceIdentification")
					{
						int num = 0;
						string text = null;
						string text2 = null;
						foreach (object obj2 in xmlNode.ChildNodes)
						{
							XmlNode xmlNode2 = (XmlNode)obj2;
							if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name == "Type")
							{
								string text3 = xmlNode2.InnerText.Trim();
								if (!string.IsNullOrEmpty(text3))
								{
									int.TryParse(text3, out num);
								}
							}
							if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name == "Id")
							{
								text = xmlNode2.InnerText.Trim();
							}
							if (xmlNode2.NodeType == XmlNodeType.Element && xmlNode2.Name == "Version")
							{
								text2 = xmlNode2.InnerText.Trim();
							}
						}
						if (num != 0 && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
						{
							IDeviceIdentification item = APEnvironment.DeviceRepository.CreateDeviceIdentification(num, text, text2);
							llist.Add(item);
						}
					}
				}
			}
			return llist;
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00031438 File Offset: 0x00030438
		public static void GetImplicitObjects(ArrayList childObjects, XmlNodeList nodes, Guid parentGuid)
		{
			foreach (object obj in nodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "ChildObject")
				{
					ChildObject childObject = new ChildObject(xmlNode, parentGuid);
					childObjects.Add(childObject);
					DeviceObject.GetImplicitObjects(childObjects, xmlNode.ChildNodes, childObject.Guid);
				}
			}
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x000314C4 File Offset: 0x000304C4
		internal static void CreateImplicitObjects(int nProjectHandle, Guid guidDevice, out bool bHasPlcLogic, bool bUpdate)
		{
			ArrayList arrayList = new ArrayList();
			bHasPlcLogic = false;
			try
			{
				IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(nProjectHandle, guidDevice);
				if (objectToRead.Object is IDeviceObjectBase)
				{
					XmlNode functionalSection = ((IDeviceObjectBase)objectToRead.Object).GetFunctionalSection();
					if (functionalSection != null)
					{
						DeviceObject.GetImplicitObjects(arrayList, functionalSection.ChildNodes, Guid.Empty);
					}
				}
				ImplicitObject[] array = new ImplicitObject[arrayList.Count];
				LList<Guid> llist = new LList<Guid>();
				int num = 0;
				foreach (object obj in arrayList)
				{
					ChildObject childObject = (ChildObject)obj;
					try
					{
						IObjectFactory factory = APEnvironment.ObjectMgr.ObjectFactoryManager.GetFactory(childObject.Guid);
						if (factory != null)
						{
							IObject obj2 = factory.Create(new string[0]);
							array[num] = new ImplicitObject(factory, obj2, childObject);
							if (childObject.ParentGuid == Guid.Empty)
							{
								llist.Add(TypeGuidAttribute.FromObject(obj2).Guid);
							}
						}
						else
						{
							array[num] = null;
						}
					}
					catch (Exception value)
					{
						array[num] = null;
						Debug.WriteLine(value);
					}
					num++;
				}
				if (num > 0)
				{
					IUndoManager undoManager = APEnvironment.ObjectMgr.GetUndoManager(nProjectHandle);
					SetFuncChildrenTypeGuidsAction setFuncChildrenTypeGuidsAction = new SetFuncChildrenTypeGuidsAction(nProjectHandle, guidDevice, llist.ToArray());
					undoManager.AddAction(setFuncChildrenTypeGuidsAction);
					setFuncChildrenTypeGuidsAction.Redo();
					num = 0;
					ImplicitObject[] array2 = array;
					int i = 0;
					while (i < array2.Length)
					{
						ImplicitObject implicitObject = array2[i];
						try
						{
							if (implicitObject != null)
							{
								Guid guid = guidDevice;
								if (implicitObject.Description.ParentGuid != Guid.Empty)
								{
									foreach (ImplicitObject implicitObject2 in array)
									{
										if (implicitObject2 != null && implicitObject2.Description.Guid == implicitObject.Description.ParentGuid)
										{
											guid = implicitObject2.DeviceGuid;
											break;
										}
									}
								}
								if (bUpdate)
								{
									foreach (Guid guid2 in APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, guid).SubObjectGuids)
									{
										IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, guid2);
										if (metaObjectStub != null && metaObjectStub.ObjectType.IsAssignableFrom(implicitObject.Object.GetType()) && metaObjectStub.Name == implicitObject.Description.Name)
										{
											implicitObject.DeviceGuid = guid2;
											break;
										}
									}
									if (implicitObject.DeviceGuid != Guid.Empty)
									{
										goto IL_2BC;
									}
								}
								Guid deviceGuid = APEnvironment.ObjectMgr.AddObject(nProjectHandle, guid, Guid.NewGuid(), implicitObject.Object, implicitObject.Description.Name, -1);
								implicitObject.NewlyCreated = true;
								implicitObject.DeviceGuid = deviceGuid;
								if (implicitObject.Object is IPlcLogicObject)
								{
									bHasPlcLogic = true;
								}
							}
						}
						catch (Exception value2)
						{
							Debug.WriteLine(value2);
						}
						goto IL_2B8;
					IL_2BC:
						i++;
						continue;
					IL_2B8:
						num++;
						goto IL_2BC;
					}
					foreach (ImplicitObject implicitObject3 in array)
					{
						try
						{
							if (implicitObject3 != null && implicitObject3.NewlyCreated)
							{
								implicitObject3.Factory.ObjectCreated(nProjectHandle, implicitObject3.DeviceGuid);
							}
						}
						catch (Exception value3)
						{
							Debug.WriteLine(value3);
						}
					}
				}
			}
			catch (Exception value4)
			{
				Debug.WriteLine(value4);
			}
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0003186C File Offset: 0x0003086C
		public Connector GetConnectorById(int nId)
		{
			foreach (object obj in this._connectors)
			{
				Connector connector = (Connector)obj;
				if (connector.ConnectorId == nId)
				{
					return connector;
				}
			}
			return null;
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x000318D0 File Offset: 0x000308D0
		public static bool IsIdentifier(string stText)
		{
			IScanner scanner = APEnvironment.LanguageModelMgr.CreateScanner(stText, true, true, true, true);
			Debug.Assert(scanner != null);
			IToken token;
			return scanner.Match(TokenType.Identifier, true, out token) > 0;
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x00031904 File Offset: 0x00030904
		public override void BeforeSerialize()
		{
			base.BeforeSerialize();
			if (this._metaObject != null)
			{
				this._hostObjectGuid = Guid.Empty;
				IDeviceObject hostDeviceObject = this.GetHostDeviceObject();
				if (hostDeviceObject != null && hostDeviceObject.MetaObject != null)
				{
					this._hostObjectGuid = hostDeviceObject.MetaObject.ObjectGuid;
				}
			}
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x00031950 File Offset: 0x00030950
		public override void AfterDeserialize()
		{
			base.AfterDeserialize();
			if (this._liAdditionalStringTable != null)
			{
				IDeviceDescription2 deviceDescription = APEnvironment.DeviceRepository.GetDevice(this._deviceId) as IDeviceDescription2;
				if (deviceDescription != null && deviceDescription.StringTable != null)
				{
					foreach (LanguageStringRef languageStringRef in this._liAdditionalStringTable)
					{
						deviceDescription.StringTable.AddLocalizedString(languageStringRef.Namespace, languageStringRef.Language, languageStringRef.Identifier, languageStringRef.Default);
					}
				}
			}
			if (this._deviceParameterSet == null)
			{
				this._deviceParameterSet = new ParameterSet();
			}
			this._deviceParameterSet.ConnectorId = -1;
			if (this._alOldConnectors.Count > 0)
			{
				Debug.Assert(this._connectors.Count == 0);
				this._connectors.AddRange(this._alOldConnectors);
				this._alOldConnectors.Clear();
			}
			this.UpdateDependentObjects(true);
			if (this._defaultDeviceInfo == null)
			{
				this._defaultDeviceInfo = new DefaultDeviceInfo();
			}
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x00031A60 File Offset: 0x00030A60
		public override void AfterClone()
		{
			base.AfterClone();
			this.UpdateDependentObjects(false);
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x00031A6F File Offset: 0x00030A6F
		internal bool NoIoDownload
		{
			get
			{
				return DeviceObjectHelper.ParseBool(this.Attributes["NoIoDownload"], false);
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x00031A87 File Offset: 0x00030A87
		internal bool UseParentPLC
		{
			get
			{
				return DeviceObjectHelper.ParseBool(this.Attributes["UseParentPLC"], false);
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x00031A9F File Offset: 0x00030A9F
		internal bool SkipInsertLibrary
		{
			get
			{
				return DeviceObjectHelper.ParseBool(this.Attributes["SkipInsertLibrary"], false);
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x00031AB8 File Offset: 0x00030AB8
		internal ChannelType LogicalGVLAssignmentErrorType
		{
			get
			{
				ChannelType result = ChannelType.Output;
				string text = this.Attributes["LogicalGVLAssignmentErrorType"];
				if (!string.IsNullOrEmpty(text) && text.ToLowerInvariant() == "input")
				{
					result = ChannelType.Input;
				}
				return result;
			}
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00031AF8 File Offset: 0x00030AF8
		internal void UpdateDependentObjects(bool bAfterDeserialized)
		{
			this._deviceParameterSet.SetIoProvider(this);
			this._deviceParameterSet.Device = this;
			if (this._communicationSettings != null)
			{
				this._communicationSettings.SetOwner(this);
				this._communicationSettings.InitializeSecureOnlineMode(DeviceObjectHelper.ParseBool(this.Attributes["SecureOnlineMode"], false));
			}
			foreach (object obj in this._connectors)
			{
				((Connector)obj).Device = this;
			}
			this._driverInfo.SetIoProvider(this);
			if (this._stFixedInputAddress != string.Empty)
			{
				this._ioProviderBase.SetUserBaseAddress(DirectVariableLocation.Input, this._stFixedInputAddress);
			}
			if (this._stFixedOutputAddress != string.Empty)
			{
				this._ioProviderBase.SetUserBaseAddress(DirectVariableLocation.Output, this._stFixedOutputAddress);
			}
			if (!bAfterDeserialized)
			{
				this.Strategy.UpdateAddresses(this);
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00031C00 File Offset: 0x00030C00
		private void LoadTypes()
		{
			if (this._typeList == null)
			{
				this._typeList = new TypeList();
			}
			if (this._deviceId == null)
			{
				return;
			}
			try
			{
				XmlNode xmlGlobalNode = APEnvironment.DeviceRepository.GetXmlGlobalNode(this._deviceId, "Types");
				if (xmlGlobalNode != null)
				{
					XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlGlobalNode);
					xmlNodeReader.Read();
					this._typeList.ReadTypes(xmlNodeReader, this._deviceId);
					xmlNodeReader.Close();
				}
			}
			catch (Exception value)
			{
				Debug.WriteLine(value);
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x00031C84 File Offset: 0x00030C84
		// (set) Token: 0x06000698 RID: 1688 RVA: 0x00031C8C File Offset: 0x00030C8C
		public string UserManagement
		{
			get
			{
				return this._stUserManagement;
			}
			set
			{
				this._stUserManagement = value;
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x00031C95 File Offset: 0x00030C95
		// (set) Token: 0x0600069A RID: 1690 RVA: 0x00031C9D File Offset: 0x00030C9D
		public string RightsManagement
		{
			get
			{
				return this._stRightsManagement;
			}
			set
			{
				this._stRightsManagement = value;
			}
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x00031CA6 File Offset: 0x00030CA6
		public void AttachToEvent(string stEvent, GenericEventDelegate callback)
		{
			if (this._baseGenericInterfaceExtensionProvider != null)
			{
				this._baseGenericInterfaceExtensionProvider.AttachToEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x00031CC3 File Offset: 0x00030CC3
		public void DetachFromEvent(string stEvent, GenericEventDelegate callback)
		{
			if (this._baseGenericInterfaceExtensionProvider != null)
			{
				this._baseGenericInterfaceExtensionProvider.DetachFromEvent(stEvent, callback);
				return;
			}
			throw new NotImplementedException();
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00031CE0 File Offset: 0x00030CE0
		public void RaiseEvent(string stEvent, XmlDocument eventData)
		{
			if (this._baseGenericInterfaceExtensionProvider != null)
			{
				this._baseGenericInterfaceExtensionProvider.RaiseEvent(stEvent, eventData);
				return;
			}
			throw new NotImplementedException();
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00031CFD File Offset: 0x00030CFD
		public bool IsFunctionAvailable(string stFunction)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			return stFunction == "GetBusTaskCycleTime" || stFunction == "GetBusTask" || stFunction == "AddConnector";
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00031D3C File Offset: 0x00030D3C
		public Guid GetBusTask(out long lCycleTime)
		{
			lCycleTime = 0L;
			Guid result = Guid.Empty;
			DeviceObject deviceObject = this.GetHostDeviceObject() as DeviceObject;
			if (deviceObject != null)
			{
				IPlcLogicObject plcLogicObject = deviceObject.GetPlcLogicObject();
				if (plcLogicObject != null)
				{
					IApplicationObject applicationObject = deviceObject.GetApplicationObject(plcLogicObject);
					if (applicationObject != null)
					{
						ITaskConfigObject taskConfigObject = deviceObject.GetTaskConfigObject(applicationObject);
						if (taskConfigObject != null)
						{
							ITaskObject[] tasks = taskConfigObject.Tasks;
							DeviceObject.TaskInfo[] array = new DeviceObject.TaskInfo[tasks.Length];
							for (int i = 0; i < tasks.Length; i++)
							{
								array[i] = new DeviceObject.TaskInfo(taskConfigObject.MetaObject.ObjectGuid, tasks[i].MetaObject.ObjectGuid, tasks[i].Name);
							}
							if (array.Length != 0)
							{
								int num = DeviceObject.GetBusTask(array, this._driverInfo);
								IIoProvider ioProvider = this;
								if (ioProvider != null)
								{
									string text = string.Empty;
									foreach (object obj in this._connectors)
									{
										string busCycleTask = ((IConnector2)obj).DriverInfo.BusCycleTask;
										if (busCycleTask != string.Empty && this.IsTaskValid(busCycleTask, array))
										{
											text = busCycleTask;
											break;
										}
									}
									if (text == string.Empty)
									{
										string busCycleTask2 = ioProvider.DriverInfo.BusCycleTask;
										if (busCycleTask2 != string.Empty && this.IsTaskValid(busCycleTask2, array))
										{
											text = busCycleTask2;
										}
									}
									if (text == string.Empty)
									{
										foreach (IConnector connector2 in this._connectors)
										{
											IConnector val2 = connector2;
											if ((int)val2.ConnectorRole != 1)
											{
												continue;
											}
											foreach (IAdapter item in (IEnumerable)val2.Adapters)
											{
												if (Array.IndexOf(item.Modules, _metaObject.ParentObjectGuid) < 0)
												{
													continue;
												}
												ioProvider = (IIoProvider)(object)((val2 is IIoProvider) ? val2 : null);
												if (ioProvider == null)
												{
													continue;
												}
												IIoProvider parent = ioProvider.Parent;
												while (parent != null && text == string.Empty)
												{
													if (parent.DriverInfo.BusCycleTask != string.Empty)
													{
														string busCycleTask3 = parent.DriverInfo.BusCycleTask;
														if (IsTaskValid(busCycleTask3, (ITaskInfo[])(object)array))
														{
															text = busCycleTask3;
															break;
														}
													}
													parent = parent.Parent;
												}
											}
										}
									}
									if (text != string.Empty)
									{
										for (int j = 0; j < tasks.Length; j++)
										{
											if (tasks[j].Name == text)
											{
												num = j;
												break;
											}
										}
									}
									if (num >= 0)
									{
										result = array[num].TaskGuid;
										KindOfTask kindOfTask;
										long num2;
										if (DeviceObject.GetTaskCycleTime(array[num].TaskGuid, out kindOfTask, out num2))
										{
											if (kindOfTask == KindOfTask.Freewheeling)
											{
												lCycleTime = 0L;
											}
											else
											{
												lCycleTime = num2;
											}
										}
										else if (kindOfTask == KindOfTask.Event || kindOfTask == KindOfTask.ExternalEvent || kindOfTask == KindOfTask.Status)
										{
											lCycleTime = -1L;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00032084 File Offset: 0x00031084
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
			if (!(stFunction == "AddConnector"))
			{
				if (!(stFunction == "GetBusTaskCycleTime") && !(stFunction == "GetBusTask"))
				{
					return xmlDocument;
				}
			}
			else
			{
				try
				{
					IMetaObject metaObject = null;
					try
					{
						metaObject = APEnvironment.ObjectMgr.GetObjectToModify(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
						if (metaObject != null)
						{
							DeviceObject deviceObject = metaObject.Object as DeviceObject;
							if (deviceObject != null)
							{
								ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this._deviceId);
								if (targetSettingsById != null)
								{
									this._bCreateBitChannels = targetSettingsById.GetBoolValue(LocalTargetSettings.CreateBitChannels.Path, this._bCreateBitChannels);
								}
								Connector connector = new Connector(functionData.DocumentElement, deviceObject._typeList, deviceObject._deviceId, this._bCreateBitChannels);
								connector.Device = deviceObject;
								deviceObject._connectors.Add(connector);
							}
						}
					}
					catch
					{
					}
					finally
					{
						if (metaObject != null && metaObject.IsToModify)
						{
							APEnvironment.ObjectMgr.SetObject(metaObject, true, null);
						}
					}
					if (this._metaObject != null && this._guidObject != Guid.Empty)
					{
						try
						{
							DeviceObjectHelper.BeginCreateDevice(this._guidObject);
							IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
							if (objectToRead != null && objectToRead.Object is IDeviceObjectBase)
							{
								DeviceObject.UpdateChildObjects(objectToRead.Object as IDeviceObjectBase, false, false, null);
							}
						}
						finally
						{
							DeviceObjectHelper.EndCreateDevice(this._guidObject);
						}
					}
					return xmlDocument;
				}
				catch
				{
					return xmlDocument;
				}
			}
			long value;
			Guid busTask = this.GetBusTask(out value);
			if (stFunction == "GetBusTask")
			{
				xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(busTask);
			}
			else
			{
				xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(value);
			}
			return xmlDocument;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x000322E4 File Offset: 0x000312E4
		internal bool IsTaskValid(string stTask, ITaskInfo[] taskinfos)
		{
			for (int i = 0; i < taskinfos.Length; i++)
			{
				if (taskinfos[i].TaskName.Equals(stTask))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x00032312 File Offset: 0x00031312
		public bool ShowParamsInDevDescOrder
		{
			get
			{
				return this._bShowParamsInDevDescOrder;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x0000464A File Offset: 0x0000364A
		public bool NeedsContextForLanguageModelProvision
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0003231C File Offset: 0x0003131C
		internal long GetPackMode()
		{
			long packMode = DeviceObjectHelper.GetPackMode(this._guidObject);
			if (packMode != 0L)
			{
				return packMode;
			}
			packMode = DeviceObjectHelper.GetPackMode(this.GetHostDeviceObject());
			DeviceObjectHelper.StorePackMode(this._guidObject, packMode);
			return packMode;
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x00032353 File Offset: 0x00031353
		internal bool DownloadParamsDevDescOrder
		{
			get
			{
				return this._bDownloadParamsDevDescOrder;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0003235B File Offset: 0x0003135B
		public LogicalDeviceList LogicalDevices
		{
			get
			{
				return this._logicalDevices;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x00032363 File Offset: 0x00031363
		public bool IsPhysical
		{
			get
			{
				return !(this is LogicalIODevice);
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x00032371 File Offset: 0x00031371
		public bool IsLogical
		{
			get
			{
				return this is LogicalIODevice;
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x0003235B File Offset: 0x0003135B
		public IMappedDeviceList MappedDevices
		{
			get
			{
				return this._logicalDevices;
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0003237C File Offset: 0x0003137C
		public long LanguageModelPositionId
		{
			get
			{
				return this._lLogicalLanguageModelPositionId;
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x00032384 File Offset: 0x00031384
		public string[] SupportedLogicalBusSystems
		{
			get
			{
				string[] array = new string[this._arSupportedLogicalBusSystems.Count];
				this._arSupportedLogicalBusSystems.CopyTo(array);
				return array;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x000323AF File Offset: 0x000313AF
		// (set) Token: 0x060006AD RID: 1709 RVA: 0x000323B7 File Offset: 0x000313B7
		public bool MappingPossible
		{
			get
			{
				return this._bMappingPossible;
			}
			set
			{
				this._bMappingPossible = value;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060006AE RID: 1710 RVA: 0x000323C0 File Offset: 0x000313C0
		// (set) Token: 0x060006AF RID: 1711 RVA: 0x000323C8 File Offset: 0x000313C8
		public bool HidePropertiesDialog
		{
			get
			{
				return this._bHidePropertiesDialog;
			}
			set
			{
				this._bHidePropertiesDialog = value;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x000323D4 File Offset: 0x000313D4
		internal LDictionary<IIoProvider, bool> LogicalGVLProviders
		{
			get
			{
				LSortedList<string, IIoProvider> lsortedList = new LSortedList<string, IIoProvider>();
				LDictionary<IIoProvider, bool> ldictionary = new LDictionary<IIoProvider, bool>();
				if (DeviceObjectHelper.GenerateCodeForLogicalDevices)
				{
					IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(this._metaObject.ProjectHandle, this._metaObject.ObjectGuid);
					if (LogicalIONotification.LogicalExchangeGVL != null)
					{
						Guid[] array = new Guid[LogicalIONotification.LogicalExchangeGVL.DeviceGuids.Count];
						LogicalIONotification.LogicalExchangeGVL.DeviceGuids.CopyTo(array, 0);
						foreach (Guid objectGuid in array)
						{
							if (APEnvironment.ObjectMgr.ExistsObject(this._metaObject.ProjectHandle, objectGuid))
							{
								IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(this._metaObject.ProjectHandle, objectGuid);
								if (typeof(ILogicalGVLObject).IsAssignableFrom(metaObjectStub.ObjectType))
								{
									IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(this._metaObject.ProjectHandle, objectGuid);
									if (hostStub.ObjectGuid == hostStub2.ObjectGuid)
									{
										IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(this._metaObject.ProjectHandle, objectGuid);
										ILogicalGVLObject logicalGVLObject = objectToRead.Object as ILogicalGVLObject;
										bool value = false;
										if (logicalGVLObject is LogicalExchangeGVLObject)
										{
											value = (logicalGVLObject as LogicalExchangeGVLObject).UseCombinedType;
										}
										if (logicalGVLObject != null && logicalGVLObject is ILogicalDevice)
										{
											foreach (object obj in (logicalGVLObject as ILogicalDevice).MappedDevices)
											{
												LogicalMappedDevice logicalMappedDevice = (LogicalMappedDevice)obj;
												if (logicalMappedDevice.IsMapped)
												{
													Guid getMappedDevice = logicalMappedDevice.GetMappedDevice;
													if (getMappedDevice != Guid.Empty)
													{
														IMetaObject objectToRead2 = APEnvironment.ObjectMgr.GetObjectToRead(this._metaObject.ProjectHandle, getMappedDevice);
														if (objectToRead2.Object is IDeviceObject)
														{
															foreach (object obj2 in (objectToRead2.Object as IDeviceObject).Connectors)
															{
																IConnector connector = (IConnector)obj2;
																if (connector.ConnectorRole != ConnectorRole.Child)
																{
																	lsortedList[objectToRead.Name] = (connector as IIoProvider);
																	if (!ldictionary.ContainsKey(connector as IIoProvider))
																	{
																		ldictionary.Add(connector as IIoProvider, value);
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				LDictionary<IIoProvider, bool> ldictionary2 = new LDictionary<IIoProvider, bool>();
				foreach (IIoProvider key in lsortedList.Values)
				{
					bool value2 = false;
					ldictionary.TryGetValue(key, out value2);
					if (!ldictionary2.ContainsKey(key))
					{
						ldictionary2.Add(key, value2);
					}
				}
				return ldictionary2;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x000326DC File Offset: 0x000316DC
		public bool ShowPropertiesDialog
		{
			get
			{
				return this.ExcludeFromBuildEnabled;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x000326E4 File Offset: 0x000316E4
		public bool ExcludeFromBuildEnabled
		{
			get
			{
				return !this._bHidePropertiesDialog && !this.AllowsDirectCommunication;
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0000464A File Offset: 0x0000364A
		public bool ExternalEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x0000464A File Offset: 0x0000364A
		public bool EnableSystemCallEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x0000464A File Offset: 0x0000364A
		public bool LinkAlwaysEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x0000464A File Offset: 0x0000364A
		public bool CompilerDefinesEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00020B35 File Offset: 0x0001FB35
		public bool GetChangedParametersInSet(out List<IParameter> changedParameters)
		{
			return LanguageModelHelper.FindChangedParametersInSet(this, out changedParameters);
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x000326FC File Offset: 0x000316FC
		public ILanguageModel GetStructuredLanguageModel(ILanguageModelBuilder lmbuilder)
		{
			List<List<string>> stringlistTable = null;
			ILanguageModel languageModel = lmbuilder.CreateLanguageModel(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty);
			string languageModel2 = this.GetLanguageModel2(lmbuilder, languageModel, out stringlistTable);
			ILanguageModel languageModel3 = lmbuilder.CreateLanguageModelOfXml(languageModel2, stringlistTable);
			if (languageModel3 != null)
			{
				foreach (ILMPOU lmpou in languageModel.Pous)
				{
					languageModel3.AddPou(lmpou);
				}
				foreach (ILMGlobVarlist lmgvl in languageModel.GlobalVariableLists)
				{
					languageModel3.AddGlobalVariableList(lmgvl);
				}
				foreach (ILMDataType lmdut in languageModel.DataTypes)
				{
					languageModel3.AddDataType(lmdut);
				}
			}
			return languageModel3;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x000327BC File Offset: 0x000317BC
		internal IDataElement2 GetParent(IDataElement dataElement)
		{
			DataElementCollection dataElementCollection = (dataElement as DataElementBase).Parent as DataElementCollection;
			if (dataElementCollection != null)
			{
				IDataElement dataElement2 = dataElementCollection.Parent as IDataElement;
				if (dataElement2 != null)
				{
					return dataElement2 as IDataElement2;
				}
			}
			return null;
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x000327F4 File Offset: 0x000317F4
		internal void SortTaskMappings(Dictionary<ITaskMappingInfo, List<ITaskMapping>> taskmapList)
		{
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq(3, 5, 2, 0))
			{
				foreach (KeyValuePair<ITaskMappingInfo, List<ITaskMapping>> keyValuePair in taskmapList)
				{
					LList<ITaskMapping> llist = new LList<ITaskMapping>();
					LDictionary<string, ITaskMapping> ldictionary = new LDictionary<string, ITaskMapping>();
					LDictionary<IDataElement, string> ldictionary2 = new LDictionary<IDataElement, string>();
					foreach (ITaskMapping taskMapping in keyValuePair.Value)
					{
						if (!taskMapping.MapToExisiting)
						{
							ldictionary[(taskMapping as Mapping).IecVar] = taskMapping;
						}
					}
					foreach (ITaskMapping taskMapping2 in keyValuePair.Value)
					{
						if (!llist.Contains(taskMapping2))
						{
							List<ITaskMapping> list = new List<ITaskMapping>();
							if (!taskMapping2.MapToExisiting && taskMapping2.DataElement is DataElementBitFieldComponent)
							{
								IDataElement2 parent = this.GetParent(taskMapping2.DataElement);
								if (parent != null && parent.HasBaseType)
								{
									ITaskMapping taskMapping3 = taskMapping2;
									foreach (object obj in parent.SubElements)
									{
										IDataElement dataElement = (IDataElement)obj;
										string text;
										if (!ldictionary2.TryGetValue(dataElement, out text))
										{
											text = (dataElement.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses).ToString();
											ldictionary2.Add(dataElement, text);
										}
										ITaskMapping taskMapping4;
										if (ldictionary.TryGetValue(text, out taskMapping4))
										{
											if (taskMapping4.ParameterBitOffset < taskMapping3.ParameterBitOffset)
											{
												taskMapping3 = taskMapping4;
											}
											list.Add(taskMapping4);
										}
									}
									if (parent.SubElements.Count == list.Count)
									{
										list.Remove(taskMapping3);
										llist.AddRange(list);
										taskMapping3.BitSize = (int)parent.GetBitSize();
										taskMapping3.DataType = parent.BaseType;
										(taskMapping3 as Mapping).DataElement = parent;
										string text2 = (parent.IoMapping as IoMapping).GetIecAddress(DeviceObjectHelper.HashIecAddresses).ToString();
										(taskMapping3 as Mapping).IecVar = text2;
										taskMapping3.MappedAddress = "ADR(" + text2 + ")";
									}
								}
							}
						}
					}
					if (llist.Count > 0)
					{
						foreach (ITaskMapping item in llist)
						{
							keyValuePair.Value.Remove(item);
						}
					}
				}
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x00032B28 File Offset: 0x00031B28
		public bool InheritedDisable
		{
			get
			{
				IDeviceObject13 deviceObject = (IDeviceObject13)DeviceObjectHelper.GetParentDeviceObject(this);
				if (deviceObject != null)
				{
					return this.Disable | deviceObject.InheritedDisable;
				}
				return this.Disable;
			}
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00003E50 File Offset: 0x00002E50
		public string GetOnlineHelpKeyword()
		{
			return null;
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00032B58 File Offset: 0x00031B58
		public string GetOnlineHelpUrl()
		{
			return this.OnlineHelpUrl;
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00032B60 File Offset: 0x00031B60
		internal void AddPlaceholderResolution(string stPlaceholder, string stResolution)
		{
			if (this._dictPlaceholderResolutions == null)
			{
				this._dictPlaceholderResolutions = new LDictionary<string, string>();
			}
			this._dictPlaceholderResolutions[stPlaceholder] = stResolution;
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x00032B82 File Offset: 0x00031B82
		// (set) Token: 0x060006C0 RID: 1728 RVA: 0x00032B8A File Offset: 0x00031B8A
		internal LDictionary<string, string> PlaceholderResolutions
		{
			get
			{
				return this._dictPlaceholderResolutions;
			}
			set
			{
				this._dictPlaceholderResolutions = value;
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00032B93 File Offset: 0x00031B93
		// (set) Token: 0x060006C2 RID: 1730 RVA: 0x00032B9C File Offset: 0x00031B9C
		public bool UseDeviceApplicationStructure
		{
			get
			{
				return this._bUseDeviceApplicationStructure;
			}
			set
			{
				if (value)
				{
					ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(this._deviceId);
					if (targetSettingsById != null && LocalTargetSettings.DeviceApplicationDisabled.GetBoolValue(targetSettingsById))
					{
						throw new InvalidOperationException(Strings.ErrorDoesNotSupportDeviceApplicationStructure);
					}
				}
				this._bUseDeviceApplicationStructure = value;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00032BE4 File Offset: 0x00031BE4
		// (set) Token: 0x060006C4 RID: 1732 RVA: 0x00032BEC File Offset: 0x00031BEC
		public bool AllowSymbolicVarAccessInSyncWithIecCycle
		{
			get
			{
				return this._bAllowSymbolicVarAccessInSyncWithIecCycle;
			}
			set
			{
				this._bAllowSymbolicVarAccessInSyncWithIecCycle = value;
			}
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00032BF8 File Offset: 0x00031BF8
		public string PlaceholderResolution(string stPlaceholder)
		{
			string result = null;
			if (!DeviceObjectHelper.SkipPlaceholderResolution && this._dictPlaceholderResolutions != null)
			{
				this._dictPlaceholderResolutions.TryGetValue(stPlaceholder, out result);
			}
			return result;
		}

		// Token: 0x040001E1 RID: 481
		internal static readonly Guid GUID_DEVICENAMESPACE = new Guid("{1ee21fdd-5562-44a0-a3ce-665d74916d50}");

		// Token: 0x040001E2 RID: 482
		internal const string ATTRIBUTE_NOIODOWNLOAD = "NoIoDownload";

		// Token: 0x040001E3 RID: 483
		internal const string ATTRIBUTE_USEPARENTPLC = "UseParentPLC";

		// Token: 0x040001E4 RID: 484
		internal const string ATTRIBUTE_SKIPINSERTLIBRARY = "SkipInsertLibrary";

		// Token: 0x040001E5 RID: 485
		internal const string ATTRIBUTE_STDCOMMUNICATIONLINK = "StdCommunicationLink";

		// Token: 0x040001E6 RID: 486
		internal const string ATTRIBUTE_SECUREONLINEMODE = "SecureOnlineMode";

		// Token: 0x040001E7 RID: 487
		internal const string ATTRIBUTE_LOGICALGVLASSIGNMENTERRORTYPE = "LogicalGVLAssignmentErrorType";

		// Token: 0x040001E8 RID: 488
		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		// Token: 0x040001E9 RID: 489
		[DefaultSerialization("Id")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected DeviceIdentification _deviceId;

		// Token: 0x040001EA RID: 490
		[DefaultSerialization("DefaultDeviceInfo")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected DefaultDeviceInfo _defaultDeviceInfo;

		// Token: 0x040001EB RID: 491
		[DefaultSerialization("DeviceParameterSet")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected ParameterSet _deviceParameterSet = new ParameterSet();

		// Token: 0x040001EC RID: 492
		[DefaultSerialization("Disable")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected bool _bDisable;

		// Token: 0x040001ED RID: 493
		[DefaultSerialization("Exclude")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected bool _bExclude;

		// Token: 0x040001EE RID: 494
		[DefaultSerialization("Connectors")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected ArrayList _alOldConnectors = new ArrayList();

		// Token: 0x040001EF RID: 495
		[DefaultSerialization("ConnectorList")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected ConnectorList _connectors = new ConnectorList();

		// Token: 0x040001F0 RID: 496
		[DefaultSerialization("Attributes")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected DeviceAttributes _attributes = new DeviceAttributes();

		// Token: 0x040001F1 RID: 497
		[DefaultSerialization("CommunicationSettings")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected CommunicationSettings _communicationSettings = new CommunicationSettings();

		// Token: 0x040001F2 RID: 498
		[DefaultSerialization("IoProviderBase")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected IoProviderBase _ioProviderBase = new IoProviderBase();

		// Token: 0x040001F3 RID: 499
		[DefaultSerialization("GuidBusCycleTask")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected Guid _guidBusCycleTask = Guid.Empty;

		// Token: 0x040001F4 RID: 500
		[DefaultSerialization("DriverInfo")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		protected DriverInfo _driverInfo = new DriverInfo();

		// Token: 0x040001F5 RID: 501
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("CustomItems")]
		[StorageVersion("3.3.0.0")]
		protected CustomItemList _customItems = new CustomItemList();

		// Token: 0x040001F6 RID: 502
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IoConfigGlobalsGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidLmIoConfigGlobals = Guid.Empty;

		// Token: 0x040001F7 RID: 503
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IoConfigGlobalsMappingGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidLmIoConfigGlobalsMapping = Guid.Empty;

		// Token: 0x040001F8 RID: 504
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IoConfigVarConfigGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidLmIoConfigVarConfig = Guid.Empty;

		// Token: 0x040001F9 RID: 505
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IoConfigErrorPouGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidLmIoConfigErrorPou = Guid.Empty;

		// Token: 0x040001FA RID: 506
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("ModuleId")]
		[StorageVersion("3.3.0.0")]
		protected int _nModuleId = -1;

		// Token: 0x040001FB RID: 507
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IoUpdateTask")]
		[StorageVersion("3.3.0.0")]
		protected string _stIoUpdateTask;

		// Token: 0x040001FC RID: 508
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("FunctionalChildren")]
		[StorageVersion("3.3.0.0")]
		protected Guid[] _funcChildrenTypeGuids = new Guid[0];

		// Token: 0x040001FD RID: 509
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("UserManagement")]
		[StorageVersion("3.3.0.0")]
		protected string _stUserManagement;

		// Token: 0x040001FE RID: 510
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("RightsManagement")]
		[StorageVersion("3.3.0.0")]
		protected string _stRightsManagement;

		// Token: 0x040001FF RID: 511
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("showParamsInDevDescOrder")]
		[StorageVersion("3.3.1.0")]
		[StorageDefaultValue(false)]
		protected bool _bShowParamsInDevDescOrder;

		// Token: 0x04000200 RID: 512
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("fixedInputAddress")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue("")]
		protected string _stFixedInputAddress = string.Empty;

		// Token: 0x04000201 RID: 513
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("fixedOutputAddress")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue("")]
		protected string _stFixedOutputAddress = string.Empty;

		// Token: 0x04000202 RID: 514
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("HostObjectGuid")]
		[StorageVersion("3.3.2.0")]
		[StorageIgnorable]
		protected Guid _hostObjectGuid = Guid.Empty;

		// Token: 0x04000203 RID: 515
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("DownloadParamsDevDescOrder")]
		[StorageVersion("3.3.2.0")]
		[StorageDefaultValue(false)]
		protected bool _bDownloadParamsDevDescOrder;

		// Token: 0x04000204 RID: 516
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("CreateBitChannels")]
		[StorageVersion("3.5.0.0")]
		[StorageDefaultValue(false)]
		protected bool _bCreateBitChannels;

		// Token: 0x04000205 RID: 517
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("AdditionalStringTable")]
		[StorageVersion("3.5.3.0")]
		[StorageDefaultValue(null)]
		protected LList<LanguageStringRef> _liAdditionalStringTable;

		// Token: 0x04000206 RID: 518
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("OnlineHelpUrl")]
		[StorageVersion("3.5.4.0")]
		[StorageDefaultValue(null)]
		protected string _stOnlineHelpUrl;

		// Token: 0x04000207 RID: 519
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("InteractiveLoginMode")]
		[StorageVersion("3.5.5.0")]
		[StorageDefaultValue(-1)]
		protected int _nInteractiveLoginMode = -1;

		// Token: 0x04000208 RID: 520
		[DefaultSerialization("UseDeviceApplicationStructure")]
		[StorageVersion("3.5.9.0")]
		[StorageDefaultValue(false)]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected bool _bUseDeviceApplicationStructure;

		// Token: 0x04000209 RID: 521
		[DefaultSerialization("AllowSymbolicVarAccessInSyncWithIecCycle")]
		[StorageVersion("3.5.9.0")]
		[StorageDefaultValue(false)]
		[DefaultDuplication(DuplicationMethod.Shallow)]
		protected bool _bAllowSymbolicVarAccessInSyncWithIecCycle;

		// Token: 0x0400020A RID: 522
		private IMetaObject _metaObject;

		// Token: 0x0400020B RID: 523
		private Guid _guidObject;

		// Token: 0x0400020C RID: 524
		private int _nConnectorIDForChild = -1;

		// Token: 0x0400020D RID: 525
		private TypeList _typeList;

		// Token: 0x0400020E RID: 526
		private LocalUniqueIdGenerator _idGenerator = new LocalUniqueIdGenerator();

		// Token: 0x0400020F RID: 527
		private IDeviceInfo _deviceInfo;

		// Token: 0x04000210 RID: 528
		private bool _bPastePrepared;

		// Token: 0x04000211 RID: 529
		internal LDictionary<string, LDictionary<IDataElement, Guid>> _dictGlobalDataTypes = new LDictionary<string, LDictionary<IDataElement, Guid>>();

		// Token: 0x04000212 RID: 530
		private StringTable _stringTable;

		// Token: 0x04000213 RID: 531
		private bool _bIsInUpdate;

		// Token: 0x04000214 RID: 532
		private int nFunctionalChildren = -1;

		// Token: 0x04000215 RID: 533
		internal static string GVL_IOCONFIG_GLOBALS = "IoConfig_Globals";

		// Token: 0x04000216 RID: 534
		private static string GVL_IOCONFIG_GLOBALS_MODULELIST = "IoConfig_Globals_ModuleList";

		// Token: 0x04000217 RID: 535
		private static string GVL_IOCONFIG_GLOBALS_MAPPING = "IoConfig_Globals_Mapping";

		// Token: 0x04000218 RID: 536
		private static string GVL_IOCONFIG_GLOBALS_FORCES = "IoConfig_Globals_Force_Variables";

		// Token: 0x04000219 RID: 537
		private static string GVL_IOCONFIG_GLOBALS_FORCES_TYPE = DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES + "_Type";

		// Token: 0x0400021A RID: 538
		private static string GVL_IOCONFIG_GLOBALS_FORCES_FLAGS = DeviceObject.GVL_IOCONFIG_GLOBALS_FORCES + "_Flags";

		// Token: 0x0400021B RID: 539
		private bool _bTaskLanguageModel;

		// Token: 0x0400021C RID: 540
		private bool _bRemoveLanguageModel;

		// Token: 0x0400021D RID: 541
		private int _iLastNumberOfTasks;

		// Token: 0x0400021E RID: 542
		private LDictionary<Guid, DeviceObject.ApplicationUsage> _dictAppUsage = new LDictionary<Guid, DeviceObject.ApplicationUsage>();

		// Token: 0x0400021F RID: 543
		[DefaultSerialization("LogicalDeviceList")]
		[StorageVersion("3.4.1.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageIgnorable]
		protected LogicalDeviceList _logicalDevices = new LogicalDeviceList();

		// Token: 0x04000220 RID: 544
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("LogicalLanguageModelPositionId")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(-1L)]
		protected long _lLogicalLanguageModelPositionId = -1L;

		// Token: 0x04000221 RID: 545
		[DefaultSerialization("SupportedLogicalBusSystems")]
		[StorageVersion("3.4.1.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageIgnorable]
		protected ArrayList _arSupportedLogicalBusSystems = new ArrayList();

		// Token: 0x04000222 RID: 546
		[DefaultSerialization("MappingPossible")]
		[StorageVersion("3.5.1.10")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		protected bool _bMappingPossible;

		// Token: 0x04000223 RID: 547
		[DefaultSerialization("HidePropertiesDialog")]
		[StorageVersion("3.5.3.30")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		[StorageIgnorable]
		protected bool _bHidePropertiesDialog;

		// Token: 0x04000224 RID: 548
		[DefaultSerialization("PlaceholderResolution")]
		[StorageVersion("3.5.7.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		private LDictionary<string, string> _dictPlaceholderResolutions;

		// Token: 0x020001B6 RID: 438
		private enum CYCLICCALLS
		{
			// Token: 0x0400079A RID: 1946
			BeforeReadInputs,
			// Token: 0x0400079B RID: 1947
			AfterReadInput,
			// Token: 0x0400079C RID: 1948
			BeforeWriteOutputs,
			// Token: 0x0400079D RID: 1949
			AfterWriteOutputs
		}

		// Token: 0x020001B7 RID: 439
		internal class TaskUsage
		{
			// Token: 0x1700087D RID: 2173
			// (get) Token: 0x060016B3 RID: 5811 RVA: 0x0007D00F File Offset: 0x0007C00F
			internal LList<IIOTaskUsage> IoUsage
			{
				get
				{
					return this._ioUsage;
				}
			}

			// Token: 0x1700087E RID: 2174
			// (get) Token: 0x060016B4 RID: 5812 RVA: 0x0007D017 File Offset: 0x0007C017
			internal LDictionary<int, IIOTaskUsage> DataElementUsage
			{
				get
				{
					return this._dictDataElementUsage;
				}
			}

			// Token: 0x0400079E RID: 1950
			private LList<IIOTaskUsage> _ioUsage = new LList<IIOTaskUsage>();

			// Token: 0x0400079F RID: 1951
			private LDictionary<int, IIOTaskUsage> _dictDataElementUsage = new LDictionary<int, IIOTaskUsage>();
		}

		// Token: 0x020001B8 RID: 440
		internal class ApplicationUsage
		{
			// Token: 0x1700087F RID: 2175
			// (get) Token: 0x060016B6 RID: 5814 RVA: 0x0007D03D File Offset: 0x0007C03D
			internal LDictionary<Guid, DeviceObject.TaskUsage> TaskUsage
			{
				get
				{
					return this._taskUsage;
				}
			}

			// Token: 0x17000880 RID: 2176
			// (get) Token: 0x060016B7 RID: 5815 RVA: 0x0007D045 File Offset: 0x0007C045
			// (set) Token: 0x060016B8 RID: 5816 RVA: 0x0007D04D File Offset: 0x0007C04D
			internal DoubleAddressTaskChecker Checker
			{
				get
				{
					return this._checker;
				}
				set
				{
					this._checker = value;
				}
			}

			// Token: 0x040007A0 RID: 1952
			private LDictionary<Guid, DeviceObject.TaskUsage> _taskUsage = new LDictionary<Guid, DeviceObject.TaskUsage>();

			// Token: 0x040007A1 RID: 1953
			private DoubleAddressTaskChecker _checker;
		}

		// Token: 0x020001B9 RID: 441
		internal class TaskInfo : ITaskInfo
		{
			// Token: 0x060016BA RID: 5818 RVA: 0x0007D069 File Offset: 0x0007C069
			public TaskInfo(Guid guidObject, Guid guidTask, string stTaskName)
			{
				this.ObjectGuid = guidObject;
				this.TaskGuid = guidTask;
				this.TaskName = stTaskName;
			}

			// Token: 0x17000881 RID: 2177
			// (get) Token: 0x060016BB RID: 5819 RVA: 0x0007D0A7 File Offset: 0x0007C0A7
			public Guid ObjectGuid { get; } = Guid.Empty;

			// Token: 0x17000882 RID: 2178
			// (get) Token: 0x060016BC RID: 5820 RVA: 0x0007D0AF File Offset: 0x0007C0AF
			public Guid TaskGuid { get; } = Guid.Empty;

			// Token: 0x17000883 RID: 2179
			// (get) Token: 0x060016BD RID: 5821 RVA: 0x0007D0B7 File Offset: 0x0007C0B7
			public string TaskName { get; } = string.Empty;
		}
	}
}
