using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class IoTaskUsage : IIOTaskUsage2, IIOTaskUsage
	{
		private List<ITaskInfo> _liTasks = new List<ITaskInfo>();

		private IParameter _parameter;

		private Guid _objectGuid;

		private uint _uiConnectorIndex;

		private long _lParameterIndex;

		private ITaskInfo _busTask;

		private string _stMappedAddress = string.Empty;

		private Mapping _mapping;

		public Guid ObjectGuid => _objectGuid;

		public IDataElement DataElement => _mapping.DataElement;

		public IParameter Parameter => _parameter;

		public List<ITaskInfo> UsedTasks => _liTasks;

		public ITaskInfo BusTask
		{
			get
			{
				return _busTask;
			}
			set
			{
				_busTask = value;
			}
		}

		public string MappedAddress
		{
			get
			{
				return _stMappedAddress;
			}
			set
			{
				_stMappedAddress = value;
			}
		}

		public uint ConnectorIndex
		{
			get
			{
				return _uiConnectorIndex;
			}
			set
			{
				_uiConnectorIndex = value;
			}
		}

		public long ParameterIndex
		{
			get
			{
				return _lParameterIndex;
			}
			set
			{
				_lParameterIndex = value;
			}
		}

		public Mapping Mapping => _mapping;

		public ITaskMapping Taskmapping => (ITaskMapping)(object)_mapping;

		public IoTaskUsage(Guid ObjectGuid, IParameter parameter, Mapping mapping)
		{
			_objectGuid = ObjectGuid;
			_parameter = parameter;
			_mapping = mapping;
		}
	}
}
