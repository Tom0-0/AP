using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.WatchList
{
	internal static class OnlineDeviceFeatureHelper
	{
		internal static bool CheckFeatureSupport(OnlineFeatureEnum feature)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return APEnvironment.OnlineMgr.CheckActiveApplicationFeatureSupport(feature);
		}
	}
}
