using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    class StructureCommentElement : CommentElement
	{
		public override bool HasBuild
		{
			get
			{
				return this.hasBuild;
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000532A File Offset: 0x0000352A
		public StructureCommentElement(string comment, CommentElement parent) : base(comment, parent)
		{
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005334 File Offset: 0x00003534
		public void SetPropertyType(IIdentifierInfo[] array)
		{
			this.propertyType = new Dictionary<string, IType>();
			if (array != null)
			{
				foreach (IIdentifierInfo identifierInfo in array)
				{
					this.propertyType.Add(identifierInfo.Name, identifierInfo.Type);
				}
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000537C File Offset: 0x0000357C
		public override void BuildChildElement(IType type)
		{
			if (this.hasBuild || !(type is IUserdefType))
			{
				return;
			}
			this.child = new Dictionary<string, CommentElement>();
			Dictionary<string, IType> dictionary = this.propertyType;
			Dictionary<string, string> myChildrens = this.GetMyChildrens(base.ChildComment);
			foreach (KeyValuePair<string, IType> keyValuePair in dictionary)
			{
				bool flag = keyValuePair.Value.Class == TypeClass.Array;
				bool flag2 = keyValuePair.Value.Class == TypeClass.Userdef;
				string text = myChildrens.ContainsKey(keyValuePair.Key) ? myChildrens[keyValuePair.Key] : string.Empty;
				string comment = string.Empty;
				string empty = string.Empty;
				if (!string.IsNullOrEmpty(text))
				{
					CommentUtility.GetComment(text, out comment, out empty);
				}
				comment = CommentUtility.GetOriginal(comment);
				if (flag)
				{
					ArrayCommentElement arrayCommentElement = new ArrayCommentElement(comment, this);
					arrayCommentElement.ChildComment = empty;
					this.child.Add(keyValuePair.Key, arrayCommentElement);
				}
				else if (flag2)
				{
					StructureCommentElement structureCommentElement = new StructureCommentElement(comment, this);
					structureCommentElement.ChildComment = empty;
					this.child.Add(keyValuePair.Key, structureCommentElement);
				}
				else
				{
					CommentElement commentElement = new CommentElement(comment, this);
					commentElement.ChildComment = empty;
					this.child.Add(keyValuePair.Key, commentElement);
				}
			}
			this.hasBuild = true;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000054FC File Offset: 0x000036FC
		private Dictionary<string, IType> GetChildType(IUserdefType userdef)
		{
			return null;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000054FF File Offset: 0x000036FF
		private Dictionary<string, string> GetMyChildrens(string childComment)
		{
			if (string.IsNullOrEmpty(childComment))
			{
				return new Dictionary<string, string>();
			}
			return CommentUtility.GetChildDictionary(childComment);
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005518 File Offset: 0x00003718
		public CommentElement GetChildBy(string key)
		{
			foreach (string text in this.child.Keys)
			{
				if (string.Equals(text, key, StringComparison.OrdinalIgnoreCase))
				{
					return this.child[text];
				}
			}
			return null;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005588 File Offset: 0x00003788
		public Dictionary<string, string> GetAllChild()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (this.child != null)
			{
				foreach (KeyValuePair<string, CommentElement> keyValuePair in this.child)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value.Comment);
				}
				return dictionary;
			}
			return dictionary;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005600 File Offset: 0x00003800
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

		// Token: 0x060000FB RID: 251 RVA: 0x0000567C File Offset: 0x0000387C
		public override string BuildChildComment()
		{
			if (this.child != null && this.child.Count > 0)
			{
				foreach (CommentElement commentElement in this.child.Values)
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

		// Token: 0x060000FC RID: 252 RVA: 0x00005708 File Offset: 0x00003908
		protected override string FormatChildComment()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.child == null)
			{
				return string.Empty;
			}
			foreach (KeyValuePair<string, CommentElement> keyValuePair in this.child)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Value.FullComment))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(keyValuePair.Key);
					stringBuilder.Append('(');
					stringBuilder.Append(keyValuePair.Value.FullComment);
					stringBuilder.Append(')');
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000061 RID: 97
		private Dictionary<string, CommentElement> child;

		// Token: 0x04000062 RID: 98
		private Dictionary<string, IType> propertyType;

		// Token: 0x04000063 RID: 99
		private bool hasBuild;
	}
}
