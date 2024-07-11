#define DEBUG
using System.Diagnostics;
using _3S.CoDeSys.Breakpoints;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.WatchList
{
	public class WatchPointItem
	{
		private IBP4 _wp;

		public IBP4 Breakpoint => _wp;

		public WatchPointItem(IBP4 bp)
		{
			_wp = bp;
		}

		public override bool Equals(object obj)
		{
			if (obj is WatchPointItem)
			{
				IBP4 wp = (obj as WatchPointItem)._wp;
				if (wp == null && _wp == null)
				{
					return true;
				}
				if (wp == null || _wp == null)
				{
					return false;
				}
				if (((IBP)wp).ApplicationGuid != ((IBP)_wp).ApplicationGuid)
				{
					return false;
				}
				if (((IBP)wp).ProjectHandle != ((IBP)_wp).ProjectHandle)
				{
					return false;
				}
				if (((IBP)wp).ObjectGuid != ((IBP)_wp).ObjectGuid)
				{
					return false;
				}
				if (((IBP)wp).Position != ((IBP)_wp).Position)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((IBP)_wp).ApplicationGuid.GetHashCode() ^ ((IBP)_wp).ProjectHandle.GetHashCode() ^ ((IBP)_wp).ObjectGuid.GetHashCode() ^ ((IBP)_wp).Position.GetHashCode();
		}

		public override string ToString()
		{
			if (_wp == null)
			{
				return Strings.EP_CyclicMonitoring;
			}
			return GetLocationText(_wp);
		}

		internal static string GetLocationText(IBP4 bp)
		{
			Debug.Assert(bp != null);
			try
			{
				IMetaObject objectToRead = APEnvironment.ObjectMgr.GetObjectToRead(((IBP)bp).ProjectHandle, ((IBP)bp).ObjectGuid);
				Debug.Assert(objectToRead != null);
				string text = "???";
				long num = default(long);
				int num2 = default(int);
				return string.Format(arg1: (!((IBPManager)APEnvironment.BPMgr).GetImplicitReturnPosition(((IBP)bp).ProjectHandle, ((IBP)bp).ObjectGuid, out num, out num2) || num != ((IBP)bp).Position) ? objectToRead.Object.GetPositionText(((IBP)bp).Position) : "RETURN", format: "{0} - {1}", arg0: objectToRead.Name);
			}
			catch
			{
			}
			return string.Empty;
		}
	}
}
