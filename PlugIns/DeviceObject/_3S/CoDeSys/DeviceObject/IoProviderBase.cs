using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{848f25ec-eecf-4290-8329-7f6500fd2e0f}")]
	[StorageVersion("3.3.0.0")]
	public class IoProviderBase : GenericObject2
	{
		[DefaultSerialization("UserBaseAddress")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string[] _userBaseAddr = new string[3];

		private IDirectVariable[] _nextUnusedAddr = (IDirectVariable[])(object)new IDirectVariable[3];

		private int[] _granularity = new int[3];

		public string GetUserBaseAddress(DirectVariableLocation location)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return _userBaseAddr[LocationToIndex(location)];
		}

		public void SetUserBaseAddress(DirectVariableLocation location, string stAddress)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_userBaseAddr[LocationToIndex(location)] = stAddress;
		}

		public IDirectVariable GetNextUnusedAddress(DirectVariableLocation location)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return _nextUnusedAddr[LocationToIndex(location)];
		}

		public void SetNextUnusedAddress(IDirectVariable addr)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_nextUnusedAddr[LocationToIndex(addr.Location)] = addr;
		}

		public int GetGranularity(DirectVariableLocation location)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return _granularity[LocationToIndex(location)];
		}

		public void SetGranularity(DirectVariableLocation location, int nGranularity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_granularity[LocationToIndex(location)] = nGranularity;
		}

		public void GetModuleAddress(IIoProvider ioProvider, out uint uiModuleType, out uint uiInstance)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			IDeviceObject host = ioProvider.GetHost();
			IIoProvider val = (IIoProvider)(object)((host is IIoProvider) ? host : null);
			if (val == null)
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)10, "Unknown host");
			}
			if (DeviceObjectHelper.GenerateCodeForLogicalDevices)
			{
				LList<IIoProvider> mappedIoProvider = DeviceObjectHelper.GetMappedIoProvider(ioProvider, bCheckForLogical: true);
				if (mappedIoProvider.Count > 0)
				{
					ioProvider = mappedIoProvider[0];
				}
			}
			uiModuleType = (uint)ioProvider.TypeId;
			uiInstance = 0u;
			if (val.IoProviderEquals(ioProvider) || FindIoProviderInChildren(val, ioProvider, uiModuleType, ref uiInstance))
			{
				return;
			}
			throw new DeviceObjectException((DeviceObjectExeptionReason)11, "IoProvider not found in host");
		}

		private int LocationToIndex(DirectVariableLocation location)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected I4, but got Unknown
			return ((int)location - 1) switch
			{
				0 => 0, 
				1 => 1, 
				2 => 2, 
				_ => throw new ArgumentException($"Unknown location <{location.ToString()}>"), 
			};
		}

		private bool FindIoProviderInChildren(IIoProvider current, IIoProvider ioProvider, uint uiModuleType, ref uint uiInstance)
		{
			IIoProvider[] children = current.Children;
			foreach (IIoProvider val in children)
			{
				if (ioProvider is Connector && val is IExplicitConnector && (ioProvider as Connector).IsExplicit && ((IObject)((val is IExplicitConnector) ? val : null)).MetaObject != null && (ioProvider as Connector).ExplicitConnectorGuid == ((IObject)((val is IExplicitConnector) ? val : null)).MetaObject.ObjectGuid)
				{
					return true;
				}
				if (((object)val).Equals((object)ioProvider))
				{
					return true;
				}
				if (val.TypeId == uiModuleType)
				{
					if (val is IConnector && ioProvider is IConnector)
					{
						if (((IConnector)((val is IConnector) ? val : null)).ConnectorId == ((IConnector)((ioProvider is IConnector) ? ioProvider : null)).ConnectorId && val.GetMetaObject().ObjectGuid == ioProvider.GetMetaObject().ObjectGuid)
						{
							return true;
						}
					}
					else if (val.GetMetaObject().ObjectGuid == ioProvider.GetMetaObject().ObjectGuid)
					{
						return true;
					}
					uiInstance++;
				}
				if (FindIoProviderInChildren(val, ioProvider, uiModuleType, ref uiInstance))
				{
					return true;
				}
			}
			return false;
		}

		public IoProviderBase()
			: base()
		{
		}
	}
}
