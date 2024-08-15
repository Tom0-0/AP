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
	[TypeGuid("{81907F9D-D4DA-4e7a-95AB-17FAE563B9B8}")]
	[StorageVersion("3.3.2.0")]
	internal class DataElementUnionType : DataElementBase
	{
		[DefaultSerialization("LmUnionTypeGuid")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private Guid _guidLmUnionType = Guid.Empty;

		[DefaultSerialization("IecType")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stIecType = string.Empty;

		[DefaultSerialization("IecTypeLib")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stIecTypeLib = string.Empty;

		[DefaultSerialization("TypeName")]
		[StorageVersion("3.3.2.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stTypeName = string.Empty;

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

		internal Guid LmUnionType
		{
			get
			{
				return _guidLmUnionType;
			}
			set
			{
				_guidLmUnionType = value;
			}
		}

		public override bool IsRangeType => false;

		public override bool IsEnumeration => false;

		public override IDataElement this[string stIdentifier] => base.SubElementCollection[stIdentifier];

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on union types");
			}
		}

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on union types");
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
						if (!string.IsNullOrEmpty(val.Value))
						{
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
				}
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on union types");
			}
		}

		public override bool SupportsSetValue => false;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on union types");
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on union types");
			}
		}

		public override string BaseType
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on union types");
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

		public DataElementUnionType()
		{
		}

		internal DataElementUnionType(DataElementUnionType original)
			: base(original)
		{
			_guidLmUnionType = original._guidLmUnionType;
		}

		public override object Clone()
		{
			DataElementUnionType obj = new DataElementUnionType(this)
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
			throw new InvalidOperationException("Operation not allowed on union types");
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
			throw new InvalidOperationException("CreateWatch not supported for union types");
		}

		public override IOnlineVarRef2 CreateWatch(bool bClientControlled)
		{
			throw new InvalidOperationException("CreateWatch not supported for union types");
		}

		internal override string GetTypeName(string stBaseName)
		{
			if (!string.IsNullOrEmpty(_stIecType))
			{
				return _stIecType;
			}
			if (!string.IsNullOrEmpty(_stTypeName))
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
					if (!string.IsNullOrEmpty(initialization))
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
			if (!bUpgrade || _guidLmUnionType == Guid.Empty)
			{
				_guidLmUnionType = LanguageModelHelper.CreateDeterministicGuid(base.IoProvider.GetMetaObject().ObjectGuid, stPath + base.DataElement.Identifier + _stTypeName);
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
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
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
					ITypeDeclarationStatement val7 = ((ILanguageModelBuilder)val).CreateUnionDeclaration((IExprementPosition)null, stTypeName, (IExpression)null, (IExpression)null, val2, (SignatureFlag)0);
					ILMDataType val8 = ((ILanguageModelBuilder)val).CreateDataType(stTypeName, _guidLmUnionType);
					ISequenceStatement val9 = (ISequenceStatement)(object)((ILanguageModelBuilder)val).CreateSequenceStatement((IExprementPosition)null);
					val9.AddStatement((IStatement)(object)val7);
					val8.Interface=(val9);
					lmcontainer.lmNew.AddDataType(val8);
				}
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("TYPE {0}:\nUNION\n", stTypeName);
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
			stringBuilder.Append("END_UNION\nEND_TYPE\n");
			Debug.Assert(_guidLmUnionType != Guid.Empty);
			lmcontainer.AddStructType(stTypeName, stringBuilder.ToString(), _guidLmUnionType, bHide);
		}

		public override long GetBitSize()
		{
			long num = 0L;
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					num = Math.Max(num, item.GetBitSize());
				}
				return num;
			}
			return num;
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, bAlwaysMapping, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, string.Empty, bMotorolaBitfield);
		}

		internal void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, string stElementName, bool bMotorolaBitfield)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
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
				if (stElementName == string.Empty)
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
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		internal override void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			UnionType unionType = (UnionType)definition;
			_stTypeName = definition.Name;
			_stIecType = unionType.IecType;
			_stIecTypeLib = unionType.IecTypeLib;
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			foreach (StructComponent component in unionType.Components)
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
				if (base.SubElementCollection.Contains(subElement.Name))
				{
					((DataElementBase)(object)base.SubElementCollection[subElement.Name])?.SetDefault(subElement);
				}
			}
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on union types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on union types");
		}

		public override void SetDefaultValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on union types");
		}
	}
}
