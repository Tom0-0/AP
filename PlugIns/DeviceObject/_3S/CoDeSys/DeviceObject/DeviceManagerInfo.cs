using System;
using System.Collections;
using System.Collections.Generic;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DeviceManagerInfo : IDeviceManagerInfo
	{
		private IObject obj;

		private int projectHandle;

		private Guid objectGuid;

		public Guid ObjectGuid => objectGuid;

		public int ProjectHandle => projectHandle;

		public IObject Object => obj;

		public bool IsExplicitConnector => obj is IExplicitConnector;

		public bool IsDevice => obj is IDeviceObject;

		internal DeviceManagerInfo(IObject obj)
			: this(obj.MetaObject.ProjectHandle, obj.MetaObject.ObjectGuid)
		{
			this.obj = obj;
		}

		internal DeviceManagerInfo(int projectHandle, Guid objectGuid)
		{
			this.projectHandle = projectHandle;
			this.objectGuid = objectGuid;
		}

		public IList<IConnector> FindConnectors(int moduleType, ConnectorRole? role)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			LList<IConnector> val = new LList<IConnector>();
			if (IsExplicitConnector)
			{
				IObject obj = this.obj;
				IExplicitConnector val2 = (IExplicitConnector)(object)((obj is IExplicitConnector) ? obj : null);
				if (((IConnector)val2).ModuleType == moduleType && (!role.HasValue || (ConnectorRole?)((IConnector)val2).ConnectorRole == role))
				{
					val.Add((IConnector)(object)val2);
				}
			}
			else if (IsDevice)
			{
				IObject obj2 = this.obj;
				{
					foreach (IConnector item in (IEnumerable)((IDeviceObject)((obj2 is IDeviceObject) ? obj2 : null)).Connectors)
					{
						IConnector val3 = item;
						if (val3.ModuleType == moduleType && (!role.HasValue || (ConnectorRole?)val3.ConnectorRole == role))
						{
							val.Add(val3);
						}
					}
					return (IList<IConnector>)val;
				}
			}
			return (IList<IConnector>)val;
		}

		internal IEnumerable<IDeviceManagerInfo> GetChildInfos()
		{
			LList<Guid> childObjects = GetChildObjects();
			int projectHandle = Object.MetaObject.ProjectHandle;
			foreach (Guid item in childObjects)
			{
				IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(projectHandle, item);
				if (typeof(IDeviceObject).IsAssignableFrom(metaObjectStub.ObjectType) || typeof(IExplicitConnector).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					DeviceManagerInfo deviceManagerInfo = DeviceManager.LoadInfoIntern(projectHandle, item);
					if (deviceManagerInfo != null)
					{
						yield return (IDeviceManagerInfo)(object)deviceManagerInfo;
					}
				}
			}
		}

		private LList<Guid> GetChildObjects()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			LList<Guid> val = new LList<Guid>();
			if (IsDevice)
			{
				IObject @object = Object;
				{
					foreach (IConnector item in (IEnumerable)((IDeviceObject)((@object is IDeviceObject) ? @object : null)).Connectors)
					{
						IConnector val2 = item;
						if ((int)val2.ConnectorRole != 0)
						{
							continue;
						}
						foreach (IAdapter item2 in (IEnumerable)val2.Adapters)
						{
							IAdapter val3 = item2;
							val.AddRange((IEnumerable<Guid>)val3.Modules);
						}
					}
					return val;
				}
			}
			if (IsExplicitConnector)
			{
				IObject obj = this.obj;
				{
					foreach (IAdapter item3 in (IEnumerable)((IConnector)((obj is IExplicitConnector) ? obj : null)).Adapters)
					{
						IAdapter val4 = item3;
						val.AddRange((IEnumerable<Guid>)val4.Modules);
					}
					return val;
				}
			}
			return val;
		}
	}
}
