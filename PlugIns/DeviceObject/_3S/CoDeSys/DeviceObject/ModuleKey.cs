using System;

namespace _3S.CoDeSys.DeviceObject
{
	internal class ModuleKey
	{
		private Guid _guidObject;

		private int _nConnectorId;

		public Guid ObjectGuid => _guidObject;

		public int ConnectorId => _nConnectorId;

		internal ModuleKey(Guid guidObject, int nConnectorId)
		{
			_guidObject = guidObject;
			_nConnectorId = nConnectorId;
		}

		public override bool Equals(object obj)
		{
			ModuleKey moduleKey = (ModuleKey)obj;
			if (_guidObject == moduleKey._guidObject)
			{
				return _nConnectorId == moduleKey._nConnectorId;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _guidObject.GetHashCode() ^ _nConnectorId;
		}
	}
}
