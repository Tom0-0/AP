using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{3839DC74-89D6-4BAF-8367-82CDE3718BAC}")]
	public class WatchListPerspectiveInitializer : IPerspectiveInitializer
	{
		private static readonly Guid GUID_GLOBALWATCHLISTVIEWFACTORY1 = new Guid("{2EFCE11C-EFB8-4282-B233-0644A415088D}");

		public IEnumerable<IView> OpenViews(IPerspective perspective)
		{
			if (perspective != null && perspective.Name == "Online")
			{
				yield return ((IEngine)APEnvironment.Engine).Frame.OpenView(GUID_GLOBALWATCHLISTVIEWFACTORY1, (string)null);
			}
		}

		public bool ShouldRemainOpen(IPerspective perspective, IView view)
		{
			if (perspective != null && perspective.Name == "Online" && view is IWatchListView)
			{
				return true;
			}
			return false;
		}
	}
}
