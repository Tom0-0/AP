using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class LateLanguageModel
	{
		private AddrToChannelMap _channelMap;

		private ConnectorMapList _connectorMapList = new ConnectorMapList();

		private CyclicCallsForLanguageModel _cyclicCalls = new CyclicCallsForLanguageModel();

		private FixedTaskUpdates _fixedTaskUpdates;

		private StringBuilder sbFbInits;

		private StringBuilder _sbAfterUpdateConfigurationCode = new StringBuilder();

		public CyclicCallsForLanguageModel CyclicCalls => _cyclicCalls;

		public AddrToChannelMap AddrToChannelMap
		{
			get
			{
				return _channelMap;
			}
			set
			{
				_channelMap = value;
			}
		}

		public ConnectorMapList ConnectorMapList => _connectorMapList;

		public FixedTaskUpdates FixedTaskUpdates => _fixedTaskUpdates;

		public LateLanguageModel(ITaskInfo[] taskInfos)
		{
			_fixedTaskUpdates = new FixedTaskUpdates(taskInfos);
		}

		public void AddFbInit(int nModuleType, uint uiInstance, int nModuleIndex, string stInstanceName, string stInitName)
		{
			if (sbFbInits == null)
			{
				sbFbInits = new StringBuilder();
			}
			sbFbInits.AppendFormat("{0}.{1}(wModuleType:={2}, dwInstance:={3}, pConnector:=ADR(moduleList[{4}]));\n", stInstanceName, stInitName, nModuleType, uiInstance, nModuleIndex);
		}

		public void AddFBInit(string stInit)
		{
			if (sbFbInits == null)
			{
				sbFbInits = new StringBuilder();
			}
			sbFbInits.AppendLine(stInit);
		}

		public void AddFbDiagInit(string stInstanceName, string stInitName, int nModuleInstance, int nModuleCount, string stParentDevInstance, string stChildDevInstance, int nChildCount)
		{
			if (sbFbInits == null)
			{
				sbFbInits = new StringBuilder();
			}
			sbFbInits.AppendFormat("{0}.{1}(ADR(moduleList[{2}], {3}, {4}, {5}, {6});\n", stInstanceName, stInitName, nModuleInstance, nModuleCount, stParentDevInstance, stChildDevInstance, nChildCount);
		}

		public void AddAfterUpdateConfigurationCode(string stCode)
		{
			_sbAfterUpdateConfigurationCode.AppendLine(stCode);
		}

		public string GetFbInits()
		{
			if (sbFbInits == null)
			{
				return "";
			}
			return sbFbInits.ToString();
		}

		public string GetAfterUpdateConfigurationCode()
		{
			return _sbAfterUpdateConfigurationCode.ToString();
		}
	}
}
