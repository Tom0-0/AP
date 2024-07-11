using _3S.CoDeSys.Controls.Controls;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.TabularDeclarationEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
    internal class InitValueNode : ITreeTableNode
    {
		private InitValueModel _model;

		private InitValueNode _parentNode;

		private int _nIndex;

		private List<ITreeTableNode> _childNodes;

		private int _realNumDisplayValues;

		private string _stExpression;

		private IExpression _initValue;

		private IExpression _defaultInitValue;

		private IType _type;

		private bool _bStructMember;

		private IIdentifierInfo[] _iiisEnums;

		private const int COLIDX_EXPRESSION = 0;

		private const int COLIDX_INITVALUE = 1;

		private const int COLIDX_DATATYPE = 2;

		private const int COLIDX_COMMENT = 3;

		private string _comment;

		private bool _isChildReady;

		private bool _notFbOrFbInOut = true;

		private Dictionary<int, string> _cacheInitValue = new Dictionary<int, string>();

		private Dictionary<int, bool> _cacheInitIsDefault = new Dictionary<int, bool>();

		private CommentElement _commentElement;

		private Tuple<int, int, int, int, bool> _tupleMonitoringRange;

		public int ChildCount
		{
			get
			{
				FillChildNodes();
				return _childNodes.Count;
			}
		}

		public bool HasChildren => CheckChildExists();

		public ITreeTableNode Parent => (ITreeTableNode)(object)_parentNode;

		public bool IsChildReady => _isChildReady;

		public bool NotFbOrFbInOut
		{
			get
			{
				return _notFbOrFbInOut;
			}
			set
			{
				_notFbOrFbInOut = value;
			}
		}

		public string Expression => _stExpression;

		public int Index => _nIndex;

		public IType Type => _type;

		public CommentElement CommentElement
		{
			get
			{
				return _commentElement;
			}
			set
			{
				_commentElement = value;
			}
		}

		internal Tuple<int, int, int, int, bool> MonitoringRange
		{
			get
			{
				return _tupleMonitoringRange;
			}
			set
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Expected O, but got Unknown
				_tupleMonitoringRange = value;
				if (value != null)
				{
					_childNodes.Clear();
					_childNodes = null;
					FillChildNodes();
					((AbstractTreeTableModel)_model).RaiseStructureChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)this, -1, (ITreeTableNode)null));
				}
			}
		}

		internal InitValueNode(InitValueModel model, InitValueNode parentNode, int nIndex, string stExpression, IExpression initValue, IExpression defaultInitValue, IType type, bool bStructMember, IIdentifierInfo[] iiisEnums, string comment)
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
			_model = model;
			_parentNode = parentNode;
			_nIndex = nIndex;
			_stExpression = stExpression;
			_initValue = initValue;
			_defaultInitValue = defaultInitValue;
			_type = type;
			_bStructMember = bStructMember;
			_iiisEnums = iiisEnums;
			_comment = comment;
			if (_defaultInitValue == null)
			{
				ref IExpression defaultInitValue2 = ref _defaultInitValue;
				IPrecompileScope scope = _model.Scope;
				defaultInitValue2 = Common.CalculateDefault((IPrecompileScope2)(object)((scope is IPrecompileScope2) ? scope : null), _type, _iiisEnums);
			}
			if (_initValue == null)
			{
				ResetToDefault();
			}
		}

		public ITreeTableNode GetChild(int nIndex)
		{
			FillChildNodes();
			return _childNodes[nIndex];
		}

		public int GetIndex(ITreeTableNode node)
		{
			if (node is InitValueNode)
			{
				FillChildNodes();
				return _childNodes.IndexOf((ITreeTableNode)(object)(InitValueNode)(object)node);
			}
			return -1;
		}

		public object GetValue(int nColumnIndex)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Invalid comparison between Unknown and I4
			switch (nColumnIndex)
			{
				case 0:
					{
						int num = _stExpression.LastIndexOf('.');
						if (num >= 0)
						{
							return _stExpression.Substring(num + 1);
						}
						return _stExpression;
					}
				case 1:
					{
						string text = ((_initValue != null && !HasChildren) ? ((IExprement)_initValue).ToString() : string.Empty);
						if ((int)_type.Class == 16 && (!text.StartsWith("'") || !text.EndsWith("'")))
						{
							text = "'" + text + "'";
						}
						string text2 = ((_defaultInitValue != null && !HasChildren) ? ((IExprement)_defaultInitValue).ToString() : string.Empty);
						bool isDefaultValue = text.Trim() == text2.Trim();
						if (IsEnumeration())
						{
							List<string> list = new List<string>();
							for (int i = 0; i < _iiisEnums.Length; i++)
							{
								string item = ((_iiisEnums[i].Variable == null || _iiisEnums[i].Variable.Type == null) ? _iiisEnums[i].Name : (((object)_iiisEnums[i].Variable.Type).ToString() + "." + _iiisEnums[i].Name));
								list.Add(item);
							}
							return new EditableComboBoxValue(list, text, isDefaultValue);
						}
						return new EditableComboBoxValue(text, isDefaultValue);
					}
				case 2:
					if (_type == null)
					{
						return string.Empty;
					}
					return ((object)_type).ToString();
				case 3:
					if (_commentElement == null)
					{
						return _comment ?? string.Empty;
					}
					if (string.IsNullOrEmpty(_commentElement.Comment))
					{
						return _comment ?? string.Empty;
					}
					return _commentElement.Comment;
				default:
					throw new ArgumentOutOfRangeException("nColumnIndex");
			}
		}

		public bool IsEditable(int nColumnIndex)
		{
			if (nColumnIndex == 1)
			{
				if (HasChildren)
				{
					return false;
				}
				if (NotFbOrFbInOut)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public bool IsEnumeration()
		{
			return _iiisEnums != null;
		}

		public void SetValue(int nColumnIndex, object value)
		{
			if (nColumnIndex == 1)
			{
				SetValue(value.ToString(), reportError: true);
				return;
			}
			throw new InvalidOperationException("This cell is read-only.");
		}

		public void SwapChildren(int nIndex1, int nIndex2)
		{
		}

		private void UpdateCache(int index, string initValue, bool isDefault)
		{
			if (!_cacheInitValue.ContainsKey(index))
			{
				_cacheInitValue.Add(index, initValue);
				_cacheInitIsDefault.Add(index, isDefault);
			}
			else
			{
				_cacheInitValue[index] = initValue;
				_cacheInitIsDefault[index] = isDefault;
			}
		}

		internal void CacheArrayInitValue()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			IPrecompileScope scope = _model.Scope;
			IType realType = Common.GetRealType((IPrecompileScope2)(object)((scope is IPrecompileScope2) ? scope : null), _type);
			if ((int)realType.Class != 26)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (ITreeTableNode childNode in _childNodes)
			{
				InitValueNode initValueNode = childNode as InitValueNode;
				if (initValueNode != null)
				{
					bool isDefaultInitValue;
					string initValue = initValueNode.GetInitValue(returnStringEvenIfDefaultValue: true, bArrayInit: true, out isDefaultInitValue);
					UpdateCache(((InitValueNode)(object)childNode).Index, initValue, isDefaultInitValue);
				}
			}
		}

		public string GetInitValueBy(int i, IIdentifierInfo[] identifierInfos, out bool isDefault)
		{
			InitValueNode initValueAtChild = GetInitValueAtChild(i);
			if (initValueAtChild != null)
			{
				return initValueAtChild.GetInitValue(returnStringEvenIfDefaultValue: true, bArrayInit: true, out isDefault);
			}
			if (_cacheInitValue.ContainsKey(i))
			{
				isDefault = _cacheInitIsDefault[i];
				return _cacheInitValue[i];
			}
			return GetInitValueOfHiddenElementBy(i, identifierInfos, out isDefault);
		}

		private string GetInitValueOfHiddenElementBy(int i, IIdentifierInfo[] identifierInfos, out bool isDefault)
		{
			IExpression val = null;
			IExpression val2 = null;
			isDefault = true;
			if (_initValue != null)
			{
				val2 = Common.ExtractElementInitValue(_initValue, i, _model.Scope);
			}
			if (identifierInfos[i].Variable != null)
			{
				if (identifierInfos[i].Variable.Initial != null)
				{
					val = identifierInfos[i].Variable.Initial;
				}
			}
			else
			{
				IIdentifierInfo[] iiisEnums = null;
				if (Common.IsEnumeration(_model.CompileContext, identifierInfos[i].Type))
				{
					iiisEnums = Common.GetEnumerationItems(_model.CompileContext, _model.SignatureGuid, identifierInfos[i].Type);
				}
				IPrecompileScope scope = _model.Scope;
				val = Common.CalculateDefault((IPrecompileScope2)(object)((scope is IPrecompileScope2) ? scope : null), identifierInfos[i].Type, iiisEnums);
			}
			if (val2 == null)
			{
				if (identifierInfos[i].Type is IUserdefType && !Common.IsEnumeration(_model.CompileContext, identifierInfos[i].Type))
				{
					return string.Empty;
				}
				if (identifierInfos[i].Type is IArrayType && !Common.IsEnumeration(_model.CompileContext, identifierInfos[i].Type))
				{
					return string.Empty;
				}
				if (val != null)
				{
					return ((IExprement)val).ToString();
				}
				return string.Empty;
			}
			string text = ((val == null) ? string.Empty : ((IExprement)val).ToString());
			string text2 = ((IExprement)val2).ToString();
			if (object.Equals(val2, "0"))
			{
				text2 = string.Empty;
			}
			if (string.Equals(text, "0"))
			{
				text = string.Empty;
			}
			if (text2.StartsWith("STRUCT"))
			{
				text2 = text2.Substring(6, text2.Length - 6);
			}
			isDefault = string.Equals(text2, text);
			return text2;
		}

		private InitValueNode GetInitValueAtChild(int i)
		{
			foreach (ITreeTableNode childNode in _childNodes)
			{
				InitValueNode initValueNode = childNode as InitValueNode;
				if (initValueNode.Index.Equals(i))
				{
					return initValueNode;
				}
			}
			return null;
		}

		internal void ResetInitValueOfElmentBy(bool isArray, bool isUserdef, ref string result)
		{
			if (isUserdef)
			{
				if (result.StartsWith("STRUCT") && result.Length > 6)
				{
					result = result.Substring(6);
				}
				if (string.IsNullOrEmpty(result))
				{
					result = "()";
				}
			}
			if (isArray && string.IsNullOrEmpty(result))
			{
				result = "[]";
			}
		}

		internal string GetInitValueOfCollapsedItem(out bool isDefaultInitValue)
		{
			string text = ((_initValue == null) ? string.Empty : ((IExprement)_initValue).ToString());
			string text2 = ((_defaultInitValue == null) ? string.Empty : ((IExprement)_defaultInitValue).ToString());
			if (string.Equals(text, "0"))
			{
				text = string.Empty;
			}
			if (string.Equals(text2, "0"))
			{
				text2 = string.Empty;
			}
			isDefaultInitValue = string.Equals(text, text2);
			return text;
		}

		internal string GetInitValue(bool returnStringEvenIfDefaultValue, bool bArrayInit, out bool isDefaultInitValue)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (HasChildren)
			{
				IPrecompileScope scope = _model.Scope;
				IType realType = Common.GetRealType((IPrecompileScope2)(object)((scope is IPrecompileScope2) ? scope : null), _type);
				TypeClass @class = realType.Class;
				if ((int)@class != 26)
				{
					if ((int)@class == 28)
					{
						List<string> list = new List<string>();
						isDefaultInitValue = true;
						if (_childNodes != null)
						{
							foreach (ITreeTableNode childNode in _childNodes)
							{
								InitValueNode initValueNode = childNode as InitValueNode;
								if (initValueNode != null)
								{
									bool isDefaultInitValue2;
									string initValue = initValueNode.GetInitValue(returnStringEvenIfDefaultValue: false, bArrayInit: false, out isDefaultInitValue2);
									if (!isDefaultInitValue2)
									{
										list.Add(initValue);
									}
									isDefaultInitValue &= isDefaultInitValue2;
								}
							}
							if (list.Count == 0)
							{
								if (bArrayInit)
								{
									stringBuilder.Append("()");
								}
							}
							else
							{
								stringBuilder.Append("(");
								stringBuilder.Append(string.Join(", ", list.ToArray()));
								stringBuilder.Append(")");
							}
						}
						else
						{
							string result = GetInitValueOfCollapsedItem(out isDefaultInitValue);
							ResetInitValueOfElmentBy(isArray: false, isUserdef: true, ref result);
							stringBuilder.Append(result);
						}
					}
					else
					{
						isDefaultInitValue = true;
					}
				}
				else
				{
					IIdentifierInfo[] subElements = _model.GetSubElements(realType);
					int num = subElements.Length;
					List<string> list2 = new List<string>();
					isDefaultInitValue = true;
					bool isUserdef = ((IArrayType)realType).Base is IUserdefType;
					bool isArray = ((IArrayType)realType).Base is IArrayType;
					if (_childNodes != null)
					{
						for (int i = 0; i < num; i++)
						{
							bool isDefault;
							string result2 = GetInitValueBy(i, subElements, out isDefault);
							ResetInitValueOfElmentBy(isArray, isUserdef, ref result2);
							list2.Add(result2);
							isDefaultInitValue &= isDefault;
						}
						if (!isDefaultInitValue)
						{
							int num2 = 0;
							int num3 = 0;
							while (num3 < list2.Count)
							{
								int j;
								for (j = num3; j < list2.Count && list2[num3] == list2[j]; j++)
								{
								}
								int num4 = j - num3;
								if (num4 > 1)
								{
									list2[num2] = $"{num4}({list2[num3]})";
								}
								else
								{
									list2[num2] = list2[num3];
								}
								num2++;
								num3 = j;
							}
							list2.RemoveRange(num2, list2.Count - num2);
							stringBuilder.Append("[");
							for (int k = 0; k < list2.Count; k++)
							{
								if (k > 0)
								{
									stringBuilder.Append(", ");
								}
								stringBuilder.Append(list2[k]);
							}
							stringBuilder.Append("]");
						}
					}
					else
					{
						string result3 = GetInitValueOfCollapsedItem(out isDefaultInitValue);
						ResetInitValueOfElmentBy(isArray: true, isUserdef: false, ref result3);
						stringBuilder.Append(result3);
					}
				}
			}
			else
			{
				string text = ((_initValue != null) ? ((IExprement)_initValue).ToString() : ((_defaultInitValue == null) ? string.Empty : ((IExprement)_defaultInitValue).ToString()));
				string text2 = ((_defaultInitValue != null) ? ((IExprement)_defaultInitValue).ToString() : string.Empty);
				if ((int)_type.Class == 16 && (!text.StartsWith("'") || !text.EndsWith("'")))
				{
					text = "'" + text + "'";
					text2 = "'" + text2 + "'";
				}
				isDefaultInitValue = text.Trim() == text2.Trim();
				if (!isDefaultInitValue || returnStringEvenIfDefaultValue)
				{
					stringBuilder.Append(text);
				}
			}
			if (stringBuilder.Length > 0 && _bStructMember)
			{
				int num5 = _stExpression.LastIndexOf('.');
				if (num5 >= 0)
				{
					stringBuilder.Insert(0, $"{_stExpression.Substring(num5 + 1)} := ");
				}
				else
				{
					stringBuilder.Insert(0, $"{_stExpression} := ");
				}
			}
			return stringBuilder.ToString();
		}

		internal bool IsEditable()
		{
			return IsEditable(1);
		}

		internal bool SetValue(string stValue, bool reportError)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Expected O, but got Unknown
			if (string.IsNullOrEmpty(stValue))
			{
				_initValue = null;
				((AbstractTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)_parentNode, _nIndex, (ITreeTableNode)(object)this));
				return true;
			}
			try
			{
				IScanner val = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateScanner(stValue, false, false, false, false);
				IParser obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).CreateParser(val);
				IParser5 val2 = (IParser5)(object)((obj is IParser5) ? obj : null);
				_initValue = val2.ParseInitialisation();
				if (_initValue == null)
				{
					if (reportError)
					{
						APEnvironment.MessageService.Error(string.Format(Resources.InvalidExpression, stValue), "InvalidExpression", Array.Empty<object>());
					}
					return false;
				}
			}
			catch
			{
				if (reportError)
				{
					APEnvironment.MessageService.Error(string.Format(Resources.InvalidExpression, stValue), "InvalidExpression", Array.Empty<object>());
				}
				_initValue = null;
				return false;
			}
			if (_initValue == null)
			{
				ResetToDefault();
			}
			((AbstractTreeTableModel)_model).RaiseChanged(new TreeTableModelEventArgs((ITreeTableNode)(object)_parentNode, _nIndex, (ITreeTableNode)(object)this));
			return true;
		}

		internal string GetValue()
		{
			return GetValue(1).ToString();
		}

		private IType ResolveAliasToBaseType(IPrecompileScope2 scope, ISignature aliasSign)
		{
			IType val = null;
			while (aliasSign != null && aliasSign.GetFlag((SignatureFlag)4))
			{
				IVariable[] all = aliasSign.All;
				if (all.Length != 1)
				{
					break;
				}
				val = all[0].Type;
				if (!(val is IUserdefType2))
				{
					break;
				}
				IUserdefType2 val2 = (IUserdefType2)(object)((val is IUserdefType2) ? val : null);
				if (val2 == null)
				{
					return null;
				}
				aliasSign = scope.FindSignatureGlobal(val2.NameExpression);
			}
			return val;
		}

		internal void ResetToDefault()
		{
			if (_defaultInitValue != null)
			{
				SetValue(((IExprement)_defaultInitValue).ToString(), reportError: false);
			}
			else
			{
				SetValue(string.Empty, reportError: false);
			}
		}

		public bool HasTheSameUserdefTypeToRoot()
		{
			if (_type is IUserdefType)
			{
				for (InitValueNode initValueNode = Parent as InitValueNode; initValueNode != null; initValueNode = initValueNode.Parent as InitValueNode)
				{
					if (initValueNode.Type == _type)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private int GetMyArrayChildrenEditLength(IIdentifierInfo[] subElements, out int start)
		{
			start = 0;
			int num = InitValueModel.MaxDisplayValues - 1;
			if (MonitoringRange != null)
			{
				start = MonitoringRange.Item1;
				num = MonitoringRange.Item2;
				int item = MonitoringRange.Item3;
				if (MonitoringRange.Item5)
				{
					start -= item;
					num -= item;
				}
			}
			return Math.Min(subElements.Length, num - start + 1);
		}

		public IExpression GetInitValueExpressionFrom(string cached, IType nodeType)
		{
			if (!string.IsNullOrEmpty(cached))
			{
				IType compiledType = null;
				IIdentifierInfo[] array = null;
				return _model.GetInitializationExpression(((object)nodeType).ToString(), cached, out compiledType, out array);
			}
			return null;
		}

		private CommentElement TryGetCommentElement(int i)
		{
			if (_commentElement != null)
			{
				try
				{
					return ((ArrayCommentElement)_commentElement).GetChildBy(i);
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		private CommentElement TryGetCommentElement(string property)
		{
			if (_commentElement != null)
			{
				try
				{
					return ((StructureCommentElement)_commentElement).GetChildBy(property);
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		private bool CheckChildExists()
		{
			if (IsEnumeration())
			{
				return false;
			}
			if (HasTheSameUserdefTypeToRoot())
			{
				return false;
			}
			IPrecompileScope scope = _model.Scope;
			IType realType = Common.GetRealType((IPrecompileScope2)(object)((scope is IPrecompileScope2) ? scope : null), _type);
			IIdentifierInfo[] subElements = _model.GetSubElements(realType);
			if (subElements == null)
			{
				return false;
			}
			return true;
		}

		private void FillChildNodes()
		{
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Invalid comparison between Unknown and I4
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Invalid comparison between Unknown and I4
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Invalid comparison between Unknown and I4
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Invalid comparison between Unknown and I4
			if (_childNodes != null || IsEnumeration() || HasTheSameUserdefTypeToRoot())
			{
				return;
			}
			_childNodes = new List<ITreeTableNode>();
			IPrecompileScope scope = _model.Scope;
			IType realType = Common.GetRealType((IPrecompileScope2)(object)((scope is IPrecompileScope2) ? scope : null), _type);
			IIdentifierInfo[] subElements = _model.GetSubElements(realType);
			Editable<IIdentifierInfo>[] subElements2 = _model.GetSubElements2(realType);
			if (subElements == null)
			{
				return;
			}
			int start = 0;
			int myArrayChildrenEditLength = GetMyArrayChildrenEditLength(subElements, out start);
			bool flag = true;
			bool flag2 = true;
			IType val = null;
			string text = null;
			if (_commentElement == null)
			{
				string attribute = _model.GetAttribute(_stExpression);
				if (!string.IsNullOrEmpty(attribute))
				{
					_commentElement = CommentElement.BuildRootNode(_type, _comment, attribute);
				}
			}
			if (_commentElement != null)
			{
				if (realType is IUserdefType)
				{
					((StructureCommentElement)_commentElement).SetPropertyType(subElements);
				}
				Common.BuildChildElementAndPromptOption(_commentElement, _type, isBuildEmpty: false);
			}
			for (int i = start; i < myArrayChildrenEditLength + start; i++)
			{
				string stExpression = _stExpression;
				bool bStructMember;
				if (subElements[i].Name.StartsWith("["))
				{
					stExpression += subElements[i].Name;
					bStructMember = false;
				}
				else
				{
					stExpression = stExpression + "." + subElements[i].Name;
					bStructMember = true;
				}
				IExpression initValue = null;
				CommentElement commentElement = null;
				if (realType != null)
				{
					if ((int)realType.Class == 26)
					{
						if (_cacheInitValue.ContainsKey(i))
						{
							string cached = _cacheInitValue[i];
							initValue = GetInitValueExpressionFrom(cached, realType);
						}
						else
						{
							initValue = Common.ExtractElementInitValue(_initValue, i, _model.Scope);
						}
						commentElement = TryGetCommentElement(i);
					}
					else if ((int)realType.Class == 28)
					{
						initValue = Common.ExtractComponentInitValue(_initValue, subElements[i].Name);
						commentElement = TryGetCommentElement(subElements[i].Name);
					}
				}
				IIdentifierInfo[] iiisEnums = null;
				if (Common.IsEnumeration(_model.CompileContext, subElements[i].Type))
				{
					iiisEnums = Common.GetEnumerationItems(_model.CompileContext, _model.SignatureGuid, subElements[i].Type);
				}
				IExpression val2 = null;
				if ((int)realType.Class == 26 && _defaultInitValue != null)
				{
					val2 = Common.ExtractElementInitValue(_defaultInitValue, i, _model.Scope);
				}
				if ((int)realType.Class == 28)
				{
					val2 = Common.ExtractComponentInitValue(_defaultInitValue, subElements[i].Name);
				}
				if (subElements[i].Variable != null && val2 == null)
				{
					val2 = subElements[i].Variable.Initial;
				}
				InitValueNode initValueNode = new InitValueNode(_model, this, i, stExpression, initValue, val2, subElements[i].Type, bStructMember, iiisEnums, subElements[i].Comment);
				if (NotFbOrFbInOut)
				{
					initValueNode.NotFbOrFbInOut = subElements2[i].CanEdit;
				}
				else
				{
					initValueNode.NotFbOrFbInOut = false;
				}
				if (commentElement != null)
				{
					initValueNode.CommentElement = commentElement;
				}
				_childNodes.Add((ITreeTableNode)(object)initValueNode);
				if (val == null)
				{
					val = initValueNode._type;
				}
				if (flag && val != initValueNode._type)
				{
					flag = false;
				}
				if (text == null)
				{
					text = ((IExprement)initValueNode._defaultInitValue).ToString();
				}
				if (initValueNode.HasChildren)
				{
					flag2 = false;
				}
				if (flag2 && text.ToString() != ((IExprement)initValueNode._defaultInitValue).ToString())
				{
					flag2 = false;
				}
			}
			_isChildReady = true;
		}
	}
}
