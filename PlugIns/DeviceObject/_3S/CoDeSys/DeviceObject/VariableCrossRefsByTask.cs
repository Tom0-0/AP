using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	internal class VariableCrossRefsByTask
	{
		internal class DirectVarInfo
		{
			internal BitDataLocation BitLocation;

			internal int iBitSize;

			public DirectVarInfo(BitDataLocation BitDataLoc, int iSize)
			{
				BitLocation = BitDataLoc;
				iBitSize = iSize;
			}
		}

		private VariableCrossRefList[] _inputs;

		private VariableCrossRefList[] _outputs;

		private VariableCrossRefList[] _missinginputs;

		private VariableCrossRefList[] _missingoutputs;

		public VariableCrossRefsByTask(ICompileContext comcon, ConnectorMapList cml, IMetaObject metaplc, int nNumTasks, int nBusTask, StringBuilder sbErrorMsg, bool bMappingForLogical, DirectVarCrossRefsByTask directVarCRefs, bool bIsDeviceApp, bool bShowAsError)
		{
			if (nNumTasks != 0)
			{
				_inputs = new VariableCrossRefList[nNumTasks];
				_outputs = new VariableCrossRefList[nNumTasks];
				_missinginputs = new VariableCrossRefList[nNumTasks];
				_missingoutputs = new VariableCrossRefList[nNumTasks];
				for (int i = 0; i < nNumTasks; i++)
				{
					_inputs[i] = new VariableCrossRefList();
					_outputs[i] = new VariableCrossRefList();
					_missinginputs[i] = new VariableCrossRefList();
					_missingoutputs[i] = new VariableCrossRefList();
				}
				if (!bMappingForLogical)
				{
					Fill(comcon, metaplc, _inputs, _missinginputs, cml.GetVariableDeclarations(bInput: true), nBusTask, sbErrorMsg, (DirectVariableLocation)1, directVarCRefs.Inputs, bIsDeviceApp, bShowAsError);
					Fill(comcon, metaplc, _outputs, _missingoutputs, cml.GetVariableDeclarations(bInput: false), nBusTask, sbErrorMsg, (DirectVariableLocation)2, directVarCRefs.Outputs, bIsDeviceApp, bShowAsError);
				}
				else
				{
					FillLogical(comcon, metaplc.Name, _missinginputs, cml.GetVariableDeclarations(bInput: true), nBusTask, sbErrorMsg, string.Empty, bCreateNoChildApp: false);
					FillLogical(comcon, metaplc.Name, _missingoutputs, cml.GetVariableDeclarations(bInput: false), nBusTask, sbErrorMsg, string.Empty, bCreateNoChildApp: false);
				}
			}
		}

		public void AddLogicalGVL(ICompileContext comcon, ConnectorMapList cml, string stDeviceName, int nBusTask, StringBuilder sbErrorMsg, string stApplication)
		{
			FillLogical(comcon, stDeviceName, _missinginputs, cml.GetVariableDeclarations(bInput: true), nBusTask, sbErrorMsg, stApplication, bCreateNoChildApp: true);
			FillLogical(comcon, stDeviceName, _missingoutputs, cml.GetVariableDeclarations(bInput: false), nBusTask, sbErrorMsg, stApplication, bCreateNoChildApp: true);
		}

		public VariableCrossRef[] GetCrossRefsForTask(byte byTask, bool bInput)
		{
			if (bInput)
			{
				VariableCrossRef[] crossRefs = _inputs[byTask].GetCrossRefs();
				VariableCrossRef[] crossRefs2 = _missinginputs[byTask].GetCrossRefs();
				VariableCrossRef[] array = new VariableCrossRef[crossRefs.Length + crossRefs2.Length];
				crossRefs.CopyTo(array, 0);
				crossRefs2.CopyTo(array, crossRefs.Length);
				return array;
			}
			VariableCrossRef[] crossRefs3 = _outputs[byTask].GetCrossRefs();
			VariableCrossRef[] crossRefs4 = _missingoutputs[byTask].GetCrossRefs();
			VariableCrossRef[] array2 = new VariableCrossRef[crossRefs3.Length + crossRefs4.Length];
			crossRefs3.CopyTo(array2, 0);
			crossRefs4.CopyTo(array2, crossRefs3.Length);
			return array2;
		}

		public VariableCrossRef[] GetMissingCrossRefsForTask(byte byTask, bool bInput)
		{
			if (bInput)
			{
				return _missinginputs[byTask].GetCrossRefs();
			}
			return _missingoutputs[byTask].GetCrossRefs();
		}

		private void FillLogical(ICompileContext comcon, string stDeviceName, VariableCrossRefList[] crefList, LList<VariableDeclaration> varDeclList, int nBusTask, StringBuilder sbErrorMsg, string stApplication, bool bCreateNoChildApp)
		{
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Invalid comparison between Unknown and I4
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Invalid comparison between Unknown and I4
			foreach (VariableDeclaration varDecl3 in varDeclList)
			{
				bool bCreateVariable = varDecl3.Variable.CreateVariable;
				string text = varDecl3.Variable.Variable;
				if (!string.IsNullOrEmpty(stApplication))
				{
					if (varDecl3.Connector == null)
					{
						continue;
					}
					IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(varDecl3.Connector.ProjectHandle, varDecl3.Connector.ObjectGuid);
					if ((!typeof(LogicalIODevice).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(IAdditionalModules).IsAssignableFrom(metaObjectStub.ObjectType) && !typeof(ILogicalGVLObject).IsAssignableFrom(metaObjectStub.ObjectType)) || (varDecl3.Channel.DataElement != null && ((ICollection)varDecl3.Channel.DataElement.IoMapping.VariableMappings).Count == 0))
					{
						continue;
					}
					bCreateVariable = false;
					text = stApplication + "." + varDecl3.Variable.Variable;
				}
				if (!bCreateNoChildApp || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)10))
				{
					VariableDeclaration varDecl = new VariableDeclaration(new VariableMapping(varDecl3.Variable.Id, text, bCreateVariable, varDecl3.Variable.IsUnusedMapping)
					{
						IoProvider = varDecl3.Variable.IoProvider,
						Parent = varDecl3.Variable.Parent
					}, varDecl3.Connector, varDecl3.Channel);
					int num = nBusTask;
					if (varDecl3.Connector.BusTask >= 0)
					{
						num = varDecl3.Connector.BusTask;
					}
					if (crefList.Length > num && num >= 0)
					{
						crefList[num].Add(new VariableCrossRef(varDecl, null, null, -1));
					}
					continue;
				}
				string text2 = stDeviceName + "." + text;
				IVariable val = null;
				IList<ITaskCrossref> list = null;
				ISignature val2 = null;
				try
				{
					IScope val3 = comcon.CreateGlobalIScope();
					IExpressionTypifier obj = ((ILanguageModelManager9)APEnvironment.LanguageModelMgr).CreateTypifier(((ICompileContextCommon)comcon).ApplicationGuid, -1, false, false);
					IExpressionTypifier2 val4 = (IExpressionTypifier2)(object)((obj is IExpressionTypifier2) ? obj : null);
					IScanner val5 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(varDecl3.Variable.Variable, false, false, false, false);
					IExpression val6 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val5).ParseOperand();
					if (val6 != null && val4 != null)
					{
						val4.TypifyExpression(val6);
						IVariableExpression variableExpression = GetVariableExpression(val6);
						if (variableExpression != null)
						{
							val = variableExpression.GetVariable(val3);
							val2 = val3[variableExpression.SignatureId];
						}
					}
					list = ((ILanguageModelManager16)APEnvironment.LanguageModelMgr).GetTaskReferencesOfInstancePath(text2);
				}
				catch
				{
				}
				VariableDeclaration varDecl2 = new VariableDeclaration(new VariableMapping(varDecl3.Variable.Id, text, bCreateVariable, varDecl3.Variable.IsUnusedMapping)
				{
					IoProvider = varDecl3.Variable.IoProvider,
					Parent = varDecl3.Variable.Parent
				}, varDecl3.Connector, varDecl3.Channel);
				bool flag = false;
				if (list != null)
				{
					foreach (ITaskCrossref item in list)
					{
						flag = true;
						bool flag2 = false;
						VariableCrossRef[] crossRefs = crefList[item.TaskId].GetCrossRefs();
						for (int i = 0; i < crossRefs.Length; i++)
						{
							if (crossRefs[i].VariableDeclaration == varDecl3)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							continue;
						}
						int num2 = item.TaskId;
						if (varDecl3.Channel.AlwaysMapping && (int)varDecl3.Channel.AlwaysMappingMode == 1)
						{
							if (nBusTask >= 0)
							{
								num2 = nBusTask;
							}
							if (varDecl3.Connector.BusTask >= 0)
							{
								num2 = varDecl3.Connector.BusTask;
							}
						}
						if (num2 >= 0)
						{
							crefList[num2].Add(new VariableCrossRef(varDecl2, null, null, -1));
						}
					}
				}
				else if (val != null)
				{
					ICrossReference[] crossReferences = val.CrossReferences;
					foreach (ICrossReference val7 in crossReferences)
					{
						ISignature signatureById = comcon.GetSignatureById(val7.CodeId);
						if (signatureById == null || signatureById.OrgName == PouDefinitions.ErrorPou_Name)
						{
							continue;
						}
						byte[] taskReferenceList = signatureById.TaskReferenceList;
						for (int j = 0; j < taskReferenceList.Length; j++)
						{
							int num3 = taskReferenceList[j];
							flag = true;
							bool flag3 = false;
							VariableCrossRef[] crossRefs = crefList[num3].GetCrossRefs();
							for (int k = 0; k < crossRefs.Length; k++)
							{
								if (crossRefs[k].VariableDeclaration == varDecl3)
								{
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								continue;
							}
							if (varDecl3.Channel.AlwaysMapping && (int)varDecl3.Channel.AlwaysMappingMode == 1)
							{
								if (nBusTask >= 0)
								{
									num3 = nBusTask;
								}
								if (varDecl3.Connector.BusTask >= 0)
								{
									num3 = varDecl3.Connector.BusTask;
								}
							}
							if (num3 >= 0)
							{
								crefList[num3].Add(new VariableCrossRef(varDecl2, null, null, -1));
							}
						}
					}
				}
				bool flag4 = varDecl3.Channel.AlwaysMapping;
				if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)9, (ushort)0))
				{
					continue;
				}
				if (!flag && !flag4 && val2 != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(varDecl3.Connector.ProjectHandle, val2.ObjectGuid))
				{
					IMetaObjectStub metaObjectStub2 = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(varDecl3.Connector.ProjectHandle, val2.ObjectGuid);
					if (typeof(ILogicalGVLObject).IsAssignableFrom(metaObjectStub2.ObjectType))
					{
						flag4 = true;
					}
				}
				if (!flag && flag4)
				{
					int num4 = nBusTask;
					if (varDecl3.Connector.BusTask >= -1)
					{
						num4 = varDecl3.Connector.BusTask;
					}
					if (crefList.Length > num4 && num4 >= 0)
					{
						crefList[num4].Add(new VariableCrossRef(varDecl2, null, null, -1));
					}
				}
			}
		}

		private IVariableExpression GetVariableExpression(IExpression exp)
		{
			if (exp is IVariableExpression)
			{
				return (IVariableExpression)(object)((exp is IVariableExpression) ? exp : null);
			}
			if (exp is ICompoAccessExpression)
			{
				return GetVariableExpression(((ICompoAccessExpression)((exp is ICompoAccessExpression) ? exp : null)).Right);
			}
			if (exp is IIndexAccessExpression)
			{
				return GetVariableExpression(((IIndexAccessExpression)((exp is IIndexAccessExpression) ? exp : null)).Var);
			}
			if (exp is IGlobalScopeExpression)
			{
				return GetVariableExpression(((IGlobalScopeExpression)((exp is IGlobalScopeExpression) ? exp : null)).Base);
			}
			return null;
		}

		private void Fill(ICompileContext comcon, IMetaObject metaPlc, VariableCrossRefList[] crefList, VariableCrossRefList[] crefListMissing, LList<VariableDeclaration> varDeclList, int nBusTask, StringBuilder sbErrorMsg, DirectVariableLocation location, DirectVarCrossRefList[] directVarRefList, bool bIsDeviceApp, bool bShowAsError)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Invalid comparison between Unknown and I4
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Invalid comparison between Unknown and I4
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Invalid comparison between Unknown and I4
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Expected I4, but got Unknown
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Invalid comparison between Unknown and I4
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0639: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ab: Invalid comparison between Unknown and I4
			//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa1: Invalid comparison between Unknown and I4
			//IL_0ddc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de4: Invalid comparison between Unknown and I4
			//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e46: Invalid comparison between Unknown and I4
			//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f97: Invalid comparison between Unknown and I4
			IDirectVariableCrossRefTable directVariableTable = comcon.GetDirectVariableTable();
			IDirectVariable[] allDirectVariables = directVariableTable.AllDirectVariables;
			Dictionary<IDirectVariable, DirectVarInfo> dictionary = new Dictionary<IDirectVariable, DirectVarInfo>();
			if (allDirectVariables != null && allDirectVariables.Length != 0)
			{
				IDirectVariable[] array = allDirectVariables;
				bool flag = default(bool);
				foreach (IDirectVariable val in array)
				{
					if (val.Location != location)
					{
						continue;
					}
					IDataLocation locBase = comcon.LocateAddress(out flag, val);
					if (flag)
					{
						continue;
					}
					int num = 0;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)1, (ushort)0))
					{
						IAddressCrossReference[] crossReferencesOfDirectVariable = directVariableTable.GetCrossReferencesOfDirectVariable(val);
						bool flag2 = false;
						IAddressCrossReference[] array2 = crossReferencesOfDirectVariable;
						foreach (IAddressCrossReference val2 in array2)
						{
							ISignature signatureById = comcon.GetSignatureById(val2.CodeId);
							if (signatureById == null)
							{
								continue;
							}
							if ((int)val.Location == 2 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0))
							{
								bool flag3 = false;
								IAddressCodePosition[] positions = val2.Positions;
								for (int k = 0; k < positions.Length; k++)
								{
									if (((int)positions[k].Access & 2) == 2)
									{
										flag3 = true;
									}
								}
								if (!flag3)
								{
									continue;
								}
							}
							if (signatureById.TaskReferenceList.Length != 0)
							{
								flag2 = true;
							}
							if ((int)val.Size != 1)
							{
								num = Math.Max(LanguageModelHelper.GetMaxBitSize(val2.Positions), num);
							}
						}
						if (!flag2)
						{
							continue;
						}
					}
					BitDataLocation bitDataLoc = new BitDataLocation(locBase);
					dictionary[val] = new DirectVarInfo(bitDataLoc, num);
				}
			}
			bool flag4 = false;
			if (((ICompileContextCommon)comcon).ApplicationGuid == DeviceObjectHelper.ConfigModeApplication(metaPlc.ObjectGuid))
			{
				flag4 = true;
			}
			bool flag7 = default(bool);
			foreach (VariableDeclaration varDecl2 in varDeclList)
			{
				bool flag5 = flag4 || varDecl2.Variable.CreateVariable;
				if (flag5 && varDecl2.Channel.AlwaysMapping)
				{
					bool flag6 = false;
					IDirectVariable val3 = null;
					IDirectVariable iecAddress = varDecl2.Channel.IecAddress;
					IDataLocation val4 = comcon.LocateAddress(out flag7, iecAddress);
					if (!flag7 && val4 != null)
					{
						BitDataLocation bitDataLocation = new BitDataLocation(val4);
						foreach (KeyValuePair<IDirectVariable, DirectVarInfo> item in dictionary)
						{
							bool flag8 = false;
							val3 = item.Key;
							if (val3.IsEqual(iecAddress))
							{
								flag6 = true;
								flag8 = true;
							}
							if (!flag6 && val3.Location == iecAddress.Location)
							{
								int num2 = item.Value.iBitSize;
								if (num2 == 0)
								{
									DirectVariableSize size = val3.Size;
									switch ((int)size - 1)
									{
									case 1:
										num2 = 8;
										break;
									case 2:
										num2 = 16;
										break;
									case 3:
										num2 = 32;
										break;
									case 4:
										num2 = 64;
										break;
									case 0:
										num2 = 1;
										break;
									}
								}
								long bitOffset = item.Value.BitLocation.BitOffset;
								long num3 = bitOffset + num2;
								long bitOffset2 = bitDataLocation.BitOffset;
								long num4 = bitDataLocation.BitOffset + varDecl2.Channel.BitSize;
								if (bitOffset <= bitOffset2 && num3 >= num4)
								{
									flag6 = true;
									flag8 = true;
								}
							}
							if (!flag8 || !varDecl2.Channel.AlwaysMapping || (int)varDecl2.Channel.AlwaysMappingMode != 1)
							{
								continue;
							}
							int num5 = nBusTask;
							if (varDecl2.Connector.BusTask >= -1)
							{
								num5 = varDecl2.Connector.BusTask;
							}
							for (int l = 0; l < directVarRefList.Length; l++)
							{
								if (num5 == -1)
								{
									directVarRefList[l].GetCrossRefs();
									if (directVarRefList[l].Contains(val3) && directVarRefList[l].TryGetValue(val3, out var cref))
									{
										directVarRefList[l].Remove(cref);
									}
								}
								else if (l != num5)
								{
									directVarRefList[l].GetCrossRefs();
									if (directVarRefList[l].Contains(val3) && directVarRefList[l].TryGetValue(val3, out var cref2))
									{
										directVarRefList[num5].Add(cref2);
										directVarRefList[l].Remove(cref2);
									}
								}
							}
						}
					}
					if (!flag6)
					{
						VariableDeclaration varDecl = new VariableDeclaration(new VariableMapping(varDecl2.Variable.Id, ((object)varDecl2.Channel.IecAddress).ToString(), varDecl2.Variable.CreateVariable, bIsUnusedMapping: true), varDecl2.Connector, varDecl2.Channel);
						int num6 = nBusTask;
						if (varDecl2.Connector.BusTask >= -1)
						{
							num6 = varDecl2.Connector.BusTask;
						}
						if (crefListMissing.Length > num6 && num6 >= 0)
						{
							crefListMissing[num6].Add(new VariableCrossRef(varDecl, null, null, -1));
						}
					}
				}
				else
				{
					if (flag5)
					{
						continue;
					}
					string text = metaPlc.Name + "." + varDecl2.Variable.Variable;
					IVariable val5 = null;
					string text2 = null;
					ISignature val6 = null;
					IList<ITaskCrossref> list = null;
					bool flag9 = false;
					try
					{
						bool flag10 = APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)83) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)90);
						text2 = ((!(APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0) || flag10)) ? varDecl2.Variable.Variable : varDecl2.Variable.Variable.Substring(varDecl2.Variable.Variable.IndexOf('.') + 1));
						IScope val7 = comcon.CreateGlobalIScope();
						IExpressionTypifier obj = ((ILanguageModelManager9)APEnvironment.LanguageModelMgr).CreateTypifier(((ICompileContextCommon)comcon).ApplicationGuid, -1, false, false);
						IExpressionTypifier2 val8 = (IExpressionTypifier2)(object)((obj is IExpressionTypifier2) ? obj : null);
						IScanner val9 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(text2, false, false, false, false);
						IExpression val10 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val9).ParseOperand();
						if (val10 != null && val8 != null)
						{
							val8.TypifyExpression(val10);
							IVariableExpression variableExpression = GetVariableExpression(val10);
							if (variableExpression != null)
							{
								val5 = variableExpression.GetVariable(val7);
								val6 = val7[variableExpression.SignatureId];
							}
							if (val5 != null)
							{
								IType type = val5.Type;
								if (((type != null) ? new TypeClass?(type.Class) : null) == (TypeClass?)23)
								{
									flag9 = true;
								}
							}
							if (!flag9 && val10 is ICompoAccessExpression)
							{
								IExpression val11 = val10;
								while (val11 is ICompoAccessExpression)
								{
									val11 = ((ICompoAccessExpression)((val11 is ICompoAccessExpression) ? val11 : null)).Left;
									IVariableExpression variableExpression2 = GetVariableExpression(val11);
									int num7;
									if (variableExpression2 == null)
									{
										num7 = 0;
									}
									else
									{
										ICompiledType type2 = ((IExpression)variableExpression2).Type;
										num7 = ((((type2 != null) ? new TypeClass?(((IType)type2).Class) : null) == (TypeClass?)23) ? 1 : 0);
									}
									if (num7 != 0)
									{
										flag9 = true;
										break;
									}
								}
							}
						}
						if (!bIsDeviceApp && (val5 == null || val6 == null))
						{
							val5 = ((ILanguageModelManager7)APEnvironment.LanguageModelMgr).GetVariableCompiled(text);
						}
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0))
						{
							list = ((ILanguageModelManager16)APEnvironment.LanguageModelMgr).GetTaskReferencesOfInstancePath(text);
						}
					}
					catch
					{
					}
					if (val5 == null)
					{
						continue;
					}
					if (val5.DataLocation != null && val5.DataLocation.IsBitLocation)
					{
						varDecl2.Channel.IecBitoffset = val5.DataLocation.BitNr;
					}
					if (!CheckMapExistingVariable(comcon, val5, varDecl2.Channel, sbErrorMsg, varDecl2.Connector.ObjectGuid, bShowAsError))
					{
						continue;
					}
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)12, (ushort)0) && flag9)
					{
						string value = $"{{messageguid '{varDecl2.Connector.ObjectGuid.ToString()}'}}\n";
						sbErrorMsg.AppendLine(value);
						if (varDecl2.Channel.LanguageModelPositionId != -1)
						{
							sbErrorMsg.AppendFormat("{{p {0} }}\n", varDecl2.Channel.LanguageModelPositionId);
						}
						sbErrorMsg.AppendFormat("{{error '{0}: map to referenced variable is not possible!' show_compile}}", val5.OrgName);
						continue;
					}
					if (val5.Address != null && (int)val5.Address.Location != 3)
					{
						if (((object)val5.Address).ToString() != ((object)varDecl2.Channel.IecAddress).ToString())
						{
							string value2 = $"{{messageguid '{varDecl2.Connector.ObjectGuid.ToString()}'}}\n";
							sbErrorMsg.AppendLine(value2);
							if (varDecl2.Channel.LanguageModelPositionId != -1)
							{
								sbErrorMsg.AppendFormat("{{p {0} }}\n", varDecl2.Channel.LanguageModelPositionId);
							}
							sbErrorMsg.AppendFormat("{{error '{0}: map to existing variable with address is not allowed!' show_compile}}", val5.OrgName);
						}
						continue;
					}
					bool flag11 = false;
					ICrossReference[] array3 = val5.CrossReferences;
					if (list != null)
					{
						array3 = (ICrossReference[])(object)new ICrossReference[list.Count];
						for (int m = 0; m < list.Count; m++)
						{
							array3[m] = list[m].CrossRef;
						}
					}
					int signId = -1;
					if (val6 != null)
					{
						signId = val6.Id;
					}
					ICrossReference[] array4 = array3;
					foreach (ICrossReference val12 in array4)
					{
						ISignature signatureById2 = comcon.GetSignatureById(val12.CodeId);
						if (signatureById2 == null || signatureById2.OrgName == PouDefinitions.ErrorPou_Name)
						{
							continue;
						}
						if ((APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0) || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)5, (ushort)110)) && signatureById2.ObjectGuid != Guid.Empty && varDecl2.Channel.IsInput && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0) && comcon is ICompileContext10)
						{
							ICompiledPOU compiledPOUById = comcon.GetCompiledPOUById(val12.CodeId);
							bool flag12 = true;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)83) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)90)))
							{
								flag12 = compiledPOUById != null;
							}
							else if (compiledPOUById == null || compiledPOUById.GetFlag((CompiledPOUFlags)4096))
							{
								if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)10))
								{
									continue;
								}
								flag12 = false;
							}
							ISignature val13 = signatureById2;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)0, (ushort)0)))
							{
								val13 = val6;
								if (val13 == null)
								{
									val13 = signatureById2;
								}
							}
							if (flag12 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0) && signatureById2 != null && signatureById2.Name == "FB_INIT")
							{
								flag12 = false;
							}
							if (flag12)
							{
								IList<ICodePosition> referencePositionsOfPOUEx = ((ICompileContext10)((comcon is ICompileContext10) ? comcon : null)).GetReferencePositionsOfPOUEx(val12.CodeId, val13.Id, val5.Id);
								if (referencePositionsOfPOUEx != null)
								{
									foreach (ICodePosition item2 in referencePositionsOfPOUEx)
									{
										if ((item2 is ICodePosition2 && ((ICodePosition2)((item2 is ICodePosition2) ? item2 : null)).GetAccessFlag((AccessFlag)64)) || ((int)item2.Access & 2) != 2)
										{
											continue;
										}
										if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0))
										{
											IPreCompileContext val14 = null;
											ISourcePosition val15 = (ISourcePosition)(object)new SourcePosition(((IEngine)APEnvironment.Engine).Projects.PrimaryProject.Handle, signatureById2.ObjectGuid, item2.EditorPosition, item2.PositionOffset, 0);
											IExprement val16 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).FindExpressionAtSourcePosition(val15, (WhatToFind)0, out val14);
											if (val16 == null || val16.ToString().IndexOf(text2, StringComparison.InvariantCultureIgnoreCase) < 0)
											{
												continue;
											}
											if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)16, (ushort)0))
											{
												string arg = string.Format(Strings.WarningWriteAccess, val16.ToString());
												sbErrorMsg.AppendLine($"{{messageguid '{signatureById2.ObjectGuid.ToString()}'}}\n");
												sbErrorMsg.AppendFormat("{{p {0}}}\n", val15.PositionCombination);
												sbErrorMsg.AppendFormat("{{warning '{0}',Length:={1} show_compile}}", arg, val16.Position.Length);
												if (varDecl2.Channel.LanguageModelPositionId != -1)
												{
													sbErrorMsg.AppendLine($"{{messageguid '{varDecl2.Connector.ObjectGuid.ToString()}'}}\n");
													sbErrorMsg.AppendFormat("{{p {0} }}\n", varDecl2.Channel.LanguageModelPositionId);
													sbErrorMsg.AppendFormat("{{info 'related position'}}");
												}
												continue;
											}
										}
										string value3 = $"{{messageguid '{varDecl2.Connector.ObjectGuid.ToString()}'}}\n";
										sbErrorMsg.AppendLine(value3);
										if (varDecl2.Channel.LanguageModelPositionId != -1)
										{
											sbErrorMsg.AppendFormat("{{p {0} }}\n", varDecl2.Channel.LanguageModelPositionId);
										}
										string arg2 = string.Format(Strings.ErrorNoValidAssignment, varDecl2.Variable.Variable);
										sbErrorMsg.AppendFormat("{{warning '{0}' show_compile}}", arg2);
										break;
									}
								}
							}
						}
						if (val6 != null && signatureById2.ObjectGuid != Guid.Empty && !varDecl2.Channel.IsInput && ((APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)1, (ushort)0) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0)) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)60) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)62))) && comcon is ICompileContext10)
						{
							ICompiledPOU compiledPOUById2 = comcon.GetCompiledPOUById(val12.CodeId);
							bool flag13 = true;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)0) || (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)83) && !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)3, (ushort)90)))
							{
								flag13 = compiledPOUById2 != null;
							}
							else if (compiledPOUById2 == null || compiledPOUById2.GetFlag((CompiledPOUFlags)4096))
							{
								flag13 = false;
							}
							if (flag13)
							{
								IList<ICodePosition> referencePositionsOfPOUEx2 = ((ICompileContext10)((comcon is ICompileContext10) ? comcon : null)).GetReferencePositionsOfPOUEx(val12.CodeId, val6.Id, val5.Id);
								if (referencePositionsOfPOUEx2 != null)
								{
									bool flag14 = false;
									foreach (ICodePosition item3 in referencePositionsOfPOUEx2)
									{
										if (((int)item3.Access & 2) == 2)
										{
											flag14 = true;
											break;
										}
									}
									if (!flag14)
									{
										continue;
									}
								}
							}
						}
						if (list != null)
						{
							continue;
						}
						byte[] taskReferenceList = signatureById2.TaskReferenceList;
						for (int j = 0; j < taskReferenceList.Length; j++)
						{
							int num8 = taskReferenceList[j];
							flag11 = true;
							if (varDecl2.Channel.AlwaysMapping && (int)varDecl2.Channel.AlwaysMappingMode == 1)
							{
								if (nBusTask >= 0)
								{
									num8 = nBusTask;
								}
								if (varDecl2.Connector.BusTask >= 0)
								{
									num8 = varDecl2.Connector.BusTask;
								}
							}
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)3, (ushort)2, (ushort)0) && num8 >= 0)
							{
								bool flag15 = false;
								VariableCrossRef[] crossRefs = crefList[num8].GetCrossRefs();
								for (int k = 0; k < crossRefs.Length; k++)
								{
									if (crossRefs[k].VariableDeclaration == varDecl2)
									{
										flag15 = true;
										break;
									}
								}
								if (!flag15 && num8 >= 0)
								{
									crefList[num8].Add(new VariableCrossRef(varDecl2, val12, val5, signId));
								}
							}
							else if (num8 >= 0)
							{
								crefList[num8].Add(new VariableCrossRef(varDecl2, val12, val5, signId));
							}
						}
					}
					if (list != null)
					{
						foreach (ITaskCrossref item4 in list)
						{
							flag11 = true;
							bool flag16 = false;
							VariableCrossRef[] crossRefs = crefList[item4.TaskId].GetCrossRefs();
							for (int i = 0; i < crossRefs.Length; i++)
							{
								if (crossRefs[i].VariableDeclaration == varDecl2)
								{
									flag16 = true;
									break;
								}
							}
							if (flag16)
							{
								continue;
							}
							int num9 = item4.TaskId;
							if (varDecl2.Channel.AlwaysMapping && (int)varDecl2.Channel.AlwaysMappingMode == 1)
							{
								if (nBusTask >= 0)
								{
									num9 = nBusTask;
								}
								if (varDecl2.Connector.BusTask >= 0)
								{
									num9 = varDecl2.Connector.BusTask;
								}
							}
							if (num9 >= 0)
							{
								crefList[num9].Add(new VariableCrossRef(varDecl2, item4.CrossRef, val5, item4.SignatureId));
							}
						}
					}
					bool flag17 = varDecl2.Channel.AlwaysMapping;
					if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)7, (ushort)0) && !flag11 && !flag17 && val6 != null && ((IObjectManager)APEnvironment.ObjectMgr).ExistsObject(metaPlc.ProjectHandle, val6.ObjectGuid))
					{
						IMetaObjectStub metaObjectStub = ((IObjectManager)APEnvironment.ObjectMgr).GetMetaObjectStub(metaPlc.ProjectHandle, val6.ObjectGuid);
						if (typeof(ILogicalGVLObject).IsAssignableFrom(metaObjectStub.ObjectType))
						{
							flag17 = true;
						}
					}
					if (!flag11 && flag17)
					{
						int num10 = nBusTask;
						if (varDecl2.Connector.BusTask >= -1)
						{
							num10 = varDecl2.Connector.BusTask;
						}
						if (num10 >= 0)
						{
							crefListMissing[num10].Add(new VariableCrossRef(varDecl2, null, val5, signId));
						}
					}
				}
			}
		}

		internal bool CheckMapExistingVariable(ICompileContext comcon, IVariable mapVariable, ChannelMap channel, StringBuilder sbErrorPragmas, Guid MessageGuid, bool bShowAsError)
		{
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Invalid comparison between Unknown and I4
			bool result = true;
			string value = $"{{messageguid '{MessageGuid.ToString()}'}}\n";
			new StringBuilder();
			VariableMapping[] variableMappings = channel.GetVariableMappings();
			foreach (VariableMapping variableMapping in variableMappings)
			{
				if (variableMapping.CreateVariable)
				{
					continue;
				}
				string text = channel.Type;
				if (text.ToUpperInvariant() == "BIT")
				{
					text = "BOOL";
				}
				if (text.ToUpperInvariant() == "SAFEBIT")
				{
					text = "SAFEBOOL";
				}
				bool flag = true;
				DataElementBase dataElementBase = variableMapping.Parent as DataElementBase;
				if (dataElementBase != null)
				{
					flag = dataElementBase.HasBaseType;
				}
				string text2 = ((object)mapVariable.OriginalType.BaseType).ToString();
				bool bIsWarning = false;
				if (ConnectorMapList.CheckForCompatibleType(comcon, text2, text, channel.IsInput, out bIsWarning, out var stMessage) || string.Compare(text, text2, ignoreCase: true) == 0 || ((int)((IType)mapVariable.OriginalType.BaseType).Class == 1 && channel.BitSize == 1) || !(!text.StartsWith("Tparam_") && flag))
				{
					continue;
				}
				sbErrorPragmas.AppendLine(value);
				if (channel.LanguageModelPositionId != -1)
				{
					sbErrorPragmas.AppendFormat("{{p {0} }}\n", channel.LanguageModelPositionId);
				}
				string arg = string.Format(Strings.ErrorTypeMismatch, variableMapping.Variable);
				if (bIsWarning && !bShowAsError)
				{
					if (!string.IsNullOrEmpty(stMessage))
					{
						sbErrorPragmas.AppendFormat("{{warning '{0}' show_compile}}", stMessage);
						continue;
					}
					sbErrorPragmas.AppendFormat("{{warning '{0}' show_compile}}", arg);
					result = false;
				}
				else
				{
					if (!string.IsNullOrEmpty(stMessage))
					{
						sbErrorPragmas.AppendFormat("{{error '{0}' show_compile}}", stMessage);
					}
					else
					{
						sbErrorPragmas.AppendFormat("{{error '{0}' show_compile}}", arg);
					}
					result = false;
				}
			}
			return result;
		}
	}
}
