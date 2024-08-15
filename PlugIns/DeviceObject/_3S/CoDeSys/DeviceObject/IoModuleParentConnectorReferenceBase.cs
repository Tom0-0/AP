using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class IoModuleParentConnectorReferenceBase : IoModuleReferenceBase
	{
		private int _nConnectorId;

		public override int ConnectorId => _nConnectorId;

		public override bool IsConnector => true;

		protected abstract IConnector GetThisConnector(out IMetaObject mo);

		protected abstract bool GetParentModule(out IMetaObject moParent, out IConnector parent, out IDeviceObject device);

		internal IoModuleParentConnectorReferenceBase(Guid guidObject, int nProjectHandle, int nConnectorId)
			: base(guidObject, nProjectHandle)
		{
			_nConnectorId = nConnectorId;
		}

		protected IConnector GetConnector(IDeviceObject device, int nConnectorId)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val = item;
				if (val.ConnectorId == nConnectorId)
				{
					return val;
				}
			}
			return null;
		}

		protected IConnector GetConnectedChildConnector(Guid guidParent, Guid guidChild)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(base.ProjectHandle, guidChild))
			{
				return null;
			}
			IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, guidChild).Object;
			IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
			if (val == null)
			{
				return null;
			}
			foreach (IConnector item in (IEnumerable)val.Connectors)
			{
				IConnector val2 = item;
				foreach (IAdapter item2 in (IEnumerable)val2.Adapters)
				{
					if (Array.IndexOf(item2.Modules, guidParent) >= 0)
					{
						return val2;
					}
				}
			}
			return null;
		}

		internal override IoModuleReferenceBase GetParent()
		{
			if (GetParentModule(out var moParent, out var parent, out var _))
			{
				if (parent == null)
				{
					return new IoModuleDeviceReference(moParent.ObjectGuid, base.ProjectHandle);
				}
				return new IoModuleChildConnectorReference(moParent.ObjectGuid, base.ProjectHandle, parent.ConnectorId);
			}
			return null;
		}

		internal override IoModuleReferenceBase GetFirstChild()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject mo;
			IConnector thisConnector = GetThisConnector(out mo);
			if (thisConnector == null)
			{
				return null;
			}
			foreach (IAdapter item in (IEnumerable)thisConnector.Adapters)
			{
				Guid[] modules = item.Modules;
				foreach (Guid guid in modules)
				{
					if (guid != Guid.Empty)
					{
						IConnector connectedChildConnector = GetConnectedChildConnector(mo.ObjectGuid, guid);
						if (connectedChildConnector != null)
						{
							return new IoModuleChildConnectorReference(guid, base.ProjectHandle, connectedChildConnector.ConnectorId);
						}
					}
				}
			}
			return null;
		}

		internal override IoModuleReferenceBase GetNextSibling()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (!GetParentModule(out var moParent, out var parent, out var device))
			{
				return null;
			}
			if (parent != null)
			{
				_=parent.ConnectorId;
			}
			bool flag = false;
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val = item;
				if (val is ErrorConnector)
				{
					return null;
				}
				if ((int)val.ConnectorRole != 0)
				{
					continue;
				}
				if (val.ConnectorId == _nConnectorId)
				{
					flag = true;
				}
				else if (flag)
				{
					IConnector activeChildConnector = (IConnector)(object)DeviceManager.GetActiveChildConnector(device);
					if (activeChildConnector == null || val.HostPath == -1 || val.HostPath == activeChildConnector.ConnectorId)
					{
						return CreateParentConnectorReference(moParent, val);
					}
				}
			}
			return null;
		}

		internal override IoModuleReferenceBase GetPrevSibling()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (!GetParentModule(out var moParent, out var parent, out var device))
			{
				return null;
			}
			if (parent != null)
			{
				_=parent.ConnectorId;
			}
			IConnector val = null;
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val2 = item;
				if ((int)val2.ConnectorRole != 0)
				{
					continue;
				}
				if (val2.ConnectorId == _nConnectorId)
				{
					if (val == null)
					{
						return null;
					}
					return CreateParentConnectorReference(moParent, val);
				}
				val = val2;
			}
			return null;
		}
	}
}
