using System.Collections;
using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	public class ChannelMap
	{
		private ArrayList _alMappings = new ArrayList();

		private uint _uiParameterId;

		private IDirectVariable _iecAddress;

		private ushort _wParamBitoffset;

		private ushort _wIecAddrBitoffset;

		private uint _uiSize;

		private bool _bInput;

		private string _stType;

		private string _stParentType = string.Empty;

		private long _LanguageModelPositionId;

		private bool _bAlwaysMapping;

		private string _stComment = string.Empty;

		private IDataElement _dataElement;

		private AlwaysMappingMode _mappingMode;

		private bool _bReadOnly;

		public string Type
		{
			get
			{
				return _stType;
			}
			set
			{
				_stType = value;
			}
		}

		public string ParentType
		{
			get
			{
				return _stParentType;
			}
			set
			{
				_stParentType = value;
			}
		}

		public IDirectVariable IecAddress
		{
			get
			{
				return _iecAddress;
			}
			set
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Invalid comparison between Unknown and I4
				_iecAddress = value;
				if (_iecAddress != null && (int)_iecAddress.Size == 1 && _uiSize > 1 && APEnvironment.CompilerVersionMgr.CompilerVersionGreaterEq((ushort)3, (ushort)5, (ushort)8, (ushort)0))
				{
					_uiSize = 1u;
				}
			}
		}

		public ushort ParamBitoffset
		{
			get
			{
				return _wParamBitoffset;
			}
			set
			{
				_wParamBitoffset = value;
			}
		}

		public ushort IecBitoffset
		{
			get
			{
				return _wIecAddrBitoffset;
			}
			set
			{
				_wIecAddrBitoffset = value;
			}
		}

		public ulong ParameterId => _uiParameterId;

		public uint BitSize => _uiSize;

		public bool IsInput => _bInput;

		public bool AlwaysMapping => _bAlwaysMapping;

		public AlwaysMappingMode AlwaysMappingMode => _mappingMode;

		public long LanguageModelPositionId
		{
			get
			{
				return _LanguageModelPositionId;
			}
			set
			{
				_LanguageModelPositionId = value;
			}
		}

		public bool ReadOnly => _bReadOnly;

		public string Comment
		{
			get
			{
				return _stComment;
			}
			set
			{
				_stComment = value;
			}
		}

		public IDataElement DataElement => _dataElement;

		public ChannelMap(long lParameterId, long lSize, bool bInput, bool bReadOnly, bool bAlwaysMapping, IDataElement dataElement, AlwaysMappingMode mappingMode)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			_uiParameterId = (uint)lParameterId;
			_uiSize = (uint)lSize;
			_bInput = bInput;
			_bAlwaysMapping = bAlwaysMapping;
			_bReadOnly = bReadOnly;
			_dataElement = dataElement;
			_mappingMode = mappingMode;
		}

		public void AddVariableMapping(VariableMapping mapping)
		{
			_alMappings.Add(mapping);
		}

		public VariableMapping[] GetVariableMappings()
		{
			VariableMapping[] array = new VariableMapping[_alMappings.Count];
			_alMappings.CopyTo(array, 0);
			return array;
		}

		private string GetIecAddress()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected I4, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("%");
			DirectVariableLocation location = _iecAddress.Location;
			switch ((int)location - 1)
			{
			case 0:
				stringBuilder.Append("I");
				break;
			case 1:
				stringBuilder.Append("O");
				break;
			case 2:
				stringBuilder.Append("M");
				break;
			}
			DirectVariableSize size = _iecAddress.Size;
			stringBuilder.Append(size.ToString());
			stringBuilder.Append(_iecAddress.Components[0].ToString());
			return stringBuilder.ToString();
		}
	}
}
