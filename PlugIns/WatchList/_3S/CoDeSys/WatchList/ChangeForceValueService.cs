#define DEBUG
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Online;
using _3S.CoDeSys.OnlineUI;

namespace _3S.CoDeSys.WatchList
{
	[TypeGuid("{781B4FAF-2BE2-4496-806E-D41836C2828D}")]
	public class ChangeForceValueService : IChangeForceValueService2, IChangeForceValueService
	{
		public bool Invoke(string stExpression, string stType, string stCurrentValue, ref object oPreparedValue, bool bForced)
		{
			ICompiledType val = null;
			try
			{
				IScanner val2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stType, false, false, false, false);
				val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val2).ParseTypeDeclaration();
			}
			catch (Exception)
			{
				return false;
			}
			return Invoke(stExpression, val, stCurrentValue, ref oPreparedValue, bForced, Guid.Empty);
		}

		public bool Invoke(string stExpression, ICompiledType cType, string stCurrentValue, ref object oPreparedValue, bool bForced, Guid application)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			ICompiledType val = cType;
			if ((int)((IType)val).Class == 23 && cType.BaseType is IEnumType2)
			{
				val = cType.BaseType;
			}
			bool flag = (int)((IType)val).Class == 25;
			TextValueDataDialog textValueDataDialog = new TextValueDataDialog(flag, null);
			object unforce = null;
			string text = "";
			if (oPreparedValue == PreparedValues.Unforce || oPreparedValue == PreparedValues.UnforceAndRestore)
			{
				unforce = oPreparedValue;
			}
			else if (oPreparedValue != null)
			{
				try
				{
					text = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterToIEC(true, true, (DisplayMode)1).GetLiteralText(oPreparedValue, ((IType)val).Class);
				}
				catch
				{
					text = ((!(oPreparedValue is IConvertible)) ? oPreparedValue.ToString() : ((IConvertible)oPreparedValue).ToString(NumberFormatInfo.InvariantInfo));
				}
			}
			if (text == string.Empty)
			{
				text = stCurrentValue;
			}
			textValueDataDialog.Initialize(stExpression, val, stCurrentValue, unforce, text, application);
			textValueDataDialog.UsageForUnforcedVariables = !bForced;
			if (textValueDataDialog.ShowDialog((IWin32Window)APEnvironment.FrameForm) == DialogResult.OK)
			{
				if (textValueDataDialog.Unforce != null)
				{
					oPreparedValue = textValueDataDialog.Unforce;
				}
				else
				{
					string preparedValue = textValueDataDialog.PreparedValue;
					if (preparedValue == null)
					{
						oPreparedValue = null;
					}
					else if (preparedValue.Length > 0)
					{
						object obj2 = null;
						try
						{
							IConverterFromIEC converterFromIEC = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetConverterFromIEC();
							Debug.Assert(converterFromIEC != null);
							TypeClass val2 = default(TypeClass);
							converterFromIEC.GetLiteralValue(preparedValue, out obj2, out val2);
							oPreparedValue = obj2;
						}
						catch (FormatException)
						{
							if (flag)
							{
								oPreparedValue = preparedValue;
							}
						}
						catch
						{
							oPreparedValue = null;
						}
					}
				}
				return true;
			}
			return false;
		}
	}
}
