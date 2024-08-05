namespace _3S.CoDeSys.DeviceEditor
{
	internal class EnumValueData : ValueData
	{
		private string _stValue;

		private int _nIndex;

		public override bool Constant => false;

		public override bool Forced => false;

		public override string Text => _stValue;

		public override bool Toggleable => false;

		public override object Value => _nIndex;

		public int Index => _nIndex;

		public EnumValueData(string stValue, int nIndex)
		{
			_stValue = stValue;
			_nIndex = nIndex;
		}
	}
}
