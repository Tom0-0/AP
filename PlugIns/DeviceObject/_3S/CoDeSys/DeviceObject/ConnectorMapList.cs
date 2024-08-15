#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	public class ConnectorMapList
	{
		private static string[] _numericTypes = new string[12]
		{
			"BYTE", "WORD", "DWORD", "LWORD", "SINT", "INT", "DINT", "LINT", "USINT", "UINT",
			"UDINT", "ULINT"
		};

		private Dictionary<int, ConnectorMap> _dictConnectorMaps = new Dictionary<int, ConnectorMap>();

		public Dictionary<int, ConnectorMap> ConnectorMaps => _dictConnectorMaps;

		public void Add(ConnectorMap map)
		{
			_dictConnectorMaps[map.MappingInfo.ModuleIndex] = map;
		}

		public void GetMappedVariableDeclarations(ISequenceStatement seq, ILanguageModelBuilder3 lmbuilder, LDictionary<Guid, string> dictApplications, IApplicationObject app, ISequenceStatement seqAttributes, IDriverInfo4 driverInfo)
		{
			Hashtable htMappedExistingVar = new Hashtable();
			foreach (ConnectorMap value in _dictConnectorMaps.Values)
			{
				AddVariableMappings(dictApplications, app, seq, lmbuilder, seqAttributes, value.GetChannelMapList(bInput: true), value, htMappedExistingVar, driverInfo);
				AddVariableMappings(dictApplications, app, seq, lmbuilder, seqAttributes, value.GetChannelMapList(bInput: false), value, htMappedExistingVar, driverInfo);
			}
		}

		public List<string> GetMappedVariableDeclarations(string stApplication, out string stAttributes, out string stErrorPragmas, bool bCreateForLogicalIos)
		{
			MultipleStringBuilder multipleStringBuilder = new MultipleStringBuilder();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			Hashtable htMappedExistingVar = new Hashtable();
			foreach (ConnectorMap value in _dictConnectorMaps.Values)
			{
				bool flag = bCreateForLogicalIos;
				if (!flag && DeviceObjectHelper.GenerateCodeForLogicalDevices)
				{
					IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(value.ProjectHandle, value.ObjectGuid);
					if (objectToRead.Object is LogicalIODevice && objectToRead.Object is IDeviceObject5 && (objectToRead.Object as IDeviceObject5).DeviceIdentificationNoSimulation.Type == 152)
					{
						flag = true;
					}
					if (objectToRead.Object is ILogicalGVLObject)
					{
						flag = true;
					}
				}
				AddVariableMappings(stApplication, multipleStringBuilder, stringBuilder, stringBuilder2, value.GetChannelMapList(bInput: true), value, htMappedExistingVar, flag);
				AddVariableMappings(stApplication, multipleStringBuilder, stringBuilder, stringBuilder2, value.GetChannelMapList(bInput: false), value, htMappedExistingVar, flag);
			}
			stAttributes = stringBuilder.ToString();
			stErrorPragmas = stringBuilder2.ToString();
			return multipleStringBuilder.StringList;
		}

		public List<string> GetForceVariableDeclarations(ref int nForces)
		{
			MultipleStringBuilder multipleStringBuilder = new MultipleStringBuilder();
			foreach (ConnectorMap value in _dictConnectorMaps.Values)
			{
				AddForceVariable(multipleStringBuilder, value.GetChannelMapList(bInput: true), value, ref nForces);
				AddForceVariable(multipleStringBuilder, value.GetChannelMapList(bInput: false), value, ref nForces);
			}
			return multipleStringBuilder.StringList;
		}

		internal LList<VariableDeclaration> GetVariableDeclarations(bool bInput)
		{
			Dictionary<string, VariableDeclaration> dictionary = new Dictionary<string, VariableDeclaration>();
			foreach (ConnectorMap value in _dictConnectorMaps.Values)
			{
				foreach (ChannelMap channelMap in value.GetChannelMapList(bInput))
				{
					VariableMapping[] variableMappings = channelMap.GetVariableMappings();
					foreach (VariableMapping variableMapping in variableMappings)
					{
						string text = variableMapping.GetPlainVariableName().ToUpperInvariant();
						if (!variableMapping.CreateVariable)
						{
							text += channelMap.IecAddress;
						}
						if (!dictionary.ContainsKey(text))
						{
							dictionary.Add(text, new VariableDeclaration(variableMapping, value, channelMap));
						}
					}
				}
			}
			LList<VariableDeclaration> obj = new LList<VariableDeclaration>();
			obj.AddRange((IEnumerable<VariableDeclaration>)dictionary.Values);
			return obj;
		}

		public void GetVariableMappings(IMetaObject[] applications)
		{
			Hashtable hashtable = new Hashtable();
			foreach (IMetaObject val in applications)
			{
				Debug.Assert(!hashtable.ContainsKey(val.Name), "Duplicate application '" + val.Name + "'");
				hashtable[val.Name] = new ArrayList();
			}
			FillVarDeclList(applications, hashtable, bInputs: true);
			CheckVarDeclListForDuplicates(hashtable);
		}

		private void FillVarDeclList(IMetaObject[] applications, Hashtable htApplications, bool bInputs)
		{
			foreach (ConnectorMap value in _dictConnectorMaps.Values)
			{
				foreach (ChannelMap channelMap in value.GetChannelMapList(bInputs))
				{
					VariableMapping[] variableMappings = channelMap.GetVariableMappings();
					foreach (VariableMapping obj in variableMappings)
					{
						string text = channelMap.Type;
						if (text.ToUpperInvariant() == "BIT")
						{
							text = "BOOL";
						}
						if (text.ToUpperInvariant() == "SAFEBIT")
						{
							text = "SAFEBOOL";
						}
						string text2 = obj.GetApplication();
						if (text2 == string.Empty)
						{
							text2 = applications[0].Name;
						}
						VariableDeclaration variableDeclaration = new VariableDeclaration(obj, value, channelMap);
						if (!htApplications.ContainsKey(text2))
						{
							variableDeclaration.SetStaticError($"{{error 'Invalid Application <{text2}>'}}\n");
							text2 = applications[0].Name;
						}
						ArrayList obj2 = (ArrayList)htApplications[text2];
						Debug.Assert(obj2 != null);
						obj2.Add(variableDeclaration);
					}
				}
			}
		}

		private void CheckVarDeclListForDuplicates(Hashtable htApplications)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			foreach (IMetaObject key in htApplications.Keys)
			{
				IMetaObject val = key;
				ArrayList arrayList = (ArrayList)htApplications[val.Name];
				Debug.Assert(arrayList != null);
				arrayList.Sort(VariableDeclaration.COMPARER_BY_NAME);
				for (int i = 0; i < arrayList.Count - 1; i++)
				{
					if (VariableDeclaration.COMPARER_BY_NAME.Compare(arrayList[i], arrayList[i + 1]) == 0)
					{
						VariableDeclaration variableDeclaration = (VariableDeclaration)arrayList[i];
						string staticError = $"{{error 'Variable {variableDeclaration.Variable.GetPlainVariableName()} mapped more then once' show_compile}}";
						variableDeclaration.SetStaticError(staticError);
						variableDeclaration = (VariableDeclaration)arrayList[i + 1];
						staticError = $"{{error 'Variable {variableDeclaration.Variable.GetPlainVariableName()} mapped more then once' show_compile}}";
						variableDeclaration.SetStaticError(staticError);
					}
				}
			}
		}

		internal static string ForceVariable(string stMetaName, string stVariable)
		{
			if (stVariable.Contains("."))
			{
				stVariable = stVariable.Substring(stVariable.LastIndexOf('.') + 1);
			}
			return stMetaName + "_" + stVariable + "_Value";
		}

		internal static string ForceFlag(string stMetaName, string stVariable)
		{
			if (stVariable.Contains("."))
			{
				stVariable = stVariable.Substring(stVariable.LastIndexOf('.') + 1);
			}
			return stMetaName + "_" + stVariable + "_Force";
		}

		private void AddForceVariable(MultipleStringBuilder sbForces, IList channelmaps, ConnectorMap connector, ref int nForces)
		{
			string st = $"{{messageguid '{connector.ObjectGuid.ToString()}'}}\n";
			foreach (ChannelMap channelmap in channelmaps)
			{
				VariableMapping[] variableMappings = channelmap.GetVariableMappings();
				foreach (VariableMapping variableMapping in variableMappings)
				{
					if (!variableMapping.IsUnusedMapping)
					{
						string text = channelmap.Type;
						if (text.ToUpperInvariant() == "BIT")
						{
							text = "BOOL";
						}
						if (text.ToUpperInvariant() == "SAFEBIT")
						{
							text = "SAFEBOOL";
						}
						sbForces.Append(st);
						if (channelmap.LanguageModelPositionId != -1)
						{
							sbForces.Append($"{{p {channelmap.LanguageModelPositionId} }}\n");
						}
						sbForces.AppendLine(ForceFlag(connector.MetaName, variableMapping.Variable) + " : BOOL;");
						sbForces.AppendLine(ForceVariable(connector.MetaName, variableMapping.Variable) + " : " + text + ";");
						nForces++;
					}
				}
			}
		}

		private void AddIoConfigErrorPouStatement(LDictionary<Guid, string> dictApplications, VariableMapping vm, IStatement statement)
		{
			string application = vm.GetApplication();
			bool flag = false;
			foreach (KeyValuePair<Guid, string> keyValuePair in dictApplications)
			{
				if (string.Compare(keyValuePair.Value, application, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					DeviceObjectHelper.IoConfigPou[keyValuePair.Key].AddStatement(statement);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (KeyValuePair<Guid, string> keyValuePair2 in dictApplications)
				{
					DeviceObjectHelper.IoConfigPou[keyValuePair2.Key].AddStatement(statement);
				}
			}
		}

		private void AddVariableMappings(LDictionary<Guid, string> dictApplications, IApplicationObject app, ISequenceStatement seq, ILanguageModelBuilder3 lmbuilder, ISequenceStatement seqAttributes, IList channelmaps, ConnectorMap connector, Hashtable htMappedExistingVar, IDriverInfo4 driverInfo)
		{
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			IStatement val = (IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateMessageGuidPragmaStatement(connector.ObjectGuid);
			bool flag = false;
			if (DeviceObjectHelper.GenerateCodeForLogicalDevices)
			{
				IMetaObject objectToRead = ((IObjectManager)APEnvironment.ObjectMgr).GetObjectToRead(connector.ProjectHandle, connector.ObjectGuid);
				if (objectToRead.Object is LogicalIODevice && objectToRead.Object is IDeviceObject5 && (objectToRead.Object as IDeviceObject5).DeviceIdentificationNoSimulation.Type == 152)
				{
					flag = true;
				}
				if (objectToRead.Object is ILogicalGVLObject)
				{
					flag = true;
				}
			}
			int num = 0;
			foreach (ChannelMap channelmap in channelmaps)
			{
				IExprementPosition val2 = null;
				val2 = ((channelmap.LanguageModelPositionId == -1) ? ((ILanguageModelBuilder)lmbuilder).CreateExprementPosition((long)num++) : ((ILanguageModelBuilder)lmbuilder).CreateExprementPosition(channelmap.LanguageModelPositionId));
				VariableMapping[] variableMappings = channelmap.GetVariableMappings();
				foreach (VariableMapping variableMapping in variableMappings)
				{
					string text = channelmap.Type;
					if (text.ToUpperInvariant() == "BIT")
					{
						text = "BOOL";
					}
					if (text.ToUpperInvariant() == "SAFEBIT")
					{
						text = "SAFEBOOL";
					}
					((ILanguageModelBuilder)lmbuilder).ParseType(text);
					StringBuilder stringBuilder = new StringBuilder();
					if (!variableMapping.CheckValidIdentifier(stringBuilder, string.Empty))
					{
						AddIoConfigErrorPouStatement(dictApplications, variableMapping, val);
						IEmptyStatement val3 = ((ILanguageModelBuilder)lmbuilder).CreateEmptyStatement(val2);
						string text2 = (variableMapping.CreateVariable ? string.Format("{0}: {1}", variableMapping.Variable.Replace("$", "$$").Replace("'", "$'"), Strings.ErrorInvalidCreateVariable.Replace("'", "$'")) : stringBuilder.ToString());
						((ILanguageModelBuilder)lmbuilder).CreateCompilerMessage(val2, (IExprement)(object)val3, text2, (Severity)2, (ShowAttribute)2);
						AddIoConfigErrorPouStatement(dictApplications, variableMapping, (IStatement)(object)val3);
						continue;
					}
					if (variableMapping.CreateVariable)
					{
						seq.AddStatement(val);
						if (channelmap.ReadOnly)
						{
							seq.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'read_only'"));
							seq.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "warning disable C0228"));
						}
						if (variableMapping.IsUnusedMapping)
						{
							seq.AddStatement(lmbuilder.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
						}
						string text3 = variableMapping.Variable;
						if (text3.Contains("$(DeviceName)"))
						{
							text3 = text3.Replace("$(DeviceName)", connector.MetaName);
						}
						seq.AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateCommentStatement(val2, connector.MetaName + " : " + channelmap.Comment));
						if (!flag)
						{
							IExpression val4 = null;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0) && driverInfo != null && (int)driverInfo.StopResetBehaviourSetting != 0 && channelmap.DataElement is DataElementBase && !channelmap.IsInput)
							{
								bool bDefault;
								string text4 = (channelmap.DataElement as DataElementBase).GetInitialization(out bDefault, !channelmap.IsInput, bCreateDefaultValue: false).Trim();
								if (!string.IsNullOrEmpty(text4.Trim(' ', '(', ')')))
								{
									val4 = ((ILanguageModelBuilder2)lmbuilder).ParseInitialisation(text4);
								}
							}
							ICompiledType val5 = ((ILanguageModelBuilder)lmbuilder).ParseType(channelmap.Type);
							seq.AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateVariableDeclarationStatement(val2, text3, val5, val4, channelmap.IecAddress));
						}
						else
						{
							ICompiledType val6 = ((ILanguageModelBuilder)lmbuilder).ParseType(channelmap.Type);
							seq.AddStatement((IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateVariableDeclarationStatement(val2, text3, val6, (IExpression)null, (IDirectVariable)null));
						}
						continue;
					}
					LList<string> val7 = Enumerable.ToLList<string>((IEnumerable<string>)dictApplications.Values);
					if (!val7.Contains(variableMapping.GetApplication().ToUpperInvariant()))
					{
						if (!val7.Contains(OnlineConfigMode.HIDDENONLINECONFIGAPPLICATION.ToUpperInvariant()))
						{
							AddIoConfigErrorPouStatement(dictApplications, variableMapping, val);
							IEmptyStatement val8 = ((ILanguageModelBuilder)lmbuilder).CreateEmptyStatement(val2);
							string text5 = $"Invalid application <{variableMapping.GetApplication()}> in mapping <{variableMapping.Variable}>";
							((ILanguageModelBuilder)lmbuilder).CreateCompilerMessage(val2, (IExprement)(object)val8, text5, (Severity)2, (ShowAttribute)2);
							AddIoConfigErrorPouStatement(dictApplications, variableMapping, (IStatement)(object)val8);
						}
						continue;
					}
					if (channelmap.IsInput)
					{
						if (!htMappedExistingVar.ContainsKey(variableMapping.Variable))
						{
							htMappedExistingVar.Add(variableMapping.Variable, channelmap);
						}
						else
						{
							AddIoConfigErrorPouStatement(dictApplications, variableMapping, val);
							IEmptyStatement val9 = ((ILanguageModelBuilder)lmbuilder).CreateEmptyStatement(val2);
							string text6 = string.Format(Strings.WarningMultipleInputMapping, variableMapping.Variable);
							((ILanguageModelBuilder)lmbuilder).CreateCompilerMessage(val2, (IExprement)(object)val9, text6, (Severity)2, (ShowAttribute)2);
							AddIoConfigErrorPouStatement(dictApplications, variableMapping, (IStatement)(object)val9);
						}
					}
					string text7 = $"attribute 'IoMap:{connector.MappingInfo.ModuleIndex},{channelmap.ParameterId},{channelmap.ParamBitoffset},{channelmap.BitSize}@{variableMapping.Variable}'\n";
					IStatement val10 = lmbuilder.CreatePragmaStatement2((IExprementPosition)null, text7);
					if (variableMapping.Variable.StartsWith(((IObject)app).MetaObject.Name))
					{
						seqAttributes.AddStatement(val10);
					}
					else
					{
						AddIoConfigErrorPouStatement(dictApplications, variableMapping, val10);
					}
					AddIoConfigErrorPouStatement(dictApplications, variableMapping, val);
					if (variableMapping.Variable.Contains("["))
					{
						IExpression val11 = ((ILanguageModelBuilder)lmbuilder).ParseExpression(variableMapping.GetPlainVariableName());
						IVariableExpression val12 = (IVariableExpression)(object)((ILanguageModelBuilder)lmbuilder).CreateVariableExpression(val2, "pPointer");
						IOperatorExpression val13 = ((ILanguageModelBuilder)lmbuilder).CreateOperatorExpression(val2, (Operator)33, val11);
						IStatement statement = (IStatement)(object)((ILanguageModelBuilder)lmbuilder).CreateAssignmentStatement(val2, (IExpression)(object)val12, (IExpression)(object)val13);
						AddIoConfigErrorPouStatement(dictApplications, variableMapping, statement);
					}
					IEmptyStatement val14 = ((ILanguageModelBuilder)lmbuilder).CreateEmptyStatement(val2);
					string text8 = $"undefined variable {variableMapping.GetPlainVariableName()}";
					((ILanguageModelBuilder)lmbuilder).CreateCompilerMessage(val2, (IExprement)(object)val14, text8, (Severity)2, (ShowAttribute)2);
					IExpression val15 = ((ILanguageModelBuilder)lmbuilder).ParseExpression(variableMapping.GetPlainVariableName());
					IVariableReference val16 = ((ILanguageModelBuilder)lmbuilder).CreateVariableReference(val2, val15);
					IDefinedExpression val17 = ((ILanguageModelBuilder)lmbuilder).CreateDefinedExpression(val2, (IExpression)(object)val16);
					IPragmaOperatorExpression val18 = ((ILanguageModelBuilder)lmbuilder).CreatePragmaOperatorExpression(val2, (Operator)133, (IExpression)(object)val17, (IExpression)(object)val17);
					ISequenceStatement2 val19 = ((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)15, (ushort)20))
					{
						string text9 = string.Empty;
						if (channelmap.LanguageModelPositionId != -1)
						{
							text9 = $"{{ p {channelmap.LanguageModelPositionId} }}";
						}
						IExpression val20 = ((ILanguageModelBuilder)lmbuilder).ParseExpression(text9 + variableMapping.GetPlainVariableName());
						IExpressionStatement val21 = ((ILanguageModelBuilder)lmbuilder).CreateExpressionStatement(val20);
						((ISequenceStatement)val19).AddStatement((IStatement)(object)val21);
					}
					else
					{
						((ISequenceStatement)val19).AddStatement((IStatement)(object)val14);
					}
					IPragmaIfStatement statement2 = ((ILanguageModelBuilder)lmbuilder).CreatePragmaIfStatement(val2, (IExpression)(object)val18, val19, (ISequenceStatement2)null);
					AddIoConfigErrorPouStatement(dictApplications, variableMapping, (IStatement)(object)statement2);
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0))
					{
						val14 = ((ILanguageModelBuilder)lmbuilder).CreateEmptyStatement(val2);
						text8 = $"mapped variable {variableMapping.GetPlainVariableName()} is a POU";
						((ILanguageModelBuilder)lmbuilder).CreateCompilerMessage(val2, (IExprement)(object)val14, text8, (Severity)2, (ShowAttribute)2);
						val15 = ((ILanguageModelBuilder)lmbuilder).ParseExpression(variableMapping.GetPlainVariableName());
						IPouReference2 val22 = ((ILanguageModelBuilder)lmbuilder).CreatePouReference(val2, val15);
						val17 = ((ILanguageModelBuilder)lmbuilder).CreateDefinedExpression(val2, (IExpression)(object)val22);
						val19 = ((ILanguageModelBuilder)lmbuilder).CreateSequenceStatement((IExprementPosition)null);
						((ISequenceStatement)val19).AddStatement((IStatement)(object)val14);
						statement2 = ((ILanguageModelBuilder)lmbuilder).CreatePragmaIfStatement(val2, (IExpression)(object)val17, val19, (ISequenceStatement2)null);
						AddIoConfigErrorPouStatement(dictApplications, variableMapping, (IStatement)(object)statement2);
					}
				}
			}
		}

		private void AddVariableMappings(string stApplication, MultipleStringBuilder sbMappings, StringBuilder sbAttributes, StringBuilder sbErrorPragmas, IList channelmaps, ConnectorMap connector, Hashtable htMappedExistingVar, bool bIsLogicalGVLOrNoAddress)
		{
			string text = $"{{messageguid '{connector.ObjectGuid.ToString()}'}}\n";
			stApplication = stApplication.ToUpperInvariant();
			foreach (ChannelMap channelmap in channelmaps)
			{
				VariableMapping[] variableMappings = channelmap.GetVariableMappings();
				foreach (VariableMapping variableMapping in variableMappings)
				{
					string text2 = channelmap.Type;
					if (text2.ToUpperInvariant() == "BIT")
					{
						text2 = "BOOL";
					}
					if (text2.ToUpperInvariant() == "SAFEBIT")
					{
						text2 = "SAFEBOOL";
					}
					if (variableMapping.Id == -1)
					{
						variableMapping.Id = channelmap.LanguageModelPositionId;
					}
					if (!variableMapping.CheckValidIdentifier(sbErrorPragmas, text))
					{
						if (variableMapping.CreateVariable)
						{
							sbErrorPragmas.Append(text);
							if (channelmap.LanguageModelPositionId != -1)
							{
								sbErrorPragmas.AppendFormat("{{p {0} }}\n", channelmap.LanguageModelPositionId);
							}
							sbErrorPragmas.Append("{error '");
							sbErrorPragmas.Append(variableMapping.Variable.Replace("$", "$$").Replace("'", "$'"));
							sbErrorPragmas.Append(": ");
							sbErrorPragmas.Append(Strings.ErrorInvalidCreateVariable.Replace("'", "$'"));
							sbErrorPragmas.AppendLine("'}");
						}
						continue;
					}
					if (variableMapping.CreateVariable)
					{
						sbMappings.Append(text);
						if (channelmap.LanguageModelPositionId != -1)
						{
							sbMappings.Append($"{{p {channelmap.LanguageModelPositionId} }}\n");
						}
						if (channelmap.ReadOnly)
						{
							sbMappings.Append("{attribute 'read_only'}");
						}
						if (variableMapping.IsUnusedMapping)
						{
							sbMappings.Append(LanguageModelHelper.PRAGMA_ATTRIBUTE_HIDE);
						}
						string text3 = variableMapping.Variable;
						if (text3.Contains("$(DeviceName)"))
						{
							text3 = text3.Replace("$(DeviceName)", connector.MetaName);
						}
						sbMappings.AppendLine("//" + connector.MetaName + " : " + channelmap.Comment);
						if (!bIsLogicalGVLOrNoAddress)
						{
							sbMappings.Append($"{text3} AT {((object)channelmap.IecAddress).ToString()} : {channelmap.Type};\n");
						}
						else if (!string.IsNullOrEmpty(text3))
						{
							sbMappings.Append($"{text3} : {channelmap.Type};\n");
						}
						continue;
					}
					if (variableMapping.GetApplication().ToUpperInvariant() != stApplication)
					{
						sbErrorPragmas.AppendLine(text);
						if (channelmap.LanguageModelPositionId != -1)
						{
							sbErrorPragmas.AppendFormat("{{p {0} }}\n", channelmap.LanguageModelPositionId);
						}
						sbErrorPragmas.AppendFormat("{{error 'Invalid application <{0}> in mapping <{1}>'}}", variableMapping.GetApplication(), variableMapping.Variable);
						continue;
					}
					if (channelmap.IsInput)
					{
						if (!htMappedExistingVar.ContainsKey(variableMapping.Variable))
						{
							htMappedExistingVar.Add(variableMapping.Variable, channelmap);
						}
						else
						{
							string arg = string.Format(Strings.WarningMultipleInputMapping, variableMapping.Variable);
							string text4 = $"{{error '{arg}'}}\n";
							text4 = $"{{messageguid '{connector.ObjectGuid}'}}\n" + text4;
							if (channelmap.LanguageModelPositionId != -1)
							{
								text4 = $"{{p {channelmap.LanguageModelPositionId} }}" + text4;
							}
							sbErrorPragmas.AppendLine(text4);
						}
					}
					sbAttributes.AppendFormat("{{attribute 'IoMap:{0},{1},{2},{3}@{4}'}}\n", connector.MappingInfo.ModuleIndex, channelmap.ParameterId, channelmap.ParamBitoffset, channelmap.BitSize, variableMapping.Variable);
					sbErrorPragmas.AppendLine(text);
					if (channelmap.LanguageModelPositionId != -1)
					{
						sbErrorPragmas.AppendFormat("{{p {0} }}\n", channelmap.LanguageModelPositionId);
					}
					if (variableMapping.Variable.Contains("["))
					{
						sbErrorPragmas.AppendFormat("pPointer := ADR({0});", variableMapping.GetPlainVariableName(), variableMapping.GetPlainVariableName());
					}
					sbErrorPragmas.AppendFormat("\r\n{{IF NOT (defined (variable: {0}))}}\r\n\t{{error 'undefined variable {0}' show_compile}}\r\n{{END_IF}}", variableMapping.GetPlainVariableName());
				}
			}
		}

		internal static bool CheckForCompatibleType(ICompileContext comcon, string stType, string stVarType, bool bIsInput, out bool bIsWarning, out string stMessage)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Invalid comparison between Unknown and I4
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Invalid comparison between Unknown and I4
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Invalid comparison between Unknown and I4
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			bIsWarning = false;
			stMessage = string.Empty;
			ICompiledType val = Types.ParseType(stType);
			if (val != null && val.BaseType != null && ((int)((IType)val).Class == 26 || (int)((IType)val).Class == 24))
			{
				stType = ((object)val.BaseType).ToString();
			}
			ICompiledType val2 = Types.ParseType(stVarType);
			if (val2 != null && val2.BaseType != null && ((int)((IType)val2).Class == 26 || (int)((IType)val2).Class == 24))
			{
				stVarType = ((object)val2.BaseType).ToString();
			}
			if (val != null && val.BaseType != null && val2 != null && val2.BaseType != null && val.BaseType == val2.BaseType)
			{
				return true;
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)16, (ushort)0) && val != null && val.BaseType != null && val2 != null && val2.BaseType != null)
			{
				IScope val3 = comcon.CreateGlobalIScope();
				TypeClass @class = ((IType)val.BaseType).Class;
				string text = @class.ToString().ToUpperInvariant();
				@class = ((IType)val2.BaseType).Class;
				string text2 = @class.ToString().ToUpperInvariant();
				if (bIsInput)
				{
					if (val.BaseType.Size(val3) < val2.BaseType.Size(val3))
					{
						stMessage = string.Format(Strings.WarningLossOfData, text2, text);
						bIsWarning = true;
						return false;
					}
					if (((ILanguageModelManager3)APEnvironment.LanguageModelMgr).TypeInfo.IsSigned(((IType)val.BaseType).Class) != ((ILanguageModelManager3)APEnvironment.LanguageModelMgr).TypeInfo.IsSigned(((IType)val2.BaseType).Class))
					{
						if (((ILanguageModelManager3)APEnvironment.LanguageModelMgr).TypeInfo.IsSigned(((IType)val.BaseType).Class))
						{
							stMessage = string.Format(Strings.WarningChangToSigned, text2, text);
						}
						else
						{
							stMessage = string.Format(Strings.WarningChangToUnSigned, text2, text);
						}
						bIsWarning = true;
						return false;
					}
				}
				else
				{
					if (val.BaseType.Size(val3) > val2.BaseType.Size(val3))
					{
						stMessage = string.Format(Strings.WarningLossOfData, text, text2);
						bIsWarning = true;
						return false;
					}
					if (((ILanguageModelManager3)APEnvironment.LanguageModelMgr).TypeInfo.IsSigned(((IType)val.BaseType).Class) != ((ILanguageModelManager3)APEnvironment.LanguageModelMgr).TypeInfo.IsSigned(((IType)val2.BaseType).Class))
					{
						if (((ILanguageModelManager3)APEnvironment.LanguageModelMgr).TypeInfo.IsSigned(((IType)val2.BaseType).Class))
						{
							stMessage = string.Format(Strings.WarningChangToSigned, text, text2);
						}
						else
						{
							stMessage = string.Format(Strings.WarningChangToUnSigned, text, text2);
						}
						bIsWarning = true;
						return false;
					}
				}
			}
			stType = stType.ToUpperInvariant();
			stVarType = stVarType.ToUpperInvariant();
			if (Array.IndexOf(_numericTypes, stType) >= 0 && Array.IndexOf(_numericTypes, stVarType) >= 0)
			{
				return true;
			}
			bool flag = stVarType.StartsWith("SAFE");
			bool flag2 = stType.StartsWith("SAFE");
			if (flag || flag2)
			{
				if (flag)
				{
					stVarType = stVarType.Substring(4);
				}
				if (flag2)
				{
					stType = stType.Substring(4);
				}
				if (flag && flag2 && Array.IndexOf(_numericTypes, stType) >= 0 && Array.IndexOf(_numericTypes, stVarType) >= 0)
				{
					return true;
				}
				if (Array.IndexOf(_numericTypes, stType) >= 0 && Array.IndexOf(_numericTypes, stVarType) >= 0)
				{
					bIsWarning = true;
					return false;
				}
			}
			return false;
		}

		private static string GetCheckForVariableType(string stType, string stVariable)
		{
			string[] array = ((Array.IndexOf(_numericTypes, stType.ToUpperInvariant()) < 0) ? new string[1] { stType } : _numericTypes);
			StringBuilder stringBuilder = new StringBuilder();
			string value = "";
			if (array.Length > 1)
			{
				stringBuilder.Append("(");
			}
			string[] array2 = array;
			foreach (string arg in array2)
			{
				stringBuilder.Append(value);
				stringBuilder.AppendFormat("(hastype (variable: {0}, {1}))", stVariable, arg);
				value = " OR ";
			}
			if (array.Length > 1)
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}
	}
}
