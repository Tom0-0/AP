#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceCommunicationEditor;
using _3S.CoDeSys.Simulation;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{58f2b04d-505d-4589-9fcf-0ec83491f9b2}")]
	[StorageVersion("3.3.0.0")]
	public class CommunicationSettings : GenericObject2, ICommunicationSettingsWithStorageLocation, ICommunicationSettings, IGenericInterfaceExtensionProvider, ICommunicationSettings7, ICommunicationSettings6, ICommunicationSettings5, ICommunicationSettings4, ICommunicationSettings3, ICommunicationSettings2, ICommunicationSettingExtension
	{
		[DefaultSerialization("SaveLocally")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bSaveLocally;

		private CommunicationSettingsBase _projectSettings = new CommunicationSettingsBase();

		private DeviceObject _owner;

		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		public override string[] SerializableValueNames
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(base.SerializableValueNames);
				arrayList.AddRange(((GenericObject)_projectSettings).SerializableValueNames);
				string[] array = new string[arrayList.Count];
				arrayList.CopyTo(array);
				return array;
			}
		}

		public bool SimulationMode
		{
			get
			{
				if (!_bSaveLocally)
				{
					return _projectSettings.SimulationMode;
				}
				return LoadSettingsFromOptions().SimulationMode;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_ = _projectSettings.SimulationMode;
					_projectSettings.SimulationMode = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				_ = communicationSettingsBase.SimulationMode;
				communicationSettingsBase.SimulationMode = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public IDeviceAddress Address
		{
			get
			{
				if (SimulationMode)
				{
					ISimulationManager simulationMgrOrNull = APEnvironment.SimulationMgrOrNull;
					if (simulationMgrOrNull != null)
					{
						return simulationMgrOrNull.GetCommunicationAddress();
					}
				}
				if (!_bSaveLocally)
				{
					return _projectSettings.Address;
				}
				return LoadSettingsFromOptions().Address;
			}
			set
			{
				string text = string.Empty;
				if (!_bSaveLocally)
				{
					if (_projectSettings.Address != null)
					{
						text = _projectSettings.Address.ToString();
					}
					_projectSettings.Address = value;
					TrackingMode = (DeviceTrackingMode)5;
				}
				else
				{
					CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
					if (communicationSettingsBase != null)
					{
						if (communicationSettingsBase.Address != null)
						{
							text = communicationSettingsBase.Address.ToString();
						}
						communicationSettingsBase.Address = value;
						communicationSettingsBase.TrackingMode = (DeviceTrackingMode)5;
						SaveSettingsToOptions(communicationSettingsBase);
					}
				}
				if (_owner != null)
				{
					if (value == null && text != string.Empty)
					{
						DeviceManager.NotifyNewDeviceAddress(_owner, _owner.MetaObject.ObjectGuid, value);
					}
					if (value != null && value.ToString() != text)
					{
						DeviceManager.NotifyNewDeviceAddress(_owner, _owner.MetaObject.ObjectGuid, value);
					}
				}
			}
		}

		public Guid Gateway
		{
			get
			{
				if (SimulationMode)
				{
					ISimulationManager simulationMgrOrNull = APEnvironment.SimulationMgrOrNull;
					if (simulationMgrOrNull != null)
					{
						return simulationMgrOrNull.GetGateway().GatewayGuid;
					}
				}
				if (!_bSaveLocally)
				{
					return _projectSettings.Gateway;
				}
				return LoadSettingsFromOptions().Gateway;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.Gateway = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.Gateway = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public bool PromptAtLogin
		{
			get
			{
				if (!_bSaveLocally)
				{
					return _projectSettings.PromptAtLogin;
				}
				return LoadSettingsFromOptions().PromptAtLogin;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.PromptAtLogin = value;
				}
				else
				{
					SetLocalPromptAtLogin(value);
				}
			}
		}

		public bool SaveLocally
		{
			get
			{
				return _bSaveLocally;
			}
			set
			{
				if (_bSaveLocally != value)
				{
					if (_bSaveLocally)
					{
						_projectSettings = (CommunicationSettingsBase)((GenericObject)LoadSettingsFromOptions()).Clone();
					}
					else
					{
						SaveSettingsToOptions(_projectSettings);
					}
					_bSaveLocally = value;
				}
			}
		}

		public ICommunicationSettings ProjectCommunicationSettings => (ICommunicationSettings)(object)_projectSettings;

		public bool SecureOnlineMode
		{
			get
			{
				return _projectSettings.SecureOnlineMode;
			}
			set
			{
				_projectSettings.SecureOnlineMode = value;
			}
		}

		public string Name
		{
			get
			{
				if (SimulationMode)
				{
					return string.Empty;
				}
				if (!_bSaveLocally)
				{
					return _projectSettings.Name;
				}
				return LoadSettingsFromOptions().Name;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.Name = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.Name = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public CommSettingFilterEnum CommunicationSettingFilter
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (!_bSaveLocally)
				{
					return _projectSettings.CommunicationSettingFilter;
				}
				return LoadSettingsFromOptions().CommunicationSettingFilter;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				if (!_bSaveLocally)
				{
					_projectSettings.CommunicationSettingFilter = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.CommunicationSettingFilter = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public CommSettingSortingEnum CommunicationSettingSorting
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (!_bSaveLocally)
				{
					return _projectSettings.CommunicationSettingSorting;
				}
				return LoadSettingsFromOptions().CommunicationSettingSorting;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				if (!_bSaveLocally)
				{
					_projectSettings.CommunicationSettingSorting = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.CommunicationSettingSorting = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public int MonitoringIntervalMsec
		{
			get
			{
				if (!_bSaveLocally)
				{
					return _projectSettings.MonitoringIntervalMsec;
				}
				return LoadSettingsFromOptions().MonitoringIntervalMsec;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.MonitoringIntervalMsec = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.MonitoringIntervalMsec = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public IScanInformation ScanInformation
		{
			get
			{
				if (!_bSaveLocally)
				{
					return _projectSettings.ScanInformation;
				}
				return LoadSettingsFromOptions().ScanInformation;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.ScanInformation = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.ScanInformation = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public DeviceTrackingMode TrackingMode
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				if (!_bSaveLocally)
				{
					return _projectSettings.TrackingMode;
				}
				return LoadSettingsFromOptions().TrackingMode;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				if (!_bSaveLocally)
				{
					_projectSettings.TrackingMode = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.TrackingMode = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public bool IsCommunicationEncrypted
		{
			get
			{
				if (!_bSaveLocally)
				{
					return _projectSettings.IsCommunicationEncrypted;
				}
				return LoadSettingsFromOptions().IsCommunicationEncrypted;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.IsCommunicationEncrypted = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.IsCommunicationEncrypted = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public bool IsNameTrackingFromScanActive
		{
			get
			{
				if (!_bSaveLocally)
				{
					return _projectSettings.IsNameTrackingFromScanActive;
				}
				return LoadSettingsFromOptions().IsNameTrackingFromScanActive;
			}
			set
			{
				if (!_bSaveLocally)
				{
					_projectSettings.IsNameTrackingFromScanActive = value;
					return;
				}
				CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
				communicationSettingsBase.IsNameTrackingFromScanActive = value;
				SaveSettingsToOptions(communicationSettingsBase);
			}
		}

		public CommunicationSettings()
			: base()
		{
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		protected CommunicationSettings(CommunicationSettings original)
			: this()
		{
			_bSaveLocally = original._bSaveLocally;
			_projectSettings = (CommunicationSettingsBase)((GenericObject)original._projectSettings).Clone();
		}

		public override object Clone()
		{
			CommunicationSettings communicationSettings = new CommunicationSettings(this);
			((GenericObject)communicationSettings).AfterClone();
			return communicationSettings;
		}

		public override object GetSerializableValue(string stValueName)
		{
			if (stValueName == "SaveLocally")
			{
				return _bSaveLocally;
			}
			return ((GenericObject)_projectSettings).GetSerializableValue(stValueName);
		}

		public override string[] GetSerializableValueNames(IArchiveVersionInfo info, IArchiveReporter reporter)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(base.GetSerializableValueNames(info, reporter));
			arrayList.AddRange(((GenericObject2)_projectSettings).GetSerializableValueNames(info, reporter));
			string[] array = new string[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public override void SetSerializableValue(string stValueName, object value)
		{
			base.SetSerializableValue(stValueName, value, (IArchiveReporter)null);
		}

		public override void SetSerializableValue(string stValueName, object value, IArchiveReporter reporter)
		{
			if (stValueName == "SaveLocally")
			{
				_bSaveLocally = (bool)value;
			}
			else
			{
				((GenericObject2)_projectSettings).SetSerializableValue(stValueName, value, reporter);
			}
		}

		public void SetLocalCommunicationSettings(Guid guidGateway, IDeviceAddress address)
		{
			CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
			communicationSettingsBase.Gateway = guidGateway;
			communicationSettingsBase.Address = address;
			SaveSettingsToOptions(communicationSettingsBase);
		}

		public bool GetLocalCommunicationSettings(out Guid guidGateway, out IDeviceAddress address)
		{
			if (_owner != null && _owner.MetaObject != null)
			{
				CommunicationSettingsBase communicationSettings = OptionsHelper.GetCommunicationSettings(_owner.MetaObject.ObjectGuid);
				if (communicationSettings != null)
				{
					guidGateway = communicationSettings.Gateway;
					address = communicationSettings.Address;
					return true;
				}
			}
			guidGateway = Guid.Empty;
			address = null;
			return false;
		}

		public void SetLocalPromptAtLogin(bool bPromptAtLogin)
		{
			CommunicationSettingsBase communicationSettingsBase = LoadSettingsFromOptions();
			communicationSettingsBase.PromptAtLogin = bPromptAtLogin;
			SaveSettingsToOptions(communicationSettingsBase);
		}

		public bool GetLocalPromptAtLogin(out bool bPromptAtLogin)
		{
			if (_owner != null && _owner.MetaObject != null)
			{
				CommunicationSettingsBase communicationSettings = OptionsHelper.GetCommunicationSettings(_owner.MetaObject.ObjectGuid);
				if (communicationSettings != null)
				{
					bPromptAtLogin = communicationSettings.PromptAtLogin;
					return true;
				}
			}
			bPromptAtLogin = false;
			return false;
		}

		public bool SetDeviceAddressAndChildAddress(Guid objectGuid, Guid gatewayGuid, IDeviceAddress address, bool bSetAddressToChildToo)
		{
			Address = address;
			if (!bSetAddressToChildToo)
			{
				return true;
			}
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
			{
				try
				{
					int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
					Guid[] allObjects = ((IObjectManager)APEnvironment.ObjectMgr).GetAllObjects(handle);
					IMetaObjectStub val = null;
					int num = 0;
					Guid[] array = allObjects;
					foreach (Guid guid in array)
					{
						if (guid == objectGuid)
						{
							continue;
						}
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
						if (metaObjectStub == null || !typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							continue;
						}
						IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid).Object;
						IDeviceObject val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
						if (val2 != null && val2.AllowsDirectCommunication && metaObjectStub.ParentObjectGuid != Guid.Empty)
						{
							IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
							while (metaObjectStub2.ParentObjectGuid != Guid.Empty)
							{
								metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObjectStub2.ProjectHandle, metaObjectStub2.ParentObjectGuid);
							}
							if (metaObjectStub2.ObjectGuid == objectGuid)
							{
								val = metaObjectStub;
								num++;
							}
						}
					}
					if (num == 1)
					{
						IMetaObject val3 = null;
						try
						{
							val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(handle, val.ObjectGuid);
							IObject object2 = val3.Object;
							bool result = DeviceObjectHelper.AutoSetActivePathForChildPlc((IDeviceObject)(object)((object2 is IDeviceObject) ? object2 : null), gatewayGuid, address);
							((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
							return result;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.Message);
						}
						finally
						{
							if (val3 != null && val3.IsToModify)
							{
								((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, false, (object)null);
							}
						}
					}
					else if (num > 1)
					{
						Debug.WriteLine("SetDeviceAddressAndChildAddress(): More than one child device found! Aborting...");
					}
					else
					{
						Debug.WriteLine("SetDeviceAddressAndChildAddress(): No child device found! Aborting...");
					}
				}
				catch (Exception ex2)
				{
					Debug.WriteLine(ex2.Message);
				}
			}
			return false;
		}

		internal void SetOwner(DeviceObject owner)
		{
			_owner = owner;
		}

		protected CommunicationSettingsBase LoadSettingsFromOptions()
		{
			CommunicationSettingsBase communicationSettingsBase = null;
			if (_owner != null && _owner.MetaObject != null)
			{
				communicationSettingsBase = OptionsHelper.GetCommunicationSettings(_owner.MetaObject.ObjectGuid);
			}
			if (communicationSettingsBase == null)
			{
				communicationSettingsBase = new CommunicationSettingsBase(_projectSettings);
			}
			return communicationSettingsBase;
		}

		protected void SaveSettingsToOptions(CommunicationSettingsBase settings)
		{
			if (_owner != null && _owner.MetaObject != null)
			{
				OptionsHelper.SetCommunicationSettings(_owner.MetaObject.ObjectGuid, settings);
			}
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

		public bool IsFunctionAvailable(string stFunction)
		{
			if (stFunction == null)
			{
				throw new ArgumentNullException("stFunction");
			}
			switch (stFunction)
			{
			case "GetSecureOnlineMode":
			case "SetSecureOnlineMode":
			case "GetName":
			case "SetName":
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
			case "GetSecureOnlineMode":
				xmlDocument.DocumentElement.InnerText = XmlConvert.ToString(SecureOnlineMode);
				break;
			case "SetSecureOnlineMode":
				SecureOnlineMode = XmlConvert.ToBoolean(functionData.DocumentElement["value"].InnerText);
				break;
			case "GetName":
				xmlDocument.DocumentElement.InnerText = Name;
				break;
			case "SetName":
				Name = functionData.DocumentElement["value"].InnerText;
				break;
			}
			return xmlDocument;
		}

		internal void InitializeSecureOnlineMode(bool bSimulationMode)
		{
			_projectSettings.InitializeSecureOnlineMode(bSimulationMode);
		}
	}
}
