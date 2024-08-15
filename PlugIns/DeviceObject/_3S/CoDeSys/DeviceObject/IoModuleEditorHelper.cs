using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	internal class IoModuleEditorHelper : IIoModuleEditorHelper, IIoModuleReference, IDisposable
	{
		private IoModuleReferenceBase _moduleReference;

		private IMetaObject _metaObject;

		private object _tag;

		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		public Guid ObjectGuid => _moduleReference.ObjectGuid;

		public int ProjectHandle => _moduleReference.ProjectHandle;

		public int ConnectorId => _moduleReference.ConnectorId;

		public bool IsConnector => _moduleReference.IsConnector;

		public bool IsExplicitConnector => _moduleReference.IsExplicitConnector;

		public event EventHandler Invalidated;

		internal IoModuleEditorHelper(IoModuleReferenceBase moduleReference, Guid[] path)
		{
			_moduleReference = moduleReference;
		}

		public IDeviceObject GetDeviceObject(bool bToModify)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			CheckState();
			if (_moduleReference.IsConnector)
			{
				throw new InvalidOperationException("Helper is associated with a connector");
			}
			return (IDeviceObject)GetMetaObject(bToModify).Object;
		}

		public IConnector GetConnector(bool bToModify)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			CheckState();
			if (!_moduleReference.IsConnector)
			{
				throw new InvalidOperationException("Helper is associated with a device");
			}
			IMetaObject metaObject = GetMetaObject(bToModify);
			if (_moduleReference.IsExplicitConnector)
			{
				return (IConnector)(IExplicitConnector)metaObject.Object;
			}
			foreach (IConnector item in (IEnumerable)((IDeviceObject)metaObject.Object).Connectors)
			{
				IConnector val = item;
				if (val.ConnectorId == _moduleReference.ConnectorId)
				{
					return val;
				}
			}
			throw new InvalidOperationException("Connector not found");
		}

		public void Save(bool bCommit, IEditor editor)
		{
			CheckState();
			if (_metaObject != null && _metaObject.IsToModify)
			{
				((IObjectManager)APEnvironment.ObjectMgr).SetObject(_metaObject, bCommit, (object)editor);
				_metaObject = null;
			}
		}

		public void Reload()
		{
			CheckState();
			_metaObject = null;
		}

		public IIoModuleEditorHelper CreateEditorHelper()
		{
			CheckState();
			return (IIoModuleEditorHelper)(object)new IoModuleEditorHelper(_moduleReference, null);
		}

		public void Dispose()
		{
		}

		protected void RaiseInvalidated()
		{
			if (this.Invalidated != null)
			{
				this.Invalidated(this, new EventArgs());
			}
		}

		private IMetaObject GetMetaObject(bool bToModify)
		{
			if (bToModify)
			{
				if (_metaObject == null || !_metaObject.IsToModify)
				{
					_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_moduleReference.ProjectHandle, _moduleReference.ObjectGuid);
				}
			}
			else if (_metaObject == null)
			{
				_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_moduleReference.ProjectHandle, _moduleReference.ObjectGuid);
			}
			return _metaObject;
		}

		private void CheckState()
		{
			if (_moduleReference == null)
			{
				throw new InvalidOperationException("Object has been invalidated");
			}
		}
	}
}
