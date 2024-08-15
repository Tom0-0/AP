using System.Text;
using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.LanguageModel;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Utilities;

namespace _3S.CoDeSys.DeviceObject
{
	[TypeGuid("{47edf8ea-3f84-452c-b998-e18f878578d3}")]
	[StorageVersion("3.3.0.0")]
	public class VariableMapping : GenericObject2, IVariableMapping3, IVariableMapping2, IVariableMapping
	{
		private class VariableNameVisitor : IExprVisitor
		{
			private StringBuilder _sbErrorList;

			private VariableMapping _mapping;

			private bool _bHasError;

			private string _stMessageGuidPragma;

			public bool HasError => _bHasError;

			public VariableNameVisitor(StringBuilder sbErrorList, string stMessageGuidPragma, VariableMapping mapping)
			{
				_sbErrorList = sbErrorList;
				_mapping = mapping;
				_stMessageGuidPragma = stMessageGuidPragma;
			}

			public void visit(IWhileStatement whilst)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IRepeatStatement repeat)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IForStatement forloop)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IExitStatement exit)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IContinueStatement cont)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ISequenceStatement seq)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IAssignmentExpression assign)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IIfStatement ifst)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IReturnStatement returnst)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IJumpStatement gotost)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ILabelStatement label)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ICommentStatement comment)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IPragmaStatement pragma)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IExpressionStatement expstat)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ICallExpression call)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IOperatorExpression op)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IConversionExpression conv)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IThisExpression thisexp)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IBaseExpression baseexp)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ILiteralExpression literal)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IAddressExpression address)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IVariableExpression variable)
			{
			}

			public void visit(IIndexAccessExpression indexaccess)
			{
			}

			public void visit(ICompoAccessExpression compo)
			{
				((IExprement)compo.Left).AcceptVisitor((IExprVisitor)(object)this);
				((IExprement)compo.Right).AcceptVisitor((IExprVisitor)(object)this);
			}

			public void visit(IDeRefAccessExpression deref)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingNoDeRef);
				_bHasError = true;
			}

			public void visit(IGlobalScopeExpression globexp)
			{
				((IExprement)globexp.Base).AcceptVisitor((IExprVisitor)(object)this);
			}

			public void visit(IEmptyStatement empty)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ICaseRangeExpression caserange)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ICaseLabelStatement caselabel)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ICaseStatement casest)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IBreakPointStatement bpstate)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IDefineReference defref)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IVariableReference varref)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(ITypeReference typeref)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IPouReference pouref)
			{
				_bHasError = true;
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
			}

			public void visit(IDefinedExpression defexp)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IPragmaOperatorExpression popexp)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IPragmaIfStatement pifst)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IDefineStatement defstate)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IHasTypeExpression hastype)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IHasAttributeExpression hasattribute)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IHasValueExpression hasvalue)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}

			public void visit(IPragmaAssertion assertion)
			{
				_mapping.AddError(_sbErrorList, _stMessageGuidPragma, Strings.ErrorMapExistingExpression);
				_bHasError = true;
			}
		}

		[DefaultDuplication(0)]
		[DefaultSerialization("Id")]
		[StorageVersion("3.3.0.0")]
		private long _lId = -1L;

		[DefaultDuplication(0)]
		protected string _stVariable = "";

		[DefaultDuplication(0)]
		[DefaultSerialization("CreateVariable")]
		[StorageVersion("3.3.0.0")]
		private bool _bCreateVariable;

		[DefaultDuplication(0)]
		[DefaultSerialization("DefaultVariable")]
		[StorageVersion("3.3.0.0")]
		private string _stDefaultVariable = "";

		[DefaultDuplication(0)]
		[DefaultSerialization("IoChannelFBInstance")]
		[StorageVersion("3.5.13.0")]
		[StorageDefaultValue("")]
		private string _stIoChannelFBInstance = string.Empty;

		private bool _bIsUnusedMapping;

		private IDataElementParent _parent;

		private IIoProvider _ioProvider;

		[DefaultSerialization("Variable")]
		[StorageVersion("3.3.0.0")]
		protected string VariableSerialization
		{
			get
			{
				return _stVariable;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_stVariable = value.Trim('\n', '\r');
				}
			}
		}

		public bool IsUnusedMapping => _bIsUnusedMapping;

		public IIoProvider IoProvider
		{
			get
			{
				return _ioProvider;
			}
			set
			{
				_ioProvider = value;
			}
		}

		internal IDataElementParent Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		public long Id
		{
			get
			{
				return _lId;
			}
			set
			{
				_lId = value;
			}
		}

		public string Variable
		{
			get
			{
				if (_stVariable.Contains("$(DeviceName)") && _ioProvider != null)
				{
					string text = _stVariable;
					IMetaObject metaObject = _ioProvider.GetMetaObject();
					if (_stVariable.StartsWith("$") && !_bCreateVariable)
					{
						DeviceObject deviceObject = _ioProvider.GetHost() as DeviceObject;
						if (deviceObject != null)
						{
							IMetaObject application = deviceObject.GetApplication();
							if (application != null)
							{
								text = application.Name + "." + text;
							}
						}
					}
					return text.Replace("$(DeviceName)", metaObject.Name);
				}
				return _stVariable;
			}
			set
			{
				string text = "";
				if (value != null)
				{
					text = value;
				}
				if (text != _stVariable)
				{
					if (!text.Contains(".") && !text.StartsWith("@"))
					{
						_stVariable = DeviceObjectHelper.BuildIecIdentifier(text.Trim('\n', '\r'));
					}
					else
					{
						_stVariable = text;
					}
					Notify();
				}
			}
		}

		public bool CreateVariable
		{
			get
			{
				return _bCreateVariable;
			}
			set
			{
				if (_bCreateVariable != value)
				{
					_bCreateVariable = value;
					Notify();
				}
			}
		}

		public string DefaultVariable
		{
			get
			{
				return _stDefaultVariable;
			}
			set
			{
				_stDefaultVariable = value;
			}
		}

		public string IoChannelFBInstance
		{
			get
			{
				return _stIoChannelFBInstance;
			}
			set
			{
				_stIoChannelFBInstance = value;
			}
		}

		public VariableMapping()
			: base()
		{
		}

		internal VariableMapping(long lId, string stVariable, bool bCreateVariable)
			: this()
		{
			_lId = lId;
			_stVariable = stVariable;
			_bCreateVariable = bCreateVariable;
		}

		internal VariableMapping(long lId, string stVariable, bool bCreateVariable, bool bIsUnusedMapping)
			: this()
		{
			_lId = lId;
			_stVariable = stVariable;
			_bCreateVariable = bCreateVariable;
			_bIsUnusedMapping = bIsUnusedMapping;
		}

		private VariableMapping(VariableMapping original)
			: this()
		{
			_lId = original._lId;
			_stVariable = original._stVariable;
			_bCreateVariable = original._bCreateVariable;
			_bIsUnusedMapping = original._bIsUnusedMapping;
			_stDefaultVariable = original._stDefaultVariable;
			_ioProvider = original._ioProvider;
			_stIoChannelFBInstance = original._stIoChannelFBInstance;
		}

		public override object Clone()
		{
			VariableMapping variableMapping = new VariableMapping(this);
			((GenericObject)variableMapping).AfterClone();
			return variableMapping;
		}

		internal string GetApplication()
		{
			if (_bCreateVariable)
			{
				return "";
			}
			string variable = Variable;
			int num = variable.IndexOf('.');
			if (num == -1)
			{
				return variable;
			}
			return variable.Substring(0, num);
		}

		internal bool CheckValidIdentifier(StringBuilder sbErrorList, string stMessageGuidPragma)
		{
			if (_stVariable == null || _stVariable == "")
			{
				return true;
			}
			string variable = Variable;
			if (_bCreateVariable)
			{
				IToken val = default(IToken);
				if (((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(variable, true, false, false, false).Match((TokenType)13, true, out val) <= 0)
				{
					return false;
				}
				return true;
			}
			IScanner val2 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateScanner(variable, false, false, false, false);
			IExpression val3 = ((ILanguageModelManager)APEnvironment.LanguageModelMgr).CreateParser(val2).ParseOperand();
			VariableNameVisitor variableNameVisitor = new VariableNameVisitor(sbErrorList, stMessageGuidPragma, this);
			if (val3 != null)
			{
				((IExprement)val3).AcceptVisitor((IExprVisitor)(object)variableNameVisitor);
			}
			else
			{
				AddError(sbErrorList, stMessageGuidPragma, Strings.ErrorMapExistingExpression);
			}
			return !variableNameVisitor.HasError;
		}

		internal void AddError(StringBuilder sbErrorList, string stMessageGuidPragma, string stError)
		{
			if (sbErrorList != null)
			{
				if (!string.IsNullOrEmpty(stMessageGuidPragma))
				{
					sbErrorList.Append(stMessageGuidPragma);
					sbErrorList.Append("{p " + PositionHelper.CombinePosition(_lId, (short)0) + "}");
					sbErrorList.Append("{error '");
					sbErrorList.Append(_stVariable.Replace("$", "$$").Replace("'", "$'"));
					sbErrorList.Append(": ");
					sbErrorList.Append(stError);
					sbErrorList.AppendLine("'}");
				}
				else
				{
					sbErrorList.Append(_stVariable.Replace("$", "$$").Replace("'", "$'"));
					sbErrorList.Append(": ");
					sbErrorList.Append(stError);
				}
			}
		}

		internal string GetPlainVariableName()
		{
			if (_bCreateVariable)
			{
				return _stVariable;
			}
			string variable = Variable;
			int num = variable.IndexOf('.');
			if (num == -1 || num + 1 == variable.Length)
			{
				return "";
			}
			return variable.Substring(num + 1);
		}

		private void Notify()
		{
			if (_parent != null)
			{
				_parent.Notify(_parent.DataElement, new string[0]);
			}
		}
	}
}
