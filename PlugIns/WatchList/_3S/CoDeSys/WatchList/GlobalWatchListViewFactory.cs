using System;
using System.Drawing;
using System.Windows.Forms;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.WatchList
{
	public abstract class GlobalWatchListViewFactory : IViewFactory
	{
		private static Sentry _sentry;

		public Icon LargeIcon => null;

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WatchListFactory_Description");

		public string Name
		{
			get
			{
				string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WatchListFactory_Name");
				return $"{@string} {Index}";
			}
		}

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.WatchList.Resources.WatchListSmall.ico");

		public Guid TypeGuid => ((TypeGuidAttribute)GetType().GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		protected abstract int Index { get; }

		public GlobalWatchListViewFactory()
		{
			if (_sentry == null)
			{
				_sentry = new Sentry();
			}
		}

		public IView Create()
		{
			WatchListView watchListView = new WatchListView(bShowWatchpointColumn: true, bIsForceListView: false);
			watchListView.SetAppearance(SmallIcon, LargeIcon, Name, (DockingPosition)16, (DockingPosition)31, BorderStyle.FixedSingle);
			watchListView.PersistenceGuid = TypeGuidAttribute.FromObject((object)this).Guid;
			watchListView.PersistenceGuid2 = watchListView.PersistenceGuid;
			watchListView.InstancePath = string.Empty;
			watchListView.ReadOnly = false;
			watchListView.Refill();
			return (IView)(object)watchListView;
		}
	}
}
