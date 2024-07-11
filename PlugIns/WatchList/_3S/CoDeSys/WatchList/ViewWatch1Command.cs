using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{EECBEE66-648B-4e9c-A738-4E5C09AB7C50}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	public class ViewWatch1Command : AbstractViewCommand
	{
		private static readonly Guid GUID_FACTORY = new Guid("{2EFCE11C-EFB8-4282-B233-0644A415088D}");

		protected override Guid FactoryGuid => GUID_FACTORY;

		protected override int Index => 1;
	}
}
