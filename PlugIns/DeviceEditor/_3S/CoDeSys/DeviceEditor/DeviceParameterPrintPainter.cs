using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using _3S.CoDeSys.Core.Printing;
using _3S.CoDeSys.Core.TargetSettings;
using _3S.CoDeSys.DeviceObject;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class DeviceParameterPrintPainter : IPrintPainterEx
	{
		internal class DeviceParameterPrintParam
		{
			public List<string> liData = new List<string>();
		}

		internal Image _image;

		internal List<DeviceParameterPrintParam> _liData = new List<DeviceParameterPrintParam>();

		private int _iLastLine;

		private IEnumerable<IDeviceParameterPrintFactory> _factories = APEnvironment.CreateDeviceParameterPrintFactories();

		private List<float> _liTabs = new List<float>();

		internal Image DeviceImage
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		internal DeviceParameterPrintPainter()
		{
		}

		internal void PrintParameter(ParameterEditor editor, IParameter parameter, IDataElement2 element, string stIdent)
		{
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Expected O, but got Unknown
			string text = ((IDataElement)element).Value;
			string text2 = ((IDataElement)element).VisibleName;
			string text3 = ((IDataElement)element).DefaultValue;
			string text4 = ((IDataElement)element).Unit;
			string text5 = ((IDataElement)element).Description;
			string text6 = null;
			if (_factories != null)
			{
				foreach (IDeviceParameterPrintFactory factory in _factories)
				{
					if (factory.ChangeDevicePrintParameters(editor.GetDeviceObject(bToModify: false), editor.GetParameterSetProvider().GetParameterSet(bToModify: false)))
					{
						text = factory.Value(parameter, (IDataElement)(object)element, text);
						text2 = factory.VisibleName(parameter, (IDataElement)(object)element, text2);
						text3 = factory.DefaultValue(parameter, (IDataElement)(object)element, text3);
						text4 = factory.Unit(parameter, (IDataElement)(object)element, text4);
						text5 = factory.Description(parameter, (IDataElement)(object)element, text5);
						if (element.HasBaseType)
						{
							text6 = factory.BaseType(parameter, (IDataElement)(object)element, ((IDataElement)element).BaseType);
						}
					}
				}
			}
			DeviceParameterPrintParam deviceParameterPrintParam = new DeviceParameterPrintParam();
			_liData.Add(deviceParameterPrintParam);
			deviceParameterPrintParam.liData.Add(stIdent + text2);
			if (element.HasBaseType)
			{
				if (element is IEnumerationDataElement)
				{
					text6 = ((IDataElement)element).GetTypeString();
				}
				else if (string.IsNullOrEmpty(text6))
				{
					text6 = ((IDataElement)element).BaseType;
				}
				deviceParameterPrintParam.liData.Add(text6);
			}
			else
			{
				deviceParameterPrintParam.liData.Add(string.Empty);
			}
			stIdent += "   ";
			if (element.HasBaseType && !string.IsNullOrEmpty(text) && !((IDataElement)element).HasSubElements)
			{
				if (text.Length > 1000)
				{
					int num;
					for (int i = 0; i < text.Length; i += num)
					{
						num = text.Length - i;
						if (num > 20)
						{
							num = 20;
						}
						if (i == 0)
						{
							deviceParameterPrintParam.liData.Add(text.Substring(0, num));
							continue;
						}
						DeviceParameterPrintParam deviceParameterPrintParam2 = new DeviceParameterPrintParam();
						_liData.Add(deviceParameterPrintParam2);
						deviceParameterPrintParam2.liData.Add(string.Empty);
						deviceParameterPrintParam2.liData.Add(string.Empty);
						deviceParameterPrintParam2.liData.Add(text.Substring(i, num));
						deviceParameterPrintParam2.liData.Add(string.Empty);
						deviceParameterPrintParam2.liData.Add(string.Empty);
					}
				}
				else
				{
					deviceParameterPrintParam.liData.Add(text);
				}
			}
			else
			{
				deviceParameterPrintParam.liData.Add(string.Empty);
			}
			if (element.HasBaseType && !string.IsNullOrEmpty(text3) && !((IDataElement)element).HasSubElements)
			{
				if (text3.Length > 1000)
				{
					int num2;
					for (int j = 0; j < text3.Length; j += num2)
					{
						num2 = text3.Length - j;
						if (num2 > 20)
						{
							num2 = 20;
						}
						if (j == 0)
						{
							deviceParameterPrintParam.liData.Add(text3.Substring(0, num2));
							continue;
						}
						DeviceParameterPrintParam deviceParameterPrintParam3 = new DeviceParameterPrintParam();
						_liData.Add(deviceParameterPrintParam3);
						deviceParameterPrintParam3.liData.Add(string.Empty);
						deviceParameterPrintParam3.liData.Add(string.Empty);
						deviceParameterPrintParam3.liData.Add(text3.Substring(j, num2));
						deviceParameterPrintParam3.liData.Add(string.Empty);
						deviceParameterPrintParam3.liData.Add(string.Empty);
					}
				}
				else
				{
					deviceParameterPrintParam.liData.Add(text3);
				}
			}
			else
			{
				deviceParameterPrintParam.liData.Add(string.Empty);
			}
			deviceParameterPrintParam.liData.Add(text4);
			deviceParameterPrintParam.liData.Add(text5);
			if (!((IDataElement)element).HasSubElements)
			{
				return;
			}
			string stIdent2 = stIdent + "   ";
			foreach (IDataElement2 item in (IEnumerable)((IDataElement)element).SubElements)
			{
				IDataElement2 element2 = item;
				PrintParameter(editor, parameter, element2, stIdent2);
			}
		}

		internal void PrintMappingParameter(IDataElement2 element, IDataElement2 parentElement, string stIdent, ref bool bFirst, bool bInput, bool bMotorolaBitfields)
		{
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Expected O, but got Unknown
			DeviceParameterPrintParam deviceParameterPrintParam = new DeviceParameterPrintParam();
			string text = string.Empty;
			if (((ICollection)((IDataElement)element).IoMapping.VariableMappings).Count > 0)
			{
				text = ((IDataElement)element).IoMapping.VariableMappings[0]
					.Variable;
				deviceParameterPrintParam.liData.Add(text);
			}
			else
			{
				deviceParameterPrintParam.liData.Add(string.Empty);
			}
			deviceParameterPrintParam.liData.Add(((IDataElement)element).VisibleName);
			if (element.HasBaseType)
			{
				deviceParameterPrintParam.liData.Add(((IDataElement)element).BaseType);
			}
			else
			{
				deviceParameterPrintParam.liData.Add(string.Empty);
			}
			if (!text.Contains("."))
			{
				string iecAddress = DeviceHelper.GetIecAddress((IDataElement)(object)element, (IDataElement)(object)parentElement, bMotorolaBitfields);
				deviceParameterPrintParam.liData.Add(iecAddress);
			}
			else
			{
				deviceParameterPrintParam.liData.Add(string.Empty);
			}
			deviceParameterPrintParam.liData.Add(((IDataElement)element).Unit);
			deviceParameterPrintParam.liData.Add(((IDataElement)element).Description);
			if (!string.IsNullOrEmpty(text))
			{
				if (!bFirst)
				{
					bFirst = true;
					DeviceParameterPrintParam deviceParameterPrintParam2 = new DeviceParameterPrintParam();
					if (bInput)
					{
						deviceParameterPrintParam2.liData.Add(Strings.Print_Input_Parameters);
					}
					else
					{
						deviceParameterPrintParam2.liData.Add(Strings.Print_Output_Parameters);
					}
					_liData.Add(deviceParameterPrintParam2);
					deviceParameterPrintParam2 = new DeviceParameterPrintParam();
					deviceParameterPrintParam2.liData.Add(Strings.Print_Parameter_Mapping);
					deviceParameterPrintParam2.liData.Add(Strings.Print_Parameter_Channel);
					deviceParameterPrintParam2.liData.Add(Strings.Print_Parameter_TypeTab);
					deviceParameterPrintParam2.liData.Add(Strings.Print_Parameter_Address);
					deviceParameterPrintParam2.liData.Add(Strings.Print_Parameter_UnitTab);
					deviceParameterPrintParam2.liData.Add(Strings.Print_Parameter_DescriptionTab);
					_liData.Add(deviceParameterPrintParam2);
				}
				_liData.Add(deviceParameterPrintParam);
			}
			if (!((IDataElement)element).HasSubElements)
			{
				return;
			}
			string stIdent2 = stIdent + "   ";
			foreach (IDataElement2 item in (IEnumerable)((IDataElement)element).SubElements)
			{
				IDataElement2 element2 = item;
				PrintMappingParameter(element2, element, stIdent2, ref bFirst, bInput, bMotorolaBitfields);
			}
		}

		internal DeviceParameterPrintPainter(ParameterEditor editor, HideParameterDelegate paramFilter)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (editor == null)
			{
				throw new ArgumentNullException("connector");
			}
			IParameterSet parameterSet = editor.GetParameterSetProvider().GetParameterSet(bToModify: false);
			if (parameterSet == null)
			{
				return;
			}
			bool flag = false;
			foreach (IParameter item2 in (IEnumerable)parameterSet)
			{
				IParameter val = item2;
				if ((paramFilter == null || !paramFilter.Invoke((int)val.Id, (string[])null)) && (int)val.ChannelType == 0 && (int)val.GetAccessRight(false) != 0)
				{
					if (!flag)
					{
						flag = true;
						DeviceParameterPrintParam item = new DeviceParameterPrintParam
						{
							liData = { Strings.Print_Parameters }
						};
						_liData.Add(item);
						item = new DeviceParameterPrintParam
						{
							liData = 
							{
								Strings.Print_Parameter_Name,
								Strings.Print_Parameter_TypeTab,
								Strings.Print_Parameter_ValueTab,
								Strings.Print_Parameter_DefaultValueTab,
								Strings.Print_Parameter_UnitTab,
								Strings.Print_Parameter_DescriptionTab
							}
						};
						_liData.Add(item);
					}
					PrintParameter(editor, val, (IDataElement2)(object)((val is IDataElement2) ? val : null), string.Empty);
				}
			}
		}

		internal DeviceParameterPrintPainter(IOMappingEditor editor, HideParameterDelegate paramFilter)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Invalid comparison between Unknown and I4
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Invalid comparison between Unknown and I4
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Invalid comparison between Unknown and I4
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Expected O, but got Unknown
			bool bMotorolaBitfields = false;
			if (editor == null)
			{
				throw new ArgumentNullException("connector");
			}
			IDeviceObject host = editor.GetHost();
			IDeviceObject5 val = (IDeviceObject5)(object)((host is IDeviceObject5) ? host : null);
			if (val != null)
			{
				ITargetSettings targetSettingsById = APEnvironment.TargetSettingsMgr.Settings.GetTargetSettingsById(val.DeviceIdentificationNoSimulation);
				bMotorolaBitfields = LocalTargetSettings.MotorolaBitfields.GetBoolValue(targetSettingsById);
			}
			bool bFirst = false;
			IParameterSet parameterSet = editor.GetParameterSetProvider().GetParameterSet(bToModify: false);
			if (parameterSet != null)
			{
				foreach (IParameter item3 in (IEnumerable)parameterSet)
				{
					IParameter val2 = item3;
					if ((paramFilter == null || !paramFilter.Invoke((int)val2.Id, (string[])null)) && (int)val2.ChannelType == 1 && (int)val2.GetAccessRight(false) != 0)
					{
						PrintMappingParameter((IDataElement2)(object)((val2 is IDataElement2) ? val2 : null), null, string.Empty, ref bFirst, bInput: true, bMotorolaBitfields);
					}
				}
				bFirst = false;
				foreach (IParameter item4 in (IEnumerable)parameterSet)
				{
					IParameter val3 = item4;
					if ((paramFilter == null || !paramFilter.Invoke((int)val3.Id, (string[])null)) && ((int)val3.ChannelType == 2 || (int)val3.ChannelType == 3) && (int)val3.GetAccessRight(false) != 0)
					{
						PrintMappingParameter((IDataElement2)(object)((val3 is IDataElement2) ? val3 : null), null, string.Empty, ref bFirst, bInput: false, bMotorolaBitfields);
					}
				}
			}
			bFirst = false;
			FbInstanceProvider fbInstanceProvider = new FbInstanceProvider(editor);
			if (fbInstanceProvider == null)
			{
				return;
			}
			foreach (IFbInstance item5 in fbInstanceProvider)
			{
				IFbInstance val4 = item5;
				if (!bFirst)
				{
					bFirst = true;
					DeviceParameterPrintParam item = new DeviceParameterPrintParam
					{
						liData = { Strings.Print_IEC_objects }
					};
					_liData.Add(item);
					item = new DeviceParameterPrintParam
					{
						liData = 
						{
							Strings.Print_IEC_objectVariable,
							Strings.Print_IEC_objectType
						}
					};
					_liData.Add(item);
				}
				DeviceParameterPrintParam item2 = new DeviceParameterPrintParam
				{
					liData = 
					{
						val4.Instance.Variable,
						val4.FbName
					}
				};
				_liData.Add(item2);
			}
		}

		public bool PrintPage(IPrintContext ctx, out int nConsumedHeight)
		{
			if (_liData.Count > 0)
			{
				using (Font font = new Font(FontFamily.GenericSansSerif, 8.25f))
				{
					using Font font3 = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Bold);
					Font font2 = font;
					bool flag = false;
					int top = ctx.ThisPageRect.Top;
					StringFormat stringFormat = new StringFormat();
					stringFormat.Trimming = StringTrimming.Word;
					if (_iLastLine == 0)
					{
						foreach (DeviceParameterPrintParam liDatum in _liData)
						{
							Size size = default(Size);
							int num = 0;
							foreach (string liDatum2 in liDatum.liData)
							{
								size = ctx.Graphics.MeasureString(liDatum2, font2, ctx.ThisPageRect.Width, stringFormat).ToSize();
								if (_liTabs.Count <= num)
								{
									_liTabs.Add(0f);
								}
								if (_liTabs[num] < (float)(size.Width + 10))
								{
									_liTabs[num] = size.Width + 10;
								}
								num++;
							}
						}
						float num2 = 0f;
						for (int i = 0; i < _liTabs.Count; i++)
						{
							num2 += _liTabs[i];
						}
						if (num2 > (float)ctx.ThisPageRect.Width)
						{
							float num3 = ctx.ThisPageRect.Width;
							int num4 = 0;
							for (int j = 0; j < _liTabs.Count; j++)
							{
								if (_liTabs[j] <= (float)(ctx.ThisPageRect.Width / (_liTabs.Count - 1)))
								{
									num3 -= _liTabs[j];
								}
								else
								{
									num4++;
								}
							}
							float value = num3 / (float)num4;
							for (int k = 0; k < _liTabs.Count; k++)
							{
								if (_liTabs[k] > (float)(ctx.ThisPageRect.Width / (_liTabs.Count - 1)))
								{
									_liTabs[k] = value;
								}
							}
						}
					}
					top = ctx.ThisPageRect.Top;
					while (_liData.Count > _iLastLine)
					{
						DeviceParameterPrintParam deviceParameterPrintParam = _liData[_iLastLine];
						int num5 = 0;
						int num6 = 0;
						int num7 = 0;
						int num8 = 0;
						foreach (string liDatum3 in deviceParameterPrintParam.liData)
						{
							if (_liTabs.Count > num5)
							{
								num7 += (int)_liTabs[num5];
							}
							if (num7 == 0 || num5 == deviceParameterPrintParam.liData.Count - 1)
							{
								num7 = ctx.ThisPageRect.Width;
							}
							if (num7 > ctx.ThisPageRect.Width)
							{
								num7 = ctx.ThisPageRect.Width;
							}
							int width = num7 - num6;
							Size size2 = ctx.Graphics.MeasureString(liDatum3, font2, width, stringFormat).ToSize();
							if (top + size2.Height > ctx.ThisPageRect.Bottom)
							{
								nConsumedHeight = top - ctx.ThisPageRect.Top;
								return true;
							}
							if (size2.Height > num8)
							{
								num8 = size2.Height;
							}
							num5++;
							num6 = num7;
						}
						num6 = 0;
						num5 = 0;
						num7 = 0;
						foreach (string liDatum4 in deviceParameterPrintParam.liData)
						{
							if (deviceParameterPrintParam.liData.Count == 1)
							{
								if (!flag)
								{
									font2 = font3;
									flag = true;
								}
							}
							else if (flag)
							{
								font2 = font;
								flag = false;
							}
							if (_liTabs.Count > num5)
							{
								num7 += (int)_liTabs[num5];
							}
							if (num7 == 0 || num5 == deviceParameterPrintParam.liData.Count - 1)
							{
								num7 = ctx.ThisPageRect.Width;
							}
							if (num7 > ctx.ThisPageRect.Width)
							{
								num7 = ctx.ThisPageRect.Width;
							}
							int width2 = num7 - num6;
							Rectangle thisPageRect = ctx.ThisPageRect;
							thisPageRect.Y = top;
							thisPageRect.Height = num8;
							thisPageRect.X += num6;
							thisPageRect.Width = width2;
							ctx.Graphics.DrawString(liDatum4, font2, Brushes.Black, thisPageRect, stringFormat);
							num5++;
							num6 = num7;
						}
						top += num8;
						_iLastLine++;
					}
					if (_liData.Count == _iLastLine && _image != null)
					{
						int num9 = _image.Height;
						int num10 = _image.Width;
						if (num9 + top > ctx.ThisPageRect.Height)
						{
							num10 = num10 * (ctx.ThisPageRect.Bottom - top) / num9;
							num9 = ctx.ThisPageRect.Bottom - top;
						}
						if (num10 > ctx.ThisPageRect.Width)
						{
							num9 = num9 * ctx.ThisPageRect.Width / num10;
							num10 = ctx.ThisPageRect.Width;
						}
						Rectangle thisPageRect2 = ctx.ThisPageRect;
						thisPageRect2.Y = top;
						thisPageRect2.Height = num9;
						thisPageRect2.X = thisPageRect2.X;
						thisPageRect2.Width = num10;
						ctx.Graphics.DrawImage(_image, thisPageRect2);
						top += num9;
					}
					nConsumedHeight = top - ctx.ThisPageRect.Top;
					return false;
				}
			}
			nConsumedHeight = 0;
			return false;
		}
	}
}
