using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class TaskMapList
	{
		private ArrayList _arErrorMsg = new ArrayList();

		private LDictionary<Guid, DeviceObject.ApplicationUsage> _dictAppUsage;

		private Guid _applicationGuid = Guid.Empty;

		private int _nCountWarnings;

		private int _nCountErrors;

		private static int MAX_ERRORS = 510;

		private int MAX_WARNINGS;

		private LList<string> _liMapToExisting = new LList<string>();

		private Dictionary<int, List<ITaskMapping>> _dictModuleIndex = new Dictionary<int, List<ITaskMapping>>();

		private Dictionary<ITaskMappingInfo, List<ITaskMapping>> _dictMappings = new Dictionary<ITaskMappingInfo, List<ITaskMapping>>();

		public Dictionary<ITaskMappingInfo, List<ITaskMapping>> Mappings => _dictMappings;

		public LList<string> MapToExisting => _liMapToExisting;

		public TaskMapList(LDictionary<Guid, DeviceObject.ApplicationUsage> dictAppUsage, Guid applicationGuid)
		{
			MAX_WARNINGS = CompileOptions.MaxCompilerWarnings + 10;
			_dictAppUsage = dictAppUsage;
			_applicationGuid = applicationGuid;
		}

		public void AddErrorMsg(string stFormat, DirectVarCrossRef cref, ICompileContext comcon, bool bWarning)
		{
			Guid messageGuid = Guid.Empty;
			List<long> list = new List<long>();
			if (cref != null && cref.CrossRef != null)
			{
				ISignature signatureById = comcon.GetSignatureById(cref.CrossRef.CodeId);
				if (signatureById != null)
				{
					messageGuid = signatureById.MessageGuid;
				}
				IAddressCodePosition[] positions = cref.CrossRef.Positions;
				foreach (IAddressCodePosition val in positions)
				{
					list.Add(val.EditorPosition);
				}
			}
			AddErrorMsg(stFormat, messageGuid, list, ((object)cref.DirectVariable).ToString(), bWarning);
		}

		public void AddErrorMsg(string stFormat, VariableCrossRef cref, ICompileContext comcon, bool bWarning)
		{
			Guid messageGuid = Guid.Empty;
			List<long> list = new List<long>();
			if (cref != null && cref.CrossReference != null)
			{
				ISignature signatureById = comcon.GetSignatureById(cref.CrossReference.CodeId);
				if (signatureById != null)
				{
					messageGuid = signatureById.MessageGuid;
				}
				if (cref.Variable != null && comcon is ICompileContext10)
				{
					int num = -1;
					num = ((cref.SignatureId != -1) ? cref.SignatureId : signatureById.Id);
					foreach (ICodePosition item in ((ICompileContext10)((comcon is ICompileContext10) ? comcon : null)).GetReferencePositionsOfPOUEx(cref.CrossReference.CodeId, num, cref.Variable.Id))
					{
						list.Add(item.EditorPosition);
					}
				}
			}
			AddErrorMsg(stFormat, messageGuid, list, cref.VariableDeclaration.Variable.Variable, bWarning);
		}

		public void AddErrorMsg(string stFormat, Guid MessageGuid, List<long> liPositions, string stVariable, bool bWarning)
		{
			if (bWarning)
			{
				_nCountWarnings++;
				if (_nCountWarnings > MAX_WARNINGS)
				{
					return;
				}
			}
			else
			{
				_nCountErrors++;
				if (_nCountErrors > MAX_ERRORS)
				{
					return;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (MessageGuid != Guid.Empty)
			{
				stringBuilder.AppendFormat("{{messageguid '{0}'}}\n", MessageGuid);
			}
			foreach (long liPosition in liPositions)
			{
				if (liPosition != -1)
				{
					stringBuilder.AppendFormat("{{p {0} }}\n", liPosition);
					break;
				}
			}
			if (bWarning)
			{
				stringBuilder.Append("{warning '");
			}
			else
			{
				stringBuilder.Append("{error '");
			}
			stringBuilder.AppendFormat(stFormat, stVariable);
			stringBuilder.AppendLine("'}");
			string text = stringBuilder.ToString();
			if (!_arErrorMsg.Contains(text))
			{
				_arErrorMsg.Add(text);
			}
		}

		public bool Add(int iTaskNbr, ConnectorMap connectorMap, uint uiParamId, string stAddr, int nParamBitOffset, int nAddrBitOffset, int nBitSize, long lBitOffset, string stBaseType, long lStartbitOffset, DoubleAddressTaskChecker checker, IDataElement dataElement, bool bNoOutputCheck)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			bool result = true;
			List<ITaskMapping> mapping = GetMapping(connectorMap.MappingInfo);
			if ((connectorMap.ShowMultipleMappingsAsError || stAddr.StartsWith("%Q")) && !bNoOutputCheck)
			{
				long num = lStartbitOffset;
				if (!stAddr.StartsWith("%") || stAddr.Substring(2, 1) != "X")
				{
					num += nAddrBitOffset;
				}
				if (num % 8 == 0L && nBitSize % 8 == 0)
				{
					if (_dictAppUsage.Values.Count > 1)
					{
						foreach (KeyValuePair<Guid, DeviceObject.ApplicationUsage> keyValuePair in this._dictAppUsage)
						{
							if (keyValuePair.Key != this._applicationGuid)
							{
								for (long num2 = num; num2 < num + (long)nBitSize; num2 += 8L)
								{
									if (keyValuePair.Value.Checker.CheckAddressForOtherTasksByte(num2, -1))
									{
										result = false;
									}
								}
							}
						}
					}
					for (long num3 = num; num3 < num + nBitSize; num3 += 8)
					{
						if (checker.CheckAddressForOtherTasksByte(num3, iTaskNbr))
						{
							result = false;
						}
						checker.SetByte(num3, iTaskNbr);
					}
				}
				else
				{
					if (_dictAppUsage.Values.Count > 1)
					{
						foreach (KeyValuePair<Guid, DeviceObject.ApplicationUsage> keyValuePair2 in this._dictAppUsage)
						{
							if (keyValuePair2.Key != this._applicationGuid)
							{
								for (long num4 = num; num4 < num + (long)nBitSize; num4 += 1L)
								{
									if (keyValuePair2.Value.Checker.CheckAddressForOtherTasksBit(num4, -1))
									{
										result = false;
									}
								}
							}
						}
					}
					for (long num5 = num; num5 < num + nBitSize; num5++)
					{
						if (checker.CheckAddressForOtherTasksBit(num5, iTaskNbr))
						{
							result = false;
						}
						checker.SetBit(num5, iTaskNbr);
					}
				}
			}
			Mapping item = new Mapping(connectorMap.ParamSet as ParameterSet, uiParamId, stAddr, nParamBitOffset, nAddrBitOffset, nBitSize, lBitOffset, stBaseType, lStartbitOffset, dataElement);
			mapping.Add((ITaskMapping)(object)item);
			return result;
		}

		public bool Add(int iTaskNbr, VariableCrossRef cref, ICompileContext comcon, VariableDeclaration vd, int nParamBitOffset, int nAddrBitOffset, int nBitSize, string stBaseType, DoubleAddressTaskChecker checker, IDataElement dataElement)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Invalid comparison between Unknown and I4
			bool flag = true;
			List<ITaskMapping> mapping = GetMapping(vd.Connector.MappingInfo);
			long num = 0L;
			if ((vd.Channel.IecAddress != null && (int)vd.Channel.IecAddress.Location == 2) || vd.Connector.ShowMultipleMappingsAsError)
			{
				bool flag2 = default(bool);
				IDataLocation val = comcon.LocateAddress(out flag2, vd.Channel.IecAddress);
				if (val == null)
				{
					return false;
				}
				num = new BitDataLocation(val).BitOffset;
				if ((int)vd.Channel.IecAddress.Size != 1)
				{
					num += nAddrBitOffset;
				}
				if (vd.Variable.IoProvider != null && vd.Variable.IoProvider.TypeId != 152)
				{
					bool flag3 = false;
					if (comcon != null && cref != null && LanguageModelHelper.CheckSkipWarning(comcon, cref))
					{
						flag3 = true;
					}
					if (!flag3)
					{
						bool flag4 = false;
						if (num % 8 == 0L && nBitSize % 8 == 0)
						{
							for (long num2 = num; num2 < num + nBitSize; num2 += 8)
							{
								if (checker.CheckAddressForTaskByte(num2, -1))
								{
									flag = false;
								}
								else if (checker.CheckAddressForOtherTasksByte(num2, -1))
								{
									flag4 = true;
									flag = false;
									break;
								}
								checker.SetBit(num2, -1);
							}
						}
						else
						{
							for (long num3 = num; num3 < num + nBitSize; num3++)
							{
								if (checker.CheckAddressForTaskBit(num3, -1))
								{
									flag = false;
								}
								else if (checker.CheckAddressForOtherTasksBit(num3, -1))
								{
									flag4 = true;
									flag = false;
									break;
								}
								checker.SetBit(num3, -1);
							}
						}
						if (!flag && !vd.Variable.CreateVariable && flag4)
						{
							string stFormat = string.Format(Strings.ErrorChannelAlreadyExistingVariable, vd.Channel.IecAddress);
							AddErrorMsg(stFormat, cref, comcon, bWarning: false);
							return true;
						}
					}
				}
			}
			ParameterSet paramSet = null;
			if (vd.Variable.IoProvider != null)
			{
				paramSet = vd.Variable.IoProvider.ParameterSet as ParameterSet;
			}
			Mapping item = new Mapping(paramSet, (uint)vd.Channel.ParameterId, vd.Variable, nParamBitOffset, nAddrBitOffset, nBitSize, stBaseType, num, dataElement);
			mapping.Add((ITaskMapping)(object)item);
			return flag;
		}

		public void AddToLanguageModel(string stAppName, int nTaskId, int nType, ref bool bFirstTask, StringBuilder sbTaskMap, StringBuilder sbModuleMap, StringBuilder sbChannelMap, StringBuilder sbErrorMap, StringBuilder sbForceVariables, ref int nForces, ICompileContext comcon, LateLanguageModel lmLate, bool bMappingForLogical, bool bIsProxyMap, int nProxyModuleIndex, StringBuilder sbParamValue)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = _dictMappings.Count;
			string text = ((nType != 1) ? "outputs" : "inputs");
			if (bFirstTask)
			{
				bFirstTask = false;
			}
			else
			{
				sbTaskMap.Append(",\n");
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (string item in _arErrorMsg)
			{
				stringBuilder2.Append(item);
			}
			_arErrorMsg.Clear();
			sbErrorMap.Append(stringBuilder2);
			if (count == 0)
			{
				sbTaskMap.AppendFormat("\t(dwTaskId:={0}, wType:={1}, wNumOfConnectorMap:=0, pConnectorMapList:=0)", nTaskId, nType);
				return;
			}
			string text2 = (bIsProxyMap ? $"connectormapproxy_{stAppName}_{nTaskId}_{text}" : $"connectormap_{stAppName}_{nTaskId}_{text}");
			sbTaskMap.AppendFormat("\t(dwTaskId:={0}, wType:={1}, wNumOfConnectorMap:={2}, pConnectorMapList:=ADR({3}[0]))", nTaskId, nType, count, text2);
			sbModuleMap.AppendLine("{attribute 'init_on_onlchange'}");
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0))
			{
				sbModuleMap.AppendLine("{attribute 'blobinit'}");
			}
			sbModuleMap.AppendFormat("{0} : ARRAY [0..{1}] OF IoConfigConnectorMap := [\n", text2, count - 1);
			bool flag = true;
			MappingComparer comparer = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)1, (ushort)1)) ? new MappingComparer(bReverse: true) : ((nType != 1) ? new MappingComparer(bReverse: true) : new MappingComparer(bReverse: false)));
			SortedList sortedList = new SortedList(comparer);
			foreach (KeyValuePair<ITaskMappingInfo, List<ITaskMapping>> dictMapping in _dictMappings)
			{
				sortedList[dictMapping.Key.ModuleIndex] = dictMapping.Value;
			}
			bool flag4 = default(bool);
			uint num4 = default(uint);
			foreach (int key in sortedList.Keys)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					sbModuleMap.Append(",\n");
				}
				List<ITaskMapping> list = sortedList[key] as List<ITaskMapping>;
				string text3 = (bIsProxyMap ? $"channelmapproxy_{stAppName}_{nTaskId}_{text}_{key}" : $"channelmap_{stAppName}_{nTaskId}_{text}_{key}");
				string arg = $"ADR(moduleList[{key}])";
				if (!bIsProxyMap)
				{
					sbModuleMap.AppendFormat("\t(pConnector:={0}, dwNumOfChannels:={1}, pChannelMapList:=ADR({2}[0]))", arg, list.Count, text3);
				}
				else
				{
					string arg2 = $"ADR(moduleList[{nProxyModuleIndex}])";
					sbModuleMap.AppendFormat("\t(pConnector:={0}, dwNumOfChannels:={1}, pChannelMapList:=ADR({2}[0]))", arg2, list.Count, text3);
				}
				bool bFirst = true;
				sbChannelMap.AppendLine("{attribute 'init_on_onlchange'}");
				sbChannelMap.AppendLine("{attribute 'no_default_init'}");
				if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0))
				{
					bool flag2 = false;
					foreach (Mapping item2 in list)
					{
						if (item2.ParamSet == null)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						sbChannelMap.AppendLine("{attribute 'blobinit'}");
					}
				}
				if (list.Count == 0)
				{
					sbChannelMap.AppendFormat("{0} : ARRAY[0..0] OF IoConfigChannelMap;\n", text3);
				}
				else
				{
					sbChannelMap.AppendFormat("{0} : ARRAY[0..{1}] OF IoConfigChannelMap := [\n", text3, list.Count - 1);
				}
				int num2 = 0;
				int num3 = 0;
				foreach (Mapping item3 in list)
				{
					if (sbForceVariables != null)
					{
						bool flag3 = false;
						foreach (ConnectorMap value6 in lmLate.ConnectorMapList.ConnectorMaps.Values)
						{
							foreach (ChannelMap channelMap in value6.GetChannelMapList(nType == 1))
							{
								string text4 = string.Empty;
								string text5 = string.Empty;
								if (item3.MapToExisiting)
								{
									VariableMapping[] variableMappings = channelMap.GetVariableMappings();
									foreach (VariableMapping variableMapping in variableMappings)
									{
										if (!variableMapping.IsUnusedMapping && item3.ExistingVar == variableMapping.GetPlainVariableName())
										{
											text4 = ConnectorMapList.ForceVariable(value6.MetaName, variableMapping.Variable);
											text5 = ConnectorMapList.ForceFlag(value6.MetaName, variableMapping.Variable);
										}
									}
								}
								else
								{
									IDataLocation locBase = comcon.LocateAddress(out flag4, channelMap.IecAddress);
									BitDataLocation bitDataLocation = new BitDataLocation(locBase);
									BitDataLocation bitDataLocation2 = new BitDataLocation(locBase, (int)(channelMap.BitSize - 1));
									if (item3.StartbitOffset >= bitDataLocation.BitOffset && item3.StartbitOffset <= bitDataLocation2.BitOffset)
									{
										VariableMapping[] variableMappings = channelMap.GetVariableMappings();
										foreach (VariableMapping variableMapping2 in variableMappings)
										{
											if (!variableMapping2.IsUnusedMapping)
											{
												text4 = ConnectorMapList.ForceVariable(value6.MetaName, variableMapping2.Variable);
												text5 = ConnectorMapList.ForceFlag(value6.MetaName, variableMapping2.Variable);
											}
										}
									}
								}
								if (!string.IsNullOrEmpty(text4) && !string.IsNullOrEmpty(text5))
								{
									string value2 = $"IoConfig_Forces[{nForces}].pbyValue := ADR({text4});";
									sbForceVariables.AppendLine(value2);
									string value3 = $"IoConfig_Forces[{nForces}].pxForce := ADR({text5});";
									sbForceVariables.AppendLine(value3);
									string value4 = $"IoConfig_Forces[{nForces}].pChannel := ADR({text3}[{num2}]);";
									sbForceVariables.AppendLine(value4);
									if (nType == 1)
									{
										string value5 = $"IoConfig_Forces[{nForces}].ForceFlags.xIsInput := TRUE;";
										sbForceVariables.AppendLine(value5);
									}
									nForces++;
									num2++;
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								break;
							}
						}
					}
					if (bMappingForLogical)
					{
						bool flag5 = false;
						foreach (ConnectorMap value7 in lmLate.ConnectorMapList.ConnectorMaps.Values)
						{
							if (value7.MappingInfo.ModuleIndex == key)
							{
								IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(value7.ProjectHandle, value7.ObjectGuid);
								if (objectToRead.Object is IAdditionalModules)
								{
									flag5 = true;
								}
								if (objectToRead.Object is LogicalIODevice && objectToRead.Object is IDeviceObject5 && (objectToRead.Object as IDeviceObject5).DeviceIdentificationNoSimulation.Type == 152)
								{
									flag5 = true;
								}
								if (objectToRead.Object is ILogicalGVLObject)
								{
									flag5 = true;
								}
								break;
							}
						}
						if (!flag5 || (item3.MapToExisiting && item3.ExistingVar.StartsWith("map")))
						{
							if (string.IsNullOrEmpty(item3.UniqueVarName))
							{
								item3.UniqueVarName = "Implicit_" + Guid.NewGuid().ToString().Replace("-", "_");
							}
							item3.MappedAddress = "ADR(" + item3.UniqueVarName + ")";
							sbErrorMap.AppendLine("{attribute 'noinit'}");
							sbErrorMap.AppendLine("{attribute 'no_default_init'}");
							sbErrorMap.AppendFormat("{0} : {1};", item3.UniqueVarName, item3.BaseType);
							sbErrorMap.AppendLine();
						}
					}
					item3.AddToLanguageModel(sbChannelMap, key, ref bFirst);
					bool flag6 = item3.MapToExisiting;
					if (!flag6)
					{
						object obj;
						if (item3 == null)
						{
							obj = null;
						}
						else
						{
							IDataElement dataElement = item3.DataElement;
							if (dataElement == null)
							{
								obj = null;
							}
							else
							{
								IIoMapping ioMapping = dataElement.IoMapping;
								obj = ((ioMapping != null) ? ioMapping.VariableMappings : null);
							}
						}
						if (obj != null)
						{
							flag6 = ((ICollection)item3.DataElement.IoMapping.VariableMappings).Count == 0;
						}
					}
					if (sbParamValue != null && flag6 && item3?.ParamSet != null && item3.ParamSet.ParamIdToIndex.TryGetValue((long)item3.ParameterId, out num4))
					{
						string text6 = $"moduleList[{key}].pParameterList[{num4}]";
						sbParamValue.AppendLine("{IF defined (IsLittleEndian)}");
						sbParamValue.AppendLine("{IF defined (pou:IoMgrCopyOutputLE)}");
						sbParamValue.AppendLine($"IF({text6}.dwFlags AND 16#2) <> 0 THEN");
						sbParamValue.AppendLine($"IoMgrCopyOutputLE(ADR({text3}[{num3}]), {text6}.dwValue);");
						sbParamValue.AppendLine("ELSE");
						sbParamValue.AppendLine($"IoMgrCopyOutputLE(ADR({text3}[{num3}]), ADR({text6}.dwValue));");
						sbParamValue.AppendLine("END_IF");
						sbParamValue.AppendLine("{END_IF}");
						sbParamValue.AppendLine("{ELSE}");
						sbParamValue.AppendLine("{IF defined (pou:IoMgrCopyOutputBE)}");
						sbParamValue.AppendLine($"IF({text6}.dwFlags AND 16#2) <> 0 THEN");
						sbParamValue.AppendLine($"IoMgrCopyOutputBE(ADR({text3}[{num3}]), {text6}.dwValue);");
						sbParamValue.AppendLine("ELSE");
						sbParamValue.AppendLine($"IoMgrCopyOutputBE(ADR({text3}[{num3}]), ADR({text6}.dwValue));");
						sbParamValue.AppendLine("END_IF");
						sbParamValue.AppendLine("{END_IF}");
						sbParamValue.AppendLine("{END_IF}");
					}
					num3++;
					if (!item3.MapToExisiting)
					{
						if ((APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)40) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0))) && item3.BitSize == 1)
						{
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0))
							{
								long num5 = item3.BitOffset;
								if (item3.BitSize == 1 && num5 % 8 != 0L)
								{
									num5 -= num5 % 8;
								}
								stringBuilder.AppendFormat("{{attribute 'MapInformation {0},{1},{2}'}}\n", num5 + item3.IecAddrBitOffset, item3.BitSize, item3.IecVar);
							}
							else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0))
							{
								if (item3.IecVar.ToLowerInvariant().Contains("x"))
								{
									stringBuilder.AppendFormat("{{attribute 'MapInformation {0},{1},{2}'}}\n", item3.BitOffset, item3.BitSize, item3.IecVar);
								}
								else
								{
									stringBuilder.AppendFormat("{{attribute 'MapInformation {0},{1},{2}'}}\n", item3.BitOffset + item3.IecAddrBitOffset, item3.BitSize, item3.IecVar);
								}
							}
							else
							{
								stringBuilder.AppendFormat("{{attribute 'MapInformation {0},{1},{2}'}}\n", item3.BitOffset, item3.BitSize, item3.IecVar);
							}
						}
						else
						{
							stringBuilder.AppendFormat("{{attribute 'MapInformation {0},{1},{2}'}}\n", item3.BitOffset + item3.IecAddrBitOffset, item3.BitSize, item3.IecVar);
						}
					}
					else
					{
						stringBuilder.AppendFormat("{{attribute 'MapInformation {0},{1},{2}'}}\n", item3.StartBit + item3.BitOffset, item3.BitSize, item3.ExistingVar);
					}
				}
				if (list.Count > 0)
				{
					sbChannelMap.Append("\n];\n");
				}
			}
			sbModuleMap.Append("\n];\n");
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0))
			{
				sbTaskMap.Insert(0, stringBuilder);
			}
		}

		private List<ITaskMapping> GetMapping(ITaskMappingInfo mappingInfo)
		{
			List<ITaskMapping> value = null;
			if (_dictModuleIndex.TryGetValue(mappingInfo.ModuleIndex, out value))
			{
				return value;
			}
			value = new List<ITaskMapping>();
			_dictMappings[mappingInfo] = value;
			_dictModuleIndex[mappingInfo.ModuleIndex] = value;
			return value;
		}
	}
}
