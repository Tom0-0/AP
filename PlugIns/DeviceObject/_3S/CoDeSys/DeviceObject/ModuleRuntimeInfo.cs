using System;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ModuleRuntimeInfo : IModuleRuntimeInfo
	{
		private int _nProjectHandle;

		private Guid _guidObject;

		private int _nConnectorId;

		private ModuleStatus _status;

		private bool _bValid;

		public int ProjectHandle => _nProjectHandle;

		public Guid ObjectGuid => _guidObject;

		public int ConnectorId => _nConnectorId;

		public ModuleStatus Status => _status;

		public bool StatusAvailable => _bValid;

		internal ModuleRuntimeInfo(int nProjectHandle, Guid guidObject, int nConnectorId, ModuleStatus status, bool bValid)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			_nProjectHandle = nProjectHandle;
			_guidObject = guidObject;
			_nConnectorId = nConnectorId;
			_status = status;
			_bValid = bValid;
		}
	}
}
