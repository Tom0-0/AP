using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class ArrayCommentElement : CommentElement
	{
		private List<CommentElement> child;

		private bool hasBuild;

		public override bool HasBuild => hasBuild;

		public ArrayCommentElement(string comment, CommentElement parent)
			: base(comment, parent)
		{
		}

		public override void BuildChildElement(IType type)
		{
			if (hasBuild)
			{
				return;
			}
			IArrayType val = (IArrayType)(object)((type is IArrayType) ? type : null);
			if (val == null)
			{
				return;
			}
			child = new List<CommentElement>();
			List<CommentFormat> childComment = CommentUtility.GetChildComment(base.ChildComment);
			for (int i = 0; i < childComment.Count; i++)
			{
				CommentFormat commentFormat = childComment[i];
				int n = commentFormat.N;
				CommentUtility.GetComment(commentFormat.Comment, out var comment, out var childComment2);
				for (int j = 0; j < n; j++)
				{
					CommentElement item = CreateCommentElement(val, CommentUtility.GetOriginal(comment), childComment2);
					child.Add(item);
				}
			}
			Common.GetArrayDimensionSize(val, out var min, out var max);
			int num = (int)(max - min + 1);
			if (child.Count < num)
			{
				for (int k = child.Count; k < num; k++)
				{
					CommentElement item2 = CreateCommentElement(val, string.Empty, string.Empty);
					child.Add(item2);
				}
			}
			if (child.Count > num)
			{
				child.RemoveRange(num, child.Count - num);
			}
			hasBuild = true;
		}

		private CommentElement CreateCommentElement(IArrayType arrayType, string comment, string childComment)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			bool flag = (int)arrayType.Base.Class == 26;
			bool flag2 = (int)arrayType.Base.Class == 28;
			if (flag)
			{
				ArrayCommentElement arrayCommentElement = new ArrayCommentElement(comment, this);
				arrayCommentElement.ChildComment = childComment;
				return arrayCommentElement;
			}
			if (flag2)
			{
				StructureCommentElement structureCommentElement = new StructureCommentElement(comment, this);
				structureCommentElement.ChildComment = childComment;
				return structureCommentElement;
			}
			CommentElement commentElement = new CommentElement(comment, this);
			commentElement.ChildComment = childComment;
			return commentElement;
		}

		public CommentElement GetChildBy(int index)
		{
			return child[index];
		}

		public int GetCount()
		{
			return child.Count;
		}

		public override string GetFullComment()
		{
			string text = FormatChildComment();
			if (base.Parent == null)
			{
				base.FullComment = $"{text}";
			}
			else if (string.IsNullOrEmpty(text))
			{
				base.FullComment = $"{CommentUtility.GetEscape(base.Comment)}";
			}
			else
			{
				base.FullComment = $"{CommentUtility.GetEscape(base.Comment)}[{text}]";
			}
			return base.FullComment;
		}

		public override string BuildChildComment()
		{
			if (child != null && child.Count > 0)
			{
				foreach (CommentElement item in child)
				{
					if (item.HasBuild)
					{
						item.BuildChildComment();
						item.GetFullComment();
					}
					else
					{
						item.ResumeFullComment();
					}
				}
			}
			return GetFullComment();
		}

		protected override string FormatChildComment()
		{
			List<CommentFormat> list = new List<CommentFormat>();
			for (int i = 0; i < child.Count; i++)
			{
				CommentElement commentElement = child[i];
				if (i == 0)
				{
					CommentFormat item = new CommentFormat
					{
						N = 1,
						Comment = commentElement.FullComment
					};
					list.Add(item);
					continue;
				}
				CommentFormat commentFormat = list[list.Count - 1];
				if (string.Equals(commentElement.FullComment, commentFormat.Comment))
				{
					commentFormat.N++;
					continue;
				}
				CommentFormat item2 = new CommentFormat
				{
					N = 1,
					Comment = commentElement.FullComment
				};
				list.Add(item2);
			}
			return CommentFormat.GetCommentFormats(list);
		}

		public string RebuldAdjustedComment()
		{
			if (child != null && child.Count > 0)
			{
				foreach (CommentElement item in child)
				{
					item.ResumeFullComment();
				}
			}
			return GetFullComment();
		}

		public void Adjust1DNewRange(int headAdd, int reserverStart, int reserverEnd, int total, IArrayType arrayType)
		{
			child.RemoveRange(0, reserverStart);
			for (int i = 0; i < headAdd; i++)
			{
				CommentElement item = CreateCommentElement(arrayType, string.Empty, string.Empty);
				child.Insert(0, item);
			}
			if (child.Count < total)
			{
				for (int j = child.Count; j < total; j++)
				{
					CommentElement item2 = CreateCommentElement(arrayType, string.Empty, string.Empty);
					child.Add(item2);
				}
			}
			if (child.Count > total)
			{
				child.RemoveRange(total, child.Count - total);
			}
		}

		public void Adjust2DNewRange(Dictionary<Tuple<int, int>, int> oldMap, Dictionary<int, Tuple<int, int>> newMap, int total, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < total; i++)
			{
				Tuple<int, int> key = newMap[i];
				if (oldMap.ContainsKey(key))
				{
					int index = oldMap[key];
					list.Add(child[index]);
				}
				else
				{
					CommentElement item = CreateCommentElement(arrayType, string.Empty, string.Empty);
					list.Add(item);
				}
			}
			child.Clear();
			child.AddRange(list);
		}

		public void Adjust3DNewRange(Dictionary<Tuple<int, int, int>, int> oldMap, Dictionary<int, Tuple<int, int, int>> newMap, int total, IArrayType arrayType)
		{
			List<CommentElement> list = new List<CommentElement>();
			for (int i = 0; i < total; i++)
			{
				Tuple<int, int, int> key = newMap[i];
				if (oldMap.ContainsKey(key))
				{
					int index = oldMap[key];
					list.Add(child[index]);
				}
				else
				{
					CommentElement item = CreateCommentElement(arrayType, string.Empty, string.Empty);
					list.Add(item);
				}
			}
			child.Clear();
			child.AddRange(list);
		}

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
					comment = child[index].Comment;
					childComment = child[index].ChildComment;
				}
				CommentElement item = CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			child.Clear();
			child.AddRange(list);
		}

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
					comment = child[index].Comment;
					childComment = child[index].ChildComment;
				}
				CommentElement item2 = CreateCommentElement(arrayType, comment, childComment);
				list.Add(item2);
			}
			child.Clear();
			child.AddRange(list);
		}

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
					comment = child[index].Comment;
					childComment = child[index].ChildComment;
				}
				CommentElement item = CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			child.Clear();
			child.AddRange(list);
		}

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
					comment = child[index].Comment;
					childComment = child[index].ChildComment;
				}
				CommentElement item = CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			child.Clear();
			child.AddRange(list);
		}

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
						comment = child[index].Comment;
						childComment = child[index].ChildComment;
					}
				}
				CommentElement item = CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			child.Clear();
			child.AddRange(list);
		}

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
					comment = child[index].Comment;
					childComment = child[index].ChildComment;
				}
				CommentElement item = CreateCommentElement(arrayType, comment, childComment);
				list.Add(item);
			}
			child.Clear();
			child.AddRange(list);
		}
	}
}
