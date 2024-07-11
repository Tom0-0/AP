using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class CommentUtility
	{
		private class PropertyValuePari
		{
			public string Property { get; set; }

			public string Value { get; set; }
		}

		public const char BRACKET_LEFT = '[';

		public const char BRACKET_RIGHT = ']';

		public const char PARENTHESES_LEFT = '(';

		public const char PARENTHESES_RIGHT = ')';

		public const char BRACES_LEFT = '{';

		public const char BRACES_RIGHT = '}';

		public const char SINGLE_QUOTA = '\'';

		public const char DOUBLE_QUOTA = '"';

		public const char SPLITER = '|';

		private static Dictionary<string, string> ESCAPE;

		private static Dictionary<string, string> DE_ESCAPE;

		public static string ESC_BRACKET_LEFT;

		public static string ESC_BRACKET_RIGHT;

		public static string ESC_PARENTHESES_LEFT;

		public static string ESC_PARENTHESES_RIGHT;

		public static string ESC_BRACES_LEFT;

		public static string ESC_BRACES_RIGHT;

		public static string ESC_SINGLE_QUOTA;

		public static string ESC_DOUBLE_QUOTA;

		public static string ESC_SPLITER;

		static CommentUtility()
		{
			ESC_BRACKET_LEFT = "";
			ESC_BRACKET_RIGHT = "";
			ESC_PARENTHESES_LEFT = "";
			ESC_PARENTHESES_RIGHT = "";
			ESC_BRACES_LEFT = "";
			ESC_BRACES_RIGHT = "";
			ESC_SINGLE_QUOTA = "";
			ESC_DOUBLE_QUOTA = "";
			ESC_SPLITER = "";
			ESCAPE = new Dictionary<string, string>();
			DE_ESCAPE = new Dictionary<string, string>();
			ESC_BRACKET_LEFT = GetCharCode('[');
			ESC_BRACKET_RIGHT = GetCharCode(']');
			ESC_PARENTHESES_LEFT = GetCharCode('(');
			ESC_PARENTHESES_RIGHT = GetCharCode(')');
			ESC_BRACES_LEFT = GetCharCode('{');
			ESC_BRACES_RIGHT = GetCharCode('}');
			ESC_SINGLE_QUOTA = GetCharCode('\'');
			ESC_DOUBLE_QUOTA = GetCharCode('"');
			ESC_SPLITER = GetCharCode('|');
			ESCAPE.Add('['.ToString(), ESC_BRACKET_LEFT);
			ESCAPE.Add(']'.ToString(), ESC_BRACKET_RIGHT);
			ESCAPE.Add('('.ToString(), ESC_PARENTHESES_LEFT);
			ESCAPE.Add(')'.ToString(), ESC_PARENTHESES_RIGHT);
			ESCAPE.Add('{'.ToString(), ESC_BRACES_LEFT);
			ESCAPE.Add('}'.ToString(), ESC_BRACES_RIGHT);
			ESCAPE.Add('\''.ToString(), ESC_SINGLE_QUOTA);
			ESCAPE.Add('"'.ToString(), ESC_DOUBLE_QUOTA);
			DE_ESCAPE.Add(ESC_BRACKET_LEFT, '['.ToString());
			DE_ESCAPE.Add(ESC_BRACKET_RIGHT, ']'.ToString());
			DE_ESCAPE.Add(ESC_PARENTHESES_LEFT, '('.ToString());
			DE_ESCAPE.Add(ESC_PARENTHESES_RIGHT, ')'.ToString());
			DE_ESCAPE.Add(ESC_BRACES_LEFT, '{'.ToString());
			DE_ESCAPE.Add(ESC_BRACES_RIGHT, '}'.ToString());
			DE_ESCAPE.Add(ESC_SINGLE_QUOTA, '\''.ToString());
			DE_ESCAPE.Add(ESC_DOUBLE_QUOTA, '"'.ToString());
		}

		private static string GetCharCode(char c)
		{
			int num = c;
			return "$" + num.ToString("X2");
		}

		public static string GetEscapeSpliter(string comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return comment;
			}
			if (comment.Contains('|'))
			{
				comment = comment.Replace('|'.ToString(), ESC_SPLITER);
			}
			return comment;
		}

		public static string GetEscape(string comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return comment;
			}
			if (comment.Contains('['))
			{
				comment = comment.Replace('['.ToString(), ESCAPE['['.ToString()]);
			}
			if (comment.Contains(']'))
			{
				comment = comment.Replace(']'.ToString(), ESCAPE[']'.ToString()]);
			}
			if (comment.Contains('('))
			{
				comment = comment.Replace('('.ToString(), ESCAPE['('.ToString()]);
			}
			if (comment.Contains(')'))
			{
				comment = comment.Replace(')'.ToString(), ESCAPE[')'.ToString()]);
			}
			if (comment.Contains('{'))
			{
				comment = comment.Replace('{'.ToString(), ESCAPE['{'.ToString()]);
			}
			if (comment.Contains('}'))
			{
				comment = comment.Replace('}'.ToString(), ESCAPE['}'.ToString()]);
			}
			if (comment.Contains('\''))
			{
				comment = comment.Replace('\''.ToString(), ESCAPE['\''.ToString()]);
			}
			if (comment.Contains('"'))
			{
				comment = comment.Replace('"'.ToString(), ESCAPE['"'.ToString()]);
			}
			return comment;
		}

		public static string GetOriginal(string comment)
		{
			if (string.IsNullOrEmpty(comment))
			{
				return comment;
			}
			if (comment.Contains(ESC_BRACKET_LEFT))
			{
				comment = comment.Replace(ESC_BRACKET_LEFT, DE_ESCAPE[ESC_BRACKET_LEFT]);
			}
			if (comment.Contains(ESC_BRACKET_RIGHT))
			{
				comment = comment.Replace(ESC_BRACKET_RIGHT, DE_ESCAPE[ESC_BRACKET_RIGHT]);
			}
			if (comment.Contains(ESC_PARENTHESES_LEFT))
			{
				comment = comment.Replace(ESC_PARENTHESES_LEFT, DE_ESCAPE[ESC_PARENTHESES_LEFT]);
			}
			if (comment.Contains(ESC_PARENTHESES_RIGHT))
			{
				comment = comment.Replace(ESC_PARENTHESES_RIGHT, DE_ESCAPE[ESC_PARENTHESES_RIGHT]);
			}
			if (comment.Contains(ESC_BRACES_LEFT))
			{
				comment = comment.Replace(ESC_BRACES_LEFT, DE_ESCAPE[ESC_BRACES_LEFT]);
			}
			if (comment.Contains(ESC_BRACES_RIGHT))
			{
				comment = comment.Replace(ESC_BRACES_RIGHT, DE_ESCAPE[ESC_BRACES_RIGHT]);
			}
			if (comment.Contains(ESC_SINGLE_QUOTA))
			{
				comment = comment.Replace(ESC_SINGLE_QUOTA, DE_ESCAPE[ESC_SINGLE_QUOTA]);
			}
			if (comment.Contains(ESC_DOUBLE_QUOTA))
			{
				comment = comment.Replace(ESC_DOUBLE_QUOTA, DE_ESCAPE[ESC_DOUBLE_QUOTA]);
			}
			return comment;
		}

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

		public static string GetIndexOfMatched(string t, char toFindedLeftChar, out int indexOfRightChar, int toFindedLeftIndex = 0)
		{
			indexOfRightChar = -1;
			string result = string.Empty;
			char c = toFindedLeftChar;
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
			int num2 = -1;
			for (; i < t.Length; i++)
			{
				char c2 = t[i];
				if (!object.Equals(c2, toFindedLeftChar) && !object.Equals(c2, c))
				{
					continue;
				}
				if (object.Equals(c2, toFindedLeftChar))
				{
					stack.Push(c2);
					if (stack.Count == 1)
					{
						num = i;
					}
					continue;
				}
				if (stack.Count == 1)
				{
					stack.Push(c2);
					num2 = i;
					result = t.Substring(num + 1, num2 - num - 1);
					indexOfRightChar = i;
					break;
				}
				if (stack.Count > 0)
				{
					stack.Pop();
					continue;
				}
				return result;
			}
			return result;
		}

		public static List<CommentFormat> GetChildComment(string childComment)
		{
			Stack<char> stack = new Stack<char>();
			List<CommentFormat> list = new List<CommentFormat>();
			int i = 0;
			CommentFormat commentFormat = null;
			int num = -1;
			int num2 = -1;
			for (; i < childComment.Length; i++)
			{
				char c = childComment[i];
				if (!object.Equals(c, '(') && !object.Equals(c, ')'))
				{
					continue;
				}
				if (object.Equals(c, '('))
				{
					stack.Push(c);
					if (stack.Count == 1)
					{
						commentFormat = new CommentFormat();
						int num3 = childComment.Substring(0, i).LastIndexOf(',');
						bool flag = num3 == -1;
						int num4 = ((!flag) ? (num3 + 1) : 0);
						if ((flag ? i : (i - num3 - 1)) == 0)
						{
							commentFormat.N = 1;
						}
						else
						{
							commentFormat.N = Convert.ToInt32(childComment.Substring(num3 + 1, i - num3 - 1));
						}
						num = i;
					}
				}
				else if (stack.Count == 1)
				{
					stack.Push(c);
					num2 = i;
					commentFormat.Comment = childComment.Substring(num + 1, num2 - num - 1);
					list.Add(commentFormat);
					num = -1;
					num2 = -1;
					stack.Clear();
					commentFormat = null;
				}
				else
				{
					stack.Pop();
				}
			}
			return list;
		}

		internal static Dictionary<string, string> GetChildDictionary(string childComment)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Stack<char> stack = new Stack<char>();
			int i = 0;
			PropertyValuePari propertyValuePari = null;
			int num = -1;
			int num2 = -1;
			for (; i < childComment.Length; i++)
			{
				char c = childComment[i];
				if (!object.Equals(c, '(') && !object.Equals(c, ')'))
				{
					continue;
				}
				if (object.Equals(c, '('))
				{
					stack.Push(c);
					if (stack.Count == 1)
					{
						propertyValuePari = new PropertyValuePari();
						int num3 = childComment.Substring(0, i).LastIndexOf(',');
						bool flag = num3 == -1;
						int num4 = ((!flag) ? (num3 + 1) : 0);
						if ((flag ? i : (i - num3 - 1)) == 0)
						{
							propertyValuePari.Property = string.Empty;
						}
						else
						{
							propertyValuePari.Property = childComment.Substring(num3 + 1, i - num3 - 1);
						}
						num = i;
					}
				}
				else if (stack.Count == 1)
				{
					stack.Push(c);
					num2 = i;
					propertyValuePari.Value = childComment.Substring(num + 1, num2 - num - 1);
					dictionary.Add(propertyValuePari.Property, propertyValuePari.Value);
					num = -1;
					num2 = -1;
					stack.Clear();
					propertyValuePari = null;
				}
				else
				{
					stack.Pop();
				}
			}
			return dictionary;
		}
	}
}
