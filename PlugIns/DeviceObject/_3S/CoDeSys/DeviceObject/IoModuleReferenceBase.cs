using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal abstract class IoModuleReferenceBase : IIoModuleReference
	{
		private Guid _guidObject;

		private int _nProjectHandle;

		public abstract int ConnectorId { get; }

		public abstract bool IsConnector { get; }

		public abstract bool IsExplicitConnector { get; }

		public Guid ObjectGuid => _guidObject;

		public int ProjectHandle => _nProjectHandle;

		internal abstract IoModuleReferenceBase GetParent();

		internal abstract IoModuleReferenceBase GetFirstChild();

		internal abstract IoModuleReferenceBase GetNextSibling();

		internal abstract IoModuleReferenceBase GetPrevSibling();

		internal IoModuleReferenceBase(Guid guidObject, int nProjectHandle)
		{
			_guidObject = guidObject;
			_nProjectHandle = nProjectHandle;
		}

		public IIoModuleEditorHelper CreateEditorHelper()
		{
			return (IIoModuleEditorHelper)(object)new IoModuleEditorHelper(this, null);
		}

		protected IoModuleReferenceBase GetFirstConnectorOfRoleParent(IMetaObject moDevice, int nHostPath)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			foreach (IConnector item in (IEnumerable)((IDeviceObject)moDevice.Object).Connectors)
			{
				IConnector val = item;
				if ((int)val.ConnectorRole == 0 && val.HostPath == nHostPath)
				{
					return CreateParentConnectorReference(moDevice, val);
				}
			}
			return null;
		}

		protected IoModuleReferenceBase CreateParentConnectorReference(IMetaObject moParent, IConnector connector)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (connector.IsExplicit)
			{
				Guid[] subObjectGuids = moParent.SubObjectGuids;
				foreach (Guid guid in subObjectGuids)
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(ProjectHandle, guid);
					if (objectToRead.Object is IExplicitConnector && ((IConnector)(IExplicitConnector)objectToRead.Object).ConnectorId == connector.ConnectorId)
					{
						return new IoModuleExplicitParentConnectorReference(guid, ProjectHandle, connector.ConnectorId);
					}
				}
				return null;
			}
			return new IoModuleParentConnectorReference(moParent.ObjectGuid, ProjectHandle, connector.ConnectorId);
		}
	}
}
