using System;
using System.Collections;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor.SimpleMappingEditor
{
	internal class DeviceParameterSetProvider : IParameterSetProvider
	{
		private int _nProjectHandle;

		private Guid _objectGuid;

		private int _nConnectorID;

		private IMetaObject _metaObject;

		private SimpleMappingTreeTableModel _model;

		private bool _bLocalizationActive;

		private IParameterSet _parameterSet;

		private bool _bToModify;

		private IDeviceObject _device;

		public bool LocalizationActive
		{
			get
			{
				return _bLocalizationActive;
			}
			set
			{
				_bLocalizationActive = value;
			}
		}

		internal IMetaObject MetaObject => _metaObject;

		public DeviceParameterSetProvider(SimpleMappingTreeTableModel model, int nProjectHandle, Guid objectGuid, int nConnectorID)
		{
			_model = model;
			_nProjectHandle = nProjectHandle;
			_objectGuid = objectGuid;
			_nConnectorID = nConnectorID;
		}

		internal void StoreObject()
		{
			if (_metaObject != null && _metaObject.IsToModify)
			{
				bool isStoring = _model.IsStoring;
				try
				{
					_model.IsStoring = true;
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(_metaObject, true, (object)null);
				}
				finally
				{
					_model.IsStoring = isStoring;
				}
			}
			_bToModify = false;
		}

		internal IMetaObject GetMetaObject(bool bToModify)
		{
			if (bToModify)
			{
				if (_metaObject == null || !_metaObject.IsToModify)
				{
					try
					{
						_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(_nProjectHandle, _objectGuid);
					}
					catch
					{
						_metaObject = null;
					}
				}
			}
			else if (_metaObject == null)
			{
				_metaObject = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, _objectGuid);
			}
			return _metaObject;
		}

		internal IDeviceObject GetDevice(bool bToModify)
		{
			if (GetMetaObject(bToModify) != null)
			{
				IObject @object = _metaObject.Object;
				return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
			}
			return null;
		}

		public IIoProvider GetIoProvider(bool bToModify)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			IDeviceObject device = GetDevice(bToModify);
			if (device != null)
			{
				if (_nConnectorID == -1)
				{
					return (IIoProvider)(object)((device is IIoProvider) ? device : null);
				}
				foreach (IConnector item in (IEnumerable)device.Connectors)
				{
					IConnector val = item;
					if (val.ConnectorId == _nConnectorID)
					{
						return (IIoProvider)(object)((val is IIoProvider) ? val : null);
					}
				}
			}
			return null;
		}

		public IParameterSet GetParameterSet(bool bToModify)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			if (_parameterSet == null || _bToModify != bToModify)
			{
				_bToModify = bToModify;
				IDeviceObject device = GetDevice(bToModify);
				if (device != null)
				{
					if (_nConnectorID == -1)
					{
						_parameterSet = device.DeviceParameterSet;
					}
					foreach (IConnector item in (IEnumerable)device.Connectors)
					{
						IConnector val = item;
						if (val.ConnectorId == _nConnectorID)
						{
							_parameterSet = val.HostParameterSet;
							break;
						}
					}
				}
			}
			return _parameterSet;
		}

		public IDeviceObject GetHost()
		{
			if (!((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(_nProjectHandle, _objectGuid))
			{
				return null;
			}
			IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, _objectGuid);
			while (metaObjectStub.ParentObjectGuid != Guid.Empty)
			{
				metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(_nProjectHandle, metaObjectStub.ParentObjectGuid);
			}
			IObject @object = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, metaObjectStub.ObjectGuid).Object;
			return (IDeviceObject)(object)((@object is IDeviceObject) ? @object : null);
		}

		public IDeviceObject GetDevice()
		{
			if (_device == null)
			{
				_device = GetDevice(bToModify: false);
			}
			return _device;
		}
	}
}
