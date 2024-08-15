using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class ConstraintHelper
	{
		public bool Match(IDeviceDescription device, out bool bUseDefaultFilter, bool bCheckParent, bool bUpdate)
		{
			bUseDefaultFilter = true;
			IProject primaryProject = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject;
			if (primaryProject == null)
			{
				return false;
			}
			ISVNode[] selectedSVNodes = primaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length == 0)
			{
				return false;
			}
			if (selectedSVNodes.Length > 1)
			{
				return false;
			}
			int projectHandle = selectedSVNodes[0].ProjectHandle;
			Guid objectGuid = selectedSVNodes[0].ObjectGuid;
			Guid deviceGuid = objectGuid;
			if (bCheckParent)
			{
				if (selectedSVNodes[0].Parent == null)
				{
					return false;
				}
				objectGuid = selectedSVNodes[0].Parent.ObjectGuid;
			}
			if (!bUpdate)
			{
				deviceGuid = objectGuid;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, objectGuid);
			if (typeof(DeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				foreach (Connector item in (IEnumerable)(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, objectGuid).Object as DeviceObject).Connectors)
				{
					if (!item.CheckConstraints(device.DeviceIdentification, bRecursion: false, deviceGuid, bCheck: true))
					{
						bUseDefaultFilter = false;
						return false;
					}
				}
			}
			if (typeof(ExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType) && !(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, objectGuid).Object as ExplicitConnector).CheckConstraints(device.DeviceIdentification, bRecursion: false, deviceGuid, bCheck: true))
			{
				bUseDefaultFilter = false;
				return false;
			}
			if (typeof(SlotDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				foreach (Connector item2 in (IEnumerable)(((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, objectGuid).Object as SlotDeviceObject).Connectors)
				{
					if (!item2.CheckConstraints(device.DeviceIdentification, bRecursion: false, deviceGuid, bCheck: true))
					{
						bUseDefaultFilter = false;
						return false;
					}
				}
			}
			return false;
		}
	}
}
