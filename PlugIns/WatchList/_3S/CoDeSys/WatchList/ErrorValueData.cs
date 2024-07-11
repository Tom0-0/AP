using System;

namespace _3S.CoDeSys.WatchList
{
	internal class ErrorValueData : ValueData
	{
		private string _stMessage;

		public override object Value => null;

		public override string Text => "<" + _stMessage + ">";

		public override bool Toggleable => false;

		public override bool Constant => false;

		public override bool Forced => false;

		public override bool IsEnum => false;

		public ErrorValueData(WatchListNode node, string stMessage)
			: base(node)
		{
			if (stMessage == null)
			{
				throw new ArgumentNullException("stMessage");
			}
			_stMessage = stMessage;
		}
	}
}
