using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{62cac7cc-2678-453e-9651-79afc8122e9b}")]
	public class StringTable : IStringTable4, IStringTable3, IStringTable2, IStringTable
	{
		private IDeviceObject _device;

		private IDeviceDescription2 _deviceDesc;

		public StringTable()
		{
			_device = null;
		}

		public StringTable(IDeviceObject device)
		{
			_device = device;
			IDeviceObject device2 = _device;
			IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((device2 is IDeviceObject5) ? device2 : null)).DeviceIdentificationNoSimulation;
			ref IDeviceDescription2 deviceDesc = ref _deviceDesc;
			IDeviceDescription device3 = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceIdentificationNoSimulation);
			deviceDesc = (IDeviceDescription2)(object)((device3 is IDeviceDescription2) ? device3 : null);
		}

		public IStringRef CreateStringRef(string stNamespace, string stIdentifier, string stDefault)
		{
			return (IStringRef)(object)new StringRef(stNamespace, stIdentifier, stDefault);
		}

		public void RemoveLocalizedString(string stNamespace, string stLanguage, string stIdentifier)
		{
			if (!(_device is DeviceObject))
			{
				return;
			}
			foreach (LanguageStringRef item in (_device as DeviceObject).AdditionalStringTable)
			{
				if (item.Namespace == stNamespace && item.Language == stLanguage && item.Identifier == stIdentifier)
				{
					(_device as DeviceObject).AdditionalStringTable.Remove(item);
					break;
				}
			}
			if (_deviceDesc != null && _deviceDesc.StringTable != null)
			{
				((IStringTable)_deviceDesc.StringTable).RemoveLocalizedString(stNamespace, stLanguage, stIdentifier);
			}
		}

		public void AddLocalizedString(string stNamespace, string stLanguage, string stIdentifier, string stValue)
		{
			if (_device is DeviceObject)
			{
				RemoveLocalizedString(stNamespace, stLanguage, stIdentifier);
				(_device as DeviceObject).AdditionalStringTable.Add(new LanguageStringRef(stLanguage, stNamespace, stIdentifier, stValue));
				if (_deviceDesc != null && _deviceDesc.StringTable != null)
				{
					((IStringTable)_deviceDesc.StringTable).AddLocalizedString(stNamespace, stLanguage, stIdentifier, stValue);
				}
			}
		}

		public bool ResolveString(string stNamespace, string stIdentifier, string stDefault, out string stValue)
		{
			return ResolveString(stNamespace, null, stIdentifier, stDefault, out stValue);
		}

		public IStringRef[] GetStringTable(string stLanguage)
		{
			if (_device == null)
			{
				return null;
			}
			IDeviceObject device = _device;
			IDeviceIdentification deviceIdentificationNoSimulation = ((IDeviceObject5)((device is IDeviceObject5) ? device : null)).DeviceIdentificationNoSimulation;
			IDeviceDescription device2 = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(deviceIdentificationNoSimulation);
			IDeviceDescription3 val = (IDeviceDescription3)(object)((device2 is IDeviceDescription3) ? device2 : null);
			if (val != null && ((IDeviceDescription2)val).StringTable != null)
			{
				return val.StringTable3.GetStringTable(stLanguage);
			}
			return null;
		}

		public bool ResolveString(string stNamespace, string stLanguage, string stIdentifier, string stDefault, out string stValue)
		{
			if (_device == null)
			{
				stValue = stDefault;
				return false;
			}
			if (_deviceDesc != null)
			{
				IStringTable2 stringTable = _deviceDesc.StringTable;
				if (stringTable != null)
				{
					if (stringTable is IStringTable4)
					{
						return ((IStringTable4)((stringTable is IStringTable4) ? stringTable : null)).ResolveString(stNamespace, stLanguage, stIdentifier, stDefault, out stValue);
					}
					return stringTable.ResolveString(stNamespace, stIdentifier, stDefault, out stValue);
				}
			}
			stValue = stDefault;
			return false;
		}
	}
}
