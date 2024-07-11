using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    class CommentFormat
    {
		public int N { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003A36 File Offset: 0x00001C36
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00003A3E File Offset: 0x00001C3E
		public string Comment { get; set; }

		// Token: 0x06000068 RID: 104 RVA: 0x00003A48 File Offset: 0x00001C48
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
					stringBuilder.Append("," + commentFormat.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003AB9 File Offset: 0x00001CB9
		public override string ToString()
		{
			if (this.N == 1)
			{
				return "(" + this.Comment + ")";
			}
			return string.Format("{0}({1})", this.N, this.Comment);
		}
	}
}
