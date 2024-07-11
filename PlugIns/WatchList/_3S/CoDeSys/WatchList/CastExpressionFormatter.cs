using System.Text;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	internal sealed class CastExpressionFormatter : IExprVisitor4, IExprVisitor3, IExprVisitor2, IExprVisitor
	{
		private StringBuilder _sb;

		private int _iStartOfInterpretedAccess;

		private static CastExpressionFormatter s_Instance;

		internal static CastExpressionFormatter Instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new CastExpressionFormatter();
				}
				return s_Instance;
			}
		}

		private CastExpressionFormatter()
		{
		}

		public void visit(IForStatement forloop)
		{
		}

		public void visit(IContinueStatement cont)
		{
		}

		public void visit(IAssignmentExpression assign)
		{
		}

		public void visit(IReturnStatement returnst)
		{
		}

		public void visit(ILabelStatement label)
		{
		}

		public void visit(IPragmaStatement pragma)
		{
		}

		public void visit(ICallExpression call)
		{
		}

		public void visit(IConversionExpression conv)
		{
		}

		public void visit(IBaseExpression baseexp)
		{
		}

		public void visit(IAddressExpression address)
		{
		}

		public void visit(IEmptyStatement empty)
		{
		}

		public void visit(ICaseLabelStatement caselabel)
		{
		}

		public void visit(IBreakPointStatement bpstate)
		{
		}

		public void visit(IVariableReference varref)
		{
		}

		public void visit(IPouReference pouref)
		{
		}

		public void visit(IPragmaOperatorExpression popexp)
		{
		}

		public void visit(IDefineStatement defstate)
		{
		}

		public void visit(IHasAttributeExpression hasattribute)
		{
		}

		public void visit(IPragmaAssertion assertion)
		{
		}

		public void visit(IHasValueExpression hasvalue)
		{
		}

		public void visit(IHasTypeExpression hastype)
		{
		}

		public void visit(IPragmaIfStatement pifst)
		{
		}

		public void visit(IDefinedExpression defexp)
		{
		}

		public void visit(ITypeReference typeref)
		{
		}

		public void visit(IDefineReference defref)
		{
		}

		public void visit(ICaseStatement casest)
		{
		}

		public void visit(ICaseRangeExpression caserange)
		{
		}

		public void visit(IGlobalScopeExpression globexp)
		{
		}

		public void visit(IThisExpression thisexp)
		{
		}

		public void visit(IOperatorExpression op)
		{
		}

		public void visit(IExpressionStatement expstat)
		{
		}

		public void visit(ICommentStatement comment)
		{
		}

		public void visit(IJumpStatement gotost)
		{
		}

		public void visit(IIfStatement ifst)
		{
		}

		public void visit(ISequenceStatement seq)
		{
		}

		public void visit(IExitStatement exit)
		{
		}

		public void visit(IRepeatStatement repeat)
		{
		}

		public void visit(IWhileStatement whilst)
		{
		}

		public void visit(IStructureInitialization structInit)
		{
		}

		public void visit(IArrayInitialization arrayInit)
		{
		}

		public void visit(IHasConstantValueExpression hasvalue)
		{
		}

		public void visit(ICompilerVersionExpression compverExpr)
		{
		}

		public void visit(IRuntimeVersionExpression runverExpr)
		{
		}

		public void visit(IIndexAccessExpression indexaccess)
		{
			((IExprement)indexaccess.Var).AcceptVisitor((IExprVisitor)(object)this);
			_sb.Append("[");
			IExpression[] accesses = indexaccess.Accesses;
			for (int i = 0; i < accesses.Length; i++)
			{
				if (0 < i)
				{
					_sb.Append(",");
				}
				((IExprement)accesses[i]).AcceptVisitor((IExprVisitor)(object)this);
			}
			_sb.Append("]");
		}

		public void visit(IDeRefAccessExpression deref)
		{
			((IExprement)deref.Base).AcceptVisitor((IExprVisitor)(object)this);
			_sb.Append("^");
		}

		public void visit(ICompoAccessExpression compo)
		{
			((IExprement)compo.Left).AcceptVisitor((IExprVisitor)(object)this);
			_sb.Append(".");
			((IExprement)compo.Right).AcceptVisitor((IExprVisitor)(object)this);
		}

		public void visit(IVariableExpression variable)
		{
			string name = variable.Name;
			int num = name.IndexOf("__CAST_");
			int num2 = name.IndexOf("_POINTER_TO_");
			if (0 <= num && 0 <= num2)
			{
				int num3 = num + "__CAST_".Length;
				string text = name.Substring(num3, num2 - num3);
				_sb.Append(text.Replace("_", "#"));
				_iStartOfInterpretedAccess = _sb.Length;
			}
			else
			{
				_sb.Append(variable.Name);
			}
		}

		public void visit(ILiteralExpression literal)
		{
			_sb.Append(((IExprement)literal).ToString());
		}

		public void visit(ICastExpression cast)
		{
			((IExprement)cast.Base).AcceptVisitor((IExprVisitor)(object)this);
			_iStartOfInterpretedAccess = _sb.Length;
		}

		internal bool IsCastExpression(string stExpression)
		{
			return stExpression.ToLowerInvariant().Contains("__cast");
		}

		internal string GetCastExpressionDisplayString(string stExpression)
		{
			int iStartOfInterpretedAccess = 0;
			return GetCastExpressionDisplayString(stExpression, out iStartOfInterpretedAccess);
		}

		internal string GetCastExpressionDisplayString(string stExpression, out int iStartOfInterpretedAccess)
		{
			iStartOfInterpretedAccess = -1;
			if (!IsCastExpression(stExpression))
			{
				return stExpression;
			}
			IScanner val = APEnvironment.LMServiceProvider.CreatorService.CreateScanner(stExpression, false, false, false, false);
			val.AllowMultipleUnderlines=(true);
			IParser obj = APEnvironment.LMServiceProvider.CreatorService.CreateParser(val);
			IExpression val2 = ((IParser2)((obj is IParser2) ? obj : null)).ParseExpression();
			ICastExpression val3 = (ICastExpression)(object)((val2 is ICastExpression) ? val2 : null);
			if (val3 != null)
			{
				string text = ((IExprement)val3.Base).ToString();
				iStartOfInterpretedAccess = text.Length;
				return $"{text}:{((object)val3.ExplicitelySpecifiedType).ToString()}";
			}
			IDeRefAccessExpression val4 = (IDeRefAccessExpression)(object)((val2 is IDeRefAccessExpression) ? val2 : null);
			if (val4 != null)
			{
				IExpression @base = val4.Base;
				val3 = (ICastExpression)(object)((@base is ICastExpression) ? @base : null);
				if (val3 != null)
				{
					string text2 = ((IExprement)val3.Base).ToString();
					iStartOfInterpretedAccess = text2.Length;
					return text2 + "^";
				}
			}
			ICompoAccessExpression val5 = (ICompoAccessExpression)(object)((val2 is ICompoAccessExpression) ? val2 : null);
			if (val5 != null)
			{
				iStartOfInterpretedAccess = 0;
				return ((IExprement)val5.Right).ToString();
			}
			if (val2 == null)
			{
				return stExpression;
			}
			_sb = new StringBuilder();
			((IExprement)val2).AcceptVisitor((IExprVisitor)(object)this);
			iStartOfInterpretedAccess = _iStartOfInterpretedAccess;
			return _sb.ToString();
		}
	}
}
