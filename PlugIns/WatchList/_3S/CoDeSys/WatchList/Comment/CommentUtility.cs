using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    class CommentUtility
    {
		static CommentUtility()
		{
			CommentUtility.ESCAPE = new Dictionary<string, string>();
			CommentUtility.DE_ESCAPE = new Dictionary<string, string>();
			CommentUtility.ESC_BRACKET_LEFT = CommentUtility.GetCharCode('[');
			CommentUtility.ESC_BRACKET_RIGHT = CommentUtility.GetCharCode(']');
			CommentUtility.ESC_PARENTHESES_LEFT = CommentUtility.GetCharCode('(');
			CommentUtility.ESC_PARENTHESES_RIGHT = CommentUtility.GetCharCode(')');
			CommentUtility.ESC_BRACES_LEFT = CommentUtility.GetCharCode('{');
			CommentUtility.ESC_BRACES_RIGHT = CommentUtility.GetCharCode('}');
			CommentUtility.ESC_SINGLE_QUOTA = CommentUtility.GetCharCode('\'');
			CommentUtility.ESC_DOUBLE_QUOTA = CommentUtility.GetCharCode('"');
			CommentUtility.ESC_SPLITER = CommentUtility.GetCharCode('|');
			CommentUtility.ESCAPE.Add('['.ToString(), CommentUtility.ESC_BRACKET_LEFT);
			CommentUtility.ESCAPE.Add(']'.ToString(), CommentUtility.ESC_BRACKET_RIGHT);
			CommentUtility.ESCAPE.Add('('.ToString(), CommentUtility.ESC_PARENTHESES_LEFT);
			CommentUtility.ESCAPE.Add(')'.ToString(), CommentUtility.ESC_PARENTHESES_RIGHT);
			CommentUtility.ESCAPE.Add('{'.ToString(), CommentUtility.ESC_BRACES_LEFT);
			CommentUtility.ESCAPE.Add('}'.ToString(), CommentUtility.ESC_BRACES_RIGHT);
			CommentUtility.ESCAPE.Add('\''.ToString(), CommentUtility.ESC_SINGLE_QUOTA);
			CommentUtility.ESCAPE.Add('"'.ToString(), CommentUtility.ESC_DOUBLE_QUOTA);
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_BRACKET_LEFT, '['.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_BRACKET_RIGHT, ']'.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_PARENTHESES_LEFT, '('.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_PARENTHESES_RIGHT, ')'.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_BRACES_LEFT, '{'.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_BRACES_RIGHT, '}'.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_SINGLE_QUOTA, '\''.ToString());
			CommentUtility.DE_ESCAPE.Add(CommentUtility.ESC_DOUBLE_QUOTA, '"'.ToString());
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003D78 File Offset: 0x00001F78
		private static string GetCharCode(char c)
		{
			int num = (int)c;
			return "$" + num.ToString("X2");
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003DA0 File Offset: 0x00001FA0
		public static string GetEscapeSpliter(string comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return comment;
			}
			if (comment.Contains('|'))
			{
				comment = comment.Replace('|'.ToString(), CommentUtility.ESC_SPLITER);
			}
			return comment;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003DDC File Offset: 0x00001FDC
		public static string GetEscape(string comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return comment;
			}
			if (comment.Contains('['))
			{
				comment = comment.Replace('['.ToString(), CommentUtility.ESCAPE['['.ToString()]);
			}
			if (comment.Contains(']'))
			{
				comment = comment.Replace(']'.ToString(), CommentUtility.ESCAPE[']'.ToString()]);
			}
			if (comment.Contains('('))
			{
				comment = comment.Replace('('.ToString(), CommentUtility.ESCAPE['('.ToString()]);
			}
			if (comment.Contains(')'))
			{
				comment = comment.Replace(')'.ToString(), CommentUtility.ESCAPE[')'.ToString()]);
			}
			if (comment.Contains('{'))
			{
				comment = comment.Replace('{'.ToString(), CommentUtility.ESCAPE['{'.ToString()]);
			}
			if (comment.Contains('}'))
			{
				comment = comment.Replace('}'.ToString(), CommentUtility.ESCAPE['}'.ToString()]);
			}
			if (comment.Contains('\''))
			{
				comment = comment.Replace('\''.ToString(), CommentUtility.ESCAPE['\''.ToString()]);
			}
			if (comment.Contains('"'))
			{
				comment = comment.Replace('"'.ToString(), CommentUtility.ESCAPE['"'.ToString()]);
			}
			return comment;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003F74 File Offset: 0x00002174
		public static string GetOriginal(string comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return comment;
			}
			if (comment.Contains(CommentUtility.ESC_BRACKET_LEFT))
			{
				comment = comment.Replace(CommentUtility.ESC_BRACKET_LEFT, CommentUtility.DE_ESCAPE[CommentUtility.ESC_BRACKET_LEFT]);
			}
			if (comment.Contains(CommentUtility.ESC_BRACKET_RIGHT))
			{
				comment = comment.Replace(CommentUtility.ESC_BRACKET_RIGHT, CommentUtility.DE_ESCAPE[CommentUtility.ESC_BRACKET_RIGHT]);
			}
			if (comment.Contains(CommentUtility.ESC_PARENTHESES_LEFT))
			{
				comment = comment.Replace(CommentUtility.ESC_PARENTHESES_LEFT, CommentUtility.DE_ESCAPE[CommentUtility.ESC_PARENTHESES_LEFT]);
			}
			if (comment.Contains(CommentUtility.ESC_PARENTHESES_RIGHT))
			{
				comment = comment.Replace(CommentUtility.ESC_PARENTHESES_RIGHT, CommentUtility.DE_ESCAPE[CommentUtility.ESC_PARENTHESES_RIGHT]);
			}
			if (comment.Contains(CommentUtility.ESC_BRACES_LEFT))
			{
				comment = comment.Replace(CommentUtility.ESC_BRACES_LEFT, CommentUtility.DE_ESCAPE[CommentUtility.ESC_BRACES_LEFT]);
			}
			if (comment.Contains(CommentUtility.ESC_BRACES_RIGHT))
			{
				comment = comment.Replace(CommentUtility.ESC_BRACES_RIGHT, CommentUtility.DE_ESCAPE[CommentUtility.ESC_BRACES_RIGHT]);
			}
			if (comment.Contains(CommentUtility.ESC_SINGLE_QUOTA))
			{
				comment = comment.Replace(CommentUtility.ESC_SINGLE_QUOTA, CommentUtility.DE_ESCAPE[CommentUtility.ESC_SINGLE_QUOTA]);
			}
			if (comment.Contains(CommentUtility.ESC_DOUBLE_QUOTA))
			{
				comment = comment.Replace(CommentUtility.ESC_DOUBLE_QUOTA, CommentUtility.DE_ESCAPE[CommentUtility.ESC_DOUBLE_QUOTA]);
			}
			return comment;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000040D4 File Offset: 0x000022D4
		public static void GetComment(string t, out string comment, out string childComment)
		{
			comment = string.Empty;
			childComment = string.Empty;
			if (string.IsNullOrEmpty(t))
			{
				return;
			}
			comment = t;
			if (t.EndsWith(']'.ToString()))
			{
				int num = t.IndexOf('[');
				if (num != -1)
				{
					comment = t.Substring(0, num);
					childComment = t.Substring(num + 1, t.Length - num - 2);
				}
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000413C File Offset: 0x0000233C
		public static string GetIndexOfMatched(string t, char toFindedLeftChar, out int indexOfRightChar, int toFindedLeftIndex = 0)
		{
			indexOfRightChar = -1;
			string result = string.Empty;
			char c;
			if (object.Equals(toFindedLeftChar, '['))
			{
				c = ']';
			}
			else
			{
				if (!object.Equals(toFindedLeftChar, '('))
				{
					throw new InvalidOperationException("invalid input parameter:");
				}
				c = ')';
			}
			int i = toFindedLeftIndex;
			Stack<char> stack = new Stack<char>();
			int num = -1;
			while (i < t.Length)
			{
				char c2 = t[i];
				if (object.Equals(c2, toFindedLeftChar) || object.Equals(c2, c))
				{
					if (object.Equals(c2, toFindedLeftChar))
					{
						stack.Push(c2);
						if (stack.Count == 1)
						{
							num = i;
						}
					}
					else
					{
						if (stack.Count == 1)
						{
							stack.Push(c2);
							int num2 = i;
							result = t.Substring(num + 1, num2 - num - 1);
							indexOfRightChar = i;
							break;
						}
						if (stack.Count <= 0)
						{
							return result;
						}
						stack.Pop();
					}
				}
				i++;
			}
			return result;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004250 File Offset: 0x00002450
		public static List<CommentFormat> GetChildComment(string childComment)
		{
			Stack<char> stack = new Stack<char>();
			List<CommentFormat> list = new List<CommentFormat>();
			int i = 0;
			CommentFormat commentFormat = null;
			int num = -1;
			while (i < childComment.Length)
			{
				char c = childComment[i];
				if (object.Equals(c, '(') || object.Equals(c, ')'))
				{
					if (object.Equals(c, '('))
					{
						stack.Push(c);
						if (stack.Count == 1)
						{
							commentFormat = new CommentFormat();
							int num2 = childComment.Substring(0, i).LastIndexOf(',');
							if (((num2 == -1) ? i : (i - num2 - 1)) == 0)
							{
								commentFormat.N = 1;
							}
							else
							{
								commentFormat.N = Convert.ToInt32(childComment.Substring(num2 + 1, i - num2 - 1));
							}
							num = i;
						}
					}
					else if (stack.Count == 1)
					{
						stack.Push(c);
						int num3 = i;
						commentFormat.Comment = childComment.Substring(num + 1, num3 - num - 1);
						list.Add(commentFormat);
						num = -1;
						stack.Clear();
						commentFormat = null;
					}
					else
					{
						stack.Pop();
					}
				}
				i++;
			}
			return list;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004388 File Offset: 0x00002588
		internal static Dictionary<string, string> GetChildDictionary(string childComment)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Stack<char> stack = new Stack<char>();
			int i = 0;
			CommentUtility.PropertyValuePari propertyValuePari = null;
			int num = -1;
			while (i < childComment.Length)
			{
				char c = childComment[i];
				if (object.Equals(c, '(') || object.Equals(c, ')'))
				{
					if (object.Equals(c, '('))
					{
						stack.Push(c);
						if (stack.Count == 1)
						{
							propertyValuePari = new CommentUtility.PropertyValuePari();
							int num2 = childComment.Substring(0, i).LastIndexOf(',');
							if (((num2 == -1) ? i : (i - num2 - 1)) == 0)
							{
								propertyValuePari.Property = string.Empty;
							}
							else
							{
								propertyValuePari.Property = childComment.Substring(num2 + 1, i - num2 - 1);
							}
							num = i;
						}
					}
					else if (stack.Count == 1)
					{
						stack.Push(c);
						int num3 = i;
						propertyValuePari.Value = childComment.Substring(num + 1, num3 - num - 1);
						dictionary.Add(propertyValuePari.Property, propertyValuePari.Value);
						num = -1;
						stack.Clear();
						propertyValuePari = null;
					}
					else
					{
						stack.Pop();
					}
				}
				i++;
			}
			return dictionary;
		}

		// Token: 0x04000014 RID: 20
		public const char BRACKET_LEFT = '[';

		// Token: 0x04000015 RID: 21
		public const char BRACKET_RIGHT = ']';

		// Token: 0x04000016 RID: 22
		public const char PARENTHESES_LEFT = '(';

		// Token: 0x04000017 RID: 23
		public const char PARENTHESES_RIGHT = ')';

		// Token: 0x04000018 RID: 24
		public const char BRACES_LEFT = '{';

		// Token: 0x04000019 RID: 25
		public const char BRACES_RIGHT = '}';

		// Token: 0x0400001A RID: 26
		public const char SINGLE_QUOTA = '\'';

		// Token: 0x0400001B RID: 27
		public const char DOUBLE_QUOTA = '"';

		// Token: 0x0400001C RID: 28
		public const char SPLITER = '|';

		// Token: 0x0400001D RID: 29
		private static Dictionary<string, string> ESCAPE;

		// Token: 0x0400001E RID: 30
		private static Dictionary<string, string> DE_ESCAPE;

		// Token: 0x0400001F RID: 31
		public static string ESC_BRACKET_LEFT = "";

		// Token: 0x04000020 RID: 32
		public static string ESC_BRACKET_RIGHT = "";

		// Token: 0x04000021 RID: 33
		public static string ESC_PARENTHESES_LEFT = "";

		// Token: 0x04000022 RID: 34
		public static string ESC_PARENTHESES_RIGHT = "";

		// Token: 0x04000023 RID: 35
		public static string ESC_BRACES_LEFT = "";

		// Token: 0x04000024 RID: 36
		public static string ESC_BRACES_RIGHT = "";

		// Token: 0x04000025 RID: 37
		public static string ESC_SINGLE_QUOTA = "";

		// Token: 0x04000026 RID: 38
		public static string ESC_DOUBLE_QUOTA = "";

		// Token: 0x04000027 RID: 39
		public static string ESC_SPLITER = "";

		// Token: 0x020000DF RID: 223
		private class PropertyValuePari
		{
			// Token: 0x17000313 RID: 787
			// (get) Token: 0x060008E8 RID: 2280 RVA: 0x0002E3E1 File Offset: 0x0002C5E1
			// (set) Token: 0x060008E9 RID: 2281 RVA: 0x0002E3E9 File Offset: 0x0002C5E9
			public string Property { get; set; }

			// Token: 0x17000314 RID: 788
			// (get) Token: 0x060008EA RID: 2282 RVA: 0x0002E3F2 File Offset: 0x0002C5F2
			// (set) Token: 0x060008EB RID: 2283 RVA: 0x0002E3FA File Offset: 0x0002C5FA
			public string Value { get; set; }
		}
	}
}
