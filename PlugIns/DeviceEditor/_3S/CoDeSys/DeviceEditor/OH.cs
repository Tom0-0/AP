#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal static class OH
	{
		public static IEnumerable<IMetaObjectStub> EnumChildrenTS<T>(int nProj, Guid gdObj)
		{
			return EnumChildrenTS<T>(GetStub(nProj, gdObj));
		}

		public static IEnumerable<IMetaObjectStub> EnumChildrenTS<T>(IMetaObjectStub mos)
		{
			return Fun.Filter<IMetaObjectStub>((Fun1<IMetaObjectStub, bool>)HasObjType<T>, EnumChildren(mos));
		}

		public static IEnumerable<IMetaObjectStub> EnumChildren(IMetaObjectStub mos)
		{
			Debug.Assert(mos != null);
			return EnumFromGuids(mos.ProjectHandle, mos.SubObjectGuids);
		}

		public static IEnumerable<IMetaObjectStub> EnumFromGuids(int nProj, IEnumerable<Guid> gds)
		{
			Debug.Assert(nProj >= 0 && gds != null);
			return Fun.Map<Guid, IMetaObjectStub>(Fun.Bind1st<int, Guid, IMetaObjectStub>((Fun2<int, Guid, IMetaObjectStub>)GetStub, nProj), gds);
		}

		public static bool HasObjType<T>(IMetaObjectStub mos)
		{
			Debug.Assert(mos != null);
			return IsAssignableFrom<T>(mos.ObjectType);
		}

		public static bool HasObjType<T>(int nProj, Guid gdObj)
		{
			return HasObjType<T>(GetStub(nProj, gdObj));
		}

		public static T Resolve<T>(IMetaObjectStub mos)
		{
			return (T)(object)((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(mos.ProjectHandle, mos.ObjectGuid).Object;
		}

		public static IMetaObjectStub GetStub(int nProj, Guid gd)
		{
			return ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(nProj, gd);
		}

		public static bool IsAssignableFrom<T>(Type t)
		{
			return typeof(T).IsAssignableFrom(t);
		}
	}
}
