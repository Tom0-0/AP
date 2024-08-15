using System;
using System.Collections;
using System.Text;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{916CBD6B-280D-4c54-A257-53519570B521}")]
	[StorageVersion("3.3.0.0")]
	internal class DataElementArrayType : DataElementBase
	{
		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Dimenstion1LowerBorder")]
		[StorageVersion("3.3.0.0")]
		private string _stDimenstion1LowerBorder = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Dimenstion1UpperBorder")]
		[StorageVersion("3.3.0.0")]
		private string _stDimenstion1UpperBorder = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Dimenstion2LowerBorder")]
		[StorageVersion("3.3.0.0")]
		private string _stDimenstion2LowerBorder = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Dimenstion2UpperBorder")]
		[StorageVersion("3.3.0.0")]
		private string _stDimenstion2UpperBorder = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Dimenstion3LowerBorder")]
		[StorageVersion("3.3.0.0")]
		private string _stDimenstion3LowerBorder = "";

		[DefaultDuplication(DuplicationMethod.Shallow)]
		[DefaultSerialization("Dimenstion3UpperBorder")]
		[StorageVersion("3.3.0.0")]
		private string _stDimenstion3UpperBorder = "";

		[DefaultSerialization("LmArrayTypeGuid")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private Guid _guidLmArrayType = Guid.Empty;

		private bool _bCreatedFromParameter;

		public bool CreatedFromParameter
		{
			get
			{
				return _bCreatedFromParameter;
			}
			set
			{
				_bCreatedFromParameter = value;
			}
		}

		public ArrayTypeBorder Dimension1
		{
			get
			{
				if (!string.IsNullOrEmpty(_stDimenstion1LowerBorder))
				{
					return new ArrayTypeBorder(_stDimenstion1LowerBorder, _stDimenstion1UpperBorder);
				}
				return null;
			}
		}

		public ArrayTypeBorder Dimension2
		{
			get
			{
				if (!string.IsNullOrEmpty(_stDimenstion2LowerBorder))
				{
					return new ArrayTypeBorder(_stDimenstion2LowerBorder, _stDimenstion2UpperBorder);
				}
				return null;
			}
		}

		public ArrayTypeBorder Dimension3
		{
			get
			{
				if (!string.IsNullOrEmpty(_stDimenstion3LowerBorder))
				{
					return new ArrayTypeBorder(_stDimenstion3LowerBorder, _stDimenstion3UpperBorder);
				}
				return null;
			}
		}

		public override bool IsRangeType => false;

		public override IEnumerationValue EnumerationValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on struct types");
			}
			set
			{
				throw new InvalidOperationException("Operation not allowed on array types");
			}
		}

		public override string BaseType
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on array types");
			}
		}

		public override string MinValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on array types");
			}
		}

		public override string MaxValue
		{
			get
			{
				throw new InvalidOperationException("Operation not allowed on array types");
			}
		}

		internal override IDataElementParent Parent
		{
			set
			{
				base.Parent = value;
			}
		}

		public override bool CanWatch => false;

		public override bool HasBaseType => false;

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

		public override bool IsEnumeration => false;

		public override IDataElement this[string stIdentifier] => base.SubElementCollection[stIdentifier];

		public override string DefaultValue => string.Empty;

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
				throw new InvalidOperationException("Operation not allowed on array types");
			}
		}

		public override bool SupportsSetValue => false;

		public DataElementArrayType()
		{
		}

		internal DataElementArrayType(DataElementArrayType original)
			: base(original)
		{
			_stDimenstion1LowerBorder = original._stDimenstion1LowerBorder;
			_stDimenstion1UpperBorder = original._stDimenstion1UpperBorder;
			_stDimenstion2LowerBorder = original._stDimenstion2LowerBorder;
			_stDimenstion2UpperBorder = original._stDimenstion2UpperBorder;
			_stDimenstion3LowerBorder = original._stDimenstion3LowerBorder;
			_stDimenstion3UpperBorder = original._stDimenstion3UpperBorder;
			_guidLmArrayType = original._guidLmArrayType;
		}

		public override object Clone()
		{
			DataElementArrayType dataElementArrayType = new DataElementArrayType(this);
			((GenericObject)dataElementArrayType).AfterClone();
			return dataElementArrayType;
		}

		public override long GetBitSize()
		{
			long num = 0L;
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					num += item.GetBitSize();
				}
				return num;
			}
			return num;
		}

		public override IOnlineVarRef CreateWatch()
		{
			throw new InvalidOperationException("CreateWatch not supported for aray types");
		}

		public override IOnlineVarRef2 CreateWatch(bool bClientControlled)
		{
			throw new InvalidOperationException("CreateWatch not supported for array types");
		}

		internal override void UpdateLanguageModelGuids(bool bUpgrade, string stPath)
		{
			if (!bUpgrade || _guidLmArrayType == Guid.Empty)
			{
				_guidLmArrayType = LanguageModelHelper.CreateDeterministicGuid(base.IoProvider.GetMetaObject().ObjectGuid, stPath + base.DataElement.Identifier + _stIdentifier);
			}
			if (base.SubElementCollection != null && base.SubElementCollection.Count > 0)
			{
				(base.SubElementCollection[0] as DataElementBase).UpdateLanguageModelGuids(bUpgrade, stPath + base.DataElement.Identifier);
			}
		}

		internal override void AddTypeDefs(string stTypeName, LanguageModelContainer lmcontainer, bool bHide)
		{
			if (base.SubElementCollection != null && base.SubElementCollection.Count > 0)
			{
				DataElementBase dataElementBase = base.SubElementCollection[0] as DataElementBase;
				if (dataElementBase is DataElementStructType || dataElementBase is DataElementUnionType)
				{
					stTypeName = stTypeName.Substring(GetTypeName(string.Empty).Length);
					dataElementBase.AddTypeDefs(stTypeName, lmcontainer, bHide);
				}
			}
		}

		internal override void AddMapping(ConnectorMap cm, long lOffset, long lParameterId, bool bInput, bool bReadOnly, string stBaseName, bool bAlwaysMapping, AlwaysMappingMode mappingMode, string stParentType, ICompileContext comcon, DirectVarCrossRefsByTask directVarCRefs, Hashtable htStartAddresses, bool bMotorolaBitfield)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
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
				string text = $"param_{stBaseName}_{lParameterId}";
				channelMap.Type = GetTypeName("T" + text);
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
					item2.AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				}
				return;
			}
			foreach (DataElementBase item3 in base.SubElementCollection)
			{
				item3.AddMapping(cm, lOffset, lParameterId, bInput, bReadOnly, stBaseName, flag, mappingMode, stParentType, comcon, directVarCRefs, htStartAddresses, bMotorolaBitfield);
				lOffset += item3.GetBitSize();
			}
		}

		internal override string GetInitialization(out bool bDefault, bool bIsOutput, bool bCreateDefaultValue)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Expected I4, but got Unknown
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
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					if (!string.IsNullOrEmpty(initialization))
					{
						if (item.LanguageModelPositionId != -1)
						{
							stringBuilder.AppendFormat("{{p {0}}}", item.LanguageModelPositionId);
						}
						stringBuilder.Append(initialization);
						continue;
					}
					TypeClass val = (TypeClass)item.GetTypeId();
					switch ((int)val)
					{
					case 0:
					case 1:
						stringBuilder.Append("false");
						break;
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
					case 14:
					case 15:
						stringBuilder.Append("0");
						break;
					case 16:
						stringBuilder.Append("''");
						break;
					case 17:
						stringBuilder.Append("\"\"");
						break;
					case 18:
						stringBuilder.Append("t#0ms");
						break;
					case 37:
						stringBuilder.Append("LTIME#0ms");
						break;
					default:
						return "";
					}
				}
			}
			string text = stringBuilder.ToString();
			if (text.Trim() != string.Empty)
			{
				return "[" + text + "]";
			}
			return "";
		}

		internal override string GetTypeName(string stBaseName)
		{
			if (base.SubElementCollection != null && base.SubElementCollection.Count > 0)
			{
				DataElementBase dataElementBase = base.SubElementCollection[0] as DataElementBase;
				if (dataElementBase.HasBaseType)
				{
					stBaseName = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)14, (ushort)0)) ? dataElementBase.GetTypeString() : dataElementBase.GetTypeName(stBaseName + "_" + dataElementBase.Identifier));
				}
				return GetTypeDimension() + stBaseName;
			}
			return stBaseName;
		}

		private string GetTypeDimension()
		{
			string text = string.Empty;
			if (_stDimenstion1LowerBorder != string.Empty)
			{
				text = text + _stDimenstion1LowerBorder + "..";
			}
			if (_stDimenstion1UpperBorder != string.Empty)
			{
				text += _stDimenstion1UpperBorder;
			}
			if (_stDimenstion2LowerBorder != string.Empty)
			{
				text = text + "," + _stDimenstion2LowerBorder + "..";
			}
			if (_stDimenstion2UpperBorder != string.Empty)
			{
				text += _stDimenstion2UpperBorder;
			}
			if (_stDimenstion3LowerBorder != string.Empty)
			{
				text = text + "," + _stDimenstion3LowerBorder + "..";
			}
			if (_stDimenstion3UpperBorder != string.Empty)
			{
				text += _stDimenstion3UpperBorder;
			}
			return $"ARRAY [{text}] OF ";
		}

		public override string GetTypeString()
		{
			if (base.SubElementCollection != null && base.SubElementCollection.Count > 0)
			{
				DataElementBase dataElementBase = base.SubElementCollection[0] as DataElementBase;
				string text = string.Empty;
				if (dataElementBase.HasBaseType)
				{
					text = dataElementBase.GetTypeString();
				}
				else
				{
					if (dataElementBase is DataElementStructType)
					{
						text = (dataElementBase as DataElementStructType).GetTypeName(dataElementBase.Identifier);
					}
					if (dataElementBase is DataElementUnionType)
					{
						text = (dataElementBase as DataElementUnionType).GetTypeName(dataElementBase.Identifier);
					}
				}
				return GetTypeDimension() + text;
			}
			return string.Empty;
		}

		internal override ushort GetTypeId()
		{
			return 26;
		}

		public override long GetBitOffset(IDataElement child)
		{
			long num = Parent.GetBitOffset((IDataElement)(object)this);
			if (base.SubElementCollection != null)
			{
				foreach (DataElementBase item in base.SubElementCollection)
				{
					if (item.Identifier.Equals(child.Identifier))
					{
						return num;
					}
					num += item.GetBitSize();
				}
			}
			throw new ArgumentException($"'{child.Identifier}' is not a child of this dataelement");
		}

		public override long GetBitOffset()
		{
			return Parent.GetBitOffset((IDataElement)(object)this);
		}

		internal override void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, IDataElementParent parent)
		{
			Import(definition, factory, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, bCreateBitChannels: false, parent);
		}

		internal void Import(TypeDefinition definition, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, bool bCreateBitChannels, IDataElementParent parent)
		{
			int result = 0;
			int result2 = 0;
			int result3 = 0;
			int result4 = 0;
			int result5 = 0;
			int result6 = 0;
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ArrayType arrayType = (ArrayType)definition;
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			if (arrayType.FirstDimension != null)
			{
				_stDimenstion1LowerBorder = arrayType.FirstDimension.LowerBorder;
				_stDimenstion1UpperBorder = arrayType.FirstDimension.UpperBorder;
				int.TryParse(_stDimenstion1LowerBorder, out result);
				int.TryParse(_stDimenstion1UpperBorder, out result2);
				if (result > result2)
				{
					throw new ArgumentException("FirstDimension: LowerBorder greater than UpperBorder");
				}
			}
			if (arrayType.SecondDimension != null)
			{
				_stDimenstion2LowerBorder = arrayType.SecondDimension.LowerBorder;
				_stDimenstion2UpperBorder = arrayType.SecondDimension.UpperBorder;
				int.TryParse(_stDimenstion2LowerBorder, out result3);
				int.TryParse(_stDimenstion2UpperBorder, out result4);
				if (result3 > result4)
				{
					throw new ArgumentException("SecondDimension: LowerBorder greater than UpperBorder");
				}
			}
			if (arrayType.ThirdDimension != null)
			{
				_stDimenstion3LowerBorder = arrayType.ThirdDimension.LowerBorder;
				_stDimenstion3UpperBorder = arrayType.ThirdDimension.UpperBorder;
				int.TryParse(_stDimenstion3LowerBorder, out result5);
				int.TryParse(_stDimenstion3UpperBorder, out result6);
				if (result5 > result6)
				{
					throw new ArgumentException("ThirdDimension: LowerBorder greater than UpperBorder");
				}
			}
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			int num = 0;
			for (int i = result; i <= result2; i++)
			{
				for (int j = result3; j <= result4; j++)
				{
					for (int k = result5; k <= result6; k++)
					{
						DataElementBase dataElementBase = null;
						string stIdentifier2 = stIdentifier + "_" + i + "_" + j + "_" + k;
						if (bUpdate && dataElementCollection.Contains(stIdentifier2))
						{
							dataElementBase = (DataElementBase)(object)dataElementCollection[stIdentifier2];
						}
						ValueElement valueElement = ((defaultValue == null || defaultValue.Count <= num) ? new ValueElement() : defaultValue[num]);
						LList<ValueElement> val = new LList<ValueElement>();
						val.Add(valueElement);
						DataElementBase element = factory.Create(stIdentifier2, val, arrayType.BaseType, visibleName, unit, description, filterFlags, this, dataElementBase, dataElementBase != null, bCreateBitChannels);
						base.SubElementCollection.Add((IDataElement)(object)element);
						num++;
					}
				}
			}
		}

		internal void Import(IArrayType ctArray, string baseTypeName, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, bool bCreateBitChannels, IDataElementParent parent)
		{
			ArrayTypeBorder dim = null;
			ArrayTypeBorder dim2 = null;
			ArrayTypeBorder dim3 = null;
			switch (ctArray.Dimensions.Length)
			{
			case 1:
			{
				IExpression lowerBorder3 = ctArray.Dimensions[0].LowerBorder;
				IExpression obj3 = ((lowerBorder3 is ILiteralExpression) ? lowerBorder3 : null);
				dim = new ArrayTypeBorder(upperBorder: (ctArray.Dimensions[0].LowerBorder as ILiteralExpression).LongValue.ToString(), lowerBorder: ((ILiteralExpression)obj3).LongValue.ToString());
				break;
			}
			case 2:
			{
				IExpression lowerBorder2 = ctArray.Dimensions[1].LowerBorder;
				IExpression obj2 = ((lowerBorder2 is ILiteralExpression) ? lowerBorder2 : null);
				dim2 = new ArrayTypeBorder(upperBorder: (ctArray.Dimensions[1].LowerBorder as ILiteralExpression).LongValue.ToString(), lowerBorder: ((ILiteralExpression)obj2).LongValue.ToString());
				break;
			}
			case 3:
			{
				IExpression lowerBorder = ctArray.Dimensions[2].LowerBorder;
				IExpression obj = ((lowerBorder is ILiteralExpression) ? lowerBorder : null);
				dim3 = new ArrayTypeBorder(upperBorder: (ctArray.Dimensions[2].LowerBorder as ILiteralExpression).LongValue.ToString(), lowerBorder: ((ILiteralExpression)obj).LongValue.ToString());
				break;
			}
			}
			Import(dim, dim2, dim3, baseTypeName, factory, stIdentifier, visibleName, unit, description, filterFlags, defaultValue, bUpdate, bCreateBitChannels, parent);
		}

		internal void Import(ArrayTypeBorder dim1, ArrayTypeBorder dim2, ArrayTypeBorder dim3, string baseTypeName, DataElementFactory factory, string stIdentifier, StringRef visibleName, StringRef unit, StringRef description, string[] filterFlags, LList<ValueElement> defaultValue, bool bUpdate, bool bCreateBitChannels, IDataElementParent parent)
		{
			int result = 0;
			int result2 = 0;
			int result3 = 0;
			int result4 = 0;
			int result5 = 0;
			int result6 = 0;
			if (base.SubElementCollection == null)
			{
				base.SubElementCollection = new DataElementCollection();
			}
			ImportBase(stIdentifier, visibleName, unit, description, filterFlags, new CustomItemList(), bUpdate, parent);
			if (dim1 != null)
			{
				_stDimenstion1LowerBorder = dim1.LowerBorder;
				_stDimenstion1UpperBorder = dim1.UpperBorder;
				int.TryParse(_stDimenstion1LowerBorder, out result);
				int.TryParse(_stDimenstion1UpperBorder, out result2);
				if (result > result2)
				{
					throw new ArgumentException("FirstDimension: LowerBorder greater than UpperBorder");
				}
			}
			if (dim2 != null)
			{
				_stDimenstion2LowerBorder = dim2.LowerBorder;
				_stDimenstion2UpperBorder = dim2.UpperBorder;
				int.TryParse(_stDimenstion2LowerBorder, out result3);
				int.TryParse(_stDimenstion2UpperBorder, out result4);
				if (result3 > result4)
				{
					throw new ArgumentException("SecondDimension: LowerBorder greater than UpperBorder");
				}
			}
			if (dim3 != null)
			{
				_stDimenstion3LowerBorder = dim3.LowerBorder;
				_stDimenstion3UpperBorder = dim3.UpperBorder;
				int.TryParse(_stDimenstion3LowerBorder, out result5);
				int.TryParse(_stDimenstion3UpperBorder, out result6);
				if (result5 > result6)
				{
					throw new ArgumentException("ThirdDimension: LowerBorder greater than UpperBorder");
				}
			}
			DataElementCollection dataElementCollection = (DataElementCollection)((GenericObject)base.SubElementCollection).Clone();
			base.SubElementCollection.Clear();
			int num = 0;
			for (int i = result; i <= result2; i++)
			{
				for (int j = result3; j <= result4; j++)
				{
					for (int k = result5; k <= result6; k++)
					{
						DataElementBase dataElementBase = null;
						string stIdentifier2 = stIdentifier.Replace("_", "") + "_" + i + "_" + j + "_" + k;
						if (bUpdate && dataElementCollection.Contains(stIdentifier2))
						{
							dataElementBase = (DataElementBase)(object)dataElementCollection[stIdentifier2];
						}
						ValueElement valueElement = ((defaultValue == null || defaultValue.Count <= num) ? new ValueElement() : defaultValue[num]);
						LList<ValueElement> val = new LList<ValueElement>();
						val.Add(valueElement);
						DataElementBase element = factory.Create(stIdentifier2, val, baseTypeName, visibleName, unit, description, filterFlags, this, dataElementBase, dataElementBase != null, bCreateBitChannels);
						base.SubElementCollection.Add((IDataElement)(object)element);
						num++;
					}
				}
			}
		}

		internal override void SetDefault(ValueElement defaultValue)
		{
			foreach (ValueElement subElement in defaultValue.SubElements)
			{
				((DataElementBase)(object)base.SubElementCollection[subElement.Name])?.SetDefault(subElement);
			}
		}

		public override void SetValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on array types");
		}

		public override void SetMinValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on array types");
		}

		public override void SetMaxValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on array types");
		}

		public override void SetDefaultValue(string stValue)
		{
			throw new InvalidOperationException("Operation not allowed on array types");
		}
	}
}
