using System;
using System.Collections.Generic;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	public class MonitoringRangeContext
	{
		private IArrayDimension[] _dimensions;

		private int _nStart = int.MinValue;

		private int _nEnd = int.MinValue;

		private IList<int> _lowerBounds = new List<int>();

		private IList<int> _upperBounds = new List<int>();

		internal bool SingleDimension => _dimensions.Length == 1;

		internal int Increment { get; private set; }

		internal int Start
		{
			get
			{
				if (_nStart != int.MinValue)
				{
					return _nStart;
				}
				return LowerBound;
			}
			set
			{
				_nStart = value;
			}
		}

		internal int End
		{
			get
			{
				if (_nEnd != int.MinValue)
				{
					return _nEnd;
				}
				return UpperBound;
			}
			set
			{
				_nEnd = value;
			}
		}

		internal Guid ApplicationGuid { get; private set; }

		internal int LowerBound { get; private set; }

		internal int UpperBound { get; private set; }

		internal IList<int> UpperBounds => _upperBounds;

		internal IList<int> LowerBounds => _lowerBounds;

		internal int TotalRange { get; private set; }

		internal ICompiledType Type { get; private set; }

		internal string Expression { get; private set; }

		internal MonitoringRangeContext(IArrayDimension[] dimensions, int nIncrement, Guid application, ICompiledType type, string stExpression)
		{
			_dimensions = dimensions;
			Increment = nIncrement;
			ApplicationGuid = application;
			UpperBound = 1;
			Type = type;
			Expression = stExpression;
			SetBounds();
		}

		private void SetBounds()
		{
			if (!(ApplicationGuid != Guid.Empty))
			{
				return;
			}
			IScope obj = ((ILanguageModelManager21)APEnvironment.LanguageModelMgr).GetCompileContext(ApplicationGuid).CreateGlobalIScope();
			IScope5 val = (IScope5)(object)((obj is IScope5) ? obj : null);
			bool flag = false;
			IArrayDimension[] dimensions = _dimensions;
			foreach (IArrayDimension val2 in dimensions)
			{
				if (val != null)
				{
					int num = val2.LowerBorderInt(out flag, (IScope)(object)val);
					_lowerBounds.Add(num);
					LowerBound = num;
					if (flag)
					{
						int num2 = val2.UpperBorderInt(out flag, (IScope)(object)val);
						_upperBounds.Add(num2);
						UpperBound *= num2;
					}
				}
			}
			if (Type is IArrayType2)
			{
				ICompiledType type = Type;
				TotalRange = ((IArrayType2)((type is IArrayType2) ? type : null)).GetNumOfElements(val, out flag);
			}
		}

		internal IArrayDimension GetDimension(int index)
		{
			return _dimensions[index];
		}
	}
}
