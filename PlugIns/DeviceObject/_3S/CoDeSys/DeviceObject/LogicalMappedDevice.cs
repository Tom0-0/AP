#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.NavigatorControl;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{D303557E-44B8-43cc-BC47-79C510479A70}")]
	[StorageVersion("3.4.1.0")]
	public class LogicalMappedDevice : GenericObject2, IMappedDevice
	{
		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("MatchingLogicalDevices")]
		[StorageVersion("3.4.1.0")]
		private MatchingLogicalDeviceList _MatchingLogicalDevices = new MatchingLogicalDeviceList();

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("MappedDevice")]
		[StorageVersion("3.4.1.0")]
		private string _stMappedDevice = string.Empty;

		[DefaultDuplication(DuplicationMethod.Deep)]
		[DefaultSerialization("InsertAutoIndex")]
		[StorageVersion("3.4.1.0")]
		private int _nInsertAutoIndex = -1;

		private IMetaObject _metaObject;

		public IMetaObject MetaObject
		{
			get
			{
				return _metaObject;
			}
			set
			{
				_metaObject = value;
			}
		}

		internal int InsertAutoIndex => _nInsertAutoIndex;

		internal MatchingLogicalDeviceList MatchingLogicalDevices => _MatchingLogicalDevices;

		public string MappedDevice
		{
			get
			{
				string result = string.Empty;
				if (_metaObject != null && _metaObject.Object is ILogicalDevice)
				{
					IObject @object = _metaObject.Object;
					if (((ILogicalDevice)((@object is ILogicalDevice) ? @object : null)).IsPhysical)
					{
						result = _stMappedDevice;
					}
					else
					{
						Guid getMappedDevice = GetMappedDevice;
						if (getMappedDevice != Guid.Empty)
						{
							result = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, getMappedDevice).Name;
						}
					}
				}
				return result;
			}
			set
			{
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				if (_metaObject != null && _metaObject.Object is ILogicalDevice && (this._metaObject.Object as ILogicalDevice).IsLogical)
				{
					throw new DeviceObjectException((DeviceObjectExeptionReason)17, "It is not allowed to set MappedDevice in logical device");
				}
				UpdateNavigator();
				_stMappedDevice = value;
				UpdateNavigator();
			}
		}

		public SortedList<string, Guid> MatchingDevices
		{
			get
			{
				SortedList<string, Guid> sortedList = new SortedList<string, Guid>();
				if (_metaObject != null)
				{
					Guid[] array2;
					if (_metaObject.Object is LogicalIODevice)
					{
						IMetaObjectStub hostStub = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
						LogicalIODevice logicalIODevice = _metaObject.Object as LogicalIODevice;
						if (logicalIODevice.DeviceIdentificationNoSimulation.Type == 152)
						{
							Guid[] array = new Guid[LogicalIONotification.LogicalExchangeGVL.DeviceGuids.Count];
							LogicalIONotification.LogicalExchangeGVL.DeviceGuids.CopyTo(array, 0);
							array2 = array;
							foreach (Guid guid in array2)
							{
								IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, guid);
								if (!typeof(ILogicalGVLObject).IsAssignableFrom(metaObjectStub.ObjectType))
								{
									continue;
								}
								IMetaObjectStub hostStub2 = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, guid);
								if (!(hostStub.ObjectGuid == hostStub2.ObjectGuid))
								{
									continue;
								}
								IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, guid).Object;
								ILogicalGVLObject val = (ILogicalGVLObject)(object)((@object is ILogicalGVLObject) ? @object : null);
								if (val == null || !(val is ILogicalDevice))
								{
									continue;
								}
								foreach (LogicalMappedDevice item in (IEnumerable)((ILogicalDevice)((val is ILogicalDevice) ? val : null)).MappedDevices)
								{
									if (!item.IsMapped)
									{
										sortedList[metaObjectStub.Name] = metaObjectStub.ObjectGuid;
									}
								}
							}
							return sortedList;
						}
						{
							foreach (Guid deviceGuid in DeviceObjectHelper.PhysicalDevices.DeviceGuids)
							{
								IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, deviceGuid);
								if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub2.ObjectType))
								{
									continue;
								}
								IMetaObjectStub hostStub3 = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, deviceGuid);
								if (!(hostStub.ObjectGuid == hostStub3.ObjectGuid))
								{
									continue;
								}
								IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, deviceGuid).Object;
								IDeviceObject5 val2 = (IDeviceObject5)(object)((object2 is IDeviceObject5) ? object2 : null);
								if (val2 == null || !(val2 is ILogicalDevice) || val2 is LogicalIODevice)
								{
									continue;
								}
								foreach (LogicalMappedDevice item2 in (IEnumerable)((ILogicalDevice)((val2 is ILogicalDevice) ? val2 : null)).MappedDevices)
								{
									if (item2.IsMapped)
									{
										continue;
									}
									foreach (MatchingLogicalDevice matchingLogicalDevice in item2._MatchingLogicalDevices)
									{
										if (CheckMatching(matchingLogicalDevice.DeviceIdentification, logicalIODevice.DeviceIdentificationNoSimulation))
										{
											sortedList[metaObjectStub2.Name] = metaObjectStub2.ObjectGuid;
										}
									}
								}
							}
							return sortedList;
						}
					}
					IMetaObjectStub hostStub4 = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
					Guid[] array3 = new Guid[DeviceObjectHelper.LogicalDevices.DeviceGuids.Count];
					DeviceObjectHelper.LogicalDevices.DeviceGuids.CopyTo(array3, 0);
					array2 = array3;
					foreach (Guid guid2 in array2)
					{
						IMetaObjectStub metaObjectStub3 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_metaObject.ProjectHandle, guid2);
						if (!typeof(IDeviceObject).IsAssignableFrom(metaObjectStub3.ObjectType))
						{
							continue;
						}
						IMetaObjectStub hostStub5 = DeviceObjectHelper.GetHostStub(_metaObject.ProjectHandle, guid2);
						if (hostStub4 == null || hostStub5 == null || !(hostStub4.ObjectGuid == hostStub5.ObjectGuid))
						{
							continue;
						}
						IObject object3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, guid2).Object;
						IDeviceObject5 val3 = (IDeviceObject5)(object)((object3 is IDeviceObject5) ? object3 : null);
						if (val3 == null)
						{
							continue;
						}
						bool flag = false;
						if (val3 is DeviceObject)
						{
							foreach (LogicalMappedDevice logicalDevice in (val3 as DeviceObject).LogicalDevices)
							{
								if (logicalDevice.IsMapped)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							continue;
						}
						if (_metaObject.Object is LogicalExchangeGVLObject && val3.DeviceIdentificationNoSimulation.Type == 152)
						{
							sortedList[metaObjectStub3.Name] = metaObjectStub3.ObjectGuid;
						}
						foreach (MatchingLogicalDevice matchingLogicalDevice2 in _MatchingLogicalDevices)
						{
							if (CheckMatching(matchingLogicalDevice2.DeviceIdentification, val3.DeviceIdentificationNoSimulation))
							{
								sortedList[metaObjectStub3.Name] = metaObjectStub3.ObjectGuid;
							}
						}
					}
				}
				return sortedList;
			}
		}

		public Guid GetMappedDevice
		{
			get
			{
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				Guid guid = Guid.Empty;
				if (_metaObject != null && _metaObject.Object is ILogicalDevice)
				{
					IObject @object = _metaObject.Object;
					ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
					Guid empty = Guid.Empty;
					if (!DeviceObjectHelper.HostsForLogicalDevices.TryGetValue(_metaObject.ObjectGuid, out empty) && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, _metaObject.ObjectGuid))
					{
						if (val.IsLogical)
						{
							DeviceObjectHelper.IsLogicalDevice(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
						}
						else
						{
							DeviceObjectHelper.IsPhysicalDevice(_metaObject.ProjectHandle, _metaObject.ObjectGuid);
						}
						DeviceObjectHelper.HostsForLogicalDevices.TryGetValue(_metaObject.ObjectGuid, out empty);
					}
					if (empty != Guid.Empty)
					{
						LList<Guid> val4 = default(LList<Guid>);
						if (val.IsLogical)
						{
							LList<Guid> val2 = default(LList<Guid>);
							if (DeviceObjectHelper.MappedDevices.TryGetValue(_metaObject.Name, out val2))
							{
								foreach (Guid item in val2)
								{
									Guid empty2 = Guid.Empty;
									if (DeviceObjectHelper.HostsForLogicalDevices.TryGetValue(item, out empty2) && empty2 == empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, item))
									{
										IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_metaObject.ProjectHandle, item).Object;
										ILogicalDevice val3 = (ILogicalDevice)(object)((object2 is ILogicalDevice) ? object2 : null);
										if (val3 != null)
										{
											foreach (IMappedDevice item2 in (IEnumerable)val3.MappedDevices)
											{
												if (item2.MappedDevice == _metaObject.Name)
												{
													guid = item;
													break;
												}
											}
										}
									}
									if (guid != Guid.Empty)
									{
										return guid;
									}
								}
								return guid;
							}
						}
						else if (DeviceObjectHelper.LogicalNames.TryGetValue(_stMappedDevice, out val4))
						{
							foreach (Guid item3 in val4)
							{
								Guid empty3 = Guid.Empty;
								if (DeviceObjectHelper.HostsForLogicalDevices.TryGetValue(item3, out empty3) && empty3 == empty && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_metaObject.ProjectHandle, item3))
								{
									return item3;
								}
							}
							return guid;
						}
					}
				}
				return guid;
			}
		}

		public int Index
		{
			get
			{
				int num = 0;
				if (_metaObject != null && _metaObject.Object is DeviceObject)
				{
					foreach (LogicalMappedDevice logicalDevice in (_metaObject.Object as DeviceObject).LogicalDevices)
					{
						if (logicalDevice == this)
						{
							return num;
						}
						if (logicalDevice.MappedDevice == _stMappedDevice && !string.IsNullOrEmpty(_stMappedDevice))
						{
							return num;
						}
						num++;
					}
				}
				return 0;
			}
		}

		public bool IsMapped => !string.IsNullOrEmpty(MappedDevice);

		public LogicalMappedDevice()
			: base()
		{
		}

		public LogicalMappedDevice(LogicalMappedDevice original)
			: this()
		{
			_MatchingLogicalDevices = original._MatchingLogicalDevices;
			_stMappedDevice = original._stMappedDevice;
			_nInsertAutoIndex = original._nInsertAutoIndex;
		}

		internal LogicalMappedDevice(XmlNode node)
			: this()
		{
			Import(node);
		}

		internal void Import(XmlNode node)
		{
			Debug.Assert(node is XmlElement);
			XmlElement xmlElement = (XmlElement)node;
			_nInsertAutoIndex = DeviceObjectHelper.ParseInt(xmlElement.GetAttribute("insertAutoIndex"), -1);
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					string name = childNode.Name;
					if (name == "MatchingLogicalDevice")
					{
						_MatchingLogicalDevices.Add(new MatchingLogicalDevice(childNode));
					}
				}
			}
		}

		public override object Clone()
		{
			LogicalMappedDevice logicalMappedDevice = new LogicalMappedDevice(this);
			((GenericObject)logicalMappedDevice).AfterClone();
			return logicalMappedDevice;
		}

		internal void UpdateNavigator()
		{
			Guid getMappedDevice = GetMappedDevice;
			if (Guid.Empty == getMappedDevice || ((IEngine)APEnvironment.Engine).Frame == null)
			{
				return;
			}
			IView[] views = ((IEngine)APEnvironment.Engine).Frame.GetViews();
			if (views == null)
			{
				return;
			}
			IView[] array = views;
			foreach (IView val in array)
			{
				if (val is INavigatorControl)
				{
					((INavigatorControl)((val is INavigatorControl) ? val : null)).UpdateNodes(new Guid[1] { getMappedDevice });
				}
			}
		}

		internal static bool CheckMatching(IDeviceIdentification devId1, IDeviceIdentification devId2)
		{
			if (devId1.Type != devId2.Type)
			{
				return false;
			}
			if (devId1.Version != devId2.Version)
			{
				return false;
			}
			string text = devId1.Id;
			string text2 = devId2.Id;
			int num = text.LastIndexOf(" - ");
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			num = text2.LastIndexOf(" - ");
			if (num >= 0)
			{
				text2 = text2.Substring(0, num);
			}
			if (text != text2)
			{
				return false;
			}
			return true;
		}
	}
}
