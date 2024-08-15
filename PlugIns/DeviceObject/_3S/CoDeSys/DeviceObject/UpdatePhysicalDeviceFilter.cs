using System;
using System.Collections;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{22291C00-E79D-4962-AB17-74925E4C1EEF}")]
	public class UpdatePhysicalDeviceFilter : IDeviceCatalogueFilter2, IDeviceCatalogueFilter
	{
		public static Guid TypeGuid => ((TypeGuidAttribute)typeof(UpdatePhysicalDeviceFilter).GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		public bool Match(IDeviceDescription device)
		{
			bool bUseDefaultFilter;
			return Match(device, out bUseDefaultFilter);
		}

		public bool Match(IDeviceDescription device, out bool bUseDefaultFilter)
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			bUseDefaultFilter = false;
			if (!(device is IDeviceDescription4))
			{
				bUseDefaultFilter = true;
				return false;
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
			int projectHandle = selectedSVNodes[0].ProjectHandle;
			Guid objectGuid = selectedSVNodes[0].ObjectGuid;
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, objectGuid);
			if (typeof(ILogicalDevice).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, objectGuid);
				if (objectToRead != null)
				{
					IObject @object = objectToRead.Object;
					ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
					if (val != null && val.IsPhysical)
					{
						foreach (IMappedDevice item in (IEnumerable)val.MappedDevices)
						{
							Guid getMappedDevice = item.GetMappedDevice;
							if (!(getMappedDevice != Guid.Empty))
							{
								continue;
							}
							IObject object2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, getMappedDevice).Object;
							IDeviceObject5 val2 = (IDeviceObject5)(object)((object2 is IDeviceObject5) ? object2 : null);
							if (val2 == null)
							{
								continue;
							}
							IDeviceIdentification deviceIdentificationNoSimulation = val2.DeviceIdentificationNoSimulation;
							IDeviceIdentification[] matchingLogicalDevices = ((IDeviceDescription4)((device is IDeviceDescription4) ? device : null)).MatchingLogicalDevices;
							if (matchingLogicalDevices.Length == 0)
							{
								return false;
							}
							IDeviceIdentification[] array = matchingLogicalDevices;
							for (int i = 0; i < array.Length; i++)
							{
								if (LogicalMappedDevice.CheckMatching(array[i], deviceIdentificationNoSimulation))
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
	}
}
