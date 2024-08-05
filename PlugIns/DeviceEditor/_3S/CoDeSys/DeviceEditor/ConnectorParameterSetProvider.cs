using _3S.CoDeSys.Core.Views;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class ConnectorParameterSetProvider : IParameterSetProvider
	{
		private IConnectorEditorFrame _connectorEditor;

		private int _nConnectorId;

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

		public ConnectorParameterSetProvider(IConnectorEditorFrame connectorEditor, int nConnectorId)
		{
			_connectorEditor = connectorEditor;
			_nConnectorId = nConnectorId;
		}

		public IIoProvider GetIoProvider(bool bToModify)
		{
			IConnector connector = _connectorEditor.GetConnector(_nConnectorId, bToModify);
			if (connector == null)
			{
				return null;
			}
			return (IIoProvider)(object)((connector is IIoProvider) ? connector : null);
		}

		public IParameterSet GetParameterSet(bool bToModify)
		{
			IConnector connector = _connectorEditor.GetConnector(_nConnectorId, bToModify);
			if (connector == null)
			{
				return null;
			}
			return connector.HostParameterSet;
		}

		public IDeviceObject GetHost()
		{
			if (((IEditorView)_connectorEditor).Editor is DeviceEditor)
			{
				return (((IEditorView)_connectorEditor).Editor as DeviceEditor).GetHost(bToModify: false);
			}
			return null;
		}

		public IDeviceObject GetDevice()
		{
			return _connectorEditor.GetAssociatedDeviceObject(false);
		}
	}
}
