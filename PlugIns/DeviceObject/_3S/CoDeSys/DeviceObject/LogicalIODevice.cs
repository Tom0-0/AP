using System;
using System.Collections;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{5861CD6F-95FF-4614-B597-9845CC9988E7}")]
	[StorageVersion("3.4.1.0")]
	internal class LogicalIODevice : DeviceObject
	{
		public static Guid TypeGuid => ((TypeGuidAttribute)typeof(LogicalIODevice).GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		public override Guid[] ObjectsToUpdate
		{
			get
			{
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				ArrayList arrayList = new ArrayList();
				if (DeviceObjectHelper.IsUpdateObjectSuppressed)
				{
					return new Guid[0];
				}
				IMetaObject metaObject = base.MetaObject;
				if (metaObject != null && metaObject.ParentObjectGuid != Guid.Empty)
				{
					IObject @object = metaObject.Object;
					ILogicalDevice val = (ILogicalDevice)(object)((@object is ILogicalDevice) ? @object : null);
					if (val != null && val.IsLogical)
					{
						foreach (IMappedDevice item in (IEnumerable)val.MappedDevices)
						{
							Guid getMappedDevice = item.GetMappedDevice;
							if (getMappedDevice != Guid.Empty)
							{
								arrayList.Add(getMappedDevice);
							}
						}
					}
					if (((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaObject.ProjectHandle, metaObject.ObjectGuid))
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObject.ProjectHandle, metaObject.ObjectGuid);
						int num = default(int);
						do
						{
							metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObjectStub.ProjectHandle, metaObjectStub.ParentObjectGuid);
							if (!typeof(LogicalIOObject).IsAssignableFrom(metaObjectStub.ObjectType))
							{
								continue;
							}
							if (!((IObjectManager)APEnvironment.ObjectMgr).IsLoadProjectFinished(metaObject.ProjectHandle, out num))
							{
								DeviceObjectHelper.AddObjectsToUpdate(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
								foreach (Guid item2 in arrayList)
								{
									DeviceObjectHelper.AddObjectsToUpdate(metaObjectStub.ProjectHandle, item2);
								}
								arrayList.Clear();
							}
							else
							{
								arrayList.Add(metaObjectStub.ObjectGuid);
							}
						}
						while (metaObjectStub.ParentObjectGuid != Guid.Empty);
					}
				}
				Guid[] array = new Guid[arrayList.Count];
				arrayList.CopyTo(array);
				return array;
			}
		}

		public LogicalIODevice()
		{
		}

		public LogicalIODevice(DeviceIdentification id)
		{
			IRepositorySource val = default(IRepositorySource);
			IDeviceDescription device = ((IDeviceRepository)APEnvironment.DeviceRepository).GetDevice((IDeviceIdentification)(object)id, out val);
			if (device != null && APEnvironment.DeviceRepository.ExternalConverterMissing(device))
			{
				try
				{
					APEnvironment.DeviceRepository.ReimportDeviceDescription(device);
				}
				catch
				{
				}
			}
			InitDeviceObject(bCreateBitChannels: false, (IDeviceIdentification)(object)id);
			_logicalDevices.Add(new LogicalMappedDevice());
		}

		private LogicalIODevice(LogicalIODevice hdo)
			: base(hdo)
		{
		}

		public override object Clone()
		{
			LogicalIODevice logicalIODevice = new LogicalIODevice(this);
			((GenericObject)logicalIODevice).AfterClone();
			return logicalIODevice;
		}

		public override bool AcceptsParentObject(IObject parentObject)
		{
			if (parentObject == null)
			{
				return false;
			}
			return parentObject is ILogicalObject;
		}

		public override bool AcceptsChildObject(Type childObjectType)
		{
			return false;
		}

		public override int CheckRelationships(IObject parentObject, IObject[] childObjects)
		{
			return 0;
		}

		public override IMetaObject GetApplication()
		{
			IMetaObject metaObject = base.MetaObject;
			if (metaObject != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaObject.ProjectHandle, metaObject.ParentObjectGuid))
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObject.ProjectHandle, metaObject.ParentObjectGuid);
				while (metaObjectStub.ParentObjectGuid != Guid.Empty && !typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaObject.ProjectHandle, metaObjectStub.ParentObjectGuid);
				}
				return ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(metaObjectStub.ProjectHandle, metaObjectStub.ObjectGuid);
			}
			return null;
		}
	}
}
