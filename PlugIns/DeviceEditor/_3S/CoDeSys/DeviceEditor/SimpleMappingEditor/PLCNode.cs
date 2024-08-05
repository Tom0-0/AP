using System;
using System.Collections.Generic;
using System.Linq;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	internal class PLCNode : DeviceNode, IPlcNode, IDeviceNode
	{
		private LList<Guid> _liApplications;

		private Guid _onlineApplication = Guid.Empty;

		private bool _bMapAllowed = true;

		private bool _bManualAddress = true;

		private bool _bMotorolaBitfields;

		private bool _bUnionRootEditable = true;

		private bool _bBaseTypeMappable = true;

		private bool _bBitfieldMappable = true;

		private bool _bMultipleMappableAllowed;

		private bool _bAlwaysMapToNew;

		private bool _DefaultColumnAvailable;

		private IDeviceObject _host;

		public Guid OnlineApplication
		{
			get
			{
				return _onlineApplication;
			}
			set
			{
				if (value != _onlineApplication)
				{
					_onlineApplication = value;
				}
			}
		}

		public bool IsConfigModeOnlineApplication
		{
			get
			{
				if (_onlineApplication != Guid.Empty && ((IEngine)APEnvironment.Engine).Projects.PrimaryProject != null)
				{
					IMetaObjectStub mos = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, _onlineApplication);
					if (typeof(IHiddenObject).IsAssignableFrom(mos.ObjectType) || APEnvironment.HiddenObjectAdorners.Any((IHiddenObjectAdorner hoa) => hoa.ShouldBeHidden(mos)))
					{
						IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(_onlineApplication);
						if (application != null && application.IsLoggedIn)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool UnionRootEditable => _bUnionRootEditable;

		public bool MotorolaBitfields => _bMotorolaBitfields;

		public bool BitfieldMappable => _bBitfieldMappable;

		public bool BaseTypeMappable => _bBaseTypeMappable;

		public bool AlwaysMapToNew => _bAlwaysMapToNew;

		public bool MultipleMappableAllowed => _bMultipleMappableAllowed;

		public bool ManualAddress => _bManualAddress;

		public bool DefaultColumnAvailable => _DefaultColumnAvailable;

		public IList<Guid> Applications
		{
			get
			{
				if (_liApplications == null)
				{
					return (IList<Guid>)new LList<Guid>();
				}
				return (IList<Guid>)_liApplications;
			}
		}

		public IDeviceObject PlcDevice => _host;

		public PLCNode(SimpleMappingTreeTableModel model, ISVNode node)
			: base(model, node, null)
		{
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Expected O, but got Unknown
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Invalid comparison between Unknown and I4
			IDeviceObject host = GetHost();
			if (host == null)
			{
				return;
			}
			_host = host;
			_liApplications = DeviceHelper.GetApplications(((IObject)host).MetaObject, bWithHidden: false);
			ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(((IDeviceObject5)((host is IDeviceObject5) ? host : null)).DeviceIdentificationNoSimulation);
			if (targetSettingsById != null)
			{
				_bMapAllowed = LocalTargetSettings.MappingChangeable.GetBoolValue(targetSettingsById);
				_bManualAddress = LocalTargetSettings.ManualAddressAllowed.GetBoolValue(targetSettingsById);
				_bMotorolaBitfields = LocalTargetSettings.MotorolaBitfields.GetBoolValue(targetSettingsById);
				_bUnionRootEditable = LocalTargetSettings.UnionRootEditable.GetBoolValue(targetSettingsById);
				_bBaseTypeMappable = LocalTargetSettings.BasetypeMappable.GetBoolValue(targetSettingsById);
				_bBitfieldMappable = LocalTargetSettings.BitfieldMappable.GetBoolValue(targetSettingsById);
				_bMultipleMappableAllowed = LocalTargetSettings.MultipleMappableAllowed.GetBoolValue(targetSettingsById);
				_bAlwaysMapToNew = LocalTargetSettings.MapAlwaysIecAddress.GetBoolValue(targetSettingsById);
			}
			IDriverInfo4 val = (IDriverInfo4)((IDeviceObject2)((host is IDeviceObject2) ? host : null)).DriverInfo;
			if (val == null)
			{
				return;
			}
			if ((int)val.StopResetBehaviourSetting == 1)
			{
				_DefaultColumnAvailable = true;
			}
			if (((IDriverInfo2)val).IoApplication != Guid.Empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(((IObject)host).MetaObject.ProjectHandle, ((IDriverInfo2)val).IoApplication))
			{
				IOnlineApplication application = ((IOnlineManager)APEnvironment.OnlineMgr).GetApplication(((IDriverInfo2)val).IoApplication);
				if (application != null && application.IsLoggedIn)
				{
					OnlineApplication = application.ApplicationGuid;
				}
			}
		}
	}
}
