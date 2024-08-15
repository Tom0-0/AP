using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class IoModuleDeviceReference : IoModuleReferenceBase
	{
		public override int ConnectorId => -1;

		public override bool IsConnector => false;

		public override bool IsExplicitConnector => false;

		internal IoModuleDeviceReference(Guid guidObject, int nProjectHandle)
			: base(guidObject, nProjectHandle)
		{
		}

		internal override IoModuleReferenceBase GetParent()
		{
			return null;
		}

		internal override IoModuleReferenceBase GetFirstChild()
		{
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(base.ProjectHandle, base.ObjectGuid);
			return GetFirstConnectorOfRoleParent(objectToRead, -1);
		}

		internal override IoModuleReferenceBase GetNextSibling()
		{
			return null;
		}

		internal override IoModuleReferenceBase GetPrevSibling()
		{
			return null;
		}
	}
}
