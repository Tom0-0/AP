#define DEBUG
using System;
using System.Diagnostics;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class IoModuleParentConnectorReference : IoModuleParentConnectorReferenceBase
	{
		public override bool IsExplicitConnector => false;

		internal IoModuleParentConnectorReference(Guid guidObject, int nProjectHandle, int nConnectorId)
			: base(guidObject, nProjectHandle, nConnectorId)
		{
		}

		protected override IConnector GetThisConnector(out IMetaObject mo)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, base.ObjectGuid);
			Debug.Assert(objectToRead != null);
			Debug.Assert(objectToRead.Object is IDeviceObject);
			IDeviceObject device = (IDeviceObject)objectToRead.Object;
			IConnector connector = GetConnector(device, ConnectorId);
			if (connector == null)
			{
				mo = null;
				return connector;
			}
			mo = objectToRead;
			return connector;
		}

		protected override bool GetParentModule(out IMetaObject moParent, out IConnector parent, out IDeviceObject device)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			IConnector thisConnector = GetThisConnector(out moParent);
			if (thisConnector == null)
			{
				moParent = null;
				parent = null;
				device = null;
				return false;
			}
			Debug.Assert(moParent != null);
			device = (IDeviceObject)moParent.Object;
			if (thisConnector.HostPath == -1)
			{
				parent = null;
				return true;
			}
			parent = GetConnector(device, thisConnector.HostPath);
			if (parent == null)
			{
				moParent = null;
				device = null;
				return false;
			}
			return true;
		}
	}
}
