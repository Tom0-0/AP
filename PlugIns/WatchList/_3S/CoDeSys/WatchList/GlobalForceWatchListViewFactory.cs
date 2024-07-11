using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.WatchList
{
	public abstract class GlobalForceWatchListViewFactory : IViewFactory
	{
		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.WatchList.Resources.ForceListSmall.ico");

		public Icon LargeIcon => null;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceListViewCommand_Name");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ForceListViewCommand_Description");

		public Guid TypeGuid => ((TypeGuidAttribute)GetType().GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false)[0]).Guid;

		public IView Create()
		{
			return (IView)(object)new ForceListView();
		}
	}
}
