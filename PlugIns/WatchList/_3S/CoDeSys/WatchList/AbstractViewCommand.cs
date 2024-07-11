using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Views;

namespace _3S.CoDeSys.WatchList
{
	public abstract class AbstractViewCommand : IStandardCommand, ICommand
	{
		private static readonly Guid GUID_VIEWCOMMANDCATEGORY = new Guid("{7550CC92-8A35-4635-A6A8-7B1882215928}");

		public string ToolTipText => Name;

		public bool Enabled => true;

		public Guid Category => GUID_VIEWCOMMANDCATEGORY;

		public Icon SmallIcon => ((IEngine)APEnvironment.Engine).ResourceManager.GetIcon(GetType(), "_3S.CoDeSys.WatchList.Resources.WatchListSmall.ico");

		public Icon LargeIcon => SmallIcon;

		public string Name
		{
			get
			{
				string @string = ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WatchListFactory_Name");
				return $"{@string} {Index}";
			}
		}

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "WatchListFactory_Description");

		public string[] BatchCommand
		{
			get
			{
				string text = $"watch{Index}";
				return new string[2] { "view", text };
			}
		}

		protected abstract Guid FactoryGuid { get; }

		protected abstract int Index { get; }

		public string[] CreateBatchArguments()
		{
			return new string[0];
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
			IView val = ((IEngine)APEnvironment.Engine).Frame.OpenView(FactoryGuid, (string)null);
			if (val != null)
			{
				((IEngine)APEnvironment.Engine).Frame.ActiveView=(val);
			}
		}

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}
	}
}
