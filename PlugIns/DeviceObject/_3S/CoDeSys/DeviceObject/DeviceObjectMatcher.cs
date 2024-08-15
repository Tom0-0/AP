using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{A38A129C-0708-4D74-A465-349FBFF90845}")]
	public class DeviceObjectMatcher : IObjectMatcher
	{
		private const int GUID_MATCH = 8;

		private const int INDEX_MATCH = 4;

		private const int NAME_MATCH = 2;

		private const int ID_MATCH = 1;

		private const int NO_MATCH = 0;

		public TObjectMatchCandidate FindMatchFor<TObjectMatchCandidate>(IObjectMatchCandidate referenceObject, IObjectMatchCandidateList<TObjectMatchCandidate> candidates) where TObjectMatchCandidate : IObjectMatchCandidate
		{
			int num = 0;
			TObjectMatchCandidate result = default(TObjectMatchCandidate);
			if (typeof(IDeviceObjectBase).IsAssignableFrom(referenceObject.ObjectType))
			{
				foreach (TObjectMatchCandidate item in (IEnumerable<TObjectMatchCandidate>)candidates)
				{
					if (typeof(IDeviceObjectBase).IsAssignableFrom(((IObjectMatchCandidate)item).ObjectType))
					{
						int num2 = MatchDevices(referenceObject, (IObjectMatchCandidate)(object)item);
						if (num2 == 8)
						{
							return item;
						}
						if (num < num2)
						{
							num = num2;
							result = item;
						}
					}
				}
				return result;
			}
			return result;
		}

		public bool IsMatchAllowed(IObjectMatchCandidate first, IObjectMatchCandidate second)
		{
			if (typeof(IDeviceObjectBase).IsAssignableFrom(first.ObjectType) && typeof(IDeviceObjectBase).IsAssignableFrom(second.ObjectType) && MatchDevices(first, second) == 0)
			{
				return false;
			}
			return true;
		}

		internal int MatchDevices(IObjectMatchCandidate first, IObjectMatchCandidate second)
		{
			if (first.ObjectGuid == second.ObjectGuid)
			{
				return 8;
			}
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(first.ProjectHandle, first.ObjectGuid);
			IObject @object = objectToRead.Object;
			IDeviceObject5 val = (IDeviceObject5)(object)((@object is IDeviceObject5) ? @object : null);
			IMetaObject objectToRead2 = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(second.ProjectHandle, second.ObjectGuid);
			IObject object2 = objectToRead2.Object;
			IDeviceObject5 val2 = (IDeviceObject5)(object)((object2 is IDeviceObject5) ? object2 : null);
			if (objectToRead.Index == objectToRead2.Index && val.DeviceIdentificationNoSimulation.Type == val2.DeviceIdentificationNoSimulation.Type && val.DeviceIdentificationNoSimulation.Id == val2.DeviceIdentificationNoSimulation.Id)
			{
				int num = 1;
				if (first.MetaObjectStub.Name.Equals(second.MetaObjectStub.Name, StringComparison.OrdinalIgnoreCase))
				{
					num += 2;
				}
				if (first.MetaObjectStub.Index == second.MetaObjectStub.Index)
				{
					num += 4;
				}
				return num;
			}
			return 0;
		}
	}
}
