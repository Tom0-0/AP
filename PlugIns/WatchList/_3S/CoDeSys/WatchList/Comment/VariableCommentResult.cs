using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    public class VariableCommentResult
    {
		public void AddChild(string property, string comment)
		{
			if (this.childComments != null)
			{
				VariableCommentResult variableCommentResult = new VariableCommentResult();
				variableCommentResult.Property = property;
				variableCommentResult.Comment = comment;
				this.childComments.Add(variableCommentResult);
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002458 File Offset: 0x00000658
		public VariableCommentResult GetChild(string property)
		{
			if (this.childComments == null)
			{
				return null;
			}
			return this.childComments.FirstOrDefault((VariableCommentResult t) => string.Equals(property, t.Property, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002494 File Offset: 0x00000694
		public void MergeTypeVariableCommentResult(VariableCommentResult typeComment)
		{
			if (typeComment != null)
			{
				if (string.IsNullOrEmpty(this.Comment))
				{
					this.Comment = typeComment.Comment;
				}
				if (this.childComments != null)
				{
					foreach (VariableCommentResult variableCommentResult in this.childComments)
					{
						if (string.IsNullOrEmpty(variableCommentResult.Comment))
						{
							VariableCommentResult child = typeComment.GetChild(variableCommentResult.Property);
							variableCommentResult.Comment = ((child == null) ? string.Empty : child.Comment);
						}
					}
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002538 File Offset: 0x00000738
		public List<VariableCommentResult> ChildComments
		{
			get
			{
				return this.childComments;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002540 File Offset: 0x00000740
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002548 File Offset: 0x00000748
		public string Property { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002551 File Offset: 0x00000751
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00002559 File Offset: 0x00000759
		public string Comment { get; set; }

		// Token: 0x0400003A RID: 58
		private List<VariableCommentResult> childComments = new List<VariableCommentResult>();
	}
}
