using System;
using System.Collections;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{48ed2d53-8733-46f1-bf62-13f190518ba8}")]
	[StorageVersion("3.3.0.0")]
	public class FixedAdapter : GenericObject2, IFixedAdapter, IAdapter, IAdapterBase, IGenericObject, IArchivable, ICloneable, IComparable
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("DefaultModules")]
		[StorageVersion("3.3.0.0")]
		private DeviceIdentification[] _defaultModules;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Modules")]
		[StorageVersion("3.3.0.0")]
		private Guid[] _moduleGuids = new Guid[0];

		[DefaultDuplication(DuplicationMethod.Deep)]
		private Hashtable _htExpectedModuleGuids;

		private bool _bSubDevicesCollapsed;

		public bool SubDevicesCollapsed => _bSubDevicesCollapsed;

		public Guid[] Modules
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _moduleGuids.Length; i++)
				{
					if (_moduleGuids[i] != Guid.Empty)
					{
						num++;
					}
				}
				Guid[] array = new Guid[num];
				num = 0;
				for (int i = 0; i < _moduleGuids.Length; i++)
				{
					if (_moduleGuids[i] != Guid.Empty)
					{
						array[num] = _moduleGuids[i];
						num++;
					}
				}
				return array;
			}
		}

		public int ModulesCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _moduleGuids.Length; i++)
				{
					if (_moduleGuids[i] != Guid.Empty)
					{
						num++;
					}
				}
				return num;
			}
		}

		public int SubObjectsCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _moduleGuids.Length; i++)
				{
					if (_moduleGuids[i] != Guid.Empty)
					{
						num++;
					}
				}
				return num;
			}
		}

		public DeviceIdentification[] DefaultModules => _defaultModules;

		internal Guid[] ModuleGuidsPlain => _moduleGuids;

		public FixedAdapter()
			: base()
		{
		}

		public FixedAdapter(XmlReader reader, IDeviceIdentification masterDeviceId)
			: this()
		{
			_bSubDevicesCollapsed = DeviceObjectHelper.ParseBool(reader.GetAttribute("subdevicesCollapsed"), bDefault: false);
			if (reader.IsEmptyElement)
			{
				_defaultModules = new DeviceIdentification[0];
				_moduleGuids = new Guid[0];
				reader.Skip();
				return;
			}
			LList<DeviceIdentification> val = new LList<DeviceIdentification>();
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name.Equals("Module"))
					{
						DeviceIdentification deviceIdentification = DeviceRefHelper.ReadDeviceRef(reader, masterDeviceId);
						if (deviceIdentification != null)
						{
							val.Add(deviceIdentification);
						}
					}
				}
				else
				{
					reader.Read();
				}
			}
			_defaultModules = new DeviceIdentification[val.Count];
			val.CopyTo(_defaultModules, 0);
			_moduleGuids = new Guid[_defaultModules.Length];
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				_moduleGuids[i] = Guid.Empty;
			}
			reader.Read();
		}

		public FixedAdapter(FixedAdapter original)
			: this()
		{
			_defaultModules = new DeviceIdentification[original._defaultModules.Length];
			for (int i = 0; i < _defaultModules.Length; i++)
			{
				_defaultModules[i] = (DeviceIdentification)((GenericObject)original._defaultModules[i]).Clone();
			}
			_moduleGuids = new Guid[original._moduleGuids.Length];
			original._moduleGuids.CopyTo(_moduleGuids, 0);
			if (original._htExpectedModuleGuids != null)
			{
				_htExpectedModuleGuids = new Hashtable(original._htExpectedModuleGuids);
			}
			else
			{
				_htExpectedModuleGuids = null;
			}
			_bSubDevicesCollapsed = original._bSubDevicesCollapsed;
		}

		public override object Clone()
		{
			FixedAdapter fixedAdapter = new FixedAdapter(this);
			((GenericObject)fixedAdapter).AfterClone();
			return fixedAdapter;
		}

		public bool CanAddModule()
		{
			return _moduleGuids[_moduleGuids.Length - 1] == Guid.Empty;
		}

		public void AddModule(Guid guidModule)
		{
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				if (_moduleGuids[i] == Guid.Empty)
				{
					_moduleGuids[i] = guidModule;
					return;
				}
			}
			throw new InvalidOperationException("Cannot add modules to a fixed adapter");
		}

		internal void RemoveDevice(Guid guidChild)
		{
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				if (_moduleGuids[i] == guidChild)
				{
					_moduleGuids[i] = Guid.Empty;
				}
			}
		}

		public IDeviceIdentification GetDeviceReference(int nIndex)
		{
			return (IDeviceIdentification)(object)_defaultModules[nIndex];
		}

		public Guid GetDeviceGuid(int nIndex)
		{
			return _moduleGuids[nIndex];
		}

		public bool CanAddModule(int nInsertPosition, IObject childDevice)
		{
			if (((childDevice != null) ? childDevice.MetaObject : null) != null && ModuleAlreadySet(nInsertPosition, childDevice.MetaObject.ObjectGuid))
			{
				return true;
			}
			IDeviceObject2 val = (IDeviceObject2)(object)((childDevice is IDeviceObject2) ? childDevice : null);
			if (nInsertPosition >= _moduleGuids.Length)
			{
				return false;
			}
			if (_moduleGuids[nInsertPosition] != Guid.Empty)
			{
				return false;
			}
			if (val == null)
			{
				return false;
			}
			return ((object)((IDeviceObject5)((val is IDeviceObject5) ? val : null)).DeviceIdentificationNoSimulation).Equals((object)_defaultModules[nInsertPosition]);
		}

		public void AddModule(int nInsertPosition, IMetaObjectStub mosChild)
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mosChild.ProjectHandle, mosChild.ObjectGuid);
			if (!CanAddModule(nInsertPosition, objectToRead.Object))
			{
				throw new InvalidOperationException("Cannot add this device at the selected position");
			}
			_moduleGuids[nInsertPosition] = mosChild.ObjectGuid;
		}

		internal bool ModuleAlreadySet(int nInsertPosition, Guid objectGuid)
		{
			if (nInsertPosition < _moduleGuids.Length && _moduleGuids[nInsertPosition] == objectGuid)
			{
				return true;
			}
			return false;
		}

		public void PreparePaste()
		{
			_htExpectedModuleGuids = new Hashtable();
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				_htExpectedModuleGuids[_moduleGuids[i]] = i;
			}
			for (int j = 0; j < _moduleGuids.Length; j++)
			{
				_moduleGuids[j] = Guid.Empty;
			}
		}

		public void UpdatePasteModuleGuid(Guid oldGuid, Guid newGuid)
		{
			if (_htExpectedModuleGuids != null)
			{
				object key = oldGuid;
				object obj = _htExpectedModuleGuids[key];
				if (obj != null)
				{
					_htExpectedModuleGuids.Remove(key);
					_htExpectedModuleGuids[newGuid] = obj;
				}
			}
		}

		public bool IsExpectedModule(Guid guidModule)
		{
			if (_htExpectedModuleGuids != null && _htExpectedModuleGuids[guidModule] != null)
			{
				return true;
			}
			return false;
		}

		public bool SetExpectedModule(Guid guidModule)
		{
			if (_htExpectedModuleGuids != null)
			{
				object obj = _htExpectedModuleGuids[guidModule];
				if (obj != null)
				{
					int num = (int)obj;
					_moduleGuids[num] = guidModule;
					_htExpectedModuleGuids.Remove(guidModule);
					if (_htExpectedModuleGuids.Count == 0)
					{
						_htExpectedModuleGuids = null;
					}
					return true;
				}
			}
			return false;
		}

		private int FindMatchingId(LList<object> alDevices, IDeviceIdentification id)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < alDevices.Count; i++)
			{
				IDeviceObject val = (IDeviceObject)alDevices[i];
				IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((val is IDeviceObject5) ? val : null)).DeviceIdentificationNoSimulation;
				IDeviceIdentification val2 = deviceIdentificationNoSimulation;
				try
				{
					IDeviceIdentification val3 = null;
					val3 = (IDeviceIdentification)(object)((!(id is IModuleIdentification) || !(deviceIdentificationNoSimulation is IModuleIdentification)) ? ((ModuleIdentification)(object)id) : new ModuleIdentification
					{
						Id = id.Id,
						Type = id.Type,
						Version = id.Version,
						ModuleId = ((IModuleIdentification)deviceIdentificationNoSimulation).ModuleId
					});
					IDeviceIdentification deviceID = null;
					if (APEnvironment.DeviceMgr.TryGetTargetDeviceID(((IObject)val).MetaObject.ProjectHandle, ((IObject)val).MetaObject.ObjectGuid, val3, out deviceID))
					{
						val2 = deviceID;
					}
				}
				catch (Exception)
				{
				}
				if (val2.Type == id.Type && (val2.Id == id.Id || id is IModuleIdentification))
				{
					if (id is IModuleIdentification && val2 is IModuleIdentification && ((IModuleIdentification)val2).ModuleId == ((IModuleIdentification)id).ModuleId)
					{
						return i;
					}
					if (!(id is IModuleIdentification) && !(val2 is IModuleIdentification))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public void UpdateModules(LList<object> alDevices, IConnector7 conUpdate)
		{
			UpdateModules(alDevices, conUpdate, bOnlyEmptyAdapter: false);
		}

		public void UpdateModules(LList<object> alDevices, IConnector7 conUpdate, bool bOnlyEmptyAdapter)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				IDeviceIdentification id = (IDeviceIdentification)(object)_defaultModules[i];
				int num = FindMatchingId(alDevices, id);
				if (num >= 0 && (!bOnlyEmptyAdapter || _moduleGuids[i] == Guid.Empty))
				{
					IMetaObject metaObject = ((IObject)(IDeviceObject)alDevices[num]).MetaObject;
					_moduleGuids[i] = metaObject.ObjectGuid;
					object obj = alDevices[num];
					AdapterBase.CheckAndChangeConnectorInChild((IDeviceObject)((obj is IDeviceObject) ? obj : null), conUpdate);
					alDevices.RemoveAt(num);
				}
			}
		}

		public void ClearModules()
		{
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				_moduleGuids[i] = Guid.Empty;
			}
		}
	}
}
