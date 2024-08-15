using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class Mapping : ITaskMapping
	{
		private uint _uiParamId;

		private ParameterSet _paramSet;

		private string _stVarAddr;

		private int _nParamBitOffset;

		private int _nAddrBitOffset;

		private int _nBitSize;

		private long _lBitOffset;

		private long _lStartbitOffset;

		private string _stBaseType = string.Empty;

		private string _stExistingVar = string.Empty;

		private string _stIecAddr = string.Empty;

		private bool _bMapToExisting;

		private string _stUniqueVarName = string.Empty;

		private IDataElement _dataElement;

		private VariableMapping _var;

		public uint ParameterId => _uiParamId;

		public int ParameterBitOffset
		{
			get
			{
				return _nParamBitOffset;
			}
			set
			{
				_nParamBitOffset = value;
			}
		}

		public int IecAddrBitOffset
		{
			get
			{
				return _nAddrBitOffset;
			}
			set
			{
				_nAddrBitOffset = value;
			}
		}

		public long StartBit
		{
			get
			{
				if (_stVarAddr.StartsWith("ADR(%QX"))
				{
					return _lStartbitOffset;
				}
				return _lStartbitOffset + _nAddrBitOffset;
			}
		}

		public int BitSize
		{
			get
			{
				return _nBitSize;
			}
			set
			{
				_nBitSize = value;
			}
		}

		public long BitOffset
		{
			get
			{
				return _lBitOffset;
			}
			set
			{
				_lBitOffset = value;
			}
		}

		public long StartbitOffset => _lStartbitOffset;

		public bool MapToExisiting => _bMapToExisting;

		public string IecVar
		{
			get
			{
				return _stIecAddr;
			}
			set
			{
				_stIecAddr = value;
			}
		}

		public string ExistingVar => _stExistingVar;

		public string MappedAddress
		{
			get
			{
				return _stVarAddr;
			}
			set
			{
				_stVarAddr = value;
			}
		}

		internal string BaseType => _stBaseType;

		internal string UniqueVarName
		{
			get
			{
				return _stUniqueVarName;
			}
			set
			{
				_stUniqueVarName = value;
			}
		}

		public IDataElement DataElement
		{
			get
			{
				return _dataElement;
			}
			set
			{
				_dataElement = value;
			}
		}

		public VariableMapping VarMapping => _var;

		public ParameterSet ParamSet => _paramSet;

		public string DataType
		{
			get
			{
				return _stBaseType;
			}
			set
			{
				_stBaseType = value;
			}
		}

		public Mapping(ParameterSet paramSet, uint uiParamId, string stAddr, int nParamBitOffset, int nAddrBitOffset, int nBitSize, long lBitOffset, string stBaseType, long lStartbitOffset, IDataElement dataElement)
		{
			_uiParamId = uiParamId;
			_nParamBitOffset = nParamBitOffset;
			_nAddrBitOffset = nAddrBitOffset;
			_nBitSize = nBitSize;
			_lBitOffset = lBitOffset;
			_stIecAddr = stAddr;
			_stVarAddr = $"ADR({stAddr})";
			_lStartbitOffset = lStartbitOffset;
			_stBaseType = stBaseType;
			_dataElement = dataElement;
			_paramSet = paramSet;
		}

		public Mapping(ParameterSet paramSet, uint uiParamId, VariableMapping var, int nParamBitOffset, int nAddrBitOffset, int nBitSize, string stBaseType, long lStartbitOffset, IDataElement dataElement)
		{
			_uiParamId = uiParamId;
			_nParamBitOffset = nParamBitOffset;
			if (var.IsUnusedMapping)
			{
				nAddrBitOffset = 0;
			}
			_nAddrBitOffset = nAddrBitOffset;
			_nBitSize = nBitSize;
			_bMapToExisting = true;
			_var = var;
			_stExistingVar = var.GetPlainVariableName();
			_stVarAddr = $"ADR({_stExistingVar})";
			_stBaseType = stBaseType;
			_lStartbitOffset = lStartbitOffset;
			_dataElement = dataElement;
			_paramSet = paramSet;
		}

		public void AddToLanguageModel(StringBuilder sbChannelMap, int nModuleIndex, ref bool bFirst)
		{
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			if (bFirst)
			{
				bFirst = false;
			}
			else
			{
				sbChannelMap.Append(",\n");
			}
			string text = string.Empty;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)5, (ushort)0) && _paramSet != null)
			{
				uint num = default(uint);
				if (_paramSet.ParamIdToIndex.TryGetValue((long)_uiParamId, out num))
				{
					text = ((!APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)11, (ushort)0)) ? $"ADR(moduleList[{nModuleIndex}].pParameterList[{num}])" : string.Format("ADR(GVL_{0}.{0}[{1}])", _paramSet.ParamsListName, num));
				}
			}
			else
			{
				text = $"IoMgrConfigGetParameter(ADR(moduleList[{nModuleIndex}]), {_uiParamId})";
			}
			ushort num2 = ushort.MaxValue;
			if (APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)2, (ushort)1, (ushort)20))
			{
				if (_stBaseType != string.Empty)
				{
					num2 = (ushort)((Types.GetTypeId(_stBaseType, ushort.MaxValue) & 0xFFu) | 0x100u);
				}
			}
			else
			{
				num2 = 0;
			}
			if ((num2 & 0xFF) == 26 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)4, (ushort)20))
			{
				ICompiledType val = Types.ParseType(_stBaseType);
				if (val != null && val.BaseType != null)
				{
					num2 = (ushort)(256 + (ushort)((IType)val.BaseType).Class);
				}
			}
			sbChannelMap.AppendFormat("\t(pParameter := {0}, pbyIecAddress := {1}, wParameterBitOffset := {2}, wIecAddressBitOffset := {3}, wSize := {4}, wDummy := {5})", text, _stVarAddr, _nParamBitOffset, _nAddrBitOffset, _nBitSize, num2);
		}
	}
}
