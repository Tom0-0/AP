namespace _3S.CoDeSys.DeviceObject
{
	public class FixedTaskUpdate
	{
		private ConnectorMap _connector;

		private ChannelMap _channel;

		private string _stAddress;

		private int _nBitOffset;

		private string _stTask;

		private IDataElement _dataElement;

		public ConnectorMap ConnectorMap => _connector;

		public ChannelMap ChannelMap => _channel;

		public string Address => _stAddress;

		public int BitOffset => _nBitOffset;

		public string Task => _stTask;

		public IDataElement DataElement => _dataElement;

		public FixedTaskUpdate(ConnectorMap connector, ChannelMap channel, string stAddress, int nBitOffset, string stTask, IDataElement dataElement)
		{
			_connector = connector;
			_channel = channel;
			_stAddress = stAddress;
			_nBitOffset = nBitOffset;
			_stTask = stTask;
			_dataElement = dataElement;
		}
	}
}
