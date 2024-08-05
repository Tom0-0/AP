namespace _3S.CoDeSys.DeviceEditor
{
	public class FactoryListElement<TFactory>
	{
		private int _nMatch;

		private TFactory _factory;

		public int Match
		{
			get
			{
				return _nMatch;
			}
			set
			{
				_nMatch = value;
			}
		}

		public TFactory Factory
		{
			get
			{
				return _factory;
			}
			set
			{
				_factory = value;
			}
		}

		public FactoryListElement(TFactory factory, int nMatch)
		{
			_factory = factory;
			_nMatch = nMatch;
		}
	}
}
