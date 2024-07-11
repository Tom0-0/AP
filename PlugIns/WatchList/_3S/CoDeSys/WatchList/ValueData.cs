using System;
using _3S.CoDeSys.Core.LanguageModel;

namespace _3S.CoDeSys.WatchList
{
	internal abstract class ValueData
	{
		private WatchListNode _node;

		public WatchListNode Node => _node;

		public abstract object Value { get; }

		public abstract string Text { get; }

		public abstract bool Toggleable { get; }

		public abstract bool Constant { get; }

		public abstract bool Forced { get; }

		public abstract bool IsEnum { get; }

		internal virtual TypeClass TypeClass => (TypeClass)29;

		internal ValueData(WatchListNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			_node = node;
		}
	}
}
