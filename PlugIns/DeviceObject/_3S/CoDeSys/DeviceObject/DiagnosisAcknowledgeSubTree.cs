using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{DA0D023D-78EE-440A-8AFD-E5461B6DBB57}")]
	public class DiagnosisAcknowledgeSubTree : IStandardCommand, ICommand
	{
		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Name");

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Description");

		public string ToolTipText => Name;

		public Icon SmallIcon => null;

		public Icon LargeIcon => SmallIcon;

		public bool Enabled => true;

		public string[] BatchCommand => new string[0];

		public void AddedToUI()
		{
		}

		public void RemovedFromUI()
		{
		}

		public bool IsVisible(bool bContextMenu)
		{
			return DiagnosisAcknowledgeHelper.IsVisible(bContextMenu);
		}

		public void ExecuteBatch(string[] arguments)
		{
			DiagnosisAcknowledgeHelper.ExecuteBatch(arguments, bRecursive: true);
		}

		public string[] CreateBatchArguments()
		{
			return DiagnosisAcknowledgeHelper.CreateBatchArguments();
		}
	}
}
