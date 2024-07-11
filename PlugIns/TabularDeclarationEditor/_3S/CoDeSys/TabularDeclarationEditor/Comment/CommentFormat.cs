using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class CommentFormat
	{
		public int N { get; set; }

		public string Comment { get; set; }

		public static string GetCommentFormats(List<CommentFormat> formats)
		{
			if (formats.Count == 1)
			{
				return formats[0].ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < formats.Count; i++)
			{
				CommentFormat commentFormat = formats[i];
				if (i == 0)
				{
					stringBuilder.Append(commentFormat.ToString());
				}
				else
				{
					stringBuilder.Append($",{commentFormat.ToString()}");
				}
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			if (N == 1)
			{
				return $"({Comment})";
			}
			return $"{N}({Comment})";
		}
	}
}
