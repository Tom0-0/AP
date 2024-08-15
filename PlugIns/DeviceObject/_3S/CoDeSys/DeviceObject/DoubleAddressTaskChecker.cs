using System;

namespace _3S.CoDeSys.DeviceObject
{
	internal class DoubleAddressTaskChecker
	{
		private byte[,] _abyCheck;

		private int _iTasks;

		private int _iSize;

		private long _lStartOffset;

		public DoubleAddressTaskChecker(long lStartOffset, int iSize, int iTasks)
		{
			_iTasks = iTasks;
			_iSize = iSize;
			_abyCheck = new byte[_iTasks + 1, _iSize];
			_lStartOffset = lStartOffset;
		}

		public void SetBit(long lBit, int iTask)
		{
			lBit -= _lStartOffset;
			long num = lBit / 8;
			byte b = (byte)(1 << (int)(lBit % 8));
			if (num >= _iSize)
			{
				_iSize = (int)num * 2;
				_abyCheck = (byte[,])ResizeArray(_abyCheck, new int[2]
				{
					_iTasks + 1,
					_iSize
				});
			}
			if (iTask >= 0)
			{
				_abyCheck[iTask, (int)checked((nint)num)] |= b;
			}
			else
			{
				_abyCheck[_iTasks, (int)checked((nint)num)] |= b;
			}
		}

		public void SetByte(long lStartBit, int iTask)
		{
			lStartBit -= _lStartOffset;
			long num = lStartBit / 8;
			if (num >= _iSize)
			{
				_iSize = (int)num * 2;
				_abyCheck = (byte[,])ResizeArray(_abyCheck, new int[2]
				{
					_iTasks + 1,
					_iSize
				});
			}
			if (iTask >= 0)
			{
				_abyCheck[iTask, (int)checked((nint)num)] = byte.MaxValue;
			}
			else
			{
				_abyCheck[_iTasks, (int)checked((nint)num)] = byte.MaxValue;
			}
		}

		public bool CheckBit(long lBit, int iTask)
		{
			lBit -= _lStartOffset;
			long num = lBit / 8;
			byte b = (byte)(1 << (int)(lBit % 8));
			if (num < _iSize && (_abyCheck[iTask, (int)checked((nint)num)] & b) != 0)
			{
				return true;
			}
			return false;
		}

		public bool CheckByte(long lStartBit, int iTask)
		{
			lStartBit -= _lStartOffset;
			long num = lStartBit / 8;
			if (num < _iSize && _abyCheck[iTask, (int)checked((nint)num)] != 0)
			{
				return true;
			}
			return false;
		}

		public bool CheckAddressForOtherTasksBit(long lBit, int iTask)
		{
			for (int i = 0; i < _iTasks; i++)
			{
				if (i != iTask && CheckBit(lBit, i))
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckAddressForOtherTasksByte(long lStartBit, int iTask)
		{
			for (int i = 0; i < _iTasks; i++)
			{
				if (i != iTask && CheckByte(lStartBit, i))
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckAddressForTaskBit(long lBit, int iTask)
		{
			if (iTask >= 0)
			{
				if (CheckBit(lBit, iTask))
				{
					return true;
				}
			}
			else if (CheckBit(lBit, _iTasks))
			{
				return true;
			}
			return false;
		}

		public bool CheckAddressForTaskByte(long lStartBit, int iTask)
		{
			if (iTask >= 0)
			{
				if (CheckByte(lStartBit, iTask))
				{
					return true;
				}
			}
			else if (CheckByte(lStartBit, _iTasks))
			{
				return true;
			}
			return false;
		}

		private static Array ResizeArray(Array arr, int[] newSizes)
		{
			if (newSizes.Length != arr.Rank)
			{
				throw new ArgumentException("arr must have the same number of dimensions as there are elements in newSizes", "newSizes");
			}
			Array array = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
			int length = arr.GetLength(1);
			int upperBound = arr.GetUpperBound(0);
			for (int i = 0; i <= upperBound; i++)
			{
				Array.Copy(arr, i * length, array, i * newSizes[1], length);
			}
			return array;
		}
	}
}
