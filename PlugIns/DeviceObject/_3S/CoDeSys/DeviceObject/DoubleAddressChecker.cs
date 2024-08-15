using System;

namespace _3S.CoDeSys.DeviceObject
{
    internal class DoubleAddressChecker
    {
        private byte[] _abyCheck;

        private int _iSize;

        private long _lStartOffset;

        public DoubleAddressChecker(long lStartOffset, int iSize)
        {
            _iSize = iSize;
            _abyCheck = new byte[_iSize];
            _lStartOffset = lStartOffset;
        }

        public void SetBit(long lBit)
        {
            lBit -= _lStartOffset;
            long num = lBit / 8;
            byte b = (byte)(1 << (int)(lBit % 8));
            if (num >= _iSize)
            {
                _iSize = (int)num * 2;
                Array.Resize(ref _abyCheck, _iSize);
            }
            _abyCheck[num] |= b;
        }

        public void SetByte(long lStartBit)
        {
            lStartBit -= _lStartOffset;
            long num = lStartBit / 8;
            if (num >= _iSize)
            {
                _iSize = (int)num * 2;
                Array.Resize(ref _abyCheck, _iSize);
            }
            _abyCheck[num] = byte.MaxValue;
        }

        public bool CheckBit(long lBit)
        {
            lBit -= _lStartOffset;
            long num = lBit / 8;
            byte b = (byte)(1 << (int)(lBit % 8));
            if (num < _abyCheck.Length && (_abyCheck[num] & b) != 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckByte(long lStartBit)
        {
            lStartBit -= _lStartOffset;
            long num = lStartBit / 8;
            if (num < _abyCheck.Length && _abyCheck[num] != 0)
            {
                return true;
            }
            return false;
        }
    }
}
