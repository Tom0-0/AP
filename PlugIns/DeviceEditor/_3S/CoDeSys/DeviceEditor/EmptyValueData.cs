namespace _3S.CoDeSys.DeviceEditor
{
	internal class EmptyValueData : ValueData
	{
		public static readonly EmptyValueData Empty = new EmptyValueData();

		public override object Value => null;

		public override string Text => string.Empty;

		public override bool Toggleable => false;

		public override bool Constant => false;

		public override bool Forced => false;
	}
}
