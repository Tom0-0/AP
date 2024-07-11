namespace _3S.CoDeSys.WatchList
{
	public class ApplicationPrefixItem
	{
		private string _stApplicationPrefix = "<unknown application>";

		public string ApplicationPrefix
		{
			get
			{
				return _stApplicationPrefix;
			}
			set
			{
				_stApplicationPrefix = value;
			}
		}

		public ApplicationPrefixItem(string stApplicationPrefix)
		{
			_stApplicationPrefix = stApplicationPrefix;
		}

		public override string ToString()
		{
			if (_stApplicationPrefix != null)
			{
				return _stApplicationPrefix;
			}
			return string.Empty;
		}
	}
}
