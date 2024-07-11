using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{7CAE844F-6685-43cb-9575-A7206BE83F31}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	public class ViewWatch2Command : AbstractViewCommand
	{
		private static readonly Guid GUID_FACTORY = new Guid("{88F734F5-B42A-4082-8FB4-178171CEA8F6}");

		protected override Guid FactoryGuid => GUID_FACTORY;

		protected override int Index => 2;
	}
}
