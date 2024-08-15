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
	[TypeGuid("{6d49a564-d0d1-4914-86ee-4465387ab144}")]
	[StorageVersion("3.3.0.0")]
	public class SlotAdapter : GenericObject2, ISlotAdapter, IAdapter, IAdapterBase, IGenericObject, IArchivable, ICloneable, IComparable
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("AllowEmptySlot")]
		[StorageVersion("3.3.0.0")]
		private bool _bAllowEmptySlot;

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

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("IncludeSlotNumber")]
		[StorageVersion("3.5.4.0")]
		[StorageDefaultValue(false)]
		private bool _bIncludeSlotNumber;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("StartSlotNumber")]
		[StorageVersion("3.5.4.0")]
		[StorageDefaultValue(0u)]
		private uint _uiStartSlotNumber;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("SlotNames")]
		[StorageVersion("3.5.6.0")]
		[StorageDefaultValue(null)]
		private ArrayList _arSlotNames;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("HiddenSlot")]
		[StorageVersion("3.5.15.0")]
		[StorageDefaultValue(false)]
		private bool _bHiddenSlot;

		private bool _bSubDevicesCollapsed;

		public bool SubDevicesCollapsed => _bSubDevicesCollapsed;

		public bool AllowEmptySlot => _bAllowEmptySlot;

		public Guid[] Modules
		{
			get
			{
				Guid[] array = new Guid[_moduleGuids.Length];
				_moduleGuids.CopyTo(array, 0);
				return array;
			}
		}

		public int ModulesCount => _moduleGuids.Length;

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

		internal Guid[] ModuleGuidsPlain => _moduleGuids;

		internal bool IncludeSlotNumber => _bIncludeSlotNumber;

		internal uint StartSlotNumber => _uiStartSlotNumber;

		internal bool HiddenSlot => _bHiddenSlot;

		internal ArrayList SlotNames => _arSlotNames;

		public SlotAdapter()
			: base()
		{
		}

		public SlotAdapter(XmlReader reader, IDeviceIdentification masterDeviceId)
			: this()
		{
			try
			{
				_bSubDevicesCollapsed = DeviceObjectHelper.ParseBool(reader.GetAttribute("subdevicesCollapsed"), bDefault: false);
				int num = 1;
				string attribute = reader.GetAttribute("count");
				if (attribute != null)
				{
					num = int.Parse(attribute);
				}
				_moduleGuids = new Guid[num];
				_bAllowEmptySlot = true;
				attribute = reader.GetAttribute("allowEmpty");
				if (attribute != null && (attribute.ToLowerInvariant().Equals("false") || attribute.Equals("0")))
				{
					_bAllowEmptySlot = false;
				}
				attribute = reader.GetAttribute("startSlotNumber");
				if (!string.IsNullOrEmpty(attribute))
				{
					uint.TryParse(attribute, out _uiStartSlotNumber);
					_bIncludeSlotNumber = true;
				}
				attribute = reader.GetAttribute("hiddenSlot");
				_bHiddenSlot = DeviceObjectHelper.ParseBool(attribute, _bHiddenSlot);
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
					if (!(name == "SlotName"))
					{
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
						if (_arSlotNames == null)
						{
							_arSlotNames = new ArrayList();
						}
						_arSlotNames.Add(reader.ReadElementString());
					}
				}
				else
				{
					reader.Read();
				}
			}
			if (_arSlotNames != null && val.Count > 0)
			{
				int num2 = 0;
				foreach (DeviceIdentification item in val)
				{
					if (string.Compare(item.BaseName, "$(DeviceName)") == 0 && _arSlotNames.Count >= num2)
					{
						item.BaseName = (string)_arSlotNames[num2];
					}
					num2++;
				}
			}
			_defaultModules = new DeviceIdentification[val.Count];
			val.CopyTo(_defaultModules, 0);
			reader.Read();
		}

		private SlotAdapter(SlotAdapter original)
			: this()
		{
			_bAllowEmptySlot = original._bAllowEmptySlot;
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
			_bIncludeSlotNumber = original._bIncludeSlotNumber;
			_uiStartSlotNumber = original._uiStartSlotNumber;
			if (original._arSlotNames != null)
			{
				_arSlotNames = (ArrayList)original._arSlotNames.Clone();
			}
			_bHiddenSlot = original._bHiddenSlot;
		}

		public override object Clone()
		{
			SlotAdapter slotAdapter = new SlotAdapter(this);
			((GenericObject)slotAdapter).AfterClone();
			return slotAdapter;
		}

		public IDeviceIdentification GetDefaultDevice(int nIndex)
		{
			if (_defaultModules == null || _defaultModules.Length <= nIndex)
			{
				return null;
			}
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
			return CanAddModule(nInsertPosition, ((object)childDevice).GetType());
		}

		public bool CanAddModule(int nInsertPosition, Type objectType)
		{
			if (nInsertPosition >= _moduleGuids.Length)
			{
				return false;
			}
			if (_moduleGuids[nInsertPosition] != Guid.Empty)
			{
				return false;
			}
			if (!typeof(IDeviceObject2).IsAssignableFrom(objectType) && !typeof(SlotDeviceObject).IsAssignableFrom(objectType))
			{
				return typeof(IExplicitConnector).IsAssignableFrom(objectType);
			}
			return true;
		}

		public void AddModule(int nInsertPosition, IMetaObjectStub mosChild)
		{
			if (!ModuleAlreadySet(nInsertPosition, mosChild.ObjectGuid) && !CanAddModule(nInsertPosition, mosChild.ObjectType))
			{
				throw new InvalidOperationException("Cannot set this child.");
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
				_moduleGuids[i] = Guid.Empty;
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

		public void UpdateModules(LList<object> alDevices, IConnector7 conUpdate)
		{
			UpdateModules(alDevices, conUpdate, bOnlyEmptyAdapter: false);
		}

		public void UpdateModules(LList<object> alDevices, IConnector7 conUpdate, bool bOnlyEmptyAdapter)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			for (int i = 0; i < Math.Min(alDevices.Count, _moduleGuids.Length); i++)
			{
				if (alDevices[i] is IDeviceObject && (!bOnlyEmptyAdapter || _moduleGuids[i] == Guid.Empty))
				{
					IDeviceObject val = (IDeviceObject)alDevices[i];
					_moduleGuids[i] = ((IObject)val).MetaObject.ObjectGuid;
					AdapterBase.CheckAndChangeConnectorInChild(val, conUpdate);
				}
				if (alDevices[i] is IExplicitConnector && (!bOnlyEmptyAdapter || _moduleGuids[i] == Guid.Empty))
				{
					IExplicitConnector val2 = (IExplicitConnector)alDevices[i];
					_moduleGuids[i] = ((IObject)val2).MetaObject.ObjectGuid;
				}
			}
			if (alDevices.Count > _moduleGuids.Length)
			{
				alDevices.RemoveRange(0, _moduleGuids.Length);
			}
			else
			{
				alDevices.Clear();
			}
		}

		internal void SetDevice(IDeviceObject slot, int nPosition)
		{
			_moduleGuids[nPosition] = ((IObject)slot).MetaObject.ObjectGuid;
		}

		internal void SetDevice(Guid guidDevice, int nPosition)
		{
			_moduleGuids[nPosition] = guidDevice;
		}

		internal bool AddDevice(IDeviceObject slot)
		{
			for (int i = 0; i < _moduleGuids.Length; i++)
			{
				if (_moduleGuids[i].Equals(Guid.Empty))
				{
					_moduleGuids[i] = ((IObject)slot).MetaObject.ObjectGuid;
					return true;
				}
			}
			return false;
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

		private DeviceIdentification ReadDefaultModule(XmlReader reader)
		{
			try
			{
				return new DeviceIdentification
				{
					Type = int.Parse(reader.GetAttribute("deviceType")),
					Id = reader.GetAttribute("deviceId"),
					Version = reader.GetAttribute("version")
				};
			}
			catch (Exception value)
			{
				Debug.WriteLine(value);
				return null;
			}
			finally
			{
				reader.Skip();
			}
		}

		internal string GetSlotName(int iSlotIndex)
		{
			string result = string.Empty;
			if (_defaultModules == null || _defaultModules.Length <= iSlotIndex)
			{
				result = ((_arSlotNames != null && _arSlotNames.Count > iSlotIndex) ? ((string)_arSlotNames[iSlotIndex]) : ((!_bIncludeSlotNumber) ? Strings.EmptySlotName : string.Format(Strings.EmptySlotNameWithNumber, iSlotIndex + _uiStartSlotNumber)));
			}
			else if (_defaultModules[iSlotIndex].BaseName != "$(DeviceName)")
			{
				result = _defaultModules[iSlotIndex].BaseName;
			}
			return result;
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
