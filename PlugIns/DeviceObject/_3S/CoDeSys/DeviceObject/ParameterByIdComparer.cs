using System.Collections;

namespace _3S.CoDeSys.DeviceObject
{
	public class ParameterByIdComparer : IComparer
	{
		private static ParameterByIdComparer _instance;

		internal static ParameterByIdComparer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ParameterByIdComparer();
				}
				return _instance;
			}
		}

		private ParameterByIdComparer()
		{
		}

		public int Compare(object x, object y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			IParameter val = (IParameter)x;
			IParameter val2 = (IParameter)y;
			return val.Id.CompareTo(val2.Id);
		}
	}
}
