#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{3f828f0b-c67b-430a-8c93-1e9bc324df64}")]
	[StorageVersion("3.3.0.0")]
	public class VarAdapter : GenericObject2, IVarAdapter2, IVarAdapter, IAdapter, IAdapterBase, IGenericObject, IArchivable, ICloneable, IComparable
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("Modules")]
		[StorageVersion("3.3.0.0")]
		private ArrayList _alModuleGuids = new ArrayList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("MaxDevices")]
		[StorageVersion("3.3.0.0")]
		private int _nMaxDevices = int.MaxValue;

		[DefaultDuplication(DuplicationMethod.Deep)]
		private Hashtable _htExpectedModuleGuids;

		private bool _bSubDevicesCollapsed;

		private DeviceIdentification[] _defaultModules;

		public bool SubDevicesCollapsed => _bSubDevicesCollapsed;

		public DeviceIdentification[] DefaultModules => _defaultModules;

		public int MaxDevices => _nMaxDevices;

		public int ModulesCount => _alModuleGuids.Count;

		public Guid[] Modules
		{
			get
			{
				Guid[] array = new Guid[_alModuleGuids.Count];
				_alModuleGuids.CopyTo(array, 0);
				return array;
			}
		}

		public int SubObjectsCount => ModulesCount;

		public IDeviceIdentification GetDefaultDevice(int nIndex)
		{
			if (_defaultModules == null || _defaultModules.Length <= nIndex)
			{
				return null;
			}
			return (IDeviceIdentification)(object)_defaultModules[nIndex];
		}

		public VarAdapter()
			: base()
		{
		}

		internal VarAdapter(XmlReader reader, IDeviceIdentification masterDeviceId)
			: base()
		{
			try
			{
				string attribute = reader.GetAttribute("max");
				if (attribute != null)
				{
					_nMaxDevices = int.Parse(attribute);
				}
				_bSubDevicesCollapsed = DeviceObjectHelper.ParseBool(reader.GetAttribute("subdevicesCollapsed"), bDefault: false);
			}
			catch
			{
			}
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			LList<DeviceIdentification> val = new LList<DeviceIdentification>();
			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					string name = reader.Name;
					if (name == "DefaultModule")
					{
						DeviceIdentification deviceIdentification = DeviceRefHelper.ReadDeviceRef(reader, masterDeviceId);
						val.Add(deviceIdentification);
					}
					else
					{
						reader.Skip();
					}
				}
				else
				{
					reader.Read();
				}
			}
			_defaultModules = new DeviceIdentification[val.Count];
			val.CopyTo(_defaultModules, 0);
			reader.Read();
		}

		private VarAdapter(VarAdapter original)
			: base()
		{
			_alModuleGuids = new ArrayList(original._alModuleGuids.Count);
			foreach (object alModuleGuid in original._alModuleGuids)
			{
				_alModuleGuids.Add(alModuleGuid);
			}
			_nMaxDevices = original._nMaxDevices;
			if (original._htExpectedModuleGuids != null)
			{
				_htExpectedModuleGuids = new Hashtable(original._htExpectedModuleGuids);
			}
			else
			{
				_htExpectedModuleGuids = null;
			}
			_bSubDevicesCollapsed = original._bSubDevicesCollapsed;
			if (original._defaultModules != null)
			{
				_defaultModules = new DeviceIdentification[original._defaultModules.Length];
				for (int i = 0; i < original._defaultModules.Length; i++)
				{
					_defaultModules[i] = (DeviceIdentification)((GenericObject)original._defaultModules[i]).Clone();
				}
			}
			else
			{
				_defaultModules = null;
			}
		}

		public override object Clone()
		{
			VarAdapter varAdapter = new VarAdapter(this);
			((GenericObject)varAdapter).AfterClone();
			return varAdapter;
		}

		public override void AfterDeserialize()
		{
			base.AfterDeserialize();
			ArrayList obj = (ArrayList)_alModuleGuids.Clone();
			_alModuleGuids.Clear();
			foreach (Guid item in obj)
			{
				if (!_alModuleGuids.Contains(item))
				{
					_alModuleGuids.Add(item);
				}
			}
		}

		public Guid GetDeviceGuid(int nIndex)
		{
			return (Guid)_alModuleGuids[nIndex];
		}

		public bool CanAddModule(int nInsertPosition, IObject childDevice)
		{
			if (_alModuleGuids.Count < _nMaxDevices && nInsertPosition <= _alModuleGuids.Count && childDevice is IDeviceObject2)
			{
				return !(childDevice is SlotDeviceObject);
			}
			return false;
		}

		public void AddModule(int nInsertPosition, IMetaObjectStub mosChild)
		{
			if (_alModuleGuids.Count >= _nMaxDevices)
			{
				throw new InvalidOperationException("Maximum number of devices already reached");
			}
			if (!_alModuleGuids.Contains(mosChild.ObjectGuid))
			{
				_alModuleGuids.Insert(nInsertPosition, mosChild.ObjectGuid);
			}
		}

		public void PreparePaste()
		{
			_htExpectedModuleGuids = new Hashtable();
			for (int i = 0; i < _alModuleGuids.Count; i++)
			{
				_htExpectedModuleGuids[_alModuleGuids[i]] = i;
			}
			_alModuleGuids.Clear();
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
					int num2 = 0;
					foreach (int value in _htExpectedModuleGuids.Values)
					{
						if (value < num)
						{
							num2++;
						}
					}
					num -= num2;
					Debug.Assert(num >= 0);
					if (!_alModuleGuids.Contains(guidModule))
					{
						if (num < _alModuleGuids.Count)
						{
							_alModuleGuids.Insert(num, guidModule);
						}
						else
						{
							_alModuleGuids.Add(guidModule);
						}
					}
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

		public void UpdateModules(LList<object> alDevices, IConnector7 conUpdate)
		{
			UpdateModules(alDevices, conUpdate, bOnlyEmptyAdapter: false);
		}

		public void UpdateModules(LList<object> alDevices, IConnector7 conUpdate, bool bOnlyEmptyAdapter)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Expected O, but got Unknown
			LList<object> val = new LList<object>();
			foreach (IDeviceObject alDevice in alDevices)
			{
				IDeviceObject val2 = alDevice;
				bool flag = false;
				foreach (IConnector7 item in (IEnumerable)val2.Connectors)
				{
					IConnector7 childConnector = item;
					if (DeviceManager.CheckMatchInterface(conUpdate, childConnector))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					try
					{
						IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((val2 is IDeviceObject5) ? val2 : null)).DeviceIdentificationNoSimulation;
						DeviceIdentification deviceIdentification = ((IConnector)conUpdate).GetDeviceObject().DeviceIdentification as DeviceIdentification;
						IDeviceIdentification val3 = null;
						val3 = (IDeviceIdentification)(object)((!(deviceIdentificationNoSimulation is IModuleIdentification)) ? ((ModuleIdentification)(object)deviceIdentificationNoSimulation) : new ModuleIdentification
						{
							Id = deviceIdentification.Id,
							Type = deviceIdentification.Type,
							Version = deviceIdentification.Version,
							ModuleId = ((IModuleIdentification)deviceIdentificationNoSimulation).ModuleId
						});
						IDeviceIdentification deviceID = null;
						if (APEnvironment.DeviceMgr.TryGetTargetDeviceID(((IObject)val2).MetaObject.ProjectHandle, ((IObject)val2).MetaObject.ObjectGuid, val3, out deviceID))
						{
							foreach (IConnector7 item2 in (IEnumerable)new DeviceObject(bCreateBitChannels: false, deviceID).Connectors)
							{
								IConnector7 childConnector2 = item2;
								if (DeviceManager.CheckMatchInterface(conUpdate, childConnector2))
								{
									flag = true;
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
				if (!flag)
				{
					val.Add((object)val2);
				}
				else if (!_alModuleGuids.Contains(((IObject)val2).MetaObject.ObjectGuid))
				{
					_alModuleGuids.Add(((IObject)val2).MetaObject.ObjectGuid);
					AdapterBase.CheckAndChangeConnectorInChild(val2, conUpdate);
				}
			}
			if (val.Count > 0)
			{
				alDevices = val;
			}
			else
			{
				alDevices.Clear();
			}
		}

		internal void Remove(Guid guidDevice)
		{
			_alModuleGuids.Remove(guidDevice);
		}

		internal void Append(Guid guidDevice)
		{
			if (_alModuleGuids.Count >= _nMaxDevices)
			{
				throw new InvalidOperationException("Maximum number of devices already reached");
			}
			if (!_alModuleGuids.Contains(guidDevice))
			{
				_alModuleGuids.Add(guidDevice);
			}
		}

		internal void Insert(int iIndex, Guid guidDevice)
		{
			if (_alModuleGuids.Count >= _nMaxDevices)
			{
				throw new InvalidOperationException("Maximum number of devices already reached");
			}
			if (!_alModuleGuids.Contains(guidDevice))
			{
				_alModuleGuids.Insert(iIndex, guidDevice);
			}
		}

		public void ClearModules()
		{
			_alModuleGuids.Clear();
		}
	}
}
