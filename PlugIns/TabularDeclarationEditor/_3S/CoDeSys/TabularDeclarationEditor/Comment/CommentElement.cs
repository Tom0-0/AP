using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class CommentElement
	{
		public string Comment { get; set; }

		public string ChildComment { get; set; }

		public string FullComment { get; protected set; }

		public CommentElement Parent { get; set; }

		public virtual bool HasBuild => true;

		public CommentElement(string comment, CommentElement parent)
		{
			Comment = comment;
			Parent = parent;
		}

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

		public bool HasAnyChildComment()
		{
			return !string.IsNullOrEmpty(ChildComment);
		}

		public virtual string BuildChildComment()
		{
			return string.Empty;
		}

		protected virtual string FormatChildComment()
		{
			return string.Empty;
		}

		public virtual string GetFullComment()
		{
			FullComment = CommentUtility.GetEscape(Comment);
			return FullComment;
		}

		public virtual void BuildChildElement(IType type)
		{
		}

		public void ResumeFullComment()
		{
			string text = null;
			string childComment = ChildComment;
			text = (FullComment = ((Parent == null) ? $"{childComment}" : ((!string.IsNullOrEmpty(childComment)) ? $"{CommentUtility.GetEscape(Comment)}[{childComment}]" : $"{CommentUtility.GetEscape(Comment)}")));
		}
	}
}
