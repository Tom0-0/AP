using System;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class RawValueData : ValueData
	{
		private TypeClass _typeClass;

		private TypeClass _baseTypeClass;

		private object _value;

		private IConverterToIEC _converter;

		private bool _bConstant;

		private bool _bForced;

		public override object Value => _value;

		public override string Text
		{
			get
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Invalid comparison between Unknown and I4
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Invalid comparison between Unknown and I4
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected O, but got Unknown
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Invalid comparison between Unknown and I4
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Invalid comparison between Unknown and I4
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Invalid comparison between I4 and Unknown
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Invalid comparison between I4 and Unknown
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					if (_value != null)
					{
						if ((int)_typeClass == 29)
						{
							return _value.ToString();
						}
						if ((int)_typeClass == 26)
						{
							LStringBuilder val = new LStringBuilder();
							val.Append("[");
							object[] array = _value as object[];
							if (array != null)
							{
								for (int i = 0; i < array.Length; i++)
								{
									if (i > 0)
									{
										val.Append(",");
									}
									val.Append(_converter.GetLiteralText(array[i], _baseTypeClass));
								}
							}
							val.Append("]");
							return ((object)val).ToString();
						}
						if ((int)_typeClass == 24)
						{
							return _converter.GetLiteralText(_value, _baseTypeClass);
						}
						if ((int)_typeClass != 28)
						{
							if (14 == (int)_typeClass || 15 == (int)_typeClass)
							{
								IConverterToIEC5 val2 = null;
								int num = -1;
								num = GlobalOptionsHelper.DecimalPlaces;
								if (-1 != num)
								{
									IConverterToIEC converter = _converter;
									val2 = (IConverterToIEC5)(object)((converter is IConverterToIEC5) ? converter : null);
									if (val2 != null)
									{
										return val2.GetLiteralText(_value, _typeClass, num);
									}
								}
							}
							return _converter.GetLiteralText(_value, _typeClass);
						}
						return string.Empty;
					}
					return string.Empty;
				}
				catch (Exception ex)
				{
					return string.Format("<0>", ex.Message);
				}
			}
		}

		public override bool Toggleable
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Invalid comparison between Unknown and I4
				if ((int)_typeClass != 0)
				{
					return (int)_typeClass == 1;
				}
				return true;
			}
		}

		public override bool Constant => _bConstant;

		public override bool Forced => _bForced;

		public RawValueData(TypeClass typeClass, object value, IConverterToIEC converter, bool bConstant, bool bForced)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			_typeClass = typeClass;
			_value = value;
			_converter = converter;
			_bConstant = bConstant;
			_bForced = bForced;
		}

		public RawValueData(TypeClass typeClass, TypeClass baseType, object value, IConverterToIEC converter, bool bConstant, bool bForced)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			_typeClass = typeClass;
			_baseTypeClass = baseType;
			_value = value;
			_converter = converter;
			_bConstant = bConstant;
			_bForced = bForced;
		}
	}
}
