using System;
using System.Drawing;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	public abstract class AbstractCommand : IStandardCommand, ICommand
	{
		public Guid Category => CommandCategory.TYPEGUID;

		public abstract string Name { get; }

		public abstract string Description { get; }

		public string ToolTipText => Name;

		public abstract Icon SmallIcon { get; }

		public Icon LargeIcon => SmallIcon;

		public abstract bool Enabled { get; }

		public abstract string[] BatchCommand { get; }

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
			if (bContextMenu)
			{
				return GetActiveEditor() != null;
			}
			return false;
		}

		public abstract void ExecuteBatch(string[] arguments);

		protected TabularDeclarationEditor GetActiveEditor()
		{
			return UIHelper.GetFirstFocusedUiElement<TabularDeclarationEditor>();
		}
	}
}
