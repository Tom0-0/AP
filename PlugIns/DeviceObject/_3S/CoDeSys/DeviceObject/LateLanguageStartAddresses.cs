using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.DeviceObject
{
	internal class LateLanguageStartAddresses
	{
		private IDirectVariable _startInAddress;

		private IDirectVariable _startOutAddress;

		internal IDirectVariable startInAddress
		{
			get
			{
				return _startInAddress;
			}
			set
			{
				_startInAddress = value;
			}
		}

		internal IDirectVariable startOutAddress
		{
			get
			{
				return _startOutAddress;
			}
			set
			{
				_startOutAddress = value;
			}
		}
	}
}
