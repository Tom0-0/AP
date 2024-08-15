using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.PlcLogicObject;

namespace _3S.CoDeSys.DeviceObject.PLCopen
{
	internal class DeviceUtil
	{
		private IProject project;

		internal DeviceUtil(IProject project)
		{
			this.project = project;
		}

		internal T Read<T>(Guid objectGuid) where T : class
		{
			if (objectGuid != Guid.Empty)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(project.Handle, objectGuid);
				if (typeof(T).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(project.Handle, objectGuid).Object as T;
				}
			}
			return null;
		}

		public static IDeviceObject GetDevice(int projectHandle, Guid objectGuid)
		{
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, objectGuid);
			if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType))
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(projectHandle, objectGuid);
				if (objectToRead != null && typeof(IDeviceObject).IsAssignableFrom(((object)objectToRead.Object).GetType()))
				{
					IObject @object = objectToRead.Object;
					return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
				}
			}
			return null;
		}

		internal IMetaObject GetPlcLogic(IDeviceObject2 deviceObject)
		{
			IMetaObject metaObject = ((IObject)deviceObject).MetaObject;
			if (metaObject == null)
			{
				return null;
			}
			Guid[] subObjectGuids = metaObject.SubObjectGuids;
			foreach (Guid guid in subObjectGuids)
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObject.ProjectHandle, guid);
				if (objectToRead.Object is IPlcLogicObject)
				{
					return objectToRead;
				}
			}
			return null;
		}
	}
}
