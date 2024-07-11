using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{325B3DCF-24EE-459c-AC41-F8BB1A158D23}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch.htm")]
	public class ViewWatch3Command : AbstractViewCommand
	{
		private static readonly Guid GUID_FACTORY = new Guid("{E3EFC840-A4EF-4f85-92DF-581AF2875E63}");

		protected override Guid FactoryGuid => GUID_FACTORY;

		protected override int Index => 3;
	}
}
