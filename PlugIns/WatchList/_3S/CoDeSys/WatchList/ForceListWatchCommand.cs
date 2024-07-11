using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{D2CC2AD7-FC8D-49a4-AEED-C8CF9F5A8CCF}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_view_watch_all_forces.htm")]
	[AssociatedOnlineHelpTopic("core.WatchList.View.chm::/Watch.htm")]
	public class ForceListWatchCommand : IStandardCommand, ICommand
	{
		private static readonly Guid GUID_VIEWCOMMANDCATEGORY = new Guid("{7550CC92-8A35-4635-A6A8-7B1882215928}");

		private static readonly Guid GUID_FACTORY = new Guid("{6DAF8F8C-0A99-4f30-B4BA-67D3819A8AD2}");

		private static readonly string[] BATCH_COMMAND = new string[2] { "forcelist", "watchallforces" };

		public Guid Category => GUID_VIEWCOMMANDCATEGORY;

		public string Name => Strings.ForceListViewCommand_Name;

		public string Description => Strings.ForceListViewCommand_Description;

		public string ToolTipText => Name;

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.WatchList.Resources.ForceListSmall.ico");

		public Icon LargeIcon => SmallIcon;

		public bool Enabled => true;

		public string[] BatchCommand => BATCH_COMMAND;

		public string[] CreateBatchArguments()
		{
			return new string[0];
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			return !bContextMenu;
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length != 0)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 0);
			}
			if (((IEngine)APEnvironment.Engine).Frame == null)
			{
				throw new BatchInteractiveException(BatchCommand);
			}
			IView val = ((IEngine)APEnvironment.Engine).Frame.OpenView(GUID_FACTORY, (string)null);
			if (val != null)
			{
				((IEngine)APEnvironment.Engine).Frame.ActiveView=(val);
			}
		}
	}
}
