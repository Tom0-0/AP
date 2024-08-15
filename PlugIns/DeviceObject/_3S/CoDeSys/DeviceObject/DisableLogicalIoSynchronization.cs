using System;
using System.Drawing;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{6CD8E674-51D8-4302-A5A8-3D66F065CBFC}")]
	public class DisableLogicalIoSynchronization : IToggleCommand2, IToggleCommand, ICommand
	{
		public string[] BatchCommand => new string[0];

		public Guid Category => DeviceCommandHelper.GUID_DEVICECMDCATEGORY;

		public bool Checked
		{
			get
			{
				IMetaObjectStub selectedNode = GetSelectedNode();
				if (selectedNode != null && typeof(ILogicalObject2).IsAssignableFrom(selectedNode.ObjectType))
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(selectedNode.ProjectHandle, selectedNode.ObjectGuid);
					IObject obj = ((objectToRead != null) ? objectToRead.Object : null);
					ILogicalObject2 val = (ILogicalObject2)(object)((obj is ILogicalObject2) ? obj : null);
					if (val != null && val.DisableSynchronization)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string Description => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Description");

		public bool Enabled => IsVisible(bContextMenu: true);

		public Icon LargeIcon => SmallIcon;

		public string Name => ((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), GetType().Name + "_Name");

		public bool RadioCheck => false;

		public Icon SmallIcon => null;

		public string ToolTipText => Name;

		public void AddedToUI()
		{
		}

		public string[] CreateBatchArguments()
		{
			return CreateBatchArguments(bInvokedByContextMenu: false);
		}

		public string[] CreateBatchArguments(bool bInvokedByContextMenu)
		{
			if (bInvokedByContextMenu)
			{
				IMetaObjectStub selectedNode = GetSelectedNode();
				if (selectedNode != null)
				{
					return new string[2]
					{
						selectedNode.ProjectHandle.ToString(),
						selectedNode.ObjectGuid.ToString()
					};
				}
			}
			return new string[0];
		}

		public void ExecuteBatch(string[] arguments)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (arguments.Length > 2)
			{
				throw new BatchTooManyArgumentsException(BatchCommand, arguments.Length, 2);
			}
			if (arguments.Length < 2)
			{
				throw new BatchTooFewArgumentsException(BatchCommand, arguments.Length, 2);
			}
			if (arguments == null || arguments.Length != 2 || !int.TryParse(arguments[0], out var result) || !Guid.TryParse(arguments[1], out var result2) || !((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(result, result2))
			{
				return;
			}
			IMetaObject val = null;
			bool flag = false;
			try
			{
				val = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToModify(result, result2);
				IObject @object = val.Object;
				ILogicalObject2 val2 = (ILogicalObject2)(object)((@object is ILogicalObject2) ? @object : null);
				if (val2 != null)
				{
					val2.DisableSynchronization=(!val2.DisableSynchronization);
				}
				flag = true;
			}
			catch
			{
			}
			finally
			{
				if (val != null && val.IsToModify)
				{
					((IObjectManager)APEnvironment.ObjectMgr).SetObject(val, flag, (object)null);
				}
			}
		}

		public bool IsVisible(bool bContextMenu)
		{
			if (bContextMenu)
			{
				IMetaObjectStub selectedNode = GetSelectedNode();
				if (selectedNode != null && typeof(ILogicalObject2).IsAssignableFrom(selectedNode.ObjectType))
				{
					return true;
				}
			}
			return false;
		}

		public void RemovedFromUI()
		{
		}

		internal IMetaObjectStub GetSelectedNode()
		{
			if (((IEngine)APEnvironment.Engine).Projects.PrimaryProject == null)
			{
				return null;
			}
			ISVNode[] selectedSVNodes = ((IEngine)APEnvironment.Engine).Projects.PrimaryProject.SelectedSVNodes;
			if (selectedSVNodes == null || selectedSVNodes.Length != 1)
			{
				return null;
			}
			return selectedSVNodes[0].GetMetaObjectStub();
		}
	}
}
