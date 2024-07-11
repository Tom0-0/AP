using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	internal class InitValueModel : AbstractTreeTableModel
	{
		private IPreCompileContext _pcc;

		private IPrecompileScope _scope;

		private Guid _signatureGuid;

		private Dictionary<string, IIdentifierInfo[]> _typeInfos = new Dictionary<string, IIdentifierInfo[]>();

		private Dictionary<string, Editable<IIdentifierInfo>[]> _typeInfos2 = new Dictionary<string, Editable<IIdentifierInfo>[]>();

		internal static int MaxDisplayValues = 1000;

		private IMetaObjectStub _invokedBy;

		internal IPreCompileContext CompileContext => _pcc;

		internal IPrecompileScope Scope => _scope;

		internal Guid SignatureGuid => _signatureGuid;

		internal InitValueModel(string stVariable, string stType, string stInitValue, string stComment, string stElemComment, IMetaObjectStub invokedBy)
		{
			if (stInitValue == null)
			{
				throw new ArgumentNullException("stInitValue");
			}
			if (invokedBy == null)
			{
				throw new ArgumentNullException("invokedBy");
			}
			_invokedBy = invokedBy;
			_signatureGuid = invokedBy.ObjectGuid;
			stInitValue = stInitValue.Trim();
			IType compiledType = null;
			IIdentifierInfo[] array = null;
			IExpression initializationExpression = GetInitializationExpression(stType, stInitValue, out compiledType, out array);
			UnderlyingModel.AddColumn(Resources.Expression, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new ExpressionRenderer(), TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Resources.InitValue, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new EditableComboBoxTreeTableViewRenderer(), (ITreeTableViewEditor)(object)new EditableComboBoxTreeTableViewEditor(), true);
			UnderlyingModel.AddColumn(Resources.Datatype, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Resources.Comment, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			InitValueNode initValueNode = new InitValueNode(this, null, 0, stVariable, initializationExpression, null, compiledType, bStructMember: false, array, stComment);
			try
			{
				CommentElement commentElement2 = (initValueNode.CommentElement = CommentElement.BuildRootNode(compiledType, stComment, stElemComment));
			}
			catch
			{
			}
			UnderlyingModel.AddRootNode((ITreeTableNode)(object)initValueNode);
		}

		internal IExpression GetInitializationExpression(string stType, string stInitValue, out IType compiledType, out IIdentifierInfo[] array)
		{
			compiledType = null;
			array = null;
			_pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(Common.GetAncestorApplicationGuid(_invokedBy));
			if (_pcc == null)
			{
				return null;
			}
			_scope = _pcc.CreatePrecompileScope(_signatureGuid);
			if (_scope == null)
			{
				return null;
			}
			IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stType, false, false, false, false);
			IParser obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
			IParser5 val2 = (IParser5)(object)((obj is IParser5) ? obj : null);
			if (val2 == null)
			{
				return null;
			}
			compiledType = (IType)(object)((IParser)val2).ParseTypeDeclaration();
			if (compiledType == null)
			{
				return null;
			}
			if (Common.IsEnumeration(_pcc, compiledType))
			{
				array = Common.GetEnumerationItems(_pcc, SignatureGuid, compiledType);
				if (array == null)
				{
					return null;
				}
			}
			string text = string.Format("VAR hugo: {0} {1}; END_VAR", stType, string.IsNullOrEmpty(stInitValue) ? string.Empty : (":= " + stInitValue));
			val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(text, false, false, false, false);
			IParser obj2 = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
			val2 = (IParser5)(object)((obj2 is IParser5) ? obj2 : null);
			if (val2 == null)
			{
				return null;
			}
			IStatement obj3 = ((IParser3)val2).ParseVariableDeclarationList();
			ISequenceStatement val3 = (ISequenceStatement)(object)((obj3 is ISequenceStatement) ? obj3 : null);
			IVariableDeclarationListStatement val4 = (IVariableDeclarationListStatement)((val3 != null && val3.Statements.Length != 0) ? (val3.Statements[0] as IVariableDeclarationListStatement) : null);
			IVariableDeclarationStatement val5 = ((val4 != null && val4.Declarations.Length != 0) ? val4.Declarations[0] : null);
			if (val5 == null)
			{
				return null;
			}
			return val5.InitializationExpression;
		}

		internal string GetInitValue()
		{
			bool isDefaultInitValue;
			if (((AbstractTreeTableModel)this).Sentinel.ChildCount > 0)
			{
				return ((InitValueNode)(object)((AbstractTreeTableModel)this).Sentinel.GetChild(0)).GetInitValue(returnStringEvenIfDefaultValue: true, bArrayInit: false, out isDefaultInitValue);
			}
			return string.Empty;
		}

		internal IIdentifierInfo[] GetSubElements(IType type)
		{
			if (_typeInfos.TryGetValue(((object)type).ToString(), out var value))
			{
				return value;
			}
			bool flag = default(bool);
			IIdentifierInfo[] array = CompileContext.FindSubelements(SignatureGuid, ((object)type).ToString(), (FindSubelementsFlags)199, out flag);
			if (flag)
			{
				return null;
			}
			_typeInfos.Add(((object)type).ToString(), array);
			return array;
		}

		internal Editable<IIdentifierInfo>[] GetSubElements2(IType type)
		{
			if (_typeInfos2.TryGetValue(((object)type).ToString(), out var value))
			{
				return value;
			}
			bool flag = default(bool);
			IIdentifierInfo[] array = CompileContext.FindSubelements(SignatureGuid, ((object)type).ToString(), (FindSubelementsFlags)199, out flag);
			if (flag)
			{
				return null;
			}
			Editable<IIdentifierInfo>[] inOutIdentifierInfo = GetInOutIdentifierInfo(type, array);
			_typeInfos2.Add(((object)type).ToString(), inOutIdentifierInfo);
			return inOutIdentifierInfo;
		}

		internal bool IsFbType(IType type, out ISignature signature)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			signature = ((ICompileContextCommon)_pcc).GetSignature(((object)type).ToString());
			if (signature == null)
			{
				return false;
			}
			if ((int)signature.POUType != 88)
			{
				return false;
			}
			return true;
		}

		private Editable<IIdentifierInfo>[] GetInOutIdentifierInfo(IType type, IIdentifierInfo[] array)
		{
			if (!IsFbType(type, out var _))
			{
				return Editable<IIdentifierInfo>.GetEditDecorate(array, canEdit: true);
			}
			Editable<IIdentifierInfo>[] array2 = new Editable<IIdentifierInfo>[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Variable != null)
				{
					if (array[i].Variable.GetFlag((VarFlag)2) || array[i].Variable.GetFlag((VarFlag)4))
					{
						array2[i] = new Editable<IIdentifierInfo>(array[i], canEdit: true);
					}
					else
					{
						array2[i] = new Editable<IIdentifierInfo>(array[i], canEdit: false);
					}
				}
				else
				{
					array2[i] = new Editable<IIdentifierInfo>(array[i], canEdit: false);
				}
			}
			return array2;
		}

		internal string GetAttribute(string expression)
		{
			IIdentifierInfo[] identifierInfo = CompileContext.GetIdentifierInfo(SignatureGuid, expression);
			if (identifierInfo != null && identifierInfo.Length != 0)
			{
				IVariable variable = identifierInfo[0].Variable;
				if (variable.HasAttribute("ElemComment"))
				{
					return variable.GetAttributeValue("ElemComment");
				}
				return string.Empty;
			}
			return string.Empty;
		}
	}
}
