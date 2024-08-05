using System.Collections;

namespace _3S.CoDeSys.DeviceEditor
{
	internal class CombinedParameterFilter
	{
		private ArrayList _alFilters = new ArrayList();

		public void AddFilter(HideParameterDelegate filter)
		{
			if (filter != null)
			{
				_alFilters.Add(filter);
			}
		}

		public HideParameterDelegate GetParameterFilter()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			if (_alFilters.Count > 0)
			{
				return new HideParameterDelegate(HideParameter);
			}
			return null;
		}

		public bool HideParameter(int nParameterId, string[] componentPath)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			foreach (HideParameterDelegate alFilter in _alFilters)
			{
				if (alFilter.Invoke(nParameterId, componentPath))
				{
					return true;
				}
			}
			return false;
		}
	}
}
