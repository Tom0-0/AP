using System;

namespace _3S.CoDeSys.WatchList
{
	public class InvalidExpressionSyntaxException : Exception
	{
		public InvalidExpressionSyntaxException(string stMessage)
			: base(stMessage)
		{
		}
	}
}
