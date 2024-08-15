namespace _3S.CoDeSys.DeviceObject
{
	public class ParameterSetTransactionHandler
	{
		private ParameterSet _paramset;

		private bool _bUpdateAddressesNeeded;

		private int _UpdateCounter;

		public bool IsInUpdate => _UpdateCounter > 0;

		public ParameterSetTransactionHandler(ParameterSet paramset)
		{
			_paramset = paramset;
		}

		public void BeginUpdate()
		{
			_UpdateCounter++;
		}

		public void EndUpdate()
		{
			if (_UpdateCounter > 0)
			{
				_UpdateCounter--;
				if (_UpdateCounter == 0)
				{
					OnEndUpdate();
				}
			}
		}

		public void UpdateAddresses()
		{
			if (IsInUpdate)
			{
				_bUpdateAddressesNeeded = true;
				return;
			}
			_paramset.UpdateAddresses();
			_bUpdateAddressesNeeded = false;
		}

		private void OnEndUpdate()
		{
			if (_bUpdateAddressesNeeded)
			{
				UpdateAddresses();
			}
		}
	}
}
