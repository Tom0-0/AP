using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class PLCopenDeviceExporter : IDeviceExporter2, IDeviceExporter
	{
		private Device _device;

		private IDeviceCatalogue _deviceCatalogue;

		public IDeviceCatalogue DeviceCatalogue
		{
			get
			{
				if (_deviceCatalogue == null)
				{
					_deviceCatalogue = APEnvironment.CreateFirstDeviceCatalogue();
				}
				return _deviceCatalogue;
			}
		}

		public void Export(XmlWriter writer, IDeviceObject iDeviceObject)
		{
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Expected O, but got Unknown
			if (iDeviceObject is SlotDeviceObject)
			{
				SlotDeviceObject slotDeviceObject = iDeviceObject as SlotDeviceObject;
				if (slotDeviceObject.GetDevice() != null)
				{
					DeviceObjectXML deviceObjectXML = new DeviceObjectXML(slotDeviceObject.GetDevice());
					Device device = new Device();
					deviceObjectXML.Init(device);
					new XmlSerializer(typeof(Device)).Serialize(writer, device);
				}
			}
			else
			{
				if (!(iDeviceObject is DeviceObject))
				{
					return;
				}
				DeviceObject deviceObject = iDeviceObject as DeviceObject;
				DeviceObjectXML deviceObjectXML2 = new DeviceObjectXML(deviceObject);
				Device device2 = new Device();
				deviceObjectXML2.Init(device2);
				int handle = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle;
				Guid parentObjectGuid = deviceObject.MetaObject.ParentObjectGuid;
				if (parentObjectGuid != Guid.Empty)
				{
					IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(handle, parentObjectGuid).Object;
					if (@object is IDeviceObject)
					{
						IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
						foreach (IConnector6 item in (IEnumerable)val.Connectors)
						{
							IConnector6 con = item;
							if (!CheckInFixedAdapter((IConnector)(object)con, deviceObject.MetaObject.ObjectGuid))
							{
								continue;
							}
							Guid[] orderedSubGuids = DeviceManager.GetOrderedSubGuids(val);
							int num = 0;
							Guid[] array = orderedSubGuids;
							foreach (Guid guid in array)
							{
								if (guid == deviceObject.MetaObject.ObjectGuid)
								{
									break;
								}
								IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(handle, guid);
								if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
								{
									num++;
								}
							}
							device2.fixedIndex = num;
							break;
						}
					}
					else if (@object is IExplicitConnector)
					{
						IExplicitConnector con2 = (IExplicitConnector)(object)((@object is IExplicitConnector) ? @object : null);
						if (CheckInFixedAdapter((IConnector)(object)con2, deviceObject.MetaObject.ObjectGuid))
						{
							device2.fixedIndex = deviceObject.MetaObject.Index;
						}
					}
				}
				new XmlSerializer(typeof(Device)).Serialize(writer, device2);
			}
		}

		private bool CheckInFixedAdapter(IConnector con, Guid guid)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IAdapter item in (IEnumerable)con.Adapters)
			{
				IAdapter val = item;
				if (!(val is FixedAdapter))
				{
					continue;
				}
				Guid[] modules = (val as FixedAdapter).Modules;
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i] == guid)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Load(XmlReader reader)
		{
			object obj = new XmlSerializer(typeof(Device)).Deserialize(reader);
			if (obj is Device)
			{
				_device = (Device)obj;
			}
		}

		private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
		{
		}

		private void serializer_UnknownElement(object sender, XmlElementEventArgs e)
		{
		}

		public bool Match(IObject parentObject)
		{
			if (parentObject is SlotDeviceObject)
			{
				string[] possibleInterfacesForPlug = (parentObject as SlotDeviceObject).GetPossibleInterfacesForPlug();
				return Match(possibleInterfacesForPlug);
			}
			if (parentObject is IDeviceObject)
			{
				IDeviceObject parentDevice = (IDeviceObject)(object)((parentObject is IDeviceObject) ? parentObject : null);
				return MatchDevice(parentDevice);
			}
			if (parentObject is IConnector)
			{
				return MatchConnector((IConnector)(object)((parentObject is IConnector) ? parentObject : null));
			}
			ISVNode[] selectedNodes = GetSelectedNodes();
			if (selectedNodes != null && selectedNodes.Length == 0)
			{
				IDeviceIdentification val = PLCopenImport.CreateDeviceIdentification(_device.DeviceType.Item);
				IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val);
				if (device != null)
				{
					return device.AllowTopLevel;
				}
			}
			return false;
		}

		private bool MatchConnector(IConnector connector)
		{
			string[] filter = new string[1] { connector.Interface };
			return Match(filter);
		}

		private bool MatchDevice(IDeviceObject parentDevice)
		{
			int num = ((IObject)parentDevice).MetaObject.SubObjectGuids.Length;
			string[] possibleInterfacesForInsert = parentDevice.GetPossibleInterfacesForInsert(num);
			return Match(possibleInterfacesForInsert);
		}

		private bool Match(string[] filter)
		{
			if (DeviceCatalogue != null)
			{
				IDeviceCatalogueFilter val = DeviceCatalogue.CreateChildConnectorFilter(filter);
				IDeviceIdentification val2 = PLCopenImport.CreateDeviceIdentification(_device.DeviceType.Item);
				IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice(val2);
				if (device != null)
				{
					return val.Match(device);
				}
			}
			return false;
		}

		internal static ISVNode[] GetSelectedNodes()
		{
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject != null)
			{
				return primaryProject.SelectedSVNodes;
			}
			return null;
		}

		public Guid Import(int nProjectHandle, Guid parentGuid, string deviceName)
		{
			IDeviceIdentification deviceID = PLCopenImport.CreateDeviceIdentification(_device.DeviceType.Item);
			try
			{
				APEnvironment.DeviceMgr.IsPLCOpenXMLImport = true;
				return new DeviceBuilder().CreateDevice(nProjectHandle, parentGuid, deviceID, deviceName, _device);
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				APEnvironment.DeviceMgr.IsPLCOpenXMLImport = false;
			}
		}

		public void PlugDevice(int nProjectHandle, Guid slotGuid, string deviceName)
		{
			IDeviceIdentification deviceID = PLCopenImport.CreateDeviceIdentification(_device.DeviceType.Item);
			try
			{
				new DeviceBuilder().PlugDevice(nProjectHandle, slotGuid, deviceID, deviceName, _device);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void ReplaceDevice(int nProjectHandle, Guid deviceGuid, string deviceName)
		{
			IDeviceIdentification deviceID = PLCopenImport.CreateDeviceIdentification(_device.DeviceType.Item);
			try
			{
				new DeviceBuilder().ReplaceDevice(nProjectHandle, deviceGuid, deviceID, deviceName, _device);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
