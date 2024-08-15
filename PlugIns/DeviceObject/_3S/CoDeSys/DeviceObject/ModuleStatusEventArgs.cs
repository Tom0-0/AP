using System;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ModuleStatusEventArgs : IModuleStatusEventArgs
	{
		private int _nProjectHandle;

		private Guid _guidObject;

		private int _nConnectorId;

		private Guid _guidApplication;

		public int ProjectHandle => _nProjectHandle;

		public Guid ObjectGuid => _guidObject;

		public int ConnectorId => _nConnectorId;

		public Guid ApplicationGuid => _guidApplication;

		internal ModuleStatusEventArgs(int nProjectHandle, Guid guidObject, int nConnectorId, Guid guidApplication)
		{
			_nProjectHandle = nProjectHandle;
			_guidObject = guidObject;
			_nConnectorId = nConnectorId;
			_guidApplication = guidApplication;
		}
	}
}
