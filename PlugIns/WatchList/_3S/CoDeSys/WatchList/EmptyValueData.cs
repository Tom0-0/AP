namespace _3S.CoDeSys.WatchList
{
	internal class EmptyValueData : ValueData
	{
		public override object Value => null;

		public override string Text => string.Empty;

		public override bool Toggleable => false;

		public override bool Constant => false;

		public override bool Forced => false;

		public override bool IsEnum => false;

		public EmptyValueData(WatchListNode node)
			: base(node)
		{
		}
	}
}
