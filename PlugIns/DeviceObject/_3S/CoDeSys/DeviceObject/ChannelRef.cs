using System;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ChannelRef : IComparable
	{
		private BitDataLocation _locationStart;

		private BitDataLocation _locationEnd;

		private uint _uiParamId;

		private int _nBitOffset;

		private int _nBitSize;

		private string _baseType;

		private IDataElement _dataElement;

		private ConnectorMap _connectorMap;

		public BitDataLocation LocationStart => _locationStart;

		public BitDataLocation LocationEnd => _locationEnd;

		public uint ParamId => _uiParamId;

		public int BitOffset => _nBitOffset;

		public string BaseType => _baseType;

		public IDataElement DataElement => _dataElement;

		public ITaskMappingInfo MappingInfo => _connectorMap.MappingInfo;

		public ConnectorMap ConnectorMap => _connectorMap;

		public ChannelRef(ConnectorMap connectorMap, long lParamId, long lBitOffset, IDataLocation datalocation, string baseType, IDataElement dataElement)
		{
			_connectorMap = connectorMap;
			_uiParamId = (uint)lParamId;
			_nBitOffset = (int)lBitOffset;
			_nBitSize = (int)dataElement.GetBitSize();
			if (datalocation.IsBitLocation && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
			{
				_nBitSize = 1;
			}
			_baseType = baseType;
			_dataElement = dataElement;
			_locationStart = new BitDataLocation(datalocation);
			_locationEnd = new BitDataLocation(datalocation, _nBitSize - 1);
		}

		public void GetIntersection(BitDataLocation locStart, int nBitSize, out int nChannelStartBit, out int nLocationStartBit, out int nNumBits)
		{
			int num = (int)(locStart.BitOffset - _locationStart.BitOffset);
			if (num < 0)
			{
				num = -num;
				nChannelStartBit = 0;
				nLocationStartBit = num;
				if (num >= nBitSize)
				{
					nNumBits = 0;
				}
				else
				{
					nNumBits = Math.Min(nBitSize - num, _nBitSize);
				}
			}
			else
			{
				nChannelStartBit = num;
				nLocationStartBit = 0;
				if (num >= _nBitSize)
				{
					nNumBits = 0;
				}
				else
				{
					nNumBits = Math.Min(_nBitSize - num, nBitSize);
				}
			}
		}

		public int CompareTo(object obj)
		{
			ChannelRef channelRef = (ChannelRef)obj;
			if (channelRef == null)
			{
				throw new ArgumentNullException("obj");
			}
			return _locationStart.CompareTo(channelRef.LocationStart);
		}
	}
}
