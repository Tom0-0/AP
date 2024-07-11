using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    class CommentElement
    {
		public string Comment { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000038F1 File Offset: 0x00001AF1
		// (set) Token: 0x06000056 RID: 86 RVA: 0x000038F9 File Offset: 0x00001AF9
		public string ChildComment { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00003902 File Offset: 0x00001B02
		// (set) Token: 0x06000058 RID: 88 RVA: 0x0000390A File Offset: 0x00001B0A
		public string FullComment { get; protected set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00003913 File Offset: 0x00001B13
		// (set) Token: 0x0600005A RID: 90 RVA: 0x0000391B File Offset: 0x00001B1B
		public CommentElement Parent { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00003924 File Offset: 0x00001B24
		public virtual bool HasBuild
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003927 File Offset: 0x00001B27
		public CommentElement(string comment, CommentElement parent)
		{
			this.Comment = comment;
			this.Parent = parent;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003940 File Offset: 0x00001B40
		public static CommentElement BuildRootNode(IType type, string comment, string commentAttribute)
		{
			bool flag = type is IArrayType;
			bool flag2 = type is IUserdefType;
			CommentElement commentElement = null;
			if (flag)
			{
				commentElement = new ArrayCommentElement(comment, null);
			}
			else if (flag2)
			{
				commentElement = new StructureCommentElement(comment, null);
			}
			commentElement.ChildComment = commentAttribute;
			return commentElement;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003981 File Offset: 0x00001B81
		public bool HasAnyChildComment()
		{
			return !string.IsNullOrEmpty(this.ChildComment);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003991 File Offset: 0x00001B91
		public virtual string BuildChildComment()
		{
			return string.Empty;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003998 File Offset: 0x00001B98
		protected virtual string FormatChildComment()
		{
			return string.Empty;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000399F File Offset: 0x00001B9F
		public virtual string GetFullComment()
		{
			this.FullComment = CommentUtility.GetEscape(this.Comment);
			return this.FullComment;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000039B8 File Offset: 0x00001BB8
		public virtual void BuildChildElement(IType type)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000039BC File Offset: 0x00001BBC
		public void ResumeFullComment()
		{
			string childComment = this.ChildComment;
			this.FullComment = ((this.Parent == null) ? (childComment ?? "") : ((!string.IsNullOrEmpty(childComment)) ? (CommentUtility.GetEscape(this.Comment) + "[" + childComment + "]") : (CommentUtility.GetEscape(this.Comment) ?? "")));
		}
	}
}
