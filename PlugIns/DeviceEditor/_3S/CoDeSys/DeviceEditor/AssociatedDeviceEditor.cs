using System;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	public class AssociatedDeviceEditor : IEditor
	{
		private int _nProjectHandle;

		private Guid _guidObject;

		private IMetaObject _metaObject;

		public bool IsModifying
		{
			get
			{
				if (_metaObject != null)
				{
					return _metaObject.IsToModify;
				}
				return false;
			}
		}

		public int ProjectHandle => _nProjectHandle;

		public Guid ObjectGuid => _guidObject;

		public event EventHandler ReloadEvent;

		public event SaveEventHandler SaveEvent;

		public IDeviceObject GetDeviceObject(bool bToModify)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			IMetaObject val = ((!bToModify) ? GetObjectToRead() : GetObjectToModify());
			if (val == null)
			{
				return null;
			}
			return (IDeviceObject)val.Object;
		}

		public void SetObject(int nProjectHandle, Guid objectGuid)
		{
			_metaObject = null;
			_nProjectHandle = nProjectHandle;
			_guidObject = objectGuid;
		}

		public void Reload()
		{
			if (this.ReloadEvent != null)
			{
				this.ReloadEvent(this, new EventArgs());
			}
		}

		public void Save(bool bCommit)
		{
			if (this.SaveEvent != null)
			{
				this.SaveEvent(this, bCommit);
			}
			if (_metaObject == null || !_metaObject.IsToModify)
			{
				return;
			}
			IUndoManager undoManager = ((IObjectManager)APEnvironment.ObjectMgr).GetUndoManager(_metaObject.ProjectHandle);
			bool flag = false;
			try
			{
				undoManager.BeginCompoundAction(string.Empty);
				((IObjectManager)APEnvironment.ObjectMgr).SetObject(_metaObject, bCommit, (object)this);
				flag = true;
			}
			finally
			{
				undoManager.EndCompoundAction();
				if (!flag)
				{
					undoManager.Undo();
				}
			}
		}

		public IMetaObject GetObjectToRead()
		{
			try
			{
				if (_metaObject == null)
				{
					_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _guidObject);
				}
				return _metaObject;
			}
			catch
			{
				return null;
			}
		}

		public IMetaObject GetObjectToModify()
		{
			try
			{
				if (_metaObject == null || !_metaObject.IsToModify)
				{
					_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _guidObject);
				}
				return _metaObject;
			}
			catch
			{
				return null;
			}
		}
	}
}
