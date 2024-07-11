using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class StructureCommentElement : CommentElement
	{
		private Dictionary<string, CommentElement> child;

		private Dictionary<string, IType> propertyType;

		private bool hasBuild;

		public override bool HasBuild => hasBuild;

		public StructureCommentElement(string comment, CommentElement parent)
			: base(comment, parent)
		{
		}

		public void SetPropertyType(IIdentifierInfo[] array)
		{
			propertyType = new Dictionary<string, IType>();
			if (array != null)
			{
				foreach (IIdentifierInfo val in array)
				{
					propertyType.Add(val.Name, val.Type);
				}
			}
		}

		public override void BuildChildElement(IType type)
		{
			if (hasBuild)
			{
				return;
			}
			IUserdefType val = (IUserdefType)(object)((type is IUserdefType) ? type : null);
			if (val == null)
			{
				return;
			}
			child = new Dictionary<string, CommentElement>();
			Dictionary<string, IType> dictionary = propertyType;
			Dictionary<string, string> myChildrens = GetMyChildrens(base.ChildComment);
			foreach (KeyValuePair<string, IType> item in dictionary)
			{
				bool flag = (int)item.Value.Class == 26;
				bool flag2 = (int)item.Value.Class == 28;
				string text = (myChildrens.ContainsKey(item.Key) ? myChildrens[item.Key] : string.Empty);
				string comment = string.Empty;
				string childComment = string.Empty;
				if (!string.IsNullOrEmpty(text))
				{
					CommentUtility.GetComment(text, out comment, out childComment);
				}
				comment = CommentUtility.GetOriginal(comment);
				if (flag)
				{
					ArrayCommentElement arrayCommentElement = new ArrayCommentElement(comment, this);
					arrayCommentElement.ChildComment = childComment;
					child.Add(item.Key, arrayCommentElement);
				}
				else if (flag2)
				{
					StructureCommentElement structureCommentElement = new StructureCommentElement(comment, this);
					structureCommentElement.ChildComment = childComment;
					child.Add(item.Key, structureCommentElement);
				}
				else
				{
					CommentElement commentElement = new CommentElement(comment, this);
					commentElement.ChildComment = childComment;
					child.Add(item.Key, commentElement);
				}
			}
			hasBuild = true;
		}

		private Dictionary<string, IType> GetChildType(IUserdefType userdef)
		{
			return null;
		}

		private Dictionary<string, string> GetMyChildrens(string childComment)
		{
			if (string.IsNullOrEmpty(childComment))
			{
				return new Dictionary<string, string>();
			}
			return CommentUtility.GetChildDictionary(childComment);
		}

		public CommentElement GetChildBy(string key)
		{
			foreach (string key2 in child.Keys)
			{
				if (string.Equals(key2, key, StringComparison.OrdinalIgnoreCase))
				{
					return child[key2];
				}
			}
			return null;
		}

		public Dictionary<string, string> GetAllChild()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (child != null)
			{
				foreach (KeyValuePair<string, CommentElement> item in child)
				{
					dictionary.Add(item.Key, item.Value.Comment);
				}
				return dictionary;
			}
			return dictionary;
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
				foreach (CommentElement value in child.Values)
				{
					if (value.HasBuild)
					{
						value.BuildChildComment();
						value.GetFullComment();
					}
					else
					{
						value.ResumeFullComment();
					}
				}
			}
			return GetFullComment();
		}

		protected override string FormatChildComment()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (child == null)
			{
				return string.Empty;
			}
			foreach (KeyValuePair<string, CommentElement> item in child)
			{
				if (!string.IsNullOrEmpty(item.Value.FullComment))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(item.Key);
					stringBuilder.Append('(');
					stringBuilder.Append(item.Value.FullComment);
					stringBuilder.Append(')');
				}
			}
			return stringBuilder.ToString();
		}
	}
}
