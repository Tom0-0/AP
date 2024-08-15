#define DEBUG
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{87130527-d8a8-4a3f-a43d-fabaec05f356}")]
	[StorageVersion("3.3.0.0")]
	internal class DataElementStructType : DataElementBase
	{
		[DefaultSerialization("LmStructTypeGuid")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private Guid _guidLmStructType = Guid.Empty;

		[DefaultSerialization("IecType")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stIecType = string.Empty;

		[DefaultSerialization("IecTypeLib")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stIecTypeLib = string.Empty;

		[DefaultSerialization("TypeName")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stTypeName = string.Empty;

		internal Guid LmStructType
		{
			get
			{
				return _guidLmStructType;
			}
			set
			{
				_guidLmStructType = value;
			}
		}

		public override bool HasSubElements
		{
			get
			{
				if (base.SubElementCollection == null)
				{
					return false;
				}
				return base.SubElementCollection.Count > 0;
			}
		}

		public override bool IsRangeType => false;

		public override bool IsEnumeration => false;

		public override IDataElement this[string stIdentifier] => base.SubElementCollection[stIdentifier];

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
		}

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
		}

		public override string DefaultValue => "";

		public override string Value
		{
			get
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected O, but got Unknown
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				stringBuilder.Append("{");
				if (base.SubElementCollection != null)
				{
					foreach (IDataElement item in base.SubElementCollection)
					{
						IDataElement val = item;
						if (flag)
						{
							flag = false;
						}
						else
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(val.Value);
					}
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
		}

		public override bool SupportsSetValue => false;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
		}

		public override string BaseType
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
		}

		public override bool HasBaseType => false;

		public bool HasIecType
		{
			get
			{
				if (_stIecType != null && _stIecType != string.Empty)
				{
					return true;
				}
				return false;
			}
		}

		public string IecType
		{
			get
			{
				return _stIecType;
			}
			set
			{
				_stIecType = value;
			}
		}

		public string GetIecTypeLib => _stIecTypeLib;

		public override bool CanWatch => false;

		public string TypeName => _stTypeName;

		public DataElementStructType()
		{
		}

		internal DataElementStructType(DataElementStructType original)
			: base(original)
		{
			_guidLmStructType = original._guidLmStructType;
		}

		public override object Clone()
		{
			DataElementStructType obj = new DataElementStructType(this)
			{
				_stIecType = _stIecType,
				_stIecTypeLib = _stIecTypeLib,
				_stTypeName = _stTypeName
			};
			((GenericObject)obj).AfterClone();
			return obj;
		}

		public override void SetValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on struct types");
		}

		public override string GetTypeString()
		{
			if (_stIecType != null && _stIecType != string.Empty)
			{
				return _stIecType;
			}
			return string.Empty;
		}

		internal override ushort GetTypeId()
		{
			return 28;
		}

		public override IOnlineVarRef CreateWatch()
		{
			throw new InvalidOperationException("CreateWatch not supported for structure types");
		}

		public override IOnlineVarRef2 CreateWatch(bool bClientControlled)
		{
			throw new InvalidOperationException("CreateWatch not supported for structure types");
		}

		internal override string GetTypeName(string stBaseName)
		{
			if (_stIecType != null && _stIecType != string.Empty)
			{
				return _stIecType;
			}
			if (_stTypeName != string.Empty)
			{
				string text = Regex.Replace(_stTypeName, "[^a-zA-Z0-9]", "a");
				if (stBaseName.EndsWith(_stIdentifier))
				{
					int length = stBaseName.LastIndexOf(_stIdentifier);
					stBaseName = stBaseName.Substring(0, length) + text;
				}
			}
			return stBaseName;
		}

		internal override string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue)
		{
			StringBuilder stringBuilder = new StringBuilder();
			CheckDefaultValueInitialisation(bIsOutput, ref bCreateDefaultValue);
			bool flag = true;
			bDefault = true;
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					bool bDefault2;
					string initialization = item.GetInitialization(out bDefault2, bIsOutput, bCreateDefaultValue);
					bDefault &= bDefault2;
					if (initialization != null && initialization != string.Empty)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							stringBuilder.Append(", ");
						}
						if (item.LanguageModelPositionId != -1)
						{
							stringBuilder.AppendFormat("{{p {0}}}", item.LanguageModelPositionId);
						}
						stringBuilder.AppendFormat("{0} := {1}", item.Identifier, initialization);
					}
				}
			}
			string text = stringBuilder.ToString();
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)4, (ushort)0))
			{
				return "(" + text + ")";
			}
			if (text.Trim() != string.Empty)
			{
				return "(" + text + ")";
			}
			return string.Empty;
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
			if (!bUpgrade || _guidLmStructType == Guid.Empty)
			{
				_guidLmStructType = LanguageModelHelper.CreateDeterministicGuid(base.IoProvider.GetMetaObject().ObjectGuid, stPath + base.DataElement.Identifier + _stTypeName);
			}
			if (base.SubElementCollection == null)
			{
				return;
			}
			foreach (DataElementBase item in base.SubElementCollection)
			{
				item.UpdateLanguageModelGuids(bUpgrade, stPath + base.DataElement.Identifier);
			}
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			ILanguageModelBuilder lmBuilder = lmcontainer.lmBuilder;
			ILanguageModelBuilder3 val = (ILanguageModelBuilder3)(object)((lmBuilder is ILanguageModelBuilder3) ? lmBuilder : null);
			if (lmcontainer.lmNew != null && val != null)
			{
				ISequenceStatement val2 = (ISequenceStatement)(object)((ILanguageModelBuilder)val).CreateSequenceStatement((IExprementPosition)null);
				if (base.SubElementCollection != null)
				{
					foreach (DataElementBase item in base.SubElementCollection)
					{
						string text = item.GetTypeName(stTypeName + "_" + item.Identifier);
						if (item is DataElementSimpleType && (item.BaseType == "BIT" || item.BaseType == "SAFEBIT"))
						{
							text = item.BaseType;
						}
						IStatement val3 = val.CreatePragmaStatement2((IExprementPosition)null, "attribute 'noinit'");
						val2.AddStatement(val3);
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)2, (ushort)0))
						{
							val2.AddStatement(val.CreatePragmaStatement2((IExprementPosition)null, "attribute 'symbol' := 'none'"));
						}
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)6, (ushort)0) && (int)item.GetAccessRight(bOnline: false) == 0)
						{
							val2.AddStatement(val.CreatePragmaStatement2((IExprementPosition)null, "attribute 'hide'"));
						}
						IMessage val4 = null;
						ICompiledType val5 = ((ILanguageModelBuilder)val).CreateComplexType(text, out val4);
						DeviceObjectHelper.CheckNameIdentifier(val, val2, item);
						IVariableDeclarationStatement val6 = ((ILanguageModelBuilder)val).CreateSimpleVariableDeclarationStatement((IExprementPosition)null, item.Identifier, val5);
						if (item.IoMapping != null && item.IoMapping.VariableMappings != null && ((ICollection)item.IoMapping.VariableMappings).Count > 0 && item.IoMapping.VariableMappings[0].CreateVariable)
						{
							bHide = false;
						}
						item.AddTypeDefs(text, lmcontainer, bHide);
						val2.AddStatement((IStatement)(object)val6);
					}
				}
				bool flag = false;
				if (lmcontainer.lmNew != null)
				{
					ILMDataType[] dataTypes = lmcontainer.lmNew.DataTypes;
					for (int i = 0; i < dataTypes.Length; i++)
					{
						if (dataTypes[i].Name == stTypeName)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					((ILanguageModelBuilder)val).CreateVariableDeclarationListStatement((IExprementPosition)null, (VarFlag)1, val2);
					ITypeDeclarationStatement val7 = ((ILanguageModelBuilder)val).CreateStructDeclaration((IExprementPosition)null, stTypeName, (IExpression)null, (IExpression)null, val2, (SignatureFlag)0);
					ILMDataType val8 = ((ILanguageModelBuilder)val).CreateDataType(stTypeName, _guidLmStructType);
					ISequenceStatement val9 = (ISequenceStatement)(object)((ILanguageModelBuilder)val).CreateSequenceStatement((IExprementPosition)null);
					val9.AddStatement((IStatement)(object)val7);
					val8.Interface=(val9);
					lmcontainer.lmNew.AddDataType(val8);
				}
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("TYPE {0}:\nSTRUCT\n", stTypeName);
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item2 in base.SubElementCollection)
				{
					string typeName = item2.GetTypeName(stTypeName + "_" + item2.Identifier);
					stringBuilder.Append("{attribute 'noinit'}");
					stringBuilder.AppendFormat("\t{0}:{1};\n", item2.Identifier, typeName);
					if (item2.IoMapping != null && item2.IoMapping.VariableMappings != null && ((ICollection)item2.IoMapping.VariableMappings).Count > 0 && item2.IoMapping.VariableMappings[0].CreateVariable)
					{
						bHide = false;
					}
					item2.AddTypeDefs(typeName, lmcontainer, bHide);
				}
			}
			stringBuilder.Append("END_STRUCT\nEND_TYPE\n");
			Debug.Assert(_guidLmStructType != Guid.Empty);
			lmcontainer.AddStructType(stTypeName, stringBuilder.ToString(), _guidLmStructType, bHide);
		}

		private long GetPackMode()
		{
			long num = DeviceObjectHelper.GetPackMode(_guidLmStructType);
			if (num == 0L)
			{
				if (Parent.IoProvider is ConnectorBase)
				{
					num = (Parent.IoProvider as ConnectorBase).GetPackMode();
				}
				if (Parent.IoProvider is DeviceObject)
				{
					num = (Parent.IoProvider as DeviceObject).GetPackMode();
				}
				if (num == 0L)
				{
					num = 64L;
				}
				DeviceObjectHelper.StorePackMode(_guidLmStructType, num);
			}
			return num;
		}

		public override long GetBitSize()
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Expected I4, but got Unknown
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Invalid comparison between Unknown and I4
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Expected I4, but got Unknown
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Expected I4, but got Unknown
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Invalid comparison between Unknown and I4
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)4, (ushort)2, (ushort)0))
			{
				long packMode = GetPackMode();
				long num = 0L;
				long num2 = 0L;
				if (base.SubElementCollection != null)
				{
					foreach (DataElementBase item in base.SubElementCollection)
					{
						long bitSize = item.GetBitSize();
						if (item.HasBaseType)
						{
							long num3 = bitSize;
							ushort typeId = item.GetTypeId();
							ICompiledType val = Types.ParseType(item.BaseType);
							if (typeId == 26)
							{
								num3 = Types.GetBitSize(((object)val.BaseType).ToString());
							}
							if ((val.IsInteger || !APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0)) && num3 > num2)
							{
								num2 = num3;
							}
							TypeClass val2;
							if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)10, (ushort)0))
							{
								val2 = (TypeClass)typeId;
								switch ((int)val2 - 14)
								{
								default:
									if ((int)val2 == 37)
									{
										if (num3 > num2)
										{
											num2 = num3;
										}
										long num4 = ((num3 < packMode) ? num3 : packMode);
										num = (num + num4 - 1) / num4 * num4;
									}
									break;
								case 0:
								case 1:
								case 4:
								case 5:
								case 6:
								case 7:
									if (num3 > num2)
									{
										num2 = num3;
									}
									break;
								case 2:
								case 3:
									break;
								}
							}
							val2 = (TypeClass)typeId;
							switch ((int)val2 - 2)
							{
							case 0:
							case 1:
							case 2:
							case 3:
							case 4:
							case 5:
							case 6:
							case 7:
							case 8:
							case 9:
							case 10:
							case 11:
							case 12:
							case 13:
							case 16:
							case 17:
							case 18:
							case 19:
							case 24:
							{
								long num6 = ((num3 < packMode) ? num3 : packMode);
								num = (num + num6 - 1) / num6 * num6;
								break;
							}
							case 15:
							{
								long num5 = ((16 < packMode) ? 16 : packMode);
								num = (num + num5 - 1) / num5 * num5;
								break;
							}
							}
						}
						else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)12, (ushort)0))
						{
							long largestSize = GetLargestSize((IDataElement)(object)item);
							if (largestSize != 0L)
							{
								long num7 = ((largestSize < packMode) ? largestSize : packMode);
								num = (num + num7 - 1) / num7 * num7;
								if (largestSize > num2)
								{
									num2 = largestSize;
								}
							}
						}
						else if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
						{
							long num8 = 0L;
							foreach (DataElementBase item2 in (IEnumerable)item.SubElements)
							{
								if (!item2.HasBaseType)
								{
									continue;
								}
								ICompiledType val3 = Types.ParseType(item2.BaseType);
								long bitSize2 = item2.GetBitSize();
								if (val3.IsInteger)
								{
									if (bitSize2 > num8)
									{
										num8 = bitSize2;
									}
								}
								else
								{
									if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)10, (ushort)0))
									{
										continue;
									}
									TypeClass val2 = ((IType)val3).Class;
									switch ((int)val2 - 14)
									{
									default:
										if ((int)val2 != 37)
										{
											continue;
										}
										break;
									case 0:
									case 1:
									case 4:
									case 5:
									case 6:
									case 7:
										break;
									case 2:
									case 3:
										continue;
									}
									if (bitSize2 > num8)
									{
										num8 = bitSize2;
									}
								}
							}
							if (num8 != 0L)
							{
								long num9 = ((num8 < packMode) ? num8 : packMode);
								num = (num + num9 - 1) / num9 * num9;
							}
						}
						num += bitSize;
					}
				}
				if (num2 != 0L)
				{
					long num10 = ((num2 < packMode) ? num2 : packMode);
					num = (num + num10 - 1) / num10 * num10;
				}
				return num;
			}
			long num11 = 0L;
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item3 in base.SubElementCollection)
				{
					num11 += item3.GetBitSize();
				}
				return num11;
			}
			return num11;
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, bAlwaysMapping, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, string.Empty, bMotorolaBitfield);
		}

		internal void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, string stElementName, bool bMotorolaBitfield)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			if (((ICollection)base.IoMapping.VariableMappings).Count == 0 && bAlwaysMapping && IsSubElementMapping(comcon, _ioMapping.GetIecAddress(htStartAddresses), this, directVarCRefs, htStartAddresses))
			{
				flag = true;
			}
			if ((((ICollection)base.IoMapping.VariableMappings).Count > 0 || bAlwaysMapping) && !flag)
			{
				ChannelMap channelMap = new ChannelMap(lParameterId, (ushort)GetBitSize(), bInput, bReadOnly, bAlwaysMapping, (IDataElement)(object)this, mappingMode);
				channelMap.IecAddress = _ioMapping.GetIecAddress(htStartAddresses);
				channelMap.ParamBitoffset = (ushort)lOffset;
				channelMap.LanguageModelPositionId = base.LanguageModelPositionId;
				channelMap.Comment = base.Description;
				if (((ICollection)base.IoMapping.VariableMappings).Count > 0)
				{
					bAlwaysMapping = false;
				}
				if (stElementName == string.Empty || !string.IsNullOrEmpty(_stIecType))
				{
					string text = $"param_{stBaseName}_{lParameterId}";
					channelMap.Type = GetTypeName("T" + text);
				}
				else
				{
					string text2 = Regex.Replace(_stTypeName, "[^a-zA-Z0-9]", "a");
					channelMap.Type = $"Tparam_{stBaseName}_{stElementName + text2}";
				}
				if (bAlwaysMapping)
				{
					AddMappingUnused(cm, channelMap);
				}
				foreach (VariableMapping item in (IEnumerable)base.IoMapping.VariableMappings)
				{
					channelMap.AddVariableMapping(item);
				}
				cm.AddChannelMap(channelMap);
			}
			if (base.SubElementCollection == null)
			{
				return;
			}
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0))
			{
				foreach (DataElementBase item2 in base.SubElementCollection)
				{
					lOffset = item2.GetBitOffset();
					if (item2 is DataElementStructType)
					{
						string stElementName2 = stElementName + Regex.Replace(_stTypeName, "[^a-zA-Z0-9]", "a") + "_";
						(item2 as DataElementStructType).AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, stElementName2, bMotorolaBitfield);
					}
					else if (item2 is DataElementUnionType)
					{
						string stElementName3 = stElementName + Regex.Replace(_stTypeName, "[^a-zA-Z0-9]", "a") + "_";
						(item2 as DataElementUnionType).AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, stElementName3, bMotorolaBitfield);
					}
					else
					{
						item2.AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
					}
				}
				return;
			}
			foreach (DataElementBase item3 in base.SubElementCollection)
			{
				if (item3 is DataElementStructType)
				{
					string stElementName4 = stElementName + Regex.Replace(_stTypeName, "[^a-zA-Z0-9]", "a") + "_";
					(item3 as DataElementStructType).AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, stElementName4, bMotorolaBitfield);
				}
				else if (item3 is DataElementUnionType)
				{
					string stElementName5 = stElementName + Regex.Replace(_stTypeName, "[^a-zA-Z0-9]", "a") + "_";
					(item3 as DataElementUnionType).AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, stElementName5, bMotorolaBitfield);
				}
				else
				{
					item3.AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				}
				lOffset += item3.GetBitSize();
			}
		}

		public override long GetBitOffset(IDataElement child)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Invalid comparison between Unknown and I4
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected I4, but got Unknown
			long packMode = GetPackMode();
			long num = Parent.GetBitOffset((IDataElement)(object)this);
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					long bitSize = item.GetBitSize();
					if (item.HasBaseType)
					{
						ushort typeId = item.GetTypeId();
						TypeClass val;
						if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)10, (ushort)0))
						{
							val = (TypeClass)typeId;
							if ((int)val == 37)
							{
								long num2 = ((bitSize < packMode) ? bitSize : packMode);
								num = (num + num2 - 1) / num2 * num2;
							}
						}
						val = (TypeClass)typeId;
						switch ((int)val - 2)
						{
						case 0:
						case 1:
						case 2:
						case 3:
						case 4:
						case 5:
						case 6:
						case 7:
						case 8:
						case 9:
						case 10:
						case 11:
						case 12:
						case 13:
						case 16:
						case 17:
						case 18:
						case 19:
						{
							long num4 = ((bitSize < packMode) ? bitSize : packMode);
							num = (num + num4 - 1) / num4 * num4;
							break;
						}
						case 15:
						{
							long num3 = ((16 < packMode) ? 16 : packMode);
							num = (num + num3 - 1) / num3 * num3;
							break;
						}
						}
					}
					else
					{
						long num5 = 0L;
						num5 = GetLargestSize((IDataElement)(object)item);
						if (num5 != 0L)
						{
							long num6 = ((num5 < packMode) ? num5 : packMode);
							num = (num + num6 - 1) / num6 * num6;
						}
					}
					if (item.Identifier.Equals(child.Identifier))
					{
						return num;
					}
					num += bitSize;
				}
			}
			throw new ArgumentException($"'{child.Identifier}' is not a child of this dataelement");
		}

		private long GetLargestSize(IDataElement elem)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected I4, but got Unknown
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Invalid comparison between Unknown and I4
			long num = 0L;
			foreach (DataElementBase item in (IEnumerable)elem.SubElements)
			{
				if (item.HasBaseType)
				{
					ICompiledType val = Types.ParseType(item.BaseType);
					long bitSize = item.GetBitSize();
					if (val.IsInteger)
					{
						if (bitSize > num)
						{
							num = bitSize;
						}
					}
					else
					{
						if (!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)10, (ushort)0))
						{
							continue;
						}
						TypeClass @class = ((IType)val).Class;
						switch ((int)@class - 14)
						{
						default:
							if ((int)@class != 37)
							{
								continue;
							}
							break;
						case 0:
						case 1:
						case 4:
						case 5:
						case 6:
						case 7:
							break;
						case 2:
						case 3:
							continue;
						}
						if (bitSize > num)
						{
							num = bitSize;
						}
					}
				}
				else
				{
					long largestSize = GetLargestSize((IDataElement)(object)item);
					if (largestSize > num)
					{
						num = largestSize;
					}
				}
			}
			return num;
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		internal override void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			StructType structType = (StructType)definition;
			_stTypeName = definition.Name;
			_stIecType = structType.IecType;
			_stIecTypeLib = structType.IecTypeLib;
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			if (defaultValue != null && defaultValue.Count > 0)
			{
				if (defaultValue[0].HasOfflineAccessRight)
				{
					_accessRightOffline = defaultValue[0].OfflineAccess;
				}
				if (defaultValue[0].HasOnlineAccessRight)
				{
					_accessRightOnline = defaultValue[0].OnlineAccess;
				}
			}
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			foreach (StructComponent component in structType.Components)
			{
				DataElementBase dataElementBase = null;
				if (bUpdate && dataElementCollection.Contains(component.Identifier))
				{
					dataElementBase = (DataElementBase)(object)dataElementCollection[component.Identifier];
				}
				if (bUpdate && ((int)component.GetAccessRight(bOnline: false) & 2) == 0)
				{
					dataElementBase = null;
				}
				ValueElement valueElement = null;
				if (defaultValue != null && defaultValue.Count > 0)
				{
					valueElement = defaultValue[0].GetSubElement(component.Identifier);
				}
				if (valueElement == null)
				{
					valueElement = component.Default;
				}
				LList<ValueElement> val = new LList<ValueElement>();
				if (valueElement != null)
				{
					val.Add(valueElement);
				}
				DataElementBase dataElementBase2 = factory.Create(component.Identifier, val, component.Type, component.VisibleName, component.Unit, component.Description, filterFlags, this, dataElementBase, dataElementBase != null, bCreateBitChannels: false);
				if (valueElement == null || !valueElement.HasOfflineAccessRight)
				{
					dataElementBase2.SetAccessRight(bOnline: false, component.GetAccessRight(bOnline: false));
				}
				if (valueElement == null || !valueElement.HasOnlineAccessRight)
				{
					dataElementBase2.SetAccessRight(bOnline: true, component.GetAccessRight(bOnline: true));
				}
				dataElementBase2.SetCustomItems(component.CustomItems);
				base.SubElementCollection.Add((IDataElement)(object)dataElementBase2);
			}
		}

		internal override void SetDefault(ValueElement defaultValue)
		{
			foreach (ValueElement subElement in defaultValue.SubElements)
			{
				((DataElementBase)(object)base.SubElementCollection[subElement.Name])?.SetDefault(subElement);
			}
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on struct types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on struct types");
		}

		public override void SetDefaultValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on struct types");
		}
	}
}
