using System;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	internal class RawValueData : ValueData
	{
		private TypeClass _typeClass;

		private object _value;

		private IConverterToIEC _converter;

		private bool _bConstant;

		private bool _bForced;

		public override object Value => _value;

		public override string Text
		{
			get
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Invalid comparison between Unknown and I4
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Invalid comparison between I4 and Unknown
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Invalid comparison between I4 and Unknown
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				string result = string.Empty;
				try
				{
					if (_value != null)
					{
						if (_converter is IConverterToIEC4 && (int)_typeClass == 25 && base.Node.OnlineVarRef.Expression != null && base.Node.OnlineVarRef.Expression.Type != null && base.Node.OnlineVarRef.Expression.Type.BaseType != null)
						{
							result = ((IConverterToIEC4)_converter).GetLiteralTextForEnum(_value, ((IType)base.Node.OnlineVarRef.Expression.Type.BaseType).Class);
						}
						else
						{
							IConverterToIEC5 val = null;
							int num = -1;
							if (14 == (int)_typeClass || 15 == (int)_typeClass)
							{
								num = GlobalOptionsHelper.DecimalPlaces;
								if (-1 != num)
								{
									IConverterToIEC converter = _converter;
									val = (IConverterToIEC5)(object)((converter is IConverterToIEC5) ? converter : null);
								}
							}
							result = ((val != null) ? val.GetLiteralText(_value, _typeClass, num) : _converter.GetLiteralText(_value, _typeClass));
						}
					}
					return result;
				}
				catch (Exception ex)
				{
					if (_value is string)
					{
						return (string)_value;
					}
					return $"{ex.Message}";
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

		public override bool IsEnum
		{
			get
			{
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Invalid comparison between Unknown and I4
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Invalid comparison between Unknown and I4
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Invalid comparison between Unknown and I4
				if (base.Node != null && base.Node.OnlineVarRef != null && base.Node.OnlineVarRef.Expression != null && base.Node.OnlineVarRef.Expression.Type != null)
				{
					if ((int)((IType)base.Node.OnlineVarRef.Expression.Type).Class == 25)
					{
						return true;
					}
					if ((int)((IType)base.Node.OnlineVarRef.Expression.Type).Class == 23 && base.Node.OnlineVarRef.Expression.Type.BaseType != null)
					{
						return (int)((IType)base.Node.OnlineVarRef.Expression.Type.BaseType).Class == 25;
					}
				}
				return false;
			}
		}

		internal override TypeClass TypeClass => _typeClass;

		public RawValueData(WatchListNode node, TypeClass typeClass, object value, IConverterToIEC converter, bool bConstant, bool bForced)
			: base(node)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
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
	}
}
