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
    class CommentModel : AbstractTreeTableModel
	{
		private string _stVariable;

		private Guid _signatureGuid;

		private Guid _appGuid;

		private IPreCompileContext _pcc;

		private IPrecompileScope _scope;

		private Dictionary<string, IIdentifierInfo[]> _typeInfos = new Dictionary<string, IIdentifierInfo[]>();

		internal IPreCompileContext CompileContext => _pcc;

		internal IPrecompileScope Scope => _scope;

		public int COLUMN_TYPE = 1;

		internal Guid SignatureGuid
		{
			get
			{
				return this._signatureGuid;
			}
		}

		internal CommentModel(string stVariable, string stType, string stCommentValue, string stElemCommentValue, IMetaObjectStub invokedBy)
        {
			_stVariable = stVariable;
			if (stCommentValue == null)
			{
				throw new ArgumentNullException("stInitValue");
			}
			if (invokedBy == null)
			{
				throw new ArgumentNullException("invokedBy");
			}
			_signatureGuid = invokedBy.ObjectGuid;
			stCommentValue = stCommentValue.Trim();
			_appGuid = Common.GetEditorUsedApplication(invokedBy);
			_pcc = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetPrecompileContext(Common.GetAncestorApplicationGuid(invokedBy));
			if (_pcc == null)
			{
				return;
			}
			_scope = _pcc.CreatePrecompileScope(_signatureGuid);
			if (_scope == null)
			{
				return;
			}
			IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stType, false, false, false, false);
			IParser obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
			IParser3 val2 = (IParser3)(object)((obj is IParser3) ? obj : null);
			if (val2 == null)
			{
				return;
			}
			ICompiledType val3 = ((IParser)val2).ParseTypeDeclaration();
			if (val3 == null)
			{
				return;
			}
			// 添加表达式,数据类型,注释表头
			UnderlyingModel.AddColumn(Resources.Expression, HorizontalAlignment.Left, (ITreeTableViewRenderer)(object)new ExpressionRenderer(), TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Resources.Datatype, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, TextBoxTreeTableViewEditor.TextBox, false);
			UnderlyingModel.AddColumn(Resources.Comment, HorizontalAlignment.Left, LabelTreeTableViewRenderer.NormalString, ArrayCommentEditor.MultiTextEditor, true);
			IIdentifierInfo[] array = null;
			if (Common.IsEnumeration(_pcc, (IType)(object)val3))
			{
				array = Common.GetEnumerationItems(_pcc, SignatureGuid, (IType)(object)val3);
				if (array == null)
				{
					return;
				}
			}
			CommentElement commentElement = CommentElement.BuildRootNode((IType)(object)val3, stCommentValue, stElemCommentValue);
			CommentNode commentNode = new CommentNode(this, _appGuid, _signatureGuid, null, 0, stVariable, (IType)(object)val3, bStructMember: false, array, commentElement);
			UnderlyingModel.AddRootNode((ITreeTableNode)(object)commentNode);
		}

		internal IIdentifierInfo[] GetSubElements(IType type)
		{
			string text = type.ToString();
			IIdentifierInfo[] result;
			if (this._typeInfos.TryGetValue(text, out result))
			{
				return result;
			}
			bool flag;
			IIdentifierInfo[] array = this.CompileContext.FindSubelements(this.SignatureGuid, text, FindSubelementsFlags.IncludeLocalVars | FindSubelementsFlags.IncludeArrayElements | FindSubelementsFlags.SearchForType | FindSubelementsFlags.ExcludeSubSignatures | FindSubelementsFlags.ExcludeProperties, out flag);
			if (flag)
			{
				return null;
			}
			array = this.GetInOutIdentifierInfo(type, array);
			this._typeInfos.Add(text, array);
			return array;
		}

		private IIdentifierInfo[] GetInOutIdentifierInfo(IType type, IIdentifierInfo[] array)
		{
			ISignature signature;
			if (!this.IsFbType(type, out signature))
			{
				return array;
			}
			List<IIdentifierInfo> list = new List<IIdentifierInfo>();
			foreach (IIdentifierInfo identifierInfo in array)
			{
				if (identifierInfo.Variable != null && (identifierInfo.Variable.GetFlag(VarFlag.Input) || identifierInfo.Variable.GetFlag(VarFlag.Output) || identifierInfo.Variable.GetFlag(VarFlag.Inout)))
				{
					list.Add(identifierInfo);
				}
			}
			return list.ToArray();
		}

		internal bool IsFbType(IType type, out ISignature signature)
		{
			signature = this._pcc.GetSignature(type.ToString());
			return signature != null && signature.POUType == Operator.FunctionBlock;
		}

		public bool CanExpandAll()
		{
			IIdentifierInfo[] identifierInfo = this._pcc.GetIdentifierInfo(this._signatureGuid, this._stVariable);
			if (identifierInfo != null && identifierInfo.Length != 0)
			{
				ISignature signature = identifierInfo[0].Signature;
				if (identifierInfo[0].Variable.GetFlag(VarFlag.Static))
				{
					return true;
				}
				if (signature.POUType == Operator.Program || signature.POUType == Operator.VarGlobal)
				{
					return true;
				}
			}
			return false;
		}

		internal string GetCommentValue()
		{
			if (base.Sentinel.ChildCount > 0)
			{
				CommentNode commentNode = (CommentNode)base.Sentinel.GetChild(0);
				string arg = commentNode.CommentElement.BuildChildComment();
				return string.Format("{0}|attribute 'ElemComment':='{1}'", CommentUtility.GetEscapeSpliter(commentNode.CommentElement.Comment), arg) + Environment.NewLine;
			}
			return string.Empty;
		}


	}
}
