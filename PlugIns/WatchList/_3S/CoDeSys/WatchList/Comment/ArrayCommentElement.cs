using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    class ArrayCommentElement : CommentElement
    {
		public override bool HasBuild
		{
			get
			{
				return this.hasBuild;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002F07 File Offset: 0x00001107
		public ArrayCommentElement(string comment, CommentElement parent) : base(comment, parent)
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002F14 File Offset: 0x00001114
		public override void BuildChildElement(IType type)
		{
			if (!this.hasBuild)
			{
				IArrayType arrayType = type as IArrayType;
				if (arrayType != null)
				{
					this.child = new List<CommentElement>();
					List<CommentFormat> childComment = CommentUtility.GetChildComment(base.ChildComment);
					for (int i = 0; i < childComment.Count; i++)
					{
						CommentFormat commentFormat = childComment[i];
						int n = commentFormat.N;
						string comment;
						string childComment2;
						CommentUtility.GetComment(commentFormat.Comment, out comment, out childComment2);
						for (int j = 0; j < n; j++)
						{
							CommentElement item = this.CreateCommentElement(arrayType, CommentUtility.GetOriginal(comment), childComment2);
							this.child.Add(item);
						}
					}
					int num;
					int num2;
					Common.GetArrayDimensionSize(arrayType, out num, out num2);
					int num3 = num2 - num + 1;
					if (this.child.Count < num3)
					{
						for (int k = this.child.Count; k < num3; k++)
						{
							CommentElement item2 = this.CreateCommentElement(arrayType, string.Empty, string.Empty);
							this.child.Add(item2);
						}
					}
					if (this.child.Count > num3)
					{
						this.child.RemoveRange(num3, this.child.Count - num3);
					}
					this.hasBuild = true;
					return;
				}
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000303C File Offset: 0x0000123C
		private CommentElement CreateCommentElement(IArrayType arrayType, string comment, string childComment)
		{
			bool flag = arrayType.Base.Class == TypeClass.Array;
			bool flag2 = arrayType.Base.Class == TypeClass.Userdef;
			if (flag)
			{
				return new ArrayCommentElement(comment, this)
				{
					ChildComment = childComment
				};
			}
			if (flag2)
			{
				return new StructureCommentElement(comment, this)
				{
					ChildComment = childComment
				};
			}
			return new CommentElement(comment, this)
			{
				ChildComment = childComment
			};
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003099 File Offset: 0x00001299
		public CommentElement GetChildBy(int index)
		{
			return this.child[index];
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000030A7 File Offset: 0x000012A7
		public int GetCount()
		{
			return this.child.Count;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000030B4 File Offset: 0x000012B4
		public override string GetFullComment()
		{
			string text = this.FormatChildComment();
			if (base.Parent == null)
			{
				base.FullComment = (text ?? "");
			}
			else if (string.IsNullOrEmpty(text))
			{
				base.FullComment = (CommentUtility.GetEscape(base.Comment) ?? "");
			}
			else
			{
				base.FullComment = CommentUtility.GetEscape(base.Comment) + "[" + text + "]";
			}
			return base.FullComment;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003130 File Offset: 0x00001330
		public override string BuildChildComment()
		{
			if (this.child != null && this.child.Count > 0)
			{
				foreach (CommentElement commentElement in this.child)
				{
					if (commentElement.HasBuild)
					{
						commentElement.BuildChildComment();
						commentElement.GetFullComment();
					}
					else
					{
						commentElement.ResumeFullComment();
					}
				}
			}
			return this.GetFullComment();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000031B8 File Offset: 0x000013B8
		protected override string FormatChildComment()
		{
			List<CommentFormat> list = new List<CommentFormat>();
			for (int i = 0; i < this.child.Count; i++)
			{
				CommentElement commentElement = this.child[i];
				if (i == 0)
				{
					CommentFormat item = new CommentFormat
					{
						N = 1,
						Comment = commentElement.FullComment
					};
					list.Add(item);
				}
				else
				{
					CommentFormat commentFormat = list[list.Count - 1];
					if (string.Equals(commentElement.FullComment, commentFormat.Comment))
					{
						CommentFormat commentFormat2 = commentFormat;
						int n = commentFormat2.N;
						commentFormat2.N = n + 1;
					}
					else
					{
						CommentFormat item2 = new CommentFormat
						{
							N = 1,
							Comment = commentElement.FullComment
						};
						list.Add(item2);
					}
				}
			}
			return CommentFormat.GetCommentFormats(list);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000327C File Offset: 0x0000147C
		public string RebuldAdjustedComment()
		{
			if (this.child != null && this.child.Count > 0)
			{
				foreach (CommentElement commentElement in this.child)
				{
					commentElement.ResumeFullComment();
				}
			}
			return this.GetFullComment();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000032E8 File Offset: 0x000014E8
		public void Adjust1DNewRange(int headAdd, int reserverStart, int reserverEnd, int total, IArrayType arrayType)
		{
			this.child.RemoveRange(0, reserverStart);
			for (int i = 0; i < headAdd; i++)
			{
				CommentElement item = this.CreateCommentElement(arrayType, string.Empty, string.Empty);
				this.child.Insert(0, item);
			}
			if (this.child.Count < total)
			{
				for (int j = this.child.Count; j < total; j++)
				{
					CommentElement item2 = this.CreateCommentElement(arrayType, string.Empty, string.Empty);
					this.child.Add(item2);
				}
			}
			if (this.child.Count > total)
			{
				this.child.RemoveRange(total, this.child.Count - total);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000033A0 File Offset: 0x000015A0
		public void Adjust2DNewRange(Dictionary<Tuple<int, int>, int> oldMap, Dictionary<int, Tuple<int, int>> newMap, int total, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < total; i++)
			{
				Tuple<int, int> key = newMap[i];
				if (oldMap.ContainsKey(key))
				{
					int index = oldMap[key];
					list.Add(this.child[index]);
				}
				else
				{
					CommentElement item = this.CreateCommentElement(arrayType, string.Empty, string.Empty);
					list.Add(item);
				}
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003420 File Offset: 0x00001620
		public void Adjust3DNewRange(Dictionary<Tuple<int, int, int>, int> oldMap, Dictionary<int, Tuple<int, int, int>> newMap, int total, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < total; i++)
			{
				Tuple<int, int, int> key = newMap[i];
				if (oldMap.ContainsKey(key))
				{
					int index = oldMap[key];
					list.Add(this.child[index]);
				}
				else
				{
					CommentElement item = this.CreateCommentElement(arrayType, string.Empty, string.Empty);
					list.Add(item);
				}
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000034A0 File Offset: 0x000016A0
		public void Adjust1DTo2DNewRange(List<Tuple<int, int>> d2Indexes, int d2Min, Dictionary<int, int> source1dMap, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < d2Indexes.Count; i++)
			{
				Tuple<int, int> tuple = d2Indexes[i];
				string comment = string.Empty;
				string childComment = string.Empty;
				if (tuple.Item2 == d2Min && source1dMap.ContainsKey(tuple.Item1))
				{
					int index = source1dMap[tuple.Item1];
					comment = this.child[index].Comment;
					childComment = this.child[index].ChildComment;
				}
				CommentElement item = this.CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003554 File Offset: 0x00001754
		public void Adjust2DTo1DNewRange(List<int> d1Indexes, int d2Min, Dictionary<Tuple<int, int>, int> source2dMap, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < d1Indexes.Count; i++)
			{
				int item = d1Indexes[i];
				string comment = string.Empty;
				string childComment = string.Empty;
				Tuple<int, int> key = new Tuple<int, int>(item, d2Min);
				if (source2dMap.ContainsKey(key))
				{
					int index = source2dMap[key];
					comment = this.child[index].Comment;
					childComment = this.child[index].ChildComment;
				}
				CommentElement item2 = this.CreateCommentElement(arrayType, comment, childComment);
				list.Add(item2);
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000035F8 File Offset: 0x000017F8
		public void Adjust1DTo3DNewRange(List<Tuple<int, int, int>> d3Indexes, int d2Min, int d3Min, Dictionary<int, int> source1dMap, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < d3Indexes.Count; i++)
			{
				Tuple<int, int, int> tuple = d3Indexes[i];
				string comment = string.Empty;
				string childComment = string.Empty;
				if (tuple.Item2 == d2Min && tuple.Item3 == d3Min && source1dMap.ContainsKey(tuple.Item1))
				{
					int index = source1dMap[tuple.Item1];
					comment = this.child[index].Comment;
					childComment = this.child[index].ChildComment;
				}
				CommentElement item = this.CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000036BC File Offset: 0x000018BC
		public void Adjust3DTo1DNewRange(List<int> d1Indexes, int d2Min, int d3Min, Dictionary<Tuple<int, int, int>, int> source3dMap, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < d1Indexes.Count; i++)
			{
				Tuple<int, int, int> key = new Tuple<int, int, int>(d1Indexes[i], d2Min, d3Min);
				string comment = string.Empty;
				string childComment = string.Empty;
				if (source3dMap.ContainsKey(key))
				{
					int index = source3dMap[key];
					comment = this.child[index].Comment;
					childComment = this.child[index].ChildComment;
				}
				CommentElement item = this.CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003764 File Offset: 0x00001964
		public void Adjust2DTo3DNewRange(List<Tuple<int, int, int>> d3Indexes, int d3Min, Dictionary<Tuple<int, int>, int> source2dMap, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < d3Indexes.Count; i++)
			{
				Tuple<int, int, int> tuple = d3Indexes[i];
				string comment = string.Empty;
				string childComment = string.Empty;
				if (tuple.Item3 == d3Min)
				{
					Tuple<int, int> key = new Tuple<int, int>(tuple.Item1, tuple.Item2);
					if (source2dMap.ContainsKey(key))
					{
						int index = source2dMap[key];
						comment = this.child[index].Comment;
						childComment = this.child[index].ChildComment;
					}
				}
				CommentElement item = this.CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003828 File Offset: 0x00001A28
		public void Adjust3DTo2DNewRange(List<Tuple<int, int>> d2Indexes, int d3Min, Dictionary<Tuple<int, int, int>, int> source3dMap, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < d2Indexes.Count; i++)
			{
				Tuple<int, int, int> key = new Tuple<int, int, int>(d2Indexes[i].Item1, d2Indexes[i].Item2, d3Min);
				string comment = string.Empty;
				string childComment = string.Empty;
				if (source3dMap.ContainsKey(key))
				{
					int index = source3dMap[key];
					comment = this.child[index].Comment;
					childComment = this.child[index].ChildComment;
				}
				CommentElement item = this.CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			this.child.Clear();
			this.child.AddRange(list);
		}

		// Token: 0x0400000C RID: 12
		private List<CommentElement> child;

		// Token: 0x0400000D RID: 13
		private bool hasBuild;

	}
}
