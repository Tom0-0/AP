using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Core.OnlineHelp;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{8EE117EB-A14C-48f2-AEF6-E44C5B2A6931}")]
	[AssociatedOnlineHelpTopic("codesys.chm::/_cds_cmd_add_all_forces_to_watchlist.htm")]
	[AssociatedOnlineHelpTopic("core.OnlineCommands.Online.chm::/Add_All_Forces_to_Watchlist.htm")]
	public class AddAllForcesToWatchListCommand : IStandardCommand, ICommand
	{
		private static readonly Guid GUID_ONLINECOMMANDCATEGORY = new Guid("{8DDBE3C7-2966-4ba9-A27B-7DB46265241D}");

		private static readonly string[] BATCH_COMMAND = new string[2] { "watchlist", "addallforces" };

		public Guid Category => GUID_ONLINECOMMANDCATEGORY;

		public string Name => Strings.AddAllForcesToWatchListCommand_Name;

		public string Description => Strings.AddAllForcesToWatchListCommand_Description;

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public bool Enabled
		{
			get
			{
				try
				{
					if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
					{
						return false;
					}
					if (!OnlineDeviceFeatureHelper.CheckFeatureSupport((OnlineFeatureEnum)4))
					{
						return false;
					}
					IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
					IWatchListView3 val = (IWatchListView3)(object)((activeView is IWatchListView3) ? activeView : null);
					if (val == null || val is ForceListView)
					{
						return false;
					}
					if (val.GetAllOnlineApplications() != null)
					{
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
		}

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
			if (OnlineDeviceFeatureHelper.CheckFeatureSupport((OnlineFeatureEnum)4))
			{
				if (!bContextMenu || ((IEngine)APEnvironment.Engine).Frame == null)
				{
					return false;
				}
				IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
				if (activeView != null && activeView is IWatchListView2 && !(activeView is ForceListView))
				{
					return true;
				}
			}
			return false;
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
			IView activeView = ((IEngine)APEnvironment.Engine).Frame.ActiveView;
			IWatchListView3 val = (IWatchListView3)(object)((activeView is IWatchListView3) ? activeView : null);
			if (val != null)
			{
				val.LoadAllForcedExpressions();
			}
		}
	}
}
