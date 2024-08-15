#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public class FlatAdressAssignmentStrategy : IAddressAssignmentStrategy3, IAddressAssignmentStrategy2, IAddressAssignmentStrategy
	{
		internal class ManualAddressData
		{
			private readonly IIoProvider _ioProvider;

			private readonly bool _bHasManualAddress;

			private readonly IParameter _firstInput;

			private readonly IParameter _lastInput;

			private readonly IParameter _firstOutput;

			private readonly IParameter _lastOutput;

			private readonly IDirectVariable _firstInputVar;

			private readonly IDirectVariable _firstOutputVar;

			private readonly bool _bHasInput;

			private readonly bool _bHasOuput;

			private ulong _ulStartInputAddress;

			private ulong _ulEndInputAddress;

			private ulong _ulStartOutputAddress;

			private ulong _ulEndOutputAddress;

			internal IIoProvider IoProvider => _ioProvider;

			internal bool HasManualAddress => _bHasManualAddress;

			internal IParameter FirstInput => _firstInput;

			internal IParameter LastInput => _lastInput;

			internal IParameter FirstOutput => _firstOutput;

			internal IParameter LastOutput => _lastOutput;

			internal ulong StartInputAddress
			{
				get
				{
					return _ulStartInputAddress;
				}
				set
				{
					_ulStartInputAddress = value;
				}
			}

			internal ulong EndInputAddress
			{
				get
				{
					return _ulEndInputAddress;
				}
				set
				{
					_ulEndInputAddress = value;
				}
			}

			internal ulong StartOutputAddress
			{
				get
				{
					return _ulStartOutputAddress;
				}
				set
				{
					_ulStartOutputAddress = value;
				}
			}

			internal ulong EndOutputAddress
			{
				get
				{
					return _ulEndOutputAddress;
				}
				set
				{
					_ulEndOutputAddress = value;
				}
			}

			internal bool HasInput => _bHasInput;

			internal bool HasOutput => _bHasOuput;

			internal IDirectVariable FirstInputVar => _firstInputVar;

			internal IDirectVariable FirstOutputVar => _firstOutputVar;

			internal ManualAddressData(IIoProvider ioProvider, FlatAdressAssignmentStrategy strategy, Hashtable htStartAddresses, bool bHasManualAddress, IParameter firstInput, IParameter lastInput, IParameter firstOutput, IParameter lastOutput)
			{
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Invalid comparison between Unknown and I4
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Invalid comparison between Unknown and I4
				_ioProvider = ioProvider;
				_bHasManualAddress = bHasManualAddress;
				_firstInput = firstInput;
				_lastInput = lastInput;
				_firstOutput = firstOutput;
				_lastOutput = lastOutput;
				if (firstInput != null && lastInput != null && ((IDataElement)firstInput).IoMapping is IoMapping)
				{
					_firstInputVar = (((IDataElement)firstInput).IoMapping as IoMapping).GetIecAddress(htStartAddresses);
					if (_firstInputVar != null)
					{
						_ulStartInputAddress = strategy.GetBitAddress(_firstInputVar);
						IDirectVariable iecAddress = (((IDataElement)lastInput).IoMapping as IoMapping).GetIecAddress(htStartAddresses);
						if ((int)iecAddress.Size == 1)
						{
							_ulEndInputAddress = strategy.GetBitAddress(iecAddress);
						}
						else
						{
							_ulEndInputAddress = (ulong)((long)strategy.GetBitAddress(iecAddress) + ((IDataElement)lastInput).GetBitSize() - 1);
						}
						_bHasInput = true;
					}
				}
				if (firstOutput == null || lastOutput == null || !(((IDataElement)firstOutput).IoMapping is IoMapping))
				{
					return;
				}
				_firstOutputVar = (((IDataElement)firstOutput).IoMapping as IoMapping).GetIecAddress(htStartAddresses);
				if (_firstOutputVar != null)
				{
					_ulStartOutputAddress = strategy.GetBitAddress(_firstOutputVar);
					IDirectVariable iecAddress2 = (((IDataElement)lastOutput).IoMapping as IoMapping).GetIecAddress(htStartAddresses);
					if ((int)iecAddress2.Size == 1)
					{
						_ulEndOutputAddress = strategy.GetBitAddress(iecAddress2);
					}
					else
					{
						_ulEndOutputAddress = (ulong)((long)strategy.GetBitAddress(iecAddress2) + ((IDataElement)lastOutput).GetBitSize() - 1);
					}
					_bHasOuput = true;
				}
			}

			internal bool AssignNewAddress(FlatAdressAssignmentStrategy strategy, LList<ManualAddressData> liAddresses, ManualAddressData checkdata)
			{
				bool result = false;
				if (checkdata.HasInput && _bHasInput)
				{
					result = AssignNewAdress(strategy, (IList<ManualAddressData>)liAddresses, bIsInput: true, checkdata, ref _ulStartInputAddress, ref _ulEndInputAddress);
				}
				if (checkdata.HasOutput && _bHasOuput)
				{
					result = AssignNewAdress(strategy, (IList<ManualAddressData>)liAddresses, bIsInput: false, checkdata, ref _ulStartOutputAddress, ref _ulEndOutputAddress);
				}
				return result;
			}

			private bool AssignNewAdress(FlatAdressAssignmentStrategy strategy, IList<ManualAddressData> liAddresses, bool bIsInput, ManualAddressData checkdata, ref ulong ulStartAddressToChange, ref ulong ulEndAddressToChange)
			{
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				ulong num;
				ulong num2;
				IDirectVariable val;
				IParameter val2;
				if (bIsInput)
				{
					num = checkdata.StartInputAddress;
					num2 = checkdata.EndInputAddress;
					val = _firstInputVar;
					val2 = _firstInput;
				}
				else
				{
					num = checkdata.StartOutputAddress;
					num2 = checkdata.EndOutputAddress;
					val = _firstOutputVar;
					val2 = _firstOutput;
				}
				bool result = false;
				if ((num >= ulStartAddressToChange && num <= ulEndAddressToChange) || (num2 >= ulStartAddressToChange && num2 <= ulEndAddressToChange) || (num < ulStartAddressToChange && num2 > ulEndAddressToChange))
				{
					ulong num3 = ulEndAddressToChange - ulStartAddressToChange;
					ulong num4 = 0uL;
					int granularity = _ioProvider.GetGranularity(val.Location);
					long num5 = granularity * 8;
					LSortedList<ulong, ulong> val3 = new LSortedList<ulong, ulong>();
					foreach (ManualAddressData liAddress in liAddresses)
					{
						if (liAddress.HasInput && bIsInput)
						{
							val3[liAddress.StartInputAddress]= liAddress.EndInputAddress;
						}
						if (liAddress.HasOutput && !bIsInput)
						{
							val3[liAddress.StartOutputAddress]= liAddress.EndOutputAddress;
						}
					}
					foreach (KeyValuePair<ulong, ulong> item in val3)
					{
						if (num4 + num3 >= item.Key && num4 <= item.Value)
						{
							num4 = item.Value + 1;
							if (num5 != 0L)
							{
								num4 = (ulong)(((long)num4 + num5 - 1) / num5 * num5);
							}
						}
					}
					FixedIecAddress fixedIecAddress = new FixedIecAddress();
					fixedIecAddress.Location = val.Location;
					fixedIecAddress.Size = val.Size;
					int[] original = strategy.AlignComponents(granularity, new int[2]
					{
						(int)num4 / 8,
						(int)num4 % 8
					});
					fixedIecAddress.Components = strategy.GetComponentsNative(fixedIecAddress.Size, original);
					((IDataElement)val2).IoMapping.SetIntermediateAddress((IDirectVariable)(object)fixedIecAddress);
					ulStartAddressToChange = num4;
					ulEndAddressToChange = num4 + num3;
					result = true;
					IEditor[] editors = ((IEngine)APEnvironment.Engine).EditorManager.GetEditors(_ioProvider.GetMetaObject());
					if (editors.Length != 0)
					{
						IEditor[] array = editors;
						for (int i = 0; i < array.Length; i++)
						{
							array[i].Reload();
						}
					}
				}
				return result;
			}
		}

		private readonly int _iMinStructureGranularity;

		public FlatAdressAssignmentStrategy(int iMinStructureGranularity)
		{
			_iMinStructureGranularity = iMinStructureGranularity;
		}

		protected IIoProvider GetPredecessor(IIoProvider ioProvider)
		{
			IIoProvider parent = ioProvider.Parent;
			if (parent == null)
			{
				return null;
			}
			IIoProvider[] children = parent.Children;
			int indexOfIoProvider = GetIndexOfIoProvider(children, ioProvider);
			switch (indexOfIoProvider)
			{
			case 0:
				return parent;
			default:
			{
				IIoProvider rootOfSubtree = children[indexOfIoProvider - 1];
				return GetLastNodeOfSubtree(rootOfSubtree);
			}
			case -1:
				return null;
			}
		}

		protected IIoProvider GetLastNodeOfSubtree(IIoProvider rootOfSubtree)
		{
			IIoProvider[] children = rootOfSubtree.Children;
			if (children == null || children.Length == 0)
			{
				return rootOfSubtree;
			}
			return GetLastNodeOfSubtree(children[children.Length - 1]);
		}

		public IDirectVariable ResolveBaseAddress(DirectVariableLocation location, IIoProvider ioProvider)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ResolveBaseAddress(location, ioProvider, null);
		}

		public IDirectVariable ResolveBaseAddress(DirectVariableLocation location, IIoProvider ioProvider, Hashtable htStartAddresses)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			IDirectVariable val = (IDirectVariable)(object)ParseIecAddress(ioProvider.GetUserBaseAddress(location));
			if (val == null)
			{
				IIoProvider predecessor = GetPredecessor(ioProvider);
				if (predecessor == null || predecessor.GetNextUnusedAddress(location) == null)
				{
					FixedIecAddress fixedIecAddress = new FixedIecAddress();
					fixedIecAddress.Location = location;
					fixedIecAddress.Size = (DirectVariableSize)2;
					fixedIecAddress.Components = GetComponentsNative(fixedIecAddress.Size, new int[2]);
					val = (IDirectVariable)(object)fixedIecAddress;
				}
				else
				{
					val = ResolveAddress(predecessor.GetNextUnusedAddress(location), predecessor, htStartAddresses);
				}
			}
			return val;
		}

		public IDirectVariable ResolveAddress(IDirectVariable relAddr, IIoProvider ioProvider)
		{
			return ResolveAddress(relAddr, ioProvider, null);
		}

		public IDirectVariable ResolveAddress(IDirectVariable addr, IIoProvider ioProvider, Hashtable htStartAddresses)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between Unknown and I4
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			if (addr == null)
			{
				return (IDirectVariable)(object)new FixedIecAddress();
			}
			if (addr is FixedIecAddress)
			{
				return addr;
			}
			IDirectVariable val = null;
			if (htStartAddresses != null && htStartAddresses.ContainsKey(ioProvider))
			{
				LateLanguageStartAddresses lateLanguageStartAddresses = htStartAddresses[ioProvider] as LateLanguageStartAddresses;
				val = (((int)addr.Location != 1) ? lateLanguageStartAddresses.startOutAddress : lateLanguageStartAddresses.startInAddress);
			}
			if (val == null)
			{
				val = ResolveBaseAddress(addr.Location, ioProvider, htStartAddresses);
			}
			int granularity = ioProvider.GetGranularity(addr.Location);
			int[] componentsNormalized = GetComponentsNormalized(val);
			componentsNormalized = AlignComponents(granularity, componentsNormalized);
			int[] componentsNormalized2 = GetComponentsNormalized(addr);
			int[] array = new int[2]
			{
				componentsNormalized[0] + componentsNormalized2[0],
				componentsNormalized[1] + componentsNormalized2[1]
			};
			array[0] += array[1] / 8;
			array[1] %= 8;
			FixedIecAddress fixedIecAddress = new FixedIecAddress();
			fixedIecAddress.Location = addr.Location;
			fixedIecAddress.Size = addr.Size;
			fixedIecAddress.Components = GetComponentsNative(fixedIecAddress.Size, array);
			return (IDirectVariable)(object)fixedIecAddress;
		}

		public void UpdateAddresses(IIoProvider ioProvider)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected I4, but got Unknown
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			foreach (IParameter item in (IEnumerable)ioProvider.ParameterSet)
			{
				IParameter val = item;
				ChannelType channelType = val.ChannelType;
				switch ((int)channelType - 1)
				{
				case 0:
					arrayList.Add(val);
					break;
				case 1:
					arrayList2.Add(val);
					break;
				case 2:
					arrayList2.Add(val);
					break;
				}
			}
			bool flag = false;
			if (typeof(IDeviceObject9).IsAssignableFrom(((object)ioProvider).GetType()))
			{
				flag = ((IDeviceObject9)((ioProvider is IDeviceObject9) ? ioProvider : null)).ShowParamsInDevDescOrder;
			}
			else
			{
				IMetaObject metaObject = ioProvider.GetMetaObject();
				if (metaObject != null && typeof(IDeviceObject9).IsAssignableFrom(((object)metaObject.Object).GetType()))
				{
					IObject @object = metaObject.Object;
					flag = ((IDeviceObject9)((@object is IDeviceObject9) ? @object : null)).ShowParamsInDevDescOrder;
				}
			}
			if (flag)
			{
				arrayList.Sort(ParameterByDevDescComparer.Instance);
				arrayList2.Sort(ParameterByDevDescComparer.Instance);
			}
			else
			{
				arrayList.Sort(ParameterByIdComparer.Instance);
				arrayList2.Sort(ParameterByIdComparer.Instance);
			}
			Update(ioProvider, arrayList, (DirectVariableLocation)1);
			Update(ioProvider, arrayList2, (DirectVariableLocation)2);
		}

		private int GetIndexOfIoProvider(IIoProvider[] providerList, IIoProvider provider)
		{
			for (int i = 0; i < providerList.Length; i++)
			{
				if (providerList[i] == provider)
				{
					return i;
				}
			}
			for (int j = 0; j < providerList.Length; j++)
			{
				if (provider.IoProviderEquals(providerList[j]))
				{
					return j;
				}
			}
			return -1;
		}

		private void Update(IIoProvider ioProvider, IList paramList, DirectVariableLocation location)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			int nGranularity = 0;
			int[] array = new int[2];
			bool bRelative = true;
			IIoProvider val = null;
			if (DeviceObjectHelper.GenerateCodeForLogicalDevices)
			{
				LList<IIoProvider> mappedIoProvider = DeviceObjectHelper.GetMappedIoProvider(ioProvider, bCheckForLogical: true);
				if (mappedIoProvider.Count > 0)
				{
					val = mappedIoProvider[0];
				}
				if (val != null && paramList.Count > 0)
				{
					foreach (IParameter item in (IEnumerable)val.ParameterSet)
					{
						IParameter val2 = item;
						ChannelType channelType = val2.ChannelType;
						object obj = paramList[0];
						if (channelType == ((IParameter)((obj is IParameter) ? obj : null)).ChannelType)
						{
							FixedIecAddress addr = ParseIecAddress(((IDataElement)val2).IoMapping.IecAddress);
							array = GetComponentsNormalized((IDirectVariable)(object)addr);
							bRelative = false;
							break;
						}
					}
				}
			}
			for (int i = 0; i < paramList.Count; i++)
			{
				IParameter val3 = (IParameter)paramList[i];
				if (val != null)
				{
					bool flag = false;
					if (val.ParameterSet.Contains(val3.Id))
					{
						FixedIecAddress fixedIecAddress = ParseIecAddress(((IDataElement)val.ParameterSet.GetParameter(val3.Id)).IoMapping.IecAddress);
						if (fixedIecAddress != null)
						{
							array = GetComponentsNormalized((IDirectVariable)(object)fixedIecAddress);
							bRelative = false;
							array = UpdateDataElement(ioProvider, null, (IDataElement)(object)val3, location, array, ref bRelative, ref nGranularity);
							flag = true;
						}
					}
					if (!flag)
					{
						array = UpdateDataElement(ioProvider, null, (IDataElement)(object)val3, location, array, ref bRelative, ref nGranularity);
					}
				}
				else
				{
					array = UpdateDataElement(ioProvider, null, (IDataElement)(object)val3, location, array, ref bRelative, ref nGranularity);
				}
			}
			IecAddressBase iecAddressBase = ((!bRelative) ? ((IecAddressBase)new FixedIecAddress()) : ((IecAddressBase)new RelativeIecAddress()));
			iecAddressBase.Location = location;
			iecAddressBase.Size = (DirectVariableSize)1;
			iecAddressBase.Components = GetComponentsNative(iecAddressBase.Size, array);
			ioProvider.SetNextUnusedAddress((IDirectVariable)(object)iecAddressBase);
			ioProvider.SetGranularity(location, nGranularity);
		}

		private int[] UpdateDataElement(IIoProvider ioProvider, IDataElement parent, IDataElement elem, DirectVariableLocation location, int[] offset, ref bool bRelative, ref int nGranularity)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Invalid comparison between Unknown and I4
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Invalid comparison between Unknown and I4
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			if (elem.HasSubElements)
			{
				bool flag = false;
				if (elem is IDataElement5)
				{
					flag = ((IDataElement5)((elem is IDataElement5) ? elem : null)).IsUnion;
				}
				IecAddressBase iecAddressBase = ((!bRelative) ? ((IecAddressBase)new FixedIecAddress()) : ((IecAddressBase)new RelativeIecAddress()));
				int nGranularity2;
				if (elem is IDataElement2 && ((IDataElement2)elem).HasBaseType)
				{
					nGranularity2 = GetByteSize(GetSize(elem.BaseType));
					offset = AlignComponents(nGranularity2, offset);
					nGranularity = Math.Max(nGranularity, nGranularity2);
				}
				else
				{
					int[] offset2 = new int[2];
					bool bRelative2 = bRelative;
					nGranularity2 = 0;
					if ((_iMinStructureGranularity > 0 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)16, (ushort)50) && (elem as Parameter)?.DataElementBase is DataElementStructType) || flag)
					{
						nGranularity2 = _iMinStructureGranularity;
					}
					for (int i = 0; i < ((ICollection)elem.SubElements).Count; i++)
					{
						offset2 = UpdateDataElement(ioProvider, elem, elem.SubElements[i], location, offset2, ref bRelative2, ref nGranularity2);
					}
					if (nGranularity2 == 0)
					{
						nGranularity2 = 1;
					}
					offset = AlignComponents(nGranularity2, offset);
					nGranularity = Math.Max(nGranularity, nGranularity2);
				}
				iecAddressBase.Size = GetSize(nGranularity2);
				iecAddressBase.Location = location;
				iecAddressBase.Components = GetComponentsNative(iecAddressBase.Size, offset);
				if (elem.IoMapping.AutomaticIecAddress)
				{
					elem.IoMapping.SetIntermediateAddress((IDirectVariable)(object)iecAddressBase);
				}
				else if (!string.IsNullOrEmpty(elem.IoMapping.IecAddress))
				{
					IecAddressBase addr = ParseIecAddress(elem.IoMapping.IecAddress);
					offset = GetComponentsNormalized((IDirectVariable)(object)addr);
					bRelative = false;
					if (this is ByteAddressFlatAdressAssignmentStrategy && (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)10) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)12, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)13, (ushort)0))))
					{
						offset = AlignComponents(nGranularity2, offset);
						IecAddressBase iecAddressBase2 = new FixedIecAddress();
						iecAddressBase2.Location = location;
						iecAddressBase2.Size = iecAddressBase.Size;
						iecAddressBase2.Components = GetComponentsNative(iecAddressBase2.Size, offset);
						elem.IoMapping.IecAddress=(iecAddressBase2.ToString());
					}
				}
				if (!flag)
				{
					for (int j = 0; j < ((ICollection)elem.SubElements).Count; j++)
					{
						offset = UpdateDataElement(ioProvider, elem, elem.SubElements[j], location, offset, ref bRelative, ref nGranularity);
					}
				}
				else
				{
					int[] array = new int[2];
					for (int k = 0; k < ((ICollection)elem.SubElements).Count; k++)
					{
						array = UpdateDataElement(ioProvider, elem, elem.SubElements[k], location, offset, ref bRelative, ref nGranularity);
					}
					offset = array;
				}
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)6, (ushort)0) || (elem is IDataElement2 && ((IDataElement2)elem).HasBaseType))
				{
					offset = AlignComponents(nGranularity2, offset);
				}
				return offset;
			}
			IecAddressBase iecAddressBase3 = null;
			if (!elem.IoMapping.AutomaticIecAddress)
			{
				iecAddressBase3 = ParseIecAddress(elem.IoMapping.IecAddress);
				bRelative = false;
			}
			if (iecAddressBase3 == null)
			{
				if (bRelative)
				{
					iecAddressBase3 = new RelativeIecAddress();
					iecAddressBase3.Location = location;
					iecAddressBase3.Size = GetSize(elem);
					if ((int)iecAddressBase3.Size == 1 && parent is Parameter && (parent as Parameter).DataElementBase is DataElementArrayType)
					{
						iecAddressBase3.Size = (DirectVariableSize)2;
					}
					offset = AlignComponents(GetByteSize(iecAddressBase3.Size), offset);
					iecAddressBase3.Components = GetComponentsNative(iecAddressBase3.Size, offset);
				}
				else
				{
					iecAddressBase3 = new FixedIecAddress();
					iecAddressBase3.Location = location;
					iecAddressBase3.Size = GetSize(elem);
					if ((int)iecAddressBase3.Size == 1 && parent is Parameter && (parent as Parameter).DataElementBase is DataElementArrayType)
					{
						iecAddressBase3.Size = (DirectVariableSize)2;
					}
					offset = AlignComponents(GetByteSize(iecAddressBase3.Size), offset);
					iecAddressBase3.Components = GetComponentsNative(iecAddressBase3.Size, offset);
				}
				elem.IoMapping.SetIntermediateAddress((IDirectVariable)(object)iecAddressBase3);
			}
			nGranularity = Math.Max(nGranularity, GetByteSize(iecAddressBase3.Size));
			return GetNextOffsetAfter((IDirectVariable)(object)iecAddressBase3);
		}

		private int[] GetNextOffsetAfter(IDirectVariable addr)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			int[] componentsNormalized = GetComponentsNormalized(addr);
			if ((int)addr.Size == 1)
			{
				componentsNormalized[1]++;
				componentsNormalized[0] += componentsNormalized[1] / 8;
				componentsNormalized[1] %= 8;
			}
			else
			{
				Debug.Assert(componentsNormalized[1] == 0);
				componentsNormalized[0] += GetByteSize(addr.Size);
			}
			return componentsNormalized;
		}

		private int[] AlignComponents(int nGranularity, int[] offset)
		{
			Debug.Assert(offset.Length == 2);
			int[] array = new int[2]
			{
				offset[0],
				offset[1]
			};
			if (nGranularity > 0)
			{
				if (array[1] != 0)
				{
					array[0]++;
					array[1] = 0;
				}
				array[0] = (array[0] + (nGranularity - 1)) / nGranularity * nGranularity;
			}
			return array;
		}

		public int GetByteSize(DirectVariableSize size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected I4, but got Unknown
			return ((int)size - 2) switch
			{
				0 => 1, 
				1 => 2, 
				2 => 4, 
				3 => 8, 
				_ => 0, 
			};
		}

		public virtual int[] GetComponentsNormalized(IDirectVariable addr)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			int[] components = addr.Components;
			Debug.Assert(components.Length != 0);
			Debug.Assert(components.Length <= 2);
			int[] array = new int[2];
			int num = GetByteSize(addr.Size);
			if (num == 0)
			{
				num = 1;
			}
			array[0] = components[0] * num;
			if (components.Length == 2)
			{
				array[1] = components[1] % 8;
				array[0] += components[1] / 8;
			}
			else
			{
				array[1] = 0;
			}
			return array;
		}

		public virtual int[] GetComponentsNative(DirectVariableSize size, int[] original)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(original.Length == 2, "Expected a normalized representation of the components");
			int byteSize = GetByteSize(size);
			int[] result;
			if (byteSize == 0)
			{
				result = new int[2]
				{
					original[0],
					original[1]
				};
			}
			else
			{
				Debug.Assert(original[0] % byteSize == 0);
				result = new int[1] { original[0] / byteSize };
			}
			return result;
		}

		private static DirectVariableSize GetSize(IDataElement elem)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Debug.Assert(!elem.HasSubElements);
			return GetSize(elem.BaseType);
		}

		internal static DirectVariableSize GetSize(string stType)
		{
			switch (stType.ToUpperInvariant())
			{
			case "BIT":
			case "BOOL":
			case "SAFEBOOL":
			case "SAFEBIT":
				return (DirectVariableSize)1;
			case "BYTE":
			case "USINT":
			case "SINT":
				return (DirectVariableSize)2;
			case "WORD":
			case "UINT":
			case "INT":
				return (DirectVariableSize)3;
			case "DWORD":
			case "UDINT":
			case "DINT":
			case "REAL":
				return (DirectVariableSize)4;
			case "LWORD":
			case "ULINT":
			case "LINT":
			case "LREAL":
				return (DirectVariableSize)5;
			case "DATE_AND_TIME":
			case "DT":
			case "TIME":
			case "TIME_OF_DAY":
			case "TOD":
			case "DATE":
			case "D":
				return (DirectVariableSize)4;
			case "LTIME":
				return (DirectVariableSize)5;
			default:
				if (!string.IsNullOrEmpty(stType))
				{
					ICompiledType val = Types.ParseType(stType);
					if (val != null && !(val is IUserdefType))
					{
						switch (val.Size((IScope)null))
						{
						case 1:
							return (DirectVariableSize)2;
						case 2:
							return (DirectVariableSize)3;
						case 4:
							return (DirectVariableSize)4;
						case 8:
							return (DirectVariableSize)5;
						}
					}
				}
				return (DirectVariableSize)2;
			}
		}

		private static DirectVariableSize GetSize(int nByteSize)
		{
			return (DirectVariableSize)(nByteSize switch
			{
				0 => 1, 
				1 => 2, 
				2 => 3, 
				4 => 4, 
				8 => 5, 
				_ => 1, 
			});
		}

		public static FixedIecAddress ParseIecAddress(string st)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (st == null)
			{
				return null;
			}
			IScanner val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(st, false, false, false, false);
			IToken val2 = default(IToken);
			if (val.Match((TokenType)7, true, out val2) <= 0)
			{
				return null;
			}
			DirectVariableLocation location = default(DirectVariableLocation);
			DirectVariableSize size = default(DirectVariableSize);
			int[] components = default(int[]);
			bool flag = default(bool);
			val.GetDirectVariable(val2, out location, out size, out components, out flag);
			return new FixedIecAddress
			{
				Location = location,
				Size = size,
				Components = components
			};
		}

		public IDirectVariable CalcBitAddress(IDirectVariable Addr, int iBitOffset)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			int[] componentsNormalized = GetComponentsNormalized(Addr);
			componentsNormalized[1] = iBitOffset;
			int[] componentsNative = GetComponentsNative((DirectVariableSize)1, componentsNormalized);
			return (IDirectVariable)(object)new FixedIecAddress
			{
				Location = Addr.Location,
				Size = (DirectVariableSize)1,
				Components = componentsNative
			};
		}

		internal ulong GetBitAddress(IDirectVariable Addr)
		{
			int[] componentsNormalized = GetComponentsNormalized(Addr);
			Debug.Assert(componentsNormalized.Length == 2);
			return (ulong)(componentsNormalized[0] * 8 + componentsNormalized[1]);
		}

		public bool UpdateAddresses(IList<IIoProvider> liIoProviders, bool bHasManualAddress, bool bLastHasConflict)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Invalid comparison between Unknown and I4
			bool flag = false;
			LList<ManualAddressData> val = new LList<ManualAddressData>();
			new LDictionary<ulong, ulong>();
			new LDictionary<ulong, ulong>();
			Hashtable htStartAddresses = new Hashtable();
			if (bLastHasConflict)
			{
				DeviceObjectHelper.HashIecAddresses.Clear();
				foreach (IIoProvider liIoProvider in liIoProviders)
				{
					liIoProvider.Strategy.UpdateAddresses(liIoProvider);
				}
			}
			if (bHasManualAddress)
			{
				foreach (IIoProvider liIoProvider2 in liIoProviders)
				{
					IParameter val2 = null;
					IParameter lastInput = null;
					IParameter val3 = null;
					IParameter lastOutput = null;
					bool bHasManualAddress2 = false;
					bool flag2 = false;
					IMetaObject metaObject = liIoProvider2.GetMetaObject();
					IObject obj = ((metaObject != null) ? metaObject.Object : null);
					IDeviceObject9 val4 = (IDeviceObject9)(object)((obj is IDeviceObject9) ? obj : null);
					if (val4 != null && val4.ShowParamsInDevDescOrder)
					{
						flag2 = true;
					}
					ArrayList arrayList = new ArrayList();
					arrayList.AddRange((ICollection)liIoProvider2.ParameterSet);
					if (flag2)
					{
						arrayList.Sort(ParameterByDevDescComparer.Instance);
					}
					foreach (IParameter item in arrayList)
					{
						IParameter val5 = item;
						if ((int)val5.ChannelType == 0)
						{
							continue;
						}
						if ((int)val5.ChannelType == 1)
						{
							if (val2 == null)
							{
								val2 = val5;
							}
							lastInput = val5;
						}
						else
						{
							if (val3 == null)
							{
								val3 = val5;
							}
							lastOutput = val5;
						}
						if (!((IDataElement)val5).IoMapping.AutomaticIecAddress)
						{
							bHasManualAddress2 = true;
						}
					}
					if (val2 != null || val3 != null)
					{
						ManualAddressData manualAddressData = new ManualAddressData(liIoProvider2, this, htStartAddresses, bHasManualAddress2, val2, lastInput, val3, lastOutput);
						val.Add(manualAddressData);
					}
				}
				for (int i = 0; i < val.Count; i++)
				{
					ManualAddressData manualAddressData2 = val[i];
					if (manualAddressData2.HasManualAddress)
					{
						continue;
					}
					for (int j = 0; j < val.Count; j++)
					{
						if (i != j)
						{
							flag |= manualAddressData2.AssignNewAddress(this, val, val[j]);
						}
					}
				}
			}
			return flag;
		}
	}
}
