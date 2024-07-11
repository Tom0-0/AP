using System.ComponentModel;
using System.Drawing;
using _3S.CoDeSys.Core.Options;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal static class IECTextEditorOptions
	{
		private static readonly string MARGIN_FONT;

		private static readonly string MARGIN_FONT_SIZE;

		private static readonly string MARGIN_FONT_STYLE;

		private static readonly string MARGIN_LINE_NUMBERS;

		private static readonly string MARGIN_BACKGROUND_COLOR;

		private static readonly string MARGIN_HIGHLIGHT_CURRENT_LINE;

		private static readonly string MARGIN_HIGHLIGHT_COLOR;

		private static readonly string MARGIN_FOREGROUND_COLOR;

		private static readonly string MARGIN_FOCUS_BORDER_COLOR;

		private static readonly string MARGIN_NO_FOCUS_BORDER_COLOR;

		private static readonly string SUB_KEY;

		private static readonly TypeConverter FONT_STYLE_CONVERTER;

		private static readonly TypeConverter COLOR_CONVERTER;

		internal static string MarginFont
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_FONT, typeof(string)))
				{
					return (string)OptionKey[MARGIN_FONT];
				}
				return FontFamily.GenericMonospace.Name;
			}
			set
			{
				OptionKey[MARGIN_FONT]= (object)value;
			}
		}

		internal static float MarginFontSize
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_FONT_SIZE, typeof(float)))
				{
					return (float)OptionKey[MARGIN_FONT_SIZE];
				}
				return 8.25f;
			}
			set
			{
				OptionKey[MARGIN_FONT_SIZE]= (object)value;
			}
		}

		internal static FontStyle MarginFontStyle
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_FONT_STYLE, typeof(string)))
				{
					return (FontStyle)FONT_STYLE_CONVERTER.ConvertFromInvariantString((string)OptionKey[MARGIN_FONT_STYLE]);
				}
				return FontStyle.Regular;
			}
			set
			{
				OptionKey[MARGIN_FONT_STYLE]= (object)FONT_STYLE_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static bool MarginLineNumbers
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_LINE_NUMBERS, typeof(bool)))
				{
					return (bool)OptionKey[MARGIN_LINE_NUMBERS];
				}
				return true;
			}
			set
			{
				OptionKey[MARGIN_LINE_NUMBERS]= (object)value;
			}
		}

		internal static Color MarginBackgroundColor
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_BACKGROUND_COLOR, typeof(string)))
				{
					return (Color)COLOR_CONVERTER.ConvertFromInvariantString((string)OptionKey[MARGIN_BACKGROUND_COLOR]);
				}
				return Color.White;
			}
			set
			{
				OptionKey[MARGIN_BACKGROUND_COLOR]= (object)COLOR_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static bool MarginHighlightCurrentLine
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_HIGHLIGHT_CURRENT_LINE, typeof(bool)))
				{
					return (bool)OptionKey[MARGIN_HIGHLIGHT_CURRENT_LINE];
				}
				return true;
			}
			set
			{
				OptionKey[MARGIN_HIGHLIGHT_CURRENT_LINE]= (object)value;
			}
		}

		internal static Color MarginHighlightColor
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_HIGHLIGHT_COLOR, typeof(string)))
				{
					return (Color)COLOR_CONVERTER.ConvertFromInvariantString((string)OptionKey[MARGIN_HIGHLIGHT_COLOR]);
				}
				return Color.Firebrick;
			}
			set
			{
				OptionKey[MARGIN_HIGHLIGHT_COLOR]= (object)COLOR_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static Color MarginForegroundColor
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_FOREGROUND_COLOR, typeof(string)))
				{
					return (Color)COLOR_CONVERTER.ConvertFromInvariantString((string)OptionKey[MARGIN_FOREGROUND_COLOR]);
				}
				return Color.SteelBlue;
			}
			set
			{
				OptionKey[MARGIN_FOREGROUND_COLOR]= (object)COLOR_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static Color MarginFocusBorderColor
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_FOCUS_BORDER_COLOR, typeof(string)))
				{
					return (Color)COLOR_CONVERTER.ConvertFromInvariantString((string)OptionKey[MARGIN_FOCUS_BORDER_COLOR]);
				}
				return Color.Teal;
			}
			set
			{
				OptionKey[MARGIN_FOCUS_BORDER_COLOR]= (object)COLOR_CONVERTER.ConvertToInvariantString(value);
			}
		}

		internal static Color MarginNoFocusBorderColor
		{
			get
			{
				if (OptionKey.HasValue(MARGIN_NO_FOCUS_BORDER_COLOR, typeof(string)))
				{
					return (Color)COLOR_CONVERTER.ConvertFromInvariantString((string)OptionKey[MARGIN_NO_FOCUS_BORDER_COLOR]);
				}
				return Color.LightGray;
			}
			set
			{
				OptionKey[MARGIN_NO_FOCUS_BORDER_COLOR]= (object)COLOR_CONVERTER.ConvertToInvariantString(value);
			}
		}

		private static IOptionKey OptionKey => APEnvironment.OptionStorage.GetRootKey((OptionRoot)4).CreateSubKey(SUB_KEY);

		static IECTextEditorOptions()
		{
			MARGIN_FONT = "MarginFont";
			MARGIN_FONT_SIZE = "MarginFontSize";
			MARGIN_FONT_STYLE = "MarginFontStyle";
			MARGIN_LINE_NUMBERS = "MarginLineNumbers";
			MARGIN_BACKGROUND_COLOR = "MarginBackgroundColor";
			MARGIN_HIGHLIGHT_CURRENT_LINE = "MarginHighlightCurrentLine";
			MARGIN_HIGHLIGHT_COLOR = "MarginHighlightColor";
			MARGIN_FOREGROUND_COLOR = "MarginForegroundColor";
			MARGIN_FOCUS_BORDER_COLOR = "MarginFocusBorderColor";
			MARGIN_NO_FOCUS_BORDER_COLOR = "MarginNoFocusBorderColor";
			SUB_KEY = "{E364D5B6-081C-495e-B183-7B798ACF5144}";
			FONT_STYLE_CONVERTER = TypeDescriptor.GetConverter(typeof(FontStyle));
			COLOR_CONVERTER = TypeDescriptor.GetConverter(typeof(Color));
		}
	}
}
