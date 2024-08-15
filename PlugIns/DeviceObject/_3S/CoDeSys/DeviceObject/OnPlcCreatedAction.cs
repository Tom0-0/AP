using System;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class OnPlcCreatedAction : IUndoableAction
	{
		private int _nProjectHandle;

		private Guid _guidDevice;

		public string Description => "Plc created";

		public OnPlcCreatedAction(int nProjectHandle, Guid guidDevice)
		{
			_nProjectHandle = nProjectHandle;
			_guidDevice = guidDevice;
		}

		public object Undo()
		{
			return _guidDevice;
		}

		public object Redo()
		{
			IMetaObject val = null;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _guidDevice);
				((IDeviceObjectBase)val.Object).OnAfterCreated();
				((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, true, (object)null);
			}
			catch
			{
				if (val != null)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, false, (object)null);
				}
			}
			return _guidDevice;
		}

		public bool Merge(IUndoableAction action)
		{
			return false;
		}
	}
}
