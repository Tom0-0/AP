#define DEBUG
using System;
using System.Diagnostics;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;

namespace _3S.CoDeSys.WatchList
{
	internal class TextValueData : ValueData
	{
		private string _stText;

		private bool _bForced;

		private object _unforce;

		public override object Value
		{
			get
			{
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Invalid comparison between Unknown and I4
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Invalid comparison between Unknown and I4
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Invalid comparison between Unknown and I4
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bf: Invalid comparison between Unknown and I4
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				if (_unforce != null)
				{
					return _unforce;
				}
				if (_stText != null && _stText.Length > 0)
				{
					try
					{
						string text = _stText;
						if (base.Node != null && base.Node.OnlineVarRef != null && base.Node.OnlineVarRef.Expression != null && base.Node.OnlineVarRef.Expression.Type != null)
						{
							ICompiledType val = base.Node.OnlineVarRef.Expression.Type;
							if ((int)((IType)val).Class == 23)
							{
								val = val.BaseType;
							}
							TypeClass @class = ((IType)val).Class;
							if ((int)@class != 16)
							{
								if ((int)@class == 17)
								{
									if (!text.Trim().StartsWith("\""))
									{
										text = "\"" + text;
									}
									if (!text.Trim().EndsWith("\""))
									{
										text += "\"";
									}
								}
							}
							else
							{
								if (!text.Trim().StartsWith("'"))
								{
									text = "'" + text;
								}
								if (!text.Trim().EndsWith("'"))
								{
									text += "'";
								}
							}
							if (text.ToUpperInvariant().Contains("$R"))
							{
								text = text.Replace("$R", "$$R");
								text = text.Replace("$r", "$$r");
							}
							if (text.ToUpperInvariant().Contains("$N"))
							{
								text = text.Replace("$N", "$$N");
								text = text.Replace("$n", "$$n");
							}
							IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
							Debug.Assert(converterFromIEC != null);
							object result = default(object);
							TypeClass val2 = default(TypeClass);
							if (converterFromIEC is IConverterFromIEC2 && (int)((IType)val).Class == 14)
							{
								((IConverterFromIEC2)converterFromIEC).GetLiteralValue(text, out result, out val2, ((IType)val).Class);
							}
							else
							{
								converterFromIEC.GetLiteralValue(text, out result, out val2);
							}
							return result;
						}
					}
					catch
					{
						return _stText;
					}
				}
				return null;
			}
		}

		public override string Text => _stText;

		public override bool Toggleable
		{
			get
			{
				try
				{
					object value = Value;
					return value != null && value is bool;
				}
				catch
				{
					return false;
				}
			}
		}

		public override bool Constant => false;

		public override bool Forced => _bForced;

		public override bool IsEnum
		{
			get
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Invalid comparison between Unknown and I4
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Invalid comparison between Unknown and I4
				if (base.Node != null && base.Node.OnlineVarRef != null && base.Node.OnlineVarRef.Expression != null && base.Node.OnlineVarRef.Expression.Type != null)
				{
					ICompiledType val = base.Node.OnlineVarRef.Expression.Type;
					if ((int)((IType)val).Class == 23)
					{
						val = val.BaseType;
					}
					if ((int)((IType)val).Class == 25)
					{
						return true;
					}
				}
				return false;
			}
		}

		public TextValueData(WatchListNode node, string stText, bool bForced)
			: base(node)
		{
			_stText = stText;
			_bForced = bForced;
		}

		public TextValueData(WatchListNode node, object unforce)
			: base(node)
		{
			if (unforce == null)
			{
				throw new ArgumentNullException("unforce");
			}
			if (unforce != PreparedValues.Unforce && unforce != PreparedValues.UnforceAndRestore)
			{
				throw new ArgumentException("unforce must be 'Unforce' or 'UnforceAndRestore'.");
			}
			_stText = string.Empty;
			_bForced = true;
			_unforce = unforce;
		}
	}
}
