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
	[TypeGuid("{850f737d-2abf-45f4-89c1-63ed6b29593d}")]
	[StorageVersion("3.3.0.0")]
	public class Connector : ConnectorBase
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("ConnectorObjectGuid")]
		[StorageVersion("3.3.0.0")]
		protected Guid _guidConnectorObject = Guid.Empty;

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("IsExplicit")]
		[StorageVersion("3.3.0.0")]
		protected bool _bExplicit;

		private IDeviceObjectBase _device;

		public Guid ExplicitConnectorGuid
		{
			get
			{
				if (!_bExplicit)
				{
					throw new InvalidOperationException("This operation is only valid on explicit connectors");
				}
				return _guidConnectorObject;
			}
		}

		public override bool Disabled
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if ((int)base.ConnectorRole == 0)
				{
					return false;
				}
				if (_device == null)
				{
					return false;
				}
				return ((IDeviceObject)_device).Disable;
			}
		}

		public override bool Excluded
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				if ((int)base.ConnectorRole == 0)
				{
					return false;
				}
				if (_device == null)
				{
					return false;
				}
				return ((IDeviceObject)_device).Exclude;
			}
		}

		public override int SubObjectsCount
		{
			get
			{
				if (IsExplicit)
				{
					if (!(_guidConnectorObject == Guid.Empty))
					{
						return 1;
					}
					return 0;
				}
				return base.SubObjectsCount;
			}
		}

		public override bool IsExplicit => _bExplicit;

		public IDeviceObjectBase Device
		{
			get
			{
				return _device;
			}
			set
			{
				_device = value;
				_hostParameterSet.Device = (IDeviceObject2)_device;
			}
		}

		public int ModulesCount
		{
			get
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Expected O, but got Unknown
				if (IsExplicit)
				{
					return 0;
				}
				int num = 0;
				foreach (IAdapter item in (IEnumerable)base.Adapters)
				{
					IAdapter val = item;
					num += val.ModulesCount;
				}
				return num;
			}
		}

		public Connector()
		{
		}

		internal Connector(XmlNode node, TypeList types, IDeviceIdentification deviceId, bool bCreateBitChannels)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			_bExplicit = DeviceObjectHelper.ParseBool(((XmlElement)node).GetAttribute("explicit"), bDefault: false);
			Import(node, types, deviceId, bUpdate: false, bCreateBitChannels);
			if (_bExplicit && (int)_role == 1)
			{
				_bExplicit = false;
			}
		}

		protected Connector(Connector original)
			: base(original)
		{
			_guidConnectorObject = original._guidConnectorObject;
			_bExplicit = original._bExplicit;
		}

		public override object Clone()
		{
			Connector connector = new Connector(this);
			((GenericObject)connector).AfterClone();
			return connector;
		}

		public override IDeviceObject GetDeviceObject()
		{
			if (_device == null)
			{
				throw new InvalidOperationException("Connector is not associated with a device object");
			}
			return (IDeviceObject)_device;
		}

		public override IIoProvider GetIoProviderParent()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected O, but got Unknown
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			if ((int)base.ConnectorRole == 0)
			{
				DeviceObject deviceObject = Device as DeviceObject;
				if (deviceObject == null)
				{
					Debug.Fail("The device of a parent connector should always be a DeviceObject");
					return null;
				}
				if (base.HostPath == -1)
				{
					Connector activeChildConnector = DeviceManager.GetActiveChildConnector((IDeviceObject)(object)deviceObject);
					if (activeChildConnector == null)
					{
						return (IIoProvider)(object)deviceObject;
					}
					return (IIoProvider)(object)activeChildConnector;
				}
				return (IIoProvider)(object)deviceObject.GetConnectorById(base.HostPath);
			}
			if (Device == null)
			{
				return null;
			}
			IMetaObject metaObject = Device.GetMetaObject();
			if (metaObject == null)
			{
				return null;
			}
			if (metaObject.ParentObjectGuid == Guid.Empty || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaObject.ProjectHandle, metaObject.ParentObjectGuid))
			{
				return null;
			}
			IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObject.ProjectHandle, metaObject.ParentObjectGuid).Object;
			if (@object is IExplicitConnector)
			{
				if (@object is IIoProvider)
				{
					return (IIoProvider)(object)((@object is IIoProvider) ? @object : null);
				}
				return null;
			}
			if (@object is IDeviceObject)
			{
				foreach (IConnector item in (IEnumerable)((IDeviceObject)@object).Connectors)
				{
					IConnector val = item;
					if ((int)val.ConnectorRole != 0)
					{
						continue;
					}
					foreach (IAdapter item2 in (IEnumerable)val.Adapters)
					{
						if (Array.IndexOf(item2.Modules, metaObject.ObjectGuid) >= 0)
						{
							return (IIoProvider)(object)((val is IIoProvider) ? val : null);
						}
					}
				}
				return null;
			}
			return null;
		}

		public override LList<IIoProvider> GetIoProviderChildren()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			LList<IIoProvider> val = new LList<IIoProvider>();
			bool flag = false;
			if ((int)base.ConnectorRole == 1)
			{
				foreach (Connector item in (IEnumerable)((IDeviceObject)Device).Connectors)
				{
					if ((int)item.ConnectorRole != 0)
					{
						continue;
					}
					flag = true;
					if (item.HostPath != base.ConnectorId && item.HostPath != -1)
					{
						continue;
					}
					if (item.IsExplicit)
					{
						ExplicitConnector connectorObject = item.GetConnectorObject();
						if (connectorObject != null)
						{
							val.Add((IIoProvider)(object)connectorObject);
						}
					}
					else
					{
						val.Add((IIoProvider)(object)item);
					}
				}
				if (val.Count == 0 && flag)
				{
					foreach (Connector item2 in (IEnumerable)((IDeviceObject)Device).Connectors)
					{
						if ((int)item2.ConnectorRole == 0)
						{
							foreach (IAdapter item3 in (IEnumerable)base.Adapters)
							{
								Guid[] modules = item3.Modules;
								foreach (Guid guid in modules)
								{
									if (!item2.IsExplicit && item2.Device != null && ((IObject)item2.Device).MetaObject != null && ((IObject)item2.Device).MetaObject.ParentObjectGuid == guid)
									{
										val.Add((IIoProvider)(object)item2);
									}
								}
							}
						}
					}
					return val;
				}
				return val;
			}
			if (IsExplicit)
			{
				ExplicitConnector connectorObject2 = GetConnectorObject();
				if (connectorObject2 != null)
				{
					return connectorObject2.GetIoProviderChildren();
				}
				return new LList<IIoProvider>(0);
			}
			foreach (IAdapter item4 in (IEnumerable)base.Adapters)
			{
				Guid[] modules = item4.Modules;
				foreach (Guid guid2 in modules)
				{
					if (!(guid2 != Guid.Empty))
					{
						continue;
					}
					try
					{
						int projectHandle = Device.GetMetaObject().ProjectHandle;
						if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(projectHandle, guid2))
						{
							continue;
						}
						IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, guid2).Object;
						IDeviceObject val2 = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
						bool flag2 = false;
						if (val2 != null)
						{
							flag2 = !val2.AllowsDirectCommunication;
							if (!flag2)
							{
								if (val2.Attributes["IoApplicationProvider"] != null && val2.Attributes["IoApplicationProvider"].ToUpperInvariant() == "PARENT")
								{
									flag2 = true;
								}
								if (val2.Attributes["UseParentPLC"] != null && val2.Attributes["UseParentPLC"].ToUpperInvariant() == "TRUE")
								{
									flag2 = true;
								}
							}
						}
						if (flag2)
						{
							IConnector val3 = FindConnectedChildConnector(val2);
							if (val3 != null)
							{
								val.Add((IIoProvider)(object)((val3 is IIoProvider) ? val3 : null));
							}
						}
					}
					catch
					{
					}
				}
			}
			return val;
		}

		public override bool IoProviderEquals(IIoProvider provider)
		{
			Connector connector = provider as Connector;
			if (connector == null)
			{
				return false;
			}
			if (connector.Device == null)
			{
				return false;
			}
			if (((IObject)connector.Device).MetaObject == null)
			{
				return false;
			}
			IMetaObject metaObject = Device.GetMetaObject();
			if (metaObject == null)
			{
				return false;
			}
			if (metaObject.ObjectGuid == ((IObject)connector.Device).MetaObject.ObjectGuid && metaObject.ProjectHandle == ((IObject)connector.Device).MetaObject.ProjectHandle)
			{
				return base.ConnectorId == connector.ConnectorId;
			}
			return false;
		}

		public override string GetIoBaseName()
		{
			return LanguageModelHelper.GetIoBaseName((IDeviceObject)Device);
		}

		public override IMetaObject GetMetaObject()
		{
			Debug.Assert(_device != null);
			return _device.GetMetaObject();
		}

		public override IIoModuleIterator CreateModuleIterator()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject metaObject = GetMetaObject();
			IoModuleReferenceBase start = (((int)base.ConnectorRole != 0) ? ((IoModuleReferenceBase)new IoModuleChildConnectorReference(metaObject.ObjectGuid, metaObject.ProjectHandle, base.ConnectorId)) : ((IoModuleReferenceBase)new IoModuleParentConnectorReference(metaObject.ObjectGuid, metaObject.ProjectHandle, base.ConnectorId)));
			return (IIoModuleIterator)(object)new IoModuleIterator(start);
		}

		public override void UpdateChildObjects(int nProjectHandle, Guid guidObject, ref int nModuleOffset, bool bUpdate, bool bCreateBitChannels, bool bVersionUpgrade, LList<DeviceIdUpdate> liDevicesToUpdate = null)
		{
			Debug.Assert(_device != null);
			if (IsExplicit)
			{
				ExplicitConnector explicitConnector = new ExplicitConnector((IConnector)(object)this);
				Guid empty = Guid.Empty;
				try
				{
					if (_guidConnectorObject == Guid.Empty && !bUpdate)
					{
						empty = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidObject, Guid.NewGuid(), (IObject)(object)explicitConnector, explicitConnector.VisibleInterfaceName, -1);
						nModuleOffset++;
					}
					else
					{
						empty = _guidConnectorObject;
						if (_guidConnectorObject != Guid.Empty)
						{
							LList<Guid> val = new LList<Guid>();
							IMetaObject objectToModify = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(nProjectHandle, _guidConnectorObject);
							try
							{
								if (objectToModify != null && objectToModify.Object is ExplicitConnector)
								{
									IObject @object = objectToModify.Object;
									IExplicitConProperty val2 = (IExplicitConProperty)(object)new ModuleType(((IConnector)((@object is IExplicitConnector) ? @object : null)).ModuleType);
									objectToModify.AddProperty((IObjectProperty)(object)val2);
									(objectToModify.Object as ExplicitConnector).UpdateConnector(((GenericObject)explicitConnector).Clone() as ExplicitConnector, val, bVersionUpgrade);
								}
							}
							finally
							{
								if (objectToModify != null)
								{
									((IObjectManager)APEnvironment.ObjectMgr).SetObject(objectToModify, true, (object)null);
								}
								nModuleOffset++;
							}
							if (objectToModify != null && explicitConnector != null && objectToModify.Name != explicitConnector.VisibleInterfaceName)
							{
								try
								{
									((IObjectManager)APEnvironment.ObjectMgr).RenameObject(nProjectHandle, _guidConnectorObject, explicitConnector.VisibleInterfaceName);
								}
								catch
								{
								}
							}
							if (val.Count > 0)
							{
								foreach (Guid item in val)
								{
									try
									{
										((IObjectManager2)APEnvironment.ObjectMgr).RemoveObjectWithoutParentCheck(nProjectHandle, item);
									}
									catch
									{
									}
								}
							}
						}
						else
						{
							empty = ((IObjectManager)APEnvironment.ObjectMgr).AddObject(nProjectHandle, guidObject, Guid.NewGuid(), (IObject)(object)explicitConnector, explicitConnector.VisibleInterfaceName, nModuleOffset);
							nModuleOffset++;
						}
					}
				}
				catch
				{
					APEnvironment.MessageService.Error(Strings.ErrorUpdateDevice, "ErrorUpdateDevice", Array.Empty<object>());
					return;
				}
				IMetaObject val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(nProjectHandle, empty);
				explicitConnector = (ExplicitConnector)(object)val3.Object;
				int nModuleOffset2 = 0;
				explicitConnector.UpdateChildObjects(nProjectHandle, empty, ref nModuleOffset2, bUpdate, bCreateBitChannels, bVersionUpgrade);
				try
				{
					val3 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(val3);
					((DeviceObject)_device).UpdateIoProvider((IIoProvider)(object)(ExplicitConnector)(object)val3.Object);
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, true, (object)null);
				}
				catch
				{
					if (val3 != null)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val3, false, (object)null);
					}
				}
			}
			else
			{
				base.UpdateChildObjects(nProjectHandle, guidObject, ref nModuleOffset, bUpdate, bCreateBitChannels, bVersionUpgrade, liDevicesToUpdate);
			}
		}

		public override int GetChildIndex(Guid subObjectGuid, out int nNumSubmoduls)
		{
			if (IsExplicit)
			{
				nNumSubmoduls = ((!(_guidConnectorObject == Guid.Empty)) ? 1 : 0);
				if (subObjectGuid == _guidConnectorObject)
				{
					return 0;
				}
				return -1;
			}
			return base.GetChildIndex(subObjectGuid, out nNumSubmoduls);
		}

		public override bool InsertChild(int nInsertPosition, IObject childDevice, Guid newChildGuid)
		{
			if (IsExplicit)
			{
				if (_guidConnectorObject != Guid.Empty)
				{
					return false;
				}
				if (nInsertPosition == 0 && childDevice is ExplicitConnector)
				{
					if (childDevice.MetaObject != null)
					{
						_guidConnectorObject = childDevice.MetaObject.ObjectGuid;
						return true;
					}
					return true;
				}
				return false;
			}
			return base.InsertChild(nInsertPosition, childDevice, newChildGuid);
		}

		public override void OnDeviceRenamed(string stOldDeviceName)
		{
			if (IsExplicit)
			{
				if (_guidConnectorObject == Guid.Empty)
				{
					return;
				}
				IMetaObject val = null;
				try
				{
					val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(GetMetaObject().ProjectHandle, _guidConnectorObject);
					((ExplicitConnector)(object)val.Object).OnDeviceRenamed(stOldDeviceName);
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
				}
				catch
				{
					if (val != null)
					{
						((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
					}
				}
			}
			else
			{
				base.OnDeviceRenamed(stOldDeviceName);
			}
		}

		internal override void PreparePaste()
		{
			if (IsExplicit)
			{
				_guidExpectedConnectorObject = _guidConnectorObject;
				_guidConnectorObject = Guid.Empty;
			}
			else
			{
				base.PreparePaste();
			}
		}

		internal override void UpdatePasteModuleGuid(Guid oldGuid, Guid newGuid)
		{
			if (IsExplicit)
			{
				if (_guidExpectedConnectorObject == oldGuid)
				{
					_guidExpectedConnectorObject = newGuid;
				}
			}
			else
			{
				base.UpdatePasteModuleGuid(oldGuid, newGuid);
			}
		}

		internal override bool IsExpectedModule(Guid guidModule)
		{
			if (IsExplicit)
			{
				if (_guidExpectedConnectorObject == guidModule)
				{
					return true;
				}
				return false;
			}
			return base.IsExpectedModule(guidModule);
		}

		internal override bool SetExpectedModule(Guid guidModule)
		{
			if (IsExplicit)
			{
				if (_guidExpectedConnectorObject == guidModule)
				{
					_guidExpectedConnectorObject = Guid.Empty;
					_guidConnectorObject = guidModule;
					return true;
				}
				return false;
			}
			return base.SetExpectedModule(guidModule);
		}

		internal void SetPositionIds(LocalUniqueIdGenerator idGenerator)
		{
			if (!IsExplicit)
			{
				_driverInfo.SetPositionIds(idGenerator);
				_hostParameterSet.SetPositionIds((IUniqueIdGenerator)(object)idGenerator);
			}
		}

		protected IConnector FindConnectedChildConnector(IDeviceObject child)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			Guid objectGuid = Device.GetMetaObject().ObjectGuid;
			foreach (IConnector item in (IEnumerable)child.Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole != 1)
				{
					continue;
				}
				foreach (IAdapter item2 in (IEnumerable)val.Adapters)
				{
					if (Array.IndexOf(item2.Modules, objectGuid) >= 0)
					{
						return val;
					}
				}
			}
			return null;
		}

		public ExplicitConnector GetConnectorObject()
		{
			return GetConnectorObject(bToModify: false);
		}

		public ExplicitConnector GetConnectorObject(bool bToModify)
		{
			Debug.Assert(_device != null);
			Debug.Assert(IsExplicit);
			IMetaObject metaObject = GetMetaObject();
			if (metaObject == null)
			{
				return null;
			}
			if (_guidConnectorObject == Guid.Empty)
			{
				return null;
			}
			ExplicitConnector result = null;
			try
			{
				if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaObject.ProjectHandle, _guidConnectorObject))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObject.ProjectHandle, _guidConnectorObject);
					if (objectToRead != null)
					{
						if (objectToRead.ParentObjectGuid != metaObject.ObjectGuid)
						{
							Debug.Fail("Invalid project structure");
						}
						else if (!(objectToRead.Object is ExplicitConnector))
						{
							Debug.Fail($"Not a connector object: <{objectToRead.Name}> of type <{((object)objectToRead.Object).GetType().FullName}>");
						}
						else
						{
							result = (ExplicitConnector)(object)objectToRead.Object;
						}
					}
					return result;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}
	}
}
