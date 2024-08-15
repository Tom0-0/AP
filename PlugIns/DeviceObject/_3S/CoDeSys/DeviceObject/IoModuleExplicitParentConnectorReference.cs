using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class IoModuleExplicitParentConnectorReference : IoModuleParentConnectorReferenceBase
	{
		public override bool IsExplicitConnector => true;

		internal IoModuleExplicitParentConnectorReference(Guid guidObject, int nProjectHandle, int nConnectorId)
			: base(guidObject, nProjectHandle, nConnectorId)
		{
		}

		protected override IConnector GetThisConnector(out IMetaObject mo)
		{
			mo = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, base.ObjectGuid);
			IObject @object = mo.Object;
			IObject obj = ((@object is IExplicitConnector) ? @object : null);
			if (obj == null)
			{
				mo = null;
			}
			return (IConnector)(object)obj;
		}

		protected override bool GetParentModule(out IMetaObject moParent, out IConnector parent, out IDeviceObject device)
		{
			IMetaObject mo;
			IConnector thisConnector = GetThisConnector(out mo);
			if (thisConnector != null)
			{
				moParent = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, mo.ParentObjectGuid);
				IObject @object = moParent.Object;
				device = (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				if (device != null)
				{
					if (thisConnector.HostPath == -1)
					{
						parent = null;
						return true;
					}
					parent = GetConnector(device, thisConnector.HostPath);
					if (parent != null)
					{
						return true;
					}
				}
			}
			moParent = null;
			parent = null;
			device = null;
			return false;
		}
	}
}
