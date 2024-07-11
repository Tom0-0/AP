using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.WatchList.Comment
{
    //[TypeGuid("{9507FA28-5D1A-42A8-B345-6CC78F875652}")]
	public class VariableCommentProvider
    {

		public VariableCommentProvider()
		{ }
		public VariableCommentResult GetPatchCommentElementForArray(string expression, Guid objectGuid, params int[] startIndex)
		{
			IPreCompileContext pcc = null;
			ISignature signature = APEnvironment.LanguageModelMgr.FindSignature(objectGuid, out pcc);
			VariableCommentResult typeSymbolCommentForArray = this.GetTypeSymbolCommentForArray(objectGuid, expression, Array.Empty<int>());
			if (!expression.Contains('.'))
			{
				return typeSymbolCommentForArray;
			}
			string expression2 = expression.Substring(0, expression.IndexOf('.'));
			IVariable variable = Common.GetVariable(objectGuid, expression2);
			if (variable == null)
			{
				return typeSymbolCommentForArray;
			}
			string comment;
			string text;
			this.GetArrayCommentWithAttribute(variable, out comment, out text);
			if (string.IsNullOrEmpty(text))
			{
				return typeSymbolCommentForArray;
			}
			string[] array = expression.Split(new char[]
			{
				'.'
			});
			int i = 0;
			CommentElement commentElement = null;
			IType type = variable.Type;
			while (i < array.Length)
			{
				string text2 = array[i];
				string text3 = text2;
				int[] array2 = null;
				if (text2.Contains('['))
				{
					array2 = Common.GetIndexOfArray(text2, out text3);
				}
				if (i == 0)
				{
					commentElement = CommentElement.BuildRootNode(type, comment, text);
					if (!commentElement.HasAnyChildComment())
					{
						return typeSymbolCommentForArray;
					}
					this.SetCommentElmentPropertyType(pcc, signature.ObjectGuid, commentElement, type);
					commentElement.BuildChildElement(type);
					if (type is IArrayType && array2 != null)
					{
						IArrayType arrayType = type as IArrayType;
						this.GetArraySymbolComment(pcc, signature.ObjectGuid, arrayType, array2, ref commentElement, ref type);
						if (!commentElement.HasAnyChildComment())
						{
							return typeSymbolCommentForArray;
						}
					}
				}
				else if (type is IUserdefType)
				{
					IUserdefType type2 = type as IUserdefType;
					commentElement = ((StructureCommentElement)commentElement).GetChildBy(text3);
					if (!commentElement.HasAnyChildComment())
					{
						return typeSymbolCommentForArray;
					}
					IType propertyTypeOfUserdefType = this.GetPropertyTypeOfUserdefType(pcc, signature.ObjectGuid, type2, text3);
					this.SetCommentElmentPropertyType(pcc, signature.ObjectGuid, commentElement, propertyTypeOfUserdefType);
					commentElement.BuildChildElement(propertyTypeOfUserdefType);
					type = propertyTypeOfUserdefType;
					if (propertyTypeOfUserdefType is IArrayType && array2 != null)
					{
						IArrayType arrayType2 = propertyTypeOfUserdefType as IArrayType;
						this.GetArraySymbolComment(pcc, signature.ObjectGuid, arrayType2, array2, ref commentElement, ref type);
						if (!commentElement.HasAnyChildComment())
						{
							return typeSymbolCommentForArray;
						}
					}
				}
				i++;
			}
			ArrayCommentElement arrayCommentElement = (ArrayCommentElement)commentElement;
			VariableCommentResult variableCommentResult = new VariableCommentResult
			{
				Comment = arrayCommentElement.Comment
			};
			int count = ((ArrayCommentElement)commentElement).GetCount();
			for (int j = 0; j < count; j++)
			{
				variableCommentResult.AddChild(j.ToString(), arrayCommentElement.GetChildBy(j).Comment);
			}
			variableCommentResult.MergeTypeVariableCommentResult(typeSymbolCommentForArray);
			return variableCommentResult;
		}

		public VariableCommentResult GetPatchCommentElementForUserdef(string expression, Guid objectGuid)
		{
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Expected O, but got Unknown
			IPreCompileContext pcc = null;
			ISignature val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).FindSignature(objectGuid, out pcc);
			VariableCommentResult typeSymbolCommentForUserdef = GetTypeSymbolCommentForUserdef(objectGuid, expression);
			string text = expression;
			if (expression.Contains('.'))
			{
				text = expression.Substring(0, expression.IndexOf('.'));
				IVariable variable = Common.GetVariable(objectGuid, text);
				if (variable == null)
				{
					return typeSymbolCommentForUserdef;
				}
				GetArrayCommentWithAttribute(variable, out var comment, out var commentAttribute);
				if (string.IsNullOrEmpty(commentAttribute))
				{
					return typeSymbolCommentForUserdef;
				}
				string[] array = expression.Split('.');
				int i = 0;
				CommentElement arrayParent = null;
				IType iType = variable.Type;
				for (; i < array.Length; i++)
				{
					string text2 = array[i];
					string varName = text2;
					int[] array2 = null;
					if (text2.Contains('['))
					{
						array2 = Common.GetIndexOfArray(text2, out varName);
					}
					if (i == 0)
					{
						arrayParent = CommentElement.BuildRootNode(iType, comment, commentAttribute);
						if (!arrayParent.HasAnyChildComment())
						{
							return typeSymbolCommentForUserdef;
						}
						SetCommentElmentPropertyType(pcc, val.ObjectGuid, arrayParent, iType);
						arrayParent.BuildChildElement(iType);
						if (iType is IArrayType && array2 != null)
						{
							IArrayType arrayType = (IArrayType)(object)((iType is IArrayType) ? iType : null);
							GetArraySymbolComment(pcc, val.ObjectGuid, arrayType, array2, ref arrayParent, ref iType);
							if (!arrayParent.HasAnyChildComment())
							{
								return typeSymbolCommentForUserdef;
							}
						}
					}
					else
					{
						if (!(iType is IUserdefType))
						{
							continue;
						}
						IUserdefType type = (IUserdefType)(object)((iType is IUserdefType) ? iType : null);
						arrayParent = ((StructureCommentElement)arrayParent).GetChildBy(varName);
						if (!arrayParent.HasAnyChildComment())
						{
							return typeSymbolCommentForUserdef;
						}
						IType propertyTypeOfUserdefType = GetPropertyTypeOfUserdefType(pcc, val.ObjectGuid, type, varName);
						SetCommentElmentPropertyType(pcc, val.ObjectGuid, arrayParent, propertyTypeOfUserdefType);
						arrayParent.BuildChildElement(propertyTypeOfUserdefType);
						iType = propertyTypeOfUserdefType;
						if (propertyTypeOfUserdefType is IArrayType && array2 != null)
						{
							IArrayType arrayType2 = (IArrayType)(object)((propertyTypeOfUserdefType is IArrayType) ? propertyTypeOfUserdefType : null);
							GetArraySymbolComment(pcc, val.ObjectGuid, arrayType2, array2, ref arrayParent, ref iType);
							if (!arrayParent.HasAnyChildComment())
							{
								return typeSymbolCommentForUserdef;
							}
						}
					}
				}
				VariableCommentResult val2 = new VariableCommentResult();
				val2.Comment=((StructureCommentElement)arrayParent).Comment;
				VariableCommentResult val3 = val2;
				foreach (KeyValuePair<string, string> item in ((StructureCommentElement)arrayParent).GetAllChild())
				{
					val3.AddChild(item.Key, item.Value);
				}
				val3.MergeTypeVariableCommentResult(typeSymbolCommentForUserdef);
				return val3;
			}
			return typeSymbolCommentForUserdef;
		}

		private VariableCommentResult GetTypeSymbolCommentForUserdef(Guid objectGuid, string expression)
		{
			IVariable variable = Common.GetVariable(objectGuid, expression);
			if (variable == null)
			{
				return null;
			}
			VariableCommentResult variableCommentResult = new VariableCommentResult();
			variableCommentResult.Comment = variable.Comment;
			IPreCompileContext pcc = null;
			ISignature signature = APEnvironment.LanguageModelMgr.FindSignature(objectGuid, out pcc);
			IType type = null;
			string comment;
			string text;
			this.GetArrayCommentWithAttribute(variable, out comment, out text);
			string typeString = variable.Type.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				if (variable.Type.Class == TypeClass.Array)
				{
					CommentElement commentElement = CommentElement.BuildRootNode(variable.Type, comment, text);
					commentElement.BuildChildElement(variable.Type);
					string text2 = expression;
					int[] indexOfArray = Common.GetIndexOfArray(expression, out text2);
					this.GetArraySymbolComment(pcc, signature.ObjectGuid, variable.Type as IArrayType, indexOfArray, ref commentElement, ref type);
					using (Dictionary<string, string>.Enumerator enumerator = ((StructureCommentElement)commentElement).GetAllChild().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, string> keyValuePair = enumerator.Current;
							variableCommentResult.AddChild(keyValuePair.Key, keyValuePair.Value);
						}
						goto IL_17D;
					}
				}
				CommentElement commentElement2 = CommentElement.BuildRootNode(variable.Type, comment, text);
				this.SetCommentElmentPropertyType(pcc, signature.ObjectGuid, commentElement2, variable.Type);
				commentElement2.BuildChildElement(variable.Type);
				foreach (KeyValuePair<string, string> keyValuePair2 in ((StructureCommentElement)commentElement2).GetAllChild())
				{
					variableCommentResult.AddChild(keyValuePair2.Key, keyValuePair2.Value);
				}
			IL_17D:
				typeString = ((type == null) ? variable.Type.ToString() : type.ToString());
			}
			else
			{
				type = variable.Type;
				while (type is IArrayType)
				{
					type = ((IArrayType)type).Base;
					IType @base = ((IArrayType)variable.Type).Base;
				}
				typeString = type.ToString();
			}
			IIdentifierInfo[] iidentifierInfoBy = this.GetIIdentifierInfoBy(typeString, pcc, signature.ObjectGuid);
			if (iidentifierInfoBy == null)
			{
				return variableCommentResult;
			}
			foreach (IIdentifierInfo identifierInfo in iidentifierInfoBy)
			{
				VariableCommentResult child = variableCommentResult.GetChild(identifierInfo.Name);
				if (child != null)
				{
					if (string.IsNullOrEmpty(child.Comment))
					{
						child.Comment = identifierInfo.Comment;
					}
				}
				else
				{
					variableCommentResult.AddChild(identifierInfo.Name, identifierInfo.Comment);
				}
			}
			return variableCommentResult;
		}

		private IIdentifierInfo[] GetIIdentifierInfoBy(string typeString, IPreCompileContext pcc, Guid objectGuid)
		{
			bool flag;
			IIdentifierInfo[] result = pcc.FindSubelements(objectGuid, typeString, FindSubelementsFlags.IncludeLocalVars | FindSubelementsFlags.IncludeArrayElements | FindSubelementsFlags.SearchForType | FindSubelementsFlags.ExcludeSubSignatures | FindSubelementsFlags.ExcludeProperties, out flag);
			if (flag)
			{
				return null;
			}
			return result;
		}

		private VariableCommentResult GetTypeSymbolCommentForArray(Guid objectGuid, string expression, params int[] index)
		{
			VariableCommentResult variableCommentResult = new VariableCommentResult();
			IPreCompileContext pcc = null;
			ISignature signature = APEnvironment.LanguageModelMgr.FindSignature(objectGuid, out pcc);
			IVariable variable = Common.GetVariable(objectGuid, expression);
			if (variable == null)
			{
				return null;
			}
			IType type = variable.Type;
			if (type.Class != TypeClass.Array)
			{
				return null;
			}
			string comment;
			string commentAttribute;
			this.GetArrayCommentWithAttribute(variable, out comment, out commentAttribute);
			CommentElement commentElement = CommentElement.BuildRootNode(type, comment, commentAttribute);
			commentElement.BuildChildElement(type);
			string text = expression;
			if (expression.EndsWith("]"))
			{
				int[] indexOfArray = Common.GetIndexOfArray(expression, out text);
				IType type2 = null;
				this.GetArraySymbolComment(pcc, signature.ObjectGuid, type as IArrayType, indexOfArray, ref commentElement, ref type2);
			}
			int count = ((ArrayCommentElement)commentElement).GetCount();
			variableCommentResult.Comment = commentElement.Comment;
			for (int i = 0; i < count; i++)
			{
				variableCommentResult.AddChild(i.ToString(), ((ArrayCommentElement)commentElement).GetChildBy(i).Comment);
			}
			return variableCommentResult;
		}

		private void GetArraySymbolComment(IPreCompileContext pcc, Guid SignatureGuid, IArrayType arrayType, int[] index, ref CommentElement arrayParent, ref IType iType)
		{
			int num = index.Length;
			int i = 0;
			IArrayType arrayType2 = arrayType;
			while (i < num)
			{
				int num2 = 0;
				int num3 = 1;
				int num4 = 1;
				for (int j = arrayType2.Dimensions.Count<IArrayDimension>() - 1; j >= 0; j--)
				{
					int num5;
					int num6;
					Common.GetArrayDimensionSize(arrayType2.Dimensions[j], out num5, out num6);
					int num7 = num6 - num5 + 1;
					int num8 = index[i + j];
					if (num8 < num5 || num8 > num5 + num7 - 1)
					{
						throw new InvalidOperationException("index out of range");
					}
					num2 += (num8 - num5) * num4;
					num4 = num7 * num4;
					num3 *= num7;
				}
				arrayParent = ((ArrayCommentElement)arrayParent).GetChildBy(num2);
				this.SetCommentElmentPropertyType(pcc, SignatureGuid, arrayParent, arrayType2.Base);
				arrayParent.BuildChildElement(arrayType2.Base);
				i += arrayType2.Dimensions.Length;
				iType = arrayType2.Base;
				if (arrayType2.Base is IArrayType)
				{
					arrayType2 = (arrayType2.Base as IArrayType);
				}
			}
		}

		internal void SetCommentElmentPropertyType(IPreCompileContext pcc, Guid SignatureGuid, CommentElement element, IType type)
		{
			if (type is IUserdefType && element is StructureCommentElement)
			{
				string stAccessPathOrType = type.ToString();
				bool flag;
				IIdentifierInfo[] propertyType = pcc.FindSubelements(SignatureGuid, stAccessPathOrType, FindSubelementsFlags.IncludeLocalVars | FindSubelementsFlags.IncludeArrayElements | FindSubelementsFlags.SearchForType | FindSubelementsFlags.ExcludeSubSignatures | FindSubelementsFlags.ExcludeProperties, out flag);
				if (!flag)
				{
					((StructureCommentElement)element).SetPropertyType(propertyType);
				}
			}
		}

		private IType GetPropertyTypeOfUserdefType(IPreCompileContext pcc, Guid objectGuid, IUserdefType type, string property)
		{
			IIdentifierInfo[] subElements = Common.GetSubElements(pcc, objectGuid, type);
			if (subElements == null)
			{
				return null;
			}
			IIdentifierInfo identifierInfo = subElements.FirstOrDefault((IIdentifierInfo t) => string.Equals(t.Name, property, StringComparison.OrdinalIgnoreCase));
			if (identifierInfo == null)
			{
				return null;
			}
			return identifierInfo.Type;
		}

		private void GetArrayCommentWithAttribute(IVariable variable, out string comment, out string commentAttribute)
		{
			comment = variable.Comment;
			commentAttribute = string.Empty;
			if (variable.HasAttribute("ElemComment"))
			{
				commentAttribute = variable.GetAttributeValue("ElemComment");
			}
		}

	}
}
