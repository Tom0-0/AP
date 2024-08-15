using System;
using System.Collections;
using System.Collections.Generic;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class AddrToChannelMap
	{
		private ArrayList _alBusCycleInfos = new ArrayList();

		private ArrayList _alWatchdogTrigger = new ArrayList();

		private LSortedList<long, ChannelRef> _inputs = new LSortedList<long, ChannelRef>();

		private LSortedList<long, ChannelRef> _outputs = new LSortedList<long, ChannelRef>();

		private DoubleAddressChecker _inChecker;

		private DoubleAddressChecker _outChecker;

		private ICompileContext _comcon;

		public StartBusCycleInfo[] GetAllBusCycleInfos
		{
			get
			{
				StartBusCycleInfo[] array = new StartBusCycleInfo[_alBusCycleInfos.Count];
				_alBusCycleInfos.CopyTo(array, 0);
				return array;
			}
		}

		public AddrToChannelMap(ICompileContext comcon, long lInStartOffset, long lOutStartOffset, int iInSize, int iOutSize)
		{
			_comcon = comcon;
			_inChecker = new DoubleAddressChecker(lInStartOffset, iInSize);
			_outChecker = new DoubleAddressChecker(lOutStartOffset, iOutSize);
		}

		internal void CheckOverlap(DoubleAddressChecker checker, ChannelRef newobj, IDirectVariable addr, IDataElement dataElement, Hashtable htStartAddresses)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			if (((ICollection)dataElement.IoMapping.VariableMappings).Count > 0)
			{
				bool flag = false;
				foreach (IVariableMapping item in (IEnumerable)dataElement.IoMapping.VariableMappings)
				{
					if (item.CreateVariable)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			long bitOffset = newobj.LocationStart.BitOffset;
			long num = newobj.LocationEnd.BitOffset + 1;
			long num2 = -1L;
			if (bitOffset % 8 == 0L && num % 8 == 0L)
			{
				for (long num3 = bitOffset; num3 < num; num3 += 8)
				{
					if (!checker.CheckByte(num3))
					{
						checker.SetByte(num3);
						continue;
					}
					num2 = num3;
					break;
				}
			}
			else
			{
				for (long num4 = bitOffset; num4 < num; num4++)
				{
					if (!checker.CheckBit(num4))
					{
						checker.SetBit(num4);
						continue;
					}
					num2 = num4;
					break;
				}
			}
			if (num2 < 0)
			{
				return;
			}
			if (num2 > bitOffset)
			{
				bool flag2 = default(bool);
				foreach (IDataElement item2 in (IEnumerable)dataElement.SubElements)
				{
					IDirectVariable iecAddress = (item2.IoMapping as IoMapping).GetIecAddress(htStartAddresses);
					BitDataLocation bitDataLocation = new BitDataLocation(_comcon.LocateAddress(out flag2, iecAddress));
					if (!flag2 && bitDataLocation.BitOffset >= num2)
					{
						addr = iecAddress;
						break;
					}
				}
			}
			throw new ArgumentException(string.Format(((IEngine)APEnvironment.Engine).ResourceManager.GetString(typeof(Strings), "ErrorAddressAlreadyUsed"), ((object)addr).ToString()));
		}

		public void AddChannel(ConnectorMap connectorMap, long lParamId, long lBitOffset, IDataLocation datalocation, IDirectVariable addr, string baseType, IDataElement dataElement, Hashtable htStartAddresses, bool bSkipCheckOverlap)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Invalid comparison between Unknown and I4
			if ((int)addr.Location == 1)
			{
				ChannelRef channelRef = new ChannelRef(connectorMap, lParamId, lBitOffset, datalocation, baseType, dataElement);
				if (!bSkipCheckOverlap)
				{
					CheckOverlap(_inChecker, channelRef, addr, dataElement, htStartAddresses);
				}
				if (!_inputs.ContainsKey(channelRef.LocationStart.BitOffset))
				{
					_inputs.Add(channelRef.LocationStart.BitOffset, channelRef);
				}
			}
			else if ((int)addr.Location == 2)
			{
				ChannelRef channelRef2 = new ChannelRef(connectorMap, lParamId, lBitOffset, datalocation, baseType, dataElement);
				if (!bSkipCheckOverlap)
				{
					CheckOverlap(_outChecker, channelRef2, addr, dataElement, htStartAddresses);
				}
				if (!_outputs.ContainsKey(channelRef2.LocationStart.BitOffset))
				{
					_outputs.Add(channelRef2.LocationStart.BitOffset, channelRef2);
				}
			}
		}

		public int FindFirstPossibleIntersection(IDirectVariable addr, BitDataLocation bitlocation)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			IList<ChannelRef> list = (((int)addr.Location != 1) ? _outputs.Values : _inputs.Values);
			int num = 0;
			int num2 = list.Count;
			while (num <= num2)
			{
				int num3 = (num + num2) / 2;
				if (num3 >= 0 && num3 < list.Count)
				{
					ChannelRef channelRef = list[num3];
					if (channelRef.LocationEnd.BitOffset < bitlocation.BitOffset)
					{
						num = num3 + 1;
						continue;
					}
					if (channelRef.LocationEnd.BitOffset > bitlocation.BitOffset)
					{
						num2 = num3 - 1;
						continue;
					}
					return num3;
				}
				return -1;
			}
			return num;
		}

		public IList<ChannelRef> GetChannelRefList(DirectVariableLocation location)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			if ((int)location == 1)
			{
				return _inputs.Values;
			}
			if ((int)location == 2)
			{
				return _outputs.Values;
			}
			return (IList<ChannelRef>)new LList<ChannelRef>();
		}

		public void AddStartBusCycle(int nModuleId, string stTask, bool bBeforeReadInputs, bool bAfterWriteOutputs, bool ExternEvent)
		{
			_alBusCycleInfos.Add(new StartBusCycleInfo(nModuleId, stTask, bBeforeReadInputs, bAfterWriteOutputs, ExternEvent));
		}

		public void AddWatchdogTrigger(int nModuleId, string stTask)
		{
			_alWatchdogTrigger.Add(new StartBusCycleInfo(nModuleId, stTask, bBeforeReadInputs: false, bAfterWriteOutputs: true, ExternEvent: false));
		}

		public StartBusCycleInfo[] GetBusCycleInfos(string stTask, bool bDefaultTask)
		{
			ArrayList arrayList = new ArrayList();
			foreach (StartBusCycleInfo alBusCycleInfo in _alBusCycleInfos)
			{
				if (alBusCycleInfo.Task == stTask || (bDefaultTask && alBusCycleInfo.Task == ""))
				{
					arrayList.Add(alBusCycleInfo);
				}
			}
			StartBusCycleInfo[] array = new StartBusCycleInfo[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}

		public StartBusCycleInfo[] GetWatchdogTriggers(string stTask, bool bDefaultTask)
		{
			ArrayList arrayList = new ArrayList();
			foreach (StartBusCycleInfo item in _alWatchdogTrigger)
			{
				if (item.Task == stTask || (bDefaultTask && item.Task == ""))
				{
					arrayList.Add(item);
				}
			}
			StartBusCycleInfo[] array = new StartBusCycleInfo[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}
	}
}
