using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DeviceParameterSetProvider : IParameterSetProvider
	{
		private IDeviceEditorFrame _deviceEditor;

		private bool _bLocalizationActive;

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

		public DeviceParameterSetProvider(IDeviceEditorFrame deviceEditor)
		{
			_deviceEditor = deviceEditor;
		}

		public IIoProvider GetIoProvider(bool bToModify)
		{
			if (_deviceEditor != null)
			{
				IDeviceObject deviceObject = _deviceEditor.GetDeviceObject(bToModify);
				if (deviceObject != null)
				{
					return (IIoProvider)(object)((deviceObject is IIoProvider) ? deviceObject : null);
				}
			}
			return null;
		}

		public IParameterSet GetParameterSet(bool bToModify)
		{
			if (_deviceEditor != null)
			{
				IDeviceObject deviceObject = _deviceEditor.GetDeviceObject(bToModify);
				if (deviceObject != null)
				{
					return deviceObject.DeviceParameterSet;
				}
			}
			return null;
		}

		public IDeviceObject GetHost()
		{
			return _deviceEditor.GetDeviceObject(false);
		}

		public IDeviceObject GetDevice()
		{
			return _deviceEditor.GetDeviceObject(false);
		}
	}
}
