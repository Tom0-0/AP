using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	public class LogicalDeviceFilter : IDeviceCatalogueFilter
	{
		public bool Match(IDeviceDescription device)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			foreach (IConnector item in (IEnumerable)device.Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole != 0)
				{
					continue;
				}
				string text = val.Interface.ToLowerInvariant();
				bool flag = text.Equals("common.logicalio");
				bool flag2 = text.Equals("common.logicalexchange");
				if (flag2)
				{
					return true;
				}
				if (!(flag || flag2))
				{
					continue;
				}
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
				ISVNode val2 = selectedSVNodes[0];
				while (typeof(IFolderObject).IsAssignableFrom(val2.ObjectType) && val2 != null)
				{
					val2 = val2.Parent;
				}
				if (val2 == null)
				{
					continue;
				}
				int projectHandle = val2.ProjectHandle;
				Guid objectGuid = val2.ObjectGuid;
				for (IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, objectGuid); metaObjectStub != null; metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, metaObjectStub.ParentObjectGuid))
				{
					if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, metaObjectStub.ObjectGuid);
						if (objectToRead != null && objectToRead.Object is DeviceObject)
						{
							string[] supportedLogicalBusSystems = (objectToRead.Object as DeviceObject).SupportedLogicalBusSystems;
							foreach (string text2 in supportedLogicalBusSystems)
							{
								if (device.DeviceIdentification.Id.ToLowerInvariant()
									.Contains(text2.ToLowerInvariant()))
								{
									return true;
								}
							}
						}
					}
					if (!(metaObjectStub.ParentObjectGuid != Guid.Empty))
					{
						break;
					}
				}
			}
			return false;
		}
	}
}
