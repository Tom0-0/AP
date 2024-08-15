using System;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.ProjectCompare;

namespace _3S.CoDeSys.DeviceObject
{
	internal static class Common
	{
		internal static bool IsEqual(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			return (int)diffState == 0;
		}

		internal static bool IsAdded(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			return (int)diffState == 1;
		}

		internal static bool IsDeleted(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			return (int)diffState == 2;
		}

		internal static bool IsContentDifferent(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			return ((int)diffState & 4) > 0;
		}

		internal static bool IsFolderDifferent(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			return ((int)diffState & 8) > 0;
		}

		internal static bool IsAccessRightsDifferent(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			return ((int)diffState & 0x10) > 0;
		}

		internal static bool IsPropertiesDifferent(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			return ((int)diffState & 0x20) > 0;
		}

		internal static bool IsAnythingDifferent(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			return ((int)diffState & 0x3C) > 0;
		}

		internal static bool IsAnyPropertyDifferent(DiffState diffState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			return ((int)diffState & 0x38) > 0;
		}

		internal static bool IsContentAccepted(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			return ((int)acceptState & 1) > 0;
		}

		internal static bool IsFolderAccepted(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			return ((int)acceptState & 2) > 0;
		}

		internal static bool IsAccessRightsAccepted(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			return ((int)acceptState & 4) > 0;
		}

		internal static bool IsPropertiesAccepted(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Invalid comparison between Unknown and I4
			return ((int)acceptState & 8) > 0;
		}

		internal static bool IsAnyPropertyAccepted(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			return ((int)acceptState & 0xE) > 0;
		}

		internal static bool IsAnythingAccepted(AcceptState acceptState)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			return ((int)acceptState & 0xF) > 0;
		}

		internal static AcceptState SetContentAccepted(AcceptState acceptState, bool bAccept)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (bAccept)
			{
				return (AcceptState)((int)acceptState | 1);
			}
			return (AcceptState)((int)acceptState & 0xFE);
		}

		internal static AcceptState SetFolderAccepted(AcceptState acceptState, bool bAccept)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (bAccept)
			{
				return (AcceptState)((int)acceptState | 2);
			}
			return (AcceptState)((int)acceptState & 0xFD);
		}

		internal static AcceptState SetAccessRightsAccepted(AcceptState acceptState, bool bAccept)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (bAccept)
			{
				return (AcceptState)((int)acceptState | 4);
			}
			return (AcceptState)((int)acceptState & 0xFB);
		}

		internal static AcceptState SetPropertiesAccepted(AcceptState acceptState, bool bAccept)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (bAccept)
			{
				return (AcceptState)((int)acceptState | 8);
			}
			return (AcceptState)((int)acceptState & 0xF7);
		}

		internal static Guid GetTypeGuid(Type type)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (type != null)
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(TypeGuidAttribute), inherit: false);
				if (customAttributes != null && customAttributes.Length != 0)
				{
					return ((TypeGuidAttribute)customAttributes[0]).Guid;
				}
			}
			return Guid.Empty;
		}
	}
}
