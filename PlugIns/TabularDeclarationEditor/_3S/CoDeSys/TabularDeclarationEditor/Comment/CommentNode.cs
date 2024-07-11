using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using System;
using System.Collections.Generic;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
    internal class CommentNode : ITreeTableNode
    {
		public CommentElement CommentElement
		{
			get
			{
				return this._commentElement;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600024D RID: 589 RVA: 0x00011EFB File Offset: 0x000100FB
		public int ChildCount
		{
			get
			{
				this.FillChildNodes();
				return this._childNodes.Count;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600024E RID: 590 RVA: 0x00011F0E File Offset: 0x0001010E
		// (set) Token: 0x0600024F RID: 591 RVA: 0x00011F18 File Offset: 0x00010118
		internal Tuple<int, int, int, int, bool> MonitoringRange
		{
			get
			{
				return this._tupleMonitoringRange;
			}
			set
			{
				this._tupleMonitoringRange = value;
				if (value != null)
				{
					this.Release();
					try
					{
						this.FillChildNodes();
						this.RaiseStructureChanged();
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000250 RID: 592 RVA: 0x00011F58 File Offset: 0x00010158
		internal Guid GuidObject
		{
			get
			{
				return this._objectGuid;
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00011F60 File Offset: 0x00010160
		private void RaiseStructureChanged()
		{
			this._model.RaiseStructureChanged(new TreeTableModelEventArgs(this, -1, null));
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000252 RID: 594 RVA: 0x00011F75 File Offset: 0x00010175
		internal Guid GuidApplication
		{
			get
			{
				return this._applicationGuid;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000253 RID: 595 RVA: 0x00011F7D File Offset: 0x0001017D
		public string Expression
		{
			get
			{
				return this._stExpression;
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000254 RID: 596 RVA: 0x00011F85 File Offset: 0x00010185
		public bool HasChildren
		{
			get
			{
				this.FillChildNodes();
				return this._childNodes != null && this._childNodes.Count > 0;
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000255 RID: 597 RVA: 0x00011FA5 File Offset: 0x000101A5
		public ITreeTableNode Parent
		{
			get
			{
				return this._parentNode;
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00011FB0 File Offset: 0x000101B0
		internal CommentNode(CommentModel model, Guid appGuid, Guid objectGuid, CommentNode parentNode, int nIndex, string stExpression, IType type, bool bStructMember, IIdentifierInfo[] iiisEnums, CommentElement commentElement)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (stExpression == null)
			{
				throw new ArgumentNullException("stExpression");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this._model = model;
			this._applicationGuid = appGuid;
			this._objectGuid = objectGuid;
			this._parentNode = parentNode;
			this._nIndex = nIndex;
			this._stExpression = stExpression;
			this._type = type;
			this._bStructMember = bStructMember;
			this._iiisEnums = iiisEnums;
			this._commentElement = commentElement;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0001203C File Offset: 0x0001023C
		public ITreeTableNode GetChild(int nIndex)
		{
			this.FillChildNodes();
			return this._childNodes[nIndex];
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00012050 File Offset: 0x00010250
		public int GetIndex(ITreeTableNode node)
		{
			if (node is CommentNode)
			{
				this.FillChildNodes();
				return this._childNodes.IndexOf((CommentNode)node);
			}
			return -1;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00012074 File Offset: 0x00010274
		public object GetValue(int nColumnIndex)
		{
			switch (nColumnIndex)
			{
				case 0:
					{
						int num = this._stExpression.LastIndexOf('.');
						if (num >= 0)
						{
							return this._stExpression.Substring(num + 1);
						}
						return this._stExpression;
					}
				case 1:
					if (this._type == null)
					{
						return string.Empty;
					}
					return this._type.ToString();
				case 2:
					if (this._commentElement.Comment != null)
					{
						return this._commentElement.Comment;
					}
					return string.Empty;
				default:
					throw new ArgumentOutOfRangeException("nColumnIndex");
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00012100 File Offset: 0x00010300
		public bool IsEditable(int nColumnIndex)
		{
			if (nColumnIndex == 1)
			{
				return false;
			}
			if (nColumnIndex != 2)
			{
				return false;
			}
			if (this._parentNode == null)
			{
				return true;
			}
			TypeClass @class = this._parentNode.GetRealType().Class;
			return true;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0001212D File Offset: 0x0001032D
		public bool IsEnumeration()
		{
			return this._iiisEnums != null;
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00012138 File Offset: 0x00010338
		public void SetValue(int nColumnIndex, object value)
		{
			if (nColumnIndex == 2)
			{
				this.SetValue(value.ToString(), true);
				return;
			}
			throw new InvalidOperationException("This cell is read-only.");
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00002E9E File Offset: 0x0000109E
		public void SwapChildren(int nIndex1, int nIndex2)
		{
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00012158 File Offset: 0x00010358
		internal bool SetValue(string stValue, bool reportError)
		{
			if (string.IsNullOrEmpty(stValue))
			{
				this._commentElement.Comment = null;
				this._model.RaiseChanged(new TreeTableModelEventArgs(this._parentNode, this._nIndex, this));
				return true;
			}
			try
			{
				this._commentElement.Comment = stValue;
			}
			catch
			{
				if (reportError)
				{
					APEnvironment.MessageService.Error(string.Format(Resources.InvalidExpression, stValue), "InvalidExpression", Array.Empty<object>());
				}
				this._commentElement.Comment = null;
				return false;
			}
			if (this._commentElement.Comment == null)
			{
				this._commentElement.Comment = "";
			}
			this._model.RaiseChanged(new TreeTableModelEventArgs(this._parentNode, this._nIndex, this));
			return true;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00012228 File Offset: 0x00010428
		internal string GetValue()
		{
			return this.GetValue(1).ToString();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00012238 File Offset: 0x00010438
		private IType ResolveAliasToBaseType(IPrecompileScope2 scope, ISignature aliasSign)
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

		// Token: 0x06000261 RID: 609 RVA: 0x00012290 File Offset: 0x00010490
		public IType GetRealType()
		{
			if (this._type.Class == TypeClass.Userdef)
			{
				IUserdefType2 userdefType = this._type as IUserdefType2;
				IPrecompileScope2 precompileScope = this._model.Scope as IPrecompileScope2;
				if (precompileScope != null)
				{
					ISignature signature = precompileScope.FindSignatureGlobal(userdefType.NameExpression);
					if (signature != null && signature.GetFlag(SignatureFlag.Alias))
					{
						IType type = this.ResolveAliasToBaseType(precompileScope, signature);
						if (type != null)
						{
							return type;
						}
					}
				}
			}
			return this._type;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000122FC File Offset: 0x000104FC
		private int GetMyLevel()
		{
			int num = 1;
			for (ITreeTableNode parent = this.Parent; parent != null; parent = parent.Parent)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00012323 File Offset: 0x00010523
		private bool IsArrayOfArray(IType itype)
		{
			if (itype is IUserdefType || itype is IPointerType || itype is IReferenceType)
			{
				return false;
			}
			IArrayType arrayType = itype as IArrayType;
			return true;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x00012348 File Offset: 0x00010548
		private bool CheckNodeTypeExpand(IType type)
		{
			int myLevel = this.GetMyLevel();
			return false;
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00012364 File Offset: 0x00010564
		public bool HasTheSameUserdefTypeToRoot()
		{
			if (this._type is IUserdefType)
			{
				for (CommentNode commentNode = this.Parent as CommentNode; commentNode != null; commentNode = (commentNode.Parent as CommentNode))
				{
					if (commentNode._type == this._type)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000123B0 File Offset: 0x000105B0
		private void FillChildNodes()
		{
			if (this._childNodes != null || this.IsEnumeration())
			{
				return;
			}
			this._childNodes = new List<ITreeTableNode>();
			IType realType = Common.GetRealType(this._model.Scope as IPrecompileScope2, this._type);
			IIdentifierInfo[] subElements = this._model.GetSubElements(realType);
			bool flag = false;
			if (this._parentNode != null)
			{
				ISignature signature;
				flag = this._model.IsFbType(this._parentNode._type, out signature);
			}
			if (flag)
			{
				return;
			}
			if (subElements == null)
			{
				return;
			}
			if (!this._model.CanExpandAll() && this._type is IUserdefType)
			{
				return;
			}
			if (this.HasTheSameUserdefTypeToRoot())
			{
				return;
			}
			int num = 0;
			int num2 = MonitoringRangeContext.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY - 1;
			if (this.MonitoringRange != null)
			{
				num = this.MonitoringRange.Item1;
				num2 = this.MonitoringRange.Item2;
				int item = this.MonitoringRange.Item3;
				if (this.MonitoringRange.Item5)
				{
					num -= item;
					num2 -= item;
				}
			}
			int num3 = Math.Min(subElements.Length, num2 - num + 1);
			if (realType is IUserdefType)
			{
				((StructureCommentElement)this._commentElement).SetPropertyType(subElements);
			}
			Common.BuildChildElementAndPromptOption(this._commentElement, this._type, true);
			for (int i = 0; i < num3; i++)
			{
				string text = this._stExpression;
				bool bStructMember;
				CommentElement childBy;
				if (subElements[i + num].Name.StartsWith("["))
				{
					text += subElements[i + num].Name;
					bStructMember = false;
					childBy = ((ArrayCommentElement)this._commentElement).GetChildBy(i + num);
				}
				else
				{
					text = text + "." + subElements[i + num].Name;
					bStructMember = true;
					childBy = ((StructureCommentElement)this._commentElement).GetChildBy(subElements[i + num].Name);
				}
				IIdentifierInfo[] iiisEnums = null;
				if (Common.IsEnumeration(this._model.CompileContext, subElements[i + num].Type))
				{
					iiisEnums = Common.GetEnumerationItems(this._model.CompileContext, this._model.SignatureGuid, subElements[i + num].Type);
				}
				CommentNode item2 = new CommentNode(this._model, this._applicationGuid, this._objectGuid, this, i, text, subElements[i + num].Type, bStructMember, iiisEnums, childBy);
				this._childNodes.Add(item2);
			}
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00012603 File Offset: 0x00010803
		private void Release()
		{
			this._childNodes.Clear();
			this._childNodes = null;
		}

		// Token: 0x040000FC RID: 252
		private CommentElement _commentElement;

		// Token: 0x040000FD RID: 253
		private CommentModel _model;

		// Token: 0x040000FE RID: 254
		private CommentNode _parentNode;

		// Token: 0x040000FF RID: 255
		private int _nIndex;

		// Token: 0x04000100 RID: 256
		private List<ITreeTableNode> _childNodes;

		// Token: 0x04000101 RID: 257
		private int _realNumDisplayValues;

		// Token: 0x04000102 RID: 258
		private string _stExpression;

		// Token: 0x04000103 RID: 259
		private Guid _applicationGuid;

		// Token: 0x04000104 RID: 260
		private Guid _objectGuid;

		// Token: 0x04000105 RID: 261
		private IType _type;

		// Token: 0x04000106 RID: 262
		private bool _bStructMember;

		// Token: 0x04000107 RID: 263
		private IIdentifierInfo[] _iiisEnums;

		// Token: 0x04000108 RID: 264
		private const int COLIDX_EXPRESSION = 0;

		// Token: 0x04000109 RID: 265
		private Tuple<int, int, int, int, bool> _tupleMonitoringRange;

		// Token: 0x0400010A RID: 266
		private const int COLIDX_DATATYPE = 2;

		// Token: 0x0400010B RID: 267
		private const int COLIDX_COMMENT = 3;
	}
}