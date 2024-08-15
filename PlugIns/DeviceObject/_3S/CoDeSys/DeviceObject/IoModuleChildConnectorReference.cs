#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class IoModuleChildConnectorReference : IoModuleReferenceBase
	{
		private int _nConnectorId;

		public override int ConnectorId => _nConnectorId;

		public override bool IsConnector => true;

		public override bool IsExplicitConnector => false;

		internal IoModuleChildConnectorReference(Guid guidObject, int nProjectHandle, int nConnectorId)
			: base(guidObject, nProjectHandle)
		{
			_nConnectorId = nConnectorId;
		}

		internal override IoModuleReferenceBase GetParent()
		{
			IConnector connector;
			return GetParentConnector(out connector);
		}

		internal override IoModuleReferenceBase GetFirstChild()
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, base.ObjectGuid);
			return GetFirstConnectorOfRoleParent(objectToRead, _nConnectorId);
		}

		internal override IoModuleReferenceBase GetNextSibling()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			IConnector connector;
			IoModuleReferenceBase parentConnector = GetParentConnector(out connector);
			if (parentConnector == null)
			{
				return null;
			}
			Debug.Assert(connector != null);
			bool flag = false;
			foreach (IAdapter item in (IEnumerable)connector.Adapters)
			{
				Guid[] modules = item.Modules;
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i].Equals(base.ObjectGuid))
					{
						flag = true;
					}
					else if (flag && !modules[i].Equals(Guid.Empty))
					{
						return GetChildConnectorReference(parentConnector, modules[i]);
					}
				}
			}
			return null;
		}

		internal override IoModuleReferenceBase GetPrevSibling()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			IConnector connector;
			IoModuleReferenceBase parentConnector = GetParentConnector(out connector);
			if (parentConnector == null)
			{
				return null;
			}
			Debug.Assert(connector != null);
			Guid guidChild = Guid.Empty;
			foreach (IAdapter item in (IEnumerable)connector.Adapters)
			{
				Guid[] modules = item.Modules;
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i].Equals(base.ObjectGuid))
					{
						if (guidChild.Equals(Guid.Empty))
						{
							return null;
						}
						return GetChildConnectorReference(parentConnector, guidChild);
					}
					guidChild = modules[i];
				}
			}
			return null;
		}

		protected IoModuleReferenceBase GetChildConnectorReference(IoModuleReferenceBase parentRef, Guid guidChild)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected O, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(parentRef.ProjectHandle, guidChild))
			{
				return null;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(parentRef.ProjectHandle, guidChild);
			Debug.Assert(objectToRead != null);
			IObject @object = objectToRead.Object;
			IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
			if (val == null)
			{
				return null;
			}
			foreach (IConnector item in (IEnumerable)val.Connectors)
			{
				IConnector val2 = item;
				if ((int)val2.ConnectorRole == 0)
				{
					continue;
				}
				foreach (IAdapter item2 in (IEnumerable)val2.Adapters)
				{
					if (Array.IndexOf(item2.Modules, parentRef.ObjectGuid) >= 0)
					{
						return new IoModuleChildConnectorReference(guidChild, parentRef.ProjectHandle, val2.ConnectorId);
					}
				}
			}
			return null;
		}

		protected IoModuleReferenceBase GetParentConnector(out IConnector connector)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Invalid comparison between Unknown and I4
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, base.ObjectGuid);
			IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(objectToRead.ProjectHandle, objectToRead.ParentObjectGuid);
			if (objectToRead2.Object is IExplicitConnector)
			{
				connector = (IConnector)(IExplicitConnector)objectToRead2.Object;
				return new IoModuleExplicitParentConnectorReference(objectToRead2.ObjectGuid, base.ProjectHandle, connector.ConnectorId);
			}
			IObject @object = objectToRead2.Object;
			IDeviceObject val = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
			if (val == null)
			{
				connector = null;
				return null;
			}
			foreach (IConnector item in (IEnumerable)val.Connectors)
			{
				IConnector val2 = item;
				if ((int)val2.ConnectorRole == 1)
				{
					continue;
				}
				foreach (IAdapter item2 in (IEnumerable)val2.Adapters)
				{
					if (Array.IndexOf(item2.Modules, base.ObjectGuid) >= 0)
					{
						connector = val2;
						return new IoModuleParentConnectorReference(objectToRead2.ObjectGuid, base.ProjectHandle, val2.ConnectorId);
					}
				}
			}
			connector = null;
			return null;
		}
	}
}
