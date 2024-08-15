using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	public class ParameterByDevDescComparer : IComparer
	{
		private static ParameterByDevDescComparer _instance;

		internal static ParameterByDevDescComparer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ParameterByDevDescComparer();
				}
				return _instance;
			}
		}

		private ParameterByDevDescComparer()
		{
		}

		public int Compare(object x, object y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected O, but got Unknown
			IParameter6 val = (IParameter6)x;
			IParameter6 val2 = (IParameter6)y;
			if (val.IndexInDevDesc == -1 || val2.IndexInDevDesc == -1)
			{
				return ((IParameter)val).Id.CompareTo(((IParameter)val2).Id);
			}
			return val.IndexInDevDesc.CompareTo(val2.IndexInDevDesc);
		}
	}
}
