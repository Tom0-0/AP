using _3S.CoDeSys.Core.LanguageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3S.CoDeSys.TabularDeclarationEditor.Comment
{
	public class MonitoringRangeContext
	{
		public MonitoringRangeContext(IPreCompileContext pcc, IArrayDimension[] dimensions, int nIncrement, Guid objectGuid, ICompiledType type, string stExpression)
		{
			this.preCompileContext = pcc;
			this._dimensions = dimensions;
			this.Increment = nIncrement;
			this.UpperBound = 1;
			this.Type = type;
			this.Expression = stExpression;
			this.ObjectGuid = objectGuid;
			this.SetBounds();
		}

		public void SetUserdefRange(int start, int end)
		{
			this.Start = start;
			this.End = end;
		}

		public void CheckAndSetDefaultRange()
		{
			if (this.End - this.Start >= MonitoringRangeContext.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY)
			{
				this.End = this.Start + MonitoringRangeContext.DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY - 1;
			}
		}

		public void CheckAndSetMaxRange()
		{
			if (this.End - this.Start >= MonitoringRangeContext.MAX_MONITORING_ELEMENTS_PER_ARRAY)
			{
				this.End = this.Start + MonitoringRangeContext.MAX_MONITORING_ELEMENTS_PER_ARRAY - 1;
			}
		}

		private void SetBounds()
		{
			IPrecompileScope4 scope = this.preCompileContext.CreatePrecompileScope(this.ObjectGuid) as IPrecompileScope4;
			foreach (IArrayDimension arrayDimension in this._dimensions)
			{
				ILiteralValue litval = (arrayDimension.LowerBorder as IExpression2).Literal(scope);
				ILiteralValue litval2 = (arrayDimension.UpperBorder as IExpression2).Literal(scope);
				int item = (int)this.GetAnyLong(litval);
				int item2 = (int)this.GetAnyLong(litval2);
				this._lowerBounds.Add(item);
				this._upperBounds.Add(item2);
			}
			int num = 1;
			for (int j = 0; j < this._upperBounds.Count; j++)
			{
				num *= this._upperBounds[j] - this._lowerBounds[j] + 1;
			}
			this.LowerBound = 0;
			this.UpperBound = num - 1;
			this.TotalRange = num;
		}

		private long GetAnyLong(ILiteralValue litval)
		{
			if (litval == null)
			{
				return 0L;
			}
			if (litval.KindOf == KindOfLiteral.UnsignedInteger)
			{
				return (long)litval.UnsignedLong;
			}
			return litval.SignedLong;
		}

		public bool SingleDimension
		{
			get
			{
				return this._dimensions.Length == 1;
			}
		}

		public int Increment { get; private set; }

		public int Start
		{
			get
			{
				if (this._nStart != -2147483648)
				{
					return this._nStart;
				}
				return this.LowerBound;
			}
			set
			{
				this._nStart = value;
			}
		}

		public int End
		{
			get
			{
				if (this._nEnd != -2147483648)
				{
					return this._nEnd;
				}
				return this.UpperBound;
			}
			set
			{
				this._nEnd = value;
			}
		}

		public Guid ApplicationGuid { get; private set; }

		public Guid ObjectGuid { get; private set; }

		public int LowerBound { get; private set; }

		public int UpperBound { get; private set; }

		public IList<int> UpperBounds
		{
			get
			{
				return this._upperBounds;
			}
		}

		public IList<int> LowerBounds
		{
			get
			{
				return this._lowerBounds;
			}
		}

		public int TotalRange { get; private set; }

		public ICompiledType Type { get; private set; }

		public string Expression { get; private set; }

		public IArrayDimension GetDimension(int index)
		{
			return this._dimensions[index];
		}
		public int NewStart { get; set; }

		public int NewEnd { get; set; }

		public static int MAX_MONITORING_ELEMENTS_PER_ARRAY = 1000;

		public static int DEFAULT_MAX_MONITORING_ELEMENTS_PER_ARRAY = 1000;

		private IPreCompileContext preCompileContext;

		private IArrayDimension[] _dimensions;

		private int _nStart = int.MinValue;

		private int _nEnd = int.MinValue;

		private IList<int> _lowerBounds = new List<int>();

		private IList<int> _upperBounds = new List<int>();
	}
}
