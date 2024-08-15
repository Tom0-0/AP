using System;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{b955cc99-aa13-473b-b43f-7c76a865c138}")]
	[StorageVersion("3.3.0.0")]
	internal class EnumValue : GenericObject2, IEnumerationValue
	{
		[DefaultSerialization("Name")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private StringRef _name;

		[DefaultSerialization("Description")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private StringRef _description;

		[DefaultSerialization("Identifier")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stIdentifier;

		[DefaultSerialization("Value")]
		[StorageVersion("3.3.0.0")]
		[DefaultDuplication(DuplicationMethod.Deep)]
		private string _stValue;

		private bool _bIsCompValid;

		private IComparable _comp;

		private IDataElementParent _parent;

		public string Description
		{
			get
			{
				string @default = _description.Default;
				Parameter parameter = _parent as Parameter;
				if (parameter == null && _parent != null)
				{
					parameter = _parent.GetParameter();
				}
				if (parameter != null && parameter.Owner != null)
				{
					try
					{
						IStringTable stringTable = parameter.Owner.StringTable;
						IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
						if (val == null)
						{
							return @default;
						}
						val.ResolveString(_description.Namespace, _description.Identifier, _description.Default, out @default);
						return @default;
					}
					catch
					{
						return @default;
					}
				}
				return @default;
			}
		}

		public string VisibleName
		{
			get
			{
				string @default = _name.Default;
				Parameter parameter = _parent as Parameter;
				if (parameter == null && _parent != null)
				{
					parameter = _parent.GetParameter();
				}
				if (parameter != null && parameter.Owner != null)
				{
					try
					{
						IStringTable stringTable = parameter.Owner.StringTable;
						IStringTable2 val = (IStringTable2)(object)((stringTable is IStringTable2) ? stringTable : null);
						if (val == null)
						{
							return @default;
						}
						val.ResolveString(_name.Namespace, _name.Identifier, _name.Default, out @default);
						return @default;
					}
					catch
					{
						return @default;
					}
				}
				return @default;
			}
		}

		public string Identifier => _stIdentifier;

		public string Value => _stValue;

		public IComparable ComparableValue
		{
			get
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				if (!_bIsCompValid)
				{
					try
					{
						string text = string.Empty;
						if (_parent != null && _parent is IDataElement)
						{
							IDataElementParent parent = _parent;
							text = ((IDataElement)((parent is IDataElement) ? parent : null)).BaseType;
							ICompiledType val = Types.ParseType(text);
							text = ((val == null || (int)((IType)val).Class != 0) ? (text + "#") : ((!(Value == "0") && !(Value == "1")) ? string.Empty : (text + "#")));
						}
						object obj = default(object);
						TypeClass val2 = default(TypeClass);
						((ILanguageModelManager)APEnvironment.LanguageModelMgr).GetConverterFromIEC().GetLiteralValue(text.ToUpperInvariant() + _stValue, out obj, out val2);
						_comp = obj as IComparable;
					}
					catch
					{
					}
					_bIsCompValid = true;
				}
				return _comp;
			}
		}

		internal IDataElementParent Parent
		{
			set
			{
				_parent = value;
			}
		}

		internal static EnumValue CreateDummyValue(string stValue)
		{
			return new EnumValue
			{
				_description = new StringRef("3S_INTERN_NO_USE", "xxx", "Unknown enum value"),
				_name = new StringRef("3S_INTERN_NO_USE", "xxx", stValue),
				_stIdentifier = "",
				_stValue = stValue
			};
		}

		public EnumValue()
			: base()
		{
		}

		public EnumValue(EnumTypeValue type)
			: this()
		{
			_description = type.Description;
			_name = type.Name;
			_stIdentifier = type.Identifier;
			_stValue = type.Value;
		}

		private EnumValue(EnumValue original)
			: this()
		{
			_name = (StringRef)((GenericObject)original._name).Clone();
			_description = (StringRef)((GenericObject)original._description).Clone();
			_stIdentifier = original._stIdentifier;
			_stValue = original._stValue;
		}

		public EnumValue(StringRef name, StringRef description, string identifier, string value)
			: this()
		{
			_name = (StringRef)((GenericObject)name).Clone();
			_description = (StringRef)((GenericObject)description).Clone();
			_stIdentifier = identifier;
			_stValue = value;
		}

		public override object Clone()
		{
			EnumValue enumValue = new EnumValue(this);
			((GenericObject)enumValue).AfterClone();
			return enumValue;
		}

		public override string ToString()
		{
			return VisibleName;
		}
	}
}
