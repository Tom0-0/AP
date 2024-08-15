using System;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class BitDataLocation
	{
		private ushort _usArea;

		private long _lBitOffset;

		public ushort Area => _usArea;

		public long BitOffset => _lBitOffset;

		public BitDataLocation(IDataLocation locBase)
			: this(locBase, 0)
		{
		}

		public BitDataLocation(IDataLocation locBase, int nAdditionalOffset)
		{
			_usArea = locBase.Area;
			_lBitOffset = locBase.Offset * 8;
			if (locBase.IsBitLocation)
			{
				_lBitOffset += locBase.BitNr;
			}
			_lBitOffset += nAdditionalOffset;
		}

		public int CompareTo(BitDataLocation locIn)
		{
			if (_usArea != locIn.Area)
			{
				throw new ArgumentException("Invalid comparison: areas do not match.", "locIn");
			}
			return _lBitOffset.CompareTo(locIn.BitOffset);
		}
	}
}
