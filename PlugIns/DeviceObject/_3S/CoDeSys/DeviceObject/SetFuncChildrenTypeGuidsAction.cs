using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class SetFuncChildrenTypeGuidsAction : IUndoableAction
	{
		private int _nProjectHandle;

		private Guid _guidDevice;

		private Guid[] _typeGuids;

		public string Description => "";

		public SetFuncChildrenTypeGuidsAction(int nProjectHandle, Guid guidDevice, Guid[] typeGuids)
		{
			_nProjectHandle = nProjectHandle;
			_guidDevice = guidDevice;
			_typeGuids = new Guid[typeGuids.Length];
			for (int i = 0; i < typeGuids.Length; i++)
			{
				_typeGuids[i] = typeGuids[i];
			}
		}

		public object Undo()
		{
			return _guidDevice;
		}

		public object Redo()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject val = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _guidDevice);
				((IDeviceObject2)val.Object).FunctionalChildrenTypeGuids=(_typeGuids);
				((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
				val = null;
				return _guidDevice;
			}
			catch
			{
				if (val != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
				}
				throw;
			}
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
