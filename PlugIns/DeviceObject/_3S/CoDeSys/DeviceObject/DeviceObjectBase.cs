#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Device;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.LanguageModelUtilities;
using _3S.CoDeSys.Refactoring;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{421DE936-9332-4F73-A6E6-2E8D2FBA974B}")]
	[StorageVersion("3.3.0.0")]
	public class DeviceObjectBase : GenericObject2, IFindReplace2, IFindReplace, IGenerateableObject, ISupportRefactoring, IRefactoringCommandContext
	{
		protected int _nProjectHandle;

		private static readonly long _lAddressFlag = 2147483648L;

		internal static readonly long _lValueFlag = 1073741824L;

		public int ProjectHandle => _nProjectHandle;

		public virtual IConnectorCollection Connectors => null;

		public virtual IConnectorCollection ConnectorsWithGroups => null;

		public virtual IParameterSet DeviceParameterSet => null;

		private void FillOneSearchBlock(string stText, long lPosition, int nOffset, ref SearchableTextBlock searchblock, ArrayList alIds, ArrayList alOffsets)
		{
			if (searchblock.Text != null)
			{
				searchblock.Text += " ";
				nOffset = searchblock.Text.Length;
			}
			searchblock.Text += stText;
			alIds.Add(lPosition);
			alOffsets.Add(nOffset);
		}

		internal string GetFindReplaceString(string stValue, ref int nLength, bool bWord, short nPositionOffset)
		{
			if (nLength == 0)
			{
				if (bWord)
				{
					if (nPositionOffset < stValue.Length && nPositionOffset >= 0)
					{
						int num = TextUtilities.FindWordStart(stValue, (int)nPositionOffset, (string)null);
						int num2 = TextUtilities.FindWordEnd(stValue, (int)nPositionOffset, (string)null);
						nPositionOffset = (short)num;
						nLength = num2 - num;
						return stValue.Substring(nPositionOffset, nLength);
					}
					return string.Empty;
				}
				nLength = stValue.Length;
				return stValue;
			}
			nLength = Math.Min(nLength, stValue.Length + nPositionOffset);
			return stValue.Substring(nPositionOffset, nLength);
		}

		public string GetFindReplaceContentString(DataElementBase dataelement, ref long nPosition, ref int nLength, bool bWord)
		{
			string text = string.Empty;
			long num = default(long);
			short nPositionOffset = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out nPositionOffset);
			if (dataelement != null)
			{
				if (dataelement.HasSubElements)
				{
					foreach (DataElementBase item in (IEnumerable)dataelement.SubElements)
					{
						text = GetFindReplaceContentString(item, ref nPosition, ref nLength, bWord);
						if (text != string.Empty)
						{
							return text;
						}
					}
				}
				bool flag = false;
				bool flag2 = false;
				if ((num & _lAddressFlag) != 0L)
				{
					flag = true;
					num &= ~_lAddressFlag;
				}
				if ((num & _lValueFlag) != 0L)
				{
					flag2 = true;
					num &= ~_lValueFlag;
				}
				if (dataelement.EditorPositionId == num)
				{
					string stValue = string.Empty;
					if (flag2)
					{
						stValue = dataelement.Value;
					}
					else if (dataelement.IoMapping != null && dataelement.IoMapping.VariableMappings != null)
					{
						if (!flag)
						{
							foreach (VariableMapping item2 in (IEnumerable)dataelement.IoMapping.VariableMappings)
							{
								stValue = item2.Variable;
							}
						}
						else
						{
							stValue = dataelement.IoMapping.IecAddress;
						}
					}
					return GetFindReplaceString(stValue, ref nLength, bWord, nPositionOffset);
				}
			}
			return text;
		}

		private string GetFindReplaceContentString(IDriverInfo driverInfo, ref long nPosition, ref int nLength, bool bWord)
		{
			string empty = string.Empty;
			long num = default(long);
			short nPositionOffset = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out nPositionOffset);
			if (driverInfo != null)
			{
				foreach (RequiredLib item in (IEnumerable)driverInfo.RequiredLibs)
				{
					foreach (FBInstance item2 in (IEnumerable)item.FbInstances)
					{
						if (item2.LanguageModelPositionId == num)
						{
							string variable = item2.Instance.Variable;
							return GetFindReplaceString(variable, ref nLength, bWord, nPositionOffset);
						}
					}
				}
				return empty;
			}
			return empty;
		}

		public string GetFindReplaceContentString(ref long nPosition, ref int nLength, bool bWord)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected O, but got Unknown
			string empty = string.Empty;
			if (this is DeviceObject)
			{
				long num = default(long);
				short nPositionOffset = default(short);
				PositionHelper.SplitPosition(nPosition, out num, out nPositionOffset);
				if (num == 0L)
				{
					string name = (this as DeviceObject).MetaObject.Name;
					empty = GetFindReplaceString(name, ref nLength, bWord, nPositionOffset);
					if (empty != string.Empty)
					{
						return empty;
					}
				}
				empty = GetFindReplaceContentString((this as DeviceObject).DriverInfo, ref nPosition, ref nLength, bWord);
				if (empty != string.Empty)
				{
					return empty;
				}
			}
			foreach (Parameter item in (IEnumerable)DeviceParameterSet)
			{
				empty = GetFindReplaceContentString(item.DataElementBase, ref nPosition, ref nLength, bWord);
				if (empty != string.Empty)
				{
					return empty;
				}
			}
			foreach (IConnector item2 in (IEnumerable)Connectors)
			{
				IConnector val = item2;
				empty = GetFindReplaceContentString((val as Connector).DriverInfo, ref nPosition, ref nLength, bWord);
				if (empty != string.Empty)
				{
					return empty;
				}
				foreach (Parameter item3 in (IEnumerable)val.HostParameterSet)
				{
					empty = GetFindReplaceContentString(item3.DataElementBase, ref nPosition, ref nLength, bWord);
					if (empty != string.Empty)
					{
						return empty;
					}
				}
			}
			return string.Empty;
		}

		private void FillSearchTextBlock(Parameter parameter, ArrayList alSearchBlocks)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			if (parameter != null)
			{
				FillSearchTextBlock(parameter.ChannelType, parameter.DataElementBase, alSearchBlocks);
			}
		}

		private void FillSearchTextBlock(ChannelType channeltype, DataElementBase dataelement, ArrayList alSearchBlocks)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Invalid comparison between Unknown and I4
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Invalid comparison between Unknown and I4
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Invalid comparison between Unknown and I4
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Invalid comparison between Unknown and I4
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Invalid comparison between Unknown and I4
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			if (dataelement == null)
			{
				return;
			}
			foreach (DataElementBase item in (IEnumerable)dataelement.SubElements)
			{
				FillSearchTextBlock(channeltype, item, alSearchBlocks);
			}
			SearchableTextBlock searchblock = default(SearchableTextBlock);
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			if ((int)channeltype == 1 || (int)channeltype == 2 || (int)channeltype == 3)
			{
				FillOneSearchBlock(dataelement.IoMapping.IecAddress, dataelement.EditorPositionId | _lAddressFlag, 0, ref searchblock, arrayList, arrayList2);
				if (dataelement.IoMapping != null && dataelement.IoMapping.VariableMappings != null)
				{
					foreach (VariableMapping item2 in (IEnumerable)dataelement.IoMapping.VariableMappings)
					{
						FillOneSearchBlock(item2.Variable, dataelement.EditorPositionId, 0, ref searchblock, arrayList, arrayList2);
					}
				}
			}
			if ((int)channeltype == 0 && ((int)dataelement.GetAccessRight(bOnline: false) == 3 || (int)dataelement.GetAccessRight(bOnline: false) == 1) && dataelement.HasBaseType)
			{
				FillOneSearchBlock(dataelement.Value, dataelement.EditorPositionId | _lValueFlag, 0, ref searchblock, arrayList, arrayList2);
			}
			if (searchblock.Text != null)
			{
				searchblock.PositionIds = new long[arrayList.Count];
				arrayList.CopyTo(searchblock.PositionIds);
				searchblock.PositionOffsets = new int[arrayList2.Count];
				arrayList2.CopyTo(searchblock.PositionOffsets);
				alSearchBlocks.Add(searchblock);
			}
		}

		private void FillSearchTextBlock(IDriverInfo driverInfo, ArrayList alSearchBlocks)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (driverInfo == null)
			{
				return;
			}
			foreach (RequiredLib item in (IEnumerable)driverInfo.RequiredLibs)
			{
				foreach (FBInstance item2 in (IEnumerable)item.FbInstances)
				{
					SearchableTextBlock searchblock = default(SearchableTextBlock);
					ArrayList arrayList = new ArrayList();
					ArrayList arrayList2 = new ArrayList();
					FillOneSearchBlock(item2.Instance.Variable, item2.LanguageModelPositionId, 0, ref searchblock, arrayList, arrayList2);
					if (searchblock.Text != null)
					{
						searchblock.PositionIds = new long[arrayList.Count];
						arrayList.CopyTo(searchblock.PositionIds);
						searchblock.PositionOffsets = new int[arrayList2.Count];
						arrayList2.CopyTo(searchblock.PositionOffsets);
						alSearchBlocks.Add(searchblock);
					}
				}
			}
		}

		public SearchableTextBlock[] GetSearchableTextBlocks()
		{
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Expected O, but got Unknown
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Expected O, but got Unknown
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Expected O, but got Unknown
			foreach (IDeviceObjectFindReplaceFactory deviceObjectFindReplaceFactory in APEnvironment.DeviceObjectFindReplaceFactories)
			{
				if (deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2)
				{
					IMetaObject val = null;
					if (this is DeviceObject)
					{
						val = (this as DeviceObject).MetaObject;
					}
					if (this is SlotDeviceObject)
					{
						val = (this as SlotDeviceObject).MetaObject;
					}
					if (val != null && deviceObjectFindReplaceFactory.GetMatch(val))
					{
						return ((IDeviceObjectFindReplaceFactory2)((deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2) ? deviceObjectFindReplaceFactory : null)).GetSearchableTextBlocks(val);
					}
				}
				else if ((this is DeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as DeviceObject).MetaObject)) || (this is SlotDeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as SlotDeviceObject).MetaObject)))
				{
					return ((IFindReplace)deviceObjectFindReplaceFactory).GetSearchableTextBlocks();
				}
			}
			ArrayList arrayList = new ArrayList();
			string text = string.Empty;
			if (this is DeviceObject)
			{
				text = (this as DeviceObject).MetaObject.Name;
			}
			if (this is SlotDeviceObject)
			{
				text = (this as SlotDeviceObject).MetaObject.Name;
			}
			if (!string.IsNullOrEmpty(text))
			{
				SearchableTextBlock searchblock = default(SearchableTextBlock);
				ArrayList arrayList2 = new ArrayList();
				ArrayList arrayList3 = new ArrayList();
				FillOneSearchBlock(text, 0L, 0, ref searchblock, arrayList2, arrayList3);
				if (searchblock.Text != null)
				{
					searchblock.PositionIds = new long[arrayList2.Count];
					arrayList2.CopyTo(searchblock.PositionIds);
					searchblock.PositionOffsets = new int[arrayList3.Count];
					arrayList3.CopyTo(searchblock.PositionOffsets);
					arrayList.Add(searchblock);
				}
			}
			if (this is DeviceObject)
			{
				FillSearchTextBlock((this as DeviceObject).DriverInfo, arrayList);
			}
			foreach (IParameter item in (IEnumerable)DeviceParameterSet)
			{
				IParameter val2 = item;
				FillSearchTextBlock(val2 as Parameter, arrayList);
			}
			foreach (IConnector item2 in (IEnumerable)Connectors)
			{
				IConnector val3 = item2;
				FillSearchTextBlock(((IConnector2)((val3 is IConnector2) ? val3 : null)).DriverInfo, arrayList);
				foreach (IParameter item3 in (IEnumerable)val3.HostParameterSet)
				{
					IParameter val4 = item3;
					FillSearchTextBlock(val4 as Parameter, arrayList);
				}
			}
			if (arrayList.Count == 0)
			{
				return null;
			}
			SearchableTextBlock[] array = (SearchableTextBlock[])(object)new SearchableTextBlock[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private bool CheckAddress(IDataElement dataElement, string st)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			if (st == null)
			{
				return false;
			}
			IScanner val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(st, false, false, false, false);
			IToken val2 = default(IToken);
			if (val.Match((TokenType)7, true, out val2) <= 0)
			{
				return false;
			}
			DirectVariableLocation val3 = default(DirectVariableLocation);
			DirectVariableSize val4 = default(DirectVariableSize);
			int[] array = default(int[]);
			bool flag = default(bool);
			val.GetDirectVariable(val2, out val3, out val4, out array, out flag);
			if (flag)
			{
				return false;
			}
			val = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(dataElement.IoMapping.IecAddress, false, false, false, false);
			if (val.Match((TokenType)7, true, out val2) <= 0)
			{
				return false;
			}
			DirectVariableLocation val5 = default(DirectVariableLocation);
			DirectVariableSize val6 = default(DirectVariableSize);
			int[] array2 = default(int[]);
			val.GetDirectVariable(val2, out val5, out val6, out array2, out flag);
			if (val5 != val3 || val6 != val4 || array2.Length != array.Length)
			{
				return false;
			}
			return true;
		}

		public bool ReplaceParameterValue(IDataElement2 dataelement, long nPosition, int nLength, string stReplacement)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			if (((IDataElement)dataelement).HasSubElements)
			{
				foreach (IDataElement2 item in (IEnumerable)((IDataElement)dataelement).SubElements)
				{
					IDataElement2 dataelement2 = item;
					if (ReplaceParameterValue(dataelement2, nPosition, nLength, stReplacement))
					{
						return true;
					}
				}
			}
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			bool flag = false;
			bool flag2 = false;
			if ((num & _lAddressFlag) != 0L)
			{
				flag = true;
				num &= ~_lAddressFlag;
			}
			if ((num & _lValueFlag) != 0L)
			{
				flag2 = true;
				num &= ~_lValueFlag;
			}
			if (dataelement.EditorPositionId == num)
			{
				if (flag2)
				{
					if (dataelement.HasBaseType)
					{
						string value = ((IDataElement)dataelement).Value;
						if (value.Length >= num2 + nLength)
						{
							value = value.Remove(num2, nLength);
							value = value.Insert(num2, stReplacement);
							((IDataElement)dataelement).Value=(value);
						}
						return true;
					}
				}
				else if (!flag)
				{
					if (((IDataElement)dataelement).IoMapping != null && ((IDataElement)dataelement).IoMapping.VariableMappings != null)
					{
						{
							IEnumerator enumerator = ((IEnumerable)((IDataElement)dataelement).IoMapping.VariableMappings).GetEnumerator();
							try
							{
								if (enumerator.MoveNext())
								{
									VariableMapping variableMapping = (VariableMapping)enumerator.Current;
									string variable = variableMapping.Variable;
									if (variable.Length >= num2 + nLength)
									{
										variable = variable.Remove(num2, nLength);
										variable = (variableMapping.Variable = variable.Insert(num2, stReplacement));
									}
									return true;
								}
							}
							finally
							{
								IDisposable disposable = enumerator as IDisposable;
								if (disposable != null)
								{
									disposable.Dispose();
								}
							}
						}
					}
				}
				else if (!string.IsNullOrEmpty(stReplacement))
				{
					string text2 = ((IDataElement)dataelement).IoMapping.IecAddress;
					if (text2.Length >= num2 + nLength)
					{
						text2 = text2.Remove(num2, nLength);
						text2 = text2.Insert(num2, stReplacement);
					}
					if (CheckAddress((IDataElement)(object)dataelement, text2))
					{
						((IDataElement)dataelement).IoMapping.AutomaticIecAddress=(false);
						((IDataElement)dataelement).IoMapping.IecAddress=(text2);
					}
				}
				else
				{
					((IDataElement)dataelement).IoMapping.AutomaticIecAddress=(true);
				}
			}
			return false;
		}

		public void Replace(long nPosition, int nLength, string stReplacement)
		{
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			foreach (IDeviceObjectFindReplaceFactory deviceObjectFindReplaceFactory in APEnvironment.DeviceObjectFindReplaceFactories)
			{
				if (deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2)
				{
					IMetaObject val = null;
					if (this is DeviceObject)
					{
						val = (this as DeviceObject).MetaObject;
					}
					if (this is SlotDeviceObject)
					{
						val = (this as SlotDeviceObject).MetaObject;
					}
					if (val != null && deviceObjectFindReplaceFactory.GetMatch(val))
					{
						((IDeviceObjectFindReplaceFactory2)((deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2) ? deviceObjectFindReplaceFactory : null)).Replace(nPosition, nLength, stReplacement, val);
						return;
					}
				}
				else if ((this is DeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as DeviceObject).MetaObject)) || (this is SlotDeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as SlotDeviceObject).MetaObject)))
				{
					((IFindReplace)deviceObjectFindReplaceFactory).Replace(nPosition, nLength, stReplacement);
					return;
				}
			}
			foreach (Parameter item in (IEnumerable)DeviceParameterSet)
			{
				if (ReplaceParameterValue((IDataElement2)(object)item, nPosition, nLength, stReplacement))
				{
					return;
				}
			}
			foreach (IConnector item2 in (IEnumerable)Connectors)
			{
				foreach (Parameter item3 in (IEnumerable)item2.HostParameterSet)
				{
					if (ReplaceParameterValue((IDataElement2)(object)item3, nPosition, nLength, stReplacement))
					{
						return;
					}
				}
			}
		}

		public SearchableTextBlock[] GetSearchableTextBlocks(long nPosition, int nLength)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			SearchableTextBlock[] result = null;
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			ArrayList arrayList = new ArrayList();
			foreach (IParameter item in (IEnumerable)DeviceParameterSet)
			{
				IParameter val = item;
				FillSearchTextBlock(val as Parameter, arrayList);
			}
			foreach (IConnector item2 in (IEnumerable)Connectors)
			{
				foreach (IParameter item3 in (IEnumerable)item2.HostParameterSet)
				{
					IParameter val2 = item3;
					FillSearchTextBlock(val2 as Parameter, arrayList);
				}
			}
			if (arrayList.Count > 0)
			{
				foreach (SearchableTextBlock item4 in arrayList)
				{
					for (int i = 0; i < item4.PositionIds.Length; i++)
					{
						if (item4.PositionIds[i] == num)
						{
							result = (SearchableTextBlock[])(object)new SearchableTextBlock[1] { item4 };
						}
					}
				}
				return result;
			}
			return result;
		}

		public void PlugDevice(Guid guidSlot, IDeviceIdentification devId, string stName)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidSlot);
			Debug.Assert(objectToRead != null);
			if (!(objectToRead.Object is SlotDeviceObject))
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)8, "The argument guidSlot is not a slot device");
			}
			DeviceCommandHelper.PlugDeviceIntoSlot(_nProjectHandle, guidSlot, devId, stName);
		}

		public void UnplugDeviceFromSlot(Guid guidSlot)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(_nProjectHandle, guidSlot);
			Debug.Assert(objectToRead != null);
			if (!(objectToRead.Object is SlotDeviceObject))
			{
				throw new DeviceObjectException((DeviceObjectExeptionReason)8, "The argument guidSlot is not a slot device");
			}
			DeviceCommandHelper.UnplugDeviceFromSlot(_nProjectHandle, guidSlot, bCheckBeforeUnplug: true);
		}

		public bool Supports(Type operation)
		{
			foreach (IDeviceObjectFindReplaceFactory deviceObjectFindReplaceFactory in APEnvironment.DeviceObjectFindReplaceFactories)
			{
				if (deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2)
				{
					IMetaObject val = null;
					if (this is DeviceObject)
					{
						val = (this as DeviceObject).MetaObject;
					}
					if (this is SlotDeviceObject)
					{
						val = (this as SlotDeviceObject).MetaObject;
					}
					if (val != null && deviceObjectFindReplaceFactory.GetMatch(val))
					{
						return ((IDeviceObjectFindReplaceFactory2)((deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2) ? deviceObjectFindReplaceFactory : null)).Supports(operation, val);
					}
				}
				else if ((this is DeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as DeviceObject).MetaObject)) || (this is SlotDeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as SlotDeviceObject).MetaObject)))
				{
					return ((ISupportRefactoring)deviceObjectFindReplaceFactory).Supports(operation);
				}
			}
			if (typeof(IRefactoringRenameVariableOperation).IsAssignableFrom(operation))
			{
				return true;
			}
			if (typeof(IRefactoringRenameSignatureOperation).IsAssignableFrom(operation))
			{
				return true;
			}
			if (typeof(IRefactoringRenameLanguageModelProvidingObjectOperation).IsAssignableFrom(operation))
			{
				return true;
			}
			return false;
		}

		internal bool RefactorParameterValue(IDataElement2 dataelement, long nPosition, string stReplacement, string stOldName)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (((IDataElement)dataelement).HasSubElements)
			{
				foreach (IDataElement2 item in (IEnumerable)((IDataElement)dataelement).SubElements)
				{
					IDataElement2 dataelement2 = item;
					if (RefactorParameterValue(dataelement2, nPosition, stReplacement, stOldName))
					{
						return true;
					}
				}
			}
			long num = default(long);
			short num2 = default(short);
			PositionHelper.SplitPosition(nPosition, out num, out num2);
			bool flag = false;
			if ((num & _lValueFlag) != 0L)
			{
				flag = true;
				num &= ~_lValueFlag;
			}
			if (dataelement.LanguageModelPositionId == num && num2 == 0)
			{
				if (flag)
				{
					if (dataelement.HasBaseType)
					{
						((IDataElement)dataelement).Value=(Regex.Replace(((IDataElement)dataelement).Value, stOldName, stReplacement, RegexOptions.IgnoreCase));
						return true;
					}
				}
				else if (((IDataElement)dataelement).IoMapping != null && ((IDataElement)dataelement).IoMapping.VariableMappings != null)
				{
					{
						IEnumerator enumerator = ((IEnumerable)((IDataElement)dataelement).IoMapping.VariableMappings).GetEnumerator();
						try
						{
							if (enumerator.MoveNext())
							{
								VariableMapping variableMapping = (VariableMapping)enumerator.Current;
								string variable = variableMapping.Variable;
								if (variable.Contains("."))
								{
									string[] array = variable.Split('.');
									bool flag2 = false;
									for (int i = 0; i < array.Length; i++)
									{
										if (string.Compare(array[i], stOldName, StringComparison.InvariantCultureIgnoreCase) == 0)
										{
											array[i] = stReplacement;
											flag2 = true;
										}
									}
									if (flag2)
									{
										StringBuilder stringBuilder = new StringBuilder();
										string[] array2 = array;
										foreach (string value in array2)
										{
											if (stringBuilder.Length > 0)
											{
												stringBuilder.Append(".");
											}
											stringBuilder.Append(value);
										}
										variableMapping.Variable = stringBuilder.ToString();
									}
								}
								else
								{
									variableMapping.Variable = Regex.Replace(variable, stOldName, stReplacement, RegexOptions.IgnoreCase);
								}
								return true;
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			return false;
		}

		public bool Perform(RefactoringExecutionPerformerEventArgs refactoringEvent, ICrossRefCollection crossReferences)
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Expected O, but got Unknown
			foreach (IDeviceObjectFindReplaceFactory deviceObjectFindReplaceFactory in APEnvironment.DeviceObjectFindReplaceFactories)
			{
				if (deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2)
				{
					IMetaObject val = null;
					if (this is DeviceObject)
					{
						val = (this as DeviceObject).MetaObject;
					}
					if (this is SlotDeviceObject)
					{
						val = (this as SlotDeviceObject).MetaObject;
					}
					if (val != null && deviceObjectFindReplaceFactory.GetMatch(val))
					{
						return ((IDeviceObjectFindReplaceFactory2)((deviceObjectFindReplaceFactory is IDeviceObjectFindReplaceFactory2) ? deviceObjectFindReplaceFactory : null)).Perform(refactoringEvent, crossReferences, val);
					}
				}
				else if ((this is DeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as DeviceObject).MetaObject)) || (this is SlotDeviceObject && deviceObjectFindReplaceFactory.GetMatch((this as SlotDeviceObject).MetaObject)))
				{
					return ((ISupportRefactoring)deviceObjectFindReplaceFactory).Perform(refactoringEvent, crossReferences);
				}
			}
			bool result = false;
			LList<long> val2 = new LList<long>();
			foreach (ICrossReferenceNode item in (IEnumerable<ICrossReferenceNode>)crossReferences)
			{
				if (val2.Contains(((IMessage)item).Position))
				{
					continue;
				}
				bool flag = false;
				IRefactoringOperation obj = crossReferences[item];
				IRefactoringRenameOperation val3 = (IRefactoringRenameOperation)(object)((obj is IRefactoringRenameOperation) ? obj : null);
				if (val3 == null)
				{
					continue;
				}
				string empty = string.Empty;
				empty = ((!(item is ICrossReferenceNode2) || !((ICrossReferenceNode2)item).Shadowed) ? val3.NewName : ((ICrossReferenceNode2)((item is ICrossReferenceNode2) ? item : null)).ShadowedNewText);
				if (this is DeviceObject && RefactorFBInstance((this as DeviceObject).DriverInfo, ((IMessage)item).Position, empty, val3.OldName))
				{
					val2.Add(((IMessage)item).Position);
					result = true;
					flag = true;
					return result;
				}
				if (!flag)
				{
					foreach (Parameter item2 in (IEnumerable)DeviceParameterSet)
					{
						if (RefactorParameterValue((IDataElement2)(object)item2, ((IMessage)item).Position, empty, val3.OldName))
						{
							val2.Add(((IMessage)item).Position);
							result = true;
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					continue;
				}
				foreach (IConnector item3 in (IEnumerable)Connectors)
				{
					IConnector val4 = item3;
					if (RefactorFBInstance(((IConnector2)((val4 is IConnector2) ? val4 : null)).DriverInfo, ((IMessage)item).Position, empty, val3.OldName))
					{
						val2.Add(((IMessage)item).Position);
						result = true;
						flag = true;
						break;
					}
					if (flag)
					{
						continue;
					}
					foreach (Parameter item4 in (IEnumerable)val4.HostParameterSet)
					{
						if (RefactorParameterValue((IDataElement2)(object)item4, ((IMessage)item).Position, empty, val3.OldName))
						{
							val2.Add(((IMessage)item).Position);
							result = true;
							flag = true;
							break;
						}
					}
				}
			}
			return result;
		}

		internal bool RefactorFBInstance(IDriverInfo driverInfo, long nPosition, string stReplacement, string stOldName)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			foreach (IRequiredLib item in (IEnumerable)driverInfo.RequiredLibs)
			{
				foreach (FBInstance item2 in ((IEnumerable)item.FbInstances).OfType<FBInstance>())
				{
					if (item2.LanguageModelPositionId == nPosition && item2.FbName == stOldName)
					{
						item2.SetFbType(Regex.Replace(item2.FbName, stOldName, stReplacement, RegexOptions.IgnoreCase));
						return true;
					}
					if (item2.LanguageModelPositionId == nPosition && item2.FbNameDiag == stOldName)
					{
						item2.FbNameDiag = Regex.Replace(item2.FbNameDiag, stOldName, stReplacement, RegexOptions.IgnoreCase);
						return true;
					}
					if (item2.LanguageModelPositionId == nPosition && item2.Instance.Variable == stOldName)
					{
						item2.Instance.Variable=(Regex.Replace(item2.Instance.Variable, stOldName, stReplacement, RegexOptions.IgnoreCase));
						return true;
					}
				}
			}
			return false;
		}

		private static bool HasRefactoringFbInstances(IDriverInfo driverInfo)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (IRequiredLib item in (IEnumerable)driverInfo.RequiredLibs)
			{
				foreach (FBInstance item2 in ((IEnumerable)item.FbInstances).OfType<FBInstance>())
				{
					if (item2.BaseName.Contains("$(DeviceName)"))
					{
						return true;
					}
				}
			}
			return false;
		}

		public RefactoringContextType GetRefactoringContext(out Guid signatureGuid, out string stDescription)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			IMetaObject val = null;
			bool flag = false;
			DeviceObject deviceObject = this as DeviceObject;
			if (deviceObject != null)
			{
				val = deviceObject.MetaObject;
				Debug.Assert(val != null);
				flag |= HasRefactoringFbInstances(deviceObject.DriverInfo);
			}
			else
			{
				SlotDeviceObject slotDeviceObject = this as SlotDeviceObject;
				if (slotDeviceObject != null)
				{
					val = slotDeviceObject.GetMetaObject();
					Debug.Assert(val != null);
					flag |= HasRefactoringFbInstances(slotDeviceObject.DriverInfo);
				}
			}
			if (!flag && val != null && Connectors != null)
			{
				foreach (IConnector2 item in (IEnumerable)Connectors)
				{
					IConnector2 val2 = item;
					flag |= HasRefactoringFbInstances(val2.DriverInfo);
					if (flag)
					{
						break;
					}
				}
			}
			if (flag)
			{
				stDescription = val.Name;
				signatureGuid = val.ObjectGuid;
				return (RefactoringContextType)8;
			}
			signatureGuid = Guid.Empty;
			stDescription = string.Empty;
			return (RefactoringContextType)0;
		}

		public DeviceObjectBase()
			: base()
		{
		}
	}
}
