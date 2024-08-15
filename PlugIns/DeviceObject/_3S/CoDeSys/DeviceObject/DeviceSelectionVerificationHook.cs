using System.Collections.Generic;
using System.Linq;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.PlcLogicObject;
using _3S.CoDeSys.RevisionControlHooks;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{c7efac90-8e9a-4e8b-a2d0-30a007be70f5}")]
	public class DeviceSelectionVerificationHook : IRcsSelectionVerificationHook
	{
		public void VerifySelection(IRcsSelectionVerificationArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if ((int)args.Action != 1 && (int)args.Action != 2)
			{
				return;
			}
			IEnumerable<IRcsVirtualProjectNode> changeSet = args.UserWantsToSelect.Concat(args.HooksWantToSelect);
			foreach (KeyValuePair<IRcsVirtualProjectNode, IRcsVirtualProjectNode> item in HandleChangeSet(changeSet))
			{
				args.RequestSelection(item.Key, item.Value, Strings.DeviceObjectSelectionVerificationHook);
			}
			IEnumerable<IRcsVirtualProjectNode> changeSet2 = args.UserWantsToDeSelect.Concat(args.HooksWantToDeSelect);
			foreach (KeyValuePair<IRcsVirtualProjectNode, IRcsVirtualProjectNode> item2 in HandleChangeSet(changeSet2))
			{
				args.RequestDeSelection(item2.Key, item2.Value, Strings.DeviceObjectSelectionVerificationHook);
			}
		}

		private IEnumerable<KeyValuePair<IRcsVirtualProjectNode, IRcsVirtualProjectNode>> HandleChangeSet(IEnumerable<IRcsVirtualProjectNode> changeSet)
		{
			Dictionary<IRcsVirtualProjectNode, IRcsVirtualProjectNode> dictionary = new Dictionary<IRcsVirtualProjectNode, IRcsVirtualProjectNode>();
			foreach (IRcsVirtualProjectNode item in changeSet)
			{
				if (!dictionary.ContainsKey(item) && item.IsObjectNode && item.IsDirty && IsInterestingTreeNode(item))
				{
					IRcsVirtualProjectNode val = item;
					while (IsInterestingTreeNode(val.ParentNode))
					{
						val = val.ParentNode;
					}
					SelectDeviceChildrenRecursively(val, dictionary, item);
				}
			}
			return dictionary;
		}

		private void SelectDeviceChildrenRecursively(IRcsVirtualProjectNode device, Dictionary<IRcsVirtualProjectNode, IRcsVirtualProjectNode> coveredObjects, IRcsVirtualProjectNode cause)
		{
			if (device.IsDirty)
			{
				coveredObjects[device] = cause;
			}
			foreach (IRcsVirtualProjectNode childNode in device.ChildNodes)
			{
				if (IsInterestingTreeNode(childNode))
				{
					SelectDeviceChildrenRecursively(childNode, coveredObjects, cause);
				}
			}
		}

		private static bool IsInterestingTreeNode(IRcsVirtualProjectNode projectNode)
		{
			IRcsVirtualObjectDataNode val = (IRcsVirtualObjectDataNode)(object)((projectNode is IRcsVirtualObjectDataNode) ? projectNode : null);
			if (val == null)
			{
				return false;
			}
			if (!typeof(IDeviceObject).IsAssignableFrom(val.ObjectType) && !typeof(IConnector).IsAssignableFrom(val.ObjectType) && !typeof(IPlcLogicObject).IsAssignableFrom(val.ObjectType))
			{
				return typeof(ISlotDeviceObject).IsAssignableFrom(val.ObjectType);
			}
			return true;
		}
	}
}
