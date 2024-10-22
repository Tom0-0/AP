using System;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public class SourcePosition : ISourcePosition
	{
		private int _nProjectHandle;

		private Guid _objectGuid;

		private short _nLength;

		private long _nPosition;

		public int ProjectHandle
		{
			get
			{
				return _nProjectHandle;
			}
			set
			{
				_nProjectHandle = value;
			}
		}

		public Guid ObjectGuid
		{
			get
			{
				return _objectGuid;
			}
			set
			{
				_objectGuid = value;
			}
		}

		public long Position
		{
			get
			{
				long result = default(long);
				short num = default(short);
				PositionHelper.SplitPosition(_nPosition, out result, out num);
				return result;
			}
		}

		public short PositionOffset
		{
			get
			{
				long num = default(long);
				short result = default(short);
				PositionHelper.SplitPosition(_nPosition, out num, out result);
				return result;
			}
		}

		public short Length
		{
			get
			{
				return _nLength;
			}
			set
			{
				_nLength = value;
			}
		}

		public long PositionCombination => _nPosition;

		public SourcePosition(int nProjectHandle, Guid objectGuid, long nPosition, short sPositionOffset, short nLength)
		{
			_nProjectHandle = nProjectHandle;
			_objectGuid = objectGuid;
			_nPosition = PositionHelper.CombinePosition(nPosition, sPositionOffset);
			_nLength = nLength;
		}
	}
}
