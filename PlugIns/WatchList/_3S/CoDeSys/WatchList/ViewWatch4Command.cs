using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{6BBE615C-8FE4-493f-87DA-976A7461E568}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	public class ViewWatch4Command : AbstractViewCommand
	{
		private static readonly Guid GUID_FACTORY = new Guid("{201E620C-9ABC-4cd2-89BE-CD83D992890D}");

		protected override Guid FactoryGuid => GUID_FACTORY;

		protected override int Index => 4;
	}
}
