using System;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.DeviceCommunicationEditor;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{4480b396-0f06-448e-a8cb-f58d983ce814}")]
	[StorageVersion("3.3.0.0")]
	public class CommunicationSettingsBase : GenericObject2, ICommunicationSettings7, ICommunicationSettings6, ICommunicationSettings5, ICommunicationSettings4, ICommunicationSettings3, ICommunicationSettings2, ICommunicationSettings, IGenericInterfaceExtensionProvider
	{
		[DefaultSerialization("GatewayGuid")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private Guid _guidGateway;

		[DefaultSerialization("Address")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private IDeviceAddress _address;

		[DefaultSerialization("PromptAtLogin")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bPromptAtLogin;

		[DefaultSerialization("SimulationMode")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bSimulationMode;

		[DefaultSerialization("SecureOnlineMode")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bSecureOnlineMode;

		[DefaultSerialization("SecureOnlineModeExplicitelySet")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private bool _bSecureOnlineModeExplicitelySet;

		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stName = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		private CommSettingFilterEnum _commSettingFilter = (CommSettingFilterEnum)1;

		[DefaultDuplication(DuplicationMethod.Deep)]
		private CommSettingSortingEnum _commSettingSorting;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(200)]
		[DefaultSerialization("MonitoringIntervalMsec")]
		[StorageVersion("3.5.3.0")]
		private int _nMonitoringIntervalMsec = 200;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(null)]
		[DefaultSerialization("ScanInformation")]
		[StorageVersion("3.5.4.0")]
		private IScanInformation _ScanInformation;

		[DefaultDuplication(DuplicationMethod.Deep)]
		private DeviceTrackingMode _trackingMode = (DeviceTrackingMode)5;

		[DefaultSerialization("IsCommunicationEncrypted")]
		[StorageVersion("3.5.10.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		private bool _bIsCommunicationEncrypted;

		[DefaultSerialization("IsDefaultNameTrackingFromScanActive")]
		[StorageVersion("3.5.16.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		[StorageDefaultValue(false)]
		private bool _isDefaultNameTrackingFromScanActive;

		private IGenericInterfaceExtensionProvider _baseGenericInterfaceExtensionProvider;

		[DefaultSerialization("CommunicationSettingFilter")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(CommSettingFilterEnum.TargetID)]
		protected int CommunicationSettingFilterSerialization
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected I4, but got Unknown
				return (int)_commSettingFilter;
			}
			set
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_commSettingFilter = (CommSettingFilterEnum)value;
			}
		}

		[DefaultSerialization("CommunicationSettingSorting")]
		[StorageVersion("3.4.1.0")]
		[StorageDefaultValue(CommSettingSortingEnum.Name)]
		protected int CommunicationSettingSortingSerialization
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected I4, but got Unknown
				return (int)_commSettingSorting;
			}
			set
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_commSettingSorting = (CommSettingSortingEnum)value;
			}
		}

		[DefaultSerialization("TrackingMode")]
		[StorageVersion("3.5.4.0")]
		[StorageDefaultValue(5)]
		protected int DeviceTrackingModeSerialization
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Expected I4, but got Unknown
				return (int)_trackingMode;
			}
			set
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_trackingMode = (DeviceTrackingMode)value;
			}
		}

		public virtual Guid Gateway
		{
			get
			{
				return _guidGateway;
			}
			set
			{
				_guidGateway = value;
			}
		}

		public virtual IDeviceAddress Address
		{
			get
			{
				return _address;
			}
			set
			{
				_address = value;
			}
		}

		public virtual bool PromptAtLogin
		{
			get
			{
				return _bPromptAtLogin;
			}
			set
			{
				_bPromptAtLogin = value;
			}
		}

		public virtual bool SimulationMode
		{
			get
			{
				return _bSimulationMode;
			}
			set
			{
				_bSimulationMode = value;
			}
		}

		public virtual bool SecureOnlineMode
		{
			get
			{
				return _bSecureOnlineMode;
			}
			set
			{
				_bSecureOnlineMode = value;
				_bSecureOnlineModeExplicitelySet = true;
			}
		}

		public virtual string Name
		{
			get
			{
				return _stName;
			}
			set
			{
				_stName = value;
			}
		}

		public virtual CommSettingFilterEnum CommunicationSettingFilter
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _commSettingFilter;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_commSettingFilter = value;
			}
		}

		public virtual CommSettingSortingEnum CommunicationSettingSorting
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _commSettingSorting;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_commSettingSorting = value;
			}
		}

		public int MonitoringIntervalMsec
		{
			get
			{
				return _nMonitoringIntervalMsec;
			}
			set
			{
				if (value >= 10 && value <= 1000 && value % 10 == 0)
				{
					_nMonitoringIntervalMsec = value;
				}
			}
		}

		public IScanInformation ScanInformation
		{
			get
			{
				return _ScanInformation;
			}
			set
			{
				_ScanInformation = value;
			}
		}

		public DeviceTrackingMode TrackingMode
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _trackingMode;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_trackingMode = value;
			}
		}

		public bool IsCommunicationEncrypted
		{
			get
			{
				return _bIsCommunicationEncrypted;
			}
			set
			{
				_bIsCommunicationEncrypted = value;
			}
		}

		public bool IsNameTrackingFromScanActive
		{
			get
			{
				return _isDefaultNameTrackingFromScanActive;
			}
			set
			{
				_isDefaultNameTrackingFromScanActive = value;
			}
		}

		public CommunicationSettingsBase()
			: base()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			CommunicationSettingFilterSerialization = 1;
			DeviceTrackingModeSerialization = 5;
			_baseGenericInterfaceExtensionProvider = APEnvironment.TryCreateGenericInterfaceExtensionProviderImpl();
		}

		internal CommunicationSettingsBase(CommunicationSettingsBase original)
			: this()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			_guidGateway = original._guidGateway;
			if (original._address != null)
			{
				_address = (IDeviceAddress)((ICloneable)original._address).Clone();
			}
			else
			{
				_address = null;
			}
			_bPromptAtLogin = original._bPromptAtLogin;
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

		internal void InitializeSecureOnlineMode(bool bSecureOnlineMode)
		{
			if (!_bSecureOnlineModeExplicitelySet)
			{
				_bSecureOnlineMode = bSecureOnlineMode;
			}
		}
	}
}
