#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using _3S.CoDeSys.ApplicationObject;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Messages;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Options;
using _3S.CoDeSys.TabularDeclarationEditor.Comment;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.TabularDeclarationEditor
{
	internal static class Common
	{
		public static bool CanExpandAllComment(Guid _signatureGuid, string _stVariable)
		{
			//IPreCompileContext4 preCompileContext = APEnvironment.LanguageModelMgr.GetPrecompileContext(Common.GetApplicationGuid()) as IPreCompileContext4;
			//IIdentifierInfo[] identifierInfo = preCompileContext.GetIdentifierInfo(_signatureGuid, _stVariable);
			//if (identifierInfo != null && identifierInfo.Length != 0)
			//{
			//	if (identifierInfo[0].Variable != null && identifierInfo[0].Variable.GetFlag(VarFlag.Static))
			//	{
			//		return true;
			//	}
			//	ISignature signature = identifierInfo[0].Signature;
			//	if (signature == null)
			//	{
			//		return false;
			//	}
			//	if (signature.GetFlag(SignatureFlag.Alias))
			//	{
			//		return false;
			//	}
			//	if (signature.POUType == Operator.Program || signature.POUType == Operator.VarGlobal)
			//	{
			//		return true;
			//	}
			//}
			return false;
		}

		public static IExpression CalculateDefault(IPrecompileScope2 precompileScope, IType type, IIdentifierInfo[] _iiisEnums)
		{
			string text;
			if (_iiisEnums != null)
			{
				text = ((_iiisEnums.Length == 0) ? string.Empty : ((_iiisEnums[0].Variable == null || _iiisEnums[0].Variable.Type == null) ? _iiisEnums[0].Name : (_iiisEnums[0].Variable.Type.ToString() + "." + _iiisEnums[0].Name)));
			}
			else
			{
				IType realType = Common.GetRealType(precompileScope, type);
				TypeClass @class = realType.Class;
				if (@class != TypeClass.Bool)
				{
					switch (@class)
					{
						case TypeClass.Real:
						case TypeClass.LReal:
							text = "0.0";
							break;
						case TypeClass.String:
							text = "''";
							break;
						case TypeClass.WString:
							text = "\"\"";
							break;
						case TypeClass.Time:
							text = "TIME#0ms";
							break;
						case TypeClass.Date:
							text = "DATE#1970-01-01";
							break;
						case TypeClass.DateAndTime:
							text = "DT#1970-01-01-00:00";
							break;
						case TypeClass.TimeOfDay:
							text = "TOD#00:00";
							break;
						default:
							if (@class != TypeClass.LTime)
							{
								text = "0";
							}
							else
							{
								text = "LTIME#0ms";
							}
							break;
					}
				}
				else
				{
					text = "FALSE";
				}
			}
			if (text.Length == 0)
			{
				return null;
			}
			IScanner scanner = APEnvironment.LanguageModelMgr.CreateScanner(text, false, false, false, false);
			return APEnvironment.LanguageModelMgr.CreateParser(scanner).ParseOperand();
		}

		internal static void BuildChildElementAndPromptOption(CommentElement commentElment, IType type, bool isBuildEmpty)
		{
			try
			{
				commentElment.BuildChildElement(type);
			}
			catch (Exception)
			{
				if (isBuildEmpty)
				{
					commentElment.ChildComment = string.Empty;
					commentElment.BuildChildElement(type);
				}
			}
		}

		public static IExpression ExtractComponentInitValue(IExpression initValue, string stComponent)
		{
			IStructureInitialization val = (IStructureInitialization)(object)((initValue is IStructureInitialization) ? initValue : null);
			if (val == null || val.CompoInits == null)
			{
				return null;
			}
			IAssignmentExpression[] compoInits = val.CompoInits;
			IAssignmentExpression[] array = compoInits;
			foreach (IAssignmentExpression val2 in array)
			{
				if (val2.LValue != null && string.Equals(((IExprement)val2.LValue).ToString(), stComponent, StringComparison.OrdinalIgnoreCase))
				{
					return val2.RValue;
				}
			}
			return null;
		}
		public static IExpression ExtractElementInitValue(IExpression initValue, int nFlatIndex, IPrecompileScope scope)
		{
			IArrayInitialization arrayInitialization = initValue as IArrayInitialization;
			if (arrayInitialization == null || arrayInitialization.InitValues == null)
			{
				return null;
			}
			int num = 0;
			IExpression[] initValues = arrayInitialization.InitValues;
			foreach (IExpression expression in initValues)
			{
				if (expression is IMultipleIndexInitialization)
				{
					bool flag;
					int num2 = ((IMultipleIndexInitialization)expression).NumberInt(out flag, scope);
					if (!flag)
					{
						return null;
					}
					if (num <= nFlatIndex && nFlatIndex < num + num2)
					{
						return ((IMultipleIndexInitialization)expression).Value;
					}
					num += num2;
				}
				else
				{
					if (num == nFlatIndex)
					{
						return expression;
					}
					num++;
				}
			}
			return null;
		}

		public static IType GetRealType(IPrecompileScope2 precompileScope, IType _type)
		{
			if (_type.Class == TypeClass.Userdef)
			{
				IUserdefType2 userdefType = _type as IUserdefType2;
				if (precompileScope != null)
				{
					ISignature signature = precompileScope.FindSignatureGlobal(userdefType.NameExpression);
					if (signature != null && signature.GetFlag(SignatureFlag.Alias))
					{
						IType type = Common.ResolveAliasToBaseType(precompileScope, signature);
						if (type != null)
						{
							return type;
						}
					}
				}
			}
			return _type;
		}

		private static IType ResolveAliasToBaseType(IPrecompileScope2 scope, ISignature aliasSign)
		{
			IType type = null;
			while (aliasSign != null && aliasSign.GetFlag(SignatureFlag.Alias))
			{
				IVariable[] all = aliasSign.All;
				if (all.Length != 1)
				{
					break;
				}
				type = all[0].Type;
				if (!(type is IUserdefType2))
				{
					break;
				}
				IUserdefType2 userdefType = type as IUserdefType2;
				if (userdefType == null)
				{
					return null;
				}
				aliasSign = scope.FindSignatureGlobal(userdefType.NameExpression);
			}
			return type;
		}

		public static IIdentifierInfo[] GetEnumerationItems(IPreCompileContext pcc, Guid guidSignature, IType type)
		{
			if (pcc == null || type == null)
			{
				return null;
			}
			if (!IsEnumeration(pcc, type))
			{
				return null;
			}
			bool flag = true;
			IIdentifierInfo[] result = pcc.FindSubelements(guidSignature, ((object)type).ToString(), (FindSubelementsFlags)1, out flag);
			if (flag)
			{
				return null;
			}
			return result;
		}

		public static bool IsEnumeration(IPreCompileContext pcc, IType type)
		{
			if (pcc == null || type == null)
			{
				return false;
			}
			if ((int)type.Class == 25)
			{
				return true;
			}
			if ((int)type.Class == 28)
			{
				ISignature[] array = pcc.FindSignature(((object)type).ToString());
				if (array != null && array.Length == 1 && array[0] != null)
				{
					return array[0].GetFlag((SignatureFlag)2);
				}
			}
			return false;
		}

		internal static Guid GetAncestorApplicationGuid(IMetaObjectStub invokedBy)
        {
			while (invokedBy.ParentObjectGuid != Guid.Empty)
			{
				invokedBy = APEnvironment.ObjectMgr.GetMetaObjectStub(invokedBy.ProjectHandle, invokedBy.ParentObjectGuid);
				if (typeof(IApplicationObject).IsAssignableFrom(invokedBy.ObjectType))
				{
					return invokedBy.ObjectGuid;
				}
			}
			return Guid.Empty;
		}

        internal static Guid GetEditorUsedApplication(IMetaObjectStub invokedBy)
        {
			IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(invokedBy.ProjectHandle, invokedBy.ObjectGuid);
			while (metaObjectStub.ParentObjectGuid != Guid.Empty)
			{
				if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return metaObjectStub.ObjectGuid;
				}
				metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(invokedBy.ProjectHandle, metaObjectStub.ParentObjectGuid);
			}
			return Guid.Empty;
		}

        /// <summary>
        /// 判断数组是否越界
        /// </summary>
        /// <param name="array"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static bool IsArraySizeOutOfRange(IArrayType array, int maxSize = 65536)
		{
			IArrayType arrayType = array;
			long num = 1L;
			while (arrayType != null)
			{
				long num2 = 0L;
				try
				{
					long num3;
					long num4;
					num2 = Common.GetArrayDimensionSize(arrayType, out num3, out num4);
				}
				catch
				{
					return true;
				}
				if (num2 > (long)maxSize)
				{
					return true;
				}
				try
				{
					num = num2 * num;
				}
				catch
				{
					return true;
				}
				if (num > (long)maxSize)
				{
					return true;
				}
				arrayType = (arrayType.Base as IArrayType);
			}
			return false;
		}

		public static string FilterElemCommentAttr(string stAttr)
		{
			if (string.IsNullOrEmpty(stAttr))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = stAttr.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.None);
			foreach (string text in array)
			{
				if (text.IndexOf("ElemComment") == -1)
				{
					stringBuilder.Append(text + Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// 获取多维数组大小
		/// </summary>
		/// <param name="arrayType"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static long GetArrayDimensionSize(IArrayType arrayType, out long min, out long max)
		{
			long num = 0L;
			long num2 = 1L;
			for (int i = 0; i < arrayType.Dimensions.Length; i++)
			{
				long arrayDimensionSize = Common.GetArrayDimensionSize(arrayType.Dimensions[i], out min, out max);
				num2 *= arrayDimensionSize;
				if (i == 0)
				{
					num = min;
				}
			}
			min = num;
			max = num + num2 - 1L;
			return num2;
		}

		/// <summary>
		/// 获取多维数组大小
		/// </summary>
		/// <param name="arrayDimension"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static long GetArrayDimensionSize(IArrayDimension arrayDimension, out long min, out long max)
		{
			long result = 0L;
			long num = 0L;
			long num2 = 0L;
			if (arrayDimension.LowerBorder != null && arrayDimension.UpperBorder != null)
			{
				IPreCompileContext4 preCompileContext = APEnvironment.LanguageModelMgr.GetPrecompileContext(Common.GetApplicationGuid()) as IPreCompileContext4;
				IPrecompileScope4 scope = preCompileContext.CreatePrecompileScope(Guid.Empty) as IPrecompileScope4;
				ILiteralValue literalValue = (arrayDimension.LowerBorder as IExpression2).Literal(scope);
				ILiteralValue literalValue2 = (arrayDimension.UpperBorder as IExpression2).Literal(scope);
				literalValue.GetSignedLong(out num);
				literalValue2.GetSignedLong(out num2);
				result = num2 - num + 1L;
				min = num;
				max = num2;
				return result;
			}
			min = 0L;
			max = 0L;
			return result;
		}

		/// <summary>
		/// 获取APP的GUID
		/// </summary>
		/// <returns></returns>
		public static Guid GetApplicationGuid()
		{
			Guid empty = Guid.Empty;
			IProject primaryProject = APEnvironment.Engine.Projects.PrimaryProject;
			if (primaryProject != null)
			{
				foreach (Guid guid in SystemInstances.ObjectMgr.GetAllObjects(primaryProject.Handle))
				{
					IMetaObjectStub metaObjectStub = SystemInstances.ObjectMgr.GetMetaObjectStub(primaryProject.Handle, guid);
					if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
					{
						return guid;
					}
				}
			}
			return empty;
		}

		public static IType ParseType(string stype)
		{
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(stype, false, false, false, false);
			IParser obj = APEnvironment.LanguageModelMgr.CreateParser(val);
			IParser3 val2 = (IParser3)(object)((obj is IParser3) ? obj : null);
			if (val2 == null)
			{
				return null;
			}
			ICompiledType val3 = ((IParser)val2).ParseTypeDeclaration();
			if (val3 == null)
			{
				return null;
			}
			return (IType)(object)val3;
		}

		internal static bool SupportsExtendedProgrammingFeatures
		{
			get
			{
				IFeatureSettingsManager featureSettingsMgrOrNull = APEnvironment.FeatureSettingsMgrOrNull;
				if (featureSettingsMgrOrNull != null)
				{
					return featureSettingsMgrOrNull.GetFeatureSettingValue("language-model", "support-extended-programming-features", true);
				}
				return true;
			}
		}

		internal static bool EndsWithNewline(string st)
		{
			for (int num = st.Length - 1; num >= 0; num--)
			{
				if (!char.IsWhiteSpace(st[num]) || st[num] == '\r' || st[num] == '\n')
				{
					if (st[num] != '\r')
					{
						return st[num] == '\n';
					}
					return true;
				}
			}
			return false;
		}

		internal static LinkedListNode<ModelToken> GetNext(LinkedListNode<ModelToken> current)
		{
			if (current == null)
			{
				return null;
			}
			current = current.Next;
			while (current != null && current.Value.HasType(ModelTokenType.AnyBlankOrComment | ModelTokenType.Pragma))
			{
				current = current.Next;
			}
			return current;
		}

		internal static LinkedListNode<ModelToken> Match(LinkedListNode<ModelToken> current, ModelTokenType type)
		{
			LinkedListNode<ModelToken> next = GetNext(current);
			if (next == null || !next.Value.HasType(type))
			{
				return null;
			}
			return next;
		}

		internal static string GetScopeText(ModelTokenType scope, bool bConstant, bool bRetain, bool bPersistent)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			LStringBuilder val = new LStringBuilder();
			switch (scope)
			{
			case ModelTokenType.Var:
				val.Append("VAR");
				break;
			case ModelTokenType.VarAccess:
				val.Append("VAR_ACCESS");
				break;
			case ModelTokenType.VarConfig:
				val.Append("VAR_CONFIG");
				break;
			case ModelTokenType.VarExternal:
				val.Append("VAR_EXTERNAL");
				break;
			case ModelTokenType.VarGlobal:
				val.Append("VAR_GLOBAL");
				break;
			case ModelTokenType.VarInOut:
				val.Append("VAR_IN_OUT");
				break;
			case ModelTokenType.VarInput:
				val.Append("VAR_INPUT");
				break;
			case ModelTokenType.VarOutput:
				val.Append("VAR_OUTPUT");
				break;
			case ModelTokenType.VarStat:
				val.Append("VAR_STAT");
				break;
			case ModelTokenType.VarTemp:
				val.Append("VAR_TEMP");
				break;
			}
			if (bConstant)
			{
				val.Append(" CONSTANT");
			}
			if (bRetain)
			{
				val.Append(" RETAIN");
			}
			if (bPersistent)
			{
				val.Append(" PERSISTENT");
			}
			return ((object)val).ToString();
		}

		private static T ReturnOrThrow<T>(T value, string stMessage, bool bThrow)
		{
			if (bThrow)
			{
				throw new FormatException(stMessage);
			}
			return value;
		}

		internal static string[] ParseDeclaratorList(string st, bool bThrowExceptionOnError)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Invalid comparison between Unknown and I4
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Invalid comparison between Unknown and I4
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Invalid comparison between Unknown and I4
			List<string> list = new List<string>();
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(st, false, false, false, false);
			IParser val2 = APEnvironment.LanguageModelMgr.CreateParser(val);
			TokenType next;
			IToken val3 = default(IToken);
			do
			{
				int sourceOffset = val.SourceOffset;
				IExpression obj = val2.ParseOperand();
				int sourceOffset2 = val.SourceOffset;
				if (obj != null && sourceOffset2 - sourceOffset > 0)
				{
					string item = st.Substring(sourceOffset, sourceOffset2 - sourceOffset).Trim();
					list.Add(item);
					next = val.GetNext(out val3);
					if ((int)next == 21)
					{
						return list.ToArray();
					}
					continue;
				}
				return ReturnOrThrow(list.ToArray(), Resources.InvalidName, bThrowExceptionOnError);
			}
			while ((int)next == 15 && (int)val.GetOperator(val3) == 171);
			return ReturnOrThrow(list.ToArray(), Resources.InvalidName, bThrowExceptionOnError);
		}

		internal static IEnumerable<string> SubtractDeclaratorList(string[] minuend, string[] subtrahend)
		{
			LHashSet<string> val = new LHashSet<string>();
			string[] array = minuend;
			foreach (string text in array)
			{
				val.Add(text.ToLowerInvariant());
			}
			array = subtrahend;
			foreach (string text2 in array)
			{
				val.Remove(text2.ToLowerInvariant());
			}
			return (IEnumerable<string>)val;
		}

		internal static bool ParseAddress(string st, bool bThrowExceptionOnError)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			if (string.IsNullOrWhiteSpace(st))
			{
				return true;
			}
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(st, true, true, true, false);
			IToken val2 = default(IToken);
			TokenType next = val.GetNext(out val2);
			if ((int)next - 7 > 1 || (int)val.GetNext(out val2) != 21)
			{
				return ReturnOrThrow(value: false, Resources.InvalidAddress, bThrowExceptionOnError);
			}
			return true;
		}

		internal static bool ParseInitValue(string st, bool bThrowExceptionOnError)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Invalid comparison between Unknown and I4
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(st, true, true, true, true);
			bool flag = true;
			IToken val2 = default(IToken);
			while ((int)val.GetNext(out val2) != 21)
			{
				if ((int)val2.Type == 2 || (int)val2.Type == 3 || (int)val2.Type == 20 || (int)val2.Type == 4)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				return ReturnOrThrow(value: false, Resources.InvalidInitValue, bThrowExceptionOnError);
			}
			return true;
		}

		internal static bool ParseDataType(string st, ResolvedDeclarationContext context, ModelTokenType scope, bool bThrowExceptionOnError)
		{
			if (!CheckType(st))
			{
				return ReturnOrThrow(value: false, Resources.InvalidDataType, bThrowExceptionOnError);
			}
			if (st.Trim().ToUpperInvariant() == "BIT")
			{
				if (context != ResolvedDeclarationContext.FunctionBlock && context != 0)
				{
					return ReturnOrThrow(value: false, Resources.BitsForStructureOnly, bThrowExceptionOnError);
				}
				if (scope == ModelTokenType.VarTemp || scope == ModelTokenType.VarStat || scope == ModelTokenType.VarInOut)
				{
					return ReturnOrThrow(value: false, Resources.BitsWrongScope, bThrowExceptionOnError);
				}
			}
			if (st.Trim().ToUpperInvariant().Contains("REFERENCE TO"))
			{
				switch (scope)
				{
				case ModelTokenType.VarOutput:
					return ReturnOrThrow(value: false, Resources.NoReferenceToOutput, bThrowExceptionOnError);
				case ModelTokenType.VarInOut:
					return ReturnOrThrow(value: false, Resources.ReferenceNotAllowedForVarInOut, bThrowExceptionOnError);
				}
			}
			return true;
		}

		internal static string GetCommentText(LinkedListNode<ModelToken> token)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (token == null)
			{
				return string.Empty;
			}
			string text = EnsureSlashNLineBreaks(token.Value.Text);
			LStringBuilder val = new LStringBuilder();
			switch (token.Value.Type)
			{
			case ModelTokenType.PascalCommentOccupyingWholeLine:
			case ModelTokenType.PascalCommentInMixedLine:
			{
				string[] array = text.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					string text7 = array[i].Trim();
					if (text7.StartsWith("(*"))
					{
						text7 = text7.Substring(2);
					}
					if (text7.EndsWith("*)"))
					{
						text7 = text7.Substring(0, text7.Length - 2);
					}
					text7 = text7.Trim();
					if (val.Length > 0)
					{
						val.Append("\n");
					}
					val.Append(text7);
				}
				break;
			}
			case ModelTokenType.CPlusPlusCommentOccupyingWholeLine:
			case ModelTokenType.CPlusPlusCommentInMixedLine:
			{
				if (text.EndsWith("\n"))
				{
					text = text.Substring(0, text.Length - 1);
				}
				int num2 = int.MaxValue;
				string[] array = text.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					string text5 = array[i].Trim();
					if (text5.StartsWith("//"))
					{
						text5 = text5.Substring(2);
					}
					int k;
					for (k = 0; k < text5.Length && char.IsWhiteSpace(text5[k]); k++)
					{
					}
					num2 = Math.Min(num2, k);
					if (num2 == 0)
					{
						break;
					}
				}
				array = text.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					string text6 = array[i].Trim();
					if (text6.StartsWith("//"))
					{
						text6 = text6.Substring(2);
					}
					text6 = text6.Substring(num2);
					if (val.Length > 0)
					{
						val.Append("\n");
					}
					val.Append(text6);
				}
				break;
			}
			case ModelTokenType.DocumentationComment:
			{
				if (text.EndsWith("\n"))
				{
					text = text.Substring(0, text.Length - 1);
				}
				int num = int.MaxValue;
				string[] array = text.Split('\n');
				foreach (string text2 in array)
				{
					string text3 = text2.Trim();
					if (text3.StartsWith("///"))
					{
						text3 = text3.Substring(3);
					}
					int j;
					for (j = 0; j < text2.Length && char.IsWhiteSpace(text2[j]); j++)
					{
					}
					num = Math.Min(num, j);
					if (num == 0)
					{
						break;
					}
				}
				array = text.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					string text4 = array[i].Trim();
					if (text4.StartsWith("///"))
					{
						text4 = text4.Substring(3);
					}
					text4 = text4.Substring(num);
					if (val.Length > 0)
					{
						val.Append("\n");
					}
					val.Append(text4);
				}
				break;
			}
			default:
				val.Append(text);
				break;
			}
			return EnsureSlashRSlashNLineBreaks(((object)val).ToString());
		}

		internal static ModelToken CreateCommentToken(string stComment, bool bIndent)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			stComment = EnsureSlashNLineBreaks(stComment);
			bool flag = false;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml("<root>" + stComment + "</root>");
				foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
				{
					if (childNode is XmlElement)
					{
						flag = true;
						break;
					}
				}
			}
			catch
			{
			}
			LStringBuilder val = new LStringBuilder();
			bool flag2 = true;
			string[] array = stComment.Split('\n');
			foreach (string text in array)
			{
				if (bIndent && !flag2)
				{
					val.Append("\t");
				}
				if (flag)
				{
					val.Append("/// ");
				}
				else
				{
					val.Append("// ");
				}
				val.Append(text);
				val.Append("\n");
				flag2 = false;
			}
			if (flag)
			{
				return new ModelToken(ModelTokenType.DocumentationComment, ((object)val).ToString());
			}
			return new ModelToken(ModelTokenType.CPlusPlusCommentOccupyingWholeLine, ((object)val).ToString());
		}

		internal static string GetAttributes(LinkedListNode<ModelToken>[] tokens)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Expected O, but got Unknown
			if (tokens == null)
			{
				return string.Empty;
			}
			LStringBuilder val = new LStringBuilder();
			for (int i = 0; i < tokens.Length; i++)
			{
				string text = tokens[i].Value.Text.Trim();
				Debug.Assert(text.StartsWith("{"));
				Debug.Assert(text.EndsWith("}"));
				text = text.Substring(1, text.Length - 2).Trim();
				val.AppendLine(text);
			}
			return EnsureSlashRSlashNLineBreaks(((object)val).ToString());
		}

		internal static ModelToken[] CreateAttributeBlockTokens(string stAttributes, bool bIndent)
		{
			LList<ModelToken> val = new LList<ModelToken>();
			bool flag = true;
			string[] array = stAttributes.Split('\r', '\n');
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text.Trim()))
				{
					if (bIndent && !flag)
					{
						val.Add(new ModelToken(ModelTokenType.Whitespace, "\t"));
					}
					val.Add(new ModelToken(ModelTokenType.Pragma, "{" + text.Trim() + "}"));
					val.Add(new ModelToken(ModelTokenType.EndOfLine, "\n"));
					flag = false;
				}
			}
			return val.ToArray();
		}

		private static string EnsureSlashRSlashNLineBreaks(string stText)
		{
			return stText.Replace("\r\n", "\0").Replace("\r", "\0").Replace("\n", "\0")
				.Replace("\0", "\r\n");
		}

		private static string EnsureSlashNLineBreaks(string stText)
		{
			return stText.Replace("\r\n", "\n").Replace("\r", "\n");
		}

		internal static Guid GetApplicationGuid(int nProjectHandle, Guid objectGuid)
		{
			IMetaObjectStub metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, objectGuid);
			Debug.Assert(metaObjectStub != null);
			while (metaObjectStub.ParentObjectGuid != Guid.Empty)
			{
				metaObjectStub = APEnvironment.ObjectMgr.GetMetaObjectStub(nProjectHandle, metaObjectStub.ParentObjectGuid);
				Debug.Assert(metaObjectStub != null);
				if (typeof(IApplicationObject).IsAssignableFrom(metaObjectStub.ObjectType))
				{
					return metaObjectStub.ObjectGuid;
				}
			}
			return Guid.Empty;
		}

		internal static bool CheckIdentifier(string stText)
		{
			IScanner obj = APEnvironment.LanguageModelMgr.CreateScanner(stText, true, true, true, true);
			Debug.Assert(obj != null);
			IToken val = default(IToken);
			return obj.Match((TokenType)13, true, out val) > 0;
		}

		internal static bool CheckQualifiedIdentifier(string stText)
		{
			Debug.Assert(stText != null);
			string[] array = stText.Split('.');
			for (int i = 0; i < array.Length; i++)
			{
				if (!CheckIdentifier(array[i].Trim()))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool CheckQualifiedIdentifierList(string stText)
		{
			Debug.Assert(stText != null);
			string[] array = stText.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (!CheckQualifiedIdentifier(array[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool CheckType(string stText)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			Debug.Assert(stText != null);
			IScanner val = APEnvironment.LanguageModelMgr.CreateScanner(stText, false, false, true, false);
			IParser val2 = APEnvironment.LanguageModelMgr.CreateParser(val);
			ICompiledType val4;
			if (val2 is IParser4)
			{
				IMessage val3 = default(IMessage);
				val4 = ((IParser4)val2).ParseTypeDeclaration(out val3);
				if (val3 != null && ((int)val3.Severity == 2 || (int)val3.Severity == 1))
				{
					val4 = null;
				}
			}
			else
			{
				val4 = val2.ParseTypeDeclaration();
			}
			return val4 != null;
		}
	}
}
