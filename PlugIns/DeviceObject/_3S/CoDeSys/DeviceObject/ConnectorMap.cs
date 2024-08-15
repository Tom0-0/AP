using System;
using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	public class ConnectorMap
	{
		private string _stAddrConnector;

		private Guid _guidObject;

		private string _stMetaName = string.Empty;

		private ArrayList _alChannelMapsInputs = new ArrayList();

		private ArrayList _alChannelMapsOutputs = new ArrayList();

		private int _nBusTask;

		private int _nProjectHandle;

		private ITaskMappingInfo _mappingInfo;

		private bool _bShowMultipleMappingsAsError;

		private IParameterSet _paramSet;

		private bool _bSkipOverlapCheck;

		public string AddrConnector => _stAddrConnector;

		public Guid ObjectGuid => _guidObject;

		public string MetaName => _stMetaName;

		public int BusTask => _nBusTask;

		public int ProjectHandle => _nProjectHandle;

		public ITaskMappingInfo MappingInfo => _mappingInfo;

		public bool ShowMultipleMappingsAsError => _bShowMultipleMappingsAsError;

		public IParameterSet ParamSet => _paramSet;

		public bool SkipOverlapCheck
		{
			get
			{
				return _bSkipOverlapCheck;
			}
			set
			{
				_bSkipOverlapCheck = value;
			}
		}

		public ConnectorMap(ITaskMappingInfo mappingInfo, IParameterSet paramSet, string stAddrConnector, int nProjectHandle, Guid guidObject, string stMetaName, int nBusTask, bool bShowMultipleMappingsAsError)
		{
			_mappingInfo = mappingInfo;
			_stAddrConnector = stAddrConnector;
			_guidObject = guidObject;
			_nProjectHandle = nProjectHandle;
			_stMetaName = stMetaName;
			_nBusTask = nBusTask;
			_bShowMultipleMappingsAsError = bShowMultipleMappingsAsError;
			_paramSet = paramSet;
		}

		public void AddChannelMap(ChannelMap map)
		{
			if (map.IsInput)
			{
				_alChannelMapsInputs.Add(map);
			}
			else
			{
				_alChannelMapsOutputs.Add(map);
			}
		}

		public IList GetChannelMapList(bool bInput)
		{
			if (bInput)
			{
				return _alChannelMapsInputs;
			}
			return _alChannelMapsOutputs;
		}
	}
}
